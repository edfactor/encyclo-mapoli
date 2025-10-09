import { describe, expect, it } from "vitest";
import {
  DATE_FORMAT_YYYYMMDD,
  DATE_TIME_FORMAT_MMDDYYYY_HHMMSS,
  dateMMDDYYYY,
  getMaskedDateDisplay,
  isMaskedDate,
  MASKED_DATE_PATTERN,
  mmDDYYFormat,
  mmDDYYYY_HHMMSS_Format,
  tryddmmyyyyToDate
} from "./dateUtils";

describe("dateUtils", () => {
  describe("dateMMDDYYYY", () => {
    it("should format Date object to MM/dd/yyyy", () => {
      const date = new Date(2024, 0, 15); // January 15, 2024
      const result = dateMMDDYYYY(date);
      expect(result).toBe("01/15/2024");
    });

    it("should return current date in MM/dd/yyyy format when date is undefined", () => {
      const result = dateMMDDYYYY(undefined);
      const today = new Date();
      const month = String(today.getMonth() + 1).padStart(2, "0");
      const day = String(today.getDate()).padStart(2, "0");
      const year = today.getFullYear();
      expect(result).toBe(`${month}/${day}/${year}`);
    });

    it("should handle leap year dates", () => {
      const date = new Date(2024, 1, 29); // February 29, 2024 (leap year)
      const result = dateMMDDYYYY(date);
      expect(result).toBe("02/29/2024");
    });

    it("should handle end of year dates", () => {
      const date = new Date(2023, 11, 31); // December 31, 2023
      const result = dateMMDDYYYY(date);
      expect(result).toBe("12/31/2023");
    });
  });

  describe("mmDDYYFormat", () => {
    it("should format Date object to MM/dd/yyyy", () => {
      const date = new Date(2024, 5, 20); // June 20, 2024
      const result = mmDDYYFormat(date);
      expect(result).toBe("06/20/2024");
    });

    it("should format ISO string to MM/dd/yyyy", () => {
      const result = mmDDYYFormat("2024-03-15");
      expect(result).toBe("03/15/2024");
    });

    it("should format MM/dd/yyyy string to MM/dd/yyyy", () => {
      const result = mmDDYYFormat("07/04/2024");
      expect(result).toBe("07/04/2024");
    });

    it("should return empty string for undefined", () => {
      const result = mmDDYYFormat(undefined);
      expect(result).toBe("");
    });

    it("should return empty string for null", () => {
      const result = mmDDYYFormat(null);
      expect(result).toBe("");
    });

    it("should return empty string for invalid date string", () => {
      const result = mmDDYYFormat("invalid-date");
      expect(result).toBe("");
    });
  });

  describe("mmDDYYYY_HHMMSS_Format", () => {
    it("should format Date object with time", () => {
      const date = new Date(2024, 0, 15, 14, 30, 45);
      const result = mmDDYYYY_HHMMSS_Format(date);
      expect(result).toBe("01/15/2024 14:30:45");
    });

    it("should format ISO datetime string with time", () => {
      const result = mmDDYYYY_HHMMSS_Format("2024-03-15T10:25:30");
      expect(result).toBe("03/15/2024 10:25:30");
    });

    it("should format ISO datetime string with timezone", () => {
      const result = mmDDYYYY_HHMMSS_Format("2024-06-20T18:45:00Z");
      expect(result).toMatch(/06\/20\/2024 \d{2}:\d{2}:\d{2}/);
    });

    it("should return empty string for undefined", () => {
      const result = mmDDYYYY_HHMMSS_Format(undefined);
      expect(result).toBe("");
    });

    it("should return empty string for invalid date string", () => {
      const result = mmDDYYYY_HHMMSS_Format("not-a-date");
      expect(result).toBe("");
    });

    it("should handle midnight times", () => {
      const date = new Date(2024, 0, 1, 0, 0, 0);
      const result = mmDDYYYY_HHMMSS_Format(date);
      expect(result).toBe("01/01/2024 00:00:00");
    });
  });

  describe("tryddmmyyyyToDate", () => {
    it("should return null for undefined", () => {
      const result = tryddmmyyyyToDate(undefined);
      expect(result).toBeNull();
    });

    it("should return null for null", () => {
      const result = tryddmmyyyyToDate(null);
      expect(result).toBeNull();
    });

    it("should return null for DATE_FORMAT_YYYYMMDD constant", () => {
      const result = tryddmmyyyyToDate(DATE_FORMAT_YYYYMMDD);
      expect(result).toBeNull();
    });

    it("should handle Date object and apply startOfDay", () => {
      const date = new Date(2024, 5, 20, 14, 30, 45);
      const result = tryddmmyyyyToDate(date);
      expect(result).toEqual(new Date(2024, 5, 20, 0, 0, 0, 0));
    });

    it("should parse yyyy-MM-dd format (DateOnly)", () => {
      const result = tryddmmyyyyToDate("2024-06-20");
      expect(result).toEqual(new Date(2024, 5, 20, 0, 0, 0, 0));
    });

    it("should parse yyyy-MM-ddTHH:mm:ss format without timezone", () => {
      const result = tryddmmyyyyToDate("2024-06-20T14:30:45");
      expect(result).toEqual(new Date(2024, 5, 20, 0, 0, 0, 0));
    });

    it("should parse MM/dd/yyyy format", () => {
      const result = tryddmmyyyyToDate("06/20/2024");
      expect(result).toEqual(new Date(2024, 5, 20, 0, 0, 0, 0));
    });

    it("should parse ISO format with timezone", () => {
      const result = tryddmmyyyyToDate("2024-06-20T00:00:00Z");
      expect(result).not.toBeNull();
      expect(result?.getFullYear()).toBe(2024);
      expect(result?.getMonth()).toBe(5); // June
      // Date may be 19 or 20 depending on timezone offset
      expect(result?.getDate()).toBeGreaterThanOrEqual(19);
      expect(result?.getDate()).toBeLessThanOrEqual(20);
    });

    it("should parse JavaScript toString format", () => {
      const testDate = new Date(2024, 0, 15);
      const result = tryddmmyyyyToDate(testDate.toString());
      expect(result).not.toBeNull();
      expect(result?.getFullYear()).toBe(2024);
      expect(result?.getMonth()).toBe(0); // January
      expect(result?.getDate()).toBe(15);
    });

    it("should return null for invalid date string", () => {
      const result = tryddmmyyyyToDate("not-a-valid-date");
      expect(result).toBeNull();
    });

    it("should handle leap year dates", () => {
      const result = tryddmmyyyyToDate("2024-02-29");
      expect(result).toEqual(new Date(2024, 1, 29, 0, 0, 0, 0));
    });

    it("should handle single-digit month and day in MM/dd/yyyy", () => {
      const result = tryddmmyyyyToDate("01/05/2024");
      expect(result).toEqual(new Date(2024, 0, 5, 0, 0, 0, 0));
    });

    it("should return start of day for all valid formats", () => {
      const formats = ["2024-06-20", "06/20/2024", new Date(2024, 5, 20, 14, 30, 45)];

      formats.forEach((format) => {
        const result = tryddmmyyyyToDate(format);
        expect(result?.getHours()).toBe(0);
        expect(result?.getMinutes()).toBe(0);
        expect(result?.getSeconds()).toBe(0);
        expect(result?.getMilliseconds()).toBe(0);
      });
    });
  });

  describe("constants", () => {
    it("should export DATE_TIME_FORMAT_MMDDYYYY_HHMMSS constant", () => {
      expect(DATE_TIME_FORMAT_MMDDYYYY_HHMMSS).toBe("MM/dd/yyyy HH:mm:ss");
    });

    it("should export DATE_FORMAT_YYYYMMDD constant", () => {
      expect(DATE_FORMAT_YYYYMMDD).toBe("yyyy-MM-dd");
    });

    it("should export MASKED_DATE_PATTERN constant", () => {
      expect(MASKED_DATE_PATTERN).toBe("XXXX-XX-XX");
    });
  });

  describe("isMaskedDate", () => {
    it("should return true for XXXX-XX-XX pattern", () => {
      expect(isMaskedDate("XXXX-XX-XX")).toBe(true);
    });

    it("should return true for dates containing XXXX", () => {
      expect(isMaskedDate("XXXX-12-25")).toBe(true);
      expect(isMaskedDate("2024-XX-XX")).toBe(true);
    });

    it("should return true for dates containing XX-XX", () => {
      expect(isMaskedDate("2024-XX-XX")).toBe(true);
    });

    it("should return false for valid date strings", () => {
      expect(isMaskedDate("2024-01-15")).toBe(false);
      expect(isMaskedDate("01/15/2024")).toBe(false);
    });

    it("should return false for Date objects", () => {
      const date = new Date(2024, 0, 15);
      expect(isMaskedDate(date)).toBe(false);
    });

    it("should return false for null, undefined, or empty strings", () => {
      expect(isMaskedDate(null)).toBe(false);
      expect(isMaskedDate(undefined)).toBe(false);
      expect(isMaskedDate("")).toBe(false);
    });
  });

  describe("getMaskedDateDisplay", () => {
    it("should return XX/XX/XXXX pattern by default", () => {
      expect(getMaskedDateDisplay()).toBe("XX/XX/XXXX");
    });

    it("should return XX/XX/XXXX pattern when useXPattern is true", () => {
      expect(getMaskedDateDisplay(true)).toBe("XX/XX/XXXX");
    });

    it("should return N/A when useXPattern is false", () => {
      expect(getMaskedDateDisplay(false)).toBe("N/A");
    });
  });

  describe("masked date handling integration", () => {
    describe("mmDDYYFormat with masked dates", () => {
      it("should return masked display for XXXX-XX-XX", () => {
        expect(mmDDYYFormat("XXXX-XX-XX")).toBe("XX/XX/XXXX");
      });

      it("should return masked display for partially masked dates", () => {
        expect(mmDDYYFormat("2024-XX-XX")).toBe("XX/XX/XXXX");
      });

      it("should still format valid dates correctly", () => {
        expect(mmDDYYFormat("2024-03-15")).toBe("03/15/2024");
      });
    });

    describe("dateMMDDYYYY with masked dates", () => {
      it("should return masked display for XXXX-XX-XX", () => {
        expect(dateMMDDYYYY("XXXX-XX-XX")).toBe("XX/XX/XXXX");
      });

      it("should still format Date objects correctly", () => {
        const date = new Date(2024, 0, 15);
        expect(dateMMDDYYYY(date)).toBe("01/15/2024");
      });

      it("should still format valid date strings correctly", () => {
        expect(dateMMDDYYYY("2024-03-15")).toBe("03/15/2024");
      });
    });

    describe("mmDDYYYY_HHMMSS_Format with masked dates", () => {
      it("should return masked display for XXXX-XX-XX", () => {
        expect(mmDDYYYY_HHMMSS_Format("XXXX-XX-XX")).toBe("XX/XX/XXXX");
      });

      it("should still format valid dates correctly", () => {
        const result = mmDDYYYY_HHMMSS_Format("2024-03-15T14:30:45");
        expect(result).toContain("03/15/2024");
      });
    });

    describe("tryddmmyyyyToDate with masked dates", () => {
      it("should return null for XXXX-XX-XX", () => {
        expect(tryddmmyyyyToDate("XXXX-XX-XX")).toBe(null);
      });

      it("should return null for partially masked dates", () => {
        expect(tryddmmyyyyToDate("2024-XX-XX")).toBe(null);
      });

      it("should still parse valid dates correctly", () => {
        const result = tryddmmyyyyToDate("2024-03-15");
        expect(result).not.toBe(null);
        expect(result?.getFullYear()).toBe(2024);
        expect(result?.getMonth()).toBe(2); // March (0-indexed)
        expect(result?.getDate()).toBe(15);
      });
    });
  });
});
