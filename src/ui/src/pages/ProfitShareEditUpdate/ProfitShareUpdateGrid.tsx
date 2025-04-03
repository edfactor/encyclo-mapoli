import { Typography } from "@mui/material";
import { useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { ProfitShareUpdateGridColumns } from "./ProfitShareUpdateGridColumns";

const ProfitShareEditUpdateGrid = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Name",
    isSortDescending: false
  });
  const columnDefs = useMemo(() => ProfitShareUpdateGridColumns(), []);
  const { profitSharingUpdate } = useSelector((state: RootState) => state.yearsEnd);

  return (
    <>
      <div className="px-[24px]">
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          {`Profit Share Update (PAY 447)`}
        </Typography>
      </div>
      {!!profitSharingUpdate && (
        <>
          <DSMGrid
            preferenceKey={"ProfitShareUpdateGrid"}
            isLoading={false}
            providedOptions={{
              rowData: "response" in profitSharingUpdate ? profitSharingUpdate.response?.results : [],
              columnDefs: columnDefs
            }}
          />
          <Pagination
            pageNumber={pageNumber}
            setPageNumber={(value: number) => {
              setPageNumber(value - 1);
            }}
            pageSize={pageSize}
            setPageSize={(value: number) => {
              setPageSize(value);
              setPageNumber(1);
            }}
            recordCount={profitSharingUpdate?.response.total ?? 0}
          />
        </>
      )}
    </>
  );
};

export default ProfitShareEditUpdateGrid;
