# Terminations Business Guide

**For Stakeholders, QA Teams, and Business Users**

## Document Purpose

This guide explains the business logic, filtering rules, vesting calculations, and beneficiary handling for the **Terminations** page in the Smart Profit Sharing system. It is designed for stakeholders who need to understand how the system processes terminated employee data and for QA teams who need to verify correct behavior.

---

## Table of Contents

1. [Overview](#overview)
2. [Business Rules & Filtering](#business-rules--filtering)
3. [Vesting Calculations](#vesting-calculations)
4. [Beneficiary Handling](#beneficiary-handling)
5. [Transaction Processing](#transaction-processing)
6. [Report Display Logic](#report-display-logic)
7. [Data Integrity & Duplicate SSN Protection](#data-integrity--duplicate-ssn-protection)
8. [Common Scenarios & Expected Behaviors](#common-scenarios--expected-behaviors)
9. [Troubleshooting & Support](#troubleshooting--support)

---

## Overview

### What is the Terminations Page?

The **Terminations** page generates reports showing terminated employees and their beneficiaries within a specified date range. The report displays:

- **Beginning balance** from the previous profit year
- **Current year transactions** (contributions, distributions, forfeitures, beneficiary allocations)
- **Ending balance** and **vested balance**
- **Vesting percentage** based on years of service and enrollment status
- **Suggested forfeiture amounts** for the current year

### Key Objectives

1. **Track terminated employees** who have profit sharing activity
2. **Calculate vesting** based on employment tenure and plan enrollment
3. **Identify forfeitures** for employees who are not fully vested
4. **Handle beneficiaries** of deceased employees separately or combined with employee records
5. **Provide actionable data** for year-end forfeiture processing

---

## Business Rules & Filtering

### Who Appears in the Report?

The Terminations report includes:

1. **Terminated Employees** who meet ALL of the following criteria:

   - Employment status is "Terminated"
   - Termination date is within the selected date range
   - NOT a retiree receiving pension (termination code excludes "Retired Receiving Pension")
   - Has profit sharing activity (balance, contributions, or transactions)

2. **Beneficiaries** of deceased employees:
   - Employee was terminated with a "Deceased" termination code
   - Beneficiary has profit sharing balances or transactions
   - Beneficiary does NOT already appear as a terminated employee in the same date range

### Enrollment & Years in Plan Filter

The system applies eligibility filters based on enrollment type:

| Enrollment Type                          | Minimum Years Required |
| ---------------------------------------- | ---------------------- |
| **Not Enrolled**                         | 3+ years               |
| **Old Vesting Plan (Has Contributions)** | 3+ years               |
| **Old Vesting Plan (Has Forfeitures)**   | 3+ years               |
| **New Vesting Plan (Has Contributions)** | 2+ years               |
| **New Vesting Plan (Has Forfeitures)**   | 2+ years               |

**What This Means:**

- Employees terminated before completing the minimum years will NOT appear in the report
- The years count represents **years in the profit sharing plan**, not total employment years

### IsInteresting Filter

Even after meeting the enrollment criteria, records must pass the "IsInteresting" filter. A record is included if it has:

- Non-zero **beginning balance** (most common inclusion reason)
- Non-zero **distribution amount**
- Non-zero **forfeit amount**
- Non-zero **beneficiary allocation** (always included)

**What This Means:**

- Employees with zero balances and no transactions are excluded
- This prevents cluttering the report with employees who have no profit sharing activity

---

## Vesting Calculations

### What is Vesting?

**Vesting** determines what percentage of an employee's profit sharing balance they are entitled to keep upon termination. The vesting percentage increases with years of service.

### Vesting Schedules

The system uses two vesting schedules based on enrollment:

#### Old Vesting Plan

| Years in Plan | Vested % |
| ------------- | -------- |
| 0-2 years     | 0%       |
| 3 years       | 25%      |
| 4 years       | 50%      |
| 5 years       | 75%      |
| 6+ years      | 100%     |

#### New Vesting Plan

| Years in Plan | Vested % |
| ------------- | -------- |
| 0-1 years     | 0%       |
| 2 years       | 20%      |
| 3 years       | 40%      |
| 4 years       | 60%      |
| 5 years       | 80%      |
| 6+ years      | 100%     |

### Special Vesting Rules

1. **100% Vesting Override (ZeroCont = 75)**

   - Employees age 65+ with first contribution more than 5 years ago are 100% vested
   - Beneficiaries are typically assigned this code and are 100% vested

2. **Deceased Employees**

   - When an employee is marked as "Deceased," their ZeroCont is set to 75 (100% vested)
   - This ensures beneficiaries receive the full balance

3. **Beneficiary-Only Records**
   - Beneficiaries who are NOT current/terminated employees are always 100% vested
   - The system sets their vesting percentage to 100% regardless of years in plan

---

## Beneficiary Handling

### How Beneficiaries Are Processed

When an employee is deceased, the system identifies beneficiaries and includes them in the report. The system uses a **PSN Suffix** to distinguish beneficiaries:

| Scenario                           | Badge Number | PSN Suffix         | Display                   |
| ---------------------------------- | ------------ | ------------------ | ------------------------- |
| **Employee Record**                | 12345        | 0                  | 12345                     |
| **Beneficiary (separate person)**  | 12345        | -1000, -2000, etc. | 12345-1000                |
| **Beneficiary who is also active** | 12345        | 0                  | 12345 (shown as employee) |

### Duplicate Prevention

**Business Rule:** If a person appears as BOTH a terminated employee and a beneficiary:

- The **employee record** is prioritized
- The **beneficiary record** is excluded to prevent duplication

**Example:**

- Employee #12345 terminates on 6/15/2024
- Employee #67890 dies on 8/20/2024 and lists #12345 as a beneficiary
- Report will show:
  - #12345 as a terminated employee (primary record)
  - #12345 will NOT appear again as a beneficiary of #67890

### Beneficiary Allocations

**Beneficiary Allocation** transactions represent:

- **Outgoing:** Transfer of funds FROM a deceased employee TO their beneficiary (negative amount, profit code 62)
- **Incoming:** Transfer of funds TO a beneficiary FROM a deceased employee (positive amount, profit code 93)

These transactions ensure the balance flows correctly between parties.

---

## Transaction Processing

### Transaction Year Boundary

**CRITICAL RULE:** The system only processes transactions up to and including the **selected ending year**.

**Example:**

- User selects date range: 1/1/2024 to 12/31/2024
- System will process transactions for 2024 and earlier years only
- If the employee has 2025 transactions, they will be ignored

### Transaction Types

The report aggregates the following transaction types:

| Transaction Type                  | Profit Code                    | Impact on Balance |
| --------------------------------- | ------------------------------ | ----------------- |
| **Contributions**                 | 1 (Incoming Contributions)     | Increases balance |
| **Earnings**                      | Various                        | Increases balance |
| **Forfeitures (Incoming)**        | 1 (Incoming Contributions)     | Increases balance |
| **Forfeitures (Outgoing)**        | 61 (Outgoing Forfeitures)      | Decreases balance |
| **Distributions**                 | 66, 67, 68                     | Decreases balance |
| **Beneficiary Allocations (Out)** | 62 (Outgoing Xfer Beneficiary) | Decreases balance |
| **Beneficiary Allocations (In)**  | 93 (Incoming QDRO Beneficiary) | Increases balance |

### Balance Calculations

**Beginning Balance:**

- Retrieved from the **previous profit year's** ending balance
- Example: For 2024 report, beginning balance is the 2023 ending balance

**Ending Balance:**

- Beginning Balance + Total Contributions + Total Earnings + Total Forfeitures (net) + Distributions + Beneficiary Allocations (net)

**Vested Balance:**

- Ending Balance × Vesting Percentage
- Special case: If ZeroCont = 75, vested balance = ending balance (100% vested)

**Suggested Forfeiture:**

- Only calculated for the **current profit year**
- Formula: Ending Balance - Vested Balance
- This represents the amount that would be forfeited if processed today

---

## Report Display Logic

### Columns Displayed

| Column                     | Description                                                        |
| -------------------------- | ------------------------------------------------------------------ |
| **Badge Number**           | Employee or beneficiary identifier (with PSN suffix if applicable) |
| **Name**                   | Full name of the employee or beneficiary                           |
| **Profit Year**            | The year being reported (can be multi-year)                        |
| **Beginning Balance**      | Balance at the start of the year                                   |
| **Beneficiary Allocation** | Net beneficiary transfers (in/out)                                 |
| **Distribution Amount**    | Total distributions paid out                                       |
| **Forfeit**                | Net forfeitures for the year                                       |
| **Ending Balance**         | Final balance at year end                                          |
| **Vested Balance**         | Amount employee is entitled to keep                                |
| **Date Term**              | Termination date                                                   |
| **YTD PS Hours**           | Year-to-date profit sharing hours                                  |
| **Is Executive**           | Whether the employee is classified as executive                    |
| **Vested Percent**         | Percentage vested (0-100%)                                         |
| **Age**                    | Calculated age at end of profit year                               |
| **Has Forfeited**          | Indicates if employee has forfeiture history                       |
| **Suggested Forfeit**      | Amount to forfeit (current year only)                              |

### Multi-Year Display

If the date range spans multiple profit years, each year is displayed as a separate row:

**Example:**

```
Badge    Name           Year  Begin     Ending    Vested
12345    John Doe       2024  $10,000   $8,500    $8,500
12345    John Doe       2023  $9,500    $10,000   $7,500
```

Records are sorted by **profit year descending** (most recent first) within each employee.

---

## Data Integrity & Duplicate SSN Protection

### Duplicate SSN Guard

The Terminations page includes a **duplicate SSN guard** that:

1. **Checks for duplicate SSNs** on page load
2. **Disables the Filter button** if duplicates are detected
3. **Displays an error message** prompting users to resolve duplicates before proceeding

**Why This Matters:**

- Duplicate SSNs cause incorrect calculations and reporting
- The system must identify a unique person for vesting and transaction history
- Duplicate SSN detection is logged as a **CRITICAL** issue for IT investigation

**What to Do:**

- Navigate to the **Duplicate SSNs on Demographics** page (December Activities menu)
- Resolve all duplicates before using the Terminations page
- Contact IT if duplicates cannot be resolved

---

## Common Scenarios & Expected Behaviors

### Scenario 1: Newly Terminated Employee (< 3 Years)

**Given:**

- Employee terminates on 6/15/2024
- Employee has 2 years in the profit sharing plan
- Enrollment: Old Vesting Plan

**Expected Behavior:**

- Employee does NOT appear in the report
- Reason: Old Vesting Plan requires 3+ years

---

### Scenario 2: Terminated Employee with Partial Vesting

**Given:**

- Employee terminates on 6/15/2024
- Employee has 4 years in the profit sharing plan
- Enrollment: Old Vesting Plan
- Ending balance: $10,000

**Expected Behavior:**

- Employee appears in the report
- Vested %: 50%
- Vested Balance: $5,000
- Suggested Forfeit: $5,000 (for 2024 only)

---

### Scenario 3: Deceased Employee with Beneficiary

**Given:**

- Employee #12345 dies on 8/20/2024
- Employee has a beneficiary (#67890, PSN suffix -1000)
- Employee's ending balance: $15,000
- Beneficiary allocation: $15,000 transferred to beneficiary

**Expected Behavior:**

- Employee #12345 appears with:
  - Ending Balance: $0 (after transfer)
  - Beneficiary Allocation: -$15,000 (outgoing)
- Beneficiary #12345-1000 appears with:
  - Ending Balance: $15,000
  - Beneficiary Allocation: +$15,000 (incoming)
  - Vested %: 100%

---

### Scenario 4: Retiree Receiving Pension

**Given:**

- Employee terminates on 6/15/2024
- Termination Code: "Retired Receiving Pension"
- Ending balance: $20,000

**Expected Behavior:**

- Employee does NOT appear in the report
- Reason: Retirees receiving pension are excluded (business rule)

---

### Scenario 5: Zero Balance with No Transactions

**Given:**

- Employee terminates on 6/15/2024
- Beginning balance: $0
- No distributions, forfeitures, or beneficiary allocations
- Years in plan: 5

**Expected Behavior:**

- Employee does NOT appear in the report
- Reason: Fails the "IsInteresting" filter (no balance or transactions)

---

## Troubleshooting & Support

### Common Issues

| Issue                                | Cause                                                           | Resolution                                                               |
| ------------------------------------ | --------------------------------------------------------------- | ------------------------------------------------------------------------ |
| **Filter button is disabled**        | Duplicate SSNs detected                                         | Navigate to "Duplicate SSNs on Demographics" page and resolve duplicates |
| **Employee not appearing in report** | Does not meet eligibility criteria (years, enrollment, balance) | Verify years in plan, enrollment status, and transaction history         |
| **Vesting % seems incorrect**        | Check enrollment type and years in plan                         | Verify vesting schedule matches enrollment (Old vs. New Plan)            |
| **Beneficiary shows $0 balance**     | Beneficiary allocation not processed yet                        | Check if beneficiary transfer transactions exist in ProfitDetails        |
| **Suggested forfeit is blank**       | Record is from a prior year (not current profit year)           | Suggested forfeit only displays for the current year                     |
| **Multiple rows for same employee**  | Report spans multiple profit years                              | Each profit year is displayed as a separate row                          |

### Contact & Support

- **For Data Issues:** Contact the IT team or database administrators
- **For Business Logic Questions:** Contact the Profit Sharing department
- **For System Errors:** Check the application logs and contact IT Support

---

## Appendix: Technical Reference

### Related Database Tables

- **Demographics**: Employee personal information, hire date, termination date, employment status
- **PayProfit**: Year-specific profit sharing enrollment, hours, income, zero contribution reasons
- **ProfitDetails**: Transaction-level detail (contributions, earnings, distributions, forfeitures, beneficiary allocations)
- **Beneficiaries**: Beneficiary relationships (PSN suffix, beneficiary contact information)

### Related Endpoints

- **GET /api/reports/terminated-employees**: Retrieves terminated employee and beneficiary data
- **GET /api/lookups/duplicate-ssn-exists**: Checks for duplicate SSN records

### Key Service Methods

- `RetrieveMemberSlices`: Loads terminated employees and beneficiaries
- `MergeAndCreateDataset`: Combines member data with transactions and balances
- `IsInteresting`: Filters out records with no balances or transactions

---

**Last Updated:** October 1, 2025  
**Document Version:** 1.0  
**Maintained By:** Development Team
