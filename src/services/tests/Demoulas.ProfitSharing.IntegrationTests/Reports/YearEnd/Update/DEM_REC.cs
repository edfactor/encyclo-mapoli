namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class DEM_REC
{
    public long DEM_BADGE { get; set; } // PIC 9(7).
    public long DEM_SSN { get; set; } // PIC 9(9).
    public string? PY_NAM { get; set; } // PIC X(40).
    public string? PY_LNAME { get; set; } // PIC X(25).
    public string? PY_FNAME { get; set; } // PIC X(25).
    public string? PY_MNAME { get; set; } // PIC X(25).
    public long PY_STOR { get; set; } // PIC 9(3).
    public long PY_DP { get; set; } // PIC 9(1).
    public long PY_CLA { get; set; } // PIC 9(3).
    public long PY_DOB { get; set; } // PIC 9(8).
    public string? PY_FUL { get; set; } // PIC X(2).
    public string? PY_FREQ { get; set; } // PIC X(1).
    public string? PY_TYPE { get; set; } // PIC X(1).
    public string? PY_SCOD { get; set; } // PIC X(1).
    public DateOnly? PY_HIRE_DT { get; set; } // PIC 9(8).
    public DateOnly? PY_FULL_DT { get; set; } // PIC 9(8).
    public DateOnly? PY_REHIRE_DT { get; set; } // PIC 9(8).
    public DateOnly? PY_TERM_DT { get; set; } // PIC 9(8).
    public string? PY_TERM { get; set; } // PIC X(1).
    public string? PY_NEW_EMP { get; set; } // PIC X(1).
    public decimal PY_SHOUR { get; set; } // PIC 9(3)V99.
    public DateOnly? PY_CLASS_DT { get; set; } // PIC 9(8).
}

/*

 01 DEM-REC.
     03 DEM-KEY.
        05 DEM-BADGE    PIC 9(7).
     03 DEM-SSN         PIC 9(9).
     03 PY-NAM          PIC X(40).
     03 PY-LNAME        PIC X(25).
     03 PY-FNAME        PIC X(25).
     03 PY-MNAME        PIC X(25).
     03 PY-STOR         PIC 9(3).
     03 PY-DP           PIC 9(1).
     03 PY-CLA          PIC 9(3).
     03 PY-ADD          PIC X(30).
     03 PY-ADD2         PIC X(30).
     03 PY-CITY         PIC X(25).
     03 PY-STATE        PIC X(2).
     03 PY-ZIP          PIC 9(5).
     03 PY-DOB          PIC 9(8).
     03 PY-FUL          PIC X(2).
     03 PY-FREQ         PIC X(1).
     03 PY-TYPE         PIC X(1).
     03 PY-SCOD         PIC X(1).
     03 PY-HIRE-DT      PIC 9(8).
     03 PY-FULL-DT      PIC 9(8).
     03 PY-REHIRE-DT    PIC 9(8).
     03 PY-TERM-DT      PIC 9(8).
     03 PY-TERM         PIC X(1).
     03 PY-ASSIGN-ID    PIC 9(15).
     03 PY-ASSIGN-DESC  PIC X(15).
     03 PY-NEW-EMP      PIC X(1).
     03 PY-GENDER       PIC X(1).
     03 PY-EMP-TELNO    PIC 9(10).
     03 PY-SHOUR        PIC 9(3)V99.
     03 PY-SET-PWD      PIC X(1).
     03 PY-SET-PWD-DT   PIC X(14).
     03 PY-CLASS-DT     PIC 9(8).
     03 PY-GUID         PIC X(256).
*/
