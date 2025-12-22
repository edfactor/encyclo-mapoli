import { AddOutlined } from "@mui/icons-material";
import { Button, Tooltip } from "@mui/material";
import React from "react";

interface ForfeituresAdjustmentPanelProps {
  onAddForfeiture: () => void;
  isReadOnly: boolean;
  currentBalance: number;
}

const ForfeituresAdjustmentPanel: React.FC<ForfeituresAdjustmentPanelProps> = ({
  onAddForfeiture,
  isReadOnly,
  currentBalance
}) => {
  const isBalanceZero = currentBalance === 0;
  const isDisabled = isReadOnly || isBalanceZero;
  return (
    <div
      style={{
        padding: "0 24px 24px 24px",
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center"
      }}>
      {isDisabled ? (
        <Tooltip
          title={
            isBalanceZero
              ? "Cannot add forfeiture when balance is zero."
              : "You are in read-only mode and cannot add forfeitures."
          }>
          <span>
            <Button
              disabled
              variant="contained"
              startIcon={<AddOutlined />}
              color="primary">
              ADD FORFEITURE
            </Button>
          </span>
        </Tooltip>
      ) : (
        <Button
          onClick={onAddForfeiture}
          variant="contained"
          startIcon={<AddOutlined />}
          color="primary">
          ADD FORFEITURE
        </Button>
      )}
    </div>
  );
};

export default ForfeituresAdjustmentPanel;
