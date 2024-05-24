using Demoulas.ProfitSharing.Data.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
