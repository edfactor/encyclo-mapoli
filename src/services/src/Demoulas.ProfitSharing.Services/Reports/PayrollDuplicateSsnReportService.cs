using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports
{
    public class PayrollDuplicateSsnReportService : IPayrollDuplicateSsnReportService
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

        public Task<ReportResponseBase<PayrollDuplicateSsnResponseDto>> GetDuplicateSsnAsync(SortedPaginationRequestDto req, CancellationToken ct)
        {
            return _dataContextFactory.UseReadOnlyContext(async ctx =>
            {
                short cutoffYear = (short)(DateTime.UtcNow.Year - 5);
                var cal = await _calendarService.GetYearStartAndEndAccountingDatesAsync(cutoffYear, ct);
                var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);

                var dupSsns = await demographics
                    .GroupBy(x => x.Ssn)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToHashSetAsync(ct);

                var rslts = await demographics
                    .Include(x => x.EmploymentStatus)
                    .Where(dem => dupSsns.Contains(dem.Ssn))
                    .OrderBy(d => d.Ssn)
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
                        EmploymentStatusName = dem.EmploymentStatus!.Name,
                        StoreNumber = dem.StoreNumber,
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
                    .ToPaginationResultsAsync(req, ct);

                DateTimeOffset endDate = DateTimeOffset.UtcNow;
                if (rslts.Results.Any())
                {
                    endDate = rslts.Results.SelectMany(r => r.PayProfits.Select(p => p.LastUpdate)).Max();
                }

                return new ReportResponseBase<PayrollDuplicateSsnResponseDto>
                {
                    ReportName = "Duplicate SSNs on Demographics",
                    StartDate = cal.FiscalBeginDate,
                    EndDate = endDate.ToDateOnly(),
                    Response = rslts,
                };
            });
        }
    }
}
