import { AddOutlined } from "@mui/icons-material";
import { Button, Tooltip, Typography } from "@mui/material";
import { CellValueChangedEvent, IRowNode, SelectionChangedEvent } from "ag-grid-community";
import React, { useCallback, useEffect, useMemo, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetExecutiveHoursAndDollarsQuery } from "reduxstore/api/YearsEndApi";
import {
  addExecutiveHoursAndDollarsGridRow,
  clearAdditionalExecutivesGrid,
  clearExecutiveRowsSelected,
  removeExecutiveHoursAndDollarsGridRow,
  setExecutiveRowsSelected,
  updateExecutiveHoursAndDollarsGridRow
} from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import {
  ExecutiveHoursAndDollars,
  ExecutiveHoursAndDollarsGrid,
  ExecutiveHoursAndDollarsRow,
  PagedReportResponse
} from "reduxstore/types";
import { DSMGrid, ISortParams, Pagination, SmartModal } from "smart-ui-library";
import ReportSummary from "../../../components/ReportSummary";
import { CAPTIONS } from "../../../constants";
import { GetManageExecutiveHoursAndDollarsColumns } from "./ManageExecutiveHoursAndDollarsGridColumns";
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
        //dispatch(clearAdditionalExecutivesChosen());
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
        title="You can only add an executive to search results.">
        <span>{addButton}</span>
      </Tooltip>
    );
  } else if (!isModal) {
    return addButton;
  } else {
    return null;
  }
};

interface ManageExecutiveHoursAndDollarsGridSearchProps {
  isModal?: boolean;
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  pageNumberReset: boolean;
  setPageNumberReset: (reset: boolean) => void;
}

const ManageExecutiveHoursAndDollarsGrid: React.FC<ManageExecutiveHoursAndDollarsGridSearchProps> = ({
  isModal,
  initialSearchLoaded,
  setInitialSearchLoaded,
  pageNumberReset,
  setPageNumberReset
}) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);

  // These are for the modal window
  const [pageAddNumber, setPageAddNumber] = useState(0);
  const [pageAddSize, setPageAddSize] = useState(25);

  let properPageNumber = pageNumber;
  let properPageSize = pageSize;

  let setProperPageNumber = setPageNumber;
  let setProperPageSize = setPageSize;

  if (isModal) {
    properPageNumber = pageAddNumber;
    properPageSize = pageAddSize;
    setProperPageNumber = setPageAddNumber;
    setProperPageSize = setPageAddSize;
  }

  const [openModal, setOpenModal] = useState<boolean>(false);
  const dispatch = useDispatch();
  const { executiveHoursAndDollarsQueryParams } = useSelector((state: RootState) => state.yearsEnd);

  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "storeNumber",
    isSortDescending: false
  });

  const {
    executiveHoursAndDollars,
    additionalExecutivesChosen,
    additionalExecutivesGrid,
    executiveHoursAndDollarsGrid
  } = useSelector((state: RootState) => state.yearsEnd);

  const [triggerSearch, { isFetching }] = useLazyGetExecutiveHoursAndDollarsQuery();

  const onSearch = useCallback(async () => {
    if (!executiveHoursAndDollarsQueryParams) return;

    const request = {
      profitYear: executiveHoursAndDollarsQueryParams.profitYear ?? 0,
      ...(executiveHoursAndDollarsQueryParams.badgeNumber && {
        badgeNumber: executiveHoursAndDollarsQueryParams.badgeNumber
      }),
      ...(executiveHoursAndDollarsQueryParams.socialSecurity !== null &&
        executiveHoursAndDollarsQueryParams.socialSecurity !== 0 && {
          socialSecurity: executiveHoursAndDollarsQueryParams.socialSecurity
        }),
      ...(executiveHoursAndDollarsQueryParams.fullNameContains && {
        fullNameContains: executiveHoursAndDollarsQueryParams.fullNameContains
      }),
      hasExecutiveHoursAndDollars: executiveHoursAndDollarsQueryParams.hasExecutiveHoursAndDollars ?? false,
      isMonthlyPayroll: executiveHoursAndDollarsQueryParams.isMonthlyPayroll ?? false,
      pagination: {
        skip: properPageNumber * properPageSize,
        take: properPageSize,
        sortBy: sortParams.sortBy,
        isSortDescending: sortParams.isSortDescending
      }
    };
    await triggerSearch(request, false);
  }, [executiveHoursAndDollarsQueryParams, properPageNumber, properPageSize, sortParams, triggerSearch]);

  useEffect(() => {
    if (initialSearchLoaded) {
      onSearch();
    }
  }, [initialSearchLoaded, properPageNumber, properPageSize, sortParams, onSearch]);

  // Need a useEffect on a change in executiveHoursAndDollars to reset the page number

  useEffect(() => {
    if (pageNumberReset) {
      setPageNumber(0);
      setPageNumberReset(false);
    }
  }, [pageNumberReset, setPageNumberReset]);

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

    // We need to make sure that the hours and dollars are not too large
    // At this time, we are limiting hours to less than 4000 and dollars
    // to be less than 20 million

    if (rowInQuestion.data.hoursExecutive > 4000) {
      // If the new value is invalid, we need to go through the executiveHoursAndDollars object,
      // find the badgeNumber equal to rowInQuestion.data.badgeNumber, find the value of hoursExecutive
      // in that object, and set it to the value of rowInQuestion.data.hoursExecutive
      rowInQuestion.data.hoursExecutive = executiveHoursAndDollars?.response.results.find(
        (obj) => obj.badgeNumber === rowInQuestion.data.badgeNumber
      )?.hoursExecutive;
      event.api.refreshCells({ force: true }); // Needed to refresh the grid cells to restore the old value
      return;
    }
    if (rowInQuestion.data.incomeExecutive > 20000000) {
      rowInQuestion.data.incomeExecutive = executiveHoursAndDollars?.response.results.find(
        (obj) => obj.badgeNumber === rowInQuestion.data.badgeNumber
      )?.incomeExecutive;
      event.api.refreshCells({ force: true });
      return;
    }

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
  const columnDefs = useMemo(() => GetManageExecutiveHoursAndDollarsColumns(isModal), [isModal]);

  const combineGridWithAddedExecs = (
    mainList: PagedReportResponse<ExecutiveHoursAndDollars> | null,
    additionalResults: ExecutiveHoursAndDollars[] | null
  ): PagedReportResponse<ExecutiveHoursAndDollars> | null => {
    const mainGridStructureCopy = structuredClone(mainList);
    const additionalResultsCopy = structuredClone(additionalResults);

    if (!mainGridStructureCopy || !mainGridStructureCopy.response || !mainGridStructureCopy.response.results) {
      return null;
    }

    if (!additionalResultsCopy || additionalResultsCopy.length === 0) {
      return mainGridStructureCopy;
    }

    // We should only add people to the main grid if they are not already there
    const existingBadgeNumbers = new Set(mainGridStructureCopy.response.results.map((item) => item.badgeNumber));
    const filteredAdditionalResults = additionalResultsCopy.filter(
      (item) => !existingBadgeNumbers.has(item.badgeNumber)
    );
    // Now we can add the filtered additional results to the main grid structure
    mainGridStructureCopy.response.results = mainGridStructureCopy.response.results.concat(filteredAdditionalResults);

    return mainGridStructureCopy;
  };

  // We memoize this not just for performance, but also because we need to
  // add in any execs chosen in the modal window, and also, because if we
  // do not memoize it, editing a column value will cause the grid to re-render
  // with the original values, even though the underlying data has changed

  const mutableCopyOfGridData = useMemo(
    () => combineGridWithAddedExecs(executiveHoursAndDollars, additionalExecutivesChosen),
    [executiveHoursAndDollars, additionalExecutivesChosen]
  );

  const isRowDataThere = (isModal: boolean | undefined): boolean => {
    if (isModal) {
      return additionalExecutivesGrid?.response != null;
    } else {
      return mutableCopyOfGridData?.response != null && executiveHoursAndDollars?.response?.results != null;
    }
  };

  // This function checks for the need to have pagination for modal and non modal grids
  const isPaginationNeeded = (isModal: boolean | undefined): boolean => {
    if (isModal) {
      return !!additionalExecutivesGrid && additionalExecutivesGrid?.response?.results.length > 0;
    } else {
      return !!mutableCopyOfGridData && mutableCopyOfGridData.response.results.length > 0;
    }
  };

  return (
    <>
      {isRowDataThere(isModal) && (
        <>
          {!isModal && (
            <>
              <div className="px-[24px]">
                <ReportSummary report={mutableCopyOfGridData} />
              </div>
              <div style={{ gap: "36px", display: "flex", justifyContent: "end", marginRight: 28 }}>
                <RenderAddExecutiveButton
                  reportReponse={mutableCopyOfGridData}
                  isModal={isModal}
                  setOpenModal={setOpenModal}
                />
              </div>
            </>
          )}
          {isModal && (
            <>
              <div className="px-[24px]">
                <Typography
                  variant="body1"
                  sx={{ color: "#db1532" }}>
                  {`Please select rows then click the add button up top`}
                </Typography>
              </div>
            </>
          )}
          <DSMGrid
            preferenceKey={CAPTIONS.MANAGE_EXECUTIVE_HOURS}
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: isModal ? additionalExecutivesGrid?.response.results : mutableCopyOfGridData?.response.results,
              columnDefs: columnDefs,
              suppressMultiSort: true,
              rowSelection: isModal ? "multiple" : undefined,
              onSelectionChanged: (event: SelectionChangedEvent) => {
                if (isModal) {
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
      {isPaginationNeeded(isModal) && (
        <Pagination
          pageNumber={properPageNumber}
          setPageNumber={(value: number) => {
            setProperPageNumber(value - 1);
            setInitialSearchLoaded(true);
          }}
          pageSize={properPageSize}
          setPageSize={(value: number) => {
            setProperPageSize(value);
            setProperPageNumber(1);
            setInitialSearchLoaded(true);
          }}
          recordCount={
            isModal ? (additionalExecutivesGrid?.response.total ?? 0) : (mutableCopyOfGridData?.response.total ?? 0)
          }
        />
      )}
      <SmartModal
        open={openModal}
        onClose={() => setOpenModal(false)}>
        <SearchAndAddExecutive
          setOpenModal={setOpenModal}
          initialSearchLoaded={initialSearchLoaded}
          setInitialSearchLoaded={setInitialSearchLoaded}
          pageNumberReset={pageNumberReset}
          setPageNumberReset={setPageNumberReset}
        />
      </SmartModal>
    </>
  );
};

export default ManageExecutiveHoursAndDollarsGrid;
