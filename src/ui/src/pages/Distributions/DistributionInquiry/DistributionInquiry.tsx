import AddIcon from "@mui/icons-material/Add";
import DownloadIcon from "@mui/icons-material/Download";
import PictureAsPdfIcon from "@mui/icons-material/PictureAsPdf";
import { Button, Divider, Grid, Tooltip } from "@mui/material";
import { useEffect, useState } from "react";
import { useDispatch } from "react-redux";
import { useLocation, useNavigate } from "react-router-dom";
import { DSMAccordion, Page, formatNumberWithComma } from "smart-ui-library";
import { MissiveAlertProvider } from "../../../components/MissiveAlerts/MissiveAlertContext";
import MissiveAlerts from "../../../components/MissiveAlerts/MissiveAlerts";
import { DISTRIBUTION_INQUIRY_MESSAGES } from "../../../components/MissiveAlerts/MissiveMessages";
import StatusDropdownActionNode from "../../../components/StatusDropdownActionNode";
import { CAPTIONS, ROUTES } from "../../../constants";
import { SortParams } from "../../../hooks/useGridPagination";
import { useMissiveAlerts } from "../../../hooks/useMissiveAlerts";
import { useReadOnlyNavigation } from "../../../hooks/useReadOnlyNavigation";
import {
  useDeleteDistributionMutation,
  useLazySearchDistributionsQuery
} from "../../../reduxstore/api/DistributionApi";
import {
  clearCurrentDistribution,
  clearCurrentMember,
  clearHistoricalDisbursements,
  clearPendingDisbursements,
  setDistributionHome
} from "../../../reduxstore/slices/distributionSlice";
import { DistributionSearchFormData, DistributionSearchRequest, DistributionSearchResponse } from "../../../types";
import { ServiceErrorResponse } from "../../../types/errors/errors";
import DeleteDistributionModal from "./DeleteDistributionModal";
import DistributionInquiryGrid from "./DistributionInquiryGrid";
import DistributionInquirySearchFilter from "./DistributionInquirySearchFilter";
import NewEntryDialog from "./NewEntryDialog";

interface LocationState {
  showSuccessMessage?: boolean;
  memberName?: string;
  amount?: number;
  operationType?: "added" | "deleted" | "delete-failed";
}

const DistributionInquiryContent = () => {
  const dispatch = useDispatch();
  const location = useLocation();
  const navigate = useNavigate();
  const [searchData, setSearchData] = useState<DistributionSearchRequest | null>(null);
  const [hasSearched, setHasSearched] = useState(false);
  const [isNewEntryDialogOpen, setIsNewEntryDialogOpen] = useState(false);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [distributionToDelete, setDistributionToDelete] = useState<DistributionSearchResponse | null>(null);
  const [triggerSearch, { data, isFetching }] = useLazySearchDistributionsQuery();
  const [deleteDistribution, { isLoading: isDeleting }] = useDeleteDistributionMutation();
  const isReadOnly = useReadOnlyNavigation();
  const { addAlert, clearAlerts } = useMissiveAlerts();

  // Clear all distribution slice data and set distribution home when component mounts
  useEffect(() => {
    dispatch(clearCurrentMember());
    dispatch(clearCurrentDistribution());
    dispatch(clearPendingDisbursements());
    dispatch(clearHistoricalDisbursements());
    dispatch(setDistributionHome(ROUTES.DISTRIBUTIONS_INQUIRY));
  }, [dispatch]);

  // Display success/error message if returning from AddDistribution or after deletion
  useEffect(() => {
    const state = location.state as LocationState | undefined;
    if (state?.showSuccessMessage && state?.memberName) {
      let message = "Distribution Saved Successfully";
      let description = "";

      if (state.operationType === "deleted") {
        message = "Distribution Deleted Successfully";
        description = `${state.memberName}'s distribution has been successfully deleted.`;
      } else if (state.operationType === "delete-failed") {
        message = "Distribution Deletion Failed";
        description = `${state.memberName}'s distribution could not be deleted.`;
      } else {
        // Default to added/saved
        const amountText = state.amount ? ` for $${formatNumberWithComma(state.amount)}` : "";
        description = `Distribution${amountText} for ${state.memberName} has been saved successfully.`;
      }

      const successMessage = {
        id: 911,
        severity: state.operationType === "delete-failed" ? ("error" as const) : ("success" as const),
        message: message,
        description: description
      };
      addAlert(successMessage);
      // Clear the state after displaying the message
      window.history.replaceState({}, document.title);
    }
  }, [location, addAlert]);

  // Listen for delete modal open event from DistributionActions
  useEffect(() => {
    const handleOpenDeleteModal = (event: Event) => {
      const customEvent = event as CustomEvent;
      const distribution = customEvent.detail as DistributionSearchResponse;
      setDistributionToDelete(distribution);
      setIsDeleteDialogOpen(true);
    };

    window.addEventListener("openDeleteModal", handleOpenDeleteModal);
    return () => window.removeEventListener("openDeleteModal", handleOpenDeleteModal);
  }, []);

  const handleSearch = async (formData: DistributionSearchFormData) => {
    try {
      clearAlerts();

      const request: DistributionSearchRequest & {
        onlyNetworkToastErrors?: boolean;
      } = {
        skip: 0,
        take: 25,
        sortBy: "badgeNumber",
        isSortDescending: false,
        onlyNetworkToastErrors: true // Suppress validation errors, only show network errors
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
    } catch (error) {
      const serviceError = error as ServiceErrorResponse;
      // Reset hasSearched to false on error to hide the grid
      setHasSearched(false);

      // Check if it's a 500 error with "Badge number not found" or "SSN not found" title
      if (
        serviceError?.data.status === 500 &&
        (serviceError?.data?.title === "Badge number not found." || serviceError?.data?.title === "SSN not found.")
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

  const handlePaginationChange = async (pageNumber: number, pageSize: number, sortParams: SortParams) => {
    if (searchData) {
      const request: DistributionSearchRequest = {
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

  const handleCloseDeleteDialog = () => {
    setIsDeleteDialogOpen(false);
    setDistributionToDelete(null);
  };

  const handleConfirmDelete = async () => {
    if (!distributionToDelete) return;

    try {
      await deleteDistribution(distributionToDelete.id).unwrap();
      handleCloseDeleteDialog();

      // Navigate to inquiry page with success message
      navigate("", {
        state: {
          showSuccessMessage: true,
          operationType: "deleted",
          memberName: distributionToDelete.fullName
        }
      });
    } catch (error) {
      const serviceError = error as ServiceErrorResponse;
      const errorMsg = serviceError?.data?.detail || "Failed to delete distribution";
      const errorMessage = {
        id: 912,
        severity: "error" as const,
        message: "Delete Failed",
        description: errorMsg
      };
      addAlert(errorMessage);
      handleCloseDeleteDialog();
    }
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

      <MissiveAlerts />

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

      {/* Delete Distribution Modal */}
      <DeleteDistributionModal
        open={isDeleteDialogOpen}
        distribution={distributionToDelete}
        onConfirm={handleConfirmDelete}
        onCancel={handleCloseDeleteDialog}
        isLoading={isDeleting}
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
