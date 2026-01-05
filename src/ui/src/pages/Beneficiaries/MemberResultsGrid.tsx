import { SortParams } from "@/hooks/useGridPagination";
import { BeneficiaryDetail } from "@/types";
import { RowClickedEvent } from "ag-grid-community";
import { useMemo } from "react";
import { Paged } from "smart-ui-library";
import { DSMPaginatedGrid } from "../../components/DSMPaginatedGrid/DSMPaginatedGrid";
import { GRID_KEYS } from "../../constants";
import { GetMemberResultsGridColumns } from "./MemberResultsGridColumns";

interface MemberResultsGridProps {
  searchResults: Paged<BeneficiaryDetail>;
  isLoading: boolean;
  pageNumber: number;
  pageSize: number;
  onRowClick: (data: BeneficiaryDetail) => void;
  onPageNumberChange: (pageNumber: number) => void;
  onPageSizeChange: (pageSize: number) => void;
  handleSortChange: (sortParams: SortParams) => void;
}

const MemberResultsGrid: React.FC<MemberResultsGridProps> = ({
  searchResults,
  isLoading,
  pageNumber,
  pageSize,
  onRowClick,
  onPageNumberChange,
  onPageSizeChange,
  handleSortChange
}) => {
  const columnDefs = useMemo(() => {
    return GetMemberResultsGridColumns();
  }, []);

  return (
    <DSMPaginatedGrid<BeneficiaryDetail>
      preferenceKey={GRID_KEYS.MEMBER_RESULTS}
      data={searchResults.results ?? []}
      columnDefs={columnDefs}
      totalRecords={searchResults?.total ?? 0}
      isLoading={isLoading}
      pagination={{
        pageNumber,
        pageSize,
        sortParams: { sortBy: "", isSortDescending: false },
        handlePageNumberChange: (value: number) => {
          onPageNumberChange(value);
        },
        handlePageSizeChange: (value: number) => {
          onPageSizeChange(value);
          onPageNumberChange(0);
        },
        handleSortChange: handleSortChange
      }}
      gridOptions={{
        suppressMultiSort: true,
        onRowClicked: ((event: RowClickedEvent<BeneficiaryDetail>) => {
          if (event.data) {
            onRowClick(event.data);
          }
        }) as (event: unknown) => void
      }}
      showPagination={searchResults?.total > 0}
      header={<p>Please click on a row below to see details</p>}
    />
  );
};

export default MemberResultsGrid;
