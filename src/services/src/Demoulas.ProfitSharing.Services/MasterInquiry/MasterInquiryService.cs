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

public sealed class MasterInquiryService : IMasterInquiryService
{
    #region Private DTO

    private sealed class MasterInquiryItem
    {
        public required ProfitDetail? ProfitDetail { get; init; }
        public required InquiryDemographics Member { get; init; }
        public required ProfitCode? ProfitCode { get; init; }
        public required ZeroContributionReason? ZeroContributionReason { get; init; }
        public required TaxCode? TaxCode { get; init; }
        public required CommentType? CommentType { get; init; }
        public DateTimeOffset TransactionDate { get; init; }
    }

    public sealed class InquiryDemographics
    {
        public int BadgeNumber { get; init; }
        public required string FullName { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }

        public byte PayFrequencyId { get; init; }
        public short PsnSuffix { get; init; }
        public int Ssn { get; init; }
        public decimal CurrentIncomeYear { get; init; }
        public decimal CurrentHoursYear { get; init; }
        public int Id { get; set; }
        public bool IsExecutive { get; set; }
    }

    // Internal DTO for SQL-translatable projection
    private sealed class MasterInquiryRawDto
    {
        public int Id { get; init; }
        public int Ssn { get; init; }
        public short ProfitYear { get; init; }
        public byte ProfitYearIteration { get; init; }
        public int DistributionSequence { get; init; }
        public byte ProfitCodeId { get; init; }
        public string ProfitCodeName { get; init; } = string.Empty;
        public decimal Contribution { get; init; }
        public decimal Earnings { get; init; }
        public decimal Forfeiture { get; init; }
        public byte MonthToDate { get; init; }
        public short YearToDate { get; init; }
        public string? Remark { get; init; }
        public byte? ZeroContributionReasonId { get; init; }
        public string? ZeroContributionReasonName { get; init; }
        public decimal FederalTaxes { get; init; }
        public decimal StateTaxes { get; init; }
        public char? TaxCodeId { get; init; }
        public string? TaxCodeName { get; init; }
        public int? CommentTypeId { get; init; }
        public string? CommentTypeName { get; init; }
        public string? CommentRelatedCheckNumber { get; init; }
        public string? CommentRelatedState { get; init; }
        public long? CommentRelatedOracleHcmId { get; init; }
        public short? CommentRelatedPsnSuffix { get; init; }
        public bool? CommentIsPartialTransaction { get; init; }
        public int BadgeNumber { get; init; }
        public short PsnSuffix { get; init; }
        public byte PayFrequencyId { get; init; }
        public DateTimeOffset TransactionDate { get; init; }
        public decimal CurrentIncomeYear { get; init; }
        public decimal CurrentHoursYear { get; init; }
        public decimal Payment { get; set; }
        public bool IsExecutive { get; set; }
    }


    #endregion

    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ILogger _logger;
    private readonly ITotalService _totalService;
    private readonly IMissiveService _missiveService;
    private readonly IDemographicReaderService _demographicReaderService;

    public MasterInquiryService(
        IProfitSharingDataContextFactory dataContextFactory,
        ITotalService totalService,
        ILoggerFactory loggerFactory,
        IMissiveService missiveService,
        IDemographicReaderService demographicReaderService)
    {
        _dataContextFactory = dataContextFactory;
        _totalService = totalService;
        _missiveService = missiveService;
        _demographicReaderService = demographicReaderService;
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
                // CRITICAL OPTIMIZATION: For highly selective filters (PaymentType, EndProfitYear),
                // query ProfitDetails FIRST to get SSN list, then join member data.
                // This avoids expensive joins on full datasets.
                HashSet<int> ssnList;

                if (ShouldUseOptimizedSsnQuery(req))
                {
                    // Fast path: Get SSNs directly from filtered ProfitDetails
                    ssnList = await GetSsnsFromProfitDetails(ctx, req, timeoutToken);
                }
                else
                {
                    // Original path: Build full query for complex filters
                    IQueryable<MasterInquiryItem> query = req.MemberType switch
                    {
                        1 => await GetMasterInquiryDemographics(ctx, req),
                        2 => GetMasterInquiryBeneficiary(ctx, req),
                        _ => (await GetMasterInquiryDemographics(ctx, req)).Union(GetMasterInquiryBeneficiary(ctx, req))
                    };

                    query = FilterMemberQuery(req, query).TagWith("MasterInquiry: Filtered member query");

                    // Get unique SSNs from the query with timeout
                    // EF Core 9: This will use optimized SQL for DISTINCT
                    ssnList = await query
                        .Select(x => x.Member.Ssn)
                        .Distinct()
                        .TagWith("MasterInquiry: Extract unique SSNs")
                        .ToHashSetAsync(timeoutToken);
                }

                var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);

                // Optimize duplicate detection - fetch in one query with SSN filtering
                // EF Core 9: This uses optimized GROUP BY with HAVING COUNT(*) > 1
                var duplicateSsns = await demographics
                    .Where(d => ssnList.Contains(d.Ssn))
                    .GroupBy(d => d.Ssn)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .TagWith("MasterInquiry: Find duplicate SSNs")
                    .ToHashSetAsync(timeoutToken);

                if (ssnList.Count == 0 && (req.Ssn != 0 || req.BadgeNumber != 0))
                {
                    // If an exact match is found, then the bene or empl is added to the ssnList.
                    await HandleExactBadgeOrSsn(ctx, ssnList, req.BadgeNumber, req.PsnSuffix, req.Ssn, timeoutToken);
                }

                // Early return if no results found
                if (ssnList.Count == 0)
                {
                    return new PaginatedResponseDto<MemberDetails>(req) { Results = [], Total = 0 };
                }

                short currentYear = req.ProfitYear;
                short previousYear = (short)(currentYear - 1);
                var memberType = req.MemberType;
                PaginatedResponseDto<MemberDetails> detailsList;

                if (memberType == 1)
                {
                    detailsList = await GetDemographicDetailsForSsns(ctx, req, ssnList, currentYear, previousYear, duplicateSsns, timeoutToken);
                }
                else if (memberType == 2)
                {
                    detailsList = await GetBeneficiaryDetailsForSsns(ctx, req, ssnList, timeoutToken);
                }
                else
                {
                    // For both, merge and deduplicate by SSN
                    var employeeDetails = await GetAllDemographicDetailsForSsns(ctx, ssnList, currentYear, previousYear, duplicateSsns, timeoutToken);
                    var beneficiaryDetails = await GetAllBeneficiaryDetailsForSsns(ctx, ssnList, timeoutToken);

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
        // Use optimized path when we have filters that can reduce the dataset before joining
        // Complex filters (name search, specific amounts) require full join
        bool hasComplexFilters = !string.IsNullOrWhiteSpace(req.Name)
            || req.ContributionAmount.HasValue
            || req.EarningsAmount.HasValue
            || req.ForfeitureAmount.HasValue
            || req.PaymentAmount.HasValue;

        if (hasComplexFilters)
        {
            return false; // Must use standard path for complex filters
        }

        // Use fast path if we have PaymentType (highly selective)
        if (req.PaymentType.HasValue && req.PaymentType.Value > 0)
        {
            return true;
        }

        // Also use fast path if we have EndProfitYear with SSN or BadgeNumber (targeted lookup)
        if (req.EndProfitYear.HasValue && (req.Ssn != 0 || (req.BadgeNumber.HasValue && req.BadgeNumber.Value > 0)))
        {
            return true;
        }

        // For broad queries (all payment types, no specific member), use standard path with pre-filtering
        return false;
    }

    /// <summary>
    /// Optimized query to get SSNs directly from ProfitDetails with selective filters.
    /// This avoids expensive joins when we have highly selective criteria.
    /// </summary>
    private static async Task<HashSet<int>> GetSsnsFromProfitDetails(
        ProfitSharingReadOnlyDbContext ctx,
        MasterInquiryRequest req,
        CancellationToken cancellationToken)
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

        // Apply month filters
        if (req.StartProfitMonth.HasValue)
        {
            query = query.Where(pd => pd.MonthToDate >= req.StartProfitMonth.Value);
        }

        if (req.EndProfitMonth.HasValue)
        {
            query = query.Where(pd => pd.MonthToDate <= req.EndProfitMonth.Value);
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

        // Get distinct SSNs - this should be MUCH faster than joining first
        return await query
            .Select(pd => pd.Ssn)
            .Distinct()
            .TagWith("MasterInquiry: Get distinct SSNs (optimized path)")
            .ToHashSetAsync(cancellationToken);
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
                int ssnBene = await ctx.Beneficiaries
                    .AsNoTracking()
                    .Where(b => b.BadgeNumber == badgeNumber && b.PsnSuffix == psnSuffix)
                    .Join(ctx.BeneficiaryContacts, b => b.BeneficiaryContactId, bc => bc.Id, (b, bc) => bc.Ssn)
                    .FirstOrDefaultAsync(cancellationToken);

                if (ssnBene != 0)
                {
                    ssnList.Add(ssnBene);
                }
            }
            else if (badgeNumber != 0)
            {
                var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
                int ssnEmpl = await demographics
                    .AsNoTracking()
                    .Where(d => d.BadgeNumber == badgeNumber)
                    .Select(d => d.Ssn)
                    .FirstOrDefaultAsync(cancellationToken);

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
            IQueryable<MasterInquiryItem> query = req.MemberType switch
            {
                1 => await GetMasterInquiryDemographics(ctx),
                2 => GetMasterInquiryBeneficiary(ctx),
                _ => (await GetMasterInquiryDemographics(ctx)).Union(GetMasterInquiryBeneficiary(ctx))
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
        var members = await _dataContextFactory.UseReadOnlyContext(ctx =>
        {
            return req.MemberType switch
            {
                1 => GetDemographicDetails(ctx, req.Id, currentYear, previousYear, cancellationToken),
                2 => GetBeneficiaryDetails(ctx, req.Id, cancellationToken),
                _ => throw new ValidationException("Invalid MemberType provided")
            };
        }, cancellationToken);
        Dictionary<int, MemberDetails> memberDetailsMap = new Dictionary<int, MemberDetails> { { members.ssn, members.memberDetails ?? new MemberDetails { Id = 0 } } };

        var details = await GetVestingDetails(memberDetailsMap, currentYear, previousYear, cancellationToken);
        return details.FirstOrDefault();
    }

    public Task<PaginatedResponseDto<MasterInquiryResponseDto>> GetMemberProfitDetails(MasterInquiryMemberDetailsRequest req, CancellationToken cancellationToken = default)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            IQueryable<MasterInquiryItem> query = req.MemberType switch
            {
                1 => await GetMasterInquiryDemographics(ctx),
                2 => GetMasterInquiryBeneficiary(ctx),
                _ => (await GetMasterInquiryDemographics(ctx)).Union(GetMasterInquiryBeneficiary(ctx))
            };

            var masterInquiryRequest = new MasterInquiryRequest
            {
                MemberType = req.MemberType,
                BadgeNumber = req.BadgeNumber,
                Ssn = !string.IsNullOrWhiteSpace(req.Ssn) ? int.Parse(req.Ssn) : 0,
                EndProfitYear = req.EndProfitYear,
                StartProfitMonth = req.StartProfitMonth,
                EndProfitMonth = req.EndProfitMonth,
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
                IsExecutive = x.Member.IsExecutive
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
                IsExecutive = x.IsExecutive
            });

            return new PaginatedResponseDto<MasterInquiryResponseDto>(req) { Results = formattedResults, Total = rawQuery.Total };
        }, cancellationToken);
    }

    private async Task<IQueryable<MasterInquiryItem>> GetMasterInquiryDemographics(ProfitSharingReadOnlyDbContext ctx, MasterInquiryRequest? req = null)
    {
        var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);

        // EF Core 9: Use AsSplitQuery to avoid cartesian explosion from multiple includes
        // TagWith helps identify this query in profiling/logging
        var profitDetailsQuery = ctx.ProfitDetails.AsSplitQuery();

        // OPTIMIZATION: Pre-filter ProfitDetails before expensive join if we have selective criteria
        if (req?.EndProfitYear.HasValue == true)
        {
            profitDetailsQuery = profitDetailsQuery.Where(pd => pd.ProfitYear <= req.EndProfitYear.Value);
        }

        if (req?.PaymentType.HasValue == true)
        {
            // Apply payment type filter early for massive performance gain
            var commentTypeIds = req.PaymentType switch
            {
                1 => new byte?[] { CommentType.Constants.Hardship.Id, CommentType.Constants.Distribution.Id },
                2 => new byte?[] { CommentType.Constants.Payoff.Id, CommentType.Constants.Forfeit.Id },
                3 => new byte?[] { CommentType.Constants.Rollover.Id, CommentType.Constants.RothIra.Id },
                _ => Array.Empty<byte?>()
            };

            if (commentTypeIds.Length > 0)
            {
                profitDetailsQuery = profitDetailsQuery.Where(pd => commentTypeIds.Contains(pd.CommentTypeId));
            }
        }

        var query = profitDetailsQuery
            .Include(pd => pd.ProfitCode)
            .Include(pd => pd.ZeroContributionReason)
            .Include(pd => pd.TaxCode)
            .Include(pd => pd.CommentType)
            .TagWith("MasterInquiry: Get demographics with profit details")
            .Join(demographics,
                pd => pd.Ssn,
                d => d.Ssn,
                (pd, d) => new { pd, d })
            .GroupJoin(
                ctx.PayProfits,
                x => new { x.d.Id, x.pd.ProfitYear },
                pp => new { Id = pp.DemographicId, pp.ProfitYear },
                (x, payProfits) => new { x.pd, x.d, pp = payProfits.FirstOrDefault() })
            .Select(x => new MasterInquiryItem
            {
                ProfitDetail = x.pd,
                ProfitCode = x.pd.ProfitCode,
                ZeroContributionReason = x.pd.ZeroContributionReason,
                TaxCode = x.pd.TaxCode,
                CommentType = x.pd.CommentType,
                TransactionDate = x.pd.CreatedAtUtc,
                Member = new InquiryDemographics
                {
                    Id = x.d.Id,
                    BadgeNumber = x.d.BadgeNumber,
                    FullName = x.d.ContactInfo.FullName != null ? x.d.ContactInfo.FullName : x.d.ContactInfo.LastName,
                    FirstName = x.d.ContactInfo.FirstName,
                    LastName = x.d.ContactInfo.LastName,
                    PayFrequencyId = x.d.PayFrequencyId,
                    Ssn = x.d.Ssn,
                    PsnSuffix = 0,
                    IsExecutive = x.d.PayFrequencyId == PayFrequency.Constants.Monthly,
                    // Use LEFT JOIN result instead of correlated subqueries
                    CurrentIncomeYear = x.pp != null ? x.pp.CurrentIncomeYear : 0,
                    CurrentHoursYear = x.pp != null ? x.pp.CurrentHoursYear : 0
                }
            });

        return query;
    }
    private static IQueryable<MasterInquiryItem> GetMasterInquiryBeneficiary(ProfitSharingReadOnlyDbContext ctx, MasterInquiryRequest? req = null)
    {
        // EF Core 9: Use AsSplitQuery to avoid cartesian explosion
        var profitDetailsQuery = ctx.ProfitDetails.AsSplitQuery();

        // OPTIMIZATION: Pre-filter ProfitDetails before expensive join if we have selective criteria
        if (req?.EndProfitYear.HasValue == true)
        {
            profitDetailsQuery = profitDetailsQuery.Where(pd => pd.ProfitYear <= req.EndProfitYear.Value);
        }

        if (req?.PaymentType.HasValue == true)
        {
            // Apply payment type filter early for massive performance gain
            var commentTypeIds = req.PaymentType switch
            {
                1 => new byte?[] { CommentType.Constants.Hardship.Id, CommentType.Constants.Distribution.Id },
                2 => new byte?[] { CommentType.Constants.Payoff.Id, CommentType.Constants.Forfeit.Id },
                3 => new byte?[] { CommentType.Constants.Rollover.Id, CommentType.Constants.RothIra.Id },
                _ => Array.Empty<byte?>()
            };

            if (commentTypeIds.Length > 0)
            {
                profitDetailsQuery = profitDetailsQuery.Where(pd => commentTypeIds.Contains(pd.CommentTypeId));
            }
        }

        var query = profitDetailsQuery
            .Include(pd => pd.ProfitCode)
            .Include(pd => pd.ZeroContributionReason)
            .Include(pd => pd.TaxCode)
            .Include(pd => pd.CommentType)
            .TagWith("MasterInquiry: Get beneficiary with profit details")
            .Join(ctx.Beneficiaries.Join(ctx.BeneficiaryContacts, b => b.BeneficiaryContactId, bc => bc.Id, (b, bc) => new { b, bc }),
                pd => pd.Ssn, bene => bene.bc.Ssn,
                (pd, d) => new MasterInquiryItem
                {
                    ProfitDetail = pd,
                    ProfitCode = pd.ProfitCode,
                    ZeroContributionReason = pd.ZeroContributionReason,
                    TaxCode = pd.TaxCode,
                    CommentType = pd.CommentType,
                    TransactionDate = pd.CreatedAtUtc,
                    Member = new InquiryDemographics
                    {
                        Id = d.bc.Id,
                        BadgeNumber = d.b.BadgeNumber,
                        FullName = d.bc.ContactInfo.FullName != null ? d.bc.ContactInfo.FullName : d.bc.ContactInfo.LastName,
                        FirstName = d.bc.ContactInfo.FirstName,
                        LastName = d.bc.ContactInfo.LastName,
                        PayFrequencyId = 0,
                        Ssn = d.bc.Ssn,
                        PsnSuffix = d.b.PsnSuffix,
                        CurrentIncomeYear = 0,
                        CurrentHoursYear = 0,
                        IsExecutive = false,
                    }
                });

        return query;
    }

    private async Task<(int ssn, MemberDetails? memberDetails)> GetDemographicDetails(ProfitSharingReadOnlyDbContext ctx,
       int id, short currentYear, short previousYear, CancellationToken cancellationToken)
    {
        var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
        var memberData = await demographics
            .Include(d => d.PayProfits)
            .ThenInclude(pp => pp.Enrollment)
            .Include(d => d.Department)
            .Include(d => d.TerminationCode)
            .Include(d => d.PayClassification)
            .Include(d => d.Gender)
            .Where(d => d.Id == id)
            .Select(d => new
            {
                d.Id,
                d.ContactInfo.FirstName,
                d.ContactInfo.LastName,
                d.ContactInfo.PhoneNumber,
                d.Address.City,
                d.Address.State,
                Address = d.Address.Street,
                d.Address.PostalCode,
                d.DateOfBirth,
                d.Ssn,
                d.BadgeNumber,
                d.PayFrequencyId,
                d.ReHireDate,
                d.HireDate,
                d.TerminationDate,
                d.StoreNumber,
                DemographicId = d.Id,
                d.EmploymentStatusId,
                d.EmploymentStatus,
                IsExecutive = d.PayFrequencyId == PayFrequency.Constants.Monthly,
                d.FullTimeDate,
                Department = d.Department != null ? d.Department.Name : "N/A",
                TerminationReason = d.TerminationCode != null ? d.TerminationCode.Name : "N/A",
                Gender = d.Gender != null ? d.Gender.Name : "N/A",
                PayClassification = d.PayClassification != null ? d.PayClassification.Name : "N/A",

                CurrentPayProfit = d.PayProfits
                    .Select(x =>
                        new
                        {
                            x.ProfitYear,
                            x.CurrentHoursYear,
                            x.Etva,
                            x.EnrollmentId,
                            x.Enrollment
                        })
                    .FirstOrDefault(x => x.ProfitYear == currentYear),
                PreviousPayProfit = d.PayProfits
                    .Select(x =>
                        new
                        {
                            x.ProfitYear,
                            x.CurrentHoursYear,
                            x.Etva,
                            x.EnrollmentId,
                            x.Enrollment,
                            x.PsCertificateIssuedDate
                        })
                    .FirstOrDefault(x => x.ProfitYear == previousYear)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (memberData == null)
        {
            return (0, new MemberDetails { Id = 0 });
        }

        var missives = await _missiveService.DetermineMissivesForSsns([memberData.Ssn], currentYear, cancellationToken);
        var missiveList = missives.TryGetValue(memberData.Ssn, out var m) ? m : new List<int>();
        var duplicateSsns = await demographics
                .GroupBy(d => d.Ssn)
                .Where(g => g.Count() > 1 && g.Key == memberData.Ssn)
                .Select(g => g.Key)
                .ToHashSetAsync(cancellationToken);

        List<int> badgeNumbersOfDuplicates = [];
        if (duplicateSsns.Any())
        {
            badgeNumbersOfDuplicates = await demographics
                .Where(d => d.Ssn == memberData.Ssn && d.Id != memberData.DemographicId)
                .Select(d => d.BadgeNumber)
                .ToListAsync(cancellationToken);
        }

        return (ssn: memberData.Ssn, memberDetails: new MemberDetails
        {
            IsEmployee = true,
            Id = memberData.Id,
            FirstName = memberData.FirstName,
            LastName = memberData.LastName,
            AddressCity = memberData.City!,
            AddressState = memberData.State!,
            Address = memberData.Address,
            AddressZipCode = memberData.PostalCode!,
            DateOfBirth = memberData.DateOfBirth,
            Age = memberData.DateOfBirth.Age(),
            Ssn = memberData.Ssn.MaskSsn(),
            YearToDateProfitSharingHours = memberData.CurrentPayProfit?.CurrentHoursYear ?? 0,
            HireDate = memberData.HireDate,
            ReHireDate = memberData.ReHireDate,
            FullTimeDate = memberData.FullTimeDate,
            TerminationDate = memberData.TerminationDate,
            StoreNumber = memberData.StoreNumber,
            EnrollmentId = memberData.CurrentPayProfit?.EnrollmentId,
            Enrollment = memberData.CurrentPayProfit?.Enrollment?.Name,
            BadgeNumber = memberData.BadgeNumber,
            PayFrequencyId = memberData.PayFrequencyId,
            IsExecutive = memberData.IsExecutive,
            CurrentEtva = memberData.CurrentPayProfit?.Etva ?? 0,
            PreviousEtva = memberData.PreviousPayProfit?.Etva ?? 0,

            EmploymentStatus = memberData.EmploymentStatus?.Name,

            Department = memberData.Department,
            TerminationReason = memberData.TerminationReason,
            Gender = memberData.Gender,
            PayClassification = memberData.PayClassification,
            PhoneNumber = memberData.PhoneNumber,

            ReceivedContributionsLastYear = memberData.PreviousPayProfit?.PsCertificateIssuedDate != null,
            Missives = missiveList,
            BadgesOfDuplicateSsns = badgeNumbersOfDuplicates
        });
    }

    private async Task<(int ssn, MemberDetails? memberDetails)> GetBeneficiaryDetails(ProfitSharingReadOnlyDbContext ctx,
     int id, CancellationToken cancellationToken)
    {
        var memberData = await ctx.Beneficiaries
            .Include(b => b.Contact)
            .Where(b => b.Id == id)
            .Select(b => new
            {
                b.Id,
                b.Contact!.ContactInfo.FirstName,
                b.Contact.ContactInfo.LastName,
                b.Contact.Address.City,
                b.Contact.Address.State,
                Address = b.Contact.Address.Street,
                b.Contact.Address.PostalCode,
                b.Contact.DateOfBirth,
                b.Contact.Ssn,
                b.BadgeNumber,
                b.PsnSuffix,
                b.DemographicId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (memberData == null)
        {
            return (0, new MemberDetails { Id = 0 });
        }


        return (memberData.Ssn, new MemberDetails
        {
            IsEmployee = false,
            Id = memberData.Id,
            FirstName = memberData.FirstName,
            LastName = memberData.LastName,
            AddressCity = memberData.City!,
            AddressState = memberData.State!,
            Address = memberData.Address,
            AddressZipCode = memberData.PostalCode!,
            DateOfBirth = memberData.DateOfBirth,
            Age = memberData.DateOfBirth.Age(),
            Ssn = memberData.Ssn.MaskSsn(),
            BadgeNumber = memberData.BadgeNumber,
            PsnSuffix = memberData.PsnSuffix,
            PayFrequencyId = 0,
            IsExecutive = false,
        });
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

                AllocationToAmount = balance?.AllocationsToBeneficiary ?? 0,
                AllocationFromAmount = balance?.AllocationsFromBeneficiary ?? 0,
                ReceivedContributionsLastYear = isPreviousYearEndComplete ? memberData.ReceivedContributionsLastYear : null
            });
        }

        return detailsList;
    }

    private async Task<PaginatedResponseDto<MemberDetails>> GetDemographicDetailsForSsns(ProfitSharingReadOnlyDbContext ctx, SortedPaginationRequestDto req, ISet<int> ssns, short currentYear, short previousYear, ISet<int> duplicateSsns, CancellationToken cancellationToken)
    {
        var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);

        // EF Core 9: Use AsSplitQuery for complex includes to avoid cartesian explosion
        var query = demographics
            .Where(d => ssns.Contains(d.Ssn))
            .TagWith("MasterInquiry: Get demographic details for SSNs")
            .AsSplitQuery(); // Splits into multiple queries to avoid JOIN performance issues

        if (((MasterInquiryRequest)req).BadgeNumber.HasValue && ((MasterInquiryRequest)req).BadgeNumber != 0)
        {
            query = query.Where(d => d.BadgeNumber == ((MasterInquiryRequest)req).BadgeNumber);
        }

        // Optimize projection: only select needed fields, avoid loading unnecessary navigation properties
        var members = await query
            .Select(d => new
            {
                d.Id,
                d.ContactInfo.FullName,
                d.ContactInfo.FirstName,
                d.ContactInfo.LastName,
                d.Address.City,
                d.Address.State,
                Address = d.Address.Street,
                d.Address.PostalCode,
                d.DateOfBirth,
                d.Ssn,
                d.BadgeNumber,
                d.PayFrequencyId,
                d.ReHireDate,
                d.HireDate,
                d.TerminationDate,
                d.StoreNumber,
                DemographicId = d.Id,
                d.EmploymentStatusId,
                d.EmploymentStatus,
                IsExecutive = d.PayFrequencyId == PayFrequency.Constants.Monthly,
                // Optimize PayProfit queries - only fetch what we need for current/previous years
                CurrentPayProfit = d.PayProfits
                    .Where(x => x.ProfitYear == currentYear)
                    .Select(x => new
                    {
                        x.ProfitYear,
                        x.CurrentHoursYear,
                        x.Etva,
                        x.EnrollmentId,
                        x.Enrollment.Name
                    }).FirstOrDefault(),
                PreviousPayProfit = d.PayProfits
                    .Where(x => x.ProfitYear == previousYear)
                    .Select(x => new
                    {
                        x.ProfitYear,
                        x.CurrentHoursYear,
                        x.Etva,
                        x.EnrollmentId,
                        x.Enrollment.Name,
                        x.PsCertificateIssuedDate
                    }).FirstOrDefault()
            })
            .ToPaginationResultsAsync(req, cancellationToken);

        // Fetch missives for all SSNs in one query (exclude duplicates)
        var nonDuplicateSsns = members.Results.Select(m => m.Ssn).Except(duplicateSsns).ToList();
        var missivesDict = await _missiveService.DetermineMissivesForSsns(nonDuplicateSsns, currentYear, cancellationToken);

        // Fetch all duplicate badge numbers in one query instead of N queries
        // EF Core 9: Optimized batch query
        var duplicateBadgeMap = new Dictionary<int, List<int>>();
        if (duplicateSsns.Any())
        {
            var duplicateData = await demographics
                .Where(d => duplicateSsns.Contains(d.Ssn))
                .Select(d => new { d.Ssn, d.BadgeNumber, d.Id })
                .TagWith("MasterInquiry: Fetch duplicate badges")
                .ToListAsync(cancellationToken);

            foreach (var dup in duplicateSsns)
            {
                duplicateBadgeMap[dup] = duplicateData
                    .Where(d => d.Ssn == dup)
                    .Select(d => d.BadgeNumber)
                    .Distinct()
                    .ToList();
            }
        }

        var detailsList = new List<MemberDetails>();
        foreach (var memberData in members.Results)
        {
            var missiveList = missivesDict.TryGetValue(memberData.Ssn, out var m) ? m : new List<int>();

            // Get duplicate badges from pre-fetched map
            var duplicateBadges = duplicateSsns.Contains(memberData.Ssn) && duplicateBadgeMap.TryGetValue(memberData.Ssn, out var badges)
                ? badges.Where(b => b != memberData.BadgeNumber).ToList()
                : new List<int>();

            detailsList.Add(new MemberDetails
            {
                IsEmployee = true,
                Id = memberData.Id,
                FirstName = memberData.FirstName,
                LastName = memberData.LastName,
                AddressCity = memberData.City!,
                AddressState = memberData.State!,
                Address = memberData.Address,
                AddressZipCode = memberData.PostalCode!,
                DateOfBirth = memberData.DateOfBirth,
                IsExecutive = memberData.IsExecutive,
                Age = memberData.DateOfBirth.Age(),
                Ssn = memberData.Ssn.MaskSsn(),
                YearToDateProfitSharingHours = memberData.CurrentPayProfit?.CurrentHoursYear ?? 0,
                HireDate = memberData.HireDate,
                ReHireDate = memberData.ReHireDate,
                TerminationDate = memberData.TerminationDate,
                StoreNumber = memberData.StoreNumber,
                EnrollmentId = memberData.CurrentPayProfit?.EnrollmentId,
                Enrollment = memberData.CurrentPayProfit?.Name,
                BadgeNumber = memberData.BadgeNumber,
                PayFrequencyId = memberData.PayFrequencyId,
                CurrentEtva = memberData.CurrentPayProfit?.Etva ?? 0,
                PreviousEtva = memberData.PreviousPayProfit?.Etva ?? 0,
                EmploymentStatus = memberData.EmploymentStatus?.Name,
                ReceivedContributionsLastYear = memberData.PreviousPayProfit?.PsCertificateIssuedDate != null,
                Missives = missiveList,
                BadgesOfDuplicateSsns = duplicateBadges
            });
        }

        return new PaginatedResponseDto<MemberDetails>(req) { Results = detailsList, Total = members.Total };
    }

    private Task<PaginatedResponseDto<MemberDetails>> GetBeneficiaryDetailsForSsns(ProfitSharingReadOnlyDbContext ctx,
        SortedPaginationRequestDto req,
        ISet<int> ssns,
        CancellationToken cancellationToken)
    {
        // EF Core 9: Optimize beneficiary query with better projection
        var membersQuery = ctx.Beneficiaries
            .Where(b => b.Contact != null && ssns.Contains(b.Contact.Ssn))
            .TagWith("MasterInquiry: Get beneficiary details for SSNs");

        // Only filter by BadgeNumber if provided and not 0
        var badgeNumber = ((MasterInquiryRequest)req).BadgeNumber;
        if (badgeNumber.HasValue && badgeNumber != 0)
        {
            membersQuery = membersQuery.Where(b => b.BadgeNumber == badgeNumber);
        }

        // Optimize projection: select only what we need
        var members = membersQuery
            .Select(b => new
            {
                b.Id,
                b.Contact!.ContactInfo.FullName,
                b.Contact!.ContactInfo.FirstName,
                b.Contact.ContactInfo.LastName,
                b.Contact.Address.City,
                b.Contact.Address.State,
                Address = b.Contact.Address.Street,
                b.Contact.Address.PostalCode,
                b.Contact.DateOfBirth,
                b.Contact.Ssn,
                b.BadgeNumber,
                b.PsnSuffix,
                DemographicId = b.Id
            });


        return members.Select(memberData => new MemberDetails
        {
            Id = memberData.Id,
            IsEmployee = false,
            FirstName = memberData.FirstName,
            LastName = memberData.LastName,
            AddressCity = memberData.City!,
            AddressState = memberData.State!,
            Address = memberData.Address,
            AddressZipCode = memberData.PostalCode!,
            DateOfBirth = memberData.DateOfBirth,
            Ssn = memberData.Ssn.MaskSsn(),
            BadgeNumber = memberData.BadgeNumber,
            PsnSuffix = memberData.PsnSuffix,
            PayFrequencyId = 0,
            IsExecutive = false,
        })
            .ToPaginationResultsAsync(req, cancellationToken);
    }

    private static IQueryable<MasterInquiryItem> FilterMemberQuery(MasterInquiryRequest req, IQueryable<MasterInquiryItem> query)
    {
        // CRITICAL: Apply most selective filters first for Oracle query optimizer
        // PaymentType is highly selective, so apply it early
        if (req.PaymentType.HasValue)
        {
            switch (req.PaymentType)
            {
                case 1: // Hardship/Distribution
                    List<byte?> array = [CommentType.Constants.Hardship.Id, CommentType.Constants.Distribution.Id];
                    query = query.Where(x => x.ProfitDetail != null && array.Contains(x.ProfitDetail.CommentTypeId));
                    break;
                case 2: // payoffs
                    array = [CommentType.Constants.Payoff.Id, CommentType.Constants.Forfeit.Id];
                    query = query.Where(x => x.ProfitDetail != null && array.Contains(x.ProfitDetail.CommentTypeId));
                    break;
                case 3: // rollovers
                    array = [CommentType.Constants.Rollover.Id, CommentType.Constants.RothIra.Id];
                    query = query.Where(x => x.ProfitDetail != null && array.Contains(x.ProfitDetail.CommentTypeId));
                    break;
            }
        }

        // Apply EndProfitYear early - it's highly selective
        if (req.EndProfitYear.HasValue)
        {
            query = query.Where(x => (x.ProfitDetail == null || x.ProfitDetail.ProfitYear <= req.EndProfitYear));
        }

        // SSN is most selective - always apply early if present
        if (req.Ssn != 0)
        {
            query = query.Where(x => (x.Member.Ssn == req.Ssn));
        }

        // BadgeNumber is very selective
        if (req.BadgeNumber.HasValue && req.BadgeNumber > 0)
        {
            query = query.Where(x => x.Member.BadgeNumber == req.BadgeNumber);
        }

        // Name filter is selective
        if (!string.IsNullOrWhiteSpace(req.Name))
        {
            var pattern = $"%{req.Name.ToUpperInvariant()}%";
            query = query.Where(x => EF.Functions.Like(x.Member.FullName.ToUpper(), pattern));
        }

        // ProfitCode is moderately selective
        if (req.ProfitCode.HasValue)
        {
            query = query.Where(x => (x.ProfitDetail == null || x.ProfitDetail.ProfitCodeId == req.ProfitCode));
        }

        // Apply other filters
        if (req.MemberType != 1 /* Employee Only */ && req.PsnSuffix > 0)
        {
            query = query.Where(x => x.Member.PsnSuffix == req.PsnSuffix);
        }

        if (req.StartProfitMonth.HasValue)
        {
            query = query.Where(x => (x.ProfitDetail == null || x.ProfitDetail.MonthToDate >= req.StartProfitMonth));
        }

        if (req.EndProfitMonth.HasValue)
        {
            query = query.Where(x => (x.ProfitDetail == null || x.ProfitDetail.MonthToDate <= req.EndProfitMonth));
        }

        if (req.ContributionAmount.HasValue)
        {
            query = query.Where(x => (x.ProfitDetail == null || x.ProfitDetail.Contribution == req.ContributionAmount));
        }

        if (req.EarningsAmount.HasValue)
        {
            query = query.Where(x => (x.ProfitDetail == null || x.ProfitDetail.Earnings == req.EarningsAmount));
        }

        if (req.ForfeitureAmount.HasValue)
        {
            query = query.Where(x =>
                (x.ProfitDetail == null || x.ProfitDetail.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id) &&
                (x.ProfitDetail == null || x.ProfitDetail.Forfeiture == req.ForfeitureAmount));
        }

        if (req.PaymentAmount.HasValue)
        {
            query = query.Where(x =>
                (x.ProfitDetail == null || x.ProfitDetail.ProfitCodeId != ProfitCode.Constants.IncomingContributions.Id) &&
                (x.ProfitDetail == null || x.ProfitDetail.Forfeiture == req.PaymentAmount));
        }

        return query;
    }

    private async Task<List<MemberDetails>> GetAllDemographicDetailsForSsns(ProfitSharingReadOnlyDbContext ctx, ISet<int> ssns, short currentYear, short previousYear, ISet<int> duplicateSsns, CancellationToken cancellationToken)
    {
        var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);

        // EF Core 9: Use AsSplitQuery to avoid cartesian explosion with PayProfits
        var query = demographics
            .Where(d => ssns.Contains(d.Ssn))
            .TagWith("MasterInquiry: Get all demographic details for SSNs")
            .AsSplitQuery();

        // Optimize projection to fetch only needed data
        var members = await query
            .Select(d => new
            {
                d.Id,
                d.ContactInfo.FirstName,
                d.ContactInfo.LastName,
                d.Address.City,
                d.Address.State,
                Address = d.Address.Street,
                d.Address.PostalCode,
                d.DateOfBirth,
                d.Ssn,
                d.BadgeNumber,
                d.PayFrequencyId,
                d.ReHireDate,
                d.HireDate,
                d.TerminationDate,
                d.StoreNumber,
                DemographicId = d.Id,
                d.EmploymentStatusId,
                d.EmploymentStatus,
                IsExecutive = d.PayFrequencyId == PayFrequency.Constants.Monthly,
                // Optimize PayProfit subqueries - filter within projection
                CurrentPayProfit = d.PayProfits
                    .Where(x => x.ProfitYear == currentYear)
                    .Select(x => new
                    {
                        x.ProfitYear,
                        x.CurrentHoursYear,
                        x.Etva,
                        x.EnrollmentId,
                        x.Enrollment.Name
                    }).FirstOrDefault(),
                PreviousPayProfit = d.PayProfits
                    .Where(x => x.ProfitYear == previousYear)
                    .Select(x => new
                    {
                        x.ProfitYear,
                        x.CurrentHoursYear,
                        x.Etva,
                        x.EnrollmentId,
                        x.Enrollment.Name,
                        x.PsCertificateIssuedDate
                    }).FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        var missivesDict = await _missiveService.DetermineMissivesForSsns(members.Select(m => m.Ssn), currentYear, cancellationToken);

        // Fetch all duplicate badge numbers in one query instead of N queries
        // EF Core 9: Optimized batch query
        var duplicateBadgeMap = new Dictionary<int, List<int>>();
        if (duplicateSsns.Any())
        {
            var duplicateData = await demographics
                .Where(d => duplicateSsns.Contains(d.Ssn))
                .Select(d => new { d.Ssn, d.BadgeNumber, d.Id })
                .TagWith("MasterInquiry: Fetch duplicate badges (all)")
                .ToListAsync(cancellationToken);

            foreach (var dup in duplicateSsns)
            {
                duplicateBadgeMap[dup] = duplicateData
                    .Where(d => d.Ssn == dup)
                    .Select(d => d.BadgeNumber)
                    .Distinct()
                    .ToList();
            }
        }

        var detailsList = new List<MemberDetails>();
        foreach (var memberData in members)
        {
            var missiveList = missivesDict.TryGetValue(memberData.Ssn, out var m) ? m : new List<int>();

            // Get duplicate badges from pre-fetched map
            var duplicateBadges = duplicateSsns.Contains(memberData.Ssn) && duplicateBadgeMap.TryGetValue(memberData.Ssn, out var badges)
                ? badges.Where(b => b != memberData.BadgeNumber).ToList()
                : new List<int>();

            detailsList.Add(new MemberDetails
            {
                IsEmployee = true,
                Id = memberData.Id,
                FirstName = memberData.FirstName,
                LastName = memberData.LastName,
                AddressCity = memberData.City!,
                AddressState = memberData.State!,
                Address = memberData.Address,
                AddressZipCode = memberData.PostalCode!,
                DateOfBirth = memberData.DateOfBirth,
                Age = memberData.DateOfBirth.Age(),
                Ssn = memberData.Ssn.MaskSsn(),
                YearToDateProfitSharingHours = memberData.CurrentPayProfit?.CurrentHoursYear ?? 0,
                HireDate = memberData.HireDate,
                ReHireDate = memberData.ReHireDate,
                TerminationDate = memberData.TerminationDate,
                StoreNumber = memberData.StoreNumber,
                EnrollmentId = memberData.CurrentPayProfit?.EnrollmentId,
                Enrollment = memberData.CurrentPayProfit?.Name,
                BadgeNumber = memberData.BadgeNumber,
                PayFrequencyId = memberData.PayFrequencyId,
                CurrentEtva = memberData.CurrentPayProfit?.Etva ?? 0,
                PreviousEtva = memberData.PreviousPayProfit?.Etva ?? 0,
                EmploymentStatus = memberData.EmploymentStatus?.Name,
                ReceivedContributionsLastYear = memberData.PreviousPayProfit?.PsCertificateIssuedDate != null,
                Missives = missiveList,
                IsExecutive = memberData.IsExecutive,
                BadgesOfDuplicateSsns = duplicateBadges
            });
        }

        return detailsList;
    }

    private async Task<List<MemberDetails>> GetAllBeneficiaryDetailsForSsns(ProfitSharingReadOnlyDbContext ctx, ISet<int> ssns, CancellationToken cancellationToken)
    {
        // EF Core 9: Optimize projection to fetch only needed data
        var members = await ctx.Beneficiaries
            .Where(b => b.Contact != null && ssns.Contains(b.Contact.Ssn))
            .TagWith("MasterInquiry: Get all beneficiary details for SSNs")
            .Select(b => new
            {
                b.Id,
                b.Contact!.ContactInfo.FirstName,
                b.Contact.ContactInfo.LastName,
                b.Contact.Address.City,
                b.Contact.Address.State,
                Address = b.Contact.Address.Street,
                b.Contact.Address.PostalCode,
                b.Contact.DateOfBirth,
                b.Contact.Ssn,
                b.BadgeNumber,
                b.PsnSuffix,
                DemographicId = b.Id
            })
            .ToListAsync(cancellationToken);

        var detailsList = new List<MemberDetails>();
        foreach (var memberData in members)
        {
            detailsList.Add(new MemberDetails
            {
                Id = memberData.Id,
                IsEmployee = false,
                FirstName = memberData.FirstName,
                LastName = memberData.LastName,
                AddressCity = memberData.City!,
                AddressState = memberData.State!,
                Address = memberData.Address,
                AddressZipCode = memberData.PostalCode!,
                DateOfBirth = memberData.DateOfBirth,
                Ssn = memberData.Ssn.MaskSsn(),
                BadgeNumber = memberData.BadgeNumber,
                PsnSuffix = memberData.PsnSuffix,
                PayFrequencyId = 0,
                IsExecutive = false,
            });
        }

        return detailsList;
    }

    private static IQueryable<MemberDetails> ApplySorting(IQueryable<MemberDetails> query, SortedPaginationRequestDto req)
    {
        if (string.IsNullOrEmpty(req.SortBy))
        {
            return query;
        }

        var isDescending = req.IsSortDescending ?? false;
        return req.SortBy.ToLower() switch
        {
            "fullname" => isDescending ? query.OrderByDescending(x => x.FullName) : query.OrderBy(x => x.FullName),
            "ssn" => isDescending ? query.OrderByDescending(x => x.Ssn) : query.OrderBy(x => x.Ssn),
            "badgenumber" => isDescending ? query.OrderByDescending(x => x.BadgeNumber) : query.OrderBy(x => x.BadgeNumber),
            "address" => isDescending ? query.OrderByDescending(x => x.Address) : query.OrderBy(x => x.Address),
            "addresscity" => isDescending ? query.OrderByDescending(x => x.AddressCity) : query.OrderBy(x => x.AddressCity),
            "addressstate" => isDescending ? query.OrderByDescending(x => x.AddressState) : query.OrderBy(x => x.AddressState),
            "addresszipCode" => isDescending ? query.OrderByDescending(x => x.AddressZipCode) : query.OrderBy(x => x.AddressZipCode),
            "age" => isDescending ? query.OrderByDescending(x => x.Age) : query.OrderBy(x => x.Age),
            "employmentStatus" => isDescending ? query.OrderByDescending(x => x.EmploymentStatus) : query.OrderBy(x => x.EmploymentStatus),
            _ => query
        };
    }
}
