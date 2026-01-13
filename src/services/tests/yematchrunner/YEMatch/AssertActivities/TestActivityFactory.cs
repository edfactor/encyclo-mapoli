using Renci.SshNet;
using YEMatch.Activities;
using YEMatch.ArrangeActivites;
using YEMatch.ReadyActivities;

namespace YEMatch.AssertActivities;

internal static class TestActivityFactory
{
    public static List<IActivity> CreateActivities(
        string dataDirectory,
        SshClient? sshClient = null,
        SftpClient? sftpClient = null,
        bool isNewScramble = true,
        ApiClient? apiClient = null)
    {
        List<IActivity> activities =
        [
            // Test
            new TestPayProfitSelectedColumns(),
            new TestProfitDetailSelectedColumns(),
            new TestEtvaNow(),
            new TestEtvaPrior(),
            new TestViews(),
            new TestEnrollmentComparison(),

            // Arrange
            new ImportReadyDbToSmartDb(),
            new TrimTo14Employees(),
            new DropBadBenesReady { IsNewScramble = isNewScramble },
            new SanityCheckEmployeeAndBenes(),
            new OverwriteBadges(),
            new SetDateOfBirthTo19YearsAgo()
        ];

        // Only add activities that require apiClient if it's provided
        if (apiClient is not null)
        {
            activities.Add(new TestMasterInquiry { ApiClient = apiClient });
            activities.Add(new UpdateNavigation { ApiClient = apiClient });
        }

        // Only add ReadyActivity if SSH clients are provided
        if (sshClient is not null && sftpClient is not null)
        {
            activities.Add(new ReadyActivity(sshClient, sftpClient, "MasterInquiryDumper", "THA-8-10", "", dataDirectory));
        }

        return activities;
    }
}
