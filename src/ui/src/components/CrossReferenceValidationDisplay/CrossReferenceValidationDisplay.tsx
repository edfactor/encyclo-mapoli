import { CheckCircle, Error as ErrorIcon, ExpandMore, Warning } from "@mui/icons-material";
import {
  Accordion,
  AccordionDetails,
  AccordionSummary,
  Alert,
  AlertTitle,
  Box,
  Chip,
  Stack,
  Typography
} from "@mui/material";
import { MasterUpdateCrossReferenceValidationResponse } from "../../types/validation/cross-reference-validation";
import ValidationFieldRow from "../ValidationFieldRow/ValidationFieldRow";

/**
 * Props for CrossReferenceValidationDisplay component
 */
interface CrossReferenceValidationDisplayProps {
  /** Complete validation result to display */
  validation: MasterUpdateCrossReferenceValidationResponse;

  /** Optional title override (defaults to "Cross-Reference Validation") */
  title?: string;
}

/**
 * Displays comprehensive cross-reference validation results using Material-UI Accordions.
 * Shows summary header, critical issues alert, warnings, and expandable validation groups.
 *
 * Each validation group (Distributions, Forfeitures, Contributions, Earnings) is displayed
 * in an accordion with its priority level, validation rule, and individual field validations.
 *
 * @example
 * // In Master Update page after API response
 * {response.crossReferenceValidation && (
 *   <CrossReferenceValidationDisplay
 *     validation={response.crossReferenceValidation}
 *   />
 * )}
 */
export const CrossReferenceValidationDisplay = ({
  validation,
  title = "Cross-Reference Validation"
}: CrossReferenceValidationDisplayProps) => {
  const {
    isValid,
    message,
    validationGroups,
    totalValidations,
    passedValidations,
    failedValidations,
    blockMasterUpdate,
    criticalIssues,
    warnings,
    validatedAt
  } = validation;

  // Format timestamp
  const formattedValidationTime = new Date(validatedAt).toLocaleString("en-US", {
    year: "numeric",
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit"
  });

  // Helper to get priority color
  const getPriorityColor = (priority: string): "error" | "warning" | "info" | "default" => {
    switch (priority) {
      case "Critical":
        return "error";
      case "High":
        return "warning";
      case "Medium":
        return "info";
      default:
        return "default";
    }
  };

  // Helper to get group status icon
  const getGroupIcon = (groupIsValid: boolean) => {
    return groupIsValid ? (
      <CheckCircle
        color="success"
        sx={{ mr: 1 }}
      />
    ) : (
      <ErrorIcon
        color="error"
        sx={{ mr: 1 }}
      />
    );
  };

  return (
    <Box sx={{ mt: 3, mb: 3 }}>
      {/* Title and Summary Header */}
      <Box
        sx={{
          mb: 2,
          p: 2,
          backgroundColor: isValid ? "success.light" : "error.light",
          borderRadius: 1,
          border: 1,
          borderColor: isValid ? "success.main" : "error.main"
        }}>
        <Stack
          direction="row"
          alignItems="center"
          justifyContent="space-between">
          <Stack
            direction="row"
            alignItems="center"
            spacing={1}>
            {isValid ? (
              <CheckCircle
                color="success"
                sx={{ fontSize: 32 }}
              />
            ) : (
              <ErrorIcon
                color="error"
                sx={{ fontSize: 32 }}
              />
            )}
            <Box>
              <Typography
                variant="h6"
                fontWeight={600}>
                {title}
              </Typography>
              <Typography
                variant="body2"
                color="text.secondary">
                {message}
              </Typography>
            </Box>
          </Stack>

          <Stack
            direction="row"
            spacing={2}
            alignItems="center">
            <Box textAlign="center">
              <Typography
                variant="h4"
                fontWeight={700}
                color={isValid ? "success.main" : "error.main"}>
                {passedValidations}/{totalValidations}
              </Typography>
              <Typography
                variant="caption"
                color="text.secondary">
                Validations Passed
              </Typography>
            </Box>

            {failedValidations > 0 && (
              <Box textAlign="center">
                <Typography
                  variant="h4"
                  fontWeight={700}
                  color="error.main">
                  {failedValidations}
                </Typography>
                <Typography
                  variant="caption"
                  color="text.secondary">
                  Failed
                </Typography>
              </Box>
            )}
          </Stack>
        </Stack>

        <Typography
          variant="caption"
          color="text.secondary"
          display="block"
          mt={1}>
          Validated at: {formattedValidationTime}
        </Typography>
      </Box>

      {/* Critical Issues Alert (Blocks Master Update) */}
      {blockMasterUpdate && criticalIssues.length > 0 && (
        <Alert
          severity="error"
          sx={{ mb: 2 }}>
          <AlertTitle fontWeight={600}>Critical Issues - Master Update Blocked</AlertTitle>
          <Box
            component="ul"
            sx={{ m: "0.5rem 0 0 1rem", pl: "1rem" }}>
            {criticalIssues.map((issue, index) => (
              <li key={index}>
                <Typography variant="body2">{issue}</Typography>
              </li>
            ))}
          </Box>
        </Alert>
      )}

      {/* Warnings Alert (Doesn't Block) */}
      {warnings.length > 0 && (
        <Alert
          severity="warning"
          sx={{ mb: 2 }}>
          <AlertTitle fontWeight={600}>Warnings</AlertTitle>
          <Box
            component="ul"
            sx={{ m: "0.5rem 0 0 1rem", pl: "1rem" }}>
            {warnings.map((warning, index) => (
              <li key={index}>
                <Typography variant="body2">{warning}</Typography>
              </li>
            ))}
          </Box>
        </Alert>
      )}

      {/* Validation Groups (Accordions) */}
      {validationGroups.map((group, index) => (
        <Accordion
          key={index}
          defaultExpanded={!group.isValid} // Expand failed groups by default
          sx={{
            mb: 1,
            border: 1,
            borderColor: group.isValid ? "divider" : "error.main",
            "&:before": {
              display: "none" // Remove default MUI accordion border
            }
          }}>
          <AccordionSummary
            expandIcon={<ExpandMore />}
            sx={{
              backgroundColor: group.isValid ? "transparent" : "error.light",
              "&.Mui-expanded": {
                minHeight: 48
              }
            }}>
            <Stack
              direction="row"
              alignItems="center"
              spacing={2}
              flex={1}>
              {/* Status Icon */}
              {getGroupIcon(group.isValid)}

              {/* Group Name */}
              <Typography
                variant="h6"
                fontWeight={600}
                flex={1}>
                {group.groupName}
              </Typography>

              {/* Priority Badge */}
              <Chip
                label={group.priority}
                color={getPriorityColor(group.priority)}
                size="small"
                sx={{ fontWeight: 600 }}
              />

              {/* Pass/Fail Count */}
              <Box
                textAlign="center"
                minWidth={80}>
                <Typography
                  variant="body2"
                  fontWeight={600}>
                  {group.validations.filter((v) => v.isValid).length}/{group.validations.length}
                </Typography>
                <Typography
                  variant="caption"
                  color="text.secondary">
                  passed
                </Typography>
              </Box>
            </Stack>
          </AccordionSummary>

          <AccordionDetails sx={{ p: 0 }}>
            {/* Validation Rule */}
            {group.validationRule && (
              <Box
                sx={{
                  px: 2,
                  py: 1.5,
                  backgroundColor: "grey.100",
                  borderBottom: 1,
                  borderColor: "divider"
                }}>
                <Typography
                  variant="caption"
                  color="text.secondary"
                  display="block">
                  Validation Rule:
                </Typography>
                <Typography
                  variant="body2"
                  fontFamily="monospace"
                  fontWeight={600}>
                  {group.validationRule}
                </Typography>
              </Box>
            )}

            {/* Group Summary */}
            {group.summary && (
              <Box
                sx={{
                  px: 2,
                  py: 1.5,
                  backgroundColor: group.isValid ? "success.light" : "error.light",
                  borderBottom: 1,
                  borderColor: "divider"
                }}>
                <Stack
                  direction="row"
                  alignItems="center"
                  spacing={1}>
                  {group.isValid ? (
                    <CheckCircle
                      color="success"
                      sx={{ fontSize: 20 }}
                    />
                  ) : (
                    <Warning
                      color="warning"
                      sx={{ fontSize: 20 }}
                    />
                  )}
                  <Typography
                    variant="body2"
                    fontWeight={600}>
                    {group.summary}
                  </Typography>
                </Stack>
              </Box>
            )}

            {/* Individual Field Validations */}
            <Box>
              {group.validations.map((fieldValidation, fieldIndex) => (
                <ValidationFieldRow
                  key={fieldIndex}
                  validation={fieldValidation}
                />
              ))}
            </Box>

            {/* Group Description (if available) */}
            {group.description && (
              <Box
                sx={{
                  px: 2,
                  py: 1.5,
                  backgroundColor: "grey.50",
                  borderTop: 1,
                  borderColor: "divider"
                }}>
                <Typography
                  variant="caption"
                  color="text.secondary">
                  {group.description}
                </Typography>
              </Box>
            )}
          </AccordionDetails>
        </Accordion>
      ))}
    </Box>
  );
};

export default CrossReferenceValidationDisplay;
