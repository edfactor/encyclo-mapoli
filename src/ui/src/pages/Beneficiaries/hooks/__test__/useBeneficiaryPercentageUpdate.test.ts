import { act, renderHook, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { BeneficiaryDto } from "../../../../types";
import { useBeneficiaryPercentageUpdate } from "../useBeneficiaryPercentageUpdate";

interface ValidateAndUpdateResult {
  success: boolean;
  previousValue?: number;
  error?: string;
}

// Create mock functions for triggers/actions
const mockTriggerUpdate = vi.fn();

// Mock the API
vi.mock("reduxstore/api/BeneficiariesApi", () => ({
  useUpdateBeneficiaryMutation: () => [mockTriggerUpdate, {}]
}));

const mockBeneficiaries: BeneficiaryDto[] = [
  {
    id: 1,
    psnSuffix: 1,
    badgeNumber: 12345,
    demographicId: 1,
    psn: "123451",
    ssn: "123-45-6789",
    firstName: "Jane",
    lastName: "Doe",
    percent: 50,
    dateOfBirth: new Date("1990-01-01"),
    relationship: "Spouse",
    street: "123 Main St",
    city: "Boston",
    state: "MA",
    postalCode: "02101",
    beneficiaryContactId: 1,
    createdDate: new Date(),
    phoneNumber: "617-555-1234"
  },
  {
    id: 2,
    psnSuffix: 2,
    badgeNumber: 12345,
    demographicId: 2,
    psn: "123452",
    ssn: "987-65-4321",
    firstName: "John",
    lastName: "Doe Jr",
    percent: 50,
    dateOfBirth: new Date("2010-01-01"),
    relationship: "Child",
    street: "123 Main St",
    city: "Boston",
    state: "MA",
    postalCode: "02101",
    beneficiaryContactId: 2,
    createdDate: new Date(),
    phoneNumber: "617-555-5678"
  }
];

describe("useBeneficiaryPercentageUpdate", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("should initialize with isUpdating false", () => {
    mockTriggerUpdate.mockReturnValue({});

    const { result } = renderHook(() => useBeneficiaryPercentageUpdate());

    expect(result.current.isUpdating).toBe(false);
  });

  describe("validateAndUpdate", () => {
    it("should succeed when new percentage keeps total at 100%", async () => {
      const mockUnwrap = vi.fn().mockResolvedValue({});
      mockTriggerUpdate.mockReturnValue({
        unwrap: mockUnwrap
      });

      const { result } = renderHook(() => useBeneficiaryPercentageUpdate());

      let validationResult: ValidateAndUpdateResult | undefined;
      await act(async () => {
        // Update ID 1 to 50% so total remains 100% (50% + 50%)
        validationResult = await result.current.validateAndUpdate(1, 50, mockBeneficiaries);
      });

      expect(validationResult?.success).toBe(true);
      expect(mockTriggerUpdate).toHaveBeenCalledWith({ id: 1, percentage: 50 });
    });

    it("should fail when new percentage would exceed 100%", async () => {
      mockTriggerUpdate.mockReturnValue({});

      const { result } = renderHook(() => useBeneficiaryPercentageUpdate());

      let validationResult: ValidateAndUpdateResult | undefined;
      await act(async () => {
        validationResult = await result.current.validateAndUpdate(1, 75, mockBeneficiaries);
      });

      expect(validationResult?.success).toBe(false);
      expect(validationResult?.error).toContain("125%");
      expect(validationResult?.previousValue).toBe(50);
      expect(mockTriggerUpdate).not.toHaveBeenCalled();
    });

    it("should allow percentage update that equals 100% total", async () => {
      const mockUnwrap = vi.fn().mockResolvedValue({});
      mockTriggerUpdate.mockReturnValue({
        unwrap: mockUnwrap
      });

      const { result } = renderHook(() => useBeneficiaryPercentageUpdate());

      let validationResult: ValidateAndUpdateResult | undefined;
      await act(async () => {
        validationResult = await result.current.validateAndUpdate(1, 50, mockBeneficiaries);
      });

      expect(validationResult?.success).toBe(true);
      expect(mockTriggerUpdate).toHaveBeenCalled();
    });

    it("should allow percentage update that results in less than 100%", async () => {
      const mockUnwrap = vi.fn().mockResolvedValue({});
      mockTriggerUpdate.mockReturnValue({
        unwrap: mockUnwrap
      });

      const { result } = renderHook(() => useBeneficiaryPercentageUpdate());

      let validationResult: ValidateAndUpdateResult | undefined;
      await act(async () => {
        validationResult = await result.current.validateAndUpdate(1, 25, mockBeneficiaries);
      });

      expect(validationResult?.success).toBe(true);
      expect(mockTriggerUpdate).toHaveBeenCalled();
    });

    it("should call onUpdateSuccess callback on successful update", async () => {
      const onUpdateSuccess = vi.fn();
      const mockUnwrap = vi.fn().mockResolvedValue({});
      mockTriggerUpdate.mockReturnValue({
        unwrap: mockUnwrap
      });

      const { result } = renderHook(() => useBeneficiaryPercentageUpdate(onUpdateSuccess));

      await act(async () => {
        // Update ID 1 to 40% so total is 90% (40% + 50%), which is valid
        await result.current.validateAndUpdate(1, 40, mockBeneficiaries);
      });

      expect(onUpdateSuccess).toHaveBeenCalled();
    });

    it("should not call onUpdateSuccess on validation failure", async () => {
      const onUpdateSuccess = vi.fn();
      mockTriggerUpdate.mockReturnValue({});

      const { result } = renderHook(() => useBeneficiaryPercentageUpdate(onUpdateSuccess));

      await act(async () => {
        await result.current.validateAndUpdate(1, 75, mockBeneficiaries);
      });

      expect(onUpdateSuccess).not.toHaveBeenCalled();
    });

    it("should handle API errors gracefully", async () => {
      const mockUnwrap = vi.fn().mockRejectedValue(new Error("API Error"));
      mockTriggerUpdate.mockReturnValue({
        unwrap: mockUnwrap
      });
      const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});

      const { result } = renderHook(() => useBeneficiaryPercentageUpdate());

      let validationResult: ValidateAndUpdateResult | undefined;
      await act(async () => {
        // Update ID 1 to 40% (valid percentage that will pass validation but fail on API)
        validationResult = await result.current.validateAndUpdate(1, 40, mockBeneficiaries);
      });

      expect(validationResult?.success).toBe(false);
      expect(validationResult?.error).toContain("Failed to update");
      expect(validationResult?.previousValue).toBe(50);
      expect(consoleErrorSpy).toHaveBeenCalled();

      consoleErrorSpy.mockRestore();
    });

    it("should return previous value for UI restoration on failure", async () => {
      mockTriggerUpdate.mockReturnValue({});

      const { result } = renderHook(() => useBeneficiaryPercentageUpdate());

      let validationResult: ValidateAndUpdateResult | undefined;
      await act(async () => {
        validationResult = await result.current.validateAndUpdate(2, 80, mockBeneficiaries);
      });

      expect(validationResult?.success).toBe(false);
      expect(validationResult?.previousValue).toBe(50); // ID 2's previous value
    });

    it("should set isUpdating true during API call and false after", async () => {
      const mockUnwrap = vi.fn().mockImplementation(() => new Promise((resolve) => setTimeout(resolve, 100)));
      mockTriggerUpdate.mockReturnValue({
        unwrap: mockUnwrap
      });

      const { result } = renderHook(() => useBeneficiaryPercentageUpdate());

      expect(result.current.isUpdating).toBe(false);

      let promise: Promise<ValidateAndUpdateResult> | undefined;
      act(() => {
        // Update ID 1 to 30% so total is 80% (30% + 50%), which is valid
        promise = result.current.validateAndUpdate(1, 30, mockBeneficiaries);
      });

      await waitFor(() => {
        expect(result.current.isUpdating).toBe(true);
      });

      await act(async () => {
        await promise;
      });

      expect(result.current.isUpdating).toBe(false);
    });

    it("should work with single beneficiary", async () => {
      const mockUnwrap = vi.fn().mockResolvedValue({});
      mockTriggerUpdate.mockReturnValue({
        unwrap: mockUnwrap
      });

      const singleBeneficiary: BeneficiaryDto[] = [{ ...mockBeneficiaries[0], percent: 100 }];

      const { result } = renderHook(() => useBeneficiaryPercentageUpdate());

      let validationResult: ValidateAndUpdateResult | undefined;
      await act(async () => {
        validationResult = await result.current.validateAndUpdate(1, 100, singleBeneficiary);
      });

      expect(validationResult?.success).toBe(true);
    });

    it("should fail when single beneficiary exceeds 100%", async () => {
      mockTriggerUpdate.mockReturnValue({});

      const singleBeneficiary: BeneficiaryDto[] = [{ ...mockBeneficiaries[0], percent: 100 }];

      const { result } = renderHook(() => useBeneficiaryPercentageUpdate());

      let validationResult: ValidateAndUpdateResult | undefined;
      await act(async () => {
        validationResult = await result.current.validateAndUpdate(1, 150, singleBeneficiary);
      });

      expect(validationResult?.success).toBe(false);
      expect(validationResult?.error).toContain("150%");
    });
  });
});
