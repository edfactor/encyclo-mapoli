# Military Contributions: Validation, Behavior, and Operations

This document consolidates the business rules, validator coverage, UI behavior, and operational guidance for Military Contributions. It is intended for multiple audiences: leadership, production support, DevOps/Operations, QA, and Development.

## Executive Summary (C‑Suite)
- We enforce strict validations to prevent duplicate and ineligible military contributions.
- Contributions are allowed only when the employee is Active as of the contribution date, at least 21 years old, and not before their earliest hire year.
- Regular contributions are unique per employee per contribution year; supplemental contributions are allowed alongside regular ones.
- When posting to a profit year different from the contribution date year, the contribution must be marked Supplemental (no Years Of Service credit). This aligns with the domain rule that `IsSupplementalContribution` determines `YearsOfServiceCredit` in `ProfitDetail`.
- Telemetry records validation failure reasons with low-cardinality tags for monitoring and alerting.

## Rule Matrix (Business + System Behavior)
- Badge number
  - Must be `> 0` and exist.
  - If not found, request is rejected.
- Contribution amount
  - Must be `> 0`.
- Contribution date
  - Must not be in the future.
  - Must be on/after the employee’s earliest known hire year.
  - Employee must be at least 21 years old on the contribution date.
  - Employment status must be Active as-of the contribution date (temporal read).
- Duplicate rule (per contribution year)
  - Regular contribution is unique per employee per contribution year.
  - Supplemental contribution bypasses the duplicate rule for that year.
- Year mismatch rule
  - If `ProfitYear != ContributionDate.Year`, the contribution must be Supplemental (no YOS credit).

## Temporal Correctness (As-Of Reads)
- Validator checks employment status using `IsActiveAsOfAsync(badge, asOfDate)` which uses a temporal snapshot of demographics as of the contribution date.
- Frozen service provides both profit-year and explicit `asOf` snapshot builders; logic is de-duplicated.

## API/Validator Mapping (Key Checks)
- `BadgeNumber`: `> 0`, `BadgeExistsAsync`.
- `ContributionAmount`: `> 0`.
- `ContributionDate`:
  - `<= Today`.
  - `>= EarliestHireYear` via `GetEarliestHireDateAsync`.
  - `Age >= 21` via `GetDateOfBirthAsync` + age calculation.
  - `Employment Active as-of` via `IsActiveAsOfAsync` (null or false → reject).
  - `Duplicate regular by contribution year` via `GetMilitaryServiceRecordAsync` against `ContributionDate.Year`.
- `IsSupplementalContribution`:
  - If `ProfitYear != ContributionDate.Year` then must be `true`.

## UI Behavior (Frontend)
- Surface server validation messages directly; keep client-side guardrails: positive amounts, date not in future, enforce required fields.
- When `ProfitYear != ContributionDate.Year`, pre-toggle `IsSupplementalContribution` and explain why (no YOS credit when years differ).
- If employment isn’t Active as-of the date, show a clear message and disable submit until date/badge changes.

## QA Test Plan (Key Scenarios)
- Valid request passes (Active, >=21, not before hire year, non-duplicate).
- Amount `<= 0` rejected.
- ProfitYear out of bounds rejected.
- Badge `<= 0` or not found rejected.
- Contribution date in future rejected.
- Under 21 rejected.
- Before earliest hire year rejected.
- Employment not Active as-of date rejected.
- Employment unknown/missing rejected.
- Duplicate regular by contribution year rejected (supplemental bypass passes).
- ProfitYear ≠ ContributionDate.Year requires Supplemental (regular rejected, supplemental passes).

## Telemetry and Operations (DevOps/IT Sec)
- Validator increments `ps_validation_failures_total` with tags: `validator="MilitaryContribution"`, `rule` in [HireDateMissing, BeforeHireYear, DobMissing, AgeUnder21, EmploymentStatusMissing, EmploymentStatusNotActive, DuplicateRegularContribution, ServiceError, YosRequiresSupplementalWhenYearMismatch].
- Recommend: add endpoint middleware counters for 4xx by endpoint and rule category, and response size histograms (see repo telemetry guidance). Start with metrics ON, PII counting OFF.
- Alert ideas: spike in `DuplicateRegularContribution`, or repeated `EmploymentStatusNotActive` for same endpoint category.

## Implementation Notes (Development)
- Validator class: `MilitaryContributionRequestValidator` in `Common/Validators`.
- Uses `IEmployeeLookupService` with `IsActiveAsOfAsync` to avoid magic constants and cross-project coupling.
- Temporal reads wired through `DemographicReaderService.BuildDemographicQueryAsOf` and `FrozenService.GetDemographicSnapshotAsOf`; projection code DRY.
- Duplicate rule queries records by `ContributionDate.Year`, not selected `ProfitYear`.
- YOS: `IsSupplementalContribution` drives `YearsOfServiceCredit` in `ProfitDetail`.

## References
- Jira: PS-1721 and related “Military contributions” tickets over 1000 in scope.
- Confluence: Business Rules for Military Contributions (link to be added when published).
- Code: see `Demoulas.ProfitSharing.Common.Validators.MilitaryContributionRequestValidator` and unit tests in `Demoulas.ProfitSharing.UnitTests/Common/Validators/MilitaryContributionRequestValidatorTests.cs`.
