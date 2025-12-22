---
applyTo: "src/ui/src/pages/DecemberActivities/ForfeituresAdjustment/**/*.*"
paths: "src/ui/src/pages/DecemberActivities/ForfeituresAdjustment/**/*.*"
---

# Forfeitures Adjustment

## Purpose

Enables administrators to search for employees and manually adjust their forfeiture amounts during December year-end activities. Users can:

- Search for employees by SSN or Badge number
- View employee details and existing forfeiture transactions
- Add new forfeiture entries (positive for forfeitures, negative for unforfeitures)
- Mark forfeitures as class action related

## Key Components

| File                                       | Responsibility                                |
| ------------------------------------------ | --------------------------------------------- |
| `ForfeituresAdjustment.tsx`                | Main page with `MissiveAlertProvider` context |
| `ForfeituresAdjustmentSearchFilter.tsx`    | Search form with SSN/Badge mutual exclusivity |
| `ForfeituresAdjustmentPanel.tsx`           | Action bar with "Add Forfeiture" button       |
| `ForfeituresTransactionGrid.tsx`           | AG Grid with forfeiture transactions          |
| `AddForfeitureModal.tsx`                   | Modal for entering new forfeiture amounts     |
| `hooks/useForfeituresAdjustment.ts`        | Main orchestration hook                       |
| `hooks/useForfeituresAdjustmentReducer.ts` | Reducer with actions and selectors            |

## State Management

### Custom Hook Pattern

**State Slices**: `search`, `memberDetails`, `transactions`, `modal`

**Action Types**: `SEARCH_START/SUCCESS/FAILURE`, `MEMBER_DETAILS_FETCH_*`, `TRANSACTIONS_FETCH_*`, `OPEN/CLOSE_ADD_FORFEITURE_MODAL`

## API Endpoints

| Hook                                    | Endpoint                                   | Purpose         |
| --------------------------------------- | ------------------------------------------ | --------------- |
| `useLazyGetForfeitureAdjustmentsQuery`  | GET /yearend/forfeiture-adjustments        | Search employee |
| `useUpdateForfeitureAdjustmentMutation` | PUT /yearend/forfeiture-adjustments/update | Save forfeiture |

## Key Patterns

- Mutual field exclusivity (SSN vs Badge)
- Progressive disclosure based on search state
- Read-only mode support via `useReadOnlyNavigation`
- Transaction filtering by `profitCodeId === 2`
