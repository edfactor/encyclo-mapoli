using Demoulas.ProfitSharing.Common.Interfaces.Audit;

namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class DemographicHistory : IDoNotAudit
{
    public long Id { get; set; }
    public int DemographicId { get; set; }
    public DateTimeOffset ValidFrom { get; set; } = new DateTime(1971, 1, 1, 0, 0, 0, DateTimeKind.Local);
    public DateTimeOffset ValidTo { get; set; } = new DateTime(2100, 1, 1, 0, 0, 0, DateTimeKind.Local);

    public required long OracleHcmId { get; set; }
    public int BadgeNumber { get; set; }
    public short StoreNumber { get; set; }
    public string PayClassificationId { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public DateOnly HireDate { get; set; }
    public DateOnly? ReHireDate { get; set; }
    public DateOnly? TerminationDate { get; set; }
    public byte DepartmentId { get; set; }
    public char EmploymentTypeId { get; set; }
    public byte PayFrequencyId { get; set; }
    public char? TerminationCodeId { get; set; }
    public char EmploymentStatusId { get; set; }

    public DateTimeOffset CreatedDateTime { get; set; } = DateTimeOffset.UtcNow;

    public static DemographicHistory FromDemographic(Demographic source, int demographicId)
    {
        var rslt = FromDemographic(source);
        rslt.DemographicId = demographicId;
        return rslt;
    }

    public static DemographicHistory FromDemographic(Demographic source)
    {
        var h = new DemographicHistory()
        {
            DemographicId = source.Id,
            OracleHcmId = source.OracleHcmId,
            BadgeNumber = source.BadgeNumber,
            StoreNumber = source.StoreNumber,
            PayClassificationId = source.PayClassificationId,
            DateOfBirth = source.DateOfBirth,
            HireDate = source.HireDate,
            ReHireDate = source.ReHireDate,
            TerminationDate = source.TerminationDate,
            DepartmentId = source.DepartmentId,
            EmploymentTypeId = source.EmploymentTypeId,
            PayFrequencyId = source.PayFrequencyId,
            TerminationCodeId = source.TerminationCodeId,
            EmploymentStatusId = source.EmploymentStatusId
        };
        return h;
    }
}
