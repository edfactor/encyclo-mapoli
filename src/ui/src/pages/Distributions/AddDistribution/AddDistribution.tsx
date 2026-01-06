import CancelIcon from "@mui/icons-material/Cancel";
import RestartAltIcon from "@mui/icons-material/RestartAlt";
import SaveIcon from "@mui/icons-material/Save";
import { Alert, Button, CircularProgress, Divider, Grid, Tooltip } from "@mui/material";
import { useEffect, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate, useParams } from "react-router-dom";
import { Page } from "smart-ui-library";
import { MissiveAlertProvider } from "../../../components/MissiveAlerts/MissiveAlertContext";
import PageErrorBoundary from "../../../components/PageErrorBoundary/PageErrorBoundary";
import MissiveAlerts from "../../../components/MissiveAlerts/MissiveAlerts";
import { DISTRIBUTION_INQUIRY_MESSAGES } from "../../../components/MissiveAlerts/MissiveMessages";
import { CAPTIONS, ROUTES } from "../../../constants";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { useMissiveAlerts } from "../../../hooks/useMissiveAlerts";
import { useReadOnlyNavigation } from "../../../hooks/useReadOnlyNavigation";
import { RootState } from "../../../reduxstore/store";
import { CreateDistributionRequest } from "../../../types";
import MasterInquiryMemberDetails from "../../InquiriesAndAdjustments/MasterInquiry/MasterInquiryMemberDetails";
import PendingDisbursementsList from "../ViewDistribution/PendingDisbursementsList";
import AddDistributionForm, { AddDistributionFormRef } from "./AddDistributionForm";
import { useAddDistribution } from "./hooks/useAddDistribution";

const AddDistributionContent = () => {
  const navigate = useNavigate();
  const { memberId, memberType } = useParams<{ memberId: string; memberType: string }>();
  const formRef = useRef<AddDistributionFormRef>(null);
  const isReadOnly = useReadOnlyNavigation();
  const [isFormValid, setIsFormValid] = useState(false);
  const [submittedAmount, setSubmittedAmount] = useState<number | null>(null);
  const { missiveAlerts, addAlert, clearAlerts } = useMissiveAlerts();

  const profitYear = useDecemberFlowProfitYear();

  const { distributionHome } = useSelector((state: RootState) => state.distribution);

  // Use the custom hook
  const {
    memberData,
    isMemberLoading,
    memberError,
    stateTaxRate,
    isStateTaxLoading,
    stateTaxError,
    sequenceNumber,
    isSequenceNumberLoading,
    sequenceNumberError,
    maximumReached,
    isSubmitting,
    submissionError,
    submissionSuccess,
    fetchMemberData,
    submitDistribution,
    clearSubmissionError
  } = useAddDistribution();

  // Fetch member data on mount
  useEffect(() => {
    if (memberId && memberType && profitYear != null) {
      const memberIdNum = parseInt(memberId, 10);
      const memberTypeNum = parseInt(memberType, 10);

      if (!isNaN(memberIdNum) && !isNaN(memberTypeNum)) {
        clearAlerts();
        fetchMemberData(memberId, memberTypeNum, profitYear);
      }
    }
  }, [memberId, memberType, profitYear, fetchMemberData, clearAlerts]);

  // Show missive alert when member not found
  useEffect(() => {
    if (memberError === "MEMBER_NOT_FOUND") {
      addAlert(DISTRIBUTION_INQUIRY_MESSAGES.MEMBER_NOT_FOUND);
    }
  }, [memberError, addAlert]);

  // Handle successful submission
  useEffect(() => {
    if (submissionSuccess && memberData) {
      // Get member name
      const memberName = memberData.fullName;

      // Navigate to distributions inquiry page with success message
      navigate(`/${ROUTES.DISTRIBUTIONS_INQUIRY}`, {
        state: {
          showSuccessMessage: true,
          memberName: memberName,
          amount: submittedAmount
        }
      });
    }
  }, [submissionSuccess, memberData, submittedAmount, navigate]);

  // Handle form submission
  const handleFormSubmit = async (data: CreateDistributionRequest) => {
    try {
      setSubmittedAmount(data.grossAmount);
      await submitDistribution(data);
    } catch (error) {
      console.error("Failed to submit distribution:", error);
    }
  };

  // Handle form reset
  const handleFormReset = () => {
    clearSubmissionError(); // Clear submission error but keep member data
    if (formRef.current) {
      formRef.current.reset(); // Reset form fields
    }
  };

  // Handle cancel
  const handleCancel = () => {
    // Navigate to the stored distribution home route, fallback to distributions inquiry
    const homeRoute = distributionHome || ROUTES.DISTRIBUTIONS_INQUIRY;
    navigate(`/${homeRoute}`);
  };

  // Handle save button click
  const handleSave = () => {
    if (formRef.current) {
      formRef.current.submit();
    }
  };

  // Track 3rd party address requirement
  const [thirdPartyAddressRequired, setThirdPartyAddressRequired] = useState(false);

  // Determine validation errors
  const noAvailableBalance = memberData?.currentVestedAmount === 0;
  const validationError = maximumReached
    ? "Member has reached maximum of nine distributions."
    : noAvailableBalance
      ? "Member has no available balance to distribute."
      : null;

  // Check form validity periodically
  useEffect(() => {
    const interval = setInterval(() => {
      if (formRef.current) {
        setIsFormValid(formRef.current.isFormValid());
        setThirdPartyAddressRequired(formRef.current.isThirdPartyAddressRequired());
      }
    }, 100);

    return () => clearInterval(interval);
  }, []);

  // Handle reset button click
  const handleReset = () => {
    handleFormReset();
  };

  // Loading state
  const isLoading = isMemberLoading || isStateTaxLoading || isSequenceNumberLoading;

  const renderActionNode = () => {
    return (
      <div style={{ display: "flex", gap: "12px" }}>
        <Tooltip
          title={
            isReadOnly
              ? "You are in read-only mode"
              : thirdPartyAddressRequired
                ? "3rd Party Address Required"
                : validationError
                  ? validationError
                  : ""
          }>
          <span>
            <Button
              variant="outlined"
              size="small"
              onClick={handleSave}
              disabled={
                isReadOnly ||
                isSubmitting ||
                isMemberLoading ||
                !isFormValid ||
                thirdPartyAddressRequired ||
                !!validationError
              }
              startIcon={<SaveIcon />}>
              SAVE
            </Button>
          </span>
        </Tooltip>
        <Button
          variant="outlined"
          size="small"
          onClick={handleReset}
          disabled={isSubmitting || !memberData}
          startIcon={<RestartAltIcon />}>
          RESET
        </Button>
        <Button
          variant="outlined"
          size="small"
          onClick={handleCancel}
          disabled={isSubmitting}
          startIcon={<CancelIcon />}>
          CANCEL
        </Button>
      </div>
    );
  };

  return (
    <PageErrorBoundary pageName="Add Distribution">
      <Page
        label={CAPTIONS.ADD_DISTRIBUTION}
        actionNode={renderActionNode()}>
        <Grid
          container
          rowSpacing="24px">
          <Grid width="100%">
            <Divider />
          </Grid>

          {/* Missive Alerts */}
          {missiveAlerts.length > 0 && (
            <Grid
              width="100%"
              sx={{ paddingX: "24px" }}>
              <MissiveAlerts />
            </Grid>
          )}

          {/* Error Messages */}
          {validationError && (
            <Grid
              width="100%"
              sx={{ paddingX: "24px" }}>
              <Alert
                severity="error"
                sx={{
                  "& .MuiAlert-message": {
                    fontSize: "1.1rem",
                    fontWeight: "bold"
                  }
                }}>
                {validationError}
              </Alert>
            </Grid>
          )}

          {memberError && memberError !== "MEMBER_NOT_FOUND" && (
            <Grid
              width="100%"
              sx={{ paddingX: "24px" }}>
              <Alert severity="error">{memberError}</Alert>
            </Grid>
          )}

          {stateTaxError && (
            <Grid
              width="100%"
              sx={{ paddingX: "24px" }}>
              <Alert severity="warning">{stateTaxError}</Alert>
            </Grid>
          )}

          {sequenceNumberError && (
            <Grid
              width="100%"
              sx={{ paddingX: "24px" }}>
              <Alert severity="warning">{sequenceNumberError}</Alert>
            </Grid>
          )}

          {submissionError && (
            <Grid
              width="100%"
              sx={{ paddingX: "24px" }}>
              <Alert severity="error">{submissionError}</Alert>
            </Grid>
          )}

          {/* Loading Indicator */}
          {isLoading && (
            <Grid
              width="100%"
              sx={{ display: "flex", justifyContent: "center", padding: "48px" }}>
              <CircularProgress />
            </Grid>
          )}

          {/* Content - Member and Distribution Details */}
          {!isLoading && memberData && (
            <>
              <MasterInquiryMemberDetails
                memberType={parseInt(memberType || "0", 10)}
                id={memberId as string}
                profitYear={profitYear || 0}
                memberDetails={memberData}
                isLoading={isLoading}
              />
              <Grid width="100%">
                <Divider />
              </Grid>

              {/* Distribution Form Section */}
              <Grid
                width="100%"
                sx={{ paddingX: "24px" }}>
                <AddDistributionForm
                  ref={formRef}
                  stateTaxRate={stateTaxRate}
                  sequenceNumber={sequenceNumber}
                  badgeNumber={memberData.badgeNumber}
                  onSubmit={handleFormSubmit}
                  onReset={handleFormReset}
                  isSubmitting={isSubmitting}
                  dateOfBirth={memberData.dateOfBirth}
                  age={memberData.age}
                  vestedAmount={memberData.currentVestedAmount}
                />
              </Grid>

              <Grid width="100%">
                <Divider />
              </Grid>

              {/* Pending Disbursements List Section */}
              <PendingDisbursementsList
                badgeNumber={memberData.badgeNumber}
                memberType={parseInt(memberType || "0", 10)}
              />
            </>
          )}

          {/* No member found */}
          {!isLoading && !memberData && !memberError && (
            <Grid
              width="100%"
              sx={{ paddingX: "24px" }}>
              <Alert severity="info">No member data available. Please check the member ID and type.</Alert>
            </Grid>
          )}
        </Grid>
      </Page>
    </PageErrorBoundary>
  );
};

const AddDistribution = () => {
  return (
    <MissiveAlertProvider>
      <AddDistributionContent />
    </MissiveAlertProvider>
  );
};

export default AddDistribution;
