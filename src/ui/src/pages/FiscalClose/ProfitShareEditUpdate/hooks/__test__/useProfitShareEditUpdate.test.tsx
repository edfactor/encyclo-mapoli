import { configureStore } from "@reduxjs/toolkit";
import { act, renderHook, waitFor } from "@testing-library/react";
import { ReactNode } from "react";
import { Provider } from "react-redux";
import { beforeEach, describe, expect, it, vi } from "vitest";
import securityReducer from "../../../../../reduxstore/slices/securitySlice";
import yearsEndReducer from "../../../../../reduxstore/slices/yearsEndSlice";
import useProfitShareEditUpdate from "../useProfitShareEditUpdate";

// Hoist all mock functions before vi.mock() calls
const {
  mockApplyMaster,
  mockTriggerRevert,
  mockTriggerStatus,
  mockUseFiscalCloseProfitYear,
  mockGetFieldValidation,
  mockUseChecksumValidation
} = vi.hoisted(() => ({
  mockApplyMaster: vi.fn(),
  mockTriggerRevert: vi.fn(),
  mockTriggerStatus: vi.fn(),
  mockUseFiscalCloseProfitYear: vi.fn(() => 2024),
  mockGetFieldValidation: vi.fn(() => ({ hasError: false })),
  mockUseChecksumValidation: vi.fn(() => ({
    validationData: null,
    getFieldValidation: mockGetFieldValidation
  }))
}));

// Mock RTK Query hooks - must return tuple format [triggerFunction, stateObject]
// Use importOriginal to preserve the actual module exports for reducer matchers
vi.mock("../../../../../reduxstore/api/YearsEndApi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("../../../../../reduxstore/api/YearsEndApi")>();
  return {
    ...actual,
    useGetMasterApplyMutation: vi.fn(() => [mockApplyMaster, { isLoading: false }]),
    useLazyGetMasterRevertQuery: vi.fn(() => [mockTriggerRevert, { isFetching: false }]),
    useLazyGetProfitMasterStatusQuery: vi.fn(() => [mockTriggerStatus, { isFetching: false }])
  };
});

// Mock custom hooks
vi.mock("../../../../../hooks/useFiscalCloseProfitYear", () => ({
  default: mockUseFiscalCloseProfitYear
}));

vi.mock("../../../../../hooks/useChecksumValidation", () => ({
  useChecksumValidation: mockUseChecksumValidation
}));

// Type definitions
interface MockedYearsEndState {
  profitSharingEditQueryParams: {
    contributionPercent: number;
    earningsPercent: number;
    maxAllowedContributions: number;
    badgeToAdjust: number;
    badgeToAdjust2: number;
    adjustContributionAmount: number;
    adjustEarningsAmount: number;
    adjustIncomingForfeitAmount: number;
    adjustEarningsSecondaryAmount: number;
    incomingForfeitPercent: number;
    secondaryEarningsPercent: number;
  };
  profitSharingUpdate: null;
  profitSharingEdit: null;
  profitMasterStatus: { updatedBy: null; updatedTime: null };
  profitShareEditUpdateShowSearch: boolean;
  profitEditUpdateRevertChangesAvailable: boolean;
  profitEditUpdateChangesAvailable: boolean;
  totalForfeituresGreaterThanZero: boolean;
  invalidProfitShareEditYear: boolean;
  profitSharingUpdateAdjustmentSummary: null;
}

interface MockedSecurityState {
  token: string;
}

interface MockedRootState {
  yearsEnd: MockedYearsEndState;
  security: MockedSecurityState;
}

interface ValidationFieldResult {
  hasError: boolean;
}

interface ChecksumValidationResult {
  validationData: null;
  getFieldValidation: ReturnType<typeof vi.fn>;
}

describe("useProfitShareEditUpdate", () => {
  const createStore = (preloadedState?: Partial<MockedRootState>) => {
    return configureStore({
      reducer: {
        yearsEnd: yearsEndReducer as any,
        security: securityReducer as any,
        yearsEndApi: () => ({ queries: {}, mutations: {}, provided: [], arg: [] })
      },
      preloadedState: preloadedState as Partial<MockedRootState>
    });
  };

  const renderHookWithProvider = (hook: () => ReturnType<typeof useProfitShareEditUpdate>) => {
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
      } as MockedYearsEndState,
      security: {
        token: "test-token"
      } as MockedSecurityState
    });

    const wrapper = ({ children }: { children: ReactNode }) => <Provider store={store}>{children}</Provider>;

    return renderHook(() => hook(), { wrapper });
  };

  beforeEach(() => {
    vi.clearAllMocks();

    // Reset all hoisted mocks to their default state
    mockUseFiscalCloseProfitYear.mockReturnValue(2024);

    mockUseChecksumValidation.mockReturnValue({
      validationData: null,
      getFieldValidation: mockGetFieldValidation
    } as ChecksumValidationResult);

    mockGetFieldValidation.mockReturnValue({ hasError: false });

    // Reset API mocks - they return [triggerFunction, stateObject] tuples
    mockApplyMaster.mockReturnValue({
      unwrap: vi.fn().mockResolvedValue({
        employeesEffected: 0,
        beneficiariesEffected: 0,
        etvasEffected: 0
      })
    });

    mockTriggerRevert.mockReturnValue({
      unwrap: vi.fn().mockResolvedValue({
        employeesEffected: 0,
        beneficiariesEffected: 0,
        etvasEffected: 0
      })
    });

    mockTriggerStatus.mockReturnValue({
      unwrap: vi.fn().mockResolvedValue({
        updatedBy: null,
        updatedTime: null
      })
    });
  });

  describe("Initial State", () => {
    it("should initialize with correct default values", () => {
      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      expect(result.current.changesApplied).toBe(false);
      expect(result.current.openSaveModal).toBe(false);
      expect(result.current.openRevertModal).toBe(false);
      expect(result.current.openEmptyModal).toBe(false);
      expect(result.current.minimumFieldsEntered).toBe(true);
      expect(result.current.updatedBy).toBe(null);
      expect(result.current.updatedTime).toBe(null);
    });

    it("should calculate minimumFieldsEntered correctly on mount", () => {
      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      expect(result.current.minimumFieldsEntered).toBe(true);
    });
  });

  describe("Form Validation", () => {
    it("should fail validation when minimum fields missing (earnings = 0)", () => {
      const preloadedState: MockedRootState = {
        yearsEnd: {
          profitSharingEditQueryParams: {
            contributionPercent: 5,
            earningsPercent: 0,
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
        },
        security: { token: "test-token" }
      };

      const storeWithoutMinFields = createStore(preloadedState);
      const wrapper = ({ children }: { children: ReactNode }) => (
        <Provider store={storeWithoutMinFields}>{children}</Provider>
      );

      const { result } = renderHook(() => useProfitShareEditUpdate(), { wrapper });

      expect(result.current.minimumFieldsEntered).toBe(false);
    });

    it("should fail validation when max allowed contributions = 0", () => {
      const preloadedState: MockedRootState = {
        yearsEnd: {
          profitSharingEditQueryParams: {
            contributionPercent: 5,
            earningsPercent: 3,
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
        },
        security: { token: "test-token" }
      };

      const storeWithoutMinFields = createStore(preloadedState);
      const wrapper = ({ children }: { children: ReactNode }) => (
        <Provider store={storeWithoutMinFields}>{children}</Provider>
      );

      const { result } = renderHook(() => useProfitShareEditUpdate(), { wrapper });

      expect(result.current.minimumFieldsEntered).toBe(false);
    });

    it("should validate badge 1 adjustments correctly", () => {
      const preloadedState: MockedRootState = {
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
        },
        security: { token: "test-token" }
      };

      const storeWithBadge1 = createStore(preloadedState);
      const wrapper = ({ children }: { children: ReactNode }) => (
        <Provider store={storeWithBadge1}>{children}</Provider>
      );

      const { result } = renderHook(() => useProfitShareEditUpdate(), { wrapper });

      expect(result.current.adjustedBadgeOneValid).toBe(true);
    });

    it("should fail badge 1 validation when earnings adjustment missing", () => {
      const preloadedState: MockedRootState = {
        yearsEnd: {
          profitSharingEditQueryParams: {
            contributionPercent: 5,
            earningsPercent: 3,
            maxAllowedContributions: 10000,
            badgeToAdjust: 12345,
            badgeToAdjust2: 0,
            adjustContributionAmount: 100,
            adjustEarningsAmount: 0,
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
        },
        security: { token: "test-token" }
      };

      const storeWithIncompleteBadge1 = createStore(preloadedState);
      const wrapper = ({ children }: { children: ReactNode }) => (
        <Provider store={storeWithIncompleteBadge1}>{children}</Provider>
      );

      const { result } = renderHook(() => useProfitShareEditUpdate(), { wrapper });

      expect(result.current.adjustedBadgeOneValid).toBe(false);
    });

    it("should validate badge 2 adjustments correctly", () => {
      const preloadedState: MockedRootState = {
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
        },
        security: { token: "test-token" }
      };

      const storeWithBadge2 = createStore(preloadedState);
      const wrapper = ({ children }: { children: ReactNode }) => (
        <Provider store={storeWithBadge2}>{children}</Provider>
      );

      const { result } = renderHook(() => useProfitShareEditUpdate(), { wrapper });

      expect(result.current.adjustedBadgeTwoValid).toBe(true);
    });

    it("should fail badge 2 validation when secondary earnings missing", () => {
      const preloadedState: MockedRootState = {
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
        },
        security: { token: "test-token" }
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
      const preloadedState: MockedRootState = {
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
        },
        security: { token: "test-token" }
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

      act(() => {
        result.current.handleOpenSaveModal();
      });
      expect(result.current.openSaveModal).toBe(true);

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
      const preloadedState: MockedRootState = {
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
        },
        security: { token: "test-token" }
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

      act(() => {
        result.current.handleCloseEmptyModal();
      });
      expect(result.current.openEmptyModal).toBe(false);
    });
  });

  describe("Save Action", () => {
    it("should call applyMaster with correct parameters", async () => {
      mockApplyMaster.mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({
          employeesEffected: 100,
          beneficiariesEffected: 50,
          etvasEffected: 150
        })
      });

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
      mockApplyMaster.mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({
          employeesEffected: 100,
          beneficiariesEffected: 50,
          etvasEffected: 150
        })
      });

      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      expect(result.current.changesApplied).toBe(false);
      expect(result.current.openSaveModal).toBe(false);

      await act(async () => {
        await result.current.saveAction();
      });

      expect(result.current.changesApplied).toBe(true);
      expect(result.current.openSaveModal).toBe(false);
    });

    it("should handle save errors gracefully", async () => {
      mockApplyMaster.mockReturnValue({
        unwrap: vi.fn().mockRejectedValue(new Error("API Error"))
      });

      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {
        // Mock implementation
      });

      await act(async () => {
        await result.current.saveAction();
      });

      expect(consoleErrorSpy).toHaveBeenCalledWith("ERROR: Did not apply changes to year end", expect.any(Error));

      expect(result.current.changesApplied).toBe(false);

      consoleErrorSpy.mockRestore();
    });
  });

  describe("Revert Action", () => {
    it("should call triggerRevert with correct parameters", async () => {
      mockTriggerRevert.mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({
          employeesEffected: 100,
          beneficiariesEffected: 50,
          etvasEffected: 150
        })
      });

      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      await act(async () => {
        await result.current.revertAction();
      });

      expect(mockTriggerRevert).toHaveBeenCalledWith(
        expect.objectContaining({
          profitYear: 2024
        }),
        false
      );
    });

    it("should handle revert success", async () => {
      mockTriggerRevert.mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({
          employeesEffected: 100,
          beneficiariesEffected: 50,
          etvasEffected: 150
        })
      });

      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      await act(async () => {
        await result.current.revertAction();
      });

      expect(result.current.changesApplied).toBe(false);
      expect(result.current.openRevertModal).toBe(false);
    });

    it("should handle revert errors gracefully", async () => {
      mockTriggerRevert.mockReturnValue({
        unwrap: vi.fn().mockRejectedValue(new Error("API Error"))
      });

      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {
        // Mock implementation
      });

      await act(async () => {
        await result.current.revertAction();
      });

      expect(consoleErrorSpy).toHaveBeenCalledWith("ERROR: Did not revert changes to year end", expect.any(Error));

      consoleErrorSpy.mockRestore();
    });
  });

  describe("Status Fetching", () => {
    it("should fetch profit master status on mount when token present", async () => {
      mockTriggerStatus.mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({
          updatedBy: "John Doe",
          updatedTime: "2024-01-15T10:30:00Z"
        })
      });

      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      await waitFor(() => {
        expect(result.current.updatedBy).toBe("John Doe");
        expect(result.current.updatedTime).not.toBeNull();
      });
    });

    it("should mark changes as applied when status has updatedTime", async () => {
      mockTriggerStatus.mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({
          updatedBy: "Jane Smith",
          updatedTime: "2024-01-15T14:45:00Z"
        })
      });

      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      await waitFor(() => {
        expect(result.current.changesApplied).toBe(true);
      });
    });

    it("should handle status fetch errors gracefully", async () => {
      mockTriggerStatus.mockReturnValue({
        unwrap: vi.fn().mockRejectedValue(new Error("Status fetch failed"))
      });

      const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {
        // Mock implementation
      });

      renderHookWithProvider(() => useProfitShareEditUpdate());

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
      mockGetFieldValidation.mockReturnValue({ hasError: false } as ValidationFieldResult);
      mockUseChecksumValidation.mockReturnValue({
        validationData: { field1: { hasError: false } },
        getFieldValidation: mockGetFieldValidation
      } as any);

      const { result } = renderHookWithProvider(() => useProfitShareEditUpdate());

      expect(result.current.validationData).toBeDefined();
      expect(result.current.getFieldValidation).toBeDefined();
    });
  });
});
