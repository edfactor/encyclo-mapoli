import { Divider } from "@mui/material";
import { Grid } from "@mui/material";
import { useState, useEffect } from "react";
import { useSelector } from "react-redux";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import ForfeituresAdjustmentGrid from "./ForfeituresAdjustmentGrid";
import ForfeituresAdjustmentSearchParameters from "./ForfeituresAdjustmentSearchParameters";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { RootState } from "reduxstore/store";
import AddForfeitureModal from "./AddForfeitureModal";
import { useLazyGetForfeitureAdjustmentsQuery } from "reduxstore/api/YearsEndApi";
import MasterInquiryEmployeeDetails from "pages/MasterInquiry/MasterInquiryEmployeeDetails";
import useDecemberFlowProfitYear from "../../hooks/useDecemberFlowProfitYear";

const ForfeituresAdjustment = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [isAddForfeitureModalOpen, setIsAddForfeitureModalOpen] = useState(false);
  const [pageNumberReset, setPageNumberReset] = useState(false);
  const { forfeitureAdjustmentData, forfeitureAdjustmentQueryParams } = useSelector(
    (state: RootState) => state.forfeituresAdjustment
  );
  const profitYear = useDecemberFlowProfitYear();
  const [triggerSearch] = useLazyGetForfeitureAdjustmentsQuery();

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
        {forfeitureAdjustmentData?.response?.results?.[0] && profitYear && (
          <MasterInquiryEmployeeDetails
            memberType={1}
            id={forfeitureAdjustmentData.response.results[0].demographicId}
            profitYear={profitYear}
          />
        )}

        <Grid width="100%">
          <ForfeituresAdjustmentGrid
            initialSearchLoaded={initialSearchLoaded}
            setInitialSearchLoaded={setInitialSearchLoaded}
            onAddForfeiture={handleOpenAddForfeitureModal}
            pageNumberReset={pageNumberReset}
            setPageNumberReset={setPageNumberReset}
          />
        </Grid>
      </Grid>

      <AddForfeitureModal
        open={isAddForfeitureModalOpen}
        onClose={handleCloseAddForfeitureModal}
        onSave={handleSaveForfeiture}
        employeeDetails={forfeitureAdjustmentData?.response?.results?.[0]}
      />
    </Page>
  );
};

export default ForfeituresAdjustment;
