# Terminated Employee & Beneficiary Report - Developer Guide

**Document Version:** 1.0  
**Last Updated:** October 1, 2025  
**Audience:** Developers, Technical Implementation

## Overview

The Terminated Employee & Beneficiary Report provides profit sharing distribution details for employees who terminated employment during a specified reporting period. This document explains the current SMART implementation architecture, data flow, and calculation logic.

---

## Table of Contents

1. [Service Architecture](#service-architecture)
2. [Data Sources](#data-sources)
3. [Calculation Flow](#calculation-flow)
4. [Transaction Processing](#transaction-processing)
5. [Vesting Calculation](#vesting-calculation)
6. [Beneficiary Handling](#beneficiary-handling)
7. [Filtering Logic](#filtering-logic)
8. [API Contract](#api-contract)
9. [Code Examples](#code-examples)
10. [Testing Guidelines](#testing-guidelines)

---

## Service Architecture

### Primary Service
**Location:** `Demoulas.ProfitSharing.Services/Reports/TerminatedEmployeeAndBeneficiaryReport/TerminatedEmployeeReportService.cs`

**Dependencies:**
- `TotalService` - Provides balance aggregation
- `CalendarService` - Provides calendar year boundaries
- `DemographicReaderService` - Reads employee demographic data
- `IProfitSharingDataContextFactory` - Database access

### Key Classes

```
TerminatedEmployeeReportService (main service)
├── MemberDto (output data structure)
│   ├── BadgePSn (badge number + optional suffix)
│   ├── BadgeNumber (employee badge)
│   ├── Name (employee name)
│   └── YearDetails[] (year-by-year calculations)
│       ├── BeginningBalance
│       ├── BeneficiaryAllocation
│       ├── DistributionAmount
│       ├── Forfeit
│       ├── EndingBalance
│       ├── VestedBalance
│       ├── YtdPsHours
│       ├── VestedPercent
│       └── Metadata (DateTerm, Age, etc.)
└── ReportTotalsDto (aggregated totals)
```

---

## Data Sources

### 1. Demographic Data
**Table:** `DEMOGRAPHIC`  
**Purpose:** Employee master data, termination information

**Key Fields:**
- `BadgeNumber` - Employee identifier
- `Ssn` - Social Security Number
- `FirstName`, `LastName` - Employee name
- `TerminationDate` - Date of employment termination
- `EmploymentStatusId` - Current status (includes deceased flag)

**Query Filter:**
```csharp
var terminated = demographics
    .Where(d => d.TerminationDate >= request.StartingDate 
             && d.TerminationDate <= request.EndingDate)
    .ToList();
```

### 2. Total Vesting Balance
**Table:** `PARTICIPANT_TOTAL_VESTING_BALANCE`  
**Purpose:** Current profit sharing balance snapshot

**Key Fields:**
- `Ssn` - Employee identifier (join to Demographics)
- `CurrentBalance` - Current total vesting balance
- `VestedBalance` - Current vested amount

**Usage:**
```csharp
var totalBalance = totalVestingBalances
    .FirstOrDefault(t => t.Ssn == demographic.Ssn);
var beginningBalance = totalBalance?.CurrentBalance ?? 0m;
```

### 3. Pay Profit Records
**Table:** `PAY_PROFIT`  
**Purpose:** Temporal profit sharing records (multiple per employee/year)

**Key Fields:**
- `DemographicId` - Links to employee
- `ProfitYear` - Year of record
- `ProfitYearAmount` - Balance for that year

**Temporal Model:** Employee can have multiple `PayProfit` records for different years

### 4. Profit Detail Transactions
**Table:** `PROFIT_DETAIL`  
**Purpose:** Individual profit sharing transactions

**Key Fields:**
- `Ssn` - Employee identifier
- `ProfitYear` - Transaction year
- `ProfitCodeId` - Transaction type (0-9)
- `YearsOfServiceCredit` - Service years earned
- `Contribution` - Contribution amount
- `Earnings` - Earnings amount
- `Forfeiture` - Forfeiture/distribution amount (overloaded!)

**Transaction Year Filter:**
```csharp
var transactions = profitDetails
    .Where(pd => pd.Ssn == demographic.Ssn 
              && pd.ProfitYear == reportYear)
    .ToList();
```

---

## Calculation Flow

### High-Level Process

```
1. Load Terminated Employees
   ↓ Filter by termination date range
   
2. For Each Employee:
   ├─→ Load Total Vesting Balance
   │   └─→ Extract BeginningBalance
   │
   ├─→ Load Profit Detail Transactions (for report year)
   │   └─→ Aggregate by transaction type
   │
   ├─→ Calculate Years in Profit Sharing
   │   └─→ Sum YEARS_OF_SERVICE_CREDIT from transactions
   │
   ├─→ Calculate Year Details
   │   ├─→ BeginningBalance (from Total Vesting Balance)
   │   ├─→ Contributions (sum Type 6)
   │   ├─→ Distributions (sum Types 1, 3, 9)
   │   ├─→ Forfeitures (sum Type 2)
   │   ├─→ BeneficiaryAllocation (Types 5, 6)
   │   ├─→ EndingBalance = Begin + Contrib - Dist - Forfeit +/- BeneAlloc
   │   ├─→ VestedPercent (from years + schedule)
   │   └─→ VestedBalance = EndingBalance × VestedPercent
   │
   ├─→ Apply IsInteresting Filter
   │   └─→ Skip if all values zero
   │
   └─→ Add to Results

3. Process Beneficiaries (for deceased employees)
   └─→ Create beneficiary records with 1000 suffix

4. Sort and Paginate
   └─→ Order by BadgePSn, apply pagination

5. Calculate Report Totals
   └─→ Sum all ending balances, vested amounts, etc.
```

### Detailed Calculation Formulas

#### Beginning Balance
```csharp
var totalBalance = await _totalService.GetTotalVestingBalanceByBadgeAsync(
    badgeNumber, cancellationToken);
    
decimal beginningBalance = totalBalance?.CurrentBalance ?? 0m;
```

**Source:** `PARTICIPANT_TOTAL_VESTING_BALANCE.CurrentBalance`

#### Years in Profit Sharing
```csharp
var yearsInPs = transactions
    .Where(t => t.ProfitYear == profitYear)
    .Sum(t => t.YearsOfServiceCredit ?? 0);
```

**Source:** Aggregated from `PROFIT_DETAIL.YEARS_OF_SERVICE_CREDIT`

#### Transaction Aggregation
```csharp
// Contributions (Type 6)
var contributions = transactions
    .Where(t => t.ProfitCodeId == 6)
    .Sum(t => t.Contribution ?? 0m);

// Distributions (Types 1, 3, 9)
var distributions = transactions
    .Where(t => t.ProfitCodeId is 1 or 3 or 9)
    .Sum(t => t.Forfeiture ?? 0m);  // Note: Uses Forfeiture field!

// Forfeitures (Type 2)
var forfeitures = transactions
    .Where(t => t.ProfitCodeId == 2)
    .Sum(t => t.Forfeiture ?? 0m);

// Beneficiary Allocation
var beneDebit = transactions
    .Where(t => t.ProfitCodeId == 5)
    .Sum(t => t.Forfeiture ?? 0m);  // Debit from employee
    
var beneCredit = transactions
    .Where(t => t.ProfitCodeId == 6)
    .Sum(t => t.Contribution ?? 0m);  // Credit to beneficiary

decimal beneficiaryAllocation = beneCredit - beneDebit;
```

#### Ending Balance
```csharp
decimal endingBalance = beginningBalance 
                      + contributions 
                      - distributions 
                      - forfeitures 
                      + beneficiaryAllocation;
```

#### Vesting Percentage
```csharp
decimal vestedPercent = CalculateVestingPercent(yearsInPs, endingBalance);

private decimal CalculateVestingPercent(int years, decimal balance)
{
    // Special cases
    if (balance == 0) return 0m;
    if (years > 2) return 1.0m;  // 100% vested after 2 years
    
    // Old schedule (< 2 years): [0, 0, 20, 40, 60, 80, 100]
    // New schedule (= 2 years): [0, 20, 40, 60, 80, 100]
    
    if (years < 2)
    {
        int[] oldSchedule = { 0, 0, 20, 40, 60, 80, 100 };
        return oldSchedule[Math.Min(years, 6)] / 100m;
    }
    else
    {
        int[] newSchedule = { 0, 20, 40, 60, 80, 100 };
        return newSchedule[Math.Min(years, 5)] / 100m;
    }
}
```

#### Vested Balance
```csharp
decimal vestedBalance = endingBalance * vestedPercent;
```

---

## Transaction Processing

### Transaction Type Mapping

The `PROFIT_DETAIL` table uses a `PROFIT_CODE_ID` field to classify transactions. **Critical:** The entity property mapping is counter-intuitive!

| Type | Description | Properties Used | Logic |
|------|-------------|-----------------|-------|
| 0 | Combined forfeit/contribution/earnings | `Forfeiture + Contribution + Earnings` | Sum all three properties |
| 1 | Standard distribution | `Forfeiture` | Distribution amount stored in Forfeiture! |
| 2 | Forfeiture | `Forfeiture` | Actual forfeiture amount |
| 3 | Hardship distribution | `Forfeiture` | Distribution amount stored in Forfeiture! |
| 5 | Beneficiary allocation debit | `Forfeiture` | Debit amount stored in Forfeiture! |
| 6 | Beneficiary allocation credit | `Contribution` | Credit amount |
| 8 | Investment earnings | `Earnings` | Earnings amount |
| 9 | Distribution (alternative) | `Forfeiture` | Distribution amount stored in Forfeiture! |

### Important: Forfeiture Field Overloading

The `ProfitDetail.Forfeiture` property holds different semantic meanings depending on `ProfitCodeId`:

```csharp
// Example: Reading a distribution (Type 1)
if (profitDetail.ProfitCodeId == 1)
{
    // Despite property name, this is a DISTRIBUTION not a forfeiture!
    decimal distribution = profitDetail.Forfeiture ?? 0m;
}

// Example: Reading actual forfeiture (Type 2)
if (profitDetail.ProfitCodeId == 2)
{
    // Now it's actually a forfeiture
    decimal actualForfeit = profitDetail.Forfeiture ?? 0m;
}
```

**Always check `ProfitCodeId` before interpreting property values!**

---

## Vesting Calculation

### Vesting Schedule Logic

Profit sharing balances vest (become owned by employee) over time based on years of service.

### Three Vesting Scenarios

#### 1. Old Schedule (Years < 2)
**Applies to:** Employees enrolled before change to new schedule

```
Year 0: 0%
Year 1: 0%
Year 2: 20%
Year 3: 40%
Year 4: 60%
Year 5: 80%
Year 6+: 100%
```

**Implementation:**
```csharp
int[] oldSchedule = { 0, 0, 20, 40, 60, 80, 100 };
int percent = oldSchedule[Math.Min(years, 6)];
```

#### 2. New Schedule (Years = 2)
**Applies to:** Employees enrolled in transition year

```
Year 0: 0%
Year 1: 20%
Year 2: 40%
Year 3: 60%
Year 4: 80%
Year 5+: 100%
```

**Implementation:**
```csharp
int[] newSchedule = { 0, 20, 40, 60, 80, 100 };
int percent = newSchedule[Math.Min(years, 5)];
```

#### 3. Immediate 100% Vesting
**Applies to:**
- Employees with > 2 years service credit
- Deceased employees (ZeroCont = 6 in COBOL logic)
- Employees at or past retirement age

**Implementation:**
```csharp
if (years > 2 || isDeceased || balance == 0)
{
    return 1.0m;  // 100%
}
```

### Edge Cases

#### Zero Balance
```csharp
// Prevent division issues, return 0% for zero balance
if (endingBalance == 0)
{
    return 0m;
}
```

#### Beneficiaries
Beneficiaries inherit the vested percentage of the deceased employee:
```csharp
// When creating beneficiary record
beneficiary.VestedPercent = employeeVestedPercent;
beneficiary.BeneficiaryAllocation = employee.VestedBalance;
```

---

## Beneficiary Handling

### Purpose
When an employee dies, their vested profit sharing balance is allocated to beneficiaries.

### Process

#### 1. Identify Deceased Employees
```csharp
const int deceasedStatusId = 6;  // Employment status for deceased

var deceased = demographics
    .Where(d => d.EmploymentStatusId == deceasedStatusId)
    .ToList();
```

#### 2. Create Beneficiary Records
```csharp
foreach (var demographic in deceased)
{
    // Find corresponding employee member record
    var employee = members.FirstOrDefault(m => 
        m.BadgeNumber == demographic.BadgeNumber);
    
    if (employee == null) continue;
    
    // Create beneficiary with special PSN
    var beneficiary = new MemberDto
    {
        // Append "1000" to create beneficiary identifier
        BadgePSn = demographic.BadgeNumber + "1000",
        
        // Keep original badge for reference
        BadgeNumber = demographic.BadgeNumber,
        
        // Copy employee details
        Name = employee.Name,
        Ssn = employee.Ssn,
        
        // Year details with beneficiary allocation
        YearDetails = new[]
        {
            new YearDetailsDto
            {
                ProfitYear = reportYear,
                
                // Beneficiary gets the vested amount
                BeneficiaryAllocation = employee.YearDetails[0].VestedBalance,
                
                // Copy other relevant fields
                BeginningBalance = 0m,
                EndingBalance = employee.YearDetails[0].VestedBalance,
                VestedBalance = employee.YearDetails[0].VestedBalance,
                VestedPercent = 1.0m,  // 100% vested for beneficiaries
                
                // Metadata
                DateTerm = demographic.TerminationDate,
                Age = CalculateAge(demographic.BirthDate, reportDate)
            }
        }
    };
    
    members.Add(beneficiary);
}
```

#### 3. Badge PSN Suffix Convention

| Suffix | Meaning | Example |
|--------|---------|---------|
| (none) | Regular employee | `705824` |
| `1000` | Primary beneficiary | `7058241000` |
| `100` | Alternate beneficiary format | `706678100` |

**Format:** `{BadgeNumber}{Suffix}`

---

## Filtering Logic

### IsInteresting Method

Purpose: Filter out employees with no activity to reduce report clutter

```csharp
private bool IsInteresting(MemberDto member)
{
    var yearDetails = member.YearDetails?.FirstOrDefault();
    if (yearDetails == null) return false;
    
    return yearDetails.BeginningBalance != 0
        || yearDetails.BeneficiaryAllocation != 0
        || yearDetails.DistributionAmount != 0
        || yearDetails.Forfeit != 0;
}
```

**Logic:** Include employee if ANY of these is non-zero:
- Beginning balance
- Beneficiary allocation
- Distribution amount  
- Forfeiture amount

**Excludes:** Employees with all zeros (no activity)

### Application Points

```csharp
// After calculating each member
if (IsInteresting(member))
{
    members.Add(member);
}
else
{
    // Skip this employee - no reportable activity
}
```

---

## API Contract

### Request Structure

```csharp
public class TerminatedEmployeeAndBeneficiaryRequest
{
    public DateTime StartingDate { get; set; }  // Filter: term date >= this
    public DateTime EndingDate { get; set; }    // Filter: term date <= this
    public int Page { get; set; }               // Pagination: page number (1-based)
    public int PageSize { get; set; }           // Pagination: results per page
}
```

**Example Request:**
```json
{
  "startingDate": "2025-01-01",
  "endingDate": "2025-12-31",
  "page": 1,
  "pageSize": 50
}
```

### Response Structure

```csharp
public class PaginatedResponseDto<TerminatedEmployeeAndBeneficiaryDataResponseDto>
{
    public List<MemberDto> Results { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public ReportTotalsDto Totals { get; set; }
}

public class MemberDto
{
    public string BadgePSn { get; set; }        // Badge + suffix
    public int BadgeNumber { get; set; }        // Original badge
    public string Name { get; set; }            // Last, First
    public string Ssn { get; set; }             // SSN
    public YearDetailsDto[] YearDetails { get; set; }
}

public class YearDetailsDto
{
    public int ProfitYear { get; set; }
    public decimal BeginningBalance { get; set; }
    public decimal BeneficiaryAllocation { get; set; }
    public decimal DistributionAmount { get; set; }
    public decimal Forfeit { get; set; }
    public decimal EndingBalance { get; set; }
    public decimal VestedBalance { get; set; }
    public decimal YtdPsHours { get; set; }
    public decimal VestedPercent { get; set; }  // 0.0 - 1.0 (not percentage)
    public DateTime? DateTerm { get; set; }
    public int Age { get; set; }
    public bool HasForfeited { get; set; }
}

public class ReportTotalsDto
{
    public decimal TotalEndingBalance { get; set; }
    public decimal TotalVested { get; set; }
    public decimal TotalDistributions { get; set; }
    public decimal TotalForfeitures { get; set; }
    public decimal TotalBeneficiaryAllocation { get; set; }
}
```

### Sorting & Pagination

**Sort Order:** BadgePSn ascending (alphanumeric)

```csharp
var sorted = members
    .OrderBy(m => m.BadgePSn)
    .ToList();
```

**Pagination:**
```csharp
var pageResults = sorted
    .Skip((request.Page - 1) * request.PageSize)
    .Take(request.PageSize)
    .ToList();
```

---

## Code Examples

### Example 1: Basic Service Call

```csharp
public async Task<PaginatedResponseDto<TerminatedEmployeeAndBeneficiaryDataResponseDto>> 
    GenerateReport(CancellationToken ct)
{
    var request = new TerminatedEmployeeAndBeneficiaryRequest
    {
        StartingDate = new DateTime(2025, 1, 1),
        EndingDate = new DateTime(2025, 12, 31),
        Page = 1,
        PageSize = 50
    };
    
    var result = await _reportService.GetReportAsync(request, ct);
    
    return result;
}
```

### Example 2: Processing Response

```csharp
var response = await _reportService.GetReportAsync(request, ct);

foreach (var member in response.Results)
{
    Console.WriteLine($"Badge: {member.BadgePSn}");
    Console.WriteLine($"Name: {member.Name}");
    
    var yearDetail = member.YearDetails[0];
    Console.WriteLine($"Beginning: ${yearDetail.BeginningBalance:N2}");
    Console.WriteLine($"Ending: ${yearDetail.EndingBalance:N2}");
    Console.WriteLine($"Vested: ${yearDetail.VestedBalance:N2}");
    Console.WriteLine($"Vested %: {yearDetail.VestedPercent:P}");
    Console.WriteLine();
}

Console.WriteLine("Report Totals:");
Console.WriteLine($"Total Ending: ${response.Totals.TotalEndingBalance:N2}");
Console.WriteLine($"Total Vested: ${response.Totals.TotalVested:N2}");
```

### Example 3: Filtering by Balance

```csharp
// Get only employees with vested balances over $10,000
var highBalance = response.Results
    .Where(m => m.YearDetails[0].VestedBalance > 10000m)
    .ToList();
```

### Example 4: Identifying Beneficiaries

```csharp
// Beneficiaries have 4-digit suffix (1000 or 100)
var beneficiaries = response.Results
    .Where(m => m.BadgePSn.Length > m.BadgeNumber.ToString().Length)
    .ToList();

foreach (var beneficiary in beneficiaries)
{
    Console.WriteLine($"Beneficiary: {beneficiary.Name}");
    Console.WriteLine($"Badge PSN: {beneficiary.BadgePSn}");
    Console.WriteLine($"Employee Badge: {beneficiary.BadgeNumber}");
    Console.WriteLine($"Allocation: ${beneficiary.YearDetails[0].BeneficiaryAllocation:N2}");
}
```

---

## Testing Guidelines

### Unit Testing Strategy

#### 1. Transaction Processing Tests
```csharp
[Theory]
[InlineData(1, 1000.00)] // Distribution
[InlineData(2, 500.00)]  // Forfeiture
[InlineData(6, 2000.00)] // Contribution
public async Task ProcessTransaction_Type_CalculatesCorrectly(
    int typeCode, decimal amount)
{
    // Arrange
    var transaction = new ProfitDetail
    {
        ProfitCodeId = typeCode,
        // Set appropriate property based on type
    };
    
    // Act
    var result = ProcessTransaction(transaction);
    
    // Assert
    result.Should().Be(amount);
}
```

#### 2. Vesting Calculation Tests
```csharp
[Theory]
[InlineData(0, 0.0)]    // 0 years = 0%
[InlineData(1, 0.0)]    // 1 year old schedule = 0%
[InlineData(2, 0.2)]    // 2 years new schedule = 20%
[InlineData(3, 0.4)]    // 3 years = 40%
[InlineData(6, 1.0)]    // 6+ years = 100%
public void CalculateVestingPercent_ReturnsCorrectSchedule(
    int years, decimal expected)
{
    // Act
    var result = CalculateVestingPercent(years, balance: 1000m);
    
    // Assert
    result.Should().Be(expected);
}
```

#### 3. IsInteresting Filter Tests
```csharp
[Fact]
public void IsInteresting_AllZeros_ReturnsFalse()
{
    // Arrange
    var member = new MemberDto
    {
        YearDetails = new[]
        {
            new YearDetailsDto
            {
                BeginningBalance = 0,
                BeneficiaryAllocation = 0,
                DistributionAmount = 0,
                Forfeit = 0
            }
        }
    };
    
    // Act
    var result = IsInteresting(member);
    
    // Assert
    result.Should().BeFalse();
}
```

### Integration Testing Strategy

#### 1. End-to-End Report Generation
```csharp
[Fact]
public async Task GetReportAsync_ValidRequest_ReturnsData()
{
    // Arrange
    var request = new TerminatedEmployeeAndBeneficiaryRequest
    {
        StartingDate = new DateTime(2025, 1, 1),
        EndingDate = new DateTime(2025, 12, 31),
        Page = 1,
        PageSize = 50
    };
    
    // Act
    var response = await _service.GetReportAsync(request, ct);
    
    // Assert
    response.Should().NotBeNull();
    response.Results.Should().NotBeEmpty();
    response.Totals.Should().NotBeNull();
    response.Totals.TotalEndingBalance.Should().BeGreaterThan(0);
}
```

#### 2. Beneficiary Processing
```csharp
[Fact]
public async Task GetReportAsync_DeceasedEmployee_CreatesBeneficiary()
{
    // Arrange - requires test data with deceased employee
    
    // Act
    var response = await _service.GetReportAsync(request, ct);
    
    // Assert
    var beneficiary = response.Results
        .FirstOrDefault(m => m.BadgePSn.EndsWith("1000"));
        
    beneficiary.Should().NotBeNull();
    beneficiary.BeneficiaryAllocation.Should().BeGreaterThan(0);
}
```

### Test Data Requirements

**Minimum Test Data Set:**
- At least 1 employee terminated in report period
- At least 1 employee with transactions (all types)
- At least 1 deceased employee (for beneficiary testing)
- At least 1 employee with zero balance (for filtering)
- At least 1 employee with each vesting schedule scenario

---

## Performance Considerations

### Database Queries

**Query Count per Request:**
1. Demographics query (terminated employees)
2. Total vesting balances (bulk or per-employee)
3. Pay profit records (per employee or bulk)
4. Profit detail transactions (per employee or bulk)
5. Deceased employees query (for beneficiaries)

**Optimization Strategies:**
```csharp
// Bulk load instead of N+1 queries
var ssns = demographics.Select(d => d.Ssn).ToList();

var totalBalances = await context.ParticipantTotalVestingBalances
    .Where(t => ssns.Contains(t.Ssn))
    .ToListAsync();

var transactions = await context.ProfitDetails
    .Where(pd => ssns.Contains(pd.Ssn) && pd.ProfitYear == reportYear)
    .ToListAsync();
```

### Memory Considerations

**Large Reports:** If report contains thousands of employees, consider:
- Streaming results instead of loading all at once
- Processing in batches
- Implementing proper pagination limits

```csharp
// Enforce maximum page size
const int MaxPageSize = 100;
request.PageSize = Math.Min(request.PageSize, MaxPageSize);
```

---

## Troubleshooting

### Common Issues

#### Issue: Missing Employees in Report
**Symptoms:** Expected employee not appearing

**Checklist:**
1. Verify termination date within request date range
2. Check if filtered by `IsInteresting` (all values zero)
3. Verify demographic record exists
4. Check if total vesting balance exists

#### Issue: Incorrect Vested Balance
**Symptoms:** Vested balance calculation seems wrong

**Checklist:**
1. Verify years in profit sharing calculation
2. Check vesting schedule being applied
3. Verify ending balance calculation
4. Check if employee is deceased (affects vesting)

#### Issue: Missing Beneficiaries
**Symptoms:** Deceased employee but no beneficiary record

**Checklist:**
1. Verify `EmploymentStatusId = 6` (deceased)
2. Check if employee has vested balance > 0
3. Verify employee passed `IsInteresting` filter
4. Check beneficiary record creation logic

---

## Related Documentation

- **COBOL Analysis:** `QPAY066_COBOL_ANALYSIS.md` - Detailed COBOL logic breakdown
- **Discrepancy Analysis:** `TERMINATED_EMPLOYEE_REPORT_ANALYSIS.md` - SMART vs READY comparison
- **API Documentation:** Swagger/OpenAPI docs (when available)
- **Database Schema:** Entity relationship diagrams

---

## Glossary

| Term | Definition |
|------|------------|
| **Badge PSN** | Badge number with optional suffix (1000 for beneficiaries) |
| **Vesting** | Process of employee earning ownership of profit sharing balance over time |
| **IsInteresting** | Filter to exclude employees with no reportable activity |
| **Temporal Model** | Data model with multiple records per entity over time |
| **Type Code** | Transaction classification in ProfitDetail (0-9) |
| **Forfeiture** | Loss of non-vested balance when employee terminates |
| **Beginning Balance** | Starting balance at beginning of report year |
| **Ending Balance** | Calculated balance at end of report year |
| **Vested Balance** | Portion of ending balance that employee owns |

---

## Change History

| Date | Version | Changes | Author |
|------|---------|---------|--------|
| 2025-10-01 | 1.0 | Initial developer guide | System |

---

## Support

For questions or issues with this implementation:
1. Review related documentation (see above)
2. Check test suite for examples
3. Consult with senior developer or architect
4. Reference COBOL source code for legacy logic
