namespace Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
public sealed record DistributionRunReportSummaryResponse
{
    public char? DistributionFrequencyId { get; set; }
    public required string DistributionTypeName { get; set; }
    public int TotalDistributions { get; set; }
    public decimal TotalGrossAmount { get; set; }
    public decimal TotalFederalTaxAmount { get; set; }
    public decimal TotalStateTaxAmount { get; set; }
    public decimal TotalCheckAmount { get; set; }

    public static DistributionRunReportSummaryResponse SampleResponse()
    {
        return new DistributionRunReportSummaryResponse
        {
            DistributionFrequencyId = 'M',
            DistributionTypeName = "Monthly",
            TotalDistributions = 150,
            TotalGrossAmount = 250000.00M,
            TotalFederalTaxAmount = 37500.00M,
            TotalStateTaxAmount = 12500.00M,
            TotalCheckAmount = 200000.00M
        };
    }
}
