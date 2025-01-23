import { Typography } from "@mui/material";
import { useState, useMemo } from "react";
import { useDispatch, useSelector } from "react-redux";
// TODO: import { useLazyGetProfitShareReportQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetProfitShareReportColumns } from "./ProfitShareReportGridColumn";

const ProfitShareReportGrid = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const dispatch = useDispatch();
  // const { profitShare } = useSelector((state: RootState) => state.yearsEnd);
  // const [_, { isLoading }] = useLazyGetProfitShareReportQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetProfitShareReportColumns(), []);

  return (
    <>
      {/* {profitShare?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`PROFIT-ELIGIBLE REPORT (${profitShare?.response.total || 0})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"ELIGIBLE_EMPLOYEES"}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: profitShare?.response.results,
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {!!profitShare && profitShare.response.results.length > 0 && (
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
          recordCount={profitShare.response.total}
        />
      )} */}
    </>
  );
};

export default ProfitShareReportGrid;
