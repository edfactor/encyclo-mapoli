import { Typography } from "@mui/material";
import { useMemo } from "react";
import { DSMGrid, Pagination } from "smart-ui-library";
import { GetMasterInquiryGridColumns } from "./MasterInquiryGridColumns";
import { CAPTIONS } from "../../constants";

interface ProfitData {
  results: any[];
  total: number;
}

interface MasterInquiryGridProps {
  profitData?: ProfitData | null;
  isLoading?: boolean;
  profitGridPagination?: {
    pageNumber: number;
    pageSize: number;
    sortParams: any;
  };
  onPaginationChange?: (pageNumber: number, pageSize: number, sortParams: any) => void;
}

const MasterInquiryGrid: React.FC<MasterInquiryGridProps> = ({ 
  profitData, 
  isLoading, 
  profitGridPagination,
  onPaginationChange 
}) => {
  const columnDefs = useMemo(() => GetMasterInquiryGridColumns(), []);

  if (isLoading) {
    return <Typography>Loading profit details...</Typography>;
  }

  if (!profitData) {
    return <Typography>No profit details found.</Typography>;
  }

  const handlePaginationChange = (pageNumber: number, pageSize: number) => {
    if (onPaginationChange && profitGridPagination) {
      onPaginationChange(pageNumber, pageSize, profitGridPagination.sortParams);
    }
  };

  const handleSortChange = (sortParams: any) => {
    if (onPaginationChange && profitGridPagination) {
      onPaginationChange(profitGridPagination.pageNumber, profitGridPagination.pageSize, sortParams);
    }
  };

  return (
    <>
      <div style={{ height: "400px", width: "100%" }}>
        <div style={{ padding: "0 24px 0 24px" }}>
          <Typography
            variant="h2"
            sx={{ color: "#0258A5" }}>
            {`Profit Details (${profitData.total} ${profitData.total === 1 ? "Record" : "Records"})`}
          </Typography>
        </div>
        <DSMGrid
          preferenceKey={CAPTIONS.MASTER_INQUIRY}
          handleSortChanged={handleSortChange}
          providedOptions={{
            rowData: profitData.results,
            columnDefs: columnDefs,
            suppressMultiSort: true
          }}
        />
        {profitGridPagination && onPaginationChange && (
          <Pagination
            pageNumber={profitGridPagination.pageNumber}
            setPageNumber={(value: number) => {
              handlePaginationChange(value - 1, profitGridPagination.pageSize);
            }}
            pageSize={profitGridPagination.pageSize}
            setPageSize={(value: number) => {
              handlePaginationChange(0, value);
            }}
            recordCount={profitData.total}
          />
        )}
      </div>
    </>
  );
};
export default MasterInquiryGrid;
