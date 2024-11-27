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
                        PY_ADD =
                            reader.IsDBNull(reader.GetOrdinal("PY_ADD"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("PY_ADD")),
                        PY_ADD2 =
                            reader.IsDBNull(reader.GetOrdinal("PY_ADD2"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("PY_ADD2")),
                        PY_CITY =
                            reader.IsDBNull(reader.GetOrdinal("PY_CITY"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("PY_CITY")),
                        PY_STATE =
                            reader.IsDBNull(reader.GetOrdinal("PY_STATE"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("PY_STATE")),
                        PY_ZIP = reader.GetInt64(reader.GetOrdinal("PY_ZIP")),
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
                        PY_ASSIGN_ID = reader.GetInt64(reader.GetOrdinal("PY_ASSIGN_ID")),
                        PY_ASSIGN_DESC =
                            reader.IsDBNull(reader.GetOrdinal("PY_ASSIGN_DESC"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("PY_ASSIGN_DESC")),
                        PY_NEW_EMP =
                            reader.IsDBNull(reader.GetOrdinal("PY_NEW_EMP"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("PY_NEW_EMP")),
                        PY_GENDER =
                            reader.IsDBNull(reader.GetOrdinal("PY_GENDER"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("PY_GENDER")),
                        PY_EMP_TELNO = reader.GetInt64(reader.GetOrdinal("PY_EMP_TELNO")),
                        PY_SHOUR = reader.GetDecimal(reader.GetOrdinal("PY_SHOUR")),
                        PY_SET_PWD =
                            reader.IsDBNull(reader.GetOrdinal("PY_SET_PWD"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("PY_SET_PWD")),
                        PY_SET_PWD_DT =
                            reader.IsDBNull(reader.GetOrdinal("PY_SET_PWD_DT"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("PY_SET_PWD_DT")),
                        PY_CLASS_DT = ParseDate(reader, "PY_CLASS_DT"),
                        PY_GUID = reader.IsDBNull(reader.GetOrdinal("PY_GUID"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("PY_GUID"))
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
        dem_rec.PY_ADD = l.PY_ADD;
        dem_rec.PY_ADD2 = l.PY_ADD2;
        dem_rec.PY_CITY = l.PY_CITY;
        dem_rec.PY_STATE = l.PY_STATE;
        dem_rec.PY_ZIP = l.PY_ZIP;
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
        dem_rec.PY_ASSIGN_ID = l.PY_ASSIGN_ID;
        dem_rec.PY_ASSIGN_DESC = l.PY_ASSIGN_DESC;
        dem_rec.PY_NEW_EMP = l.PY_NEW_EMP;
        dem_rec.PY_GENDER = l.PY_GENDER;
        dem_rec.PY_EMP_TELNO = l.PY_EMP_TELNO;
        dem_rec.PY_SHOUR = l.PY_SHOUR;
        dem_rec.PY_SET_PWD = l.PY_SET_PWD;
        dem_rec.PY_SET_PWD_DT = l.PY_SET_PWD_DT;
        dem_rec.PY_CLASS_DT = l.PY_CLASS_DT;
        dem_rec.PY_GUID = l.PY_GUID;
        return "00";
    }
}
