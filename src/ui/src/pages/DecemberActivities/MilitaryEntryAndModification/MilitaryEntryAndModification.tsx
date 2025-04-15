import { Dialog, DialogContent, DialogTitle, Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useCallback, useEffect, useState } from "react";
import { DSMAccordion, ISortParams, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import MilitaryContributionForm from "./MilitaryContributionForm";
import MilitaryContributionGrid from "./MilitaryContributionFormGrid";
import { RootState } from "reduxstore/store";
import { useLazyGetMilitaryContributionsQuery} from "../../../reduxstore/api/MilitaryApi";
import { useSelector } from "react-redux";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import MilitaryEntryAndModificationSearchFilter from "./MilitaryEntryAndModificationSearchFilter";

const MilitaryEntryAndModification = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [showContributions, setShowContributions] = useState(false);  
  const { masterInquiryEmployeeDetails } = useSelector((state: RootState) => state.inquiry);
  const [fetchContributions, { isFetching }] = useLazyGetMilitaryContributionsQuery();
  const profitYear = useDecemberFlowProfitYear();
  
  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  const handleOpenForm = () => {
    setIsDialogOpen(true);
  };

  const handleCloseForm = () => {
    setIsDialogOpen(false);
    handleFetchContributions();
    setInitialSearchLoaded(true); // This will trigger the grid to refresh
  };

  const handleFetchContributions = useCallback(() => {
    if (masterInquiryEmployeeDetails) {
      fetchContributions({
        badgeNumber: Number(masterInquiryEmployeeDetails.badgeNumber),
        profitYear: profitYear,
        contributionAmount: 0,
        contributionDate: "",
        pagination: {
          skip: 0,
          take: 25,
          sortBy: "contributionDate",
          isSortDescending: false}});
      setShowContributions(true);
    }
  }, [masterInquiryEmployeeDetails, fetchContributions]);

  useEffect(() => {
    if (masterInquiryEmployeeDetails) {
      handleFetchContributions();
    }
  }, [handleFetchContributions, masterInquiryEmployeeDetails]);

  return (
    <Page
      label={CAPTIONS.MILITARY_CONTRIBUTIONS}
      actionNode={renderActionNode()}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <MilitaryEntryAndModificationSearchFilter setInitialSearchLoaded={setInitialSearchLoaded} />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <MilitaryContributionGrid
            setInitialSearchLoaded={setInitialSearchLoaded}
            initialSearchLoaded={initialSearchLoaded}
            onAddContribution={handleOpenForm}
          />
        </Grid2>
      </Grid2>

        {/* Military Contribution Form Dialog */}
        <Dialog
          open={isDialogOpen}
          onClose={handleCloseForm}
          maxWidth="md"
          fullWidth
        >
          <DialogTitle>Add Military Contribution</DialogTitle>
          <DialogContent>
            <MilitaryContributionForm
              onSubmit={(rows) => {
                handleCloseForm();         
              }}
              onCancel={handleCloseForm}
              badgeNumber={Number(masterInquiryEmployeeDetails?.badgeNumber)}
              profitYear={profitYear}
            />
          </DialogContent>
        </Dialog>
      </Page>
    );
  };
export default MilitaryEntryAndModification;