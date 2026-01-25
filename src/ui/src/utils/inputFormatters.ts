/**
 * Input Formatters - Live formatting utilities for form inputs
 *
 * Provides real-time formatting for SSN, phone, zip codes, and standardized placeholders.
 * All formatters maintain underlying values while providing visual formatting.
 * Performance optimized with memoization patterns.
 */

/**
 * Centralized input placeholders for consistent UX across the application
 */
export const INPUT_PLACEHOLDERS = {
  // Identity Fields
  SSN: "#########",
  SSN_HINT: "9 digits",
  BADGE: "12345",
  BADGE_HINT: "3-7 digits",
  PSN: "123450001",
  PSN_HINT: "badge + 4 digits (8-11 total)",
  BADGE_OR_PSN: "Badge or PSN",
  BADGE_OR_PSN_HINT: "3-11 digits",

  // Contact Fields
  PHONE: "(###) ###-####",
  PHONE_HINT: "10 digits",
  EMAIL: "user@example.com",
  ZIP: "#####",
  ZIP_EXTENDED: "#####-####",

  // Financial Fields
  CURRENCY: "0.00",
  CURRENCY_WITH_SYMBOL: "$0.00",
  PERCENTAGE: "##.##",
  ROUTING_NUMBER: "#########",
  ROUTING_NUMBER_HINT: "9 digits",
  ACCOUNT_NUMBER: "Account number",
  ACCOUNT_NUMBER_HINT: "up to 34 digits",

  // Date/Time Fields
  DATE: "MM/DD/YYYY",
  YEAR: "YYYY",
  MONTH: "MM",
  PROFIT_YEAR: "YYYY",

  // Address Fields
  STREET: "123 Main Street",
  CITY: "City",
  STATE: "MA",

  // Other
  MEMO: "Enter memo or comments...",
  SEARCH: "Search..."
} as const;

/**
 * Accessibility descriptions for formatted inputs
 */
export const ARIA_DESCRIPTIONS = {
  SSN_FORMAT: "Enter 9 digits, hyphens added automatically",
  PHONE_FORMAT: "Enter 10 digits, parentheses and hyphens added automatically",
  ZIP_FORMAT: "Enter 5 digits, leading zeros added automatically for Massachusetts zip codes",
  BADGE_FORMAT: "Enter badge number (3-7 digits) or beneficiary PSN (8-11 digits)",
  BADGE_DYNAMIC: "Enter badge number (3-7 digits) or beneficiary PSN (8-11 digits)",
  CURRENCY_FORMAT: "Enter amount in dollars and cents",
  PROFIT_YEAR: "Enter 4-digit profit year",
  ROUTING_NUMBER: "Enter 9-digit bank routing number",
  ACCOUNT_NUMBER: "Enter account number, up to 34 digits"
} as const;

/**
 * Format SSN input with live ###-##-#### formatting
 * @param value - Raw input value
 * @returns Object with formatted display value and raw numeric value
 * @example
 * formatSSNInput("123456789") // { display: "123-45-6789", raw: "123456789" }
 * formatSSNInput("123-45-6789") // { display: "123-45-6789", raw: "123456789" }
 */
export const formatSSNInput = (value: string): { display: string; raw: string } => {
  // Remove all non-digit characters
  const cleaned = value.replace(/\D/g, "");

  // Limit to 9 digits
  const limited = cleaned.slice(0, 9);

  // Format with hyphens as user types
  let formatted = limited;
  if (limited.length > 5) {
    // ###-##-####
    formatted = `${limited.slice(0, 3)}-${limited.slice(3, 5)}-${limited.slice(5)}`;
  } else if (limited.length > 3) {
    // ###-##
    formatted = `${limited.slice(0, 3)}-${limited.slice(3)}`;
  }
  // else: less than 3 digits, no formatting needed

  return {
    display: formatted,
    raw: limited
  };
};

/**
 * Format phone number input with live (###) ###-#### formatting
 * @param value - Raw input value
 * @returns Object with formatted display value and raw numeric value
 * @example
 * formatPhoneInput("5081234567") // { display: "(508) 123-4567", raw: "5081234567" }
 */
export const formatPhoneInput = (value: string): { display: string; raw: string } => {
  // Remove all non-digit characters
  const cleaned = value.replace(/\D/g, "");

  // Limit to 10 digits
  const limited = cleaned.slice(0, 10);

  // Format with parentheses and hyphens as user types
  let formatted = limited;
  if (limited.length > 6) {
    // (###) ###-####
    formatted = `(${limited.slice(0, 3)}) ${limited.slice(3, 6)}-${limited.slice(6)}`;
  } else if (limited.length > 3) {
    // (###) ###
    formatted = `(${limited.slice(0, 3)}) ${limited.slice(3)}`;
  } else if (limited.length > 0) {
    // (###
    formatted = `(${limited}`;
  }

  return {
    display: formatted,
    raw: limited
  };
};

/**
 * Format zip code with automatic leading zero for Massachusetts zips
 * @param value - Raw input value
 * @returns Object with formatted display value and raw value
 * @example
 * formatZipCode("1850") // { display: "01850", raw: "01850" }
 * formatZipCode("02138") // { display: "02138", raw: "02138" }
 * formatZipCode("12345") // { display: "12345", raw: "12345" }
 */
export const formatZipCode = (value: string): { display: string; raw: string } => {
  // Remove all non-digit characters
  const cleaned = value.replace(/\D/g, "");

  // Limit to 9 digits (5 + 4 for extended zip)
  const limited = cleaned.slice(0, 9);

  // Auto-prepend zero for 4-digit Massachusetts zips
  let formatted = limited;
  if (limited.length === 4 && !limited.startsWith("0")) {
    formatted = `0${limited}`;
  }

  return {
    display: formatted,
    raw: formatted // Store formatted value since we want to keep the leading zero
  };
};

/**
 * Get dynamic placeholder for badge/PSN field based on current input length
 * @param length - Current input length
 * @returns Appropriate placeholder text
 * @example
 * getBadgeOrPSNPlaceholder(0) // "Badge or PSN"
 * getBadgeOrPSNPlaceholder(5) // "Employee Badge (3-7 digits)"
 * getBadgeOrPSNPlaceholder(9) // "Beneficiary PSN (badge + 4 digits)"
 */
export const getBadgeOrPSNPlaceholder = (length: number): string => {
  if (length === 0) {
    return INPUT_PLACEHOLDERS.BADGE_OR_PSN;
  } else if (length >= 3 && length <= 7) {
    return `Employee Badge (${INPUT_PLACEHOLDERS.BADGE_HINT})`;
  } else if (length >= 8) {
    return `Beneficiary PSN (${INPUT_PLACEHOLDERS.PSN_HINT})`;
  }
  return INPUT_PLACEHOLDERS.BADGE_OR_PSN;
};

/**
 * Currency formatting helper (for display, not live input)
 * @param value - Numeric value
 * @returns Formatted currency string
 */
export const formatCurrency = (value: number | null | undefined): string => {
  if (value === null || value === undefined) return "N/A";
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(value);
};

/**
 * Validate SSN format (9 digits, with or without hyphens)
 * @param value - SSN value to validate
 * @returns True if valid SSN format
 */
export const isValidSSNFormat = (value: string): boolean => {
  const cleaned = value.replace(/\D/g, "");
  return cleaned.length === 9;
};

/**
 * Validate phone format (10 digits)
 * @param value - Phone value to validate
 * @returns True if valid phone format
 */
export const isValidPhoneFormat = (value: string): boolean => {
  const cleaned = value.replace(/\D/g, "");
  return cleaned.length === 10;
};

/**
 * Validate zip code format (5 or 9 digits)
 * @param value - Zip code value to validate
 * @returns True if valid zip format
 */
export const isValidZipFormat = (value: string): boolean => {
  const cleaned = value.replace(/\D/g, "");
  return cleaned.length === 5 || cleaned.length === 9;
};
