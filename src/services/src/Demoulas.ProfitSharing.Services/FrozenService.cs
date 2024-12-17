using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;

/// <summary>
/// This service contains logic related to getting temporal data for tables with ValidFrom and ValidTo.
/// </summary>
public class FrozenService: IFrozenService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public FrozenService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }

    /// <summary>
    /// Returns a query object representing Demographic data as of a certain date time
    /// </summary>
    /// <param name="ctx">The data context used for querying</param>
    /// <param name="asOfDateTime">The UTC datetime for which data changed after this point in time will be disregarded.</param>
    /// <returns></returns>
    public static IQueryable<Demographic> GetDemographicSnapshot(IProfitSharingDbContext ctx, DateTime asOfDateTime)
    {
        return (
            from dh in ctx.DemographicHistories
            join d in ctx.Demographics on dh.DemographicId equals d.Id
            where asOfDateTime >= dh.ValidFrom && asOfDateTime < dh.ValidTo
            select new Demographic()
            {
                Id = dh.DemographicId,
                OracleHcmId = dh.OracleHcmId,
                Ssn = dh.Ssn,
                EmployeeId = dh.EmployeeId,
                LastModifiedDate = dh.ValidFrom,
                StoreNumber = dh.StoreNumber,
                PayClassificationId = dh.PayClassificationId,
                ContactInfo = d.ContactInfo,
                Address = d.Address,
                DateOfBirth = dh.DateOfBirth,
                FullTimeDate = d.FullTimeDate,
                HireDate = dh.HireDate,
                ReHireDate = dh.ReHireDate,
                TerminationDate = dh.TerminationDate,
                DepartmentId = dh.DepartmentId,
                EmploymentTypeId = dh.EmploymentTypeId,
                GenderId = d.GenderId,
                PayFrequencyId = dh.PayFrequencyId,
                TerminationCodeId = dh.TerminationCodeId,
                EmploymentStatusId = dh.EmploymentStatusId,
            }

        );
    }

    /// <summary>
    /// Returns a query object representing Demographic data as of a certain date time
    /// </summary>
    /// <param name="ctx">The data context used for querying</param>
    /// <param name="profitYear">Show data as of the last frozen date for a profit year.</param>
    /// <returns></returns>
    public static IQueryable<Demographic> GetDemographicSnapshot(IProfitSharingDbContext ctx, short profitYear)
    {

        return (
            from dh in ctx.DemographicHistories
            join d in ctx.Demographics.Include(x => x.ContactInfo).Include(x => x.Address) on dh.DemographicId equals d.Id
            from fs in ctx.FrozenStates.Where(x => x.ProfitYear == profitYear && x.IsActive)
            where fs.AsOfDateTime >= dh.ValidFrom && fs.AsOfDateTime < dh.ValidTo
            select new Demographic()
            {
                Id = dh.DemographicId,
                OracleHcmId = dh.OracleHcmId,
                Ssn = dh.Ssn,
                EmployeeId = dh.EmployeeId,
                LastModifiedDate = dh.ValidFrom,
                StoreNumber = dh.StoreNumber,
                PayClassificationId = dh.PayClassificationId,
                ContactInfo = d.ContactInfo,
                Address = d.Address,
                DateOfBirth = dh.DateOfBirth,
                FullTimeDate = d.FullTimeDate,
                HireDate = dh.HireDate,
                ReHireDate = dh.ReHireDate,
                TerminationDate = dh.TerminationDate,
                DepartmentId = dh.DepartmentId,
                EmploymentTypeId = dh.EmploymentTypeId,
                GenderId = d.GenderId,
                PayFrequencyId = dh.PayFrequencyId,
                TerminationCodeId = dh.TerminationCodeId,
                EmploymentStatusId = dh.EmploymentStatusId,
            }

        );
    }

    /// <summary>
    /// Sets the cutoff data for a particular profit year.  Deactivates any prior "Freezes" for the year.
    /// </summary>
    /// <param name="profitYear">Profit year for which to set the freeze date/time</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<SetFrozenStateResponse> FreezeDemographics(short profitYear, DateTime asOfDateTime, CancellationToken cancellationToken = default)
    {

        return _dataContextFactory.UseWritableContext(async ctx =>
        {
            //Inactivate any prior frozen states
            await ctx.FrozenStates.Where(x => x.ProfitYear == profitYear && x.IsActive).ForEachAsync(x => x.IsActive = false, cancellationToken);

            //Create new record
            var frozenState = new FrozenState() { IsActive = true, ProfitYear = profitYear, AsOfDateTime = asOfDateTime };
            ctx.FrozenStates.Add(frozenState);

            await ctx.SaveChangesAsync(cancellationToken);

            return new SetFrozenStateResponse()
            {
                Id = frozenState.Id,
                ProfitYear = profitYear
            };
        }, cancellationToken);
    }
}
