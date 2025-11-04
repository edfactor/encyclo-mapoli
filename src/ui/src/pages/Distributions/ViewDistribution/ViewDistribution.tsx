import CloseIcon from "@mui/icons-material/Close";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import { Button, CircularProgress, Divider, Grid, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate, useParams } from "react-router-dom";
import { Page } from "smart-ui-library";
import { MissiveAlertProvider } from "../../../components/MissiveAlerts/MissiveAlertContext";
import { CAPTIONS, ROUTES } from "../../../constants";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { useReadOnlyNavigation } from "../../../hooks/useReadOnlyNavigation";
import { useDeleteDistributionMutation } from "../../../reduxstore/api/DistributionApi";
import { setCurrentDistribution } from "../../../reduxstore/slices/distributionSlice";
import { RootState } from "../../../reduxstore/store";
import { ServiceErrorResponse } from "../../../types/errors/errors";
import MasterInquiryMemberDetails from "../../InquiriesAndAdjustments/MasterInquiry/MasterInquiryMemberDetails";
import DeleteDistributionModal from "../DistributionInquiry/DeleteDistributionModal";
import DisbursementsHistory from "./DisbursementsHistory";
import DistributionDetailsSection from "./DistributionDetailsSection";
import useViewDistribution from "./hooks/useViewDistribution";
import PendingDisbursementsList from "./PendingDisbursementsList";

const ViewDistributionContent = () => {
  const { memberId, memberType } = useParams<{ memberId: string; memberType: string }>();
  const navigate = useNavigate();
  const dispatch = useDispatch();

  const profitYear = useDecemberFlowProfitYear();
  const isReadOnly = useReadOnlyNavigation();

  const { currentMember, currentDistribution } = useSelector((state: RootState) => state.distribution);

  const { isLoading, fetchMember, clearMemberData } = useViewDistribution();

  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [deleteDistribution, { isLoading: isDeleting }] = useDeleteDistributionMutation();

  // Fetch member data when component mounts or memberId changes
  useEffect(() => {
    if (memberId && memberType && profitYear) {
      fetchMember(memberId, memberType, profitYear);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [memberId, memberType, profitYear]);

  // Clear data only on unmount
  useEffect(() => {
    return () => {
      clearMemberData();
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleEdit = () => {
    // Navigate to edit page using URL parameters
    if (memberId && memberType) {
      // Set current distribution if available
      if (currentDistribution) {
        dispatch(setCurrentDistribution(currentDistribution));
      }
      navigate(`/${ROUTES.EDIT_DISTRIBUTION}/${memberId}/${memberType}`);
    }
  };

  const handleCancel = () => {
    navigate(`/${ROUTES.DISTRIBUTIONS_INQUIRY}`);
  };

  const handleDelete = () => {
    setIsDeleteDialogOpen(true);
  };

  const handleCloseDeleteDialog = () => {
    setIsDeleteDialogOpen(false);
  };

  const handleConfirmDelete = async () => {
    if (!currentDistribution) return;

    try {
      await deleteDistribution(currentDistribution.id).unwrap();
      handleCloseDeleteDialog();

      // Navigate to inquiry page with success message
      navigate(`/${ROUTES.DISTRIBUTIONS_INQUIRY}`, {
        state: {
          showSuccessMessage: true,
          operationType: "deleted",
          memberName: currentDistribution.fullName
        }
      });
    } catch (error) {
      const serviceError = error as ServiceErrorResponse;
      const errorMsg = serviceError?.data?.detail || "Failed to delete distribution";
      console.error("Delete failed:", errorMsg);
      handleCloseDeleteDialog();
      // Optionally show error notification here using MissiveAlerts if needed
    }
  };

  // Show error if no memberId or memberType parameter
  if (!memberId || !memberType) {
    return (
      <Grid
        container
        padding="24px">
        <Typography
          variant="h6"
          color="error">
          Member ID and Member Type are required in the URL.
        </Typography>
        <Typography
          variant="body1"
          sx={{ marginTop: "8px" }}>
          Example: /view-distribution/12345/1
        </Typography>
      </Grid>
    );
  }

  return (
    <Grid
      container
      rowSpacing="24px">
      <Grid width="100%">
        <Divider />
      </Grid>

      {/* Action Buttons */}
      <Grid
        width="100%"
        sx={{ display: "flex", justifyContent: "flex-end", paddingX: "24px", gap: "12px" }}>
        <Button
          variant="outlined"
          disabled={isReadOnly || isLoading}
          onClick={handleEdit}
          startIcon={<EditIcon />}>
          EDIT
        </Button>
        <Button
          variant="outlined"
          color="error"
          disabled={isReadOnly || isLoading || isDeleting}
          onClick={handleDelete}
          startIcon={<DeleteIcon />}>
          DELETE
        </Button>
        <Button
          variant="outlined"
          onClick={handleCancel}
          startIcon={<CloseIcon />}>
          CANCEL
        </Button>
      </Grid>

      {/* Loading State */}
      {isLoading && (
        <Grid
          width="100%"
          sx={{ display: "flex", justifyContent: "center", padding: "48px" }}>
          <CircularProgress />
        </Grid>
      )}

      {/* Content - Member and Distribution Details */}
      {!isLoading && currentMember && (
        <>
          <Grid width="100%">
            <Divider />
          </Grid>
          <MasterInquiryMemberDetails
            memberType={parseInt(memberType || "0")}
            id={parseInt(memberId || "0")}
            profitYear={profitYear}
            memberDetails={currentMember}
            isLoading={isLoading}
          />
          <Grid width="100%">
            <Divider />
          </Grid>
          {currentDistribution && (
            <>
              <DistributionDetailsSection distribution={currentDistribution} />
              <Grid width="100%">
                <Divider />
              </Grid>
            </>
          )}

          {/* Pending Disbursements List */}
          <PendingDisbursementsList
            badgeNumber={currentMember.badgeNumber}
            memberType={currentMember.isEmployee ? 1 : 2}
          />

          {/* Disbursements History */}
          <DisbursementsHistory
            badgeNumber={currentMember.badgeNumber}
            memberType={currentMember.isEmployee ? 1 : 2}
          />
        </>
      )}

      {/* No member found */}
      {!isLoading && !currentMember && (
        <Grid
          width="100%"
          padding="24px">
          <Typography
            variant="h6"
            color="error">
            Member not found for ID: {memberId}
          </Typography>
        </Grid>
      )}

      {/* Delete Distribution Modal */}
      <DeleteDistributionModal
        open={isDeleteDialogOpen}
        distribution={currentDistribution}
        onConfirm={handleConfirmDelete}
        onCancel={handleCloseDeleteDialog}
        isLoading={isDeleting}
      />
    </Grid>
  );
};

const ViewDistribution = () => {
  const renderActionNode = () => {
    return null;
  };

  return (
    <Page
      label={CAPTIONS.VIEW_DISTRIBUTION}
      actionNode={renderActionNode()}>
      <MissiveAlertProvider>
        <ViewDistributionContent />
      </MissiveAlertProvider>
    </Page>
  );
};

export default ViewDistribution;
