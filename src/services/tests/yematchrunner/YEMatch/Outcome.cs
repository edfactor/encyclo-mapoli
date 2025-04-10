namespace YEMatch;

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
}