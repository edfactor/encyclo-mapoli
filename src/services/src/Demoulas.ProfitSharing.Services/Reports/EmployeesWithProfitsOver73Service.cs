using System.Numerics;
using System.Security.Principal;
using System.Text;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Demoulas.ProfitSharing.Common.Contracts.Request.FrozenReportsByAgeRequest;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Demoulas.ProfitSharing.Services.Reports;

/// <summary>
/// Service for generating the PROF-LETTER73 adhoc report - Employees with profits over age 73.
/// </summary>
public class EmployeesWithProfitsOver73Service : IEmployeesWithProfitsOver73Service
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly TotalService _totalService;

    public EmployeesWithProfitsOver73Service(
        IProfitSharingDataContextFactory dataContextFactory,
        IDemographicReaderService demographicReaderService,
        TotalService totalService)
    {
        _dataContextFactory = dataContextFactory;
        _demographicReaderService = demographicReaderService;
        _totalService = totalService;
    }

    public Task<ReportResponseBase<EmployeesWithProfitsOver73DetailDto>> GetEmployeesWithProfitsOver73Async(
        EmployeesWithProfitsOver73Request request,
        CancellationToken cancellationToken = default)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            // Get all current demographics with contact info
            var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx);

            // Calculate age threshold - employees must be over 73 years old
            var today = DateOnly.FromDateTime(DateTime.Today);
            var ageThresholdDate = today.AddYears(-73);

            // Start with base query for employees over 73
            var query = demographicQuery.Where(d => d.DateOfBirth <= ageThresholdDate);

            // Apply badge number filter if provided
            if (request.BadgeNumbers != null && request.BadgeNumbers.Any())
            {
                query = query.Where(d => request.BadgeNumbers.Contains(d.BadgeNumber));
            }

            // Query for employees over 73 with all required fields
            var employeesOver73 = await query
                .Select(d => new
                {
                    d.Ssn,
                    d.BadgeNumber,
                    FullName = d.ContactInfo.FullName,
                    FirstName = d.ContactInfo.FirstName,
                    LastName = d.ContactInfo.LastName,
                    MiddleInitial = d.ContactInfo.MiddleName != null ? d.ContactInfo.MiddleName[0].ToString() : string.Empty,
                    Address = d.Address.Street,
                    City = d.Address.City,
                    State = d.Address.State,
                    Zip = d.Address.PostalCode,
                    d.DateOfBirth,
                    d.TerminationDate,
                    d.EmploymentStatusId,
                    EmploymentStatusName = d.EmploymentStatus != null ? d.EmploymentStatus.Name : string.Empty
                })
                .ToListAsync(cancellationToken);

            var employeeSsns = employeesOver73.Select(e => e.Ssn).ToHashSet();

            // Get total balances for these employees
            var totalBalances = await _totalService.GetTotalBalanceSet(ctx, request.ProfitYear)
                .Where(tb => employeeSsns.Contains(tb.Ssn))
                .Where(tb => tb.TotalAmount > 0) // Only include employees with positive balances
                .ToDictionaryAsync(tb => tb.Ssn, cancellationToken);

            // Build detail records with pagination support
            var detailRecords = employeesOver73
                .Where(e => totalBalances.ContainsKey(e.Ssn))
                .Select(employee =>
                {
                    totalBalances.TryGetValue(employee.Ssn, out var balance);
                    var age = today.Year - employee.DateOfBirth.Year;

                    return new EmployeesWithProfitsOver73DetailDto
                    {
                        BadgeNumber = employee.BadgeNumber,
                        Name = employee.FullName ?? string.Empty,
                        FirstName = employee.FirstName,
                        LastName = employee.LastName,
                        MiddleInitial = employee.MiddleInitial ?? string.Empty,
                        Address = employee.Address ?? string.Empty,
                        City = employee.City ?? string.Empty,
                        State = employee.State ?? string.Empty,
                        Zip = employee.Zip ?? string.Empty,
                        Status = employee.EmploymentStatusName,
                        Ssn = employee.Ssn.MaskSsn(),
                        DateOfBirth = employee.DateOfBirth,
                        TerminationDate = employee.TerminationDate,
                        Age = age,
                        Balance = balance?.TotalAmount ?? 0,
                        RequiredMinimumDistributions = 0.0M 
                    };
                })
                .AsQueryable();

            // Apply pagination
            var paginatedResults = await detailRecords.ToPaginationResultsAsync(request, cancellationToken);

            return new ReportResponseBase<EmployeesWithProfitsOver73DetailDto>
            {
                ReportName = "PROF-LETTER73: Employees with Profits Over Age 73",
                ReportDate = DateTimeOffset.UtcNow,
                StartDate = today,
                EndDate = today,
                Response = paginatedResults
            };
        }, cancellationToken);
    }

    public async Task<string> GetEmployeesWithProfitsOver73FormLetterAsync(
        EmployeesWithProfitsOver73Request request,
        CancellationToken cancellationToken = default)
    {
        var result = await GetEmployeesWithProfitsOver73Async(request, cancellationToken);

        // Generate form letters
        return GenerateFormLettersForOver73Employees(result);
    }


    private static string GenerateFormLettersForOver73Employees(ReportResponseBase<EmployeesWithProfitsOver73DetailDto> report)
    {

        if (!report.Response.Results.Any())
        {
            return "No employees over 73 with positive balances found.";
        }

        var letter = new StringBuilder();
        var space7 = new string(' ', 7);

        foreach (EmployeesWithProfitsOver73DetailDto emp in report.Response.Results)
        {
            #region Beginning of letter
            letter.AppendLine();
            letter.AppendLine("DJDE JDE=QPS073,JDL=PAYROL,END,;");
            letter.AppendLine($"{DateTime.Now.Month.ToString("MMMM")} {DateTime.Now.Year}");
            #endregion

            ////#region Return address
            ////letter.AppendLine(" DEMOULAS PROFIT SHARING PLAN AND TRUST");
            ////letter.AppendLine(" 875 EAST STREET");
            ////letter.AppendLine(" TEWKSBURY, MA 01876");
            ////#endregion

            #region Spacing
            letter.AppendLine();
            letter.AppendLine();
            #endregion

            #region Member information
            letter.AppendLine($"{space7}{emp.FirstName}{(emp.MiddleInitial != string.Empty ? " " : "")}{emp.MiddleInitial} {emp.LastName}");
            letter.AppendLine($"{space7}{emp.Address}");
            letter.AppendLine($"{space7}{emp.City}, {emp.State} {emp.Zip}");
            #endregion

            #region Spacing
            letter.AppendLine();
            letter.AppendLine();
            letter.AppendLine($"{space7}RE: Demoulas Profit Sharing Plan & Trust(the Plan); Required Minimum");
            letter.AppendLine($"{space7}Distribution(RMD)");
            #endregion

            #region Spacing
            letter.AppendLine();
            letter.AppendLine();
            #endregion

            #region Salutation
            letter.AppendLine($"{space7}Dear Profit Sharing Plan Participant,");
            #endregion

            letter.AppendLine();

            letter.AppendLine($"{space7}Federal law dictates that minimum distribution rules apply to all participants in");  
            letter.AppendLine($"{space7}the Demoulas Profit Sharing Plan who are over the age of 73.These rules");
            letter.AppendLine($"{space7}establish a mandatory required beginning date for participants to begin");
            letter.AppendLine($"{space7}receiving payments from the Plan.");
            letter.AppendLine();
            letter.AppendLine($"{space7}You are receiving this letter because you are no longer employed by the");
            letter.AppendLine($"{space7}Company and have a balance in the Plan that requires a minimum payment");
            letter.AppendLine($"{space7}from your Plan account for the current tax year.The calculated payment");
            letter.AppendLine($"{space7}amount is reflected via the attached check.");
            letter.AppendLine();
            letter.AppendLine($"{space7}Alternatively, you may already be receiving payments from the Plan, but your");
            letter.AppendLine($"{space7}payments do not meet the minimum requirements. In this case, the attached");
            letter.AppendLine($"{space7}check represents the additional distribution amount needed to meet your");
            letter.AppendLine($"{space7}required minimum distribution for the year.  ");
            letter.AppendLine();
            letter.AppendLine($"{space7}Please note that this mailing only represents the minimum distribution amount");
            letter.AppendLine($"{space7}for the current tax year.You are free to request a higher distribution amount at");
            letter.AppendLine($"{space7}any time.Please contact us should you wish to request any additional payment");
            letter.AppendLine($"{space7}or if you have any other questions.");
            letter.AppendLine();
            letter.AppendLine($"{space7}Sincerely");
            letter.AppendLine();
            letter.AppendLine($"{space7}Demoulas Profit Sharing Plan & Trust");
            letter.AppendLine();
            letter.AppendLine();
            letter.AppendLine();
            letter.AppendLine($"{space7}{emp.BadgeNumber}");
            ////#region Spacing
            ////letter.AppendLine();
            ////letter.AppendLine();
            ////letter.AppendLine();
            ////letter.AppendLine();
            ////#endregion

            ////#region Salutation
            ////letter.AppendLine($"{space7}Dear {emp.FirstName}:");
            ////#endregion

            ////#region Spacing
            ////letter.AppendLine();
            ////letter.AppendLine();
            ////#endregion

            ////#region Body of letter
            ////letter.AppendLine($"{space7}As you have reached age 73, you are now required to begin taking required minimum");
            ////letter.AppendLine($"{space7}distributions (RMDs) from your Demoulas Profit Sharing Plan and Trust account under");
            ////letter.AppendLine($"{space7}federal tax law.");
            ////#endregion

            ////#region Spacing
            ////letter.AppendLine();
            ////letter.AppendLine();
            ////#endregion

            ////#region Instructions
            ////letter.AppendLine($"{space7}Please contact the Plan Administrator to discuss your distribution options and ensure");
            ////letter.AppendLine($"{space7}compliance with IRS regulations. You may reach us at:");
            ////#endregion

            ////#region Spacing
            ////letter.AppendLine();
            ////letter.AppendLine();
            ////#endregion

            ////#region Contact information
            ////letter.AppendLine($"{space25}Demoulas Profit Sharing Plan and Trust");
            ////letter.AppendLine($"{space25}875 East Street");
            ////letter.AppendLine($"{space25}Tewksbury, MA  01876");
            ////letter.AppendLine($"{space25}Phone: (978) 851-8000");
            ////#endregion

            ////#region Spacing
            ////letter.AppendLine();
            ////letter.AppendLine();
            ////#endregion

            ////#region Closing
            ////letter.AppendLine($"{space7}Sincerely,");
            ////#endregion

            ////#region Spacing
            ////letter.AppendLine();
            ////letter.AppendLine();
            ////letter.AppendLine();
            ////#endregion

            ////#region Signature
            ////letter.AppendLine($"{space7}DEMOULAS PROFIT SHARING PLAN & TRUST");
            ////#endregion

            ////#region Printer Control
            ////letter.AppendLine();
            ////letter.AppendLine();
            ////letter.AppendLine("DJDE JDE=DISNO1,JDL=PAYROL,END,;");
            ////letter.AppendLine();
            ////letter.AppendLine();
            ////letter.AppendLine();
            ////letter.AppendLine("DJDE JDE=DISNO2,JDL=PAYROL,END,;");
            ////letter.AppendLine();
            ////letter.AppendLine();
            ////letter.AppendLine();
            ////letter.AppendLine("DJDE JDE=DISNO3,JDL=PAYROL,END,;");
            ////letter.AppendLine();
            ////letter.AppendLine();
            ////letter.AppendLine();
            ////letter.AppendLine("DJDE JDE=DISNO4,JDL=PAYROL,END,;");
            ////letter.AppendLine();
            ////letter.AppendLine();
            ////letter.AppendLine();
            ////letter.AppendLine("DJDE JDE=DISNO5,JDL=PAYROL,END,;");
            ////letter.AppendLine();
            ////letter.AppendLine();
            ////letter.AppendLine();
            ////letter.AppendLine("DJDE JDE=BENDS1,JDL=PAYROL,END,;");
            ////letter.AppendLine();
            ////letter.AppendLine();
            ////letter.AppendLine();
            ////letter.AppendLine("DJDE JDE=ACKNRC,JDL=PAYROL,END,;");
            ////letter.Append("\f"); // Form feed to end the letter
            ////#endregion

            letter.Append("\f"); // Form feed to end the letter
        }

        return letter.ToString();
    }
}
