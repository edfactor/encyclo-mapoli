namespace Demoulas.ProfitSharing.Data.Extensions;
public static class SsnExtensions
{
    public static string MaskSsn(this long ssn)
    {
        Span<char> ssnSpan = stackalloc char[9];
        ssn.ToString().AsSpan().CopyTo(ssnSpan[(9 - ssn.ToString().Length)..]);
        ssnSpan[..(9 - ssn.ToString().Length)].Fill('0');

        Span<char> resultSpan = stackalloc char[11];
        "XXX-XX-".AsSpan().CopyTo(resultSpan);
        ssnSpan.Slice(5, 4).CopyTo(resultSpan[7..]);

        return new string(resultSpan);
    }
}
