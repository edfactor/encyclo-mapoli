using Demoulas.Common.Contracts.Request;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports;
public class YearEndService : IYearEndService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ILogger<YearEndService> _logger;

    public YearEndService(IProfitSharingDataContextFactory dataContextFactory, 
        ILoggerFactory factory)
    {
        _dataContextFactory = dataContextFactory;
        _logger = factory.CreateLogger<YearEndService>();
    }

    public async Task<ReportResponseBase<PayrollDuplicateSSNResponseDto>> GetDuplicateSSNs(PaginationRequestDto req, CancellationToken ct)
    {
        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var dupSsns = await ctx.Demographics.GroupBy(x => x.SSN).Where(x => x.Count() > 1).Select(x => x.Key).ToListAsync(cancellationToken: ct);

            var rslts = await (from dem in ctx.Demographics
                    join pdJoin in ctx.ProfitDetails on dem.SSN equals pdJoin.SSN into demPdJoin
                    from pd in demPdJoin.DefaultIfEmpty()
                    join pp in ctx.PayProfits on dem.SSN equals pp.SSN  into DemPdPpJoin
                    from DemPdPp in DemPdPpJoin.DefaultIfEmpty()
                    where dupSsns.Contains(dem.SSN)
                    group new { dem, DemPdPp }
                        by new
                        {
                            dem.BadgeNumber,
                            dem.SSN,
                            dem.FullName,
                            dem.Address.Street,
                            dem.Address.City,
                            dem.Address.State,
                            dem.Address.PostalCode,
                            dem.HireDate,
                            dem.TerminationDate,
                            dem.ReHireDate,
                            dem.EmploymentStatusId,
                            dem.StoreNumber,
                            DemPdPp.HoursCurrentYear,
                            DemPdPp.EarningsCurrentYear
                        }
                    into grp
                    select new PayrollDuplicateSSNResponseDto
                    {
                        BadgeNumber = grp.Key.BadgeNumber,
                        SSN = grp.Key.SSN,
                        Name = grp.Key.FullName,
                        Address = new AddressResponseDto
                        {
                            Street = grp.Key.Street,
                            City = grp.Key.City,
                            State = grp.Key.State,
                            PostalCode = grp.Key.PostalCode,
                            CountryISO = Constants.US
                        },
                        HireDate = grp.Key.HireDate,
                        TerminationDate = grp.Key.TerminationDate,
                        RehireDate = grp.Key.ReHireDate,
                        Status = grp.Key.EmploymentStatusId,
                        StoreNumber = grp.Key.StoreNumber,
                        ProfitSharingRecords = grp.Count(),
                        HoursCurrentYear = grp.Key.HoursCurrentYear ?? 0,
                        EarningsCurrentYear = grp.Key.EarningsCurrentYear ?? 0,
                    }
                ).ToPaginationResultsAsync(req, forceSingleQuery: true, ct);

            return new ReportResponseBase<PayrollDuplicateSSNResponseDto>
            {
                ReportDate = DateTimeOffset.Now, ReportName = "Duplicate SSNs", Response = rslts
            };
        });
    }

    public async Task<ReportResponseBase<PayProfitBadgesNotInDemographicsResponse>> GetPayProfitBadgesNotInDemographics(PaginationRequestDto req, CancellationToken ct = default)
    {
        var results = await _dataContextFactory.UseReadOnlyContext(ctx =>
        {
            return (from pp in ctx.PayProfits
             join dem in ctx.Demographics on pp.PSN equals dem.BadgeNumber into demTmp
             from dem in demTmp.DefaultIfEmpty()
             where dem == null
             orderby pp.PSN, pp.SSN
             select new PayProfitBadgesNotInDemographicsResponse { EmployeeBadge = pp.PSN, EmployeeSSN = pp.SSN }
            ).ToPaginationResultsAsync(req, forceSingleQuery: true, ct);
        });

        return new ReportResponseBase<PayProfitBadgesNotInDemographicsResponse>
        {
            ReportName = "Payprofit Badges not in Demographics",
            ReportDate = DateTimeOffset.Now,
            Response = results
        };
    }

    public async Task<ReportResponseBase<NegativeETVAForSSNsOnPayProfitResponse>> GetNegativeETVAForSSNsOnPayProfitResponse(PaginationRequestDto req, CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request NEGATIVE ETVA FOR SSNs ON PAYPROFIT"))
        {
            var results = await _dataContextFactory.UseReadOnlyContext(c =>
            {
                var ssnUnion = c.Demographics.Select(d => d.SSN).Union(c.Beneficiaries.Select(b => b.SSN));

                return c.PayProfits
                    .Where(p => ssnUnion.Contains(p.SSN) && p.EarningsEtvaValue < 0)
                    .Select(p => new NegativeETVAForSSNsOnPayProfitResponse
                    {
                        EmployeeBadge = p.PSN, EmployeeSSN = p.SSN, EtvaValue = p.EarningsEtvaValue
                    })
                    .OrderBy(p => p.EmployeeBadge)
                    .ToPaginationResultsAsync(req, forceSingleQuery: true, cancellationToken);
            });

            _logger.LogWarning("Returned {results} records", results.Results.Count());

            return new ReportResponseBase<NegativeETVAForSSNsOnPayProfitResponse>
            {
                ReportName = "NEGATIVE ETVA FOR SSNs ON PAYPROFIT", ReportDate = DateTimeOffset.Now, Response = results
            };
        }
    }

    public async Task<ReportResponseBase<MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto>> GetMismatchedSsnsPayprofitAndDemographicsOnSameBadge(PaginationRequestDto req, CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request MISMATCHED SSNs PAYPROFIT AND DEMO ON SAME BADGE"))
        {
            var results = await _dataContextFactory.UseReadOnlyContext(c =>
            {
                var query = from demographic in c.Demographics
                    join payProfit in c.PayProfits
                        on demographic.BadgeNumber equals payProfit.PSN
                            where payProfit.SSN != demographic.SSN
                    orderby demographic.BadgeNumber, demographic.SSN, payProfit.SSN
                    select new MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto
                    {
                        Name = demographic.FullName ?? $"{demographic.FirstName} {demographic.LastName}",
                        EmployeeBadge = demographic.BadgeNumber,
                        EmployeeSSN = demographic.SSN,
                        PayProfitSSN = payProfit.SSN,
                       Store = demographic.StoreNumber,
                       Status = demographic.EmploymentStatusId
                    };

                return query.ToPaginationResultsAsync(req, forceSingleQuery: true, cancellationToken);
            });

            _logger.LogWarning("Returned {results} records", results.Results.Count());

            return new ReportResponseBase<MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto>
            {
                ReportName = "MISMATCHED SSNs PAYPROFIT AND DEMO ON SAME BADGE",
                ReportDate = DateTimeOffset.Now,
                Response = results
            };
        }
    }

    public async Task<ReportResponseBase<PayrollDuplicateSsnsOnPayprofitResponseDto>> GetPayrollDuplicateSsnsOnPayprofit(PaginationRequestDto req, CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request PAYROLL DUPLICATE SSNs ON PAYPROFIT"))
        {
            var results = await _dataContextFactory.UseReadOnlyContext(context =>
            {
                var query = from payProfit in context.PayProfits
                            join demographics in context.Demographics
                                on payProfit.PSN equals demographics.BadgeNumber into demGroup
                            from demographics in demGroup.DefaultIfEmpty()
                            join profitDetail in context.ProfitDetails
                                on payProfit.SSN equals profitDetail.SSN into detGroup
                            from profitDetail in detGroup.DefaultIfEmpty()
                            where context.PayProfits
                                .GroupBy(p => p.SSN)
                                .Where(g => g.Count() > 1)
                                .Select(g => g.Key)
                                .Contains(payProfit.SSN)
                            group new { payProfit, demographics, profitDetail } by new
                            {
                                BadgeNumber=payProfit.PSN,
                                payProfit.SSN,
                                demographics.FullName,
                                demographics.HireDate,
                                demographics.TerminationDate,
                                demographics.ReHireDate,
                                demographics.EmploymentStatusId,
                                demographics.StoreNumber,
                                demographics.Address.Street,
                                demographics.Address.Street2,
                                demographics.Address.City,
                                demographics.Address.State,
                                demographics.Address.PostalCode,
                                demographics.Address.CountryISO,
                                demographics.ContactInfo.EmailAddress,
                                demographics.ContactInfo.PhoneNumber,
                                demographics.ContactInfo.MobileNumber,
                                payProfit.EarningsCurrentYear
                            } into g
                            orderby g.Key.SSN, g.Key.BadgeNumber
                            select new PayrollDuplicateSsnsOnPayprofitResponseDto
                            {
                                Count = g.Count(),
                                EmployeeBadge = g.Key.BadgeNumber,
                                EmployeeSSN = g.Key.SSN,
                                Name = g.Key.FullName,
                                HireDate = g.Key.HireDate,
                                TermDate = g.Key.TerminationDate,
                                RehireDate = g.Key.ReHireDate,
                                Status = g.Key.EmploymentStatusId,
                                Store = g.Key.StoreNumber,
                                EarningsCurrentYear = g.Key.EarningsCurrentYear ?? 0,
                                ContactInfo = new ContactInfoResponseDto
                                {
                                    EmailAddress = g.Key.EmailAddress,
                                    MobileNumber = g.Key.MobileNumber,
                                    PhoneNumber = g.Key.PhoneNumber
                                },
                                Address = new AddressResponseDto
                                {
                                    Street = g.Key.Street,
                                    Street2 = g.Key.Street2,
                                    City = g.Key.City,
                                    State = g.Key.State,
                                    PostalCode = g.Key.PostalCode,
                                    CountryISO = g.Key.CountryISO
                                }
                            };

                return query.ToPaginationResultsAsync(req, forceSingleQuery: true, cancellationToken: cancellationToken);
            });

            _logger.LogWarning("Returned {results} records", results.Results.Count());

            return new ReportResponseBase<PayrollDuplicateSsnsOnPayprofitResponseDto>
            {
                ReportName = "PAYROLL DUPLICATE SSNs ON PAYPROFIT",
                ReportDate = DateTimeOffset.Now,
                Response = results
            };
        }
    }

    public async Task<ReportResponseBase<DemographicBadgesNotInPayProfitResponse>> GetDemographicBadgesNotInPayProfit(PaginationRequestDto req, CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request BEGIN DEMOGRAPHIC BADGES NOT IN PAY PROFIT"))
        {
            var results = await _dataContextFactory.UseReadOnlyContext(ctx =>
            {
                var query = from dem in ctx.Demographics
                            where !(from pp in ctx.PayProfits select pp.PSN).Contains(dem.BadgeNumber)
                            select new DemographicBadgesNotInPayProfitResponse
                            {
                                EmployeeBadge = dem.BadgeNumber,
                                EmployeeSSN = dem.SSN,
                                EmployeeName = dem.FullName ?? "",
                                Status = dem.EmploymentStatusId,
                                Store = dem.StoreNumber,
                            };
                return query.ToPaginationResultsAsync(req, forceSingleQuery:true, cancellationToken: cancellationToken);
            });

            _logger.LogInformation("Returned {results} records", results.Results.Count());

            return new ReportResponseBase<DemographicBadgesNotInPayProfitResponse>
            {
                ReportDate = DateTimeOffset.Now,
                ReportName = "DEMOGRAPHICS BADGES NOT ON PAYPROFIT",
                Response = results
            };
        }
    }

    public async Task<ReportResponseBase<NamesMissingCommaResponse>> GetNamesMissingComma(PaginationRequestDto req, CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request BEGIN DEMOGRAPHIC BADGES NOT IN PAY PROFIT"))
        {
            var results = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                var query = from dem in ctx.Demographics
                            where dem.FullName == null || !dem.FullName.Contains(",")
                            select new NamesMissingCommaResponse
                            {
                                EmployeeBadge = dem.BadgeNumber,
                                EmployeeSSN = dem.SSN,
                                EmployeeName = dem.FullName ?? "",
                            };
                return await query.ToPaginationResultsAsync(req, forceSingleQuery: true, cancellationToken: cancellationToken);
            });

            _logger.LogInformation("Returned {results} records", results.Results.Count());

            return new ReportResponseBase<NamesMissingCommaResponse>
            {
                ReportDate = DateTimeOffset.Now,
                ReportName = "MISSING COMMA IN PY_NAME",
                Response = results
            };
        }
    }

    public async Task<ReportResponseBase<DuplicateNamesAndBirthdaysResponse>> GetDuplicateNamesAndBirthdays(PaginationRequestDto req, CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request BEGIN DUPLICATE NAMES AND BIRTHDAYS"))
        {
            var results = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                var dupNameSlashDateOfBirth = await (from dem in ctx.Demographics
                                                 group dem by new { dem.FullName, dem.DateOfBirth } into g
                                                 where g.Count() > 1
                                                 select g.Key.FullName).ToListAsync();

                var query = from dem in ctx.Demographics
                            join ppLj in ctx.PayProfits on dem.BadgeNumber equals ppLj.PSN into tmpPayProfit
                            from pp in tmpPayProfit.DefaultIfEmpty()
                            join pdLj in ctx.ProfitDetails on dem.SSN equals pdLj.SSN into tmpProfitDetails
                            from pd in tmpProfitDetails.DefaultIfEmpty()
                            where dupNameSlashDateOfBirth.Contains(dem.FullName)
                            group new { dem, pp, pd } by new
                            {
                                dem.BadgeNumber,
                                dem.SSN,
                                dem.FullName,
                                dem.DateOfBirth,
                                dem.Address.Street,
                                dem.Address.City,
                                dem.Address.State,
                                dem.Address.PostalCode,
                                dem.Address.CountryISO,
                                pp.CompanyContributionYears,
                                dem.HireDate,
                                dem.TerminationDate,
                                dem.EmploymentStatusId,
                                dem.StoreNumber,
                                PdSsn = (long?)(pd != null ? pd.SSN : null),
                                pp.NetBalanceLastYear,
                                pp.HoursCurrentYear,
                                pp.EarningsCurrentYear
                            } into g
                            orderby g.Key.FullName, g.Key.DateOfBirth, g.Key.SSN, g.Key.BadgeNumber
                            select new DuplicateNamesAndBirthdaysResponse
                            {
                                BadgeNumber = g.Key.BadgeNumber,
                                SSN = g.Key.SSN,
                                Name = g.Key.FullName,
                                DateOfBirth = g.Key.DateOfBirth,
                                Address = new AddressResponseDto()
                                {
                                    City = g.Key.City,
                                    State = g.Key.State,
                                    Street = g.Key.Street,
                                    CountryISO = g.Key.CountryISO,
                                    PostalCode = g.Key.PostalCode,
                                },
                                Years = g.Key.CompanyContributionYears,
                                HireDate = g.Key.HireDate,
                                TerminationDate = g.Key.TerminationDate,
                                Status = g.Key.EmploymentStatusId,
                                StoreNumber = g.Key.StoreNumber,
                                Count = g.Count(),
                                NetBalance = g.Key.NetBalanceLastYear,
                                HoursCurrentYear = g.Key.HoursCurrentYear,
                                EarningsCurrentYear = g.Key.EarningsCurrentYear
                            };

                return await query.ToPaginationResultsAsync(req, forceSingleQuery: true, cancellationToken: cancellationToken);
            });

            _logger.LogInformation("Returned {results} records", results.Results.Count());

            return new ReportResponseBase<DuplicateNamesAndBirthdaysResponse>()
            {
                ReportDate = DateTimeOffset.Now,
                ReportName = "DUPLICATE NAMES AND BIRTHDAYS",
                Response = results
            };
        }
    }
}
