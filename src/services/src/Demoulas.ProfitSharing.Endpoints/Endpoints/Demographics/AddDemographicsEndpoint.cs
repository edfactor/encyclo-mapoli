using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Demographics;

public class AddDemographicsEndpoint : Endpoint<IEnumerable<DemographicsRequest>, ISet<DemographicResponseDto>?>
{
    private readonly IDemographicsService _demographicsService;

    public AddDemographicsEndpoint(IDemographicsService demographicsService)
    {
        _demographicsService = demographicsService;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Post("");
        Summary(s =>
        {
            s.Summary = "Add Demographics";
            s.Description = "API to add a collection of employee's into the Demographics table";
            s.ExampleRequest = new List<DemographicsRequest>
            {
                new DemographicsRequest
                {
                    BadgeNumber = 123456789,
                    SSN = 123456789,
                    OracleHcmId = 0,
                    FullName = "John Doe",
                    FirstName = "John",
                    MiddleName = "Vikramaditya",
                    LastName = "Doe",
                    StoreNumber = 0,
                    DepartmentId = Department.Constants.Produce,
                    PayClassificationId = 0,
                    ContactInfo = new ContactInfoRequestDto(),
                    Address = new AddressRequestDto
                    {
                        Street = "123 Street",
                        State = "MA",
                        City = "Andover",
                        PostalCode = "02589",
                        CountryISO = Country.Constants.US
                    },
                    DateOfBirth = default,
                    HireDate = default,
                    ReHireDate = default,
                    EmploymentTypeCode = EmploymentType.Constants.PartTime,
                    PayFrequencyId = PayFrequency.Constants.Weekly,
                    GenderCode = Gender.Constants.Female,
                    EmploymentStatusId = EmploymentStatus.Constants.Active
                }
            };
            s.ResponseExamples = new Dictionary<int, object> { { 200, new List<DemographicResponseDto> { DemographicResponseDto.ResponseExample() } } };
        });
        Group<DemographicsGroup>();
    }

    public override Task<ISet<DemographicResponseDto>?> ExecuteAsync(IEnumerable<DemographicsRequest> req, CancellationToken ct)
    {
        return _demographicsService.AddDemographics(req, ct);
    }
}
