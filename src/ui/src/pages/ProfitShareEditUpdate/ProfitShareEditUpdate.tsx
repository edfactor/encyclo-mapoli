import { Replay } from "@mui/icons-material";
import InfoOutlinedIcon from "@mui/icons-material/InfoOutlined";
import { Alert, AlertTitle, Button, CircularProgress, Grid, Tooltip, Typography } from "@mui/material";
import { useCallback, useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { DSMAccordion, numberToCurrency, Page, setMessage, SmartModal, TotalsGrid } from "smart-ui-library";
import StatusDropdownActionNode from "../../components/StatusDropdownActionNode";
import useFiscalCloseProfitYear from "../../hooks/useFiscalCloseProfitYear";
import { useReadOnlyNavigation } from "../../hooks/useReadOnlyNavigation";
import {
  useGetMasterApplyMutation,
  useLazyGetMasterRevertQuery,
  useLazyGetProfitMasterStatusQuery
} from "../../reduxstore/api/YearsEndApi";
import {
  clearProfitSharingEdit,
  clearProfitSharingEditQueryParams,
  clearProfitSharingUpdate,
  setInvalidProfitShareEditYear,
  setProfitEditUpdateChangesAvailable,
  setProfitEditUpdateRevertChangesAvailable,
  setProfitShareApplyOrRevertLoading,
  setProfitShareEditUpdateShowSearch,
  setResetYearEndPage
} from "../../reduxstore/slices/yearsEndSlice";
import { RootState } from "../../reduxstore/store";
import {
  ProfitMasterStatus,
  ProfitShareEditUpdateQueryParams,
  ProfitShareMasterApplyRequest,
  ProfitShareMasterResponse,
  ProfitYearRequest
} from "../../reduxstore/types";
// usePrerequisiteNavigations now encapsulated by PrerequisiteGuard
import PrerequisiteGuard from "../../components/PrerequisiteGuard";
import { MasterUpdateCrossReferenceValidationResponse } from "../../types/validation/cross-reference-validation";
import { MessageKeys, Messages } from "../../utils/messageDictonary";
import ChangesList from "./ChangesList";
import ProfitShareEditConfirmation from "./ProfitShareEditConfirmation";
import ProfitShareEditUpdateSearchFilter from "./ProfitShareEditUpdateSearchFilter";
import ProfitShareEditUpdateTabs from "./ProfitShareEditUpdateTabs";

const useRevertAction = (
  setEmployeesReverted: (count: number) => void,
  setBeneficiariesReverted: (count: number) => void,
  setEtvasReverted: (count: number) => void,
  setChangesApplied: (changes: boolean) => void
) => {
  const [trigger] = useLazyGetMasterRevertQuery();
  const dispatch = useDispatch();
  const profitYear = useFiscalCloseProfitYear();
  //const { profitSharingEdit, profitSharingUpdate } = useSelector((state: RootState) => state.yearsEnd);

  const revertAction = async (): Promise<void> => {
    const params: ProfitYearRequest = {
      profitYear: profitYear ?? 0
    };

    await trigger(params, false)
      .unwrap()
      .then((payload) => {
        setEmployeesReverted(payload?.employeesEffected || 0);
        setBeneficiariesReverted(payload?.beneficiariesEffected || 0);
        setEtvasReverted(payload?.etvasEffected || 0);
        //dispatch(setMessage(successMessage));
        dispatch(setProfitEditUpdateChangesAvailable(false));
        dispatch(setProfitEditUpdateRevertChangesAvailable(false));
        dispatch(clearProfitSharingEditQueryParams());
        dispatch(
          setMessage({
            ...Messages.ProfitShareRevertSuccess,
            message: {
              ...Messages.ProfitShareRevertSuccess.message,
              message: `Employees affected: ${payload?.employeesEffected} | Beneficiaries: ${payload?.beneficiariesEffected} | ETVAs: ${payload?.etvasEffected} `
            }
          })
        );
        // Clear form and grids (we need to call reset on the search filter page)
        dispatch(setResetYearEndPage(true));
        // Bring search filters back
        dispatch(setProfitShareEditUpdateShowSearch(true));
        dispatch(clearProfitSharingEdit());
        dispatch(clearProfitSharingUpdate());
        // Set the changes applied to false
        setChangesApplied(false);
      })
      .catch((error) => {
        console.error("ERROR: Did not revert changes to year end", error);
        //dispatch(setMessage(failMessage));
        dispatch(
          setMessage({
            ...Messages.ProfitShareRevertFail,
            message: {
              ...Messages.ProfitShareRevertFail.message,
              message: `Employees affected: 0 | Beneficiaries: 0, | ETVAs: 0 `
            }
          })
        );
        setEmployeesReverted(0);
        setBeneficiariesReverted(0);
        setEtvasReverted(0);
      });
  };
  return revertAction;
};

const useSaveAction = (
  setEmployeesReverted: (count: number) => void,
  setBeneficiariesReverted: (count: number) => void,
  setEtvasReverted: (count: number) => void,
  setValidationResponse: (response: MasterUpdateCrossReferenceValidationResponse | null) => void
) => {
  const { profitSharingEditQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const [applyMaster] = useGetMasterApplyMutation();
  const dispatch = useDispatch();
  const profitYear = useFiscalCloseProfitYear();

  const saveAction = async (): Promise<void> => {
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

    dispatch(setProfitShareApplyOrRevertLoading(true));

    await applyMaster(params)
      .unwrap()
      .then((payload: ProfitShareMasterResponse) => {
        dispatch(setProfitEditUpdateChangesAvailable(false));

        console.log("Successfully applied changes to year end: ", payload);
        console.log("Employees affected: ", payload?.employeesEffected);

        // Capture cross-reference validation if present
        if (payload.crossReferenceValidation) {
          setValidationResponse(payload.crossReferenceValidation);
          console.log("Cross-reference validation:", payload.crossReferenceValidation);
        }

        setEmployeesReverted(payload?.employeesEffected ?? 0);
        setBeneficiariesReverted(payload?.beneficiariesEffected ?? 0);
        setEtvasReverted(payload?.etvasEffected ?? 0);
        dispatch(
          setMessage({
            ...Messages.ProfitShareApplySuccess,
            message: {
              ...Messages.ProfitShareApplySuccess.message,
              message: `Employees affected: ${payload?.employeesEffected} | Beneficiaries: ${payload?.beneficiariesEffected} | ETVAs: ${payload?.etvasEffected} `
            }
          })
        );
        dispatch(setResetYearEndPage(true));
        dispatch(setProfitEditUpdateRevertChangesAvailable(true));
        dispatch(setProfitShareEditUpdateShowSearch(false));
        // Clear the grids
        dispatch(clearProfitSharingUpdate());
      })
      .catch((error) => {
        console.error("ERROR: Did not apply changes to year end", error);
        dispatch(
          setMessage({
            ...Messages.ProfitShareApplyFail,
            message: {
              ...Messages.ProfitShareApplyFail.message,
              message: `Employees affected: 0 | Beneficiaries: 0, | ETVAs: 0 `
            }
          })
        );
      });
    dispatch(setProfitShareApplyOrRevertLoading(false));
  };

  return saveAction;
};

const wasFormUsed = (profitSharingEditQueryParams: ProfitShareEditUpdateQueryParams) => {
  return (
    (profitSharingEditQueryParams?.contributionPercent ?? 0) > 0 ||
    (profitSharingEditQueryParams?.earningsPercent ?? 0) > 0 ||
    (profitSharingEditQueryParams?.incomingForfeitPercent ?? 0) > 0 ||
    (profitSharingEditQueryParams?.maxAllowedContributions ?? 0) > 0 ||
    (profitSharingEditQueryParams?.badgeToAdjust ?? 0) > 0 ||
    (profitSharingEditQueryParams?.adjustContributionAmount ?? 0) > 0 ||
    (profitSharingEditQueryParams?.adjustEarningsAmount ?? 0) > 0 ||
    (profitSharingEditQueryParams?.adjustIncomingForfeitAmount ?? 0) > 0 ||
    (profitSharingEditQueryParams?.badgeToAdjust2 ?? 0) > 0 ||
    (profitSharingEditQueryParams?.adjustEarningsSecondaryAmount ?? 0) > 0
  );
};

// This really just opens the modal. The modal for this has the function to call
// the back end
const RenderSaveButton = (
  setOpenSaveModal: (open: boolean) => void,
  setOpenEmptyModal: (open: boolean) => void,
  status: ProfitMasterStatus | null,
  isLoading: boolean,
  minimumFieldsEntered: boolean = false,
  adjustedBadgeOneValid: boolean = true,
  adjustedBadgeTwoValid: boolean = true,
  prerequisitesComplete: boolean = true,
  isReadOnly: boolean = false
) => {
  // The incoming status field is about whether or not changes have already been applied
  const {
    profitEditUpdateChangesAvailable,
    profitSharingEditQueryParams,
    profitShareApplyOrRevertLoading,
    totalForfeituresGreaterThanZero,
    invalidProfitShareEditYear
  } = useSelector((state: RootState) => state.yearsEnd);
  const navigationList = useSelector((state: RootState) => state.navigation.navigationData);
  const currentNavigationId = parseInt(localStorage.getItem("navigationId") ?? "");
  // Determine tooltip reason when disabled by prerequisites
  const prereqTooltip = !prerequisitesComplete
    ? "All prerequisite navigations must be complete before saving."
    : undefined;
  const saveButton = (
    <Button
      disabled={
        (!profitEditUpdateChangesAvailable && status?.updatedTime !== null) ||
        isLoading ||
        totalForfeituresGreaterThanZero ||
        invalidProfitShareEditYear ||
        !prerequisitesComplete ||
        isReadOnly
      }
      variant="outlined"
      color="primary"
      size="medium"
      onClick={
        isReadOnly
          ? undefined
          : async () => {
              if (
                profitSharingEditQueryParams &&
                wasFormUsed(profitSharingEditQueryParams) &&
                adjustedBadgeOneValid &&
                adjustedBadgeTwoValid &&
                minimumFieldsEntered &&
                prerequisitesComplete
              ) {
                setOpenSaveModal(true);
              } else {
                setOpenEmptyModal(true);
              }
            }
      }>
      {isLoading || profitShareApplyOrRevertLoading ? (
        //Prevent loading spinner from shrinking button
        <div className="spinner">
          <CircularProgress
            color="inherit"
            size="20px"
          />
        </div>
      ) : (
        "Save Updates"
      )}
    </Button>
  );

  if (
    !profitEditUpdateChangesAvailable ||
    invalidProfitShareEditYear ||
    totalForfeituresGreaterThanZero ||
    !prerequisitesComplete ||
    isReadOnly
  ) {
    return (
      <Tooltip
        placement="top"
        title={
          isReadOnly
            ? "You are in read-only mode and cannot apply changes."
            : invalidProfitShareEditYear
              ? "Invalid year for saving changes"
              : totalForfeituresGreaterThanZero == true
                ? "Total forfeitures is greater than zero."
                : !prerequisitesComplete
                  ? prereqTooltip
                  : "You must have previewed data before saving."
        }>
        <span>{saveButton}</span>
      </Tooltip>
    );
  } else {
    return saveButton;
  }
};

// This really just opens the modal. The modal for this has the function to call
// the back end
const RenderRevertButton = (
  setOpenRevertModal: (open: boolean) => void,
  isLoading: boolean,
  isReadOnly: boolean = false
) => {
  // The incoming status field is about whether or not changes have already been applied
  const { profitEditUpdateRevertChangesAvailable, profitShareApplyOrRevertLoading } = useSelector(
    (state: RootState) => state.yearsEnd
  );

  const revertButton = (
    <Button
      disabled={!profitEditUpdateRevertChangesAvailable || isLoading || isReadOnly}
      variant="outlined"
      color="primary"
      size="medium"
      startIcon={
        isLoading ? null : (
          <Replay color={profitEditUpdateRevertChangesAvailable && !isReadOnly ? "primary" : "disabled"} />
        )
      }
      onClick={
        isReadOnly
          ? undefined
          : async () => {
              setOpenRevertModal(true);
            }
      }>
      {isLoading || profitShareApplyOrRevertLoading ? (
        //Prevent loading spinner from shrinking button
        <div className="spinner">
          <CircularProgress
            color="inherit"
            size="20px"
          />
        </div>
      ) : (
        "Revert"
      )}
    </Button>
  );

  if (!profitEditUpdateRevertChangesAvailable || isReadOnly) {
    return (
      <Tooltip
        placement="top"
        title={
          isReadOnly ? "You are in read-only mode and cannot revert changes." : "You must have applied data to revert."
        }>
        <span>{revertButton}</span>
      </Tooltip>
    );
  } else {
    return revertButton;
  }
};

const ProfitShareEditUpdate = () => {
  const [beneficiariesAffected, setBeneficiariesAffected] = useState(0);
  const [employeesAffected, setEmployeesAffected] = useState(0);
  const [etvasAffected, setEtvasAffected] = useState(0);
  const [beneficiariesReverted, setBeneficiariesReverted] = useState(0);
  const [employeesReverted, setEmployeesReverted] = useState(0);
  const [etvasReverted, setEtvasReverted] = useState(0);
  const [updatedBy, setUpdatedBy] = useState<string | null>(null);
  const [updatedTime, setUpdatedTime] = useState<string | null>(null);

  // State for cross-reference validation response
  const [validationResponse, setValidationResponse] = useState<MasterUpdateCrossReferenceValidationResponse | null>(
    null
  );

  // State for validation popup - track which field popup is open
  const [openValidationField, setOpenValidationField] = useState<string | null>(null);

  const handleValidationToggle = (fieldName: string) => {
    setOpenValidationField(openValidationField === fieldName ? null : fieldName);
  };

  // Helper function to find validation for a specific field
  const getFieldValidation = (fieldKey: string) => {
    console.log("getFieldValidation called with fieldKey:", fieldKey);
    console.log("validationResponse:", validationResponse);

    if (!validationResponse) {
      console.log("No validation response");
      return null;
    }

    console.log("validationGroups:", validationResponse.validationGroups);

    for (const group of validationResponse.validationGroups) {
      console.log("Checking group:", group);
      console.log("Group validations:", group.validations);
      const validation = group.validations.find((v) => {
        console.log(`Comparing v.fieldName="${v.fieldName}" with fieldKey="${fieldKey}"`);
        return v.fieldName === fieldKey;
      });
      if (validation) {
        console.log("Found validation:", validation);
        return validation;
      }
    }
    console.log("No validation found for fieldKey:", fieldKey);
    return null;
  };

  // Helper to render validation icon positioned absolutely in a TotalsGrid (like State Taxes pattern)
  const renderValidationIconInGrid = (fieldKey: string, fieldDisplayName: string) => {
    console.log("renderValidationIconInGrid called for:", fieldKey, fieldDisplayName);
    const validation = getFieldValidation(fieldKey);
    console.log("Validation result for", fieldKey, ":", validation);
    if (!validation) {
      console.log("No validation found, returning null for:", fieldKey);
      return null;
    }

    return (
      <div
        className="absolute right-2 top-1/2 -mt-0.5 -translate-y-1/2"
        onClick={() => handleValidationToggle(fieldKey)}>
        <InfoOutlinedIcon
          className={`cursor-pointer ${validation.isValid ? "text-green-500" : "text-orange-500"}`}
          fontSize="small"
        />
        {openValidationField === fieldKey && (
          <div className="absolute left-0 top-full z-[1000] mt-1 max-h-[300px] w-[350px] overflow-auto rounded border border-gray-300 bg-white shadow-lg">
            <div className="p-2 px-4 pb-4">
              <Typography
                variant="subtitle2"
                sx={{ p: 1, fontWeight: "bold" }}>
                {fieldDisplayName}
              </Typography>
              <table className="w-full border-collapse text-[0.95rem]">
                <thead>
                  <tr>
                    <th className="border-b border-gray-300 px-2 py-1 text-left font-semibold">Report</th>
                    <th className="border-b border-gray-300 px-2 py-1 text-right font-semibold">Amount</th>
                  </tr>
                </thead>
                <tbody>
                  <tr>
                    <td className="border-b border-gray-100 px-2 py-1 text-left">Current (PAY444)</td>
                    <td className="border-b border-gray-100 px-2 py-1 text-right">
                      {numberToCurrency(validation.currentValue || 0)}
                    </td>
                  </tr>
                  <tr>
                    <td className="border-b border-gray-100 px-2 py-1 text-left">Expected (PAY443)</td>
                    <td className="border-b border-gray-100 px-2 py-1 text-right">
                      {numberToCurrency(validation.expectedValue || 0)}
                    </td>
                  </tr>
                  {!validation.isValid && (validation.variance || 0) !== 0 && (
                    <tr className="bg-orange-50">
                      <td className="px-2 py-1 text-left font-semibold text-orange-700">Variance</td>
                      <td className="px-2 py-1 text-right font-bold text-orange-700">
                        {numberToCurrency(validation.variance || 0)}
                      </td>
                    </tr>
                  )}
                </tbody>
              </table>
              {validation.message && (
                <Typography
                  variant="caption"
                  sx={{ mt: 1, display: "block", px: 1, color: "text.secondary" }}>
                  {validation.message}
                </Typography>
              )}
              <div className="mt-2 flex items-center justify-end gap-2 px-2">
                <Typography
                  variant="caption"
                  sx={{
                    color: validation.isValid ? "success.main" : "warning.main",
                    fontWeight: "bold"
                  }}>
                  {validation.isValid ? "✓ Values Match" : "⚠ Values Mismatch"}
                </Typography>
              </div>
            </div>
          </div>
        )}
      </div>
    );
  };

  // Helper to render validation icon with popup for a specific field (legacy - replaced by renderValidationIconInGrid)
  const renderValidationIcon = (fieldKey: string, fieldDisplayName: string) => {
    console.log("renderValidationIcon called for:", fieldKey, fieldDisplayName);
    const validation = getFieldValidation(fieldKey);
    console.log("Validation result for", fieldKey, ":", validation);
    if (!validation) {
      console.log("No validation found, returning null for:", fieldKey);
      return null;
    }

    const isOpen = openValidationField === fieldKey;

    return (
      <div className="relative ml-1 inline-block">
        <InfoOutlinedIcon
          className={`cursor-pointer ${validation.isValid ? "text-green-500" : "text-orange-500"}`}
          fontSize="small"
          onClick={() => handleValidationToggle(fieldKey)}
        />
        {isOpen && (
          <div className="absolute left-0 top-full z-[1000] mt-1 w-[350px] rounded border border-gray-300 bg-white shadow-lg">
            <div className="p-3">
              <div className="mb-2 flex items-center justify-between">
                <Typography
                  variant="subtitle2"
                  sx={{ fontWeight: "bold" }}>
                  {fieldDisplayName}
                </Typography>
                <Typography
                  variant="caption"
                  sx={{
                    color: validation.isValid ? "success.main" : "warning.main",
                    fontWeight: "bold"
                  }}>
                  {validation.isValid ? "✓ Match" : "⚠ Mismatch"}
                </Typography>
              </div>
              <table className="w-full border-collapse text-sm">
                <tbody>
                  <tr>
                    <td className="border-b border-gray-200 py-2 pr-2 font-semibold text-gray-700">
                      Current (PAY444):
                    </td>
                    <td className="border-b border-gray-200 py-2 text-right">
                      {numberToCurrency(validation.currentValue || 0)}
                    </td>
                  </tr>
                  <tr>
                    <td className="border-b border-gray-200 py-2 pr-2 font-semibold text-gray-700">
                      Expected (PAY443):
                    </td>
                    <td className="border-b border-gray-200 py-2 text-right">
                      {numberToCurrency(validation.expectedValue || 0)}
                    </td>
                  </tr>
                  {!validation.isValid && (validation.variance || 0) !== 0 && (
                    <tr className="bg-orange-50">
                      <td className="py-2 pr-2 font-semibold text-orange-700">Variance:</td>
                      <td className="py-2 text-right font-semibold text-orange-700">
                        {(validation.variance || 0) > 0 ? "+" : ""}
                        {numberToCurrency(validation.variance || 0)}
                      </td>
                    </tr>
                  )}
                </tbody>
              </table>
              {validation.message && (
                <Typography
                  variant="caption"
                  sx={{ display: "block", mt: 1, color: "text.secondary", fontStyle: "italic" }}>
                  {validation.message}
                </Typography>
              )}
            </div>
          </div>
        )}
      </div>
    );
  };

  // This is a flag used to indicate that the year end change have been made
  // and a banner should be shown indicating this
  const [changesApplied, setChangesApplied] = useState<boolean>(false);

  const revertAction = useRevertAction(
    setEmployeesReverted,
    setBeneficiariesReverted,
    setEtvasReverted,
    setChangesApplied
  );
  const saveAction = useSaveAction(
    setEmployeesAffected,
    setBeneficiariesAffected,
    setEtvasAffected,
    setValidationResponse
  );
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [pageNumberReset, setPageNumberReset] = useState(false);
  const hasToken = !!useSelector((state: RootState) => state.security.token);

  // These are to track validity of fields in the search filter
  const [minimumFieldsEntered, setMinimumFieldsEntered] = useState(false);
  const [adjustedBadgeOneValid, setAdjustedBadgeOneValid] = useState(true);
  const [adjustedBadgeTwoValid, setAdjustedBadgeTwoValid] = useState(true);

  const {
    profitSharingUpdateAdjustmentSummary,
    profitSharingUpdate,
    profitSharingEdit,
    profitSharingEditQueryParams,
    profitMasterStatus,
    profitShareEditUpdateShowSearch,
    profitEditUpdateRevertChangesAvailable,
    totalForfeituresGreaterThanZero
  } = useSelector((state: RootState) => state.yearsEnd);
  const [openSaveModal, setOpenSaveModal] = useState<boolean>(false);
  const [openRevertModal, setOpenRevertModal] = useState<boolean>(false);
  const [openEmptyModal, setOpenEmptyModal] = useState<boolean>(false);

  const [triggerStatusUpdate, { isLoading }] = useLazyGetProfitMasterStatusQuery();

  const profitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();
  const currentNavigationId = parseInt(localStorage.getItem("navigationId") ?? "");
  const isReadOnly = useReadOnlyNavigation();

  // Extract cross-reference validation from profitSharingUpdate response
  useEffect(() => {
    if (profitSharingUpdate?.crossReferenceValidation) {
      setValidationResponse(profitSharingUpdate.crossReferenceValidation);
      console.log("Loaded cross-reference validation from GET response:", profitSharingUpdate.crossReferenceValidation);
    }
  }, [profitSharingUpdate]);

  useEffect(() => {
    const currentYear = new Date().getFullYear();
    if (profitYear !== currentYear - 1) {
      dispatch(setInvalidProfitShareEditYear(true));
      dispatch(
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
      dispatch(setInvalidProfitShareEditYear(false));
    }
  }, [profitYear, dispatch]);

  const onStatusSearch = useCallback(async () => {
    const request: ProfitYearRequest = {
      profitYear: profitYear ?? 0
    };

    await triggerStatusUpdate(request, false)
      .unwrap()
      .then((payload) => {
        if (payload?.updatedBy) {
          setUpdatedBy(payload.updatedBy);

          // Since we have something to revert, set this to button appears
          dispatch(setProfitEditUpdateRevertChangesAvailable(true));
          // Hide the search filters
          dispatch(setProfitShareEditUpdateShowSearch(false));
        }

        if (payload?.updatedTime) {
          setUpdatedTime(
            new Date(payload.updatedTime).toLocaleString("en-US", {
              month: "long",
              day: "numeric",
              year: "numeric",
              hour: "2-digit",
              minute: "2-digit"
            })
          );
        } else {
          dispatch(setProfitEditUpdateRevertChangesAvailable(false));
        }
      })
      .catch((error) => {
        console.error("ERROR: Did not revert changes to year end", error);
      });
  }, [profitYear, triggerStatusUpdate, dispatch]);
  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  useEffect(() => {
    if (hasToken) {
      onStatusSearch();
      if (updatedTime) {
        setChangesApplied(true);
        dispatch(setProfitEditUpdateChangesAvailable(false));
        dispatch(setProfitEditUpdateRevertChangesAvailable(true));
      } else {
        setChangesApplied(false);
      }
    }
  }, [onStatusSearch, hasToken, updatedTime, updatedBy]);

  return (
    <PrerequisiteGuard
      navigationId={currentNavigationId}
      messageTemplate={Messages.ProfitSharePrerequisiteIncomplete as any}>
      {({ prerequisitesComplete }) => (
        <Page
          label="Master Update (PAY444|PAY447)"
          actionNode={
            <div className="flex items-center justify-end gap-2">
              {RenderRevertButton(setOpenRevertModal, isLoading, isReadOnly)}
              {RenderSaveButton(
                setOpenSaveModal,
                setOpenEmptyModal,
                profitMasterStatus,
                isLoading,
                minimumFieldsEntered,
                adjustedBadgeOneValid,
                adjustedBadgeTwoValid,
                prerequisitesComplete,
                isReadOnly
              )}
              {renderActionNode()}
            </div>
          }>
          {
            // We are using an AlertTitle directly and not a missive because we want this alert message
            // to remain in place, not fade away
            changesApplied && (
              <div className="w-full py-3">
                <Alert severity={Messages.ProfitShareMasterUpdated.message.type}>
                  <AlertTitle sx={{ fontWeight: "bold" }}>{Messages.ProfitShareMasterUpdated.message.title}</AlertTitle>
                  {`Updated By: ${updatedBy} | Date: ${updatedTime} `}
                </Alert>
              </div>
            )
          }

          <Grid
            container
            rowSpacing="24px"
            width={"100%"}>
            <Grid
              width={"100%"}
              hidden={!profitShareEditUpdateShowSearch}>
              <DSMAccordion title="Filter">
                <ProfitShareEditUpdateSearchFilter
                  setInitialSearchLoaded={setInitialSearchLoaded}
                  setPageReset={setPageNumberReset}
                  setMinimumFieldsEntered={setMinimumFieldsEntered}
                  setAdjustedBadgeOneValid={setAdjustedBadgeOneValid}
                  setAdjustedBadgeTwoValid={setAdjustedBadgeTwoValid}
                />
              </DSMAccordion>
            </Grid>
            {profitEditUpdateRevertChangesAvailable && (
              <>
                <Grid
                  width="100%"
                  sx={{ marginLeft: "50px" }}>
                  <Typography
                    component="span"
                    variant="h6"
                    sx={{ fontWeight: "bold" }}>
                    These changes have already been applied:
                  </Typography>
                </Grid>
                <Grid
                  width="100%"
                  sx={{ marginLeft: "50px" }}>
                  <ChangesList params={profitSharingEditQueryParams || profitMasterStatus} />
                </Grid>
              </>
            )}
            {profitSharingUpdate && profitSharingUpdate.profitShareUpdateTotals && profitSharingEdit && (
              <Grid width={"100%"}>
                <div className="px-[24px]">
                  <h2 className="text-dsm-secondary">Summary (PAY444)</h2>
                  <Typography
                    fontWeight="bold"
                    variant="body2">
                    {`Employees: ${profitSharingUpdate.profitShareUpdateTotals.totalEmployees} | Beneficiaries: ${profitSharingUpdate.profitShareUpdateTotals.totalBeneficiaries}`}
                  </Typography>
                </div>

                {/* Multiple TotalsGrids side-by-side, exactly like QPAY129 State Taxes pattern */}
                <div className="flex items-start gap-2">
                  {/* Beginning Balance with validation icon */}
                  <div className="relative flex-1">
                    <TotalsGrid
                      displayData={[
                        [numberToCurrency(profitSharingUpdate.profitShareUpdateTotals.beginningBalance || 0)],
                        [""],
                        [""]
                      ]}
                      leftColumnHeaders={["Total", "Allocation", "Point"]}
                      topRowHeaders={["Beginning Balance"]}
                      breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}
                    />
                    {(() => {
                      console.log("=== BEGINNING BALANCE ICON CHECK ===");
                      console.log("validationResponse exists?", !!validationResponse);
                      const validation = getFieldValidation("TotalProfitSharingBalance");
                      console.log("validation result:", validation);
                      console.log("Should show icon?", !!validationResponse && !!validation);
                      return (
                        validationResponse &&
                        validation && (
                          <div
                            className="absolute right-2 top-1/2 -mt-0.5 -translate-y-1/2"
                            onClick={() => handleValidationToggle("TotalProfitSharingBalance")}>
                            <InfoOutlinedIcon
                              className={`cursor-pointer ${validation.isValid ? "text-green-500" : "text-orange-500"}`}
                              fontSize="small"
                            />
                            {openValidationField === "TotalProfitSharingBalance" && (
                              <div className="absolute left-0 top-full z-[1000] mt-1 max-h-[300px] w-[350px] overflow-auto rounded border border-gray-300 bg-white shadow-lg">
                                <div className="p-2 px-4 pb-4">
                                  <Typography
                                    variant="subtitle2"
                                    sx={{ p: 1 }}>
                                    Beginning Balance
                                  </Typography>
                                  <table className="w-full border-collapse text-[0.95rem]">
                                    <thead>
                                      <tr>
                                        <th className="border-b border-gray-300 px-2 py-1 text-left font-semibold">
                                          Report
                                        </th>
                                        <th className="border-b border-gray-300 px-2 py-1 text-right font-semibold">
                                          Amount
                                        </th>
                                      </tr>
                                    </thead>
                                    <tbody>
                                      <tr>
                                        <td className="border-b border-gray-100 px-2 py-1 text-left">
                                          PAY444 (Current)
                                        </td>
                                        <td className="border-b border-gray-100 px-2 py-1 text-right">
                                          {(() => {
                                            console.log("=== BEGINNING BALANCE VALUES ===");
                                            console.log("validation object:", validation);
                                            console.log("currentValue:", validation.currentValue);
                                            console.log("expectedValue:", validation.expectedValue);
                                            console.log("reportCode:", validation.reportCode);
                                            console.log("fieldName:", validation.fieldName);
                                            return numberToCurrency(validation.currentValue || 0);
                                          })()}
                                        </td>
                                      </tr>
                                      <tr>
                                        <td className="px-2 py-1 text-left">PAY443 (Expected)</td>
                                        <td className="px-2 py-1 text-right">
                                          {numberToCurrency(validation.expectedValue || 0)}
                                        </td>
                                      </tr>
                                    </tbody>
                                  </table>
                                </div>
                              </div>
                            )}
                          </div>
                        )
                      );
                    })()}
                  </div>

                  {/* Contributions */}
                  <div className="relative flex-1">
                    <TotalsGrid
                      displayData={[
                        [numberToCurrency(profitSharingUpdate.profitShareUpdateTotals.totalContribution || 0)],
                        [numberToCurrency(profitSharingUpdate.profitShareUpdateTotals.allocations || 0)],
                        [numberToCurrency(profitSharingUpdate.profitShareUpdateTotals.contributionPoints || 0)]
                      ]}
                      leftColumnHeaders={["Total", "Allocation", "Point"]}
                      topRowHeaders={["Contributions"]}
                      breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}
                    />
                    {validationResponse && getFieldValidation("TotalContributions") && (
                      <div
                        className="absolute right-2 top-1/2 -mt-0.5 -translate-y-1/2"
                        onClick={() => handleValidationToggle("TotalContributions")}>
                        <InfoOutlinedIcon
                          className={`cursor-pointer ${getFieldValidation("TotalContributions")!.isValid ? "text-green-500" : "text-orange-500"}`}
                          fontSize="small"
                        />
                        {openValidationField === "TotalContributions" && (
                          <div className="absolute left-0 top-full z-[1000] mt-1 max-h-[300px] w-[350px] overflow-auto rounded border border-gray-300 bg-white shadow-lg">
                            <div className="p-2 px-4 pb-4">
                              <Typography
                                variant="subtitle2"
                                sx={{ p: 1 }}>
                                Contributions
                              </Typography>
                              <table className="w-full border-collapse text-[0.95rem]">
                                <thead>
                                  <tr>
                                    <th className="border-b border-gray-300 px-2 py-1 text-left font-semibold">
                                      Report
                                    </th>
                                    <th className="border-b border-gray-300 px-2 py-1 text-right font-semibold">
                                      Amount
                                    </th>
                                  </tr>
                                </thead>
                                <tbody>
                                  <tr>
                                    <td className="border-b border-gray-100 px-2 py-1 text-left">PAY444 (Current)</td>
                                    <td className="border-b border-gray-100 px-2 py-1 text-right">
                                      {numberToCurrency(
                                        getFieldValidation("PAY444.TotalContributions")!.currentValue || 0
                                      )}
                                    </td>
                                  </tr>
                                  <tr>
                                    <td className="px-2 py-1 text-left">PAY443 (Expected)</td>
                                    <td className="px-2 py-1 text-right">
                                      {numberToCurrency(
                                        getFieldValidation("PAY444.TotalContributions")!.expectedValue || 0
                                      )}
                                    </td>
                                  </tr>
                                </tbody>
                              </table>
                            </div>
                          </div>
                        )}
                      </div>
                    )}
                  </div>

                  {/* Earnings */}
                  <div className="relative flex-1">
                    <TotalsGrid
                      displayData={[
                        [numberToCurrency(profitSharingUpdate.profitShareUpdateTotals.earnings || 0)],
                        [""],
                        [numberToCurrency(profitSharingUpdate.profitShareUpdateTotals.earningPoints || 0)]
                      ]}
                      leftColumnHeaders={["Total", "Allocation", "Point"]}
                      topRowHeaders={["Earnings"]}
                      breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}
                    />
                    {validationResponse && getFieldValidation("TotalEarnings") && (
                      <div
                        className="absolute right-2 top-1/2 -mt-0.5 -translate-y-1/2"
                        onClick={() => handleValidationToggle("TotalEarnings")}>
                        <InfoOutlinedIcon
                          className={`cursor-pointer ${getFieldValidation("TotalEarnings")!.isValid ? "text-green-500" : "text-orange-500"}`}
                          fontSize="small"
                        />
                        {openValidationField === "TotalEarnings" && (
                          <div className="absolute left-0 top-full z-[1000] mt-1 max-h-[300px] w-[350px] overflow-auto rounded border border-gray-300 bg-white shadow-lg">
                            <div className="p-2 px-4 pb-4">
                              <Typography
                                variant="subtitle2"
                                sx={{ p: 1 }}>
                                Earnings
                              </Typography>
                              <table className="w-full border-collapse text-[0.95rem]">
                                <thead>
                                  <tr>
                                    <th className="border-b border-gray-300 px-2 py-1 text-left font-semibold">
                                      Report
                                    </th>
                                    <th className="border-b border-gray-300 px-2 py-1 text-right font-semibold">
                                      Amount
                                    </th>
                                  </tr>
                                </thead>
                                <tbody>
                                  <tr>
                                    <td className="border-b border-gray-100 px-2 py-1 text-left">PAY444 (Current)</td>
                                    <td className="border-b border-gray-100 px-2 py-1 text-right">
                                      {numberToCurrency(getFieldValidation("PAY444.TotalEarnings")!.currentValue || 0)}
                                    </td>
                                  </tr>
                                  <tr>
                                    <td className="px-2 py-1 text-left">PAY443 (Expected)</td>
                                    <td className="px-2 py-1 text-right">
                                      {numberToCurrency(getFieldValidation("PAY444.TotalEarnings")!.expectedValue || 0)}
                                    </td>
                                  </tr>
                                </tbody>
                              </table>
                            </div>
                          </div>
                        )}
                      </div>
                    )}
                  </div>

                  {/* Earnings2 */}
                  <div className="flex-1">
                    <TotalsGrid
                      displayData={[
                        [numberToCurrency(profitSharingUpdate.profitShareUpdateTotals.earnings2 || 0)],
                        [""],
                        [""]
                      ]}
                      leftColumnHeaders={["Total", "Allocation", "Point"]}
                      topRowHeaders={["Earnings2"]}
                      breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}
                    />
                  </div>

                  {/* Forfeitures */}
                  <div className="relative flex-1">
                    <TotalsGrid
                      displayData={[
                        [numberToCurrency(profitSharingUpdate.profitShareUpdateTotals.forfeiture || 0)],
                        [numberToCurrency(profitSharingUpdate.profitShareUpdateTotals.maxPointsTotal || 0)],
                        [""]
                      ]}
                      leftColumnHeaders={["Total", "Allocation", "Point"]}
                      topRowHeaders={["Forfeitures"]}
                      breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}
                    />
                    {validationResponse && getFieldValidation("TotalForfeitures") && (
                      <div
                        className="absolute right-2 top-1/2 -mt-0.5 -translate-y-1/2"
                        onClick={() => handleValidationToggle("TotalForfeitures")}>
                        <InfoOutlinedIcon
                          className={`cursor-pointer ${getFieldValidation("TotalForfeitures")!.isValid ? "text-green-500" : "text-orange-500"}`}
                          fontSize="small"
                        />
                        {openValidationField === "TotalForfeitures" && (
                          <div className="absolute left-0 top-full z-[1000] mt-1 max-h-[300px] w-[350px] overflow-auto rounded border border-gray-300 bg-white shadow-lg">
                            <div className="p-2 px-4 pb-4">
                              <Typography
                                variant="subtitle2"
                                sx={{ p: 1 }}>
                                Forfeitures
                              </Typography>
                              <table className="w-full border-collapse text-[0.95rem]">
                                <thead>
                                  <tr>
                                    <th className="border-b border-gray-300 px-2 py-1 text-left font-semibold">
                                      Report
                                    </th>
                                    <th className="border-b border-gray-300 px-2 py-1 text-right font-semibold">
                                      Amount
                                    </th>
                                  </tr>
                                </thead>
                                <tbody>
                                  <tr>
                                    <td className="border-b border-gray-100 px-2 py-1 text-left">PAY444 (Current)</td>
                                    <td className="border-b border-gray-100 px-2 py-1 text-right">
                                      {numberToCurrency(
                                        getFieldValidation("PAY444.TotalForfeitures")!.currentValue || 0
                                      )}
                                    </td>
                                  </tr>
                                  <tr>
                                    <td className="px-2 py-1 text-left">PAY443 (Expected)</td>
                                    <td className="px-2 py-1 text-right">
                                      {numberToCurrency(
                                        getFieldValidation("PAY444.TotalForfeitures")!.expectedValue || 0
                                      )}
                                    </td>
                                  </tr>
                                </tbody>
                              </table>
                            </div>
                          </div>
                        )}
                      </div>
                    )}
                  </div>

                  {/* Distributions */}
                  <div className="relative flex-1">
                    <TotalsGrid
                      displayData={[
                        [numberToCurrency(profitSharingUpdate.profitShareUpdateTotals.distributions || 0)],
                        [""],
                        [""]
                      ]}
                      leftColumnHeaders={["Total", "Allocation", "Point"]}
                      topRowHeaders={["Distributions"]}
                      breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}
                    />
                    {validationResponse && getFieldValidation("DistributionTotals") && (
                      <div
                        className="absolute right-2 top-1/2 -mt-0.5 -translate-y-1/2"
                        onClick={() => handleValidationToggle("DistributionTotals")}>
                        <InfoOutlinedIcon
                          className={`cursor-pointer ${getFieldValidation("DistributionTotals")!.isValid ? "text-green-500" : "text-orange-500"}`}
                          fontSize="small"
                        />
                        {openValidationField === "DistributionTotals" && (
                          <div className="absolute left-0 top-full z-[1000] mt-1 max-h-[300px] w-[350px] overflow-auto rounded border border-gray-300 bg-white shadow-lg">
                            <div className="p-2 px-4 pb-4">
                              <Typography
                                variant="subtitle2"
                                sx={{ p: 1 }}>
                                Distributions
                              </Typography>
                              <table className="w-full border-collapse text-[0.95rem]">
                                <thead>
                                  <tr>
                                    <th className="border-b border-gray-300 px-2 py-1 text-left font-semibold">
                                      Report
                                    </th>
                                    <th className="border-b border-gray-300 px-2 py-1 text-right font-semibold">
                                      Amount
                                    </th>
                                  </tr>
                                </thead>
                                <tbody>
                                  <tr>
                                    <td className="border-b border-gray-100 px-2 py-1 text-left">PAY444 (Current)</td>
                                    <td className="border-b border-gray-100 px-2 py-1 text-right">
                                      {numberToCurrency(getFieldValidation("PAY444.Distributions")!.currentValue || 0)}
                                    </td>
                                  </tr>
                                  <tr>
                                    <td className="px-2 py-1 text-left">PAY443 (Expected)</td>
                                    <td className="px-2 py-1 text-right">
                                      {numberToCurrency(getFieldValidation("PAY444.Distributions")!.expectedValue || 0)}
                                    </td>
                                  </tr>
                                </tbody>
                              </table>
                            </div>
                          </div>
                        )}
                      </div>
                    )}
                  </div>

                  {/* Military/Paid Allocation */}
                  <div className="flex-1">
                    <TotalsGrid
                      displayData={[
                        [numberToCurrency(profitSharingUpdate.profitShareUpdateTotals.military || 0)],
                        [numberToCurrency(profitSharingUpdate.profitShareUpdateTotals.paidAllocations || 0)],
                        [""]
                      ]}
                      leftColumnHeaders={["Total", "Allocation", "Point"]}
                      topRowHeaders={["Military/Paid Allocation"]}
                      breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}
                    />
                  </div>

                  {/* Ending Balance */}
                  <div className="flex-1">
                    <TotalsGrid
                      displayData={[
                        [numberToCurrency(profitSharingUpdate.profitShareUpdateTotals.endingBalance || 0)],
                        [
                          numberToCurrency(
                            (profitSharingUpdate.profitShareUpdateTotals.allocations || 0) +
                              (profitSharingUpdate.profitShareUpdateTotals.paidAllocations || 0)
                          )
                        ],
                        [""]
                      ]}
                      leftColumnHeaders={["Total", "Allocation", "Point"]}
                      topRowHeaders={["Ending Balance"]}
                      breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}
                    />
                  </div>
                </div>

                <TotalsGrid
                  tablePadding="12px"
                  displayData={[
                    [
                      numberToCurrency(profitSharingUpdate.profitShareUpdateTotals.maxOverTotal || 0),
                      numberToCurrency(profitSharingUpdate.profitShareUpdateTotals.maxPointsTotal || 0),
                      numberToCurrency(profitSharingEditQueryParams?.maxAllowedContributions || 0)
                    ]
                  ]}
                  leftColumnHeaders={[]}
                  topRowHeaders={["Total Forfeitures", "Total Points", "For Employees Exceeding Max Contribution"]}
                />
                {totalForfeituresGreaterThanZero && (
                  <div className="-mt-2 px-[24px] text-sm text-red-600">
                    <em>
                      * Total Forfeitures value highlighted in red indicates an issue that must be resolved before
                      saving.
                    </em>
                  </div>
                )}
                <div className="h-5" />
                <div className="px-[24px]">
                  <h2 className="text-dsm-secondary">Summary (PAY447)</h2>
                </div>
                <div className="flex gap-2">
                  <TotalsGrid
                    breakPoints={{ xs: 5, sm: 5, md: 5, lg: 5, xl: 5 }}
                    tablePadding="4px"
                    displayData={[
                      [
                        numberToCurrency(profitSharingEdit.beginningBalanceTotal || 0),
                        numberToCurrency(profitSharingEdit.contributionGrandTotal || 0),
                        numberToCurrency(profitSharingEdit.earningsGrandTotal || 0),
                        numberToCurrency(profitSharingEdit.incomingForfeitureGrandTotal || 0)
                      ]
                    ]}
                    leftColumnHeaders={["Grand Totals"]}
                    topRowHeaders={["", "Beginning Balance", "Contributions", "Earnings", "Forfeit"]}
                  />
                </div>
                <br />
                {profitSharingUpdateAdjustmentSummary?.badgeNumber && (
                  <>
                    <div className="px-[24px]">
                      <h2 className="text-dsm-secondary">Adjustments Entered</h2>
                    </div>

                    <TotalsGrid
                      displayData={[
                        [
                          profitSharingUpdateAdjustmentSummary?.badgeNumber,
                          numberToCurrency(profitSharingUpdateAdjustmentSummary?.contributionAmountUnadjusted || 0),
                          numberToCurrency(profitSharingUpdateAdjustmentSummary?.earningsAmountUnadjusted || 0),
                          numberToCurrency(
                            profitSharingUpdateAdjustmentSummary?.secondaryEarningsAmountUnadjusted || 0
                          ),
                          numberToCurrency(
                            profitSharingUpdateAdjustmentSummary?.incomingForfeitureAmountUnadjusted || 0
                          )
                        ],
                        [
                          "",
                          "", // need the requested contribution adjustment (from the request)
                          "", // need the requested earnings adjustment amount
                          "", // need the requested secondary earnings
                          "" // need the requested incoming forfeiture adjustment
                        ],
                        [
                          "",
                          numberToCurrency(profitSharingUpdateAdjustmentSummary?.contributionAmountAdjusted || 0),
                          numberToCurrency(profitSharingUpdateAdjustmentSummary?.earningsAmountAdjusted || 0),
                          numberToCurrency(profitSharingUpdateAdjustmentSummary?.secondaryEarningsAmountAdjusted || 0),
                          numberToCurrency(profitSharingUpdateAdjustmentSummary?.incomingForfeitureAmountAdjusted || 0)
                        ]
                      ]}
                      leftColumnHeaders={["Initial", "Adjustment", "Totals"]}
                      topRowHeaders={["", "Badge", "Contributions", "Earnings", "Earnings2", "Forfeitures"]}
                      headerCellStyle={{}}
                    />
                  </>
                )}
                <Grid width="100%">
                  <ProfitShareEditUpdateTabs
                    initialSearchLoaded={initialSearchLoaded}
                    setInitialSearchLoaded={setInitialSearchLoaded}
                    pageNumberReset={pageNumberReset}
                    setPageNumberReset={setPageNumberReset}
                  />
                </Grid>
              </Grid>
            )}
          </Grid>
          <SmartModal
            key={"saveModal"}
            maxWidth="sm"
            open={openSaveModal}
            onClose={() => setOpenSaveModal(false)}>
            <ProfitShareEditConfirmation
              key={"saveConfirmation"}
              performLabel="YES, SAVE"
              closeLabel="NO, CANCEL"
              setOpenModal={setOpenSaveModal}
              actionFunction={() => {
                saveAction();
                setOpenSaveModal(false);
              }}
              messageType="confirmation"
              messageHeadline="You are about to apply the following changes:"
              params={profitSharingEditQueryParams}
              lastWarning="Ready to save? It may take a few minutes to process."
            />
          </SmartModal>
          <SmartModal
            key={"revertModal"}
            maxWidth="sm"
            open={openRevertModal}
            onClose={() => setOpenRevertModal(false)}>
            <ProfitShareEditConfirmation
              key={"revertConfirmation"}
              performLabel="YES, REVERT"
              closeLabel="NO, CANCEL"
              setOpenModal={setOpenRevertModal}
              actionFunction={() => {
                revertAction();
                setOpenRevertModal(false);
              }}
              messageType="warning"
              messageHeadline="Reverting to the last update will modify the following:"
              params={profitSharingEditQueryParams || profitMasterStatus}
              lastWarning="Do you still wish to revert?"
            />
          </SmartModal>

          <SmartModal
            key={"emptyModal"}
            open={openEmptyModal}
            maxWidth="sm"
            onClose={() => setOpenEmptyModal(false)}>
            <ProfitShareEditConfirmation
              key={"emptyConfirmation"}
              performLabel="OK"
              closeLabel=""
              setOpenModal={setOpenEmptyModal}
              actionFunction={() => {}}
              messageType="info"
              messageHeadline={
                !minimumFieldsEntered
                  ? "You must enter at least contribution, earnings, and max allowed contributions."
                  : !adjustedBadgeOneValid
                    ? "If you adjust a badge, you must also enter the contribution, earnings, and incoming forfeiture."
                    : !adjustedBadgeTwoValid
                      ? "If you adjust a secondary badge, you must also enter the earnings amount."
                      : ""
              }
              params={profitSharingEditQueryParams}
              lastWarning=""
            />
          </SmartModal>
        </Page>
      )}
    </PrerequisiteGuard>
  );
};

export default ProfitShareEditUpdate;
