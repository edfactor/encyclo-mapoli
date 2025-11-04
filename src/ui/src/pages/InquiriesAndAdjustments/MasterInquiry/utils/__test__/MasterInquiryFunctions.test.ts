import { describe, expect, it } from "vitest";
import { MasterInquirySearch } from "reduxstore/types";
import {
  isSimpleSearch,
  memberTypeGetNumberMap,
  paymentTypeGetNumberMap,
  splitFullPSN
} from "../MasterInquiryFunctions";

describe("MasterInquiryFunctions", () => {
  describe("paymentTypeGetNumberMap", () => {
    it("should have correct payment type mappings", () => {
      expect(paymentTypeGetNumberMap.all).toBe(0);
      expect(paymentTypeGetNumberMap.hardship).toBe(1);
      expect(paymentTypeGetNumberMap.payoffs).toBe(2);
      expect(paymentTypeGetNumberMap.rollovers).toBe(3);
    });
  });

  describe("memberTypeGetNumberMap", () => {
    it("should have correct member type mappings", () => {
      expect(memberTypeGetNumberMap.all).toBe(0);
      expect(memberTypeGetNumberMap.employees).toBe(1);
      expect(memberTypeGetNumberMap.beneficiaries).toBe(2);
      expect(memberTypeGetNumberMap.none).toBe(3);
    });
  });

  describe("isSimpleSearch", () => {
    it("should return true for search with only name", () => {
      const params: MasterInquirySearch = {
        name: "John Doe",
        endProfitYear: 2024,
        startProfitMonth: undefined,
        endProfitMonth: undefined,
        socialSecurity: undefined,
        badgeNumber: undefined,
        paymentType: "all",
        memberType: "all",
        contribution: undefined,
        earnings: undefined,
        forfeiture: undefined,
        payment: undefined,
        voids: false,
        pagination: { skip: 0, take: 5, sortBy: "name", isSortDescending: false }
      };

      expect(isSimpleSearch(params)).toBe(true);
    });

    it("should return true for search with only socialSecurity", () => {
      const params: MasterInquirySearch = {
        name: undefined,
        socialSecurity: "123456789",
        badgeNumber: undefined,
        endProfitYear: 2024,
        startProfitMonth: undefined,
        endProfitMonth: undefined,
        paymentType: "all",
        memberType: "all",
        contribution: undefined,
        earnings: undefined,
        forfeiture: undefined,
        payment: undefined,
        voids: false,
        pagination: { skip: 0, take: 5, sortBy: "name", isSortDescending: false }
      };

      expect(isSimpleSearch(params)).toBe(true);
    });

    it("should return true for search with only badgeNumber", () => {
      const params: MasterInquirySearch = {
        name: undefined,
        socialSecurity: undefined,
        badgeNumber: "12345",
        endProfitYear: 2024,
        startProfitMonth: undefined,
        endProfitMonth: undefined,
        paymentType: "all",
        memberType: "all",
        contribution: undefined,
        earnings: undefined,
        forfeiture: undefined,
        payment: undefined,
        voids: false,
        pagination: { skip: 0, take: 5, sortBy: "name", isSortDescending: false }
      };

      expect(isSimpleSearch(params)).toBe(true);
    });

    it("should return false for search with name and contribution", () => {
      const params: MasterInquirySearch = {
        name: "John Doe",
        contribution: 1000,
        socialSecurity: undefined,
        badgeNumber: undefined,
        endProfitYear: 2024,
        startProfitMonth: undefined,
        endProfitMonth: undefined,
        paymentType: "all",
        memberType: "all",
        earnings: undefined,
        forfeiture: undefined,
        payment: undefined,
        voids: false,
        pagination: { skip: 0, take: 5, sortBy: "name", isSortDescending: false }
      };

      expect(isSimpleSearch(params)).toBe(false);
    });

    it("should return false for search with name and startProfitMonth", () => {
      const params: MasterInquirySearch = {
        name: "John Doe",
        startProfitMonth: 1,
        socialSecurity: undefined,
        badgeNumber: undefined,
        endProfitYear: 2024,
        endProfitMonth: undefined,
        paymentType: "all",
        memberType: "all",
        contribution: undefined,
        earnings: undefined,
        forfeiture: undefined,
        payment: undefined,
        voids: false,
        pagination: { skip: 0, take: 5, sortBy: "name", isSortDescending: false }
      };

      expect(isSimpleSearch(params)).toBe(false);
    });

    it("should return false for search with name and earnings", () => {
      const params: MasterInquirySearch = {
        name: "John Doe",
        earnings: 5000,
        socialSecurity: undefined,
        badgeNumber: undefined,
        endProfitYear: 2024,
        startProfitMonth: undefined,
        endProfitMonth: undefined,
        paymentType: "all",
        memberType: "all",
        contribution: undefined,
        forfeiture: undefined,
        payment: undefined,
        voids: false,
        pagination: { skip: 0, take: 5, sortBy: "name", isSortDescending: false }
      };

      expect(isSimpleSearch(params)).toBe(false);
    });

    it("should return false for search with name and forfeiture", () => {
      const params: MasterInquirySearch = {
        name: "John Doe",
        forfeiture: 500,
        socialSecurity: undefined,
        badgeNumber: undefined,
        endProfitYear: 2024,
        startProfitMonth: undefined,
        endProfitMonth: undefined,
        paymentType: "all",
        memberType: "all",
        contribution: undefined,
        earnings: undefined,
        payment: undefined,
        voids: false,
        pagination: { skip: 0, take: 5, sortBy: "name", isSortDescending: false }
      };

      expect(isSimpleSearch(params)).toBe(false);
    });

    it("should return false for search with name and payment", () => {
      const params: MasterInquirySearch = {
        name: "John Doe",
        payment: 2000,
        socialSecurity: undefined,
        badgeNumber: undefined,
        endProfitYear: 2024,
        startProfitMonth: undefined,
        endProfitMonth: undefined,
        paymentType: "all",
        memberType: "all",
        contribution: undefined,
        earnings: undefined,
        forfeiture: undefined,
        voids: false,
        pagination: { skip: 0, take: 5, sortBy: "name", isSortDescending: false }
      };

      expect(isSimpleSearch(params)).toBe(false);
    });

    it("should return false when params is null", () => {
      expect(isSimpleSearch(null)).toBe(false);
    });

    it("should return false when no simple search fields are provided", () => {
      const params: MasterInquirySearch = {
        name: undefined,
        socialSecurity: undefined,
        badgeNumber: undefined,
        endProfitYear: 2024,
        startProfitMonth: undefined,
        endProfitMonth: undefined,
        paymentType: "all",
        memberType: "all",
        contribution: undefined,
        earnings: undefined,
        forfeiture: undefined,
        payment: undefined,
        voids: false,
        pagination: { skip: 0, take: 5, sortBy: "name", isSortDescending: false }
      };

      expect(isSimpleSearch(params)).toBe(false);
    });
  });

  describe("splitFullPSN", () => {
    it("should return undefined values for undefined input", () => {
      const result = splitFullPSN(undefined);

      expect(result.psnSuffix).toBeUndefined();
      expect(result.verifiedBadgeNumber).toBeUndefined();
    });

    it("should return undefined values for empty string", () => {
      const result = splitFullPSN("");

      expect(result.psnSuffix).toBeUndefined();
      expect(result.verifiedBadgeNumber).toBeUndefined();
    });

    it("should parse badge number without PSN suffix (7 digits or less)", () => {
      const result = splitFullPSN("1234567");

      expect(result.verifiedBadgeNumber).toBe(1234567);
      expect(result.psnSuffix).toBeUndefined();
    });

    it("should parse short badge number", () => {
      const result = splitFullPSN("12345");

      expect(result.verifiedBadgeNumber).toBe(12345);
      expect(result.psnSuffix).toBeUndefined();
    });

    it("should parse badge number with PSN suffix (more than 7 digits)", () => {
      const result = splitFullPSN("12345670001");

      expect(result.verifiedBadgeNumber).toBe(1234567);
      expect(result.psnSuffix).toBe(1);
    });

    it("should parse badge number with multi-digit PSN suffix", () => {
      const result = splitFullPSN("12345671234");

      expect(result.verifiedBadgeNumber).toBe(1234567);
      expect(result.psnSuffix).toBe(1234);
    });

    it("should handle 8 digit badge number (7 badge + 1 PSN)", () => {
      const result = splitFullPSN("12345678");

      expect(result.verifiedBadgeNumber).toBe(1234);
      expect(result.psnSuffix).toBe(5678);
    });

    it("should handle exactly 11 digits", () => {
      const result = splitFullPSN("99999999999");

      expect(result.verifiedBadgeNumber).toBe(9999999);
      expect(result.psnSuffix).toBe(9999);
    });

    it("should handle badge with leading zeros in PSN", () => {
      const result = splitFullPSN("12345670000");

      expect(result.verifiedBadgeNumber).toBe(1234567);
      expect(result.psnSuffix).toBe(0);
    });
  });
});
