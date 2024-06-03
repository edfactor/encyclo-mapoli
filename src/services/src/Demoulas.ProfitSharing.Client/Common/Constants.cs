using System.Text.Json;
using Demoulas.ProfitSharing.Common.Enums;

namespace Demoulas.ProfitSharing.Client.Common;
internal sealed class Constants
{
    internal const string HttpClient = "ProfitSharing.Client";

    internal static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }
}
