# Testing Infrastructure Limitations

## Current State (October 24, 2025)

### Mock-Based Testing (CURRENT APPROACH)

**Status**: ✅ Working for most scenarios  
**Implementation**: `ScenarioDataContextFactory` (parameterless constructor) + `MockQueryable.Moq`

**Advantages**:

- Works with existing test patterns
- Fast test execution
- Easy to setup specific test scenarios
- No database infrastructure required

**Known Limitations**:

1. **Async Query Provider Not Supported**

   - Mock IQueryable does NOT implement `IAsyncQueryProvider`
   - EF Core async methods (`.ToListAsync()`, `.FirstOrDefaultAsync()`, etc.) fail with:
     ```
     "The source IQueryable doesn't implement IAsyncEnumerable"
     ```
   - **Impact**: Tests using async LINQ methods will fail
   - **Workaround**: Use synchronous methods in tests or switch to InMemory database

2. **No CRUD Persistence**
   - `.Add()`, `.Remove()`, `.Update()` operations don't persist across queries
   - Each query sees a snapshot of the initial seed data
   - **Impact**: Tests validating Add/Update/Delete need manual mock setup
   - **Workaround**: Use backing lists with `BuildMockDbSetWithBackingList()`

### InMemory Database Testing (EXPERIMENTAL - NOT READY)

**Status**: ⚠️ Experimental, significant blockers  
**Implementation**: `ScenarioDataContextFactory.Create()` - marked `[Obsolete]`

**Potential Advantages**:

- Real EF Core behavior including IAsyncQueryProvider
- CRUD operations persist naturally
- No mock setup complexity

**Current Blockers** (preventing production use):

1. **Navigation Property Validation Errors**

   - Demographics entity has 7+ navigation properties (EmploymentStatus, EmploymentType, Gender, etc.)
   - EF Core tries to validate/add related entities during seeding
   - Error: `"The property 'EmploymentStatus.Id' does not have a value set and no value generator is available"`
   - **Fix Required**: Seed all lookup tables before adding Demographics

2. **ExecuteUpdate/ExecuteDelete Not Supported**

   - InMemory provider doesn't support `ExecuteUpdateAsync()` / `ExecuteDeleteAsync()`
   - Error: `"The methods 'ExecuteUpdate' and 'ExecuteUpdateAsync' are not supported by the current database provider"`
   - **Fix Required**: Rewrite repository methods to use `.ToList()` then modify in memory

3. **FromSqlRaw Not Supported**

   - InMemory provider doesn't support raw SQL queries
   - **Fix Required**: Rewrite repository methods to use LINQ (affects 2 tests)

4. **ReadOnly Context Casting Issues**

   - Cannot cast `ProfitSharingDbContext` to `ProfitSharingReadOnlyDbContext`
   - Current workaround causes validation issues
   - **Fix Required**: Rethink readonly context strategy for InMemory

5. **Lookup Data Seeding Required**
   - Need to populate 7+ lookup tables:
     - Department (byte ID)
     - EmploymentStatus (char ID)
     - EmploymentType (char ID)
     - Gender (char ID)
     - PayFrequency (byte ID)
     - TerminationCode (char ID)
     - PayClassification (string ID)
   - **Fix Required**: Extend `ScenarioDataContextFactory.Create()` with lookup parameters

### Test Results Summary

**DemographicsRepositoryTests**:

- Total: 17 tests
- Passing: 1 test
- Failing: 16 tests
  - 11 InvalidCastException (readonly context casting)
  - 3 NotSupportedException (navigation property validation)
  - 2 InvalidOperationException (ExecuteUpdate not supported)

## Effort Estimate to Fix InMemory Approach

| Task                                   | Effort          | Priority |
| -------------------------------------- | --------------- | -------- |
| Seed lookup data in Create()           | 4-6 hours       | High     |
| Update ScenarioFactory to pass lookups | 1-2 hours       | High     |
| Modify DemographicFaker for InMemory   | 2-3 hours       | Medium   |
| Handle readonly context properly       | 2-3 hours       | High     |
| Rewrite FromSqlRaw methods to LINQ     | 2-3 hours       | Medium   |
| Rewrite ExecuteUpdate to LINQ          | 2-3 hours       | High     |
| Update all test files                  | 2-4 hours       | Low      |
| Testing and validation                 | 2-3 hours       | High     |
| **TOTAL**                              | **17-27 hours** |          |

## Recommendations

### Short Term (Current Sprint)

✅ **Use mock-based testing** - It works for most scenarios  
✅ **Document limitations** - This file serves as that documentation  
✅ **Mark InMemory as experimental** - Done via `[Obsolete]` attribute

### Medium Term (Next Quarter)

Consider switching to InMemory if:

- Async query testing becomes critical
- Team has 17-27 hours to invest in fixing blockers
- Benefits outweigh the migration effort

### Long Term (Future)

**Real Database Testing** (highest reliability):

- Use SQLite InMemory as alternative to EF Core InMemory
- Use test containers (Docker) with real Oracle database
- Both approaches eliminate mock limitations entirely
- Trade-off: Higher setup complexity, slower test execution

## Related Files

- `ScenarioDataContextFactory.cs` - Test factory with both approaches
- `ScenarioFactory.cs` - Higher-level test scenario builder
- `DemographicsRepositoryTests.cs` - Main test file affected
- `CLAUDE.md` - AI conversation history documenting investigation

## References

- [MockQueryable GitHub](https://github.com/romantitov/MockQueryable) - Mock IQueryable limitations
- [EF Core InMemory Provider Docs](https://learn.microsoft.com/en-us/ef/core/providers/in-memory/) - InMemory limitations
- [EF Core Testing Docs](https://learn.microsoft.com/en-us/ef/core/testing/) - Testing strategies

---

**Last Updated**: October 24, 2025  
**Status**: Mock-based approach working, InMemory approach blocked  
**Next Review**: After completing current library upgrade work
