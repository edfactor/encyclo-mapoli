import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";
import { Box, Button, Typography } from "@mui/material";
import React from "react";
import { ErrorBoundary, FallbackProps } from "react-error-boundary";

interface AppErrorBoundaryProps {
  children: React.ReactNode;
}

const ErrorFallback = ({ error }: FallbackProps) => {
  const handleReturn = () => {
    window.location.href = "/";
  };

  return (
    <Box
      sx={{
        width: "100%",
        height: "100%",
        minHeight: "400px",
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center"
      }}>
      <ErrorOutlineIcon
        sx={{
          width: 80,
          height: 80,
          color: "#0000008A"
        }}
      />

      <Typography
        variant="h1"
        sx={{
          fontWeight: 500,
          textAlign: "center"
        }}>
        AN UNEXPECTED ERROR HAS OCCURRED.
      </Typography>

      <Typography
        sx={{
          marginBottom: 4
        }}>
        Error: "{error.message}"
      </Typography>

      <Button
        variant="contained"
        onClick={handleReturn}>
        Return to Homepage
      </Button>
    </Box>
  );
};

const AppErrorBoundary: React.FC<AppErrorBoundaryProps> = ({ children }) => {
  return (
    <ErrorBoundary
      FallbackComponent={ErrorFallback}
      onReset={() => {}}>
      {children}
    </ErrorBoundary>
  );
};

export default AppErrorBoundary;
