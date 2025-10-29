import { Divider, Grid } from "@mui/material";
import React, { useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { useGetDivorceReportQuery } from "../../../reduxstore/api/DivorceReportApi";
import DivorceReportFilterSection, { DivorceReportFilterParams } from "./DivorceReportFilterSection";
import DivorceReportTable from "./DivorceReportTable";

const DivorceReport: React.FC = () => {
  const [filterParams, setFilterParams] = useState<DivorceReportFilterParams | null>(null);

  // Construct the query parameters
  const queryParams = filterParams
    ? {
        badgeNumber: parseInt(filterParams.badgeNumber, 10),
        startDate: filterParams.startDate ? filterParams.startDate.toISOString().split("T")[0] : undefined,
        endDate: filterParams.endDate ? filterParams.endDate.toISOString().split("T")[0] : undefined
      }
    : null;

  // Execute the query only when we have valid filter params
  const {
    data,
    isLoading: isQueryLoading,
    error
  } = useGetDivorceReportQuery(queryParams || { badgeNumber: 0, startDate: undefined, endDate: undefined }, {
    skip: !queryParams
  });

  const handleFilterChange = (params: DivorceReportFilterParams) => {
    setFilterParams(params);
  };

  const handleReset = () => {
    setFilterParams(null);
  };

  return (
    <Page label={CAPTIONS.DIVORCE_REPORT}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <DSMAccordion title="Filter">
            <DivorceReportFilterSection
              onFilterChange={handleFilterChange}
              onReset={handleReset}
              isLoading={isQueryLoading}
            />
          </DSMAccordion>
        </Grid>

        {filterParams && (
          <Grid width="100%">
            <DivorceReportTable
              data={data}
              isLoading={isQueryLoading}
              error={error}
              showData={!!filterParams}
            />
          </Grid>
        )}
      </Grid>
    </Page>
  );
};

export default DivorceReport;
