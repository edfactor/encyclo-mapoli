import { Typography } from "@mui/material";
import { Grid } from "@mui/material";
import StoreManagementGrid from "./StoreManagementGrid";
import AssociatesGrid from "./AssociatesGrid";
import { useState, useCallback } from "react";

interface StoreContentProps {
  store: number | null;
  onLoadingChange?: (isLoading: boolean) => void;
}

const StoreContent: React.FC<StoreContentProps> = ({ store, onLoadingChange }) => {
  // Note: this is the correct pattern, but probably does not apply to this component
  // as there are two different grids that need to reset their page number
  const [pageNumberReset, setPageNumberReset] = useState(false);

  // Track loading state from both grids
  const [managementLoading, setManagementLoading] = useState(false);
  const [associatesLoading, setAssociatesLoading] = useState(false);

  // Notify parent when any grid is loading
  const handleManagementLoadingChange = useCallback(
    (isLoading: boolean) => {
      setManagementLoading(isLoading);
      onLoadingChange?.(isLoading || associatesLoading);
    },
    [associatesLoading, onLoadingChange]
  );

  const handleAssociatesLoadingChange = useCallback(
    (isLoading: boolean) => {
      setAssociatesLoading(isLoading);
      onLoadingChange?.(isLoading || managementLoading);
    },
    [managementLoading, onLoadingChange]
  );

  return (
    <Grid
      container
      direction="column"
      width="100%">
      <Grid paddingX="24px">
        <Typography
          variant="h2"
          sx={{ color: "#0258A5", marginBottom: "16px" }}>
          {store ? `Store ${store}` : "No Store Selected"}
        </Typography>
      </Grid>

      <Grid
        container
        direction="column"
        spacing={3}
        width="100%">
        {store && (
          <>
            <Grid>
              <StoreManagementGrid
                store={store}
                pageNumberReset={pageNumberReset}
                setPageNumberReset={setPageNumberReset}
                onLoadingChange={handleManagementLoadingChange}
              />
            </Grid>

            <Grid style={{ marginTop: "32px" }}>
              <AssociatesGrid
                store={store}
                pageNumberReset={pageNumberReset}
                setPageNumberReset={setPageNumberReset}
                onLoadingChange={handleAssociatesLoadingChange}
              />
            </Grid>
          </>
        )}
      </Grid>
    </Grid>
  );
};

export default StoreContent;
