import { describe, expect, it } from "vitest";
import { getMonthStartDate, getMonthEndDate, getMonthDateRange } from "./dateRangeUtils";

describe("dateRangeUtils", () => {
  describe("getMonthStartDate", () => {
    it("should return the first day of the month", () => {
      const date = new Date(2025, 2, 15); // March 15, 2025
      const result = getMonthStartDate(date);
      expect(result).not.toBeNull();
      expect(result?.getFullYear()).toBe(2025);
      expect(result?.getMonth()).toBe(2); // March (0-indexed)
      expect(result?.getDate()).toBe(1);
    });

    it("should handle January", () => {
      const date = new Date(2025, 0, 15); // January 15, 2025
      const result = getMonthStartDate(date);
      expect(result?.getFullYear()).toBe(2025);
      expect(result?.getMonth()).toBe(0);
      expect(result?.getDate()).toBe(1);
    });

    it("should handle December", () => {
      const date = new Date(2025, 11, 31); // December 31, 2025
      const result = getMonthStartDate(date);
      expect(result?.getFullYear()).toBe(2025);
      expect(result?.getMonth()).toBe(11);
      expect(result?.getDate()).toBe(1);
    });

    it("should return null when input is null", () => {
      const result = getMonthStartDate(null);
      expect(result).toBeNull();
    });

    it("should work with date at start of month", () => {
      const date = new Date(2025, 5, 1); // June 1, 2025
      const result = getMonthStartDate(date);
      expect(result?.getDate()).toBe(1);
    });

    it("should work with date at end of month", () => {
      const date = new Date(2025, 5, 30); // June 30, 2025
      const result = getMonthStartDate(date);
      expect(result?.getMonth()).toBe(5);
      expect(result?.getDate()).toBe(1);
    });

    it("should handle leap year February", () => {
      const date = new Date(2024, 1, 29); // February 29, 2024 (leap year)
      const result = getMonthStartDate(date);
      expect(result?.getFullYear()).toBe(2024);
      expect(result?.getMonth()).toBe(1);
      expect(result?.getDate()).toBe(1);
    });
  });

  describe("getMonthEndDate", () => {
    it("should return the last day of March (31 days)", () => {
      const date = new Date(2025, 2, 15); // March 15, 2025
      const result = getMonthEndDate(date);
      expect(result).not.toBeNull();
      expect(result?.getFullYear()).toBe(2025);
      expect(result?.getMonth()).toBe(2);
      expect(result?.getDate()).toBe(31);
    });

    it("should return the last day of April (30 days)", () => {
      const date = new Date(2025, 3, 15); // April 15, 2025
      const result = getMonthEndDate(date);
      expect(result?.getDate()).toBe(30);
    });

    it("should return the last day of February in non-leap year (28 days)", () => {
      const date = new Date(2025, 1, 15); // February 15, 2025 (not leap year)
      const result = getMonthEndDate(date);
      expect(result?.getFullYear()).toBe(2025);
      expect(result?.getMonth()).toBe(1);
      expect(result?.getDate()).toBe(28);
    });

    it("should return the last day of February in leap year (29 days)", () => {
      const date = new Date(2024, 1, 15); // February 15, 2024 (leap year)
      const result = getMonthEndDate(date);
      expect(result?.getFullYear()).toBe(2024);
      expect(result?.getMonth()).toBe(1);
      expect(result?.getDate()).toBe(29);
    });

    it("should handle January (31 days)", () => {
      const date = new Date(2025, 0, 15); // January 15, 2025
      const result = getMonthEndDate(date);
      expect(result?.getDate()).toBe(31);
    });

    it("should handle December (31 days)", () => {
      const date = new Date(2025, 11, 15); // December 15, 2025
      const result = getMonthEndDate(date);
      expect(result?.getDate()).toBe(31);
    });

    it("should return null when input is null", () => {
      const result = getMonthEndDate(null);
      expect(result).toBeNull();
    });

    it("should work with date at start of month", () => {
      const date = new Date(2025, 5, 1); // June 1, 2025
      const result = getMonthEndDate(date);
      expect(result?.getMonth()).toBe(5);
      expect(result?.getDate()).toBe(30);
    });

    it("should work with date at end of month", () => {
      const date = new Date(2025, 5, 30); // June 30, 2025
      const result = getMonthEndDate(date);
      expect(result?.getDate()).toBe(30);
    });

    it("should handle all 30-day months correctly", () => {
      // April, June, September, November
      const thirtyDayMonths = [3, 5, 8, 10]; // 0-indexed
      
      thirtyDayMonths.forEach(month => {
        const date = new Date(2025, month, 15);
        const result = getMonthEndDate(date);
        expect(result?.getDate()).toBe(30);
      });
    });

    it("should handle all 31-day months correctly", () => {
      // January, March, May, July, August, October, December
      const thirtyOneDayMonths = [0, 2, 4, 6, 7, 9, 11]; // 0-indexed
      
      thirtyOneDayMonths.forEach(month => {
        const date = new Date(2025, month, 15);
        const result = getMonthEndDate(date);
        expect(result?.getDate()).toBe(31);
      });
    });

    it("should handle century leap years correctly", () => {
      // 2000 was a leap year (divisible by 400)
      const date2000 = new Date(2000, 1, 15);
      const result2000 = getMonthEndDate(date2000);
      expect(result2000?.getDate()).toBe(29);

      // 1900 was NOT a leap year (divisible by 100 but not 400)
      const date1900 = new Date(1900, 1, 15);
      const result1900 = getMonthEndDate(date1900);
      expect(result1900?.getDate()).toBe(28);
    });
  });

  describe("getMonthDateRange", () => {
    it("should return both start and end dates for a month", () => {
      const date = new Date(2025, 2, 15); // March 15, 2025
      const result = getMonthDateRange(date);
      
      expect(result.startDate).not.toBeNull();
      expect(result.endDate).not.toBeNull();
      expect(result.startDate?.getDate()).toBe(1);
      expect(result.endDate?.getDate()).toBe(31);
      expect(result.startDate?.getMonth()).toBe(2);
      expect(result.endDate?.getMonth()).toBe(2);
    });

    it("should return nulls when input is null", () => {
      const result = getMonthDateRange(null);
      expect(result.startDate).toBeNull();
      expect(result.endDate).toBeNull();
    });

    it("should handle February leap year correctly", () => {
      const date = new Date(2024, 1, 15); // February 15, 2024
      const result = getMonthDateRange(date);
      
      expect(result.startDate?.getDate()).toBe(1);
      expect(result.endDate?.getDate()).toBe(29);
    });

    it("should handle February non-leap year correctly", () => {
      const date = new Date(2025, 1, 15); // February 15, 2025
      const result = getMonthDateRange(date);
      
      expect(result.startDate?.getDate()).toBe(1);
      expect(result.endDate?.getDate()).toBe(28);
    });

    it("should work for single-month query (same month)", () => {
      const date = new Date(2025, 2, 15); // March 2025
      const result = getMonthDateRange(date);
      
      // User selects same month for start and end
      // Should get full month range
      expect(result.startDate?.getFullYear()).toBe(2025);
      expect(result.startDate?.getMonth()).toBe(2);
      expect(result.startDate?.getDate()).toBe(1);
      
      expect(result.endDate?.getFullYear()).toBe(2025);
      expect(result.endDate?.getMonth()).toBe(2);
      expect(result.endDate?.getDate()).toBe(31);
    });
  });

  describe("Real-world use cases", () => {
    it("should handle the reported FL tax issue scenario (March 2025)", () => {
      // User wants to see March transactions for badge 68318
      const marchDate = new Date(2025, 2, 1); // March 2025
      
      const startDate = getMonthStartDate(marchDate);
      const endDate = getMonthEndDate(marchDate);
      
      // Should query from March 1 to March 31
      expect(startDate?.toISOString().split('T')[0]).toBe('2025-03-01');
      expect(endDate?.toISOString().split('T')[0]).toBe('2025-03-31');
    });

    it("should handle quarter range (Q1 2025)", () => {
      // User selects January to March
      const january = getMonthStartDate(new Date(2025, 0, 1));
      const march = getMonthEndDate(new Date(2025, 2, 1));
      
      expect(january?.toISOString().split('T')[0]).toBe('2025-01-01');
      expect(march?.toISOString().split('T')[0]).toBe('2025-03-31');
    });

    it("should handle full year range (2025)", () => {
      // User selects January to December
      const yearStart = getMonthStartDate(new Date(2025, 0, 1));
      const yearEnd = getMonthEndDate(new Date(2025, 11, 1));
      
      expect(yearStart?.toISOString().split('T')[0]).toBe('2025-01-01');
      expect(yearEnd?.toISOString().split('T')[0]).toBe('2025-12-31');
    });

    it("should handle profit year that matches fiscal year", () => {
      // Typical profit year: January 1 to December 31
      const profitYearStart = getMonthStartDate(new Date(2025, 0, 1));
      const profitYearEnd = getMonthEndDate(new Date(2025, 11, 1));
      
      expect(profitYearStart?.getMonth()).toBe(0); // January
      expect(profitYearStart?.getDate()).toBe(1);
      expect(profitYearEnd?.getMonth()).toBe(11); // December
      expect(profitYearEnd?.getDate()).toBe(31);
    });
  });

  describe("Edge cases", () => {
    it("should handle year boundaries correctly", () => {
      // December to January crossing year boundary
      const december = getMonthEndDate(new Date(2024, 11, 15));
      const january = getMonthStartDate(new Date(2025, 0, 15));
      
      expect(december?.getFullYear()).toBe(2024);
      expect(december?.getMonth()).toBe(11);
      expect(december?.getDate()).toBe(31);
      
      expect(january?.getFullYear()).toBe(2025);
      expect(january?.getMonth()).toBe(0);
      expect(january?.getDate()).toBe(1);
    });

    it("should maintain time component as start of day", () => {
      const date = new Date(2025, 2, 15, 14, 30, 45); // March 15, 2025, 2:30:45 PM
      const startDate = getMonthStartDate(date);
      
      // Should be midnight of the first day
      expect(startDate?.getHours()).toBe(0);
      expect(startDate?.getMinutes()).toBe(0);
      expect(startDate?.getSeconds()).toBe(0);
      expect(startDate?.getMilliseconds()).toBe(0);
    });

    it("should handle dates from different timezones consistently", () => {
      // Create dates that might be affected by timezone
      const date1 = new Date(2025, 2, 15);
      const date2 = new Date(Date.UTC(2025, 2, 15));
      
      const start1 = getMonthStartDate(date1);
      const start2 = getMonthStartDate(date2);
      
      // Both should return first day of March
      expect(start1?.getDate()).toBe(1);
      expect(start2?.getDate()).toBe(1);
      expect(start1?.getMonth()).toBe(2);
      expect(start2?.getMonth()).toBe(2);
    });
  });
});
