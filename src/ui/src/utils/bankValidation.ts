/**
 * Bank account validation utilities
 * These validators match the backend FluentValidation rules in:
 * - CreateBankAccountRequestValidator.cs
 * - UpdateBankAccountRequestValidator.cs
 */

/**
 * Validates that a routing number is exactly 9 digits
 * @param value - The routing number to validate
 * @returns Error message if invalid, empty string if valid
 */
export const validateRoutingNumber = (value: string | undefined | null): string => {
  if (!value || value.trim() === "") {
    return "Routing number is required.";
  }

  // Must be exactly 9 digits
  if (!/^\d{9}$/.test(value)) {
    return "Routing number must be exactly 9 digits.";
  }

  return "";
};

/**
 * Validates that an account number meets requirements
 * @param value - The account number to validate
 * @returns Error message if invalid, empty string if valid
 */
export const validateAccountNumber = (value: string | undefined | null): string => {
  if (!value || value.trim() === "") {
    return "Account number is required.";
  }

  if (value.length > 34) {
    return "Account number cannot exceed 34 characters.";
  }

  return "";
};

/**
 * Validates servicing Fed routing number (optional field)
 * @param value - The servicing Fed routing number to validate
 * @returns Error message if invalid, empty string if valid
 */
export const validateServicingFedRoutingNumber = (value: string | undefined | null): string => {
  // Optional field - empty is valid
  if (!value || value.trim() === "") {
    return "";
  }

  // If provided, must be exactly 9 digits
  if (!/^\d{9}$/.test(value)) {
    return "Servicing Fed routing number must be exactly 9 digits.";
  }

  return "";
};

/**
 * Validates servicing Fed address (optional field)
 * @param value - The servicing Fed address to validate
 * @returns Error message if invalid, empty string if valid
 */
export const validateServicingFedAddress = (value: string | undefined | null): string => {
  // Optional field - empty is valid
  if (!value || value.trim() === "") {
    return "";
  }

  if (value.length > 200) {
    return "Servicing Fed address cannot exceed 200 characters.";
  }

  return "";
};

/**
 * Validates Fedwire telegraphic name (optional field)
 * @param value - The Fedwire telegraphic name to validate
 * @returns Error message if invalid, empty string if valid
 */
export const validateFedwireTelegraphicName = (value: string | undefined | null): string => {
  // Optional field - empty is valid
  if (!value || value.trim() === "") {
    return "";
  }

  if (value.length > 50) {
    return "Fedwire telegraphic name cannot exceed 50 characters.";
  }

  return "";
};

/**
 * Validates Fedwire location (optional field)
 * @param value - The Fedwire location to validate
 * @returns Error message if invalid, empty string if valid
 */
export const validateFedwireLocation = (value: string | undefined | null): string => {
  // Optional field - empty is valid
  if (!value || value.trim() === "") {
    return "";
  }

  if (value.length > 100) {
    return "Fedwire location cannot exceed 100 characters.";
  }

  return "";
};

/**
 * Validates notes field (optional field)
 * @param value - The notes to validate
 * @returns Error message if invalid, empty string if valid
 */
export const validateNotes = (value: string | undefined | null): string => {
  // Optional field - empty is valid
  if (!value || value.trim() === "") {
    return "";
  }

  if (value.length > 1000) {
    return "Notes cannot exceed 1000 characters.";
  }

  return "";
};

/**
 * Validates effective date (optional field, cannot be in the future)
 * @param value - The effective date to validate
 * @returns Error message if invalid, empty string if valid
 */
export const validateEffectiveDate = (value: Date | undefined | null): string => {
  // Optional field - empty is valid
  if (!value) {
    return "";
  }

  const today = new Date();
  today.setHours(0, 0, 0, 0);

  if (value > today) {
    return "Effective date cannot be in the future.";
  }

  return "";
};

/**
 * Handler for routing number input that only allows numeric characters up to 9 digits
 * @param value - The input value from the TextField
 * @returns The validated value
 */
export const handleRoutingNumberInput = (value: string): string => {
  // Only allow numeric input
  const numericValue = value.replace(/\D/g, "");

  // Limit to 9 digits
  return numericValue.slice(0, 9);
};

/**
 * Handler for account number input that enforces max length
 * @param value - The input value from the TextField
 * @returns The validated value
 */
export const handleAccountNumberInput = (value: string): string => {
  // Limit to 34 characters
  return value.slice(0, 34);
};

/**
 * Validates all required fields for creating a bank account
 * @param routingNumber - The routing number
 * @param accountNumber - The account number
 * @returns Object with field errors
 */
export const validateBankAccountForm = (
  routingNumber: string,
  accountNumber: string
): { routingNumber: string; accountNumber: string; isValid: boolean } => {
  const routingError = validateRoutingNumber(routingNumber);
  const accountError = validateAccountNumber(accountNumber);

  return {
    routingNumber: routingError,
    accountNumber: accountError,
    isValid: !routingError && !accountError
  };
};
