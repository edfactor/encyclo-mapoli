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
    private readonly ContributionService _contributionService;
    private readonly ILogger<CleanupReportService> _logger;

    public CleanupReportService(IProfitSharingDataContextFactory dataContextFactory, 
        ContributionService contributionService,
        ILoggerFactory factory)
    {
        _dataContextFactory = dataContextFactory;
        _contributionService = contributionService;
        _logger = factory.CreateLogger<CleanupReportService>();
    }

    public async Task<ReportResponseBase<PayrollDuplicateSsnResponseDto>> GetDuplicateSsNs(FiscalYearRequest req, CancellationToken ct)
    {
        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var dupSsns = await ctx.Demographics.GroupBy(x => x.Ssn).Where(x => x.Count() > 1).Select(x => x.Key).ToListAsync(cancellationToken: ct);

            var rslts = await (from dem in ctx.Demographics
                               join pdJoin in ctx.ProfitDetails on dem.Ssn equals pdJoin.Ssn into demPdJoin
                               from pd in demPdJoin.DefaultIfEmpty()
                               join pp in ctx.PayProfits on dem.OracleHcmId equals pp.OracleHcmId into DemPdPpJoin
                               from DemPdPp in DemPdPpJoin.DefaultIfEmpty()
                               where DemPdPp.FiscalYear == req.ReportingYear &&  dupSsns.Contains(dem.Ssn)
                               group new { dem, DemPdPp }
                                   by new
                                   {
                                       dem.BadgeNumber,
                                       SSN = dem.Ssn,
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

    public async Task<ReportResponseBase<DemographicBadgesNotInPayProfitResponse>> GetDemographicBadgesNotInPayProfit(PaginationRequestDto req, CancellationToken cancellationToken = default)
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
#pragma warning disable CA1847
                    where dem.FullName == null || !dem.FullName.Contains(",")
#pragma warning restore CA1847
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

    public async Task<ReportResponseBase<DuplicateNamesAndBirthdaysResponse>> GetDuplicateNamesAndBirthdays(FiscalYearRequest req, CancellationToken cancellationToken = default)
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
                            join ppLj in ctx.PayProfits on dem.OracleHcmId equals ppLj.OracleHcmId into tmpPayProfit
                            from pp in tmpPayProfit.DefaultIfEmpty()
                            join pdLj in ctx.ProfitDetails on dem.Ssn equals pdLj.Ssn into tmpProfitDetails
                            from pd in tmpProfitDetails.DefaultIfEmpty()
                            where pp.FiscalYear == req.ReportingYear && dupNameSlashDateOfBirth.Contains(dem.FullName)
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
                                dem.HireDate,
                                dem.TerminationDate,
                                dem.EmploymentStatusId,
                                dem.StoreNumber,
                                PdSsn = pd?.Ssn,
                               // pp.NetBalanceLastYear,
                                pp.CurrentHoursYear,
                                pp.CurrentIncomeYear
                            } into g
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
                                //NetBalance = g.Key.NetBalanceLastYear,
                                HoursCurrentYear = g.Key.CurrentHoursYear,
                                IncomeCurrentYear = g.Key.CurrentIncomeYear
                            };

                return await query.ToPaginationResultsAsync(req, forceSingleQuery: true, cancellationToken: cancellationToken);
            });

            _logger.LogInformation("Returned {Results} records", results.Results.Count());

            ISet<int> badgeNumbers = results.Results.Select(r => r.BadgeNumber).ToHashSet();
            var dict = await _contributionService.GetContributionYears(badgeNumbers);
            
            foreach (DuplicateNamesAndBirthdaysResponse dup in results.Results)
            {
                 _ = dict.TryGetValue(dup.BadgeNumber, out int years);
                 dup.Years = (short)years;
            }

            return new ReportResponseBase<DuplicateNamesAndBirthdaysResponse>()
            {
                ReportDate = DateTimeOffset.Now,
                ReportName = "DUPLICATE NAMES AND BIRTHDAYS",
                Response = results
            };
        }
    }
}
