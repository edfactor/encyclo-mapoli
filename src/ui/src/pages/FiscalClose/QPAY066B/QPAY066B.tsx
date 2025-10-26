import { Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import React, { useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import QPAY066BFilterSection, { QPAY066BFilterParams } from "./QPAY066BFilterSection";
import QPAY066BGrid from "./QPAY066BGrid";

const QPAY066B: React.FC = () => {
  /*
  const [filterParams, setFilterParams] = useState<QPAY066BFilterParams>({
    qpay066Presets: "QPay066B",
    startDate: null,
    endDate: null,
    vestedPercentage: "< 20%",
    age: "",
    employeeStatus: ""
  });
  */
  const [isLoading, setIsLoading] = useState(false);
  const [hasSearched, setHasSearched] = useState(false);

  const handleFilterChange = (params: QPAY066BFilterParams) => {
    setFilterParams(params);
    setHasSearched(true);
  };

  const handleReset = () => {
    /*
    setFilterParams({
      qpay066Presets: "QPay066B",
      startDate: null,
      endDate: null,
      vestedPercentage: "< 20%",
      age: "",
      employeeStatus: ""
    });
    */
    setHasSearched(false);
  };

  const handleLoadingChange = (loading: boolean) => {
    setIsLoading(loading);
  };

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  return (
    <Page
      label={CAPTIONS.QPAY066B}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <DSMAccordion title="Filter">
            <QPAY066BFilterSection
              onFilterChange={handleFilterChange}
              onReset={handleReset}
              isLoading={isLoading}
            />
          </DSMAccordion>
        </Grid>

        {hasSearched && (
          <Grid width="100%">
            <QPAY066BGrid
              //filterParams={filterParams}
              onLoadingChange={handleLoadingChange}
            />
          </Grid>
        )}
      </Grid>
    </Page>
  );
};

export default QPAY066B;
