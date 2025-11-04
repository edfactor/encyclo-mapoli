import { Button, CircularProgress, Grid, Stack, Tooltip } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useEffect, useState } from "react";
import { ApiMessageAlert, DSMAccordion, numberToCurrency, Page, SmartModal, TotalsGrid } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { useReadOnlyNavigation } from "../../../hooks/useReadOnlyNavigation";
import usePayMasterUpdate from "./hooks/usePayMasterUpdate";
import PayMasterUpdateGrid from "./PayMasterUpdateGrid";
import PayMasterUpdateSearchFilters from "./PayMasterUpdateSearchFilter";

interface ProfitYearSearch {
  profitYear: number;
}

const PayMasterUpdateSummary = () => {
  const [pageNumberReset, setPageNumberReset] = useState(false);
  const fiscalCloseProfitYear = useFiscalCloseProfitYear();
  const isReadOnly = useReadOnlyNavigation();

  const {
    summaryData,
    searchCompleted,
    executeSearch,
    handleStatusChange,
    handleUpdate,
    isUpdating,
    isModalOpen,
    setIsModalOpen,
    handleCancel,
    gridPagination
  } = usePayMasterUpdate();

  // Initialize search on mount
  useEffect(() => {
    if (fiscalCloseProfitYear) {
      executeSearch(fiscalCloseProfitYear);
    }
  }, [fiscalCloseProfitYear, executeSearch]);

  const renderActionNode = () => {
    if (!summaryData) return null;

    const updateButton = (
      <Button
        onClick={isReadOnly ? undefined : () => setIsModalOpen(true)}
        variant="outlined"
        disabled={isReadOnly}
        className="h-10 min-w-fit whitespace-nowrap">
        Update
      </Button>
    );

    return (
      <Stack
        direction="row"
        spacing={2}>
        {isReadOnly ? (
          <Tooltip title="You are in read-only mode and cannot update enrollment.">
            <span>{updateButton}</span>
          </Tooltip>
        ) : (
          updateButton
        )}
        <StatusDropdownActionNode onStatusChange={handleStatusChange} />
      </Stack>
    );
  };

  const onSearch = (data: ProfitYearSearch) => {
    setPageNumberReset(true);
    executeSearch(data.profitYear);
  };

  const onReset = () => {
    setPageNumberReset(true);
  };

  return (
    <Page
      label={CAPTIONS.PAY450_SUMMARY}
      actionNode={renderActionNode()}>
      <ApiMessageAlert commonKey="UpdateEnrollment" />
      <Grid
        container
        rowSpacing="24px">
        <Grid width="100%">
          <DSMAccordion title="Filter">
            <PayMasterUpdateSearchFilters
              onSearch={onSearch}
              onReset={onReset}
              setPageReset={setPageNumberReset}
            />
          </DSMAccordion>
        </Grid>

        {summaryData?.response && (
          <Grid paddingX="24px">
            <TotalsGrid
              displayData={[
                [
                  summaryData.totalNumberOfEmployees.toString(),
                  summaryData.totalNumberOfBeneficiaries.toString(),
                  numberToCurrency(summaryData.totalBeforeProfitSharingAmount),
                  numberToCurrency(summaryData.totalBeforeVestedAmount),
                  numberToCurrency(summaryData.totalAfterProfitSharingAmount),
                  numberToCurrency(summaryData.totalAfterVestedAmount)
                ]
              ]}
              leftColumnHeaders={[]}
              topRowHeaders={[
                "Employees Updated",
                "Beneficiaries Updated",
                "Before Profit Sharing Amount",
                "Before Vested Amount",
                "After Profit Sharing Amount",
                "After Vested Amount"
              ]}
              breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}
            />
          </Grid>
        )}
        <Grid width="100%">
          {searchCompleted && (
            <PayMasterUpdateGrid
              summaryData={summaryData}
              gridPagination={gridPagination}
              pageNumberReset={pageNumberReset}
              setPageNumberReset={setPageNumberReset}
            />
          )}
        </Grid>
      </Grid>

      <SmartModal
        open={isModalOpen}
        onClose={handleCancel}
        actions={[
          <Button
            onClick={handleUpdate}
            variant="contained"
            color="primary"
            disabled={isUpdating}
            className="mr-2">
            Yes, Update
            {isUpdating && (
              <CircularProgress
                size={"15px"}
                color={"inherit"}
              />
            )}
          </Button>,
          <Button
            onClick={handleCancel}
            variant="outlined"
            disabled={isUpdating}>
            No, Cancel
          </Button>
        ]}
        title="Update Enrollment">
        This update will bring new employees into the Profit Sharing System as enrolled members
      </SmartModal>
    </Page>
  );
};

export default PayMasterUpdateSummary;
