import { Box, Typography } from "@mui/material";
import { RowClickedEvent } from "ag-grid-community";
import React, { memo, useMemo } from "react";
import { DSMGrid, formatNumberWithComma, Pagination, ISortParams } from "smart-ui-library";
import { EmployeeDetails } from "../../../reduxstore/types";
import { SortParams } from "../../../hooks/useGridPagination";
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
    sortParams: SortParams;
  };
  onPaginationChange: (pageNumber: number, pageSize: number) => void;
  onSortChange: (sortParams: SortParams) => void;
  isLoading?: boolean;
}

const MasterInquiryMemberGrid: React.FC<MasterInquiryMemberGridProps> = memo(
  ({
    searchResults,
    onMemberSelect,
    memberGridPagination,
    onPaginationChange,
    onSortChange,
    isLoading = false
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

    const handleSortChange = (sortParams: ISortParams) => {
      onSortChange(sortParams);
    };

    return (
      <Box sx={{ width: "100%", paddingTop: "24px" }}>
        {/* CSS for badge link styling */}
        <style>
          {`
            .badge-link-style {
              color: #0258A5 !important;
              text-decoration: underline !important;
              cursor: pointer !important;
              font-weight: 500 !important;
            }
            .badge-link-style:hover {
              color: #014073 !important;
            }
          `}
        </style>
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
          isLoading={isLoading}
          providedOptions={{
            rowData: searchResults.results,
            columnDefs: columns,
            context: { onBadgeClick: handleMemberClick },
            onRowClicked: ((event: RowClickedEvent<EmployeeDetails>) => {
              if (event.data) {
                handleMemberClick(event.data);
              }
            }) as (event: unknown) => void
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
  },
  (prevProps, nextProps) => {
    // Custom comparison function
    // Only re-render if incoming props are different
    return (
      prevProps.searchResults.results === nextProps.searchResults.results &&
      prevProps.searchResults.total === nextProps.searchResults.total &&
      prevProps.memberGridPagination.pageNumber === nextProps.memberGridPagination.pageNumber &&
      prevProps.memberGridPagination.pageSize === nextProps.memberGridPagination.pageSize &&
      prevProps.memberGridPagination.sortParams === nextProps.memberGridPagination.sortParams &&
      prevProps.isLoading === nextProps.isLoading &&
      prevProps.onMemberSelect === nextProps.onMemberSelect &&
      prevProps.onPaginationChange === nextProps.onPaginationChange &&
      prevProps.onSortChange === nextProps.onSortChange
    );
  }
);

export default MasterInquiryMemberGrid;
