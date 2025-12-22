import { useContentAwareGridHeight } from "@/hooks/useContentAwareGridHeight";
import FullscreenIcon from "@mui/icons-material/Fullscreen";
import FullscreenExitIcon from "@mui/icons-material/FullscreenExit";
import { Box, IconButton, Typography } from "@mui/material";
import React, { memo, useMemo } from "react";
import { DSMGrid, formatNumberWithComma, ISortParams, Pagination } from "smart-ui-library";
import { GRID_KEYS } from "../../../constants";
import { SortParams } from "../../../hooks/useGridPagination";
import { EmployeeDetails } from "../../../reduxstore/types";
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
    handlePageNumberChange: (pageNumber: number) => void;
    handlePageSizeChange: (pageSize: number) => void;
  };
  onSortChange: (sortParams: SortParams) => void;
  isLoading?: boolean;
  isGridExpanded?: boolean;
  onToggleExpand?: () => void;
}

const MasterInquiryMemberGrid: React.FC<MasterInquiryMemberGridProps> = memo(
  ({
    searchResults,
    onMemberSelect,
    memberGridPagination,
    onSortChange,
    isLoading = false,
    isGridExpanded = false,
    onToggleExpand
  }: MasterInquiryMemberGridProps) => {
    // Use content-aware grid height - shrinks for small result sets
    const gridMaxHeight = useContentAwareGridHeight({
      rowCount: searchResults.results?.length ?? 0,
      heightPercentage: isGridExpanded ? 0.85 : 0.5
    });

    const handleMemberClick = (member: EmployeeDetails) => {
      onMemberSelect({
        memberType: member.isEmployee ? 1 : 2,
        id: Number(member.id),
        ssn: Number(member.ssn),
        badgeNumber: Number(member.badgeNumber),
        psnSuffix: Number(member.psnSuffix)
      });
    };

    const handleNavigate = (path: string) => {
      console.log(`Navigating to ${path}`);
      const i = path.lastIndexOf('/');
      const badgeNumberParameter = path.substring(i + 1);

      const employee = searchResults.results.find(emp => {
        const badgeNumberPlusSuffix = emp.psnSuffix > 0 
        ? `${emp.badgeNumber}${String(emp.psnSuffix).padStart(4, "0")}` 
        : emp.badgeNumber.toString();
        return badgeNumberPlusSuffix === badgeNumberParameter;
      });

      if (employee) {
        handleMemberClick(employee);
      } else {
        console.warn(`Employee with badge number and suffix ${badgeNumberParameter} not found in current results.`);
      }
    }

    const handleSortChange = (sortParams: ISortParams) => {
      onSortChange(sortParams);
    };

    const columns = useMemo(
      () => GetMasterInquiryMemberGridColumns(handleNavigate), 
      [handleNavigate]
    );

    return (
      <Box sx={{ width: "100%", paddingTop: "24px" }}>
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            padding: "0 24px",
            marginBottom: "8px"
          }}>
          <Typography
            variant="h2"
            sx={{ color: "#0258A5" }}>
            {`Search Results (${formatNumberWithComma(searchResults.total)} ${searchResults.total === 1 ? "Record" : "Records"})`}
          </Typography>
          <IconButton
            onClick={onToggleExpand}
            sx={{ zIndex: 1 }}
            aria-label={isGridExpanded ? "Exit fullscreen" : "Enter fullscreen"}>
            {isGridExpanded ? <FullscreenExitIcon /> : <FullscreenIcon />}
          </IconButton>
        </Box>
        <DSMGrid
          preferenceKey={GRID_KEYS.MASTER_INQUIRY_MEMBER}
          handleSortChanged={handleSortChange}
          isLoading={isLoading}
          maxHeight={gridMaxHeight}
          providedOptions={{
            rowData: searchResults.results.filter((row) => row && Object.keys(row).length > 0),
            columnDefs: columns,
          }}
        />
        <Pagination
          rowsPerPageOptions={[5, 10, 50, 100]}
          pageNumber={memberGridPagination.pageNumber}
          setPageNumber={(value: number) => memberGridPagination.handlePageNumberChange(value - 1)}
          pageSize={memberGridPagination.pageSize}
          setPageSize={memberGridPagination.handlePageSizeChange}
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
      prevProps.isGridExpanded === nextProps.isGridExpanded &&
      prevProps.onMemberSelect === nextProps.onMemberSelect &&
      prevProps.onSortChange === nextProps.onSortChange &&
      prevProps.onToggleExpand === nextProps.onToggleExpand
    );
  }
);

export default MasterInquiryMemberGrid;
