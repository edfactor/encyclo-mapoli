namespace Demoulas.ProfitSharing.Endpoints.Contracts.Contracts.Response;

public record DemographicsResponseDto
{
    public int BadgeNumber { get; set; }


    public AddressResponseDto? Address { get; set; }
}
