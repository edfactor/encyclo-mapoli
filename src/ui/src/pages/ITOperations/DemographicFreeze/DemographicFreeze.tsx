import { Divider, Grid } from "@mui/material";
import { useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { ImpersonationRoles } from "reduxstore/types";
import { Page } from "smart-ui-library";
import DemographicFreezeGrid from "./DemographicFreezeGrid";
import DemographicFreezeManager from "./DemographicFreezeManager";

const DemographicFreeze = () => {
  const { impersonating } = useSelector((state: RootState) => state.security);
  const localStorageImpersonating = localStorage.getItem("impersonatingRole");

  const hasITOperationsRole =
    impersonating === ImpersonationRoles.ItDevOps || localStorageImpersonating === ImpersonationRoles.ItDevOps;

  if (!hasITOperationsRole) {
    return (
      <Page>
        <Grid
          container
          rowSpacing="24px">
          <Grid width={"100%"}>
            <Divider />
          </Grid>
          <Grid width={"100%"}>
            <div style={{ padding: "24px", textAlign: "center" }}>
              <h3>Access Denied</h3>
              <p>You do not have permission to access this page. This page requires IT-Operations role.</p>
            </div>
          </Grid>
        </Grid>
      </Page>
    );
  }

  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [pageNumberReset, setPageNumberReset] = useState(false);

  return (
    <Page label="IT Commands - Demographic Freeze">
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <DemographicFreezeManager
            setInitialSearchLoaded={setInitialSearchLoaded}
            setPageReset={setPageNumberReset}
          />
        </Grid>

        <Grid width="100%">
          <DemographicFreezeGrid
            setInitialSearchLoaded={setInitialSearchLoaded}
            initialSearchLoaded={initialSearchLoaded}
            pageNumberReset={pageNumberReset}
            setPageNumberReset={setPageNumberReset}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default DemographicFreeze;
