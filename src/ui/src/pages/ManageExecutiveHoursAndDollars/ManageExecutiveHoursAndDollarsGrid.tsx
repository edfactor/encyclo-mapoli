import { Typography } from "@mui/material";
import { CellClickedEvent, CellValueChangedEvent, RowValueChangedEvent } from "ag-grid-community";
import { useState, useMemo } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetManageExecutiveHoursAndDollarsColumns } from "./ManageExecutiveHoursAndDollarsGridColumns";
import { ExecutiveHoursAndDollars, PagedReportResponse } from "reduxstore/types";

const ManageExecutiveHoursAndDollarsGrid = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const { executiveHoursAndDollars } = useSelector((state: RootState) => state.yearsEnd);

  const dataChanged = (
    originalData: PagedReportResponse<ExecutiveHoursAndDollars> | null,
    newData: PagedReportResponse<ExecutiveHoursAndDollars> | null
  ) => JSON.stringify(originalData) !== JSON.stringify(newData);

  //let gridHoursAndDollars<PagedReportResponse>;</PagedReportResponse>;
  //Object.assign(gridHoursAndDollars, executiveHoursAndDollars);
  const copiedResponse = structuredClone(executiveHoursAndDollars);

  console.log("Initial value was: " + dataChanged(copiedResponse, executiveHoursAndDollars));

  // We cannot use the values directly as they are immutable

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetManageExecutiveHoursAndDollarsColumns(), []);

  return (
    <>
      {copiedResponse?.response && (
        <>
          <div className="px-[24px]">
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`Manage Executive Hours and Dollars (${copiedResponse?.response.total || 0})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"DUPE_SSNS"}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: copiedResponse?.response.results,
              columnDefs: columnDefs,
              // eslint-disable-next-line @typescript-eslint/no-unused-vars
              onCellValueChanged: (event: CellValueChangedEvent) =>
                dataChanged(executiveHoursAndDollars, copiedResponse)
                  ? console.log("Row changed for badge: " + event.node.data.badgeNumber)
                  : console.log("Data did not change")
              // eslint-disable-next-line @typescript-eslint/no-unused-vars
              //onRowValueChanged: (event: RowValueChangedEvent) => console.log("Row was updated")
            }}
          />
        </>
      )}
      {!!copiedResponse && copiedResponse.response.results.length > 0 && (
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
          recordCount={copiedResponse?.response.total}
        />
      )}
    </>
  );
};

export default ManageExecutiveHoursAndDollarsGrid;
