using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.TypeConverters;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Cleanup;
public class DuplicateNamesAndBirthdaysEndpoint : EndpointWithCsvBase<ProfitYearRequest, DuplicateNamesAndBirthdaysResponse, DuplicateNamesAndBirthdaysEndpoint.DuplicateNamesAndBirthdaysResponseMap>
{
    private readonly ICleanupReportService _cleanupReportService;

    public DuplicateNamesAndBirthdaysEndpoint(ICleanupReportService cleanupReportService)
        : base(Navigation.Constants.DuplicateNamesAndBirthdays)
    {
        _cleanupReportService = cleanupReportService;
    }

    public override void Configure()
    {
        Get("duplicate-names-and-birthdays");
        Summary(s =>
        {
            s.Summary = "List of duplicate names, and birthdays in the demographics area";
            s.ExampleRequest = SimpleExampleRequest;
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<DuplicateNamesAndBirthdaysResponse>
                    {
                        ReportDate = DateTime.Now,
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
                        EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        ReportName = "DUPLICATE NAMES AND BIRTHDAYS",
                        Response = new PaginatedResponseDto<DuplicateNamesAndBirthdaysResponse>
                        {
                            Results = new List<DuplicateNamesAndBirthdaysResponse>
                            {
                                new DuplicateNamesAndBirthdaysResponse
                                {
                                    Address = new AddressResponseDto
                                    {
                                        State = "MA",
                                        PostalCode = "01876",
                                        City = "Tewksbury",
                                        Street = "1900 Main St",
                                        CountryIso="US"
                                    },
                                    BadgeNumber = 100110,
                                    Count = 2,
                                    IncomeCurrentYear = 23003,
                                    DateOfBirth = new DateOnly(1990,7,30),
                                    HireDate = new DateOnly(2015,9,14),
                                    HoursCurrentYear = 1524,
                                    Name = "Henry Rollins",
                                    NetBalance = 52500,
                                    Ssn = "XXX-XX-1234",
                                    Status = 'A',
                                    EmploymentStatusName = "Active",
                                    StoreNumber = 22,
                                    Years = 3,
                                    IsExecutive = false,
                                }
                            }
                        }
                    }
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "duplicate-names-and-birthdays";

    public override Task<ReportResponseBase<DuplicateNamesAndBirthdaysResponse>> GetResponse(ProfitYearRequest req, CancellationToken ct)
    {
        return _cleanupReportService.GetDuplicateNamesAndBirthdaysAsync(req, ct);
    }

    public sealed class DuplicateNamesAndBirthdaysResponseMap : ClassMap<DuplicateNamesAndBirthdaysResponse>
    {
        public DuplicateNamesAndBirthdaysResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.BadgeNumber).Index(2).Name("BADGE");
            Map(m => m.Ssn).Index(3).Name("SSN");
            Map(m => m.Name).Index(4).Name("NAME");
            Map(m => m.DateOfBirth).Index(5).Name("DOB").TypeConverter<YearMonthDayTypeConverter>();
            Map(m => m.Address.Street).Index(6).Name("ADDRESS");
            Map(m => m.Address.City).Index(7).Name("CITY");
            Map(m => m.Address.State).Index(8).Name("ST");
            Map(m => m.Years).Index(9).Name("YRS");
            Map(m => m.HireDate).Index(10).Name("HIRE").TypeConverter<YearMonthDayTypeConverter>();
            Map(m => m.TerminationDate).Index(11).Name("TERM").TypeConverter<YearMonthDayTypeConverter>();
            Map(m => m.Status).Index(12).Name("ST");
            Map(m => m.StoreNumber).Index(13).Name("STORE");
            Map(m => m.Count).Index(14).Name("PS#");
            Map(m => m.NetBalance).Index(15).Name("PSBAL");
            Map(m => m.HoursCurrentYear).Index(16).Name("CUR HURS");
            Map(m => m.IncomeCurrentYear).Index(17).Name("CUR WAGE");
        }
    }
}
