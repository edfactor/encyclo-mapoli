namespace YEMatch;

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

            // Arrange
            new ImportReadyDbToSmartDb(),
            new TrimTo14Employees(),
            new DropBadBenes(),
            new OverwriteBadges(),
            new SetDateOfBirthTo19YearsAgo()
        ];
    }
}
