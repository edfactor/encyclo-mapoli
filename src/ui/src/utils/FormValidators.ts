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
 * Validates that a profit year is a number between 2020 and 2100 (required)
 */
export const profitYearValidator = yup
  .number()
  .typeError("Year must be a number")
  .integer("Year must be an integer")
  .min(2020, "Year must be 2020 or later")
  .max(2100, "Year must be 2100 or earlier")
  .required("Year is required");

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
