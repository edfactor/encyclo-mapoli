namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.UpdateSummary;

public static class Pay450Comparisons
{
    private const string Red = "\u001b[31m";
    private const string Reset = "\u001b[0m";

    public static string ToComparisonString(Pay450Record a, Pay450Record b)
    {
        string FieldStr(string val, int width) => (val ?? "").PadRight(width);
        string FieldInt(int? val, int width) => $"{(val?.ToString() ?? "").PadLeft(width)}";
        string FieldDec(decimal val, int width) => val.ToString("N2").PadLeft(width);

        string DiffInt(int? valA, int? valB, int width) =>
            valA == valB
                ? FieldInt(valB, width)
                : $"{Red}{FieldInt(valB, width)}{Reset}";

        string DiffDec(decimal valA, decimal valB, int width) =>
            valA == valB
                ? FieldDec(valB, width)
                : $"{Red}{FieldDec(valB, width)}{Reset}";

        return string.Join(" ",
            FieldStr(a.BadgeAndStore, 12),
            FieldStr(a.Name, 20),
            FieldDec(a.BeforeAmount, 12),
            DiffDec(a.BeforeAmount, b.BeforeAmount, 12),
            FieldDec(a.BeforeVested, 12),
            DiffDec(a.BeforeVested, b.BeforeVested, 12),
            FieldInt(a.BeforeYears, 6),
            DiffInt(a.BeforeYears, b.BeforeYears, 6),
            FieldInt(a.BeforeEnroll, 6),
            DiffInt(a.BeforeEnroll, b.BeforeEnroll, 6),
            FieldDec(a.AfterAmount, 12),
            DiffDec(a.AfterAmount, b.AfterAmount, 12),
            FieldDec(a.AfterVested, 12),
            DiffDec(a.AfterVested, b.AfterVested, 12),
            FieldInt(a.AfterYears, 6),
            DiffInt(a.AfterYears, b.AfterYears, 6),
            FieldInt(a.AfterEnroll, 6),
            DiffInt(a.AfterEnroll, b.AfterEnroll, 6)
        );
    }

    public static string ToComparisonHeader()
    {
        string H(string label, int width) => label.PadRight(width);
        return string.Join(" ",
            H("BadgeStore", 12),
            H("Name", 20),
            H("BefAmt", 12),
            H("Cmp", 12),
            H("BefVest", 12),
            H("Cmp", 12),
            H("BefYrs", 6),
            H("Cmp", 6),
            H("BefEnr", 6),
            H("Cmp", 6),
            H("AftAmt", 12),
            H("Cmp", 12),
            H("AftVest", 12),
            H("Cmp", 12),
            H("AftYrs", 6),
            H("Cmp", 6),
            H("AftEnr", 6),
            H("Cmp", 6)
        );
    }

}
