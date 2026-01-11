import { Alert, Box, Button, Typography } from "@mui/material";
import { useEffect, useRef } from "react";
import { useSelector } from "react-redux";
import { isRouteErrorResponse, useNavigate, useRouteError } from "react-router-dom";
import type { RootState } from "../../reduxstore/store";

/**
 * Error boundary component for React Router v7 loader failures.
 * 
 * Handles two types of errors:
 * 1. 401 Unauthorized - Missing authentication token
 *    - Displays "Authentication in progress..." message
 *    - Automatically retries ONCE when Redux token becomes available
 * 2. Generic loader errors - Network failures, server errors, etc.
 *    - Displays user-friendly error message with manual retry
 * 
 * The automatic retry on auth completion is critical for handling race conditions
 * where the router initializes before Okta authentication completes.
 * 
 * CRITICAL FIX: Uses hasRevalidatedRef to prevent infinite navigate(0) loops.
 * The error state from useRouteError() persists even after token arrives,
 * so we must track if we've already triggered revalidation.
 */
const LoaderErrorBoundary: React.FC = () => {
  const error = useRouteError();
  const navigate = useNavigate();
  const token = useSelector((state: RootState) => state.security.token);

  // CRITICAL: Track if we've already triggered revalidation to prevent loops
  const hasRevalidatedRef = useRef(false);

  const is401 = isRouteErrorResponse(error) && error.status === 401;
  const isNetworkError = isRouteErrorResponse(error) && error.status >= 500;

  // CRITICAL: Auto-retry ONCE when token becomes available (fixes auth race condition)
  useEffect(() => {
    if (token && is401 && !hasRevalidatedRef.current) {
      hasRevalidatedRef.current = true;
      // navigate(0) forces router to revalidate all loaders
      navigate(0);
    }
  }, [token, is401, navigate]);

  // Handle 401 Unauthorized (authentication pending)
  if (is401) {
    return (
      <Box
        sx={{
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          justifyContent: "center",
          minHeight: "100vh",
          padding: 3
        }}>
        <Alert severity="info" sx={{ maxWidth: 600, marginBottom: 2 }}>
          <Typography variant="h6" gutterBottom>
            Authentication in Progress
          </Typography>
          <Typography variant="body2">
            Please wait while we complete your authentication. This page will automatically refresh once authentication
            is complete.
          </Typography>
        </Alert>
        <Typography variant="body2" color="text.secondary">
          If this message persists, please refresh the page manually.
        </Typography>
      </Box>
    );
  }

  // Handle network/server errors
  if (isNetworkError) {
    const errorMessage =
      isRouteErrorResponse(error) && error.statusText ? error.statusText : "Failed to load application data";

    return (
      <Box
        sx={{
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          justifyContent: "center",
          minHeight: "100vh",
          padding: 3
        }}>
        <Alert severity="error" sx={{ maxWidth: 600, marginBottom: 3 }}>
          <Typography variant="h6" gutterBottom>
            Unable to Load Application
          </Typography>
          <Typography variant="body2" gutterBottom>
            {errorMessage}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            This may be due to a network issue or the server being temporarily unavailable.
          </Typography>
        </Alert>
        <Button
          variant="contained"
          color="primary"
          onClick={() => window.location.reload()}
          sx={{ marginTop: 2 }}>
          Retry
        </Button>
      </Box>
    );
  }

  // Handle unexpected errors
  const errorMessage = error instanceof Error ? error.message : "An unexpected error occurred";

  return (
    <Box
      sx={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        minHeight: "100vh",
        padding: 3
      }}>
      <Alert severity="error" sx={{ maxWidth: 600, marginBottom: 3 }}>
        <Typography variant="h6" gutterBottom>
          Application Error
        </Typography>
        <Typography variant="body2" gutterBottom>
          {errorMessage}
        </Typography>
        <Typography variant="body2" color="text.secondary">
          Please try refreshing the page or contact support if the problem persists.
        </Typography>
      </Alert>
      <Button
        variant="contained"
        color="primary"
        onClick={() => window.location.reload()}
        sx={{ marginTop: 2 }}>
        Refresh Page
      </Button>
    </Box>
  );
};

export default LoaderErrorBoundary;
