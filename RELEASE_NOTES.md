# Release Notes

**Release Date:** November 18, 2025

**Commit:** 5b7ef35a056 (Merged in develop - PR #1385)

---

## Executive Summary

This release marks the merge of feature PS-1331 for unforfeit functionality on QPREV-PROF transactions, with comprehensive unit test coverage. Also includes various bug fixes and improvements to Master Inquiry, navigation refinements, and UI/UX enhancements focused on dynamic grid handling and duplicate name/birthday report fixes.

---

## Features & Enhancements

### Backend & Services
- **PS-1331:** Added unforfeit support for QPREV-PROF transactions with unit test coverage
- **PS-1976:** Fixed ORA-00979 error in GetYearsOfServiceQuery; refactored JWT token logging
- **Security Enhancement:** Implemented path validation utilities to prevent open redirect attacks
- **Audit Functionality:** Created audit search endpoint and service with time filters
- **Master Inquiry:** Enhanced dynamic grid height calculation and integration into member grid display

### Frontend UI & Reports
- **PS-2115 (Duplicate Names/Birthdays):** Initial screen implementation, sorting fixes, navigation status translation, and column definition updates
- **Master Inquiry UI:** Dynamic grid height improvements for better responsiveness
- **Navigation:** Menu updates in Inquiries and Adjustments sections
- **Dialog Enhancement:** Added dialog for larger text display

### Data & Configuration
- **Package Updates:** Updated package versions to stable releases in Directory.Packages.props
- **QPAY066B:** Added to Adhoc Report
- **Analyzers & Utilities:** Added new analyzers, utilities, and Swagger header processor
- **Masking:** Implemented masking JSON converter to preserve security

### Bug Fixes
- Fixed duplicate names and birthdays report display
- Corrected years of service query grouping logic
- Resolved Master Inquiry member grid display issues
- Fixed JWT token logging for security compliance

---

## Known Issues

### To Do - Critical & High Priority Bugs

**Employee Search & Data Quality:**
- **PS-2149:** `To Do` - Pure Beneficiaries incorrectly showing employee badges in contribution amount search; member details display and clicking badge causes spinning issue
- **PS-1969:** `To Do` - Missing "number of active employees" field in breakdown report totals response
- **PS-1887:** `To Do` - 2024 points earned calculation tracked against profit year 2023 instead of current year

**Report Display & Functionality:**
- **PS-1838:** `In Progress` - YTD Wages Extract showing "Live" data source instead of "Frozen" during fiscal close
- **PS-1889:** `To Do` - Store breakdown report missing terminated employees with balance but no vested percentage
- **PS-1931:** `To Do` - API not honoring search criteria for beneficiary allocation in QPAY066D

**Data Calculations & Vesting:**
- **PS-2123:** `To Do` - Master Inquiry showing incorrect vested amounts for employees who are both employee and beneficiary
- **PS-2122:** `To Do` - Terminations showing suggested forfeiture for already-forfeited beneficiary/employees
- **PS-2120:** `To Do` - Forfeiture amount showing as negative after saving suggested forfeit amount
- **PS-2121:** `To Do` - Forfeiture amount showing as negative after saving suggested forfeit amount (Clone)

**UI & Masking Issues:**
- **PS-2140:** `To Do` - PAY426 masking issues - leading zeros and NaN values; points not masked
- **PS-2113:** `To Do` - PAY426/426N masking problems; points unmasked, hours showing as NaN, leading zeros before masking
- **PS-2142:** `To Do` - YTD Wages Extract not loading in full page when rows per page changed to 200
- **PS-2143:** `To Do` - PAY426 Summary showing leading 0 in total wages and total balance for IT dev ops

**Grid & Navigation:**
- **PS-2139:** `In Progress` - Master Inquiry list of names header row not freezing during scroll
- **PS-2146:** `To Do` - QPAY066 employee count mismatch between top grid (557) and displayed records (464)

**Other Functional Issues:**
- **PS-2144:** `To Do` - Recently Terminated Employees sort by SSN returns error
- **PS-2136:** `To Do` - Account History Report validation error when searching by PSN in badge field
- **PS-2104:** `To Do` - Distribution Add Entry button order incorrect (Cancel showing before Continue)
- **PS-2103:** `To Do` - Distribution Add Entry showing "Failed to fetch member data" error

### In Progress Bugs

- **PS-1838:** `In Progress` - YTD Wages Extract data source indicator
- **PS-1883:** `In Progress` - Terminations - save button still enabled when status is "On Hold"
- **PS-2139:** `In Progress` - Master Inquiry list of names header row freezing

---

## Technical Improvements

### Code Quality & Organization
- Refactored JWT token logging to use range operator for substring extraction
- Enhanced MaskSensitive attribute implementation
- Code organization improvements across audit and search services
- Improved error handling for years of service calculations

### Security Enhancements
- Path validation utilities to prevent open redirect attacks
- Masking JSON converter for PII protection
- Enhanced JWT token handling and logging

### Testing & Validation
- Added comprehensive unit tests for PS-1331 unforfeit scenarios
- Improved audit search functionality with validation

---

## Migration Notes

- QPREV-PROF transactions now support unforfeit operations via PS-1331
- Years of service query updated to resolve ORA-00979 errors
- Master Inquiry grid height calculation now dynamic
- Security headers and path validation now enforced

---

## Contributors

Development and QA teams

---

## For Support

- Refer to specific Jira ticket numbers for detailed issue information
- Known Issues section lists all bugs by priority and category
- Review ticket status in Jira project PS for real-time updates

---

*This release includes the unforfeit functionality for QPREV-PROF transactions and various UI/UX improvements with focus on Master Inquiry enhancements and security hardening.*

---

## For Support

- Refer to specific Jira ticket numbers for detailed issue information
- Known Issues section lists all bugs by priority and category
- Review ticket status in Jira project PS for real-time updates

---

*This release prioritizes data accuracy, user experience improvements, and bug resolution. All major defects have been prioritized and tracked in the Known Issues section above.*
