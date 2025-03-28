using System.Linq.Dynamic.Core;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using static FastEndpoints.Ep;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Demoulas.ProfitSharing.Services;

public class MasterInquiryService : IMasterInquiryService
{
    // Add this class at the bottom of your file
    private sealed  class MasterInquiryItem
    {
        public ProfitDetail ProfitDetail { get; set; } = null!;
        public InquiryDemographics Member { get; set; } = null!;
    }

    private sealed class InquiryDemographics
    {
        public int BadgeNumber { get; set; }
        public byte PayFrequencyId { get; set; }
        public short PsnSuffix { get; set; }
        public int Ssn { get; set; }
    }


    private readonly IProfitSharingDataContextFactory _dataContextFactory;
#pragma warning disable S125
    private readonly ILogger _logger;
#pragma warning restore S125
    private readonly ITotalService _totalService;

    public MasterInquiryService(
        IProfitSharingDataContextFactory dataContextFactory,
        ITotalService totalService,
        ILoggerFactory loggerFactory
    )
    {
        _dataContextFactory = dataContextFactory;
#pragma warning disable S125
        _totalService = totalService;
#pragma warning restore S125
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
                combinedQuery = GetMasterInquiryBeneficiaryAsync(ctx, req);
            }
            else // All or null
            {
                var demographics = GetMasterInquiryDemographics(ctx, req);
                var beneficiary = GetMasterInquiryBeneficiaryAsync(ctx, req);
                combinedQuery = demographics.Union(beneficiary);
            }
#pragma warning disable CS1963
            var formattedQuery = combinedQuery.Select(x => new MasterInquiryResponseDto
            {
                Id = x.ProfitDetail.Id,
                Ssn = x.ProfitDetail.Ssn.MaskSsn(),
                ProfitYear = x.ProfitDetail.ProfitYear,
                ProfitYearIteration = x.ProfitDetail.ProfitYearIteration,
                DistributionSequence = x.ProfitDetail.DistributionSequence,
                ProfitCodeId = x.ProfitDetail.ProfitCodeId,
                ProfitCodeName = x.ProfitDetail.ProfitCode.Name,
                Contribution = x.ProfitDetail.Contribution,
                Earnings = x.ProfitDetail.Earnings,
                Forfeiture = x.ProfitDetail.Forfeiture,
                MonthToDate = x.ProfitDetail.MonthToDate,
                YearToDate = x.ProfitDetail.YearToDate,
                Remark = x.ProfitDetail.Remark,
                ZeroContributionReasonId = x.ProfitDetail.ZeroContributionReasonId,
                ZeroContributionReasonName =
                    x.ProfitDetail.ZeroContributionReason != null
                        ? x.ProfitDetail.ZeroContributionReason.Name
                        : string.Empty,
                FederalTaxes = x.ProfitDetail.FederalTaxes,
                StateTaxes = x.ProfitDetail.StateTaxes,
                TaxCodeId = x.ProfitDetail.TaxCodeId,
                TaxCodeName = x.ProfitDetail.TaxCode != null ? x.ProfitDetail.TaxCode.Name : string.Empty,
                CommentTypeId = x.ProfitDetail.CommentTypeId,
                CommentTypeName =
                    x.ProfitDetail.CommentType != null ? x.ProfitDetail.CommentType.Name : string.Empty,
                CommentRelatedCheckNumber = x.ProfitDetail.CommentRelatedCheckNumber,
                CommentRelatedState = x.ProfitDetail.CommentRelatedState,
                CommentRelatedOracleHcmId = x.ProfitDetail.CommentRelatedOracleHcmId,
                CommentRelatedPsnSuffix = x.ProfitDetail.CommentRelatedPsnSuffix,
                CommentIsPartialTransaction = x.ProfitDetail.CommentIsPartialTransaction,
                BadgeNumber = x.Member.BadgeNumber,
                PsnSuffix =  x.Member.PsnSuffix
            });
            var result = await formattedQuery.ToPaginationResultsAsync(req, cancellationToken);
            ISet<int> uniqueSsns = await combinedQuery.Select(q => q.Member.Ssn).ToHashSetAsync(cancellationToken: cancellationToken);
            EmployeeDetails? employeeDetails = null;

            if (uniqueSsns.Count == 1)
            {
                int ssn = uniqueSsns.First();
                short currentYear = (short)DateTime.Today.Year;
                short previousYear = (short)(currentYear - 1);

                var demographicData = await ctx.Demographics
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

                if (demographicData != null)
                {
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

                    employeeDetails = new EmployeeDetails
                    {
                        FirstName = demographicData.FirstName,
                        LastName = demographicData.LastName,
                        AddressCity = demographicData.City!,
                        AddressState = demographicData.State!,
                        Address = demographicData.Address,
                        AddressZipCode = demographicData.PostalCode!,
                        DateOfBirth = demographicData.DateOfBirth,
                        Ssn = demographicData.Ssn.MaskSsn(),
                        YearToDateProfitSharingHours = demographicData.CurrentPayProfit?.CurrentHoursYear ?? 0,
                        YearsInPlan = currentBalance?.YearsInPlan ?? (short)0,
                        HireDate = demographicData.HireDate,
                        ReHireDate = demographicData.ReHireDate,
                        TerminationDate = demographicData.TerminationDate,
                        StoreNumber = demographicData.StoreNumber,
                        PercentageVested = currentBalance?.VestingPercent ?? 0,
                        ContributionsLastYear = previousBalance is { CurrentBalance: > 0 },
                        EnrollmentId = demographicData.CurrentPayProfit?.EnrollmentId,
                        Enrollment = demographicData.CurrentPayProfit?.Enrollment?.Name,
                        BadgeNumber = demographicData.BadgeNumber,
                        BeginPSAmount = (previousBalance?.CurrentBalance ?? 0),
                        CurrentPSAmount = (currentBalance?.CurrentBalance ?? 0),
                        BeginVestedAmount = (previousBalance?.VestedBalance ?? 0),
                        CurrentVestedAmount = (currentBalance?.VestedBalance ?? 0),
                        CurrentEtva = demographicData.CurrentPayProfit?.Etva ?? 0,
                        PreviousEtva = demographicData.PreviousPayProfit?.Etva ?? 0,
                    };
                }
            }
#pragma warning restore CS1963
            return new MasterInquiryWithDetailsResponseDto { InquiryResults = result, EmployeeDetails = employeeDetails };
        });

        return inquiryResults;

#pragma warning disable S125
        //if (req.MemberType.HasValue && req.MemberType == 0) //Member Type "ALL" is selected
#pragma warning restore S125
        //{
        //    demographics.InquiryResults.AddRange(beneficiary.InquiryResults);
        //    demographics.TotalRecord += beneficiary.TotalRecord;
        //}


        //return new MasterInquiryWithDetailsResponseDto()
        //{
        //    //EmployeeDetails = demographics.EmployeeDetails,
        //    InquiryResults = new PaginatedResponseDto<MasterInquiryResponseDto>(req)
        //    {
        //        Results = req.MemberType == 0 ? inquiryResults : //Member Type = "ALL" Value = 0
        //            req.MemberType == 1 ? inquiryResults.InquiryResults : //Member Type = "Employee" Value = 1
        //            req.MemberType == 2 ? inquiryResults.InquiryResults : //Member Type = "Beneficiary" Value = 2
        //            new List<MasterInquiryResponseDto>(), //Member Type = "None" Value = 3
        //        Total = req.MemberType == 0 ? inquiryResults.TotalRecord : //Member Type = "ALL" Value = 0
        //            req.MemberType == 1 ? inquiryResults.TotalRecord : //Member Type = "Employee" Value = 1
        //            req.MemberType == 2 ? inquiryResults.TotalRecord : //Member Type = "Beneficiary" Value = 2
        //            0 //Member Type = "None" Value = 3
        //    }
        //};

    }


    //private async Task<MasterInquiryResponseWithoutPaginationDto> GetMasterInquiryDemograhicAsync(
    //    MasterInquiryRequest req, CancellationToken cancellationToken = default)
#pragma warning disable S125
    //{
#pragma warning restore S125
    //    using (_logger.BeginScope("REQUEST MASTER INQUIRY"))
    //    {
    //        var rslt = await _dataContextFactory.UseReadOnlyContext(async ctx =>
    //        {
    //            var query = ctx.ProfitDetails
    //                .Include(pd => pd.ProfitCode)
    //                .Include(pd => pd.ZeroContributionReason)
    //                .Include(pd => pd.TaxCode)
    //                .Include(pd => pd.CommentType)
    //                .Join(ctx.Demographics,
    //                    pd => pd.Ssn,
    //                    d => d.Ssn,
    //                    (pd, d) => new { ProfitDetail = pd, Demographics = d })
    //                .Where(x => x.Demographics.PayFrequencyId == PayFrequency.Constants.Weekly);

    //            if (req.BadgeNumber.HasValue)
    //            {
    //                query = query.Where(x => x.Demographics.BadgeNumber == req.BadgeNumber);
    //            }

    //            if (req.StartProfitYear.HasValue)
    //            {
    //                query = query.Where(x => x.ProfitDetail.ProfitYear >= req.StartProfitYear);
    //            }

    //            if (req.EndProfitYear.HasValue)
    //            {
    //                query = query.Where(x => x.ProfitDetail.ProfitYear <= req.EndProfitYear);
    //            }

    //            if (req.StartProfitMonth.HasValue)
    //            {
    //                query = query.Where(x => x.ProfitDetail.MonthToDate >= req.StartProfitMonth);
    //            }

    //            if (req.EndProfitMonth.HasValue)
    //            {
    //                query = query.Where(x => x.ProfitDetail.MonthToDate <= req.EndProfitMonth);
    //            }

    //            if (req.ProfitCode.HasValue)
    //            {
    //                query = query.Where(x => x.ProfitDetail.ProfitCodeId == req.ProfitCode);
    //            }

    //            if (req.ContributionAmount.HasValue)
    //            {
    //                query = query.Where(x => x.ProfitDetail.Contribution == req.ContributionAmount);
    //            }

    //            if (req.EarningsAmount.HasValue)
    //            {
    //                query = query.Where(x => x.ProfitDetail.Earnings == req.EarningsAmount);
    //            }

    //            if (req.SocialSecurity != null)
    //            {
    //                query = query.Where(x => x.ProfitDetail.Ssn == req.SocialSecurity);
    //            }

    //            if (req.ForfeitureAmount.HasValue)
    //            {
    //                query = query.Where(x =>
    //                    x.ProfitDetail.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id &&
    //                    x.ProfitDetail.Forfeiture == req.ForfeitureAmount);
    //            }

    //            if (req.PaymentAmount.HasValue)
    //            {
    //                query = query.Where(x =>
    //                    x.ProfitDetail.ProfitCodeId != ProfitCode.Constants.IncomingContributions.Id &&
    //                    x.ProfitDetail.Forfeiture == req.PaymentAmount);
    //            }

    //            var formattedQuery = query
    //                .Select(x => new MasterInquiryResponseDto
    //                {
    //                    Id = x.ProfitDetail.Id,
    //                    Ssn = x.ProfitDetail.Ssn.MaskSsn(),
    //                    ProfitYear = x.ProfitDetail.ProfitYear,
    //                    ProfitYearIteration = x.ProfitDetail.ProfitYearIteration,
    //                    DistributionSequence = x.ProfitDetail.DistributionSequence,
    //                    ProfitCodeId = x.ProfitDetail.ProfitCodeId,
    //                    ProfitCodeName = x.ProfitDetail.ProfitCode.Name,
    //                    Contribution = x.ProfitDetail.Contribution,
    //                    Earnings = x.ProfitDetail.Earnings,
    //                    Forfeiture = x.ProfitDetail.Forfeiture,
    //                    MonthToDate = x.ProfitDetail.MonthToDate,
    //                    YearToDate = x.ProfitDetail.YearToDate,
    //                    Remark = x.ProfitDetail.Remark,
    //                    ZeroContributionReasonId = x.ProfitDetail.ZeroContributionReasonId,
    //                    ZeroContributionReasonName =
    //                        x.ProfitDetail.ZeroContributionReason != null
    //                            ? x.ProfitDetail.ZeroContributionReason.Name
    //                            : string.Empty,
    //                    FederalTaxes = x.ProfitDetail.FederalTaxes,
    //                    StateTaxes = x.ProfitDetail.StateTaxes,
    //                    TaxCodeId = x.ProfitDetail.TaxCodeId,
    //                    TaxCodeName = x.ProfitDetail.TaxCode != null ? x.ProfitDetail.TaxCode.Name : string.Empty,
    //                    CommentTypeId = x.ProfitDetail.CommentTypeId,
    //                    CommentTypeName =
    //                        x.ProfitDetail.CommentType != null ? x.ProfitDetail.CommentType.Name : string.Empty,
    //                    CommentRelatedCheckNumber = x.ProfitDetail.CommentRelatedCheckNumber,
    //                    CommentRelatedState = x.ProfitDetail.CommentRelatedState,
    //                    CommentRelatedOracleHcmId = x.ProfitDetail.CommentRelatedOracleHcmId,
    //                    CommentRelatedPsnSuffix = x.ProfitDetail.CommentRelatedPsnSuffix,
    //                    CommentIsPartialTransaction = x.ProfitDetail.CommentIsPartialTransaction,
    //                    BadgeNumber = x.Demographics.BadgeNumber,
    //                });

    //            var results = await formattedQuery.ToPaginationResultsAsync(req, cancellationToken);


    //            ISet<int> uniqueSsns = await query.Select(q => q.Demographics.Ssn)
    //                .ToHashSetAsync(cancellationToken: cancellationToken);
    //            EmployeeDetails? employeeDetails = null;

    //            if (uniqueSsns.Count == 1)
    //            {
    //                int ssn = uniqueSsns.First();
    //                short currentYear = (short)DateTime.Today.Year;
    //                short previousYear = (short)(currentYear - 1);

    //                var demographicData = await ctx.Demographics
    //                    .Include(d => d.PayProfits)
    //                    .ThenInclude(pp => pp.Enrollment)
    //                    .Where(d => d.Ssn == ssn)
    //                    .Select(d => new
    //                    {
    //                        d.ContactInfo.FirstName,
    //                        d.ContactInfo.LastName,
    //                        d.Address.City,
    //                        d.Address.State,
    //                        Address = d.Address.Street,
    //                        d.Address.PostalCode,
    //                        d.DateOfBirth,
    //                        d.Ssn,
    //                        d.BadgeNumber,
    //                        d.ReHireDate,
    //                        d.HireDate,
    //                        d.TerminationDate,
    //                        d.StoreNumber,
    //                        DemographicId = d.Id,
    //                        CurrentPayProfit = d.PayProfits
    //                            .FirstOrDefault(x => x.ProfitYear == currentYear),
    //                        PreviousPayProfit = d.PayProfits
    //                            .FirstOrDefault(x => x.ProfitYear == currentYear)
    //                    })
    //                    .FirstOrDefaultAsync(cancellationToken);

    //                if (demographicData != null)
    //                {
    //                    BalanceEndpointResponse? currentBalance = null;
    //                    BalanceEndpointResponse? previousBalance = null;
    //                    try
    //                    {
    //                        Task<BalanceEndpointResponse?> previousBalanceTask =
    //                            _totalService.GetVestingBalanceForSingleMemberAsync(
    //                                SearchBy.Ssn, ssn, previousYear, cancellationToken);

    //                        Task<BalanceEndpointResponse?> currentBalanceTask =
    //                            _totalService.GetVestingBalanceForSingleMemberAsync(
    //                                SearchBy.Ssn, ssn, currentYear, cancellationToken);

    //                        await Task.WhenAll(previousBalanceTask, currentBalanceTask);

    //                        currentBalance = await currentBalanceTask;
    //                        previousBalance = await previousBalanceTask;
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        _logger.LogWarning(ex, "Failed to retrieve balances for SSN {SSN}", ssn);
    //                    }

    //                    employeeDetails = new EmployeeDetails
    //                    {
    //                        FirstName = demographicData.FirstName,
    //                        LastName = demographicData.LastName,
    //                        AddressCity = demographicData.City!,
    //                        AddressState = demographicData.State!,
    //                        Address = demographicData.Address,
    //                        AddressZipCode = demographicData.PostalCode!,
    //                        DateOfBirth = demographicData.DateOfBirth,
    //                        Ssn = demographicData.Ssn.MaskSsn(),
    //                        YearToDateProfitSharingHours = demographicData.CurrentPayProfit?.CurrentHoursYear ?? 0,
    //                        YearsInPlan = currentBalance?.YearsInPlan ?? (short)0,
    //                        HireDate = demographicData.HireDate,
    //                        ReHireDate = demographicData.ReHireDate,
    //                        TerminationDate = demographicData.TerminationDate,
    //                        StoreNumber = demographicData.StoreNumber,
    //                        PercentageVested = currentBalance?.VestingPercent ?? 0,
    //                        ContributionsLastYear = previousBalance is { CurrentBalance: > 0 },
    //                        EnrollmentId = demographicData.CurrentPayProfit?.EnrollmentId,
    //                        Enrollment = demographicData.CurrentPayProfit?.Enrollment?.Name,
    //                        BadgeNumber = demographicData.BadgeNumber,
    //                        BeginPSAmount = (previousBalance?.CurrentBalance ?? 0),
    //                        CurrentPSAmount = (currentBalance?.CurrentBalance ?? 0),
    //                        BeginVestedAmount = (previousBalance?.VestedBalance ?? 0),
    //                        CurrentVestedAmount = (currentBalance?.VestedBalance ?? 0),
    //                        CurrentEtva = demographicData.CurrentPayProfit?.Etva ?? 0,
    //                        PreviousEtva = demographicData.PreviousPayProfit?.Etva ?? 0,
    //                    };
    //                }
    //            }

    //            return new MasterInquiryResponseWithoutPaginationDto
    //            {
    //                EmployeeDetails = employeeDetails,
    //                InquiryResults = results.Results.ToList(),
    //                TotalRecord = await formattedQuery.CountAsync()
    //            };
    //        });

    //        return rslt;
    //    }
    //}


    private IQueryable<MasterInquiryItem> GetMasterInquiryDemographics(IProfitSharingDbContext ctx,
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

    private IQueryable<MasterInquiryItem> GetMasterInquiryBeneficiaryAsync(IProfitSharingDbContext ctx, MasterInquiryRequest req)
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
            query = query.Where(x => x.Member.BadgeNumber == req.BadgeNumber);
        }

        return query;
    }


}
