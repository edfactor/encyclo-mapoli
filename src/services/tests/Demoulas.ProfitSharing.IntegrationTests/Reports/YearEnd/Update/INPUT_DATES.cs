namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class INPUT_DATES
{
   public string? EFFECTIVE_DATE { get; set; } // PIC X(4)
   public string? EFFECTIVE_CENT { get; set; } // PIC XX.
   public string? EFFECTIVE_YEAR { get; set; } // PIC XX.
   public long EFFECT_DATE { get; set; } // PIC 9(4)
   public long EFFECT_CC { get; set; } // PIC 99.
   public long EFFECT_YEAR { get; set; } // PIC 99.
}

/*
 01  INPUT-DATES.
      03 EFFECTIVE-DATE             PIC X(4) VALUE SPACE.
      03 DUMMY REDEFINES EFFECTIVE-DATE.
         05 EFFECTIVE-CENT          PIC XX.
         05 EFFECTIVE-YEAR          PIC XX.
      03 EFFECT-DATE                PIC 9(4) VALUE ZERO.
      03 DUMMY REDEFINES EFFECT-DATE.
         05 EFFECT-CC               PIC 99.
         05 EFFECT-YEAR             PIC 99.
*/
