# Smart Profit Sharing Services Test Plan

## 1. Purpose

Establish comprehensive, enforceable tests for the unified Result<T> + typed HTTP union pattern, endpoint semantics, domain invariants (history/audit), performance safeguards, and regression stability as additional endpoint batches are refactored.

## 2. Scope

Included: Service layer logic, FastEndpoints endpoint contracts, mapping, history/audit behaviors, authorization flows, error mapping, empty vs not-found semantics, performance characteristics of dynamic filtering and batched operations.
Excluded (current phase): Full E2E UI flows, Oracle real integration (stubbed or containerized later), RabbitMQ integration behavior (future phase), long-running job orchestration.

## 3. Test Categories

- Unit Tests: Pure functions, mapping, Result extensions, service business logic.
- Endpoint Tests: FastEndpoints host with in-memory test server validating HTTP contract and auth.
- Integration Tests (incremental): Service + EF Core with lightweight DB (SQLite/Oracle container optional).
- Contract / Snapshot Tests: DTO payload shapes, list envelopes, error bodies.
- Performance Guard Tests: Query count and response time heuristics for master inquiry & batch demographics sync.
- Invariant Tests: Demographics history correctness, audit insertion batching.
- Security Tests: Authorization policies and role gating.
- Regression Safety: Snapshot/golden tests for key payloads.

## 4. Critical Test Objectives

1. Empty collection → 200 (never 404) for search & list endpoints.
2. True absence → 404 with no sensitive details.
3. Validation failure → Problem (400/422) with structured errors.
4. Domain error vs unexpected exception mapping differentiation.
5. No endpoint-level DbContext usage (reflection guard test).
6. Demographic history closure creates new row & sets ValidTo of prior active row.
7. Duplicate SSN audit creates expected audit rows & masks SSN.
8. Authorization: 401 (no token), 403 (wrong role), 200 (correct role).

## 5. Representative Test Cases

### Result Mapping

- Success → Ok<T>.
- Failure (known not-found) → NotFound.
- Failure (other) → Problem.
- ValidationFailure → Problem with validation details.

### Master Inquiry Search

- No filters, dataset present → 200 with page metadata.
- Filters narrowing to zero → 200, total 0.
- Multiple filters (AND semantics) maintain consistent total.

### Demographics History

- Insert new employee → history row created with default ValidFrom.
- Update non-history-changing fields → no new row.
- Update history-relevant field → closes prior row (ValidTo set) and inserts new with ValidFrom = previous ValidTo.

### Security

- Missing token → 401.
- Token lacking role → 403.
- Token with required role → 200.

### Performance (heuristic)

- Master inquiry with 50 OR conditions executes <= baseline query count (test asserts no proportional N+1 pattern).

## 6. Tooling & Utilities

- Builders (Bogus) for entity creation.
- Reflection-based guard to fail if endpoint assembly references EF Core DbSet usage directly.
- Optional query interception (future) for performance assertions.
- Snapshot harness (future enhancement) for JSON contract.

## 7. Risks & Mitigations

- Stale binaries: Always run tests without --no-build (CI enforced).
- Mapping drift: Add mapper completeness tests.
- Silent DbContext leaks: Reflection guard.
- Performance regressions: Add baseline query count metrics.

## 8. Initial Wave (This PR)

- ResultHttpExtensionsTests
- MasterInquirySearchEndpointTests (empty result scenario)
- EndpointNoDbContextUsageTests
- DemographicsHistoryServiceTests (core history behaviors)

## 9. Success Metrics

- 100% pass rate existing + new tests.
- Core infra (ResultHttpExtensions, Demographics history) coverage > 90%.
- No newly introduced 404 for empty list scenarios.

## 10. Future Enhancements

- Introduce snapshot tests for representative payloads.
- Add retry/resilience tests (transient DB failures).
- Add performance benchmarks with BenchmarkDotNet (conditional).
- Add OpenAPI diff gating (if/when spec generation is wired).

---

Generated on 2025-09-20.
