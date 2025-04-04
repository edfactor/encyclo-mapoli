import { Typography } from "@mui/material";
import { useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { ProfitShareEditUpdateGridColumns } from "./ProfitShareEditGridColumns";

const ProfitShareEditGrid = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Name",
    isSortDescending: false
  });

  const editColumnDefs = useMemo(() => ProfitShareEditUpdateGridColumns(), []);
  const { profitSharingEdit } = useSelector((state: RootState) => state.yearsEnd);

  return (
    <>
      <div className="px-[24px]">
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          {`Profit Share Edit (PAY444)`}
        </Typography>
      </div>
      {!!profitSharingEdit && (
        <>
          <DSMGrid
            preferenceKey={"ProfitShareEditGrid"}
            isLoading={false}
            providedOptions={{
              rowData: "response" in profitSharingEdit ? profitSharingEdit.response?.results : [],
              columnDefs: editColumnDefs
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
            recordCount={profitSharingEdit?.response.total ?? 0}
          />
        </>
      )}
    </>
  );
};

export default ProfitShareEditGrid;
