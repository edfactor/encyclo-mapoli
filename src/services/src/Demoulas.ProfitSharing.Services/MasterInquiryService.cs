using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
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
                    Ssn = x.ProfitDetail.Ssn,
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

                _logger.LogInformation("Returned {Results} records", results.Results.Count());

                var uniqueSsns = results.Results.Select(r => r.Ssn).Distinct().ToList();
                EmployeeDetails? employeeDetails = null;

                if (uniqueSsns.Count == 1)
                {
                    int ssn = (int) uniqueSsns[0];
                    short currentYear = (short) DateTime.Today.Year;
                    short previousYear = (short) (currentYear - 1);

                    var previousBalance = await _totalService.GetVestingBalanceForSingleMemberAsync(SearchBy.Ssn, ssn, previousYear, cancellationToken);
                    var currentBalance = await _totalService.GetVestingBalanceForSingleMemberAsync(SearchBy.Ssn, ssn, currentYear, cancellationToken);

                    var demographicData = await ctx.Demographics
                     .Where(d => d.Ssn == uniqueSsns[0])
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
                         d.EmployeeId,
                         d.ReHireDate,
                         d.HireDate,
                         d.TerminationDate,
                         d.StoreNumber,
                         DemographicId = d.Id,
                         LatestPayProfit = d.PayProfits
                             .OrderByDescending(p => p.ProfitYear)
                             .FirstOrDefault()
                     })
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
                            Ssn = demographicData.Ssn,
                            YearToDateProfitSharingHours = demographicData.LatestPayProfit?.CurrentHoursYear ?? 0,
                            YearsInPlan = demographicData.LatestPayProfit?.YearsInPlan ?? 0,
                            HireDate = demographicData.HireDate,
                            ReHireDate = demographicData.ReHireDate,
                            TerminationDate = demographicData.TerminationDate,
                            StoreNumber = demographicData.StoreNumber,
                            PercentageVested = currentBalance?.VestingPercent ?? 0,
                            ContributionsLastYear = previousBalance != null && previousBalance.CurrentBalance > 0,
                            Enrolled = demographicData.LatestPayProfit?.EnrollmentId != 0,
                            EmployeeId = demographicData.EmployeeId.ToString(),
                            BeginPSAmount = (long) (previousBalance?.CurrentBalance ?? 0),
                            CurrentPSAmount = (long) (currentBalance?.CurrentBalance ?? 0),
                            BeginVestedAmount = (long) (previousBalance?.VestedBalance ?? 0),
                            CurrentVestedAmount = (long) (currentBalance?.VestedBalance ?? 0)
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
