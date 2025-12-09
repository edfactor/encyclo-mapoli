# Duplicate Names and Birthdays Report

This document explains the `GetDuplicateNamesAndBirthdaysAsync` report implemented in the backend `CleanupReportService`.

## Purpose

The report finds potentially duplicate demographic records by comparing full names and birthdates. It's intended to help data cleanup teams identify records that may be duplicates due to data-entry errors, variations in name spelling, or small differences in recorded birth dates.

## High-level algorithm

1. Identify candidate duplicate name pairs (production):

   - Run an Oracle-optimized SQL query that:
     - Excludes records with placeholder/`FAKE_SSNS` values.
     - Uses `UTL_MATCH.EDIT_DISTANCE` to compute name similarity with a threshold (< 3).
     - Uses `SOUNDEX` to restrict to phonetic matches.
     - Allows a ±3 day tolerance on `DATE_OF_BIRTH` to accommodate small entry differences.
     - Joins the filtered demographic rows to themselves (p1 < p2) to avoid self-joins and duplicates.
     - Returns both directions via `UNION ALL` (so each FullName maps to matched badge ids).

2. Fallback for test/in-memory environments:

   - The in-memory provider cannot run Oracle functions. For tests, the service projects FullName and Id pairs directly from the demographics query to allow tests to exercise the LINQ codepath.

3. Materialize the set of candidate full names and use it to filter the main demographics projection.

4. For matching demographics, join to PayProfit (left), balance set, and years-of-service lookups to produce a rich projection that includes:

   - Badge, masked SSN, address, employment status and store,
   - Net balance (from totals service),
   - Hours/Income for the configured profit year,
   - Years of service and hire/termination dates.

5. Project into API DTOs and compute Count per badge (how many matched partners were found).

## Business decisions and assumptions

- The name similarity threshold (EDIT_DISTANCE < 3) and the 3-day DOB tolerance are business rules chosen to balance false positives and negatives. They can be adjusted if needed, but expect trade-offs.

- Records with SSNs present in `FAKE_SSNS` are explicitly excluded from duplicate detection. This table is used for legacy or placeholder SSNs that shouldn't match real records.

- The production path uses Oracle-specific functions and therefore cannot be executed by in-memory database providers used in unit tests. The fallback ensures tests can still validate the broader projection logic.

- The report is read-only and should not modify any data.

## Where to find the implementation

- Backend service: `src/services/src/Demoulas.ProfitSharing.Services/Reports/CleanupReportService.cs` — method `GetDuplicateNamesAndBirthdaysAsync`.

## Suggested follow-ups

- Add unit tests for the LINQ fallback path to assert projection shapes and counting logic.
- Consider extracting the SQL query into a named constant or resource to make testing and maintenance easier.
- If duplicate detection thresholds need tuning, add configuration flags or feature flags to avoid code changes for small business adjustments.

---

Generated documentation for developer reference.
