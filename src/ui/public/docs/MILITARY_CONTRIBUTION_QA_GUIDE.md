# Military Contribution Business Logic - QA Testing Guide

## Overview

This document explains the business logic and validation rules for Military Contributions in the Profit Sharing system. It's designed to help QA teams understand what to test and what expected behaviors should be observed.

## Table of Contents

- [Business Context](#business-context)
- [Key Concepts](#key-concepts)
- [Validation Rules](#validation-rules)
- [Test Scenarios](#test-scenarios)
- [Error Messages Reference](#error-messages-reference)
- [Edge Cases & Special Situations](#edge-cases--special-situations)
- [Testing Checklist](#testing-checklist)

## Business Context

Military Contributions are special contributions made to the profit-sharing plan for employees who served in the military. These contributions can be either:
- **Regular Contributions**: Count toward Years of Service (YOS) credit
- **Supplemental Contributions**: Do NOT count toward Years of Service credit

The system enforces strict business rules to ensure data integrity and compliance with profit-sharing regulations.

## Key Concepts

### Years of Service (YOS) Credit
- **Regular Contributions**: Award 1 year of service credit (`YearsOfServiceCredit = 1`)
- **Supplemental Contributions**: Award 0 years of service credit (`YearsOfServiceCredit = 0`)
- YOS credit affects vesting percentages and profit-sharing calculations

### Contribution vs. Profit Year
- **Contribution Date**: The actual date the military service occurred
- **Profit Year**: The profit-sharing year the contribution is posted to
- These can be different, but it affects whether the contribution must be supplemental

## Validation Rules

### 1. Contribution Amount Validation
**Rule**: Contribution amount must be greater than zero.

**QA Test**: 
- ✅ Valid: `$1.00`, `$1000.50`, `$99999.99`
- ❌ Invalid: `$0.00`, `-$100.00`, `null`

**Error Message**: "The ContributionAmount must be greater than zero."

---

### 2. Profit Year Validation
**Rule**: Profit year must be between 2020 and the current year (inclusive).

**QA Test**:
- ✅ Valid: `2020`, `2024`, `2025` (current year)
- ❌ Invalid: `2019`, `2026` (future year), `1999`

**Error Messages**: 
- "ProfitYear must not less than 2020."
- "ProfitYear must not be greater than this year."

---

### 3. Badge Number Validation
**Rule**: Badge number must be positive and correspond to an existing employee.

**QA Test**:
- ✅ Valid: Existing employee badge numbers > 0 
- ❌ Invalid: `0`, `-1`, `999999` (non-existent)

**Error Messages**:
- "BadgeNumber must be greater than zero."
- "Employee with the given badge number was not found."

---

### 4. Contribution Date Validation
**Rule**: Contribution date cannot be in the future.

**QA Test**:
- ✅ Valid: Today's date, yesterday, any past date
- ❌ Invalid: Tomorrow, any future date

**Error Message**: "ContributionDate cannot be in the future."

---

### 5. Employee Age Validation
**Rule**: Employee must be at least 21 years old on the contribution date.

**QA Test**:
- ✅ Valid: Employee turns 21 on or before contribution date
- ❌ Invalid: Employee is 20 on contribution date

**Error Message**: "Employee must be at least 21 years old on the contribution date YYYY-MM-DD."

---

### 6. Hire Date Validation
**Rule**: Contribution year must be >= employee's earliest hire year.

**QA Test**:
- ✅ Valid: Contribution in 2024, hired in 2023
- ❌ Invalid: Contribution in 2022, hired in 2023

**Error Message**: "Contribution year YYYY is before the employee's earliest known hire year."

---

### 8. Duplicate Contribution Validation
**Rule**: Only one regular (non-supplemental) contribution allowed per year.

**QA Test Scenarios**:
- ✅ Valid: First regular contribution for the year
- ✅ Valid: Multiple supplemental contributions for the year
- ✅ Valid: Regular + supplemental contributions for the year
- ❌ Invalid: Second regular contribution for the same year

**Error Message**: "Regular Contribution already recorded for year YYYY. Duplicates are not allowed."

---

### 9. Cross-Year Posting Rule (CRITICAL)
**Rule**: When profit year differs from contribution date year, contribution MUST be supplemental.

**QA Test Scenarios**:
- ✅ Valid: Contribution date 2024, Profit year 2024, either regular or supplemental
- ✅ Valid: Contribution date 2023, Profit year 2024, marked as supplemental
- ❌ Invalid: Contribution date 2023, Profit year 2024, marked as regular

**Error Message**: "When profit year (YYYY) differs from contribution year (YYYY), the contribution must be marked Supplemental."

**⚠️ QA Note**: This prevents YOS credit for contributions posted to different years than when the service occurred.

## Test Scenarios

### Scenario 1: Valid Regular Contribution
```
Badge Number: 12345 (Active employee)
Contribution Date: 2024-01-15
Contribution Amount: $1000.00
Profit Year: 2024
Is Supplemental: false
Expected: ✅ SUCCESS - Creates regular contribution with YOS credit
```

### Scenario 2: Valid Supplemental Contribution
```
Badge Number: 12345 (Active employee)
Contribution Date: 2024-01-15
Contribution Amount: $500.00
Profit Year: 2024
Is Supplemental: true
Expected: ✅ SUCCESS - Creates supplemental contribution without YOS credit
```

### Scenario 3: Cross-Year Posting (Must be Supplemental)
```
Badge Number: 12345
Contribution Date: 2023-06-15
Contribution Amount: $750.00
Profit Year: 2024
Is Supplemental: false
Expected: ❌ FAIL - Must be marked supplemental for cross-year posting
```

### Scenario 4: Duplicate Regular Contribution
```
Step 1: Create regular contribution for badge 12345, year 2024 ✅
Step 2: Try to create another regular contribution for badge 12345, year 2024
Expected: ❌ FAIL - Only one regular contribution per year allowed
```

### Scenario 5: Under-Age Employee
```
Badge Number: 11111 (Employee born 2004-01-01)
Contribution Date: 2024-01-01 (employee is 20)
Contribution Amount: $500.00
Profit Year: 2024
Is Supplemental: false
Expected: ❌ FAIL - Employee not yet 21
```

### Scenario 6: Future Date
```
Badge Number: 12345
Contribution Date: 2026-01-01 (future date)
Contribution Amount: $1000.00
Profit Year: 2025
Is Supplemental: false
Expected: ❌ FAIL - Cannot contribute for future dates
```

### Scenario 8: Pre-Hire Contribution
```
Badge Number: 22222 (Employee hired 2024-01-01)
Contribution Date: 2023-06-15 (before hire)
Contribution Amount: $1000.00
Profit Year: 2024
Is Supplemental: false
Expected: ❌ FAIL - Cannot contribute for service before hire
```

## Error Messages Reference

| Validation Rule | Error Message |
|-----------------|---------------|
| Zero/Negative Amount | "The ContributionAmount must be greater than zero." |
| Profit Year Too Early | "ProfitYear must not less than 2020." |
| Profit Year Too Late | "ProfitYear must not be greater than this year." |
| Invalid Badge | "BadgeNumber must be greater than zero." |
| Non-existent Employee | "Employee with the given badge number was not found." |
| Future Date | "ContributionDate cannot be in the future." |
| Under Age | "Employee must be at least 21 years old on the contribution date YYYY-MM-DD." |
| Pre-Hire Service | "Contribution year YYYY is before the employee's earliest known hire year." |
| Duplicate Regular | "Regular Contribution already recorded for year YYYY. Duplicates are not allowed." |
| Cross-Year Must be Supplemental | "When profit year (YYYY) differs from contribution year (YYYY), the contribution must be marked Supplemental." |

## Edge Cases & Special Situations

### Edge Case 1: Leap Year Dates
**Situation**: Contribution date of February 29th in leap years.
**Expected Behavior**: Should be handled correctly by date validation.
**Test**: Use 2024-02-29 as contribution date.

### Edge Case 3: Year-End Contributions
**Situation**: Contribution dated December 31st.
**Expected Behavior**: Should be processed normally.
**Test**: Use December 31st as contribution date.

### Edge Case 4: Multiple Supplemental Contributions
**Situation**: Several supplemental contributions for the same employee and year.
**Expected Behavior**: Should all be allowed (no limit on supplement
**Test**: Create 3+ supplemental contributions for same employee/year.

### Edge Case 5: Minimum Age on Birthday
**Situation**: Employee turns 21 on the contribution date.
**Expected Behavior**: Should be allowed (age >= 21).
**Test**: Contribution date = 21st birthday.

## Testing Checklist

### Pre-Test Setup
- [ ] Verify test employees exist in the system
- [ ] Confirm test employees' birth dates and hire dates
- [ ] Clear any existing military contributions for test employees
- [ ] Verify current system date for year validation tests

### Basic Validation Tests
- [ ] Test all numeric field boundaries (amount, year, badge)
- [ ] Test all date validations (future, past, leap year)
- [ ] Test age validation (under 21, exactly 21, over 21)
- [ ] Test hire date validation (before hire, after hire)

### Business Logic Tests
- [ ] Test regular vs supplemental contribution creation
- [ ] Test duplicate regular contribution prevention
- [ ] Test cross-year posting supplemental requirement
- [ ] Test YOS credit assignment (1 for regular, 0 for supplemental)

### Error Handling Tests
- [ ] Verify all error messages match expected text
- [ ] Test error handling for missing employee data
- [ ] Test system behavior with invalid/malformed requests
- [ ] Verify multiple validation errors are reported correctly

### Integration Tests
- [ ] Test UI form validation matches backend validation
- [ ] Test API endpoint responses for all scenarios
- [ ] Verify database records created correctly
- [ ] Test reporting/inquiry functionality shows contributions

### Performance Tests
- [ ] Test validation performance with large employee datasets
- [ ] Test concurrent contribution creation
- [ ] Verify validation doesn't cause system slowdowns

## QA Automation Recommendations

### High-Priority Automated Tests
1. **Cross-Year Posting Rule**: Complex logic, easy to break
2. **Duplicate Prevention**: Data integrity protection
3. **Age & Hire Date Validation**: Critical business rules
4. **Basic Field Validation**: High-volume, low-risk tests

### Manual Testing Focus
1. **Edge Cases**: Complex scenarios requiring human verification
2. **UI Integration**: Form behavior and user experience
3. **Error Message Accuracy**: Ensure messages are user-friendly
4. **Business Rule Changes**: When requirements evolve

## Troubleshooting Common Issues

### Issue: "Must be marked Supplemental"
**Cause**: Profit year ≠ contribution date year, but marked as regular
**Solution**: Either mark as supplemental or adjust profit year

### Issue: "Regular contribution already recorded"
**Cause**: Attempting second regular contribution for same year
**Solution**: Mark as supplemental or verify existing contribution

### Issue: "Employee not found"
**Cause**: Invalid badge number or employee not in system
**Solution**: Verify badge number, check employee data import

---

## Related Documentation

- [Military Contribution API Documentation](./API_MILITARY_CONTRIBUTIONS.md)
- [Employee Lookup Service Documentation](./EMPLOYEE_LOOKUP_SERVICE.md)
- [Profit Sharing Business Rules](./PROFIT_SHARING_BUSINESS_RULES.md)

## Contact Information

For questions about this documentation or military contribution business rules:
- **Development Team**: Smart Profit Sharing Dev Team
- **Business Analyst**: [Contact Information]
- **QA Lead**: [Contact Information]

---

*Last Updated: September 30, 2025*
*Document Version: 1.0*