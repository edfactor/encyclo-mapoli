import { Typography } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import StoreManagementGrid from "./StoreManagementGrid";
import AssociatesGrid from "./AssociatesGrid";

interface StoreContentProps {
  store: number;
}

const StoreContent: React.FC<StoreContentProps> = ({ store }) => {
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
          <StoreManagementGrid store={store} />
        </Grid2>
        
        <Grid2 style={{ marginTop: "32px" }}>
          <AssociatesGrid store={store} />
        </Grid2>
      </Grid2>
    </Grid2>
  );
};

export default StoreContent; 