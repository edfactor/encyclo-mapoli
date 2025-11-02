import { describe, expect, it } from "vitest";
import { createValidationErrorsMessage } from "./errorUtils";
import { ServiceErrorResponse } from "../types/errors/errors";

describe("errorUtils", () => {
  describe("createValidationErrorsMessage", () => {
    it("should format validation errors with field-level messages", () => {
      const errorResponse: ServiceErrorResponse = {
        data: {
          title: "Validation Failed",
          detail: "One or more validation errors occurred.",
          status: 400,
          errors: {
            BadgeNumber: ["Badge number is required", "Badge number must be positive"],
            Name: ["Name is required"]
          }
        },
        status: 400
      };

      const result = createValidationErrorsMessage(errorResponse);

      expect(result).toContain("Validation Failed:");
      expect(result).toContain("BadgeNumber:");
      expect(result).toContain("  • Badge number is required");
      expect(result).toContain("  • Badge number must be positive");
      expect(result).toContain("Name:");
      expect(result).toContain("  • Name is required");
    });

    it("should handle single field with single error", () => {
      const errorResponse: ServiceErrorResponse = {
        data: {
          title: "Validation Error",
          detail: "Invalid input",
          status: 400,
          errors: {
            Email: ["Email format is invalid"]
          }
        },
        status: 400
      };

      const result = createValidationErrorsMessage(errorResponse);

      expect(result).toContain("Validation Error:");
      expect(result).toContain("Email:");
      expect(result).toContain("  • Email format is invalid");
    });

    it("should use detail when errors object is not present", () => {
      const errorResponse: ServiceErrorResponse = {
        data: {
          title: "Bad Request",
          detail: "The request was malformed",
          status: 400
        },
        status: 400
      };

      const result = createValidationErrorsMessage(errorResponse);

      expect(result).toContain("Bad Request:");
      expect(result).toContain("  • The request was malformed");
    });

    it("should handle errors object with undefined", () => {
      const errorResponse: ServiceErrorResponse = {
        data: {
          title: "Error",
          detail: "Something went wrong",
          status: 500
        },
        status: 500
      };

      const result = createValidationErrorsMessage(errorResponse);

      expect(result).toContain("Error:");
      expect(result).toContain("  • Something went wrong");
    });

    it("should format multiple fields with multiple errors each", () => {
      const errorResponse: ServiceErrorResponse = {
        data: {
          title: "Multiple Validation Errors",
          detail: "Request validation failed",
          status: 400,
          errors: {
            FirstName: ["First name is required", "First name must be at least 2 characters"],
            LastName: ["Last name is required", "Last name must be at least 2 characters"],
            Age: ["Age must be at least 18"]
          }
        },
        status: 400
      };

      const result = createValidationErrorsMessage(errorResponse);

      expect(result).toContain("Multiple Validation Errors:");
      expect(result).toContain("FirstName:");
      expect(result).toContain("  • First name is required");
      expect(result).toContain("  • First name must be at least 2 characters");
      expect(result).toContain("LastName:");
      expect(result).toContain("  • Last name is required");
      expect(result).toContain("  • Last name must be at least 2 characters");
      expect(result).toContain("Age:");
      expect(result).toContain("  • Age must be at least 18");
    });

    it("should handle empty errors object", () => {
      const errorResponse: ServiceErrorResponse = {
        data: {
          title: "Empty Validation",
          detail: "No specific errors",
          status: 400,
          errors: {}
        },
        status: 400
      };

      const result = createValidationErrorsMessage(errorResponse);

      // Empty errors object falls through to detail branch
      expect(result).toContain("Empty Validation:");
      expect(result).toContain("Errors:");
    });

    it("should format field names and messages correctly", () => {
      const errorResponse: ServiceErrorResponse = {
        data: {
          title: "Validation Error",
          detail: "Invalid data",
          status: 400,
          errors: {
            "User.Email": ["Invalid email format"],
            "User.Password": ["Password too short", "Password must contain numbers"]
          }
        },
        status: 400
      };

      const result = createValidationErrorsMessage(errorResponse);

      expect(result).toContain("User.Email:");
      expect(result).toContain("  • Invalid email format");
      expect(result).toContain("User.Password:");
      expect(result).toContain("  • Password too short");
      expect(result).toContain("  • Password must contain numbers");
    });

    it("should handle special characters in error messages", () => {
      const errorResponse: ServiceErrorResponse = {
        data: {
          title: "Validation Failed",
          detail: "Error with special chars",
          status: 400,
          errors: {
            Field: ["Error with 'quotes' and \"double quotes\"", "Error with <tags>"]
          }
        },
        status: 400
      };

      const result = createValidationErrorsMessage(errorResponse);

      expect(result).toContain("Field:");
      expect(result).toContain("  • Error with 'quotes' and \"double quotes\"");
      expect(result).toContain("  • Error with <tags>");
    });
  });
});
