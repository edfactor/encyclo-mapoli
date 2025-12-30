import { useCallback, useEffect, useReducer, useRef } from "react";
import { useDispatch, useSelector } from "react-redux";
import { setMessage } from "smart-ui-library";
import useFiscalCloseProfitYear from "../../../../hooks/useFiscalCloseProfitYear";
import {
  useGetMasterApplyMutation,
  useLazyGetMasterRevertQuery,
  useLazyGetProfitMasterStatusQuery
} from "../../../../reduxstore/api/YearsEndApi";
import {
  clearProfitSharingEdit,
  clearProfitSharingEditQueryParams,
  clearProfitSharingUpdate,
  setProfitEditUpdateChangesAvailable,
  setProfitEditUpdateRevertChangesAvailable,
  setProfitShareApplyOrRevertLoading,
  setProfitShareEditUpdateShowSearch,
  setResetYearEndPage
} from "../../../../reduxstore/slices/yearsEndSlice";
import { RootState } from "../../../../reduxstore/store";
import { ProfitShareMasterApplyRequest, ProfitYearRequest } from "../../../../reduxstore/types";
import { Messages } from "../../../../utils/messageDictonary";
import { useChecksumValidation } from "./useChecksumValidation";

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
  const hasToken = !!useSelector((state: RootState) => state.security.token);

  // Track if we've fetched status on initial mount to avoid multiple fetches
  const hasInitializedRef = useRef(false);

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
    invalidProfitShareEditYear,
    profitMasterStatus
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
            (profitSharingUpdate.profitShareUpdateTotals.paidAllocations || 0),
          TotalForfeitPoints: profitSharingUpdate.profitShareUpdateTotals.contributionPoints,
          TotalEarningPoints: profitSharingUpdate.profitShareUpdateTotals.earningPoints
        }
      : undefined
  });

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
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [profitSharingEditQueryParams]);

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
          reduxDispatch(setProfitShareEditUpdateShowSearch(false));
        }
      }
    } catch (error) {
      console.error("Failed to fetch profit master status:", error);
    }
  }, [profitYear, triggerStatus, reduxDispatch]);

  // Initialize status on mount
  useEffect(() => {
    if (hasToken && !hasInitializedRef.current) {
      hasInitializedRef.current = true;
      onStatusSearch();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [hasToken]);

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
    profitMasterStatus,

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
