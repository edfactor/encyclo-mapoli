using System.Text;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Extensions;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.PrintFormatting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Demoulas.ProfitSharing.Services.Reports;

/// <summary>
/// Service for generating the PROF-LETTER73 adhoc report - Employees with profits over age 73.
/// </summary>
public class EmployeesWithProfitsOver73Service : IEmployeesWithProfitsOver73Service
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly TotalService _totalService;
    private readonly ICalendarService _calendarService;
    private readonly TimeProvider _timeProvider;
    private readonly IProfitSharingAuditService _profitSharingAuditService;
    private readonly DjdeDirectiveOptions _djdeDirectiveOptions;

    public EmployeesWithProfitsOver73Service(
        IProfitSharingDataContextFactory dataContextFactory,
        IDemographicReaderService demographicReaderService,
        TotalService totalService,
        ICalendarService calendarService,
        TimeProvider timeProvider,
        IProfitSharingAuditService profitSharingAuditService,
        IOptions<DjdeDirectiveOptions> djdeDirectiveOptions)
    {
        _dataContextFactory = dataContextFactory;
        _demographicReaderService = demographicReaderService;
        _totalService = totalService;
        _calendarService = calendarService;
        _timeProvider = timeProvider;
        _profitSharingAuditService = profitSharingAuditService;
        _djdeDirectiveOptions = djdeDirectiveOptions.Value;
    }

    public Task<ReportResponseBase<EmployeesWithProfitsOver73DetailDto>> GetEmployeesWithProfitsOver73Async(
        EmployeesWithProfitsOver73Request request,
        CancellationToken cancellationToken = default)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            // Get fiscal year dates for payment calculation
            var calendarInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);
            var fiscalStartDate = calendarInfo.FiscalBeginDate;
            var fiscalEndDate = calendarInfo.FiscalEndDate;

            // Get all current demographics with contact info
            var demographicQuery = await _demographicReaderService.BuildDemographicQueryAsync(ctx);

            // Calculate age threshold - employees must be over 73 years old
            DateOnly today = DateOnly.FromDateTime(_timeProvider.GetLocalNow().DateTime);
            var ageThresholdDate = today.AddYears(-73);

            // Start with base query for employees over 73
            var query = demographicQuery.Where(d => d.DateOfBirth <= ageThresholdDate);

            // Apply badge number filter if provided
            if (request.BadgeNumbers != null && request.BadgeNumbers.Any())
            {
                query = query.Where(d => request.BadgeNumbers.Contains(d.BadgeNumber));
            }

            // Get total balances for these employees (SQL join to avoid large IN lists)
            var totalBalanceQuery = _totalService.GetTotalBalanceSet(ctx, request.ProfitYear)
                .TagWith($"GetTotalBalances-Over73-{request.ProfitYear}")
                .Where(tb => tb.TotalAmount > 0);

            // Query for employees over 73 with positive balances and all required fields
            var employeesOver73 = await (
                    from d in query
                    join tb in totalBalanceQuery on d.Ssn equals tb.Ssn
                    select new
                    {
                        d.Ssn,
                        d.BadgeNumber,
                        FullName = d.ContactInfo.FullName,
                        Address = d.Address.Street,
                        City = d.Address.City,
                        State = d.Address.State,
                        Zip = d.Address.PostalCode,
                        d.DateOfBirth,
                        d.TerminationDate,
                        d.EmploymentStatusId,
                        EmploymentStatusName = d.EmploymentStatus != null ? d.EmploymentStatus.Name : string.Empty,
                        TotalAmount = tb.TotalAmount.HasValue ? tb.TotalAmount.Value : 0m
                    })
                .TagWith($"GetEmployeesOver73WithBalances-{request.ProfitYear}")
                .ToListAsync(cancellationToken);

            if (employeesOver73.Count == 0)
            {
                return new ReportResponseBase<EmployeesWithProfitsOver73DetailDto>
                {
                    ReportName = "PROF-LETTER73: Employees with Profits Over Age 73",
                    ReportDate = DateTimeOffset.UtcNow,
                    StartDate = fiscalStartDate,
                    EndDate = fiscalEndDate,
                    Response = new PaginatedResponseDto<EmployeesWithProfitsOver73DetailDto>(request)
                    {
                        Results = [],
                        Total = 0
                    }
                };
            }

            HashSet<int> employeeSsns = employeesOver73.Select(e => e.Ssn).ToHashSet();

            // Load all RMD factors from database (ages 73-99)
            var rmdFactors = await ctx.RmdsFactorsByAge
                .TagWith("GetRmdFactors-Over73Report")
                .ToDictionaryAsync(r => (int)r.Age, r => r.Factor, cancellationToken);

            // Get payment profit codes (codes that represent distributions/payments)
            byte[] paymentProfitCodes = ProfitDetailExtensions.GetProfitCodesForBalanceCalc();

            // Calculate payments within fiscal year for these employees
            // Payment records are in PROFIT_DETAIL where:
            // - ProfitCodeId is a payment/distribution code
            // - ProfitYear <= request.ProfitYear (payments can span multiple years)
            // - We'll use YearToDate to approximate fiscal year boundary
            var paymentsInFiscalYear = await ctx.ProfitDetails
                .TagWith($"GetPaymentsInFiscalYear-{request.ProfitYear}")
                .Where(pd => employeeSsns.Contains(pd.Ssn))
                .Where(pd => paymentProfitCodes.Contains(pd.ProfitCodeId))
                .Where(pd => pd.ProfitYear == request.ProfitYear)
                .GroupBy(pd => pd.Ssn)
                .Select(g => new
                {
                    Ssn = g.Key,
                    TotalPayments = g.Sum(pd => Math.Abs(pd.Forfeiture)) // Forfeiture is negative for payments
                })
                .ToListAsync(cancellationToken);
            var paymentsBySsn = paymentsInFiscalYear.ToLookup(x => x.Ssn);

            // Build detail records with pagination support
            var detailRecords = employeesOver73
                .Select(employee =>
                {
                    decimal balance = employee.TotalAmount;
                    int age = today.Year - employee.DateOfBirth.Year;

                    // Get RMD factor for this age (default to 0 if age not found)
                    decimal factor = rmdFactors.GetValueOrDefault(age, 0m);

                    // Calculate RMD: Balance ÷ Factor (protect against divide by zero)
                    // IMPORTANT: Parentheses around (balance?.TotalAmount ?? 0) ensure correct order of operations
                    decimal rmd = factor > 0 ? Math.Round(balance / factor, 2, MidpointRounding.AwayFromZero) : 0m;

                    // Get payments made in the fiscal year
                    decimal paymentsInYear = paymentsBySsn[employee.Ssn].FirstOrDefault()?.TotalPayments ?? 0m;

                    // Calculate suggested RMD check amount based on de minimis threshold
                    decimal currentBalance = balance;
                    decimal suggestRmdCheckAmount = currentBalance <= request.DeMinimusValue
                        ? currentBalance  // De minimis: liquidate entire account
                        : Math.Max(0, rmd - paymentsInYear);  // Above threshold: RMD minus payments already received

                    return new EmployeesWithProfitsOver73DetailDto
                    {
                        BadgeNumber = employee.BadgeNumber,
                        FullName = employee.FullName ?? string.Empty,
                        Address = employee.Address ?? string.Empty,
                        City = employee.City ?? string.Empty,
                        State = employee.State ?? string.Empty,
                        Zip = employee.Zip ?? string.Empty,
                        Status = employee.EmploymentStatusName,
                        Ssn = employee.Ssn.MaskSsn(),
                        DateOfBirth = employee.DateOfBirth,
                        TerminationDate = employee.TerminationDate,
                        Age = age,
                        Balance = currentBalance,
                        Factor = factor,
                        Rmd = rmd,
                        PaymentsInProfitYear = paymentsInYear,
                        SuggestRmdCheckAmount = suggestRmdCheckAmount
                    };
                })
                .AsQueryable();

            // Apply pagination
            var paginatedResults = await detailRecords.ToPaginationResultsAsync(request, cancellationToken);

            return new ReportResponseBase<EmployeesWithProfitsOver73DetailDto>
            {
                ReportName = "PROF-LETTER73: Employees with Profits Over Age 73",
                ReportDate = DateTimeOffset.UtcNow,
                StartDate = fiscalStartDate,
                EndDate = fiscalEndDate,
                Response = paginatedResults
            };
        }, cancellationToken);
    }

    public async Task<string> GetEmployeesWithProfitsOver73FormLetterAsync(
        EmployeesWithProfitsOver73Request request,
        CancellationToken cancellationToken = default)
    {
        var result = await GetEmployeesWithProfitsOver73Async(request, cancellationToken);
        string letterContent = GenerateFormLettersForOver73Employees(result, request.IsXerox);
        int recordCount = result.Response.Results.Count();

        await _profitSharingAuditService.LogSensitiveDataAccessAsync(
            operationName: "Prof Letter 73 Print",
            tableName: "ProfLetter73",
            primaryKey: $"ProfitYear:{request.ProfitYear}",
            details: $"Records:{recordCount}, BadgeCount:{request.BadgeNumbers?.Count ?? 0}, IsXerox:{request.IsXerox}",
            cancellationToken: cancellationToken);

        return letterContent;
    }


    private string GenerateFormLettersForOver73Employees(ReportResponseBase<EmployeesWithProfitsOver73DetailDto> report, bool isXerox)
    {

        if (!report.Response.Results.Any())
        {
            return "No employees over 73 with positive balances found.";
        }

        StringBuilder letter = new StringBuilder();
        string space7 = new string(' ', 7);

        foreach (EmployeesWithProfitsOver73DetailDto emp in report.Response.Results)
        {
            #region Beginning of letter
            letter.AppendLine();
            PrintFormatHelper.AppendXeroxLine(letter, _djdeDirectiveOptions.ProfitsOver73Header, isXerox);
            var now = _timeProvider.GetLocalNow().DateTime;
            letter.AppendLine($"{now.Month.ToString("MMMM")} {now.Year}");
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
            letter.AppendLine($"{space7}{emp.FullName}");
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
            letter.Append("\f"); // Form feed to end the letter
        }

        return letter.ToString();
    }
}
