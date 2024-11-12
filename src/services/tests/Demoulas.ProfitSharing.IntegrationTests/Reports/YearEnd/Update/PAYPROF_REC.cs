namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class PAYPROF_REC {
    public long PAYPROF_BADGE { get; set; } // PIC 9(7).
    public long PAYPROF_SSN { get; set; } // PIC 9(9).
    public decimal PY_PH { get; set; } // PIC 9(4)V99.
    public decimal PY_PD { get; set; } // PIC 9(6)V99.
    public long PY_WEEKS_WORK { get; set; } // PIC 99.
    public string? PY_PROF_CERT { get; set; } // PIC X.
    public long PY_PS_ENROLLED { get; set; } // PIC 99.
    public long PY_PS_YEARS { get; set; } // PIC 99.
    public long PY_PROF_BENEFICIARY { get; set; } // PIC 9(1).
    public long PY_PROF_INITIAL_CONT { get; set; } // PIC 9(4)v9.
    public decimal PY_PS_AMT { get; set; } // PIC S9(7)V99.
    public decimal PY_PS_VAMT { get; set; } // PIC S9(7)V99.
    public decimal PY_PH_LASTYR { get; set; } // PIC S9(5)V99.
    public decimal PY_PD_LASTYR { get; set; } // PIC S9(6)V99.
    public long PY_PROF_NEWEMP { get; set; } // PIC 99.
    public long PY_PROF_POINTS { get; set; } // PIC S9(5).
    public decimal PY_PROF_CONT { get; set; } // PIC S9(7)V99.
    public decimal PY_PROF_FORF { get; set; } // PIC S9(7)V99.
    public string? PY_VESTED_FLAG { get; set; } // PIC X.
    public long PY_PROF_MAXCONT { get; set; } // PIC 99.
    public long PY_PROF_ZEROCONT { get; set; } // PIC 99.
    public long PY_WEEKS_WORK_LAST { get; set; } // PIC 99.
    public decimal PY_PROF_EARN { get; set; } // PIC S9(7)V99.
    public decimal PY_PS_ETVA { get; set; } // PIC S9(8)V99.
    public decimal PY_PRIOR_ETVA { get; set; } // PIC S9(8)V99.
    public decimal PY_PROF_ETVA { get; set; } // PIC S9(7)V99.
    public decimal PY_PROF_EARN2 { get; set; } // PIC S9(7)V99.
    public decimal PY_PROF_ETVA2 { get; set; } // PIC S9(7)V99.
    public long PY_PH_EXEC { get; set; } // PIC 9(4)v99.
    public decimal PY_PD_EXEC { get; set; } // PIC 9(6)V99.

}
/*

 01  PAYPROF-REC.
     05  PAYPROF-KEY.
         07  PAYPROF-BADGE        PIC  9(7).
     05  PAYPROF-SSN              PIC  9(9).
     05  PY-PH                 PIC  9(4)V99.
     05  PY-PD                 PIC  9(6)V99.
     05  PY-WEEKS-WORK         PIC  99.
     05  PY-PROF-CERT          PIC  X.
     05  PY-PS-ENROLLED        PIC  99.
     05  PY-PS-YEARS           PIC  99.
     05  PY-PROF-BENEFICIARY   PIC  9(1).
     05  PY-PROF-INITIAL-CONT  PIC  9(4)v9.
     05  PY-PS-AMT             PIC  S9(7)V99.
     05  PY-PS-VAMT            PIC  S9(7)V99.
     05  PY-PH-LASTYR          PIC  S9(5)V99.
     05  PY-PD-LASTYR          PIC  S9(6)V99.
     05  PY-PROF-NEWEMP        PIC  99.
     05  PY-PROF-POINTS        PIC  S9(5).
     05  PY-PROF-CONT          PIC  S9(7)V99.
     05  PY-PROF-FORF          PIC  S9(7)V99.
     05  PY-VESTED-FLAG        PIC  X.
     05  PY-PROF-MAXCONT       PIC  99.
     05  PY-PROF-ZEROCONT      PIC  99.
     05  PY-WEEKS-WORK-LAST    PIC  99.
     05  PY-PROF-EARN          PIC  S9(7)V99.
     05  PY-PS-ETVA            PIC  S9(8)V99.
     05  PY-PRIOR-ETVA         PIC  S9(8)V99.
     05  PY-PROF-ETVA          PIC  S9(7)V99.
     05  PY-PROF-EARN2         PIC  S9(7)V99.
     05  PY-PROF-ETVA2         PIC  S9(7)V99.     
     05  PY-PH-EXEC            PIC  9(4)v99.
     05  PY-PD-EXEC            PIC  9(6)V99. 
*/
