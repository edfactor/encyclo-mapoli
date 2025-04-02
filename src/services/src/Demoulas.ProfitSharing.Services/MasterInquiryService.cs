using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services;

public class MasterInquiryService : IMasterInquiryService
{
    private sealed  class MasterInquiryItem
    {
        public required ProfitDetail ProfitDetail { get; init; }
        public required InquiryDemographics Member { get; init; }
        public required ProfitCode ProfitCode { get; init; }
        public required ZeroContributionReason? ZeroContributionReason { get; init; }
        public required TaxCode? TaxCode { get; init; }
        public required CommentType? CommentType { get; init; }
    }

    private sealed class InquiryDemographics
    {
        public int BadgeNumber { get; init; }
        public byte PayFrequencyId { get; init; }
        public short PsnSuffix { get; init; }
        public int Ssn { get; init; }
    }


    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ILogger _logger;
    private readonly ITotalService _totalService;

    public MasterInquiryService(
        IProfitSharingDataContextFactory dataContextFactory,
        ITotalService totalService,
        ILoggerFactory loggerFactory
    )
    {
        _dataContextFactory = dataContextFactory;
        _totalService = totalService;
        _logger = loggerFactory.CreateLogger<MasterInquiryService>();
    }


    public async Task<MasterInquiryWithDetailsResponseDto> GetMasterInquiryAsync(MasterInquiryRequest req,
        CancellationToken cancellationToken = default)
    {
        var inquiryResults = await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            // Determine which queries to run based on MemberType
            IQueryable<MasterInquiryItem> combinedQuery;

            if (req.MemberType == 1) // Employee only
            {
                combinedQuery = GetMasterInquiryDemographics(ctx, req);
            }
            else if (req.MemberType == 2) // Beneficiary only
            {
                combinedQuery = GetMasterInquiryBeneficiary(ctx, req);
            }
            else // All or null
            {
                var demographics = GetMasterInquiryDemographics(ctx, req);
                var beneficiary = GetMasterInquiryBeneficiary(ctx, req);
                combinedQuery = demographics.Union(beneficiary);
            }

            var formattedQuery = combinedQuery.Select(x => new MasterInquiryResponseDto
            {
                Id = x.ProfitDetail.Id,
                Ssn = x.ProfitDetail.Ssn.MaskSsn(),
                ProfitYear = x.ProfitDetail.ProfitYear,
                ProfitYearIteration = x.ProfitDetail.ProfitYearIteration,
                DistributionSequence = x.ProfitDetail.DistributionSequence,
                ProfitCodeId = x.ProfitDetail.ProfitCodeId,
                ProfitCodeName = x.ProfitCode.Name,
                Contribution = x.ProfitDetail.CalculateContribution(),
                Earnings = x.ProfitDetail.CalculateEarnings(),
                Forfeiture = x.ProfitDetail.CalculateForfeiture(),
                Payment = x.ProfitDetail.CalculatePayment(),
                MonthToDate = x.ProfitDetail.MonthToDate,
                YearToDate = x.ProfitDetail.YearToDate,
                Remark = x.ProfitDetail.Remark,
                ZeroContributionReasonId = x.ProfitDetail.ZeroContributionReasonId,
                ZeroContributionReasonName =
                    x.ZeroContributionReason != null
                        ? x.ZeroContributionReason.Name
                        : string.Empty,
                FederalTaxes = x.ProfitDetail.FederalTaxes,
                StateTaxes = x.ProfitDetail.StateTaxes,
                TaxCodeId = x.ProfitDetail.TaxCodeId != null ? x.ProfitDetail.TaxCodeId : TaxCode.Constants.Unknown.Id,
                TaxCodeName = x.TaxCode != null ? x.TaxCode.Name : TaxCode.Constants.Unknown.Name,
                CommentTypeId = x.ProfitDetail.CommentTypeId,
                CommentTypeName =
                    x.CommentType != null ? x.CommentType.Name : string.Empty,
                CommentRelatedCheckNumber = x.ProfitDetail.CommentRelatedCheckNumber,
                CommentRelatedState = x.ProfitDetail.CommentRelatedState,
                CommentRelatedOracleHcmId = x.ProfitDetail.CommentRelatedOracleHcmId,
                CommentRelatedPsnSuffix = x.ProfitDetail.CommentRelatedPsnSuffix,
                CommentIsPartialTransaction = x.ProfitDetail.CommentIsPartialTransaction != null ? x.ProfitDetail.CommentIsPartialTransaction : false,
                BadgeNumber = x.Member.BadgeNumber,
                PsnSuffix =  x.Member.PsnSuffix
            });
            var result = await formattedQuery.ToPaginationResultsAsync(req, cancellationToken);
            ISet<int> uniqueSsns = await combinedQuery.Select(q => q.Member.Ssn).ToHashSetAsync(cancellationToken: cancellationToken);
            EmployeeDetails? employeeDetails = null;

            if (uniqueSsns.Count == 1)
            {
                int ssn = uniqueSsns.First();
                short currentYear = req.EndProfitYear ?? (short)DateTime.Today.Year;
                short previousYear = (short)(currentYear - 1);

                employeeDetails = await GetDemographicDetails(ctx, ssn, currentYear, previousYear, cancellationToken) ??
                                  await GetBeneficiaryDetails(ctx, ssn, currentYear, previousYear, cancellationToken);
            }

            return new MasterInquiryWithDetailsResponseDto { InquiryResults = result, EmployeeDetails = employeeDetails };
        });

        return inquiryResults;
    }

    private static IQueryable<MasterInquiryItem> GetMasterInquiryDemographics(IProfitSharingDbContext ctx,
        MasterInquiryRequest req)
    {
        var query = ctx.ProfitDetails
            .Include(pd => pd.ProfitCode)
            .Include(pd => pd.ZeroContributionReason)
            .Include(pd => pd.TaxCode)
            .Include(pd => pd.CommentType)
            .Join(ctx.Demographics,
                pd => pd.Ssn,
                d => d.Ssn,
                (pd, d) => new MasterInquiryItem
                {
                    ProfitDetail = pd,
                    ProfitCode = pd.ProfitCode,
                    ZeroContributionReason = pd.ZeroContributionReason,
                    TaxCode = pd.TaxCode,
                    CommentType = pd.CommentType,
                    Member = new InquiryDemographics
                    {
                        BadgeNumber = d.BadgeNumber,
                        PayFrequencyId = d.PayFrequencyId,
                        Ssn = d.Ssn,
                        PsnSuffix = 0
                    }
                })
            .Where(x => x.Member.PayFrequencyId == PayFrequency.Constants.Weekly);

        if (req.BadgeNumber.HasValue)
        {
            query = query.Where(x => x.Member.BadgeNumber == req.BadgeNumber);
        }

        if (req.StartProfitYear.HasValue)
        {
            query = query.Where(x => x.ProfitDetail.ProfitYear >= req.StartProfitYear);
        }

        if (req.EndProfitYear.HasValue)
        {
            query = query.Where(x => x.ProfitDetail.ProfitYear <= req.EndProfitYear);
        }

        if (req.StartProfitMonth.HasValue)
        {
            query = query.Where(x => x.ProfitDetail.MonthToDate >= req.StartProfitMonth);
        }

        if (req.EndProfitMonth.HasValue)
        {
            query = query.Where(x => x.ProfitDetail.MonthToDate <= req.EndProfitMonth);
        }

        if (req.ProfitCode.HasValue)
        {
            query = query.Where(x => x.ProfitDetail.ProfitCodeId == req.ProfitCode);
        }

        if (req.ContributionAmount.HasValue)
        {
            query = query.Where(x => x.ProfitDetail.Contribution == req.ContributionAmount);
        }

        if (req.EarningsAmount.HasValue)
        {
            query = query.Where(x => x.ProfitDetail.Earnings == req.EarningsAmount);
        }

        if (req.SocialSecurity != null)
        {
            query = query.Where(x => x.ProfitDetail.Ssn == req.SocialSecurity);
        }

        if (req.ForfeitureAmount.HasValue)
        {
            query = query.Where(x =>
                x.ProfitDetail.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id &&
                x.ProfitDetail.Forfeiture == req.ForfeitureAmount);
        }

        if (req.PaymentAmount.HasValue)
        {
            query = query.Where(x =>
                x.ProfitDetail.ProfitCodeId != ProfitCode.Constants.IncomingContributions.Id &&
                x.ProfitDetail.Forfeiture == req.PaymentAmount);
        }

        return query;
    }

    private static IQueryable<MasterInquiryItem> GetMasterInquiryBeneficiary(IProfitSharingDbContext ctx, MasterInquiryRequest req)
    {
        var query = ctx.ProfitDetails
            .Include(pd => pd.ProfitCode)
            .Include(pd => pd.ZeroContributionReason)
            .Include(pd => pd.TaxCode)
            .Include(pd => pd.CommentType)
            .Join(ctx.Beneficiaries.Include(m => m.Contact),
                pd => pd.Ssn,
                b => b.Contact != null ? b.Contact.Ssn : 0,
                (pd, d) => new MasterInquiryItem
                {
                    ProfitDetail = pd,
                    ProfitCode = pd.ProfitCode,
                    ZeroContributionReason = pd.ZeroContributionReason,
                    TaxCode = pd.TaxCode,
                    CommentType = pd.CommentType,
                    Member = new InquiryDemographics
                    {
                        BadgeNumber = d.BadgeNumber, PayFrequencyId = PayFrequency.Constants.Weekly,PsnSuffix = d.PsnSuffix,
                        Ssn = d.Contact!=null?d.Contact.Ssn: 0
                    }
                });

        // Apply filters
        if (req.StartProfitYear.HasValue)
        {
            query = query.Where(x => x.ProfitDetail.ProfitYear >= req.StartProfitYear);
        }

        if (req.EndProfitYear.HasValue)
        {
            query = query.Where(x => x.ProfitDetail.ProfitYear <= req.EndProfitYear);
        }

        if (req.StartProfitMonth.HasValue)
        {
            query = query.Where(x => x.ProfitDetail.MonthToDate >= req.StartProfitMonth);
        }

        if (req.EndProfitMonth.HasValue)
        {
            query = query.Where(x => x.ProfitDetail.MonthToDate <= req.EndProfitMonth);
        }

        if (req.ProfitCode.HasValue)
        {
            query = query.Where(x => x.ProfitDetail.ProfitCodeId == req.ProfitCode);
        }

        if (req.ContributionAmount.HasValue)
        {
            query = query.Where(x => x.ProfitDetail.Contribution == req.ContributionAmount);
        }

        if (req.EarningsAmount.HasValue)
        {
            query = query.Where(x => x.ProfitDetail.Earnings == req.EarningsAmount);
        }

        if (req.SocialSecurity != null)
        {
            query = query.Where(x => x.ProfitDetail.Ssn == req.SocialSecurity);
        }

        if (req.ForfeitureAmount.HasValue)
        {
            query = query.Where(x =>
                x.ProfitDetail.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id &&
                x.ProfitDetail.Forfeiture == req.ForfeitureAmount);
        }

        if (req.PaymentAmount.HasValue)
        {
            query = query.Where(x =>
                x.ProfitDetail.ProfitCodeId != ProfitCode.Constants.IncomingContributions.Id &&
                x.ProfitDetail.Forfeiture == req.PaymentAmount);
        }

        if (req.BadgeNumber.HasValue)
        {
            int.TryParse(req.BadgeNumber.ToString()?[..5], out int badgeNumber);
            int.TryParse(req.BadgeNumber.ToString()?[6..], out int psnSuffix);

            if (badgeNumber > 0)
            {
                query = query.Where(x => x.Member.BadgeNumber == badgeNumber);
            }

            if (psnSuffix > 0)
            {
                query = query.Where(x => x.Member.PsnSuffix == psnSuffix);
            }
        }

        return query;
    }

    private async Task<EmployeeDetails?> GetDemographicDetails(ProfitSharingReadOnlyDbContext ctx,
       int ssn, short currentYear, short previousYear, CancellationToken cancellationToken)
    {
        var memberData = await ctx.Demographics
            .Include(d => d.PayProfits)
            .ThenInclude(pp => pp.Enrollment)
            .Where(d => d.Ssn == ssn)
            .Select(d => new
            {
                d.ContactInfo.FirstName,
                d.ContactInfo.LastName,
                d.Address.City,
                d.Address.State,
                Address = d.Address.Street,
                d.Address.PostalCode,
                d.DateOfBirth,
                d.Ssn,
                d.BadgeNumber,
                d.ReHireDate,
                d.HireDate,
                d.TerminationDate,
                d.StoreNumber,
                DemographicId = d.Id,
                CurrentPayProfit = d.PayProfits
                    .FirstOrDefault(x => x.ProfitYear == currentYear),
                PreviousPayProfit = d.PayProfits
                    .FirstOrDefault(x => x.ProfitYear == currentYear)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (memberData == null)
        {
            return null;
        }

        BalanceEndpointResponse? currentBalance = null;
        BalanceEndpointResponse? previousBalance = null;
        try
        {
            Task<BalanceEndpointResponse?> previousBalanceTask =
                _totalService.GetVestingBalanceForSingleMemberAsync(
                    SearchBy.Ssn, ssn, previousYear, cancellationToken);

            Task<BalanceEndpointResponse?> currentBalanceTask =
                _totalService.GetVestingBalanceForSingleMemberAsync(
                    SearchBy.Ssn, ssn, currentYear, cancellationToken);

            await Task.WhenAll(previousBalanceTask, currentBalanceTask);

            currentBalance = await currentBalanceTask;
            previousBalance = await previousBalanceTask;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to retrieve balances for SSN {SSN}", ssn);
        }

        return new EmployeeDetails
        {
            IsEmployee = true,
            FirstName = memberData.FirstName,
            LastName = memberData.LastName,
            AddressCity = memberData.City!,
            AddressState = memberData.State!,
            Address = memberData.Address,
            AddressZipCode = memberData.PostalCode!,
            DateOfBirth = memberData.DateOfBirth,
            Ssn = memberData.Ssn.MaskSsn(),
            YearToDateProfitSharingHours = memberData.CurrentPayProfit?.CurrentHoursYear ?? 0,
            YearsInPlan = currentBalance?.YearsInPlan ?? (short)0,
            HireDate = memberData.HireDate,
            ReHireDate = memberData.ReHireDate,
            TerminationDate = memberData.TerminationDate,
            StoreNumber = memberData.StoreNumber,
            PercentageVested = currentBalance?.VestingPercent ?? 0,
            ContributionsLastYear = previousBalance is { CurrentBalance: > 0 },
            EnrollmentId = memberData.CurrentPayProfit?.EnrollmentId,
            Enrollment = memberData.CurrentPayProfit?.Enrollment?.Name,
            BadgeNumber = memberData.BadgeNumber,
            BeginPSAmount = (previousBalance?.CurrentBalance ?? 0),
            CurrentPSAmount = (currentBalance?.CurrentBalance ?? 0),
            BeginVestedAmount = (previousBalance?.VestedBalance ?? 0),
            CurrentVestedAmount = (currentBalance?.VestedBalance ?? 0),
            CurrentEtva = memberData.CurrentPayProfit?.Etva ?? 0,
            PreviousEtva = memberData.PreviousPayProfit?.Etva ?? 0,
        };
    }

    private async Task<EmployeeDetails?> GetBeneficiaryDetails(ProfitSharingReadOnlyDbContext ctx,
       int ssn, short currentYear, short previousYear, CancellationToken cancellationToken)
    {
        var memberData = await ctx.Beneficiaries
            .Include(b => b.Contact)
            .Where(b => b.Contact!.Ssn == ssn)
            .Select(b => new
            {
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
            .FirstOrDefaultAsync(cancellationToken);

        if (memberData == null)
        {
            return null;
        }

        BalanceEndpointResponse? currentBalance = null;
        BalanceEndpointResponse? previousBalance = null;
        try
        {
            Task<BalanceEndpointResponse?> previousBalanceTask =
                _totalService.GetVestingBalanceForSingleMemberAsync(
                    SearchBy.Ssn, ssn, previousYear, cancellationToken);

            Task<BalanceEndpointResponse?> currentBalanceTask =
                _totalService.GetVestingBalanceForSingleMemberAsync(
                    SearchBy.Ssn, ssn, currentYear, cancellationToken);

            await Task.WhenAll(previousBalanceTask, currentBalanceTask);

            currentBalance = await currentBalanceTask;
            previousBalance = await previousBalanceTask;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to retrieve balances for SSN {SSN}", ssn);
        }

        return new EmployeeDetails
        {
            IsEmployee = false,
            FirstName = memberData.FirstName,
            LastName = memberData.LastName,
            AddressCity = memberData.City!,
            AddressState = memberData.State!,
            Address = memberData.Address,
            AddressZipCode = memberData.PostalCode!,
            DateOfBirth = memberData.DateOfBirth,
            Ssn = memberData.Ssn.MaskSsn(),
            YearsInPlan = currentBalance?.YearsInPlan ?? (short)0,
            PercentageVested = currentBalance?.VestingPercent ?? 0,
            ContributionsLastYear = previousBalance is { CurrentBalance: > 0 },
            BadgeNumber = memberData.BadgeNumber,
            PsnSuffix = memberData.PsnSuffix,
            BeginPSAmount = (previousBalance?.CurrentBalance ?? 0),
            CurrentPSAmount = (currentBalance?.CurrentBalance ?? 0),
            BeginVestedAmount = (previousBalance?.VestedBalance ?? 0),
            CurrentVestedAmount = (currentBalance?.VestedBalance ?? 0)
        };
    }
}
