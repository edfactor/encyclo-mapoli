using System.Text.Json;
using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Microsoft.EntityFrameworkCore;
using Moq;

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

    public static Mock<DbSet<ParticipantTotal>> FakeParticipantTotals { get; set; } = new Mock<DbSet<ParticipantTotal>>();
    public static Mock<DbSet<ParticipantTotal>> FakeEtvaTotals { get; set; } = new Mock<DbSet<ParticipantTotal>>();
    public static Mock<DbSet<ProfitShareTotal>> ProfitShareTotals { get; set; } = new Mock<DbSet<ProfitShareTotal>>();
    public static Mock<DbSet<ParticipantTotalVestingBalance>> FakeParticipantTotalVestingBalances { get; set; } = new Mock<DbSet<ParticipantTotalVestingBalance>>();

    internal static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };
    }
}
