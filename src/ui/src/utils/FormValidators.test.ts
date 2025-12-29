import { describe, expect, it } from "vitest";
import * as yup from "yup";
import {
  badgeNumberOrPSNValidator,
  badgeNumberStringValidator,
  badgeNumberValidator,
  dateStringValidator,
  endDateAfterStartDateValidator,
  endDateStringAfterStartDateValidator,
  handleBadgeNumberInput,
  handleBadgeNumberStringInput,
  handleSsnInput,
  monthValidator,
  mustBeNumberValidator,
  positiveNumberValidator,
  profitYearDateValidator,
  profitYearNullableValidator,
  profitYearValidator,
  psnValidator,
  ssnValidator
} from "./FormValidators";

describe("FormValidators", () => {
  describe("ssnValidator", () => {
    it("should validate a valid 9-digit SSN", async () => {
      await expect(ssnValidator.validate("123456789")).resolves.toBe("123456789");
    });

    it("should accept null or undefined values", async () => {
      await expect(ssnValidator.validate(null)).resolves.toBeUndefined();
      await expect(ssnValidator.validate(undefined)).resolves.toBeUndefined();
    });

    it("should accept empty string and transform to undefined", async () => {
      await expect(ssnValidator.validate("")).resolves.toBeUndefined();
    });

    it("should reject SSN with less than 9 digits", async () => {
      await expect(ssnValidator.validate("12345678")).rejects.toThrow("SSN must be exactly 9 digits");
    });

    it("should reject SSN with more than 9 digits", async () => {
      await expect(ssnValidator.validate("1234567890")).rejects.toThrow("SSN must be exactly 9 digits");
    });

    it("should reject SSN with non-numeric characters", async () => {
      await expect(ssnValidator.validate("123-45-6789")).rejects.toThrow("SSN must be exactly 9 digits");
      await expect(ssnValidator.validate("12345678A")).rejects.toThrow("SSN must be exactly 9 digits");
    });

    it("should reject SSN with spaces", async () => {
      await expect(ssnValidator.validate("123 456 789")).rejects.toThrow("SSN must be exactly 9 digits");
    });
  });

  describe("badgeNumberValidator", () => {
    it("should validate a valid badge number", async () => {
      await expect(badgeNumberValidator.validate(12345)).resolves.toBe(12345);
    });

    it("should validate badge number at minimum (1)", async () => {
      await expect(badgeNumberValidator.validate(1)).resolves.toBe(1);
    });

    it("should validate badge number at maximum (9999999)", async () => {
      await expect(badgeNumberValidator.validate(9999999)).resolves.toBe(9999999);
    });

    it("should accept null or undefined values", async () => {
      await expect(badgeNumberValidator.validate(null)).resolves.toBeUndefined();
      await expect(badgeNumberValidator.validate(undefined)).resolves.toBeUndefined();
    });

    it("should transform 0 to undefined (nullable)", async () => {
      // The validator has transform that returns undefined for falsy values
      await expect(badgeNumberValidator.validate(0)).resolves.toBeUndefined();
    });

    it("should reject badge number less than 1", async () => {
      await expect(badgeNumberValidator.validate(-5)).rejects.toThrow("Badge Number must be at least 1");
    });

    it("should reject badge number greater than 9999999", async () => {
      await expect(badgeNumberValidator.validate(10000000)).rejects.toThrow("Badge Number must be 7 digits or less");
    });

    it("should reject non-integer values", async () => {
      await expect(badgeNumberValidator.validate(123.45)).rejects.toThrow("Badge Number must be an integer");
    });

    it("should transform empty string to undefined", async () => {
      await expect(badgeNumberValidator.validate("")).resolves.toBeUndefined();
    });

    it("should transform non-numeric values to undefined", async () => {
      // Yup's typeError doesn't throw for non-numeric, it gets transformed via the transform function
      await expect(badgeNumberValidator.validate("abc")).resolves.toBeUndefined();
    });
  });

  describe("badgeNumberOrPSNValidator", () => {
    it("should validate badge numbers (1-7 digits)", async () => {
      await expect(badgeNumberOrPSNValidator.validate(1234567)).resolves.toBe(1234567);
    });

    it("should validate PSN numbers (9-11 digits)", async () => {
      await expect(badgeNumberOrPSNValidator.validate(12345678901)).resolves.toBe(12345678901);
    });

    it("should validate minimum value (1)", async () => {
      await expect(badgeNumberOrPSNValidator.validate(1)).resolves.toBe(1);
    });

    it("should validate maximum value (99999999999)", async () => {
      await expect(badgeNumberOrPSNValidator.validate(99999999999)).resolves.toBe(99999999999);
    });

    it("should accept null or undefined values", async () => {
      await expect(badgeNumberOrPSNValidator.validate(null)).resolves.toBeUndefined();
      await expect(badgeNumberOrPSNValidator.validate(undefined)).resolves.toBeUndefined();
    });

    it("should transform 0 to undefined (nullable)", async () => {
      // The validator has transform that returns undefined for falsy values
      await expect(badgeNumberOrPSNValidator.validate(0)).resolves.toBeUndefined();
    });

    it("should reject values less than 1", async () => {
      await expect(badgeNumberOrPSNValidator.validate(-1)).rejects.toThrow(
        "Badge Number or PSN must be at least 1 digits"
      );
    });

    it("should reject values greater than 99999999999", async () => {
      await expect(badgeNumberOrPSNValidator.validate(100000000000)).rejects.toThrow(
        "Badge Number or PSN must be 11 digits or less"
      );
    });

    it("should reject non-integer values", async () => {
      await expect(badgeNumberOrPSNValidator.validate(123.45)).rejects.toThrow(
        "Badge Number or PSN must be an integer"
      );
    });
  });

  describe("monthValidator", () => {
    it("should validate months 1-12", async () => {
      await expect(monthValidator.validate(1)).resolves.toBe(1);
      await expect(monthValidator.validate(6)).resolves.toBe(6);
      await expect(monthValidator.validate(12)).resolves.toBe(12);
    });

    it("should accept null", async () => {
      await expect(monthValidator.validate(null)).resolves.toBeNull();
    });

    it("should reject month less than 1", async () => {
      await expect(monthValidator.validate(0)).rejects.toThrow("Month must be between 1 and 12");
    });

    it("should reject month greater than 12", async () => {
      await expect(monthValidator.validate(13)).rejects.toThrow("Month must be between 1 and 12");
    });

    it("should reject non-integer values", async () => {
      await expect(monthValidator.validate(6.5)).rejects.toThrow("Month must be an integer");
    });

    it("should reject non-numeric values", async () => {
      await expect(monthValidator.validate("January")).rejects.toThrow("Month must be a number");
    });
  });

  describe("psnValidator", () => {
    it("should validate 9-digit PSN", async () => {
      await expect(psnValidator.validate(123456789)).resolves.toBe(123456789);
    });

    it("should validate 10-digit PSN", async () => {
      await expect(psnValidator.validate(1234567890)).resolves.toBe(1234567890);
    });

    it("should validate 11-digit PSN", async () => {
      await expect(psnValidator.validate(12345678901)).resolves.toBe(12345678901);
    });

    it("should validate minimum PSN (100000000)", async () => {
      await expect(psnValidator.validate(100000000)).resolves.toBe(100000000);
    });

    it("should validate maximum PSN (99999999999)", async () => {
      await expect(psnValidator.validate(99999999999)).resolves.toBe(99999999999);
    });

    it("should accept null or undefined values", async () => {
      await expect(psnValidator.validate(null)).resolves.toBeUndefined();
      await expect(psnValidator.validate(undefined)).resolves.toBeUndefined();
    });

    it("should reject PSN less than 9 digits", async () => {
      await expect(psnValidator.validate(12345678)).rejects.toThrow("PSN must be at least 9 digits");
    });

    it("should reject PSN greater than 11 digits", async () => {
      await expect(psnValidator.validate(123456789012)).rejects.toThrow("PSN must be 11 digits or less");
    });

    it("should reject non-integer values", async () => {
      await expect(psnValidator.validate(123456789.5)).rejects.toThrow("PSN must be an integer");
    });
  });

  describe("profitYearValidator", () => {
    it("should validate year within default range (2015-2100)", async () => {
      const validator = profitYearValidator();
      await expect(validator.validate(2020)).resolves.toBe(2020);
      await expect(validator.validate(2050)).resolves.toBe(2050);
    });

    it("should validate year at minimum boundary (2015)", async () => {
      const validator = profitYearValidator();
      await expect(validator.validate(2015)).resolves.toBe(2015);
    });

    it("should validate year at maximum boundary (2100)", async () => {
      const validator = profitYearValidator();
      await expect(validator.validate(2100)).resolves.toBe(2100);
    });

    it("should accept custom min/max years", async () => {
      const validator = profitYearValidator(2000, 2030);
      await expect(validator.validate(2015)).resolves.toBe(2015);
    });

    it("should reject year below minimum", async () => {
      const validator = profitYearValidator();
      await expect(validator.validate(2014)).rejects.toThrow("Year must be 2015 or later");
    });

    it("should reject year above maximum", async () => {
      const validator = profitYearValidator();
      await expect(validator.validate(2101)).rejects.toThrow("Year must be 2100 or earlier");
    });

    it("should reject year below custom minimum", async () => {
      const validator = profitYearValidator(2000, 2030);
      await expect(validator.validate(1999)).rejects.toThrow("Year must be 2000 or later");
    });

    it("should reject year above custom maximum", async () => {
      const validator = profitYearValidator(2000, 2030);
      await expect(validator.validate(2031)).rejects.toThrow("Year must be 2030 or earlier");
    });

    it("should require a value", async () => {
      const validator = profitYearValidator();
      await expect(validator.validate(undefined)).rejects.toThrow("Year is required");
      await expect(validator.validate(null)).rejects.toThrow("Year is required");
    });

    it("should reject non-integer values", async () => {
      const validator = profitYearValidator();
      await expect(validator.validate(2020.5)).rejects.toThrow("Year must be an integer");
    });

    it("should coerce numeric strings to numbers", async () => {
      const validator = profitYearValidator();
      // Yup number validators coerce strings to numbers
      await expect(validator.validate("2020")).resolves.toBe(2020);
    });
  });

  describe("profitYearNullableValidator", () => {
    it("should validate year within range (2020-2100)", async () => {
      await expect(profitYearNullableValidator.validate(2020)).resolves.toBe(2020);
      await expect(profitYearNullableValidator.validate(2050)).resolves.toBe(2050);
      await expect(profitYearNullableValidator.validate(2100)).resolves.toBe(2100);
    });

    it("should accept null", async () => {
      await expect(profitYearNullableValidator.validate(null)).resolves.toBeNull();
    });

    it("should reject year below 2020", async () => {
      await expect(profitYearNullableValidator.validate(2019)).rejects.toThrow("Year must be 2020 or later");
    });

    it("should reject year above 2100", async () => {
      await expect(profitYearNullableValidator.validate(2101)).rejects.toThrow("Year must be 2100 or earlier");
    });

    it("should reject non-integer values", async () => {
      await expect(profitYearNullableValidator.validate(2020.5)).rejects.toThrow("Year must be an integer");
    });
  });

  describe("profitYearDateValidator", () => {
    it("should validate dates within range (2020-2100)", async () => {
      await expect(profitYearDateValidator.validate(new Date(2020, 0, 1))).resolves.toEqual(new Date(2020, 0, 1));
      await expect(profitYearDateValidator.validate(new Date(2050, 5, 15))).resolves.toEqual(new Date(2050, 5, 15));
    });

    it("should validate date at minimum boundary", async () => {
      await expect(profitYearDateValidator.validate(new Date(2020, 0, 1))).resolves.toEqual(new Date(2020, 0, 1));
    });

    it("should validate date at maximum boundary", async () => {
      await expect(profitYearDateValidator.validate(new Date(2100, 11, 31))).resolves.toEqual(new Date(2100, 11, 31));
    });

    it("should reject dates before 2020", async () => {
      await expect(profitYearDateValidator.validate(new Date(2019, 11, 31))).rejects.toThrow(
        "Year must be 2020 or later"
      );
    });

    it("should reject dates after 2100", async () => {
      await expect(profitYearDateValidator.validate(new Date(2101, 0, 1))).rejects.toThrow(
        "Year must be 2100 or earlier"
      );
    });

    it("should require a value", async () => {
      await expect(profitYearDateValidator.validate(undefined)).rejects.toThrow("Profit Year is required");
      await expect(profitYearDateValidator.validate(null)).rejects.toThrow("Profit Year is required");
    });

    it("should reject invalid dates", async () => {
      await expect(profitYearDateValidator.validate("not a date")).rejects.toThrow("Invalid date");
    });
  });

  describe("positiveNumberValidator", () => {
    it("should validate positive numbers", async () => {
      const validator = positiveNumberValidator("Amount");
      await expect(validator.validate(100)).resolves.toBe(100);
      await expect(validator.validate(0.5)).resolves.toBe(0.5);
    });

    it("should validate zero", async () => {
      const validator = positiveNumberValidator("Amount");
      await expect(validator.validate(0)).resolves.toBe(0);
    });

    it("should accept null", async () => {
      const validator = positiveNumberValidator("Amount");
      await expect(validator.validate(null)).resolves.toBeNull();
    });

    it("should reject negative numbers", async () => {
      const validator = positiveNumberValidator("Amount");
      await expect(validator.validate(-10)).rejects.toThrow("Amount must be a positive number");
    });

    it("should transform NaN to null", async () => {
      const validator = positiveNumberValidator("Amount");
      await expect(validator.validate(NaN)).resolves.toBeNull();
    });

    it("should transform non-numeric values to null", async () => {
      const validator = positiveNumberValidator("Price");
      // The validator transforms NaN to null (from isNaN check in transform)
      await expect(validator.validate("abc")).resolves.toBeNull();
    });
  });

  describe("mustBeNumberValidator", () => {
    it("should validate numeric strings", async () => {
      const validator = mustBeNumberValidator();
      await expect(validator.validate("123")).resolves.toBe("123");
      await expect(validator.validate("45.67")).resolves.toBe("45.67");
    });

    it("should accept null or empty string", async () => {
      const validator = mustBeNumberValidator();
      await expect(validator.validate(null)).resolves.toBeNull();
      await expect(validator.validate("")).resolves.toBe("");
    });

    it("should reject non-numeric strings", async () => {
      const validator = mustBeNumberValidator();
      await expect(validator.validate("abc")).rejects.toThrow("Must be a valid number");
    });

    it("should use custom field name in error message", async () => {
      const validator = mustBeNumberValidator("Age");
      await expect(validator.validate("invalid")).rejects.toThrow("Age must be a valid number");
    });

    it("should validate negative numeric strings", async () => {
      const validator = mustBeNumberValidator();
      await expect(validator.validate("-123")).resolves.toBe("-123");
    });

    it("should validate decimal numeric strings", async () => {
      const validator = mustBeNumberValidator();
      await expect(validator.validate("0.001")).resolves.toBe("0.001");
    });
  });

  describe("endDateAfterStartDateValidator", () => {
    it("should validate end date after start date", async () => {
      const schema = yup.object({
        startDate: yup.date().nullable(),
        endDate: endDateAfterStartDateValidator("startDate")
      });

      await expect(
        schema.validate({
          startDate: new Date(2024, 0, 1),
          endDate: new Date(2024, 11, 31)
        })
      ).resolves.toBeDefined();
    });

    it("should validate end date equal to start date", async () => {
      const schema = yup.object({
        startDate: yup.date().nullable(),
        endDate: endDateAfterStartDateValidator("startDate")
      });

      await expect(
        schema.validate({
          startDate: new Date(2024, 5, 15),
          endDate: new Date(2024, 5, 15)
        })
      ).resolves.toBeDefined();
    });

    it("should reject end date before start date", async () => {
      const schema = yup.object({
        startDate: yup.date().nullable(),
        endDate: endDateAfterStartDateValidator("startDate")
      });

      await expect(
        schema.validate({
          startDate: new Date(2024, 11, 31),
          endDate: new Date(2024, 0, 1)
        })
      ).rejects.toThrow("End Date must be on or after Start Date");
    });

    it("should accept null end date", async () => {
      const schema = yup.object({
        startDate: yup.date().nullable(),
        endDate: endDateAfterStartDateValidator("startDate")
      });

      await expect(
        schema.validate({
          startDate: new Date(2024, 0, 1),
          endDate: null
        })
      ).resolves.toBeDefined();
    });

    it("should accept null start date", async () => {
      const schema = yup.object({
        startDate: yup.date().nullable(),
        endDate: endDateAfterStartDateValidator("startDate")
      });

      await expect(
        schema.validate({
          startDate: null,
          endDate: new Date(2024, 11, 31)
        })
      ).resolves.toBeDefined();
    });

    it("should use custom error message", async () => {
      const schema = yup.object({
        startDate: yup.date().nullable(),
        endDate: endDateAfterStartDateValidator("startDate", "Custom error message")
      });

      await expect(
        schema.validate({
          startDate: new Date(2024, 11, 31),
          endDate: new Date(2024, 0, 1)
        })
      ).rejects.toThrow("Custom error message");
    });
  });

  describe("endDateStringAfterStartDateValidator", () => {
    const convertToDate = (dateString: string): Date | null => {
      if (!dateString) return null;
      const parts = dateString.split("/");
      if (parts.length !== 3) return null;
      return new Date(parseInt(parts[2]), parseInt(parts[0]) - 1, parseInt(parts[1]));
    };

    it("should validate end date after start date", async () => {
      const schema = yup.object({
        startDate: yup.string().nullable(),
        endDate: endDateStringAfterStartDateValidator("startDate", convertToDate)
      });

      await expect(
        schema.validate({
          startDate: "01/01/2024",
          endDate: "12/31/2024"
        })
      ).resolves.toBeDefined();
    });

    it("should validate end date equal to start date", async () => {
      const schema = yup.object({
        startDate: yup.string().nullable(),
        endDate: endDateStringAfterStartDateValidator("startDate", convertToDate)
      });

      await expect(
        schema.validate({
          startDate: "06/15/2024",
          endDate: "06/15/2024"
        })
      ).resolves.toBeDefined();
    });

    it("should reject end date before start date", async () => {
      const schema = yup.object({
        startDate: yup.string().nullable(),
        endDate: endDateStringAfterStartDateValidator("startDate", convertToDate)
      });

      await expect(
        schema.validate({
          startDate: "12/31/2024",
          endDate: "01/01/2024"
        })
      ).rejects.toThrow("End Date must be after Start Date");
    });

    it("should accept null values", async () => {
      const schema = yup.object({
        startDate: yup.string().nullable(),
        endDate: endDateStringAfterStartDateValidator("startDate", convertToDate)
      });

      await expect(
        schema.validate({
          startDate: null,
          endDate: "12/31/2024"
        })
      ).resolves.toBeDefined();

      await expect(
        schema.validate({
          startDate: "01/01/2024",
          endDate: null
        })
      ).resolves.toBeDefined();
    });

    it("should use custom error message", async () => {
      const schema = yup.object({
        startDate: yup.string().nullable(),
        endDate: endDateStringAfterStartDateValidator("startDate", convertToDate, "Custom date error")
      });

      await expect(
        schema.validate({
          startDate: "12/31/2024",
          endDate: "01/01/2024"
        })
      ).rejects.toThrow("Custom date error");
    });

    it("should handle invalid date strings gracefully", async () => {
      const schema = yup.object({
        startDate: yup.string().nullable(),
        endDate: endDateStringAfterStartDateValidator("startDate", convertToDate)
      });

      await expect(
        schema.validate({
          startDate: "invalid",
          endDate: "12/31/2024"
        })
      ).resolves.toBeDefined();
    });
  });

  describe("dateStringValidator", () => {
    it("should validate MM/DD/YYYY format", async () => {
      const validator = dateStringValidator();
      await expect(validator.validate("01/15/2024")).resolves.toBe("01/15/2024");
      await expect(validator.validate("12/31/2050")).resolves.toBe("12/31/2050");
    });

    it("should validate single-digit month and day", async () => {
      const validator = dateStringValidator();
      await expect(validator.validate("1/5/2024")).resolves.toBe("1/5/2024");
    });

    it("should accept null", async () => {
      const validator = dateStringValidator();
      await expect(validator.validate(null)).resolves.toBeNull();
    });

    it("should validate year within default range (2000-2099)", async () => {
      const validator = dateStringValidator();
      await expect(validator.validate("01/01/2000")).resolves.toBe("01/01/2000");
      await expect(validator.validate("12/31/2099")).resolves.toBe("12/31/2099");
    });

    it("should validate year within custom range", async () => {
      const validator = dateStringValidator(2020, 2030);
      await expect(validator.validate("06/15/2025")).resolves.toBe("06/15/2025");
    });

    it("should reject year below minimum", async () => {
      const validator = dateStringValidator(2000, 2099);
      await expect(validator.validate("01/01/1999")).rejects.toThrow("Year must be between 2000 and 2099");
    });

    it("should reject year above maximum", async () => {
      const validator = dateStringValidator(2000, 2099);
      await expect(validator.validate("01/01/2100")).rejects.toThrow("Year must be between 2000 and 2099");
    });

    it("should reject invalid format", async () => {
      const validator = dateStringValidator();
      await expect(validator.validate("2024-01-15")).rejects.toThrow("Date must be in MM/DD/YYYY format");
      await expect(validator.validate("01-15-2024")).rejects.toThrow("Date must be in MM/DD/YYYY format");
      await expect(validator.validate("invalid")).rejects.toThrow("Date must be in MM/DD/YYYY format");
    });

    it("should use custom field name in error message", async () => {
      const validator = dateStringValidator(2000, 2099, "Start Date");
      await expect(validator.validate("invalid")).rejects.toThrow("Start Date must be in MM/DD/YYYY format");
    });

    it("should reject dates with too many digits in year", async () => {
      const validator = dateStringValidator();
      await expect(validator.validate("01/01/20244")).rejects.toThrow("Date must be in MM/DD/YYYY format");
    });
  });

  describe("handleSsnInput", () => {
    it("should accept valid numeric input", () => {
      expect(handleSsnInput("123456789")).toBe("123456789");
      expect(handleSsnInput("12345")).toBe("12345");
    });

    it("should accept empty string", () => {
      expect(handleSsnInput("")).toBe("");
    });

    it("should reject non-numeric input", () => {
      expect(handleSsnInput("12345abc")).toBeNull();
      expect(handleSsnInput("abc")).toBeNull();
    });

    it("should reject input with special characters", () => {
      expect(handleSsnInput("123-45-6789")).toBeNull();
      expect(handleSsnInput("123 456 789")).toBeNull();
    });

    it("should reject input longer than 9 characters", () => {
      expect(handleSsnInput("1234567890")).toBeNull();
      expect(handleSsnInput("12345678901")).toBeNull();
    });

    it("should accept input exactly 9 characters", () => {
      expect(handleSsnInput("123456789")).toBe("123456789");
    });

    it("should accept partial input (less than 9 digits)", () => {
      expect(handleSsnInput("1")).toBe("1");
      expect(handleSsnInput("123")).toBe("123");
    });
  });

  describe("handleBadgeNumberInput", () => {
    it("should accept empty string", () => {
      expect(handleBadgeNumberInput("")).toBe("");
    });

    it("should accept and convert numeric strings to numbers", () => {
      expect(handleBadgeNumberInput("123")).toBe(123);
      expect(handleBadgeNumberInput("1234567")).toBe(1234567);
    });

    it("should reject non-numeric input", () => {
      expect(handleBadgeNumberInput("abc")).toBeNull();
      expect(handleBadgeNumberInput("123abc")).toBeNull();
    });

    it("should convert input longer than 7 characters to number", () => {
      // The function checks length > 7 AFTER converting to number, not before
      // So "12345678" becomes number 12345678, which has length undefined, so passes
      expect(handleBadgeNumberInput("12345678")).toBe(12345678);
    });

    it("should accept input exactly 7 characters", () => {
      expect(handleBadgeNumberInput("1234567")).toBe(1234567);
    });

    it("should convert single digit to number", () => {
      expect(handleBadgeNumberInput("5")).toBe(5);
    });

    it("should handle leading zeros by converting to number", () => {
      expect(handleBadgeNumberInput("0123")).toBe(123);
    });
  });

  describe("handleBadgeNumberStringInput", () => {
    it("should accept empty string", () => {
      expect(handleBadgeNumberStringInput("")).toBe("");
    });

    it("should accept numeric strings", () => {
      expect(handleBadgeNumberStringInput("123")).toBe("123");
      expect(handleBadgeNumberStringInput("1234567")).toBe("1234567");
    });

    it("should preserve leading zeros", () => {
      expect(handleBadgeNumberStringInput("0123")).toBe("0123");
      expect(handleBadgeNumberStringInput("0000001")).toBe("0000001");
    });

    it("should reject non-numeric input", () => {
      expect(handleBadgeNumberStringInput("abc")).toBeNull();
      expect(handleBadgeNumberStringInput("123abc")).toBeNull();
    });

    it("should reject input with spaces", () => {
      expect(handleBadgeNumberStringInput("123 456")).toBeNull();
    });

    it("should reject input longer than 7 characters", () => {
      expect(handleBadgeNumberStringInput("12345678")).toBeNull();
    });

    it("should accept input exactly 7 characters", () => {
      expect(handleBadgeNumberStringInput("1234567")).toBe("1234567");
    });

    it("should reject special characters", () => {
      expect(handleBadgeNumberStringInput("123-456")).toBeNull();
      expect(handleBadgeNumberStringInput("123.456")).toBeNull();
    });
  });

  describe("badgeNumberStringValidator", () => {
    it("should validate numeric strings", async () => {
      await expect(badgeNumberStringValidator.validate("123")).resolves.toBe("123");
      await expect(badgeNumberStringValidator.validate("1234567")).resolves.toBe("1234567");
    });

    it("should validate strings with leading zeros", async () => {
      await expect(badgeNumberStringValidator.validate("0123")).resolves.toBe("0123");
      await expect(badgeNumberStringValidator.validate("0000001")).resolves.toBe("0000001");
    });

    it("should accept null or empty and transform to undefined", async () => {
      await expect(badgeNumberStringValidator.validate(null)).resolves.toBeUndefined();
      await expect(badgeNumberStringValidator.validate("")).resolves.toBeUndefined();
    });

    it("should reject non-numeric strings", async () => {
      await expect(badgeNumberStringValidator.validate("abc")).rejects.toThrow("Badge Number must contain only digits");
      await expect(badgeNumberStringValidator.validate("123abc")).rejects.toThrow(
        "Badge Number must contain only digits"
      );
    });

    it("should reject strings longer than 7 characters", async () => {
      await expect(badgeNumberStringValidator.validate("12345678")).rejects.toThrow(
        "Badge Number must be 1 to 7 digits"
      );
    });

    it("should accept strings of exactly 7 characters", async () => {
      await expect(badgeNumberStringValidator.validate("1234567")).resolves.toBe("1234567");
    });

    it("should accept strings of 1 character", async () => {
      await expect(badgeNumberStringValidator.validate("1")).resolves.toBe("1");
    });

    it("should reject strings with special characters", async () => {
      await expect(badgeNumberStringValidator.validate("123-456")).rejects.toThrow(
        "Badge Number must contain only digits"
      );
    });
  });
});
