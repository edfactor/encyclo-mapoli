import { Box, CircularProgress, Typography } from "@mui/material";
import React, { useEffect, useRef, useState } from "react";
import { useLazySearchProfitMasterInquiryQuery } from "reduxstore/api/InquiryApi";
import { EmployeeDetails, MasterInquiryRequest } from "reduxstore/types";
import { DSMGrid, formatNumberWithComma } from "smart-ui-library";
import Pagination from "../../components/Pagination/Pagination";
import './MasterInquiryMemberGrid.css'; // Import the CSS file for styles
import { GetMasterInquiryMemberGridColumns } from "./MasterInquiryMemberGridColumns";



interface MasterInquiryMemberGridProps extends MasterInquiryRequest {
  onBadgeClick?: (args: { memberType: number; id: number, ssn: number, badgeNumber:number, psnSuffix:number } | undefined) => void;
}

const MasterInquiryMemberGrid: React.FC<MasterInquiryMemberGridProps> = (searchParams) => {
  
  const [request, setRequest] = useState<MasterInquiryRequest>(searchParams);
  
  const [trigger, { data, isLoading, isError }] = useLazySearchProfitMasterInquiryQuery();
  const autoSelectedRef = useRef<number | null>(null);

  useEffect(() => {
    setRequest(searchParams);
  }, [searchParams]);

  useEffect(() => {
    trigger(request);
  }, [request, trigger]);

  // If only one member is returned, auto-select and hide the grid
  useEffect(() => {
    if (
      data &&
      data.results.length === 1 &&
      searchParams.onBadgeClick &&
      autoSelectedRef.current !== data.results[0].id
    ) {
      const member = data.results[0];
      searchParams.onBadgeClick({
        memberType: member.isEmployee ? 1 : 2,
        id: Number(member.id),
        ssn: Number(member.ssn),
        badgeNumber: Number(member.badgeNumber),
        psnSuffix: Number(member.psnSuffix)
      });
      autoSelectedRef.current = member.id;
    }
    // If no results, clear selection
    if (data && data.results.length === 0 && searchParams.onBadgeClick) {
      searchParams.onBadgeClick(undefined);
    }
  }, [data, searchParams]);

  // If no searchParams, render nothing
  if (!searchParams || Object.keys(searchParams).length === 0) return null;

  const pageSize = request.pagination.take;
  const pageNumber = Math.floor(request.pagination.skip / request.pagination.take);

  // Show a message if no results
  if (data && data.results.length === 0) {
    return (
      <Box sx={{ width: "100%", padding: "24px" }}>
        <Typography color="error" variant="h6">No results found.</Typography>
      </Box>
    );
  }

  const columns = GetMasterInquiryMemberGridColumns();

  // Hide the grid if only one member is returned
  if (data && data.results.length === 1) {
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
            providedOptions={{
              rowData: Array.isArray(data?.results) ? data.results as EmployeeDetails[] : [],
              columnDefs: columns,
              context: { onBadgeClick: searchParams.onBadgeClick }
            }}
          />
          <Pagination
            rowsPerPageOptions={[5, 10, 50]}
            pageNumber={pageNumber}
            setPageNumber={(value: number) => {
              setRequest((prev) => ({
 
                ...prev,
                pagination: {
                  ...prev.pagination,
                  skip: (value - 1) * prev.pagination.take
                }
              }));
            }}
            pageSize={pageSize}
            setPageSize={(value: number) => {
              setRequest((prev) => ({
                ...prev,
                pagination: {
                  ...prev.pagination,
                  take: value,
                  skip: 0
                }
              }));
            }}
            recordCount={data.total}
          />
        </>
      )}
    </Box>
  );
};

export default MasterInquiryMemberGrid;