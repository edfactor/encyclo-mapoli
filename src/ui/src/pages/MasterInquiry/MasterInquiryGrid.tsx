import { Typography } from "@mui/material";
import { useState, useMemo, useCallback, useEffect } from "react";
import { useSelector } from "react-redux";
import { useLazyGetProfitMasterInquiryQuery } from "reduxstore/api/InquiryApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetMasterInquiryGridColumns } from "./MasterInquiryGridColumns";
import { MasterInquiryRequest } from "reduxstore/types";
import { paymentTypeGetNumberMap, memberTypeGetNumberMap } from "./MasterInquiryFunctions";
interface MasterInquiryGridProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  handleSortChanged: (sort: ISortParams) => void;
}

const MasterInquiryGrid: React.FC<MasterInquiryGridProps> = ({ initialSearchLoaded, setInitialSearchLoaded }) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [_sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "profitYear",
    isSortDescending: false
  });

  const { masterInquiryData, masterInquiryRequestParams } = useSelector((state: RootState) => state.inquiry);
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [triggerSearch, { isFetching }] = useLazyGetProfitMasterInquiryQuery();

  const createMasterInquiryRequest = (skip: number, sortBy: string, isSortDescending: boolean): MasterInquiryRequest | null => {
    if (!masterInquiryRequestParams) return null;

    return {
      pagination: { skip, take: pageSize, sortBy, isSortDescending },
      ...(!!masterInquiryRequestParams.startProfitYear && {
        startProfitYear: masterInquiryRequestParams.startProfitYear.getFullYear()
      }),
      ...(!!masterInquiryRequestParams.endProfitYear && {
        endProfitYear: masterInquiryRequestParams.endProfitYear.getFullYear()
      }),
      ...(!!masterInquiryRequestParams.startProfitMonth && {
        startProfitMonth: masterInquiryRequestParams.startProfitMonth
      }),
      ...(!!masterInquiryRequestParams.endProfitMonth && { endProfitMonth: masterInquiryRequestParams.endProfitMonth }),
      ...(!!masterInquiryRequestParams.socialSecurity && { socialSecurity: masterInquiryRequestParams.socialSecurity }),
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
  };

  const sortEventHandler = (update: ISortParams) => {
    
    if ( update.sortBy === "" )
    {
      update.sortBy = "profitYear";
      update.isSortDescending = false;
    }
    setSortParams(update);
    setPageNumber(0); 
    
    const request = createMasterInquiryRequest(0, update.sortBy, update.isSortDescending);
    if (!request) return;

    triggerSearch(request, false);
  };

  const columnDefs = useMemo(() => GetMasterInquiryGridColumns(), []);

  const onSearch = useCallback(async () => {
    const request = createMasterInquiryRequest(
      pageNumber * pageSize, 
      _sortParams.sortBy, 
      _sortParams.isSortDescending
    );
    if (!request) return;

    await triggerSearch(request, false);
  }, [createMasterInquiryRequest, pageNumber, pageSize, _sortParams, triggerSearch]);

  useEffect(() => {
    if (initialSearchLoaded) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, _sortParams, onSearch]);

  return (
    <>
      {!!masterInquiryData && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`Master Inquiry (${masterInquiryData?.inquiryResults.total || 0})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"ProfitYear"}
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
