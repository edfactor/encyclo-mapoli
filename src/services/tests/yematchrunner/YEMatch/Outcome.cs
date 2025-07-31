namespace YEMatch.YEMatch;

public enum OutcomeStatus
{
    Ok,
    NoOperation,
    Error,
    ToBeDone
}

public sealed record Outcome(
    string ActivityLetterNumber, // aka A10
    string Name, // Short name of the activity, "PROF TERM"
    string fullcommand, // command and argumens where appopriate
    OutcomeStatus Status, // Did this run OK or not?
    string Message, // it not ok, this is more detail
    TimeSpan? took, /*Should be nullable TimeSpan.  Unfortunately at the moment a status also gets plopped in here. */
    bool isSmart, // used mostly for formatting
    string StandardOut = "",
    string StandardError = ""
)
{
    public override string ToString()
    {
        return $"{ActivityLetterNumber}: {Status} - {Name}\n    {Indented(Message)}";
    }

    private static string Indented(string message)
    {
        return message.Replace("\n", "\n    ");
    }

    // When running activities in parallel - at the end the two outcomes get merged.  
    // Merging two activities is awkward, but gets the job done for now
    public Outcome Merge(Outcome secondOutcome)
    {
        OutcomeStatus mergedStatus = Status;
        if (secondOutcome.Status == OutcomeStatus.Error)
        {
            mergedStatus = OutcomeStatus.Error;
        }

        TimeSpan? mergeToLongerTime = took;
        if (took != null && secondOutcome.took != null && secondOutcome.took.Value > took.Value)
        {
            mergeToLongerTime = secondOutcome.took;
        }

        return new Outcome(ActivityLetterNumber + "/" + secondOutcome.ActivityLetterNumber,
            Name + "/" + secondOutcome.Name,
            fullcommand, mergedStatus, $"first:{Message}\nsecond:{secondOutcome.Message}",
            mergeToLongerTime, isSmart, $"firstOut:{StandardOut}\nsecondOut:{secondOutcome.StandardOut}",
            $"firstErr:{StandardError}\nsecondErr:{secondOutcome.StandardError}");
    }
}
