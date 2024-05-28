using Demoulas.ProfitSharing.Common.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Demoulas.ProfitSharing.Data.Contexts.ValueConverters;

public class TerminationCodeEnumConverter : ValueConverter<TerminationCode, char>
{
    public TerminationCodeEnumConverter() : base(
        v => ConvertToDatabase(v),
        v => ConvertToEntity(v))
    {
    }

    private static char ConvertToDatabase(TerminationCode department)
    {
        return (char)department;
    }

    private static TerminationCode ConvertToEntity(char department)
    {
        return (TerminationCode)department;
    }
}
