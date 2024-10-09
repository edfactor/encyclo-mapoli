using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Cleanup;

public class DemographicBadgesNotInPayProfitEndpoint : EndpointWithCsvBase<PaginationRequestDto, DemographicBadgesNotInPayProfitResponse,
    DemographicBadgesNotInPayProfitEndpoint.DemographicBadgesNotInPayProfitResponseMap>
{
    private readonly ICleanupReportService _cleanupReportService;

    public DemographicBadgesNotInPayProfitEndpoint(ICleanupReportService cleanupReportService)
    {
        _cleanupReportService = cleanupReportService;
    }

    public override void Configure()
    {
        Get("demographic-badges-not-in-payprofit");
        Summary(s =>
        {
            s.Summary = "Demographic badges not in Payprofit";
            s.Description = "Returns a list of employees who have demographic badges but are not in Payprofit.";
            s.ExampleRequest = SimpleExampleRequest;
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<DemographicBadgesNotInPayProfitResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        Response = new PaginatedResponseDto<DemographicBadgesNotInPayProfitResponse>()
                        {
                            Results = new List<DemographicBadgesNotInPayProfitResponse>
                            {
                                new DemographicBadgesNotInPayProfitResponse()
                                {
                                    EmployeeBadge = 47425,
                                    EmployeeSsn = 900047425,
                                    EmployeeName = "John",
                                    Status = EmploymentStatus.Constants.Active
                                },
                                new DemographicBadgesNotInPayProfitResponse
                                {
                                    EmployeeBadge = 82424,
                                    EmployeeSsn = 900082424,
                                    EmployeeName = "Jane",
                                    Status = EmploymentStatus.Constants.Delete
                                },
                                new DemographicBadgesNotInPayProfitResponse
                                {
                                    EmployeeBadge = 85744,
                                    EmployeeSsn = 900085744,
                                    EmployeeName = "Tim",
                                    Status = EmploymentStatus.Constants.Inactive
                                },
                                new DemographicBadgesNotInPayProfitResponse
                                {
                                    EmployeeBadge = 94861,
                                    EmployeeSsn = 900094861,
                                    EmployeeName = "Sally",
                                    Status = EmploymentStatus.Constants.Terminated,
                                    Store = 4
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

    public override string ReportFileName => "DEMOGRAPHIC-WITHOUT-DEMOGRAPHICS";

    public override async Task<ReportResponseBase<DemographicBadgesNotInPayProfitResponse>> GetResponse(PaginationRequestDto req, CancellationToken ct)
    {
        return await _cleanupReportService.GetDemographicBadgesNotInPayProfit(req, ct);
    }

    public sealed class DemographicBadgesNotInPayProfitResponseMap : ClassMap<DemographicBadgesNotInPayProfitResponse>
    {
        public DemographicBadgesNotInPayProfitResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.EmployeeBadge).Index(2).Name("BADGE");
            Map(m => m.EmployeeSsn).Index(3).Name("DEM SSN");
            Map(m => m.EmployeeName).Index(4).Name("Name");
            Map(m => m.Store).Index(5).Name("Store");
            Map(m => m.Status).Index(6).Name("StatusEnum");
        }
    }
}
