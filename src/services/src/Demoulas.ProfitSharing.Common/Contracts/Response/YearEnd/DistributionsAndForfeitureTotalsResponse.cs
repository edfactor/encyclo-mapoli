
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record DistributionsAndForfeitureTotalsResponse : ReportResponseBase<DistributionsAndForfeitureResponse>
{
    public required decimal DistributionTotal { get; init; }
    public required decimal StateTaxTotal { get; init; }
    public required decimal FederalTaxTotal { get; init; }
    public required decimal ForfeitureTotal { get; init; }

    public required Dictionary<string, decimal> StateTaxTotals { get; init; }

    public static DistributionsAndForfeitureTotalsResponse ResponseExample()
    {
        return new DistributionsAndForfeitureTotalsResponse
        {
            ReportName = "Distributions and Forfeitures",
            ReportDate = DateTimeOffset.Now,

            DistributionTotal = 123456.78m,
            StateTaxTotal = 1234.56m,
            FederalTaxTotal = 987.65m,
            ForfeitureTotal = 345.67m,
            StateTaxTotals = new Dictionary<string, decimal>
            {
                { "MA", 100.00m }, { "NH", 23.56m }
            },
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Response = new PaginatedResponseDto<DistributionsAndForfeitureResponse>
            {
                Results = [DistributionsAndForfeitureResponse.ResponseExample()]
            }
        };
    }
}
