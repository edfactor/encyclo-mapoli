import { Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import React, { useState } from "react";
import { QPAY066xAdHocReportPreset } from "reduxstore/types";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import reports from "./availableQPAY066xReports";
import QPAY066xAdHocReportsGrid from "./QPAY066xAdHocReportsGrid";
import QPAY066xAdHocSearchFilter from "./QPAY066xAdHocSearchFilter";

const QPAY066xAdHocReports: React.FC = () => {
  const [currentPreset, setCurrentPreset] = useState<QPAY066xAdHocReportPreset | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [storeNumber, setStoreNumber] = useState<string>("");
  const [badgeNumber, setBadgeNumber] = useState<string>("");
  const [employeeName, setEmployeeName] = useState<string>("");
  const [storeManagement, setStoreManagement] = useState<boolean>(false);
  const [startDate, setStartDate] = useState<string>("");
  const [endDate, setEndDate] = useState<string>("");

  const handlePresetChange = (preset: QPAY066xAdHocReportPreset | null) => {
    setCurrentPreset(preset);
  };

  const handleReset = () => {
    setCurrentPreset(null);
    setBadgeNumber("");
    setEmployeeName("");
    setStoreManagement(false);
    setStartDate("");
    setEndDate("");
  };

  const handleStoreNumberChange = (storeNumber: string) => {
    setStoreNumber(storeNumber);
  };

  const handleBadgeNumberChange = (badgeNumber: string) => {
    setBadgeNumber(badgeNumber);
  };

  const handleEmployeeNameChange = (employeeName: string) => {
    setEmployeeName(employeeName);
  };

  const handleStoreManagementChange = (storeManagement: boolean) => {
    setStoreManagement(storeManagement);
  };

  const handleStartDateChange = (startDate: string) => {
    setStartDate(startDate);
  };

  const handleEndDateChange = (endDate: string) => {
    setEndDate(endDate);
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
            <QPAY066xAdHocSearchFilter
              presets={reports}
              currentPreset={currentPreset}
              onPresetChange={handlePresetChange}
              onReset={handleReset}
              onStoreNumberChange={handleStoreNumberChange}
              onBadgeNumberChange={handleBadgeNumberChange}
              onEmployeeNameChange={handleEmployeeNameChange}
              onStoreManagementChange={handleStoreManagementChange}
              onStartDateChange={handleStartDateChange}
              onEndDateChange={handleEndDateChange}
              isLoading={isLoading}
            />
          </DSMAccordion>
        </Grid>

        <Grid width="100%">
          {currentPreset && storeNumber.trim() && (
            <QPAY066xAdHocReportsGrid
              params={currentPreset.params}
              storeNumber={parseInt(storeNumber)}
              badgeNumber={badgeNumber ? parseInt(badgeNumber) : undefined}
              employeeName={employeeName || undefined}
              storeManagement={storeManagement}
              startDate={startDate || undefined}
              endDate={endDate || undefined}
              onLoadingChange={handleLoadingChange}
            />
          )}
        </Grid>
      </Grid>
    </Page>
  );
};

export default QPAY066xAdHocReports;
