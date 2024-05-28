using Demoulas.ProfitSharing.Common.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Demoulas.ProfitSharing.Data.ValueConverters;

public class GenderEnumConverter : ValueConverter<GenderEnum, char>
{
    public GenderEnumConverter() : base(
        v => ConvertToDatabase(v),
        v => (GenderEnum)ConvertToEntity(v))
    {
    }

    private static char ConvertToDatabase(GenderEnum gender)
    {
        return (char)gender;
    }

    private static GenderEnum ConvertToEntity(char gender)
    {
        return (GenderEnum)gender;
    }
}
