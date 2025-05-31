import React, { useState, useEffect } from "react";
import { useLazySearchProfitMasterInquiryQuery } from "reduxstore/api/InquiryApi";
import { MasterInquiryRequest, EmployeeDetails } from "reduxstore/types";
import { DSMGrid } from "smart-ui-library";
import { Box, CircularProgress } from "@mui/material";
import { GridColDef } from "@mui/x-data-grid";

const columns: GridColDef[] = [
  { field: "badgeNumber", headerName: "Badge #", width: 120 },
  { field: "firstName", headerName: "First Name", width: 160 },
  { field: "lastName", headerName: "Last Name", width: 160 },
  { field: "dateOfBirth", headerName: "Date of Birth", width: 140 }
];

const defaultRequest: MasterInquiryRequest = {
  pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: false }
};

const MasterInquiryMemberGrid: React.FC = () => {
  const [request, setRequest] = useState<MasterInquiryRequest>(defaultRequest);
  const [trigger, { data, isLoading, isError }] = useLazySearchProfitMasterInquiryQuery();

  useEffect(() => {
    trigger(request);
  }, [request, trigger]);

  return (
    <Box sx={{ width: "100%" }}>
      {isLoading && <CircularProgress />}
      {isError && <div>Error loading data.</div>}
      {data && (
        <DSMGrid
          providedOptions={{
            rowData: data.response.data as EmployeeDetails[],
            columnDefs: columns
          }}
          totalRowCount={data.response.total}
          pageSize={request.pagination.take}
          page={Math.floor(request.pagination.skip / request.pagination.take)}
          paginationMode="server"
          onPageChange={(page: number) => {
            setRequest((prev) => ({
              ...prev,
              pagination: {
                ...prev.pagination,
                skip: page * prev.pagination.take
              }
            }));
          }}
          onPageSizeChange={(pageSize: number) => {
            setRequest((prev) => ({
              ...prev,
              pagination: {
                ...prev.pagination,
                take: pageSize,
                skip: 0
              }
            }));
          }}
        />
      )}
    </Box>
  );
};

export default MasterInquiryMemberGrid;