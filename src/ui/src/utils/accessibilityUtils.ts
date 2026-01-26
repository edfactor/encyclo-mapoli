/**
 * Accessibility Helper Functions - WCAG 2.2 AA compliant utilities
 *
 * Provides utility functions for implementing accessible forms.
 * Components are in accessibilityComponents.tsx
 */

/**
 * Generate ARIA invalid attribute for form fields
 *
 * @param hasError - Whether field has validation error
 * @returns aria-invalid attribute value
 * @example
 * const ariaInvalid = getAriaInvalid(!!errors.ssn);
 * <TextField aria-invalid={ariaInvalid} />
 */
export const getAriaInvalid = (hasError: boolean): boolean | undefined => {
  return hasError ? true : undefined;
};

/**
 * Generate ARIA describedby attribute for form fields
 * Combines error, hint, and helper text IDs
 *
 * @param fieldName - Name of the field
 * @param hasError - Whether field has error
 * @param hasHint - Whether field has hint text
 * @returns aria-describedby attribute value
 * @example
 * const ariaDescribedBy = getAriaDescribedBy("ssn", !!errors.ssn, true);
 * <TextField aria-describedby={ariaDescribedBy} />
 */
export const getAriaDescribedBy = (
  fieldName: string,
  hasError: boolean,
  hasHint: boolean = false
): string | undefined => {
  const ids: string[] = [];

  if (hasError) {
    ids.push(`${fieldName}-error`);
  }

  if (hasHint) {
    ids.push(`${fieldName}-hint`);
  }

  return ids.length > 0 ? ids.join(" ") : undefined;
};

/**
 * Generate unique ID for form field and its label association
 *
 * @param fieldName - Name of the field
 * @returns Unique field ID
 * @example
 * const fieldId = generateFieldId("socialSecurity");
 * <FormLabel htmlFor={fieldId}>SSN</FormLabel>
 * <TextField id={fieldId} />
 */
export const generateFieldId = (fieldName: string): string => {
  return `field-${fieldName}`;
};

/**
 * Generate ARIA label for button with icon only
 *
 * @param action - Action the button performs
 * @param context - Optional context
 * @returns ARIA label string
 * @example
 * getButtonAriaLabel("delete", "employee record") // "Delete employee record"
 */
export const getButtonAriaLabel = (action: string, context?: string): string => {
  const capitalizedAction = action.charAt(0).toUpperCase() + action.slice(1);
  return context ? `${capitalizedAction} ${context}` : capitalizedAction;
};

/**
 * Focus management helper - focuses first field with error
 *
 * @param errors - Form errors object from react-hook-form
 * @example
 * const { formState: { errors } } = useForm();
 * useEffect(() => {
 *   focusFirstError(errors);
 * }, [errors]);
 */
export const focusFirstError = (errors: Record<string, unknown>): void => {
  const firstErrorKey = Object.keys(errors)[0];
  if (firstErrorKey) {
    const fieldId = generateFieldId(firstErrorKey);
    const element = document.getElementById(fieldId);
    element?.focus();
  }
};

/**
 * Check if element is focusable
 * Used for focus management and keyboard navigation
 *
 * @param element - DOM element to check
 * @returns True if element is focusable
 */
export const isFocusable = (element: HTMLElement): boolean => {
  const tabindex = element.getAttribute("tabindex");
  if (tabindex && parseInt(tabindex) < 0) return false;

  if (element.hasAttribute("disabled")) return false;
  if (element.getAttribute("aria-hidden") === "true") return false;

  const style = window.getComputedStyle(element);
  if (style.display === "none" || style.visibility === "hidden") return false;

  const focusableSelectors = [
    "a[href]",
    "button:not([disabled])",
    "input:not([disabled])",
    "select:not([disabled])",
    "textarea:not([disabled])",
    "[tabindex]:not([tabindex='-1'])"
  ];

  return focusableSelectors.some((selector) => element.matches(selector));
};
