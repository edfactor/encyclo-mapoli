namespace YEMatch.YEMatch.AssertActivities.MasterInquiry;

/* Used to compare two "OutFL" instances. One from READY and one from SMART.   Ignores the SSN and Error Message fields. */
public static class OutFLComparer
{
    public static bool IsSame(OutFL a, OutFL b)
    {
        return Normalize(a).Equals(Normalize(b));
    }

    public static OutFL Normalize(OutFL record)
    {
        return record with
        {
            OUT_SSN = string.Empty,
            OUT_ERR_MESG = string.Empty,
            OUT_HRS = Math.Round(record.OUT_HRS, 2),
            OUT_ENROLLED = record.OUT_ENROLLED.Trim(),
            OUT_BEGIN_BAL = Math.Round(record.OUT_BEGIN_BAL, 2),
            OUT_BEGIN_VEST = Math.Round(record.OUT_BEGIN_VEST, 2),
            OUT_CURRENT_BAL = Math.Round(record.OUT_CURRENT_BAL, 2),
            OUT_VESTING_PCT = Math.Round(record.OUT_VESTING_PCT, 2),
            OUT_VESTING_AMT = Math.Round(record.OUT_VESTING_AMT, 2)
        };
    }

    public static string FormatValue(object? value)
    {
        return value switch
        {
            null => string.Empty,
            decimal d => d.ToString("N2"),
            int i => i.ToString(),
            string s => s,
            bool b => b ? "YES" : "NO",
            _ => value?.ToString() ?? string.Empty
        };
    }

    public static string MarkupIfDifferent<T>(Dictionary<string, int> diffMap, string field, T ready, T smart)
    {
        bool areEqual = EqualityComparer<T>.Default.Equals(ready, smart);
        if (typeof(T) == typeof(decimal))
        {
            areEqual = Math.Round(Convert.ToDecimal(ready), 2) == Math.Round(Convert.ToDecimal(smart), 2);
        }

        if (!areEqual)
        {
            if (!diffMap.TryAdd(field, 1))
            {
                diffMap[field]++;
            }

            return $"⚠️{FormatValue(smart)}";
        }

        return FormatValue(smart);
    }
}
