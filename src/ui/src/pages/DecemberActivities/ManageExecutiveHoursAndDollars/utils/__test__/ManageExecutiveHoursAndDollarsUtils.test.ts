import { ExecutiveHoursAndDollarsRequestDto } from "../../../../../types/fiscal/executive";
import { isSimpleSearch } from "../ManageExecutiveHoursAndDollarsUtils";

describe("ManageExecutiveHoursAndDollarsUtils", () => {
  describe("isSimpleSearch", () => {
    it("should return true for badge number only search", () => {
      const searchParams: ExecutiveHoursAndDollarsRequestDto = {
        profitYear: 2024,
        badgeNumber: 123456,
        hasExecutiveHoursAndDollars: false,
        isMonthlyPayroll: false,
        pagination: {
          skip: 0,
          take: 25,
          sortBy: "fullName",
          isSortDescending: false
        }
      };

      expect(isSimpleSearch(searchParams)).toBe(true);
    });

    it("should return true for SSN only search", () => {
      const searchParams: ExecutiveHoursAndDollarsRequestDto = {
        profitYear: 2024,
        socialSecurity: 123456789,
        hasExecutiveHoursAndDollars: false,
        isMonthlyPayroll: false,
        pagination: {
          skip: 0,
          take: 25,
          sortBy: "fullName",
          isSortDescending: false
        }
      };

      expect(isSimpleSearch(searchParams)).toBe(true);
    });

    it("should return true for name only search", () => {
      const searchParams: ExecutiveHoursAndDollarsRequestDto = {
        profitYear: 2024,
        fullNameContains: "Smith",
        hasExecutiveHoursAndDollars: false,
        isMonthlyPayroll: false,
        pagination: {
          skip: 0,
          take: 25,
          sortBy: "fullName",
          isSortDescending: false
        }
      };

      expect(isSimpleSearch(searchParams)).toBe(true);
    });

    it("should return false for search with hasExecutiveHoursAndDollars checkbox", () => {
      const searchParams: ExecutiveHoursAndDollarsRequestDto = {
        profitYear: 2024,
        badgeNumber: 123456,
        hasExecutiveHoursAndDollars: true,
        isMonthlyPayroll: false,
        pagination: {
          skip: 0,
          take: 25,
          sortBy: "fullName",
          isSortDescending: false
        }
      };

      expect(isSimpleSearch(searchParams)).toBe(false);
    });

    it("should return false for search with isMonthlyPayroll checkbox", () => {
      const searchParams: ExecutiveHoursAndDollarsRequestDto = {
        profitYear: 2024,
        badgeNumber: 123456,
        hasExecutiveHoursAndDollars: false,
        isMonthlyPayroll: true,
        pagination: {
          skip: 0,
          take: 25,
          sortBy: "fullName",
          isSortDescending: false
        }
      };

      expect(isSimpleSearch(searchParams)).toBe(false);
    });

    it("should return false for search with both checkboxes enabled", () => {
      const searchParams: ExecutiveHoursAndDollarsRequestDto = {
        profitYear: 2024,
        badgeNumber: 123456,
        hasExecutiveHoursAndDollars: true,
        isMonthlyPayroll: true,
        pagination: {
          skip: 0,
          take: 25,
          sortBy: "fullName",
          isSortDescending: false
        }
      };

      expect(isSimpleSearch(searchParams)).toBe(false);
    });

    it("should return false for search with no criteria", () => {
      const searchParams: ExecutiveHoursAndDollarsRequestDto = {
        profitYear: 2024,
        hasExecutiveHoursAndDollars: false,
        isMonthlyPayroll: false,
        pagination: {
          skip: 0,
          take: 25,
          sortBy: "fullName",
          isSortDescending: false
        }
      };

      expect(isSimpleSearch(searchParams)).toBe(false);
    });

    it("should return false for null search params", () => {
      expect(isSimpleSearch(null)).toBe(false);
    });
  });
});
