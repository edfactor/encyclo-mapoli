using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.Data.Repositories;

/// <summary>
/// Repository implementation for Demographics data access operations.
/// Encapsulates EF Core operations and provides clean abstraction for services.
/// Uses factory pattern directly for context management.
/// </summary>
public sealed class DemographicsRepository : IDemographicsRepository
{
    private readonly IProfitSharingDataContextFactory _contextFactory;

    public DemographicsRepository(IProfitSharingDataContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    #region Query Operations

    public Task<List<Demographic>> GetByOracleIdsAsync(
        IEnumerable<long> oracleIds,
        CancellationToken ct)
    {
        return _contextFactory.UseReadOnlyContext(async ctx =>
        {
            var idList = oracleIds.ToList();
            if (idList.Count == 0)
            {
                return new List<Demographic>();
            }

            return await ctx.Demographics
                .TagWith($"DemographicsRepo-GetByOracleIds-Count:{idList.Count}")
                .Where(d => idList.Contains(d.OracleHcmId))
                .Include(d => d.ContactInfo)
                .Include(d => d.Address)
                .ToListAsync(ct);
        }, ct);
    }

    public Task<List<Demographic>> GetBySsnAndBadgePairsAsync(
        IEnumerable<(int Ssn, int BadgeNumber)> pairs,
        CancellationToken ct)
    {
        return _contextFactory.UseReadOnlyContext(async ctx =>
        {
            var pairsList = pairs
                .Where(p => p.BadgeNumber != 0) // Filter out zero badges to prevent degenerate queries
                .Distinct()
                .ToList();

            if (pairsList.Count == 0)
            {
                return new List<Demographic>();
            }

            // Build dynamic SQL with parameters for Oracle
            var clauses = new List<string>();
            var parameters = new List<OracleParameter>();

            for (int i = 0; i < pairsList.Count; i++)
            {
                var (ssn, badge) = pairsList[i];
                string pSsn = $":p{i * 2}";
                string pBadge = $":p{i * 2 + 1}";

                parameters.Add(new OracleParameter($"p{i * 2}", ssn));
                parameters.Add(new OracleParameter($"p{i * 2 + 1}", badge));

                clauses.Add($"(d.SSN = {pSsn} AND d.BADGE_NUMBER = {pBadge})");
            }

            string sql = $"SELECT * FROM DEMOGRAPHIC d WHERE {string.Join(" OR ", clauses)}";

            return await ctx.Demographics
                .FromSqlRaw(sql, parameters.ToArray())
                .TagWith($"DemographicsRepo-GetBySsnAndBadge-Count:{pairsList.Count}")
                .Include(d => d.ContactInfo)
                .Include(d => d.Address)
                .ToListAsync(ct);
        }, ct);
    }

    public Task<Demographic?> GetBySsnAsync(int ssn, CancellationToken ct)
    {
        return _contextFactory.UseReadOnlyContext(async ctx =>
        {
            return await ctx.Demographics
                .TagWith($"DemographicsRepo-GetBySsn-Ssn:{ssn}")
                .FirstOrDefaultAsync(d => d.Ssn == ssn, ct);
        }, ct);
    }

    public Task<List<Demographic>> GetDuplicateSsnsAsync(
        IEnumerable<int> ssns,
        CancellationToken ct)
    {
        return _contextFactory.UseReadOnlyContext(async ctx =>
        {
            var ssnList = ssns.Distinct().ToList();
            if (ssnList.Count == 0)
            {
                return new List<Demographic>();
            }

            return await ctx.Demographics
                .TagWith($"DemographicsRepo-GetDuplicateSsns-Count:{ssnList.Count}")
                .Where(d => ssnList.Contains(d.Ssn))
                .GroupBy(d => d.Ssn)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g)
                .ToListAsync(ct);
        }, ct);
    }

    #endregion

    #region Command Operations

    public Task AddAsync(Demographic demographic, CancellationToken ct)
    {
        return _contextFactory.UseWritableContext(async ctx =>
        {
            await ctx.Demographics.AddAsync(demographic, ct);
            await ctx.SaveChangesAsync(ct);
        }, ct);
    }

    public Task AddRangeAsync(IEnumerable<Demographic> demographics, CancellationToken ct)
    {
        return _contextFactory.UseWritableContext(async ctx =>
        {
            await ctx.Demographics.AddRangeAsync(demographics, ct);
            await ctx.SaveChangesAsync(ct);
        }, ct);
    }

    public void Update(Demographic demographic)
    {
        // Not implemented - use ExecuteWithChanges for update operations
        throw new NotImplementedException(
            "Use ExecuteWithChangesAsync to perform updates within a transaction context");
    }

    public void UpdateRange(IEnumerable<Demographic> demographics)
    {
        // Not implemented - use ExecuteWithChanges for update operations
        throw new NotImplementedException(
            "Use ExecuteWithChangesAsync to perform updates within a transaction context");
    }

    /// <summary>
    /// Executes a function that performs multiple operations within a single transaction context.
    /// Use this for complex operations that need queries, updates, and saves together.
    /// </summary>
    /// <example>
    /// var result = await repo.ExecuteWithChangesAsync(async ctx =>
    /// {
    ///     var demo = await ctx.Demographics.FindAsync(id);
    ///     demo.Ssn = newSsn;
    ///     return await ctx.SaveChangesAsync(ct);
    /// }, ct);
    /// </example>
    public Task<TResult> ExecuteWithChangesAsync<TResult>(
        Func<ProfitSharingDbContext, Task<TResult>> operation,
        CancellationToken ct)
    {
        return _contextFactory.UseWritableContext(operation, ct);
    }

    #endregion

    #region Related Entity Operations

    public Task UpdateRelatedSsnAsync(int oldSsn, int newSsn, CancellationToken ct)
    {
        return _contextFactory.UseWritableContext(async ctx =>
        {
            // Update BeneficiaryContacts
            await ctx.BeneficiaryContacts
                .Where(b => b.Ssn == oldSsn)
                .ExecuteUpdateAsync(s => s.SetProperty(b => b.Ssn, newSsn), ct);

            // Update ProfitDetails
            await ctx.ProfitDetails
                .Where(p => p.Ssn == oldSsn)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.Ssn, newSsn), ct);
        }, ct);
    }

    public Task<List<Demographic>> GetBySsnsAsync(IEnumerable<int> ssns, CancellationToken ct)
    {
        return _contextFactory.UseReadOnlyContext(async ctx =>
        {
            var ssnList = ssns.ToList();
            if (ssnList.Count == 0)
            {
                return new List<Demographic>();
            }

            return await ctx.Demographics
                .TagWith($"DemographicsRepo-GetBySsns-Count:{ssnList.Count}")
                .Where(d => ssnList.Contains(d.Ssn))
                .ToListAsync(ct);
        }, ct);
    }

    #endregion

    #region Transaction Management

    public Task<int> SaveChangesAsync(CancellationToken ct)
    {
        throw new NotImplementedException(
            "SaveChangesAsync requires a context. Use ExecuteWithChangesAsync or Add/AddRange methods instead.");
    }

    #endregion
}
