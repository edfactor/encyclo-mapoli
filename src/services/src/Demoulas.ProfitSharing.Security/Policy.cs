using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Security;
public static class Policy
{
    public static readonly string CanViewYearEndReports = "CAN_VIEW_YEAR_END_REPORTS";
    public static readonly string CanAddDemographics = "CAN_ADD_DEMOGRAPHICS";
    public static readonly string CanViewPayClassificationTypes = "CAN_VIEW_PAY_CLASSIFICATION_TYPES";
    public static readonly string CanGetPayProfitRecords = "CAN_GET_PAY_PROFIT_RECORDS";
}
