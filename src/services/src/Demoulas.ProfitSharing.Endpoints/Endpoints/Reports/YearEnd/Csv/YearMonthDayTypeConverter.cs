using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public partial class PayrollDuplicateSsnsOnPayprofitEndpoint
{
    public sealed class YearMonthDayTypeConverter:DefaultTypeConverter
    {
        public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value == null)
            {
                return null;
            }

            var d = (DateOnly)value;
            return d.ToString("YYYYMMDD");
        }
    }
}
