namespace Demoulas.ProfitSharing.IntegrationTests.TotalSvc;

public record PayProfitData
{
    public int BadgeNumber { get; set; }
    public int Ssn { get; set; }

    public decimal Amount { get; set; }

    public decimal VestedAmount { get; set; }

    public int Years { get; set; }

    public int Enrollment { get; set; }

    public int Frequency { get; set; }

    public DateOnly? TerminationDate { get; set; }

    public char? TerminationCodeId { get; set; }
}
