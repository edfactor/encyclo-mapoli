import { Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import React, { useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import QPAY600FilterSection, { QPAY600FilterParams } from "./QPAY600FilterSection";
import QPAY600Grid from "./QPAY600Grid";

const QPAY600: React.FC = () => {
  const [filterParams, setFilterParams] = useState<QPAY600FilterParams>({
    startDate: null,
    endDate: null,
    employeeStatus: "Full time",
    employeeType: ""
  });
  const [isLoading, setIsLoading] = useState(false);

  const handleFilterChange = (params: QPAY600FilterParams) => {
    setFilterParams(params);
  };

  const handleReset = () => {
    setFilterParams({
      startDate: null,
      endDate: null,
      employeeStatus: "Full time",
      employeeType: ""
    });
  };

  const handleLoadingChange = (loading: boolean) => {
    setIsLoading(loading);
  };

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  return (
    <Page
      label={CAPTIONS.QPAY600}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <DSMAccordion title="Filter">
            <QPAY600FilterSection
              onFilterChange={handleFilterChange}
              onReset={handleReset}
              isLoading={isLoading}
            />
          </DSMAccordion>
        </Grid>

        {filterParams.employeeStatus === "Full time" && (
          <Grid width="100%">
            <QPAY600Grid
              filterParams={filterParams}
              employeeStatus="Full time"
              onLoadingChange={handleLoadingChange}
            />
          </Grid>
        )}

        {filterParams.employeeStatus === "Part time" && (
          <Grid width="100%">
            <QPAY600Grid
              filterParams={filterParams}
              employeeStatus="Part time"
              onLoadingChange={handleLoadingChange}
            />
          </Grid>
        )}
      </Grid>
    </Page>
  );
};

export default QPAY600;
