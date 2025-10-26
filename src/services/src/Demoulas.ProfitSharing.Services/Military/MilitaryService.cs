using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Contracts.Response.Military;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Demoulas.ProfitSharing.Services.Military;

public class MilitaryService : IMilitaryService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly ILogger<MilitaryService> _logger;
    private readonly ICalendarService _calendarService;
    
    public MilitaryService(
        IProfitSharingDataContextFactory dataContextFactory,
        IDemographicReaderService demographicReaderService,
        ILogger<MilitaryService> logger,
        ICalendarService calendarService)
    {
        _dataContextFactory = dataContextFactory;
        _demographicReaderService = demographicReaderService;
        _logger = logger;
        _calendarService = calendarService;

    }

    public Task<Result<MilitaryContributionResponse>> CreateMilitaryServiceRecordAsync(
        CreateMilitaryContributionRequest req, CancellationToken cancellationToken = default)
    {
        return _dataContextFactory.UseWritableContext(async c =>
        {
            try
            {
#pragma warning disable DSMPS001
                var d = await c.Demographics.FirstOrDefaultAsync(d => d.BadgeNumber == req.BadgeNumber,
#pragma warning restore DSMPS001
                    cancellationToken);

                if (d == null)
                {
                    return Result<MilitaryContributionResponse>.Failure(Error.EmployeeNotFound);
                }

                var pd = new ProfitDetail
                {
                    ProfitCodeId = /* 0 */ProfitCode.Constants.IncomingContributions,
                    ProfitYear = req.ProfitYear,
                    ProfitYearIteration = 1,
                    CommentTypeId = /* 19 */CommentType.Constants.Military.Id,
                    Remark = /* 19 */CommentType.Constants.Military.Name,
                    Contribution = req.ContributionAmount,
                    Ssn = d.Ssn,
                    YearsOfServiceCredit = (sbyte)(req.IsSupplementalContribution ? 0 : 1),
                    MonthToDate = 12,
                    YearToDate = (short)req.ContributionDate.Year
                };
                c.ProfitDetails.Add(pd);

                await c.SaveChangesAsync(cancellationToken);

                return Result<MilitaryContributionResponse>.Success(new MilitaryContributionResponse
                {
                    BadgeNumber = req.BadgeNumber,
                    CommentTypeId = /* 19 */CommentType.Constants.Military,
                    ProfitYear = req.ProfitYear,
                    ContributionDate = new DateOnly(pd.YearToDate, pd.MonthToDate, 31),
                    Amount = pd.Contribution,
                    IsSupplementalContribution = pd.YearsOfServiceCredit == 0
                });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error creating military contribution for Badge: {BadgeNumber}, ProfitYear: {ProfitYear}",
                    req.BadgeNumber, req.ProfitYear);

                // Check if it's a duplicate key/constraint violation
                if (ex.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true ||
                    ex.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true)
                {
                    return Result<MilitaryContributionResponse>.Failure(Error.MilitaryContributionDuplicate);
                }

                return Result<MilitaryContributionResponse>.Failure(Error.Unexpected("Failed to save military contribution record"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating military contribution for Badge: {BadgeNumber}, ProfitYear: {ProfitYear}",
                    req.BadgeNumber, req.ProfitYear);
                return Result<MilitaryContributionResponse>.Failure(Error.Unexpected(ex.Message));
            }
        }, cancellationToken);
    }

    public async Task<Result<PaginatedResponseDto<MilitaryContributionResponse>>> GetMilitaryServiceRecordAsync(GetMilitaryContributionRequest req, bool isArchiveRequest,
        CancellationToken cancellationToken = default)
    {
        try
        {
            #region Validation

            var validator = new InlineValidator<GetMilitaryContributionRequest>();

            if (!isArchiveRequest)
            {
                validator.RuleFor(r => r.BadgeNumber)
                    .GreaterThan(0)
                    .WithMessage($"{nameof(GetMilitaryContributionRequest.BadgeNumber)} must be greater than zero.");
            }

            var validationResult = await validator.ValidateAsync(req, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                return Result<PaginatedResponseDto<MilitaryContributionResponse>>.ValidationFailure(errors);
            }

            #endregion

            var result = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
            ctx.UseReadOnlyContext(cancellationToken);
            var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
            var query = ctx.ProfitDetails
                .Include(pd => pd.CommentType)
                .Join(demographics,
                    c => c.Ssn,
                    cm => cm.Ssn,
                    (pd, d) => new { pd, d })
                .Where(x => x.pd.CommentTypeId == CommentType.Constants.Military.Id)
                .OrderByDescending(x => x.pd.ProfitYear)
                .ThenByDescending(x => x.pd.CreatedAtUtc)
                .ThenByDescending(x => x.pd.Id)
                .Select(x => new MilitaryContributionResponse
                {
                    BadgeNumber = x.d.BadgeNumber,
                    ProfitYear = x.pd.ProfitYear,
                    CommentTypeId = x.pd.CommentTypeId,
                    ContributionDate = new DateOnly(x.pd.YearToDate == 0 ? x.pd.ProfitYear : x.pd.YearToDate, x.pd.MonthToDate == 0 ? 12 : x.pd.MonthToDate, 31),
                    Amount = x.pd.Contribution,
                    IsSupplementalContribution = x.pd.YearsOfServiceCredit == 0,
                    IsExecutive = x.d.PayFrequencyId == Demoulas.ProfitSharing.Data.Entities.PayFrequency.Constants.Monthly
                });

                if (!isArchiveRequest)
                {
                    query = query.Where(x => x.BadgeNumber == req.BadgeNumber);
                }

                var results = await query.ToPaginationResultsAsync(req, cancellationToken);

                // get the correct fiscal end date for the contribution year
                await GetCalendarProfitYearDateForResponses(results, cancellationToken);
                return results;
            });

            return Result<PaginatedResponseDto<MilitaryContributionResponse>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving military contribution records for Badge: {BadgeNumber}, IsArchiveRequest: {IsArchiveRequest}",
                req.BadgeNumber, isArchiveRequest);
            return Result<PaginatedResponseDto<MilitaryContributionResponse>>.Failure(Error.Unexpected("Failed to retrieve military contribution records"));
        }
    }
    /// <summary>
    /// method to pull out the correct fiscal end date for the contribution year
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task GetCalendarProfitYearDateForResponses(PaginatedResponseDto<MilitaryContributionResponse> militaryContributions, CancellationToken cancellationToken)
    {
        Dictionary<int, DateOnly> profitYearsMap = new Dictionary<int, DateOnly>();

        foreach ( var contributions in militaryContributions.Results)
        {
            if (profitYearsMap.ContainsKey(contributions.ContributionDate.Year))
            {
                contributions.ContributionDate = profitYearsMap.GetValueOrDefault(contributions.ContributionDate.Year);
            } 
            else
            {
                var dates = await _calendarService.GetYearStartAndEndAccountingDatesAsync((short)contributions.ContributionDate.Year, cancellationToken);
                profitYearsMap.Add(contributions.ContributionDate.Year, contributions.ContributionDate);
                contributions.ContributionDate = dates.FiscalEndDate;
            }
        }
    }
}
