---
applyTo: "src/ui/src/**/*.*"
paths: "src/ui/src/**/*.*"
---

# UI Accessibility Instructions (WCAG 2.2 AA)

This document defines **mandatory accessibility (a11y) requirements** for all UI components to ensure WCAG 2.2 Level AA compliance and provide an accessible experience for all users, including those using assistive technologies.

## Core Principles

All new UI components and features MUST follow these accessibility principles:

1. **Perceivable**: Information and UI components must be presentable to users in ways they can perceive
2. **Operable**: UI components and navigation must be operable by all users
3. **Understandable**: Information and operation of UI must be understandable
4. **Robust**: Content must be robust enough to be interpreted by assistive technologies

## Required Patterns for Form Inputs

### ARIA Attributes (MANDATORY)

Every form input MUST include appropriate ARIA attributes:

```typescript
// Required attributes for all inputs
<TextField
  id={generateFieldId("fieldName")}           // Unique ID for label association
  aria-invalid={!!errors.fieldName}            // REQUIRED: Mark invalid state
  aria-required={isRequired}                   // REQUIRED if field is required
  aria-describedby={ariaDescribedBy}          // REQUIRED: Link to errors/hints
  inputProps={{
    inputMode: "numeric"  // For numeric fields (mobile keyboard)
  }}
/>
```

### Label Associations (MANDATORY)

Every input MUST have a properly associated label:

```typescript
import { generateFieldId } from "utils/accessibilityHelpers";

const fieldId = generateFieldId("socialSecurity");

<FormLabel htmlFor={fieldId}>Social Security Number</FormLabel>
<Controller
  name="socialSecurity"
  control={control}
  render={({ field }) => (
    <TextField
      {...field}
      id={fieldId}
      aria-invalid={!!errors.socialSecurity}
      aria-required={true}
      aria-describedby={useAriaDescribedBy("socialSecurity", !!errors.socialSecurity, true)}
    />
  )}
/>
```

### Error Announcements (MANDATORY)

All validation errors MUST be announced to screen readers:

**Inline validation** (while typing) - use `aria-live="polite"`:

```typescript
{errors.fieldName && (
  <div
    id="fieldName-error"
    aria-live="polite"
    aria-atomic="true">
    <FormHelperText error>{errors.fieldName.message}</FormHelperText>
  </div>
)}
```

**Form submission errors** - use `role="alert"`:

```typescript
{submitError && (
  <div
    role="alert"
    aria-live="assertive"
    aria-atomic="true">
    <FormHelperText error>{submitError}</FormHelperText>
  </div>
)}
```

### Placeholders and Helper Text

All inputs SHOULD include:

1. **Placeholder** showing expected format
2. **Helper text** (via `aria-describedby`) explaining format

```typescript
import { INPUT_PLACEHOLDERS, ARIA_DESCRIPTIONS } from "utils/inputFormatters";

<TextField
  placeholder={INPUT_PLACEHOLDERS.SSN}
  aria-describedby="ssn-hint"
/>
<VisuallyHidden id="ssn-hint">
  {ARIA_DESCRIPTIONS.SSN_FORMAT}
</VisuallyHidden>
```

## Live Input Formatting

Use formatters from `utils/inputFormatters.ts` for consistent formatting:

### SSN Formatting (REQUIRED for all SSN inputs)

```typescript
import { formatSSNInput, INPUT_PLACEHOLDERS, ARIA_DESCRIPTIONS } from "utils/inputFormatters";
import { useCallback } from "react";

// Memoize handler for performance
const handleSSNChange = useCallback((
  e: React.ChangeEvent<HTMLInputElement>,
  field: ControllerRenderProps
) => {
  const { display, raw } = formatSSNInput(e.target.value);

  // Update display value in input
  e.target.value = display;

  // Store raw numeric value for validation/submission
  field.onChange(raw || null);
}, []);

<Controller
  name="socialSecurity"
  control={control}
  render={({ field }) => (
    <TextField
      {...field}
      placeholder={INPUT_PLACEHOLDERS.SSN}
      aria-describedby="ssn-hint"
      onChange={(e) => handleSSNChange(e, field)}
    />
  )}
/>
<VisuallyHidden id="ssn-hint">
  {ARIA_DESCRIPTIONS.SSN_FORMAT}
</VisuallyHidden>
```

### Phone Formatting (REQUIRED for phone inputs)

```typescript
import { formatPhoneInput } from "utils/inputFormatters";

const handlePhoneChange = useCallback(
  (e: React.ChangeEvent<HTMLInputElement>, field: ControllerRenderProps) => {
    const { display, raw } = formatPhoneInput(e.target.value);
    e.target.value = display;
    field.onChange(raw || null);
  },
  [],
);
```

### Zip Code Formatting (REQUIRED for zip inputs)

```typescript
import { formatZipCode } from "utils/inputFormatters";

const handleZipChange = useCallback(
  (e: React.ChangeEvent<HTMLInputElement>, field: ControllerRenderProps) => {
    const { display, raw } = formatZipCode(e.target.value);
    e.target.value = display;
    field.onChange(raw || null);
  },
  [],
);
```

## Loading States (MANDATORY)

All async operations MUST announce loading state to screen readers:

```typescript
import { LoadingAnnouncement } from "utils/accessibilityHelpers";

<LoadingAnnouncement
  isLoading={isFetching}
  loadingMessage="Loading search results..."
  loadedMessage="Search results loaded"
/>

{/* Visual loading indicator */}
{isFetching && <CircularProgress aria-hidden="true" />}
```

## Modal/Dialog Accessibility (MANDATORY)

All modals MUST include:

```typescript
<Dialog
  open={open}
  aria-labelledby="dialog-title"
  aria-describedby="dialog-description"
  onClose={handleClose}>
  <DialogTitle id="dialog-title">
    Edit Employee Record
  </DialogTitle>
  <DialogContent id="dialog-description">
    Please update the employee information below.
  </DialogContent>
  <DialogActions>
    <Button onClick={handleClose}>Cancel</Button>
    <Button type="submit" autoFocus>Save</Button>
  </DialogActions>
</Dialog>
```

## Button Accessibility

### Icon-only buttons (REQUIRED)

All icon-only buttons MUST have `aria-label`:

```typescript
import { getButtonAriaLabel } from "utils/accessibilityHelpers";

<IconButton
  aria-label={getButtonAriaLabel("delete", "employee record")}
  onClick={handleDelete}>
  <DeleteIcon />
</IconButton>
```

### Loading buttons (REQUIRED)

```typescript
<Button
  disabled={isLoading}
  aria-busy={isLoading}
  aria-label={isLoading ? "Saving..." : "Save"}>
  {isLoading ? <CircularProgress size={20} /> : "Save"}
</Button>
```

## Keyboard Navigation (MANDATORY)

All interactive elements MUST be keyboard accessible:

1. **Tab order**: Elements receive focus in logical order
2. **Enter/Space**: Activate buttons and links
3. **Escape**: Close modals and dropdowns
4. **Arrow keys**: Navigate within complex widgets

```typescript
// Example: Keyboard-accessible custom button
<div
  role="button"
  tabIndex={0}
  onClick={handleClick}
  onKeyDown={(e) => {
    if (e.key === "Enter" || e.key === " ") {
      e.preventDefault();
      handleClick();
    }
  }}>
  Custom Button
</div>
```

## Focus Management (MANDATORY)

### Focus indicators

All focusable elements MUST have visible focus indicators (see `src/ui/src/styles/index.css`):

```css
button:focus-visible,
input:focus-visible,
a:focus-visible {
  outline: 2px solid #0258a5;
  outline-offset: 2px;
}
```

### Focus on first error

```typescript
import { focusFirstError } from "utils/accessibilityHelpers";
import { useEffect } from "react";

const {
  formState: { errors },
} = useForm();

useEffect(() => {
  if (Object.keys(errors).length > 0) {
    focusFirstError(errors);
  }
}, [errors]);
```

### Autofocus in modals

First input in modal SHOULD receive focus:

```typescript
<TextField
  autoFocus
  id={generateFieldId("firstName")}
  {...field}
/>
```

## Color Contrast (MANDATORY)

All text and UI components MUST meet WCAG 2.2 AA contrast requirements:

- **Normal text**: 4.5:1 minimum contrast ratio
- **Large text** (18pt+ or 14pt+ bold): 3:1 minimum
- **UI components** (buttons, form borders): 3:1 minimum
- **Focus indicators**: 3:1 minimum against background

Test using browser DevTools or WAVE extension.

## Screen Reader Only Content (WHEN NEEDED)

Use `VisuallyHidden` component for screen reader only text:

```typescript
import { VisuallyHidden } from "utils/accessibilityHelpers";

<VisuallyHidden>
  Currently viewing page 1 of 10
</VisuallyHidden>

<button aria-label="Go to next page">
  <NextIcon aria-hidden="true" />
</button>
```

## Skip Links (REQUIRED for main app layout)

Add skip navigation links before header:

```typescript
import { SkipLink } from "utils/accessibilityHelpers";

<SkipLink targetId="main-content" label="Skip to main content" />
<SkipLink targetId="navigation" label="Skip to navigation" />

{/* ... header navigation ... */}

<main id="main-content">
  {/* page content */}
</main>
```

## Automated Testing (REQUIRED)

### Unit Tests with jest-axe

Every component with form inputs or interactive elements MUST have a11y tests:

```typescript
import { axe, toHaveNoViolations } from "jest-axe";
import { render } from "@testing-library/react";

expect.extend(toHaveNoViolations);

describe("MyFormComponent", () => {
  it("should have no accessibility violations", async () => {
    const { container } = render(<MyFormComponent />);
    const results = await axe(container);
    expect(results).toHaveNoViolations();
  });

  it("should announce errors to screen readers", async () => {
    const { getByRole } = render(<MyFormComponent />);

    // Submit invalid form
    fireEvent.submit(getByRole("button", { name: /submit/i }));

    // Check for error announcement
    expect(getByRole("alert")).toBeInTheDocument();
  });
});
```

### E2E Tests with Playwright + axe

Critical user flows MUST include a11y checks:

```typescript
import { test, expect } from "@playwright/test";
import AxeBuilder from "@axe-core/playwright";

test("search form should be accessible", async ({ page }) => {
  await page.goto("/master-inquiry");

  const accessibilityScanResults = await new AxeBuilder({ page })
    .withTags(["wcag2a", "wcag2aa", "wcag22aa"])
    .analyze();

  expect(accessibilityScanResults.violations).toEqual([]);
});

test("form errors should be announced", async ({ page }) => {
  await page.goto("/master-inquiry");

  // Submit without required fields
  await page.click('button[type="submit"]');

  // Check for error announcement
  const errorAlert = page.locator('[role="alert"]');
  await expect(errorAlert).toBeVisible();
});
```

### CI/CD Integration

Add to `package.json`:

```json
{
  "scripts": {
    "test:a11y": "jest --testPathPattern=a11y",
    "e2e:a11y": "playwright test --grep @a11y"
  }
}
```

Add to pipeline (e.g., `bitbucket-pipelines.yml`):

```yaml
- step:
    name: Accessibility Tests
    script:
      - cd src/ui
      - npm run test:a11y
      - npm run e2e:a11y
```

## Common Violations to Avoid

### ❌ DON'T

```typescript
// No label association
<FormLabel>SSN</FormLabel>
<TextField name="ssn" />

// Missing aria-invalid
<TextField error={!!errors.ssn} />

// Placeholder only (no visible label)
<TextField placeholder="Enter SSN" />

// Icon button without aria-label
<IconButton onClick={handleDelete}>
  <DeleteIcon />
</IconButton>

// Loading state not announced
{isLoading && <CircularProgress />}

// Error not associated with input
<TextField name="ssn" />
{errors.ssn && <div>{errors.ssn.message}</div>}
```

### ✅ DO

```typescript
// Proper label association
const fieldId = generateFieldId("ssn");
<FormLabel htmlFor={fieldId}>SSN</FormLabel>
<TextField
  id={fieldId}
  name="ssn"
  aria-invalid={!!errors.ssn}
  aria-describedby={useAriaDescribedBy("ssn", !!errors.ssn)}
/>

// Icon button with aria-label
<IconButton
  aria-label="Delete employee record"
  onClick={handleDelete}>
  <DeleteIcon aria-hidden="true" />
</IconButton>

// Loading state announced
<LoadingAnnouncement isLoading={isLoading} />
{isLoading && <CircularProgress aria-hidden="true" />}

// Error properly associated
<TextField
  id="ssn-input"
  aria-describedby="ssn-error"
/>
{errors.ssn && (
  <div id="ssn-error" role="alert">
    {errors.ssn.message}
  </div>
)}
```

## Resources

### Documentation

- [ACCESSIBILITY_GUIDE.md](../../../src/ui/public/docs/ACCESSIBILITY_GUIDE.md) - Comprehensive guide
- [WCAG 2.2 Quick Reference](https://www.w3.org/WAI/WCAG22/quickref/)
- [ARIA Authoring Practices Guide](https://www.w3.org/WAI/ARIA/apg/)

### Testing Tools

- [axe DevTools Browser Extension](https://www.deque.com/axe/devtools/)
- [WAVE Browser Extension](https://wave.webaim.org/extension/)
- [jest-axe](https://github.com/nickcolley/jest-axe)
- [@axe-core/playwright](https://www.npmjs.com/package/@axe-core/playwright)

### Screen Readers

- **Windows**: NVDA (free), JAWS (commercial)
- **macOS**: VoiceOver (built-in)
- **Mobile**: TalkBack (Android), VoiceOver (iOS)

## Checklist for New Components

Before submitting PR, verify:

- [ ] All inputs have `id` and matching `htmlFor` labels
- [ ] All inputs have `aria-invalid` when in error state
- [ ] Required inputs have `aria-required="true"`
- [ ] All inputs have `aria-describedby` linking to errors/hints
- [ ] Errors use `aria-live="polite"` (inline) or `role="alert"` (submit)
- [ ] Loading states use `LoadingAnnouncement` component
- [ ] Icon-only buttons have `aria-label`
- [ ] Modals have `aria-labelledby` and `aria-describedby`
- [ ] All interactive elements are keyboard accessible
- [ ] Focus indicators are visible (3:1 contrast minimum)
- [ ] Color contrast meets WCAG 2.2 AA (4.5:1 normal text, 3:1 large/UI)
- [ ] SSN inputs use `formatSSNInput` with live formatting
- [ ] Phone inputs use `formatPhoneInput` with live formatting
- [ ] Placeholders show expected format
- [ ] Unit tests include `jest-axe` checks
- [ ] E2E tests include axe-core checks for critical flows

---

**Last updated**: January 25, 2026
**WCAG Version**: 2.2 Level AA
**Applies to**: All UI components in `src/ui/src/`
