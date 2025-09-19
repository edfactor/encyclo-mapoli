import { Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import StandaloneMemberDetails from "pages/MasterInquiry/StandaloneMemberDetails";
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetProfitMasterInquiryMemberQuery } from "reduxstore/api/InquiryApi";
import { useLazyGetForfeitureAdjustmentsQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { ApiMessageAlert, DSMAccordion, formatNumberWithComma, Page, setMessage } from "smart-ui-library";
import { MessageKeys, Messages } from "utils/messageDictonary";
import { MissiveAlertProvider } from "../../components/MissiveAlerts/MissiveAlertContext";
import MissiveAlerts from "../../components/MissiveAlerts/MissiveAlerts";
import { CAPTIONS } from "../../constants";
import useDecemberFlowProfitYear from "../../hooks/useDecemberFlowProfitYear";
import { InquiryApi } from "../../reduxstore/api/InquiryApi";
import { clearForfeitureAdjustmentData } from "../../reduxstore/slices/forfeituresAdjustmentSlice";
import AddForfeitureModal from "./AddForfeitureModal";
import AddUnforfeitModal from "./AddUnforfeitModal";
import ForfeituresAdjustmentPanel from "./ForfeituresAdjustmentPanel";
import ForfeituresAdjustmentSearchParameters from "./ForfeituresAdjustmentSearchParameters";

const ForfeituresAdjustment = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [isAddForfeitureModalOpen, setIsAddForfeitureModalOpen] = useState(false);
  const [isAddUnforfeitModalOpen, setIsAddUnforfeitModalOpen] = useState(false);
  const [pageNumberReset, setPageNumberReset] = useState(false);
  const { forfeitureAdjustmentData, forfeitureAdjustmentQueryParams } = useSelector(
    (state: RootState) => state.forfeituresAdjustment
  );
  const profitYear = useDecemberFlowProfitYear();
  const [triggerSearch] = useLazyGetForfeitureAdjustmentsQuery();
  const [triggerMemberDetails] = useLazyGetProfitMasterInquiryMemberQuery();
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

  const handleOpenAddUnforfeitModal = () => {
    setIsAddUnforfeitModalOpen(true);
  };

  const handleCloseAddUnforfeitModal = () => {
    setIsAddUnforfeitModalOpen(false);
  };

  const handleSaveForfeiture = (formData: { forfeitureAmount: number; classAction: boolean }) => {
    if (forfeitureAdjustmentQueryParams && forfeitureAdjustmentData) {
      // Store the demographic ID to fetch employee details
      const demographicId = forfeitureAdjustmentData.demographicId;

      dispatch(clearForfeitureAdjustmentData());

      triggerSearch(forfeitureAdjustmentQueryParams)
        .unwrap()
        .then(() => {
          setInitialSearchLoaded(true);
          dispatch(InquiryApi.util.invalidateTags(["memberDetails"]));

          // Fetch employee details to get the name for the success message
          return triggerMemberDetails({
            memberType: 1,
            id: demographicId,
            profitYear: profitYear
          }).unwrap();
        })
        .then((memberDetails) => {
          const employeeName =
            memberDetails.firstName && memberDetails.lastName
              ? `${memberDetails.firstName} ${memberDetails.lastName}`
              : "the selected employee";

          dispatch(
            setMessage({
              ...Messages.ForfeituresSaveSuccess,
              message: {
                ...Messages.ForfeituresSaveSuccess.message,
                message: `The forfeiture of amount $${formatNumberWithComma(formData.forfeitureAmount)} for ${employeeName} saved successfully`
              }
            })
          );
        })
        .catch((error: unknown) => {
          console.error("Error refreshing forfeiture adjustments:", error);
        });
    }
  };

  const handleSaveUnforfeit = (formData: { forfeitureAmount: number; classAction: boolean }) => {
    // Use the same logic as forfeiture save since the API endpoint is the same
    // The amount will already be negative from the modal
    handleSaveForfeiture(formData);
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
    <MissiveAlertProvider>
      <Page
        label={CAPTIONS.FORFEITURES_ADJUSTMENT}
        actionNode={renderActionNode()}>
        <div>
          <ApiMessageAlert commonKey={MessageKeys.ForfeituresAdjustment} />
        </div>
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
          <MissiveAlerts />

          {forfeitureAdjustmentData && profitYear && (
            <StandaloneMemberDetails
              memberType={1}
              id={forfeitureAdjustmentData.demographicId}
              profitYear={profitYear}
            />
          )}

          {forfeitureAdjustmentData && profitYear && (
            <Grid width="100%">
              <ForfeituresAdjustmentPanel
                initialSearchLoaded={initialSearchLoaded}
                setInitialSearchLoaded={setInitialSearchLoaded}
                onAddForfeiture={handleOpenAddForfeitureModal}
                onAddUnforfeit={handleOpenAddUnforfeitModal}
                suggestedForfeitAmount={forfeitureAdjustmentData.suggestedForfeitAmount}
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

        <AddUnforfeitModal
          open={isAddUnforfeitModalOpen}
          onClose={handleCloseAddUnforfeitModal}
          onSave={handleSaveUnforfeit}
          suggestedForfeitResponse={forfeitureAdjustmentData}
        />
      </Page>
    </MissiveAlertProvider>
  );
};

export default ForfeituresAdjustment;
