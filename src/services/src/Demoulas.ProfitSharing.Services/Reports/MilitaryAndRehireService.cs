using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports;

public sealed class MilitaryAndRehireService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly CalendarService _calendarService;

    public MilitaryAndRehireService(IProfitSharingDataContextFactory dataContextFactory, CalendarService calendarService)
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
    }

    /// <summary>
    /// Retrieves all inactive military members for a specified calendar year. (QPAY 511 "Military and Rehire")
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of inactive military members.</returns>
    /// <remarks>
    /// This method queries the database for members who have a termination code indicating military service and an employment status of inactive,
    /// within the specified calendar year.
    /// </remarks>
    public async Task GetAllInactiveMilitaryMembers(CancellationToken cancellationToken)
    {
        await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            var inactiveMilitaryMembers = await context.Demographics.Where(d => d.TerminationCodeId == TerminationCode.Constants.Military
                                                                                && d.EmploymentStatusId == EmploymentStatus.Constants.Inactive)
                .OrderBy(d=> d.FullName)
                .ToListAsync(cancellationToken: cancellationToken);

            return inactiveMilitaryMembers;
        });
    }
}
