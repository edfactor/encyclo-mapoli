import { AddOutlined } from "@mui/icons-material";
import { Button, Typography } from "@mui/material";
import React from "react";
import { formatNumberWithComma } from "smart-ui-library";

interface ForfeituresAdjustmentPanelProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  onAddForfeiture?: () => void;
  suggestedForfeitAmount?: number;
}

const ForfeituresAdjustmentPanel: React.FC<ForfeituresAdjustmentPanelProps> = ({
  initialSearchLoaded,
  onAddForfeiture
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
            {onAddForfeiture && (
              <div style={{ display: "flex", alignItems: "center", gap: "16px" }}>
                {onAddForfeiture && (
                  <Button
                    onClick={onAddForfeiture}
                    variant="contained"
                    startIcon={<AddOutlined />}
                    color="primary">
                    ADD FORFEITURE
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
