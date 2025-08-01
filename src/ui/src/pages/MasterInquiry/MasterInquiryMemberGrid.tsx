import { Box, CircularProgress, Typography } from "@mui/material";
import React, { useCallback, useEffect, useRef, useState, useMemo } from "react";
import { useLazySearchProfitMasterInquiryQuery } from "reduxstore/api/InquiryApi";
import { EmployeeDetails, MasterInquiryRequest } from "reduxstore/types";
import { DSMGrid, formatNumberWithComma, ISortParams } from "smart-ui-library";
import Pagination from "../../components/Pagination/Pagination";
import "./MasterInquiryMemberGrid.css"; // Import the CSS file for styles
import { GetMasterInquiryMemberGridColumns } from "./MasterInquiryMemberGridColumns";
import { isSimpleSearch } from "./MasterInquiryFunctions";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";

interface MasterInquiryMemberGridProps extends MasterInquiryRequest {
  searchParams: MasterInquiryRequest;
  onBadgeClick: (
    args: { memberType: number; id: number; ssn: number; badgeNumber: number; psnSuffix: number } | undefined
  ) => void;
}

const MasterInquiryMemberGrid: React.FC<MasterInquiryMemberGridProps> = ({
  searchParams,
  onBadgeClick
}: MasterInquiryMemberGridProps) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(5);
  // Add sort state management
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: searchParams.pagination?.sortBy || "badgeNumber",
    isSortDescending: searchParams.pagination?.isSortDescending || false
  });
  const [trigger, { data, isLoading, isError }] = useLazySearchProfitMasterInquiryQuery();
  const autoSelectedRef = useRef<number | null>(null);

  const { masterInquiryRequestParams } = useSelector((state: RootState) => state.inquiry);

  // Add sort event handler
  const sortEventHandler = (update: ISortParams) => {
    setSortParams(update);
    setPageNumber(0); // Reset to first page when sorting
  };

  const onSearch = useCallback(async () => {
    // We are going to do another search here which skips zero and takes all.

    await trigger({
      ...searchParams,
      pagination: {
        skip: pageNumber * pageSize,
        take: pageSize,
        sortBy: sortParams.sortBy,
        isSortDescending: sortParams.isSortDescending
      }
    });
  }, [pageNumber, pageSize, sortParams, searchParams, trigger]);

  useEffect(() => {
    onSearch();
  }, [onSearch]);

  // If only one member is returned, auto-select and hide the grid
  useEffect(() => {
    if (data && data.results.length === 1 && onBadgeClick && autoSelectedRef.current !== data.results[0].id) {
      const member = data.results[0];
      onBadgeClick({
        memberType: member.isEmployee ? 1 : 2,
        id: Number(member.id),
        ssn: Number(member.ssn),
        badgeNumber: Number(member.badgeNumber),
        psnSuffix: Number(member.psnSuffix)
      });
      autoSelectedRef.current = member.id;
    }
    // If no results in a complex search, clear selection
    // For simple searches, don't clear selection to allow "Member Not Found" message to show
    if (data && data.results.length === 0 && onBadgeClick && !isSimpleSearch(masterInquiryRequestParams)) {
      onBadgeClick(undefined);
    }
  }, [data, onBadgeClick]);

  const columns = useMemo(() => GetMasterInquiryMemberGridColumns(), []);

  // If no searchParams, render nothing
  if (!searchParams || Object.keys(searchParams).length === 0) {
    return null;
  }

  // Show a message if no results
  if (!isSimpleSearch(masterInquiryRequestParams) && data && data.results.length === 0) {
    return (
      <Box sx={{ width: "100%", padding: "24px" }}>
        <Typography
          color="error"
          variant="h6">
          No results found.
        </Typography>
      </Box>
    );
  }

  // Hide the grid if only one member is returned
  // But if the last page returns one result, we still want to show the grid
  // so we check the total number of results to make sure it's 1 also
  if (data && data.results.length === 1 && data.total === 1) {
    return null;
  }

  return (
    <Box sx={{ width: "100%" }}>
      {isLoading && <CircularProgress />}
      {isError && <div>Error loading data.</div>}
      {data && data.results.length > 0 && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`Search Results (${formatNumberWithComma(data?.total) || 0} ${data?.total === 1 ? "Record" : "Records"})`}
            </Typography>
          </div>
          <DSMGrid
            isLoading={isLoading}
            preferenceKey="MASTER_INQUIRY_MEMBER_GRID"
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: Array.isArray(data?.results) ? (data.results as EmployeeDetails[]) : [],
              columnDefs: columns,
              context: { onBadgeClick: onBadgeClick }
            }}
          />
          <Pagination
            rowsPerPageOptions={[5, 10, 50]}
            pageNumber={pageNumber}
            setPageNumber={(value: number) => {
              setPageNumber(value - 1);
            }}
            pageSize={pageSize}
            setPageSize={(value: number) => {
              setPageSize(value);
              setPageNumber(1);
            }}
            recordCount={(() => {
              return data.total;
            })()}
          />
        </>
      )}
    </Box>
  );
};

export default MasterInquiryMemberGrid;
