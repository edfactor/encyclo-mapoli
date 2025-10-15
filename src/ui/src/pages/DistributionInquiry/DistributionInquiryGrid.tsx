import { Typography } from "@mui/material";
import { useCallback, useMemo } from "react";
import { DSMGrid, numberToCurrency, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import { useDynamicGridHeight } from "../../hooks/useDynamicGridHeight";
import { useGridPagination } from "../../hooks/useGridPagination";
import { DistributionSearchResponse } from "../../types";
import { GetDistributionInquiryColumns } from "./DistributionInquiryGridColumns";

interface DistributionInquiryGridProps {
  postReturnData: DistributionSearchResponse[] | null;
  totalRecords: number;
  isLoading: boolean;
  onPaginationChange: (pageNumber: number, pageSize: number, sortParams: any) => void;
}

const DistributionInquiryGrid: React.FC<DistributionInquiryGridProps> = ({
  postReturnData,
  totalRecords,
  isLoading,
  onPaginationChange
}) => {
  const { pageNumber, pageSize, handlePaginationChange, handleSortChange } = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "badgeNumber",
    initialSortDescending: false,
    onPaginationChange: useCallback(
      async (pageNum: number, pageSz: number, sortPrms: any) => {
        onPaginationChange(pageNum, pageSz, sortPrms);
      },
      [onPaginationChange]
    )
  });

  const gridMaxHeight = useDynamicGridHeight();
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
    <>
      <div style={{ padding: "24px 24px 0px 24px" }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          {`Distribution Records (${totalRecords} ${totalRecords === 1 ? "Record" : "Records"})`}
        </Typography>
      </div>

      <DSMGrid
        preferenceKey={CAPTIONS.DISTRIBUTIONS_INQUIRY}
        isLoading={isLoading}
        handleSortChanged={handleSortChange}
        maxHeight={gridMaxHeight}
        providedOptions={{
          rowData: postReturnData,
          columnDefs: columnDefs,
          suppressMultiSort: true
        }}
      />

      {postReturnData && postReturnData.length > 0 && (
        <>
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
              <Typography
                variant="subtitle2"
                sx={{ fontWeight: "bold" }}>
                Total Gross Amount (Page):
              </Typography>
              <Typography variant="body1">{numberToCurrency(totals.grossAmount)}</Typography>
            </div>
            <div>
              <Typography
                variant="subtitle2"
                sx={{ fontWeight: "bold" }}>
                Total Federal Tax (Page):
              </Typography>
              <Typography variant="body1">{numberToCurrency(totals.federalTax)}</Typography>
            </div>
            <div>
              <Typography
                variant="subtitle2"
                sx={{ fontWeight: "bold" }}>
                Total State Tax (Page):
              </Typography>
              <Typography variant="body1">{numberToCurrency(totals.stateTax)}</Typography>
            </div>
            <div>
              <Typography
                variant="subtitle2"
                sx={{ fontWeight: "bold" }}>
                Total Check Amount (Page):
              </Typography>
              <Typography variant="body1">{numberToCurrency(totals.checkAmount)}</Typography>
            </div>
          </div>

          <Pagination
            pageNumber={pageNumber}
            setPageNumber={(value: number) => {
              handlePaginationChange(value - 1, pageSize);
            }}
            pageSize={pageSize}
            setPageSize={(value: number) => {
              handlePaginationChange(0, value);
            }}
            recordCount={totalRecords}
          />
        </>
      )}
    </>
  );
};

export default DistributionInquiryGrid;
