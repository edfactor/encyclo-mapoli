import VisibilityIcon from "@mui/icons-material/Visibility";
import { Alert, Box, CircularProgress, IconButton, Tooltip } from "@mui/material";
import { useCallback, useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { ImpersonationRoles } from "reduxstore/types";
import { useUnmaskSsnMutation } from "../../../reduxstore/api/YearsEndApi";
import { RootState } from "../../../reduxstore/store";
import { DuplicateNameAndBirthday } from "../../../types";
import EnvironmentUtils from "../../../utils/environmentUtils";

interface SsnCellRendererProps {
  data: DuplicateNameAndBirthday;
}

/**
 * Custom cell renderer for SSN column that displays masked value with an eye icon.
 * Clicking the eye icon calls the unmask API and displays the unmasked SSN for a configurable duration,
 * then returns to the masked value.
 *
 * Auto-revert timeout:
 * - Production/UAT: 5 minutes (300,000 ms)
 * - Dev/QA: 1 minute (60,000 ms)
 *
 * In Production/UAT: Only shows the eye icon if user has the SsnUnmasking permission.
 * In Dev/QA: Shows the eye icon if user has either the permission or SsnUnmasking impersonation role.
 */
const SsnCellRenderer = ({ data }: SsnCellRendererProps) => {
  const [unmaskedSsn, setUnmaskedSsn] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [unmaskSsn] = useUnmaskSsnMutation();
  const userPermissions = useSelector((state: RootState) => state.security.userPermissions);
  const impersonatingRoles = useSelector((state: RootState) => state.security.impersonating);

  // Compile-time configuration: 5 minutes for Production/UAT, 1 minute for Dev/QA
  const SSN_REVERT_TIMEOUT_MS = EnvironmentUtils.isUAT || EnvironmentUtils.isProduction ? 300000 : 60000;

  // In Dev/QA, check both permissions and impersonation; otherwise just permissions
  const canUnmaskSsn = EnvironmentUtils.isDevelopmentOrQA
    ? userPermissions.includes(ImpersonationRoles.SsnUnmasking) ||
      impersonatingRoles.includes(ImpersonationRoles.SsnUnmasking)
    : userPermissions.includes(ImpersonationRoles.SsnUnmasking);

  // Auto-revert to masked after configured timeout (5 min for Prod/UAT, 1 min for Dev/QA)
  useEffect(() => {
    if (!unmaskedSsn) return;

    const timer = setTimeout(() => {
      setUnmaskedSsn(null);
    }, SSN_REVERT_TIMEOUT_MS);

    return () => clearTimeout(timer);
  }, [unmaskedSsn, SSN_REVERT_TIMEOUT_MS]);

  const handleUnmask = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);

      const result = await unmaskSsn({
        demographicId: data.demographicId
      }).unwrap();

      setUnmaskedSsn(result.unmaskedSsn);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : "Failed to unmask SSN";
      setError(errorMessage);
      console.error("Error unmasking SSN:", err);
    } finally {
      setIsLoading(false);
    }
  }, [data.demographicId, unmaskSsn]);

  return (
    <Box
      display="flex"
      alignItems="center"
      gap={1}>
      <span>{unmaskedSsn || data.ssn}</span>
      {canUnmaskSsn && (
        <Tooltip
          title={
            unmaskedSsn
              ? `SSN unmasked for ${SSN_REVERT_TIMEOUT_MS / 1000 / 60} minute${SSN_REVERT_TIMEOUT_MS / 1000 / 60 > 1 ? "s" : ""}`
              : "Click to unmask SSN"
          }>
          <span>
            <IconButton
              size="small"
              onClick={handleUnmask}
              disabled={isLoading || !!unmaskedSsn}
              sx={{
                padding: "4px",
                "&:hover": {
                  backgroundColor: "rgba(0, 0, 0, 0.04)"
                }
              }}>
              {isLoading ? <CircularProgress size={20} /> : <VisibilityIcon fontSize="small" />}
            </IconButton>
          </span>
        </Tooltip>
      )}
      {error && (
        <Alert
          severity="error"
          sx={{ fontSize: "0.75rem", padding: "4px 8px" }}>
          {error}
        </Alert>
      )}
    </Box>
  );
};

export default SsnCellRenderer;
