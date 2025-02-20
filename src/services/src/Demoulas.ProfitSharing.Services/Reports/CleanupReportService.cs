using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.Util.Extensions;
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

    public Task<ReportResponseBase<PayrollDuplicateSsnResponseDto>> GetDuplicateSsnAsync(PaginationRequestDto req, CancellationToken ct)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            int cutoffYear = DateTime.UtcNow.Year - 5;

            var dupSsns = await ctx.Demographics
                .GroupBy(x => x.Ssn)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToHashSetAsync(ct);

            var rslts = await ctx.Demographics
                .Where(dem => dupSsns.Contains(dem.Ssn))
                .Select(dem => new PayrollDuplicateSsnResponseDto
                {
                    BadgeNumber = dem.BadgeNumber,
                    Ssn = dem.Ssn.MaskSsn(),
                    Name = dem.ContactInfo.FullName,
                    Address = new AddressResponseDto
                    {
                        Street = dem.Address.Street,
                        City = dem.Address.City,
                        State = dem.Address.State,
                        PostalCode = dem.Address.PostalCode,
                        CountryIso = Country.Constants.Us
                    },
                    HireDate = dem.HireDate,
                    TerminationDate = dem.TerminationDate,
                    RehireDate = dem.ReHireDate,
                    Status = dem.EmploymentStatusId,
                    StoreNumber = dem.StoreNumber,
                    ProfitSharingRecords = dem.PayProfits.Count(pp => pp.ProfitYear >= cutoffYear),
                    PayProfits = dem.PayProfits
                        .Where(pp => pp.ProfitYear >= cutoffYear)
                        .OrderByDescending(pp => pp.ProfitYear)
                        .Select(pp => new PayProfitResponseDto
                        {
                            DemographicId = pp.DemographicId,
                            ProfitYear = pp.ProfitYear,
                            CurrentHoursYear = pp.CurrentHoursYear,
                            CurrentIncomeYear = pp.CurrentIncomeYear,
                            WeeksWorkedYear = pp.WeeksWorkedYear,
                            LastUpdate = pp.LastUpdate,
                            PointsEarned = pp.PointsEarned,
                            YearsInPlan = pp.YearsInPlan
                        }).ToList()
                })
                .ToPaginationResultsAsync(req, forceSingleQuery: true, ct);

            return new ReportResponseBase<PayrollDuplicateSsnResponseDto>
            {
                ReportDate = DateTimeOffset.Now,
                ReportName = "Duplicate SSNs",
                Response = rslts
            };
        });
    }


    public async Task<ReportResponseBase<NegativeEtvaForSsNsOnPayProfitResponse>> GetNegativeETVAForSSNsOnPayProfitResponseAsync(ProfitYearRequest req,
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
                                && p.Etva < 0)
                    .Select(p => new NegativeEtvaForSsNsOnPayProfitResponse
                    {
                        BadgeNumber = p.Demographic!.BadgeNumber,
                        Ssn = p.Demographic.Ssn.MaskSsn(),
                        EtvaValue = p.Etva
                    })
                    .OrderBy(p => p.BadgeNumber)
                    .ToPaginationResultsAsync(req, forceSingleQuery: true, cancellationToken);
            });

            _logger.LogWarning("Returned {Results} records", results.Results.Count());

            return new ReportResponseBase<NegativeEtvaForSsNsOnPayProfitResponse>
            {
                ReportName = "NEGATIVE ETVA FOR SSNs ON PAYPROFIT",
                ReportDate = DateTimeOffset.Now,
                Response = results
            };
        }
    }

    public async Task<ReportResponseBase<DemographicBadgesNotInPayProfitResponse>> GetDemographicBadgesNotInPayProfitAsync(PaginationRequestDto req,
        CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request BEGIN DEMOGRAPHIC BADGES NOT IN PAY PROFIT"))
        {
            var results = await _dataContextFactory.UseReadOnlyContext(ctx =>
            {
                var query = from dem in ctx.Demographics
                        .Include(d => d.EmploymentStatus)
                            where !(from pp in ctx.PayProfits select pp.DemographicId).Contains(dem.Id)
                            select new DemographicBadgesNotInPayProfitResponse
                            {
                                BadgeNumber = dem.BadgeNumber,
                                Ssn = dem.Ssn.MaskSsn(),
                                EmployeeName = dem.ContactInfo.FullName ?? "",
                                Status = dem.EmploymentStatusId,
                                StatusName = dem.EmploymentStatus!.Name,
                                Store = dem.StoreNumber,
                            };
                return query.ToPaginationResultsAsync(req, forceSingleQuery: true, cancellationToken: cancellationToken);
            });

            _logger.LogInformation("Returned {Results} records", results.Results.Count());

            return new ReportResponseBase<DemographicBadgesNotInPayProfitResponse>
            {
                ReportDate = DateTimeOffset.Now,
                ReportName = "DEMOGRAPHICS BADGES NOT ON PAYPROFIT",
                Response = results
            };
        }
    }

    public async Task<ReportResponseBase<NamesMissingCommaResponse>> GetNamesMissingCommaAsync(PaginationRequestDto req,
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
                            select new NamesMissingCommaResponse { BadgeNumber = dem.BadgeNumber, Ssn = dem.Ssn.MaskSsn(), EmployeeName = dem.ContactInfo.FullName ?? "", };
                return await query.ToPaginationResultsAsync(req, forceSingleQuery: true, cancellationToken: cancellationToken);
            });

            _logger.LogInformation("Returned {Results} records", results.Results.Count());

            return new ReportResponseBase<NamesMissingCommaResponse>
            {
                ReportDate = DateTimeOffset.Now,
                ReportName = "MISSING COMMA IN PY_NAME",
                Response = results
            };
        }
    }

    public async Task<ReportResponseBase<DuplicateNamesAndBirthdaysResponse>> GetDuplicateNamesAndBirthdaysAsync(ProfitYearRequest req,
        CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request BEGIN DUPLICATE NAMES AND BIRTHDAYS"))
        {
            var dict = new Dictionary<int, byte>();
            var results = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                var dupNameSlashDateOfBirth = (from dem in ctx.Demographics
                                               group dem by new { dem.ContactInfo.FullName, dem.DateOfBirth }
                    into g
                                               where g.Count() > 1
                                               select g.Key.FullName);

                var query = from dem in ctx.Demographics
                            join ppLj in ctx.PayProfits on new { DemographicId = dem.Id, req.ProfitYear } equals new { ppLj.DemographicId, ppLj.ProfitYear } into tmpPayProfit
                            from pp in tmpPayProfit.DefaultIfEmpty()
                            join pdLj in ctx.ProfitDetails on new { dem.Ssn, req.ProfitYear } equals new { pdLj.Ssn, pdLj.ProfitYear } into tmpProfitDetails
                            from pd in tmpProfitDetails.DefaultIfEmpty()
                            where dupNameSlashDateOfBirth.Contains(dem.ContactInfo.FullName)
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
                                PdSsn = pd != null ? pd.Ssn : 0,
                                CurrentHoursYear = pp != null ? pp.CurrentHoursYear : 0,
                                CurrentIncomeYear = pp != null ? pp.CurrentIncomeYear : 0
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

                var rslt = await query.ToPaginationResultsAsync(req, cancellationToken: cancellationToken);

                dict = await (
                    from yis in _totalService.GetYearsOfService(ctx, req.ProfitYear)
                    join d in ctx.Demographics on yis.Ssn equals d.Ssn
                    select new
                    {
                        BadgeNumber = d.BadgeNumber,
                        Years = (byte)yis.Years
                    }
                ).ToDictionaryAsync(x => x.BadgeNumber, x => x.Years, cancellationToken: cancellationToken);

                return rslt;
            });

            ISet<int> badgeNumbers = results.Results.Select(r => r.BadgeNumber).ToHashSet();
            var balanceDict = await _contributionService.GetNetBalance(req.ProfitYear, badgeNumbers, cancellationToken);

            foreach (DuplicateNamesAndBirthdaysResponse dup in results.Results)
            {
                _ = dict.TryGetValue(dup.BadgeNumber, out byte years);
                dup.Years = years;

                balanceDict.TryGetValue(dup.BadgeNumber, out var balance);
                dup.NetBalance = balance?.TotalEarnings ?? 0;
            }

            return new ReportResponseBase<DuplicateNamesAndBirthdaysResponse>()
            {
                ReportDate = DateTimeOffset.Now,
                ReportName = "DUPLICATE NAMES AND BIRTHDAYS",
                Response = results
            };
        }
    }

    public async Task<ReportResponseBase<DistributionsAndForfeitureResponse>> GetDistributionsAndForfeitureAsync(DistributionsAndForfeituresRequest req,
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
                var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
                var nameAndDobQuery = ctx.Demographics
                    .Include(d => d.ContactInfo)
                    .Select(x => new
                    {
                        x.Ssn,
                        x.ContactInfo.FirstName,
                        x.ContactInfo.LastName,
                        x.DateOfBirth,
                        BadgeNumber = x.BadgeNumber
                    }).Union(ctx.Beneficiaries.Include(b => b.Contact).Select(x => new
                    {
                        x.Contact!.Ssn,
                        x.Contact.ContactInfo.FirstName,
                        x.Contact.ContactInfo.LastName,
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

                var transferAndQdroCommentTypes = new List<int>() { CommentType.Constants.TransferIn.Id, CommentType.Constants.TransferOut.Id, CommentType.Constants.QdroIn.Id, CommentType.Constants.QdroOut.Id };

                var query = from pd in ctx.ProfitDetails
                            join nameAndDob in nameAndDobQuery on pd.Ssn equals nameAndDob.Ssn
                            where pd.ProfitYear == req.ProfitYear &&
                                  validProfitCodes.Contains(pd.ProfitCodeId) &&
                                  (pd.ProfitCodeId != ProfitCode.Constants.Outgoing100PercentVestedPayment.Id || (pd.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment.Id && (!pd.CommentTypeId.HasValue || !transferAndQdroCommentTypes.Contains(pd.CommentTypeId.Value)))) &&
                                  (req.StartMonth == 0 || pd.MonthToDate >= req.StartMonth) &&
                                  (req.EndMonth == 0 || pd.MonthToDate <= req.EndMonth)
                            orderby nameAndDob.LastName, nameAndDob.FirstName
                            select new DistributionsAndForfeitureResponse()
                            {
                                BadgeNumber = nameAndDob.BadgeNumber,
                                Ssn = pd.Ssn.MaskSsn(),
                                EmployeeName = $"{nameAndDob.LastName}, {nameAndDob.FirstName}",
                                DistributionAmount = distributionProfitCodes.Contains(pd.ProfitCodeId) ? pd.Forfeiture : 0,
                                TaxCode = pd.TaxCodeId,
                                StateTax = pd.StateTaxes,
                                FederalTax = pd.FederalTaxes,
                                ForfeitAmount = pd.ProfitCodeId == 2 ? pd.Forfeiture : 0,
                                LoanDate = pd.MonthToDate > 0 ? new DateOnly(pd.YearToDate, pd.MonthToDate, 1) : null,
                                Age = (byte)nameAndDob.DateOfBirth.Age(calInfo.FiscalEndDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc))
                            };
                return await query.ToPaginationResultsAsync(req, cancellationToken: cancellationToken);
            });

            _logger.LogInformation("Returned {Results} records", results.Results.Count());

            return new ReportResponseBase<DistributionsAndForfeitureResponse>()
            {
                ReportDate = DateTimeOffset.Now,
                ReportName = "DISTRIBUTIONS AND FORFEITURES",
                Response = results
            };
        }
    }

    public async Task<ReportResponseBase<YearEndProfitSharingReportResponse>> GetYearEndProfitSharingReportAsync(YearEndProfitSharingReportRequest req, CancellationToken cancellationToken = default)
    {
        var response = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
        var over18BirthDate = response.FiscalEndDate.AddYears(-18);

        var rslt = await _dataContextFactory.UseReadOnlyContext(ctx =>
        {
            var qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                .Join(_totalService.GetYearsOfService(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn, (p, tot) => new { pp = p, yip = tot })
                .Select(p => new
                {
                    BadgeNumber = p.pp.Demographic!.BadgeNumber,
                    p.pp.CurrentHoursYear,
                    p.pp.HoursExecutive,
                    p.pp.Demographic!.DateOfBirth,
                    p.pp.Demographic!.EmploymentStatusId,
                    p.pp.Demographic!.TerminationDate,
                    p.pp.Demographic!.Ssn,
                    p.pp.Demographic!.ContactInfo.LastName,
                    p.pp.Demographic!.ContactInfo.FirstName,
                    p.pp.Demographic!.StoreNumber,
                    EmploymentTypeId = p.pp.Demographic!.EmploymentTypeId.ToString(), //There seems to be some sort of issue in the oracle ef provider that struggles with the char type.  It maps this expression
                                                                                      //to: NOT (CAST((BITXOR("s"."EMPLOYMENT_TYPE_ID", N'')) AS NUMBER(1)) )
                                                                                      //Converting to a string appears to fix this issue.
                    p.pp.CurrentIncomeYear,
                    p.pp.IncomeExecutive,
                    p.pp.PointsEarned,
                    p.yip.Years
                });

            if (req.IsYearEnd)
            {
                qry = (
                    from pp in ctx.PayProfits
                    join d in FrozenService.GetDemographicSnapshot(ctx, req.ProfitYear) on pp.DemographicId equals d.Id
                    join t in _totalService.GetYearsOfService(ctx, req.ProfitYear) on d.Ssn equals t.Ssn
                    where pp.ProfitYear == req.ProfitYear
                    select new
                    {
                        BadgeNumber = d.BadgeNumber,
                        pp.CurrentHoursYear,
                        pp.HoursExecutive,
                        d.DateOfBirth,
                        d.EmploymentStatusId,
                        d.TerminationDate,
                        d.Ssn,
                        d.ContactInfo.LastName,
                        d.ContactInfo.FirstName,
                        d.StoreNumber,
                        EmploymentTypeId = d.EmploymentTypeId.ToString(),
                        pp.CurrentIncomeYear,
                        pp.IncomeExecutive,
                        pp.PointsEarned,
                        t.Years
                    }
                );
            }

            if (req.IncludeBeneficiaries)
            {
                qry = from b in ctx.Beneficiaries
                        .Include(b => b.Contact)
                        .ThenInclude(c => c!.ContactInfo)
                      where (!ctx.Demographics.Any(x => x.Ssn == b.Contact!.Ssn)) //Filter out employees who are beneficiaries
                      select new
                      {
                          BadgeNumber = 0,
                          CurrentHoursYear = 0m,
                          HoursExecutive = 0m,
                          b.Contact!.DateOfBirth,
                          EmploymentStatusId = ' ',
                          TerminationDate = (DateOnly?)null,
                          b.Contact!.Ssn,
                          b.Contact!.ContactInfo.LastName,
                          b.Contact!.ContactInfo.FirstName,
                          StoreNumber = (short)0,
                          EmploymentTypeId = " ",
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

            return joinedQry
                      .OrderBy(p => p.pp.LastName)
                      .ThenBy(p => p.pp.FirstName)
                      .Select(x => new YearEndProfitSharingReportResponse()
                      {
                          BadgeNumber = x.pp.BadgeNumber,
                          EmployeeName = $"{x.pp.LastName}, {x.pp.FirstName}",
                          StoreNumber = x.pp.StoreNumber,
                          EmployeeTypeCode = x.pp.EmploymentTypeId[0],
                          DateOfBirth = x.pp.DateOfBirth,
                          Age = 0, //Filled out below after materialization
                          Ssn = x.pp.Ssn.MaskSsn(),
                          Wages = x.pp.CurrentIncomeYear + x.pp.IncomeExecutive,
                          Hours = x.pp.CurrentHoursYear + x.pp.HoursExecutive,
                          Points = Convert.ToInt16(x.pp.PointsEarned),
                          IsNew = x.pp.EmploymentTypeId == EmployeeType.Constants.NewLastYear.ToString(),
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

    public async Task<YearEndProfitSharingReportSummaryResponse> GetYearEndProfitSharingSummaryReportAsync(FrozenProfitYearRequest req, CancellationToken cancellationToken = default)
    {
        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
        return await _dataContextFactory.UseReadOnlyContext(async ctx => 
        {
            var birthday18 = calInfo.FiscalEndDate.AddYears(-18);
            var birthday21 = calInfo.FiscalEndDate.AddYears(-21);
            var nonTerminatedStatuses = new List<char>() { EmploymentStatus.Constants.Active, EmploymentStatus.Constants.Inactive };
            var response = new YearEndProfitSharingReportSummaryResponse() { LineItems = new List<YearEndProfitSharingReportSummaryLineItem>() };

            // AGE 18-20 WITH >= 1000 PS HOURS
            var qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                             .Where(p => nonTerminatedStatuses.Contains(p.Demographic!.EmploymentStatusId) || (p.Demographic!.TerminationDate > calInfo.FiscalEndDate))
                             .Where(x => (x.CurrentHoursYear + x.HoursExecutive) >= 1000 && x.Demographic!.DateOfBirth <= birthday18 && x.Demographic!.DateOfBirth > birthday21)
                             .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn, (pp, tot) => new { pp, tot });

            var lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "Active and Inactive",
                LineItemPrefix = "1",
                LineItemTitle = "AGE 18-20 WITH >= 1000 PS HOURS",
                NumberOfMembers = x.Count(),
                TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                TotalBalance = x.Sum(y => y.tot.Total)
            }).FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null) { response.LineItems.Add(lineItem); }

            // >= AGE 21 WITH >= 1000 PS HOURS
            qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                             .Where(p => nonTerminatedStatuses.Contains(p.Demographic!.EmploymentStatusId) || (p.Demographic!.TerminationDate > calInfo.FiscalEndDate))
                             .Where(x => (x.CurrentHoursYear + x.HoursExecutive) >= 1000 && x.Demographic!.DateOfBirth <= birthday21)
                             .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn, (pp, tot) => new { pp, tot });

            lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "Active and Inactive",
                LineItemPrefix = "2",
                LineItemTitle = ">= AGE 21 WITH >= 1000 PS HOURS",
                NumberOfMembers = x.Count(),
                TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                TotalBalance = x.Sum(y => y.tot.Total)
            }).FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null) { response.LineItems.Add(lineItem); }

            // <  AGE 18
            qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                             .Where(p => nonTerminatedStatuses.Contains(p.Demographic!.EmploymentStatusId) || (p.Demographic!.TerminationDate > calInfo.FiscalEndDate))
                             .Where(x => x.Demographic!.DateOfBirth > birthday18)
                             .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn, (pp, tot) => new { pp, tot });

            lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "Active and Inactive",
                LineItemPrefix = "3",
                LineItemTitle = "<  AGE 18",
                NumberOfMembers = x.Count(),
                TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                TotalBalance = x.Sum(y => y.tot.Total)
            }).FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null) { response.LineItems.Add(lineItem); }

            //>= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT
            qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                             .Where(p => nonTerminatedStatuses.Contains(p.Demographic!.EmploymentStatusId) || (p.Demographic!.TerminationDate > calInfo.FiscalEndDate))
                             .Where(x => (x.CurrentHoursYear + x.HoursExecutive) >= 1000 && x.Demographic!.DateOfBirth < birthday18)
                             .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn, (pp, tot) => new { pp, tot })
                             .Where(x => x.tot.Total > 0);
            
            lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "Active and Inactive",
                LineItemPrefix = "4",
                LineItemTitle = ">= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT",
                NumberOfMembers = x.Count(),
                TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                TotalBalance = x.Sum(y => y.tot.Total)
            })
            .FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null) { response.LineItems.Add(lineItem); }

            //>= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT
            qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                             .Where(p => nonTerminatedStatuses.Contains(p.Demographic!.EmploymentStatusId) || (p.Demographic!.TerminationDate > calInfo.FiscalEndDate))
                             .Where(x => (x.CurrentHoursYear + x.HoursExecutive) >= 1000 && x.Demographic!.DateOfBirth < birthday18)
                             .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn, (pp, tot) => new { pp, tot })
                             .Where(x => x.tot.Total == 0);

            lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "Active and Inactive",
                LineItemPrefix = "5",
                LineItemTitle = ">= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT",
                NumberOfMembers = x.Count(),
                TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                TotalBalance = x.Sum(y => y.tot.Total)
            })
            .FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null) { response.LineItems.Add(lineItem); }

            //Terminated >= AGE 18 WITH >= 1000 PS HOURS 
            qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                             .Where(p => p.Demographic!.EmploymentStatusId == EmploymentStatus.Constants.Terminated && p.Demographic!.TerminationDate <= calInfo.FiscalEndDate && p.Demographic!.TerminationDate >= calInfo.FiscalBeginDate)
                             .Where(x => (x.CurrentHoursYear + x.HoursExecutive) >= 1000 && x.Demographic!.DateOfBirth <= birthday18)
                             .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn, (pp, tot) => new { pp, tot });

            lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = "6",
                LineItemTitle = ">= AGE 18 WITH >= 1000 PS HOURS",
                NumberOfMembers = x.Count(),
                TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                TotalBalance = x.Sum(y => y.tot.Total)
            }).FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null) { response.LineItems.Add(lineItem); }

            //Terminated >= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT
            qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                             .Where(p => p.Demographic!.EmploymentStatusId == EmploymentStatus.Constants.Terminated && p.Demographic!.TerminationDate <= calInfo.FiscalEndDate && p.Demographic!.TerminationDate >= calInfo.FiscalBeginDate)
                             .Where(x => (x.CurrentHoursYear + x.HoursExecutive) < 1000 && x.Demographic!.DateOfBirth <= birthday18)
                             .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn, (pp, tot) => new { pp, tot })
                             .Where(x => x.tot.Total == 0);

            lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = "7",
                LineItemTitle = ">= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT",
                NumberOfMembers = x.Count(),
                TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                TotalBalance = x.Sum(y => y.tot.Total)
            }).FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null) { response.LineItems.Add(lineItem); }

            //Terminated >= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT
            qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                             .Where(p => p.Demographic!.EmploymentStatusId == EmploymentStatus.Constants.Terminated && p.Demographic!.TerminationDate <= calInfo.FiscalEndDate && p.Demographic!.TerminationDate >= calInfo.FiscalBeginDate)
                             .Where(x => (x.CurrentHoursYear + x.HoursExecutive) < 1000 && x.Demographic!.DateOfBirth <= birthday18)
                             .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn, (pp, tot) => new { pp, tot })
                             .Where(x => x.tot.Total != 0);

            lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = "8",
                LineItemTitle = ">= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT",
                NumberOfMembers = x.Count(),
                TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                TotalBalance = x.Sum(y => y.tot.Total)
            }).FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null) { response.LineItems.Add(lineItem); }

            //Terminated <  AGE 18           NO WAGES :   0
            qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                             .Where(p => p.Demographic!.EmploymentStatusId == EmploymentStatus.Constants.Terminated && p.Demographic!.TerminationDate <= calInfo.FiscalEndDate && p.Demographic!.TerminationDate >= calInfo.FiscalBeginDate)
                             .Where(x => (x.CurrentIncomeYear + x.IncomeExecutive) == 0 && x.Demographic!.DateOfBirth > birthday18)
                             .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn, (pp, tot) => new { pp, tot });

            lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = "X",
                LineItemTitle = "<  AGE 18           NO WAGES :   0",
                NumberOfMembers = x.Count(),
                TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                TotalBalance = x.Sum(y => y.tot.Total)
            }).FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null) { response.LineItems.Add(lineItem); }

            var beneQry = ctx.BeneficiaryContacts.Where(bc=>!ctx.Demographics.Any(x=>x.Ssn == bc.Ssn))
                             .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Ssn, x => x.Ssn, (pp, tot) => new { pp, tot });

            lineItem = await beneQry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = "N",
                LineItemTitle = "NON-EMPLOYEE BENEFICIARIES",
                NumberOfMembers = x.Count(),
                TotalWages = 0,
                TotalBalance = x.Sum(y => y.tot.Total)
            }).FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null) { response.LineItems.Add(lineItem); }

            return response;
        });
    }
}
