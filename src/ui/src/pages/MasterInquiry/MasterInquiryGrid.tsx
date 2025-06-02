import { Typography } from "@mui/material";
import { useState, useMemo, useCallback, useEffect } from "react";
import { useSelector } from "react-redux";
import { useLazySearchProfitMasterInquiryQuery, useLazyGetProfitMasterInquiryMemberDetailsQuery } from "reduxstore/api/InquiryApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetMasterInquiryGridColumns } from "./MasterInquiryGridColumns";
import { MasterInquiryRequest } from "reduxstore/types";
import { paymentTypeGetNumberMap, memberTypeGetNumberMap } from "./MasterInquiryFunctions";
import { CAPTIONS } from "../../constants";
interface MasterInquiryGridProps {
  initialSearchLoaded?: boolean;
  setInitialSearchLoaded?: (loaded: boolean) => void;
  memberType?: number;
  id?: number;
}

const MasterInquiryGrid: React.FC<MasterInquiryGridProps> = ({ initialSearchLoaded, setInitialSearchLoaded, memberType, id }) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [_sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "profitYear",
    isSortDescending: true
  });

  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const { masterInquiryData, masterInquiryRequestParams } = useSelector((state: RootState) => state.inquiry);
  const [triggerSearch, { isFetching }] = useLazySearchProfitMasterInquiryQuery();
  const [triggerMemberDetails, { data: memberDetailsData, isFetching: isFetchingMemberDetails }] = useLazyGetProfitMasterInquiryMemberDetailsQuery();

  useEffect(() => {
    if (memberType !== undefined && id !== undefined) {
      triggerMemberDetails({ memberType, id });
    }
  }, [memberType, id, triggerMemberDetails]);

  const createMasterInquiryRequest = useCallback(
    (skip: number, sortBy: string, isSortDescending: boolean): MasterInquiryRequest | null => {
      if (!masterInquiryRequestParams) return null;

      return {
        pagination: { skip, take: pageSize, sortBy, isSortDescending },
        ...(!!masterInquiryRequestParams.endProfitYear && {
          endProfitYear: masterInquiryRequestParams.endProfitYear
        }),
        ...(!!masterInquiryRequestParams.startProfitMonth && {
          startProfitMonth: masterInquiryRequestParams.startProfitMonth
        }),
        ...(!!masterInquiryRequestParams.endProfitMonth && {
          endProfitMonth: masterInquiryRequestParams.endProfitMonth
        }),
        ...(!!masterInquiryRequestParams.socialSecurity && {
          socialSecurity: masterInquiryRequestParams.socialSecurity
        }),
        ...(!!masterInquiryRequestParams.name && { name: masterInquiryRequestParams.name }),
        ...(!!masterInquiryRequestParams.badgeNumber && { badgeNumber: masterInquiryRequestParams.badgeNumber }),
        ...(!!masterInquiryRequestParams.comment && { comment: masterInquiryRequestParams.comment }),
        ...(!!masterInquiryRequestParams.paymentType && {
          paymentType: paymentTypeGetNumberMap[masterInquiryRequestParams.paymentType]
        }),
        ...(!!masterInquiryRequestParams.memberType && {
          memberType: memberTypeGetNumberMap[masterInquiryRequestParams.memberType]
        }),
        ...(!!masterInquiryRequestParams.contribution && { contribution: masterInquiryRequestParams.contribution }),
        ...(!!masterInquiryRequestParams.earnings && { earnings: masterInquiryRequestParams.earnings }),
        ...(!!masterInquiryRequestParams.forfeiture && { forfeiture: masterInquiryRequestParams.forfeiture }),
        ...(!!masterInquiryRequestParams.payment && { payment: masterInquiryRequestParams.payment })
      };
    },
    [masterInquiryRequestParams, pageSize, _sortParams]
  );

  const sortEventHandler = (update: ISortParams) => {
    if (update.sortBy === "") {
      update.sortBy = "profitYear";
      update.isSortDescending = true;
    }
    setSortParams(update);
    setPageNumber(0);

    const request = createMasterInquiryRequest(0, update.sortBy, update.isSortDescending);
    if (!request) return;

    triggerSearch(request, false);
  };

  const columnDefs = useMemo(() => GetMasterInquiryGridColumns(), []);

  const onSearch = useCallback(async () => {
    const request = createMasterInquiryRequest(pageNumber * pageSize, _sortParams.sortBy, _sortParams.isSortDescending);
    if (!request) return;

    await triggerSearch(request, false);
  }, [createMasterInquiryRequest, pageNumber, pageSize, _sortParams, triggerSearch]);

  useEffect(() => {
    if (initialSearchLoaded) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, _sortParams, onSearch]);

  if (memberType !== undefined && id !== undefined) {
    return (
      <>
        {isFetchingMemberDetails && <Typography>Loading member details...</Typography>}
        {memberDetailsData && (
          <DSMGrid
            preferenceKey={CAPTIONS.MASTER_INQUIRY}
            isLoading={isFetchingMemberDetails}
            providedOptions={{
              rowData: memberDetailsData.results,
              columnDefs: columnDefs,
              suppressMultiSort: true
            }}
          />
        )}
      </>
    );
  }

  return (
    <>
      {!!masterInquiryData && (
        <>
          <div className="master-inquiry-header">
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`Master Inquiry (${masterInquiryData?.inquiryResults.total || 0} ${masterInquiryData?.inquiryResults.total === 1 ? "Record" : "Records"})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={CAPTIONS.MASTER_INQUIRY}
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: masterInquiryData?.inquiryResults.results,
              columnDefs: columnDefs,
              suppressMultiSort: true
            }}
          />
        </>
      )}
      {!!masterInquiryData && masterInquiryData.inquiryResults.results.length > 0 && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => {
            setPageNumber(value - 1);
            setInitialSearchLoaded(true);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            setPageSize(value);
            setPageNumber(1);
            setInitialSearchLoaded(true);
          }}
          recordCount={masterInquiryData.inquiryResults.total}
        />
      )}
    </>
  );
};

export default MasterInquiryGrid;
