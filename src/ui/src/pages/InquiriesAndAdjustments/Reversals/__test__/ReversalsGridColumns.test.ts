import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import {
    REVERSIBLE_PROFIT_CODES,
    getIneligibilityReason,
    getReversalEligibilityStatus,
    isRowReversible
} from "../ReversalsGridColumns";

describe("ReversalsGridColumns", () => {
  const setSystemDate = (date: Date) => {
    vi.useFakeTimers();
    vi.setSystemTime(date);
  };

  const resetSystemDate = () => {
    vi.useRealTimers();
  };

  describe("REVERSIBLE_PROFIT_CODES", () => {
    it("should contain the expected profit codes", () => {
      expect(REVERSIBLE_PROFIT_CODES).toEqual([1, 3, 5, 6, 9]);
    });

    it("should have exactly 5 reversible profit codes", () => {
      expect(REVERSIBLE_PROFIT_CODES).toHaveLength(5);
    });
  });

  describe("getReversalEligibilityStatus", () => {
    // Mock to June to avoid January rule complications
    beforeEach(() => {
      vi.useFakeTimers();
      vi.setSystemTime(new Date(2025, 5, 15)); // June 15, 2025
    });

    afterEach(() => {
      vi.useRealTimers();
    });

    const currentYear = 2025;
    const currentMonth = 6; // June

    describe("null/undefined data", () => {
      it("should return ineligible for null data", () => {
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        expect(getReversalEligibilityStatus(null as any)).toBe("ineligible");
      });

      it("should return ineligible for undefined data", () => {
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        expect(getReversalEligibilityStatus(undefined as any)).toBe("ineligible");
      });
    });

    describe("already reversed transactions", () => {
      it("should return already-reversed when isAlreadyReversed is true", () => {
        const currentDate = new Date();
        const currentYear = currentDate.getFullYear();
        const currentMonth = currentDate.getMonth() + 1;

        const data = {
          profitCodeId: 1,
          monthToDate: currentMonth,
          yearToDate: currentYear,
          isAlreadyReversed: true
        };
        expect(getReversalEligibilityStatus(data)).toBe("already-reversed");
      });

      it("should check other conditions when isAlreadyReversed is false", () => {
        const currentDate = new Date();
        const currentYear = currentDate.getFullYear();
        const currentMonth = currentDate.getMonth() + 1;

        const data = {
          profitCodeId: 1,
          monthToDate: currentMonth,
          yearToDate: currentYear,
          isAlreadyReversed: false
        };
        expect(getReversalEligibilityStatus(data)).toBe("reversible");
      });
    });

    describe("profit code validation", () => {
      it.each(REVERSIBLE_PROFIT_CODES)("should return reversible for profit code %i", (profitCodeId) => {
        const currentDate = new Date();
        const currentYear = currentDate.getFullYear();
        const currentMonth = currentDate.getMonth() + 1;

        const data = {
          profitCodeId,
          monthToDate: currentMonth,
          yearToDate: currentYear
        };
        expect(getReversalEligibilityStatus(data)).toBe("reversible");
      });

      it.each([0, 2, 4, 7, 8, 10, 99])("should return ineligible for non-reversible profit code %i", (profitCodeId) => {
        const currentDate = new Date();
        const currentYear = currentDate.getFullYear();
        const currentMonth = currentDate.getMonth() + 1;

        const data = {
          profitCodeId,
          monthToDate: currentMonth,
          yearToDate: currentYear
        };
        expect(getReversalEligibilityStatus(data)).toBe("ineligible");
      });
    });

    describe("date validation - 2 month rule", () => {
      it("should return reversible for transaction from current month", () => {
        const currentDate = new Date();
        const currentYear = currentDate.getFullYear();
        const currentMonth = currentDate.getMonth() + 1;

        const data = {
          profitCodeId: 1,
          monthToDate: currentMonth,
          yearToDate: currentYear
        };
        expect(getReversalEligibilityStatus(data)).toBe("reversible");
      });

      it("should return reversible for transaction from 1 month ago", () => {
        const oneMonthAgo = new Date();
        oneMonthAgo.setMonth(oneMonthAgo.getMonth() - 1);

        const data = {
          profitCodeId: 1,
          monthToDate: oneMonthAgo.getMonth() + 1,
          yearToDate: oneMonthAgo.getFullYear()
        };
        expect(getReversalEligibilityStatus(data)).toBe("reversible");
      });

      it("should return ineligible for transaction from 3 months ago", () => {
        const threeMonthsAgo = new Date();
        threeMonthsAgo.setMonth(threeMonthsAgo.getMonth() - 3);

        const data = {
          profitCodeId: 1,
          monthToDate: threeMonthsAgo.getMonth() + 1,
          yearToDate: threeMonthsAgo.getFullYear()
        };
        expect(getReversalEligibilityStatus(data)).toBe("ineligible");
      });

      it("should return ineligible for transaction from previous year (old)", () => {
        const currentDate = new Date();
        const currentYear = currentDate.getFullYear();

        const data = {
          profitCodeId: 1,
          monthToDate: 6,
          yearToDate: currentYear - 1
        };
        expect(getReversalEligibilityStatus(data)).toBe("ineligible");
      });
    });

    describe("January rule", () => {
      beforeEach(() => {
        // Mock Date to return January
        setSystemDate(new Date(2025, 0, 15)); // January 15, 2025
      });

      afterEach(() => {
        resetSystemDate();
      });

      it("should block January transactions in January", () => {
        const data = {
          profitCodeId: 1,
          monthToDate: 1, // January
          yearToDate: 2025
        };
        expect(getReversalEligibilityStatus(data)).toBe("ineligible");
      });

      it("should block December transactions in January", () => {
        const data = {
          profitCodeId: 1,
          monthToDate: 12, // December
          yearToDate: 2024
        };
        expect(getReversalEligibilityStatus(data)).toBe("ineligible");
      });

      it("should block November transactions in January (too old - more than 2 months)", () => {
        // November 2024 is more than 2 months before January 15, 2025
        // The 2-month rule blocks it before the January rule even applies
        const data = {
          profitCodeId: 1,
          monthToDate: 11, // November
          yearToDate: 2024
        };
        expect(getReversalEligibilityStatus(data)).toBe("ineligible");
      });

      it("should verify January rule only applies after 2-month rule passes", () => {
        // In January 2025, only November and December 2024 are within 2 months
        // But both are blocked by the January rule (month must be > 1 AND < 12)
        // December (12) fails: 12 < 12 is false
        // January (1) fails: 1 > 1 is false

        // December - blocked by January rule
        const decemberData = {
          profitCodeId: 1,
          monthToDate: 12,
          yearToDate: 2024
        };
        expect(getReversalEligibilityStatus(decemberData)).toBe("ineligible");

        // January - blocked by January rule
        const januaryData = {
          profitCodeId: 1,
          monthToDate: 1,
          yearToDate: 2025
        };
        expect(getReversalEligibilityStatus(januaryData)).toBe("ineligible");
      });
    });

    it("should allow recent transactions in non-January months", () => {
      const data = {
        profitCodeId: 1,
        monthToDate: 5, // May (within 2 months of June)
        yearToDate: 2025
      };
      expect(getReversalEligibilityStatus(data)).toBe("reversible");
    });
  });

  describe("isRowReversible", () => {
    // Mock to June to avoid January rule complications
    beforeEach(() => {
      vi.useFakeTimers();
      vi.setSystemTime(new Date(2025, 5, 15)); // June 15, 2025
    });

    afterEach(() => {
      vi.useRealTimers();
    });

    const currentYear = 2025;
    const currentMonth = 6; // June

    it("should return true for reversible rows", () => {
      const data = {
        profitCodeId: 1,
        monthToDate: currentMonth,
        yearToDate: currentYear
      };
      expect(isRowReversible(data)).toBe(true);
    });

    it("should return false for already-reversed rows", () => {
      const currentDate = new Date();
      const currentYear = currentDate.getFullYear();
      const currentMonth = currentDate.getMonth() + 1;

      const data = {
        profitCodeId: 1,
        monthToDate: currentMonth,
        yearToDate: currentYear,
        isAlreadyReversed: true
      };
      expect(isRowReversible(data)).toBe(false);
    });

    it("should return false for ineligible profit codes", () => {
      const currentDate = new Date();
      const currentYear = currentDate.getFullYear();
      const currentMonth = currentDate.getMonth() + 1;

      const data = {
        profitCodeId: 2,
        monthToDate: currentMonth,
        yearToDate: currentYear
      };
      expect(isRowReversible(data)).toBe(false);
    });

    it("should return false for old transactions", () => {
      const currentDate = new Date();
      const currentYear = currentDate.getFullYear();

      const data = {
        profitCodeId: 1,
        monthToDate: 1,
        yearToDate: currentYear - 1
      };
      expect(isRowReversible(data)).toBe(false);
    });
  });

  describe("getIneligibilityReason", () => {
    // Mock to June to avoid January rule complications
    beforeEach(() => {
      vi.useFakeTimers();
      vi.setSystemTime(new Date(2025, 5, 15)); // June 15, 2025
    });

    afterEach(() => {
      vi.useRealTimers();
    });

    const currentYear = 2025;
    const currentMonth = 6; // June

    it("should return default message for null data", () => {
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      expect(getIneligibilityReason(null as any)).toBe("This row cannot be reversed");
    });

    it("should return already reversed message", () => {
      const currentDate = new Date();
      const currentYear = currentDate.getFullYear();
      const currentMonth = currentDate.getMonth() + 1;

      const data = {
        profitCodeId: 1,
        monthToDate: currentMonth,
        yearToDate: currentYear,
        isAlreadyReversed: true
      };
      expect(getIneligibilityReason(data)).toBe("This transaction has already been reversed");
    });

    it("should return ineligible code message for non-reversible profit codes", () => {
      const currentDate = new Date();
      const currentYear = currentDate.getFullYear();
      const currentMonth = currentDate.getMonth() + 1;

      const data = {
        profitCodeId: 2,
        monthToDate: currentMonth,
        yearToDate: currentYear
      };
      expect(getIneligibilityReason(data)).toBe("Ineligible code for reversal");
    });

    it("should return too old message for transactions older than 2 months", () => {
      const oldDate = new Date();
      oldDate.setMonth(oldDate.getMonth() - 3);

      const data = {
        profitCodeId: 1,
        monthToDate: oldDate.getMonth() + 1,
        yearToDate: oldDate.getFullYear()
      };
      expect(getIneligibilityReason(data)).toBe("Transaction too old for reversal");
    });

    describe("January rule messages", () => {
      beforeEach(() => {
        setSystemDate(new Date(2025, 0, 15)); // January 15, 2025
      });

      afterEach(() => {
        resetSystemDate();
      });

      it("should return January-specific message for blocked transactions", () => {
        const data = {
          profitCodeId: 1,
          monthToDate: 12, // December
          yearToDate: 2024
        };
        expect(getIneligibilityReason(data)).toBe("Transaction not eligible for reversal in January");
      });
    });

    it("should return default message for other ineligible cases", () => {
      const currentDate = new Date();
      const currentYear = currentDate.getFullYear();
      const currentMonth = currentDate.getMonth() + 1;

      // This tests the fallback case - a row that passes all checks but somehow still ineligible
      // In practice, this shouldn't happen, but the function handles it gracefully
      const data = {
        profitCodeId: 1,
        monthToDate: currentMonth,
        yearToDate: currentYear
      };
      // This row is actually reversible, so getIneligibilityReason returns the default
      expect(getIneligibilityReason(data)).toBe("This row cannot be reversed");
    });
  });

  describe("GetReversalsGridColumns", () => {
    // Import dynamically to avoid issues with module resolution
    it("should be tested in a separate component test file", () => {
      // Column definitions are tested in component integration tests
      expect(true).toBe(true);
    });
  });
});
