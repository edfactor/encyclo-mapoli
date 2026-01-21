import PageErrorBoundary from "@/components/PageErrorBoundary";
import {
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Divider,
  FormControl,
  Grid,
  InputLabel,
  MenuItem,
  Select
} from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useFakeTimeAwareYear } from "hooks/useFakeTimeAwareDate";
import useNavigationYear from "hooks/useNavigationYear";
import React, { useState } from "react";
import { useGetMissingAnnuityYearsQuery } from "reduxstore/api/administrationApi";
import { useLazyDownloadCertificatesFileQuery } from "reduxstore/api/YearsEndApi";
import { DSMAccordion, Page } from "smart-ui-library";
import { ConfirmationDialog } from "../../../components/ConfirmationDialog";
import { CAPTIONS } from "../../../constants";
import ReprintCertificatesFilterSection, { ReprintCertificatesFilterParams } from "./ReprintCertificatesFilterSection";
import ReprintCertificatesGrid from "./ReprintCertificatesGrid";

const ReprintCertificates: React.FC = () => {
  const selectedProfitYear = useNavigationYear();
  const currentYear = useFakeTimeAwareYear();
  const [filterParams, setFilterParams] = useState<ReprintCertificatesFilterParams>({
    profitYear: selectedProfitYear ?? currentYear,
    badgeNumber: "",
    socialSecurityNumber: ""
  });

  const shouldCheckAnnuityRates = typeof filterParams.profitYear === "number" && filterParams.profitYear > 0;
  const { data: missingYearsData } = useGetMissingAnnuityYearsQuery(
    shouldCheckAnnuityRates
      ? {
          startYear: filterParams.profitYear,
          endYear: filterParams.profitYear,
          suppressAllToastErrors: true
        }
      : undefined,
    { skip: !shouldCheckAnnuityRates }
  );

  const selectedYearStatus = missingYearsData?.years?.find((y) => y.year === filterParams.profitYear);
  const rawMissingAges = selectedYearStatus?.missingAges as unknown;
  const missingAges = Array.isArray(rawMissingAges)
    ? rawMissingAges
    : typeof rawMissingAges === "string"
      ? rawMissingAges
          .split(",")
          .map((x: string) => Number(x.trim()))
          .filter((x: number) => Number.isFinite(x))
      : [];
  const showMissingAgesWarning = selectedYearStatus ? !selectedYearStatus.isComplete : false;

  const [hasSearched, setHasSearched] = useState(false);
  const [selectedBadgeNumbers, setSelectedBadgeNumbers] = useState<number[]>([]);
  const [errorDialog, setErrorDialog] = useState<{ title: string; message: string } | null>(null);

  const [downloadCertificatesFile, { isFetching: isDownloading }] = useLazyDownloadCertificatesFileQuery();

  const [isPrintDialogOpen, setIsPrintDialogOpen] = useState(false);
  const [printContent, setPrintContent] = useState("");
  const [printDialogTitle, setPrintDialogTitle] = useState("Print Preview - Profit Certificates");
  const [isXerox, setIsXerox] = useState(false);

  const printModeLabel = isXerox ? "Xerox" : "Default";
  const printDialogTitleWithMode = `${printDialogTitle} (${printModeLabel})`;

  const handleFilterChange = (params: ReprintCertificatesFilterParams) => {
    setFilterParams(params);
    setHasSearched(true);
  };

  const handleReset = () => {
    setFilterParams({
      profitYear: selectedProfitYear ?? currentYear,
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
        badgeNumbers: selectedBadgeNumbers,
        isXerox: isXerox
      });

      if ("data" in result && result.data) {
        const text = await (result.data as Blob).text();
        setPrintContent(text);
        setPrintDialogTitle("Print Preview - Test Certificates");
        setIsPrintDialogOpen(true);
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
        badgeNumbers: selectedBadgeNumbers,
        isXerox: isXerox
      });

      if ("data" in result && result.data) {
        const text = await (result.data as Blob).text();
        setPrintContent(text);
        setPrintDialogTitle("Print Preview - Profit Certificates");
        setIsPrintDialogOpen(true);
      }
    } catch (error) {
      console.error("Print failed:", error);
      setErrorDialog({
        title: "Print Failed",
        message: "Unable to generate the certificate file. Please verify your selections and try again."
      });
    }
  };

  const printCertificates = (content: string, title: string) => {
    const escapeHtml = (text: string) => {
      const div = document.createElement("div");
      div.textContent = text;
      return div.innerHTML;
    };
    const printWindow = window.open("", "_blank");
    if (printWindow) {
      printWindow.document.write(`
        <html>
          <head>
            <title>${title}</title>
            <style>
              body {
                font-family: monospace;
                font-size: 12px;
                white-space: pre-wrap;
                margin: 20px;
              }
              @media print {
                body { margin: 0; }
                @page {
                  margin: 0;
                  size: auto;
                }
              }
            </style>
          </head>
          <body>
            ${escapeHtml(content)}
          </body>
        </html>
      `);
      printWindow.document.close();
      printWindow.focus();
      printWindow.print();
      setTimeout(() => printWindow.close(), 1000);
    } else {
      setErrorDialog({
        title: "Print Failed",
        message: "Unable to open the print window. Please check your popup blocker settings."
      });
    }
  };

  const renderActionNode = () => {
    return (
      <div className="flex h-10 items-center gap-2">
        <StatusDropdownActionNode />
        <FormControl
          size="small"
          sx={{ minWidth: 140 }}>
          <InputLabel id="reprint-certificates-printer-label">Printer</InputLabel>
          <Select
            labelId="reprint-certificates-printer-label"
            value={isXerox ? "xerox" : "default"}
            label="Printer"
            onChange={(event) => setIsXerox(event.target.value === "xerox")}
            className="h-10">
            <MenuItem value="default">Default</MenuItem>
            <MenuItem value="xerox">Xerox</MenuItem>
          </Select>
        </FormControl>
        <Button
          onClick={handleTestPrint}
          variant="outlined"
          disabled={selectedBadgeNumbers.length === 0 || isDownloading}
          className="h-10 min-w-fit whitespace-nowrap">
          {isDownloading
            ? "Generating..."
            : `TEST PRINT ${selectedBadgeNumbers.length > 0 ? `(${selectedBadgeNumbers.length})` : ""}`}
        </Button>
        <Button
          onClick={handlePrint}
          variant="contained"
          disabled={selectedBadgeNumbers.length === 0 || isDownloading}
          className="h-10 min-w-fit whitespace-nowrap">
          {isDownloading
            ? "Generating..."
            : `PRINT ${selectedBadgeNumbers.length > 0 ? `(${selectedBadgeNumbers.length})` : ""}`}
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
        <Dialog
          open={isPrintDialogOpen}
          onClose={() => setIsPrintDialogOpen(false)}
          maxWidth="md"
          fullWidth>
          <DialogTitle>{printDialogTitleWithMode}</DialogTitle>
          <DialogContent>
            <pre className="whitespace-pre-wrap font-mono text-xs">{printContent}</pre>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setIsPrintDialogOpen(false)}>Close</Button>
            <Button
              variant="contained"
              onClick={() => printCertificates(printContent, printDialogTitleWithMode)}>
              Print
            </Button>
          </DialogActions>
        </Dialog>
      </Page>
    </PageErrorBoundary>
  );
};

export default ReprintCertificates;
