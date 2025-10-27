# Forfeit Activities Shared Utilities

This directory contains shared utility functions for Forfeit-related Activities master-detail grid screens (Termination and UnForfeit).

## Files

### `gridDataHelpers.ts`

Shared utilities for grid data manipulation and row management.

**Key Functions:**

- `generateRowKey()` - Generate unique row keys for edit tracking

  - UnForfeit: uses `profitDetailId` (multiple forfeitures per badge/year possible)
  - Termination: uses composite key `${badgeNumber}-${profitYear}`

- `transformForfeitureValue()` - Transform values before saving

  - UnForfeit: negates value (positive UI → negative backend)
  - Termination: no transformation

- `getEditableFieldName()` - Get field name for activity type

  - UnForfeit: `'suggestedUnforfeiture'`
  - Termination: `'suggestedForfeit'`

- `isDetailRowEditable()` - Determine if a detail row is editable

  - UnForfeit: editable if suggested value is not null
  - Termination: editable only for current profit year

- `flattenMasterDetailData()` - Flatten master-detail structure for AG Grid

  - Generic implementation that works for both activity types

- `getEditedValue()` - Get current edited value with fallback to original

- `hasRowError()` - Check if a row has validation errors

### `saveOperationHelpers.ts`

Shared utilities for save operations, message generation, and error handling.

**Key Functions:**

- `prepareSaveRequest()` - Prepare a single save request with transformations

- `prepareBulkSaveRequests()` - Prepare multiple save requests

- `generateSaveSuccessMessage()` - Generate success message for individual save

- `generateBulkSaveSuccessMessage()` - Generate success message for bulk save

- `getRowKeysForRequests()` - Extract row keys from save requests

- `clearGridSelectionsForBadges()` - Clear AG Grid selections for saved items

- `getErrorMessage()` - Get error message for activity type and operation

## Usage Examples

### In `useUnForfeitGrid.ts`

```typescript
import {
  clearGridSelectionsForBadges,
  generateBulkSaveSuccessMessage,
  generateSaveSuccessMessage,
  getErrorMessage,
  getRowKeysForRequests,
  prepareBulkSaveRequests,
  prepareSaveRequest
} from "../utils/forfeitActivities/saveOperationHelpers";
import { flattenMasterDetailData, generateRowKey } from "../utils/forfeitActivities/gridDataHelpers";

// Configuration for shared utilities
const ACTIVITY_CONFIG = {
  activityType: "unforfeit" as const,
  rowKeyConfig: { type: "unforfeit" as const }
};

// Individual save
const handleSave = async (request, name) => {
  // Prepare request with value transformation
  const transformedRequest = prepareSaveRequest(ACTIVITY_CONFIG, request);
  await updateForfeitureAdjustment(transformedRequest);

  // Generate row key
  const rowKey = generateRowKey(ACTIVITY_CONFIG.rowKeyConfig, {
    badgeNumber: request.badgeNumber,
    profitYear: request.profitYear,
    profitDetailId: request.offsettingProfitDetailId
  });

  // Generate success message
  const successMessage = generateSaveSuccessMessage(ACTIVITY_CONFIG.activityType, name, request.forfeitureAmount);
};

// Bulk save
const handleBulkSave = async (requests, names) => {
  const transformedRequests = prepareBulkSaveRequests(ACTIVITY_CONFIG, requests);
  await updateForfeitureAdjustmentBulk(transformedRequests);

  const rowKeys = getRowKeysForRequests(ACTIVITY_CONFIG, requests);
  const bulkSuccessMessage = generateBulkSaveSuccessMessage(ACTIVITY_CONFIG.activityType, requests.length, names);
};

// Flatten grid data
const gridData = useMemo(() => {
  return flattenMasterDetailData(unForfeits.response.results, expandedRows, {
    getKey: (row) => row.badgeNumber.toString(),
    getDetails: (row) => row.details,
    hasDetails: (row) => Boolean(row.details && row.details.length > 0)
  });
}, [unForfeits, expandedRows]);
```

### In `useTerminationGrid.ts`

```typescript
// Same imports and similar usage pattern

// Configuration for Termination
const ACTIVITY_CONFIG = {
  activityType: "termination" as const,
  rowKeyConfig: { type: "termination" as const }
};

// All the same utility functions work, but with termination-specific behavior
// (no value negation, different row key strategy, different field names)
```
