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
public sealed class UnForfeitService : IUnForfeitService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly TotalService _totalService;

    public UnForfeitService(
        IProfitSharingDataContextFactory dataContextFactory,
        IDemographicReaderService demographicReaderService,
        TotalService totalService)
    {
        _dataContextFactory = dataContextFactory;
        _demographicReaderService = demographicReaderService;
        _totalService = totalService;
    }

    public async Task<ReportResponseBase<UnforfeituresResponse>> FindRehiresWhoMayBeEntitledToForfeituresTakenOutInPriorYearsAsync(StartAndEndDateRequest req, CancellationToken cancellationToken)
    {
        var validator = new StartAndEndDateRequestValidator();
        await validator.ValidateAndThrowAsync(req, cancellationToken);

        var militaryMembers = await _dataContextFactory.UseReadOnlyContext(async context =>
        {

            var beginning = req.BeginningDate;
            var ending = req.EndingDate;

            var yearsOfServiceQuery = _totalService.GetYearsOfService(context, (short)req.EndingDate.Year);
            var vestingServiceQuery = _totalService.TotalVestingBalance(context, (short)req.EndingDate.Year, ending);
            var demo = await _demographicReaderService.BuildDemographicQuery(context);
            
            var query =
                from d in demo
                join pp in context.PayProfits.Include(p=> p.Enrollment) on d.Id equals pp.DemographicId
                join pd in context.ProfitDetails on new { d.Ssn, pp.ProfitYear } equals new { Ssn = pd.Ssn, pd.ProfitYear }
                join yos in yearsOfServiceQuery on d.Ssn equals yos.Ssn into yosTmp
                from yos in yosTmp.DefaultIfEmpty()
                join vest in vestingServiceQuery on d.Ssn equals vest.Ssn into vestTmp
                from vest in vestTmp.DefaultIfEmpty()
                where pd.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures.Id 
                      && d.EmploymentStatusId == EmploymentStatus.Constants.Active
                      && pp.ProfitYear >= beginning.Year && pp.ProfitYear <= ending.Year
                      && (!req.ExcludeZeroBalance || (vest != null && (vest.CurrentBalance != 0 || vest.VestedBalance != 0)))
                group new { d, pp, pd, yos, vest } by new
                {
                    d.BadgeNumber,
                    d.ContactInfo.FullName,
                    d.Ssn,
                    d.HireDate,
                    d.TerminationDate,
                    d.ReHireDate,
                    d.StoreNumber,
                    YearsOfService = yos != null ? yos.Years : (byte)0,
                    NetBalanceLastYear = vest != null ? vest.CurrentBalance ?? 0 : 0,
                    VestedBalanceLastYear = vest != null ? vest.VestedBalance ?? 0 : 0,
                    d.EmploymentStatusId,
                }
                into g
                select new UnforfeituresResponse
                {
                    BadgeNumber = g.Key.BadgeNumber,
                    FullName = g.Key.FullName,
                    Ssn = g.Key.Ssn.MaskSsn(),
                    HireDate = g.Key.HireDate,
                    TerminationDate = g.Key.TerminationDate,
                    ReHiredDate = g.Key.ReHireDate ?? default,
                    StoreNumber = g.Key.StoreNumber,
                    CompanyContributionYears = g.Key.YearsOfService,
                    NetBalanceLastYear = g.Key.NetBalanceLastYear,
                    VestedBalanceLastYear = g.Key.VestedBalanceLastYear,
                    Details = g.Select(x => new MilitaryRehireProfitSharingDetailResponse
                    {
                        ProfitYear = x.pp.ProfitYear,
                        ProfitDetailId = x.pd.Id,
                        HoursCurrentYear = x.pp.CurrentHoursYear,
                        EnrollmentName = x.pp.Enrollment!.Name,
                        EnrollmentId = x.pp.EnrollmentId,
                        Forfeiture = x.pd.Forfeiture,
                        Remark = x.pd.Remark,
                        ProfitCodeId = x.pd.ProfitCodeId,
                        Wages = x.pp.CurrentIncomeYear + x.pp.IncomeExecutive,
                        SuggestedForfeiture = (x.pp.EnrollmentId == Enrollment.Constants.OldVestingPlanHasForfeitureRecords || x.pp.EnrollmentId == Enrollment.Constants.NewVestingPlanHasForfeitureRecords) && 
                            x.pp.ProfitYear == req.ProfitYear ?
                            - x.pd.Forfeiture 
                            : null
                    })
                    .OrderByDescending(x=>x.ProfitYear)    
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
