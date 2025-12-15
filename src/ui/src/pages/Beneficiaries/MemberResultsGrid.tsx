import { BeneficiaryDetail } from "@/types";
import { RowClickedEvent } from "ag-grid-community";
import { useMemo } from "react";
import { DSMGrid, Paged, Pagination } from "smart-ui-library";
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
}

const MemberResultsGrid: React.FC<MemberResultsGridProps> = ({
  searchResults,
  isLoading,
  pageNumber,
  pageSize,
  onRowClick,
  onPageNumberChange,
  onPageSizeChange
}) => {
  const columnDefs = useMemo(() => {
    return GetMemberResultsGridColumns();
  }, []);

  return (
    <>
      <p>Please click on a row below to see details</p>
      <DSMGrid
        preferenceKey={GRID_KEYS.MEMBER_RESULTS}
        isLoading={isLoading}
        providedOptions={{
          rowData: searchResults.results,
          columnDefs: columnDefs,
          suppressMultiSort: true,

          onRowClicked: ((event: RowClickedEvent<BeneficiaryDetail>) => {
            if (event.data) {
              onRowClick(event.data);
            }
          }) as (event: unknown) => void
        }}
      />

      <Pagination
        pageNumber={pageNumber}
        setPageNumber={(value: number) => {
          onPageNumberChange(value - 1);
        }}
        pageSize={pageSize}
        setPageSize={(value: number) => {
          onPageSizeChange(value);
          onPageNumberChange(1);
        }}
        recordCount={searchResults?.total}
      />
    </>
  );
};

export default MemberResultsGrid;
