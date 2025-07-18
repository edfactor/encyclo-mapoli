import { Typography } from "@mui/material";
import { useMemo, useEffect, useState, useRef } from "react";
import { useLazyGetProfitMasterInquiryMemberDetailsQuery } from "reduxstore/api/InquiryApi";
import { DSMGrid, Pagination } from "smart-ui-library";
import { GetMasterInquiryGridColumns } from "./MasterInquiryGridColumns";
import { CAPTIONS } from "../../constants";

interface MasterInquiryGridProps {
  initialSearchLoaded?: boolean;
  setInitialSearchLoaded?: (loaded: boolean) => void;
  memberType?: number;
  id?: number;
}

const MasterInquiryGrid: React.FC<MasterInquiryGridProps> = ({ memberType, id }) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const columnDefs = useMemo(() => GetMasterInquiryGridColumns(), []);

  const [
    triggerMemberDetails,
    {
      data: memberDetailsData,
      isFetching: isFetchingMemberDetails,
      isError: isErrorMemberDetails,
      error: errorMemberDetails
    }
  ] = useLazyGetProfitMasterInquiryMemberDetailsQuery();

  useEffect(() => {
    if (id === undefined || memberType === undefined) return;
    triggerMemberDetails({
      memberType,
      id,
      skip: pageNumber * pageSize,
      take: pageSize,
      sortBy: "profitYear",
      isSortDescending: true
    });
  }, [memberType, id, pageNumber, pageSize, triggerMemberDetails]);

  // Need a useEffect to reset the page number when memberDetailsData changes
  const prevMemberDetailsData = useRef<any>(null);
  useEffect(() => {
    if (
      memberDetailsData &&
      (prevMemberDetailsData.current === undefined || memberDetailsData.total !== prevMemberDetailsData.current.total)
    ) {
      setPageNumber(0);
    }
    prevMemberDetailsData.current = memberDetailsData;
  }, [memberDetailsData]);

  if (isFetchingMemberDetails) {
    return <Typography>Loading profit details...</Typography>;
  }

  if (isErrorMemberDetails) {
    return (
      <Typography color="error">
        Error loading member details: {errorMemberDetails && JSON.stringify(errorMemberDetails)}
      </Typography>
    );
  }

  return (
    <>
      <div style={{ height: "400px", width: "100%" }}>
        {!!memberDetailsData && (
          <>
            <div style={{ padding: "0 24px 0 24px" }}>
              <Typography
                variant="h2"
                sx={{ color: "#0258A5" }}>
                {`Profit Details (${memberDetailsData?.total || 0} ${memberDetailsData?.total === 1 ? "Record" : "Records"})`}
              </Typography>
            </div>
            <DSMGrid
              preferenceKey={CAPTIONS.MASTER_INQUIRY}
              isLoading={isFetchingMemberDetails}
              providedOptions={{
                rowData: memberDetailsData?.results,
                columnDefs: columnDefs,
                suppressMultiSort: true
              }}
            />
          </>
        )}
        {!!memberDetailsData && memberDetailsData.results.length > 0 && (
          <Pagination
            pageNumber={pageNumber}
            setPageNumber={(value: number) => {
              setPageNumber(value - 1);
            }}
            pageSize={pageSize}
            setPageSize={(value: number) => {
              setPageSize(value);
              setPageNumber(0);
            }}
            recordCount={memberDetailsData.total}
          />
        )}
      </div>
    </>
  );
};
export default MasterInquiryGrid;
