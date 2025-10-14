import { describe, expect, it } from "vitest";
import { formatPhoneNumber } from "./phoneUtils";

describe("formatPhoneNumber", () => {
  it("formats 10-digit phone number correctly", () => {
    expect(formatPhoneNumber("5081234567")).toBe("(508) 123-4567");
  });

  it("formats phone number with existing formatting", () => {
    expect(formatPhoneNumber("(508) 123-4567")).toBe("(508) 123-4567");
  });

  it("formats phone number with dashes", () => {
    expect(formatPhoneNumber("508-123-4567")).toBe("(508) 123-4567");
  });

  it("handles null phone number", () => {
    expect(formatPhoneNumber(null)).toBe("N/A");
  });

  it("handles undefined phone number", () => {
    expect(formatPhoneNumber(undefined)).toBe("N/A");
  });

  it("handles empty string", () => {
    expect(formatPhoneNumber("")).toBe("N/A");
  });

  it("returns original for invalid length", () => {
    expect(formatPhoneNumber("123")).toBe("123");
  });

  it("returns original for non-numeric input", () => {
    expect(formatPhoneNumber("abc")).toBe("abc");
  });

  it("formats phone with spaces", () => {
    expect(formatPhoneNumber("508 123 4567")).toBe("(508) 123-4567");
  });

  it("formats phone with dots", () => {
    expect(formatPhoneNumber("508.123.4567")).toBe("(508) 123-4567");
  });
});
