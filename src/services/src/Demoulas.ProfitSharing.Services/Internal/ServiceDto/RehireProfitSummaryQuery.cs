namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;

internal sealed record RehireProfitSummaryQuery
{
    internal required int BadgeNumber { get; set; }
    internal required string? FullName { get; set; }
    internal required int Ssn { get; set; }
    internal required short StoreNumber { get; set; }
    internal required DateOnly HireDate { get; set; }
    internal required DateOnly? TerminationDate { get; set; }
    internal required DateOnly ReHiredDate { get; set; }
    internal required byte CompanyContributionYears { get; set; }

    internal decimal NetBalanceLastYear { get; set; }
    internal decimal VestedBalanceLastYear { get; set; }
    internal char EmploymentStatusId { get; set; }
    internal string? EmploymentStatus { get; set; }
    internal List<RehireProfitSummaryQueryDetails> Details { get; set; } = [];
}

internal sealed record RehireProfitSummaryQueryDetails
{
    internal required short ProfitYear { get; set; }
    internal required decimal Forfeiture { get; set; }
    internal required string? Remark { get; set; }
    internal required decimal HoursCurrentYear { get; set; }
    internal required byte EnrollmentId { get; set; }
    internal required string EnrollmentName { get; set; }
    internal byte ProfitCodeId { get; set; }
}
