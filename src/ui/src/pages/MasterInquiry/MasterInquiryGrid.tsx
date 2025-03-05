import { Typography } from "@mui/material";
import { useState, useMemo, useCallback, useEffect } from "react";
import { useSelector } from "react-redux";
import { useLazyGetProfitMasterInquiryQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetMasterInquiryGridColumns } from "./MasterInquiryGridColumns";
import { MasterInquiryRequest } from "reduxstore/types";
import { paymentTypeMap, memberTypeMap } from "./MasterInquiryFunctions";
interface MasterInquiryGridProps {
  startProfitYearCurrent: Date | null;
  endProfitYearCurrent: Date | null;
  startProfitMonthCurrent: number | null;
  endProfitMonthCurrent: number | null;
  socialSecurityCurrent: number | null;
  nameCurrent: string | null;
  badgeNumberCurrent: number | null;
  commentCurrent: string | null;
  paymentTypeCurrent: "all" | "hardship" | "payoffs" | "rollovers";
  memberTypeCurrent: "all" | "employees" | "beneficiaries" | "none";
  contributionCurrent: number | null;
  earningsCurrent: number | null;
  forfeitureCurrent: number | null;
  paymentCurrent: number | null;
  //voidsCurrent: boolean;
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const MasterInquiryGrid: React.FC<MasterInquiryGridProps> = ({
  startProfitYearCurrent,
  endProfitYearCurrent,
  startProfitMonthCurrent,
  endProfitMonthCurrent,
  socialSecurityCurrent,
  nameCurrent,
  badgeNumberCurrent,
  commentCurrent,
  paymentTypeCurrent,
  memberTypeCurrent,
  contributionCurrent,
  earningsCurrent,
  forfeitureCurrent,
  paymentCurrent,
  //voidsCurrent,
  initialSearchLoaded,
  setInitialSearchLoaded
}) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [_sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const { masterInquiryData } = useSelector((state: RootState) => state.yearsEnd);
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [triggerSearch, { isFetching }] = useLazyGetProfitMasterInquiryQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetMasterInquiryGridColumns(), []);

  const onSearch = useCallback(async () => {
    const request: MasterInquiryRequest = {
      pagination: { skip: pageNumber * pageSize, take: pageSize },
      ...(!!startProfitYearCurrent && { startProfitYear: startProfitYearCurrent.getFullYear() }),
      ...(!!endProfitYearCurrent && { endProfitYear: endProfitYearCurrent.getFullYear() }),
      ...(!!startProfitMonthCurrent && { startProfitMonth: startProfitMonthCurrent }),
      ...(!!endProfitMonthCurrent && { endProfitMonth: endProfitMonthCurrent }),
      ...(!!socialSecurityCurrent && { socialSecurity: socialSecurityCurrent }),
      ...(!!nameCurrent && { name: nameCurrent }),
      ...(!!badgeNumberCurrent && { badgeNumber: badgeNumberCurrent }),
      ...(!!commentCurrent && { comment: commentCurrent }),
      ...(!!paymentTypeCurrent && { paymentType: paymentTypeMap[paymentTypeCurrent] }),
      ...(!!memberTypeCurrent && { memberType: memberTypeMap[memberTypeCurrent] }),
      ...(!!contributionCurrent && { contribution: contributionCurrent }),
      ...(!!earningsCurrent && { earnings: earningsCurrent }),
      ...(!!forfeitureCurrent && { forfeiture: forfeitureCurrent }),
      ...(!!paymentCurrent && { payment: paymentCurrent })
      //voids: voidsCurrent,
    };

    await triggerSearch(request, false);
  }, [
    startProfitYearCurrent,
    endProfitYearCurrent,
    startProfitMonthCurrent,
    endProfitMonthCurrent,
    socialSecurityCurrent,
    nameCurrent,
    badgeNumberCurrent,
    commentCurrent,
    paymentTypeCurrent,
    memberTypeCurrent,
    contributionCurrent,
    earningsCurrent,
    forfeitureCurrent,
    paymentCurrent,
    //voidsCurrent,
    pageNumber,
    pageSize,
    triggerSearch
  ]);

  useEffect(() => {
    if (initialSearchLoaded) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, onSearch]);

  return (
    <>
      {!!masterInquiryData && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`Master Inquiry (${masterInquiryData?.total || 0})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"DUPE_SSNS"}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: masterInquiryData?.results,
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {!!masterInquiryData && masterInquiryData.results.length > 0 && (
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
          recordCount={masterInquiryData.total}
        />
      )}
    </>
  );
};

export default MasterInquiryGrid;
