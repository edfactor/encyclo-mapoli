import { AddOutlined } from "@mui/icons-material";
import { Button } from "@mui/material";
import React, { useCallback, useEffect, useMemo, useState } from "react";

interface ForfeituresAdjustmentPanelProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  onAddForfeiture?: () => void;
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
              <Button
                onClick={onAddForfeiture}
                variant="contained"
                startIcon={<AddOutlined />}
                color="primary">
                ADD FORFEITURE
              </Button>
            )}
          </div>
        </>
      )}
    </>
  );
};

export default ForfeituresAdjustmentPanel;
