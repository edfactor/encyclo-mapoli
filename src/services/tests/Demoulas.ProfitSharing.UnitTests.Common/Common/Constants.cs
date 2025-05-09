using System.Text.Json;
using Demoulas.ProfitSharing.Data.Entities.Virtual;

namespace Demoulas.ProfitSharing.UnitTests.Common.Common;
public static class Constants
{
    public static class Http
    {
        internal const string HttpClient = "ProfitSharing.Client";
        internal const string ReportsHttpClient = "ProfitSharing.Client.Reports";
        internal const string ReportsDownloadClient = "ProfitSharing.Client.Reports.Download";
    }

    public static class ErrorMessages
    {
        internal const string ReportNotFound = "Report not found";
    }

    public static List<ParticipantTotal> FakeParticipantTotals { get; set; } = new List<ParticipantTotal>();
    public static List<ParticipantTotalVestingBalance> FakeParticipantTotalVestingBalances { get; set; } = new List<ParticipantTotalVestingBalance>();

    internal static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };
    }
}
