using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Extensions;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.Util.Extensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.MasterInquiry;

/// <summary>
/// Master Inquiry Service - Orchestrates employee and beneficiary lookup operations.
/// Phase 1: Employee operations delegated to EmployeeMasterInquiryService.
/// Phase 2: Beneficiary operations will be delegated to BeneficiaryMasterInquiryService.
/// DTOs (MasterInquiryItem, InquiryDemographics, MasterInquiryRawDto) are in MasterInquiryDtos.cs.
/// </summary>
public sealed class MasterInquiryService : IMasterInquiryService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ILogger _logger;
    private readonly ITotalService _totalService;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly IEmployeeMasterInquiryService _employeeInquiryService;
    private readonly IBeneficiaryMasterInquiryService _beneficiaryInquiryService;

    public MasterInquiryService(
        IProfitSharingDataContextFactory dataContextFactory,
        ITotalService totalService,
        ILoggerFactory loggerFactory,
        IDemographicReaderService demographicReaderService,
        IEmployeeMasterInquiryService employeeInquiryService,
        IBeneficiaryMasterInquiryService beneficiaryInquiryService)
    {
        _dataContextFactory = dataContextFactory;
        _totalService = totalService;
        _demographicReaderService = demographicReaderService;
        _employeeInquiryService = employeeInquiryService;
        _beneficiaryInquiryService = beneficiaryInquiryService;
        _logger = loggerFactory.CreateLogger<MasterInquiryService>();
    }


    public async Task<PaginatedResponseDto<MemberDetails>> GetMembersAsync(MasterInquiryRequest req, CancellationToken cancellationToken = default)
    {
        // Create a 30-second timeout token combined with the caller's token
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(30));
        var timeoutToken = timeoutCts.Token;

        try
        {
            return await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                // CRITICAL FIX: Keep SSN query composable to avoid Oracle's ~10K limit on IN clauses
                // and prevent loading 150K+ SSNs into memory. Use query composition for efficient JOINs.
                IQueryable<int> ssnQuery;
                bool useOptimizedPath = ShouldUseOptimizedSsnQuery(req);

                if (useOptimizedPath)
                {
                    // Fast path: Get SSN query directly from filtered ProfitDetails (not materialized)
                    ssnQuery = GetSsnQueryFromProfitDetails(ctx, req);
                }
                else
                {
                    // Original path: Build full query for complex filters
                    // FIXED: Pass context to avoid nested context creation deadlock
                    _logger.LogInformation("TRACE: Building query for MemberType {MemberType}", req.MemberType);
                    IQueryable<MasterInquiryItem> query = req.MemberType switch
                    {
                        1 => await GetMasterInquiryDemographics(ctx, req, timeoutToken),
                        2 => await GetMasterInquiryBeneficiary(ctx, req, timeoutToken),
                        _ => (await GetMasterInquiryDemographics(ctx, req, timeoutToken)).Union(await GetMasterInquiryBeneficiary(ctx, req, timeoutToken))
                    };
                    _logger.LogInformation("TRACE: Query built for MemberType {MemberType}", req.MemberType);

                    query = FilterMemberQuery(req, query).TagWith("MasterInquiry: Filtered member query");
                    _logger.LogInformation("TRACE: Query filtered");

                    // Get unique SSNs from the query with timeout (keep as IQueryable)
                    // EF Core 9: This will use optimized SQL for DISTINCT
                    ssnQuery = query
                        .Select(x => x.Member.Ssn)
                        .Distinct()
                        .TagWith("MasterInquiry: Extract unique SSNs");
                    _logger.LogInformation("TRACE: SSN query composed");
                }

                // Safety check: Log warning if SSN set might be large (for monitoring)
                _logger.LogInformation("TRACE: About to execute CountAsync");
                var estimatedCount = await ssnQuery.CountAsync(timeoutToken).ConfigureAwait(false);
                _logger.LogInformation("TRACE: CountAsync completed, count={Count}", estimatedCount);

                if (estimatedCount > 50000)
                {
                    _logger.LogWarning(
                        "Master inquiry query returned {SsnCount} SSNs for profit year {ProfitYear}. Consider adding more selective filters.",
                        estimatedCount, req.ProfitYear);
                }

                _logger.LogInformation("TRACE: About to call BuildDemographicQuery for duplicates");
                var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);

                // Use query composition instead of .Contains() to avoid Oracle limits
                // EF Core 9 will generate efficient JOIN or EXISTS instead of IN clause
                _logger.LogInformation("TRACE: About to execute duplicate SSN query");
                var duplicateSsns = await demographics
                    .Where(d => ssnQuery.Contains(d.Ssn))
                    .GroupBy(d => d.Ssn)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .TagWith("MasterInquiry: Find duplicate SSNs via JOIN")
                    .ToHashSetAsync(timeoutToken).ConfigureAwait(false);
                _logger.LogInformation("TRACE: Duplicate SSN query completed, count={Count}", duplicateSsns.Count);

                // Materialize SSN list only when needed for exact match handling
                var ssnList = new HashSet<int>();
                if (estimatedCount == 0 && (req.Ssn != 0 || req.BadgeNumber != 0))
                {
                    _logger.LogInformation("TRACE: Calling HandleExactBadgeOrSsn for Ssn={Ssn}, Badge={Badge}", req.Ssn, req.BadgeNumber);
                    // If an exact match is found, then the bene or empl is added to the ssnList.
                    await HandleExactBadgeOrSsn(ctx, ssnList, req.BadgeNumber, req.PsnSuffix, req.Ssn, timeoutToken);
                    _logger.LogInformation("TRACE: HandleExactBadgeOrSsn completed, ssnList.Count={Count}", ssnList.Count);

                    // Early return if still no results found
                    if (ssnList.Count == 0)
                    {
                        _logger.LogInformation("TRACE: Early return - no results found");
                        return new PaginatedResponseDto<MemberDetails>(req) { Results = [], Total = 0 };
                    }
                }
                else if (estimatedCount == 0)
                {
                    // Early return if no results found and no exact match to try
                    return new PaginatedResponseDto<MemberDetails>(req) { Results = [], Total = 0 };
                }
                else
                {
                    // Materialize for downstream processing only when we have results
                    ssnList = await ssnQuery.ToHashSetAsync(timeoutToken);
                }
                short currentYear = req.ProfitYear;
                short previousYear = (short)(currentYear - 1);
                var memberType = req.MemberType;
                PaginatedResponseDto<MemberDetails> detailsList;

                if (memberType == 1)
                {
                    detailsList = await GetDemographicDetailsForSsns(req, ssnList, currentYear, previousYear, duplicateSsns, timeoutToken);
                }
                else if (memberType == 2)
                {
                    detailsList = await GetBeneficiaryDetailsForSsns(req, ssnList, timeoutToken);
                }
                else
                {
                    // For both, merge and deduplicate by SSN
                    var employeeDetails = await GetAllDemographicDetailsForSsns(ssnList, currentYear, previousYear, duplicateSsns, timeoutToken);
                    var beneficiaryDetails = await GetAllBeneficiaryDetailsForSsns(ssnList, timeoutToken);

                    // Combine and deduplicate by SSN
                    var allResults = employeeDetails.Concat(beneficiaryDetails)
                            .GroupBy(d => d.Ssn)
                            .Select(g => g.First())
                            .ToList();

                    // Apply sorting based on request
                    var sortedResults = ApplySorting(allResults.AsQueryable(), req).ToList();

                    // Apply pagination to the final deduplicated result set
                    var skip = req.Skip ?? 0;
                    var take = req.Take ?? 25;
                    var paginatedResults = sortedResults.Skip(skip).Take(take).ToList();

                    detailsList = new PaginatedResponseDto<MemberDetails>(req) { Results = paginatedResults, Total = sortedResults.Count };
                }

                // Calculate age efficiently in-memory
                foreach (MemberDetails details in detailsList.Results)
                {
                    details.Age = details.DateOfBirth.Age();
                }

                return detailsList;
            }, timeoutToken);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            // Timeout occurred (not user cancellation)
            _logger.LogWarning("Master inquiry search timed out after 30 seconds for profit year {ProfitYear}", req.ProfitYear);

            // Return empty result with message indicating timeout
            // Note: Add IsPartialResult and Message properties to PaginatedResponseDto if not present
            return new PaginatedResponseDto<MemberDetails>(req)
            {
                Results = [],
                Total = 0
                // TODO: Add IsPartialResult = true and Message properties to PaginatedResponseDto
                // IsPartialResult = true,
                // Message = "Query timed out after 30 seconds. Please narrow your search criteria."
            };
        }
    }

    /// <summary>
    /// Determines if we should use the optimized SSN query path.
    /// Use optimization when we have highly selective filters that can dramatically reduce the dataset.
    /// </summary>
    private static bool ShouldUseOptimizedSsnQuery(MasterInquiryRequest req)
    {
        return MasterInquiryHelpers.ShouldUseOptimizedSsnQuery(req);
    }

    /// <summary>
    /// Optimized query to get SSN query (not materialized) directly from ProfitDetails with selective filters.
    /// This avoids expensive joins when we have highly selective criteria.
    /// CRITICAL: Returns IQueryable to enable query composition and avoid Oracle's ~10K IN clause limit.
    /// </summary>
    private static IQueryable<int> GetSsnQueryFromProfitDetails(
        ProfitSharingReadOnlyDbContext ctx,
        MasterInquiryRequest req)
    {
        var query = ctx.ProfitDetails
            .TagWith("MasterInquiry: Optimized SSN extraction from ProfitDetails");

        // Apply PaymentType filter (highly selective)
        if (req.PaymentType.HasValue)
        {
            var commentTypeIds = req.PaymentType switch
            {
                1 => new byte?[] { CommentType.Constants.Hardship.Id, CommentType.Constants.Distribution.Id },
                2 => new byte?[] { CommentType.Constants.Payoff.Id, CommentType.Constants.Forfeit.Id },
                3 => new byte?[] { CommentType.Constants.Rollover.Id, CommentType.Constants.RothIra.Id },
                _ => Array.Empty<byte?>()
            };

            if (commentTypeIds.Length > 0)
            {
                query = query.Where(pd => commentTypeIds.Contains(pd.CommentTypeId));
            }
        }

        // Apply year filter
        if (req.EndProfitYear.HasValue)
        {
            query = query.Where(pd => pd.ProfitYear <= req.EndProfitYear.Value);
        }

        // Apply ProfitCode filter
        if (req.ProfitCode.HasValue)
        {
            query = query.Where(pd => pd.ProfitCodeId == req.ProfitCode.Value);
        }

        // Apply SSN filter if provided (most selective)
        if (req.Ssn != 0)
        {
            query = query.Where(pd => pd.Ssn == req.Ssn);
        }

        // Return composable query - DO NOT materialize here
        return query
            .Select(pd => pd.Ssn)
            .Distinct()
            .TagWith("MasterInquiry: Get distinct SSNs (optimized path - composable)");
    }

    /* This handles the case where we are given an exact badge or ssn and there are no PROFIT_DETAIL rows */
    private async Task HandleExactBadgeOrSsn(ProfitSharingReadOnlyDbContext ctx, HashSet<int> ssnList, int? badgeNumber, short? psnSuffix, int ssn, CancellationToken cancellationToken = default)
    {

        // Some Members do not have Transactions yet (aka new employees, or new Bene) - so if we are asked about a specific psn/badge, we handle that here.
        if (ssnList.Count == 0 && (ssn != 0 || badgeNumber != 0))
        {
            if (ssn != 0)
            {
                // If they gave us the ssn... then lets use that.
                ssnList.Add(ssn);
            }
            // If they gave us the badge... then lets use that.
            else if (psnSuffix > 0)
            {
                int ssnBene = await _beneficiaryInquiryService.FindBeneficiarySsnByBadgeAsync(badgeNumber!.Value, psnSuffix.Value, cancellationToken);

                if (ssnBene != 0)
                {
                    ssnList.Add(ssnBene);
                }
            }
            else if (badgeNumber != 0)
            {
                int ssnEmpl = await _employeeInquiryService.FindEmployeeSsnByBadgeAsync(badgeNumber.Value, cancellationToken);

                if (ssnEmpl != 0)
                {
                    ssnList.Add(ssnEmpl);
                }
            }
        }
    }

    public async Task<PaginatedResponseDto<GroupedProfitSummaryDto>> GetGroupedProfitDetails(MasterInquiryRequest req, CancellationToken cancellationToken = default)
    {
        // These are the ProfitCode IDs used in GetProfitCodesForBalanceCalc()
        byte[] balanceProfitCodes = new byte[]
        {
            ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id, ProfitCode.Constants.OutgoingForfeitures.Id, ProfitCode.Constants.OutgoingDirectPayments.Id,
            ProfitCode.Constants.OutgoingXferBeneficiary.Id, ProfitCode.Constants.Outgoing100PercentVestedPayment.Id
        };

        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            // Use context-based overloads to avoid nested context disposal
            IQueryable<MasterInquiryItem> query = req.MemberType switch
            {
                1 => await _employeeInquiryService.GetEmployeeInquiryQueryAsync(ctx, null, cancellationToken),
                2 => await _beneficiaryInquiryService.GetBeneficiaryInquiryQueryAsync(ctx, null, cancellationToken),
                _ => (await _employeeInquiryService.GetEmployeeInquiryQueryAsync(ctx, null, cancellationToken))
                    .Union(await _beneficiaryInquiryService.GetBeneficiaryInquiryQueryAsync(ctx, null, cancellationToken))
            };

            query = FilterMemberQuery(req, query);

            // Only group by non-null ProfitDetail
            query = query.Where(x => x.ProfitDetail != null);

            return await query
                .GroupBy(x => new
                {
                    ProfitYear = x.ProfitDetail != null ? x.ProfitDetail.ProfitYear : (short)0,
                    MonthToDate = x.ProfitDetail != null ? x.ProfitDetail.MonthToDate : (byte)0
                })
                .Select(g => new GroupedProfitSummaryDto
                {
                    ProfitYear = g.Key.ProfitYear,
                    MonthToDate = g.Key.MonthToDate,
                    TotalContribution = g.Sum(x => x.ProfitDetail != null ? x.ProfitDetail.Contribution : 0),
                    TotalEarnings = g.Sum(x => x.ProfitDetail != null ? x.ProfitDetail.Earnings : 0),
                    TotalForfeiture = g.Sum(x =>
                        x.ProfitDetail != null && !balanceProfitCodes.Contains(x.ProfitDetail.ProfitCodeId) ? x.ProfitDetail.Forfeiture : 0),
                    TotalPayment = g.Sum(x =>
                        x.ProfitDetail != null && balanceProfitCodes.Contains(x.ProfitDetail.ProfitCodeId) ? x.ProfitDetail.Forfeiture : 0),
                    TransactionCount = g.Count()
                })
                .OrderBy(x => x.ProfitYear)
                .ThenBy(x => x.MonthToDate)
                .ToPaginationResultsAsync(req, cancellationToken);
        }, cancellationToken);
    }


    public async Task<MemberProfitPlanDetails?> GetMemberVestingAsync(MasterInquiryMemberRequest req, CancellationToken cancellationToken = default)
    {
        short currentYear = req.ProfitYear;
        short previousYear = (short)(currentYear - 1);
        var members = req.MemberType switch
        {
            1 => await GetDemographicDetails(req.Id, currentYear, previousYear, cancellationToken),
            2 => await GetBeneficiaryDetails(req.Id, cancellationToken),
            _ => throw new ValidationException("Invalid MemberType provided")
        };
        Dictionary<int, MemberDetails> memberDetailsMap = new Dictionary<int, MemberDetails> { { members.ssn, members.memberDetails ?? new MemberDetails { Id = 0 } } };

        var details = await GetVestingDetails(memberDetailsMap, currentYear, previousYear, cancellationToken);
        return details.FirstOrDefault();
    }

    public Task<PaginatedResponseDto<MasterInquiryResponseDto>> GetMemberProfitDetails(MasterInquiryMemberDetailsRequest req, CancellationToken cancellationToken = default)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            // Use context-based overloads to avoid nested context disposal
            IQueryable<MasterInquiryItem> query = req.MemberType switch
            {
                1 => await _employeeInquiryService.GetEmployeeInquiryQueryAsync(ctx, null, cancellationToken),
                2 => await _beneficiaryInquiryService.GetBeneficiaryInquiryQueryAsync(ctx, null, cancellationToken),
                _ => (await _employeeInquiryService.GetEmployeeInquiryQueryAsync(ctx, null, cancellationToken))
                    .Union(await _beneficiaryInquiryService.GetBeneficiaryInquiryQueryAsync(ctx, null, cancellationToken))
            };

            var masterInquiryRequest = new MasterInquiryRequest
            {
                MemberType = req.MemberType,
                BadgeNumber = req.BadgeNumber,
                Ssn = !string.IsNullOrWhiteSpace(req.Ssn) ? int.Parse(req.Ssn) : 0,
                EndProfitYear = req.ProfitYear,
                ProfitCode = req.ProfitCode,
                ContributionAmount = req.ContributionAmount,
                EarningsAmount = req.EarningsAmount,
                ForfeitureAmount = req.ForfeitureAmount,
                PaymentAmount = req.PaymentAmount,
                Name = req.Name,
                PaymentType = req.PaymentType
            };

            query = FilterMemberQuery(masterInquiryRequest, query);

            if (req.Id.HasValue)
            {
                query = query.Where(x => x.Member.Id == req.Id.Value);
            }

            if (req.ProfitYear.HasValue)
            {
                query = query.Where(x => x.ProfitDetail != null && x.ProfitDetail.ProfitYear == req.ProfitYear.Value);
            }

            if (req.MonthToDate.HasValue)
            {
                query = query.Where(x => x.ProfitDetail != null && x.ProfitDetail.MonthToDate == req.MonthToDate.Value);
            }

            var paymentProfitCodes = ProfitDetailExtensions.GetProfitCodesForBalanceCalc();

            // First projection: SQL-translatable only
            var rawQuery = await query.Select(x => new MasterInquiryRawDto
            {
                Id = x.ProfitDetail != null ? x.ProfitDetail.Id : 0,
                Ssn = x.ProfitDetail != null ? x.ProfitDetail.Ssn : x.Member.Ssn,
                ProfitYear = x.ProfitDetail != null ? x.ProfitDetail.ProfitYear : (short)0,
                ProfitYearIteration = x.ProfitDetail != null ? x.ProfitDetail.ProfitYearIteration : (byte)0,
                DistributionSequence = x.ProfitDetail != null ? x.ProfitDetail.DistributionSequence : 0,
                ProfitCodeId = x.ProfitDetail != null ? x.ProfitDetail.ProfitCodeId : (byte)0,
                ProfitCodeName = x.ProfitCode != null ? x.ProfitCode.Name : string.Empty,
                Contribution = x.ProfitDetail != null ? x.ProfitDetail.Contribution : 0,
                Earnings = x.ProfitDetail != null ? x.ProfitDetail.Earnings : 0,
                Forfeiture = x.ProfitDetail != null ? !paymentProfitCodes.Contains(x.ProfitDetail.ProfitCodeId) ? x.ProfitDetail.Forfeiture : 0 : 0,
                Payment = x.ProfitDetail != null ? paymentProfitCodes.Contains(x.ProfitDetail.ProfitCodeId) ? x.ProfitDetail.Forfeiture : 0 : 0,
                MonthToDate = x.ProfitDetail != null ? x.ProfitDetail.MonthToDate : (byte)0,
                YearToDate = x.ProfitDetail != null ? x.ProfitDetail.YearToDate : (short)0,
                Remark = x.ProfitDetail != null ? x.ProfitDetail.Remark : null,
                ZeroContributionReasonId = x.ProfitDetail != null ? x.ProfitDetail.ZeroContributionReasonId : null,
                ZeroContributionReasonName = x.ZeroContributionReason != null ? x.ZeroContributionReason.Name : string.Empty,
                FederalTaxes = x.ProfitDetail != null ? x.ProfitDetail.FederalTaxes : 0,
                StateTaxes = x.ProfitDetail != null ? x.ProfitDetail.StateTaxes : 0,
                TaxCodeId = x.ProfitDetail != null && x.ProfitDetail.TaxCodeId != null ? x.ProfitDetail.TaxCodeId : TaxCode.Constants.Unknown.Id,
                TaxCodeName = x.TaxCode != null ? x.TaxCode.Name : TaxCode.Constants.Unknown.Name,
                CommentTypeId = x.ProfitDetail != null ? x.ProfitDetail.CommentTypeId : null,
                CommentTypeName = x.CommentType != null ? x.CommentType.Name : string.Empty,
                CommentRelatedCheckNumber = x.ProfitDetail != null ? x.ProfitDetail.CommentRelatedCheckNumber : null,
                CommentRelatedState = x.ProfitDetail != null ? x.ProfitDetail.CommentRelatedState : null,
                CommentRelatedOracleHcmId = x.ProfitDetail != null ? x.ProfitDetail.CommentRelatedOracleHcmId : null,
                CommentRelatedPsnSuffix = x.ProfitDetail != null ? x.ProfitDetail.CommentRelatedPsnSuffix : null,
                CommentIsPartialTransaction = x.ProfitDetail != null && x.ProfitDetail.CommentIsPartialTransaction != null ? x.ProfitDetail.CommentIsPartialTransaction : false,
                BadgeNumber = x.Member.BadgeNumber,
                PsnSuffix = x.Member.PsnSuffix,
                PayFrequencyId = x.Member.PayFrequencyId,
                TransactionDate = x.TransactionDate,
                CurrentIncomeYear = x.Member.CurrentIncomeYear,
                CurrentHoursYear = x.Member.CurrentHoursYear,
                IsExecutive = x.Member.IsExecutive,
            }).ToPaginationResultsAsync(req, cancellationToken);

            var formattedResults = rawQuery.Results.Select(x => new MasterInquiryResponseDto
            {
                Id = x.Id,
                Ssn = x.Ssn.MaskSsn(),
                ProfitYear = x.ProfitYear,
                ProfitYearIteration = x.ProfitYearIteration,
                DistributionSequence = x.DistributionSequence,
                ProfitCodeId = x.ProfitCodeId,
                ProfitCodeName = x.ProfitCodeName,
                Contribution = x.Contribution,
                Earnings = x.Earnings,
                Forfeiture = x.Forfeiture,
                Payment = x.Payment,
                MonthToDate = x.MonthToDate,
                YearToDate = x.YearToDate,
                Remark = x.Remark,
                ZeroContributionReasonId = x.ZeroContributionReasonId,
                ZeroContributionReasonName = x.ZeroContributionReasonName,
                FederalTaxes = x.FederalTaxes,
                StateTaxes = x.StateTaxes,
                TaxCodeId = x.TaxCodeId,
                TaxCodeName = x.TaxCodeName,
                CommentTypeId = x.CommentTypeId,
                CommentTypeName = x.CommentTypeName,
                CommentRelatedCheckNumber = x.CommentRelatedCheckNumber,
                CommentRelatedState = x.CommentRelatedState,
                CommentRelatedOracleHcmId = x.CommentRelatedOracleHcmId,
                CommentRelatedPsnSuffix = x.CommentRelatedPsnSuffix,
                CommentIsPartialTransaction = x.CommentIsPartialTransaction,
                BadgeNumber = x.BadgeNumber,
                PsnSuffix = x.PsnSuffix,
                PayFrequencyId = x.PayFrequencyId,
                TransactionDate = x.TransactionDate,
                CurrentIncomeYear = x.CurrentIncomeYear,
                CurrentHoursYear = x.CurrentHoursYear,
                IsExecutive = x.IsExecutive,
                EmploymentStatusId = x.EmploymentStatusId ?? '\0',
            });

            return new PaginatedResponseDto<MasterInquiryResponseDto>(req) { Results = formattedResults, Total = rawQuery.Total };
        }, cancellationToken);
    }

    private async Task<IQueryable<MasterInquiryItem>> GetMasterInquiryDemographics(
        ProfitSharingReadOnlyDbContext ctx,
        MasterInquiryRequest? req = null,
        CancellationToken cancellationToken = default)
    {
        return await _employeeInquiryService.GetEmployeeInquiryQueryAsync(ctx, req, cancellationToken);
    }

    private async Task<IQueryable<MasterInquiryItem>> GetMasterInquiryBeneficiary(
        ProfitSharingReadOnlyDbContext ctx,
        MasterInquiryRequest? req = null,
        CancellationToken cancellationToken = default)
    {
        return await _beneficiaryInquiryService.GetBeneficiaryInquiryQueryAsync(ctx, req, cancellationToken);
    }

    private async Task<(int ssn, MemberDetails? memberDetails)> GetDemographicDetails(
       int id, short currentYear, short previousYear, CancellationToken cancellationToken = default)
    {
        return await _employeeInquiryService.GetEmployeeDetailsAsync(id, currentYear, previousYear, cancellationToken);
    }

    private async Task<(int ssn, MemberDetails? memberDetails)> GetBeneficiaryDetails(
     int id, CancellationToken cancellationToken = default)
    {
        return await _beneficiaryInquiryService.GetBeneficiaryDetailsAsync(id, cancellationToken);
    }

    private async Task<IEnumerable<MemberProfitPlanDetails>> GetVestingDetails(Dictionary<int, MemberDetails> memberDetailsMap,
        short currentYear,
        short previousYear,
        CancellationToken cancellationToken)
    {
        // Here we recognize 2024 as the transition year to relying on the SMART YE Process
        bool isPreviousYearEndComplete = (previousYear < ReferenceData.SmartTransitionYear) || await _dataContextFactory.UseReadOnlyContext(async ctx =>
            await ctx.YearEndUpdateStatuses
                .AnyAsync(x => x.ProfitYear == previousYear && x.IsYearEndCompleted, cancellationToken), cancellationToken);
        bool isProfitYearYearEndComplete = (currentYear < ReferenceData.SmartTransitionYear) || await _dataContextFactory.UseReadOnlyContext(async ctx =>
            await ctx.YearEndUpdateStatuses
                .AnyAsync(x => x.ProfitYear == currentYear && x.IsYearEndCompleted, cancellationToken), cancellationToken);
        bool isWallClockYear = currentYear == DateTime.Now.Year;

        var ssnCollection = memberDetailsMap.Keys.ToHashSet();
        List<BalanceEndpointResponse> currentBalance = [];
        List<BalanceEndpointResponse> previousBalance = [];
        try
        {
            var previousBalanceTask =
                _totalService.GetVestingBalanceForMembersAsync(
                    SearchBy.Ssn, ssnCollection, previousYear, cancellationToken);

            var currentBalanceTask =
                _totalService.GetVestingBalanceForMembersAsync(
                    SearchBy.Ssn, ssnCollection, currentYear, cancellationToken);

            await Task.WhenAll(previousBalanceTask, currentBalanceTask);

            currentBalance = await currentBalanceTask;
            previousBalance = await previousBalanceTask;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to retrieve balances for SSN {SSN}", ssnCollection);
        }

        var detailsList = new List<MemberProfitPlanDetails>(memberDetailsMap.Count);

        foreach (var kvp in memberDetailsMap)
        {
            var memberData = kvp.Value;
            var balance = currentBalance.FirstOrDefault(b => b.Id == kvp.Key, new BalanceEndpointResponse { Id = kvp.Key, Ssn = memberData.Ssn });
            var previousBalanceItem = previousBalance.FirstOrDefault(b => b.Id == kvp.Key);

            detailsList.Add(new MemberProfitPlanDetails
            {
                IsEmployee = memberData.IsEmployee,
                Id = memberData.Id,
                FirstName = memberData.FirstName,
                LastName = memberData.LastName,
                AddressCity = memberData.AddressCity,
                AddressState = memberData.AddressState,
                Address = memberData.Address,
                AddressZipCode = memberData.AddressZipCode,
                DateOfBirth = memberData.DateOfBirth,
                Age = memberData.DateOfBirth.Age(),
                Ssn = memberData.Ssn,
                IsExecutive = memberData.IsExecutive,
                YearToDateProfitSharingHours = memberData.YearToDateProfitSharingHours,
                HireDate = memberData.HireDate,
                ReHireDate = memberData.ReHireDate,
                TerminationDate = memberData.TerminationDate,
                StoreNumber = memberData.StoreNumber,
                EnrollmentId = memberData.EnrollmentId,
                Enrollment = memberData.Enrollment,
                CurrentEtva = memberData.CurrentEtva,
                PreviousEtva = memberData.PreviousEtva,
                EmploymentStatus = memberData.EmploymentStatus,
                Missives = memberData.Missives,
                BadgesOfDuplicateSsns = memberData.BadgesOfDuplicateSsns,
                YearsInPlan = balance?.YearsInPlan ?? 0,
                PercentageVested = balance?.VestingPercent ?? 0,
                BadgeNumber = memberData.BadgeNumber,
                PsnSuffix = memberData.PsnSuffix,
                PayFrequencyId = 0,
                BeginPSAmount = isPreviousYearEndComplete ? (previousBalanceItem?.CurrentBalance ?? 0) : null,
                // "Current" is really "Now" or "At end of Year End"
                CurrentPSAmount = isWallClockYear || isProfitYearYearEndComplete ? (balance?.CurrentBalance ?? 0) : null,
                BeginVestedAmount = isPreviousYearEndComplete ? (previousBalanceItem?.VestedBalance ?? 0) : null,
                // "Current" is really "Now" or "At end of Year End"
                CurrentVestedAmount = isWallClockYear || isProfitYearYearEndComplete ? (balance?.VestedBalance ?? 0) : null,

                FullTimeDate = memberData.FullTimeDate,
                Department = memberData.Department,
                TerminationReason = memberData.TerminationReason,
                Gender = memberData.Gender,
                PayClassification = memberData.PayClassification,
                PhoneNumber = memberData.PhoneNumber,
                WorkLocation = memberData.WorkLocation,

                AllocationToAmount = balance?.AllocationsToBeneficiary ?? 0,
                AllocationFromAmount = balance?.AllocationsFromBeneficiary ?? 0,
                ReceivedContributionsLastYear = isPreviousYearEndComplete ? memberData.ReceivedContributionsLastYear : null
            });
        }

        return detailsList;
    }

    private async Task<PaginatedResponseDto<MemberDetails>> GetDemographicDetailsForSsns(SortedPaginationRequestDto req, ISet<int> ssns, short currentYear, short previousYear, ISet<int> duplicateSsns, CancellationToken cancellationToken = default)
    {
        return await _employeeInquiryService.GetEmployeeDetailsForSsnsAsync((MasterInquiryRequest)req, ssns, currentYear, previousYear, duplicateSsns, cancellationToken);
    }

    private Task<PaginatedResponseDto<MemberDetails>> GetBeneficiaryDetailsForSsns(
        SortedPaginationRequestDto req,
        ISet<int> ssns,
        CancellationToken cancellationToken = default)
    {
        return _beneficiaryInquiryService.GetBeneficiaryDetailsForSsnsAsync(req, ssns, cancellationToken);
    }

    private static IQueryable<MasterInquiryItem> FilterMemberQuery(MasterInquiryRequest req, IQueryable<MasterInquiryItem> query)
    {
        return MasterInquiryHelpers.FilterMemberQuery(req, query);
    }

    private async Task<List<MemberDetails>> GetAllDemographicDetailsForSsns(ISet<int> ssns, short currentYear, short previousYear, ISet<int> duplicateSsns, CancellationToken cancellationToken = default)
    {
        return await _employeeInquiryService.GetAllEmployeeDetailsForSsnsAsync(ssns, currentYear, previousYear, duplicateSsns, cancellationToken);
    }

    private async Task<List<MemberDetails>> GetAllBeneficiaryDetailsForSsns(ISet<int> ssns, CancellationToken cancellationToken = default)
    {
        return await _beneficiaryInquiryService.GetAllBeneficiaryDetailsForSsnsAsync(ssns, cancellationToken);
    }

    private static IQueryable<MemberDetails> ApplySorting(IQueryable<MemberDetails> query, SortedPaginationRequestDto req)
    {
        return MasterInquiryHelpers.ApplySorting(query, req);
    }
}
