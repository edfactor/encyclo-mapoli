using System.Data;
using System.Text.Json;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Validators;
using Demoulas.ProfitSharing.Data.Contexts;
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

    public UnForfeitService(
        IProfitSharingDataContextFactory dataContextFactory,
        IDemographicReaderService demographicReaderService)
    {
        _dataContextFactory = dataContextFactory;
        _demographicReaderService = demographicReaderService;
    }

    public async Task<ReportResponseBase<RehireForfeituresResponse>> FindRehiresWhoMayBeEntitledToForfeituresTakenOutInPriorYearsAsync(StartAndEndDateRequest req, CancellationToken cancellationToken)
    {
        var validator = new StartAndEndDateRequestValidator();
        await validator.ValidateAndThrowAsync(req, cancellationToken);

        var militaryMembers = await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            var demo = await _demographicReaderService.BuildDemographicQuery(context);
            var query =
                from d in demo.Include(d=> d.EmploymentStatus)
                join pp in context.PayProfits.Include(p=> p.Enrollment) on d.Id equals pp.DemographicId
                join pd in context.ProfitDetails on new { d.Ssn, pp.ProfitYear } equals new { Ssn = pd.Ssn, pd.ProfitYear }
                where pd.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures.Id && d.EmploymentStatusId == EmploymentStatus.Constants.Active
                group new { d, pp, pd } by new
                {
                    d.BadgeNumber,
                    d.ContactInfo.FullName,
                    d.Ssn,
                    d.HireDate,
                    d.TerminationDate,
                    d.ReHireDate,
                    d.StoreNumber,
                    EmploymentStatus = d.EmploymentStatus!.Name
                }
                into g
                orderby g.Key.BadgeNumber
                select new RehireForfeituresResponse
                {
                    BadgeNumber = g.Key.BadgeNumber,
                    FullName = g.Key.FullName,
                    Ssn = g.Key.Ssn.MaskSsn(),
                    HireDate = g.Key.HireDate,
                    TerminationDate = g.Key.TerminationDate,
                    ReHiredDate = g.Key.ReHireDate ?? default,
                    StoreNumber = g.Key.StoreNumber,
                    CompanyContributionYears = (byte)g.Sum(x => x.pd.YearsOfServiceCredit),
                    EmploymentStatus = g.Key.EmploymentStatus,
                    Details = g.Select(x => new MilitaryRehireProfitSharingDetailResponse
                    {
                        ProfitYear = x.pp.ProfitYear,
                        HoursCurrentYear = x.pp.CurrentHoursYear,
                        EnrollmentName = x.pp.Enrollment!.Name,
                        EnrollmentId = x.pp.EnrollmentId,
                        Forfeiture = x.pd.Forfeiture,
                        Remark = x.pd.Remark,
                        ProfitCodeId = x.pd.ProfitCodeId
                    }).ToList()
                };

            return await query.ToPaginationResultsAsync(req, cancellationToken);
        });

        return new ReportResponseBase<RehireForfeituresResponse>
        {
            ReportName = "REHIRE'S PROFIT SHARING DATA",
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = req.BeginningDate,
            EndDate = req.EndingDate,
            Response = militaryMembers
        };
    }
}
