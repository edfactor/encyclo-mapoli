using System.Security.Cryptography;
using System.Text;

namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;

/// <summary>
///     A slice of a member in the profit sharing system. An instance of this slice is either employee
///     information or beneficiary information.  People are grouped by their full name.
/// </summary>
internal sealed record MemberSlice
{
    internal short PsnSuffix { get; init; }
    internal required int BadgeNumber { get; init; }
    internal int Ssn { get; init; }
    internal decimal HoursCurrentYear { get; init; }
    internal char EmploymentStatusCode { get; init; }
    internal string? FullName { get; init; } = string.Empty;
    internal string FirstName { get; init; } = string.Empty;
    internal string LastName { get; init; } = string.Empty;
    internal byte YearsInPs { get; init; }
    internal DateOnly? BirthDate { get; init; }
    internal DateOnly? TerminationDate { get; init; }
    internal decimal IncomeRegAndExecCurrentYear { get; init; }
    internal char? TerminationCode { get; init; }
    internal byte? ZeroCont { get; init; }
    internal byte EnrollmentId { get; init; }
    internal decimal Etva { get; init; }
    public bool IsBeneficiaryAndEmployee { get; set; }
    public bool IsOnlyBeneficiary { get; set; }
    public short ProfitYear { get; set; }
    public bool IsExecutive { get; set; }

    public byte[] CheckSum
    {
        get
        {
            byte[] bytes = Encoding.UTF8.GetBytes(
                $"{PsnSuffix}{BadgeNumber}{Ssn}{HoursCurrentYear}{EmploymentStatusCode}{FullName}{FirstName}{LastName}{YearsInPs}{BirthDate}{TerminationDate}{IncomeRegAndExecCurrentYear}{TerminationCode}{ZeroCont}{EnrollmentId}{Etva}{IsBeneficiaryAndEmployee}{IsOnlyBeneficiary}{ProfitYear}");
            return SHA256.HashData(bytes);
        }
    }
}
