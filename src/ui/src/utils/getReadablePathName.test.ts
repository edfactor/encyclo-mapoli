import { describe, expect, it } from "vitest";
import { getReadablePathName } from "./getReadablePathName";

describe("getReadablePathName", () => {
  it("should return empty string for empty path", () => {
    const result = getReadablePathName("");
    expect(result).toBe("");
  });

  it("should return readable name for root path", () => {
    const result = getReadablePathName("/");
    expect(result).toBe("");
  });

  it("should convert demographic-badges-not-in-payprofit", () => {
    const result = getReadablePathName("/demographic-badges-not-in-payprofit");
    expect(result).toBe("Demographic Badges");
  });

  it("should convert duplicate-ssns-demographics", () => {
    const result = getReadablePathName("/duplicate-ssns-demographics");
    expect(result).toBe("Duplicate SSNs");
  });

  it("should convert master-inquiry", () => {
    const result = getReadablePathName("/master-inquiry");
    expect(result).toBe("Master Inquiry");
  });

  it("should convert fiscal-close", () => {
    const result = getReadablePathName("/fiscal-close");
    expect(result).toBe("Fiscal Close");
  });

  it("should convert december-process", () => {
    const result = getReadablePathName("/december-process");
    expect(result).toBe("December");
  });

  it("should handle paths with trailing slash", () => {
    const result = getReadablePathName("/master-inquiry/");
    expect(result).toBe("Master Inquiry");
  });

  it("should handle paths with multiple segments (use first segment)", () => {
    const result = getReadablePathName("/master-inquiry/details/123");
    expect(result).toBe("Master Inquiry");
  });

  it("should handle paths without leading slash", () => {
    const result = getReadablePathName("master-inquiry");
    expect(result).toBe("Master Inquiry");
  });

  it("should return original path for unknown paths", () => {
    const result = getReadablePathName("/unknown-path");
    expect(result).toBe("unknown-path");
  });

  it("should convert unforfeitures", () => {
    const result = getReadablePathName("/unforfeitures");
    expect(result).toBe("UnForfeit");
  });

  it("should convert prof-term", () => {
    const result = getReadablePathName("/prof-term");
    expect(result).toBe("Terminations");
  });

  it("should convert beneficiary", () => {
    const result = getReadablePathName("/beneficiary");
    expect(result).toBe("Beneficiary Inquiry");
  });

  it("should convert documentation", () => {
    const result = getReadablePathName("/documentation");
    expect(result).toBe("Documentation");
  });

  it("should handle report paths", () => {
    expect(getReadablePathName("/profit-share-report")).toBe("Profit Share Report");
    expect(getReadablePathName("/balance-by-age")).toBe("Balance by Age");
    expect(getReadablePathName("/distributions-by-age")).toBe("Distributions by Age");
  });

  it("should handle PAY426 variants", () => {
    expect(getReadablePathName("/pay426-9")).toBe("Profit Summary");
  });

  it("should handle QPAY report paths", () => {
    expect(getReadablePathName("/qpay066ta")).toBe("QPAY066TA");
  });

  it("should handle fiscal close sub-paths", () => {
    expect(getReadablePathName("/eligible-employees")).toBe("Eligible Employees");
    expect(getReadablePathName("/manage-executive-hours-and-dollars")).toBe("Manage Executive Hours");
  });

  it("should handle demographic paths", () => {
    expect(getReadablePathName("/demographic")).toBe("Demographic Freeze");
    expect(getReadablePathName("/demographic-freeze")).toBe("Demographic Freeze");
  });

  it("should handle IT DevOps manage state taxes path", () => {
    expect(getReadablePathName("/manage-state-taxes")).toBe("Manage State Tax Rates");
  });

  it("should handle IT DevOps manage annuity rates path", () => {
    expect(getReadablePathName("/manage-annuity-rates")).toBe("Manage Annuity Rates");
  });

  it("should handle case-sensitive IT DevOps path", () => {
    const result = getReadablePathName("/it devops");
    expect(result).toBe("IT DevOps");
  });

  it("should strip leading and trailing slashes before processing", () => {
    expect(getReadablePathName("///master-inquiry///")).toBe("Master Inquiry");
    expect(getReadablePathName("/master-inquiry/")).toBe("Master Inquiry");
    expect(getReadablePathName("master-inquiry/")).toBe("Master Inquiry");
  });
});
