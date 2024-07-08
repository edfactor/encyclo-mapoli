using Demoulas.Common.Contracts.Request;
using Demoulas.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
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
        Post("all");
        Summary(s =>
        {
            s.Summary = "Get all Demographics";
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new List<DemographicsResponseDto>
                {
                    new DemographicsResponseDto
                    {
                        SSN = "XXX-XX-6789",
                        OracleHcmId = 0,
                        FullName = "John Doe",
                        LastName = "John",
                        FirstName = "Doe",
                        StoreNumber = 0,
                        Department = new DepartmentResponseDto { Id = Department.Constants.Produce, Name = "Produce"},
                        PayClassificationId = 0,
                        ContactInfo = new ContactInfoResponseDto(),
                        Address = new AddressResponseDto
                        {
                            Street = "123 Street",
                            State = "MA",
                            City = "Andover",
                            PostalCode = "02589",
                            CountryISO = Constants.US
                        },
                        DateOfBirth = default,
                        HireDate = default,
                        ReHireDate = default,
                        EmploymentType = new EmploymentTypeResponseDto { Name = "Supreme Leader"},
                        PayFrequency = new PayFrequencyResponseDto { Name = "Hourly"},
                        Gender = new GenderResponseDto { Name = "Yes"}
                    }
                }
            } };
        });
        Group<DemographicsGroup>();
    }

    public override Task<PaginatedResponseDto<DemographicsResponseDto>?> ExecuteAsync(PaginationRequestDto req, CancellationToken ct)
    {
        return _demographicsService.GetAllDemographics(req, ct);
    }
}
