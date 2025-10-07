# Military Contribution Validation Rules

## Overview

This document details the complete business rules and validation logic for Military Contributions in the SMART Profit Sharing system, migrated from the legacy READY system (TPR008-13.cbl).

**Last Updated**: October 6, 2025  
**COBOL Source**: TPR008-13.cbl (MAIN-1286/MAIN-1280 Oracle HCM Integration)  
**C# Validator**: `MilitaryContributionRequestValidator.cs`

## Business Rules Summary

### 1. Five-Year Lookback Window (Proj 19567)

**Rule**: Military contributions can be entered for the current year OR up to 5 years prior.

**COBOL Source**: Lines 1256-1265 (410-CHECK-EACH section)
```cobol
* EXPANDED TO ALLOW 5 YEARS BACK
  OR (WS-XU-CONT-YR < WS-YR - 5))
    MOVE "INVALID YEAR - ENTER CURRENT YEAR - 1 OR 5 YEARS PRIOR"
```

**History**: 
- **12/31/2013**: Expanded from 3 years to 5 years (Proj 19567, D. Prugh)
- **Rationale**: Allows backdating of military service contributions discovered later

**C# Implementation**:
```csharp
.Must((request, profitYear) =>
{
    var currentYear = DateTime.Today.Year;
    var contributionYear = request.ContributionDate.Year;
    var isValid = contributionYear == currentYear || 
                 (contributionYear >= currentYear - 5 && contributionYear < currentYear);
    return isValid || TrackFailure("ContributionYearOutsideLookbackWindow");
})
.WithMessage(request => $"Contribution year {request.ContributionDate.Year} must be current year or within 5 years prior (allowed: {DateTime.Today.Year - 5} to {DateTime.Today.Year}).");
```

**Valid Examples** (assuming current year is 2025):
- ✅ 2025 (current year)
- ✅ 2024, 2023, 2022, 2021, 2020 (5 years back)
- ❌ 2019 (6 years back - too old)
- ❌ 2026 (future year)

### 2. Age Requirement (21+ on Contribution Date)

**Rule**: Employee must be at least 21 years old on the contribution date.

**Rationale**: Federal retirement plan eligibility rules require participants to be 21+.

**COBOL Source**: Lines 273-279 (WS-AGE, WS-BDATE-TO-CONVERT fields)
```cobol
01  WS-AGE                PIC 999      VALUE ZERO.
```

**C# Implementation**:
```csharp
.MustAsync((request, _, ct) => ValidateAtLeast21OnContribution(request, ct))
.WithMessage(request => $"Employee must be at least 21 years old on the contribution date {request.ContributionDate:yyyy-MM-dd}.");

private async Task<bool> ValidateAtLeast21OnContribution(CreateMilitaryContributionRequest req, CancellationToken token)
{
    var dob = await _employeeLookup.GetDateOfBirthAsync(req.BadgeNumber, token);
    if (!dob.HasValue)
    {
        return TrackFailure("DobMissing");
    }
    return dob.Value.Age(req.ContributionDate) >= 21 || TrackFailure("AgeUnder21");
}
```

**Test Scenarios**:
- ✅ Employee born 1995-01-15, contribution date 2024-06-01 (29 years old)
- ✅ Employee born 2003-12-31, contribution date 2025-01-01 (21 years, 1 day old)
- ❌ Employee born 2004-06-15, contribution date 2025-01-01 (20 years old)

### 3. Hire Date Validation

**Rule**: Contribution year must be on or after the employee's earliest known hire year.

**Rationale**: Cannot contribute for service before employee was hired.

**COBOL Source**: Lines 310-1035 (310-START-SSN, 320-START-BADGE - employee lookup)
```cobol
MOVE XU-SSN TO DEM-SSN.
CALL "START-ALT-EQUAL-DEMOGRAPHICS"
```

**C# Implementation**:
```csharp
.MustAsync((request, _, ct) => ValidateNotBeforeHire(request, ct))
.WithMessage(request => $"Contribution year {request.ContributionDate.Year} is before the employee's earliest known hire year.");

private async Task<bool> ValidateNotBeforeHire(CreateMilitaryContributionRequest req, CancellationToken token)
{
    var earliest = await _employeeLookup.GetEarliestHireDateAsync(req.BadgeNumber, token);
    if (!earliest.HasValue)
    {
        return TrackFailure("HireDateMissing");
    }
    return req.ContributionDate.Year >= earliest.Value.Year || TrackFailure("BeforeHireYear");
}
```

**Test Scenarios**:
- ✅ Hire date 2015-03-15, contribution date 2020-06-01 (5 years after hire)
- ✅ Hire date 2024-01-01, contribution date 2024-12-31 (same year as hire)
- ❌ Hire date 2020-06-15, contribution date 2019-01-01 (before hire year)

### 4. Payroll Frequency Restriction (Monthly Employees)

**Rule**: Monthly employees (PY-FREQ=2) can only be processed by authorized users. Weekly employees (PY-FREQ=1) have no restriction.

**Rationale**: Monthly-paid employees require special handling and elevated permissions due to different payroll processing requirements.

**Payroll Frequency Values**:
- `PY-FREQ = 1`: Weekly (paid every week) - no restrictions
- `PY-FREQ = 2`: Monthly (paid once a month) - restricted access

**COBOL Source**: Lines 735-743 (PY-FREQ check for monthly associates)
```cobol
IF PY-FREQ = "2"
  IF (TDS-USER-IDENT NOT = "DEMH0062")
    AND (TDS-USER-IDENT NOT = "DEMH9999")
    AND (TDS-USER-IDENT NOT = "DEMH0013")
      MOVE "EMPLOYEE NOT ON FILE" TO XP-ERROR-MESG
```

**Authorized Users for Monthly Employees** (as of MAIN-668):
- DEMH0062
- DEMH9999
- DEMH0013 (John Jurczak - added 03/13/19)

**C# Implementation Note**: The modern C# implementation **does NOT validate employment status** (this check was not in the original COBOL). The COBOL only restricted monthly-paid employees to authorized users, which has been simplified to role-based access control at the application level. There is no Active/Terminated/Leave employment status validation in either COBOL or the current C# implementation.

**COBOL Payroll Frequency Check** (removed in C#):
- If employee is monthly-paid (PY-FREQ = 2), only 3 authorized users could process
- If employee is weekly-paid (PY-FREQ = 1), no restriction
- This has been replaced with application-level role-based access control

### 5. Duplicate Prevention (Regular Contributions Only)

**Rule**: No duplicate regular contributions allowed for the same year. Supplemental contributions bypass this check.

**COBOL Source**: Lines 1530-1547 (460-PROCESS-MONTH-1-12-CHECK)
```cobol
IF WS-PROFIT-YEAR-2 = 1
  AND WS-PROFIT-YEAR-1B = WS-XU-CONT-YR
    MOVE "INVALID - MILITARY ENTRY ALREADY EXISTS FOR THAT YEAR" 
      TO XP-ERROR-MESG
```

**Key Distinction**: 
- **Regular Contributions**: One per profit year (provides YOS credit)
- **Supplemental Contributions**: Multiple allowed per year (no YOS credit)

**C# Implementation**:
```csharp
private async Task<bool> ValidateContributionDate(CreateMilitaryContributionRequest req, CancellationToken token)
{
    if (req.IsSupplementalContribution)
    {
        return true; // Supplemental contributions bypass duplicate check
    }

    // Query by the contribution date year rather than the selected ProfitYear
    var results = await _militaryService.GetMilitaryServiceRecordAsync(
        new GetMilitaryContributionRequest { BadgeNumber = req.BadgeNumber, Take = short.MaxValue },
        isArchiveRequest: false,
        cancellationToken: token);

    if (!results.IsSuccess)
    {
        return TrackFailure("ServiceError");
    }
    
    var records = results.Value!.Results.Where(x => x.ProfitYear == (short)req.ContributionDate.Year);
    var ok = records.All(x => x.IsSupplementalContribution || x.ContributionDate.Year != req.ContributionDate.Year);
    return ok || TrackFailure("DuplicateRegularContribution");
}
```

**Test Scenarios**:
- ✅ No existing contributions for 2024, add regular contribution for 2024
- ✅ Existing supplemental contribution for 2024, add another supplemental for 2024
- ✅ Existing regular contribution for 2024, add supplemental contribution for 2024
- ❌ Existing regular contribution for 2024, add another regular for 2024

### 6. Year/Date Consistency (Supplemental Flag)

**Rule**: When profit year differs from contribution date year, the contribution MUST be marked supplemental.

**Rationale**: YOS (Years of Service) credit should only be awarded for contributions posted in the same year as the service. Cross-year postings must be supplemental.

**C# Implementation**:
```csharp
RuleFor(r => r.IsSupplementalContribution)
    .Must((request, isSupp) =>
    {
        if (request.ProfitYear == (short)request.ContributionDate.Year)
        {
            return true; // same year, either regular or supplemental allowed
        }
        // Different posting year than contribution date year must be supplemental
        return isSupp || TrackFailure("YosRequiresSupplementalWhenYearMismatch");
    })
    .WithMessage(r => $"When profit year ({r.ProfitYear}) differs from contribution year ({r.ContributionDate.Year}), the contribution must be marked Supplemental.");
```

**Test Scenarios**:
- ✅ Profit year 2024, contribution date 2024-06-01, regular or supplemental
- ✅ Profit year 2025, contribution date 2024-06-01, marked supplemental
- ❌ Profit year 2025, contribution date 2024-06-01, marked regular (YOS mismatch)

### 7. Amount Validation

**Rule**: Contribution amount must be greater than zero.

**COBOL Source**: Lines 1223-1230 (410-CHECK-EACH)
```cobol
IF XU-CL-AMOUNT(IDX) < 0
  AND TSA-DIST-AMT(IDX) = 0
    MOVE "INVALID CONTRIBUTION AMOUNT" TO XP-ERROR-MESG
```

**C# Implementation**:
```csharp
RuleFor(r => r.ContributionAmount)
    .GreaterThan(0)
    .WithMessage($"The {nameof(CreateMilitaryContributionRequest.ContributionAmount)} must be greater than zero.");
```

**Test Scenarios**:
- ✅ Amount = 1000.00
- ✅ Amount = 0.01 (minimum positive)
- ❌ Amount = 0.00
- ❌ Amount = -500.00

### 8. Date Format Validation

**Rule**: Month must be 1-12 or 20 (special code for year-only entry).

**COBOL Source**: Lines 1240-1253 (410-CHECK-EACH)
```cobol
IF ((WS-XU-CONT-MO = 20)
  OR (WS-XU-CONT-MO > 0 AND WS-XU-CONT-MO < 13))
    NEXT SENTENCE
ELSE
  MOVE "INVALID DATE - ENTER MONTH/YEAR OR YEAR - EX '0705' OR '2005'"
```

**Note**: The C# API uses standard `DateTime` format, so month validation is handled by .NET framework. The special "20" month code from COBOL is not exposed in the modern API.

### 9. Screen Duplicate Check

**Rule**: Same contribution date cannot appear multiple times on the entry screen.

**COBOL Source**: Lines 1270-1398 (420-CHECK-SCREEN-DATE, 432-CHECK-SCREEN-YR)
```cobol
IF XU-CONT-DATE(1) = XU-CONT-DATE(2)
  OR XU-CONT-DATE(1) = XU-CONT-DATE(3)
    MOVE "DATE ALREADY EXISTS ON THIS SCREEN" TO XP-ERROR-MESG
```

**Note**: This validation was specific to the COBOL screen-based entry form where 5 contributions could be entered at once. The modern API processes one contribution at a time, so this validation is not applicable.

### 10. Original Entry Requirement

**Rule**: For month-specific entries (MMYY format with months 1-12), the year.0 (original) record must exist first. The year.0 record uses month code "20".

**COBOL Source**: Lines 1460-1489 (450-PROCESS-MONTH-20-CHECK)
```cobol
* THIS CHECK IS FOR WHEN A USER ENTERS A 20 FOR THE MONTH AND 03,
* 04 OR 05 ETC FOR A PRIOR YEAR. EX. 2002, 2003 OR 2004
* THE PROFIT-YEAR DOT ZERO RECORD CAN NOT EXIST ON THE PROFIT
* SHARING FILE WITH A CONTRIBUTION(PROF-CONT) GREATER THAN ZEROS.
```

**Note**: The modern API uses a boolean `IsSupplementalContribution` flag instead of the COBOL "month 20 vs 1-12" distinction. The original entry requirement is implicitly handled by the duplicate check (Rule #5).

## COBOL-to-C# Mapping

| COBOL Field/Check | C# Property/Validation | Notes |
|-------------------|------------------------|-------|
| `WS-XU-CONT-YR` year check | `ProfitYear` + 5-year lookback | Expanded from 3 to 5 years (Proj 19567) |
| `WS-AGE >= 21` | `ValidateAtLeast21OnContribution()` | Federal eligibility rule |
| `DEM-SSN`, `DEM-BADGE` lookup | `_employeeLookup.BadgeExistsAsync()` | Oracle HCM integration |
| `PY-FREQ = "2"` check | `IsActiveAsOfAsync()` | Simplified to general Active status |
| `PROFIT-YEAR` duplicate check | `ValidateContributionDate()` | Regular vs supplemental distinction |
| `XU-CL-AMOUNT > 0` | `ContributionAmount > 0` | Positive amount required |
| `WS-XU-CONT-MO = 20` or `1-12` | `IsSupplementalContribution` flag | Boolean instead of month code |
| Multiple screen dates | N/A - single entry API | Screen-based validation obsolete |

## Testing Checklist

### Happy Path Tests
- [ ] Current year contribution, active employee, age 21+
- [ ] 1-year-old contribution (within 5-year window)
- [ ] 5-year-old contribution (edge of 5-year window)
- [ ] Supplemental contribution (duplicate year allowed)
- [ ] Cross-year posting (profit year ≠ contribution year, marked supplemental)

### Boundary Tests
- [ ] Contribution on employee's 21st birthday (exact age boundary)
- [ ] Contribution in hire year (same year as hire)
- [ ] Contribution exactly 5 years ago (edge of lookback window)
- [ ] Contribution exactly 6 years ago (should fail)
- [ ] Current year contribution on Dec 31 (end of year edge case)

### Error Cases
- [ ] Contribution before hire year
- [ ] Employee under 21 on contribution date
- [ ] Inactive employee on contribution date
- [ ] Duplicate regular contribution for same year
- [ ] Negative contribution amount
- [ ] Zero contribution amount
- [ ] Future contribution date
- [ ] Contribution 6+ years in past
- [ ] Cross-year posting marked as regular (should require supplemental)
- [ ] Invalid badge number
- [ ] Missing DOB data
- [ ] Missing hire date data
- [ ] Missing employment status data

### Integration Tests
- [ ] Multiple contributions for different years (should all pass)
- [ ] Regular contribution followed by supplemental for same year (both pass)
- [ ] Two regular contributions for same year (second should fail)
- [ ] Employee with rehire history (earliest hire date used)
- [ ] Military employee with special PY-FREQ handling

## Known Differences from COBOL

### Simplified in C#:
1. **Screen-based validation**: COBOL validated 5 simultaneous entries on screen; C# API processes one at a time
2. **Month code "20"**: COBOL used special month code; C# uses boolean `IsSupplementalContribution` flag
3. **Terminal type checks**: COBOL had IBM3270 terminal checks; C# is terminal-agnostic web API
4. **PY-FREQ specific handling**: COBOL had special logic for monthly associates; C# generalizes to Active status check

### Enhanced in C#:
1. **Async validation**: Database lookups are async for better scalability
2. **Telemetry**: Validation failures tracked with metrics (`ps_validation_failures_total`)
3. **Structured error messages**: FluentValidation provides clear field-level errors
4. **Type safety**: Strong typing prevents many runtime errors from COBOL

## Related Documentation

- **COBOL Source**: `TPR008-13.cbl` - Original implementation
- **C# Validator**: `MilitaryContributionRequestValidator.cs`
- **Confluence**: [Military Contributions Validation Behavior](https://demoulas.atlassian.net/wiki/spaces/NGDS/pages/537919492)
- **VALIDATION_PATTERNS.md**: General validation patterns guide
- **TELEMETRY_GUIDE.md**: Observability and metrics for validation

## Maintenance Notes

When updating validation rules:
1. ✅ Document COBOL source line references
2. ✅ Update this documentation file
3. ✅ Update XML comments in validator
4. ✅ Add unit tests covering new rules
5. ✅ Update Confluence documentation
6. ✅ Consider telemetry metrics for new failure modes
7. ✅ Review impact on existing data

## Contact

- **Questions**: #platform-engineering, #profit-sharing-dev
- **COBOL Source Expertise**: Legacy system team
- **Business Rules Validation**: HR, Finance teams

---

**Document Version**: 1.0  
**Last Review**: October 6, 2025  
**Next Review**: Annual or when business rules change
