# Frontend Input Form Validation Guide

## Overview

This application uses **React Hook Form** with **Yup** schema validation to provide comprehensive client-side form validation. The validation architecture combines declarative schema definitions with controlled Material-UI components for a robust user experience.

## ⚠️ MANDATORY: Yup Validation Required

**ALL FORM COMPONENTS MUST USE YUP FOR VALIDATION. NO EXCEPTIONS.**

- Custom validation logic MUST be implemented in Yup schemas
- Manual validation (e.g., checking `if (!value)`) is NOT allowed
- All form inputs MUST be wrapped in `Controller` with Yup schema validation
- Direct use of `useState` for form fields is NOT allowed - use React Hook Form

## Core Technology Stack

- **React Hook Form**: Form state management and validation orchestration (REQUIRED)
- **Yup**: Schema-based validation rules (REQUIRED)
- **@hookform/resolvers/yup**: Integration bridge between React Hook Form and Yup (REQUIRED)
- **Material-UI (MUI)**: UI components with built-in error display capabilities

## Validation Architecture

### 1. Schema Definition with Yup

All form validation begins with a Yup schema that defines the validation rules for each form field:

```typescript
import * as yup from "yup";
import {
  ssnValidator,
  badgeNumberValidator,
  monthValidator,
} from "../../utils/FormValidators";

const schema = yup.object().shape({
  socialSecurity: ssnValidator,
  badgeNumber: badgeNumberValidator,
  name: yup.string().nullable(),
  startMonth: monthValidator,
  endMonth: monthValidator.min(
    yup.ref("startMonth"),
    "End month must be after start month"
  ),
  contribution: yup
    .number()
    .typeError("Contribution must be a number")
    .min(0, "Contribution must be a positive number")
    .nullable(),
});
```

**Key patterns:**

- **Reusable validators** from `utils/FormValidators.ts` (SSN, badge numbers, dates, etc.)
- **Cross-field validation** using `yup.ref()` to compare fields (e.g., end date after start date)
- **Custom test methods** for complex business logic
- **Nullable fields** with `.nullable()` for optional inputs
- **Transform methods** to convert empty strings to `null` or `undefined`

### 2. Form Initialization with useForm Hook

React Hook Form's `useForm` hook integrates the Yup schema and manages form state:

```typescript
import { useForm, Controller } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";

const {
  control,
  handleSubmit,
  formState: { errors, isValid },
  reset,
  setValue,
  watch,
} = useForm<FormDataType>({
  resolver: yupResolver(schema) as Resolver<FormDataType>,
  mode: "onBlur", // Validate on blur events
  defaultValues: {
    socialSecurity: undefined,
    badgeNumber: undefined,
    name: undefined,
    // ... other defaults
  },
});
```

**Configuration options:**

- **resolver**: Connects Yup schema to form validation
- **mode**: `"onBlur"` validates when user leaves field (less intrusive than `"onChange"`)
- **defaultValues**: Initial form state and reset values

### 3. Controller Component Pattern

Each input field is wrapped in a `Controller` component to connect React Hook Form with Material-UI components:

```typescript
<Controller
  name="socialSecurity"
  control={control}
  render={({ field }) => (
    <TextField
      {...field}
      fullWidth
      size="small"
      variant="outlined"
      value={field.value ?? ""}
      error={!!errors.socialSecurity}
      onChange={(e) => {
        const value = e.target.value;
        // Custom input validation
        if (value !== "" && !/^\d*$/.test(value)) {
          return; // Block non-numeric input
        }
        if (value.length > 9) {
          return; // Block input beyond max length
        }
        const parsedValue = value === "" ? null : value;
        field.onChange(parsedValue);
      }}
    />
  )}
/>;
{
  errors.socialSecurity && (
    <FormHelperText error>{errors.socialSecurity.message}</FormHelperText>
  );
}
```

**Key patterns:**

- **`{...field}`**: Spreads `onChange`, `onBlur`, `value`, `name`, and `ref` to the input
- **`value={field.value ?? ""}`**: Prevents uncontrolled input warnings for null/undefined values
- **`error={!!errors.fieldName}`**: Shows error styling when validation fails
- **Custom `onChange`**: Implements real-time input restrictions (numeric-only, max length, etc.)
- **`FormHelperText`**: Displays validation error messages below the input

### 4. Input Restrictions and Sanitization

Many fields implement **preventative validation** in the `onChange` handler to block invalid input before it reaches the schema:

```typescript
onChange={(e) => {
  const value = e.target.value;

  // Only allow numeric input
  if (value !== "" && !/^\d*$/.test(value)) {
    return; // Block the input
  }

  // Prevent input beyond 9 characters
  if (value.length > 9) {
    return; // Block the input
  }

  const parsedValue = value === "" ? null : value;
  field.onChange(parsedValue);
}}
```

**Common patterns:**

- **SSN fields**: Only digits, max 9 characters
- **Badge numbers**: Only digits, max 7-11 characters depending on type
- **Numeric fields**: Parse to number, handle empty as null
- **Date fields**: Format validation before accepting input

### 5. Form Submission with Validation

Form submission is handled through `handleSubmit`, which automatically validates the entire form:

```typescript
const onSubmit = (data: FormDataType) => {
  // Transform form data to API request format
  const searchParams: SearchRequest = {
    badgeNumber: data.badgeNumber ?? undefined,
    socialSecurity: data.socialSecurity
      ? Number(data.socialSecurity)
      : undefined,
    name: data.name ?? undefined,
    // ... other field transformations
  };

  onSearch(searchParams);
};

const validateAndSubmit = handleSubmit(onSubmit);

// In the form
<form onSubmit={validateAndSubmit}>{/* form fields */}</form>;
```

**Flow:**

1. User clicks submit button
2. `handleSubmit` triggers validation against Yup schema
3. If invalid, errors are set and submission is blocked
4. If valid, `onSubmit` callback is executed with validated data
5. Form data is transformed to API request format

### 6. Search Button State Management

The search button is typically disabled until the form is valid and has sufficient criteria:

```typescript
<SearchAndReset
  handleReset={handleReset}
  handleSearch={validateAndSubmit}
  isFetching={isSearching}
  disabled={!isValid || isSearching || !hasSearchCriteria}
/>
```

**Disabled conditions:**

- `!isValid`: Form has validation errors
- `isSearching`: Request is in progress
- `!hasSearchCriteria`: No search criteria entered (optional, page-specific)

### 7. Reset Functionality

Reset handlers clear form state and related Redux state:

```typescript
const handleReset = () => {
  // Clear Redux state
  dispatch(clearSearchData());
  dispatch(clearSearchParams());

  // Reset form to default values
  reset({
    socialSecurity: undefined,
    badgeNumber: undefined,
    name: undefined,
    // ... other defaults
  });

  // Notify parent component
  onReset();
};
```

## Reusable Validators (utils/FormValidators.ts)

The project maintains a library of reusable validators to ensure consistency:

### SSN Validator

```typescript
export const ssnValidator = yup
  .string()
  .nullable()
  .test("is-9-digits", "SSN must be exactly 9 digits", function (value) {
    if (!value) return true;
    return /^\d{9}$/.test(value);
  })
  .transform((value) => value || undefined);
```

### Badge Number Validator

```typescript
export const badgeNumberValidator = yup
  .number()
  .typeError("Badge Number must be a number")
  .integer("Badge Number must be an integer")
  .min(1, "Badge Number must be at least 1")
  .max(9999999, "Badge Number must be 7 digits or less")
  .nullable()
  .transform((value) => value || undefined);
```

### Month Validator

```typescript
export const monthValidator = yup
  .number()
  .typeError("Month must be a number")
  .integer("Month must be an integer")
  .min(1, "Month must be between 1 and 12")
  .max(12, "Month must be between 1 and 12")
  .nullable();
```

### Positive Number Validator (Factory Function)

```typescript
export const positiveNumberValidator = (fieldName: string) =>
  yup
    .number()
    .typeError(`${fieldName} must be a number`)
    .min(0, `${fieldName} must be a positive number`)
    .nullable()
    .transform((value) => (isNaN(value) ? null : value));
```

### Profit Year Validator

```typescript
export const profitYearNullableValidator = yup
  .number()
  .typeError("Year must be a number")
  .integer("Year must be an integer")
  .min(2020, "Year must be 2020 or later")
  .max(2100, "Year must be 2100 or earlier")
  .nullable();
```

### Cross-Field Date Validators

```typescript
export const endDateAfterStartDateValidator = (
  startFieldName: string,
  errorMessage?: string
) =>
  yup
    .date()
    .nullable()
    .test(
      "is-after-start",
      errorMessage || "End Date must be after Start Date",
      function (value) {
        const startDate = this.parent[startFieldName];
        if (!startDate || !value) return true;
        return value > startDate;
      }
    );
```

## Advanced Validation Patterns

### Cross-Field Dependencies

Validate one field based on another field's value:

```typescript
const schema = yup.object().shape({
  endProfitYear: profitYearNullableValidator.test(
    "greater-than-start",
    "End year must be after start year",
    function (endYear) {
      const startYear = this.parent.startProfitYear;
      return !startYear || !endYear || endYear >= startYear;
    }
  ),
  startProfitYear: profitYearNullableValidator,
});
```

### Conditional Field Disabling

Disable fields based on other field values using `useWatch`:

```typescript
// Watch values
const badgeNumberValue = useWatch({ control, name: "badgeNumber" });
const ssnValue = useWatch({ control, name: "socialSecurity" });
const nameValue = useWatch({ control, name: "name" });

// Compute disabled states (mutually exclusive fields)
const hasBadge = badgeNumberValue !== null && badgeNumberValue !== undefined;
const hasSSN = ssnValue !== null && ssnValue !== undefined;
const hasName = nameValue !== null && nameValue !== undefined;

const isBadgeDisabled = hasSSN || hasName;
const isSSNDisabled = hasBadge || hasName;
const isNameDisabled = hasBadge || hasSSN;

// Apply to TextField
<TextField {...field} disabled={isBadgeDisabled} />;
```

### Dynamic Search Criteria Detection

Determine if sufficient search criteria has been entered:

```typescript
const hasSearchCriteria = useMemo(() => {
  const hasFieldValues =
    hasValue(watchedBadgeNumber) ||
    hasValue(watchedSSN) ||
    hasValue(watchedName);

  const hasNonDefaultSelections =
    watchedMemberType !== "all" || watchedPaymentType !== "all";

  return hasFieldValues || hasNonDefaultSelections;
}, [
  watchedBadgeNumber,
  watchedSSN,
  watchedName,
  watchedMemberType,
  watchedPaymentType,
]);
```

### Auto-Update Related Fields

Update one field when another changes:

```typescript
const handleBadgeNumberChange = (e: React.ChangeEvent<HTMLInputElement>) => {
  const badgeStr = e.target.value;
  let memberType: string;

  if (badgeStr.length === 0) {
    memberType = "all";
  } else if (badgeStr.length >= 8) {
    memberType = "beneficiaries";
  } else {
    memberType = "employees";
  }

  setValue("memberType", memberType);
};

// In TextField onChange
onChange={(e) => {
  field.onChange(e.target.value);
  handleBadgeNumberChange(e);
}}
```

## Select Component Pattern

Select dropdowns use a similar Controller pattern:

```typescript
<Controller
  name="memberType"
  control={control}
  render={({ field }) => (
    <Select
      {...field}
      fullWidth
      size="small"
      value={field.value ?? ""}
      error={!!errors.memberType}
      onChange={(e) => field.onChange(e.target.value)}
    >
      <MenuItem value="">
        <em>None</em>
      </MenuItem>
      <MenuItem value="1">Employees</MenuItem>
      <MenuItem value="2">Beneficiaries</MenuItem>
    </Select>
  )}
/>
```

## Checkbox Pattern

Checkboxes use a boolean value:

```typescript
<Controller
  name="voids"
  control={control}
  render={({ field }) => (
    <FormControlLabel
      control={
        <Checkbox
          {...field}
          checked={field.value ?? false}
          onChange={field.onChange}
        />
      }
      label="Include Voids"
    />
  )}
/>
```

## Radio Group Pattern

Radio buttons for mutually exclusive options:

```typescript
<Controller
  name="paymentType"
  control={control}
  render={({ field }) => (
    <RadioGroup {...field} row>
      <FormControlLabel
        value="all"
        control={<Radio size="small" />}
        label="All"
      />
      <FormControlLabel
        value="hardship"
        control={<Radio size="small" />}
        label="Hardship"
      />
      <FormControlLabel
        value="payoffs"
        control={<Radio size="small" />}
        label="Payoffs"
      />
    </RadioGroup>
  )}
/>
```

## Date Picker Pattern

Custom `DsmDatePicker` component integration:

```typescript
<Controller
  name="profitYear"
  control={control}
  render={({ field }) => (
    <DsmDatePicker
      id="profit-year"
      onChange={(value: Date | null) =>
        field.onChange(value?.getFullYear() || undefined)
      }
      value={field.value ? new Date(field.value, 0) : null}
      required={true}
      label="Profit Year"
      disableFuture
      views={["year"]}
      minDate={new Date(2020, 0)}
      error={errors.profitYear?.message}
    />
  )}
/>;
{
  errors.profitYear && (
    <FormHelperText error>{errors.profitYear.message}</FormHelperText>
  );
}
```

## Error Display Pattern

Consistent error messaging below each field:

```typescript
{
  errors?.fieldName && (
    <FormHelperText error>{errors.fieldName.message}</FormHelperText>
  );
}
```

## Best Practices

### 1. **MANDATORY: Always Use Yup for All Form Validation**

- **REQUIRED**: Define a Yup schema for every form
- **REQUIRED**: Use `yupResolver` to connect schema to `useForm`
- **FORBIDDEN**: Manual validation checks (e.g., `if (!name.trim())` in submit handlers)
- **FORBIDDEN**: Using `useState` for form field management
- All validation rules MUST be declared in the Yup schema

### 2. **Always Use Controller for MUI Components**

Never use `register()` directly with Material-UI components always use `Controller` for proper integration.

### 3. **Preventative + Declarative Validation**

Combine real-time input restrictions (`onChange` blocking) with schema validation for best UX:

- **Preventative**: Block invalid characters immediately
- **Declarative**: Validate complete field on blur with detailed error messages

### 4. **Null vs Undefined Handling**

- Use `value={field.value ?? ""}` to prevent uncontrolled input warnings
- Transform empty strings to `null` or `undefined` in `onChange` for consistent API payloads
- Use `.nullable()` in schemas for optional fields

### 5. **Reuse Validators**

Always check `utils/FormValidators.ts` before creating new validation logic. Reuse existing validators for consistency.

### 6. **Validation Mode Selection**

- **`"onBlur"`**: Recommended for most forms validates when user leaves field
- **`"onChange"`**: Use sparingly can be intrusive but useful for real-time feedback
- **`"onSubmit"`**: Validates only on submit use for simple forms

### 7. **Error Message Clarity**

Write user-friendly error messages that explain what's wrong and how to fix it:

- "SSN must be exactly 9 digits"
- "Badge Number must be 7 digits or less"
- L "Invalid input"

### 8. **Type Safety**

Define TypeScript interfaces for form data that match your schema shape:

```typescript
interface SearchFormData {
  badgeNumber?: number | null;
  socialSecurity?: string | null;
  name?: string | null;
  memberType: "all" | "employees" | "beneficiaries";
}

const schema = yup.object().shape({
  // ... schema matches interface
});

useForm<SearchFormData>({
  resolver: yupResolver(schema),
});
```

### 9. **Transform Data for API**

Separate form state from API request format. Transform data in `onSubmit`:

```typescript
const onSubmit = (data: SearchFormData) => {
  const apiRequest: ApiSearchRequest = {
    badge_number: data.badgeNumber ?? undefined,
    ssn: data.socialSecurity ? Number(data.socialSecurity) : undefined,
    full_name: data.name ?? undefined,
    member_type_code:
      data.memberType === "all" ? undefined : Number(data.memberType),
  };

  apiCall(apiRequest);
};
```

## Complete Example: Search Filter Component

```typescript
import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, FormLabel, Grid, TextField } from "@mui/material";
import { Controller, useForm } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import { ssnValidator, badgeNumberValidator } from "../../utils/FormValidators";

// 1. Define schema
const schema = yup.object().shape({
  badgeNumber: badgeNumberValidator,
  name: yup.string().nullable(),
  socialSecurity: ssnValidator,
});

// 2. Define TypeScript interface
interface SearchFormData {
  badgeNumber?: number | null;
  name?: string | null;
  socialSecurity?: string | null;
}

// 3. Component props
interface SearchFilterProps {
  onSearch: (params: SearchRequest) => void;
  isSearching?: boolean;
}

const SearchFilter: React.FC<SearchFilterProps> = ({
  onSearch,
  isSearching = false,
}) => {
  // 4. Initialize form
  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
  } = useForm<SearchFormData>({
    resolver: yupResolver(schema) as any,
    mode: "onBlur",
    defaultValues: {
      badgeNumber: undefined,
      name: undefined,
      socialSecurity: undefined,
    },
  });

  // 5. Submit handler
  const onSubmit = (data: SearchFormData) => {
    const searchParams: SearchRequest = {
      badgeNumber: data.badgeNumber ?? undefined,
      name: data.name ?? undefined,
      ssn: data.socialSecurity ? Number(data.socialSecurity) : undefined,
    };
    onSearch(searchParams);
  };

  const validateAndSubmit = handleSubmit(onSubmit);

  // 6. Reset handler
  const handleReset = () => {
    reset({
      badgeNumber: undefined,
      name: undefined,
      socialSecurity: undefined,
    });
  };

  // 7. Render form
  return (
    <form onSubmit={validateAndSubmit}>
      <Grid container spacing={3} paddingX="24px">
        {/* Badge Number Field */}
        <Grid size={{ xs: 12, sm: 6, md: 4 }}>
          <FormLabel>Badge Number</FormLabel>
          <Controller
            name="badgeNumber"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                fullWidth
                size="small"
                variant="outlined"
                value={field.value ?? ""}
                error={!!errors.badgeNumber}
                onChange={(e) => {
                  const value = e.target.value;
                  const parsedValue = value === "" ? null : Number(value);
                  field.onChange(parsedValue);
                }}
              />
            )}
          />
          {errors.badgeNumber && (
            <FormHelperText error>{errors.badgeNumber.message}</FormHelperText>
          )}
        </Grid>

        {/* Name Field */}
        <Grid size={{ xs: 12, sm: 6, md: 4 }}>
          <FormLabel>Name</FormLabel>
          <Controller
            name="name"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                fullWidth
                size="small"
                variant="outlined"
                value={field.value ?? ""}
                error={!!errors.name}
              />
            )}
          />
          {errors.name && (
            <FormHelperText error>{errors.name.message}</FormHelperText>
          )}
        </Grid>

        {/* SSN Field */}
        <Grid size={{ xs: 12, sm: 6, md: 4 }}>
          <FormLabel>SSN</FormLabel>
          <Controller
            name="socialSecurity"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                fullWidth
                size="small"
                variant="outlined"
                value={field.value ?? ""}
                error={!!errors.socialSecurity}
                onChange={(e) => {
                  const value = e.target.value;
                  // Only allow numeric input
                  if (value !== "" && !/^\d*$/.test(value)) return;
                  // Prevent input beyond 9 characters
                  if (value.length > 9) return;
                  const parsedValue = value === "" ? null : value;
                  field.onChange(parsedValue);
                }}
              />
            )}
          />
          {errors.socialSecurity && (
            <FormHelperText error>
              {errors.socialSecurity.message}
            </FormHelperText>
          )}
        </Grid>

        {/* Search and Reset Buttons */}
        <Grid container justifyContent="flex-end" paddingY="16px">
          <Grid size={{ xs: 12 }}>
            <SearchAndReset
              handleReset={handleReset}
              handleSearch={validateAndSubmit}
              isFetching={isSearching}
              disabled={!isValid || isSearching}
            />
          </Grid>
        </Grid>
      </Grid>
    </form>
  );
};

export default SearchFilter;
```

## Testing Validation

When writing tests for forms with validation:

1. **Test valid submission**: Verify form submits with valid data
2. **Test invalid submission**: Verify form blocks submission with invalid data
3. **Test error messages**: Verify correct error messages display
4. **Test field interactions**: Verify cross-field validation and auto-updates
5. **Test reset functionality**: Verify form resets to default state

Example test structure:

```typescript
describe("SearchFilter Validation", () => {
  it("should submit with valid data", async () => {
    // Fill form with valid data
    // Click submit
    // Verify onSearch was called with correct params
  });

  it("should block submission with invalid SSN", async () => {
    // Fill SSN with 8 digits (invalid)
    // Click submit
    // Verify error message displays
    // Verify onSearch was not called
  });

  it("should prevent non-numeric input in SSN field", async () => {
    // Type letters into SSN field
    // Verify field value is empty
  });
});
```

## Summary

The form validation architecture in this application provides:

- **Declarative validation** via Yup schemas
- **Real-time input restrictions** via `onChange` handlers
- **Consistent error messaging** via `FormHelperText`
- **Type safety** via TypeScript interfaces
- **Reusable validators** via `utils/FormValidators.ts`
- **Cross-field dependencies** via Yup's `.test()` and `this.parent`
- **Proper controlled component handling** via React Hook Form's `Controller`

By following these patterns, you ensure a consistent, user-friendly validation experience across all forms in the application.
