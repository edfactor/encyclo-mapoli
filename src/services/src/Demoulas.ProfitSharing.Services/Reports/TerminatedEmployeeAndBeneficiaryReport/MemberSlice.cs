using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;

/// <summary>
/// A slice of a member in the profit sharing system. An instance of this slice is either employee
/// information or beneficiary information.  People are grouped by their full name.
/// </summary>
internal sealed record MemberSlice(
    long Psn,
    long Ssn,
    decimal HoursCurrentYear,
    decimal NetBalanceLastYear,
    decimal VestedBalanceLastYear,
    char EmploymentStatusCode,
    string FullName,
    string FirstName,
    string MiddleInitial,
    string LastName,
    long YearsInPs,
    DateOnly? BirthDate,
    DateOnly? TerminationDate,
    decimal IncomeRegAndExecCurrentYear,
    char? TerminationCode,
    byte? ZeroCont,
    byte Enrolled,
    decimal Etva,
    decimal BeneficiaryAllocation);
