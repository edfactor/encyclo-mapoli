using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Demographics;

public class AddDemographicsEndpoint : Endpoint<IEnumerable<DemographicsRequestDto>, ISet<DemographicsResponseDto>?>
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
            s.ExampleRequest = new List<DemographicsRequestDto>
            {
                new DemographicsRequestDto
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
                        CountryISO = Constants.US
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
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new List<DemographicsResponseDto>
                {
                    new DemographicsResponseDto
                    {
                        SSN = "123-45-6789",
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

    public override Task<ISet<DemographicsResponseDto>?> ExecuteAsync(IEnumerable<DemographicsRequestDto> req, CancellationToken ct)
    {
        return _demographicsService.AddDemographics(req, ct);
    }
}
