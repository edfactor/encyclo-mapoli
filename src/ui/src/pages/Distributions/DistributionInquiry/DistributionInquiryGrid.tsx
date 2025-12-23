import { Typography } from "@mui/material";
import { useCallback, useMemo } from "react";
import { numberToCurrency } from "smart-ui-library";
import DSMPaginatedGrid from "../../../components/DSMPaginatedGrid/DSMPaginatedGrid";
import { GRID_KEYS } from "../../../constants";
import { SortParams, useGridPagination } from "../../../hooks/useGridPagination";
import { DistributionSearchResponse } from "../../../types";
import { GetDistributionInquiryColumns } from "./DistributionInquiryGridColumns";

interface DistributionInquiryGridProps {
  postReturnData: DistributionSearchResponse[] | null;
  totalRecords: number;
  isLoading: boolean;
  onPaginationChange: (pageNumber: number, pageSize: number, sortParams: SortParams) => void;
}

const DistributionInquiryGrid: React.FC<DistributionInquiryGridProps> = ({
  postReturnData,
  totalRecords,
  isLoading,
  onPaginationChange
}) => {
  const pagination = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "badgeNumber",
    initialSortDescending: false,
    persistenceKey: GRID_KEYS.DISTRIBUTION_INQUIRY,
    onPaginationChange: useCallback(
      async (pageNum: number, pageSz: number, sortPrms: SortParams) => {
        onPaginationChange(pageNum, pageSz, sortPrms);
      },
      [onPaginationChange]
    )
  });

  const { pageNumber } = pagination;

  const columnDefs = useMemo(() => GetDistributionInquiryColumns(), []);

  // Calculate totals
  const totals = useMemo(() => {
    if (!postReturnData || postReturnData.length === 0) {
      return {
        grossAmount: 0,
        federalTax: 0,
        stateTax: 0,
        checkAmount: 0
      };
    }

    return postReturnData.reduce(
      (acc, row) => ({
        grossAmount: acc.grossAmount + row.grossAmount,
        federalTax: acc.federalTax + row.federalTax,
        stateTax: acc.stateTax + row.stateTax,
        checkAmount: acc.checkAmount + row.checkAmount
      }),
      {
        grossAmount: 0,
        federalTax: 0,
        stateTax: 0,
        checkAmount: 0
      }
    );
  }, [postReturnData]);

  return (
    <DSMPaginatedGrid<DistributionSearchResponse>
      preferenceKey={GRID_KEYS.DISTRIBUTION_INQUIRY}
      data={postReturnData ?? []}
      columnDefs={columnDefs}
      totalRecords={totalRecords}
      isLoading={isLoading}
      pagination={pagination}
      onSortChange={pagination.handleSortChange}
      showPagination={postReturnData !== null && postReturnData.length > 0}
      header={
        <Typography variant="h2" sx={{ color: "#0258A5" }}>
          {`Distribution Records (${totalRecords} ${totalRecords === 1 ? "Record" : "Records"})`}
        </Typography>
      }
      afterGrid={
        postReturnData && postReturnData.length > 0 ? (
          <div
            style={{
              padding: "16px 24px",
              borderTop: "2px solid #0258A5",
              backgroundColor: "#f5f5f5",
              display: "grid",
              gridTemplateColumns: "repeat(4, 1fr)",
              gap: "16px"
            }}>
            <div>
              <Typography variant="subtitle2" sx={{ fontWeight: "bold" }}>
                Total Gross Amount (Page {pageNumber + 1}):
              </Typography>
              <Typography variant="body1">{numberToCurrency(totals.grossAmount)}</Typography>
            </div>
            <div>
              <Typography variant="subtitle2" sx={{ fontWeight: "bold" }}>
                Total Federal Tax (Page {pageNumber + 1}):
              </Typography>
              <Typography variant="body1">{numberToCurrency(totals.federalTax)}</Typography>
            </div>
            <div>
              <Typography variant="subtitle2" sx={{ fontWeight: "bold" }}>
                Total State Tax (Page {pageNumber + 1}):
              </Typography>
              <Typography variant="body1">{numberToCurrency(totals.stateTax)}</Typography>
            </div>
            <div>
              <Typography variant="subtitle2" sx={{ fontWeight: "bold" }}>
                Total Check Amount (Page {pageNumber + 1}):
              </Typography>
              <Typography variant="body1">{numberToCurrency(totals.checkAmount)}</Typography>
            </div>
          </div>
        ) : undefined
      }
    />
  );
};

export default DistributionInquiryGrid;
