import { Typography } from "@mui/material";
import React, { useState, useEffect, useRef } from "react";
import { useLazySearchProfitMasterInquiryQuery } from "reduxstore/api/InquiryApi";
import { MasterInquiryRequest, EmployeeDetails } from "reduxstore/types";
import { DSMGrid, Pagination } from "smart-ui-library";
import { Box, CircularProgress } from "@mui/material";
import { ColDef } from "ag-grid-community";
import './MasterInquiryMemberGrid.css'; // Import the CSS file for styles

const columns: ColDef[] = [
  {
    field: "badgeNumber",
    headerName: "Badge #",
    maxWidth: 120,
    cellRenderer: (params: any) => {
      const { badgeNumber, psnSuffix, isEmployee, id } = params.data;
      return (
        <a
          href="#"
          className="badge-link"
          onClick={e => {
            e.preventDefault();
            if (params.context && params.context.onBadgeClick) {
              params.context.onBadgeClick({ memberType: isEmployee ? 1 : 2, id });
            }
          }}
        >
          {psnSuffix > 0 ? `${badgeNumber}-${psnSuffix}` : badgeNumber}
        </a>
      );
    }
  },
  { field: "fullName", headerName: "Name", width: 500 },
  { field: "ssn", headerName: "SSN", maxWidth: 250 },
  { field: "address", headerName: "Street", maxWidth: 400 },
  { field: "addressCity", headerName: "City", maxWidth: 300 },
  { field: "addressState", headerName: "State", maxWidth: 100 },
  { field: "addressZipCode", headerName: "Zip", maxWidth: 160 },
  { field: "age", headerName: "Age", maxWidth: 120, },
  { field: "employmentStatus", headerName: "Status", maxWidth: 120, },
];

interface MasterInquiryMemberGridProps extends MasterInquiryRequest {
  onBadgeClick?: (args: { memberType: number; id: number, ssn: number }) => void;
}

const MasterInquiryMemberGrid: React.FC<MasterInquiryMemberGridProps> = (searchParams) => {
  // If no searchParams, render nothing
  if (!searchParams || Object.keys(searchParams).length === 0) return null;

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
        id: member.id,
        ssn: member.ssn
      });
      autoSelectedRef.current = member.id;
    }
    // If no results, clear selection
    if (data && data.results.length === 0 && searchParams.onBadgeClick) {
      searchParams.onBadgeClick(undefined);
    }
  }, [data, searchParams]);

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
              {`Search Results (${data?.total || 0} ${data?.total === 1 ? "Record" : "Records"})`}
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
            pageNumber={pageNumber}
            setPageNumber={(value: number) => {
              setRequest((prev) => ({
                ...prev,
                pagination: {
                  ...prev.pagination,
                  skip: value * prev.pagination.take
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