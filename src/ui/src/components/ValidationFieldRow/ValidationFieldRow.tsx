import { CheckCircle, Error as ErrorIcon } from "@mui/icons-material";
import { Box, Chip, Stack, Typography } from "@mui/material";
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
  const { reportCode, fieldName, isValid, currentValue, expectedValue, variance, message, archivedAt } = validation;

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

  return (
    <Box
      sx={{
        display: "flex",
        alignItems: "center",
        justifyContent: "space-between",
        py: 1.5,
        px: 2,
        borderBottom: "1px solid",
        borderColor: "divider",
        "&:last-child": {
          borderBottom: "none"
        },
        backgroundColor: isValid ? "transparent" : "error.light",
        opacity: isValid ? 1 : 0.9
      }}>
      {/* Left side: Status icon + Field info */}
      <Stack
        direction="row"
        spacing={2}
        alignItems="center"
        flex={1}>
        {/* Status Icon */}
        {isValid ? (
          <CheckCircle
            color="success"
            sx={{ fontSize: 24 }}
          />
        ) : (
          <ErrorIcon
            color="error"
            sx={{ fontSize: 24 }}
          />
        )}

        {/* Field Name */}
        <Box>
          <Typography
            variant="body1"
            fontWeight={600}>
            {reportCode}.{fieldName}
          </Typography>
          {message && (
            <Typography
              variant="body2"
              color="text.secondary">
              {message}
            </Typography>
          )}
        </Box>
      </Stack>

      {/* Right side: Values and metadata */}
      <Stack
        direction="row"
        spacing={3}
        alignItems="center">
        {/* Current Value */}
        {currentValue !== null && (
          <Box textAlign="right">
            <Typography
              variant="caption"
              color="text.secondary"
              display="block">
              Current
            </Typography>
            <Typography
              variant="body2"
              fontWeight={600}>
              {numberToCurrency(currentValue)}
            </Typography>
          </Box>
        )}

        {/* Expected Value (only if mismatch) */}
        {!isValid && expectedValue !== null && (
          <Box textAlign="right">
            <Typography
              variant="caption"
              color="text.secondary"
              display="block">
              Expected
            </Typography>
            <Typography
              variant="body2"
              fontWeight={600}>
              {numberToCurrency(expectedValue)}
            </Typography>
          </Box>
        )}

        {/* Variance (only if mismatch) */}
        {!isValid && variance !== null && (
          <Box textAlign="right">
            <Typography
              variant="caption"
              color="text.secondary"
              display="block">
              Variance
            </Typography>
            <Chip
              label={numberToCurrency(variance)}
              color="error"
              size="small"
              sx={{ fontWeight: 600 }}
            />
          </Box>
        )}

        {/* Archived Timestamp */}
        <Box
          textAlign="right"
          minWidth={140}>
          <Typography
            variant="caption"
            color="text.secondary"
            display="block">
            Archived
          </Typography>
          <Typography
            variant="body2"
            fontSize="0.75rem">
            {formattedDate}
          </Typography>
        </Box>
      </Stack>
    </Box>
  );
};

export default ValidationFieldRow;
