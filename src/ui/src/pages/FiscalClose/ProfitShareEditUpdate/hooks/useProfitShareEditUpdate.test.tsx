import React, { ReactNode } from "react";
import { describe, it, expect, beforeEach, vi } from "vitest";
import { renderHook, act, waitFor } from "@testing-library/react";
import { Provider } from "react-redux";
import { configureStore, PreloadedState } from "@reduxjs/toolkit";
import useProfitShareEditUpdate from "./useProfitShareEditUpdate";
import yearsEndReducer from "../../../../reduxstore/slices/yearsEndSlice";
import securityReducer from "../../../../reduxstore/slices/securitySlice";
import * as YearsEndApi from "../../../../reduxstore/api/YearsEndApi";
import * as useFiscalCloseProfitYearModule from "../../../../hooks/useFiscalCloseProfitYear";
import * as useChecksumValidationModule from "../../../../hooks/useChecksumValidation";

// Mock the API hooks
vi.mock("../../../../reduxstore/api/YearsEndApi");
vi.mock("../../../../hooks/useFiscalCloseProfitYear");
vi.mock("../../../../hooks/useChecksumValidation");

describe("useProfitShareEditUpdate", () => {
  let store: ReturnType<typeof configureStore>;

  const createStore = (preloadedState?: PreloadedState<any>) => {
    return configureStore({
      reducer: {
        yearsEnd: yearsEndReducer,
        security: securityReducer
      },
      preloadedState
    });
  };

  const renderHookWithProvider = (hook: () => any) => {
    const store = createStore({
      yearsEnd: {
        profitSharingEditQueryParams: {
          contributionPercent: 5,
          earningsPercent: 3,
          maxAllowedContributions: 10000,
          badgeToAdjust: 0,
          badgeToAdjust2: 0,
          adjustContributionAmount: 0,
          adjustEarningsAmount: 0,
          adjustIncomingForfeitAmount: 0,
          adjustEarningsSecondaryAmount: 0,
          incomingForfeitPercent: 0,
          secondaryEarningsPercent: 0
        },
        profitSharingUpdate: null,
        profitSharingEdit: null,
        profitMasterStatus: { updatedBy: null, updatedTime: null },
        profitShareEditUpdateShowSearch: true,
        profitEditUpdateRevertChangesAvailable: false,
        profitEditUpdateChangesAvailable: false,
        totalForfeituresGreaterThanZero: false,
        invalidProfitShareEditYear: false,
        profitSharingUpdateAdjustmentSummary: null
      } as any,
      security: {
        token: "test-token"
      } as any
    });

    const wrapper = ({ children }: { children: ReactNode }) => (
      <Provider store={store}>{children}</Provider>
    );

    return renderHook(() => hook(), { wrapper });
  };

  beforeEach(() => {
    vi.clearAllMocks();

    // Mock useFiscalCloseProfitYear
    (useFiscalCloseProfitYearModule.default as any).mockReturnValue(2024);

    // Mock useChecksumValidation
    (useChecksumValidationModule.useChecksumValidation as any).mockReturnValue({
      validationData: null,
      getFieldValidation: vi.fn(() => ({ hasError: false }))
    });

    // Mock API hooks
    (YearsEndApi.useGetMasterApplyMutation as any).mockReturnValue([vi.fn()]);
    (YearsEndApi.useLazyGetMasterRevertQuery as any).mockReturnValue([vi.fn()]);
    (YearsEndApi.useLazyGetProfitMasterStatusQuery as any).mockReturnValue([vi.fn()]);
  });

  describe("Initial State", () => {
    it("should initialize with correct default values", () => {
      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      expect(result.current.changesApplied).toBe(false);
      expect(result.current.openSaveModal).toBe(false);
      expect(result.current.openRevertModal).toBe(false);
      expect(result.current.openEmptyModal).toBe(false);
      expect(result.current.minimumFieldsEntered).toBe(true); // Should be true with default params
      expect(result.current.updatedBy).toBe(null);
      expect(result.current.updatedTime).toBe(null);
    });

    it("should calculate minimumFieldsEntered correctly on mount", () => {
      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      // With default params (contribution: 5, earnings: 3, max: 10000), should be true
      expect(result.current.minimumFieldsEntered).toBe(true);
    });
  });

  describe("Form Validation", () => {
    it("should fail validation when minimum fields missing (earnings = 0)", () => {
      const preloadedState = {
        yearsEnd: {
          profitSharingEditQueryParams: {
            contributionPercent: 5,
            earningsPercent: 0, // Missing earnings
            maxAllowedContributions: 10000,
            badgeToAdjust: 0,
            badgeToAdjust2: 0,
            adjustContributionAmount: 0,
            adjustEarningsAmount: 0,
            adjustIncomingForfeitAmount: 0,
            adjustEarningsSecondaryAmount: 0,
            incomingForfeitPercent: 0,
            secondaryEarningsPercent: 0
          },
          profitSharingUpdate: null,
          profitSharingEdit: null,
          profitMasterStatus: { updatedBy: null, updatedTime: null },
          profitShareEditUpdateShowSearch: true,
          profitEditUpdateRevertChangesAvailable: false,
          profitEditUpdateChangesAvailable: false,
          totalForfeituresGreaterThanZero: false,
          invalidProfitShareEditYear: false,
          profitSharingUpdateAdjustmentSummary: null
        } as any,
        security: { token: "test-token" } as any
      };

      const storeWithoutMinFields = createStore(preloadedState);
      const wrapper = ({ children }: { children: ReactNode }) => (
        <Provider store={storeWithoutMinFields}>{children}</Provider>
      );

      const { result } = renderHook(() => useProfitShareEditUpdate(), { wrapper });

      expect(result.current.minimumFieldsEntered).toBe(false);
    });

    it("should fail validation when max allowed contributions = 0", () => {
      const preloadedState = {
        yearsEnd: {
          profitSharingEditQueryParams: {
            contributionPercent: 5,
            earningsPercent: 3,
            maxAllowedContributions: 0, // Missing max allowed
            badgeToAdjust: 0,
            badgeToAdjust2: 0,
            adjustContributionAmount: 0,
            adjustEarningsAmount: 0,
            adjustIncomingForfeitAmount: 0,
            adjustEarningsSecondaryAmount: 0,
            incomingForfeitPercent: 0,
            secondaryEarningsPercent: 0
          },
          profitSharingUpdate: null,
          profitSharingEdit: null,
          profitMasterStatus: { updatedBy: null, updatedTime: null },
          profitShareEditUpdateShowSearch: true,
          profitEditUpdateRevertChangesAvailable: false,
          profitEditUpdateChangesAvailable: false,
          totalForfeituresGreaterThanZero: false,
          invalidProfitShareEditYear: false,
          profitSharingUpdateAdjustmentSummary: null
        } as any,
        security: { token: "test-token" } as any
      };

      const storeWithoutMinFields = createStore(preloadedState);
      const wrapper = ({ children }: { children: ReactNode }) => (
        <Provider store={storeWithoutMinFields}>{children}</Provider>
      );

      const { result } = renderHook(() => useProfitShareEditUpdate(), { wrapper });

      expect(result.current.minimumFieldsEntered).toBe(false);
    });

    it("should validate badge 1 adjustments correctly", () => {
      const preloadedState = {
        yearsEnd: {
          profitSharingEditQueryParams: {
            contributionPercent: 5,
            earningsPercent: 3,
            maxAllowedContributions: 10000,
            badgeToAdjust: 12345,
            badgeToAdjust2: 0,
            adjustContributionAmount: 100,
            adjustEarningsAmount: 50,
            adjustIncomingForfeitAmount: 25,
            adjustEarningsSecondaryAmount: 0,
            incomingForfeitPercent: 0,
            secondaryEarningsPercent: 0
          },
          profitSharingUpdate: null,
          profitSharingEdit: null,
          profitMasterStatus: { updatedBy: null, updatedTime: null },
          profitShareEditUpdateShowSearch: true,
          profitEditUpdateRevertChangesAvailable: false,
          profitEditUpdateChangesAvailable: false,
          totalForfeituresGreaterThanZero: false,
          invalidProfitShareEditYear: false,
          profitSharingUpdateAdjustmentSummary: null
        } as any,
        security: { token: "test-token" } as any
      };

      const storeWithBadge1 = createStore(preloadedState);
      const wrapper = ({ children }: { children: ReactNode }) => (
        <Provider store={storeWithBadge1}>{children}</Provider>
      );

      const { result } = renderHook(() => useProfitShareEditUpdate(), { wrapper });

      expect(result.current.adjustedBadgeOneValid).toBe(true);
    });

    it("should fail badge 1 validation when earnings adjustment missing", () => {
      const preloadedState = {
        yearsEnd: {
          profitSharingEditQueryParams: {
            contributionPercent: 5,
            earningsPercent: 3,
            maxAllowedContributions: 10000,
            badgeToAdjust: 12345,
            badgeToAdjust2: 0,
            adjustContributionAmount: 100,
            adjustEarningsAmount: 0, // Missing earnings adjustment
            adjustIncomingForfeitAmount: 25,
            adjustEarningsSecondaryAmount: 0,
            incomingForfeitPercent: 0,
            secondaryEarningsPercent: 0
          },
          profitSharingUpdate: null,
          profitSharingEdit: null,
          profitMasterStatus: { updatedBy: null, updatedTime: null },
          profitShareEditUpdateShowSearch: true,
          profitEditUpdateRevertChangesAvailable: false,
          profitEditUpdateChangesAvailable: false,
          totalForfeituresGreaterThanZero: false,
          invalidProfitShareEditYear: false,
          profitSharingUpdateAdjustmentSummary: null
        } as any,
        security: { token: "test-token" } as any
      };

      const storeWithIncompleteBadge1 = createStore(preloadedState);
      const wrapper = ({ children }: { children: ReactNode }) => (
        <Provider store={storeWithIncompleteBadge1}>{children}</Provider>
      );

      const { result } = renderHook(() => useProfitShareEditUpdate(), { wrapper });

      expect(result.current.adjustedBadgeOneValid).toBe(false);
    });

    it("should validate badge 2 adjustments correctly", () => {
      const preloadedState = {
        yearsEnd: {
          profitSharingEditQueryParams: {
            contributionPercent: 5,
            earningsPercent: 3,
            maxAllowedContributions: 10000,
            badgeToAdjust: 0,
            badgeToAdjust2: 67890,
            adjustContributionAmount: 0,
            adjustEarningsAmount: 0,
            adjustIncomingForfeitAmount: 0,
            adjustEarningsSecondaryAmount: 75,
            incomingForfeitPercent: 0,
            secondaryEarningsPercent: 0
          },
          profitSharingUpdate: null,
          profitSharingEdit: null,
          profitMasterStatus: { updatedBy: null, updatedTime: null },
          profitShareEditUpdateShowSearch: true,
          profitEditUpdateRevertChangesAvailable: false,
          profitEditUpdateChangesAvailable: false,
          totalForfeituresGreaterThanZero: false,
          invalidProfitShareEditYear: false,
          profitSharingUpdateAdjustmentSummary: null
        } as any,
        security: { token: "test-token" } as any
      };

      const storeWithBadge2 = createStore(preloadedState);
      const wrapper = ({ children }: { children: ReactNode }) => (
        <Provider store={storeWithBadge2}>{children}</Provider>
      );

      const { result } = renderHook(() => useProfitShareEditUpdate(), { wrapper });

      expect(result.current.adjustedBadgeTwoValid).toBe(true);
    });

    it("should fail badge 2 validation when secondary earnings missing", () => {
      const preloadedState = {
        yearsEnd: {
          profitSharingEditQueryParams: {
            contributionPercent: 5,
            earningsPercent: 3,
            maxAllowedContributions: 10000,
            badgeToAdjust: 0,
            badgeToAdjust2: 67890,
            adjustContributionAmount: 0,
            adjustEarningsAmount: 0,
            adjustIncomingForfeitAmount: 0,
            adjustEarningsSecondaryAmount: 0, // Missing secondary earnings
            incomingForfeitPercent: 0,
            secondaryEarningsPercent: 0
          },
          profitSharingUpdate: null,
          profitSharingEdit: null,
          profitMasterStatus: { updatedBy: null, updatedTime: null },
          profitShareEditUpdateShowSearch: true,
          profitEditUpdateRevertChangesAvailable: false,
          profitEditUpdateChangesAvailable: false,
          totalForfeituresGreaterThanZero: false,
          invalidProfitShareEditYear: false,
          profitSharingUpdateAdjustmentSummary: null
        } as any,
        security: { token: "test-token" } as any
      };

      const storeWithIncompleteBadge2 = createStore(preloadedState);
      const wrapper = ({ children }: { children: ReactNode }) => (
        <Provider store={storeWithIncompleteBadge2}>{children}</Provider>
      );

      const { result } = renderHook(() => useProfitShareEditUpdate(), { wrapper });

      expect(result.current.adjustedBadgeTwoValid).toBe(false);
    });
  });

  describe("Modal Management", () => {
    it("should open save modal when validation passes", () => {
      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      expect(result.current.openSaveModal).toBe(false);

      act(() => {
        result.current.handleOpenSaveModal();
      });

      expect(result.current.openSaveModal).toBe(true);
      expect(result.current.openEmptyModal).toBe(false);
    });

    it("should open empty modal when validation fails", () => {
      const preloadedState = {
        yearsEnd: {
          profitSharingEditQueryParams: {
            contributionPercent: 0, // Invalid
            earningsPercent: 0, // Invalid
            maxAllowedContributions: 0, // Invalid
            badgeToAdjust: 0,
            badgeToAdjust2: 0,
            adjustContributionAmount: 0,
            adjustEarningsAmount: 0,
            adjustIncomingForfeitAmount: 0,
            adjustEarningsSecondaryAmount: 0,
            incomingForfeitPercent: 0,
            secondaryEarningsPercent: 0
          },
          profitSharingUpdate: null,
          profitSharingEdit: null,
          profitMasterStatus: { updatedBy: null, updatedTime: null },
          profitShareEditUpdateShowSearch: true,
          profitEditUpdateRevertChangesAvailable: false,
          profitEditUpdateChangesAvailable: false,
          totalForfeituresGreaterThanZero: false,
          invalidProfitShareEditYear: false,
          profitSharingUpdateAdjustmentSummary: null
        } as any,
        security: { token: "test-token" } as any
      };

      const storeWithInvalidForm = createStore(preloadedState);
      const wrapper = ({ children }: { children: ReactNode }) => (
        <Provider store={storeWithInvalidForm}>{children}</Provider>
      );

      const { result } = renderHook(() => useProfitShareEditUpdate(), { wrapper });

      act(() => {
        result.current.handleOpenSaveModal();
      });

      expect(result.current.openEmptyModal).toBe(true);
      expect(result.current.openSaveModal).toBe(false);
    });

    it("should close save modal when handler called", () => {
      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      // Open save modal
      act(() => {
        result.current.handleOpenSaveModal();
      });
      expect(result.current.openSaveModal).toBe(true);

      // Close save modal
      act(() => {
        result.current.handleCloseSaveModal();
      });
      expect(result.current.openSaveModal).toBe(false);
    });

    it("should open and close revert modal", () => {
      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      expect(result.current.openRevertModal).toBe(false);

      act(() => {
        result.current.handleOpenRevertModal();
      });
      expect(result.current.openRevertModal).toBe(true);

      act(() => {
        result.current.handleCloseRevertModal();
      });
      expect(result.current.openRevertModal).toBe(false);
    });

    it("should close empty modal", () => {
      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      expect(result.current.openEmptyModal).toBe(false);

      // Manually set to open (via direct action)
      act(() => {
        result.current.handleOpenSaveModal(); // With invalid form, opens empty modal
      });

      // Create a store with invalid form to open empty modal
      const preloadedState = {
        yearsEnd: {
          profitSharingEditQueryParams: {
            contributionPercent: 0,
            earningsPercent: 0,
            maxAllowedContributions: 0,
            badgeToAdjust: 0,
            badgeToAdjust2: 0,
            adjustContributionAmount: 0,
            adjustEarningsAmount: 0,
            adjustIncomingForfeitAmount: 0,
            adjustEarningsSecondaryAmount: 0,
            incomingForfeitPercent: 0,
            secondaryEarningsPercent: 0
          },
          profitSharingUpdate: null,
          profitSharingEdit: null,
          profitMasterStatus: { updatedBy: null, updatedTime: null },
          profitShareEditUpdateShowSearch: true,
          profitEditUpdateRevertChangesAvailable: false,
          profitEditUpdateChangesAvailable: false,
          totalForfeituresGreaterThanZero: false,
          invalidProfitShareEditYear: false,
          profitSharingUpdateAdjustmentSummary: null
        } as any,
        security: { token: "test-token" } as any
      };

      const storeWithInvalidForm = createStore(preloadedState);
      const wrapper = ({ children }: { children: ReactNode }) => (
        <Provider store={storeWithInvalidForm}>{children}</Provider>
      );

      const { result: resultWithInvalidForm } = renderHook(() => useProfitShareEditUpdate(), { wrapper });

      act(() => {
        resultWithInvalidForm.current.handleOpenSaveModal();
      });
      expect(resultWithInvalidForm.current.openEmptyModal).toBe(true);

      act(() => {
        resultWithInvalidForm.current.handleCloseEmptyModal();
      });
      expect(resultWithInvalidForm.current.openEmptyModal).toBe(false);
    });
  });

  describe("Save Action", () => {
    it("should call applyMaster with correct parameters", async () => {
      const mockApplyMaster = vi.fn().mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({
          employeesEffected: 100,
          beneficiariesEffected: 50,
          etvasEffected: 150
        })
      });

      (YearsEndApi.useGetMasterApplyMutation as any).mockReturnValue([mockApplyMaster]);

      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      await act(async () => {
        await result.current.saveAction();
      });

      expect(mockApplyMaster).toHaveBeenCalledWith(
        expect.objectContaining({
          profitYear: 2024,
          contributionPercent: 5,
          earningsPercent: 3,
          maxAllowedContributions: 10000
        })
      );
    });

    it("should handle save success", async () => {
      const mockApplyMaster = vi.fn().mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({
          employeesEffected: 100,
          beneficiariesEffected: 50,
          etvasEffected: 150
        })
      });

      (YearsEndApi.useGetMasterApplyMutation as any).mockReturnValue([mockApplyMaster]);

      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      expect(result.current.changesApplied).toBe(false);
      expect(result.current.openSaveModal).toBe(false);

      await act(async () => {
        await result.current.saveAction();
      });

      // After save succeeds, changesApplied should be true and modal should close
      expect(result.current.changesApplied).toBe(true);
      expect(result.current.openSaveModal).toBe(false);
    });

    it("should handle save errors gracefully", async () => {
      const mockApplyMaster = vi.fn().mockReturnValue({
        unwrap: vi.fn().mockRejectedValue(new Error("API Error"))
      });

      (YearsEndApi.useGetMasterApplyMutation as any).mockReturnValue([mockApplyMaster]);

      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation();

      await act(async () => {
        await result.current.saveAction();
      });

      expect(consoleErrorSpy).toHaveBeenCalledWith(
        "ERROR: Did not apply changes to year end",
        expect.any(Error)
      );

      // Should not mark as applied on error
      expect(result.current.changesApplied).toBe(false);

      consoleErrorSpy.mockRestore();
    });
  });

  describe("Revert Action", () => {
    it("should call triggerRevert with correct parameters", async () => {
      const mockRevert = vi.fn().mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({
          employeesEffected: 100,
          beneficiariesEffected: 50,
          etvasEffected: 150
        })
      });

      (YearsEndApi.useLazyGetMasterRevertQuery as any).mockReturnValue([mockRevert]);

      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      await act(async () => {
        await result.current.revertAction();
      });

      expect(mockRevert).toHaveBeenCalledWith(
        expect.objectContaining({
          profitYear: 2024
        }),
        false
      );
    });

    it("should handle revert success", async () => {
      const mockRevert = vi.fn().mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({
          employeesEffected: 100,
          beneficiariesEffected: 50,
          etvasEffected: 150
        })
      });

      (YearsEndApi.useLazyGetMasterRevertQuery as any).mockReturnValue([mockRevert]);

      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      // Manually set changesApplied to true to simulate state after save
      // (in real usage, this would come from Redux state)
      await act(async () => {
        await result.current.revertAction();
      });

      expect(result.current.changesApplied).toBe(false);
      expect(result.current.openRevertModal).toBe(false);
    });

    it("should handle revert errors gracefully", async () => {
      const mockRevert = vi.fn().mockReturnValue({
        unwrap: vi.fn().mockRejectedValue(new Error("API Error"))
      });

      (YearsEndApi.useLazyGetMasterRevertQuery as any).mockReturnValue([mockRevert]);

      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation();

      await act(async () => {
        await result.current.revertAction();
      });

      expect(consoleErrorSpy).toHaveBeenCalledWith(
        "ERROR: Did not revert changes to year end",
        expect.any(Error)
      );

      consoleErrorSpy.mockRestore();
    });
  });

  describe("Status Fetching", () => {
    it("should fetch profit master status on mount when token present", async () => {
      const mockTriggerStatus = vi.fn().mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({
          updatedBy: "John Doe",
          updatedTime: "2024-01-15T10:30:00Z"
        })
      });

      (YearsEndApi.useLazyGetProfitMasterStatusQuery as any).mockReturnValue([mockTriggerStatus]);

      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      await waitFor(() => {
        expect(result.current.updatedBy).toBe("John Doe");
        expect(result.current.updatedTime).not.toBeNull();
      });
    });

    it("should mark changes as applied when status has updatedTime", async () => {
      const mockTriggerStatus = vi.fn().mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({
          updatedBy: "Jane Smith",
          updatedTime: "2024-01-15T14:45:00Z"
        })
      });

      (YearsEndApi.useLazyGetProfitMasterStatusQuery as any).mockReturnValue([mockTriggerStatus]);

      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      await waitFor(() => {
        expect(result.current.changesApplied).toBe(true);
      });
    });

    it("should handle status fetch errors gracefully", async () => {
      const mockTriggerStatus = vi.fn().mockReturnValue({
        unwrap: vi.fn().mockRejectedValue(new Error("Status fetch failed"))
      });

      (YearsEndApi.useLazyGetProfitMasterStatusQuery as any).mockReturnValue([mockTriggerStatus]);

      const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation();

      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      await waitFor(() => {
        expect(consoleErrorSpy).toHaveBeenCalled();
      });

      consoleErrorSpy.mockRestore();
    });
  });

  describe("Redux State Passthrough", () => {
    it("should return Redux state values correctly", () => {
      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      expect(result.current.profitSharingEditQueryParams).toBeDefined();
      expect(result.current.profitSharingEditQueryParams?.contributionPercent).toBe(5);
    });
  });

  describe("Validation Data Integration", () => {
    it("should integrate with useChecksumValidation hook", () => {
      const mockGetFieldValidation = vi.fn().mockReturnValue({ hasError: false });
      (useChecksumValidationModule.useChecksumValidation as any).mockReturnValue({
        validationData: { field1: { hasError: false } },
        getFieldValidation: mockGetFieldValidation
      });

      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      expect(result.current.validationData).toBeDefined();
      expect(result.current.getFieldValidation).toBeDefined();
    });
  });
});
