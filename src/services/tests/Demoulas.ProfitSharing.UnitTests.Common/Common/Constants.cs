using System.Text.Json;

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

    internal static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };
    }
}
