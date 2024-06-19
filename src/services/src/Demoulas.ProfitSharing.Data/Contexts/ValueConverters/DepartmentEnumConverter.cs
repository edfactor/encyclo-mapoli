using Demoulas.ProfitSharing.Common.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Demoulas.ProfitSharing.Data.Contexts.ValueConverters;

public class DepartmentEnumConverter : ValueConverter<Department, byte>
{
    public DepartmentEnumConverter() : base(
        v => ConvertToDatabase(v),
        v => ConvertToEntity(v))
    {
    }

    private static byte ConvertToDatabase(Department department)
    {
        return (byte)department;
    }

    private static Department ConvertToEntity(byte department)
    {
        return (Department)department;
    }
}
