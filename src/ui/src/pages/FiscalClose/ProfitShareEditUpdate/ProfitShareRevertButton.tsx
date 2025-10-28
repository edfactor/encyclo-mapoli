import { Replay } from "@mui/icons-material";
import { Button, CircularProgress, Tooltip } from "@mui/material";
import { useSelector } from "react-redux";
import { RootState } from "../../../reduxstore/store";

interface ProfitShareRevertButtonProps {
  setOpenRevertModal: (open: boolean) => void;
  isLoading: boolean;
  isReadOnly?: boolean;
}

/**
 * Revert button for Profit Share Edit/Update page
 * Handles reverting profit share changes back to previous state
 */
export const ProfitShareRevertButton: React.FC<ProfitShareRevertButtonProps> = ({
  setOpenRevertModal,
  isLoading,
  isReadOnly = false
}) => {
  const { profitEditUpdateRevertChangesAvailable, profitShareApplyOrRevertLoading } = useSelector(
    (state: RootState) => state.yearsEnd
  );

  const handleClick = () => {
    setOpenRevertModal(true);
  };

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
      onClick={isReadOnly ? undefined : handleClick}>
      {isLoading || profitShareApplyOrRevertLoading ? (
        <div className="spinner">
          <CircularProgress color="inherit" size="20px" />
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
  }

  return revertButton;
};

export default ProfitShareRevertButton;
