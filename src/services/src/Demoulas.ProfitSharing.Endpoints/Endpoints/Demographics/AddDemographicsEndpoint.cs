using Demoulas.Common.Contracts.Request;
using Demoulas.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Enums;
using Demoulas.ProfitSharing.Common.Interfaces;
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
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new List<DemographicsRequestDto>
                {
                    new DemographicsRequestDto
                    {
                        OracleHcmId = 0,
                        FullName = "John Doe",
                        LastName = "John",
                        FirstName = "Doe",
                        StoreNumber = 0,
                        Department = (Department)0,
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
                        EmploymentType = (EmploymentType)0,
                        PayFrequency = (PayFrequency)0,
                        Gender = (Gender)0
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
