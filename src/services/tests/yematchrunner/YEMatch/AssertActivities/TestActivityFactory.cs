using YEMatch.YEMatch.Activities;
using YEMatch.YEMatch.ArrangeActivites;
using YEMatch.YEMatch.ReadyActivities;

namespace YEMatch.YEMatch.AssertActivities;

internal static class TestActivityFactory
{
    public static List<IActivity> CreateActivities(string dataDirectory)
    {
        return
        [
            // Test
            new TestPayProfitSelectedColumns(),
            new TestProfitDetailSelectedColumns(),
            new TestEtvaNow(),
            new TestEtvaPrior(),
            new TestMasterInquiry(),
            new TestViews(),

            // Arrange
            new ImportReadyDbToSmartDb(),
            new TrimTo14Employees(),
            new DropBadBenesReady(),
            new SanityCheckEmployeeAndBenes(),
            new OverwriteBadges(),
            new SetDateOfBirthTo19YearsAgo(),
            new UpdateNavigation(),
            new SmartPay456(),
            new ReadyActivity(ReadyActivityFactory.sshClient!, ReadyActivityFactory.SftpClient!, true, "MasterInquiryDumper", "THA-8-10", "", dataDirectory),

        ];
    }
}
