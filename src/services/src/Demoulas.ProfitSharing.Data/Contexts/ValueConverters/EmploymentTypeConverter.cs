using Demoulas.ProfitSharing.Common.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Demoulas.ProfitSharing.Data.Contexts.ValueConverters;

public class EmploymentTypeConverter : ValueConverter<EmploymentType, string>
{
    public EmploymentTypeConverter() : base(
        v => ConvertToDatabase(v),
        v => ConvertToEntity(v))
    {
    }

    private static string ConvertToDatabase(EmploymentType employmentType)
    {
        return ((char)employmentType).ToString();
    }

    private static EmploymentType ConvertToEntity(string employmentType)
    {
        return (EmploymentType)employmentType[0];
    }
}
