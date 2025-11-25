# Release Notes - November 2025

**Release Date:** November 25, 2025

---

## What's Changing?

This release focuses on **improving system reliability, simplifying operations, and enhancing user experience**. We've removed outdated features, consolidated reports for easier access, and improved how the system handles employee information.

**Key improvements:**

- Simplified employee record handling
- Consolidated report pages for better organization
- Removed outdated PDF export option (use print or download instead)
- Improved system performance
- Cleaner, more intuitive user interface

---

## What's New & What's Different

### Employee Information Management

- **How employee names are displayed:** Employee names are now calculated more efficiently by the system. This change is invisible to you but improves system performance.
- **No action needed:** This change happens automatically; no changes to how you use the system.

### Reports & Data Export

- **Account History Reports:**
  - ✅ **Still available** - You can still access all account history data
  - ✅ **Better performance** - Reports load faster and are easier to search
  - ❌ **PDF export removed** - Instead, use your browser's print function or download as Excel
- **QPAY066 Reports (Employee Distribution Reports):**
  - ✅ **Consolidated** - All QPAY066 report variations now available in one unified location
  - ✅ **Easier to find** - No need to hunt for different report types
  - ✅ **Better filtering options** - Find the data you need faster

### Master Inquiry (Employee Search)

- **Improved interface** - Faster searches and better organized information
- **Better responsiveness** - Works more smoothly across different screen sizes
- **Clearer data display** - Member information is easier to read and understand

### State Lookup

- **What changed:** The state dropdown now displays state data instantly without database queries
- **Why:** Reduces system load and makes the interface more responsive
- **What you'll notice:** State selection is now instant - no waiting

### System Behind-the-Scenes Improvements

- **Cleaned up obsolete code** - Removed 313 old or unused files
- **Better organization** - Simplified how the system is structured
- **Faster operations** - Optimized database queries and system processes
- **Improved audit trails** - Better tracking of system changes for compliance

---

## What You Need to Do

### For Most Users

**Nothing!** This update requires no action from end users. All changes are automatic.

### For IT/System Administrators

**Before deploying this update:**

1. Back up your current database (standard procedure)
2. Review the "What's Changing" section above
3. Allow time for database updates to complete during deployment
4. No special configuration changes needed

**After deployment:**

- Clear your browser cache (Ctrl+Shift+Delete) for best performance
- Test the following areas:
  - Employee lookup/Master Inquiry
  - Account History reports
  - QPAY066 reports (now in one consolidated location)
  - State dropdown (should load instantly)

### For Report Users

- **Account History Reports:** Still available and working the same way, just faster
- **Print or Export:** If you need a file, use your browser's print function or "Export to Excel"
- **QPAY066 Reports:** All variations are now in one place - no more hunting for different report types

---

## Known Issues Being Worked On

We're tracking and working on the following issues. If you experience any of these, please contact IT Support with your specific situation.

### Data & Search Issues

- Employee search showing incorrect badge information in some cases
- Missing employee counts in certain breakdown reports
- Incorrect profit year tracking in points calculation

### Report & Display Issues

- YTD Wages showing "Live" instead of "Frozen" data during fiscal close
- Some employee records missing from store breakdown reports
- QPAY066 report employee counts not matching between different views
- Employee count mismatches in reports

### Account History Report Fixes (PS-2160)

- **Fixed:** Account History Report now uses internal report ID instead of badge number for member details lookups
- **Issue:** When multiple badge numbers existed for a member, the old logic could return duplicate or incorrect records
- **Solution:** Implemented ID-based extraction logic that uniquely identifies member records
- **Testing:** Added comprehensive unit tests validating ID extraction and deduplication
- **Status:** ✅ Complete and merged

### System Issues

- Master Inquiry header row not staying frozen while scrolling
- Distribution entry button order incorrect
- Distribution entry sometimes shows "Failed to fetch" error
- Recently Terminated Employees sort by SSN causing errors
- Account History search showing validation error for some badge formats
- PAY426 reports showing display formatting issues

### In Development

- Various formatting and display improvements being finalized

**Need Help?** Contact IT Support with:

- Which page or report you're using
- What you were trying to do
- Any error messages you see

---

## Migration & Upgrade Guide

### Database Changes

1. **EF Core Migrations Applied:**

   - `20251006232422_initialMigration` - Consolidated initial schema
   - `20251007111734_allowNegativeYearsOfService` - Support negative years of service
   - `20251010011517_AddProfitYearSsnIndex` - Performance index added
   - `20251016154000_addForfeitAdministrative` - Forfeit administrative fields
   - `20251023154537_OutstandingMigrations` - Outstanding record handling
   - `20251023154839_AddManualCheckNumber` - Manual check number support
   - `20251113131524_AddComputedColumnsInPayProfit` - Computed columns for pay profit
   - `20251117151804_AddChangesHashToAuditEvent` - Audit event hash tracking

2. **No manual schema changes required** - All changes applied via migrations

### API Changes

- **Removed Endpoints:**
  - `DELETE /api/reports/account-history/pdf` - PDF export endpoint removed
- **Updated Endpoints:**
  - GET `/api/lookups/states` - Now returns static data (no longer queries database)
  - GET `/api/reports/account-history` - Enhanced ID extraction
  - GET `/api/reports/qpay066-adhoc` - Unified QPAY066 reporting endpoint (replaces `qpay066x`)
- **No Breaking Changes:** All response contracts maintained or improved

### Frontend Changes

- **Navigation Updates:**
  - QPAY066x reports consolidated to QPAY066 Ad Hoc Reports
  - Menu structure cleaned up
- **Component Removals:**
  - `QPAY066xAdHocReports` - Use new `QPAY066AdHocReports` instead
- **Component Updates:**
  - `MasterInquiry` - Improved grid and filter handling
  - All search filters - Standardized structure and behavior

### Deployment Notes

1. Run database migrations before deploying updated API
2. Clear browser cache due to UI component updates
3. No worker role changes required
4. No configuration changes required
5. Service layer fully backward compatible

---

## Technical Improvements

### Architecture Cleanup

- Removed tight coupling between reporting and PDF generation
- Separated state lookup logic to simple configuration
- Consolidated report endpoints for better API consistency
- Improved service layer organization with focused responsibilities

### Code Quality

- Reduced technical debt through removal of unused code paths
- Improved test coverage organization
- Enhanced error handling patterns
- Cleaner DTOs without database-specific computed columns
- Better separation of concerns in Frontend components

### Performance Optimization

- Added profit year SSN index for faster lookups
- Improved query patterns in services
- Reduced unnecessary computed columns
- Optimized migration structure for faster deployments

### Security Enhancements

- Improved telemetry and masking patterns
- Enhanced audit trail with changes hash
- Better error message handling
- Removed unnecessary PDF generation vectors

---

## Questions or Problems?

**Report Issues?**

- Account History not working as expected
- QPAY066 reports missing
- Master Inquiry slow or not loading
- Contact: IT Support

**Specific Issues?**

- Check the "Known Issues Being Worked On" section above
- Contact IT Support with details about what you were doing

**Training?**

- System works the same way as before
- Reports moved to better locations
- All features still available

---

## Summary

This update makes the profit sharing system more reliable, faster, and easier to use. It requires no action from most users - everything works automatically. IT staff should review the deployment notes before installing this update.

**What to expect:**

- ✅ Faster system performance
- ✅ Better organized reports
- ✅ Same functionality you're used to
- ✅ More reliable employee information handling

---

_For technical details about this release, contact your development team._
