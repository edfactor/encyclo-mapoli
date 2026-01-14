using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public class GenderResponseDto
{
    public char Id { get; set; }
    [MaskSensitive] public required string Name { get; set; }

    public static GenderResponseDto ResponseExample()
    {
        return new GenderResponseDto
        {
            Id = 'M',
            Name = "Male"
        };
    }
}
