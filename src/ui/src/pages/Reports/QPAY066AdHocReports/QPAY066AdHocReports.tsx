import { Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import React, { useState } from "react";
import { ReportPreset } from "reduxstore/types";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import FilterSection from "./FilterSection";
import presets from "./presets";
import ReportGrid from "./ReportGrid";

const QPAY066AdHocReports: React.FC = () => {
  const [currentPreset, setCurrentPreset] = useState<ReportPreset | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [storeNumber, setStoreNumber] = useState<string>("");

  const handlePresetChange = (preset: ReportPreset | null) => {
    setCurrentPreset(preset);
  };

  const handleReset = () => {
    setCurrentPreset(null);
  };

  const handleStoreNumberChange = (storeNumber: string) => {
    setStoreNumber(storeNumber);
  };

  const handleLoadingChange = (loading: boolean) => {
    setIsLoading(loading);
  };

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  return (
    <Page
      label={CAPTIONS.QPAY066_ADHOC}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <DSMAccordion title="Filter">
            <FilterSection
              presets={presets}
              currentPreset={currentPreset}
              onPresetChange={handlePresetChange}
              onReset={handleReset}
              onStoreNumberChange={handleStoreNumberChange}
              isLoading={isLoading}
            />
          </DSMAccordion>
        </Grid>

        <Grid width="100%">
          {currentPreset && storeNumber.trim() && (
            <ReportGrid
              params={currentPreset.params}
              storeNumber={parseInt(storeNumber)}
              onLoadingChange={handleLoadingChange}
            />
          )}
        </Grid>
      </Grid>
    </Page>
  );
};

export default QPAY066AdHocReports;
