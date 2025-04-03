import { useState, useMemo } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams } from "smart-ui-library";
import { ProfitShareUpdateGridColumns } from "./ProfitShareEditUpdateGridColumns";
import { ProfitShareEditUpdateGridColumns } from "./ProfitShareEditGridColumns";
import { Typography } from "@mui/material";

const ProfitShareEditUpdateGrid = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Name",
    isSortDescending: false
  });
  const columnDefs = useMemo(() => ProfitShareUpdateGridColumns(), []);
  const editColumnDefs = useMemo(() => ProfitShareEditUpdateGridColumns(), []);
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
        <DSMGrid
          preferenceKey={"ProfitShareUpdateGrid"}
          isLoading={false}
          providedOptions={{
            rowData: "response" in profitSharingUpdate ? profitSharingUpdate.response?.results : [],
            columnDefs: columnDefs
          }}
        />
      )}
    </>
  );
};

export default ProfitShareEditUpdateGrid;
