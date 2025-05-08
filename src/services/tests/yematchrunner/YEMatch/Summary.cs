using System.Text.Json;

namespace YEMatch.YEMatch;

public static class Summary
{
    private static readonly int nameWidth = 35;

    public static void print(string filename)
    {
        string directory = "../../.."; // Change this if needed
        string searchPattern = "outcomes-*.json";

        if (filename == "latest")
        {
// Get the latest file based on name (timestamps are part of filenames)
            FileInfo? latestFile = new DirectoryInfo(directory)
                .GetFiles(searchPattern)
                .OrderByDescending(f => f.Name) // Sorting by name since format is yyyyMMdd-HHmmss
                .FirstOrDefault();

            if (latestFile == null)
            {
                Console.WriteLine("No matching files found.");
                return;
            }

            filename = latestFile.FullName;
        }

        string json = File.ReadAllText(filename);
        List<Outcome>? outcomes = JsonSerializer.Deserialize<List<Outcome>>(json);


        if (outcomes == null || outcomes.Count == 0)
        {
            Console.WriteLine("No outcomes found in the file.");
            return;
        }

        if (outcomes.Count == 1)
        {
            if (outcomes[0].isSmart)
            {
                summarizeOneSide(outcomes!);
            }
            else
            {
                summarizeReadyAndSmart(outcomes!);
            }

            return;
        }

        // Outcomes are only 1 side.
        if (outcomes?[0].isSmart == outcomes?[1].isSmart)
        {
            summarizeOneSide(outcomes!);
        }
        else
        {
            summarizeReadyAndSmart(outcomes!);
        }
    }

    private static void summarizeOneSide(List<Outcome> outcomes)
    {
        Console.WriteLine($" {"Name".PadRight(nameWidth)}, {"READY",8}, {"SMART",8}");
        TimeSpan sideTook = TimeSpan.Zero;
        foreach (Outcome outcome in outcomes)
        {
            string readyTook = outcome.isSmart ? "" : Summarize(outcome);
            string smartSummary = outcome.isSmart ? Summarize(outcome) : "";

            Console.WriteLine(
                $"{Program.ModChar(outcome)}{TruncateAndPad(outcome.ActivityLetterNumber.PadRight(4) + " " + outcome.Name)}, {readyTook,8}, {smartSummary}");

            sideTook = sideTook.Add(outcome.took ?? TimeSpan.Zero);
        }

        Console.WriteLine($"---- Total Time  {timeFmt(sideTook)}\n");
        Console.WriteLine(" *            = activities where READY/SMART change the contents of the database.");
        Console.WriteLine(" ToBeDone     = means the work on SMART or YEMatch is incomplete");
        Console.WriteLine(" No Operation = means no activity for ready/smart, these are external actors or time.");
    }

    private static void summarizeReadyAndSmart(List<Outcome> outcomes)
    {
        Console.WriteLine($" {"Name".PadRight(nameWidth)},     {"READY",8}, {"SMART",8}");
        TimeSpan readyTime = TimeSpan.Zero;
        TimeSpan smartTime = TimeSpan.Zero;

        if (outcomes?.Count == 1)
        {
            Console.WriteLine("Only 1 outcome.");
            Console.WriteLine($"{Summarize(outcomes[0])}");
        }
        else
        {
            foreach (Outcome[] outcomePair in outcomes!.Chunk(2))
            {
                Outcome readyOutcome = outcomePair[0];
                Outcome smartOutcome = outcomePair[1];
                string ksh = readyOutcome.Name.Split(" ")[0];
                ksh = ksh.StartsWith('!') ? ksh.Substring(1) : ksh;
                Console.WriteLine(
                    $"{Program.ModChar(readyOutcome)}{(readyOutcome.ActivityLetterNumber.PadRight(4) + " " + ksh).PadRight(nameWidth)},     {Summarize(readyOutcome),8},    {Summarize(smartOutcome)}");

                readyTime = readyTime.Add(readyOutcome.took ?? TimeSpan.Zero);
                smartTime = smartTime.Add(smartOutcome.took ?? TimeSpan.Zero);
            }
        }

        Console.WriteLine($"\n{"---- Total Time".PadRight(nameWidth)} ,     {timeFmt(readyTime),8},{timeFmt(smartTime),8}\n");
        Console.WriteLine(" *            = activities where READY/SMART change the contents of the database.");
        Console.WriteLine(" ToBeDone     = means the work on SMART or YEMatch is incomplete");
        Console.WriteLine(" No Operation = means no activity for ready/smart, these are external actors or time.");
    }

    private static string timeFmt(TimeSpan ts)
    {
        return $"{ts.TotalMinutes:00}:{ts.Seconds:00}";
    }

    private static string Summarize(Outcome outcome)
    {
        if (outcome.Status == OutcomeStatus.Ok)
        {
            string outMsg = "";
            if (outcome.isSmart)
            {
                outMsg = " " + outcome.Message.Replace("\n", ", ").Trim();
                int lineLimit = 180;
                if (outMsg.Length > lineLimit)
                {
                    outMsg = outMsg.Substring(0, lineLimit) + "...";
                }
            }

            return $"{outcome.took!.Value.TotalMinutes:00}:{outcome.took!.Value.Seconds:00}{outMsg}";
        }

        if (outcome.Status == OutcomeStatus.NoOperation)
        {
            if (outcome.isSmart)
            {
                return "--:-- NOP";
            }

            return "--:--";
        }

        if (outcome.Status == OutcomeStatus.Error)
        {
            return "ERROR";
        }

        if (OutcomeStatus.ToBeDone == outcome.Status)
        {
            return "??:??   ToBeDone          - " + outcome.Message;
        }

#pragma warning disable S112
        throw new Exception("Unknown status");
#pragma warning restore S112
    }

    private static string TruncateAndPad(string input)
    {
        int paranDex = input.IndexOf("(");
        if (paranDex > 0)
        {
            input = input.Substring(0, paranDex);
        }

        string truncated = input.Length > 35 ? input.Substring(0, 35) : input;
        return truncated.PadRight(35);
    }
}
