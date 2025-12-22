import CloseIcon from "@mui/icons-material/Close";
import EditIcon from "@mui/icons-material/Edit";
import { Button, CircularProgress, Divider, Grid, Typography } from "@mui/material";
import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate, useParams } from "react-router-dom";
import { Page } from "smart-ui-library";
import { MissiveAlertProvider } from "../../../components/MissiveAlerts/MissiveAlertContext";
import { CAPTIONS, ROUTES } from "../../../constants";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { useReadOnlyNavigation } from "../../../hooks/useReadOnlyNavigation";
import { setCurrentDistribution } from "../../../reduxstore/slices/distributionSlice";
import { RootState } from "../../../reduxstore/store";
import { isSafePath, isValidRouteParams } from "../../../utils/pathValidation";
import MasterInquiryMemberDetails from "../../InquiriesAndAdjustments/MasterInquiry/MasterInquiryMemberDetails";
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

  const { currentMember, currentDistribution, distributionHome } = useSelector(
    (state: RootState) => state.distribution
  );

  const { isLoading, fetchMember, clearMemberData } = useViewDistribution();

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
    // Validate parameters to prevent injection attacks
    if (isValidRouteParams(memberId, memberType)) {
      // Set current distribution if available
      if (currentDistribution) {
        dispatch(setCurrentDistribution(currentDistribution));
      }
      const path = `/${ROUTES.EDIT_DISTRIBUTION}/${memberId}/${memberType}`;

      // Final path validation before navigation
      if (isSafePath(path)) {
        navigate(path);
      }
    }
  };

  const handleClose = () => {
    // Navigate to the stored distribution home route, fallback to distributions inquiry
    const homeRoute = distributionHome || ROUTES.DISTRIBUTIONS_INQUIRY;
    navigate(`/${homeRoute}`);
  };

  const renderActionNode = () => {
    return (
      <div style={{ display: "flex", gap: "12px" }}>
        <Button
          variant="outlined"
          size="small"
          disabled={isReadOnly || isLoading}
          onClick={handleEdit}
          startIcon={<EditIcon />}>
          EDIT
        </Button>
        <Button
          variant="outlined"
          size="small"
          onClick={handleClose}
          startIcon={<CloseIcon />}>
          CLOSE
        </Button>
      </div>
    );
  };

  // Show error if no memberId or memberType parameter
  if (!memberId || !memberType) {
    return (
      <Page
        label={CAPTIONS.VIEW_DISTRIBUTION}
        actionNode={null}>
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
      </Page>
    );
  }

  return (
    <Page
      label={CAPTIONS.VIEW_DISTRIBUTION}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width="100%">
          <Divider />
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
      </Grid>
    </Page>
  );
};

const ViewDistribution = () => {
  return (
    <MissiveAlertProvider>
      <ViewDistributionContent />
    </MissiveAlertProvider>
  );
};

export default ViewDistribution;
