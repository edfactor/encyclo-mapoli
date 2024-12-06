namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public sealed record SetExecutiveHoursAndDollarsRequest
{
    public required short ProfitYear { get; set; }
    public required List<SetExecutiveHoursAndDollarsDto> ExecutiveHoursAndDollars { get; set; } = [];

    public static SetExecutiveHoursAndDollarsRequest Example()
    {
        return new()
        {
            ProfitYear = 1876,
            ExecutiveHoursAndDollars =
            [
                SetExecutiveHoursAndDollarsDto.Example()
            ]
        };
    }
}

