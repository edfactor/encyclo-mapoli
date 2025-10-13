# Validation Hooks

Reusable React hooks for fetching and managing checksum validation data in the Profit Sharing application.

## Overview

These hooks provide a clean, reusable way to fetch validation data from the API and make it available to components. They decouple validation logic from page components and enable independent validation fetching.

## Available Hooks

### `useChecksumValidation`

**Comprehensive validation hook** that fetches all cross-reference validation data for a profit year.

```typescript
const {
  validationData,
  isLoading,
  error,
  refetch,
  getFieldValidation,
  getValidationGroup,
  isAllValid
} = useChecksumValidation({
  profitYear: 2024,
  autoFetch: true,
  onValidationLoaded: (data) => console.log("Loaded:", data),
  onError: (err) => console.error("Error:", err)
});
```

**Returns:**
- `validationData`: Complete validation response with all groups (contributions, earnings, forfeitures, distributions, ALLOC transfers)
- `isLoading`: Boolean indicating if data is being fetched
- `error`: Error message string or null
- `refetch()`: Function to manually trigger a new fetch
- `getFieldValidation(fieldName)`: Helper to find a specific field validation
- `getValidationGroup(groupName)`: Helper to get all validations for a group
- `isAllValid()`: Check if all validations pass (no critical issues)

**Validation Groups Included:**
1. **PAY443/PAY444 Contributions** - TotalContributions field
2. **PAY443/PAY444 Earnings** - TotalEarnings field  
3. **PAY443/PAY444 Forfeitures** - TotalForfeitures field
4. **PAY443/PAY444 Distributions** - DistributionTotals field
5. **ALLOC/PAID ALLOC Transfers** - IncomingAllocations, OutgoingAllocations, NetAllocTransfer

### `useBalanceValidation`

**Focused hook** for ALLOC/PAID ALLOC transfer validation only.

```typescript
const { validationData, isLoading, error, refetch } = useBalanceValidation(2024);
```

**Returns:**
- `validationData`: Just the ALLOC transfer validation group
- `isLoading`: Boolean indicating if data is being fetched
- `error`: Error message string or null
- `refetch()`: Function to manually trigger a new fetch

**Fields in ALLOC Validation:**
- `IncomingAllocations`: ALLOC transfers (code 6) - Contribution field
- `OutgoingAllocations`: PAID ALLOC transfers (code 5) - Forfeiture field  
- `NetAllocTransfer`: Sum of both (should equal $0.00)

## Usage Examples

### Basic Usage

```tsx
import { useChecksumValidation } from "../../hooks";

const MyComponent = () => {
  const { validationData, isLoading } = useChecksumValidation({
    profitYear: 2024,
    autoFetch: true
  });

  if (isLoading) return <CircularProgress />;

  return <div>{/* Use validationData */}</div>;
};
```

### With Manual Refresh

```tsx
const MyComponent = () => {
  const { validationData, refetch, isLoading } = useChecksumValidation({
    profitYear: 2024,
    autoFetch: false // Don't fetch automatically
  });

  return (
    <div>
      <Button onClick={refetch} disabled={isLoading}>
        {isLoading ? "Loading..." : "Load Validation"}
      </Button>
      {/* Use validationData */}
    </div>
  );
};
```

### Finding Specific Field Validations

```tsx
const { getFieldValidation } = useChecksumValidation({
  profitYear: 2024,
  autoFetch: true
});

// Get specific field
const allocValidation = getFieldValidation("NetAllocTransfer");

if (allocValidation) {
  return (
    <div>
      <InfoIcon color={allocValidation.isValid ? "success" : "warning"} />
      <span>{allocValidation.message}</span>
    </div>
  );
}
```

### Getting Validation Groups

```tsx
const { getValidationGroup } = useChecksumValidation({
  profitYear: 2024,
  autoFetch: true
});

const allocGroup = getValidationGroup("ALLOC/PAID ALLOC Transfers");

if (allocGroup) {
  return (
    <div>
      <h3>{allocGroup.groupName}</h3>
      <p>Priority: {allocGroup.priority}</p>
      <ul>
        {allocGroup.validations.map((v) => (
          <li key={v.fieldName}>
            {v.fieldName}: {v.isValid ? "✅" : "❌"}
          </li>
        ))}
      </ul>
    </div>
  );
}
```

### Checking Overall Validation Status

```tsx
const { isAllValid } = useChecksumValidation({
  profitYear: 2024,
  autoFetch: true
});

return (
  <Alert severity={isAllValid() ? "success" : "warning"}>
    {isAllValid() 
      ? "All validations passed!" 
      : "Some validations failed. Review details."}
  </Alert>
);
```

### ALLOC-Only Validation

```tsx
import { useBalanceValidation } from "../../hooks";

const AllocStatus = ({ year }) => {
  const { validationData, isLoading } = useBalanceValidation(year);

  if (isLoading) return <Skeleton />;
  if (!validationData) return <div>No ALLOC validation data</div>;

  const netTransfer = validationData.validations.find(
    (v) => v.fieldName === "NetAllocTransfer"
  );

  return (
    <Chip
      label={netTransfer?.isValid ? "Balanced" : "Imbalanced"}
      color={netTransfer?.isValid ? "success" : "error"}
    />
  );
};
```

## Integration with Existing Components

### Refactoring ProfitShareEditUpdate

Replace the manual validation state management:

**Before:**
```tsx
const [validationResponse, setValidationResponse] = useState(null);

// Manual setting from props
useEffect(() => {
  setValidationResponse(profitSharingUpdate.crossReferenceValidation);
}, [profitSharingUpdate]);
```

**After:**
```tsx
const { 
  validationData: validationResponse,
  refetch: refetchValidation
} = useChecksumValidation({
  profitYear: profitSharingUpdate?.profitYear || 0,
  autoFetch: true
});
```

The existing `getFieldValidation` helper can remain unchanged, or you can use the hook's built-in helper directly.

## API Endpoints

### Checksum Validation (All Groups)
```
GET /api/validation/checksum/master-update/{profitYear}
```

Returns `MasterUpdateCrossReferenceValidationResponse` with all validation groups.

### Balance Validation (ALLOC Only)
```
GET /api/balance-validation/alloc-transfers/{profitYear}
```

Returns `CrossReferenceValidationGroup` with just ALLOC transfer validations.

## Error Handling

Both hooks handle errors gracefully:

```tsx
const { error, validationData } = useChecksumValidation({
  profitYear: 2024,
  autoFetch: true,
  onError: (err) => {
    // Handle error (e.g., show toast notification)
    toast.error(`Validation failed: ${err}`);
  }
});

// Also check error state directly
if (error) {
  return <Alert severity="error">{error}</Alert>;
}
```

404 responses are handled specially and don't generate errors - they simply result in `validationData: null`.

## Performance Considerations

- **Auto-fetch is debounced**: Changes to `profitYear` trigger a new fetch after dependency changes
- **Manual control**: Set `autoFetch: false` to manually control when data is fetched
- **Caching**: Consider adding React Query or SWR for automatic caching (future enhancement)

## Testing

```tsx
import { renderHook, waitFor } from "@testing-library/react";
import { useChecksumValidation } from "./useChecksumValidation";

describe("useChecksumValidation", () => {
  it("should fetch validation data", async () => {
    const { result } = renderHook(() =>
      useChecksumValidation({
        profitYear: 2024,
        autoFetch: true
      })
    );

    await waitFor(() => {
      expect(result.current.validationData).not.toBeNull();
    });
  });
});
```

## Future Enhancements

- [ ] Add React Query integration for automatic caching and refetching
- [ ] Add WebSocket support for real-time validation updates
- [ ] Add validation result caching to localStorage
- [ ] Add optimistic updates for better UX
- [ ] Add retry logic for failed requests
- [ ] Add request cancellation on component unmount

## Related Files

- `src/types/validation/cross-reference-validation.ts` - TypeScript types
- `src/pages/ProfitShareEditUpdate/ProfitShareEditUpdate.tsx` - Main consumer
- `src/services/src/.../ValidateAllocTransfersEndpoint.cs` - ALLOC endpoint
- `src/services/src/.../ValidateMasterUpdateCrossReferencesEndpoint.cs` - Checksum endpoint
