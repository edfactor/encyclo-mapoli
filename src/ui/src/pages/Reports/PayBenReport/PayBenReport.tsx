import { Divider, Grid } from "@mui/material";
import { useRef } from "react";
import { Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import PayBenReportGrid from "./PayBenReportGrid";
import usePayBenReport from "./hooks/usePayBenReport";

const PayBenReport = () => {
  const componentRef = useRef<HTMLDivElement>(null);
  const { searchResults, isSearching, pagination, showData, hasResults } = usePayBenReport();

  const recordCount = searchResults?.total || 0;

  return (
    <Page label={`${CAPTIONS.PAYBEN_REPORT} (${recordCount} records)`}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>

        <Grid width="100%">
          <PayBenReportGrid
            innerRef={componentRef}
            data={searchResults}
            isLoading={isSearching}
            showData={showData}
            hasResults={hasResults ?? false}
            pagination={pagination}
            onPaginationChange={pagination.handlePaginationChange}
            onSortChange={pagination.handleSortChange}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default PayBenReport;
