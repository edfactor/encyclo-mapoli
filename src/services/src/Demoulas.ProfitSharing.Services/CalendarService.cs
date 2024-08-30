using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;
public sealed class CalendarService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public CalendarService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }

    public Task<DateOnly> FindWeekendingDateFromDate(DateOnly dateTime, CancellationToken cancellationToken = default)
    {
        return _dataContextFactory.UseReadOnlyContext(context =>
        {
            return context.CaldarRecords.Where(record => record.WeekDate > dateTime)
                .Select(r => r.AccApWkend).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        });
    }
}
