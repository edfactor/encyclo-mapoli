
namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto
{
    internal sealed record DemographicMatchDto
    {
        public required string FullName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public byte NameDistance { get; set; }
    }
}
