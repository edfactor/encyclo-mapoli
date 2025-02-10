import { Typography } from "@mui/material";
import { CellValueChangedEvent, IRowNode } from "ag-grid-community";
import { useState, useMemo } from "react";
import { useSelector, useDispatch } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetManageExecutiveHoursAndDollarsColumns } from "./ManageExecutiveHoursAndDollarsGridColumns";
import { ExecutiveHoursAndDollars, ExecutiveHoursAndDollarsGrid } from "reduxstore/types";
import {
  addExecutiveHoursAndDollarsGridRow,
  removeExecutiveHoursAndDollarsGridRow,
  updateExecutiveHoursAndDollarsGridRow
} from "reduxstore/slices/yearsEndSlice";

const ManageExecutiveHoursAndDollarsGrid = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const dispatch = useDispatch();

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const { executiveHoursAndDollars, executiveHoursAndDollarsGrid } = useSelector((state: RootState) => state.yearsEnd);

  // This function checks to see if we have a change for this badge number already pending for a save
  const isRowAlreadyPending = (badge: number): boolean => {
    const found = executiveHoursAndDollarsGrid?.executiveHoursAndDollars.find((obj) => obj.badgeNumber === badge);
    return found != undefined;
  };

  const isThisTheOriginalRow = (badge: number, hours: number, dollars: number): boolean => {
    const found: ExecutiveHoursAndDollars | undefined = executiveHoursAndDollars?.response.results.find(
      (obj) => obj.badgeNumber === badge
    );

    if (found) {
      if (hours === found.hoursExecutive && dollars === found.incomeExecutive) {
        return true;
      }
    }
    return false;
  };

  const processRow = function (event: CellValueChangedEvent): void {
    const rowInQuestion: IRowNode = event.node;
    console.log("In process row. Badge was: " + rowInQuestion.data.badgeNumber);
    console.log("Column was: " + event.colDef.field);

    const rowToAdd: ExecutiveHoursAndDollarsGrid = {
      executiveHoursAndDollars: [
        {
          badgeNumber: rowInQuestion.data.badgeNumber,
          executiveHours: rowInQuestion.data.hoursExecutive,
          executiveDollars: rowInQuestion.data.incomeExecutive
        }
      ],
      profitYear: executiveHoursAndDollarsGrid?.profitYear || null
    };

    if (isRowAlreadyPending(rowInQuestion.data.badgeNumber)) {
      console.log("Row pending already!");

      // But we have two situations with the row already there:
      // 1. The changes are a reversion to what was in the database
      // 2. The changes are additional changes that do not match
      //    what is in the database
      if (
        isThisTheOriginalRow(
          rowInQuestion.data.badgeNumber,
          rowInQuestion.data.hoursExecutive,
          rowInQuestion.data.incomeExecutive
        )
      ) {
        dispatch(removeExecutiveHoursAndDollarsGridRow(rowToAdd));
      } else {
        dispatch(updateExecutiveHoursAndDollarsGridRow(rowToAdd));
      }
    } else {
      console.log("Row not there, so... adding row");
      dispatch(addExecutiveHoursAndDollarsGridRow(rowToAdd));
    }

    // Now we need to update this value in the grid's data
    if (copiedResponse) {
      // Need to loop through
      for (const element of copiedResponse.response.results) {
        if (element.badgeNumber === rowInQuestion.data.badgeNumber) {
          element.incomeExecutive = rowInQuestion.data.incomeExecutive;
          element.hoursExecutive = rowInQuestion.data.hoursExecutive;
        }
      }

      return;
    }
    return;
  };

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetManageExecutiveHoursAndDollarsColumns(), []);
  const copiedResponse = useMemo(() => structuredClone(executiveHoursAndDollars), [executiveHoursAndDollars]);

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
              onCellValueChanged: (event: CellValueChangedEvent) => processRow(event)
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
