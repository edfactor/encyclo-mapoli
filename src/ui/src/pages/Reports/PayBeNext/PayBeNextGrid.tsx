import { Typography } from "@mui/material";
import { RefObject, useMemo } from "react";
import { DSMPaginatedGrid } from "../../../components/DSMPaginatedGrid/DSMPaginatedGrid";
import { GRID_KEYS } from "../../../constants";
import { useContentAwareGridHeight } from "../../../hooks/useContentAwareGridHeight";
import { GridPaginationActions, GridPaginationState, SortParams } from "../../../hooks/useGridPagination";
import { adhocBeneficiariesReportResponse } from "../../../reduxstore/types";
import { PayBeNextGridColumns } from "./PayBeNextGridColumns";
import { PayBeNextGridRow } from "./hooks/usePayBeNext";

/**
 * Props for PayBeNextGrid component
 */
interface PayBeNextGridProps {
  innerRef?: RefObject<HTMLDivElement | null>;
  data: adhocBeneficiariesReportResponse | null;
  gridData: PayBeNextGridRow[];
  isLoading: boolean;
  showData: boolean;
  hasResults: boolean;
  totalEndingBalance: number | null;
  profitYear: number;
  pagination: GridPaginationState & GridPaginationActions;
  onSortChange: (sortParams: SortParams) => void;
  onRowExpansion?: (rowKey: string) => void;
}

/**
 * Detail cell renderer for profit details
 */
const DetailCellRenderer = (params: { data: PayBeNextGridRow }) => {
  const pDetails = params.data?.profitDetails || [];

  if (pDetails.length === 0) {
    return <div style={{ padding: "10px" }}>No profit details found.</div>;
  }

  return (
    <div style={{ padding: "10px" }}>
      <table
        style={{
          width: "100%",
          marginTop: "8px",
          borderCollapse: "collapse"
        }}>
        <thead>
          <tr style={{ backgroundColor: "#f5f5f5" }}>
            <th style={{ padding: "8px", textAlign: "left" }}>Year</th>
            <th style={{ padding: "8px", textAlign: "left" }}>Code</th>
            <th style={{ padding: "8px", textAlign: "right" }}>Contributions</th>
            <th style={{ padding: "8px", textAlign: "right" }}>Earnings</th>
            <th style={{ padding: "8px", textAlign: "right" }}>Forfeitures</th>
            <th style={{ padding: "8px", textAlign: "left" }}>Date</th>
            <th style={{ padding: "8px", textAlign: "left" }}>Comments</th>
          </tr>
        </thead>
        <tbody>
          {pDetails.map((detail, index) => (
            <tr
              key={`${detail.year}-${detail.code}-${index}`}
              style={{ borderBottom: "1px solid #eee" }}>
              <td style={{ padding: "8px" }}>{detail.year}</td>
              <td style={{ padding: "8px" }}>{detail.code}</td>
              <td style={{ padding: "8px", textAlign: "right" }}>
                {detail.contributions?.toLocaleString("en-US", {
                  style: "currency",
                  currency: "USD"
                })}
              </td>
              <td style={{ padding: "8px", textAlign: "right" }}>
                {detail.earnings?.toLocaleString("en-US", {
                  style: "currency",
                  currency: "USD"
                })}
              </td>
              <td style={{ padding: "8px", textAlign: "right" }}>
                {detail.forfeitures?.toLocaleString("en-US", {
                  style: "currency",
                  currency: "USD"
                })}
              </td>
              <td style={{ padding: "8px" }}>
                {detail.date ? new Date(detail.date).toLocaleDateString() : ""}
              </td>
              <td style={{ padding: "8px" }}>{detail.comments}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

/**
 * Grid component for PayBeNext report
 * Displays beneficiary data with master-detail view for profit details
 */
const PayBeNextGrid = ({
  innerRef,
  data,
  gridData,
  isLoading,
  showData,
  hasResults,
  totalEndingBalance,
  profitYear,
  pagination,
  onSortChange
}: PayBeNextGridProps) => {
  const columnDefs = useMemo(() => PayBeNextGridColumns(), []);

  // Use content-aware grid height utility hook
  const gridMaxHeight = useContentAwareGridHeight({
    rowCount: gridData?.length ?? 0
  });

  if (!showData || !data?.response?.results) {
    return null;
  }

  return (
    <div
      className="relative"
      ref={innerRef}>
      <div className="mb-4">
        <Typography
          variant="h2"
          sx={{ color: "#0258A5", marginBottom: "8px" }}>
          {`Employee Beneficiaries Control Sheet Starting at Year ${profitYear}`}
        </Typography>
        <Typography variant="body1">
          <strong>Ending Balance:</strong>{" "}
          {totalEndingBalance?.toLocaleString("en-US", {
            style: "currency",
            currency: "USD"
          }) ?? "$0.00"}
        </Typography>
      </div>

      <DSMPaginatedGrid<PayBeNextGridRow>
        preferenceKey={GRID_KEYS.PAY_BE_NEXT}
        data={gridData}
        columnDefs={columnDefs}
        totalRecords={data.response.total || 0}
        isLoading={isLoading}
        pagination={pagination}
        onSortChange={onSortChange}
        heightConfig={{
          mode: "content-aware",
          maxHeight: gridMaxHeight
        }}
        gridOptions={{
          suppressMultiSort: true,
          masterDetail: true,
          detailCellRenderer: DetailCellRenderer
        }}
        showPagination={hasResults}
      />
    </div>
  );
};

export default PayBeNextGrid;
