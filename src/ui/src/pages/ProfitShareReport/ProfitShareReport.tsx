import { useState } from "react";
import { Button, Divider, Typography, Box } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { DSMAccordion, Page, TotalsGrid } from "smart-ui-library";
import ProfitShareReportSearchFilter from "./ProfitShareReportSearchFilter";
import ProfitShareReportGrid from "./ProfitShareReportGrid";
import StatusDropdown, { ProcessStatus } from "components/StatusDropdown";
import { useNavigate } from "react-router";
import { MENU_LABELS, CAPTIONS } from "../../constants";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";

const ProfitShareReport = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const navigate = useNavigate();
  const { yearEndProfitSharingReport } = useSelector((state: RootState) => state.yearsEnd);

  const handleStatusChange = async (newStatus: ProcessStatus) => {
    console.info("Logging new status: ", newStatus);
  };

  const renderActionNode = () => {
    return (
      <div className="flex items-center gap-2 h-10">
        <StatusDropdown onStatusChange={handleStatusChange} />
        <Button
          onClick={() => navigate("/profit-share-report-edit-run")}
          variant="outlined"
          className="h-10 whitespace-nowrap min-w-fit">
          {MENU_LABELS.GO_TO_PROFIT_SHARE_EDIT_RUN}
        </Button>
      </div>
    );
  };

  const totalsData = {
    sectionTotal: [[
      yearEndProfitSharingReport?.wagesTotal?.toLocaleString('en-US', { style: 'currency', currency: 'USD' }) || "$0.00",
      yearEndProfitSharingReport?.hoursTotal?.toLocaleString() || "0",
      yearEndProfitSharingReport?.pointsTotal?.toLocaleString() || "0"
    ]],
    employeeTotals: [[
      yearEndProfitSharingReport?.numberOfEmployees?.toString() || "0",
      yearEndProfitSharingReport?.numberOfNewEmployees?.toString() || "0",
      yearEndProfitSharingReport?.numberOfEmployeesUnder21?.toString() || "0",
      yearEndProfitSharingReport?.numberOfEmployeesInPlan?.toString() || "0"
    ]],
    secondSectionTotal: [[
      yearEndProfitSharingReport?.terminatedWagesTotal?.toLocaleString('en-US', { style: 'currency', currency: 'USD' }) || "$0.00",
      yearEndProfitSharingReport?.terminatedHoursTotal?.toLocaleString() || "0",
      "0",
      "0"
    ]]
  };

  return (
    <Page
      label={CAPTIONS.PROFIT_SHARE_REPORT}
      actionNode={renderActionNode()}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <ProfitShareReportSearchFilter setInitialSearchLoaded={setInitialSearchLoaded} />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <Box sx={{ mb: 3 }}>
            <div style={{ padding: "0 24px 0 24px" }}>
              <Typography
                variant="h2"
                sx={{ color: "#0258A5" }}>
                {`${CAPTIONS.PROFIT_SHARE_TOTALS}`}
              </Typography>
            </div>

            {yearEndProfitSharingReport && (
              <>
                <TotalsGrid
                  displayData={totalsData.sectionTotal}
                  leftColumnHeaders={["Section Total"]}
                  topRowHeaders={["", "Wages", "Hours", "Points"]}
                  tablePadding="0px"
                />

                <Divider sx={{ my: 2 }} />

                <TotalsGrid
                  displayData={totalsData.employeeTotals}
                  leftColumnHeaders={["Employee Totals"]}
                  topRowHeaders={["", "All Employees", "New Employees", "Employees < 21", "In-Plan"]}
                  tablePadding="0px"
                />

                <TotalsGrid
                  displayData={totalsData.secondSectionTotal}
                  leftColumnHeaders={["Section Total"]}
                  topRowHeaders={["", "", "", "", ""]}
                  tablePadding="0px"
                />
              </>
            )}
          </Box>
        </Grid2>

        <Grid2 width="100%">
          <ProfitShareReportGrid
            initialSearchLoaded={initialSearchLoaded}
            setInitialSearchLoaded={setInitialSearchLoaded}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default ProfitShareReport;
