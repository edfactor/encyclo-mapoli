import { Box, Typography } from "@mui/material";
import React, { useMemo } from "react";
import { EmployeeDetails } from "reduxstore/types";
import { DSMGrid, formatNumberWithComma } from "smart-ui-library";
import Pagination from "../../components/Pagination/Pagination";
import { GetMasterInquiryMemberGridColumns } from "./MasterInquiryMemberGridColumns";

interface SearchResponse {
  results: EmployeeDetails[];
  total: number;
}

interface SelectedMember {
  memberType: number;
  id: number;
  ssn: number;
  badgeNumber: number;
  psnSuffix: number;
}

interface MasterInquiryMemberGridProps {
  searchResults: SearchResponse;
  onMemberSelect: (member: SelectedMember) => void;
  memberGridPagination: {
    pageNumber: number;
    pageSize: number;
    sortParams: any;
  };
  onPaginationChange: (pageNumber: number, pageSize: number) => void;
  onSortChange: (sortParams: any) => void;
}

const MasterInquiryMemberGrid: React.FC<MasterInquiryMemberGridProps> = ({
  searchResults,
  onMemberSelect,
  memberGridPagination,
  onPaginationChange,
  onSortChange
}: MasterInquiryMemberGridProps) => {
  const columns = useMemo(() => GetMasterInquiryMemberGridColumns(), []);

  const handleMemberClick = (member: EmployeeDetails) => {
    onMemberSelect({
      memberType: member.isEmployee ? 1 : 2,
      id: Number(member.id),
      ssn: Number(member.ssn),
      badgeNumber: Number(member.badgeNumber),
      psnSuffix: Number(member.psnSuffix)
    });
  };

  const handlePaginationChange = (pageNumber: number, pageSize: number) => {
    onPaginationChange(pageNumber, pageSize);
  };

  const handleSortChange = (sortParams: any) => {
    onSortChange(sortParams);
  };

  return (
    <Box sx={{ width: "100%" }}>
      <div style={{ padding: "0 24px 0 24px" }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          {`Search Results (${formatNumberWithComma(searchResults.total)} ${searchResults.total === 1 ? "Record" : "Records"})`}
        </Typography>
      </div>
      <DSMGrid
        preferenceKey="MASTER_INQUIRY_MEMBER_GRID"
        handleSortChanged={handleSortChange}
        isLoading={false}
        providedOptions={{
          rowData: searchResults.results,
          columnDefs: columns,
          context: { onBadgeClick: handleMemberClick },
          onRowClicked: (event) => {
            if (event.data) {
              handleMemberClick(event.data);
            }
          }
        }}
      />
      <Pagination
        rowsPerPageOptions={[5, 10, 50]}
        pageNumber={memberGridPagination.pageNumber}
        setPageNumber={(value: number) => {
          handlePaginationChange(value - 1, memberGridPagination.pageSize);
        }}
        pageSize={memberGridPagination.pageSize}
        setPageSize={(value: number) => {
          handlePaginationChange(0, value);
        }}
        recordCount={searchResults.total}
      />
    </Box>
  );
};

export default MasterInquiryMemberGrid;
