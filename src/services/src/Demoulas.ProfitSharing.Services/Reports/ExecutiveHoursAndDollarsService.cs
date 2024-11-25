using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports;

public sealed class ExecutiveHoursAndDollarsService : IExecutiveHoursAndDollarsService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public ExecutiveHoursAndDollarsService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }

    /// <summary>
    /// Generates a list of executives who have hours and dollars for the provided year.
    /// </summary>
    /// <param name="request">The year and pagination details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the report response with details of executive hours/dollars</returns>
    public async Task<ReportResponseBase<ExecutiveHoursAndDollarsResponse>> GetExecutiveHoursAndDollarsReport(ExecutiveHoursAndDollarsRequest request, CancellationToken cancellationToken)
    {
        var result = _dataContextFactory.UseReadOnlyContext(c =>
        {
            var query = c.PayProfits
                .Where(p=> p.ProfitYear == request.ProfitYear)
                .Include(p => p.Demographic)
                .AsQueryable();
            if (request.HasExecutiveHoursAndDollars.HasValue && request.HasExecutiveHoursAndDollars.Value)
            {
                query = query.Where(p => p.HoursExecutive > 0 || p.IncomeExecutive > 0);
            }
            if (request.BadgeNumber.HasValue)
            {
                query = query.Where(pp => pp.Demographic!.EmployeeId == request.BadgeNumber);
            }
            if (request.FullNameContains != null)
            {
                // LINQ needs this simpler query so it can convert it to SQL.
#pragma warning disable RCS1155
                query = query.Where(pp =>
                    pp.Demographic!.ContactInfo!.FullName!.ToLower().Contains(request.FullNameContains.ToLower()));
#pragma warning restore RCS1155
            }
            return query
                .Select(p => new ExecutiveHoursAndDollarsResponse
                {
                    BadgeNumber = p.Demographic!.EmployeeId,
                    FullName = p.Demographic.ContactInfo.FullName,
                    StoreNumber = p.Demographic.StoreNumber,
                    HoursExecutive = p.HoursExecutive,
                    IncomeExecutive = p.IncomeExecutive,
                    CurrentHoursYear = p.CurrentHoursYear,
                    CurrentIncomeYear = p.CurrentIncomeYear,
                    PayFrequencyId = p.Demographic.PayFrequencyId,
                    EmploymentStatusId = p.Demographic.EmploymentStatusId
                })
                .OrderBy(p => p.StoreNumber)
                .ThenBy(p => p.BadgeNumber)
                .ToPaginationResultsAsync(request, cancellationToken);
        });

        return new ReportResponseBase<ExecutiveHoursAndDollarsResponse>
        {
            ReportName = $"Executive Hours and Dollars for Year {request.ProfitYear}",
            ReportDate = DateTimeOffset.Now,
            Response = await result
        };

    }


    public Task SetExecutiveHoursAndDollars(short profitYear, List<SetExecutiveHoursAndDollarsDto> executiveHoursAndDollarsDtos, CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseWritableContext(async ctx =>
        {
            var hasDataForYear = await ctx.PayProfits.AnyAsync(pp => pp.ProfitYear == profitYear, cancellationToken);
            if (!hasDataForYear)
            {
                throw new BadHttpRequestException($"Year {profitYear} is not valid.");
            }
            var badges = executiveHoursAndDollarsDtos.Select(dto => dto.BadgeNumber).ToList();

            var ppQuery = await ctx.PayProfits
                .Include(p => p.Demographic)
                .Where(p => p.ProfitYear == profitYear)
                .Where(p => badges.Contains(p.Demographic!.EmployeeId))
                .ToListAsync(cancellationToken);

            if (executiveHoursAndDollarsDtos.Count != ppQuery.Count)
            {
                throw new BadHttpRequestException("One or more badge numbers were not found.");
            }

            foreach (var pp in ppQuery)
            {
                var dto = executiveHoursAndDollarsDtos.First(x => x.BadgeNumber == pp.Demographic!.EmployeeId);
                pp.HoursExecutive = dto.ExecutiveHours;
                pp.IncomeExecutive = dto.ExecutiveDollars;
            }

            return await ctx.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }


}
