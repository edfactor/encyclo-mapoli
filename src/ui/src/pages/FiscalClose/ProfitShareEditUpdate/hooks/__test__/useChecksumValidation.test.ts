import { renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import type { MasterUpdateCrossReferenceValidationResponse } from "../../../../../types/validation/cross-reference-validation";
import { useChecksumValidation } from "../useChecksumValidation";

// Mock RTK Query hook
const mockUseGetMasterUpdateValidationQuery = vi.fn();
vi.mock("../../../../../reduxstore/api/ValidationApi", () => ({
  useGetMasterUpdateValidationQuery: (...args: unknown[]) => mockUseGetMasterUpdateValidationQuery(...args)
}));

describe("useChecksumValidation", () => {
  beforeEach(() => {
    mockUseGetMasterUpdateValidationQuery.mockClear();
  });

  const mockValidationResponse: MasterUpdateCrossReferenceValidationResponse = {
    profitYear: 2024,
    validationGroups: [
      {
        groupName: "Contributions",
        description: null,
        summary: null,
        priority: "High",
        validationRule: null,
        validations: [
          {
            reportCode: "PAY443",
            fieldName: "ContributionTotals",
            expectedValue: 10000,
            currentValue: null,
            isValid: false,
            variance: null,
            message: "Pending validation",
            archivedAt: null,
            notes: null
          }
        ],
        isValid: true
      }
    ],
    criticalIssues: [],
    isValid: true,
    message: "All validations passed",
    totalValidations: 1,
    passedValidations: 1,
    failedValidations: 0,
    validatedReports: ["PAY443"],
    blockMasterUpdate: false,
    warnings: [],
    validatedAt: "2024-01-01T00:00:00Z"
  };

  describe("initial state with autoFetch=false", () => {
    it("should skip fetch when autoFetch is false", () => {
      mockUseGetMasterUpdateValidationQuery.mockReturnValue({
        data: undefined,
        isLoading: false,
        error: null,
        refetch: vi.fn()
      });

      const { result } = renderHook(() =>
        useChecksumValidation({
          profitYear: 2024,
          autoFetch: false
        })
      );

      expect(mockUseGetMasterUpdateValidationQuery).toHaveBeenCalledWith(2024, {
        skip: true
      });

      expect(result.current.validationData).toBeNull();
      expect(result.current.isLoading).toBe(false);
      expect(result.current.error).toBeNull();
    });
  });

  describe("initial state with autoFetch=true", () => {
    it("should fetch validation data when autoFetch is true", () => {
      mockUseGetMasterUpdateValidationQuery.mockReturnValue({
        data: mockValidationResponse,
        isLoading: false,
        error: null,
        refetch: vi.fn()
      });

      const { result } = renderHook(() =>
        useChecksumValidation({
          profitYear: 2024,
          autoFetch: true
        })
      );

      expect(mockUseGetMasterUpdateValidationQuery).toHaveBeenCalledWith(2024, {
        skip: false
      });

      expect(result.current.validationData).toBeDefined();
      expect(result.current.validationData?.profitYear).toBe(2024);
    });

    it("should skip fetch when profitYear is 0 or negative", () => {
      mockUseGetMasterUpdateValidationQuery.mockReturnValue({
        data: undefined,
        isLoading: false,
        error: null,
        refetch: vi.fn()
      });

      renderHook(() =>
        useChecksumValidation({
          profitYear: 0,
          autoFetch: true
        })
      );

      expect(mockUseGetMasterUpdateValidationQuery).toHaveBeenCalledWith(0, {
        skip: true
      });
    });
  });

  describe("enrichment with current values", () => {
    it("should enrich validation with current values and recalculate isValid", () => {
      const apiResponse: MasterUpdateCrossReferenceValidationResponse = {
        profitYear: 2024,
        validationGroups: [
          {
            groupName: "Contributions",
            description: null,
            summary: null,
            priority: "High" as const,
            validationRule: null,
            validations: [
              {
                reportCode: "PAY443",
                fieldName: "ContributionTotals",
                expectedValue: 10000,
                currentValue: null,
                isValid: false,
                variance: null,
                message: "Pending",
                archivedAt: null,
                notes: null
              }
            ],
            isValid: true
          }
        ],
        criticalIssues: [],
        isValid: true,
        message: "All validations passed",
        totalValidations: 1,
        passedValidations: 1,
        failedValidations: 0,
        validatedReports: ["PAY443"],
        blockMasterUpdate: false,
        warnings: [],
        validatedAt: "2024-01-01T00:00:00Z"
      };

      mockUseGetMasterUpdateValidationQuery.mockReturnValue({
        data: apiResponse,
        isLoading: false,
        error: null,
        refetch: vi.fn()
      });

      const { result } = renderHook(() =>
        useChecksumValidation({
          profitYear: 2024,
          autoFetch: true,
          currentValues: {
            ContributionTotals: 10000
          }
        })
      );

      const enrichedValidation = result.current.validationData?.validationGroups[0].validations[0];

      expect(enrichedValidation?.currentValue).toBe(10000);
      expect(enrichedValidation?.isValid).toBe(true);
      expect(enrichedValidation?.variance).toBe(0);
      expect(enrichedValidation?.message).toContain("matches archived value");
    });

    it("should detect mismatch when current value does not match expected", () => {
      const apiResponse: MasterUpdateCrossReferenceValidationResponse = {
        profitYear: 2024,
        validationGroups: [
          {
            groupName: "Distributions",
            description: null,
            summary: null,
            priority: "High" as const,
            validationRule: null,
            validations: [
              {
                reportCode: "PAY443",
                fieldName: "DistributionTotals",
                expectedValue: 15000,
                currentValue: null,
                isValid: false,
                variance: null,
                message: "Pending",
                archivedAt: null,
                notes: null
              }
            ],
            isValid: true
          }
        ],
        criticalIssues: [],
        isValid: true,
        message: "All validations passed",
        totalValidations: 1,
        passedValidations: 1,
        failedValidations: 0,
        validatedReports: ["PAY443"],
        blockMasterUpdate: false,
        warnings: [],
        validatedAt: "2024-01-01T00:00:00Z"
      };

      mockUseGetMasterUpdateValidationQuery.mockReturnValue({
        data: apiResponse,
        isLoading: false,
        error: null,
        refetch: vi.fn()
      });

      const { result } = renderHook(() =>
        useChecksumValidation({
          profitYear: 2024,
          autoFetch: true,
          currentValues: {
            DistributionTotals: 15100
          }
        })
      );

      const enrichedValidation = result.current.validationData?.validationGroups[0].validations[0];

      expect(enrichedValidation?.currentValue).toBe(15100);
      expect(enrichedValidation?.isValid).toBe(false);
      expect(enrichedValidation?.variance).toBe(100);
      expect(enrichedValidation?.message).toContain("does NOT match archived value");
    });

    it("should handle floating point precision with tolerance", () => {
      const apiResponse: MasterUpdateCrossReferenceValidationResponse = {
        profitYear: 2024,
        validationGroups: [
          {
            groupName: "Test",
            description: null,
            summary: null,
            priority: "High" as const,
            validationRule: null,
            validations: [
              {
                reportCode: "PAY443",
                fieldName: "EarningsTotals",
                expectedValue: 1000.0,
                currentValue: null,
                isValid: false,
                variance: null,
                message: "Pending",
                archivedAt: null,
                notes: null
              }
            ],
            isValid: true
          }
        ],
        criticalIssues: [],
        isValid: true,
        message: "All validations passed",
        totalValidations: 1,
        passedValidations: 1,
        failedValidations: 0,
        validatedReports: ["PAY443"],
        blockMasterUpdate: false,
        warnings: [],
        validatedAt: "2024-01-01T00:00:00Z"
      };

      mockUseGetMasterUpdateValidationQuery.mockReturnValue({
        data: apiResponse,
        isLoading: false,
        error: null,
        refetch: vi.fn()
      });

      const { result } = renderHook(() =>
        useChecksumValidation({
          profitYear: 2024,
          autoFetch: true,
          currentValues: {
            EarningsTotals: 1000.005 // Within 0.01 tolerance
          }
        })
      );

      const enrichedValidation = result.current.validationData?.validationGroups[0].validations[0];

      expect(enrichedValidation?.isValid).toBe(true);
    });

    it("should handle fieldName variations (PAY443.DistributionTotals vs DistributionTotals)", () => {
      const apiResponse: MasterUpdateCrossReferenceValidationResponse = {
        profitYear: 2024,
        validationGroups: [
          {
            groupName: "Test",
            description: null,
            summary: null,
            priority: "High" as const,
            validationRule: null,
            validations: [
              {
                reportCode: "PAY443",
                fieldName: "PAY443.DistributionTotals",
                expectedValue: 5000,
                currentValue: null,
                isValid: false,
                variance: null,
                message: "Pending",
                archivedAt: null,
                notes: null
              }
            ],
            isValid: true
          }
        ],
        criticalIssues: [],
        isValid: true,
        message: "All validations passed",
        totalValidations: 1,
        passedValidations: 1,
        failedValidations: 0,
        validatedReports: ["PAY443"],
        blockMasterUpdate: false,
        warnings: [],
        validatedAt: "2024-01-01T00:00:00Z"
      };

      mockUseGetMasterUpdateValidationQuery.mockReturnValue({
        data: apiResponse,
        isLoading: false,
        error: null,
        refetch: vi.fn()
      });

      const { result } = renderHook(() =>
        useChecksumValidation({
          profitYear: 2024,
          autoFetch: true,
          currentValues: {
            DistributionTotals: 5000
          }
        })
      );

      const enrichedValidation = result.current.validationData?.validationGroups[0].validations[0];

      expect(enrichedValidation?.currentValue).toBe(5000);
      expect(enrichedValidation?.isValid).toBe(true);
    });

    it("should handle missing current value gracefully", () => {
      const apiResponse: MasterUpdateCrossReferenceValidationResponse = {
        profitYear: 2024,
        validationGroups: [
          {
            groupName: "Test",
            description: null,
            summary: null,
            priority: "High" as const,
            validationRule: null,
            validations: [
              {
                reportCode: "PAY443",
                fieldName: "ContributionTotals",
                expectedValue: 5000,
                currentValue: null,
                isValid: false,
                variance: null,
                message: "Pending",
                archivedAt: null,
                notes: null
              }
            ],
            isValid: true
          }
        ],
        criticalIssues: [],
        isValid: true,
        message: "All validations passed",
        totalValidations: 1,
        passedValidations: 1,
        failedValidations: 0,
        validatedReports: ["PAY443"],
        blockMasterUpdate: false,
        warnings: [],
        validatedAt: "2024-01-01T00:00:00Z"
      };

      mockUseGetMasterUpdateValidationQuery.mockReturnValue({
        data: apiResponse,
        isLoading: false,
        error: null,
        refetch: vi.fn()
      });

      const { result } = renderHook(() =>
        useChecksumValidation({
          profitYear: 2024,
          autoFetch: true,
          currentValues: {
            // ContributionTotals not provided
            DistributionTotals: 1000
          }
        })
      );

      const enrichedValidation = result.current.validationData?.validationGroups[0].validations[0];

      // Should return API validation as-is when current value not available
      expect(enrichedValidation?.currentValue).toBeNull();
      expect(enrichedValidation?.expectedValue).toBe(5000);
    });

    it("should handle missing expected value", () => {
      const apiResponse: MasterUpdateCrossReferenceValidationResponse = {
        profitYear: 2024,
        validationGroups: [
          {
            groupName: "Test",
            description: null,
            summary: null,
            priority: "High" as const,
            validationRule: null,
            validations: [
              {
                reportCode: "PAY443",
                fieldName: "ContributionTotals",
                expectedValue: null,
                currentValue: null,
                isValid: false,
                variance: null,
                message: "Pending",
                archivedAt: null,
                notes: null
              }
            ],
            isValid: true
          }
        ],
        criticalIssues: [],
        isValid: true,
        message: "All validations passed",
        totalValidations: 1,
        passedValidations: 1,
        failedValidations: 0,
        validatedReports: ["PAY443"],
        blockMasterUpdate: false,
        warnings: [],
        validatedAt: "2024-01-01T00:00:00Z"
      };

      mockUseGetMasterUpdateValidationQuery.mockReturnValue({
        data: apiResponse,
        isLoading: false,
        error: null,
        refetch: vi.fn()
      });

      const { result } = renderHook(() =>
        useChecksumValidation({
          profitYear: 2024,
          autoFetch: true,
          currentValues: {
            ContributionTotals: 5000
          }
        })
      );

      const enrichedValidation = result.current.validationData?.validationGroups[0].validations[0];

      expect(enrichedValidation?.currentValue).toBe(5000);
      expect(enrichedValidation?.isValid).toBe(false);
      expect(enrichedValidation?.message).toContain("No archived value found for comparison");
    });
  });

  describe("helper functions", () => {
    it("getFieldValidation should find validation by field name", () => {
      const apiResponse: MasterUpdateCrossReferenceValidationResponse = {
        profitYear: 2024,
        validationGroups: [
          {
            groupName: "Group 1",
            description: null,
            summary: null,
            priority: "High" as const,
            validationRule: null,
            validations: [
              {
                reportCode: "PAY443",
                fieldName: "ContributionTotals",
                expectedValue: 1000,
                currentValue: 1000,
                isValid: true,
                variance: 0,
                message: "OK",
                archivedAt: null,
                notes: null
              }
            ],
            isValid: true
          },
          {
            groupName: "Group 2",
            description: null,
            summary: null,
            priority: "High" as const,
            validationRule: null,
            validations: [
              {
                reportCode: "PAY443",
                fieldName: "DistributionTotals",
                expectedValue: 2000,
                currentValue: 2000,
                isValid: true,
                variance: 0,
                message: "OK",
                archivedAt: null,
                notes: null
              }
            ],
            isValid: true
          }
        ],
        criticalIssues: [],
        isValid: true,
        message: "All validations passed",
        totalValidations: 1,
        passedValidations: 1,
        failedValidations: 0,
        validatedReports: ["PAY443"],
        blockMasterUpdate: false,
        warnings: [],
        validatedAt: "2024-01-01T00:00:00Z"
      };

      mockUseGetMasterUpdateValidationQuery.mockReturnValue({
        data: apiResponse,
        isLoading: false,
        error: null,
        refetch: vi.fn()
      });

      const { result } = renderHook(() =>
        useChecksumValidation({
          profitYear: 2024,
          autoFetch: true
        })
      );

      const validation = result.current.getFieldValidation("DistributionTotals");

      expect(validation).toBeDefined();
      expect(validation?.fieldName).toBe("DistributionTotals");
      expect(validation?.expectedValue).toBe(2000);
    });

    it("getFieldValidation should return null when field not found", () => {
      mockUseGetMasterUpdateValidationQuery.mockReturnValue({
        data: mockValidationResponse,
        isLoading: false,
        error: null,
        refetch: vi.fn()
      });

      const { result } = renderHook(() =>
        useChecksumValidation({
          profitYear: 2024,
          autoFetch: true
        })
      );

      const validation = result.current.getFieldValidation("NonExistentField");

      expect(validation).toBeNull();
    });

    it("getValidationGroup should find group by name", () => {
      mockUseGetMasterUpdateValidationQuery.mockReturnValue({
        data: mockValidationResponse,
        isLoading: false,
        error: null,
        refetch: vi.fn()
      });

      const { result } = renderHook(() =>
        useChecksumValidation({
          profitYear: 2024,
          autoFetch: true
        })
      );

      const group = result.current.getValidationGroup("Contributions");

      expect(group).toBeDefined();
      expect(group?.groupName).toBe("Contributions");
      expect(group?.validations).toHaveLength(1);
    });

    it("getValidationGroup should return null when group not found", () => {
      mockUseGetMasterUpdateValidationQuery.mockReturnValue({
        data: mockValidationResponse,
        isLoading: false,
        error: null,
        refetch: vi.fn()
      });

      const { result } = renderHook(() =>
        useChecksumValidation({
          profitYear: 2024,
          autoFetch: true
        })
      );

      const group = result.current.getValidationGroup("NonExistentGroup");

      expect(group).toBeNull();
    });

    it("isAllValid should return true when no critical issues", () => {
      mockUseGetMasterUpdateValidationQuery.mockReturnValue({
        data: mockValidationResponse,
        isLoading: false,
        error: null,
        refetch: vi.fn()
      });

      const { result } = renderHook(() =>
        useChecksumValidation({
          profitYear: 2024,
          autoFetch: true
        })
      );

      expect(result.current.isAllValid()).toBe(true);
    });

    it("isAllValid should return false when critical issues exist", () => {
      const responseWithIssues: MasterUpdateCrossReferenceValidationResponse = {
        ...mockValidationResponse,
        criticalIssues: ["Issue 1", "Issue 2"]
      };

      mockUseGetMasterUpdateValidationQuery.mockReturnValue({
        data: responseWithIssues,
        isLoading: false,
        error: null,
        refetch: vi.fn()
      });

      const { result } = renderHook(() =>
        useChecksumValidation({
          profitYear: 2024,
          autoFetch: true
        })
      );

      expect(result.current.isAllValid()).toBe(false);
    });

    it("isAllValid should return false when no validation data", () => {
      mockUseGetMasterUpdateValidationQuery.mockReturnValue({
        data: undefined,
        isLoading: false,
        error: null,
        refetch: vi.fn()
      });

      const { result } = renderHook(() =>
        useChecksumValidation({
          profitYear: 2024,
          autoFetch: false
        })
      );

      expect(result.current.isAllValid()).toBe(false);
    });
  });

  describe("error handling", () => {
    it("should handle RTK Query error object with error property", () => {
      mockUseGetMasterUpdateValidationQuery.mockReturnValue({
        data: undefined,
        isLoading: false,
        error: { error: "Network error" },
        refetch: vi.fn()
      });

      const { result } = renderHook(() =>
        useChecksumValidation({
          profitYear: 2024,
          autoFetch: true
        })
      );

      expect(result.current.error).toBe("Network error");
    });

    it("should handle RTK Query error object with status", () => {
      mockUseGetMasterUpdateValidationQuery.mockReturnValue({
        data: undefined,
        isLoading: false,
        error: { status: 500 },
        refetch: vi.fn()
      });

      const { result } = renderHook(() =>
        useChecksumValidation({
          profitYear: 2024,
          autoFetch: true
        })
      );

      expect(result.current.error).toBe("HTTP 500");
    });

    it("should handle unknown error format", () => {
      mockUseGetMasterUpdateValidationQuery.mockReturnValue({
        data: undefined,
        isLoading: false,
        error: { someUnknownProperty: "value" },
        refetch: vi.fn()
      });

      const { result } = renderHook(() =>
        useChecksumValidation({
          profitYear: 2024,
          autoFetch: true
        })
      );

      expect(result.current.error).toBe("Unknown error");
    });
  });

  describe("callbacks", () => {
    it("should call onValidationLoaded when data is fetched", () => {
      const onValidationLoaded = vi.fn();

      mockUseGetMasterUpdateValidationQuery.mockReturnValue({
        data: mockValidationResponse,
        isLoading: false,
        error: null,
        refetch: vi.fn()
      });

      renderHook(() =>
        useChecksumValidation({
          profitYear: 2024,
          autoFetch: true,
          onValidationLoaded
        })
      );

      expect(onValidationLoaded).toHaveBeenCalledWith(
        expect.objectContaining({
          profitYear: 2024
        })
      );
    });

    it("should call onError when error occurs", () => {
      const onError = vi.fn();

      mockUseGetMasterUpdateValidationQuery.mockReturnValue({
        data: undefined,
        isLoading: false,
        error: { error: "Test error" },
        refetch: vi.fn()
      });

      renderHook(() =>
        useChecksumValidation({
          profitYear: 2024,
          autoFetch: true,
          onError
        })
      );

      expect(onError).toHaveBeenCalledWith("Test error");
    });
  });

  describe("refetch", () => {
    it("should expose refetch function from RTK Query", () => {
      const mockRefetch = vi.fn();

      mockUseGetMasterUpdateValidationQuery.mockReturnValue({
        data: mockValidationResponse,
        isLoading: false,
        error: null,
        refetch: mockRefetch
      });

      const { result } = renderHook(() =>
        useChecksumValidation({
          profitYear: 2024,
          autoFetch: true
        })
      );

      expect(result.current.refetch).toBe(mockRefetch);
    });
  });
});
