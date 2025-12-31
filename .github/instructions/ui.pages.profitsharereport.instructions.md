---
applyTo: "src/ui/src/pages/DecemberActivities/ProfitShareReport/**/*.*"
paths: "src/ui/src/pages/DecemberActivities/ProfitShareReport/**/*.*"
---

# ProfitShareReport

## Purpose

Provides a comprehensive view of year-end profit sharing data for employees. Features:

- Aggregated totals display
- Filtering by predefined report presets (PAY426-1 through PAY426-10)
- Drill-down into employee-level records
- Archive functionality when status is "Complete"

## Key Components

| File                                | Responsibility                                                |
| ----------------------------------- | ------------------------------------------------------------- |
| `ProfitShareReport.tsx`             | Main page with totals, preset selection, and archive handling |
| `ProfitShareReportSearchFilter.tsx` | Badge number search with Yup validation                       |
| `ProfitShareReportGrid.tsx`         | Paginated AG Grid with dynamic height                         |
| `ProfitShareReportGridColumns.ts`   | Column definitions using factory utilities                    |
| `CommitModal.tsx`                   | Confirmation modal for data commit                            |

## State Management

### Redux (Global State)

- `yearEndProfitSharingReportTotals`: Aggregated totals
- `yearEndProfitSharingReportLive`: Paginated employee records

### Local State

- `selectedPresetParams`: Current report preset (1-10)
- `currentSearchParams`: Active search parameters
- `isInitialSearchLoaded`: Grid loading gate

## API Endpoints

| Hook                                              | Purpose                            |
| ------------------------------------------------- | ---------------------------------- |
| `useLazyGetYearEndProfitSharingReportTotalsQuery` | Fetches aggregated totals          |
| `useLazyGetYearEndProfitSharingReportLiveQuery`   | Fetches paginated employee records |

## Key Patterns

- Progressive disclosure (totals → preset → filter → grid)
- Preset-based filtering (age ranges, hours thresholds, status)
- Smooth scroll navigation between sections
- Archive on status "Complete"
