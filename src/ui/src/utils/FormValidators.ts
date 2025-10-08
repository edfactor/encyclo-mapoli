import * as yup from "yup";

/**
 * Validates that a Social Security Number is exactly 9 digits
 */
export const ssnValidator = yup
  .string()
  .nullable()
  .test("is-9-digits", "SSN must be exactly 9 digits", function (value) {
    if (!value) return true;
    return /^\d{9}$/.test(value);
  })
  .transform((value) => value || undefined);

/**
 * Validates that a Badge Number is between 1 and 7 digits (1 to 9999999)
 */
export const badgeNumberValidator = yup
  .number()
  .typeError("Badge Number must be a number")
  .integer("Badge Number must be an integer")
  .min(1, "Badge Number must be at least 1")
  .max(9999999, "Badge Number must be 7 digits or less")
  .nullable()
  .transform((value) => value || undefined);

export const badgeNumberOrPSNValidator = yup
  .number()
  .typeError("Badge Number or PSN must be a number")
  .integer("Badge Number or PSN must be an integer")
  .min(10000, "Badge Number or PSN must be at least 5 digits")
  .max(99999999999, "Badge Number or PSN must be 11 digits or less")
  .nullable()
  .transform((value) => value || undefined);

/**
 * Validates that a month number is between 1 and 12
 */
export const monthValidator = yup
  .number()
  .typeError("Month must be a number")
  .integer("Month must be an integer")
  .min(1, "Month must be between 1 and 12")
  .max(12, "Month must be between 1 and 12")
  .nullable();

/**
 * Validates that a PSN (Profit Sharing Number) is between 9 and 11 digits
 */
export const psnValidator = yup
  .number()
  .typeError("PSN must be a number")
  .integer("PSN must be an integer")
  .min(100000000, "PSN must be at least 9 digits")
  .max(99999999999, "PSN must be 11 digits or less")
  .nullable()
  .transform((value) => value || undefined);

/**
 * Validates that a profit year is a number between specified years (required)
 * @param minYear - Minimum allowed year (default: 2020)
 * @param maxYear - Maximum allowed year (default: 2100)
 */
export const profitYearValidator = (minYear: number = 2015, maxYear: number = 2100) => {
  console.log(`Validating profit year between ${minYear} and ${maxYear}`);
  return yup
    .number()
    .typeError("Year must be a number")
    .integer("Year must be an integer")
    .min(minYear, `Year must be ${minYear} or later`)
    .max(maxYear, `Year must be ${maxYear} or earlier`)
    .required("Year is required");
};
/**
 * Validates that a profit year is a number between 2020 and 2100 (nullable)
 */
export const profitYearNullableValidator = yup
  .number()
  .typeError("Year must be a number")
  .integer("Year must be an integer")
  .min(2020, "Year must be 2020 or later")
  .max(2100, "Year must be 2100 or earlier")
  .nullable();

/**
 * Validates that a profit year is a date between 2020 and 2100 (required)
 */
export const profitYearDateValidator = yup
  .date()
  .typeError("Invalid date")
  .min(new Date(2020, 0, 1), "Year must be 2020 or later")
  .max(new Date(2100, 11, 31), "Year must be 2100 or earlier")
  .required("Profit Year is required");

/**
 * Returns a validator for a positive number, with custom field name in error messages.
 * @param fieldName - The name of the field to use in error messages
 */
export const positiveNumberValidator = (fieldName: string) =>
  yup
    .number()
    .typeError(`${fieldName} must be a number`)
    .min(0, `${fieldName} must be a positive number`)
    .nullable()
    .transform((value) => (isNaN(value) ? null : value));

/**
 * Returns a validator that checks if a string value is a valid number
 * @param fieldName - Optional field name to use in error message (defaults to "Must be a valid number")
 */
export const mustBeNumberValidator = (fieldName?: string) =>
  yup
    .string()
    .nullable()
    .test("is-number", fieldName ? `${fieldName} must be a valid number` : "Must be a valid number", function (value) {
      if (!value) return true;
      return !isNaN(Number(value));
    });

/**
 * Returns a validator for an end date (Date object) that must be after a start date
 * @param startFieldName - The name of the start date field in the parent schema
 * @param errorMessage - Optional custom error message
 */
export const endDateAfterStartDateValidator = (startFieldName: string, errorMessage?: string) =>
  yup
    .date()
    .nullable()
    .test("is-after-start", errorMessage || "End Date must be after Start Date", function (value) {
      const startDate = this.parent[startFieldName];
      if (!startDate || !value) return true;
      return value > startDate;
    });

/**
 * Returns a validator for an end date string that must be after a start date string
 * Requires a date conversion function to convert string to Date
 * @param startFieldName - The name of the start date field in the parent schema
 * @param convertToDate - Function to convert date string to Date object
 * @param errorMessage - Optional custom error message
 */
export const endDateStringAfterStartDateValidator = (
  startFieldName: string,
  convertToDate: (dateString: string) => Date | null,
  errorMessage?: string
) =>
  yup
    .string()
    .nullable()
    .test("is-after-start", errorMessage || "End Date must be after Start Date", function (value) {
      const startDateString = this.parent[startFieldName];
      if (!startDateString || !value) return true;
      const startDate = convertToDate(startDateString);
      const endDate = convertToDate(value);
      if (!startDate || !endDate) return true;
      return endDate >= startDate;
    });

/**
 * Returns a validator for a date string in MM/DD/YYYY format with year range validation
 * @param minYear - Minimum allowed year (default: 2000)
 * @param maxYear - Maximum allowed year (default: 2099)
 * @param fieldName - Optional field name for error messages (default: "Date")
 */
export const dateStringValidator = (minYear: number = 2000, maxYear: number = 2099, fieldName: string = "Date") => {
  console.log(`Validating ${fieldName} with year between ${minYear} and ${maxYear}`);

  return yup
    .string()
    .nullable()
    .test("is-valid-format", `${fieldName} must be in MM/DD/YYYY format`, function (value) {
      console.log(`FORMAT FORMAT Validating format of ${fieldName}:`, value);
      if (!value) return true;
      return /^\d{1,2}\/\d{1,2}\/\d{4}$/.test(value);
    })
    .test("is-valid-year", `Year must be between ${minYear} and ${maxYear}`, function (value) {
      console.log(`VALID VALID Validating year of ${fieldName}:`, value);
      if (!value) return true;
      const match = value.match(/^\d{1,2}\/\d{1,2}\/(\d{4})$/);
      if (!match) return true;
      const year = parseInt(match[1]);
      return year >= minYear && year <= maxYear;
    });
};

/**
 * Handler for SSN input that only allows numeric characters up to 9 digits
 * @param value - The input value from the TextField
 * @returns The validated value or null if invalid
 */
export const handleSsnInput = (value: string): string | null => {
  // Only allow numeric input
  if (value !== "" && !/^\d*$/.test(value)) {
    return null;
  }
  // Prevent input beyond 9 characters
  if (value.length > 9) {
    return null;
  }
  return value === "" ? "" : value;
};

/**
 * Handler for badge number input that only allows numeric values
 * @param value - The input value from the TextField
 * @returns The validated numeric value, empty string, or null if invalid
 */
export const handleBadgeNumberInput = (value: string): number | string | null => {
  // Allow empty string or numeric values
  if (value === "" || !isNaN(Number(value))) {
    return value === "" ? "" : Number(value);
  }

  // Prevent input beyond 7 characters
  if (value.length > 7) {
    return null;
  }
  return null;
};
