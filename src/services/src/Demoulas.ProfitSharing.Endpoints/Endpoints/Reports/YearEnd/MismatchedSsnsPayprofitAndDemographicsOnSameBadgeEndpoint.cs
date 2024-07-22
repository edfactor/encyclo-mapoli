using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.MismatchedSsnsPayprofitAndDemographicsOnSameBadgeEndpoint;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public class MismatchedSsnsPayprofitAndDemographicsOnSameBadgeEndpoint : EndpointWithCSVBase<EmptyRequest, MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto, MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseMap>
{
    private readonly IYearEndService _reportService;

    public MismatchedSsnsPayprofitAndDemographicsOnSameBadgeEndpoint(IYearEndService reportService)
    {
        _reportService = reportService;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Get("mismatched-ssns-payprofit-and-demo-on-same-badge");
        Summary(s =>
        {
            s.Summary = "Mismatched SSN's On Pay Profit And Demographics with the same badge number";
            s.Description = @"SSN and ""clean up"" reports to highlight possible problems which should be corrected before profit sharing is run. This job can be run multiple times.";
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto>
                    {
                        ReportName = "MISMATCHED SSNs PAYPROFIT AND DEMO ON SAME BADGE", 
                        ReportDate = DateTimeOffset.Now,
                        Results = new HashSet<MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto>
                        {
                            new MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto { EmployeeBadge = 47425, EmployeeSSN = 900047425, Name = "John", Status = EmploymentStatus.Constants.Active},
                            new MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto { EmployeeBadge = 82424, EmployeeSSN = 900082424, Name = "Jane", Status = EmploymentStatus.Constants.Delete},
                            new MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto { EmployeeBadge = 85744, EmployeeSSN = 900085744, Name = "Tim", Status = EmploymentStatus.Constants.Inactive},
                            new MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto { EmployeeBadge = 94861, EmployeeSSN = 900094861, Name = "Sally", Status = EmploymentStatus.Constants.Terminated}
                        }
                    }
                }
            };
        });
        Group<YearEndGroup>();
        Options(x => x.CacheOutput(p => p.Expire(TimeSpan.FromMinutes(5))));
    }

    public override string ReportFileName => "MISMATCHED-PAYPROF-DEM-SSNS";

    public override Task<ReportResponseBase<MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto>> GetResponse(CancellationToken ct)
    {
        return _reportService.GetMismatchedSsnsPayprofitAndDemographicsOnSameBadge(ct);
    }


    public sealed class MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseMap : ClassMap<MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto>
    {
        public MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.EmployeeBadge).Index(2).Name("BADGE");
            Map(m => m.PayProfitSSN).Index(3).Name("PAYPROFIT SSN");
            Map(m => m.EmployeeSSN).Index(4).Name("DEMOGRAPHICS SSN");
            Map(m => m.Name).Index(5).Name("NAME");
            Map(m => m.Store).Index(6).Name("STORE");
            Map(m => m.Status).Index(7).Name("STATUS");
        }
    }
}
