# Demoulas.ProfitSharing.OracleHcm

## Overview
This project integrates with Oracle HCM Cloud to synchronize employee and payroll data into the Profit Sharing system. It uses scheduled background jobs to perform full, delta, and payroll synchronizations, leveraging Oracle HCM REST APIs and Atom feeds.

---

## Major Processes & Workflows

### 1. **Employee Full Sync**
- **Purpose:** Retrieve all employee demographic data from Oracle HCM and queue it for processing.
- **Workflow:**
  1. Scheduled by `EmployeeFullSyncService` (Quartz job).
  2. Calls `EmployeeSyncService.ExecuteFullSyncAsync()`.
  3. Uses `EmployeeFullSyncClient.GetAllEmployees()` to page through `/workers` API.
  4. Queues batches of employees for downstream processing via message bus.
- **Performance/Issues:**
  - Uses small page sizes (50) to avoid timeouts, but this increases API calls.
  - No parallelization of API calls.
  - No retry/backoff for transient API failures.
- **Improvements:**
  - Use larger page sizes if API allows, or parallelize requests.
  - Implement retry logic for transient errors.
  - Use the latest Oracle HCM Workers API fields and filtering for efficiency.

### 2. **Employee Delta Sync**
- **Purpose:** Retrieve only changed employees (new hires, updates, terminations, assignments) since last sync.
- **Workflow:**
  1. Scheduled by `EmployeeDeltaSyncService` (Quartz job).
  2. Calls `EmployeeSyncService.ExecuteDeltaSyncAsync()`.
  3. Uses `AtomFeedClient.GetFeedDataAsync()` for each feed type (newhire, empassignment, empupdate, termination).
  4. Merges results, deduplicates by PersonId, and fetches current data for each.
  5. Queues for downstream processing.
- **Performance/Issues:**
  - Atom feed page size is small (25), leading to many requests.
  - Feeds are processed sequentially, not in parallel.
  - No deduplication across feeds until after all are loaded.
- **Improvements:**
  - Parallelize feed fetching.
  - Increase page size if possible.
  - Use Oracle HCM's change tracking endpoints if available.

### 3. **Payroll Sync**
- **Purpose:** Retrieve payroll process results and update profit sharing records.
- **Workflow:**
  1. Scheduled by `EmployeePayrollSyncService` (Quartz job).
  2. Calls `PayrollSyncClient.RetrievePayrollBalancesAsync()`.
  3. Pages through `/personProcessResults` API for payroll actions.
  4. For each payroll item, fetches balance types (dollars, hours, weeks) via additional API calls.
  5. Updates or creates `PayProfit` records in the database.
- **Performance/Issues:**
  - For each payroll item, makes 3+ additional API calls (one per balance type).
  - No batching or parallelization of balance type requests.
  - No caching of static data (e.g., balance type IDs).
- **Improvements:**
  - Batch balance type requests if API supports.
  - Parallelize balance type fetches per payroll item.
  - Use Oracle HCM's bulk endpoints if available.

---

## General Observations
- **Error Handling:** Most jobs log and audit errors, but retry/backoff is not implemented.
- **Resource Usage:** Manual GC.Collect is called after jobs, which is not ideal in modern .NET.
- **Configuration:** Page sizes and intervals are conservative; could be tuned for higher throughput.
- **API Usage:** Uses older style Atom feeds and REST endpoints; Oracle HCM 25B+ offers improved APIs and filtering.

---

## Recommendations (Oracle HCM 25B+)
- Use the [Workers API](https://docs.oracle.com/en/cloud/saas/human-resources/25b/farws/api-workers.html) for more efficient employee syncs.
- Use the [Person Process Results API](https://docs.oracle.com/en/cloud/saas/human-resources/25b/farws/api-person-process-results.html) for payroll, leveraging filtering and bulk options.
- Implement retry/backoff for all API calls.
- Consider parallelizing paged requests and balance fetches.
- Remove explicit GC.Collect calls; let .NET manage memory.
- Monitor and tune page sizes and intervals based on real-world performance.

---

## Mermaid.js Diagram

```
flowchart TD
    subgraph SyncJobs
        FullSync[Employee Full Sync]
        DeltaSync[Employee Delta Sync]
        PayrollSync[Payroll Sync]
    end
    FullSync -->|/workers API| Queue[Queue Employees]
    DeltaSync -->|Atom Feeds| Queue
    PayrollSync -->|/personProcessResults API| PayrollProcess[Process Payroll Items]
    PayrollProcess -->|Balance Type API| UpdateProfit[Update PayProfit Records]
    Queue -->|Message Bus| Downstream[Downstream Processing]
```

---

## See Also
- [Oracle HCM Workers API](https://docs.oracle.com/en/cloud/saas/human-resources/25b/farws/api-workers.html)
- [Oracle HCM Person Process Results API](https://docs.oracle.com/en/cloud/saas/human-resources/25b/farws/api-person-process-results.html)
