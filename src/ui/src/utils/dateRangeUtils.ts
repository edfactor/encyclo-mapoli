/**
 * Utility functions for expanding month/year selections to full date ranges.
 * Used primarily in date pickers that show month/year views instead of full date selection.
 */

/**
 * Expands a month/year selection to the first day of that month.
 *
 * @param date - The selected date (typically first of month from month picker)
 * @returns Date set to the first day of the selected month, or null if input is null
 *
 * @example
 * // User selects "March 2025"
 * getMonthStartDate(new Date(2025, 2, 15)) // Returns 2025-03-01
 */
export const getMonthStartDate = (date: Date | null): Date | null => {
  if (!date) return null;
  return new Date(date.getFullYear(), date.getMonth(), 1);
};

/**
 * Expands a month/year selection to the last day of that month.
 * Automatically handles varying month lengths (28, 29, 30, or 31 days).
 *
 * @param date - The selected date (typically first of month from month picker)
 * @returns Date set to the last day of the selected month, or null if input is null
 *
 * @example
 * // User selects "March 2025"
 * getMonthEndDate(new Date(2025, 2, 15)) // Returns 2025-03-31
 *
 * @example
 * // Handles leap years correctly
 * getMonthEndDate(new Date(2024, 1, 15)) // Returns 2024-02-29
 */
export const getMonthEndDate = (date: Date | null): Date | null => {
  if (!date) return null;
  // Get last day by going to next month's first day and subtracting one day
  return new Date(date.getFullYear(), date.getMonth() + 1, 0);
};

/**
 * Gets the full date range for a given month.
 * Convenience function that returns both start and end dates.
 *
 * @param date - The selected date
 * @returns Object with startDate and endDate for the month, or nulls if input is null
 *
 * @example
 * // User selects "March 2025"
 * getMonthDateRange(new Date(2025, 2, 15))
 * // Returns { startDate: 2025-03-01, endDate: 2025-03-31 }
 */
export const getMonthDateRange = (date: Date | null): { startDate: Date | null; endDate: Date | null } => {
  return {
    startDate: getMonthStartDate(date),
    endDate: getMonthEndDate(date)
  };
};
