using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Demoulas.ProfitSharing.Data.Entities;
public class DemographicHistory
{
    public DemographicHistory()
    {
        CreatedDateTime = DateTime.UtcNow;
        ValidTo = new DateTime(2100, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        ValidFrom = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }

    public long Id { get; set; }
    public int DemographicId { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }

    public required long OracleHcmId { get; set; }
    public int Ssn { get; set; }
    public int EmployeeId { get; set; }
    public short StoreNumber { get; set; }
    public byte PayClassificationId { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public DateOnly HireDate { get; set; }
    public DateOnly? ReHireDate { get; set; }
    public DateOnly? TerminationDate { get; set; }
    public byte DepartmentId { get; set; }
    public char EmploymentTypeId { get; set; }
    public byte PayFrequencyId { get; set; }
    public char? TerminationCodeId { get; set; }
    public char EmploymentStatusId { get; set; }

    public DateTime CreatedDateTime { get; set; }

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
            Ssn = source.Ssn,
            EmployeeId = source.EmployeeId,
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

