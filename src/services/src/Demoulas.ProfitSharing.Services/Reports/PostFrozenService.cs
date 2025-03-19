using System.Threading;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Demoulas.Util.Extensions;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore;
using static FastEndpoints.Ep;

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

        public async Task<ReportResponseBase<ProfitSharingUnder21BreakdownByStoreResponse>> ProfitSharingUnder21BreakdownByStore(ProfitYearRequest request, CancellationToken cancellation)
        {
            var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellation);
            var age21 = calInfo.FiscalEndDate.AddYears(-21);
            short lastYear = (short)(request.ProfitYear - 1);
            var earningsProfitCodes = new List<int> 
            { 
                ProfitCode.Constants.IncomingContributions.Id,
                ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id,
                ProfitCode.Constants.OutgoingForfeitures.Id,
                ProfitCode.Constants.OutgoingDirectPayments.Id,
                ProfitCode.Constants.Incoming100PercentVestedEarnings.Id,
            };
            var contributionProfitCodes = new[]
            {
                ProfitCode.Constants.IncomingContributions.Id,
                ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id,
                ProfitCode.Constants.OutgoingForfeitures.Id,
                ProfitCode.Constants.OutgoingDirectPayments.Id,
            };
            var distributionProfitCodes = new[]
            {
                ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id,
                ProfitCode.Constants.OutgoingForfeitures.Id,
                ProfitCode.Constants.Outgoing100PercentVestedPayment.Id,
            };


            var rslt = await _profitSharingDataContextFactory.UseReadOnlyContext(async ctx => {

                var qry = (
                    from pp in ctx.PayProfits.Where(x => x.ProfitYear == request.ProfitYear)
                    join d in ctx.Demographics.Include(d => d.ContactInfo) on pp.DemographicId equals d.Id
                    join lyTotTbl in _totalService.GetTotalBalanceSet(ctx, lastYear) on d.Ssn equals lyTotTbl.Ssn into lyTotTmp
                    from lyTot in lyTotTmp.DefaultIfEmpty()
                    join tyTotTbl in _totalService.TotalVestingBalance(ctx, request.ProfitYear, calInfo.FiscalEndDate) on d.Ssn equals tyTotTbl.Ssn into tyTotalTmp
                    from tyTot in tyTotalTmp.DefaultIfEmpty()
                    join tyPdGrpTbl in (
                            from pd in ctx.ProfitDetails.Where(x => x.ProfitYear == request.ProfitYear)
                            group pd by pd.Ssn into pdGrp
                            select new
                            {
                                Ssn = pdGrp.Key,
                                Earnings = pdGrp.Where(x=>earningsProfitCodes.Contains(x.ProfitCodeId)).Sum(x=>x.Earnings),
                                Contributions = pdGrp.Where(x=>contributionProfitCodes.Contains(x.ProfitCodeId)).Sum(x=>x.Contribution),
                                Forfeitures = pdGrp.Where(x=>x.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id).Sum(x=>x.Forfeiture) - 
                                                pdGrp.Where(x=>x.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures.Id).Sum(x=>x.Forfeiture),
                                Distributions = pdGrp.Where(x=>distributionProfitCodes.Contains(x.ProfitCodeId)).Sum(x=>x.Forfeiture * -1)
                            }
                          ) on d.Ssn equals tyPdGrpTbl.Ssn into tyPdGrpTmp
                    from tyPdGrp in tyPdGrpTmp.DefaultIfEmpty()
                    where d.DateOfBirth > age21
                    orderby d.StoreNumber, d.ContactInfo.LastName, d.ContactInfo.FirstName
                    select new ProfitSharingUnder21BreakdownByStoreResponse()
                    {
                        StoreNumber = d.StoreNumber,
                        BadgeNumber = d.BadgeNumber,
                        FullName = $"{d.ContactInfo.LastName}, {d.ContactInfo.FirstName}",
                        BeginningBalance = lyTot.Total ?? 0,
                        Earnings = tyPdGrp.Earnings,
                        Contributions = tyPdGrp.Contributions,
                        Forfeitures = tyPdGrp.Forfeitures,
                        Distributions = tyPdGrp.Distributions,
                        VestedAmount = tyTot.VestedBalance,
                        EndingBalance = tyTot.CurrentBalance,
                        VestingPercentage = tyTot.VestingPercent,
                        DateOfBirth = d.DateOfBirth,
                        Age = 0, //To be determined after materializing
                        EnrollmentId = pp.EnrollmentId
                    }
                );
                var pagedResults = await qry.ToPaginationResultsAsync(request, cancellation);
                var fiscalEndDateTime = calInfo.FiscalEndDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
                foreach (var row in pagedResults.Results)
                {
                    row.Age = (byte)row.DateOfBirth.Age(fiscalEndDateTime);
                }
                return pagedResults;
            });

            return new ReportResponseBase<ProfitSharingUnder21BreakdownByStoreResponse>()
            {
                ReportDate = DateTime.UtcNow,
                ReportName = ProfitSharingUnder21BreakdownByStoreResponse.REPORT_NAME,
                Response = rslt
            };
        }

        public async Task<ReportResponseBase<ProfitSharingUnder21InactiveNoBalanceResponse>> ProfitSharingUnder21InactiveNoBalance(ProfitYearRequest request, CancellationToken cancellationToken)
        {
            var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);
            var age21 = calInfo.FiscalEndDate.AddYears(-21);
            short lastYear = (short)(request.ProfitYear - 1);
            var rslt = await _profitSharingDataContextFactory.UseReadOnlyContext(ctx =>
            {
                return (
                    from d in FrozenService.GetDemographicSnapshot(ctx, request.ProfitYear).Where(x => x.DateOfBirth >= age21)
                    join pp in ctx.PayProfits.Where(x=>x.ProfitYear == request.ProfitYear) on d.Id equals pp.DemographicId
                    join balTbl in _totalService.TotalVestingBalance(ctx, request.ProfitYear, calInfo.FiscalEndDate)
                        on d.Ssn equals balTbl.Ssn into balTmp
                    from bal in balTmp.DefaultIfEmpty()
                    where 
                        d.TerminationCodeId != TerminationCode.Constants.Retired &&
                        (
                            d.EmploymentStatusId == EmploymentStatus.Constants.Inactive ||
                            (
                                d.EmploymentStatusId == EmploymentStatus.Constants.Terminated
                            )
                        )
                        && (bal.VestedBalance > 0 || bal.YearsInPlan > 0)
                        && (bal.CurrentBalance <= 0)
                    orderby d.ContactInfo.LastName, d.ContactInfo.FirstName
                    select new ProfitSharingUnder21InactiveNoBalanceResponse()
                    {
                        BadgeNumber = d.BadgeNumber,
                        LastName = d.ContactInfo.LastName,
                        FirstName = d.ContactInfo.FirstName,
                        BirthDate = d.DateOfBirth,
                        HireDate = d.HireDate,
                        TerminationDate = d.TerminationDate,
                        Age = 0,
                        EnrollmentId = pp.EnrollmentId
                    }
                ).ToPaginationResultsAsync(request, cancellationToken);
            });

            var fiscalEndDateTime = calInfo.FiscalEndDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            foreach (var row in rslt.Results) {
                row.Age = (byte)row.BirthDate.Age(fiscalEndDateTime);
            }

            return new ReportResponseBase<ProfitSharingUnder21InactiveNoBalanceResponse>() 
            {
                ReportDate = DateTime.UtcNow,
                ReportName = ProfitSharingUnder21InactiveNoBalanceResponse.REPORT_NAME,
                Response = rslt
            };
        }

        internal class Under21IntermediaryResult
        {
            internal required Demographic d { get; set; }
            internal required ParticipantTotalVestingBalanceDto bal { get; set; }
            internal required ParticipantTotalVestingBalanceDto lyBal { get; set; }
        }
    }
}
