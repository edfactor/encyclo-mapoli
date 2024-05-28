using Demoulas.ProfitSharing.Common.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Demoulas.ProfitSharing.Data.Contexts.ValueConverters;

public class EmploymentTypeConverter : ValueConverter<EmploymentTypeEnum, string>
{
    public EmploymentTypeConverter() : base(
        v => ConvertToDatabase(v),
        v => ConvertToEntity(v))
    {
    }

    private static string ConvertToDatabase(EmploymentTypeEnum employmentType)
    {
        return ((char)employmentType).ToString();
    }

    private static EmploymentTypeEnum ConvertToEntity(string employmentType)
    {
        return (EmploymentTypeEnum)employmentType[0];
    }
}
