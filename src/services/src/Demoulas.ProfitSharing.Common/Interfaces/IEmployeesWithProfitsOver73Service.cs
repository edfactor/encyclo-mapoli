using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// Service interface for generating the PROF-LETTER73 adhoc report.
/// </summary>
public interface IEmployeesWithProfitsOver73Service
{
    /// <summary>
    /// Generates a report of employees with profits over age 73.
    /// </summary>
    /// <param name="request">The request containing profit year and pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A response containing paginated employee detail records.</returns>
    Task<ReportResponseBase<EmployeesWithProfitsOver73DetailDto>> GetEmployeesWithProfitsOver73Async(
        EmployeesWithProfitsOver73Request request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates form letters for employees with profits over age 73 who must take required minimum distributions.
    /// </summary>
    /// <param name="request">The request containing profit year parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the form letter content.</returns>
    Task<string> GetEmployeesWithProfitsOver73FormLetterAsync(
        EmployeesWithProfitsOver73Request request,
        CancellationToken cancellationToken = default);
}
