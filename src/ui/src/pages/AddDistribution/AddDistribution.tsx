import { Alert, Button, CircularProgress, Divider, Grid, Tooltip } from "@mui/material";
import { useEffect, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Page } from "smart-ui-library";
import { MissiveAlertProvider } from "../../components/MissiveAlerts/MissiveAlertContext";
import { CAPTIONS, ROUTES } from "../../constants";
import useDecemberFlowProfitYear from "../../hooks/useDecemberFlowProfitYear";
import { useReadOnlyNavigation } from "../../hooks/useReadOnlyNavigation";
import { CreateDistributionRequest } from "../../types";
import MasterInquiryMemberDetails from "../MasterInquiry/MasterInquiryMemberDetails";
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

  const profitYear = useDecemberFlowProfitYear();

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
        fetchMemberData(memberId, memberTypeNum, profitYear);
      }
    }
  }, [memberId, memberType, profitYear, fetchMemberData]);

  // Handle successful submission
  useEffect(() => {
    if (submissionSuccess && memberData) {
      // Get member name
      const memberName = `${memberData.firstName} ${memberData.lastName}`;

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
    navigate(`/${ROUTES.DISTRIBUTIONS_INQUIRY}`);
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
  const maxDistributionsReached = sequenceNumber === 10;
  const noAvailableBalance = memberData?.currentVestedAmount === 0;
  const validationError = maxDistributionsReached
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
              variant="contained"
              onClick={handleSave}
              disabled={
                isReadOnly ||
                isSubmitting ||
                isMemberLoading ||
                !isFormValid ||
                thirdPartyAddressRequired ||
                !!validationError
              }
              className="h-10 min-w-fit whitespace-nowrap">
              SAVE
            </Button>
          </span>
        </Tooltip>
        <Button
          variant="outlined"
          onClick={handleReset}
          disabled={isSubmitting}
          className="h-10 min-w-fit whitespace-nowrap">
          RESET
        </Button>
        <Button
          variant="outlined"
          onClick={handleCancel}
          disabled={isSubmitting}
          className="h-10 min-w-fit whitespace-nowrap">
          CANCEL
        </Button>
      </Grid>

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

      {memberError && (
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
          <Grid width="100%">
            <Divider />
          </Grid>
          <MasterInquiryMemberDetails
            //memberType={parseInt(memberType || "0", 10)}
            //id={memberId as string}
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
              badgeNumber={parseInt(memberId || "0", 10)}
              onSubmit={handleFormSubmit}
              onReset={handleFormReset}
              isSubmitting={isSubmitting}
              dateOfBirth={memberData.dateOfBirth}
              age={
                memberData.dateOfBirth
                  ? Math.floor(
                      (Date.now() - new Date(memberData.dateOfBirth).getTime()) / (1000 * 60 * 60 * 24 * 365.25)
                    )
                  : undefined
              }
              vestedAmount={memberData.currentVestedAmount}
            />
          </Grid>

          <Grid width="100%">
            <Divider />
          </Grid>

          {/* Pending Disbursements List Section */}
          <PendingDisbursementsList
            badgeNumber={parseInt(memberId || "0", 10)}
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
  );
};

const AddDistribution = () => {
  const renderActionNode = () => {
    return null;
  };

  return (
    <Page
      label={CAPTIONS.ADD_DISTRIBUTION}
      actionNode={renderActionNode()}>
      <MissiveAlertProvider>
        <AddDistributionContent />
      </MissiveAlertProvider>
    </Page>
  );
};

export default AddDistribution;
