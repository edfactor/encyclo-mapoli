import { Button, CircularProgress, Tooltip } from "@mui/material";
import { useSelector } from "react-redux";
import { RootState } from "../../../reduxstore/store";
import { ProfitMasterStatus } from "../../../reduxstore/types";
import { wasFormUsed } from "./utils/formValidation";

interface ProfitShareSaveButtonProps {
  setOpenSaveModal: (open: boolean) => void;
  setOpenEmptyModal: (open: boolean) => void;
  status: ProfitMasterStatus | null;
  isLoading: boolean;
  minimumFieldsEntered?: boolean;
  adjustedBadgeOneValid?: boolean;
  adjustedBadgeTwoValid?: boolean;
  prerequisitesComplete?: boolean;
  isReadOnly?: boolean;
}

/**
 * Save button for Profit Share Edit/Update page
 * Handles validation, tooltips, and modal opening logic
 */
export const ProfitShareSaveButton: React.FC<ProfitShareSaveButtonProps> = ({
  setOpenSaveModal,
  setOpenEmptyModal,
  status,
  isLoading,
  minimumFieldsEntered = false,
  adjustedBadgeOneValid = true,
  adjustedBadgeTwoValid = true,
  prerequisitesComplete = true,
  isReadOnly = false
}) => {
  const {
    profitEditUpdateChangesAvailable,
    profitSharingEditQueryParams,
    profitShareApplyOrRevertLoading,
    totalForfeituresGreaterThanZero,
    invalidProfitShareEditYear
  } = useSelector((state: RootState) => state.yearsEnd);

  const prereqTooltip = !prerequisitesComplete
    ? "All prerequisite navigations must be complete before saving."
    : undefined;

  const handleClick = () => {
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
  };

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
      onClick={isReadOnly ? undefined : handleClick}>
      {isLoading || profitShareApplyOrRevertLoading ? (
        <div className="spinner">
          <CircularProgress color="inherit" size="20px" />
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
              : totalForfeituresGreaterThanZero === true
                ? "Total forfeitures is greater than zero."
                : !prerequisitesComplete
                  ? prereqTooltip
                  : "You must have previewed data before saving."
        }>
        <span>{saveButton}</span>
      </Tooltip>
    );
  }

  return saveButton;
};

export default ProfitShareSaveButton;
