import { Typography } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import StoreManagementGrid from "./StoreManagementGrid";
import AssociatesGrid from "./AssociatesGrid";
import { useState } from "react";

interface StoreContentProps {
  store: number;
}

const StoreContent: React.FC<StoreContentProps> = ({ store }) => {

  // Note: this is the correct pattern, but probably does not apply to this component
  // as there are two different grids that need to reset their page number
  const [pageNumberReset, setPageNumberReset] = useState(false);

  return (
    <Grid2 container direction="column" width="100%">
      <Grid2 paddingX="24px">
        <Typography
          variant="h2"
          sx={{ color: "#0258A5", marginBottom: "16px" }}>
          {`Store ${store}`}
        </Typography>
      </Grid2>

      <Grid2 container direction="column" spacing={3} width="100%">
        <Grid2>
          <StoreManagementGrid store={store} pageNumberReset={pageNumberReset} setPageNumberReset={setPageNumberReset} />
        </Grid2>
        
        <Grid2 style={{ marginTop: "32px" }}>
          <AssociatesGrid store={store} pageNumberReset={pageNumberReset} setPageNumberReset={setPageNumberReset} />
        </Grid2>
      </Grid2>
    </Grid2>
  );
};

export default StoreContent; 