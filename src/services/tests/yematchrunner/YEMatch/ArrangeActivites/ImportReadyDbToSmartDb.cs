using YEMatch.Activities;

namespace YEMatch.ArrangeActivites;

// This task takes the READY database and imports it directly to SMART.  Handy for isolating smart activities.
internal sealed class ImportReadyDbToSmartDb : IActivity
{
    private const string ProjectDirectory = "prj/smart-profit-sharing";
    public const string CliProject = "src/services/src/Demoulas.ProfitSharing.Data.Cli";

    private const string Args =
        "run -- import-from-ready --connection-name ProfitSharing --source-schema \"tbherrmann\" --sql-file \"../../../../src/database/ready_import/SQL copy all from ready to smart ps.sql\"";

    private static readonly string _homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    private readonly string _workingDir = Path.Combine(_homeDirectory, ProjectDirectory, CliProject);

    public string Name()
    {
        return "ImportReadyDbToSmartDb";
    }

    public Task<Outcome> Execute()
    {
        int res = ScriptRunner.Run(true, _workingDir, "dotnet", Args);
        if (res != 0)
        {
            return Task.FromResult(new Outcome(Name(), Name(), "", OutcomeStatus.Error, "Problem setting up database\n", null, true));
        }

        return Task.FromResult(new Outcome(Name(), Name(), "", OutcomeStatus.Ok, "Database setup complete.\n", null, true));
    }
}
