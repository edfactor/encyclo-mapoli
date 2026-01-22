import { Typography } from "@mui/material";
import { Grid } from "@mui/material";
import StoreManagementGrid from "./StoreManagementGrid";
import AssociatesGrid from "./AssociatesGrid";
import { useState, useCallback, useEffect } from "react";
import { useGridExpansion } from "../../../../hooks/useGridExpansion";

interface StoreContentProps {
  store: number | null;
  onLoadingChange?: (isLoading: boolean) => void;
  refetchTrigger?: number;
  onGridExpandChange?: (isExpanded: boolean) => void;
}

const StoreContent: React.FC<StoreContentProps> = ({ store, onLoadingChange, refetchTrigger, onGridExpandChange }) => {
  // Note: this is the correct pattern, but probably does not apply to this component
  // as there are two different grids that need to reset their page number
  const [pageNumberReset, setPageNumberReset] = useState(false);
  const [localRefetchTrigger, setLocalRefetchTrigger] = useState(0);
  
  // Use separate grid expansion hooks for each grid
  const { isGridExpanded: managementGridExpanded, handleToggleGridExpand: toggleManagementGrid } = useGridExpansion();
  const { isGridExpanded: associatesGridExpanded, handleToggleGridExpand: toggleAssociatesGrid } = useGridExpansion();

  // When refetchTrigger changes from parent, update local trigger
  useEffect(() => {
    if (refetchTrigger !== undefined) {
      setLocalRefetchTrigger(refetchTrigger);
    }
  }, [refetchTrigger]);

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

  const isAnyGridExpanded = managementGridExpanded || associatesGridExpanded;

  // Notify parent when grid expansion state changes
  useEffect(() => {
    onGridExpandChange?.(isAnyGridExpanded);
  }, [isAnyGridExpanded, onGridExpandChange]);

  return (
    <Grid
      container
      direction="column"
      width="100%">
      {!isAnyGridExpanded && (
        <Grid paddingX="24px">
          <Typography
            variant="h2"
            sx={{ color: "#0258A5", marginBottom: "16px" }}>
            {store !== null && store > 0 ? `Store ${store}` : store === -1 ? "All Stores" : "No Store Selected"}
          </Typography>
        </Grid>
      )}

      <Grid
        container
        direction="column"
        spacing={3}
        width="100%">
        {store !== null && (
          <>
            {(!isAnyGridExpanded || managementGridExpanded) && (
              <Grid>
                <StoreManagementGrid
                  store={store}
                  pageNumberReset={pageNumberReset}
                  setPageNumberReset={setPageNumberReset}
                  onLoadingChange={handleManagementLoadingChange}
                  refetchTrigger={localRefetchTrigger}
                  isGridExpanded={managementGridExpanded}
                  onToggleExpand={toggleManagementGrid}
                />
              </Grid>
            )}

            {(!isAnyGridExpanded || associatesGridExpanded) && (
              <Grid style={{ marginTop: isAnyGridExpanded ? "0" : "32px" }}>
                <AssociatesGrid
                  store={store}
                  pageNumberReset={pageNumberReset}
                  setPageNumberReset={setPageNumberReset}
                  onLoadingChange={handleAssociatesLoadingChange}
                  refetchTrigger={localRefetchTrigger}
                  isGridExpanded={associatesGridExpanded}
                  onToggleExpand={toggleAssociatesGrid}
                />
              </Grid>
            )}
          </>
        )}
      </Grid>
    </Grid>
  );
};

export default StoreContent;
