import { Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import StandaloneMemberDetails from "pages/MasterInquiry/StandaloneMemberDetails";
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetForfeitureAdjustmentsQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import useDecemberFlowProfitYear from "../../hooks/useDecemberFlowProfitYear";
import { InquiryApi } from "../../reduxstore/api/InquiryApi";
import { MissiveAlertProvider } from "../MasterInquiry/utils/MissiveAlertContext";
import AddForfeitureModal from "./AddForfeitureModal";
import ForfeituresAdjustmentPanel from "./ForfeituresAdjustmentPanel";
import ForfeituresAdjustmentSearchParameters from "./ForfeituresAdjustmentSearchParameters";

const ForfeituresAdjustment = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [isAddForfeitureModalOpen, setIsAddForfeitureModalOpen] = useState(false);
  const [pageNumberReset, setPageNumberReset] = useState(false);
  const { forfeitureAdjustmentData, forfeitureAdjustmentQueryParams } = useSelector(
    (state: RootState) => state.forfeituresAdjustment
  );
  const profitYear = useDecemberFlowProfitYear();
  const [triggerSearch] = useLazyGetForfeitureAdjustmentsQuery();
  const dispatch = useDispatch();

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  const handleSearchComplete = (loaded: boolean) => {
    setInitialSearchLoaded(loaded);
  };

  const handleOpenAddForfeitureModal = () => {
    setIsAddForfeitureModalOpen(true);
  };

  const handleCloseAddForfeitureModal = () => {
    setIsAddForfeitureModalOpen(false);
  };

  const handleSaveForfeiture = () => {
    // Refresh grid data after saving a new forfeiture
    if (forfeitureAdjustmentQueryParams) {
      triggerSearch(forfeitureAdjustmentQueryParams)
        .unwrap()
        .then(() => {
          setInitialSearchLoaded(true);
        })
        .catch((error: unknown) => {
          console.error("Error refreshing forfeiture adjustments:", error);
        });
      dispatch(InquiryApi.util.invalidateTags(["memberDetails"]));
    }
  };

  useEffect(() => {
    if (forfeitureAdjustmentQueryParams && (!forfeitureAdjustmentData || !initialSearchLoaded)) {
      triggerSearch(forfeitureAdjustmentQueryParams)
        .unwrap()
        .then(() => {
          setInitialSearchLoaded(true);
        })
        .catch((error: unknown) => {
          console.error("Error refreshing forfeiture adjustments:", error);
        });
    }
  }, []);

  return (
    <Page
      label={CAPTIONS.FORFEITURES_ADJUSTMENT}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <DSMAccordion title="Filter">
            <ForfeituresAdjustmentSearchParameters
              setInitialSearchLoaded={handleSearchComplete}
              setPageReset={setPageNumberReset}
            />
          </DSMAccordion>
        </Grid>

        {/* Only show details if we have forfeitureAdjustmentData and a result */}
        {forfeitureAdjustmentData && profitYear && (
          <MissiveAlertProvider>
            <StandaloneMemberDetails
              memberType={1}
              id={forfeitureAdjustmentData.demographicId}
              profitYear={profitYear}
            />
          </MissiveAlertProvider>
        )}

        {forfeitureAdjustmentData && profitYear && (
          <Grid width="100%">
            <ForfeituresAdjustmentPanel
              initialSearchLoaded={initialSearchLoaded}
              setInitialSearchLoaded={setInitialSearchLoaded}
              onAddForfeiture={handleOpenAddForfeitureModal}
            />
          </Grid>
        )}
      </Grid>

      <AddForfeitureModal
        open={isAddForfeitureModalOpen}
        onClose={handleCloseAddForfeitureModal}
        onSave={handleSaveForfeiture}
        suggestedForfeitResponse={forfeitureAdjustmentData}
      />
    </Page>
  );
};

export default ForfeituresAdjustment;
