using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports;

public sealed class ExecutiveHoursAndDollarsService : IExecutiveHoursAndDollarsService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ICalendarService _calendarService;

    public ExecutiveHoursAndDollarsService(IProfitSharingDataContextFactory dataContextFactory,
        ICalendarService calendarService)
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
    }

    /// <summary>
    /// Generates a list of executives who have hours and dollars for the provided year.
    /// </summary>
    /// <param name="request">The year and pagination details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the report response with details of executive hours/dollars</returns>
    public async Task<ReportResponseBase<ExecutiveHoursAndDollarsResponse>> GetExecutiveHoursAndDollarsReportAsync(ExecutiveHoursAndDollarsRequest request, CancellationToken cancellationToken)
    {
        var result = await _dataContextFactory.UseReadOnlyContext(async c =>
        {
            var query = c.PayProfits
                .Where(p => p.ProfitYear == request.ProfitYear)
                .Include(p => p.Demographic)
                .ThenInclude(d => d!.PayFrequency)
                .Include(p => p.Demographic)
                .ThenInclude(d => d!.EmploymentStatus)
                .AsQueryable();

            if (request.HasExecutiveHoursAndDollars.HasValue && request.HasExecutiveHoursAndDollars.Value && request.IsMonthlyPayroll.HasValue && request.IsMonthlyPayroll.Value)
            {
                query = query.Where(p => (p.HoursExecutive > 0 || p.IncomeExecutive > 0) || (p.Demographic!.PayFrequencyId == PayFrequency.Constants.Monthly));
            }
            else
            {
                if (request.HasExecutiveHoursAndDollars.HasValue && request.HasExecutiveHoursAndDollars.Value)
                {
                    query = query.Where(p => p.HoursExecutive > 0 || p.IncomeExecutive > 0);
                }
                // Executives often have a pay frequency value of 2
                if (request.IsMonthlyPayroll.HasValue && request.IsMonthlyPayroll.Value)
                {
                    query = query.Where(pp => pp.Demographic!.PayFrequencyId == PayFrequency.Constants.Monthly);
                }
            }


            if (request.BadgeNumber.HasValue)
            {
                query = query.Where(pp => pp.Demographic!.BadgeNumber == request.BadgeNumber);
            }

            if (request.Ssn != null)
            {
                query = query.Where(pp => pp.Demographic!.Ssn == request.Ssn);
            }
            if (request.FullNameContains != null)
            {
                var pattern = $"%{request.FullNameContains.ToUpperInvariant()}%";
                query = query.Where(pp => EF.Functions.Like(pp.Demographic!.ContactInfo!.FullName!.ToUpper(), pattern));
            }

            // Select unmasked SSN for sorting, then mask after pagination
            var paginatedResult = await query
                .Select(p => new ExecutiveHoursAndDollarsResponse
                {
                    BadgeNumber = p.Demographic!.BadgeNumber,
                    FullName = p.Demographic.ContactInfo.FullName,
                    StoreNumber = p.Demographic.StoreNumber,
                    Ssn = p.Demographic.Ssn.ToString(), // Keep unmasked for sorting
                    HoursExecutive = p.HoursExecutive,
                    IncomeExecutive = p.IncomeExecutive,
                    CurrentHoursYear = p.CurrentHoursYear,
                    CurrentIncomeYear = p.CurrentIncomeYear,
                    PayFrequencyId = p.Demographic.PayFrequencyId,
                    PayFrequencyName = p.Demographic.PayFrequency!.Name,
                    EmploymentStatusId = p.Demographic.EmploymentStatusId,
                    EmploymentStatusName = p.Demographic.EmploymentStatus!.Name,
                    IsExecutive = p.Demographic.PayFrequencyId == 2,
                })
                .ToPaginationResultsAsync(request, cancellationToken);

            // Mask SSN after pagination/sorting
            var results = paginatedResult.Results.Select(r => r with { Ssn = r.Ssn.MaskSsn() }).ToList();

            return new PaginatedResponseDto<ExecutiveHoursAndDollarsResponse>
            {
                Results = results,
                Total = paginatedResult.Total
            };
        }, cancellationToken);

        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);

        return new ReportResponseBase<ExecutiveHoursAndDollarsResponse>
        {
            ReportName = $"Executive Hours and Dollars for Year {request.ProfitYear}",
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = calInfo.FiscalBeginDate,
            EndDate = calInfo.FiscalEndDate,
            Response = result
        };

    }


    public Task SetExecutiveHoursAndDollarsAsync(short profitYear, List<SetExecutiveHoursAndDollarsDto> executiveHoursAndDollarsDtos, CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseWritableContext(async ctx =>
        {
            var hasDataForYear = await ctx.PayProfits.AnyAsync(pp => pp.ProfitYear == profitYear, cancellationToken);
            if (!hasDataForYear)
            {
                throw new BadHttpRequestException($"Year {profitYear} is not valid.");
            }
            var badges = executiveHoursAndDollarsDtos.Select(dto => dto.BadgeNumber).ToHashSet();

            var ppQuery = await ctx.PayProfits
                .Include(p => p.Demographic)
                .Where(p => p.ProfitYear == profitYear)
                .Where(p => badges.Contains(p.Demographic!.BadgeNumber))
                .ToListAsync(cancellationToken);

            if (badges.Count != ppQuery.Count)
            {
                throw new BadHttpRequestException("One or more badge numbers were not found.");
            }

            foreach (var pp in ppQuery)
            {
                var dto = executiveHoursAndDollarsDtos.First(x => x.BadgeNumber == pp.Demographic!.BadgeNumber);
                pp.HoursExecutive = dto.ExecutiveHours;
                pp.IncomeExecutive = dto.ExecutiveDollars;
            }

            return await ctx.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }


}
