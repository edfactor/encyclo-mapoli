using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;

using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Services.Reports
{
    public class PayrollDuplicateSsnReportService : IPayrollDuplicateSsnReportServiceInternal, IPayrollDuplicateSsnReportService
    {
        private readonly IProfitSharingDataContextFactory _dataContextFactory;
        private readonly IDemographicReaderService _demographicReaderService;
        private readonly ICalendarService _calendarService;

        public PayrollDuplicateSsnReportService(IProfitSharingDataContextFactory dataContextFactory,
            IDemographicReaderService demographicReaderService,
            ICalendarService calendarService)
        {
            _dataContextFactory = dataContextFactory;
            _demographicReaderService = demographicReaderService;
            _calendarService = calendarService;
        }

        public Task<bool> DuplicateSsnExistsAsync(CancellationToken ct)
        {
            return _dataContextFactory.UseReadOnlyContext(async ctx =>
            {

                IQueryable<Demographic> demographics = await _demographicReaderService.BuildDemographicQuery(ctx);

                return await getDuplicateSsnQuery(demographics).AnyAsync(ct);

            });
        }

        public Task<ReportResponseBase<PayrollDuplicateSsnResponseDto>> GetDuplicateSsnAsync(ProfitYearRequest req, CancellationToken ct)
        {
            return _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                short cutoffYear = (short)(DateTime.UtcNow.Year - 5);
                var cal = await _calendarService.GetYearStartAndEndAccountingDatesAsync(cutoffYear, ct);
                var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);

                var dupSsns = await getDuplicateSsnQuery(demographics).ToHashSetAsync(ct);

                var sortTmp = req.SortBy?.ToLowerInvariant() switch
                {
                    "address" => "street,city,state,postalcode",
                    _ => req.SortBy,
                };

                var sortReq = req with { SortBy = sortTmp };

                var data = await demographics
                    .Include(x => x.EmploymentStatus)
                    .Where(dem => dupSsns.Contains(dem.Ssn))
                    .OrderBy(d => d.Ssn)
                    .Select(dem => new
                    {
                        dem.BadgeNumber,
                        dem.Ssn,
                        Name = dem.ContactInfo.FullName,
                        dem.Address.Street,
                        dem.Address.City,
                        dem.Address.State,
                        dem.Address.PostalCode,
                        CountryIso = Country.Constants.Us,
                        dem.HireDate,
                        dem.TerminationDate,
                        RehireDate = dem.ReHireDate,
                        Status = dem.EmploymentStatusId,
                        EmploymentStatusName = dem.EmploymentStatus!.Name,
                        dem.StoreNumber,
                        IsExecutive = dem.PayFrequencyId == PayFrequency.Constants.Monthly,
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
                                LastUpdate = pp.ModifiedAtUtc == null ? pp.CreatedAtUtc : pp.ModifiedAtUtc.Value,
                                PointsEarned = pp.PointsEarned
                            }).ToList()
                    })
                    .ToPaginationResultsAsync(sortReq, ct);



                DateTimeOffset endDate = DateTimeOffset.UtcNow;
                if (data.Results.Any())
                {
                    endDate = data.Results.SelectMany(r => r.PayProfits.Select(p => p.LastUpdate)).Max();
                }

                var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, ct);

                return new ReportResponseBase<PayrollDuplicateSsnResponseDto>
                {
                    ReportName = "Duplicate SSNs on Demographics",
                    StartDate = calInfo.FiscalBeginDate,
                    EndDate = calInfo.FiscalEndDate,
                    Response = new Demoulas.Common.Contracts.Contracts.Response.PaginatedResponseDto<PayrollDuplicateSsnResponseDto>
                    {
                        Total = data.Total,
                        Results = data.Results.Select(x => new PayrollDuplicateSsnResponseDto()
                        {
                            BadgeNumber = x.BadgeNumber,
                            Ssn = x.Ssn.MaskSsn(),
                            Name = x.Name,
                            Address = new AddressResponseDto
                            {
                                Street = x.Street,
                                City = x.City,
                                State = x.State,
                                PostalCode = x.PostalCode,
                                CountryIso = x.CountryIso
                            },
                            HireDate = x.HireDate,
                            TerminationDate = x.TerminationDate,
                            RehireDate = x.RehireDate,
                            Status = x.Status == EmploymentStatus.Constants.Active ? 'A' : 'T',
                            EmploymentStatusName = x.EmploymentStatusName,
                            StoreNumber = x.StoreNumber,
                            ProfitSharingRecords = x.ProfitSharingRecords,
                            PayProfits = x.PayProfits,
                            IsExecutive = x.IsExecutive
                        }).ToList()
                    },
                };
            });
        }


        private IQueryable<int> getDuplicateSsnQuery(IQueryable<Demographic> demographics)
        {
            return demographics
                .GroupBy(x => x.Ssn)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);
        }
    }
}
