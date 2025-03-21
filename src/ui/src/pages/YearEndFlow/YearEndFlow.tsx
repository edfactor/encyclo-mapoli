import {Divider, MenuItem, Select, SelectChangeEvent } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import DSMCollapsedAccordion from "components/DSMCollapsedAccordion";
import { useEffect } from "react";
import { useSelector, useDispatch } from "react-redux";
import { useNavigate } from "react-router";
import { RootState } from "reduxstore/store";
import { Page } from "smart-ui-library";
import { setSelectedProfitYearForFiscalClose } from "reduxstore/slices/yearsEndSlice";


const FiscalFlow = () => {
  const { selectedProfitYearForFiscalClose } = useSelector((state: RootState) => state.yearsEnd);
  const dispatch = useDispatch();


  const navigate = useNavigate();

  const ProfitYearSelector = () => {
    const handleChange = (event: SelectChangeEvent) => {
      dispatch(setSelectedProfitYearForFiscalClose(Number(event.target.value)));
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
          onChange={handleChange}
        >
          <MenuItem value={2024}>2024</MenuItem>
          <MenuItem value={2025}>2025</MenuItem>
          <MenuItem value={2026}>2026</MenuItem>
        </Select>
      </div>
    );
  };

  return (
    <Page label="Fiscal Flow" actionNode={<ProfitYearSelector />}>
      <Grid2 container>
        <Grid2 size={{ xs: 12 }} width={"100%"}>
          <Divider />
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Payprofit Extract"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/payprofit-extract')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="YTD Wages Extract (Prof Hours Dollars)"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/ytd-wages-extract')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Manage Executive Hours"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/manage-executive-hours-and-dollars')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Profit Share Report - Edit Run (Pay426)"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/profit-share-report-edit-run')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Profit Share Report - Final Run (Pay426)"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/profit-share-report-final-run')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Get Eligible Employees"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/eligible-employees')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Profit Share Forfeit (Pay443)"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/forfeit')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Profit Share Updates (PAY444 PAY447 PAY460 PROFTLD)"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/profit-share-updates')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Profit Share Edit (Pay447)"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/profit-share-edit')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Profit Master Update"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/profit-master-update')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Prof Paymaster UPD"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/paymaster-update')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Profit Share Report By Age (prof130, prof 130b, prof130v, prof130y)"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/profit-share-report-by-age')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Prof Share Gross Report (Qpay501)"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/prof-share-gross-report')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default FiscalFlow;