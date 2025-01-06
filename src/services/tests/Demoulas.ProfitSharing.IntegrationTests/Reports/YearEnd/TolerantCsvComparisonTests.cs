namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;

/* Ensure the CSV comparision is working correctly */
public class TolerantCsvComparisonTests
{
    [Fact]
    public void EnsureTwoCsvsWithDifferentFormattingCompare()
    {
        // Note the spacing and leading zeros
        string csv1 = """
                      BADGE,NAME,STR,EXEC HRS,EXEC DOLS,ORA HRS CUR,ORA DOLS CUR,FREQ,STATUS
                      0700253,"DUFFY, SOPHIE"                           ,004,      1558,  27586.33,         0,         0,1,I
                      0700298,"MURPHY, JASON"                           ,004,      1165,   51077.3,         0,         0,1,I
                      """;

        string csv2 = """
                      BADGE,NAME,STR,EXEC HRS,EXEC DOLS,ORA HRS CUR,ORA DOLS CUR,FREQ,STATUS
                      700253,"DUFFY, SOPHIE",4,1558,27586.33,0,0,1,i
                      700298,"MURPHY, JASON",4,1165,51077.3,0,0,1,i
                      """;

        TolerantCsvComparisonUtility.ShouldBeTheSame(csv1, csv2);
    }
}
