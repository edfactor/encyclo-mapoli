import { describe, it, expect } from "vitest";
import {
  parseBadgeAndPSN,
  detectMemberTypeFromBadge,
  isValidBadgeIdentifiers,
  decomposePSNSuffix
} from "./badgeUtils";

describe("badgeUtils", () => {
  describe("parseBadgeAndPSN", () => {
    it("should parse single digit badge", () => {
      const result = parseBadgeAndPSN("5");
      expect(result).toEqual({ badge: 5 });
      expect(result.psn).toBeUndefined();
    });

    it("should parse 7-digit badge (standard employee badge)", () => {
      const result = parseBadgeAndPSN("1234567");
      expect(result).toEqual({ badge: 1234567 });
      expect(result.psn).toBeUndefined();
    });

    it("should parse 8-digit badge/PSN (employee badge + 1 digit suffix)", () => {
      const result = parseBadgeAndPSN("12345678");
      expect(result).toEqual({ badge: 1234567, psn: 8 });
    });

    it("should parse 9-digit PSN (employee badge + 2 digit suffix)", () => {
      const result = parseBadgeAndPSN("123456789");
      expect(result).toEqual({ badge: 1234567, psn: 89 });
    });

    it("should parse 10-digit PSN (employee badge + 3 digit suffix)", () => {
      const result = parseBadgeAndPSN("1234567890");
      expect(result).toEqual({ badge: 1234567, psn: 890 });
    });

    it("should handle numeric input", () => {
      const result = parseBadgeAndPSN(12345678);
      expect(result).toEqual({ badge: 1234567, psn: 8 });
    });

    it("should handle whitespace around input", () => {
      const result = parseBadgeAndPSN("  1234567  ");
      expect(result).toEqual({ badge: 1234567 });
    });

    it("should parse PSN starting with 0", () => {
      const result = parseBadgeAndPSN("12345670");
      expect(result).toEqual({ badge: 1234567, psn: 0 });
    });

    it("should handle boundary: exactly 7 digits (no PSN)", () => {
      const result = parseBadgeAndPSN("9999999");
      expect(result).toEqual({ badge: 9999999 });
      expect(result.psn).toBeUndefined();
    });

    it("should handle boundary: exactly 8 digits (has PSN)", () => {
      const result = parseBadgeAndPSN("99999999");
      expect(result).toEqual({ badge: 9999999, psn: 9 });
    });

    it("should handle large numbers", () => {
      const result = parseBadgeAndPSN("99999999999");
      expect(result).toEqual({ badge: 9999999, psn: 9999 });
    });

    it("should handle zero badge", () => {
      const result = parseBadgeAndPSN("0");
      expect(result).toEqual({ badge: 0 });
    });

    it("should parse real-world example 1", () => {
      const result = parseBadgeAndPSN("4567890");
      expect(result).toEqual({ badge: 4567890 });
    });

    it("should parse real-world example 2 (with PSN)", () => {
      const result = parseBadgeAndPSN("45678901");
      expect(result).toEqual({ badge: 4567890, psn: 1 });
    });
  });

  describe("detectMemberTypeFromBadge", () => {
    it("should return 0 (All) for empty string", () => {
      expect(detectMemberTypeFromBadge("")).toBe(0);
    });

    it("should return 0 (All) for whitespace only", () => {
      expect(detectMemberTypeFromBadge("   ")).toBe(0);
    });

    it("should return 1 (Employees) for single digit", () => {
      expect(detectMemberTypeFromBadge("5")).toBe(1);
    });

    it("should return 1 (Employees) for 7-digit badge", () => {
      expect(detectMemberTypeFromBadge("1234567")).toBe(1);
    });

    it("should return 2 (Beneficiaries) for 8-digit PSN", () => {
      expect(detectMemberTypeFromBadge("12345678")).toBe(2);
    });

    it("should return 2 (Beneficiaries) for 9-digit PSN", () => {
      expect(detectMemberTypeFromBadge("123456789")).toBe(2);
    });

    it("should return 2 (Beneficiaries) for 10-digit PSN", () => {
      expect(detectMemberTypeFromBadge("1234567890")).toBe(2);
    });

    it("should handle numeric input (7 digits = Employees)", () => {
      expect(detectMemberTypeFromBadge(1234567)).toBe(1);
    });

    it("should handle numeric input (8 digits = Beneficiaries)", () => {
      expect(detectMemberTypeFromBadge(12345678)).toBe(2);
    });

    it("should handle whitespace around input", () => {
      expect(detectMemberTypeFromBadge("  1234567  ")).toBe(1);
    });

    it("should return 2 for boundary case: exactly 8 digits", () => {
      expect(detectMemberTypeFromBadge("99999999")).toBe(2);
    });

    it("should return 1 for boundary case: exactly 7 digits", () => {
      expect(detectMemberTypeFromBadge("9999999")).toBe(1);
    });

    it("should return 2 for very large PSN", () => {
      expect(detectMemberTypeFromBadge("99999999999")).toBe(2);
    });

    it("should return 1 for zero", () => {
      expect(detectMemberTypeFromBadge("0")).toBe(1);
    });

    it("should return 2 for PSN starting with zero", () => {
      expect(detectMemberTypeFromBadge("12345670")).toBe(2);
    });
  });

  describe("isValidBadgeIdentifiers", () => {
    it("should return true for valid badge and PSN", () => {
      expect(isValidBadgeIdentifiers(1234567, 1)).toBe(true);
    });

    it("should return true for valid badge and PSN of 0", () => {
      expect(isValidBadgeIdentifiers(1234567, 0)).toBe(true);
    });

    it("should return false when badgeNumber is null", () => {
      expect(isValidBadgeIdentifiers(null, 1)).toBe(false);
    });

    it("should return false when badgeNumber is undefined", () => {
      expect(isValidBadgeIdentifiers(undefined, 1)).toBe(false);
    });

    it("should return false when badgeNumber is 0", () => {
      expect(isValidBadgeIdentifiers(0, 1)).toBe(false);
    });

    it("should return false when psnSuffix is null", () => {
      expect(isValidBadgeIdentifiers(1234567, null)).toBe(false);
    });

    it("should return false when psnSuffix is undefined", () => {
      expect(isValidBadgeIdentifiers(1234567, undefined)).toBe(false);
    });

    it("should return false when both are missing", () => {
      expect(isValidBadgeIdentifiers(undefined, undefined)).toBe(false);
    });

    it("should return false when both are null", () => {
      expect(isValidBadgeIdentifiers(null, null)).toBe(false);
    });

    it("should return true for large badge numbers", () => {
      expect(isValidBadgeIdentifiers(9999999, 9999)).toBe(true);
    });

    it("should return true for badge 1 and PSN 0", () => {
      expect(isValidBadgeIdentifiers(1, 0)).toBe(true);
    });

    it("should return false for badge 0 even with valid PSN", () => {
      expect(isValidBadgeIdentifiers(0, 5)).toBe(false);
    });

    it("should handle negative badge numbers", () => {
      expect(isValidBadgeIdentifiers(-1234567, 1)).toBe(true);
    });

    it("should handle negative PSN suffix", () => {
      expect(isValidBadgeIdentifiers(1234567, -1)).toBe(true);
    });

    it("should return false for NaN values", () => {
      expect(isValidBadgeIdentifiers(NaN, 1)).toBe(false);
    });
  });

  describe("decomposePSNSuffix", () => {
    it("should decompose 3-digit PSN", () => {
      const result = decomposePSNSuffix(123);
      expect(result).toEqual({
        firstLevel: 0,
        secondLevel: 1,
        thirdLevel: 2
      });
    });

    it("should decompose single digit PSN (only third level)", () => {
      const result = decomposePSNSuffix(5);
      expect(result).toEqual({
        firstLevel: 0,
        secondLevel: 0,
        thirdLevel: 0
      });
    });

    it("should decompose two-digit PSN", () => {
      const result = decomposePSNSuffix(45);
      expect(result).toEqual({
        firstLevel: 0,
        secondLevel: 0,
        thirdLevel: 4
      });
    });

    it("should decompose 4-digit PSN", () => {
      const result = decomposePSNSuffix(1234);
      expect(result).toEqual({
        firstLevel: 1,
        secondLevel: 2,
        thirdLevel: 3
      });
    });

    it("should decompose zero", () => {
      const result = decomposePSNSuffix(0);
      expect(result).toEqual({
        firstLevel: 0,
        secondLevel: 0,
        thirdLevel: 0
      });
    });

    it("should decompose PSN with all same digits", () => {
      const result = decomposePSNSuffix(1111);
      expect(result).toEqual({
        firstLevel: 1,
        secondLevel: 1,
        thirdLevel: 1
      });
    });

    it("should decompose PSN 999", () => {
      const result = decomposePSNSuffix(999);
      expect(result).toEqual({
        firstLevel: 0,
        secondLevel: 9,
        thirdLevel: 9
      });
    });

    it("should decompose PSN 9999", () => {
      const result = decomposePSNSuffix(9999);
      expect(result).toEqual({
        firstLevel: 9,
        secondLevel: 9,
        thirdLevel: 9
      });
    });

    it("should decompose PSN with leading zeros in result", () => {
      const result = decomposePSNSuffix(100);
      expect(result).toEqual({
        firstLevel: 0,
        secondLevel: 1,
        thirdLevel: 0
      });
    });

    it("should decompose very large PSN", () => {
      const result = decomposePSNSuffix(123456);
      expect(result).toEqual({
        firstLevel: 3,
        secondLevel: 4,
        thirdLevel: 5
      });
    });

    it("should decompose PSN 10 correctly", () => {
      const result = decomposePSNSuffix(10);
      expect(result).toEqual({
        firstLevel: 0,
        secondLevel: 0,
        thirdLevel: 1
      });
    });

    it("should decompose real-world example 1", () => {
      const result = decomposePSNSuffix(234);
      expect(result).toEqual({
        firstLevel: 0,
        secondLevel: 2,
        thirdLevel: 3
      });
    });

    it("should decompose real-world example 2", () => {
      const result = decomposePSNSuffix(5678);
      expect(result).toEqual({
        firstLevel: 5,
        secondLevel: 6,
        thirdLevel: 7
      });
    });

    it("should handle floating point by truncating", () => {
      const result = decomposePSNSuffix(123.9);
      expect(result).toEqual({
        firstLevel: 0,
        secondLevel: 1,
        thirdLevel: 2
      });
    });
  });
});
