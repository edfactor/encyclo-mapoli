import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";
import RefreshIcon from "@mui/icons-material/Refresh";
import { Box, Button, Typography } from "@mui/material";
import React from "react";
import { ErrorBoundary, FallbackProps } from "react-error-boundary";

interface PageErrorBoundaryProps {
  children: React.ReactNode;
  pageName?: string;
}

const PageErrorFallback = ({ error, resetErrorBoundary }: FallbackProps & { pageName?: string }) => {
  const handleRefresh = () => {
    resetErrorBoundary();
  };

  const handleReturn = () => {
    window.location.href = "/";
  };

  return (
    <Box
      sx={{
        width: "100%",
        height: "100%",
        minHeight: "300px",
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        padding: 4,
        backgroundColor: "#fafafa",
        borderRadius: 2
      }}>
      <ErrorOutlineIcon
        sx={{
          width: 64,
          height: 64,
          color: "#d32f2f",
          marginBottom: 2
        }}
      />

      <Typography
        variant="h5"
        sx={{
          fontWeight: 500,
          textAlign: "center",
          marginBottom: 1,
          color: "#333"
        }}>
        Something went wrong
      </Typography>

      <Typography
        sx={{
          marginBottom: 3,
          color: "#666",
          textAlign: "center",
          maxWidth: "500px"
        }}>
        {error.message || "An unexpected error occurred while loading this page."}
      </Typography>

      <Box sx={{ display: "flex", gap: 2 }}>
        <Button
          variant="outlined"
          startIcon={<RefreshIcon />}
          onClick={handleRefresh}>
          Try Again
        </Button>
        <Button
          variant="contained"
          onClick={handleReturn}>
          Return to Home
        </Button>
      </Box>
    </Box>
  );
};

/**
 * Error boundary wrapper for page components.
 *
 * Wraps page content and catches any unhandled errors, displaying
 * a user-friendly error message with options to retry or return home.
 *
 * Usage:
 * ```tsx
 * <PageErrorBoundary pageName="Distribution Inquiry">
 *   <DistributionInquiryContent />
 * </PageErrorBoundary>
 * ```
 */
const PageErrorBoundary: React.FC<PageErrorBoundaryProps> = ({ children, pageName }) => {
  const handleError = (error: Error, info: React.ErrorInfo) => {
    // Log error for debugging/monitoring
    console.error(`Error in ${pageName || "page"}:`, error, info);
  };

  return (
    <ErrorBoundary
      FallbackComponent={(props) => (
        <PageErrorFallback
          {...props}
          pageName={pageName}
        />
      )}
      onError={handleError}
      onReset={() => {
        // Reset any state that might have caused the error
      }}>
      {children}
    </ErrorBoundary>
  );
};

export default PageErrorBoundary;
