import { describe, expect, it } from "vitest";
import { formatPercentage } from "./formatPercentage";

describe("formatPercentage", () => {
  it("should format decimal to percentage with 0 decimal places", () => {
    const result = formatPercentage(0.5);
    expect(result).toBe("50%");
  });

  it("should handle 0", () => {
    const result = formatPercentage(0);
    expect(result).toBe("0%");
  });

  it("should handle 1 (100%)", () => {
    const result = formatPercentage(1);
    expect(result).toBe("100%");
  });

  it("should round down for values less than 0.5%", () => {
    const result = formatPercentage(0.004);
    expect(result).toBe("0%");
  });

  it("should round up for values 0.5% and above", () => {
    const result = formatPercentage(0.005);
    expect(result).toBe("1%");
  });

  it("should handle small percentages", () => {
    const result = formatPercentage(0.01);
    expect(result).toBe("1%");
  });

  it("should handle large percentages over 100%", () => {
    const result = formatPercentage(1.5);
    expect(result).toBe("150%");
  });

  it("should handle very small decimals", () => {
    const result = formatPercentage(0.001);
    expect(result).toBe("0%");
  });

  it("should handle negative percentages", () => {
    const result = formatPercentage(-0.25);
    expect(result).toBe("-25%");
  });

  it("should round to nearest integer", () => {
    expect(formatPercentage(0.123)).toBe("12%");
    expect(formatPercentage(0.126)).toBe("13%");
    expect(formatPercentage(0.125)).toBe("13%");
  });

  it("should handle very large numbers", () => {
    const result = formatPercentage(10);
    expect(result).toBe("1000%");
  });

  it("should format common percentages correctly", () => {
    expect(formatPercentage(0.25)).toBe("25%");
    expect(formatPercentage(0.33)).toBe("33%");
    expect(formatPercentage(0.5)).toBe("50%");
    expect(formatPercentage(0.75)).toBe("75%");
  });
});
