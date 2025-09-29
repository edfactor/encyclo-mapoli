using YEMatch.YEMatch.Activities;
using YEMatch.YEMatch.ArrangeActivites;

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
            new FixFrozenReady(),
            new SanityCheckEmployeeAndBenes(),
            new OverwriteBadges(),
            new SetDateOfBirthTo19YearsAgo(),
            new UpdateNavigation()
            , new Give2023Hours()
        ];
    }
}
