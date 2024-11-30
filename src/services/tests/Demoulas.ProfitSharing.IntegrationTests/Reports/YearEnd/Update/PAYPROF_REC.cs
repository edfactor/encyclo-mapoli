namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class PAYPROF_REC
{
    public long PAYPROF_BADGE { get; set; } // PIC 9(7).
    public int PAYPROF_SSN { get; set; } // PIC 9(9).
    public long PY_PS_ENROLLED { get; set; } // PIC 99.
    public long PY_PS_YEARS { get; set; } // PIC 99.
    public decimal PY_PS_AMT { get; set; } // PIC S9(7)V99.
    public long PY_PROF_NEWEMP { get; set; } // PIC 99.
    public long PY_PROF_POINTS { get; set; } // PIC S9(5).
    public decimal PY_PROF_CONT { get; set; } // PIC S9(7)V99.
    public decimal PY_PROF_FORF { get; set; } // PIC S9(7)V99.
    public decimal PY_PROF_EARN { get; set; } // PIC S9(7)V99.
    public decimal PY_PS_ETVA { get; set; } // PIC S9(8)V99.
    public decimal PY_PROF_ETVA { get; set; } // PIC S9(7)V99.
    public decimal PY_PROF_EARN2 { get; set; } // PIC S9(7)V99.
    public decimal PY_PROF_ETVA2 { get; set; } // PIC S9(7)V99.
}
