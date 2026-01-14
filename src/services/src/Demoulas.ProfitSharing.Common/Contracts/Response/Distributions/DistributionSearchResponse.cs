using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;

public sealed record DistributionSearchResponse : IIsExecutive
{
    public required long Id { get; set; }
    public required int PaymentSequence { get; set; }
    public required string Ssn { get; set; }
    public long? BadgeNumber { get; set; }
    [MaskSensitive]
    public required string FullName { get; set; }
    public bool IsExecutive { get; set; }
    public bool IsEmployee { get; set; }
    public char FrequencyId { get; set; }
    public required string FrequencyName { get; set; }
    public char StatusId { get; set; }
    public required string StatusName { get; set; }
    public char TaxCodeId { get; set; }
    public required string TaxCodeName { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal FederalTax { get; set; }
    public decimal StateTax { get; set; }
    public decimal CheckAmount { get; set; }
    public int? DemographicId { get; set; }
    public int? BeneficiaryId { get; set; }

    public static DistributionSearchResponse SampleResponse()
    {
        var response = new DistributionSearchResponse
        {
            Id = 1001,
            PaymentSequence = 1,
            Ssn = "XXX-XX-1234",
            BadgeNumber = 701001,
            FullName = "John Doe",
            IsExecutive = false,
            IsEmployee = true,
            FrequencyId = 'W',
            FrequencyName = "Weekly",
            StatusId = 'P',
            StatusName = "Processed",
            TaxCodeId = 'A',
            TaxCodeName = "Standard",
            GrossAmount = 1500.00M,
            FederalTax = 150.00M,
            StateTax = 75.00M,
            CheckAmount = 1275.00M,
            DemographicId = 5001,
            BeneficiaryId = null
        };

        return response;
    }

    /// <summary>
    /// Example data for testing and API documentation.
    /// </summary>
    public static DistributionSearchResponse ResponseExample()
    {
        return new DistributionSearchResponse
        {
            Id = 2001,
            PaymentSequence = 2,
            Ssn = "***-**-5678",
            BadgeNumber = 701002,
            FullName = "Jane Smith",
            IsExecutive = true,
            IsEmployee = false,
            FrequencyId = 'B',
            FrequencyName = "Bi-weekly",
            StatusId = 'P',
            StatusName = "Processed",
            TaxCodeId = 'B',
            TaxCodeName = "Special",
            GrossAmount = 2500.00m,
            FederalTax = 350.00m,
            StateTax = 125.00m,
            CheckAmount = 2025.00m,
            DemographicId = 5002,
            BeneficiaryId = 101
        };
    }
}
