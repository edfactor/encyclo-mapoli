import { describe, expect, it } from "vitest";
import { MasterInquirySearch } from "reduxstore/types";
import { transformSearchParams } from "./transformSearchParams";

describe("transformSearchParams", () => {
  const baseSearchData: MasterInquirySearch = {
    endProfitYear: 2024,
    startProfitMonth: undefined,
    endProfitMonth: undefined,
    socialSecurity: undefined,
    name: undefined,
    badgeNumber: undefined,
    paymentType: "all",
    memberType: "all",
    contribution: undefined,
    earnings: undefined,
    forfeiture: undefined,
    payment: undefined,
    voids: false,
    pagination: { skip: 0, take: 5, sortBy: "badgeNumber", isSortDescending: true }
  };

  it("should transform basic search parameters", () => {
    const result = transformSearchParams(baseSearchData, 2024);

    expect(result.pagination).toEqual({
      skip: 0,
      take: 5,
      sortBy: "badgeNumber",
      isSortDescending: true
    });
    expect(result.endProfitYear).toBe(2024);
    expect(result._timestamp).toBeDefined();
  });

  it("should use default profit year when endProfitYear is not provided", () => {
    const data = { ...baseSearchData, endProfitYear: undefined };
    const result = transformSearchParams(data, 2023);

    expect(result.endProfitYear).toBe(2023);
  });

  it("should include SSN when provided", () => {
    const data = { ...baseSearchData, socialSecurity: "123456789" };
    const result = transformSearchParams(data, 2024);

    expect(result.ssn).toBe("123456789");
  });

  it("should include name when provided", () => {
    const data = { ...baseSearchData, name: "John Doe" };
    const result = transformSearchParams(data, 2024);

    expect(result.name).toBe("John Doe");
  });

  it("should split badge number without PSN suffix", () => {
    const data = { ...baseSearchData, badgeNumber: "1234567" };
    const result = transformSearchParams(data, 2024);

    expect(result.badgeNumber).toBe(1234567);
    expect(result.psnSuffix).toBeUndefined();
  });

  it("should split badge number with PSN suffix", () => {
    const data = { ...baseSearchData, badgeNumber: "12345670001" };
    const result = transformSearchParams(data, 2024);

    expect(result.badgeNumber).toBe(1234567);
    expect(result.psnSuffix).toBe(1);
  });

  it("should include startProfitMonth when provided", () => {
    const data = { ...baseSearchData, startProfitMonth: "3" };
    const result = transformSearchParams(data, 2024);

    expect(result.startProfitMonth).toBe("3");
  });

  it("should include endProfitMonth when provided", () => {
    const data = { ...baseSearchData, endProfitMonth: "9" };
    const result = transformSearchParams(data, 2024);

    expect(result.endProfitMonth).toBe("9");
  });

  it("should map paymentType to number", () => {
    const hardshipData = { ...baseSearchData, paymentType: "hardship" as const };
    const result1 = transformSearchParams(hardshipData, 2024);
    expect(result1.paymentType).toBe(1);

    const payoffsData = { ...baseSearchData, paymentType: "payoffs" as const };
    const result2 = transformSearchParams(payoffsData, 2024);
    expect(result2.paymentType).toBe(2);

    const rolloversData = { ...baseSearchData, paymentType: "rollovers" as const };
    const result3 = transformSearchParams(rolloversData, 2024);
    expect(result3.paymentType).toBe(3);
  });

  it("should map memberType to number", () => {
    const employeesData = { ...baseSearchData, memberType: "employees" as const };
    const result1 = transformSearchParams(employeesData, 2024);
    expect(result1.memberType).toBe(1);

    const beneficiariesData = { ...baseSearchData, memberType: "beneficiaries" as const };
    const result2 = transformSearchParams(beneficiariesData, 2024);
    expect(result2.memberType).toBe(2);

    const noneData = { ...baseSearchData, memberType: "none" as const };
    const result3 = transformSearchParams(noneData, 2024);
    expect(result3.memberType).toBe(3);
  });

  it("should include contribution when provided", () => {
    const data = { ...baseSearchData, contribution: "1000" };
    const result = transformSearchParams(data, 2024);

    expect(result.contributionAmount).toBe("1000");
  });

  it("should include earnings when provided", () => {
    const data = { ...baseSearchData, earnings: "5000" };
    const result = transformSearchParams(data, 2024);

    expect(result.earningsAmount).toBe("5000");
  });

  it("should include forfeiture when provided", () => {
    const data = { ...baseSearchData, forfeiture: "500" };
    const result = transformSearchParams(data, 2024);

    expect(result.forfeitureAmount).toBe("500");
  });

  it("should include payment when provided", () => {
    const data = { ...baseSearchData, payment: "2000" };
    const result = transformSearchParams(data, 2024);

    expect(result.paymentAmount).toBe("2000");
  });

  it("should use default pagination values when not provided", () => {
    const data = { ...baseSearchData, pagination: undefined };
    const result = transformSearchParams(data, 2024);

    expect(result.pagination).toEqual({
      skip: 0,
      take: 5,
      sortBy: "badgeNumber",
      isSortDescending: true
    });
  });

  it("should use provided pagination values", () => {
    const data = {
      ...baseSearchData,
      pagination: {
        skip: 10,
        take: 25,
        sortBy: "name",
        isSortDescending: false
      }
    };
    const result = transformSearchParams(data, 2024);

    expect(result.pagination).toEqual({
      skip: 10,
      take: 25,
      sortBy: "name",
      isSortDescending: false
    });
  });

  it("should handle complex search with multiple fields", () => {
    const data: MasterInquirySearch = {
      endProfitYear: 2024,
      startProfitMonth: "1",
      endProfitMonth: "12",
      socialSecurity: "987654321",
      name: "Jane Smith",
      badgeNumber: "12345670002",
      paymentType: "hardship",
      memberType: "employees",
      contribution: "1500",
      earnings: "6000",
      forfeiture: "750",
      payment: "3000",
      voids: true,
      pagination: {
        skip: 20,
        take: 50,
        sortBy: "ssn",
        isSortDescending: false
      }
    };

    const result = transformSearchParams(data, 2024);

    expect(result).toMatchObject({
      endProfitYear: 2024,
      startProfitMonth: "1",
      endProfitMonth: "12",
      ssn: "987654321",
      name: "Jane Smith",
      badgeNumber: 1234567,
      psnSuffix: 2,
      paymentType: 1,
      memberType: 1,
      contributionAmount: "1500",
      earningsAmount: "6000",
      forfeitureAmount: "750",
      paymentAmount: "3000",
      pagination: {
        skip: 20,
        take: 50,
        sortBy: "ssn",
        isSortDescending: false
      }
    });
    expect(result._timestamp).toBeDefined();
  });

  it("should not include optional fields when they are undefined", () => {
    const result = transformSearchParams(baseSearchData, 2024);

    expect(result).not.toHaveProperty("ssn");
    expect(result).not.toHaveProperty("name");
    expect(result).not.toHaveProperty("startProfitMonth");
    expect(result).not.toHaveProperty("endProfitMonth");
    expect(result).not.toHaveProperty("contributionAmount");
    expect(result).not.toHaveProperty("earningsAmount");
    expect(result).not.toHaveProperty("forfeitureAmount");
    expect(result).not.toHaveProperty("paymentAmount");
  });

  it("should not include psnSuffix when badge number is short", () => {
    const data = { ...baseSearchData, badgeNumber: "12345" };
    const result = transformSearchParams(data, 2024);

    expect(result.badgeNumber).toBe(12345);
    expect(result).not.toHaveProperty("psnSuffix");
  });

  it("should handle numeric badge number", () => {
    const data = { ...baseSearchData, badgeNumber: 9876543 as any };
    const result = transformSearchParams(data, 2024);

    expect(result.badgeNumber).toBe(9876543);
    expect(result).not.toHaveProperty("psnSuffix");
  });

  it("should generate unique timestamps", async () => {
    const result1 = transformSearchParams(baseSearchData, 2024);
    await new Promise((resolve) => setTimeout(resolve, 10));
    const result2 = transformSearchParams(baseSearchData, 2024);

    expect(result1._timestamp).toBeDefined();
    expect(result2._timestamp).toBeDefined();
    expect(result2._timestamp).toBeGreaterThan(result1._timestamp);
  });
});
