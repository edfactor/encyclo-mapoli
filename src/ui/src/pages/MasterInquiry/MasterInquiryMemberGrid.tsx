import React, { useState, useEffect } from "react";
import { useLazySearchProfitMasterInquiryQuery } from "reduxstore/api/InquiryApi";
import { MasterInquiryRequest, EmployeeDetails } from "reduxstore/types";
import { DSMGrid, Pagination } from "smart-ui-library";
import { Box, CircularProgress } from "@mui/material";
import { ColDef } from "ag-grid-community";

const columns: ColDef[] = [
  { field: "badgeNumber", headerName: "Badge #", width: 120 },
  { field: "firstName", headerName: "First Name", width: 160 },
  { field: "lastName", headerName: "Last Name", width: 160 },
  { field: "dateOfBirth", headerName: "Date of Birth", width: 140 }
];

const MasterInquiryMemberGrid: React.FC<MasterInquiryRequest> = (searchParams) => {
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
            providedOptions={{
              rowData: Array.isArray(data?.results) ? data.results as EmployeeDetails[] : [],
              columnDefs: columns
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