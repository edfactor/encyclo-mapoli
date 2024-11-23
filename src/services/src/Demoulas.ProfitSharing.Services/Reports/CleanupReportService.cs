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
    private readonly ICalendarService _calendarService;
    private readonly ILogger<CleanupReportService> _logger;
    private readonly TotalService _totalService;

    public CleanupReportService(IProfitSharingDataContextFactory dataContextFactory,
        ContributionService contributionService,
        ILoggerFactory factory,
        ICalendarService calendarService,
        TotalService totalService)
    {
        _dataContextFactory = dataContextFactory;
        _contributionService = contributionService;
        _calendarService = calendarService;
        _totalService = totalService;
        _logger = factory.CreateLogger<CleanupReportService>();
        
    }

    public Task<ReportResponseBase<PayrollDuplicateSsnResponseDto>> GetDuplicateSsNs(ProfitYearRequest req, CancellationToken ct)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var dupSsns = await ctx.Demographics.GroupBy(x => x.Ssn).Where(x => x.Count() > 1).Select(x => x.Key).ToListAsync(cancellationToken: ct);

            var rslts = await (from dem in ctx.Demographics
                    join pdJoin in ctx.ProfitDetails on dem.Ssn equals pdJoin.Ssn into demPdJoin
                    from pd in demPdJoin.DefaultIfEmpty()
                    join pp in ctx.PayProfits on dem.Id equals pp.DemographicId into DemPdPpJoin
                    from DemPdPp in DemPdPpJoin.DefaultIfEmpty()
                    where DemPdPp.ProfitYear == req.ProfitYear && dupSsns.Contains(dem.Ssn)
                    group new { dem, DemPdPp }
                        by new
                        {
                            dem.EmployeeId,
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
                        BadgeNumber = grp.Key.EmployeeId,
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
                        HoursCurrentYear = grp.Key.CurrentHoursYear,
                        IncomeCurrentYear = grp.Key.CurrentIncomeYear,
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
                        EmployeeBadge = p.Demographic!.EmployeeId, EmployeeSsn = p.Demographic.Ssn, EtvaValue = p.EarningsEtvaValue
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
                    where !(from pp in ctx.PayProfits select pp.DemographicId).Contains(dem.Id)
                    select new DemographicBadgesNotInPayProfitResponse
                    {
                        EmployeeBadge = dem.EmployeeId,
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
                    select new NamesMissingCommaResponse { EmployeeBadge = dem.EmployeeId, EmployeeSsn = dem.Ssn, EmployeeName = dem.ContactInfo.FullName ?? "", };
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
            var results = await _dataContextFactory.UseReadOnlyContext(ctx =>
            {
                var dupNameSlashDateOfBirth = (from dem in ctx.Demographics
                    group dem by new { dem.ContactInfo.FullName, dem.DateOfBirth }
                    into g
                    where g.Count() > 1
                    select g.Key.FullName);

                var query = from dem in ctx.Demographics
                    join ppLj in ctx.PayProfits on dem.Id equals ppLj.DemographicId into tmpPayProfit
                    from pp in tmpPayProfit.DefaultIfEmpty()
                    join pdLj in ctx.ProfitDetails on dem.Ssn equals pdLj.Ssn into tmpProfitDetails
                    from pd in tmpProfitDetails.DefaultIfEmpty()
                    where pp.ProfitYear == req.ProfitYear && dupNameSlashDateOfBirth.Contains(dem.ContactInfo.FullName)
                    group new { dem, pp, pd } by new
                    {
                        dem.EmployeeId,
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
                    orderby g.Key.FullName, g.Key.DateOfBirth, g.Key.SSN, g.Key.EmployeeId
                            select new DuplicateNamesAndBirthdaysResponse
                    {
                        BadgeNumber = g.Key.EmployeeId,
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

                return query.ToPaginationResultsAsync(req, cancellationToken: cancellationToken);
            });

            ISet<int> badgeNumbers = results.Results.Select(r => r.BadgeNumber).ToHashSet();
            var dict = await _contributionService.GetContributionYears(badgeNumbers);
            var balanceDict = await _contributionService.GetNetBalance(req.ProfitYear, badgeNumbers, cancellationToken);


            foreach (DuplicateNamesAndBirthdaysResponse dup in results.Results)
            {
                _ = dict.TryGetValue(dup.BadgeNumber, out int years);
                dup.Years = (short)years;

                balanceDict.TryGetValue(dup.BadgeNumber, out var balance);
                dup.NetBalance = balance?.TotalEarnings ?? 0;
            }

            return new ReportResponseBase<DuplicateNamesAndBirthdaysResponse>()
            {
                ReportDate = DateTimeOffset.Now, ReportName = "DUPLICATE NAMES AND BIRTHDAYS", Response = results
            };
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
                        x.EmployeeId
                }).Union(ctx.Beneficiaries.Include(b => b.Contact).Select(x => new
                    {
                        x.Contact!.Ssn,
                        x.Contact.ContactInfo.FirstName,
                        x.Contact.ContactInfo.LastName,
                        x.Contact.DateOfBirth,
                        EmployeeId = 0
                    }))
                    .GroupBy(x => x.Ssn)
                    .Select(x => new
                    {
                        Ssn = x.Key,
                        FirstName = x.Max(m => m.FirstName),
                        LastName = x.Max(m => m.LastName),
                        DateOfBirth = x.Max(m => m.DateOfBirth),
                        BadgeNumber = x.Max(m => m.EmployeeId)
                    });

                var transferAndQdroCommentTypes = new List<int>() { CommentType.Constants.TransferIn.Id, CommentType.Constants.TransferOut.Id, CommentType.Constants.QdroIn.Id, CommentType.Constants.QdroOut.Id };

                var query = from pd in ctx.ProfitDetails
                    join nameAndDob in nameAndDobQuery on pd.Ssn equals nameAndDob.Ssn
                    where pd.ProfitYear == req.ProfitYear &&
                          validProfitCodes.Contains(pd.ProfitCodeId) &&
                          (pd.ProfitCodeId != 9 || (pd.ProfitCodeId == 9 && (!pd.CommentTypeId.HasValue || !transferAndQdroCommentTypes.Contains(pd.CommentTypeId.Value)))) &&
                          (req.StartMonth == 0 || pd.MonthToDate >= req.StartMonth) &&
                          (req.EndMonth == 0 || pd.MonthToDate <= req.EndMonth)
                    orderby nameAndDob.LastName, nameAndDob.FirstName
                    select new DistributionsAndForfeitureResponse()
                    {
                        EmployeeId = nameAndDob.BadgeNumber,
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
        var response = await _calendarService.GetYearStartAndEndAccountingDates(req.ProfitYear, cancellationToken);
        var over18BirthDate = response.FiscalEndDate.AddYears(-18);
        var rslt = await _dataContextFactory.UseReadOnlyContext(ctx =>
        {
            var qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                .Join(_totalService.GetYearsOfService(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn, (p, tot) => new { pp=p, yip = tot })
                .Select(p => new 
                {
                    p.pp.Demographic!.EmployeeId,
                    p.pp.CurrentHoursYear,
                    p.pp.HoursExecutive,
                    p.pp.Demographic!.DateOfBirth,
                    p.pp.Demographic!.EmploymentStatusId,
                    p.pp.Demographic!.TerminationDate,
                    p.pp.Demographic!.Ssn,
                    p.pp.Demographic!.ContactInfo.LastName,
                    p.pp.Demographic!.ContactInfo.FirstName,
                    p.pp.Demographic!.StoreNumber,
                    p.pp.Demographic!.EmploymentTypeId,
                    p.pp.CurrentIncomeYear,
                    p.pp.IncomeExecutive,
                    p.pp.PointsEarned,
                    p.yip.Years
                });

            if (req.IncludeBeneficiaries)
            {
                qry = from b in ctx.Beneficiaries
                      where (!ctx.Demographics.Any(x=>x.Ssn == b.Contact!.Ssn)) //Filter out employees who are beneficiaries
                      select new
                      {
                          EmployeeId = 0,
                          CurrentHoursYear = 0m,
                          HoursExecutive = 0m,
                          b.Contact!.DateOfBirth,
                          EmploymentStatusId = ' ',
                          TerminationDate = (DateOnly?)null,
                          b.Contact!.Ssn,
                          b.Contact!.ContactInfo.LastName,
                          b.Contact!.ContactInfo.FirstName,
                          StoreNumber = (short)0,
                          EmploymentTypeId = ' ',
                          CurrentIncomeYear = 0m,
                          IncomeExecutive = 0m,
                          PointsEarned = (decimal?)null,
                          Years = (short)0
                      };
            }

            if (req.MinimumHoursInclusive.HasValue)
            {
                qry = qry.Where(p => (p.CurrentHoursYear + p.HoursExecutive) >= req.MinimumHoursInclusive.Value);
            }
            if (req.MaximumHoursInclusive.HasValue)
            {
                qry = qry.Where(p => (p.CurrentHoursYear + p.HoursExecutive) <= req.MaximumHoursInclusive.Value);
            }
            if (req.MinimumAgeInclusive.HasValue)
            {
                var minBirthDate = response.FiscalEndDate.AddYears(req.MinimumAgeInclusive.Value * -1);
                qry = qry.Where(p => p.DateOfBirth <= minBirthDate);
            }
            if (req.MaximumAgeInclusive.HasValue)
            {
                var maxBirthDate = response.FiscalEndDate.AddYears((req.MaximumAgeInclusive.Value + 1) * -1).AddDays(1);
                qry = qry.Where(p => p.DateOfBirth >= maxBirthDate);
            }
            if (!req.IncludeBeneficiaries && (!req.IncludeActiveEmployees || !req.IncludeEmployeesTerminatedThisYear || !req.IncludeInactiveEmployees))
            {
                var validStatus = new List<char>();
                if (req.IncludeActiveEmployees)
                {
                    validStatus.Add(EmploymentStatus.Constants.Active);
                }
                if (req.IncludeInactiveEmployees)
                {
                    validStatus.Add(EmploymentStatus.Constants.Inactive);
                }
                if (req.IncludeEmployeesTerminatedThisYear || req.IncludeTerminatedEmployees)
                {
                    validStatus.Add(EmploymentStatus.Constants.Terminated);
                }
                if (req is { IncludeActiveEmployees: true, IncludeEmployeesTerminatedThisYear: false })
                {
                    qry = qry.Where(p => validStatus.Contains(p.EmploymentStatusId) || p.TerminationDate > response.FiscalEndDate);
                } 
                else if (req.IncludeEmployeesTerminatedThisYear && req is { IncludeActiveEmployees: false, IncludeInactiveEmployees: false })
                {
                    qry = qry.Where(p => validStatus.Contains(p.EmploymentStatusId) && p.TerminationDate <= response.FiscalEndDate && p.TerminationDate >= response.FiscalBeginDate);
                }
                else if (req.IncludeTerminatedEmployees && req is { IncludeInactiveEmployees: false, IncludeActiveEmployees: false })
                {
                    qry = qry.Where(p => validStatus.Contains(p.EmploymentStatusId) && p.TerminationDate <= response.FiscalEndDate);
                }
                else
                {
                    qry = qry.Where(p => validStatus.Contains(p.EmploymentStatusId));
                }
                
            }
            
            var joinedQry = qry
                      .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Ssn, x => x.Ssn, (pp, tot) => new { pp, tot });


            if (req is { IncludeEmployeesWithNoPriorProfitSharingAmounts: false, IncludeEmployeesWithPriorProfitSharingAmounts: true })
            {
                joinedQry = joinedQry.Where(jq => jq.tot.Total > 0);
            }
            if (req is { IncludeEmployeesWithNoPriorProfitSharingAmounts: true, IncludeEmployeesWithPriorProfitSharingAmounts: false })
            {
                joinedQry = joinedQry.Where(jq => jq.tot.Total == 0);
            }

            var r = joinedQry.ToListAsync(cancellationToken: cancellationToken);

            return joinedQry
                      .OrderBy(p => p.pp.LastName)
                      .ThenBy(p => p.pp.FirstName)
                      .Select(x => new YearEndProfitSharingReportResponse()
                      {
                          EmployeeId = x.pp.EmployeeId,
                          EmployeeName = $"{x.pp.LastName}, {x.pp.FirstName}",
                          StoreNumber = x.pp.StoreNumber,
                          EmployeeTypeCode = x.pp.EmploymentTypeId,
                          DateOfBirth = x.pp.DateOfBirth,
                          Age = 0, //Filled out below after materialization
                          EmployeeSsn = x.pp.Ssn.MaskSsn(),
                          Wages = x.pp.CurrentIncomeYear + x.pp.IncomeExecutive,
                          Hours = x.pp.CurrentHoursYear + x.pp.HoursExecutive,
                          Points = Convert.ToInt16(x.pp.PointsEarned),
                          IsNew = x.pp.EmploymentTypeId == EmployeeType.Constants.NewLastYear,
                          IsUnder21 = false, //Filled out below after materialization
                          EmployeeStatus = x.pp.EmploymentStatusId,
                          Balance = x.tot.Total,
                          YearsInPlan = x.pp.Years
                      })
                      .ToPaginationResultsAsync(req, cancellationToken);
        });

        foreach (var item in rslt.Results)
        {
            item.Age = (byte)((response.FiscalEndDate.Year - item.DateOfBirth.Year) - (response.FiscalEndDate.DayOfYear < item.DateOfBirth.DayOfYear ? 1 : 0));
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
