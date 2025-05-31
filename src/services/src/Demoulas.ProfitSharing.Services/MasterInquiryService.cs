using System.Runtime.Intrinsics.X86;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Extensions;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Mappers;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services;

public sealed class MasterInquiryService : IMasterInquiryService
{
    #region Private DTO

    private sealed  class MasterInquiryItem
    {
        public required ProfitDetail ProfitDetail { get; init; }
        public required InquiryDemographics Member { get; init; }
        public required ProfitCode ProfitCode { get; init; }
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


    public async Task<MasterInquiryWithDetailsResponseDto> GetMasterInquiryAsync(MasterInquiryRequest req,
        CancellationToken cancellationToken = default)
    {
        var inquiryResults = await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            // Determine which queries to run based on MemberType
            IQueryable<MasterInquiryItem> combinedQuery;

            if (req.MemberType == 1) // Employee only
            {
                combinedQuery = GetMasterInquiryDemographics(ctx);
            }
            else if (req.MemberType == 2) // Beneficiary only
            {
                combinedQuery = GetMasterInquiryBeneficiary(ctx);
            }
            else // All or null
            {
                var demographics = GetMasterInquiryDemographics(ctx);
                var beneficiary = GetMasterInquiryBeneficiary(ctx);
                combinedQuery = demographics.Union(beneficiary);
            }

            combinedQuery = FilterPaymentType(req, combinedQuery);

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
                PsnSuffix =  x.Member.PsnSuffix,
                PayFrequencyId = x.Member.PayFrequencyId,
                TransactionDate = x.TransactionDate,
                CurrentIncomeYear = x.Member.CurrentIncomeYear,
                CurrentHoursYear = x.Member.CurrentHoursYear
            });
            var result = await formattedQuery.ToPaginationResultsAsync(req, cancellationToken);
            ISet<int> uniqueSsns = await combinedQuery.Select(q => q.Member.Ssn).ToHashSetAsync(cancellationToken: cancellationToken);
            MemberDetails? employeeDetails = null;

            if (uniqueSsns.Count == 1)
            {
                int ssn = uniqueSsns.First();
                short currentYear = req.EndProfitYear ?? (short)DateTime.Today.Year;
                short previousYear = (short)(currentYear - 1);

                if (req.MemberType == 1) // Employee only
                {
                    employeeDetails = await GetDemographicDetails(ctx, ssn, currentYear, previousYear, cancellationToken);
                }
                else if (req.MemberType == 2) // Beneficiary only
                {
                    employeeDetails = await GetBeneficiaryDetails(ctx, ssn, currentYear, previousYear, cancellationToken);
                }
                else // All or null
                {
                    employeeDetails = await GetDemographicDetails(ctx, ssn, currentYear, previousYear, cancellationToken) ??
                                      await GetBeneficiaryDetails(ctx, ssn, currentYear, previousYear, cancellationToken);
                }
               
            }

            return new MasterInquiryWithDetailsResponseDto { InquiryResults = result, EmployeeDetails = employeeDetails };
        });

        return inquiryResults;
    }

    public async Task<PaginatedResponseDto<MemberDetails>> GetMembersAsync(MasterInquiryMemberRequest req, CancellationToken cancellationToken = default)
    {
        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            IQueryable<MasterInquiryItem> query = req.MemberType switch
            {
                1 => GetMasterInquiryDemographics(ctx),
                2 => GetMasterInquiryBeneficiary(ctx),
                _ => GetMasterInquiryDemographics(ctx).Union(GetMasterInquiryBeneficiary(ctx))
            };

            if (req.Ssn != 0)
            {
                query = query.Where(x => x.Member.Ssn == req.Ssn);
            }

            var memberDetailsQuery = query.Select(x => new MemberDetails
            {
                FirstName = x.Member.FirstName,
                LastName = x.Member.LastName,
                BadgeNumber = x.Member.BadgeNumber,
                PsnSuffix = x.Member.PsnSuffix,
                Ssn = x.Member.Ssn.ToString(),
                
            });

            return await memberDetailsQuery.ToPaginationResultsAsync(req, cancellationToken);
        });
    }

    public async Task<MemberDetails?> GetMemberAsync(MasterInquiryMemberRequest req, CancellationToken cancellationToken = default)
    {
        short currentYear = req.ProfitYear;
        short previousYear = (short)(currentYear - 1);
        return await _dataContextFactory.UseReadOnlyContext(ctx =>
        {
            return req.MemberType switch
            {
                1 => GetDemographicDetails(ctx, req.Ssn, currentYear, previousYear, cancellationToken),
                2 => GetBeneficiaryDetails(ctx, req.Ssn, currentYear, previousYear, cancellationToken),
                _ => throw new ValidationException("Invalid MemberType provided")
            };
        });
    }

    public Task<PaginatedResponseDto<MasterInquiryResponseDto>> GetMemberProfitDetails(MasterInquiryMemberDetailsRequest req, CancellationToken cancellationToken = default)
    {
        return _dataContextFactory.UseReadOnlyContext(ctx =>
        {
            IQueryable<MasterInquiryItem> query = req.MemberType switch
            {
                1 => GetMasterInquiryDemographics(ctx),
                2 => GetMasterInquiryBeneficiary(ctx),
                _ => GetMasterInquiryDemographics(ctx).Union(GetMasterInquiryBeneficiary(ctx))
            };

            if (req.Id.HasValue)
            {
                query = query.Where(x => x.ProfitDetail.Id == req.Id.Value);
            }

            var formattedQuery = query.Select(x => new MasterInquiryResponseDto
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
                ZeroContributionReasonName = x.ZeroContributionReason != null ? x.ZeroContributionReason.Name : string.Empty,
                FederalTaxes = x.ProfitDetail.FederalTaxes,
                StateTaxes = x.ProfitDetail.StateTaxes,
                TaxCodeId = x.ProfitDetail.TaxCodeId != null ? x.ProfitDetail.TaxCodeId : TaxCode.Constants.Unknown.Id,
                TaxCodeName = x.TaxCode != null ? x.TaxCode.Name : TaxCode.Constants.Unknown.Name,
                CommentTypeId = x.ProfitDetail.CommentTypeId,
                CommentTypeName = x.CommentType != null ? x.CommentType.Name : string.Empty,
                CommentRelatedCheckNumber = x.ProfitDetail.CommentRelatedCheckNumber,
                CommentRelatedState = x.ProfitDetail.CommentRelatedState,
                CommentRelatedOracleHcmId = x.ProfitDetail.CommentRelatedOracleHcmId,
                CommentRelatedPsnSuffix = x.ProfitDetail.CommentRelatedPsnSuffix,
                CommentIsPartialTransaction = x.ProfitDetail.CommentIsPartialTransaction != null ? x.ProfitDetail.CommentIsPartialTransaction : false,
                BadgeNumber = x.Member.BadgeNumber,
                PsnSuffix = x.Member.PsnSuffix,
                PayFrequencyId = x.Member.PayFrequencyId,
                TransactionDate = x.TransactionDate,
                CurrentIncomeYear = x.Member.CurrentIncomeYear,
                CurrentHoursYear = x.Member.CurrentHoursYear
            });

            return formattedQuery.ToPaginationResultsAsync(req, cancellationToken);
        });
    }

    private static IQueryable<MasterInquiryItem> GetMasterInquiryDemographics(IProfitSharingDbContext ctx)
    {
        var query = ctx.ProfitDetails
            .Include(pd => pd.ProfitCode)
            .Include(pd => pd.ZeroContributionReason)
            .Include(pd => pd.TaxCode)
            .Include(pd => pd.CommentType)
            .Join(ctx.Demographics
                    .Include(d=> d.PayProfits),
                pd => pd.Ssn,
                d => d.Ssn,
                (pd, d) => new MasterInquiryItem
                {
                    ProfitDetail = pd,
                    ProfitCode = pd.ProfitCode,
                    ZeroContributionReason = pd.ZeroContributionReason,
                    TaxCode = pd.TaxCode,
                    CommentType = pd.CommentType,
                    TransactionDate = pd.TransactionDate,
                    Member = new InquiryDemographics
                    {
                        BadgeNumber = d.BadgeNumber,
                        FullName = d.ContactInfo.FullName != null ? d.ContactInfo.FullName : d.ContactInfo.LastName,
                        FirstName = d.ContactInfo.FirstName,
                        LastName = d.ContactInfo.LastName,
                        PayFrequencyId = d.PayFrequencyId,
                        Ssn = d.Ssn,
                        PsnSuffix = 0,
                        CurrentIncomeYear = d.PayProfits.Where(x=> x.ProfitYear == pd.ProfitYear)
                            .Select(x => x.CurrentIncomeYear)
                            .FirstOrDefault(),
                        CurrentHoursYear = d.PayProfits.Where(x => x.ProfitYear == pd.ProfitYear)
                            .Select(x => x.CurrentHoursYear)
                            .FirstOrDefault()
                    }
                })
            .Where(x => x.Member.PayFrequencyId == PayFrequency.Constants.Weekly);

        return query;
    }

    private static IQueryable<MasterInquiryItem> GetMasterInquiryBeneficiary(IProfitSharingDbContext ctx)
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
                    TransactionDate = pd.TransactionDate,
                    Member = new InquiryDemographics
                    {
                        BadgeNumber = d.BadgeNumber,
                        FullName = d.Contact!.ContactInfo.FullName != null ? d.Contact.ContactInfo.FullName : d.Contact.ContactInfo.LastName,
                        FirstName = d.Contact!.ContactInfo.FirstName,
                        LastName = d.Contact!.ContactInfo.LastName,
                        PayFrequencyId = PayFrequency.Constants.Weekly,
                        PsnSuffix = d.PsnSuffix,
                        Ssn = d.Contact!=null?d.Contact.Ssn: 0,
                        CurrentIncomeYear = 0,
                        CurrentHoursYear = 0
                    }
                });

        return query;
    }

    private async Task<MemberDetails?> GetDemographicDetails(ProfitSharingReadOnlyDbContext ctx,
       int ssn, short currentYear, short previousYear, CancellationToken cancellationToken)
    {
        var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
        var memberData = await demographics
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
                EmploymentStatusId = d.EmploymentStatusId,
                EmploymentStatus = d.EmploymentStatus,
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

        var missives = await _missiveService.DetermineMissivesForSsn(ssn, currentYear, cancellationToken);

        return new MemberDetails
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
            EmploymentStatus = memberData.EmploymentStatus?.Name,
            Missives = missives
        };
    }

    private async Task<MemberDetails?> GetBeneficiaryDetails(ProfitSharingReadOnlyDbContext ctx,
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

        return new MemberDetails
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
            YearsInPlan = 0,
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

    private static IQueryable<MasterInquiryItem> FilterPaymentType(MasterInquiryRequest req, IQueryable<MasterInquiryItem> query)
    {
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

        if (req.Ssn != 0)
        {
            query = query.Where(x => x.ProfitDetail.Ssn == req.Ssn);
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

        if (!string.IsNullOrWhiteSpace(req.Name))
        {
            var pattern = $"%{req.Name.ToUpperInvariant()}%";
            query = query.Where(x => EF.Functions.Like(x.Member.FullName.ToUpper(), pattern));
        }

        if (req.PaymentType.HasValue)
        {
            switch (req.PaymentType)
            {
                case 1: // Hardship/Distribution
                    List<byte?> array = [CommentType.Constants.Hardship.Id, CommentType.Constants.Distribution.Id];
                    query = query.Where(x => array.Contains(x.ProfitDetail.CommentTypeId));
                    break;
                case 2: // payoffs
                    array = [CommentType.Constants.Payoff.Id, CommentType.Constants.Forfeit.Id];
                    query = query.Where(x => array.Contains(x.ProfitDetail.CommentTypeId));
                    break;
                case 3: // rollovers
                    array = [CommentType.Constants.Rollover.Id, CommentType.Constants.RothIra.Id];
                    query = query.Where(x => array.Contains(x.ProfitDetail.CommentTypeId));
                    break;
            }
        }

        return query;
    }
}
