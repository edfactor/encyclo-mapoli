using Demoulas.ProfitSharing.Common.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Demoulas.ProfitSharing.Data.Contexts.ValueConverters;

public class PayClassificationEnumConverter : ValueConverter<PayClassification, short>
{
    public PayClassificationEnumConverter() : base(
        v => ConvertToDatabase(v),
        v => ConvertToEntity(v))
    {
    }

    private static short ConvertToDatabase(PayClassification department)
    {
        return (short)department;
    }

    private static PayClassification ConvertToEntity(short department)
    {
        return (PayClassification)department;
    }
}
