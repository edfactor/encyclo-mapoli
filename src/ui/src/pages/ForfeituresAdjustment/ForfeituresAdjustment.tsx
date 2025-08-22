import { Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import StandaloneMemberDetails from "pages/MasterInquiry/StandaloneMemberDetails";
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetProfitMasterInquiryMemberQuery } from "reduxstore/api/InquiryApi";
import { useLazyGetForfeitureAdjustmentsQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import {
  ApiMessageAlert,
  DSMAccordion,
  formatNumberWithComma,
  MessageUpdate,
  Page,
  setMessage
} from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import useDecemberFlowProfitYear from "../../hooks/useDecemberFlowProfitYear";
import { InquiryApi } from "../../reduxstore/api/InquiryApi";
import { clearForfeitureAdjustmentData } from "../../reduxstore/slices/forfeituresAdjustmentSlice";
import { MissiveAlertProvider } from "../MasterInquiry/utils/MissiveAlertContext";
import AddForfeitureModal from "./AddForfeitureModal";
import ForfeituresAdjustmentPanel from "./ForfeituresAdjustmentPanel";
import ForfeituresAdjustmentSearchParameters from "./ForfeituresAdjustmentSearchParameters";

enum MessageKeys {
  ForfeituresAdjustment = "ForfeituresAdjustment"
}

export class Messages {
  static readonly ForfeituresSaveSuccess: MessageUpdate = {
    key: MessageKeys.ForfeituresAdjustment,
    message: {
      type: "success",
      title: "Forfeiture saved successfully",
      message: `A forfeiture with the amount $X for Y saved successfully`
    }
  };
}

const ForfeituresAdjustment = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [isAddForfeitureModalOpen, setIsAddForfeitureModalOpen] = useState(false);
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
                message: `A forfeiture with the amount $${formatNumberWithComma(formData.forfeitureAmount)} for ${employeeName} saved successfully`
              }
            })
          );
        })
        .catch((error: unknown) => {
          console.error("Error refreshing forfeiture adjustments:", error);
        });
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
