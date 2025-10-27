using System.Reflection;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace Demoulas.ProfitSharing.UnitTests.Common.Mocks;

/// <summary>
/// Test factory supporting both InMemory database (via Create - EXPERIMENTAL) and mock-based testing (via parameterless constructor - RECOMMENDED).
///
/// RECOMMENDED APPROACH: Use parameterless constructor + mock setup patterns
/// - Instantiate: `var factory = new ScenarioDataContextFactory()`
/// - Setup mocks: `factory.ProfitSharingDbContext.Setup(...).Returns(...)`
/// - Or use ScenarioFactory.BuildMocks() for standard setups
///
/// EXPERIMENTAL APPROACH (NOT READY): InMemory database via Create()
/// - Purpose: Provide real DbContext for testing without mocking IQueryable
/// - Status: Has significant limitations, not production-ready
/// - Known Issues:
///   * Navigation properties cause validation errors (EmploymentStatus.Id, etc.)
///   * Requires extensive lookup data seeding (7+ lookup tables)
///   * ExecuteUpdate/ExecuteDelete not supported by InMemory provider
///   * FromSqlRaw queries not supported by InMemory provider
///   * InvalidCastException when casting writable to readonly context
/// - Estimated Effort to Fix: 14-22 hours (see CLAUDE.md for breakdown)
/// 
/// LIMITATION: Mock IQueryable Async Support
/// Mock-based IQueryable from MockQueryable does NOT implement IAsyncQueryProvider.
/// This means EF Core async methods (.ToListAsync(), .FirstOrDefaultAsync(), etc.) will fail with:
/// "The source IQueryable doesn't implement IAsyncEnumerable"
/// 
/// Workaround options:
/// 1. Use synchronous methods (.ToList(), .FirstOrDefault()) in tests  
/// 2. Switch to InMemory database (requires fixing issues above)
/// 3. Use real database with test containers (highest reliability, highest setup cost)
/// </summary>
public sealed class ScenarioDataContextFactory : IProfitSharingDataContextFactory
{
    private readonly ProfitSharingDbContext? _writableContext;
    private readonly bool _isInMemoryMode;

    // Mock-based properties (for backward compatibility with existing tests)
    public Mock<ProfitSharingDbContext> ProfitSharingDbContext { get; }
    public Mock<ProfitSharingReadOnlyDbContext> ProfitSharingReadOnlyDbContext { get; }
    public Mock<DemoulasCommonDataContext> StoreInfoDbContext { get; }

    /// <summary>
    /// Parameterless constructor for mock-based testing.
    /// Tests using this constructor must setup their own mocks via ProfitSharingDbContext/ProfitSharingReadOnlyDbContext properties.
    /// </summary>
    public ScenarioDataContextFactory()
    {
        _isInMemoryMode = false;
        ProfitSharingDbContext = new Mock<ProfitSharingDbContext>();
        ProfitSharingReadOnlyDbContext = new Mock<ProfitSharingReadOnlyDbContext>();
        StoreInfoDbContext = new Mock<DemoulasCommonDataContext>();
    }

    /// <summary>
    /// Private constructor for InMemory database mode (used by Create).
    /// </summary>
    private ScenarioDataContextFactory(ProfitSharingDbContext writableContext)
    {
        _isInMemoryMode = true;
        _writableContext = writableContext;

        // Initialize mock properties to prevent null reference in legacy code
        ProfitSharingDbContext = new Mock<ProfitSharingDbContext>();
        ProfitSharingReadOnlyDbContext = new Mock<ProfitSharingReadOnlyDbContext>();
        StoreInfoDbContext = new Mock<DemoulasCommonDataContext>();
    }

    /// <summary>
    /// Creates a factory with InMemory database populated with test data.
    /// Both writable and read-only contexts share the same in-memory database.
    /// 
    /// EXPERIMENTAL/NOT READY: This method has known limitations and should not be used yet.
    /// See https://github.com/your-org/repo/issues/XXX for tracking InMemory implementation.
    /// 
    /// Known Issues:
    /// - Navigation properties cause EF Core validation errors (EmploymentStatus.Id, etc.)
    /// - ExecuteUpdate/ExecuteDelete not supported by InMemory provider
    /// - Requires extensive lookup data seeding (Department, EmploymentStatus, Gender, etc.)
    /// - FromSqlRaw queries not supported by InMemory provider
    /// 
    /// Use ScenarioFactory.BuildMocks() instead for standard mock-based testing.
    /// </summary>
#pragma warning disable S1133 // Obsolete method intentionally kept for future InMemory implementation
    [Obsolete("InMemory database approach has known limitations. Use parameterless constructor and mock setup instead.", error: false)]
#pragma warning restore S1133
    public static ScenarioDataContextFactory Create(
        List<Demographic>? demographics = null,
        List<DemographicHistory>? histories = null,
        List<DemographicSyncAudit>? audits = null,
        List<BeneficiaryContact>? beneficiaryContacts = null,
        List<ProfitDetail>? profitDetails = null)
    {
        // Use unique database name for test isolation
        var databaseName = $"TestDb_{Guid.NewGuid()}";

        // Create InMemory database options - shared between contexts
        var options = new DbContextOptionsBuilder<ProfitSharingDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        // Create writable context
        var writableContext = new ProfitSharingDbContext(options);

        // Seed data if provided - clear navigation properties to avoid EF Core trying to add them
        if (demographics?.Any() == true)
        {
            // Clear navigation properties that cause issues with InMemory seeding
            foreach (var demo in demographics)
            {
                demo.EmploymentStatus = null;
                demo.EmploymentType = null;
                demo.Gender = null;
                demo.PayFrequency = null;
                demo.Department = null;
                demo.TerminationCode = null;
                demo.PayClassification = null;
            }
            writableContext.Demographics.AddRange(demographics);
        }
        if (histories?.Any() == true)
        {
            writableContext.DemographicHistories.AddRange(histories);
        }
        if (audits?.Any() == true)
        {
            writableContext.DemographicSyncAudit.AddRange(audits);
        }
        if (beneficiaryContacts?.Any() == true)
        {
            writableContext.BeneficiaryContacts.AddRange(beneficiaryContacts);
        }
        if (profitDetails?.Any() == true)
        {
            writableContext.ProfitDetails.AddRange(profitDetails);
        }

        // Save initial seed data
        writableContext.SaveChanges();

        return new ScenarioDataContextFactory(writableContext);
    }

    public async Task UseWritableContext(Func<ProfitSharingDbContext, Task> func, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_isInMemoryMode)
            {
                await func.Invoke(_writableContext!);
            }
            else
            {
                // Mock mode - invoke with mock object
                await func.Invoke(ProfitSharingDbContext.Object);
            }
        }
        catch (TargetInvocationException ex)
        {
            switch (ex.InnerException)
            {
                case null:
                    throw;
                default:
                    throw ex.InnerException;
            }
        }
    }

    public async Task<T> UseWritableContext<T>(Func<ProfitSharingDbContext, Task<T>> func, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_isInMemoryMode)
            {
                return await func.Invoke(_writableContext!);
            }
            else
            {
                // Mock mode - invoke with mock object
                return await func.Invoke(ProfitSharingDbContext.Object);
            }
        }
        catch (TargetInvocationException ex)
        {
            switch (ex.InnerException)
            {
                case null:
                    throw;
                default:
                    throw ex.InnerException;
            }
        }
    }

    public async Task<T> UseWritableContextAsync<T>(
        Func<ProfitSharingDbContext, IDbContextTransaction, Task<T>> action,
        CancellationToken cancellationToken)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_isInMemoryMode)
            {
                // InMemory database doesn't support transactions - pass null
                return await action.Invoke(_writableContext!, null!).ConfigureAwait(false);
            }
            else
            {
                // Mock mode - invoke with mock object
                return await action.Invoke(ProfitSharingDbContext.Object, null!).ConfigureAwait(false);
            }
        }
        catch (TargetInvocationException ex) when (ex.InnerException != null)
        {
            throw ex.InnerException;
        }
    }

    public async Task<T> UseReadOnlyContext<T>(Func<ProfitSharingReadOnlyDbContext, Task<T>> func, CancellationToken cancellationToken = default)
    {
        try
        {
            if (_isInMemoryMode)
            {
                // Cast writable context to readonly - InMemory doesn't support separate readonly context instances properly
                return await func.Invoke((ProfitSharingReadOnlyDbContext)(object)_writableContext!);
            }
            else
            {
                // Mock mode - invoke with mock object
                return await func.Invoke(ProfitSharingReadOnlyDbContext.Object);
            }
        }
        catch (TargetInvocationException ex)
        {
            switch (ex.InnerException)
            {
                case null:
                    throw;
                default:
                    throw ex.InnerException;
            }
        }
    }

#pragma warning disable RCS1229
    public Task UseReadOnlyContext(Func<ProfitSharingReadOnlyDbContext, Task> func, CancellationToken cancellationToken = new CancellationToken())
#pragma warning restore RCS1229
    {
        try
        {
            if (_isInMemoryMode)
            {
                // Cast writable context to readonly - InMemory doesn't support separate readonly context instances properly
                return func.Invoke((ProfitSharingReadOnlyDbContext)(object)_writableContext!);
            }
            else
            {
                // Mock mode - invoke with mock object
                return func.Invoke(ProfitSharingReadOnlyDbContext.Object);
            }
        }
        catch (TargetInvocationException ex)
        {
            switch (ex.InnerException)
            {
                case null:
                    throw;
                default:
                    throw ex.InnerException;
            }
        }
    }

    public async Task<T> UseStoreInfoContext<T>(Func<DemoulasCommonDataContext, Task<T>> func)
    {
        try
        {
            return await func.Invoke(StoreInfoDbContext.Object);
        }
        catch (TargetInvocationException ex)
        {
            switch (ex.InnerException)
            {
                case null:
                    throw;
                default:
                    throw ex.InnerException;
            }
        }
    }
}
