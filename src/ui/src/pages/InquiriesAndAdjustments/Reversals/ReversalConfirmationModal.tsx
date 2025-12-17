import { Button, CircularProgress, Dialog, DialogActions, DialogContent, DialogTitle, Typography } from "@mui/material";
import React, { memo } from "react";
import { numberToCurrency } from "smart-ui-library";

export interface ReversalItem {
  id: number;
  payment: number;
}

interface ReversalConfirmationModalProps {
  open: boolean;
  selectedItems: ReversalItem[];
  currentIndex: number;
  onConfirm: () => void;
  onCancel: () => void;
  isProcessing: boolean;
}

const ReversalConfirmationModal: React.FC<ReversalConfirmationModalProps> = memo(
  ({ open, selectedItems, currentIndex, onConfirm, onCancel, isProcessing }) => {
    if (!open || selectedItems.length === 0) {
      return null;
    }

    const currentItem = selectedItems[currentIndex];
    const totalCount = selectedItems.length;
    const isMultiple = totalCount > 1;

    const title = isMultiple ? `Reverse Transaction (${currentIndex + 1}/${totalCount})` : "Reverse Transaction";

    const formattedAmount = numberToCurrency(currentItem?.payment ?? 0);

    return (
      <Dialog
        open={open}
        onClose={onCancel}
        maxWidth="sm"
        fullWidth
        disableEscapeKeyDown={isProcessing}>
        <DialogTitle sx={{ fontWeight: "bold" }}>{title}</DialogTitle>
        <DialogContent>
          <Typography>
            This will reverse distribution {currentItem?.id} for {formattedAmount}.
          </Typography>
        </DialogContent>
        <DialogActions sx={{ padding: "16px 24px" }}>
          <Button
            variant="contained"
            color="error"
            onClick={onConfirm}
            disabled={isProcessing}
            sx={{
              minWidth: 140,
              fontWeight: "bold"
            }}>
            {isProcessing ? (
              <CircularProgress
                size={24}
                color="inherit"
              />
            ) : (
              "YES, REVERSE"
            )}
          </Button>
          <Button
            variant="outlined"
            onClick={onCancel}
            disabled={isProcessing}
            sx={{
              minWidth: 120,
              color: "success.main",
              borderColor: "success.main",
              "&:hover": {
                borderColor: "success.dark",
                backgroundColor: "rgba(46, 125, 50, 0.04)"
              }
            }}>
            NO, CANCEL
          </Button>
        </DialogActions>
      </Dialog>
    );
  }
);

ReversalConfirmationModal.displayName = "ReversalConfirmationModal";

export default ReversalConfirmationModal;
