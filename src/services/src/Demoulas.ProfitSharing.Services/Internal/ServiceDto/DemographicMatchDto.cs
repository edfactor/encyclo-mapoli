
namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto
{
    internal sealed record DemographicMatchDto
    {
        public required string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
