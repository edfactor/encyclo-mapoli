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

namespace Demoulas.ProfitSharing.Services;
public class MasterInquiryService : IMasterInquiryService
{
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


    public async Task<MasterInquiryWithDetailsResponseDto> GetMasterInquiryAsync(MasterInquiryRequest req, CancellationToken cancellationToken = default)
    {
        
        var demographics = await  GetMasterInquiryDemograhicAsync(req, cancellationToken);
        var beneficiary = await GetMasterInquiryBeneficiaryAsync(req, cancellationToken);


        if(req.MemberType.HasValue && req.MemberType == 0) //Member Type "ALL" is selected
        {
            demographics.InquiryResults.AddRange(beneficiary.InquiryResults);
            demographics.TotalRecord += beneficiary.TotalRecord;
        }
             
        
        return new MasterInquiryWithDetailsResponseDto()
        {
            EmployeeDetails = demographics.EmployeeDetails,
            InquiryResults = new PaginatedResponseDto<MasterInquiryResponseDto>(req)
            {
                Results = req.MemberType == 0 ? demographics.InquiryResults: //Member Type = "ALL" Value = 0
                req.MemberType == 1? demographics.InquiryResults: //Member Type = "Employee" Value = 1
                req.MemberType == 2? beneficiary.InquiryResults: //Member Type = "Beneficiary" Value = 2
                new List<MasterInquiryResponseDto>(), //Member Type = "None" Value = 3
                Total = req.MemberType == 0 ? demographics.TotalRecord : //Member Type = "ALL" Value = 0
                req.MemberType == 1 ? demographics.TotalRecord : //Member Type = "Employee" Value = 1
                req.MemberType == 2 ? beneficiary.TotalRecord : //Member Type = "Beneficiary" Value = 2
                0 //Member Type = "None" Value = 3
            }
        };

    }


    private async Task<MasterInquiryResponseWithoutPaginationDto> GetMasterInquiryDemograhicAsync(MasterInquiryRequest req, CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("REQUEST MASTER INQUIRY"))
        {
            var rslt = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                var query = ctx.ProfitDetails
                    .Include(pd => pd.ProfitCode)
                    .Include(pd => pd.ZeroContributionReason)
                    .Include(pd => pd.TaxCode)
                    .Include(pd => pd.CommentType)
                            .Join(ctx.Demographics,
                                pd => pd.Ssn,
                                d => d.Ssn,
                                (pd, d) => new { ProfitDetail = pd, Demographics = d })
                            .Where(x => x.Demographics.PayFrequencyId == PayFrequency.Constants.Weekly);

                if (req.BadgeNumber.HasValue)
                {
                    query = query.Where(x => x.Demographics.BadgeNumber == req.BadgeNumber);
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
                var formattedQuery = query
                .Select(x => new MasterInquiryResponseDto
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
                    ZeroContributionReasonName = x.ProfitDetail.ZeroContributionReason != null ? x.ProfitDetail.ZeroContributionReason.Name : string.Empty,
                    FederalTaxes = x.ProfitDetail.FederalTaxes,
                    StateTaxes = x.ProfitDetail.StateTaxes,
                    TaxCodeId = x.ProfitDetail.TaxCodeId,
                    TaxCodeName = x.ProfitDetail.TaxCode != null ? x.ProfitDetail.TaxCode.Name : string.Empty,
                    CommentTypeId = x.ProfitDetail.CommentTypeId,
                    CommentTypeName = x.ProfitDetail.CommentType != null ? x.ProfitDetail.CommentType.Name : string.Empty,
                    CommentRelatedCheckNumber = x.ProfitDetail.CommentRelatedCheckNumber,
                    CommentRelatedState = x.ProfitDetail.CommentRelatedState,
                    CommentRelatedOracleHcmId = x.ProfitDetail.CommentRelatedOracleHcmId,
                    CommentRelatedPsnSuffix = x.ProfitDetail.CommentRelatedPsnSuffix,
                    CommentIsPartialTransaction = x.ProfitDetail.CommentIsPartialTransaction,
                    BadgeNumber = x.Demographics.BadgeNumber,
                });

                var results = await formattedQuery.ToPaginationResultsAsync(req, cancellationToken);
                    

                ISet<int> uniqueSsns = await query.Select(q => q.Demographics.Ssn).ToHashSetAsync(cancellationToken: cancellationToken);
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
                            Task<BalanceEndpointResponse?> previousBalanceTask = _totalService.GetVestingBalanceForSingleMemberAsync(
                                SearchBy.Ssn, ssn, previousYear, cancellationToken);

                            Task<BalanceEndpointResponse?> currentBalanceTask = _totalService.GetVestingBalanceForSingleMemberAsync(
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

                return new MasterInquiryResponseWithoutPaginationDto
                {
                    EmployeeDetails = employeeDetails,
                    InquiryResults = results.Results.ToList(),
                    TotalRecord =await  formattedQuery.CountAsync()
                };
            });

            return rslt;
        }
    }

    private async Task<MasterInquiryResponseWithoutPaginationDto> GetMasterInquiryBeneficiaryAsync(MasterInquiryRequest req, CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("REQUEST MASTER INQUIRY"))
        {
            var rslt = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                var query = ctx.ProfitDetails
                    .Include(pd => pd.ProfitCode)
                    .Include(pd => pd.ZeroContributionReason)
                    .Include(pd => pd.TaxCode)
                    .Include(pd => pd.CommentType)
                            .Join(ctx.Beneficiaries.Include(m => m.Contact),
                                pd => pd.Ssn,
                                b => b.Contact != null ? b.Contact.Ssn : 0,
                                (pd, b) => new { ProfitDetail = pd, Beneficiary = b });



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
                var formattedQuery = query
                .Select(x => new MasterInquiryResponseDto
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
                    ZeroContributionReasonName = x.ProfitDetail.ZeroContributionReason != null ? x.ProfitDetail.ZeroContributionReason.Name : string.Empty,
                    FederalTaxes = x.ProfitDetail.FederalTaxes,
                    StateTaxes = x.ProfitDetail.StateTaxes,
                    TaxCodeId = x.ProfitDetail.TaxCodeId,
                    TaxCodeName = x.ProfitDetail.TaxCode != null ? x.ProfitDetail.TaxCode.Name : string.Empty,
                    CommentTypeId = x.ProfitDetail.CommentTypeId,
                    CommentTypeName = x.ProfitDetail.CommentType != null ? x.ProfitDetail.CommentType.Name : string.Empty,
                    CommentRelatedCheckNumber = x.ProfitDetail.CommentRelatedCheckNumber,
                    CommentRelatedState = x.ProfitDetail.CommentRelatedState,
                    CommentRelatedOracleHcmId = x.ProfitDetail.CommentRelatedOracleHcmId,
                    CommentRelatedPsnSuffix = x.ProfitDetail.CommentRelatedPsnSuffix,
                    CommentIsPartialTransaction = x.ProfitDetail.CommentIsPartialTransaction,
                    BadgeNumber = 0,
                });

                var results = await formattedQuery.ToPaginationResultsAsync(req, cancellationToken);

                ISet<int> uniqueSsns = await query.Select(q => q.Beneficiary.Contact != null ? q.Beneficiary.Contact.Ssn : 0).ToHashSetAsync(cancellationToken: cancellationToken);
                EmployeeDetails? employeeDetails = null;

                if (uniqueSsns.Count == 1)
                {
                    int ssn = uniqueSsns.First();
                    short currentYear = (short)DateTime.Today.Year;
                    short previousYear = (short)(currentYear - 1);

                    var BeneficiaryData = await ctx.Beneficiaries.Include(b => b.Contact)
                        .Where(d => d.Contact != null && d.Contact.Ssn == ssn)
                        .Select(d => new
                        {
                            FirstName = d.Contact != null ? d.Contact.ContactInfo.FirstName : "",
                            LastName = d.Contact != null ? d.Contact.ContactInfo.LastName : "",
                            City = d.Contact != null ? d.Contact.Address.City : "",
                            State = d.Contact != null ? d.Contact.Address.State : "",
                            Address = d.Contact != null ? d.Contact.Address.Street : "",
                            PostalCode = d.Contact != null ? d.Contact.Address.PostalCode : "",
                            DateOfBirth = d.Contact != null ? d.Contact.DateOfBirth : DateOnly.MaxValue,
                            Ssn = d.Contact != null ? d.Contact.Ssn : 0,
                            BadgeNumber = 0,
                            ReHireDate = DateOnly.MaxValue,
                            HireDate = DateOnly.MaxValue,
                            TerminationDate = DateOnly.MaxValue,
                            StoreNumber = 0,
                            DemographicId = d.Id,
                            CurrentPayProfit = 0,
                            PreviousPayProfit = 0
                        })
                        .FirstOrDefaultAsync(cancellationToken);

                    if (BeneficiaryData != null)
                    {
                        BalanceEndpointResponse? currentBalance = null;
                        BalanceEndpointResponse? previousBalance = null;
                        try
                        {
                            Task<BalanceEndpointResponse?> previousBalanceTask = _totalService.GetVestingBalanceForSingleMemberAsync(
                                SearchBy.Ssn, ssn, previousYear, cancellationToken);

                            Task<BalanceEndpointResponse?> currentBalanceTask = _totalService.GetVestingBalanceForSingleMemberAsync(
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
                            FirstName = BeneficiaryData.FirstName,
                            LastName = BeneficiaryData.LastName,
                            AddressCity = BeneficiaryData.City!,
                            AddressState = BeneficiaryData.State!,
                            Address = BeneficiaryData.Address,
                            AddressZipCode = BeneficiaryData.PostalCode!,
                            DateOfBirth = BeneficiaryData.DateOfBirth,
                            Ssn = BeneficiaryData.Ssn.MaskSsn(),
                            YearToDateProfitSharingHours = 0,
                            YearsInPlan = currentBalance?.YearsInPlan ?? (short)0,
                            HireDate = BeneficiaryData.HireDate,
                            ReHireDate = BeneficiaryData.ReHireDate,
                            TerminationDate = BeneficiaryData.TerminationDate,
                            StoreNumber = 0,
                            PercentageVested = currentBalance?.VestingPercent ?? 0,
                            ContributionsLastYear = previousBalance is { CurrentBalance: > 0 },
                            EnrollmentId = 0,
                            Enrollment = "",
                            BadgeNumber = BeneficiaryData.BadgeNumber,
                            BeginPSAmount = (previousBalance?.CurrentBalance ?? 0),
                            CurrentPSAmount = (currentBalance?.CurrentBalance ?? 0),
                            BeginVestedAmount = (previousBalance?.VestedBalance ?? 0),
                            CurrentVestedAmount = (currentBalance?.VestedBalance ?? 0),
                            CurrentEtva = 0,
                            PreviousEtva = 0,
                        };
                    }
                }

                return new MasterInquiryResponseWithoutPaginationDto
                {
                    EmployeeDetails = employeeDetails,
                    InquiryResults = results.Results.ToList(),
                    TotalRecord = await formattedQuery.CountAsync()
                };
            });

            return rslt;
        }
    }






    
}
