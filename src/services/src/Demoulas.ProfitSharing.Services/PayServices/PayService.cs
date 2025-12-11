using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.PayServices;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.PayServices;

/// <summary>
/// Service implementation for PayServices operations.
/// Handles demographic data retrieval and processing using consolidated Entity Framework queries.
/// Aggregates data by years since hire date for reporting purposes.
/// </summary>
public sealed class PayService : IPayService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ILogger<PayService> _logger;
    private readonly ICalendarService _calendarService;
    private readonly IDemographicReaderService _demographicReaderService;

    public PayService(
        IProfitSharingDataContextFactory dataContextFactory,
        ILogger<PayService> logger,
        ICalendarService calendarService,
        IDemographicReaderService demographicReaderService)
    {
        _dataContextFactory = dataContextFactory;
        _logger = logger;
        _calendarService = calendarService;
        _demographicReaderService = demographicReaderService;
    }

    /// <summary>
    /// Retrieves aggregated pay services data for a specific profit year and employment type.
    /// Performs filtering and joining in the database, then calculates years since hire
    /// and performs grouping/aggregation in memory for Oracle compatibility.
    /// </summary>
    /// <param name="request">Pay services request containing profit year and pagination parameters</param>
    /// <param name="employmentType">Employment type (P=Part-time, H/G/F=Full-time variants)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Result containing paginated pay services data aggregated by years since hire</returns>
    public Task<Result<PayServicesResponse>> GetPayServices(
        PayServicesRequest request,
        char employmentType,
        CancellationToken cancellationToken = default)
    {
        return _dataContextFactory.UseReadOnlyContext<Result<PayServicesResponse>>(async ctx =>
        {
            try
            {
                // Get fiscal year end date for years calculation
                var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(
                    request.ProfitYear,
                    cancellationToken);

                // Build demographics query with frozen/live data selection
                var demographics = await _demographicReaderService.BuildDemographicQuery(ctx, false);

                // Extract fiscal end year for calculation
                int fiscalEndYear = calInfo.FiscalEndDate.Year;

                // STEP 1: Perform filtering and joining in database
                // This retrieves only the necessary data for active employees with matching employment type
                var rawData = await (
                    from d in demographics
                    where d.EmploymentStatusId == EmploymentStatus.Constants.Active
                        && d.EmploymentTypeId == employmentType
                    join pp in ctx.PayProfits.Where(p => p.ProfitYear == request.ProfitYear)
                        on d.Id equals pp.DemographicId into payProfitGroup
                    from payProfit in payProfitGroup.DefaultIfEmpty()
                    select new
                    {
                        HireMonth = d.HireDate.Month,
                        DemographicId = d.Id,
                        HireYear = d.HireDate.Year,
                        Wages = payProfit != null ? payProfit.CurrentIncomeYear : 0m
                    }
                )
                .TagWith($"PayServices-RawData-{request.ProfitYear}-EmpType-{employmentType}")
                .ToListAsync(cancellationToken);

                // STEP 2: Calculate years since hire and perform grouping/aggregation in memory
                // Oracle EF provider cannot translate the GROUP BY calculation, so we do this client-side
                var aggregatedData = rawData
                    .GroupBy(x => fiscalEndYear - x.HireYear != 0 ? fiscalEndYear - x.HireYear : x.HireMonth < 7 ? -2 : -1)
                        .Select(g => new
                        {
                            YearsSinceHire = g.Key,
                            Employees = g.Select(x => x.DemographicId).Distinct().Count(),
                            TotalWages = g.Sum(x => x.Wages)
                        })
                        .OrderBy(x => x.YearsSinceHire)
                        .ToList();

                // Map to DTOs with weekly pay calculation
                var payServicesDtos = aggregatedData.Select(s => new PayServicesDto
                {
                    Employees = s.Employees,
                    YearsOfServiceLabel = s.YearsSinceHire != -1 && s.YearsSinceHire != -2 ? $"{s.YearsSinceHire}" :
                        (s.YearsSinceHire == -1 ? "> 6 mos" : "< 6 mos"),
                    YearsOfService = s.YearsSinceHire,
                    YearsWages = s.TotalWages
                })
                .ToList();

                // Apply pagination to the aggregated results
                var paginatedResults = await payServicesDtos
                    .AsQueryable()
                    .ToPaginationResultsAsync(request, cancellationToken);

                var response = new PayServicesResponse
                {
                    ProfitYear = request.ProfitYear,
                    PayServicesForYear = paginatedResults,
                    Description = $"Successfully retrieved {payServicesDtos.Count} pay service records for employment type '{employmentType}' (aggregated by years since hire).",
                    TotalEmployeeNumber = aggregatedData.Sum(s => s.Employees),
                    TotalEmployeesWages = aggregatedData.Sum(s => s.TotalWages)
                };

                _logger.LogInformation(
                    "Retrieved pay services for ProfitYear: {ProfitYear}, EmploymentType: {EmploymentType}, TotalRecords: {RecordCount}, TotalEmployees: {EmployeeCount} (by years since hire)",
                    request.ProfitYear, employmentType, payServicesDtos.Count, response.TotalEmployeeNumber);

                return Result<PayServicesResponse>.Success(response);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(ex,
                    "GetPayServices operation cancelled for EmploymentType: {EmploymentType}, ProfitYear: {ProfitYear}",
                    employmentType, request.ProfitYear);
                return Result<PayServicesResponse>.Failure(Error.Unexpected($"Operation cancelled."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error retrieving pay services for EmploymentType: {EmploymentType}, ProfitYear: {ProfitYear}",
                    employmentType, request.ProfitYear);
                return Result<PayServicesResponse>.Failure(
                    Error.Unexpected($"Failed to retrieve pay services: {ex.Message}"));
            }
        }, cancellationToken);
    }
}
