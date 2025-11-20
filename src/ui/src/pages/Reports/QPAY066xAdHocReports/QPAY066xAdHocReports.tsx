import { Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import React, { useState } from "react";
import { useDispatch } from "react-redux";
import { QPAY066xAdHocReportPreset } from "reduxstore/types";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { useGridPagination } from "../../../hooks/useGridPagination";
import {
  clearBreakdownByStore,
  clearBreakdownByStoreManagement,
  clearBreakdownByStoreTotals
} from "../../../reduxstore/slices/yearsEndSlice";
import { useQPAY066xAdHocReports } from "./hooks/useQPAY066xAdHocReports";
import QPAY066xAdHocReportsGrid from "./QPAY066xAdHocReportsGrid";
import QPAY066xAdHocSearchFilter from "./QPAY066xAdHocSearchFilter";
import reports from "./availableQPAY066xReports";

const QPAY066xAdHocReports: React.FC = () => {
  const dispatch = useDispatch();
  const [currentPreset, setCurrentPreset] = useState<QPAY066xAdHocReportPreset | null>(null);
  const [storeNumber, setStoreNumber] = useState<string>("");
  const [badgeNumber, setBadgeNumber] = useState<string>("");
  const [employeeName, setEmployeeName] = useState<string>("");
  const [storeManagement, setStoreManagement] = useState<boolean>(false);
  const [startDate, setStartDate] = useState<string>("");
  const [endDate, setEndDate] = useState<string>("");
  const [hasSearched, setHasSearched] = useState(false);

  const gridPagination = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "badgeNumber",
    initialSortDescending: false,
    onPaginationChange: (pageNumber, pageSize, sortParams) => {
      if (currentPreset) {
        executeSearch({
          reportId: currentPreset.id,
          storeNumber: storeNumber ? parseInt(storeNumber) : undefined,
          badgeNumber: badgeNumber ? parseInt(badgeNumber) : undefined,
          employeeName: employeeName || undefined,
          storeManagement,
          startDate: startDate || undefined,
          endDate: endDate || undefined,
          pagination: {
            skip: pageNumber * pageSize,
            take: pageSize,
            sortBy: sortParams.sortBy,
            isSortDescending: sortParams.isSortDescending
          }
        });
      }
    }
  });

  const { executeSearch, isLoading, currentReportId, getReportTitle } = useQPAY066xAdHocReports();

  const handlePresetChange = (preset: QPAY066xAdHocReportPreset | null) => {
    setCurrentPreset(preset);
    setHasSearched(false);
  };

  const handleReset = () => {
    setCurrentPreset(null);
    setBadgeNumber("");
    setEmployeeName("");
    setStoreManagement(false);
    setStartDate("");
    setEndDate("");
    setHasSearched(false);
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

  const handleSearch = () => {
    if (!currentPreset) {
      return;
    }

    // Clear previous grid data before new search
    dispatch(clearBreakdownByStore());
    dispatch(clearBreakdownByStoreManagement());
    dispatch(clearBreakdownByStoreTotals());

    // Reset pagination to first page on new search
    gridPagination.resetPagination();

    executeSearch({
      reportId: currentPreset.id,
      storeNumber: storeNumber ? parseInt(storeNumber) : undefined,
      badgeNumber: badgeNumber ? parseInt(badgeNumber) : undefined,
      employeeName: employeeName || undefined,
      storeManagement,
      startDate: startDate || undefined,
      endDate: endDate || undefined,
      pagination: {
        skip: 0,
        take: gridPagination.pageSize,
        sortBy: gridPagination.sortParams.sortBy,
        isSortDescending: gridPagination.sortParams.isSortDescending
      }
    });

    setHasSearched(true);
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
              onSearch={handleSearch}
              isLoading={isLoading}
            />
          </DSMAccordion>
        </Grid>

        <Grid width="100%">
          {hasSearched && currentReportId && (
            <QPAY066xAdHocReportsGrid
              reportTitle={getReportTitle(currentReportId)}
              isLoading={isLoading}
              storeNumber={storeNumber}
              gridPagination={gridPagination}
            />
          )}
        </Grid>
      </Grid>
    </Page>
  );
};

export default QPAY066xAdHocReports;
