import React, { useState, useEffect } from "react";
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
    width: 120,
    cellRenderer: (params: any) => {
      const { badgeNumber, isEmployee, id } = params.data;
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
          {badgeNumber}
        </a>
      );
    }
  },
  { field: "firstName", headerName: "First Name", width: 160 },
  { field: "lastName", headerName: "Last Name", width: 160 },
  { field: "dateOfBirth", headerName: "Date of Birth", width: 140 }
];

interface MasterInquiryMemberGridProps extends MasterInquiryRequest {
  onBadgeClick?: (args: { memberType: number; id: number }) => void;
}

const MasterInquiryMemberGrid: React.FC<MasterInquiryMemberGridProps> = (searchParams) => {
  // If no searchParams, render nothing
  if (!searchParams || Object.keys(searchParams).length === 0) return null;

  const [request, setRequest] = useState<MasterInquiryRequest>(searchParams);
  const [trigger, { data, isLoading, isError }] = useLazySearchProfitMasterInquiryQuery();

  useEffect(() => {
    setRequest(searchParams);
  }, [searchParams]);

  useEffect(() => {
    trigger(request);
  }, [request, trigger]);

  const pageSize = request.pagination.take;
  const pageNumber = Math.floor(request.pagination.skip / request.pagination.take);

  return (
    <Box sx={{ width: "100%" }}>
      {isLoading && <CircularProgress />}
      {isError && <div>Error loading data.</div>}
      {data && (
        <>
          <DSMGrid
            isLoading={isLoading}
            preferenceKey="MASTER_INQUIRY_MEMBER_GRID"
            providedOptions={{
              rowData: Array.isArray(data?.results) ? data.results as EmployeeDetails[] : [],
              columnDefs: columns,
              context: { onBadgeClick: searchParams.onBadgeClick }
            }}
          />
          {Array.isArray(data?.results) && data.results.length > 0 && (
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
          )}
        </>
      )}
    </Box>
  );
};

export default MasterInquiryMemberGrid;