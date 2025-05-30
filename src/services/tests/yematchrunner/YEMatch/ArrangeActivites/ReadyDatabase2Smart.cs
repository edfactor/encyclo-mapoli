namespace YEMatch;

// This task takes the READY database and imports it directly to SMART.  Handy for isolating smart activities.
internal sealed class ReadyDatabase2Smart : IActivity
{
    public string Name()
    {
        return "R2S"; // Short for Ready to Smart
    }

    public Task<Outcome> Execute()
    {
        // Consider using CLI tool for reset the smart schema to stock 

        int res = ScriptRunner.Run(false, "import-bh-from-ready"); // Good enough and fast
        if (res != 0)
        {
            return Task.FromResult(new Outcome(Name(), Name(), "", OutcomeStatus.Error, "Problem setting up database\n", null, true));
        }

        return Task.FromResult(new Outcome(Name(), Name(), "", OutcomeStatus.Ok, "Database setup complete.\n", null, true));
    }
}
