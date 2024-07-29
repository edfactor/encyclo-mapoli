using System.Collections.Frozen;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Mappers;
using FastEndpoints;
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

    public async Task<ReportResponseBase<PayrollDuplicateSSNResponseDto>> GetDuplicateSSNs(CancellationToken ct)
    {
        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var dupSsns = await ctx.Demographics.GroupBy(x => x.SSN).Where(x => x.Count() > 1).Select(x => x.Key).ToListAsync();

            var rslts = await (from dem in ctx.Demographics
                    join pdJoin in ctx.ProfitDetails on dem.SSN equals pdJoin.SSN into demPdJoin
                    from pd in demPdJoin.DefaultIfEmpty()
                    join pp in ctx.PayProfits on dem.SSN equals pp.EmployeeSSN into DemPdPpJoin
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
                        Address = grp.Key.Street,
                        City = grp.Key.City,
                        State = grp.Key.State,
                        PostalCode = grp.Key.PostalCode,
                        HireDate = grp.Key.HireDate,
                        TerminationDate = grp.Key.TerminationDate,
                        RehireDate = grp.Key.ReHireDate,
                        Status = grp.Key.EmploymentStatusId,
                        StoreNumber = grp.Key.StoreNumber,
                        ProfitSharingRecords = grp.Count(),
                        HoursCurrentYear = grp.Key.HoursCurrentYear,
                        EarningsCurrentYear = grp.Key.EarningsCurrentYear,
                    }
                ).ToListAsync(ct);

            return new ReportResponseBase<PayrollDuplicateSSNResponseDto>()
            {
                ReportDate = DateTimeOffset.Now,
                ReportName = "Duplicate SSNs",
                Results = rslts.ToFrozenSet()
            };
        });
    }

    public async Task<ReportResponseBase<PayProfitBadgesNotInDemographicsResponse>> GetPayProfitBadgesNotInDemographics(CancellationToken ct = default)
    {
        var results = await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            return await (from pp in ctx.PayProfits
             join dem in ctx.Demographics on pp.EmployeeBadge equals dem.BadgeNumber into demTmp
             from dem in demTmp.DefaultIfEmpty()
             where dem == null
             orderby pp.EmployeeBadge, pp.EmployeeSSN
             select new PayProfitBadgesNotInDemographicsResponse { EmployeeBadge = pp.EmployeeBadge, EmployeeSSN = pp.EmployeeSSN }
            ).ToListAsync(ct);
        });

        return new ReportResponseBase<PayProfitBadgesNotInDemographicsResponse>
        {
            ReportName = "Payprofit Badges not in Demographics",
            ReportDate = DateTimeOffset.Now,
            Results = results.ToFrozenSet()
        };
    }

    public async Task<ReportResponseBase<NegativeETVAForSSNsOnPayProfitResponse>> GetNegativeETVAForSSNsOnPayProfitResponse(CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request NEGATIVE ETVA FOR SSNs ON PAYPROFIT"))
        {
            List<NegativeETVAForSSNsOnPayProfitResponse> results = await _dataContextFactory.UseReadOnlyContext(async c =>
            {
                var ssnUnion = c.Demographics.Select(d => d.SSN).Union(c.Beneficiaries.Select(b => b.SSN));

                return await c.PayProfits
                    .Where(p => ssnUnion.Contains(p.EmployeeSSN) && p.EarningsEtvaValue < 0)
                    .Select(p => new NegativeETVAForSSNsOnPayProfitResponse
                    {
                        EmployeeBadge = p.EmployeeBadge, EmployeeSSN = p.EmployeeSSN, EtvaValue = p.EarningsEtvaValue
                    })
                    .OrderBy(p => p.EmployeeBadge)
                    .ToListAsync(cancellationToken);
            });

            _logger.LogWarning("Returned {results} records", results.Count);

            return new ReportResponseBase<NegativeETVAForSSNsOnPayProfitResponse>
            {
                ReportName = "NEGATIVE ETVA FOR SSNs ON PAYPROFIT", ReportDate = DateTimeOffset.Now, Results = results.ToFrozenSet()
            };
        }
    }

    public async Task<ReportResponseBase<MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto>> GetMismatchedSsnsPayprofitAndDemographicsOnSameBadge(CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request MISMATCHED SSNs PAYPROFIT AND DEMO ON SAME BADGE"))
        {
            List<MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto> results = await _dataContextFactory.UseReadOnlyContext(c =>
            {
                var query = from demographic in c.Demographics
                    join payProfit in c.PayProfits
                        on demographic.BadgeNumber equals payProfit.EmployeeBadge
                    where payProfit.EmployeeSSN != demographic.SSN
                    orderby demographic.BadgeNumber, demographic.SSN, payProfit.EmployeeSSN
                    select new MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto
                    {
                        Name = demographic.FullName ?? $"{demographic.FirstName} {demographic.LastName}",
                        EmployeeBadge = demographic.BadgeNumber,
                        EmployeeSSN = demographic.SSN,
                        PayProfitSSN = payProfit.EmployeeSSN,
                       Store = demographic.StoreNumber,
                       Status = demographic.EmploymentStatusId
                    };

                return query.ToListAsync(cancellationToken);
            });

            _logger.LogWarning("Returned {results} records", results.Count);

            return new ReportResponseBase<MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto>
            {
                ReportName = "MISMATCHED SSNs PAYPROFIT AND DEMO ON SAME BADGE",
                ReportDate = DateTimeOffset.Now,
                Results = results.ToFrozenSet()
            };
        }
    }

    public async Task<ReportResponseBase<PayrollDuplicateSsnsOnPayprofitResponseDto>> GetPayrollDuplicateSsnsOnPayprofit(CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request PAYROLL DUPLICATE SSNs ON PAYPROFIT"))
        {
            List<PayrollDuplicateSsnsOnPayprofitResponseDto> results = await _dataContextFactory.UseReadOnlyContext(context =>
            {
                var query = from payProfit in context.PayProfits
                            join demographics in context.Demographics
                                on payProfit.EmployeeBadge equals demographics.BadgeNumber into demGroup
                            from demographics in demGroup.DefaultIfEmpty()
                            join profitDetail in context.ProfitDetails
                                on payProfit.EmployeeSSN equals profitDetail.SSN into detGroup
                            from profitDetail in detGroup.DefaultIfEmpty()
                            where context.PayProfits
                                .GroupBy(p => p.EmployeeSSN)
                                .Where(g => g.Count() > 1)
                                .Select(g => g.Key)
                                .Contains(payProfit.EmployeeSSN)
                            group new { payProfit, demographics, profitDetail } by new
                            {
                                payProfit.EmployeeBadge,
                                payProfit.EmployeeSSN,
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
                            orderby g.Key.EmployeeSSN, g.Key.EmployeeBadge
                            select new PayrollDuplicateSsnsOnPayprofitResponseDto
                            {
                                Count = g.Count(),
                                EmployeeBadge = g.Key.EmployeeBadge,
                                EmployeeSSN = g.Key.EmployeeSSN,
                                Name = g.Key.FullName,
                                HireDate = g.Key.HireDate,
                                TermDate = g.Key.TerminationDate,
                                RehireDate = g.Key.ReHireDate,
                                Status = g.Key.EmploymentStatusId,
                                Store = g.Key.StoreNumber,
                                EarningsCurrentYear = g.Key.EarningsCurrentYear,
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

                return query.ToListAsync(cancellationToken: cancellationToken);
            });

            _logger.LogWarning("Returned {results} records", results.Count);

            return new ReportResponseBase<PayrollDuplicateSsnsOnPayprofitResponseDto>
            {
                ReportName = "PAYROLL DUPLICATE SSNs ON PAYPROFIT",
                ReportDate = DateTimeOffset.Now,
                Results = results.ToFrozenSet()
            };
        }
    }

    public async Task<ReportResponseBase<DemographicBadgesNotInPayProfitResponse>> GetDemographicBadgesNotInPayProfit(CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request BEGIN DEMOGRAPHIC BADGES NOT IN PAY PROFIT"))
        {
            List<DemographicBadgesNotInPayProfitResponse> results = await _dataContextFactory.UseReadOnlyContext(ctx =>
            {
                var query = from dem in ctx.Demographics
                            where !(from pp in ctx.PayProfits select pp.EmployeeBadge).Contains(dem.BadgeNumber)
                            select new DemographicBadgesNotInPayProfitResponse
                            {
                                EmployeeBadge = dem.BadgeNumber,
                                EmployeeSSN = dem.SSN,
                                EmployeeName = dem.FullName ?? "",
                                Status = dem.EmploymentStatusId,
                                Store = dem.StoreNumber,
                            };
                return query.ToListAsync(cancellationToken: cancellationToken);
            });

            _logger.LogInformation("Returned {results} records", results.Count);

            return new ReportResponseBase<DemographicBadgesNotInPayProfitResponse>
            {
                ReportDate = DateTimeOffset.Now,
                ReportName = "DEMOGRAPHICS BADGES NOT ON PAYPROFIT",
                Results = results.ToFrozenSet()
            };
        }
    }

    public async Task<ReportResponseBase<NamesMissingCommaResponse>> GetNamesMissingComma(CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request BEGIN DEMOGRAPHIC BADGES NOT IN PAY PROFIT"))
        {
            List<NamesMissingCommaResponse> results = await _dataContextFactory.UseReadOnlyContext(ctx =>
            {
                var query = from dem in ctx.Demographics
                            where dem.FullName == null || !dem.FullName.Contains(",")
                            select new NamesMissingCommaResponse
                            {
                                EmployeeBadge = dem.BadgeNumber,
                                EmployeeSSN = dem.SSN,
                                EmployeeName = dem.FullName ?? "",
                            };
                return query.ToListAsync(cancellationToken: cancellationToken);
            });

            _logger.LogInformation("Returned {results} records", results.Count);

            return new ReportResponseBase<NamesMissingCommaResponse>
            {
                ReportDate = DateTimeOffset.Now,
                ReportName = "MISSING COMMA IN PY_NAME",
                Results = results.ToFrozenSet()
            };
        }
    }

    public async Task<ReportResponseBase<DuplicateNamesAndBirthdaysResponse>> GetDuplicateNamesAndBirthdays(CancellationToken cancellationToken = default)
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
                            join ppLj in ctx.PayProfits on dem.BadgeNumber equals ppLj.EmployeeBadge into tmpPayProfit
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

                return await query.ToListAsync(cancellationToken: cancellationToken);
            });

            _logger.LogInformation("Returned {results} records", results.Count);

            return new ReportResponseBase<DuplicateNamesAndBirthdaysResponse>()
            {
                ReportDate = DateTimeOffset.Now,
                ReportName = "DUPLICATE NAMES AND BIRTHDAYS",
                Results = results.ToFrozenSet()
            };
        }
    }
}
