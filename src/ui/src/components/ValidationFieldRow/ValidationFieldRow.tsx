import { CheckCircle, Warning } from "@mui/icons-material";
import { Box, Stack, Typography } from "@mui/material";
import { numberToCurrency } from "smart-ui-library";
import { CrossReferenceValidation } from "../../types/validation/cross-reference-validation";

/**
 * Props for ValidationFieldRow component
 */
interface ValidationFieldRowProps {
  /** Individual validation result to display */
  validation: CrossReferenceValidation;
}

/**
 * Displays a single field validation with report code, field name, current value,
 * variance (if mismatch), status icon, and archived timestamp.
 *
 * Used within CrossReferenceValidationDisplay accordions to show field-level validation details.
 *
 * @example
 * <ValidationFieldRow
 *   validation={{
 *     reportCode: "PAY443",
 *     fieldName: "DistributionTotals",
 *     isValid: false,
 *     currentValue: 1234567.89,
 *     expectedValue: 1234560.00,
 *     variance: 7.89,
 *     message: "Distribution totals mismatch",
 *     archivedAt: "2025-10-06T10:30:00Z"
 *   }}
 * />
 */
export const ValidationFieldRow = ({ validation }: ValidationFieldRowProps) => {
  const { reportCode, isValid, currentValue, expectedValue, variance, message, archivedAt } = validation;

  // Format date for display
  const formattedDate = archivedAt
    ? new Date(archivedAt).toLocaleString("en-US", {
        year: "numeric",
        month: "short",
        day: "numeric",
        hour: "2-digit",
        minute: "2-digit"
      })
    : "N/A";

  // Determine the display value
  const displayValue = isValid ? currentValue : (expectedValue ?? currentValue);

  return (
    <Box
      sx={{
        display: "flex",
        alignItems: "center",
        justifyContent: "space-between",
        py: 2,
        px: 2,
        borderBottom: "1px solid",
        borderColor: "divider",
        "&:last-child": {
          borderBottom: "none"
        },
        backgroundColor: isValid ? "transparent" : "warning.light",
        borderLeft: isValid ? "none" : "4px solid",
        borderLeftColor: isValid ? "transparent" : "error.main"
      }}>
      {/* Left side: Report label + short message */}
      <Stack
        direction="row"
        spacing={1.5}
        alignItems="center"
        flex={1}>
        {/* Status Icon */}
        {isValid ? (
          <CheckCircle
            color="success"
            sx={{ fontSize: 26 }}
          />
        ) : (
          <Warning
            color="error"
            sx={{ fontSize: 28 }}
          />
        )}

        {/* Field Name */}
        <Box>
          <Typography
            variant="body1"
            fontWeight={600}
            color="text.primary">
            {reportCode}
          </Typography>
          {message && (
            <Typography
              variant="body2"
              color="text.secondary"
              fontSize="0.875rem">
              {message}
            </Typography>
          )}
        </Box>
      </Stack>

      {/* Right side: Prominent numeric value with details */}
      <Stack
        direction="row"
        spacing={4}
        alignItems="center">
        {/* Main Value (Current or Expected if valid) */}
        {displayValue !== null && (
          <Box textAlign="right">
            <Typography
              variant="h5"
              fontWeight={700}
              color={isValid ? "success.main" : "error.main"}>
              {numberToCurrency(displayValue)}
            </Typography>
            <Typography
              variant="caption"
              color="text.secondary"
              display="block">
              {isValid ? "Current" : "Expected"}
            </Typography>
          </Box>
        )}

        {/* Variance Display (only if mismatch) */}
        {!isValid && variance !== null && Math.abs(variance) > 0.01 && (
          <Box textAlign="right">
            <Typography
              variant="h6"
              fontWeight={600}
              color="error.dark">
              {variance > 0 ? "+" : ""}
              {numberToCurrency(variance)}
            </Typography>
            <Typography
              variant="caption"
              color="text.secondary"
              display="block">
              Variance
            </Typography>
          </Box>
        )}

        {/* Archived Timestamp */}
        <Box
          textAlign="right"
          minWidth={140}>
          <Typography
            variant="body2"
            fontSize="0.8rem"
            color="text.secondary">
            {formattedDate}
          </Typography>
          <Typography
            variant="caption"
            color="text.secondary"
            display="block">
            Archived
          </Typography>
        </Box>
      </Stack>
    </Box>
  );
};

export default ValidationFieldRow;
