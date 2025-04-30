import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { useState, useEffect } from "react";
import { useSelector } from "react-redux";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import ForfeituresAdjustmentGrid from "./ForfeituresAdjustmentGrid";
import ForfeituresAdjustmentSearchParameters from "./ForfeituresAdjustmentSearchParameters";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import MasterInquiryEmployeeDetails from "pages/MasterInquiry/MasterInquiryEmployeeDetails";
import { RootState } from "reduxstore/store";
import AddForfeitureModal from "./AddForfeitureModal";
import { useLazyGetForfeitureAdjustmentsQuery } from "reduxstore/api/YearsEndApi";
import { useLazyGetProfitMasterInquiryQuery } from "reduxstore/api/InquiryApi";

const ForfeituresAdjustment = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [showEmployeeDetails, setShowEmployeeDetails] = useState(false);
  const [isAddForfeitureModalOpen, setIsAddForfeitureModalOpen] = useState(false);
  const { forfeitureAdjustmentData, forfeitureAdjustmentQueryParams } = useSelector((state: RootState) => state.forfeituresAdjustment);
  const { masterInquiryEmployeeDetails } = useSelector((state: RootState) => state.inquiry);
  const [triggerSearch] = useLazyGetForfeitureAdjustmentsQuery();
  const [triggerMasterInquiry] = useLazyGetProfitMasterInquiryQuery();

  const renderActionNode = () => {
    return (
      <StatusDropdownActionNode />
    );
  };

  const handleSearchComplete = (loaded: boolean) => {
    setInitialSearchLoaded(loaded);
    setShowEmployeeDetails(loaded);
    
    // If we have results, get employee details
    if (loaded && forfeitureAdjustmentData?.response?.results && forfeitureAdjustmentData.response.results.length > 0) {
      const badgeNumber = forfeitureAdjustmentData.response.results[0].badgeNumber;
      fetchEmployeeDetails(badgeNumber);
    }
  };

  const fetchEmployeeDetails = (badgeNumber: number) => {
    // Call Master Inquiry API
    triggerMasterInquiry({
      badgeNumber,
      pagination: {
        take: 10,
        skip: 0,
        sortBy: "profitYear",
        isSortDescending: true
      }
    });
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

  // Reset employee details visibility when search results change
  useEffect(() => {
    if (forfeitureAdjustmentData) {
      setShowEmployeeDetails(true);
      
      // If we have results, get employee details
      if (forfeitureAdjustmentData.response?.results && forfeitureAdjustmentData.response.results.length > 0) {
        const badgeNumber = forfeitureAdjustmentData.response.results[0].badgeNumber;
        fetchEmployeeDetails(badgeNumber);
      }
    } else {
      setShowEmployeeDetails(false);
    }
  }, [forfeitureAdjustmentData]);

  return (
    <Page label={CAPTIONS.FORFEITURES_ADJUSTMENT} actionNode={renderActionNode()}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <ForfeituresAdjustmentSearchParameters setInitialSearchLoaded={handleSearchComplete} />
          </DSMAccordion>
        </Grid2>

        {showEmployeeDetails && masterInquiryEmployeeDetails && (
          <MasterInquiryEmployeeDetails details={masterInquiryEmployeeDetails} />
        )}

        <Grid2 width="100%">
          <ForfeituresAdjustmentGrid
            initialSearchLoaded={initialSearchLoaded}
            setInitialSearchLoaded={setInitialSearchLoaded}
            onAddForfeiture={handleOpenAddForfeitureModal}
          />
        </Grid2>
      </Grid2>

      <AddForfeitureModal
        open={isAddForfeitureModalOpen}
        onClose={handleCloseAddForfeitureModal}
        onSave={handleSaveForfeiture}
        employeeDetails={masterInquiryEmployeeDetails}
      />
    </Page>
  );
};

export default ForfeituresAdjustment;
