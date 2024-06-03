using Demoulas.Common.Contracts.Request;
using Demoulas.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Enums;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Demographics;

public class GetAllDemographicsEndpoint : Endpoint<PaginationRequestDto, PaginatedResponseDto<DemographicsResponseDto>?>
{
    private readonly IDemographicsService _demographicsService;

    public GetAllDemographicsEndpoint(IDemographicsService demographicsService)
    {
        _demographicsService = demographicsService;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Get("all");
        Summary(s =>
        {
            s.Summary = "Get all Demographics";
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new List<DemographicsResponseDto>
                {
                    new DemographicsResponseDto
                    {
                        OracleHcmId = 0,
                        FullName = "John Doe",
                        LastName = "John",
                        FirstName = "Doe",
                        StoreNumber = 0,
                        Department = (Department)0,
                        PayClassificationId = 0,
                        ContactInfo = new ContactInfoResponseDto(),
                        DateOfBirth = default,
                        HireDate = default,
                        ReHireDate = default,
                        EmploymentType = (EmploymentType)0,
                        PayFrequency = (PayFrequency)0,
                        Gender = (Gender)0
                    }
                }
            } };
        });
        Group<LookupGroup>();
    }

    public override Task<PaginatedResponseDto<DemographicsResponseDto>?> ExecuteAsync(PaginationRequestDto req, CancellationToken ct)
    {
        return _demographicsService.GetAllDemographics(req, ct);
    }
}
