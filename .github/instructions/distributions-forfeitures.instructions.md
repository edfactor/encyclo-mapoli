---
applyTo: "src/ui/src/pages/DecemberActivities/DistributionsAndForfeitures/**/*.*"
paths: "src/ui/src/pages/DecemberActivities/DistributionsAndForfeitures/**/*.*"
---

# Distributions and Forfeitures

## Purpose

This feature provides a searchable report of profit-sharing distributions and forfeitures for a given profit year. It is part of the December Activities workflow, allowing users to:

- View distribution and forfeiture transactions within a date range
- Filter by state and tax code
- See aggregated totals for distributions, state taxes, federal taxes, and forfeitures
- View detailed breakdowns of state taxes by state and forfeitures by type

## Key Components

| File                                         | Responsibility                                       |
| -------------------------------------------- | ---------------------------------------------------- |
| `DistributionAndForfeitures.tsx`             | Main page component with collapsible filter section  |
| `DistributionAndForfeituresSearchFilter.tsx` | Search form with date range and multi-select filters |
| `DistributionAndForfeituresGrid.tsx`         | AG Grid with summary totals and interactive tooltips |
| `DistributionAndForfeituresGridColumns.ts`   | Column definitions using shared factory functions    |

## State Management

### Redux (Global State)

- `distributionsAndForfeitures`: API response data (results + totals)
- `distributionsAndForfeituresQueryParams`: Current filter parameters

### Local State

- `initialSearchLoaded`: Coordinates initial search trigger between filter and grid
- Pagination state managed by `useGridPagination` hook

## API Endpoints

| Endpoint                                     | Method | Purpose                                     |
| -------------------------------------------- | ------ | ------------------------------------------- |
| `/api/yearend/distributions-and-forfeitures` | POST   | Fetches paginated transactions with filters |
| `/api/lookups/states`                        | GET    | State dropdown options                      |
| `/api/lookups/taxcodes`                      | GET    | Tax code dropdown options                   |

## Key Patterns

- Month picker date expansion (first/last day of month)
- "All" selection pattern for multi-selects
- Tooltip breakdowns for State Tax and Forfeitures
