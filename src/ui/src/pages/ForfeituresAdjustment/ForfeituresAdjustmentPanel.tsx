import { AddOutlined } from "@mui/icons-material";
import { Button, Typography } from "@mui/material";
import React from "react";
import { formatNumberWithComma } from "smart-ui-library";

interface ForfeituresAdjustmentPanelProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  onAddForfeiture?: () => void;
  onAddUnforfeit?: () => void;
  suggestedForfeitAmount?: number;
}

const ForfeituresAdjustmentPanel: React.FC<ForfeituresAdjustmentPanelProps> = ({
  initialSearchLoaded,
  onAddForfeiture,
  onAddUnforfeit,
  suggestedForfeitAmount
}) => {
  return (
    <>
      {initialSearchLoaded && (
        <>
          <div
            style={{
              padding: "0 24px 24px 24px",
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center"
            }}>
            {(onAddForfeiture || onAddUnforfeit) && (
              <div style={{ display: "flex", alignItems: "center", gap: "16px" }}>
                {onAddForfeiture && (
                  <Button
                    onClick={onAddForfeiture}
                    variant="contained"
                    startIcon={<AddOutlined />}
                    color="primary"
                    disabled={suggestedForfeitAmount !== undefined && suggestedForfeitAmount !== null && suggestedForfeitAmount <= 0}>
                    ADD FORFEITURE
                  </Button>
                )}
                {onAddUnforfeit && (
                  <Button
                    onClick={onAddUnforfeit}
                    variant="contained"
                    startIcon={<AddOutlined />}
                    color="primary"
                    disabled={suggestedForfeitAmount !== undefined && suggestedForfeitAmount !== null && suggestedForfeitAmount >= 0}>
                    ADD UNFORFEIT
                  </Button>
                )}
              </div>
            )}
          </div>
        </>
      )}
    </>
  );
};

export default ForfeituresAdjustmentPanel;
