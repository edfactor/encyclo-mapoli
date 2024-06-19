using Demoulas.ProfitSharing.Common.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Demoulas.ProfitSharing.Data.Contexts.ValueConverters;

public class PayFrequencyConverter : ValueConverter<PayFrequency, char>
{
    public PayFrequencyConverter() : base(
        v => ConvertToDatabase(v),
        v => ConvertToEntity(v))
    {
    }

    private static char ConvertToDatabase(PayFrequency payFrequency)
    {
        return (char)payFrequency;
    }

    private static PayFrequency ConvertToEntity(char payFrequency)
    {
        return (PayFrequency)payFrequency;
    }
}
