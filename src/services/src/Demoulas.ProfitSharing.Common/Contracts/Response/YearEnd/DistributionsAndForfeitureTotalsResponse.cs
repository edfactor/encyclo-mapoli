
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record DistributionsAndForfeitureTotalsResponse : ReportResponseBase<DistributionsAndForfeitureResponse>
{
    [YearEndArchiveProperty("QPAY129_DistributionTotals")]
    public required decimal DistributionTotal { get; init; }
    [YearEndArchiveProperty]
    public required decimal StateTaxTotal { get; init; }
    [YearEndArchiveProperty]
    public required decimal FederalTaxTotal { get; init; }
    [YearEndArchiveProperty]
    public required decimal ForfeitureTotal { get; init; }

    /// <summary>
    /// Regular forfeitures (not Administrative or Class Action)
    /// </summary>
    [YearEndArchiveProperty]
    public decimal ForfeitureRegularTotal { get; init; }

    /// <summary>
    /// Administrative forfeitures (MAIN-2170)
    /// </summary>
    [YearEndArchiveProperty]
    public decimal ForfeitureAdministrativeTotal { get; init; }

    /// <summary>
    /// Class Action forfeitures
    /// </summary>
    [YearEndArchiveProperty]
    public decimal ForfeitureClassActionTotal { get; init; }

    public required Dictionary<string, decimal> StateTaxTotals { get; init; }

    /// <summary>
    /// Records with state taxes but no state code attribution.
    /// These represent data quality issues in state extraction (PS-2031).
    /// </summary>
    public UnattributedTotals? UnattributedTotals { get; init; }

    /// <summary>
    /// Indicates if any unattributed records were found during this query.
    /// </summary>
    public bool HasUnattributedRecords { get; init; }

    public static DistributionsAndForfeitureTotalsResponse ResponseExample()
    {
        return new DistributionsAndForfeitureTotalsResponse
        {
            ReportName = ReportNames.DistributionAndForfeitures.Name,
            ReportDate = DateTimeOffset.Now,

            DistributionTotal = 123456.78m,
            StateTaxTotal = 1234.56m,
            FederalTaxTotal = 987.65m,
            ForfeitureTotal = 345.67m,
            ForfeitureRegularTotal = 200.00m,
            ForfeitureAdministrativeTotal = 100.00m,
            ForfeitureClassActionTotal = 45.67m,
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
