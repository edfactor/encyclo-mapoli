using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.CheckRun;
using Demoulas.ProfitSharing.Common.Contracts.Response.CheckRun;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces.CheckRun;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Services.Reports.PrintFormatting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Services.CheckRun;

public sealed class CheckRunPrintFileService : ICheckRunPrintFileService
{
    private const string ProfitShareCheckNumberSequenceName = "PROFIT_SHARE_CHECK_NUMBER_SEQ";
    private const int MaxPayableNameLength = 84;

    private readonly IProfitSharingDataContextFactory _factory;
    private readonly ICheckRunWorkflowService _workflowService;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly DjdeFormatBuilder _djdeFormatBuilder;
    private readonly IMicrFormatterFactory _micrFormatterFactory;
    private readonly ILogger<CheckRunPrintFileService> _logger;

    public CheckRunPrintFileService(
        IProfitSharingDataContextFactory factory,
        ICheckRunWorkflowService workflowService,
        IDemographicReaderService demographicReaderService,
        DjdeFormatBuilder djdeFormatBuilder,
        IMicrFormatterFactory micrFormatterFactory,
        ILogger<CheckRunPrintFileService> logger)
    {
        _factory = factory;
        _workflowService = workflowService;
        _demographicReaderService = demographicReaderService;
        _djdeFormatBuilder = djdeFormatBuilder;
        _micrFormatterFactory = micrFormatterFactory;
        _logger = logger;
    }

    public async Task<Result<CheckRunPrintFileResult>> GenerateAsync(CheckRunStartRequest request, CancellationToken cancellationToken)
    {
        var distributionIds = request.DistributionIds
            .Where(x => x > 0)
            .Distinct()
            .ToArray();

        if (distributionIds.Length == 0)
        {
            return Result<CheckRunPrintFileResult>.ValidationFailure(new Dictionary<string, string[]>
            {
                [nameof(CheckRunStartRequest.DistributionIds)] = ["DistributionIds must contain at least one id."]
            });
        }

        Guid runId;
        int startingCheckNumber;

        var missingDistributionIds = await _factory.UseReadOnlyContext(async context =>
        {
            var foundIds = await context.Distributions
                .TagWith($"CheckRun-ValidateDistributionsExist-{request.ProfitYear}")
                .Where(d => distributionIds.Contains(d.Id))
                .Select(d => d.Id)
                .ToListAsync(cancellationToken);

            var found = foundIds.ToHashSet();
            return distributionIds.Where(id => !found.Contains(id)).ToArray();
        }, cancellationToken);

        if (missingDistributionIds.Length > 0)
        {
            return Result<CheckRunPrintFileResult>.ValidationFailure(new Dictionary<string, string[]>
            {
                [nameof(CheckRunStartRequest.DistributionIds)] = [
                    $"One or more distributions were not found: {string.Join(", ", missingDistributionIds)}"
                ]
            });
        }

        var checkNumbers = await GetNextCheckNumbersAsync(distributionIds.Length, cancellationToken);
        if (checkNumbers.Count != distributionIds.Length)
        {
            return Result<CheckRunPrintFileResult>.Failure(Error.Unexpected("Failed to allocate check numbers."));
        }

        startingCheckNumber = checkNumbers[0];

        if (request.IsReprint)
        {
            var currentRunResult = await _workflowService.GetCurrentRunAsync(request.ProfitYear, cancellationToken);
            if (!currentRunResult.IsSuccess)
            {
                return Result<CheckRunPrintFileResult>.Failure(currentRunResult.Error!);
            }

            runId = currentRunResult.Value!.Id;

            var canReprintResult = await _workflowService.CanReprintAsync(runId, cancellationToken);
            if (!canReprintResult.IsSuccess)
            {
                return Result<CheckRunPrintFileResult>.Failure(canReprintResult.Error!);
            }

            if (!canReprintResult.Value)
            {
                return Result<CheckRunPrintFileResult>.ValidationFailure(new Dictionary<string, string[]>
                {
                    [nameof(CheckRunStartRequest.IsReprint)] = ["This check run cannot be reprinted (limit reached or not same-day)."]
                });
            }

            var incrementResult = await _workflowService.IncrementReprintCountAsync(runId, request.UserName, cancellationToken);
            if (!incrementResult.IsSuccess)
            {
                return Result<CheckRunPrintFileResult>.Failure(incrementResult.Error!);
            }
        }
        else
        {
            var workflowResult = await _workflowService.StartNewRunAsync(
                request.ProfitYear,
                request.CheckRunDate,
                startingCheckNumber,
                request.UserName,
                cancellationToken);

            if (!workflowResult.IsSuccess)
            {
                return Result<CheckRunPrintFileResult>.Failure(workflowResult.Error!);
            }

            runId = workflowResult.Value!.Id;
        }

        return await _factory.UseWritableContext(async context =>
        {
            if (request.IsReprint)
            {
                await context.ProfitShareChecks
                    .Where(c => c.CheckRunWorkflowId == runId && (c.IsVoided == null || c.IsVoided == false))
                    .ExecuteUpdateAsync(
                        updates => updates
                            .SetProperty(c => c.IsVoided, true)
                            .SetProperty(c => c.VoidDate, request.CheckRunDate),
                        cancellationToken);

                var workflow = await context.CheckRunWorkflows.FirstOrDefaultAsync(w => w.Id == runId, cancellationToken);
                if (workflow is not null)
                {
                    workflow.CheckNumber = startingCheckNumber;
                    workflow.ModifiedByUserName = request.UserName;
                    workflow.ModifiedDate = DateTimeOffset.UtcNow;
                }
            }

            var distributions = await context.Distributions
                .Include(d => d.Payee)
                .TagWith($"CheckRun-LoadDistributions-{request.ProfitYear}")
                .Where(d => distributionIds.Contains(d.Id))
                .ToListAsync(cancellationToken);

            if (distributions.Count != distributionIds.Length)
            {
                var found = distributions.Select(d => d.Id).ToHashSet();
                var missing = distributionIds.Where(id => !found.Contains(id)).ToArray();

                return Result<CheckRunPrintFileResult>.ValidationFailure(new Dictionary<string, string[]>
                {
                    [nameof(CheckRunStartRequest.DistributionIds)] = [
                        $"One or more distributions were not found: {string.Join(", ", missing)}"
                    ]
                });
            }

            var ssns = distributions.Select(d => d.Ssn).Distinct().ToArray();

            var demographicQuery = await _demographicReaderService.BuildDemographicQueryAsync(context);

            var demographics = await demographicQuery
                .TagWith($"CheckRun-LoadDemographics-{request.ProfitYear}")
                .Where(d => ssns.Contains(d.Ssn))
                .Select(d => new
                {
                    d.Ssn,
                    d.Id,
                    d.BadgeNumber,
                    d.OracleHcmId,
                    d.CreatedAtUtc,
                    d.ModifiedAtUtc
                })
                .ToListAsync(cancellationToken);

            var demographicsBySsn = demographics.ToLookup(d => d.Ssn);

            var missingDemographics = ssns
                .Where(ssn => !demographicsBySsn[ssn].Any())
                .Select(ssn => ssn.MaskSsn())
                .ToArray();

            if (missingDemographics.Length > 0)
            {
                return Result<CheckRunPrintFileResult>.ValidationFailure(new Dictionary<string, string[]>
                {
                    ["Demographics"] = [
                        $"No demographics were found for one or more SSNs: {string.Join(", ", missingDemographics)}"
                    ]
                });
            }

            var demographicSingleBySsn = ssns.ToDictionary(
                ssn => ssn,
                ssn =>
                {
                    var matches = demographicsBySsn[ssn];
                    var selected = matches
                        .OrderByDescending(d => d.OracleHcmId)
                        .ThenByDescending(d => d.ModifiedAtUtc ?? d.CreatedAtUtc)
                        .First();

                    if (matches.Skip(1).Any())
                    {
                        _logger.LogWarning(
                            "Multiple demographics found for SSN; selecting deterministically. Ssn={MaskedSsn} SelectedOracleHcmId={OracleHcmId}",
                            ssn.MaskSsn(),
                            selected.OracleHcmId);
                    }

                    return selected;
                });

            var orderedDistributions = distributions.OrderBy(d => d.Id).ToList();

            var checkDatas = new List<CheckData>(orderedDistributions.Count);
            var checks = new List<Data.Entities.ProfitShareCheck>(orderedDistributions.Count);

            for (var index = 0; index < orderedDistributions.Count; index++)
            {
                var distribution = orderedDistributions[index];
                var demographic = demographicSingleBySsn[distribution.Ssn];

                var checkNumber = checkNumbers[index];

                var payableName = distribution.Payee?.Name;
                if (string.IsNullOrWhiteSpace(payableName))
                {
                    payableName = distribution.EmployeeName;
                }

                payableName = (payableName ?? string.Empty).Trim();
                if (payableName.Length == 0)
                {
                    _logger.LogWarning(
                        "PayableName was blank; defaulting to placeholder. DistributionId={DistributionId}",
                        distribution.Id);

                    payableName = "PAYEE";
                }

                if (payableName.Length > MaxPayableNameLength)
                {
                    _logger.LogWarning(
                        "PayableName exceeds max length; truncating. DistributionId={DistributionId} Length={Length}",
                        distribution.Id,
                        payableName.Length);

                    payableName = payableName[..MaxPayableNameLength];
                }

                var checkAmount = Math.Round(
                    distribution.GrossAmount - distribution.FederalTaxAmount - distribution.StateTaxAmount,
                    2,
                    MidpointRounding.AwayFromZero);

                var check = new Data.Entities.ProfitShareCheck
                {
                    CheckNumber = checkNumber,
                    CheckRunWorkflowId = runId,
                    Ssn = distribution.Ssn,
                    DemographicId = demographic.Id,
                    PayableName = payableName,
                    CheckAmount = checkAmount,
                    TaxCodeId = distribution.TaxCodeId,
                    CheckDate = request.CheckRunDate,
                    CheckRunDate = request.CheckRunDate,
                    IsVoided = false,
                    PscCheckId = checkNumber
                };

                checks.Add(check);

                checkDatas.Add(new CheckData
                {
                    CheckNumber = checkNumber,
                    Amount = checkAmount,
                    RecipientName = payableName,
                    Ssn = distribution.Ssn.ToString("D9"),
                    BadgeNumber = demographic.BadgeNumber,
                    IssueDate = request.CheckRunDate
                });
            }

            context.ProfitShareChecks.AddRange(checks);
            await context.SaveChangesAsync(cancellationToken);

            string content = request.PrinterType switch
            {
                CheckRunPrinterType.XeroxDjde => GenerateXeroxDjde(checkDatas),
                CheckRunPrinterType.Standard => GenerateStandard(checkDatas),
                _ => throw new NotSupportedException($"PrinterType '{request.PrinterType}' is not supported.")
            };

            var fileName = request.PrinterType switch
            {
                CheckRunPrinterType.XeroxDjde => "PROFCHKS_DJDE.txt",
                CheckRunPrinterType.Standard => "PROFCHKS.txt",
                _ => "PROFCHKS.txt"
            };

            return Result<CheckRunPrintFileResult>.Success(new CheckRunPrintFileResult
            {
                RunId = runId,
                FileName = fileName,
                ContentType = "text/plain",
                Content = content,
                CheckCount = checkDatas.Count
            });
        }, cancellationToken);
    }

    private Task<List<int>> GetNextCheckNumbersAsync(int count, CancellationToken cancellationToken)
    {
        return _factory.UseReadOnlyContext(async context =>
        {
            var sql = $"SELECT {ProfitShareCheckNumberSequenceName}.NEXTVAL FROM DUAL CONNECT BY LEVEL <= {{0}}";
            return await context.Database
                .SqlQueryRaw<int>(sql, count)
                .ToListAsync(cancellationToken);
        }, cancellationToken);
    }

    private string GenerateXeroxDjde(IEnumerable<CheckData> checkDatas)
    {
        var template = _djdeFormatBuilder.CreateProfitShareCheckTemplate();
        return string.Join(Environment.NewLine + Environment.NewLine, checkDatas.Select(template.GenerateDirectives));
    }

    private string GenerateStandard(IEnumerable<CheckData> checkDatas)
    {
        var micrFormatter = _micrFormatterFactory.GetFormatter("026004297");

        return string.Join(Environment.NewLine + Environment.NewLine, checkDatas.Select(checkData =>
        {
            var micrLine = micrFormatter.FormatMicrLine(checkData.CheckNumber, checkData.Amount);

            var lines = new List<string>
            {
                $"Check #{checkData.CheckNumber}",
                $"Date: {checkData.IssueDate:MM/dd/yyyy}",
                $"Pay to: {checkData.RecipientName}",
                $"Amount: ${checkData.Amount:N2}",
                $"Badge: {checkData.BadgeNumber}",
                $"SSN: {checkData.Ssn.MaskSsn()}",
                string.Empty,
                micrLine
            };

            return string.Join(Environment.NewLine, lines);
        }));
    }
}
