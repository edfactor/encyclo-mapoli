using Demoulas.ProfitSharing.Common.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Demoulas.ProfitSharing.Data.Contexts.ValueConverters;

public class GenderEnumConverter : ValueConverter<Gender, char>
{
    public GenderEnumConverter() : base(
        v => ConvertToDatabase(v),
        v => ConvertToEntity(v))
    {
    }

    private static char ConvertToDatabase(Gender gender)
    {
        return (char)gender;
    }

    private static Gender ConvertToEntity(char gender)
    {
        return (Gender)gender;
    }
}
