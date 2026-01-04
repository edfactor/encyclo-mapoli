import PageErrorBoundary from "@/components/PageErrorBoundary";
import { Alert, Button, Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import React, { useState } from "react";
import { useGetMissingAnnuityYearsQuery } from "reduxstore/api/administrationApi";
import { useLazyDownloadCertificatesFileQuery } from "reduxstore/api/YearsEndApi";
import { DSMAccordion, Page } from "smart-ui-library";
import { downloadFileFromResponse } from "utils/fileDownload";
import { ConfirmationDialog } from "../../../components/ConfirmationDialog";
import { CAPTIONS } from "../../../constants";
import ReprintCertificatesFilterSection, { ReprintCertificatesFilterParams } from "./ReprintCertificatesFilterSection";
import ReprintCertificatesGrid from "./ReprintCertificatesGrid";

const ReprintCertificates: React.FC = () => {
  const selectedProfitYear = useFiscalCloseProfitYear();
  const [filterParams, setFilterParams] = useState<ReprintCertificatesFilterParams>({
    profitYear: selectedProfitYear,
    badgeNumber: "",
    socialSecurityNumber: ""
  });

  const { data: missingYearsData } = useGetMissingAnnuityYearsQuery({
    startYear: filterParams.profitYear,
    endYear: filterParams.profitYear
  });

  const selectedYearStatus = missingYearsData?.years?.find((y) => y.year === filterParams.profitYear);
  const missingAges = selectedYearStatus?.missingAges ?? [];
  const showMissingAgesWarning = selectedYearStatus ? !selectedYearStatus.isComplete : false;

  const [hasSearched, setHasSearched] = useState(false);
  const [selectedBadgeNumbers, setSelectedBadgeNumbers] = useState<number[]>([]);
  const [errorDialog, setErrorDialog] = useState<{ title: string; message: string } | null>(null);

  const [downloadCertificatesFile] = useLazyDownloadCertificatesFileQuery();

  const handleFilterChange = (params: ReprintCertificatesFilterParams) => {
    setFilterParams(params);
    setHasSearched(true);
  };

  const handleReset = () => {
    setFilterParams({
      profitYear: selectedProfitYear,
      badgeNumber: "",
      socialSecurityNumber: ""
    });
    setHasSearched(false);
  };

  const handleSelectionChange = (badgeNumbers: number[]) => {
    setSelectedBadgeNumbers(badgeNumbers);
  };

  const handleTestPrint = async () => {
    try {
      const result = await downloadCertificatesFile({
        profitYear: filterParams.profitYear,
        badgeNumbers: selectedBadgeNumbers
      });

      if ("data" in result && result.data) {
        await downloadFileFromResponse(Promise.resolve({ data: result.data }), "PAYCERT-TEST.txt");
      }
    } catch (error) {
      console.error("Test print failed:", error);
      setErrorDialog({
        title: "Test Print Failed",
        message: "Unable to generate the test print file. Please verify your selections and try again."
      });
    }
  };

  const handlePrint = async () => {
    try {
      const result = await downloadCertificatesFile({
        profitYear: filterParams.profitYear,
        badgeNumbers: selectedBadgeNumbers
      });

      if ("data" in result && result.data) {
        await downloadFileFromResponse(Promise.resolve({ data: result.data }), "PAYCERT.txt");
      }
    } catch (error) {
      console.error("Print failed:", error);
      setErrorDialog({
        title: "Print Failed",
        message: "Unable to generate the certificate file. Please verify your selections and try again."
      });
    }
  };

  const renderActionNode = () => {
    return (
      <div className="flex h-10 items-center gap-2">
        <StatusDropdownActionNode />
        <Button
          onClick={handleTestPrint}
          variant="outlined"
          disabled={selectedBadgeNumbers.length === 0}
          className="h-10 min-w-fit whitespace-nowrap">
          TEST PRINT {selectedBadgeNumbers.length > 0 && `(${selectedBadgeNumbers.length})`}
        </Button>
        <Button
          onClick={handlePrint}
          variant="contained"
          disabled={selectedBadgeNumbers.length === 0}
          className="h-10 min-w-fit whitespace-nowrap">
          PRINT {selectedBadgeNumbers.length > 0 && `(${selectedBadgeNumbers.length})`}
        </Button>
      </div>
    );
  };

  return (
    <PageErrorBoundary pageName="Reprint Certificates">
      <Page
        label={CAPTIONS.REPRINT_CERTIFICATES}
        actionNode={renderActionNode()}>
        <Grid
          container
          rowSpacing="24px">
          <Grid width="100%">
            <Divider />
          </Grid>

          {showMissingAgesWarning && (
            <Grid width="100%">
              <Alert severity="warning">
                Annuity rates are incomplete for profit year {filterParams.profitYear}
                {missingAges.length > 0 ? ` (missing ages: ${missingAges.join(", ")}).` : "."} Certificate generation
                may fail until annuity rates are updated.
              </Alert>
            </Grid>
          )}
          <Grid width="100%">
            <DSMAccordion title="Search">
              <ReprintCertificatesFilterSection
                onFilterChange={handleFilterChange}
                onReset={handleReset}
              />
            </DSMAccordion>
          </Grid>

          {hasSearched && (
            <Grid width="100%">
              <ReprintCertificatesGrid
                filterParams={filterParams}
                onSelectionChange={handleSelectionChange}
              />
            </Grid>
          )}
        </Grid>

        <ConfirmationDialog
          open={!!errorDialog}
          title={errorDialog?.title || "Error"}
          description={errorDialog?.message || "An error occurred"}
          onClose={() => setErrorDialog(null)}
        />
      </Page>
    </PageErrorBoundary>
  );
};

export default ReprintCertificates;
