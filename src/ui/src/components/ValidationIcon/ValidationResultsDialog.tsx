import { Button, Dialog, DialogActions, DialogContent, DialogTitle, Typography } from "@mui/material";
import { numberToCurrency } from "smart-ui-library";
import { CrossReferenceValidationGroup } from "../../types/validation/cross-reference-validation";

/**
 * Props for ValidationResultsDialog component
 */
export interface ValidationResultsDialogProps {
  /** Whether the dialog is open */
  open: boolean;

  /** Handler called when the dialog should close */
  onClose: () => void;

  /** The validation group containing field validations (or null if not available) */
  validationGroup: CrossReferenceValidationGroup | null;

  /** The field name to display validation results for */
  fieldName: string | null;

  /** Optional display name for the field (defaults to fieldName if not provided) */
  fieldDisplayName?: string;
}

/**
 * Dialog component that displays cross-reference validation results for a specific field.
 *
 * Shows a two-column table with Report and Amount columns, displaying:
 * - Current value row (from the current report)
 * - Expected value row (from the archived/reference data)
 * - Variance row (if values don't match)
 *
 * If the validationGroup is null or the specified fieldName cannot be found,
 * displays an appropriate message to the user.
 *
 * @example
 * // Basic usage
 * <ValidationResultsDialog
 *   open={isDialogOpen}
 *   onClose={() => setIsDialogOpen(false)}
 *   validationGroup={distributionsValidationGroup}
 *   fieldName="DistributionTotals"
 *   fieldDisplayName="Distribution Totals"
 * />
 */
export const ValidationResultsDialog: React.FC<ValidationResultsDialogProps> = ({
  open,
  onClose,
  validationGroup,
  fieldName,
  fieldDisplayName
}) => {
  // Find the validation entry matching the field name
  const validation = validationGroup?.validations.find((v) => v.fieldName === fieldName) ?? null;

  // Determine display name
  const displayName = fieldDisplayName ?? fieldName;

  // Check if we have valid data to display
  const hasValidationData = validationGroup !== null && validation !== null;

  return (
    <Dialog
      open={open}
      onClose={onClose}
      maxWidth="sm"
      fullWidth>
      <DialogTitle>
        <div className="flex items-center justify-between">
          <Typography
            variant="h6"
            component="span"
            sx={{ fontWeight: "bold" }}>
            {displayName}
          </Typography>
          {hasValidationData && (
            <Typography
              variant="caption"
              sx={{
                color: validation.isValid ? "success.main" : "warning.main",
                fontWeight: "bold"
              }}>
              {validation.isValid ? "✓ Match" : "⚠ Mismatch"}
            </Typography>
          )}
        </div>
      </DialogTitle>
      <DialogContent>
        {!hasValidationData ? (
          <Typography
            variant="body1"
            color="text.secondary">
            {validationGroup === null
              ? "No validation data is available."
              : `No validation found for field "${fieldName}".`}
          </Typography>
        ) : (
          <table className="w-full border-collapse text-sm">
            <thead>
              <tr>
                <th className="border-b border-gray-300 px-3 py-2 text-left font-semibold">Report</th>
                <th className="border-b border-gray-300 px-3 py-2 text-right font-semibold">Amount</th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <td className="border-b border-gray-100 px-3 py-2 text-left">Current</td>
                <td className="border-b border-gray-100 px-3 py-2 text-right">
                  {numberToCurrency(validation.currentValue ?? 0)}
                </td>
              </tr>
              <tr>
                <td className="border-b border-gray-100 px-3 py-2 text-left">Expected</td>
                <td className="border-b border-gray-100 px-3 py-2 text-right">
                  {numberToCurrency(validation.expectedValue ?? 0)}
                </td>
              </tr>
              {!validation.isValid && (validation.variance ?? 0) !== 0 && (
                <tr className="bg-orange-50">
                  <td className="px-3 py-2 text-left font-semibold text-orange-700">Variance</td>
                  <td className="px-3 py-2 text-right font-bold text-orange-700">
                    {numberToCurrency(validation.variance ?? 0)}
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        )}
      </DialogContent>
      <DialogActions>
        <Button
          onClick={onClose}
          variant="contained"
          color="primary">
          Close
        </Button>
      </DialogActions>
    </Dialog>
  );
};
