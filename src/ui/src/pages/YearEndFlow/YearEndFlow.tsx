import { Divider, MenuItem, Select, SelectChangeEvent } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import DSMCollapsedAccordion from "components/DSMCollapsedAccordion";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate } from "react-router";
import {
  checkFiscalCloseParamsAndGridsProfitYears,
  setSelectedProfitYearForFiscalClose
} from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { Page } from "smart-ui-library";
import { CAPTIONS, MENU_LABELS } from "../../constants";

const FiscalFlow = () => {
  const { selectedProfitYearForFiscalClose } = useSelector((state: RootState) => state.yearsEnd);
  const dispatch = useDispatch();

  const navigate = useNavigate();

  const thisYear = new Date().getFullYear();

  const ProfitYearSelector = () => {
    const handleChange = (event: SelectChangeEvent) => {
      dispatch(setSelectedProfitYearForFiscalClose(Number(event.target.value)));
      dispatch(checkFiscalCloseParamsAndGridsProfitYears(Number(event.target.value)));
    };
    
    

    return (
      <div className="flex items-center gap-2 h-10 min-w-[174px]">
        <Select
          labelId="fiscal-flow-profit-year-select"
          id="fiscal-flow-profit-year-select"
          defaultValue="2024"
          value={selectedProfitYearForFiscalClose.toString()}
          size="small"
          fullWidth
          onChange={handleChange}>
          <MenuItem value={thisYear - 1}>{thisYear - 1}</MenuItem>
          <MenuItem value={thisYear}>{thisYear}</MenuItem>
          <MenuItem value={thisYear + 1}>{thisYear + 1}</MenuItem>
        </Select>
      </div>
    );
  };

  return (
    <Page
      label={MENU_LABELS.FISCAL_CLOSE}
      actionNode={<ProfitYearSelector />}>
      <Grid2 container>
        <Grid2
          size={{ xs: 12 }}
          width={"100%"}>
          <Divider />
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
            title={CAPTIONS.PAYMASTER_UPDATE}
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate("/paymaster-update")}
            isCollapsedOnRender={true}>
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Profit Share Report By Age (PROF130, PROF130B, PROF130V, PROF130Y)"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate("/profit-share-report-by-age")}
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
      </Grid2>
    </Page>
  );
};

export default FiscalFlow;
