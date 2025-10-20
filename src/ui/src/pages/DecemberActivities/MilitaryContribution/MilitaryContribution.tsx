import { Dialog, DialogContent, DialogTitle, Grid } from "@mui/material";
import FrozenYearWarning from "components/FrozenYearWarning";
import MissiveAlerts from "components/MissiveAlerts/MissiveAlerts";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import StatusReadOnlyInfo from "components/StatusReadOnlyInfo";
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { ApiMessageAlert, DSMAccordion, formatNumberWithComma, Page, setMessage } from "smart-ui-library";
import { MissiveAlertProvider } from "../../../components/MissiveAlerts/MissiveAlertContext";
import { CAPTIONS } from "../../../constants";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { useIsProfitYearFrozen } from "../../../hooks/useIsProfitYearFrozen";
import { useIsReadOnlyByStatus } from "../../../hooks/useIsReadOnlyByStatus";
import { useMissiveAlerts } from "../../../hooks/useMissiveAlerts";
import { useReadOnlyNavigation } from "../../../hooks/useReadOnlyNavigation";
import { InquiryApi } from "../../../reduxstore/api/InquiryApi";
import { MessageKeys, Messages } from "../../../utils/messageDictonary";
import useMilitaryContribution from "./hooks/useMilitaryContribution";
import MilitaryContributionForm from "./MilitaryContributionForm";
import MilitaryContributionGrid from "./MilitaryContributionFormGrid";
import MilitaryContributionSearchFilter from "./MilitaryContributionSearchFilter";

const MilitaryContributionContent = () => {
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [currentStatus, setCurrentStatus] = useState<string | null>(null);
  const [memberDetailsRefreshTrigger, setMemberDetailsRefreshTrigger] = useState(0);
  const { masterInquiryMemberDetails } = useSelector((state: RootState) => state.inquiry);
  const profitYear = useDecemberFlowProfitYear();
  const dispatch = useDispatch();
  const { missiveAlerts } = useMissiveAlerts();
  const isReadOnly = useReadOnlyNavigation();
  const isReadOnlyByStatus = useIsReadOnlyByStatus();
  const isFrozen = useIsProfitYearFrozen(profitYear);

  const {
    contributionsData,
    isLoadingContributions,
    contributionsGridPagination,
    fetchMilitaryContributions,
    resetSearch
  } = useMilitaryContribution();

  const handleStatusChange = (newStatus: string, statusName?: string) => {
    if (statusName === "Complete" && currentStatus !== "Complete") {
      setCurrentStatus("Complete");
      fetchMilitaryContributions();
    } else {
      setCurrentStatus(statusName || newStatus);
    }
  };

  const renderActionNode = () => {
    return <StatusDropdownActionNode onStatusChange={handleStatusChange} />;
  };

  const handleOpenForm = () => {
    if (!isReadOnly) {
      setIsDialogOpen(true);
    }
  };

  const handleCloseForm = () => {
    setIsDialogOpen(false);
    fetchMilitaryContributions();
    setMemberDetailsRefreshTrigger((prev) => prev + 1);
  };

  const handleContributionSaved = (contribution: {
    contributionAmount: number;
    contributionYear: number;
    isSupplementalContribution: boolean;
  }) => {
    const employeeName =
      masterInquiryMemberDetails?.firstName && masterInquiryMemberDetails?.lastName
        ? `${masterInquiryMemberDetails.firstName} ${masterInquiryMemberDetails.lastName}`
        : "the selected employee";

    const contributionType = contribution.isSupplementalContribution ? "supplemental" : "regular";
    const successMessage = `The ${contributionType} military contribution of $${formatNumberWithComma(contribution.contributionAmount)} for year ${contribution.contributionYear} for ${employeeName} saved successfully`;

    dispatch(
      setMessage({
        ...Messages.MilitaryContributionSaveSuccess,
        message: {
          ...Messages.MilitaryContributionSaveSuccess.message,
          message: successMessage
        }
      })
    );

    handleCloseForm();
  };

  useEffect(() => {
    if (masterInquiryMemberDetails) {
      fetchMilitaryContributions();
    }
  }, [masterInquiryMemberDetails, fetchMilitaryContributions]);

  // Clear the member and contributions state when this component unmounts
  // to ensure visiting the page fresh doesn't show the previous search.
  useEffect(() => {
    return () => {
      resetSearch();
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return (
    <Grid
      container
      rowSpacing="24px">
      {isFrozen && <FrozenYearWarning profitYear={profitYear} />}
      {isReadOnlyByStatus && <StatusReadOnlyInfo />}
      <Grid width={"100%"}>
        <ApiMessageAlert commonKey={MessageKeys.MilitaryContribution} />
      </Grid>

      <Grid width={"100%"}>
        <DSMAccordion title="Filter">
          <MilitaryContributionSearchFilter />
        </DSMAccordion>
      </Grid>

      {missiveAlerts.length > 0 && <MissiveAlerts />}

      <Grid width="100%">
        {masterInquiryMemberDetails ? (
          <MilitaryContributionGrid
            militaryContributionsData={contributionsData}
            isLoadingContributions={isLoadingContributions}
            contributionsGridPagination={contributionsGridPagination}
            onAddContribution={handleOpenForm}
            refreshTrigger={memberDetailsRefreshTrigger}
            isReadOnly={isReadOnly}
          />
        ) : (
          <div className="military-contribution-message">
            Please search for and select an employee to view military contributions.
          </div>
        )}
      </Grid>

      <Dialog
        open={isDialogOpen}
        onClose={handleCloseForm}
        maxWidth="md"
        fullWidth>
        <DialogTitle>Add Military Contribution</DialogTitle>
        <DialogContent>
          <MilitaryContributionForm
            onSubmit={(contribution) => {
              handleContributionSaved(contribution);
              dispatch(InquiryApi.util.invalidateTags(["memberDetails"]));
            }}
            onCancel={handleCloseForm}
            badgeNumber={Number(masterInquiryMemberDetails?.badgeNumber)}
            profitYear={profitYear}
          />
        </DialogContent>
      </Dialog>
    </Grid>
  );
};

const MilitaryContribution = () => {
  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  return (
    <Page
      label={CAPTIONS.MILITARY_CONTRIBUTIONS}
      actionNode={renderActionNode()}>
      <MissiveAlertProvider>
        <MilitaryContributionContent />
      </MissiveAlertProvider>
    </Page>
  );
};

export default MilitaryContribution;
