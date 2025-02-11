import { Typography, Button, Tooltip } from "@mui/material";
import { CellValueChangedEvent, IRowNode } from "ag-grid-community";
import { useState, useMemo } from "react";
import { useSelector, useDispatch } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetManageExecutiveHoursAndDollarsColumns } from "./ManageExecutiveHoursAndDollarsGridColumns";
import { ExecutiveHoursAndDollars, ExecutiveHoursAndDollarsGrid, PagedReportResponse } from "reduxstore/types";
import {
  addExecutiveHoursAndDollarsGridRow,
  removeExecutiveHoursAndDollarsGridRow,
  updateExecutiveHoursAndDollarsGridRow
} from "reduxstore/slices/yearsEndSlice";
import { AddOutlined } from "@mui/icons-material";

const RenderAddExecutiveButton = (reportReponse: PagedReportResponse<ExecutiveHoursAndDollars> | null) => {
  // We cannot add an employee if there is no result set there
  const gridAvailable: boolean = reportReponse?.response != null && reportReponse?.response != undefined;

  const addButton = (
    <Button
      disabled={!gridAvailable}
      variant="outlined"
      color="secondary"
      size="medium"
      startIcon={<AddOutlined color={gridAvailable ? "secondary" : "disabled"} />}
      onClick={async () => {
        console.log("Clicked!");
      }}>
      Add Executive
    </Button>
  );

  if (!gridAvailable) {
    return (
      <Tooltip
        placement="top"
        title="You can only add an exec to a search result.">
        <span>{addButton}</span>
      </Tooltip>
    );
  } else {
    return addButton;
  }
};

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
  const isRowStagedToSave = (badge: number): boolean => {
    const found = executiveHoursAndDollarsGrid?.executiveHoursAndDollars.find((obj) => obj.badgeNumber === badge);
    return found != undefined;
  };

  const isTheEditTheOriginalRow = (badge: number, hours: number, dollars: number): boolean => {
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

  const processEditedRow = function (event: CellValueChangedEvent): void {
    const rowInQuestion: IRowNode = event.node;

    const rowRecord: ExecutiveHoursAndDollarsGrid = {
      executiveHoursAndDollars: [
        {
          badgeNumber: rowInQuestion.data.badgeNumber,
          executiveHours: rowInQuestion.data.hoursExecutive,
          executiveDollars: rowInQuestion.data.incomeExecutive
        }
      ],
      profitYear: executiveHoursAndDollarsGrid?.profitYear || null
    };

    if (isRowStagedToSave(rowInQuestion.data.badgeNumber)) {
      // We have two situations if the row is in the pending batch already:
      // 1. The changes are a reversion to what is already in the database
      // 2. The changes are additional unsaved changes that do not match
      //    what is in the database
      if (
        isTheEditTheOriginalRow(
          rowInQuestion.data.badgeNumber,
          rowInQuestion.data.hoursExecutive,
          rowInQuestion.data.incomeExecutive
        )
      ) {
        // We remove the pending row entirely as no changes are needed
        dispatch(removeExecutiveHoursAndDollarsGridRow(rowRecord));
      } else {
        // So these are additional edits that need to be saved
        dispatch(updateExecutiveHoursAndDollarsGridRow(rowRecord));
      }
    } else {
      dispatch(addExecutiveHoursAndDollarsGridRow(rowRecord));
    }

    // Now we need to update this changed row in the grid's underlying
    // data or else it will be undone on the next re-render
    if (mutableCopyOfGridData) {
      for (const element of mutableCopyOfGridData.response.results) {
        if (element.badgeNumber === rowInQuestion.data.badgeNumber) {
          element.incomeExecutive = rowInQuestion.data.incomeExecutive;
          element.hoursExecutive = rowInQuestion.data.hoursExecutive;
        }
      }
    }
  };

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetManageExecutiveHoursAndDollarsColumns(), []);

  // We memoize this because we only want to copy this once as there will be differences
  // once edits are made
  const mutableCopyOfGridData = useMemo(() => structuredClone(executiveHoursAndDollars), [executiveHoursAndDollars]);

  return (
    <>
      {mutableCopyOfGridData?.response && (
        <>
          <div className="px-[24px]">
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`Manage Executive Hours and Dollars (${mutableCopyOfGridData?.response.total || 0})`}
            </Typography>
          </div>
          <div style={{ gap: "36px", display: "flex", justifyContent: "end", marginRight: 8 }}>
            {RenderAddExecutiveButton(executiveHoursAndDollars)}
          </div>
          <DSMGrid
            preferenceKey={"DUPE_SSNS"}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: mutableCopyOfGridData?.response.results,
              columnDefs: columnDefs,
              onCellValueChanged: (event: CellValueChangedEvent) => processEditedRow(event),
              getRowStyle: (params) => {
                // Rows with unsaved changes will have yellow color
                if (isRowStagedToSave(params.node.data.badgeNumber)) {
                  return { background: "lemonchiffon" };
                } else {
                  return { background: "white" };
                }
              }
            }}
          />
        </>
      )}
      {!!mutableCopyOfGridData && mutableCopyOfGridData.response.results.length > 0 && (
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
          recordCount={mutableCopyOfGridData?.response.total}
        />
      )}
    </>
  );
};

export default ManageExecutiveHoursAndDollarsGrid;
