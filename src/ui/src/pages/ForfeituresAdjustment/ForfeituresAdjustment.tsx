import { Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import StandaloneMemberDetails from "pages/MasterInquiry/StandaloneMemberDetails";
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  useLazyGetProfitMasterInquiryMemberDetailsQuery,
  useLazyGetProfitMasterInquiryMemberQuery
} from "reduxstore/api/InquiryApi";
import { useLazyGetForfeitureAdjustmentsQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { ApiMessageAlert, DSMAccordion, formatNumberWithComma, Page, setMessage } from "smart-ui-library";
import { MessageKeys, Messages } from "utils/messageDictonary";
import { MissiveAlertProvider } from "../../components/MissiveAlerts/MissiveAlertContext";
import MissiveAlerts from "../../components/MissiveAlerts/MissiveAlerts";
import { CAPTIONS } from "../../constants";
import useDecemberFlowProfitYear from "../../hooks/useDecemberFlowProfitYear";
import { SortParams, useGridPagination } from "../../hooks/useGridPagination";
import { useReadOnlyNavigation } from "../../hooks/useReadOnlyNavigation";
import { InquiryApi } from "../../reduxstore/api/InquiryApi";
import { clearForfeitureAdjustmentData } from "../../reduxstore/slices/forfeituresAdjustmentSlice";
import { MasterInquiryResponseDto } from "../../types/master-inquiry/master-inquiry";
import AddForfeitureModal from "./AddForfeitureModal";
import ForfeituresAdjustmentPanel from "./ForfeituresAdjustmentPanel";
import ForfeituresAdjustmentSearchFilter from "./ForfeituresAdjustmentSearchFilter";
import ForfeituresTransactionGrid from "./ForfeituresTransactionGrid";

interface TransactionData {
  results: MasterInquiryResponseDto[];
  total: number;
}

const ForfeituresAdjustment = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [isAddForfeitureModalOpen, setIsAddForfeitureModalOpen] = useState(false);
  //const [pageNumberReset, setPageNumberReset] = useState(false);
  const [transactionData, setTransactionData] = useState<TransactionData | null>(null);
  const { forfeitureAdjustmentData, forfeitureAdjustmentQueryParams } = useSelector(
    (state: RootState) => state.forfeituresAdjustment
  );
  const profitYear = useDecemberFlowProfitYear();
  const isReadOnly = useReadOnlyNavigation();
  const [triggerSearch] = useLazyGetForfeitureAdjustmentsQuery();
  const [triggerMemberDetails] = useLazyGetProfitMasterInquiryMemberQuery();
  const [triggerTransactionDetails, { isLoading: isLoadingTransactions }] =
    useLazyGetProfitMasterInquiryMemberDetailsQuery();
  const dispatch = useDispatch();

  const handleTransactionGridPaginationChange = (pageNumber: number, pageSize: number, sortParams: SortParams) => {
    fetchTransactionDetails(pageNumber, pageSize, sortParams);
  };

  const transactionGridPagination = useGridPagination({
    initialPageSize: 50,
    initialSortBy: "profitYear",
    initialSortDescending: true,
    onPaginationChange: handleTransactionGridPaginationChange
  });

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

  const fetchTransactionDetails = (pageNumber = 0, pageSize = 50, sortParams: SortParams | null = null) => {
    if (forfeitureAdjustmentData) {
      const apiParams = {
        memberType: 1,
        id: forfeitureAdjustmentData.demographicId,
        skip: pageNumber * pageSize,
        take: pageSize,
        sortBy: sortParams?.sortBy,
        isSortDescending: sortParams?.isSortDescending
      };

      triggerTransactionDetails(apiParams)
        .unwrap()
        .then((response) => {
          // Filter for "Outgoing forfeitures" transactions (profit code 2)
          const filteredResults = response.results.filter((transaction) => {
            return transaction.profitCodeId === 2;
          });

          console.log(
            `Filtered ${filteredResults.length} forfeit transactions from ${response.results.length} total transactions`
          );

          setTransactionData({
            results: filteredResults,
            total: filteredResults.length
          });
        })
        .catch((error) => {
          console.error("Error fetching transaction details:", error);
          setTransactionData(null);
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

  useEffect(() => {
    if (forfeitureAdjustmentData && initialSearchLoaded) {
      fetchTransactionDetails(
        transactionGridPagination.pageNumber,
        transactionGridPagination.pageSize,
        transactionGridPagination.sortParams
      );
    }
  }, [forfeitureAdjustmentData, initialSearchLoaded]);

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
              <ForfeituresAdjustmentSearchFilter setInitialSearchLoaded={handleSearchComplete} />
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
                isReadOnly={isReadOnly}
              />
            </Grid>
          )}

          {forfeitureAdjustmentData && profitYear && initialSearchLoaded && (
            <Grid width="100%">
              <ForfeituresTransactionGrid
                transactionData={transactionData}
                isLoading={isLoadingTransactions}
                onPaginationChange={transactionGridPagination.handlePaginationChange}
                onSortChange={transactionGridPagination.handleSortChange}
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
    </MissiveAlertProvider>
  );
};

export default ForfeituresAdjustment;
