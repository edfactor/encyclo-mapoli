using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Lookup;

public sealed class EmployeeLookupService(IProfitSharingDataContextFactory factory, IDemographicReaderService demographicReaderService) : IEmployeeLookupService
{
    public Task<bool> BadgeExistsAsync(int badgeNumber, CancellationToken cancellationToken = default)
    {
        return factory.UseReadOnlyContext(async ctx =>
        {
#pragma warning disable DSMPS001
            return await ctx.Demographics.AnyAsync(d => d.BadgeNumber == badgeNumber, cancellationToken);
#pragma warning restore DSMPS001
        }, cancellationToken);
    }

    public Task<DateOnly?> GetEarliestHireDateAsync(int badgeNumber, CancellationToken cancellationToken = default)
    {
        return factory.UseReadOnlyContext(async ctx =>
        {
#pragma warning disable DSMPS001
            var demo = await ctx.Demographics
                .Where(d => d.BadgeNumber == badgeNumber)
                .OrderBy(d => d.HireDate)
                .Select(d => new { d.HireDate, d.ReHireDate })
                .FirstOrDefaultAsync(cancellationToken);
#pragma warning restore DSMPS001
            DateOnly? earliest = null;
            if (demo != null)
            {
                if (demo.ReHireDate.HasValue && demo.ReHireDate.Value < demo.HireDate)
                {
                    earliest = demo.ReHireDate.Value;
                }
                else
                {
                    earliest = demo.HireDate;
                }
            }

            return earliest;
        }, cancellationToken);
    }

    public Task<DateOnly?> GetDateOfBirthAsync(int badgeNumber, CancellationToken cancellationToken = default)
    {
        return factory.UseReadOnlyContext(async ctx =>
        {
#pragma warning disable DSMPS001
            var dob = await ctx.Demographics
                .Where(d => d.BadgeNumber == badgeNumber)
                .Select(d => (DateOnly?)d.DateOfBirth)
                .FirstOrDefaultAsync(cancellationToken);
#pragma warning restore DSMPS001
            return dob;
        }, cancellationToken);
    }

    public Task<char?> GetEmploymentStatusIdAsOfAsync(int badgeNumber, DateOnly asOfDate, CancellationToken cancellationToken = default)
    {
        return factory.UseReadOnlyContext(async ctx =>
        {
            var asOfTimestamp = new DateTimeOffset(asOfDate.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
            var demoQuery = demographicReaderService.BuildDemographicQueryAsOf(ctx, asOfTimestamp);
            var row = await demoQuery
                .Where(d => d.BadgeNumber == badgeNumber)
                .Select(d => new { d.EmploymentStatusId, d.TerminationDate })
                .FirstOrDefaultAsync(cancellationToken);

            if (row is null)
            {
                return (char?)null;
            }

            if (row.TerminationDate.HasValue && row.TerminationDate.Value <= asOfDate)
            {
                return Demoulas.ProfitSharing.Data.Entities.EmploymentStatus.Constants.Terminated;
            }

            return row.EmploymentStatusId;
        }, cancellationToken);
    }

    public async Task<bool?> IsActiveAsOfAsync(int badgeNumber, DateOnly asOfDate, CancellationToken cancellationToken = default)
    {
        var status = await GetEmploymentStatusIdAsOfAsync(badgeNumber, asOfDate, cancellationToken);
        if (!status.HasValue)
        {
            return null;
        }
        return status.Value == Demoulas.ProfitSharing.Data.Entities.EmploymentStatus.Constants.Active;
    }
}
