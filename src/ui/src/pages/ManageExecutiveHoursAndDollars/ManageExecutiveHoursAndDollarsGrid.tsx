import { Typography } from "@mui/material";
import { useState, useMemo } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetManageExecutiveHoursAndDollarsColumns } from "./ManageExecutiveHoursAndDollarsGridColumns";

const ManageExecutiveHoursAndDollarsGrid = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const { executiveHoursAndDollars } = useSelector((state: RootState) => state.yearsEnd);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetManageExecutiveHoursAndDollarsColumns(), []);

  return (
    <>
      {executiveHoursAndDollars?.response && (
        <>
          <div className="px-[24px]">
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`Manage Executive Hours and Dollars (${executiveHoursAndDollars?.response.total || 0})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"DUPE_SSNS"}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: executiveHoursAndDollars?.response.results,
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {!!executiveHoursAndDollars && executiveHoursAndDollars.response.results.length > 0 && (
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
          recordCount={executiveHoursAndDollars.response.total}
        />
      )}
    </>
  );
};

export default ManageExecutiveHoursAndDollarsGrid;
