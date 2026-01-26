## This document is a summary of some of the fields in the READY database.   Along with a quick understanding of the purpose of the field and how it maps to the SMART codebase and database.

### Core tables
| READY Tablename | Description        | SMART Equivalent | Notes |
|-----------------|--------------------|------------------|-------|
| DEMOGRAPHICS    |Holds all employees | The demographics table is accessed using the IDemographicReaderService interface in SMART.  More details in the [IDemographicReaderService](#IDemographicReaderService) section below. |  |
| PAYBEN          |Holds all beneficiaries (name/address) | BeneficiaryContact | |
| PROFIT_DETAIL   | Holds all transactions | ProfitDetail | |
| PROFIT_SS_DETAIL| There are also transaction | ProfitDetail |
| PAYPROFIT | Holds totals for the year | PAY_PROFIT | Note that in READY this table would have be cleared to work with a new year's data.  In SMART this table is cumulative, separated by PROFIT_YEAR. |
| PAYREL | Holds relationships between employees and beneficiaries | Beneficiary | |
| PROFIT_DIST_REQ | Holds hardship distribution requests | DISTRIBUTION_REQUEST | |
| PROFIT_DIST | Holds distributions | DISTRIBUTION | |

### READY DEMOGRAPHICS table
We expect this table to contain about 25000 rows.

| NAME | SMART Database Column | SMART Code Property | Notes |
|------|-----------------------|---------------------|-------|
|DEM_BADGE | BADGE_NUMBER | BadgeNumber | |
|DEM_SSN | SSN | Ssn | When SSNs are output, we always mask them as a string using the MaskSsn extension method we have written.|
|PY_NAM | FULL_NAME | FullName | |
|PY_LNAME | LAST_NAME | LastName | |
|PY_FNAME | FIRST_NAME | FirstName | |
|PY_MNAME | MIDDLE_NAME | MiddleName | |
|PY_STOR | STORE_NUMBER | StoreNumber | |
|PY_DP | DEPARTMENT | DepartmentId | |
|PY_CLA | PAY_CLASSIFICATION_ID | PayClassificationId | |
|PY_ADD | STREET1 | ContactInfo.Address.Street | |
|PY_ADD2 | STREET2 | ContactInfo.Address.Street2 | |
|PY_CITY | CITY | ContactInfo.Address.City | |
|PY_STATE | STATE | ContactInfo.Address.State | |
|PY_ZIP | POSTAL_CODE | ContactInfo.Address.PostalCode | |
|PY_DOB | DATE_OF_BIRTH | DateOfBirth | |
|PY_FUL | EMPLOYMENT_TYPE_ID | EmploymentTypeId | |
|PY_FREQ | PAY_FREQUENCY_ID | PayFrequencyId | |
|PY_TYPE | Not used in SMART | n/a | |
|PY_SCOD | EMPLOYMENT_STATUS_ID | EmploymentStatusId | |
|PY_HIRE_DT | HIRE_DATE | HireDate | |
|PY_FULL_DT | FULL_TIME_DATE | FullTimeDate | |
|PY_REHIRE_DT | REHIRE_DATE | RehireDate | |
|PY_TERM_DT | TERMINATION_DATE | TerminationDate | |
|PY_TERM | TERMINATION_CODE_ID | TerminationCodeId | |
|PY_ASSIGN_ID | ORACLE_HCMID | OracleHcmId | |
|PY_ASSIGN_DESC | Not used in SMART | n/a | |
|PY_NEW_EMP | Not used in SMART | n/a | |
|PY_GENDER | GENDER_ID | GenderId | |
|PY_EMP_TELNO | PHONE_NUMBER | ContactInfo.PhoneNumber | |

### READY PROFIT_DETAIL table
We expect this table to contain about 250000 rows.
| NAME | SMART Database Column | SMART Code Property | Notes |
|------|-----------------------|---------------------|-------|
|PROFIT_YEAR | PROFIT_YEAR, PROFIT_YEAR_ITERATION |ProfitYear, ProfitYearIteration | In READY, the year is specified as a numeric field, stated as yyyy.x, where yyyy is the year, and x is the iteration (0 for initial, 2 for military, etc).  In SMART, these are two separate fields. |
|PROFIT_CLIENT | Not used in SMART | n/a| |
|PROFIT_CODE | PROFIT_CODE_ID | ProfitCodeId | |
|PROFIT_CONT | CONTRIBUTION | Contribution | |
|PROFIT_EARN | EARNINGS | Earnings | |
|PROFIT_FORT | FORFEITURE | Forfeiture | |
|PROFIT_MDTE | MONTH_TO_DATE | MonthToDate | |
|PROFIT_YDTE | YEAR_TO_DATE | YearToDate | |
|PROFIT_CMNT | REMARK | Remark | This column contained consistent free-form text.   Those values were parsed out into the COMMENT_TYPE_ID in SMART.  The original READY values were kept in this field |
|PROFIT_FED_TAXES | FEDERAL_TAXES | FederalTaxes | |
|PROFIT_STATE_TAXES | STATE_TAXES | StateTaxes | |
|PROFIT_TAX_CODE | TAX_CODE_ID | TaxCodeId | |
|PR_DET_S_SEC_NUMBER | SSN | Ssn | When SSNs are output, we always mask them as a string using the MaskSsn extension method we have written.|
|PROFIT_DET_PR_DET_S_SEQNUM | DISTRIBUTION_SEQUENCE | DistributionSequence | This field is a foreign key reference to the PROFDIST table.

### READY PROFIT_SS_DETAIL table
| NAME | SMART Database Column | SMART Code Property | Notes |
|------|-----------------------|---------------------|-------|
|PROFIT_SS_YEAR | PROFIT_YEAR, PROFIT_YEAR_ITERATION |ProfitYear, ProfitYearIteration | In READY, the year is specified as a numeric field, stated as yyyy.x, where yyyy is the year, and x is the iteration (0 for initial, 2 for military, etc).  In SMART, these are two separate fields. |
|PROFIT_SS_CLIENT | Not used in SMART | n/a| |
|PROFIT_SS_CODE | PROFIT_CODE_ID | ProfitCodeId | |
|PROFIT_SS_CONT | CONTRIBUTION | Contribution | |
|PROFIT_SS_EARN | EARNINGS | Earnings | |
|PROFIT_SS_FORT | FORFEITURE | Forfeiture | |
|PROFIT_SS_MDTE | MONTH_TO_DATE | MonthToDate | |
|PROFIT_SS_YDTE | YEAR_TO_DATE | YearToDate | |
|PROFIT_SS_CMNT | REMARK | Remark | This column contained consistent free-form text.   Those values were parsed out into the COMMENT_TYPE_ID in SMART.  The original READY values were kept in this field |
|PROFIT_SS_FED_TAXES | FEDERAL_TAXES | FederalTaxes | |
|PROFIT_SS_STATE_TAXES | STATE_TAXES | StateTaxes | |
|PROFIT_SS_TAX_CODE | TAX_CODE_ID | TaxCodeId | |
|PR_SS_D_S_SEC_NUMBER  | SSN | Ssn | When SSNs are output, we always mask them as a string using the MaskSsn extension method we have written.|
|PROFIT_SS_DET_PR_SS_D_S_SEQNUM | DISTRIBUTION_SEQUENCE | DistributionSequence | This field is a foreign key reference to the PROFDIST table.

### READY PAYBEN table
| NAME | SMART Database Column | SMART Code Property | Notes |
|------|-----------------------|---------------------|-------|
|PYBEN_PSN | BENEFICIARY.DEMOGRAPHIC_ID, BENEFICIARY.PSN_SUFFIX | Beneficiary.DemographicId, Beneficiary.PsnSuffix | The PSN contains 11 digits.   The first 7 digits are the Demographics.BadgeNumber of the employee who is awarding the beneficiary.  The last 4 digits are the PsnSuffix, which is used to differentiate multiple beneficiaries for the same employee. |
|PYBEN_TYPE | Not used in SMART | n/a | |
|PYBEN_PERCENT | BENEFICIARY.PERCENT | Percent | |
|PYBEN_NAME | BENEFICIARY_CONTACT.FULL_NAME | BeneficiaryContact.ContactInfo.Name | |
|PAYBEN_ADD |BENEFICIARY_CONTACT.STREET1 | BeneficiaryContact.ContactInfo.Address.Street | |
|PAYBEN_CITY | BENEFICIARY_CONTACT.CITY | BeneficiaryContact.ContactInfo.Address.City | |
|PAYBEN_STATE | BENEFICIARY_CONTACT.STATE | BeneficiaryContact.ContactInfo.Address.State |
|PAYBEN_ZIP | BENEFICIARY_CONTACT.PostalCode | BeneficiaryContact.ContactInfo.Address.PostalCode | |
|PAYBEN_DOBIRTH | BENEFICIARY_CONTACT.DATE_OF_BIRTH | BeneficiaryContact.DateOfBirth | |
|PYBEN_PSDISB  | Not used in SMART | n/a | |
|PYBEN_PSAMT | calculated field in SMART | totalService.TotalVestingBalance().VestedBalance |
|PYBEN_PROF_EARN| Not used in SMART | n/a | |

### READY PAYPROFIT table
We expect this table to contain about 50000 rows.
| NAME | SMART Database Column | SMART Code Property | Notes |
|------|-----------------------|---------------------|-------|
|PAYPROF_BADGE | Not used in SMART | n/a | The relationship to the demographic employee is via PAY_PROFIT.DEMOGRAPHIC_ID in SMART. |
|PAYPROF_SSN | Not used in SMART | n/a | The SSN is stored in the DEMOGRAPHICS table in SMART. |
| PY_PH | PAY_PROFIT.CURRENT_HOURS_YEAR | CurrentHoursYear | |
| PY_PD | PAY_PROFIT.CURRENT_INCOME_YEAR | CurrentIncomeYear | |
| PY_WEEKS_WORK | Not used in SMART | n/a | |
| PY_PROF_CERT | Not used in SMART | n/a | |
| PY_PS_ENROLLED | PAY_PROFIT.VESTING_SCHEDULE_ID | VestingScheduleId | |
| PY_PS_YEARS | calculated field in SMART | totalService.CalculateVestingYears() | |
| PY_PROF_BENEFICIARY | Not used in SMART | n/a | PayProfit rows are employee only |
| PY_PROF_INITIAL_CONT | calculated field in SMART | totalService.GetFirstContributionYear() | |
| PY_PS_AMT | calculated field in SMART | totalService.TotalVestingBalance().CurrentBalance | | 
| PY_PS_VAMT | calculated field in SMART | totalService.TotalVestingBalance().VestedBalance | |
| PY_PH_LASTYR | calculated field in SMART | | In smart, prior year data is retrieved by substracting 1 from the current profit year.  In this instance the field retrieved would be PY_PH |
| PY_PD_LASTYR  | calculated field in SMART | | In smart, prior year data is retrieved by substracting 1 from the current profit year.  In this instance the field retrieved would be PY_PD |
| PY_PROF_NEWEMP | EMPLOYEE_TYPE_ID | EmployeeTypeId | |
| PY_PROF_POINTS | POINTS_EARNED | PointsEarned | |
| PY_PROF_CONT | Not used in SMART | n/a | |
| PY_PROF_FORF | Not used in SMART | n/a | |
| PY_VESTED_FLAG | Not used in SMART | n/a | |
| PY_PROF_MAXCONT | Not used in SMART | n/a | |
| PY_PROF_ZEROCONT | PAY_PROFIT.ZERO_CONTRIBUTION_FLAG_ID | ZeroContributionFlagId | |
| PY_WEEKS_WORK_LAST | Not used in SMART | n/a | |
| PY_PROF_EARN | Not used in SMART | n/a | |
| PY_PS_ETVA | PAY_PROFIT.ETVA | Etva | |
| PY_PRIOR_ETVA | calculated field in SMART | | In smart, prior year data is retrieved by substracting 1 from the current profit year.  In this instance the field retrieved would be PY_PS_ETVA |
| PY_PROF_EARN2 | Not used in SMART | n/a | |
| PY_PROF_ETVA2 | Not used in SMART | n/a | |
| PY_PH_EXEC | PAY_PROFIT.CURRENT_HOURS_YEAR | CurrentHoursYear | |
| PY_PD_EXEC | PAY_PROFIT.CURRENT_INCOME_YEAR | CurrentIncomeYear | |

### IDemographicReaderServive {#IDemographicReaderService}
The IDemographicReaderService returns an IQueryable of the Demographic class.   The main method to be called is BuildDemographicQueryAsync.  It takes the DbContext, and a boolean specifying whether or not the query is to return frozen demographic data, or not.  Generally, READY code that accesses the DEMO_PROFSHARE table will want to pass true for this parameter.

### ITotalService {#ITotalService}
Many of the transaction totals in READY were persisted in the database. In SMART, these totals are calculated on the fly using the ITotalService. It has the following methods:

- `Task<TotalVestingBalance> TotalVestingBalance(DbContext ctx, int profitYear, DateOnly asOfDate)` - Returns a domain object containing:
  - `VestedBalance` - total vested amount (replaces PY_PS_VAMT)
  - `CurrentBalance` - total balance (replaces PY_PS_AMT)
  - `VestingPercent` - vesting percentage (replaces manual calculation from PY_PS_ENROLLED + PY_PS_YEARS)

> ⚠️ **IMPORTANT**: Always use `TotalVestingBalance().VestingPercent` for vesting percentage. Do NOT manually implement vesting schedule logic (old schedule: 0/0/20/40/60/80/100 vs new schedule: 0/20/40/60/80/100). The TotalService handles all schedule variations, special termination codes, and edge cases.

- `Task<ParticipantTotalYear> GetYearsOfService(...)` - Returns years of service (replaces PY_PS_YEARS)
- `Task<SsnAndFirstYear> GetFirstContributionYear(...)` - Returns first contribution year (replaces PY_PROF_INITIAL_CONT)

### Profit Codes
Often, the COBOL source specifies profit codes directly. In SMART, profit codes are represented by the ProfitCode entity, which exposes a constant for each profit code ID. Listed below are the known READY profit codes, their names in SMART:

| READY PROFIT_CODE | SMART ProfitCode Constant |
| ------------------|---------------------------|
| 0 | ProfitCode.Constants.IncomingContributions.Id |
| 1 | ProfitCode.Constants.OutgoingPaymentsPartialWithdrawals.Id |
| 2 | ProfitCode.Constants.OutgoingForfeitures.Id |
| 3 | ProfitCode.Constants.OutgoingDirectPayments.Id |
| 5 | ProfitCode.Constants.OutgoingXferBeneficiary.Id |
| 6 | ProfitCode.Constants.IncomingQdroBeneficiary.Id |
| 8 | ProfitCode.Constants.Incoming100PercentVestedEarnings.Id |
| 9 | ProfitCode.Constants.Outgoing100PercentVestedPayment.Id |

###Employment Status Codes (PY_SCOD)
In smart, employement status codes are defined as constants as shown below.   These constants contain the value, don't add .Id.  When referencing values from READY, use the following table to translate the values:
| READY PY_SCOD | SMART EmploymentStatusCode Constant |
|---------------|----------------------------------|
| a | EmploymentStatusCode.Constants.Active |
| i | EmploymentStatusCode.Constants.Inactive |
| t | EmploymentStatusCode.Constants.Terminated |
| d | EmploymentStatusCode.Constants.Delete |

### Termination Codes (PY_TERM)
In smart, termination codes are defined as constants as shown below.   These constants contain the value, don't add .Id.  When referencing values from READY, use the following table to translate the values:
| READY PY_TERM | SMART TerminationCode Constant |
| Value | Constant Name |
|-------|---------------|
| A | LeftOnOwn |
| B | PersonalOrFamilyReason |
| C | CouldNotWorkAvailableHours |
| D | Stealing |
| E | NotFollowingCompanyPolicy |
| F | FmlaExpired |
| G | TerminatedPrivate |
| H | JobAbandonment |
| I | HealthReasonsNonFmla |
| J | LayoffNoWork |
| K | SchoolOrSports |
| L | MoveOutOfArea |
| M | PoorPerformance |
| N | OffForSummer |
| O | WorkmansCompensation |
| P | Injured |
| Q | Transferred |
| R | Retired |
| S | Competition |
| T | AnotherJob |
| U | WouldNotRehire |
| V | NeverReported |
| W | RetiredReceivingPension |
| X | Military |
| Y | FmlaApproved |
| Z | Deceased |

It is important to use the constants from the ProfitCode class rather than hardcoding numeric values, to ensure consistency and maintainability.

### Calculating Age
In SMART, age is using calculated via the .Age extension method on DateOnly.   To calculate the age against the current clock, use .Age().   To calculate age as of a specific date, pass that date as a parameter to the Age method.

### DBContext
In SMART, all database access is attempted via EntityFramework.   Typically, a IProfitSharingDataContextFactory is injected into the class.  This factory is used to create a DbContext for database access.   Example for read usage:

```csharp
return _dataContextFactory.UseReadOnlyContext(async ctx =>
{
    return ctx.Demographics.Where(d => d.BadgeNumber == badgeNumber).FirstOrDefaultAsync();
});
```
