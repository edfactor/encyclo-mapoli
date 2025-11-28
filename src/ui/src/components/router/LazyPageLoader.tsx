import { Box, CircularProgress } from "@mui/material";

/**
 * Loading fallback component displayed while a lazy-loaded page is being fetched
 * Provides visual feedback during code-splitting chunk loading
 */
export const PageLoadingFallback = () => (
  <Box
    sx={{
      display: "flex",
      justifyContent: "center",
      alignItems: "center",
      height: "100vh",
      width: "100%"
    }}>
    <CircularProgress />
  </Box>
);
