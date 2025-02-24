import { Typography } from "@mui/material";
import { useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import { GetYTDWagesColumns } from "./YTDWagesGridColumn";

import { RefObject } from "react";

interface YTDWagesGridProps {
  innerRef: RefObject<HTMLDivElement>;
}

const YTDWagesGrid = ({ innerRef }: YTDWagesGridProps) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const { employeeWagesForYear } = useSelector((state: RootState) => state.yearsEnd);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetYTDWagesColumns(), []);

  return (
    <>
      {employeeWagesForYear?.response && (
        <div ref={innerRef}>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`${CAPTIONS.YTD_WAGES_EXTRACT} (${employeeWagesForYear.response.total || 0})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"TERM"}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: employeeWagesForYear?.response.results,
              columnDefs: columnDefs
            }}
          />
        </div>
      )}
      {/* We need to check the response also because if the user asked for a CSV, this variable will exist, but have a blob in it instead of a response */}
      {!!employeeWagesForYear && employeeWagesForYear.response && employeeWagesForYear.response.results.length > 0 && (
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
          recordCount={employeeWagesForYear.response.total}
        />
      )}
    </>
  );
};

export default YTDWagesGrid;
