using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Demoulas.ProfitSharing.Data.Entities;

public class PayProfit
{
    /// <summary>
    /// Employee badge number
    /// </summary>
    public required long EmployeeBadge { get; set; }

    /// <summary>
    /// Employee social security number
    /// </summary>
    public required string EmployeeSSN { get; set; }

    /// <summary>
    /// Hours towards Profit Sharing in the current year (updated weekly)
    /// </summary>
    public required decimal HoursPSYear { get; set; }

    /// <summary>
    /// Dollars earned by the employee in the current year (updated weekly)
    /// </summary>
    public required decimal EarningsPSYear { get; set; }

    /// <summary>
    /// Number of weeks worked in the current year
    /// </summary>
    public required byte WeeksWorkedYear { get; set; }

    /// <summary>
    /// Years the company has contributed to the employee 
    /// </summary>
    public required byte CompanyContributionYears { get; set; }

    /// <summary>
    /// Date the last PS Certificate was issued
    /// </summary>
    public DateOnly? PSCertificateIssuedDate { get; set; }

    public required Enrollment Enrollment { get; set; }
}
