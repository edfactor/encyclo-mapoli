# DuplicateSsnGuard Usage Examples

## Overview
The `DuplicateSsnGuard` component provides flexible handling of duplicate SSN scenarios with two modes:
- **Error Mode (default)**: Blocks page functionality when duplicates are detected
- **Warning Mode**: Shows a warning but allows page to remain usable

## Error Mode (Default - Blocking Behavior)

This is the current behavior that blocks the page when duplicates exist.

```tsx
import DuplicateSsnGuard from "../../components/DuplicateSsnGuard";

<DuplicateSsnGuard>
  {({ prerequisitesComplete }) => (
    <SearchAndReset
      handleReset={handleReset}
      handleSearch={validateAndSearch}
      isFetching={isFetching}
      disabled={!isValid || !prerequisitesComplete}  // Button is disabled when duplicates exist
    />
  )}
</DuplicateSsnGuard>
```

**Visual Result**: 
- Shows red error banner with error icon
- `prerequisitesComplete` is `false` when duplicates exist
- Typically used to disable critical actions

## Warning Mode (Non-Blocking Behavior)

This mode shows a warning but allows the page to function normally.

```tsx
import DuplicateSsnGuard from "../../components/DuplicateSsnGuard";

<DuplicateSsnGuard mode="warning">
  {({ prerequisitesComplete, hasDuplicates }) => (
    <>
      <SearchAndReset
        handleReset={handleReset}
        handleSearch={validateAndSearch}
        isFetching={isFetching}
        disabled={!isValid}  // prerequisitesComplete is always true in warning mode
      />
      {hasDuplicates && (
        <InfoMessage>Note: Results may be affected by duplicate SSNs in the system.</InfoMessage>
      )}
    </>
  )}
</DuplicateSsnGuard>
```

**Visual Result**:
- Shows orange warning banner with warning icon
- `prerequisitesComplete` is always `true` (page remains usable)
- `hasDuplicates` can be used for conditional rendering or additional warnings

## Advanced: Conditional Mode Based on User Role

```tsx
import DuplicateSsnGuard from "../../components/DuplicateSsnGuard";
import { useSelector } from "react-redux";
import { selectUserRole } from "../reduxstore/slices/authSlice";

const MyComponent = () => {
  const userRole = useSelector(selectUserRole);
  
  // Admins see warning, others get blocked
  const guardMode = userRole === "ADMINISTRATOR" ? "warning" : "error";

  return (
    <DuplicateSsnGuard mode={guardMode}>
      {({ prerequisitesComplete, hasDuplicates, refresh }) => (
        <div>
          {hasDuplicates && userRole === "ADMINISTRATOR" && (
            <button onClick={refresh}>
              Recheck for Duplicates
            </button>
          )}
          <SearchAndReset
            handleReset={handleReset}
            handleSearch={validateAndSearch}
            disabled={!prerequisitesComplete}
          />
        </div>
      )}
    </DuplicateSsnGuard>
  );
};
```

## Props Reference

### DuplicateSsnGuardProps

| Prop | Type | Default | Description |
|------|------|---------|-------------|
| `showAlert` | `boolean` | `true` | Whether to show the alert banner |
| `mode` | `"error" \| "warning"` | `"error"` | Determines blocking vs non-blocking behavior |
| `children` | `(ctx) => ReactNode` | required | Render prop function receiving context |

### Context Object (passed to children)

| Property | Type | Description |
|----------|------|-------------|
| `prerequisitesComplete` | `boolean` | `false` in error mode when duplicates exist, always `true` in warning mode |
| `hasDuplicates` | `boolean` | `true` when duplicate SSNs are detected (regardless of mode) |
| `refresh` | `() => void` | Function to re-query the duplicate check |

## When to Use Each Mode

### Use Error Mode (default) When:
- The operation is critical and data integrity is paramount
- Duplicate SSNs could cause incorrect calculations or processing
- Examples: Year-end calculations, financial reports, final distributions

### Use Warning Mode When:
- The operation is informational or read-only
- Duplicates are problematic but not immediately critical
- Users need to proceed despite known data issues
- Examples: Search/lookup operations, preliminary reports, data review screens

## Migration Guide

### Existing Components (No Changes Required)
All existing uses of `DuplicateSsnGuard` will continue to work with error mode (blocking behavior) by default:

```tsx
// This works exactly as before
<DuplicateSsnGuard>
  {({ prerequisitesComplete }) => (
    <Button disabled={!prerequisitesComplete}>Submit</Button>
  )}
</DuplicateSsnGuard>
```

### Adding Warning Mode
To convert a page to warning mode, simply add the `mode` prop:

```tsx
// Add mode="warning" to allow page usage
<DuplicateSsnGuard mode="warning">
  {({ prerequisitesComplete, hasDuplicates }) => (
    <Button disabled={false}>Submit</Button>  // Always enabled
  )}
</DuplicateSsnGuard>
```

### Using the hasDuplicates Flag
The new `hasDuplicates` flag allows conditional logic regardless of mode:

```tsx
<DuplicateSsnGuard mode="warning">
  {({ hasDuplicates }) => (
    <div>
      <DataGrid data={results} />
      {hasDuplicates && (
        <Typography color="warning.main">
          ⚠️ Some results may be affected by duplicate SSNs
        </Typography>
      )}
    </div>
  )}
</DuplicateSsnGuard>
```
