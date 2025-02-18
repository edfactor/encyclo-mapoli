import { Typography, Button, Tooltip } from "@mui/material";
import { CellValueChangedEvent, IRowNode, SelectionChangedEvent } from "ag-grid-community";
import { useState, useMemo } from "react";
import { useSelector, useDispatch } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination, SmartModal } from "smart-ui-library";
import { GetManageExecutiveHoursAndDollarsColumns } from "./ManageExecutiveHoursAndDollarsGridColumns";
import {
  ExecutiveHoursAndDollars,
  ExecutiveHoursAndDollarsGrid,
  ExecutiveHoursAndDollarsRow,
  PagedReportResponse
} from "reduxstore/types";
import {
  addExecutiveHoursAndDollarsGridRow,
  clearAdditionalExecutivesChosen,
  clearAdditionalExecutivesGrid,
  clearExecutiveRowsSelected,
  removeExecutiveHoursAndDollarsGridRow,
  setExecutiveRowsSelected,
  updateExecutiveHoursAndDollarsGridRow
} from "reduxstore/slices/yearsEndSlice";
import { AddOutlined } from "@mui/icons-material";
import { WrapperProps } from "./ManageExecutiveHoursAndDollarsSearchFilter";
import SearchAndAddExecutive from "./SearchAndAddExecutive";

interface RenderAddExecutiveButtonProps {
  reportReponse: PagedReportResponse<ExecutiveHoursAndDollars> | null;
  isModal: boolean | undefined;
  setOpenModal: React.Dispatch<React.SetStateAction<boolean>>;
}

const RenderAddExecutiveButton: React.FC<RenderAddExecutiveButtonProps> = ({
  reportReponse,
  isModal,
  setOpenModal
}) => {
  const dispatch = useDispatch();
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
        // We need to clear out previous result rows in redux
        dispatch(clearAdditionalExecutivesChosen());
        dispatch(clearExecutiveRowsSelected());
        dispatch(clearAdditionalExecutivesGrid());
        setOpenModal(true);
      }}>
      Add Executive
    </Button>
  );

  if (!isModal && !gridAvailable) {
    return (
      <Tooltip
        placement="top"
        title="You can only add an exec to a search result.">
        <span>{addButton}</span>
      </Tooltip>
    );
  } else if (!isModal) {
    return addButton;
  } else {
    return null;
  }
};

const ManageExecutiveHoursAndDollarsGrid = (props: WrapperProps) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [openModal, setOpenModal] = useState<boolean>(false);
  const dispatch = useDispatch();

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const {
    executiveHoursAndDollars,
    additionalExecutivesChosen,
    additionalExecutivesGrid,
    executiveHoursAndDollarsGrid
  } = useSelector((state: RootState) => state.yearsEnd);

  // This function checks to see if we have a change for this badge number already pending for a save
  const isRowStagedToSave = (badge: number): boolean => {
    const found = executiveHoursAndDollarsGrid?.executiveHoursAndDollars.find(
      (obj: ExecutiveHoursAndDollarsRow) => obj.badgeNumber === badge
    );
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
  const columnDefs = useMemo(() => GetManageExecutiveHoursAndDollarsColumns(props.isModal), [props.isModal]);

  // We memoize this because we only want to copy this once as there will be differences
  // once edits are made
  const mutableCopyOfGridData = structuredClone(executiveHoursAndDollars);

  // Let us add in the any selected execs in redux
  if (additionalExecutivesChosen) {
    const mutableAdditionalExecutiveRows = structuredClone(additionalExecutivesChosen);
    console.log("First Exec row: " + mutableAdditionalExecutiveRows[0].fullName);
    mutableCopyOfGridData?.response.results.unshift(...mutableAdditionalExecutiveRows);
  }

  const mutableCopyOfAdditionalExecutivesGrid = useMemo(
    () => structuredClone(additionalExecutivesGrid),
    [additionalExecutivesGrid]
  );

  const isRowDataThere = (isModal: boolean | undefined): boolean => {
    if (isModal) {
      return (
        mutableCopyOfAdditionalExecutivesGrid?.response != null &&
        mutableCopyOfAdditionalExecutivesGrid?.response != undefined
      );
    } else {
      return mutableCopyOfGridData?.response != null && mutableCopyOfGridData?.response != undefined;
    }
  };

  const isPaginationNeeded = (isModal: boolean | undefined): boolean => {
    if (isModal) {
      return (
        !!mutableCopyOfAdditionalExecutivesGrid && mutableCopyOfAdditionalExecutivesGrid?.response?.results.length > 0
      );
    } else {
      return !!mutableCopyOfGridData && mutableCopyOfGridData.response.results.length > 0;
    }
  };

  return (
    <>
      {isRowDataThere(props.isModal) && (
        <>
          {!props.isModal && (
            <>
              <div className="px-[24px]">
                <Typography
                  variant="h2"
                  sx={{ color: "#0258A5" }}>
                  {`Manage Executive Hours and Dollars (${mutableCopyOfGridData?.response.total || 0})`}
                </Typography>
              </div>
              <div style={{ gap: "36px", display: "flex", justifyContent: "end", marginRight: 8 }}>
                <RenderAddExecutiveButton
                  reportReponse={mutableCopyOfGridData}
                  isModal={props.isModal}
                  setOpenModal={setOpenModal}
                />
              </div>
            </>
          )}
          {props.isModal && (
            <>
              <div className="px-[24px]">
                <Typography
                  variant="body1"
                  sx={{ color: "#db1532" }}>
                  {`Please select one executive and click the add button up top`}
                </Typography>
              </div>
            </>
          )}
          <DSMGrid
            preferenceKey={"DUPE_SSNS"}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: props.isModal
                ? mutableCopyOfAdditionalExecutivesGrid?.response.results
                : mutableCopyOfGridData?.response.results,
              columnDefs: columnDefs,
              rowSelection: props.isModal ? "single" : undefined,
              onSelectionChanged: (event: SelectionChangedEvent) => {
                dispatch(clearAdditionalExecutivesChosen());
                if (props.isModal) {
                  const selectedRows = event.api.getSelectedRows();
                  dispatch(setExecutiveRowsSelected(selectedRows));
                }
              },
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
      {isPaginationNeeded(props.isModal) && (
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
          recordCount={mutableCopyOfGridData?.response.total ?? 0}
        />
      )}

      <SmartModal
        open={openModal}
        onClose={() => setOpenModal(false)}>
        <SearchAndAddExecutive />
      </SmartModal>
    </>
  );
};

export default ManageExecutiveHoursAndDollarsGrid;
