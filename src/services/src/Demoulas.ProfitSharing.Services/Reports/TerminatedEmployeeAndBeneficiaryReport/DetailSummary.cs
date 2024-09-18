namespace Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;

internal sealed record DetailSummary
(
    decimal Distribution,
    decimal Forfeiture,
    decimal BeneficiaryAllocation
);
