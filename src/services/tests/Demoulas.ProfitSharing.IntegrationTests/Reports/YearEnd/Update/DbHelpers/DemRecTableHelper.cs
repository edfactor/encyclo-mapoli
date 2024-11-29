using System.Globalization;
using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update.DbHelpers;

public class DemRecTableHelper
{
#pragma warning disable S2933
    private readonly List<DEM_REC> demographicRecords = new();
    private readonly DEM_REC dem_rec;

    public DemRecTableHelper(OracleConnection connection, DEM_REC dem_rec)
    {
        this.dem_rec = dem_rec;

        string query = "SELECT * FROM PROFITSHARE.DEMOGRAPHICS";
        using (OracleCommand command = new OracleCommand(query, connection))
        {
            using (OracleDataReader? reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    DEM_REC record = new DEM_REC
                    {
                        DEM_BADGE = reader.GetInt64(reader.GetOrdinal("DEM_BADGE")),
                        DEM_SSN = reader.GetInt64(reader.GetOrdinal("DEM_SSN")),
                        PY_NAM =
                            reader.IsDBNull(reader.GetOrdinal("PY_NAM"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("PY_NAM")),
                        PY_LNAME =
                            reader.IsDBNull(reader.GetOrdinal("PY_LNAME"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("PY_LNAME")),
                        PY_FNAME =
                            reader.IsDBNull(reader.GetOrdinal("PY_FNAME"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("PY_FNAME")),
                        PY_MNAME =
                            reader.IsDBNull(reader.GetOrdinal("PY_MNAME"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("PY_MNAME")),
                        PY_STOR = reader.GetInt64(reader.GetOrdinal("PY_STOR")),
                        PY_DP = reader.GetInt64(reader.GetOrdinal("PY_DP")),
                        PY_CLA = reader.GetInt64(reader.GetOrdinal("PY_CLA")),
                        PY_DOB = reader.GetInt64(reader.GetOrdinal("PY_DOB")),
                        PY_FUL =
                            reader.IsDBNull(reader.GetOrdinal("PY_FUL"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("PY_FUL")),
                        PY_FREQ =
                            reader.IsDBNull(reader.GetOrdinal("PY_FREQ"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("PY_FREQ")),
                        PY_TYPE =
                            reader.IsDBNull(reader.GetOrdinal("PY_TYPE"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("PY_TYPE")),
                        PY_SCOD =
                            reader.IsDBNull(reader.GetOrdinal("PY_SCOD"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("PY_SCOD")),
                        PY_HIRE_DT = ParseDate(reader, "PY_HIRE_DT"),
                        PY_FULL_DT = ParseDate(reader, "PY_FULL_DT"),
                        PY_REHIRE_DT = ParseDate(reader, "PY_REHIRE_DT"),
                        PY_TERM_DT = ParseDate(reader, "PY_TERM_DT"),
                        PY_TERM =
                            reader.IsDBNull(reader.GetOrdinal("PY_TERM"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("PY_TERM")),
                        PY_NEW_EMP =
                            reader.IsDBNull(reader.GetOrdinal("PY_NEW_EMP"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("PY_NEW_EMP")),
                        PY_SHOUR = reader.GetDecimal(reader.GetOrdinal("PY_SHOUR")),
                        PY_CLASS_DT = ParseDate(reader, "PY_CLASS_DT")
                    };
                    demographicRecords.Add(record);
                }
            }
        }
    }


    private static DateOnly? ParseDate(OracleDataReader reader, string columnName)
    {
        if (reader.IsDBNull(reader.GetOrdinal(columnName)))
        {
            return null;
        }

        long dateValue = reader.GetInt64(reader.GetOrdinal(columnName));
        string dateString = dateValue.ToString("D8"); // Convert to yyyyMMdd format
        if (DateOnly.TryParseExact(dateString, "yyyyMMdd", null, DateTimeStyles.None, out DateOnly date))
        {
            return date;
        }

        return null;
    }


    public string? getByBadge(DEM_REC demRec)
    {
        DEM_REC l = demographicRecords.Where(dr => dr.DEM_BADGE == demRec.DEM_BADGE).First();
        dem_rec.DEM_BADGE = l.DEM_BADGE;
        dem_rec.DEM_SSN = l.DEM_SSN;
        dem_rec.PY_NAM = l.PY_NAM;
        dem_rec.PY_LNAME = l.PY_LNAME;
        dem_rec.PY_FNAME = l.PY_FNAME;
        dem_rec.PY_MNAME = l.PY_MNAME;
        dem_rec.PY_STOR = l.PY_STOR;
        dem_rec.PY_DP = l.PY_DP;
        dem_rec.PY_CLA = l.PY_CLA;
        dem_rec.PY_DOB = l.PY_DOB;
        dem_rec.PY_FUL = l.PY_FUL;
        dem_rec.PY_FREQ = l.PY_FREQ;
        dem_rec.PY_TYPE = l.PY_TYPE;
        dem_rec.PY_SCOD = l.PY_SCOD;
        dem_rec.PY_HIRE_DT = l.PY_HIRE_DT;
        dem_rec.PY_FULL_DT = l.PY_FULL_DT;
        dem_rec.PY_REHIRE_DT = l.PY_REHIRE_DT;
        dem_rec.PY_TERM_DT = l.PY_TERM_DT;
        dem_rec.PY_TERM = l.PY_TERM;
        dem_rec.PY_NEW_EMP = l.PY_NEW_EMP;
        dem_rec.PY_SHOUR = l.PY_SHOUR;
        dem_rec.PY_CLASS_DT = l.PY_CLASS_DT;
        return "00";
    }
}
