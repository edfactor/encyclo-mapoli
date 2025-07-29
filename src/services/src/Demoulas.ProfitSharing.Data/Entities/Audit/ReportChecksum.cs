using Demoulas.ProfitSharing.Data.Entities.Base;
using Demoulas.ProfitSharing.Data.Entities.Navigations;

namespace Demoulas.ProfitSharing.Data.Entities.Audit;

public sealed class ReportChecksum : ModifiedBase
{
    public int Id { get; set; }
    public string ReportType { get; set; } = string.Empty; // REPORT_TYPE NVARCHAR2(50) NOT NULL
    public short ProfitYear { get; set; } // PROFIT_YEAR NUMBER(4) NOT NULL
    public string RequestJson { get; set; } = string.Empty; // CHECKSUM_DATA CLOB NOT NULL
    public string ReportJson { get; set; } = string.Empty; // CHECKSUM_DATA CLOB NOT NULL

    public List<KeyValuePair<string, KeyValuePair<decimal, byte[]>>> KeyFieldsChecksumJson { get; set; } = new(); }
