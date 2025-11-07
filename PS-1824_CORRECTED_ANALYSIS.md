# PS-1824 Analysis (CORRECTED): Military Contributions - Allow Supplemental Even If Not Active

## Executive Summary

**Jira Issue**: PS-1824  
**Title**: Military Contributions - should allow supplemental even if not active  
**Status**: To Do  
**Priority**: Medium - B  
**Components**: YE, YE December Activities, YE Dec - Military & Rehire  
**Related Issues**: PS-1645, PS-1796, PS-1776, PS-1644 (all marked Done)

---

## Business Logic: Legacy COBOL vs Current Implementation

### Issue Identification (CORRECTED)

The issue stems from a **PAY FREQUENCY restriction**, not active status:

**Legacy COBOL (TPR008-13.cbl) Logic (Lines 735-746)**

Pay Frequency Reference:
```
PY-FREQ - ALPHANUMERIC DATA INDICATES PAY FREQUENCY CODE
  1 = WEEKLY
  2 = MONTHLY  ← RESTRICTED FROM MILITARY CONTRIBUTIONS
```

The restriction (Lines 735-746):
```cobol
IF PY-FREQ = "2"  [MONTHLY frequency employees]
  IF (TDS-USER-IDENT NOT = "DEMH0062")
    AND (TDS-USER-IDENT NOT = "DEMH9999")
    AND (TDS-USER-IDENT NOT = "DEMH0013")
      MOVE "EMPLOYEE NOT ON FILE" TO XP-ERROR-MESG
      MOVE 1 TO WS-ERROR-SW
      GO TO 300-EXIT.
```

**Business Logic Interpretation**:
1. **Monthly-frequency employees** (PY-FREQ = "2") are **BLOCKED** from military contributions
2. **Exception**: Only 3 authorized users can bypass this:
   - DEMH0062 (original permission holder)
   - DEMH9999 (test/ops account)
   - DEMH0013 (John Jurczak - added in MAIN-668)
3. **Weekly-frequency employees** (PY-FREQ = "1") have NO restriction

**Current .NET Implementation Logic:**
- `MilitaryContributionRequestValidator.cs` (lines 37-120)
- `CreateMilitaryContributionRecord.cs` endpoint
- **NO CHECK for pay frequency at all**
- **NO USER-LEVEL EXCEPTIONS**
- Validation focuses on:
  - Badge/employee exists in system
  - Employee was at least 21 on contribution date
  - Contribution date is not before hire date
  - Supplemental flag rules (YOS, duplicate prevention)

**Key Missing Checks**:
1. ❌ Pay frequency validation (PY-FREQ check)
2. ❌ User authorization exceptions (3 special users)
3. ❌ Supplemental contributions should bypass pay frequency restriction (PS-1824 intent)

---

## Business Rule Requirements

### From Legacy COBOL (TPR008-13.cbl)

1. **Pay Frequency Restriction** (Lines 735-746)
   - Monthly-frequency employees (PY-FREQ = "2") are restricted
   - Weekly-frequency employees (PY-FREQ = "1") are allowed
   - Exception list: DEMH0062, DEMH9999, DEMH0013 can override

2. **User Authorization Bypass**
   ```cobol
   IF PY-FREQ = "2"
     IF user NOT IN (DEMH0062, DEMH9999, DEMH0013)
       REJECT with "EMPLOYEE NOT ON FILE"
   ```
   - Only specific system users can add military contributions for monthly employees
   - No role-based override, only user ID hardcoded list

3. **Contribution Types** (Lines 1528-1547, 460-PROCESS-MONTH-1-12-CHECK)
   - Regular contributions (profit-year dot zero): One per year, enforces YOS
   - Supplemental contributions (profit-year dot one): Multiple allowed, no YOS credit

### PS-1824 Business Intent

The issue title **"allow supplemental even if not active"** is misleading given the actual COBOL logic. The real issue appears to be:

**Two Possible Interpretations**:

**Interpretation A**: "Not Active" = Monthly Frequency
- Supplemental contributions should bypass pay frequency restrictions
- Users (even non-authorized ones) should be able to submit supplemental for monthly employees
- Regular contributions remain restricted

**Interpretation B**: "Not Active" = Actually Inactive/Terminated
- Related to whether employee is active in the system on the contribution date
- Supplemental contributions should be allowed for inactive employees
- Regular contributions may still require active status

**Evidence for Interpretation A**:
- Title says "allow supplemental even if **not active**" (vague term)
- Related issues PS-1796 mention "V(ested Only)" employee - likely terminated
- COBOL code shows pay frequency check, not active status check
- Supplemental contributions are already designed for edge cases

**Evidence for Interpretation B**:
- "Not active" more naturally refers to employment status
- Separated employees might still have vesting/profit sharing
- Supplemental contributions are for post-separation scenarios

---

## Gap Analysis: What's Missing

| Rule | COBOL | Current .NET | Status |
|------|-------|-------------|--------|
| Employee exists | ✅ Required | ✅ Required | **GOOD** |
| Pay frequency check (monthly = restricted) | ✅ Explicit | ❌ **NOT CHECKED** | **GAP** |
| Special user exceptions | ✅ Hardcoded 3 users | ❌ **NOT IMPLEMENTED** | **GAP** |
| Allow supplemental to bypass restrictions | ✅ Implicit (no check for supplemental) | ❌ **NO BYPASS LOGIC** | **GAP** |
| Contribution not before hire | ✅ Implicit | ✅ Explicit check | **GOOD** |
| At least 21 on contribution date | ✅ Implicit age calc | ✅ Explicit check | **GOOD** |
| Duplicate prevention (regular only) | ✅ Yes, supplemental bypass | ✅ Yes, supplemental bypass | **GOOD** |
| YOS credit (regular only) | ✅ Yes, supplemental=0 | ✅ Yes, IsSupplementalContribution flag | **GOOD** |

---

## Suggested Changes

### Option 1: Implement Full Pay Frequency + User Authorization (Match Legacy)

**If PS-1824 means: "supplemental should bypass pay frequency restriction"**

#### 1.1 Add Pay Frequency Validation Service

**File**: `src/services/src/Demoulas.ProfitSharing.Services/MilitaryService.cs`

```csharp
private async Task<bool> IsMonthlyFrequencyEmployeeAsync(int badgeNumber, CancellationToken ct)
{
    var payroll = await _payrollService.GetPayrollInfoAsync(badgeNumber, ct);
    return payroll?.PayFrequency == "2"; // "2" = MONTHLY
}

private bool IsAuthorizedUserForMonthlyEmployee(string userId)
{
    // Legacy hardcoded list from COBOL
    var authorizedUsers = new[] { "DEMH0062", "DEMH9999", "DEMH0013" };
    return authorizedUsers.Contains(userId, StringComparer.OrdinalIgnoreCase);
}
```

#### 1.2 Add Validator Rule

**File**: `src/services/src/Demoulas.ProfitSharing.Common/Validators/MilitaryContributionRequestValidator.cs`

```csharp
RuleFor(r => r.IsSupplementalContribution)
    .MustAsync(async (request, isSupp, ct) =>
    {
        // Supplemental contributions bypass pay frequency restriction
        if (isSupp)
            return true;

        // Regular contributions: check pay frequency
        var isMonthly = await _militaryService.IsMonthlyFrequencyEmployeeAsync(
            request.BadgeNumber, ct);

        if (!isMonthly)
            return true; // Weekly-frequency employees are always allowed

        // Monthly employees: only authorized users can add regular contributions
        return TrackFailure("MonthlyEmployeeRequiresSupplementalOrAuthorization");
    })
    .WithMessage(request =>
        "Monthly-frequency employees can only have supplemental military contributions added. " +
        "Mark as Supplemental to proceed.");
```

#### 1.3 Update Frontend

**File**: `src/ui/src/pages/DecemberActivities/MilitaryContribution/MilitaryContributionForm.tsx`

```typescript
} else if (error.reason.includes("Monthly-frequency employees")) {
  errorMessages.push(
    "- This employee is on a monthly pay frequency. " +
    "Regular contributions are not allowed for monthly employees. " +
    "Please check the 'Supplemental' box to add this contribution."
  );
}
```

---

### Option 2: Implement Employment Status (Active/Inactive) Check

**If PS-1824 means: "supplemental should work for inactive/separated employees"**

#### 2.1 Add Active Status Validation

**File**: `src/services/src/Demoulas.ProfitSharing.Common/Validators/MilitaryContributionRequestValidator.cs`

```csharp
RuleFor(r => r.IsSupplementalContribution)
    .MustAsync(async (request, isSupp, ct) =>
    {
        // Supplemental contributions bypass active status check
        if (isSupp)
            return true;

        // Regular contributions require active status on contribution date
        var isActive = await _employeeLookup.IsActiveAsOfAsync(
            request.BadgeNumber,
            DateOnly.FromDateTime(request.ContributionDate),
            ct);

        return isActive || TrackFailure("RegularContributionRequiresActiveStatus");
    })
    .WithMessage(request => 
        $"Regular contributions require the employee to be active on {request.ContributionDate:yyyy-MM-dd}. " +
        $"Mark as Supplemental to allow contributions for inactive/separated employees.");
```

#### 2.2 Update Frontend

**File**: `src/ui/src/pages/DecemberActivities/MilitaryContribution/MilitaryContributionForm.tsx`

```typescript
} else if (error.reason.includes("active on")) {
  errorMessages.push(
    "- Employee is no longer active as of the contribution date. " +
    "Please check the 'Supplemental' box to add this contribution for a separated employee."
  );
}
```

---

### Option 3: Hybrid Approach (Recommend)

**Implement BOTH checks with clear precedence**:

1. Check pay frequency FIRST
2. Check active status SECOND
3. Supplemental contributions bypass BOTH

This matches the COBOL most closely while also catching the "not active" scenario.

---

## Unit Tests

### For Option 1 (Pay Frequency)

```csharp
[Fact]
[Description("PS-1824 : Supplemental bypasses monthly frequency restriction")]
public async Task Supplemental_bypasses_monthly_frequency_restriction()
{
    var (validator, employeeLookupMock, militaryServiceMock) = CreateValidator();

    employeeLookupMock.Reset();
    employeeLookupMock
        .Setup(x => x.BadgeExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(true);
    employeeLookupMock
        .Setup(x => x.GetEarliestHireDateAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(new DateOnly(2020, 1, 1));
    employeeLookupMock
        .Setup(x => x.GetDateOfBirthAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(new DateOnly(1980, 1, 1));

    militaryServiceMock.Reset();
    militaryServiceMock
        .Setup(m => m.IsMonthlyFrequencyEmployeeAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(true);  // Employee is MONTHLY frequency
    militaryServiceMock
        .Setup(m => m.GetMilitaryServiceRecordAsync(It.IsAny<GetMilitaryContributionRequest>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(Result<PaginatedResponseDto<MilitaryContributionResponse>>.Success(
            new PaginatedResponseDto<MilitaryContributionResponse> { Results = new List<MilitaryContributionResponse>() }));

    var request = new CreateMilitaryContributionRequest
    {
        BadgeNumber = 1234567,
        ContributionDate = new DateTime(2024, 6, 15),
        ContributionAmount = 1000.00m,
        ProfitYear = 2024,
        IsSupplementalContribution = true  // Supplemental = OK
    };

    var result = await validator.ValidateAsync(request);
    
    result.IsValid.Should().BeTrue();  // Should pass for supplemental
}

[Fact]
[Description("PS-1824 : Regular rejects monthly frequency unless authorized")]
public async Task Regular_rejects_monthly_frequency()
{
    var (validator, employeeLookupMock, militaryServiceMock) = CreateValidator();

    employeeLookupMock.Reset();
    employeeLookupMock
        .Setup(x => x.BadgeExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(true);
    employeeLookupMock
        .Setup(x => x.GetEarliestHireDateAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(new DateOnly(2020, 1, 1));
    employeeLookupMock
        .Setup(x => x.GetDateOfBirthAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(new DateOnly(1980, 1, 1));

    militaryServiceMock.Reset();
    militaryServiceMock
        .Setup(m => m.IsMonthlyFrequencyEmployeeAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(true);  // Employee is MONTHLY frequency
    militaryServiceMock
        .Setup(m => m.GetMilitaryServiceRecordAsync(It.IsAny<GetMilitaryContributionRequest>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(Result<PaginatedResponseDto<MilitaryContributionResponse>>.Success(
            new PaginatedResponseDto<MilitaryContributionResponse> { Results = new List<MilitaryContributionResponse>() }));

    var request = new CreateMilitaryContributionRequest
    {
        BadgeNumber = 1234567,
        ContributionDate = new DateTime(2024, 6, 15),
        ContributionAmount = 1000.00m,
        ProfitYear = 2024,
        IsSupplementalContribution = false  // Regular = NOT OK for monthly
    };

    var result = await validator.ValidateAsync(request);
    
    result.IsValid.Should().BeFalse();
    result.Errors.Should().Contain(e => e.PropertyName == "IsSupplementalContribution" && 
        e.ErrorMessage.Contains("monthly") || e.ErrorMessage.Contains("supplemental"));
}
```

---

## Related Issues - What They Tell Us

- **PS-1645**: "Military Contribution | able to add military contribution to the employee for the year 2018 even though employee was hired in 2020"
  - ✅ Hire date validation (already implemented)

- **PS-1796**: "Military Contribution | Receiving unexpected error while submitting supplemental contribution to the V(ested Only) employee"
  - 🔴 **V(ested Only)** suggests **terminated/inactive employee**
  - User tried to use supplemental flag, still got error
  - **Suggests**: Inactive status is the actual blocker, not just frequency

- **PS-1776**: "Military Contribution | Unable to submit the Military contribution the employee even as supplemental"
  - 🔴 User explicitly marked as supplemental, still rejected
  - **Strong evidence**: Current system blocks supplemental entirely

- **PS-1644**: "Military Contribution | Not seeing successful message after adding military contribution"
  - ✅ UI feedback issue (not validation logic)

**Conclusion**: PS-1796 and PS-1776 suggest the issue is about **inactive employees**, and supplemental contributions should bypass whatever restriction exists.

---

## Recommendation

### Most Likely Scenario: Employment Status Check

Based on related issues PS-1796 and PS-1776, the issue is likely about **employment status**, not pay frequency.

**Recommended Implementation**:

1. **Add employment status check** for regular contributions only
2. **Allow supplemental** for any status
3. **Clear messaging** to guide users to use supplemental for inactive employees

**Why**:
- Related issues mention "V(ested Only)" and "supplemental" together
- User explicitly tried supplemental flag and still got rejected
- "Not active" more naturally means inactive/separated
- Supplemental contributions are designed for edge cases like separated employees

---

## Acceptance Criteria

### Questions to Resolve BEFORE Implementation

1. **What does "not active" mean?**
   - [ ] Pay frequency = monthly?
   - [ ] Employment status = inactive/terminated?
   - [ ] Both checks needed?

2. **Who can override restrictions?**
   - [ ] No override needed (let supplemental bypass)?
   - [ ] Specific user IDs (DEMH0062, DEMH9999, DEMH0013)?
   - [ ] Specific roles (administrator)?

3. **Should regular contributions work for inactive?**
   - [ ] No (only supplemental for inactive)
   - [ ] Yes (no restrictions)

4. **Should regular contributions work for monthly frequency?**
   - [ ] No (only supplemental for monthly)
   - [ ] Yes (no restrictions)

---

## Reference Files

### Core Implementation Files
- `src/services/src/Demoulas.ProfitSharing.Common/Validators/MilitaryContributionRequestValidator.cs`
- `src/services/src/Demoulas.ProfitSharing.Endpoints/Endpoints/Military/CreateMilitaryContributionRecord.cs`
- `src/services/tests/Demoulas.ProfitSharing.UnitTests/Common/Validators/MilitaryContributionRequestValidatorTests.cs`
- `src/ui/src/pages/DecemberActivities/MilitaryContribution/MilitaryContributionForm.tsx`

### Documentation Files
- `src/ui/public/docs/MILITARY_CONTRIBUTION_VALIDATION.md`
- `src/ui/public/docs/MILITARY_CONTRIBUTION_QA_GUIDE.md`

### Legacy Reference
- `TPR008-13.cbl` - Lines 735-746 (pay frequency check), 1528-1547 (duplicate prevention)

