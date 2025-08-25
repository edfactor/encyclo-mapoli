using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Validators;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports;

public sealed class UnforfeitService : IUnforfeitService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly TotalService _totalService;

    public UnforfeitService(
        IProfitSharingDataContextFactory dataContextFactory,
        IDemographicReaderService demographicReaderService,
        TotalService totalService)
    {
        _dataContextFactory = dataContextFactory;
        _demographicReaderService = demographicReaderService;
        _totalService = totalService;
    }

    public async Task<ReportResponseBase<UnforfeituresResponse>> FindRehiresWhoMayBeEntitledToForfeituresTakenOutInPriorYearsAsync(StartAndEndDateRequest req,
        CancellationToken cancellationToken)
    {
        var validator = new StartAndEndDateRequestValidator();
        await validator.ValidateAndThrowAsync(req, cancellationToken);

        var militaryMembers = await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            var beginning = req.BeginningDate;
            var ending = req.EndingDate;
            var yearsOfServiceQuery = _totalService.GetYearsOfService(context, req.ProfitYear);
            var vestingServiceQuery = _totalService.TotalVestingBalance(context, req.ProfitYear, ending);
            var demo = await _demographicReaderService.BuildDemographicQuery(context);

            var query =
                from pd in context.ProfitDetails
                join d in demo on pd.Ssn equals d.Ssn
                join ppYE in context.PayProfits.Include(p => p.Enrollment)
                    on new { d.Id, ProfitYear = req.ProfitYear } equals new { Id = ppYE.DemographicId, ppYE.ProfitYear }
                join pp in context.PayProfits.Include(p => p.Enrollment)
                    on new { d.Id, ProfitYear = pd.ProfitYear } equals new { Id = pp.DemographicId, pp.ProfitYear } into ppTmp
                from pp in ppTmp.DefaultIfEmpty()
                join yos in yearsOfServiceQuery on d.Ssn equals yos.Ssn into yosTmp
                from yos in yosTmp.DefaultIfEmpty()
                join vest in vestingServiceQuery on d.Ssn equals vest.Ssn into vestTmp
                from vest in vestTmp.DefaultIfEmpty()
                where pd.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures.Id
                      && d.EmploymentStatusId == EmploymentStatus.Constants.Active
                      && pd.ProfitYear >= beginning.Year && pd.ProfitYear <= ending.Year
                      && (!req.ExcludeZeroBalance || (vest != null && (vest.CurrentBalance != 0 || vest.VestedBalance != 0)))
                group new
                {
                    d,
                    pp,
                    ppYE,
                    pd,
                    yos,
                    vest
                } by new
                {
                    d.BadgeNumber,
                    d.ContactInfo.FullName,
                    d.Ssn,
                    d.HireDate,
                    d.ReHireDate,
                    d.StoreNumber,
                    YearsOfService = yos != null ? yos.Years : (byte)0,
                    NetBalanceLastYear = vest != null ? vest.CurrentBalance ?? 0 : 0,
                    VestedBalanceLastYear = vest != null ? vest.VestedBalance ?? 0 : 0,
                    d.EmploymentStatusId,
                    d.PayFrequencyId,
                    ppYE.EnrollmentId,
                    EnrollmentName = ppYE.Enrollment!.Name,
                    HoursProfitYear = ppYE.HoursExecutive + ppYE.CurrentHoursYear,
                    WagesProfitYear = ppYE.IncomeExecutive + ppYE.CurrentIncomeYear,
                }
                into g
                select new UnforfeituresResponse
                {
                    BadgeNumber = g.Key.BadgeNumber,
                    FullName = g.Key.FullName,
                    Ssn = g.Key.Ssn.MaskSsn(),
                    HireDate = g.Key.HireDate,
                    ReHiredDate = g.Key.ReHireDate ?? default,
                    StoreNumber = g.Key.StoreNumber,
                    CompanyContributionYears = g.Key.YearsOfService,
                    NetBalanceLastYear = g.Key.NetBalanceLastYear,
                    VestedBalanceLastYear = g.Key.VestedBalanceLastYear,
                    EnrollmentName = g.Key.EnrollmentName,
                    EnrollmentId = g.Key.EnrollmentId,
                    HoursProfitYear = g.Key.HoursProfitYear,
                    WagesProfitYear = g.Key.WagesProfitYear,
                    IsExecutive = g.Key.PayFrequencyId == PayFrequency.Constants.Monthly,
                    Details = g.Select(x => new RehireTransactionDetailResponse
                        {
                            ProfitYear = x.pd.ProfitYear,
                            HoursTransactionYear = x.pp != null ? x.pp.CurrentHoursYear : null,
                            Forfeiture = x.pd.Forfeiture,
                            Remark = x.pd.Remark,
                            ProfitCodeId = x.pd.ProfitCodeId,
                            WagesTransactionYear = x.pp != null ? (x.pp.CurrentIncomeYear + x.pp.IncomeExecutive) : null,
                            SuggestedUnforfeiture = x.pp != null &&
                                                    // only consider the latest Forfeit transaction
                                                    x.pd.ProfitYear == g.Max(item => item.pd.ProfitYear) &&
                                                    (
                                                        // Employees with 1 year and enrollment of 2 are a special case. 
                                                        (g.Key.YearsOfService == 1 && g.Key.EnrollmentId == /*2*/ Enrollment.Constants.NewVestingPlanHasContributions)
                                                        || g.Key.EnrollmentId == /*3*/ Enrollment.Constants.OldVestingPlanHasForfeitureRecords
                                                        || g.Key.EnrollmentId == /*4*/ Enrollment.Constants.NewVestingPlanHasForfeitureRecords
                                                    )
                                                    && x.pd.Forfeiture > 0 &&
                                                    x.pd.CommentType == CommentType.Constants.Forfeit
                                ? x.pd.Forfeiture
                                : (decimal?)null,
                            ProfitDetailId = x.pd.Id
                        })
                        .OrderByDescending(x => x.ProfitYear)
                        .ToList()
                };
            return await query.ToPaginationResultsAsync(req, cancellationToken);
        });

        return new ReportResponseBase<UnforfeituresResponse>
        {
            ReportName = "REHIRE'S PROFIT SHARING DATA",
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = req.BeginningDate,
            EndDate = req.EndingDate,
            Response = militaryMembers
        };
    }
}
