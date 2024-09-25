namespace Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;

/// <summary>
/// A member in the profit sharing system.   Can be an employee a beneficiary or both.
/// </summary>
internal sealed record Member 
   (
    long Psn,
    string FullName,
    string FirstName,
    string MiddleInitial,
    string LastName,
    DateOnly? Birthday,
    decimal HoursCurrentYear,
    decimal EarningsCurrentYear,
    long Ssn,
    DateOnly? TerminationDate,
    char? TerminationCode,
    decimal DistributionAmount,
    decimal BeginningAmount,
    decimal CurrentVestedAmount,
    decimal ForfeitAmount,
    decimal EndingBalance,
    long YearsInPlan,
    decimal VestedBalance,
    byte? ZeroCont,
    byte Enrolled,
    decimal Evta,
    decimal BeneficiaryAllocation
);


