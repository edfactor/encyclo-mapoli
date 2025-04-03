import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import DSMCollapsedAccordion from "components/DSMCollapsedAccordion";
import { useNavigate } from "react-router";
import { RootState } from "reduxstore/store";
import { Page } from "smart-ui-library";
import { CAPTIONS, MENU_LABELS } from "../../constants";

const FiscalFlow = () => {
  const navigate = useNavigate();

  return (
    <Page
      label={MENU_LABELS.FISCAL_CLOSE}>
      <Grid2 container>
        <Grid2
          size={{ xs: 12 }}
          width={"100%"}>
          <Divider />
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title={CAPTIONS.MANAGE_EXECUTIVE_HOURS}
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate("/manage-executive-hours-and-dollars")}
            isCollapsedOnRender={true}>
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title={CAPTIONS.YTD_WAGES_EXTRACT}
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate("/ytd-wages-extract")}
            isCollapsedOnRender={true}>
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title={CAPTIONS.PROFIT_SHARE_REPORT}
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate("/profit-share-report")}
            isCollapsedOnRender={true}>
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title={CAPTIONS.PROFIT_SHARE_REPORT_EDIT_RUN}
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate("/profit-share-report-edit-run")}
            isCollapsedOnRender={true}>
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title={CAPTIONS.ELIGIBLE_EMPLOYEES}
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate("/eligible-employees")}
            isCollapsedOnRender={true}>
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title={CAPTIONS.FORFEIT}
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate("/forfeit")}
            isCollapsedOnRender={true}>
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title={CAPTIONS.PROFIT_SHARE_UPDATE}
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate("/profit-share-update")}
            isCollapsedOnRender={true}>
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title={CAPTIONS.PROFIT_SHARE_GROSS_REPORT}
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate("/prof-share-gross-report")}
            isCollapsedOnRender={true}>
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title={CAPTIONS.PROFIT_SHARE_BY_STORE}
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate("/profit-share-by-store")}
            isCollapsedOnRender={true}>
            <></>
          </DSMCollapsedAccordion>
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default FiscalFlow;
