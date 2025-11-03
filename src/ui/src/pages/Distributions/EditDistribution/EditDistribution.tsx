import CancelIcon from "@mui/icons-material/Cancel";
import RestartAltIcon from "@mui/icons-material/RestartAlt";
import SaveIcon from "@mui/icons-material/Save";
import { Alert, Button, CircularProgress, Divider, Grid, Tooltip } from "@mui/material";
import { useEffect, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate, useParams } from "react-router-dom";
import { Page } from "smart-ui-library";
import { MissiveAlertProvider } from "../../../components/MissiveAlerts/MissiveAlertContext";
import { CAPTIONS, ROUTES } from "../../../constants";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { useReadOnlyNavigation } from "../../../hooks/useReadOnlyNavigation";
import {
  useLazyGetProfitMasterInquiryMemberQuery,
  useLazySearchProfitMasterInquiryQuery
} from "../../../reduxstore/api/InquiryApi";
import { useLazyGetStateTaxQuery } from "../../../reduxstore/api/LookupsApi";
import type { RootState } from "../../../reduxstore/store";
import { EditDistributionRequest, EmployeeDetails } from "../../../types";
import { ServiceErrorResponse } from "../../../types/errors/errors";
import MasterInquiryMemberDetails from "../../InquiriesAndAdjustments/MasterInquiry/MasterInquiryMemberDetails";
import PendingDisbursementsList from "../ViewDistribution/PendingDisbursementsList";
import EditDistributionForm, { EditDistributionFormRef } from "./EditDistributionForm";
import { useEditDistribution } from "./hooks/useEditDistribution";

const EditDistributionContent = () => {
  const navigate = useNavigate();
  const { memberId, memberType } = useParams<{ memberId: string; memberType: string }>();
  const formRef = useRef<EditDistributionFormRef>(null);
  const isReadOnly = useReadOnlyNavigation();
  const [isFormValid, setIsFormValid] = useState(false);

  // Get current distribution from Redux
  const currentDistribution = useSelector((state: RootState) => state.distribution.currentDistribution);

  const profitYear = useDecemberFlowProfitYear();

  // Use the custom hook
  const { isSubmitting, submissionError, submissionSuccess, submitDistribution, clearSubmissionError } =
    useEditDistribution();

  // Fetch member data
  const [triggerSearchMember] = useLazySearchProfitMasterInquiryQuery();
  const [triggerGetMember] = useLazyGetProfitMasterInquiryMemberQuery();
  const [triggerGetStateTax] = useLazyGetStateTaxQuery();

  const [memberData, setMemberData] = useState<EmployeeDetails | null>(null);
  const [isMemberLoading, setIsMemberLoading] = useState(false);
  const [memberError, setMemberError] = useState<string | null>(null);
  const [stateTaxRate, setStateTaxRate] = useState<number | null>(null);
  const [isStateTaxLoading, setIsStateTaxLoading] = useState(false);
  const [stateTaxError, setStateTaxError] = useState<string | null>(null);

  // Fetch member data on mount
  useEffect(() => {
    const fetchMemberInfo = async () => {
      if (!memberId || !memberType || !currentDistribution || !profitYear) {
        return;
      }

      try {
        setIsMemberLoading(true);
        setMemberError(null);

        const memberIdNum = parseInt(memberId, 10);
        const memberTypeNum = parseInt(memberType, 10);

        if (isNaN(memberIdNum) || isNaN(memberTypeNum)) {
          throw new Error("Invalid member ID or member type");
        }

        // Step 1: Search for member using badge number
        const searchResponse = await triggerSearchMember({
          badgeNumber: currentDistribution.badgeNumber,
          memberType: memberTypeNum,
          endProfitYear: profitYear,
          pagination: {
            skip: 0,
            take: 1,
            sortBy: "badgeNumber",
            isSortDescending: false
          }
        }).unwrap();

        const results = Array.isArray(searchResponse) ? searchResponse : searchResponse.results;
        if (!results || results.length === 0) {
          throw new Error("Member not found");
        }

        // Step 2: Fetch member details using the ID
        const memberResponse = await triggerGetMember({
          id: results[0].id,
          memberType: memberTypeNum,
          profitYear
        }).unwrap();

        setMemberData(memberResponse);

        // Fetch state tax rate
        if (memberResponse?.addressState) {
          try {
            setIsStateTaxLoading(true);
            const stateTaxResponse = await triggerGetStateTax(memberResponse.addressState).unwrap();
            setStateTaxRate(stateTaxResponse.stateTaxRate);
          } catch (error) {
            const serviceError = error as ServiceErrorResponse;
            const errorMsg = serviceError?.data?.detail || "Failed to fetch state tax rate";
            setStateTaxError(errorMsg);
            setStateTaxRate(0);
          } finally {
            setIsStateTaxLoading(false);
          }
        }
      } catch (error) {
        const serviceError = error as ServiceErrorResponse;
        const errorMsg = serviceError?.data?.detail || "Failed to fetch member data";
        setMemberError(errorMsg);
      } finally {
        setIsMemberLoading(false);
      }
    };

    fetchMemberInfo();
  }, [
    memberId,
    memberType,
    currentDistribution,
    profitYear,
    triggerSearchMember,
    triggerGetMember,
    triggerGetStateTax
  ]);

  // Handle successful submission
  useEffect(() => {
    if (submissionSuccess && memberData) {
      const memberName = `${memberData.firstName} ${memberData.lastName}`;

      navigate(`/${ROUTES.DISTRIBUTIONS_INQUIRY}`, {
        state: {
          showSuccessMessage: true,
          memberName: memberName,
          operationType: "updated"
        }
      });
    }
  }, [submissionSuccess, memberData, navigate]);

  // Handle form submission
  const handleFormSubmit = async (data: EditDistributionRequest) => {
    try {
      await submitDistribution(data);
    } catch (error) {
      console.error("Failed to submit distribution update:", error);
    }
  };

  // Handle form reset
  const handleFormReset = () => {
    clearSubmissionError();
    if (formRef.current) {
      formRef.current.reset();
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

  // Loading state
  const isLoading = isMemberLoading || isStateTaxLoading;

  // Check if we have required data
  if (!currentDistribution) {
    return (
      <Grid
        container
        rowSpacing="24px"
        sx={{ paddingX: "24px" }}>
        <Grid width="100%">
          <Alert severity="info">
            No distribution selected. Please select a distribution to edit from the Distribution Inquiry page.
          </Alert>
        </Grid>
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
        <Tooltip
          title={
            isReadOnly ? "You are in read-only mode" : thirdPartyAddressRequired ? "3rd Party Address Required" : ""
          }>
          <span>
            <Button
              variant="outlined"
              onClick={handleSave}
              disabled={isReadOnly || isSubmitting || isMemberLoading || !isFormValid || thirdPartyAddressRequired}
              startIcon={<SaveIcon />}>
              SAVE
            </Button>
          </span>
        </Tooltip>
        <Button
          variant="outlined"
          onClick={handleFormReset}
          disabled={isSubmitting}
          startIcon={<RestartAltIcon />}>
          RESET
        </Button>
        <Button
          variant="outlined"
          onClick={handleCancel}
          disabled={isSubmitting}
          startIcon={<CancelIcon />}>
          CANCEL
        </Button>
      </Grid>

      {/* Error Messages */}
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
            profitYear={profitYear || 0}
            memberDetails={memberData}
            isLoading={isLoading}
            memberType={parseInt(memberType || "0")}
            id={parseInt(memberId || "0")}
          />
          <Grid width="100%">
            <Divider />
          </Grid>

          {/* Distribution Form Section */}
          <Grid
            width="100%"
            sx={{ paddingX: "24px" }}>
            <EditDistributionForm
              ref={formRef}
              distribution={currentDistribution}
              stateTaxRate={stateTaxRate}
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
            badgeNumber={currentDistribution.badgeNumber}
            memberType={currentDistribution.demographicId ? 1 : 2}
          />
        </>
      )}

      {/* No member found */}
      {!isLoading && !memberData && !memberError && (
        <Grid
          width="100%"
          sx={{ paddingX: "24px" }}>
          <Alert severity="info">No member data available. Please try again.</Alert>
        </Grid>
      )}
    </Grid>
  );
};

const EditDistribution = () => {
  const renderActionNode = () => {
    return null;
  };

  return (
    <Page
      label={CAPTIONS.EDIT_DISTRIBUTION}
      actionNode={renderActionNode()}>
      <MissiveAlertProvider>
        <EditDistributionContent />
      </MissiveAlertProvider>
    </Page>
  );
};

export default EditDistribution;
