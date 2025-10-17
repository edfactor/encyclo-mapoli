import AddIcon from "@mui/icons-material/Add";
import DownloadIcon from "@mui/icons-material/Download";
import PictureAsPdfIcon from "@mui/icons-material/PictureAsPdf";
import { Button, Divider, Grid, Tooltip } from "@mui/material";
import { useEffect, useState } from "react";
import { useDispatch } from "react-redux";
import { useLocation } from "react-router-dom";
import { DSMAccordion, Page, formatNumberWithComma } from "smart-ui-library";
import { MissiveAlertProvider } from "../../components/MissiveAlerts/MissiveAlertContext";
import MissiveAlerts from "../../components/MissiveAlerts/MissiveAlerts";
import { DISTRIBUTION_INQUIRY_MESSAGES } from "../../components/MissiveAlerts/MissiveMessages";
import StatusDropdownActionNode from "../../components/StatusDropdownActionNode";
import { CAPTIONS } from "../../constants";
import { useMissiveAlerts } from "../../hooks/useMissiveAlerts";
import { useReadOnlyNavigation } from "../../hooks/useReadOnlyNavigation";
import { useLazySearchDistributionsQuery } from "../../reduxstore/api/DistributionApi";
import {
  clearCurrentDistribution,
  clearCurrentMember,
  clearHistoricalDisbursements,
  clearPendingDisbursements
} from "../../reduxstore/slices/distributionSlice";
import { DistributionSearchFormData } from "../../types";
import DistributionInquiryGrid from "./DistributionInquiryGrid";
import DistributionInquirySearchFilter from "./DistributionInquirySearchFilter";
import NewEntryDialog from "./NewEntryDialog";

const DistributionInquiryContent = () => {
  const dispatch = useDispatch();
  const location = useLocation();
  const [searchData, setSearchData] = useState<any>(null);
  const [hasSearched, setHasSearched] = useState(false);
  const [isNewEntryDialogOpen, setIsNewEntryDialogOpen] = useState(false);
  const [triggerSearch, { data, isFetching }] = useLazySearchDistributionsQuery();
  const isReadOnly = useReadOnlyNavigation();
  const { missiveAlerts, addAlert, clearAlerts } = useMissiveAlerts();

  // Clear all distribution slice data when component mounts
  useEffect(() => {
    dispatch(clearCurrentMember());
    dispatch(clearCurrentDistribution());
    dispatch(clearPendingDisbursements());
    dispatch(clearHistoricalDisbursements());
  }, [dispatch]);

  // Display success message if returning from AddDistribution
  useEffect(() => {
    const state = location.state as any;
    if (state?.showSuccessMessage && state?.memberName) {
      const amountText = state.amount ? ` for $${formatNumberWithComma(state.amount)}` : "";
      const successMessage = {
        id: 911,
        severity: "success" as const,
        message: "Distribution Saved Successfully",
        description: `Distribution${amountText} for ${state.memberName} has been saved successfully.`
      };
      addAlert(successMessage);
      // Clear the state after displaying the message
      window.history.replaceState({}, document.title);
    }
  }, [location, addAlert]);

  const handleSearch = async (formData: DistributionSearchFormData) => {
    try {
      clearAlerts();

      const request: any = {
        skip: 0,
        take: 25,
        sortBy: "badgeNumber",
        isSortDescending: false,
        onlyNetworkToastErrors: true
      };

      // Map SSN directly
      if (formData.socialSecurity) {
        request.ssn = formData.socialSecurity.trim();
      }

      // Map badge number directly
      if (formData.badgeNumber) {
        request.badgeNumber = Number(formData.badgeNumber);
      }

      // Map member type: "all" -> null, "employees" -> 1, "beneficiaries" -> 2
      if (formData.memberType) {
        request.memberType =
          formData.memberType === "all"
            ? null
            : formData.memberType === "employees"
              ? 1
              : formData.memberType === "beneficiaries"
                ? 2
                : null;
      }

      if (formData.frequency) {
        request.distributionFrequencyId = formData.frequency;
      }

      // Support both single and multiple payment flags
      if (formData.paymentFlags && formData.paymentFlags.length > 0) {
        request.distributionStatusIds = formData.paymentFlags;
      } else if (formData.paymentFlag) {
        request.distributionStatusId = formData.paymentFlag;
      }

      if (formData.taxCode) {
        request.taxCodeId = formData.taxCode;
      }

      if (formData.minGrossAmount) {
        request.minGrossAmount = parseFloat(formData.minGrossAmount);
      }

      if (formData.maxGrossAmount) {
        request.maxGrossAmount = parseFloat(formData.maxGrossAmount);
      }

      if (formData.minCheckAmount) {
        request.minCheckAmount = parseFloat(formData.minCheckAmount);
      }

      if (formData.maxCheckAmount) {
        request.maxCheckAmount = parseFloat(formData.maxCheckAmount);
      }

      setSearchData(request);
      await triggerSearch(request).unwrap();
      // Only set hasSearched to true if the search was successful
      setHasSearched(true);
    } catch (error: any) {
      // Reset hasSearched to false on error to hide the grid
      setHasSearched(false);

      // Check if it's a 500 error with "Badge number not found" or "SSN not found" title
      if (
        error?.status === 500 &&
        (error?.data?.title === "Badge number not found." || error?.data?.title === "SSN not found.")
      ) {
        addAlert(DISTRIBUTION_INQUIRY_MESSAGES.MEMBER_NOT_FOUND);
      } else {
        // For other errors, you might want to show a generic error message
        console.error("Search failed:", error);
      }
    }
  };

  const handleReset = () => {
    setSearchData(null);
    setHasSearched(false);
    clearAlerts();
  };

  const handlePaginationChange = async (pageNumber: number, pageSize: number, sortParams: any) => {
    if (searchData) {
      const request = {
        ...searchData,
        skip: pageNumber * pageSize,
        take: pageSize,
        sortBy: sortParams.sortBy,
        isSortDescending: sortParams.isSortDescending
      };
      await triggerSearch(request);
    }
  };

  const handleNewEntry = () => {
    setIsNewEntryDialogOpen(true);
  };

  const handleCloseNewEntryDialog = () => {
    setIsNewEntryDialogOpen(false);
  };

  const handleExport = () => {
    console.log("Export clicked");
  };

  const handleReport = () => {
    console.log("Report clicked");
  };

  return (
    <Grid
      container
      rowSpacing="24px">
      <Grid width="100%">
        <Divider />
      </Grid>

      {missiveAlerts.length > 0 && <MissiveAlerts />}

      <Grid
        width="100%"
        sx={{ display: "flex", justifyContent: "flex-end", paddingX: "24px", gap: "12px" }}>
        <Tooltip title={isReadOnly ? "You are in read-only mode" : ""}>
          <span>
            <Button
              variant="outlined"
              onClick={handleNewEntry}
              disabled={isReadOnly}
              startIcon={<AddIcon />}>
              NEW ENTRY
            </Button>
          </span>
        </Tooltip>
        <Button
          variant="outlined"
          disabled={true}
          onClick={handleExport}
          startIcon={<DownloadIcon />}>
          EXPORT
        </Button>
        <Button
          variant="outlined"
          disabled={true}
          onClick={handleReport}
          startIcon={<PictureAsPdfIcon />}>
          REPORT
        </Button>
      </Grid>

      <Grid width="100%">
        <DSMAccordion title="Filter">
          <DistributionInquirySearchFilter
            onSearch={handleSearch}
            onReset={handleReset}
            isLoading={isFetching}
          />
        </DSMAccordion>
      </Grid>

      {hasSearched && (
        <Grid width="100%">
          <DistributionInquiryGrid
            postReturnData={data?.results ?? null}
            totalRecords={data?.total ?? 0}
            isLoading={isFetching}
            onPaginationChange={handlePaginationChange}
          />
        </Grid>
      )}

      {/* New Entry Dialog */}
      <NewEntryDialog
        open={isNewEntryDialogOpen}
        onClose={handleCloseNewEntryDialog}
      />
    </Grid>
  );
};

const DistributionInquiry = () => {
  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  return (
    <Page
      label={CAPTIONS.DISTRIBUTIONS_INQUIRY}
      actionNode={renderActionNode()}>
      <MissiveAlertProvider>
        <DistributionInquiryContent />
      </MissiveAlertProvider>
    </Page>
  );
};

export default DistributionInquiry;
