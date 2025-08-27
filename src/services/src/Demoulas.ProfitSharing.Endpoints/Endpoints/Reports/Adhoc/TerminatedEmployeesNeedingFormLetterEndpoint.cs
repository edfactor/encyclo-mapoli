using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.Adhoc;
public sealed class TerminatedEmployeesNeedingFormLetterEndpoint : EndpointWithCsvBase<StartAndEndDateRequest, AdhocTerminatedEmployeeResponse, TerminatedEmployeesNeedingFormLetterEndpoint.EndpointMap>
{
    private readonly IAdhocTerminatedEmployeesService _adhocTerminatedEmployeesService;

    public TerminatedEmployeesNeedingFormLetterEndpoint(IAdhocTerminatedEmployeesService adhocTerminatedEmployeesService) : base(Navigation.Constants.Unknown) //TBD
    {
        _adhocTerminatedEmployeesService = adhocTerminatedEmployeesService;
    }
    public override string ReportFileName => "Adhoc Terminated Employees Report needing Form letter";

    public override void Configure()
    {
        Get("adhoc-terminated-employees-report-needing-letter");
        Summary(s =>
        {
            s.Summary = "Adhoc Terminated Employees Report needing a form letter";
            s.Description = "Returns a report of terminated employees who have not yet been sent a form letter to accompany a set of forms to receive vested interest .";
            s.ExampleRequest = new StartAndEndDateRequest
            {
                ProfitYear = 2023,
                Skip = 0,
                Take = 100,
                SortBy = "TerminationDate",
                IsSortDescending = false,
                BeginningDate = new DateOnly(2023, 1, 1),
                EndingDate = new DateOnly(2025, 12, 31),
            };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<AdhocTerminatedEmployeeResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        StartDate = DateOnly.MinValue,
                        EndDate = DateOnly.MaxValue,
                        Response = new PaginatedResponseDto<AdhocTerminatedEmployeeResponse>
                        {
                            Results = new List<AdhocTerminatedEmployeeResponse>
                            {
                                new AdhocTerminatedEmployeeResponse
                                {
                                    BadgeNumber = 12345,
                                    FullName = "John Doe",
                                    Ssn = "123-45-6789",
                                    TerminationDate = new DateOnly(2023, 5, 15),
                                    TerminationCodeId = 'A',
                                    Address = "123 Main St",
                                    State = "MA",
                                    City = "Andover",
                                    PostalCode = "01810",
                                }
                            },
                            Total = 1
                        }
                    }
                }
            };
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override Task<ReportResponseBase<AdhocTerminatedEmployeeResponse>> GetResponse(StartAndEndDateRequest req, CancellationToken ct)
    {
        return _adhocTerminatedEmployeesService.GetTerminatedEmployeesNeedingFormLetter(req, ct);
    }

    public class EndpointMap : ClassMap<AdhocTerminatedEmployeeResponse>
    {
        public EndpointMap()
        {
            Map(m => m.BadgeNumber).Name("Badge Number");
            Map(m => m.FullName).Name("Full Name");
            Map(m => m.Ssn).Name("SSN");
            Map(m => m.TerminationDate).Name("Termination Date");
            Map(m => m.TerminationCodeId).Name("Termination Code ID");
            Map(m => m.Address).Name("Address");
            Map(m => m.Address2).Name("Address 2");
            Map(m => m.City).Name("City");
            Map(m => m.State).Name("State");
            Map(m => m.PostalCode).Name("Postal Code");
        }
    }
}
