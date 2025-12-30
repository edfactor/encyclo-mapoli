import { Typography } from "@mui/material";
import { useCallback, useMemo } from "react";
import { DSMPaginatedGrid } from "../../../components/DSMPaginatedGrid";
import { GRID_KEYS } from "../../../constants";
import { SortParams, useGridPagination } from "../../../hooks/useGridPagination";
import { DistributionSearchResponse } from "../../../types";
import { GetDisbursementGridColumns } from "./DisbursementGridColumns";

interface DisbursementGridProps {
  title: string;
  data: DistributionSearchResponse[] | null;
  totalRecords: number;
  isLoading: boolean;
  initialPageSize?: number;
  rowsPerPageOptions?: number[];
  onPaginationChange: (pageNumber: number, pageSize: number, sortParams: SortParams) => void;
}

const DisbursementGrid: React.FC<DisbursementGridProps> = ({
  title,
  data,
  totalRecords,
  isLoading,
  initialPageSize = 25,
  rowsPerPageOptions = [10, 25, 50, 100],
  onPaginationChange
}) => {
  const pagination = useGridPagination({
    initialPageSize,
    initialSortBy: "paymentSequence",
    initialSortDescending: false,
    persistenceKey: `${GRID_KEYS.DISBURSEMENT_PREFIX}${title}`,
    onPaginationChange: useCallback(
      async (pageNum: number, pageSz: number, sortPrms: SortParams) => {
        onPaginationChange(pageNum, pageSz, sortPrms);
      },
      [onPaginationChange]
    )
  });

  const columnDefs = useMemo(() => GetDisbursementGridColumns(), []);
  const preferenceKey = `${GRID_KEYS.DISBURSEMENT_PREFIX}${title}`;

  return (
    <DSMPaginatedGrid
      preferenceKey={preferenceKey}
      data={data}
      columnDefs={columnDefs}
      totalRecords={totalRecords}
      isLoading={isLoading}
      pagination={pagination}
      rowsPerPageOptions={rowsPerPageOptions}
      header={
        <div style={{ padding: "0px 24px 0px 24px" }}>
          <Typography
            variant="h2"
            sx={{ color: "#0258A5", marginBottom: "8px" }}>
            {`${title} (${totalRecords} ${totalRecords === 1 ? "Record" : "Records"})`}
          </Typography>
        </div>
      }
    />
  );
};

export default DisbursementGrid;
