import { RefObject, useMemo } from "react";
import { DSMGrid, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { DuplicateNameAndBirthday, PagedReportResponse } from "../../../types";
import { GetDuplicateNamesAndBirthdayColumns } from "./DuplicateNamesAndBirthdaysGridColumns";
import { GridPaginationState, GridPaginationActions } from "../../../hooks/useGridPagination";

interface DuplicateNamesAndBirthdaysGridProps {
  innerRef: RefObject<HTMLDivElement | null>;
  data: PagedReportResponse<DuplicateNameAndBirthday> | null;
  isLoading: boolean;
  showData: boolean;
  hasResults: boolean;
  pagination: GridPaginationState & GridPaginationActions;
  onPaginationChange: (pageNumber: number, pageSize: number) => void;
  onSortChange: (sortParams: any) => void;
}

const DuplicateNamesAndBirthdaysGrid = ({
  innerRef,
  data,
  isLoading,
  showData,
  hasResults,
  pagination,
  onPaginationChange,
  onSortChange
}: DuplicateNamesAndBirthdaysGridProps) => {
  const columnDefs = useMemo(() => GetDuplicateNamesAndBirthdayColumns(), []);

  return (
    <>
      {showData && data?.response && (
        <div ref={innerRef}>
          <DSMGrid
            preferenceKey={CAPTIONS.DUPLICATE_NAMES}
            isLoading={isLoading}
            handleSortChanged={onSortChange}
            providedOptions={{
              rowData: data.response.results,
              columnDefs: columnDefs
            }}
          />
        </div>
      )}
      {hasResults && data?.response && (
        <Pagination
          pageNumber={pagination.pageNumber}
          setPageNumber={(value: number) => {
            onPaginationChange(value - 1, pagination.pageSize);
          }}
          pageSize={pagination.pageSize}
          setPageSize={(value: number) => {
            onPaginationChange(0, value);
          }}
          recordCount={data.response.total || 0}
        />
      )}
    </>
  );
};

export default DuplicateNamesAndBirthdaysGrid;
