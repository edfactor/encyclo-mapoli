using Demoulas.ProfitSharing.Data.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class DepartmentEnumConverter : ValueConverter<DepartmentEnum, byte>
{
    public DepartmentEnumConverter() : base(
        v => ConvertToDatabase(v),
        v => ConvertToEntity(v))
    {
    }

    private static byte ConvertToDatabase(DepartmentEnum department)
    {
        return ((byte)department);
    }

    private static DepartmentEnum ConvertToEntity(byte department)
    {
        return (DepartmentEnum)department;
    }
}
