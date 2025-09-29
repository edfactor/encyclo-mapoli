import { Typography } from "@mui/material";
import { Grid } from "@mui/material";
import StoreManagementGrid from "./StoreManagementGrid";
import AssociatesGrid from "./AssociatesGrid";
import { useState } from "react";

interface StoreContentProps {
  store: number | null;
}

const StoreContent: React.FC<StoreContentProps> = ({ store }) => {
  // Note: this is the correct pattern, but probably does not apply to this component
  // as there are two different grids that need to reset their page number
  const [pageNumberReset, setPageNumberReset] = useState(false);

  return (
    <Grid
      container
      direction="column"
      width="100%">
      <Grid paddingX="24px">
        <Typography
          variant="h2"
          sx={{ color: "#0258A5", marginBottom: "16px" }}>
          {store ? `Store ${store}` : 'No Store Selected'}
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
              />
            </Grid>

            <Grid style={{ marginTop: "32px" }}>
              <AssociatesGrid
                store={store}
                pageNumberReset={pageNumberReset}
                setPageNumberReset={setPageNumberReset}
              />
            </Grid>
          </>
        )}
      </Grid>
    </Grid>
  );
};

export default StoreContent;
