using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports;

public class CleanupReportService : ICleanupReportService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ContributionService _contributionService;
    private readonly CalendarService _calendarService;
    private readonly ILogger<CleanupReportService> _logger;

    public CleanupReportService(IProfitSharingDataContextFactory dataContextFactory,
        ContributionService contributionService,
        ILoggerFactory factory,
        CalendarService calendarService)
    {
        _dataContextFactory = dataContextFactory;
        _contributionService = contributionService;
        _calendarService = calendarService;
        _logger = factory.CreateLogger<CleanupReportService>();
        
    }

    public async Task<ReportResponseBase<PayrollDuplicateSsnResponseDto>> GetDuplicateSsNs(ProfitYearRequest req, CancellationToken ct)
    {
        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var dupSsns = await ctx.Demographics.GroupBy(x => x.Ssn).Where(x => x.Count() > 1).Select(x => x.Key).ToListAsync(cancellationToken: ct);

            var rslts = await (from dem in ctx.Demographics
                    join pdJoin in ctx.ProfitDetails on dem.Ssn equals pdJoin.Ssn into demPdJoin
                    from pd in demPdJoin.DefaultIfEmpty()
                    join pp in ctx.PayProfits on dem.OracleHcmId equals pp.OracleHcmId into DemPdPpJoin
                    from DemPdPp in DemPdPpJoin.DefaultIfEmpty()
                    where DemPdPp.ProfitYear == req.ProfitYear && dupSsns.Contains(dem.Ssn)
                    group new { dem, DemPdPp }
                        by new
                        {
                            dem.BadgeNumber,
                            SSN = dem.Ssn,
                            dem.ContactInfo.FullName,
                            dem.Address.Street,
                            dem.Address.City,
                            dem.Address.State,
                            dem.Address.PostalCode,
                            dem.HireDate,
                            dem.TerminationDate,
                            dem.ReHireDate,
                            dem.EmploymentStatusId,
                            dem.StoreNumber,
                            DemPdPp.CurrentHoursYear,
                            DemPdPp.CurrentIncomeYear
                        }
                    into grp
                    select new PayrollDuplicateSsnResponseDto
                    {
                        BadgeNumber = grp.Key.BadgeNumber,
                        Ssn = grp.Key.SSN.MaskSsn(),
                        Name = grp.Key.FullName,
                        Address = new AddressResponseDto
                        {
                            Street = grp.Key.Street,
                            City = grp.Key.City,
                            State = grp.Key.State,
                            PostalCode = grp.Key.PostalCode,
                            CountryIso = Country.Constants.Us
                        },
                        HireDate = grp.Key.HireDate,
                        TerminationDate = grp.Key.TerminationDate,
                        RehireDate = grp.Key.ReHireDate,
                        Status = grp.Key.EmploymentStatusId,
                        StoreNumber = grp.Key.StoreNumber,
                        ProfitSharingRecords = grp.Count(),
                        HoursCurrentYear = grp.Key.CurrentHoursYear ?? 0,
                        IncomeCurrentYear = grp.Key.CurrentIncomeYear ?? 0,
                    }
                ).ToPaginationResultsAsync(req, ct);

            return new ReportResponseBase<PayrollDuplicateSsnResponseDto> { ReportDate = DateTimeOffset.Now, ReportName = "Duplicate SSNs", Response = rslts };
        });
    }

    public async Task<ReportResponseBase<NegativeEtvaForSsNsOnPayProfitResponse>> GetNegativeETVAForSSNsOnPayProfitResponse(ProfitYearRequest req,
        CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request NEGATIVE ETVA FOR SSNs ON PAYPROFIT"))
        {
            var results = await _dataContextFactory.UseReadOnlyContext(c =>
            {
                var ssnUnion = c.Demographics.Select(d => d.Ssn)
                    .Union(c.Beneficiaries
                        .Include(b => b.Contact)
                        .Select(b => b.Contact!.Ssn));

                return c.PayProfits
                    .Include(p => p.Demographic)
                    .Where(p => p.ProfitYear == req.ProfitYear
                                && ssnUnion.Contains(p.Demographic!.Ssn)
                                && p.EarningsEtvaValue < 0)
                    .Select(p => new NegativeEtvaForSsNsOnPayProfitResponse
                    {
                        EmployeeBadge = p.Demographic!.BadgeNumber, EmployeeSsn = p.Demographic.Ssn, EtvaValue = p.EarningsEtvaValue
                    })
                    .OrderBy(p => p.EmployeeBadge)
                    .ToPaginationResultsAsync(req, forceSingleQuery: true, cancellationToken);
            });

            _logger.LogWarning("Returned {Results} records", results.Results.Count());

            return new ReportResponseBase<NegativeEtvaForSsNsOnPayProfitResponse>
            {
                ReportName = "NEGATIVE ETVA FOR SSNs ON PAYPROFIT", ReportDate = DateTimeOffset.Now, Response = results
            };
        }
    }

    public async Task<ReportResponseBase<DemographicBadgesNotInPayProfitResponse>> GetDemographicBadgesNotInPayProfit(PaginationRequestDto req,
        CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request BEGIN DEMOGRAPHIC BADGES NOT IN PAY PROFIT"))
        {
            var results = await _dataContextFactory.UseReadOnlyContext(ctx =>
            {
                var query = from dem in ctx.Demographics
                    where !(from pp in ctx.PayProfits select pp.OracleHcmId).Contains(dem.OracleHcmId)
                    select new DemographicBadgesNotInPayProfitResponse
                    {
                        EmployeeBadge = dem.BadgeNumber,
                        EmployeeSsn = dem.Ssn,
                        EmployeeName = dem.ContactInfo.FullName ?? "",
                        Status = dem.EmploymentStatusId,
                        Store = dem.StoreNumber,
                    };
                return query.ToPaginationResultsAsync(req, forceSingleQuery: true, cancellationToken: cancellationToken);
            });

            _logger.LogInformation("Returned {Results} records", results.Results.Count());

            return new ReportResponseBase<DemographicBadgesNotInPayProfitResponse>
            {
                ReportDate = DateTimeOffset.Now, ReportName = "DEMOGRAPHICS BADGES NOT ON PAYPROFIT", Response = results
            };
        }
    }

    public async Task<ReportResponseBase<NamesMissingCommaResponse>> GetNamesMissingComma(PaginationRequestDto req,
        CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request BEGIN DEMOGRAPHIC BADGES NOT IN PAY PROFIT"))
        {
            var results = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                var query = from dem in ctx.Demographics
#pragma warning disable CA1847
                    where dem.ContactInfo.FullName == null || !dem.ContactInfo.FullName.Contains(",")
#pragma warning restore CA1847
                    select new NamesMissingCommaResponse { EmployeeBadge = dem.BadgeNumber, EmployeeSsn = dem.Ssn, EmployeeName = dem.ContactInfo.FullName ?? "", };
                return await query.ToPaginationResultsAsync(req, forceSingleQuery: true, cancellationToken: cancellationToken);
            });

            _logger.LogInformation("Returned {Results} records", results.Results.Count());

            return new ReportResponseBase<NamesMissingCommaResponse>
            {
                ReportDate = DateTimeOffset.Now, ReportName = "MISSING COMMA IN PY_NAME", Response = results
            };
        }
    }

    public async Task<ReportResponseBase<DuplicateNamesAndBirthdaysResponse>> GetDuplicateNamesAndBirthdays(ProfitYearRequest req,
        CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request BEGIN DUPLICATE NAMES AND BIRTHDAYS"))
        {
            return await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                var dupNameSlashDateOfBirth = (from dem in ctx.Demographics
                    group dem by new { dem.ContactInfo.FullName, dem.DateOfBirth }
                    into g
                    where g.Count() > 1
                    select g.Key.FullName);

                var query = from dem in ctx.Demographics
                    join ppLj in ctx.PayProfits on dem.OracleHcmId equals ppLj.OracleHcmId into tmpPayProfit
                    from pp in tmpPayProfit.DefaultIfEmpty()
                    join pdLj in ctx.ProfitDetails on dem.Ssn equals pdLj.Ssn into tmpProfitDetails
                    from pd in tmpProfitDetails.DefaultIfEmpty()
                    where pp.ProfitYear == req.ProfitYear && dupNameSlashDateOfBirth.Contains(dem.ContactInfo.FullName)
                    group new { dem, pp, pd } by new
                    {
                        dem.BadgeNumber,
                        SSN = dem.Ssn,
                        dem.ContactInfo.FullName,
                        dem.DateOfBirth,
                        dem.Address.Street,
                        dem.Address.City,
                        dem.Address.State,
                        dem.Address.PostalCode,
                        CountryISO = dem.Address.CountryIso,
                        dem.HireDate,
                        dem.TerminationDate,
                        dem.EmploymentStatusId,
                        dem.StoreNumber,
                        PdSsn = pd.Ssn,
                        pp.CurrentHoursYear,
                        pp.CurrentIncomeYear
                    }
                    into g
                    orderby g.Key.FullName, g.Key.DateOfBirth, g.Key.SSN, g.Key.BadgeNumber
                    select new DuplicateNamesAndBirthdaysResponse
                    {
                        BadgeNumber = g.Key.BadgeNumber,
                        Ssn = g.Key.SSN.MaskSsn(),
                        Name = g.Key.FullName,
                        DateOfBirth = g.Key.DateOfBirth,
                        Address = new AddressResponseDto()
                        {
                            City = g.Key.City,
                            State = g.Key.State,
                            Street = g.Key.Street,
                            CountryIso = g.Key.CountryISO,
                            PostalCode = g.Key.PostalCode,
                        },
                        HireDate = g.Key.HireDate,
                        TerminationDate = g.Key.TerminationDate,
                        Status = g.Key.EmploymentStatusId,
                        StoreNumber = g.Key.StoreNumber,
                        Count = g.Count(),
                        HoursCurrentYear = g.Key.CurrentHoursYear,
                        IncomeCurrentYear = g.Key.CurrentIncomeYear
                    };

                var results = await query.ToPaginationResultsAsync(req, cancellationToken: cancellationToken);


                ISet<int> badgeNumbers = results.Results.Select(r => r.BadgeNumber).ToHashSet();
                var dict = await ContributionService.GetContributionYears(ctx, req.ProfitYear, badgeNumbers, cancellationToken);
                var balanceDict = await _contributionService.GetNetBalance(ctx, req.ProfitYear, badgeNumbers, cancellationToken);


                foreach (DuplicateNamesAndBirthdaysResponse dup in results.Results)
                {
                    _ = dict.TryGetValue(dup.BadgeNumber, out byte years);
                    dup.Years = (short)years;

                    balanceDict.TryGetValue(dup.BadgeNumber, out var balance);
                    dup.NetBalance = balance?.TotalEarnings ?? 0;
                }

                return new ReportResponseBase<DuplicateNamesAndBirthdaysResponse>()
                {
                    ReportDate = DateTimeOffset.Now, ReportName = "DUPLICATE NAMES AND BIRTHDAYS", Response = results
                };
            });
        }
    }

    public async Task<ReportResponseBase<DistributionsAndForfeitureResponse>> GetDistributionsAndForfeiture(DistributionsAndForfeituresRequest req,
        CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request BEGIN DISTRIBUTIONS AND FORFEITURES"))
        {
            var distributionProfitCodes = new byte[]
            {
                ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id, ProfitCode.Constants.OutgoingDirectPayments.Id,
                ProfitCode.Constants.Outgoing100PercentVestedPayment.Id
            };

            var validProfitCodes = new byte[]
            {
                ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id, ProfitCode.Constants.OutgoingForfeitures.Id,
                ProfitCode.Constants.OutgoingDirectPayments.Id, ProfitCode.Constants.Outgoing100PercentVestedPayment.Id
            };

            var results = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                var nameAndDobQuery = ctx.Demographics.Select(x => new
                    {
                        x.Ssn,
                        x.ContactInfo.FirstName,
                        x.ContactInfo.LastName,
                        x.DateOfBirth,
                        x.BadgeNumber
                    }).Union(ctx.Beneficiaries.Include(b => b.Contact).Select(x => new
                    {
                        x.Contact!.Ssn,
                        x.Contact.FirstName,
                        x.Contact.LastName,
                        x.Contact.DateOfBirth,
                        BadgeNumber = 0
                    }))
                    .GroupBy(x => x.Ssn)
                    .Select(x => new
                    {
                        Ssn = x.Key,
                        FirstName = x.Max(m => m.FirstName),
                        LastName = x.Max(m => m.LastName),
                        DateOfBirth = x.Max(m => m.DateOfBirth),
                        BadgeNumber = x.Max(m => m.BadgeNumber)
                    });

                var query = from pd in ctx.ProfitDetails
                    join nameAndDob in nameAndDobQuery on pd.Ssn equals nameAndDob.Ssn
                    where pd.ProfitYear == req.ProfitYear &&
                          validProfitCodes.Contains(pd.ProfitCodeId) &&
                          (pd.ProfitCodeId != 9 || (pd.ProfitCodeId == 9 && !pd.IsTransferOut && !pd.IsTransferIn)) &&
                          (req.StartMonth == 0 || pd.MonthToDate >= req.StartMonth) &&
                          (req.EndMonth == 0 || pd.MonthToDate <= req.EndMonth)
                    orderby nameAndDob.LastName, nameAndDob.FirstName
                    select new DistributionsAndForfeitureResponse()
                    {
                        BadgeNumber = nameAndDob.BadgeNumber,
                        EmployeeSsn = pd.Ssn.MaskSsn(),
                        EmployeeName = $"{nameAndDob.LastName}, {nameAndDob.FirstName}",
                        DistributionAmount = distributionProfitCodes.Contains(pd.ProfitCodeId) ? pd.Forfeiture : 0,
                        TaxCode = pd.TaxCodeId,
                        StateTax = pd.StateTaxes,
                        FederalTax = pd.FederalTaxes,
                        ForfeitAmount = pd.ProfitCodeId == 2 ? pd.Forfeiture : 0,
                        LoanDate = pd.MonthToDate > 0 ? new DateOnly(pd.YearToDate, pd.MonthToDate, 1) : null,
                        Age = Convert.ToByte(Math.Floor((DateOnly.FromDateTime(DateTime.Now).DayNumber - nameAndDob.DateOfBirth.DayNumber) / 365.2499))
                    };
                return await query.ToPaginationResultsAsync(req, cancellationToken: cancellationToken);
            });

            _logger.LogInformation("Returned {Results} records", results.Results.Count());

            return new ReportResponseBase<DistributionsAndForfeitureResponse>()
            {
                ReportDate = DateTimeOffset.Now, ReportName = "DISTRIBUTIONS AND FORFEITURES", Response = results
            };
        }
    }

    public async Task<ReportResponseBase<YearEndProfitSharingReportResponse>> GetYearEndProfitSharingReport(YearEndProfitSharingReportRequest req, CancellationToken cancellationToken = default)
    {
        var yearEndDate = (await _calendarService.GetYearStartAndEndAccountingDates(req.ProfitYear, cancellationToken)).YearEndDate;
        var over18BirthDate = yearEndDate.AddYears(-18);
        var rslt = await _dataContextFactory.UseReadOnlyContext(ctx =>
        {
            return ctx.PayProfits
                      .Include(d => d.Demographic)
                      .Where(p => p.ProfitYear == req.ProfitYear) // Get right year
                      .Where(p => p.Demographic!.EmploymentStatusId != EmploymentStatus.Constants.Terminated) //Don't show terminated employees
                      .Where(p => (p.CurrentHoursYear  + p.HoursExecutive) >= 1000) //Employee worked 1000 hrs
                      .Where(p => p.Demographic!.DateOfBirth < over18BirthDate) // Employee must be eighteen
                      .OrderBy(p=>p.Demographic!.ContactInfo.LastName)
                      .ThenBy(p=>p.Demographic!.ContactInfo.FirstName)
                      .Select(x => new YearEndProfitSharingReportResponse()
                      {
                          BadgeNumber = x.Demographic!.BadgeNumber,
                          EmployeeName = $"{x.Demographic!.ContactInfo.LastName}, {x.Demographic.ContactInfo.FirstName}",
                          StoreNumber = x.Demographic!.StoreNumber,
                          EmployeeTypeCode = x.Demographic!.EmploymentTypeId,
                          DateOfBirth = x.Demographic!.DateOfBirth,
                          Age = 0, //Filled out below after materialization
                          EmployeeSsn = x.Demographic!.Ssn.MaskSsn(),
                          Wages = (x.CurrentIncomeYear ?? 0m) + (x.IncomeExecutive),
                          Hours = Math.Floor((x.CurrentHoursYear ?? 0m) + (x.HoursExecutive)),
                          Points = 0, //Filled out below after materialization
                          IsNew = x.EmployeeTypeId == EmployeeType.Constants.NewLastYear,
                          IsUnder21 = false, //Filled out below after materialization
                          EmployeeStatus = x.Demographic!.EmploymentStatusId
                      })
                      .ToPaginationResultsAsync(req, cancellationToken);
        });

        foreach (var item in rslt.Results)
        {
            item.Points = Convert.ToInt16(Math.Round(item.Wages / 100, 0, MidpointRounding.AwayFromZero));
            item.Age = (byte)((yearEndDate.Year - item.DateOfBirth.Year) - (yearEndDate.DayOfYear < item.DateOfBirth.DayOfYear ? 1 : 0));
            if (item.Age < 21)
            {
                item.IsUnder21 = true;
                item.Points = 0;
            }
        }

        return new ReportResponseBase<YearEndProfitSharingReportResponse>
        {
            ReportDate = DateTimeOffset.Now,
            ReportName = $"PROFIT SHARE YEAR END REPORT FOR {req.ProfitYear}",
            Response = rslt
        };
    }
}
