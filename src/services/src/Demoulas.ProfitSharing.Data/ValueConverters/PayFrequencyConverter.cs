using Demoulas.ProfitSharing.Data.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class PayFrequencyConverter : ValueConverter<PayFrequencyEnum, char>
{
    public PayFrequencyConverter() : base(
        v => ConvertToDatabase(v),
        v => ConvertToEntity(v))
    {
    }

    private static char ConvertToDatabase(PayFrequencyEnum payFrequency)
    {
        return ((char)payFrequency);
    }

    private static PayFrequencyEnum ConvertToEntity(char payFrequency)
    {
        return (PayFrequencyEnum)payFrequency;
    }
}
