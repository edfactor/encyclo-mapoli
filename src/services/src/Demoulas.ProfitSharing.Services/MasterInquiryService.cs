using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
        using (_logger.BeginScope("REQUEST MASTER INQUIRY"))
        {
            var rslt = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                var query = ctx.ProfitDetails
                            .Join(ctx.Demographics,
                                pd => pd.Ssn,
                                d => d.Ssn,
                                (pd, d) => new { ProfitDetail = pd, Demographics = d })
                            .Where(x => x.Demographics.PayFrequencyId == 1);

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
                var results = await query
                .Select(x => new MasterInquiryResponseDto
                {
                    Id = x.ProfitDetail.Id,
                    Ssn = x.ProfitDetail.Ssn.MaskSsn(),
                    ProfitYear = x.ProfitDetail.ProfitYear,
                    ProfitYearIteration = x.ProfitDetail.ProfitYearIteration,
                    DistributionSequence = x.ProfitDetail.DistributionSequence,
                    ProfitCodeId = x.ProfitDetail.ProfitCodeId,
                    Contribution = x.ProfitDetail.Contribution,
                    Earnings = x.ProfitDetail.Earnings,
                    Forfeiture = x.ProfitDetail.Forfeiture,
                    MonthToDate = x.ProfitDetail.MonthToDate,
                    YearToDate = x.ProfitDetail.YearToDate,
                    Remark = x.ProfitDetail.Remark,
                    ZeroContributionReasonId = x.ProfitDetail.ZeroContributionReasonId,
                    FederalTaxes = x.ProfitDetail.FederalTaxes,
                    StateTaxes = x.ProfitDetail.StateTaxes,
                    TaxCodeId = x.ProfitDetail.TaxCodeId,
                    CommentTypeId = x.ProfitDetail.CommentTypeId,
                    CommentRelatedCheckNumber = x.ProfitDetail.CommentRelatedCheckNumber,
                    CommentRelatedState = x.ProfitDetail.CommentRelatedState,
                    CommentRelatedOracleHcmId = x.ProfitDetail.CommentRelatedOracleHcmId,
                    CommentRelatedPsnSuffix = x.ProfitDetail.CommentRelatedPsnSuffix,
                    CommentIsPartialTransaction = x.ProfitDetail.CommentIsPartialTransaction
                })
                .OrderByDescending(x => x.ProfitYear)
                .ToPaginationResultsAsync(req, cancellationToken);

                ISet<int> uniqueSsns = await query.Select(q => q.Demographics.Ssn).ToHashSetAsync(cancellationToken: cancellationToken);
                EmployeeDetails? employeeDetails = null;

                if (uniqueSsns.Count == 1)
                {
                    int ssn = uniqueSsns.First();
                    short currentYear = (short)DateTime.Today.Year;
                    short previousYear = (short)(currentYear - 1);

                    BalanceEndpointResponse? previousBalance = null;
                    BalanceEndpointResponse? currentBalance = null;
                    try
                    {
                        previousBalance = await _totalService.GetVestingBalanceForSingleMemberAsync(
                            SearchBy.Ssn, ssn, previousYear, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to retrieve previous year balance for SSN {SSN}", ssn);
                    }
                    try
                    {
                        currentBalance = await _totalService.GetVestingBalanceForSingleMemberAsync(
                            SearchBy.Ssn, ssn, currentYear, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to retrieve current year balance for SSN {SSN}", ssn);
                    }

                    var maxProfitYear = req.EndProfitYear.HasValue ? req.EndProfitYear : short.MaxValue;

                    var demographicData = await ctx.Demographics
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
                            BadgeNumber = d.BadgeNumber,
                            d.ReHireDate,
                            d.HireDate,
                            d.TerminationDate,
                            d.StoreNumber,
                            DemographicId = d.Id,
                            LatestPayProfit = d.PayProfits
                                .Where(x => x.ProfitYear <= maxProfitYear)
                                .OrderByDescending(p => p.ProfitYear)
                                .Select(p => new
                                {
                                    p.CurrentHoursYear,
                                    p.YearsInPlan,
                                    p.EnrollmentId,
                                    p.EarningsEtvaValue
                                })
                                .FirstOrDefault()
                        })
                        .AsNoTracking()
                        .FirstOrDefaultAsync(cancellationToken);

                    if (demographicData != null)
                    {
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
                            YearToDateProfitSharingHours = demographicData.LatestPayProfit?.CurrentHoursYear ?? 0,
                            YearsInPlan = demographicData.LatestPayProfit?.YearsInPlan ?? 0,
                            HireDate = demographicData.HireDate,
                            ReHireDate = demographicData.ReHireDate,
                            TerminationDate = demographicData.TerminationDate,
                            StoreNumber = demographicData.StoreNumber,
                            PercentageVested = currentBalance?.VestingPercent ?? 0,
                            ContributionsLastYear = previousBalance != null && previousBalance.CurrentBalance > 0,
                            Enrolled = demographicData.LatestPayProfit?.EnrollmentId != 0,
                            BadgeNumber = demographicData.BadgeNumber.ToString(),
                            BeginPSAmount = (long)(previousBalance?.CurrentBalance ?? 0),
                            CurrentPSAmount = (long)(currentBalance?.CurrentBalance ?? 0),
                            BeginVestedAmount = (long)(previousBalance?.VestedBalance ?? 0),
                            CurrentVestedAmount = (long)(currentBalance?.VestedBalance ?? 0)
                        };
                    }
                }

                return new MasterInquiryWithDetailsResponseDto
                {
                    EmployeeDetails = employeeDetails,
                    InquiryResults = results
                };
            });

            return rslt;
        }
    }
}
