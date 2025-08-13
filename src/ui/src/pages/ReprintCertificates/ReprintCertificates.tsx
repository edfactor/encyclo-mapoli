import { Divider, Grid, Button } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import React, { useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import ReprintCertificatesFilterSection, { ReprintCertificatesFilterParams } from "./ReprintCertificatesFilterSection";
import ReprintCertificatesGrid from "./ReprintCertificatesGrid";

const ReprintCertificates: React.FC = () => {
  const [filterParams, setFilterParams] = useState<ReprintCertificatesFilterParams>({
    employeeNumber: "",
    name: "",
    socialSecurityNumber: ""
  });
  const [isLoading, setIsLoading] = useState(false);
  const [hasSearched, setHasSearched] = useState(false);

  const handleFilterChange = (params: ReprintCertificatesFilterParams) => {
    setFilterParams(params);
    setHasSearched(true);
  };

  const handleReset = () => {
    setFilterParams({
      employeeNumber: "",
      name: "",
      socialSecurityNumber: ""
    });
    setHasSearched(false);
  };

  const handleLoadingChange = (loading: boolean) => {
    setIsLoading(loading);
  };

  const handleTestPrint = () => {
    // TODO: Implement test print functionality
    console.log("Test Print clicked");
  };

  const handlePrint = () => {
    // TODO: Implement print functionality
    console.log("Print clicked");
  };

  const renderActionNode = () => {
    return (
      <div className="flex items-center gap-2 h-10">
        <StatusDropdownActionNode />
        <Button
          onClick={handleTestPrint}
          variant="outlined"
          className="h-10 whitespace-nowrap min-w-fit">
          TEST PRINT
        </Button>
        <Button
          onClick={handlePrint}
          variant="contained"
          className="h-10 whitespace-nowrap min-w-fit">
          PRINT
        </Button>
      </div>
    );
  };

  return (
    <Page
      label={CAPTIONS.REPRINT_CERTIFICATES}
      actionNode={renderActionNode()}>
      <Grid container rowSpacing="24px">
        <Grid width="100%">
          <Divider />
        </Grid>
        <Grid width="100%">
          <DSMAccordion title="Search">
            <ReprintCertificatesFilterSection
              onFilterChange={handleFilterChange}
              onReset={handleReset}
              isLoading={isLoading}
            />
          </DSMAccordion>
        </Grid>

        {hasSearched && (
          <Grid width="100%">
            <ReprintCertificatesGrid
              filterParams={filterParams}
              onLoadingChange={handleLoadingChange}
            />
          </Grid>
        )}
      </Grid>
    </Page>
  );
};

export default ReprintCertificates;