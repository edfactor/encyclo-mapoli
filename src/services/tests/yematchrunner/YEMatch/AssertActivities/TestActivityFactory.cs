namespace YEMatch;

internal static class TestActivityFactory
{
    public static List<IActivity> CreateActivities(string dataDirectory)
    {
        return
        [
            new TestPayProfitSelectedColumns(),
            new TestProfitDetailSelectedColumns(),
            new TestEtvaNow(),
            new TestEtvaPrior(),
            new TrimTo14Employees(),
            new ReadyDatabase2Smart(),
            new SetDateOfBirthTo19YearsAgo(),
            new OverwriteBadges()
        ];
    }
}
