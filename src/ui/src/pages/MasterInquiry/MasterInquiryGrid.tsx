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

const MasterInquiryGrid: React.FC<MasterInquiryGridProps> = ({ initialSearchLoaded, setInitialSearchLoaded, handleSortChanged }) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [_sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "ProfitYear",
    isSortDescending: false
  });

  const { masterInquiryData, masterInquiryRequestParams } = useSelector((state: RootState) => state.inquiry);
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [triggerSearch, { isFetching }] = useLazyGetProfitMasterInquiryQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetMasterInquiryGridColumns(), []);

  const onSearch = useCallback(async () => {
    if (!masterInquiryRequestParams) return;

    const request: MasterInquiryRequest = {
      pagination: { skip: pageNumber * pageSize, take: pageSize, sortBy: _sortParams.sortBy, isSortDescending: _sortParams.isSortDescending },
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
      //voids: voidsCurrent,
    };

    await triggerSearch(request, false);
  }, [pageNumber, pageSize, _sortParams, triggerSearch, masterInquiryRequestParams]);

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
            preferenceKey={"DUPE_SSNS"}
            isLoading={false}
            handleSortChanged={handleSortChanged}
            providedOptions={{
              rowData: masterInquiryData?.inquiryResults.results,
              columnDefs: columnDefs
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
