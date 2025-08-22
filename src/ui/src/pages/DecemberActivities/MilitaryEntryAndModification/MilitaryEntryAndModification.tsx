import { Dialog, DialogContent, DialogTitle, Divider, Grid } from "@mui/material";
import MissiveAlerts from "components/MissiveAlerts/MissiveAlerts";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { InquiryApi } from "../../../reduxstore/api/InquiryApi";
import { useMissiveAlerts } from "../../MasterInquiry/hooks/useMissiveAlerts";
import { MissiveAlertProvider } from "../../MasterInquiry/utils/MissiveAlertContext";
import useMilitaryEntryAndModification from "./hooks/useMilitaryEntryAndModification";
import MilitaryContributionForm from "./MilitaryContributionForm";
import MilitaryContributionGrid from "./MilitaryContributionFormGrid";
import MilitaryEntryAndModificationSearchFilter from "./MilitaryEntryAndModificationSearchFilter";

const MilitaryEntryAndModificationContent = () => {
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [currentStatus, setCurrentStatus] = useState<string | null>(null);
  const { masterInquiryMemberDetails } = useSelector((state: RootState) => state.inquiry);
  const profitYear = useDecemberFlowProfitYear();
  const dispatch = useDispatch();
  const { missiveAlerts } = useMissiveAlerts();

  const { contributionsData, isLoadingContributions, contributionsGridPagination, fetchMilitaryContributions } =
    useMilitaryEntryAndModification();

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
    setIsDialogOpen(true);
  };

  const handleCloseForm = () => {
    setIsDialogOpen(false);
    fetchMilitaryContributions();
  };

  useEffect(() => {
    if (masterInquiryMemberDetails) {
      fetchMilitaryContributions();
    }
  }, [masterInquiryMemberDetails, fetchMilitaryContributions]);

  return (
    <Grid
      container
      rowSpacing="24px">
      <Grid width={"100%"}>
        <Divider />
      </Grid>
      <Grid width={"100%"}>
        <DSMAccordion title="Filter">
          <MilitaryEntryAndModificationSearchFilter />
        </DSMAccordion>
      </Grid>

      {missiveAlerts.length > 0 && <MissiveAlerts missiveAlerts={missiveAlerts} />}

      <Grid width="100%">
        {masterInquiryMemberDetails ? (
          <MilitaryContributionGrid
            militaryContributionsData={contributionsData}
            isLoadingContributions={isLoadingContributions}
            contributionsGridPagination={contributionsGridPagination}
            onAddContribution={handleOpenForm}
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
            onSubmit={(rows) => {
              handleCloseForm();
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

const MilitaryEntryAndModification = () => {
  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  return (
    <Page
      label={CAPTIONS.MILITARY_CONTRIBUTIONS}
      actionNode={renderActionNode()}>
      <MissiveAlertProvider>
        <MilitaryEntryAndModificationContent />
      </MissiveAlertProvider>
    </Page>
  );
};

export default MilitaryEntryAndModification;
