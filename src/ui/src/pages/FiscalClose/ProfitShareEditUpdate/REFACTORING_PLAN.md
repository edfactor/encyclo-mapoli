# ProfitShareEditUpdate Refactoring Plan

## Status: Phase 1 Complete ✅

This document outlines the complete refactoring plan for the ProfitShareEditUpdate page, a complex 800-line component that manages profit-sharing calculations with multiple validation systems, modals, and state tracking.

---

## Phase 1: Extract Helper Functions and Button Components ✅ COMPLETE

**Goal**: Reduce complexity by extracting reusable components and utilities.

### Completed Tasks

- ✅ Created `utils/formValidation.ts` with `wasFormUsed` function
- ✅ Created `ProfitShareSaveButton.tsx` component
- ✅ Created `ProfitShareRevertButton.tsx` component
- ✅ Updated main file to use extracted components
- ✅ Validated with ESLint and TypeScript
- ✅ Reduced main file from 800 to ~640 lines (20% reduction)

### Files Created

```
ProfitShareEditUpdate/
├── utils/
│   └── formValidation.ts          (NEW - 24 lines)
├── ProfitShareSaveButton.tsx      (NEW - 104 lines)
├── ProfitShareRevertButton.tsx    (NEW - 63 lines)
└── ProfitShareEditUpdate.tsx      (MODIFIED - reduced by 160 lines)
```

---

## Phase 2: Create Custom Hook for Business Logic

**Goal**: Extract all business logic, API calls, and state management into a custom hook.

### Estimated Effort

- **Time**: 3-4 hours
- **Complexity**: High
- **Testing Required**: Extensive

### Tasks

#### 2.1: Create `hooks/useProfitShareEditUpdate.ts`

**File Structure**:

```typescript
import { useCallback, useEffect, useReducer, useRef } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  useGetMasterApplyMutation,
  useLazyGetMasterRevertQuery,
  useLazyGetProfitMasterStatusQuery
} from "../../../../reduxstore/api/YearsEndApi";
import { useChecksumValidation } from "../../../../hooks/useChecksumValidation";
import useFiscalCloseProfitYear from "../../../../hooks/useFiscalCloseProfitYear";

// State interface
interface ProfitShareEditUpdateState {
  changesApplied: boolean;
  modals: {
    saveModalOpen: boolean;
    revertModalOpen: boolean;
    emptyModalOpen: boolean;
  };
  validation: {
    minimumFieldsEntered: boolean;
    adjustedBadgeOneValid: boolean;
    adjustedBadgeTwoValid: boolean;
  };
  profitMasterStatus: {
    updatedBy: string | null;
    updatedTime: string | null;
  };
}

// Action types
type ProfitShareEditUpdateAction =
  | { type: "OPEN_SAVE_MODAL" }
  | { type: "CLOSE_SAVE_MODAL" }
  | { type: "OPEN_REVERT_MODAL" }
  | { type: "CLOSE_REVERT_MODAL" }
  | { type: "OPEN_EMPTY_MODAL" }
  | { type: "CLOSE_EMPTY_MODAL" }
  | { type: "SET_CHANGES_APPLIED"; payload: boolean }
  | { type: "SET_VALIDATION"; payload: Partial<ProfitShareEditUpdateState["validation"]> }
  | { type: "SET_STATUS"; payload: { updatedBy: string | null; updatedTime: string | null } }
  | { type: "SAVE_SUCCESS" }
  | { type: "REVERT_SUCCESS" };

// Reducer
const profitShareEditUpdateReducer = (
  state: ProfitShareEditUpdateState,
  action: ProfitShareEditUpdateAction
): ProfitShareEditUpdateState => {
  switch (action.type) {
    case "OPEN_SAVE_MODAL":
      return { ...state, modals: { ...state.modals, saveModalOpen: true } };
    case "CLOSE_SAVE_MODAL":
      return { ...state, modals: { ...state.modals, saveModalOpen: false } };
    case "OPEN_REVERT_MODAL":
      return { ...state, modals: { ...state.modals, revertModalOpen: true } };
    case "CLOSE_REVERT_MODAL":
      return { ...state, modals: { ...state.modals, revertModalOpen: false } };
    case "OPEN_EMPTY_MODAL":
      return { ...state, modals: { ...state.modals, emptyModalOpen: true } };
    case "CLOSE_EMPTY_MODAL":
      return { ...state, modals: { ...state.modals, emptyModalOpen: false } };
    case "SET_CHANGES_APPLIED":
      return { ...state, changesApplied: action.payload };
    case "SET_VALIDATION":
      return { ...state, validation: { ...state.validation, ...action.payload } };
    case "SET_STATUS":
      return { ...state, profitMasterStatus: action.payload };
    case "SAVE_SUCCESS":
      return {
        ...state,
        changesApplied: true,
        modals: { ...state.modals, saveModalOpen: false }
      };
    case "REVERT_SUCCESS":
      return {
        ...state,
        changesApplied: false,
        modals: { ...state.modals, revertModalOpen: false }
      };
    default:
      return state;
  }
};

const initialState: ProfitShareEditUpdateState = {
  changesApplied: false,
  modals: {
    saveModalOpen: false,
    revertModalOpen: false,
    emptyModalOpen: false
  },
  validation: {
    minimumFieldsEntered: false,
    adjustedBadgeOneValid: true,
    adjustedBadgeTwoValid: true
  },
  profitMasterStatus: {
    updatedBy: null,
    updatedTime: null
  }
};

export const useProfitShareEditUpdate = () => {
  const [state, dispatch] = useReducer(profitShareEditUpdateReducer, initialState);
  const reduxDispatch = useDispatch();
  const profitYear = useFiscalCloseProfitYear();

  // Redux selectors
  const {
    profitEditUpdateChangesAvailable,
    profitEditUpdateRevertChangesAvailable,
    profitShareEditUpdateShowSearch,
    profitSharingEdit,
    profitSharingUpdate,
    profitSharingEditQueryParams,
    profitShareApplyOrRevertLoading,
    totalForfeituresGreaterThanZero,
    invalidProfitShareEditYear
  } = useSelector((state: RootState) => state.yearsEnd);

  // API hooks
  const [applyMaster] = useGetMasterApplyMutation();
  const [triggerRevert] = useLazyGetMasterRevertQuery();
  const [triggerStatus] = useLazyGetProfitMasterStatusQuery();

  // Checksum validation hook
  const { validationData, getFieldValidation } = useChecksumValidation({
    profitYear: profitYear || 0,
    autoFetch: true,
    currentValues: profitSharingUpdate?.profitShareUpdateTotals
      ? {
          TotalProfitSharingBalance: profitSharingUpdate.profitShareUpdateTotals.beginningBalance,
          DistributionTotals: profitSharingUpdate.profitShareUpdateTotals.distributions,
          ForfeitureTotals: profitSharingUpdate.profitShareUpdateTotals.forfeiture,
          ContributionTotals: profitSharingUpdate.profitShareUpdateTotals.totalContribution,
          EarningsTotals: profitSharingUpdate.profitShareUpdateTotals.earnings,
          IncomingAllocations: profitSharingUpdate.profitShareUpdateTotals.allocations,
          OutgoingAllocations: profitSharingUpdate.profitShareUpdateTotals.paidAllocations,
          NetAllocTransfer:
            (profitSharingUpdate.profitShareUpdateTotals.allocations || 0) +
            (profitSharingUpdate.profitShareUpdateTotals.paidAllocations || 0)
        }
      : undefined
  });

  // Year validation effect
  useEffect(() => {
    const currentYear = new Date().getFullYear();
    if (profitYear !== currentYear - 1) {
      reduxDispatch(setInvalidProfitShareEditYear(true));
      reduxDispatch(
        setMessage({
          key: MessageKeys.ProfitShareEditUpdate,
          message: {
            type: "warning",
            title: "Invalid Year Selected",
            message: `Please select a ${currentYear - 1} date in the drawer menu to proceed.`
          }
        })
      );
    } else {
      reduxDispatch(setInvalidProfitShareEditYear(false));
    }
  }, [profitYear, reduxDispatch]);

  // Save action
  const saveAction = useCallback(async (): Promise<void> => {
    const params: ProfitShareMasterApplyRequest = {
      profitYear: profitYear ?? 0,
      contributionPercent: profitSharingEditQueryParams?.contributionPercent ?? 0,
      earningsPercent: profitSharingEditQueryParams?.earningsPercent ?? 0,
      incomingForfeitPercent: profitSharingEditQueryParams?.incomingForfeitPercent ?? 0,
      secondaryEarningsPercent: profitSharingEditQueryParams?.secondaryEarningsPercent ?? 0,
      maxAllowedContributions: profitSharingEditQueryParams?.maxAllowedContributions ?? 0,
      badgeToAdjust: profitSharingEditQueryParams?.badgeToAdjust ?? 0,
      adjustContributionAmount: profitSharingEditQueryParams?.adjustContributionAmount ?? 0,
      adjustEarningsAmount: profitSharingEditQueryParams?.adjustEarningsAmount ?? 0,
      adjustIncomingForfeitAmount: profitSharingEditQueryParams?.adjustEarningsSecondaryAmount ?? 0,
      badgeToAdjust2: profitSharingEditQueryParams?.badgeToAdjust2 ?? 0,
      adjustEarningsSecondaryAmount: profitSharingEditQueryParams?.adjustEarningsSecondaryAmount ?? 0
    };

    reduxDispatch(setProfitShareApplyOrRevertLoading(true));

    try {
      const payload = await applyMaster(params).unwrap();

      dispatch({ type: "SAVE_SUCCESS" });

      reduxDispatch(setProfitEditUpdateChangesAvailable(false));
      reduxDispatch(setProfitEditUpdateRevertChangesAvailable(true));
      reduxDispatch(setProfitShareEditUpdateShowSearch(false));
      reduxDispatch(clearProfitSharingUpdate());
      reduxDispatch(setResetYearEndPage(true));

      reduxDispatch(
        setMessage({
          ...Messages.ProfitShareApplySuccess,
          message: {
            ...Messages.ProfitShareApplySuccess.message,
            message: `Employees affected: ${payload?.employeesEffected} | Beneficiaries: ${payload?.beneficiariesEffected} | ETVAs: ${payload?.etvasEffected}`
          }
        })
      );
    } catch (error) {
      console.error("ERROR: Did not apply changes to year end", error);
      reduxDispatch(
        setMessage({
          ...Messages.ProfitShareApplyFail,
          message: {
            ...Messages.ProfitShareApplyFail.message,
            message: "Employees affected: 0 | Beneficiaries: 0 | ETVAs: 0"
          }
        })
      );
    } finally {
      reduxDispatch(setProfitShareApplyOrRevertLoading(false));
    }
  }, [profitYear, profitSharingEditQueryParams, applyMaster, reduxDispatch]);

  // Revert action
  const revertAction = useCallback(async (): Promise<void> => {
    const params: ProfitYearRequest = {
      profitYear: profitYear ?? 0
    };

    try {
      const payload = await triggerRevert(params, false).unwrap();

      dispatch({ type: "REVERT_SUCCESS" });

      reduxDispatch(setProfitEditUpdateChangesAvailable(false));
      reduxDispatch(setProfitEditUpdateRevertChangesAvailable(false));
      reduxDispatch(clearProfitSharingEditQueryParams());
      reduxDispatch(setResetYearEndPage(true));
      reduxDispatch(setProfitShareEditUpdateShowSearch(true));
      reduxDispatch(clearProfitSharingEdit());
      reduxDispatch(clearProfitSharingUpdate());

      reduxDispatch(
        setMessage({
          ...Messages.ProfitShareRevertSuccess,
          message: {
            ...Messages.ProfitShareRevertSuccess.message,
            message: `Employees affected: ${payload?.employeesEffected} | Beneficiaries: ${payload?.beneficiariesEffected} | ETVAs: ${payload?.etvasEffected}`
          }
        })
      );
    } catch (error) {
      console.error("ERROR: Did not revert changes to year end", error);
      reduxDispatch(
        setMessage({
          ...Messages.ProfitShareRevertFail,
          message: {
            ...Messages.ProfitShareRevertFail.message,
            message: "Employees affected: 0 | Beneficiaries: 0 | ETVAs: 0"
          }
        })
      );
    }
  }, [profitYear, triggerRevert, reduxDispatch]);

  // Form validation
  const validateForm = useCallback(() => {
    if (!profitSharingEditQueryParams) return false;

    // Check minimum fields
    const minFieldsEntered =
      (profitSharingEditQueryParams.contributionPercent ?? 0) > 0 &&
      (profitSharingEditQueryParams.earningsPercent ?? 0) > 0 &&
      (profitSharingEditQueryParams.maxAllowedContributions ?? 0) > 0;

    // Check badge 1 adjustment validity
    const badge1Valid =
      (profitSharingEditQueryParams.badgeToAdjust ?? 0) === 0 ||
      ((profitSharingEditQueryParams.adjustContributionAmount ?? 0) > 0 &&
        (profitSharingEditQueryParams.adjustEarningsAmount ?? 0) > 0 &&
        (profitSharingEditQueryParams.adjustIncomingForfeitAmount ?? 0) > 0);

    // Check badge 2 adjustment validity
    const badge2Valid =
      (profitSharingEditQueryParams.badgeToAdjust2 ?? 0) === 0 ||
      (profitSharingEditQueryParams.adjustEarningsSecondaryAmount ?? 0) > 0;

    dispatch({
      type: "SET_VALIDATION",
      payload: {
        minimumFieldsEntered: minFieldsEntered,
        adjustedBadgeOneValid: badge1Valid,
        adjustedBadgeTwoValid: badge2Valid
      }
    });

    return minFieldsEntered && badge1Valid && badge2Valid;
  }, [profitSharingEditQueryParams]);

  // Update validation when params change
  useEffect(() => {
    validateForm();
  }, [validateForm]);

  // Fetch profit master status
  const onStatusSearch = useCallback(async () => {
    try {
      const result = await triggerStatus({ profitYear: profitYear || 0 }).unwrap();
      if (result) {
        dispatch({
          type: "SET_STATUS",
          payload: {
            updatedBy: result.updatedBy || null,
            updatedTime: result.updatedTime || null
          }
        });

        if (result.updatedTime) {
          dispatch({ type: "SET_CHANGES_APPLIED", payload: true });
          reduxDispatch(setProfitEditUpdateChangesAvailable(false));
          reduxDispatch(setProfitEditUpdateRevertChangesAvailable(true));
        }
      }
    } catch (error) {
      console.error("Failed to fetch profit master status:", error);
    }
  }, [profitYear, triggerStatus, reduxDispatch]);

  // Initialize status on mount
  useEffect(() => {
    onStatusSearch();
  }, [onStatusSearch]);

  // Modal handlers
  const handleOpenSaveModal = useCallback(() => {
    if (validateForm()) {
      dispatch({ type: "OPEN_SAVE_MODAL" });
    } else {
      dispatch({ type: "OPEN_EMPTY_MODAL" });
    }
  }, [validateForm]);

  const handleCloseSaveModal = useCallback(() => {
    dispatch({ type: "CLOSE_SAVE_MODAL" });
  }, []);

  const handleOpenRevertModal = useCallback(() => {
    dispatch({ type: "OPEN_REVERT_MODAL" });
  }, []);

  const handleCloseRevertModal = useCallback(() => {
    dispatch({ type: "CLOSE_REVERT_MODAL" });
  }, []);

  const handleCloseEmptyModal = useCallback(() => {
    dispatch({ type: "CLOSE_EMPTY_MODAL" });
  }, []);

  // Return hook API
  return {
    // State
    changesApplied: state.changesApplied,
    openSaveModal: state.modals.saveModalOpen,
    openRevertModal: state.modals.revertModalOpen,
    openEmptyModal: state.modals.emptyModalOpen,
    minimumFieldsEntered: state.validation.minimumFieldsEntered,
    adjustedBadgeOneValid: state.validation.adjustedBadgeOneValid,
    adjustedBadgeTwoValid: state.validation.adjustedBadgeTwoValid,
    updatedBy: state.profitMasterStatus.updatedBy,
    updatedTime: state.profitMasterStatus.updatedTime,

    // Redux state (passthrough)
    profitEditUpdateChangesAvailable,
    profitEditUpdateRevertChangesAvailable,
    profitShareEditUpdateShowSearch,
    profitSharingEdit,
    profitSharingUpdate,
    profitSharingEditQueryParams,
    profitShareApplyOrRevertLoading,
    totalForfeituresGreaterThanZero,
    invalidProfitShareEditYear,

    // Validation
    validationData,
    getFieldValidation,

    // Actions
    saveAction,
    revertAction,
    onStatusSearch,
    handleOpenSaveModal,
    handleCloseSaveModal,
    handleOpenRevertModal,
    handleCloseRevertModal,
    handleCloseEmptyModal
  };
};

export default useProfitShareEditUpdate;
```

#### 2.2: Update ProfitShareEditUpdate.tsx to Use the Hook

**Changes**:
- Remove `useRevertAction` and `useSaveAction` custom hooks
- Remove all modal state (`useState` calls for modals)
- Remove validation state
- Remove updatedBy/updatedTime state
- Import and call `useProfitShareEditUpdate`
- Pass hook values to child components

**Before**:
```typescript
const ProfitShareEditUpdate = () => {
  const [openSaveModal, setOpenSaveModal] = useState(false);
  const [openRevertModal, setOpenRevertModal] = useState(false);
  const [openEmptyModal, setOpenEmptyModal] = useState(false);
  const [changesApplied, setChangesApplied] = useState(false);
  const [minimumFieldsEntered, setMinimumFieldsEntered] = useState(false);
  // ... many more state variables

  const saveAction = useSaveAction();
  const revertAction = useRevertAction(setChangesApplied);

  // ... 600+ lines of logic
};
```

**After**:
```typescript
const ProfitShareEditUpdate = () => {
  const {
    changesApplied,
    openSaveModal,
    openRevertModal,
    openEmptyModal,
    minimumFieldsEntered,
    adjustedBadgeOneValid,
    adjustedBadgeTwoValid,
    updatedBy,
    updatedTime,
    profitEditUpdateChangesAvailable,
    profitShareEditUpdateShowSearch,
    profitSharingEdit,
    profitSharingUpdate,
    profitSharingEditQueryParams,
    validationData,
    getFieldValidation,
    saveAction,
    revertAction,
    handleOpenSaveModal,
    handleCloseSaveModal,
    handleOpenRevertModal,
    handleCloseRevertModal,
    handleCloseEmptyModal
  } = useProfitShareEditUpdate();

  const isReadOnly = useReadOnlyNavigation();
  const [isLoading] = useState(false); // Keep if needed for local loading states

  // ... much cleaner rendering logic
};
```

#### 2.3: Expected Results

- **Main file reduced**: From ~640 lines to ~300-400 lines
- **Custom hook**: ~400-500 lines (testable in isolation)
- **Total reduction**: ~200-300 lines of code (better organization, not just deletion)
- **Complexity**: Significantly reduced - main component focuses on rendering

---

## Phase 3: Write Unit Tests for Custom Hook

**Goal**: Ensure the custom hook works correctly and prevent regressions.

### Estimated Effort

- **Time**: 2-3 hours
- **Complexity**: Medium
- **Critical**: Yes (this is a financial calculation system)

### Tasks

#### 3.1: Create `hooks/useProfitShareEditUpdate.test.ts`

**Test Structure**:

```typescript
import { renderHook, act, waitFor } from "@testing-library/react";
import { Provider } from "react-redux";
import { configureStore } from "@reduxjs/toolkit";
import { useProfitShareEditUpdate } from "./useProfitShareEditUpdate";
import * as YearsEndApi from "../../../../reduxstore/api/YearsEndApi";

// Mock the API hooks
jest.mock("../../../../reduxstore/api/YearsEndApi");
jest.mock("../../../../hooks/useFiscalCloseProfitYear");
jest.mock("../../../../hooks/useChecksumValidation");

describe("useProfitShareEditUpdate", () => {
  let store: ReturnType<typeof configureStore>;

  beforeEach(() => {
    // Setup mock store
    store = configureStore({
      reducer: {
        yearsEnd: yearsEndReducer,
        // ... other reducers
      },
      preloadedState: {
        yearsEnd: {
          profitSharingEditQueryParams: {
            contributionPercent: 5,
            earningsPercent: 3,
            maxAllowedContributions: 10000
          },
          // ... other state
        }
      }
    });
  });

  describe("Initial State", () => {
    it("should initialize with correct default values", () => {
      const { result } = renderHook(() => useProfitShareEditUpdate(), {
        wrapper: ({ children }) => <Provider store={store}>{children}</Provider>
      });

      expect(result.current.changesApplied).toBe(false);
      expect(result.current.openSaveModal).toBe(false);
      expect(result.current.openRevertModal).toBe(false);
      expect(result.current.openEmptyModal).toBe(false);
    });
  });

  describe("Save Action", () => {
    it("should successfully apply changes", async () => {
      const mockApplyMaster = jest.fn().mockResolvedValue({
        unwrap: () =>
          Promise.resolve({
            employeesEffected: 100,
            beneficiariesEffected: 50,
            etvasEffected: 150
          })
      });

      (YearsEndApi.useGetMasterApplyMutation as jest.Mock).mockReturnValue([
        mockApplyMaster
      ]);

      const { result } = renderHook(() => useProfitShareEditUpdate(), {
        wrapper: ({ children }) => <Provider store={store}>{children}</Provider>
      });

      await act(async () => {
        await result.current.saveAction();
      });

      expect(mockApplyMaster).toHaveBeenCalledWith(
        expect.objectContaining({
          profitYear: 2023,
          contributionPercent: 5,
          earningsPercent: 3,
          maxAllowedContributions: 10000
        })
      );

      await waitFor(() => {
        expect(result.current.changesApplied).toBe(true);
      });
    });

    it("should handle save errors gracefully", async () => {
      const mockApplyMaster = jest.fn().mockResolvedValue({
        unwrap: () => Promise.reject(new Error("API Error"))
      });

      (YearsEndApi.useGetMasterApplyMutation as jest.Mock).mockReturnValue([
        mockApplyMaster
      ]);

      const { result } = renderHook(() => useProfitShareEditUpdate(), {
        wrapper: ({ children }) => <Provider store={store}>{children}</Provider>
      });

      await act(async () => {
        await result.current.saveAction();
      });

      // Should not mark as applied on error
      expect(result.current.changesApplied).toBe(false);
    });
  });

  describe("Revert Action", () => {
    it("should successfully revert changes", async () => {
      const mockRevert = jest.fn().mockResolvedValue({
        unwrap: () =>
          Promise.resolve({
            employeesEffected: 100,
            beneficiariesEffected: 50,
            etvasEffected: 150
          })
      });

      (YearsEndApi.useLazyGetMasterRevertQuery as jest.Mock).mockReturnValue([
        mockRevert
      ]);

      // Start with changes applied
      const { result } = renderHook(() => useProfitShareEditUpdate(), {
        wrapper: ({ children }) => <Provider store={store}>{children}</Provider>
      });

      // First apply changes
      await act(async () => {
        result.current.handleOpenSaveModal();
      });

      // Then revert
      await act(async () => {
        await result.current.revertAction();
      });

      expect(mockRevert).toHaveBeenCalledWith(
        expect.objectContaining({
          profitYear: 2023
        }),
        false
      );

      await waitFor(() => {
        expect(result.current.changesApplied).toBe(false);
      });
    });
  });

  describe("Form Validation", () => {
    it("should validate minimum fields correctly", () => {
      const storeWithMinFields = configureStore({
        reducer: { yearsEnd: yearsEndReducer },
        preloadedState: {
          yearsEnd: {
            profitSharingEditQueryParams: {
              contributionPercent: 5,
              earningsPercent: 3,
              maxAllowedContributions: 10000
            }
          }
        }
      });

      const { result } = renderHook(() => useProfitShareEditUpdate(), {
        wrapper: ({ children }) => <Provider store={storeWithMinFields}>{children}</Provider>
      });

      expect(result.current.minimumFieldsEntered).toBe(true);
    });

    it("should fail validation when minimum fields missing", () => {
      const storeWithoutMinFields = configureStore({
        reducer: { yearsEnd: yearsEndReducer },
        preloadedState: {
          yearsEnd: {
            profitSharingEditQueryParams: {
              contributionPercent: 5,
              earningsPercent: 0, // Missing earnings
              maxAllowedContributions: 10000
            }
          }
        }
      });

      const { result } = renderHook(() => useProfitShareEditUpdate(), {
        wrapper: ({ children }) => <Provider store={storeWithoutMinFields}>{children}</Provider>
      });

      expect(result.current.minimumFieldsEntered).toBe(false);
    });

    it("should validate badge 1 adjustments", () => {
      const storeWithBadge1 = configureStore({
        reducer: { yearsEnd: yearsEndReducer },
        preloadedState: {
          yearsEnd: {
            profitSharingEditQueryParams: {
              contributionPercent: 5,
              earningsPercent: 3,
              maxAllowedContributions: 10000,
              badgeToAdjust: 12345,
              adjustContributionAmount: 100,
              adjustEarningsAmount: 50,
              adjustIncomingForfeitAmount: 25
            }
          }
        }
      });

      const { result } = renderHook(() => useProfitShareEditUpdate(), {
        wrapper: ({ children }) => <Provider store={storeWithBadge1}>{children}</Provider>
      });

      expect(result.current.adjustedBadgeOneValid).toBe(true);
    });

    it("should fail badge 1 validation when fields incomplete", () => {
      const storeWithIncompleteBadge1 = configureStore({
        reducer: { yearsEnd: yearsEndReducer },
        preloadedState: {
          yearsEnd: {
            profitSharingEditQueryParams: {
              contributionPercent: 5,
              earningsPercent: 3,
              maxAllowedContributions: 10000,
              badgeToAdjust: 12345,
              adjustContributionAmount: 100,
              adjustEarningsAmount: 0, // Missing earnings
              adjustIncomingForfeitAmount: 25
            }
          }
        }
      });

      const { result } = renderHook(() => useProfitShareEditUpdate(), {
        wrapper: ({ children }) => <Provider store={storeWithIncompleteBadge1}>{children}</Provider>
      });

      expect(result.current.adjustedBadgeOneValid).toBe(false);
    });

    it("should validate badge 2 adjustments", () => {
      const storeWithBadge2 = configureStore({
        reducer: { yearsEnd: yearsEndReducer },
        preloadedState: {
          yearsEnd: {
            profitSharingEditQueryParams: {
              contributionPercent: 5,
              earningsPercent: 3,
              maxAllowedContributions: 10000,
              badgeToAdjust2: 67890,
              adjustEarningsSecondaryAmount: 75
            }
          }
        }
      });

      const { result } = renderHook(() => useProfitShareEditUpdate(), {
        wrapper: ({ children }) => <Provider store={storeWithBadge2}>{children}</Provider>
      });

      expect(result.current.adjustedBadgeTwoValid).toBe(true);
    });
  });

  describe("Modal Management", () => {
    it("should open save modal when validation passes", () => {
      const { result } = renderHook(() => useProfitShareEditUpdate(), {
        wrapper: ({ children }) => <Provider store={store}>{children}</Provider>
      });

      act(() => {
        result.current.handleOpenSaveModal();
      });

      expect(result.current.openSaveModal).toBe(true);
    });

    it("should open empty modal when validation fails", () => {
      const storeWithInvalidForm = configureStore({
        reducer: { yearsEnd: yearsEndReducer },
        preloadedState: {
          yearsEnd: {
            profitSharingEditQueryParams: {
              contributionPercent: 0, // Invalid
              earningsPercent: 0, // Invalid
              maxAllowedContributions: 0 // Invalid
            }
          }
        }
      });

      const { result } = renderHook(() => useProfitShareEditUpdate(), {
        wrapper: ({ children }) => <Provider store={storeWithInvalidForm}>{children}</Provider>
      });

      act(() => {
        result.current.handleOpenSaveModal();
      });

      expect(result.current.openEmptyModal).toBe(true);
      expect(result.current.openSaveModal).toBe(false);
    });

    it("should close modals when handlers called", () => {
      const { result } = renderHook(() => useProfitShareEditUpdate(), {
        wrapper: ({ children }) => <Provider store={store}>{children}</Provider>
      });

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
  });

  describe("Year Validation", () => {
    it("should allow previous year (currentYear - 1)", () => {
      const currentYear = new Date().getFullYear();
      const mockUseFiscalCloseProfitYear = require("../../../../hooks/useFiscalCloseProfitYear");
      mockUseFiscalCloseProfitYear.default.mockReturnValue(currentYear - 1);

      const { result } = renderHook(() => useProfitShareEditUpdate(), {
        wrapper: ({ children }) => <Provider store={store}>{children}</Provider>
      });

      expect(result.current.invalidProfitShareEditYear).toBe(false);
    });

    it("should block current year", () => {
      const currentYear = new Date().getFullYear();
      const mockUseFiscalCloseProfitYear = require("../../../../hooks/useFiscalCloseProfitYear");
      mockUseFiscalCloseProfitYear.default.mockReturnValue(currentYear);

      const { result } = renderHook(() => useProfitShareEditUpdate(), {
        wrapper: ({ children }) => <Provider store={store}>{children}</Provider>
      });

      expect(result.current.invalidProfitShareEditYear).toBe(true);
    });

    it("should block years older than previous year", () => {
      const currentYear = new Date().getFullYear();
      const mockUseFiscalCloseProfitYear = require("../../../../hooks/useFiscalCloseProfitYear");
      mockUseFiscalCloseProfitYear.default.mockReturnValue(currentYear - 2);

      const { result } = renderHook(() => useProfitShareEditUpdate(), {
        wrapper: ({ children }) => <Provider store={store}>{children}</Provider>
      });

      expect(result.current.invalidProfitShareEditYear).toBe(true);
    });
  });

  describe("Status Fetching", () => {
    it("should fetch and set profit master status on mount", async () => {
      const mockTriggerStatus = jest.fn().mockResolvedValue({
        unwrap: () =>
          Promise.resolve({
            updatedBy: "John Doe",
            updatedTime: "2024-01-15T10:30:00Z"
          })
      });

      (YearsEndApi.useLazyGetProfitMasterStatusQuery as jest.Mock).mockReturnValue([
        mockTriggerStatus
      ]);

      const { result } = renderHook(() => useProfitShareEditUpdate(), {
        wrapper: ({ children }) => <Provider store={store}>{children}</Provider>
      });

      await waitFor(() => {
        expect(result.current.updatedBy).toBe("John Doe");
        expect(result.current.updatedTime).toBe("2024-01-15T10:30:00Z");
      });
    });

    it("should mark changes as applied when status has updatedTime", async () => {
      const mockTriggerStatus = jest.fn().mockResolvedValue({
        unwrap: () =>
          Promise.resolve({
            updatedBy: "Jane Smith",
            updatedTime: "2024-01-15T14:45:00Z"
          })
      });

      (YearsEndApi.useLazyGetProfitMasterStatusQuery as jest.Mock).mockReturnValue([
        mockTriggerStatus
      ]);

      const { result } = renderHook(() => useProfitShareEditUpdate(), {
        wrapper: ({ children }) => <Provider store={store}>{children}</Provider>
      });

      await waitFor(() => {
        expect(result.current.changesApplied).toBe(true);
      });
    });
  });
});
```

#### 3.2: Test Coverage Goals

- **Target**: 90%+ code coverage
- **Critical paths**:
  - Save action success/failure
  - Revert action success/failure
  - All validation rules
  - Modal state transitions
  - Year validation logic
  - Status fetching and state updates

#### 3.3: Run Tests

```bash
npm --prefix ./src/ui test -- useProfitShareEditUpdate.test.ts --coverage
```

---

## Phase 4: Integration Testing (Optional but Recommended)

**Goal**: Ensure the refactored components work together correctly in the full page context.

### Estimated Effort

- **Time**: 1-2 hours
- **Complexity**: Medium

### Tasks

#### 4.1: Manual Testing Checklist

- [ ] **Preview Flow**: Enter form parameters → Click Preview → Verify grids show data
- [ ] **Save Flow**: Preview → Click Save Updates → Confirm modal → Verify success message
- [ ] **Revert Flow**: After save → Click Revert → Confirm modal → Verify reverted state
- [ ] **Validation**: Try to save without minimum fields → Verify empty modal shows
- [ ] **Badge 1 Validation**: Enter badge without adjustment amounts → Verify error
- [ ] **Badge 2 Validation**: Enter badge 2 without secondary earnings → Verify error
- [ ] **Year Validation**: Select current year → Verify save button disabled with tooltip
- [ ] **Forfeitures Validation**: Ensure total forfeitures ≠ 0 → Verify save button disabled
- [ ] **Prerequisites**: Test with incomplete prerequisites → Verify save button disabled
- [ ] **Read-Only Mode**: Enable read-only → Verify buttons disabled with tooltips
- [ ] **Changes Applied State**: After save → Verify filter hidden, changes list shown
- [ ] **Checksum Validation**: Verify validation icons appear in summary table
- [ ] **Status Persistence**: Refresh page after save → Verify "changes applied" state persists

#### 4.2: Create Integration Test (Optional)

```typescript
// ProfitShareEditUpdate.integration.test.tsx
import { render, screen, waitFor, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { Provider } from "react-redux";
import { store } from "../../../reduxstore/store";
import ProfitShareEditUpdate from "./ProfitShareEditUpdate";

describe("ProfitShareEditUpdate Integration", () => {
  it("should complete full save workflow", async () => {
    const user = userEvent.setup();

    render(
      <Provider store={store}>
        <ProfitShareEditUpdate />
      </Provider>
    );

    // Fill in form
    await user.type(screen.getByLabelText(/contribution %/i), "5");
    await user.type(screen.getByLabelText(/earnings %/i), "3");
    await user.type(screen.getByLabelText(/max allowed/i), "10000");

    // Click Preview
    await user.click(screen.getByRole("button", { name: /preview/i }));

    // Wait for grids to load
    await waitFor(() => {
      expect(screen.getByText(/preview updates/i)).toBeInTheDocument();
    });

    // Click Save
    await user.click(screen.getByRole("button", { name: /save updates/i }));

    // Confirm in modal
    const modal = screen.getByRole("dialog");
    await user.click(within(modal).getByRole("button", { name: /yes, save/i }));

    // Verify success message
    await waitFor(() => {
      expect(screen.getByText(/successfully applied/i)).toBeInTheDocument();
    });
  });
});
```

---

## Phase 5: Documentation Updates

**Goal**: Document the new architecture for future developers.

### Estimated Effort

- **Time**: 30 minutes
- **Complexity**: Low

### Tasks

#### 5.1: Update Component Documentation

Update `Claude.md` in the ProfitShareEditUpdate directory:

```markdown
## Refactored Architecture (2025)

The ProfitShareEditUpdate page has been refactored into a modular, testable architecture:

### Custom Hook: `useProfitShareEditUpdate`

**Location**: `hooks/useProfitShareEditUpdate.ts`

**Purpose**: Encapsulates all business logic, state management, API calls, and validation.

**Returns**:
- State: `changesApplied`, `openSaveModal`, `openRevertModal`, etc.
- Actions: `saveAction`, `revertAction`, modal handlers
- Validation: `minimumFieldsEntered`, `adjustedBadgeOneValid`, etc.
- Redux state: Passthrough of Redux selectors

**Testing**: See `useProfitShareEditUpdate.test.ts` for unit tests.

### Button Components

**ProfitShareSaveButton**: Handles save button rendering, validation tooltips, and modal opening.

**ProfitShareRevertButton**: Handles revert button rendering and modal opening.

Both components are pure presentation components that receive all state via props.

### Utility Functions

**`utils/formValidation.ts`**: Contains `wasFormUsed` helper for checking if form has any values.

### Benefits of Refactored Architecture

1. **Testability**: Business logic in custom hook can be unit tested independently
2. **Separation of Concerns**: Rendering logic separate from business logic
3. **Reusability**: Button components are reusable and encapsulated
4. **Maintainability**: Each file has a single, clear responsibility
5. **Reduced Complexity**: Main component reduced from 800 to ~400 lines
```

#### 5.2: Update Main README (if exists)

Add section about the refactoring:

```markdown
## Recent Improvements

### ProfitShareEditUpdate Refactoring (2025)

The complex 800-line ProfitShareEditUpdate component has been refactored into a modular architecture:

- **Custom hook pattern**: Business logic extracted to `useProfitShareEditUpdate`
- **Component extraction**: Button components separated into reusable files
- **Comprehensive tests**: 90%+ coverage of critical business logic
- **Reduced complexity**: Main component reduced by 50%

For details, see `src/ui/src/pages/FiscalClose/ProfitShareEditUpdate/REFACTORING_PLAN.md`
```

---

## Success Criteria

### Phase 2 Complete When:
- ✅ Custom hook created with all business logic
- ✅ Main component updated to use hook
- ✅ ESLint passes (0 warnings)
- ✅ TypeScript compiles successfully
- ✅ Main file reduced to ~300-400 lines

### Phase 3 Complete When:
- ✅ Unit tests written for all critical paths
- ✅ Test coverage ≥ 90%
- ✅ All tests passing
- ✅ Edge cases covered (validation, errors, state transitions)

### Phase 4 Complete When:
- ✅ All manual testing checklist items verified
- ✅ Integration test created (optional)
- ✅ No regressions found

### Phase 5 Complete When:
- ✅ Component documentation updated
- ✅ Architecture documented
- ✅ Testing instructions added

---

## Risk Mitigation

### High-Risk Areas

1. **Financial Calculations**: This affects profit-sharing payouts
   - **Mitigation**: Comprehensive unit tests, manual verification with test data

2. **State Synchronization**: Complex Redux + local state
   - **Mitigation**: Careful review of all Redux dispatches, test state transitions

3. **Validation Logic**: Multiple validation systems must work together
   - **Mitigation**: Test all validation paths, edge cases

### Rollback Plan

If issues are discovered after deployment:

1. **Immediate**: Revert to previous commit (before Phase 2)
2. **Short-term**: Create hotfix branch from last stable state
3. **Long-term**: Address issues in refactored code, add missing tests

---

## Estimated Total Effort

| Phase | Time Estimate | Complexity |
|-------|---------------|------------|
| Phase 1 | ✅ Complete | Low-Medium |
| Phase 2 | 3-4 hours | High |
| Phase 3 | 2-3 hours | Medium |
| Phase 4 | 1-2 hours | Medium |
| Phase 5 | 30 minutes | Low |
| **Total** | **6.5-9.5 hours** | **High** |

---

## Next Steps

1. **Review this plan** with team lead/senior developer
2. **Schedule dedicated time** for Phase 2 (requires focus, no interruptions)
3. **Set up test environment** with representative data
4. **Begin Phase 2** when ready
5. **Commit after each phase** to enable easy rollback if needed

---

## Questions to Address Before Phase 2

- [ ] Do we have representative test data for manual testing?
- [ ] Is there a QA environment where this can be tested thoroughly?
- [ ] Who should review the custom hook implementation?
- [ ] Are there any upcoming releases that would conflict with this refactoring?
- [ ] Should we create a feature flag to enable/disable the refactored version?

---

## References

- **Original File**: `ProfitShareEditUpdate.tsx` (800 lines)
- **Complexity Analysis**: See `Claude.md` in same directory
- **Pattern**: Follows MilitaryContributions refactoring pattern
- **Similar Refactorings**: RecentlyTerminated, TerminatedLetters, PayMasterUpdate (all completed 2025)
