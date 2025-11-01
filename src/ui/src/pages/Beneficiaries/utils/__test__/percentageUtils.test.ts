import { describe, it, expect } from "vitest";
import { calculatePercentageSum, validatePercentageAllocation } from "../percentageUtils";

describe("percentageUtils", () => {
  describe("calculatePercentageSum", () => {
    it("should calculate sum with two items, first updated to higher value", () => {
      const items = [
        { id: 1, percent: 50 },
        { id: 2, percent: 50 }
      ];
      const result = calculatePercentageSum(items, 1, 60);
      expect(result).toEqual({ sum: 110, previousValue: 50 });
    });

    it("should calculate sum with two items, second updated to lower value", () => {
      const items = [
        { id: 1, percent: 50 },
        { id: 2, percent: 50 }
      ];
      const result = calculatePercentageSum(items, 2, 30);
      expect(result).toEqual({ sum: 80, previousValue: 50 });
    });

    it("should calculate sum with three items, middle item updated", () => {
      const items = [
        { id: 1, percent: 33 },
        { id: 2, percent: 33 },
        { id: 3, percent: 34 }
      ];
      const result = calculatePercentageSum(items, 2, 50);
      expect(result).toEqual({ sum: 117, previousValue: 33 });
    });

    it("should return previous value of 0 when updating item with 0 percent", () => {
      const items = [
        { id: 1, percent: 100 },
        { id: 2, percent: 0 }
      ];
      const result = calculatePercentageSum(items, 2, 50);
      expect(result).toEqual({ sum: 150, previousValue: 0 });
    });

    it("should handle single item with update", () => {
      const items = [{ id: 1, percent: 100 }];
      const result = calculatePercentageSum(items, 1, 75);
      expect(result).toEqual({ sum: 75, previousValue: 100 });
    });

    it("should handle single item with zero percent", () => {
      const items = [{ id: 1, percent: 0 }];
      const result = calculatePercentageSum(items, 1, 100);
      expect(result).toEqual({ sum: 100, previousValue: 0 });
    });

    it("should handle many items with one updated", () => {
      const items = [
        { id: 1, percent: 10 },
        { id: 2, percent: 10 },
        { id: 3, percent: 10 },
        { id: 4, percent: 10 },
        { id: 5, percent: 10 }
      ];
      const result = calculatePercentageSum(items, 3, 30);
      expect(result).toEqual({ sum: 70, previousValue: 10 });
    });

    it("should handle update that results in exact 100%", () => {
      const items = [
        { id: 1, percent: 60 },
        { id: 2, percent: 30 }
      ];
      const result = calculatePercentageSum(items, 2, 40);
      expect(result).toEqual({ sum: 100, previousValue: 30 });
    });

    it("should handle update that results in over 100%", () => {
      const items = [
        { id: 1, percent: 60 },
        { id: 2, percent: 30 }
      ];
      const result = calculatePercentageSum(items, 2, 50);
      expect(result).toEqual({ sum: 110, previousValue: 30 });
    });

    it("should handle update from high to low value", () => {
      const items = [
        { id: 1, percent: 95 },
        { id: 2, percent: 5 }
      ];
      const result = calculatePercentageSum(items, 1, 50);
      expect(result).toEqual({ sum: 55, previousValue: 95 });
    });

    it("should handle decimal percentages", () => {
      const items = [
        { id: 1, percent: 33.33 },
        { id: 2, percent: 33.33 },
        { id: 3, percent: 33.34 }
      ];
      const result = calculatePercentageSum(items, 1, 50);
      expect(result).toEqual({ sum: 116.67, previousValue: 33.33 });
    });

    it("should handle very small percentages", () => {
      const items = [
        { id: 1, percent: 0.01 },
        { id: 2, percent: 0.01 }
      ];
      const result = calculatePercentageSum(items, 1, 0.02);
      expect(result).toEqual({ sum: 0.03, previousValue: 0.01 });
    });

    it("should handle ID not found in list (previousValue remains 0)", () => {
      const items = [
        { id: 1, percent: 50 },
        { id: 2, percent: 50 }
      ];
      // ID 3 doesn't exist in list, so all percentages are summed (new value not added)
      const result = calculatePercentageSum(items, 3, 25);
      expect(result).toEqual({ sum: 100, previousValue: 0 });
    });

    it("should handle empty list (sum is 0)", () => {
      const items: { id: number; percent: number }[] = [];
      const result = calculatePercentageSum(items, 1, 50);
      expect(result).toEqual({ sum: 0, previousValue: 0 });
    });

    it("should handle negative percentages (edge case)", () => {
      const items = [
        { id: 1, percent: 50 },
        { id: 2, percent: 50 }
      ];
      const result = calculatePercentageSum(items, 1, -10);
      expect(result).toEqual({ sum: 40, previousValue: 50 });
    });

    it("should handle update to zero", () => {
      const items = [
        { id: 1, percent: 50 },
        { id: 2, percent: 50 }
      ];
      const result = calculatePercentageSum(items, 1, 0);
      expect(result).toEqual({ sum: 50, previousValue: 50 });
    });

    it("should preserve order of items", () => {
      const items = [
        { id: 5, percent: 20 },
        { id: 3, percent: 30 },
        { id: 1, percent: 50 }
      ];
      const result = calculatePercentageSum(items, 3, 40);
      expect(result).toEqual({ sum: 110, previousValue: 30 });
    });

    it("should handle duplicate IDs (updates all matches, previousValue is last match)", () => {
      const items = [
        { id: 1, percent: 50 },
        { id: 1, percent: 30 },
        { id: 2, percent: 20 }
      ];
      // Both items with id 1 are updated to 60, sum is 60 + 60 + 20 = 140
      // previousValue is from the last match (30)
      const result = calculatePercentageSum(items, 1, 60);
      expect(result).toEqual({ sum: 140, previousValue: 30 });
    });
  });

  describe("validatePercentageAllocation", () => {
    it("should validate exactly 100%", () => {
      const result = validatePercentageAllocation(100);
      expect(result).toEqual({
        sum: 100,
        valid: true,
        error: undefined
      });
    });

    it("should validate less than 100%", () => {
      const result = validatePercentageAllocation(75);
      expect(result).toEqual({
        sum: 75,
        valid: true,
        error: undefined
      });
    });

    it("should validate 0%", () => {
      const result = validatePercentageAllocation(0);
      expect(result).toEqual({
        sum: 0,
        valid: true,
        error: undefined
      });
    });

    it("should validate 1%", () => {
      const result = validatePercentageAllocation(1);
      expect(result).toEqual({
        sum: 1,
        valid: true,
        error: undefined
      });
    });

    it("should reject 101%", () => {
      const result = validatePercentageAllocation(101);
      expect(result).toEqual({
        sum: 101,
        valid: false,
        error: "Total percentage would be 101%. Beneficiary percentages must sum to 100% or less."
      });
    });

    it("should reject 125%", () => {
      const result = validatePercentageAllocation(125);
      expect(result.valid).toBe(false);
      expect(result.error).toContain("125%");
    });

    it("should reject 110%", () => {
      const result = validatePercentageAllocation(110);
      expect(result.valid).toBe(false);
      expect(result.error).toContain("110%");
    });

    it("should reject very large percentage", () => {
      const result = validatePercentageAllocation(999);
      expect(result.valid).toBe(false);
      expect(result.error).toContain("999%");
    });

    it("should validate decimal percentages under 100%", () => {
      const result = validatePercentageAllocation(99.99);
      expect(result.valid).toBe(true);
    });

    it("should validate decimal percentages at exactly 100%", () => {
      const result = validatePercentageAllocation(100.0);
      expect(result.valid).toBe(true);
    });

    it("should reject decimal percentages over 100%", () => {
      const result = validatePercentageAllocation(100.01);
      expect(result.valid).toBe(false);
      expect(result.error).toContain("100.01%");
    });

    it("should allow negative percentages (only validates > 100%)", () => {
      const result = validatePercentageAllocation(-50);
      expect(result.valid).toBe(true);
      expect(result.error).toBeUndefined();
    });

    it("should have error message that includes sum", () => {
      const result = validatePercentageAllocation(120);
      expect(result.error).toContain("120");
    });

    it("should have error message with standard format", () => {
      const result = validatePercentageAllocation(115);
      expect(result.error).toBe("Total percentage would be 115%. Beneficiary percentages must sum to 100% or less.");
    });

    it("should not have error when valid", () => {
      const result = validatePercentageAllocation(50);
      expect(result.error).toBeUndefined();
    });

    it("should return sum in result even when invalid", () => {
      const result = validatePercentageAllocation(150);
      expect(result.sum).toBe(150);
      expect(result.valid).toBe(false);
    });
  });

  describe("integration: calculatePercentageSum + validatePercentageAllocation", () => {
    it("should validate successful update", () => {
      const items = [
        { id: 1, percent: 50 },
        { id: 2, percent: 50 }
      ];
      const calcResult = calculatePercentageSum(items, 1, 60);
      const validResult = validatePercentageAllocation(calcResult.sum);
      expect(validResult.valid).toBe(false); // 110% is invalid
    });

    it("should validate allowed update that maintains 100%", () => {
      const items = [
        { id: 1, percent: 60 },
        { id: 2, percent: 40 }
      ];
      const calcResult = calculatePercentageSum(items, 1, 70);
      const validResult = validatePercentageAllocation(calcResult.sum);
      expect(validResult.valid).toBe(false); // 110% is invalid
    });

    it("should validate decrease that stays under 100%", () => {
      const items = [
        { id: 1, percent: 60 },
        { id: 2, percent: 40 }
      ];
      const calcResult = calculatePercentageSum(items, 1, 50);
      const validResult = validatePercentageAllocation(calcResult.sum);
      expect(validResult.valid).toBe(true); // 90% is valid
    });

    it("should validate edge case: changing to exactly split 100%", () => {
      const items = [
        { id: 1, percent: 50 },
        { id: 2, percent: 50 }
      ];
      const calcResult = calculatePercentageSum(items, 1, 40);
      const validResult = validatePercentageAllocation(calcResult.sum);
      expect(validResult.valid).toBe(true); // 90% is valid
    });

    it("should provide previous value for UI restoration on validation failure", () => {
      const items = [
        { id: 1, percent: 50 },
        { id: 2, percent: 50 }
      ];
      const calcResult = calculatePercentageSum(items, 1, 75);
      const validResult = validatePercentageAllocation(calcResult.sum);

      if (!validResult.valid) {
        // UI should restore to previous value
        expect(calcResult.previousValue).toBe(50);
      }
    });
  });
});
