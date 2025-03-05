using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports
{
    public class PostFrozenService : IPostFrozenService
    {
        private readonly IProfitSharingDataContextFactory _profitSharingDataContextFactory;
        private readonly TotalService _totalService;
        private readonly ICalendarService _calendarService;

        public PostFrozenService(IProfitSharingDataContextFactory profitSharingDataContextFactory, TotalService totalService, ICalendarService calendarService)
        {
            _profitSharingDataContextFactory = profitSharingDataContextFactory;
            _totalService = totalService;
            _calendarService = calendarService;
        }

        public async Task<ProfitSharingUnder21ReportResponse> ProfitSharingUnder21Report(ProfitYearRequest request, CancellationToken cancellationToken)
        {
            var lastProfitYear = (short)(request.ProfitYear - 1);
            var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);
            var lyCalInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(lastProfitYear, cancellationToken);
            

            var rslt = await _profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
            {
                //Report uses the current date as the offset to calculate the age.
                var birthDate21 = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-21));

                var totalBaseQuery = await (from d in ctx.Demographics.Where(x => x.DateOfBirth >= birthDate21)
                                      join balTbl in _totalService.TotalVestingBalance(ctx, request.ProfitYear, request.ProfitYear, calInfo.FiscalEndDate) on d.Ssn equals balTbl.Ssn into balTmp
                                      from bal in balTmp.DefaultIfEmpty()
                                      join lyBalTbl in _totalService.TotalVestingBalance(ctx, lastProfitYear, lastProfitYear, calInfo.FiscalEndDate) on d.Ssn equals lyBalTbl.Ssn into lyBalTmp
                                      from lyBal in lyBalTmp.DefaultIfEmpty()
                                      select new Under21IntermediaryResult() { d=d, bal = bal ?? new ParticipantTotalVestingBalanceDto(), lyBal = lyBal ?? new Internal.ServiceDto.ParticipantTotalVestingBalanceDto() }
                                 ).ToListAsync(cancellationToken);

                //Get Active under 21 counts
                var activeTotalVested = totalBaseQuery
                                                .Where(x => x.d.EmploymentStatusId == EmploymentStatus.Constants.Active || x.d.TerminationDate > calInfo.FiscalEndDate)
                                                .Count(x => (x.bal.YearsInPlan > 6 || x.lyBal.VestedBalance > 0));

                var activePartiallyVested = totalBaseQuery
                                                .Where(x => x.d.EmploymentStatusId == EmploymentStatus.Constants.Active || x.d.TerminationDate > calInfo.FiscalEndDate)
                                                .Count(x => (x.lyBal == null || x.lyBal.VestedBalance <= 0) && (x.bal != null && x.bal.YearsInPlan > 2 && x.bal.YearsInPlan < 6));

                var activePartiallyVestedButLessThanThreeYears = totalBaseQuery
                                                .Where(x => x.d.EmploymentStatusId == EmploymentStatus.Constants.Active || x.d.TerminationDate > calInfo.FiscalEndDate)
                                                .Count(x => (x.lyBal == null || x.lyBal.VestedBalance <= 0) && (x.bal != null && x.bal.YearsInPlan > 0 && x.bal.YearsInPlan < 3));

                //Get Not active, nor terminated (Inactive) under 21 counts
                var inactiveTotalVested = totalBaseQuery
                                                .Where(x => !(x.d.EmploymentStatusId == EmploymentStatus.Constants.Active || x.d.TerminationDate > calInfo.FiscalEndDate) && x.d.EmploymentStatusId != EmploymentStatus.Constants.Terminated)
                                                .Count(x => ((x.bal != null && x.bal.YearsInPlan > 6) || (x.lyBal != null && x.lyBal.VestedBalance > 0)));

                var inactivePartiallyVested = totalBaseQuery
                                                .Where(x => !(x.d.EmploymentStatusId == EmploymentStatus.Constants.Active || x.d.TerminationDate > calInfo.FiscalEndDate) && x.d.EmploymentStatusId != EmploymentStatus.Constants.Terminated)
                                                .Count(x => (x.lyBal == null || x.lyBal.VestedBalance <= 0) && (x.bal != null && x.bal.YearsInPlan > 2 && x.bal.YearsInPlan < 6));

                var inactivePartiallyVestedButLessThanThreeYears = totalBaseQuery
                                                .Where(x => !(x.d.EmploymentStatusId == EmploymentStatus.Constants.Active || x.d.TerminationDate > calInfo.FiscalEndDate) && x.d.EmploymentStatusId != EmploymentStatus.Constants.Terminated)
                                                .Count(x => (x.lyBal == null || x.lyBal.VestedBalance <= 0) && (x.bal != null && x.bal.YearsInPlan > 0 && x.bal.YearsInPlan < 3));

                //Get terminated under 21 counts.
                var terminatedTotalVested = totalBaseQuery
                                                .Where(x => !(x.d.EmploymentStatusId == EmploymentStatus.Constants.Active || x.d.TerminationDate > calInfo.FiscalEndDate) && x.d.EmploymentStatusId == EmploymentStatus.Constants.Terminated)
                                                .Count(x => ((x.bal != null && x.bal.YearsInPlan > 6) || (x.lyBal != null && x.lyBal.VestedBalance > 0)));

                var terminatedPartiallyVested = totalBaseQuery
                                                .Where(x => !(x.d.EmploymentStatusId == EmploymentStatus.Constants.Active || x.d.TerminationDate > calInfo.FiscalEndDate) && x.d.EmploymentStatusId == EmploymentStatus.Constants.Terminated)
                                                .Count(x => (x.lyBal == null || x.lyBal.VestedBalance <= 0) && (x.bal != null && x.bal.YearsInPlan > 2 && x.bal.YearsInPlan < 6));

                var terminatedPartiallyVestedButLessThanThreeYears = totalBaseQuery
                                                .Where(x => !(x.d.EmploymentStatusId == EmploymentStatus.Constants.Active || x.d.TerminationDate > calInfo.FiscalEndDate) && x.d.EmploymentStatusId == EmploymentStatus.Constants.Terminated)
                                                .Count(x => (x.lyBal == null || x.lyBal.VestedBalance <= 0) && (x.bal != null && x.bal.YearsInPlan > 0 && x.bal.YearsInPlan < 3));

                var totalUnder21 = totalBaseQuery.Count;

                var pagedData = await (
                    from d in ctx.Demographics.Where(x => x.DateOfBirth >= birthDate21)
                    join bal in _totalService.TotalVestingBalance(ctx, request.ProfitYear, request.ProfitYear, calInfo.FiscalEndDate) on d.Ssn equals bal.Ssn 
                    join lyPpTbl in ctx.PayProfits.Where(x => x.ProfitYear == request.ProfitYear - 1) on d.Id equals lyPpTbl.DemographicId into lyPpTmp
                    from lyPp in lyPpTmp.DefaultIfEmpty()
                    join tyPpTbl in ctx.PayProfits.Where(x => x.ProfitYear == request.ProfitYear) on d.Id equals tyPpTbl.DemographicId into tyPpTmp
                    from tyPp in tyPpTmp.DefaultIfEmpty()
                    where bal.YearsInPlan > 0 || bal.VestedBalance > 0
                    orderby d.StoreNumber, d.ContactInfo.FullName
                    select new ProfitSharingUnder21ReportDetail(
                        d.StoreNumber,
                        d.BadgeNumber,
                        d.ContactInfo.FirstName,
                        d.ContactInfo.LastName,
                        d.Ssn.MaskSsn(),
                        bal.YearsInPlan,
                        d.EmploymentTypeId.ToString() == EmployeeType.Constants.NewLastYear.ToString(),
                        tyPp != null ? tyPp.CurrentHoursYear : 0,
                        lyPp != null ? lyPp.CurrentHoursYear : 0,
                        d.HireDate,
                        d.FullTimeDate,
                        d.TerminationDate,
                        d.DateOfBirth,
                        d.DateOfBirth.Age(), //Current report uses today's date for calculating age
                        d.EmploymentStatusId,
                        bal.CurrentBalance,
                        tyPp.EnrollmentId
                    )).ToPaginationResultsAsync(request, cancellationToken: cancellationToken);

                var response = new ProfitSharingUnder21ReportResponse()
                {
                    ActiveTotals = new ProfitSharingUnder21TotalForStatus(activeTotalVested, activePartiallyVested, activePartiallyVestedButLessThanThreeYears),
                    InactiveTotals = new ProfitSharingUnder21TotalForStatus(inactiveTotalVested, inactivePartiallyVested, inactivePartiallyVestedButLessThanThreeYears),
                    TerminatedTotals = new ProfitSharingUnder21TotalForStatus(terminatedTotalVested, terminatedPartiallyVested, terminatedPartiallyVestedButLessThanThreeYears),
                    TotalUnder21 = totalUnder21,
                    ReportDate = DateTime.UtcNow,
                    ReportName = ProfitSharingUnder21ReportResponse.REPORT_NAME,
                    Response = pagedData
                };

                return response;
            });
            return rslt;
        }

        internal class Under21IntermediaryResult
        {
            internal required Demographic d { get; set; }
            internal required ParticipantTotalVestingBalanceDto bal { get; set; }
            internal required ParticipantTotalVestingBalanceDto lyBal { get; set; }
        }
    }
}
