using System.Text.Json;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Common.Common;
internal static class Constants
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

    internal static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };
    }
}
