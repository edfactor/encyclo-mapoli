import { AddOutlined } from "@mui/icons-material";
import { Button, Tooltip } from "@mui/material";
import React from "react";

interface ForfeituresAdjustmentPanelProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  onAddForfeiture?: () => void;
  suggestedForfeitAmount?: number;
  isReadOnly?: boolean;
}

const ForfeituresAdjustmentPanel: React.FC<ForfeituresAdjustmentPanelProps> = ({
  initialSearchLoaded,
  onAddForfeiture,
  isReadOnly = false
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
                {onAddForfeiture &&
                  (isReadOnly ? (
                    <Tooltip title="You are in read-only mode and cannot add forfeitures.">
                      <span>
                        <Button
                          disabled={true}
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
                  ))}
              </div>
            )}
          </div>
        </>
      )}
    </>
  );
};

export default ForfeituresAdjustmentPanel;
