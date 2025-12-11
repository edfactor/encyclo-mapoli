using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.Data.Interfaces;

/// <summary>
/// Repository interface for Demographics data access operations.
/// Provides abstraction over EF Core DbContext for testability and separation of concerns.
/// </summary>
public interface IDemographicsRepository
{
    #region Query Operations

    /// <summary>
    /// Retrieves demographics by Oracle HCM IDs with related entities (ContactInfo, Address).
    /// </summary>
    /// <param name="oracleIds">Collection of Oracle HCM IDs to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of matching demographics with includes</returns>
    Task<List<Demographic>> GetByOracleIdsAsync(
        IEnumerable<long> oracleIds,
        CancellationToken ct);

    /// <summary>
    /// Retrieves demographics by (SSN, BadgeNumber) pairs with related entities.
    /// Filters out pairs where BadgeNumber is 0 to prevent degenerate queries.
    /// </summary>
    /// <param name="pairs">Collection of (SSN, BadgeNumber) pairs to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of matching demographics with includes</returns>
    Task<List<Demographic>> GetBySsnAndBadgePairsAsync(
        IEnumerable<(int Ssn, int BadgeNumber)> pairs,
        CancellationToken ct);

    /// <summary>
    /// Retrieves a single demographic by SSN.
    /// </summary>
    /// <param name="ssn">Social Security Number</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Demographic if found, null otherwise</returns>
    Task<Demographic?> GetBySsnAsync(int ssn, CancellationToken ct);

    /// <summary>
    /// Retrieves demographics that have duplicate SSNs from the provided collection.
    /// Groups by SSN and returns only those with count > 1.
    /// </summary>
    /// <param name="ssns">Collection of SSNs to check for duplicates</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of demographics with duplicate SSNs</returns>
    Task<List<Demographic>> GetDuplicateSsnsAsync(
        IEnumerable<int> ssns,
        CancellationToken ct);

    /// <summary>
    /// Retrieves demographics by multiple SSNs.
    /// </summary>
    /// <param name="ssns">Collection of SSNs to search for</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of demographics matching the SSNs</returns>
    Task<List<Demographic>> GetBySsnsAsync(
        IEnumerable<int> ssns,
        CancellationToken ct);

    #endregion

    #region Command Operations

    /// <summary>
    /// Inserts a new demographic record.
    /// Does not call SaveChanges - caller is responsible for transaction management.
    /// </summary>
    /// <param name="demographic">Demographic to insert</param>
    /// <param name="ct">Cancellation token</param>
    Task AddAsync(Demographic demographic, CancellationToken ct);

    /// <summary>
    /// Inserts multiple demographic records in batch.
    /// Does not call SaveChanges - caller is responsible for transaction management.
    /// </summary>
    /// <param name="demographics">Demographics to insert</param>
    /// <param name="ct">Cancellation token</param>
    Task AddRangeAsync(IEnumerable<Demographic> demographics, CancellationToken ct);

    /// <summary>
    /// Updates an existing demographic record.
    /// Assumes demographic is already tracked by context.
    /// Does not call SaveChanges - caller is responsible for transaction management.
    /// </summary>
    /// <param name="demographic">Demographic to update</param>
    void Update(Demographic demographic);

    /// <summary>
    /// Updates multiple demographic records in batch.
    /// Assumes demographics are already tracked by context.
    /// Does not call SaveChanges - caller is responsible for transaction management.
    /// </summary>
    /// <param name="demographics">Demographics to update</param>
    void UpdateRange(IEnumerable<Demographic> demographics);

    #endregion

    #region Related Entity Operations

    /// <summary>
    /// Updates SSN references in related entities (BeneficiaryContacts, ProfitDetails).
    /// Used when an employee's SSN changes.
    /// </summary>
    /// <param name="oldSsn">Old SSN value</param>
    /// <param name="newSsn">New SSN value</param>
    /// <param name="ct">Cancellation token</param>
    Task UpdateRelatedSsnAsync(int oldSsn, int newSsn, CancellationToken ct);

    #endregion



    #region Transaction Management

    /// <summary>
    /// Saves all pending changes to the database.
    /// Should be called by the service layer to commit transactions.
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Number of entities written to database</returns>
    Task<int> SaveChangesAsync(CancellationToken ct);

    #endregion
}
