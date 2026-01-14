namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public sealed record SetExecutiveHoursAndDollarsRequest : YearRequest
{
    public required List<SetExecutiveHoursAndDollarsDto> ExecutiveHoursAndDollars { get; set; } = [];

    public static new SetExecutiveHoursAndDollarsRequest RequestExample()
    {
        return new()
        {
            ProfitYear = 1876,
            ExecutiveHoursAndDollars =
            [
                SetExecutiveHoursAndDollarsDto.RequestExample()
            ]
        };
    }
}

