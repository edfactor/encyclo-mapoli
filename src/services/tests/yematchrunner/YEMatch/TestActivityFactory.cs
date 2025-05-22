
namespace YEMatch;

internal static class TestActivityFactory

{
    public static List<IActivity> CreateActivities(string dataDirectory)
    {
        return
        [
            new TestPayProfitSelect(),
            new TestProfitDetailSelected(),
            new TestEtvaNow(),
            new TestEtvaPrior()
        ];
    }
}
