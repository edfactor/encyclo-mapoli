import AccessTimeIcon from "@mui/icons-material/AccessTime";
import WarningAmberIcon from "@mui/icons-material/WarningAmber";
import { Box, Tooltip, Typography } from "@mui/material";
import React from "react";
import { useSelector } from "react-redux";
import { useGetFakeTimeStatusQuery } from "../../reduxstore/api/ItOperationsApi";
import { RootState } from "../../reduxstore/store";

/**
 * Banner component that displays when fake time is active.
 * This provides a persistent visual indicator that the application
 * is running with simulated time for testing purposes.
 */
export const FakeTimeBanner: React.FC = () => {
  const hasToken = !!useSelector((state: RootState) => state.security.token);

  // Poll every 120 seconds to detect changes (requires app restart to change)
  // Skip API call if user is not authenticated
  const { data: fakeTimeStatus, isLoading } = useGetFakeTimeStatusQuery(undefined, {
    pollingInterval: 120000,
    skip: !hasToken
  });

  // Don't render anything if not authenticated, not active, or still loading
  if (!hasToken || isLoading || !fakeTimeStatus?.isActive) {
    return null;
  }

  const formatDateTime = (dateString: string | null | undefined): string => {
    if (!dateString) return "N/A";
    try {
      return new Date(dateString).toLocaleString();
    } catch {
      return dateString;
    }
  };

  const tooltipContent = (
    <Box sx={{ p: 1 }}>
      <Typography
        variant="body2"
        fontWeight="bold"
        gutterBottom>
        Fake Time Configuration
      </Typography>
      <Typography variant="body2">
        <strong>Current Fake Time:</strong> {formatDateTime(fakeTimeStatus.currentFakeDateTime)}
      </Typography>
      <Typography variant="body2">
        <strong>Configured Start:</strong> {formatDateTime(fakeTimeStatus.configuredDateTime)}
      </Typography>
      <Typography variant="body2">
        <strong>Time Zone:</strong> {fakeTimeStatus.timeZone ?? "System Default"}
      </Typography>
      <Typography variant="body2">
        <strong>Time Advances:</strong> {fakeTimeStatus.advanceTime ? "Yes" : "No (Frozen)"}
      </Typography>
      <Typography variant="body2">
        <strong>Real Time:</strong> {formatDateTime(fakeTimeStatus.realDateTime)}
      </Typography>
      <Typography
        variant="caption"
        color="warning.main"
        sx={{ mt: 1, display: "block" }}>
        ⚠️ App restart required to change fake time
      </Typography>
    </Box>
  );

  return (
    <Tooltip
      title={tooltipContent}
      arrow
      placement="bottom">
      <Box
        data-testid="fake-time-banner"
        sx={{
          backgroundColor: "#ff9800", // Warning orange
          color: "#000",
          py: 0.5,
          px: 2,
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          gap: 1,
          cursor: "help",
          "&:hover": {
            backgroundColor: "#f57c00"
          }
        }}>
        <WarningAmberIcon fontSize="small" />
        <AccessTimeIcon fontSize="small" />
        <Typography
          variant="body2"
          fontWeight="bold"
          sx={{ letterSpacing: "0.5px" }}>
          FAKE TIME ACTIVE: {formatDateTime(fakeTimeStatus.currentFakeDateTime)}
        </Typography>
        <AccessTimeIcon fontSize="small" />
        <WarningAmberIcon fontSize="small" />
      </Box>
    </Tooltip>
  );
};

export default FakeTimeBanner;
