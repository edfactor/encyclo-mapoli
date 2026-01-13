using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public class DepartmentResponseDto
{
    public byte Id { get; set; }
    [MaskSensitive] public required string Name { get; set; }

    public static DepartmentResponseDto ResponseExample()
    {
        return new DepartmentResponseDto
        {
            Id = 1,
            Name = "Grocery"
        };
    }
}
