import { Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField, Typography } from "@mui/material";
import React, { memo } from "react";
import { AdjustmentDraft } from "./hooks/useProfitSharingAdjustments";

interface ProfitSharingAdjustmentModalProps {
  open: boolean;
  draft: AdjustmentDraft;
  onUpdateDraft: (updates: Partial<AdjustmentDraft>) => void;
  onApply: () => void;
  onCancel: () => void;
}

const ProfitSharingAdjustmentModal: React.FC<ProfitSharingAdjustmentModalProps> = memo(
  ({ open, draft, onUpdateDraft, onApply, onCancel }) => {
    return (
      <Dialog
        open={open}
        onClose={onCancel}
        fullWidth
        maxWidth="sm">
        <DialogTitle>Make adjustment</DialogTitle>
        <DialogContent>
          <Typography
            variant="body2"
            color="text.secondary"
            sx={{ mb: 2 }}>
            Creates an administrative adjustment row (EXT=3) using the values below.
          </Typography>

          <Box
            sx={{
              display: "flex",
              gap: 2,
              flexWrap: "wrap"
            }}>
            <TextField
              label="Contribution"
              size="small"
              type="text"
              value={draft.contribution}
              onChange={(e) =>
                onUpdateDraft({
                  contribution: e.target.value
                })
              }
              inputProps={{ inputMode: "decimal" }}
              sx={{ width: 180 }}
            />
            <TextField
              label="Earnings"
              size="small"
              type="text"
              value={draft.earnings}
              onChange={(e) =>
                onUpdateDraft({
                  earnings: e.target.value
                })
              }
              inputProps={{ inputMode: "decimal" }}
              sx={{ width: 180 }}
            />
            <TextField
              label="Forfeiture"
              size="small"
              type="text"
              value={draft.forfeiture}
              onChange={(e) =>
                onUpdateDraft({
                  forfeiture: e.target.value
                })
              }
              inputProps={{ inputMode: "decimal" }}
              sx={{ width: 180 }}
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button
            variant="outlined"
            onClick={onCancel}>
            Cancel
          </Button>
          <Button
            variant="contained"
            onClick={onApply}>
            Apply
          </Button>
        </DialogActions>
      </Dialog>
    );
  }
);

ProfitSharingAdjustmentModal.displayName = "ProfitSharingAdjustmentModal";

export default ProfitSharingAdjustmentModal;
