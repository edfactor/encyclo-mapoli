using System.Diagnostics;
using System;
using System.Text;
using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;


public class ProftShareUpdateTests
{
    [Fact]
    public void EnsureSmartReportMatchesCobolReport()
    {
        // string connectionString = "User Id=SYS;Password=Password1!;Data Source=localhost:1521/ORCLCDB;DBA Privilege=SYSDBA"
        // string connectionString = "User Id=mtpr3;Password=mtpr3;Data Source=tdcexa-scan1:1521/test10"
        //string connectionString =
            // "Data Source= (DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = tdcexa-scan1)(PORT = 1521)) ) (CONNECT_DATA = (SERVICE_NAME = test10d) (SERVER = DEDICATED)));User Id=mtpr3;Password=mtpr3;"
        string connectionString =
            "Data Source= (DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = tdcexa-scan1)(PORT = 1521)) ) (CONNECT_DATA = (SERVICE_NAME = test10d) (SERVER = DEDICATED)));User Id=bherrmann;Password=bherrmann;";

        using (var connection = new OracleConnection(connectionString))
        {
            try
            {
                connection.Open();
                Console.WriteLine("Connected to Oracle Database.");

                mainBit(connection);
#if false
                string query = "SELECT COUNT(*) FROM all_tables";
                using (var command = new OracleCommand(query, connection))
                {
                    var count = command.ExecuteScalar();
                    Console.WriteLine($"Total table count: {count}");
                }
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
            finally
            {
                connection.Close();
                Console.WriteLine("Connection closed.");
            }
        }
    }

     static void mainBit(OracleConnection connection){

    //- * Meta-sw (2) = 1 : Special Run
        //- * Meta-sw (3) = 1 : Do NOT ask for Input Values.
        //- * Meta-sw (4) = 1 : Manual Adjustments
        //- * Meta-sw (5) = 1 : Secondary Earnings
        //- * Meta-sw (8) = 1 : Do NOT update PAYR/PAYBEN

        Dictionary<int, int> metaSw = new();
        metaSw[2] = 0;
        metaSw[3] = 1;
        metaSw[4] = 0;
        metaSw[5] = 0;
        metaSw[8] = 1;  // reports only mode

        PAY444 pay444 = new();
        string etext = "2023*Something";
        pay444.connection = connection;
        pay444.m015MainProcessing(metaSw, etext);

        File.WriteAllText("/tmp/ref3.txt", string.Join("\n", pay444.outputLines),  Encoding.ASCII);

        var startInfo = new ProcessStartInfo
        {
            FileName = "/Program Files/Meld/Meld.exe",
            ArgumentList = { "/tmp/ref1", "/tmp/ref3.txt" },
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        // Start the process
        using var process = Process.Start(startInfo);


    }
}
