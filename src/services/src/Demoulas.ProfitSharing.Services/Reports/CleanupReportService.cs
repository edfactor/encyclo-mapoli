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
public class CleanupReportService : IYearEndService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ILogger<CleanupReportService> _logger;

    public CleanupReportService(IProfitSharingDataContextFactory dataContextFactory, 
        ILoggerFactory factory)
    {
        _dataContextFactory = dataContextFactory;
        _logger = factory.CreateLogger<CleanupReportService>();
    }

    public async Task<ReportResponseBase<PayrollDuplicateSsnResponseDto>> GetDuplicateSsNs(FiscalYearRequest req, CancellationToken ct)
    {
        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var dupSsns = await ctx.Demographics.GroupBy(x => x.Ssn).Where(x => x.Count() > 1).Select(x => x.Key).ToListAsync(cancellationToken: ct);

            
            
            var rslts = ctx.Demographics
                .Include(d => d.PayProfits
                    .FirstOrDefault(pp => pp.FiscalYear == req.ReportingYear))
                .Where(d => dupSsns.Contains(d.Ssn))
                    .Select(d=> new PayrollDuplicateSsnResponseDto
                    {
                        BadgeNumber = d.BadgeNumber,
                        Ssn = d.Ssn.MaskSsn(),
                        Name = d.FullName,
                        Address = new AddressResponseDto
                        {
                            Street = d.Address.Street,
                            City = d.Address.City,
                            State = d.Address.State,
                            PostalCode = d.Address.PostalCode,
                            CountryIso = Country.Constants.Us
                        },
                        HireDate = d.HireDate,
                        TerminationDate = d.TerminationDate,
                        RehireDate = d.ReHireDate,
                        Status = d.EmploymentStatusId,
                        StoreNumber = d.StoreNumber,
                        ProfitSharingRecords = grp.Count(),
                        HoursCurrentYear = d.PayProfits.FirstOrDefault()?.CurrentHoursYear ?? 0,
                        IncomeCurrentYear = d.PayProfits.FirstOrDefault()?.CurrentIncomeYear ?? 0,
                    }
                ).ToPaginationResultsAsync(req, forceSingleQuery: true, ct);

            return new ReportResponseBase<PayrollDuplicateSsnResponseDto>
            {
                ReportDate = DateTimeOffset.Now, ReportName = "Duplicate SSNs", Response = rslts
            };
        });
    }

    public async Task<ReportResponseBase<NegativeEtvaForSsNsOnPayProfitResponse>> GetNegativeETVAForSSNsOnPayProfitResponse(PaginationRequestDto req, CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request NEGATIVE ETVA FOR SSNs ON PAYPROFIT"))
        {
            var results = await _dataContextFactory.UseReadOnlyContext(c =>
            {
                var ssnUnion = c.Demographics.Select(d => d.Ssn)
                    .Union(c.Beneficiaries.Select(b => b.Ssn));

                return c.PayProfits
                    .Include(p=> p.Demographic)
                    .Where(p => ssnUnion.Contains(p.Demographic!.Ssn) && p.EarningsEtvaValue < 0)
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

    public async Task<ReportResponseBase<MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto>> GetMismatchedSsnsPayprofitAndDemographicsOnSameBadge(PaginationRequestDto req, CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request MISMATCHED SSNs PAYPROFIT AND DEMO ON SAME BADGE"))
        {
            var results = await _dataContextFactory.UseReadOnlyContext(c =>
            {
                var query = from demographic in c.Demographics
                    join payProfit in c.PayProfits
                        on demographic.BadgeNumber equals payProfit.BadgeNumber
                            where payProfit.Ssn != demographic.Ssn
                    orderby demographic.BadgeNumber, demographic.Ssn, payProfit.Ssn
                    select new MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto
                    {
                        Name = demographic.FullName ?? $"{demographic.FirstName} {demographic.LastName}",
                        EmployeeBadge = demographic.BadgeNumber,
                        EmployeeSsn = demographic.Ssn,
                        PayProfitSsn = payProfit.Ssn,
                       Store = demographic.StoreNumber,
                       Status = demographic.EmploymentStatusId
                    };

                return query.ToPaginationResultsAsync(req, forceSingleQuery: true, cancellationToken);
            });

            _logger.LogWarning("Returned {Results} records", results.Results.Count());

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
                                on payProfit.BadgeNumber equals demographics.BadgeNumber into demGroup
                            from demographics in demGroup.DefaultIfEmpty()
                            join profitDetail in context.ProfitDetails
                                on payProfit.Ssn equals profitDetail.Ssn into detGroup
                            from profitDetail in detGroup.DefaultIfEmpty()
                            where context.PayProfits
                                .GroupBy(p => p.Ssn)
                                .Where(g => g.Count() > 1)
                                .Select(g => g.Key)
                                .Contains(payProfit.Ssn)
                            group new { payProfit, demographics, profitDetail } by new
                            {
                                BadgeNumber=payProfit.BadgeNumber,
                                SSN = payProfit.Ssn,
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
                                CountryISO = demographics.Address.CountryIso,
                                demographics.ContactInfo.EmailAddress,
                                demographics.ContactInfo.PhoneNumber,
                                demographics.ContactInfo.MobileNumber,
                                payProfit.IncomeCurrentYear
                            } into g
                            orderby g.Key.SSN, g.Key.BadgeNumber
                            select new PayrollDuplicateSsnsOnPayprofitResponseDto
                            {
                                Count = g.Count(),
                                BadgeNumber = g.Key.BadgeNumber,
                                EmployeeSsn = g.Key.SSN,
                                Name = g.Key.FullName,
                                HireDate = g.Key.HireDate,
                                TermDate = g.Key.TerminationDate,
                                RehireDate = g.Key.ReHireDate,
                                Status = g.Key.EmploymentStatusId,
                                Store = g.Key.StoreNumber,
                                IncomeCurrentYear = g.Key.IncomeCurrentYear ?? 0,
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
                                    CountryIso = g.Key.CountryISO
                                }
                            };

                return query.ToPaginationResultsAsync(req, forceSingleQuery: true, cancellationToken: cancellationToken);
            });

            _logger.LogWarning("Returned {Results} records", results.Results.Count());

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
                            where !(from pp in ctx.PayProfits select pp.BadgeNumber).Contains(dem.BadgeNumber)
                            select new DemographicBadgesNotInPayProfitResponse
                            {
                                EmployeeBadge = dem.BadgeNumber,
                                EmployeeSsn = dem.Ssn,
                                EmployeeName = dem.FullName ?? "",
                                Status = dem.EmploymentStatusId,
                                Store = dem.StoreNumber,
                            };
                return query.ToPaginationResultsAsync(req, forceSingleQuery:true, cancellationToken: cancellationToken);
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
                                EmployeeSsn = dem.Ssn,
                                EmployeeName = dem.FullName ?? "",
                            };
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

    public async Task<ReportResponseBase<DuplicateNamesAndBirthdaysResponse>> GetDuplicateNamesAndBirthdays(PaginationRequestDto req, CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request BEGIN DUPLICATE NAMES AND BIRTHDAYS"))
        {
            var results = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                var dupNameSlashDateOfBirth = await (from dem in ctx.Demographics
                                                 group dem by new { dem.FullName, dem.DateOfBirth } into g
                                                 where g.Count() > 1
                                                 select g.Key.FullName).ToListAsync(cancellationToken: cancellationToken);

                var query = from dem in ctx.Demographics
                            join ppLj in ctx.PayProfits on dem.BadgeNumber equals ppLj.BadgeNumber into tmpPayProfit
                            from pp in tmpPayProfit.DefaultIfEmpty()
                            join pdLj in ctx.ProfitDetails on dem.Ssn equals pdLj.Ssn into tmpProfitDetails
                            from pd in tmpProfitDetails.DefaultIfEmpty()
                            where dupNameSlashDateOfBirth.Contains(dem.FullName)
                            group new { dem, pp, pd } by new
                            {
                                dem.BadgeNumber,
                                SSN = dem.Ssn,
                                dem.FullName,
                                dem.DateOfBirth,
                                dem.Address.Street,
                                dem.Address.City,
                                dem.Address.State,
                                dem.Address.PostalCode,
                                CountryISO = dem.Address.CountryIso,
                                pp.CompanyContributionYears,
                                dem.HireDate,
                                dem.TerminationDate,
                                dem.EmploymentStatusId,
                                dem.StoreNumber,
                                PdSsn = (long?)(pd != null ? pd.Ssn : null),
                                pp.NetBalanceLastYear,
                                pp.HoursCurrentYear,
                                pp.IncomeCurrentYear
                            } into g
                            orderby g.Key.FullName, g.Key.DateOfBirth, g.Key.SSN, g.Key.BadgeNumber
                            select new DuplicateNamesAndBirthdaysResponse
                            {
                                BadgeNumber = g.Key.BadgeNumber,
                                Ssn = g.Key.SSN,
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
                                Years = g.Key.CompanyContributionYears,
                                HireDate = g.Key.HireDate,
                                TerminationDate = g.Key.TerminationDate,
                                Status = g.Key.EmploymentStatusId,
                                StoreNumber = g.Key.StoreNumber,
                                Count = g.Count(),
                                NetBalance = g.Key.NetBalanceLastYear,
                                HoursCurrentYear = g.Key.HoursCurrentYear,
                                IncomeCurrentYear = g.Key.IncomeCurrentYear
                            };

                return await query.ToPaginationResultsAsync(req, forceSingleQuery: true, cancellationToken: cancellationToken);
            });

            _logger.LogInformation("Returned {Results} records", results.Results.Count());

            return new ReportResponseBase<DuplicateNamesAndBirthdaysResponse>()
            {
                ReportDate = DateTimeOffset.Now,
                ReportName = "DUPLICATE NAMES AND BIRTHDAYS",
                Response = results
            };
        }
    }
}
