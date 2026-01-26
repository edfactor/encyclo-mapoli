import { Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField, Typography } from "@mui/material";
import { CellValueChangedEvent, ColDef } from "ag-grid-community";
import { DSMGrid } from "smart-ui-library";
import { GRID_KEYS } from "../../../constants";
import { AnnuityRateInputRequest } from "../../../reduxstore/types";
import { VisuallyHidden } from "../../../utils/accessibilityHelpers";
import { generateFieldId, getAriaDescribedBy } from "../../../utils/accessibilityUtils";
import { ARIA_DESCRIPTIONS, INPUT_PLACEHOLDERS } from "../../../utils/inputFormatters";

interface ManageAnnuityRatesCopyModalProps {
  isOpen: boolean;
  isCreating: boolean;
  copySourceYear: number | null;
  copyYear: number | "";
  copyRates: AnnuityRateInputRequest[];
  copyColumnDefs: ColDef[];
  onClose: () => void;
  onCreate: () => void;
  onYearChange: (rawValue: string) => void;
  onCellValueChanged: (event: CellValueChangedEvent) => void;
}

const ManageAnnuityRatesCopyModal = ({
  isOpen,
  isCreating,
  copySourceYear,
  copyYear,
  copyRates,
  copyColumnDefs,
  onClose,
  onCreate,
  onYearChange,
  onCellValueChanged
}: ManageAnnuityRatesCopyModalProps) => {
  return (
    <Dialog
      open={isOpen}
      onClose={onClose}
      maxWidth="md"
      fullWidth>
      <DialogTitle sx={{ fontWeight: "bold" }}>Copy Annuity Rates</DialogTitle>
      <DialogContent>
        <Typography sx={{ mb: 2 }}>
          {copySourceYear ? `Copying rates from ${copySourceYear}.` : "Select a year to copy rates from."}
        </Typography>
        <Box sx={{ display: "flex", gap: 2, alignItems: "center", mb: 2 }}>
          <TextField
            label="New Year"
            id={generateFieldId("copyYear")}
            placeholder={INPUT_PLACEHOLDERS.PROFIT_YEAR}
            value={copyYear}
            onChange={(event) => onYearChange(event.target.value)}
            inputProps={{
              inputMode: "numeric",
              pattern: "[0-9]*",
              "aria-describedby": getAriaDescribedBy(generateFieldId("copyYear"), false, true)
            }}
            size="small"
          />
          <VisuallyHidden id="copyYear-hint">{ARIA_DESCRIPTIONS.PROFIT_YEAR}</VisuallyHidden>
        </Box>
        <DSMGrid
          preferenceKey={`${GRID_KEYS.MANAGE_ANNUITY_RATES}-copy`}
          isLoading={isCreating}
          providedOptions={{
            rowData: copyRates,
            columnDefs: copyColumnDefs,
            suppressMultiSort: true,
            stopEditingWhenCellsLoseFocus: true,
            enterNavigatesVertically: true,
            enterNavigatesVerticallyAfterEdit: true,
            onCellValueChanged
          }}
        />
      </DialogContent>
      <DialogActions sx={{ padding: "16px 24px" }}>
        <Button
          variant="outlined"
          onClick={onClose}
          disabled={isCreating}>
          Cancel
        </Button>
        <Button
          variant="contained"
          onClick={onCreate}
          disabled={isCreating}>
          Create
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default ManageAnnuityRatesCopyModal;
