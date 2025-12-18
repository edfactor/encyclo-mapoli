import { AddOutlined } from "@mui/icons-material";
import { Button, Tooltip, Typography } from "@mui/material";
import { CellValueChangedEvent, IRowNode, SelectionChangedEvent } from "ag-grid-community";
import React, { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { DSMGrid, Pagination, SmartModal } from "smart-ui-library";
import ReportSummary from "../../../components/ReportSummary";
import { GRID_KEYS } from "../../../constants";
import { useContentAwareGridHeight } from "../../../hooks/useContentAwareGridHeight";
import { GridPaginationActions, GridPaginationState } from "../../../hooks/useGridPagination";
import { ExecutiveHoursAndDollars, PagedReportResponse } from "../../../reduxstore/types";
import { GetManageExecutiveHoursAndDollarsColumns } from "./ManageExecutiveHoursAndDollarsGridColumns";
import SearchAndAddExecutive from "./SearchAndAddExecutive";

interface ManageExecutiveSearchForm {
  [key: string]: unknown;
}

interface RenderAddExecutiveButtonProps {
  reportReponse: PagedReportResponse<ExecutiveHoursAndDollars> | null;
  isModal?: boolean;
  onOpenModal: () => void;
  isReadOnly?: boolean;
}

const RenderAddExecutiveButton: React.FC<RenderAddExecutiveButtonProps> = ({
  reportReponse,
  isModal,
  onOpenModal,
  isReadOnly = false
}) => {
  // We cannot add an employee if there is no result set there
  const gridAvailable: boolean = reportReponse?.response != null && reportReponse?.response != undefined;
  const isDisabled = !gridAvailable || isReadOnly;

  const addButton = (
    <Button
      disabled={isDisabled}
      variant="outlined"
      color="secondary"
      size="medium"
      startIcon={<AddOutlined color={isDisabled ? "disabled" : "secondary"} />}
      onClick={isReadOnly ? undefined : () => onOpenModal()}>
      Add Executive
    </Button>
  );

  if (!isModal && isDisabled) {
    const tooltipTitle = isReadOnly
      ? "You are in read-only mode and cannot add executives."
      : "You can only add an executive to search results.";

    return (
      <Tooltip
        placement="top"
        title={tooltipTitle}>
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
  // Props for main grid (not needed when isModal=true)
  gridData?: PagedReportResponse<ExecutiveHoursAndDollars> | null;
  isModalOpen?: boolean;
  openModal?: () => void;
  closeModal?: () => void;
  updateExecutiveRow?: (badge: number, hours: number, dollars: number) => void;
  isRowStagedToSave?: (badge: number) => boolean;
  mainGridPagination?: GridPaginationState & GridPaginationActions;
  executeModalSearch?: (searchForm: ManageExecutiveSearchForm) => void;
  modalSelectedExecutives?: ExecutiveHoursAndDollars[];
  addExecutivesToMainGrid?: () => void;
  isModalSearching?: boolean;
  isReadOnly?: boolean;
  // canEdit: true only when page status is "In Progress" AND user has ExecutiveAdministrator role
  canEdit?: boolean;
  // Props for modal grid (not needed when isModal=false)
  modalResults?: PagedReportResponse<ExecutiveHoursAndDollars> | null;
  modalGridPagination?: GridPaginationState & GridPaginationActions;
  // Shared props
  isSearching: boolean;
  selectExecutivesInModal?: (executives: ExecutiveHoursAndDollars[]) => void;
}

const ManageExecutiveHoursAndDollarsGrid: React.FC<ManageExecutiveHoursAndDollarsGridSearchProps> = ({
  isModal = false,
  gridData = null,
  modalResults = null,
  isSearching,
  isModalOpen = false,
  openModal = () => {},
  closeModal = () => {},
  selectExecutivesInModal = () => {},
  updateExecutiveRow = () => {},
  isRowStagedToSave = () => false,
  mainGridPagination = null,
  modalGridPagination = null,
  // Modal-specific props
  executeModalSearch = () => {},
  modalSelectedExecutives = [],
  addExecutivesToMainGrid = () => {},
  isModalSearching = false,
  isReadOnly = false,
  canEdit = false
}) => {
  const currentData = isModal ? modalResults : gridData;
  const currentPagination = isModal ? modalGridPagination : mainGridPagination;
  const sortEventHandler = currentPagination?.handleSortChange;
  const gridMaxHeight = useContentAwareGridHeight({
    rowCount: currentData?.response?.results?.length ?? 0
  });
  // Pass canEdit to column definitions - editing requires page status "In Progress" AND ExecutiveAdministrator role
  const columnDefs = useMemo(
    () => GetManageExecutiveHoursAndDollarsColumns({ mini: isModal, canEdit }),
    [isModal, canEdit]
  );

  const processEditedRow = useCallback(
    (event: CellValueChangedEvent) => {
      // Safety guard: prevent editing if canEdit is false
      // (columns should already be non-editable, but this is a safety measure)
      if (!canEdit && !isModal) {
        event.api.refreshCells({ force: true });
        return;
      }

      const rowInQuestion: IRowNode = event.node;

      // Mark that we're editing to prevent data resets
      isEditingRef.current = true;

      // Validate hours and dollars limits
      if (rowInQuestion.data.hoursExecutive > 4000) {
        // Find original value to restore
        const originalRow = currentData?.response.results.find(
          (obj) => obj.badgeNumber === rowInQuestion.data.badgeNumber
        );
        if (originalRow) {
          rowInQuestion.data.hoursExecutive = originalRow.hoursExecutive;
          event.api.refreshCells({ force: true });
          isEditingRef.current = false;
          return;
        }
      }

      if (rowInQuestion.data.incomeExecutive > 20000000) {
        // Find original value to restore
        const originalRow = currentData?.response.results.find(
          (obj) => obj.badgeNumber === rowInQuestion.data.badgeNumber
        );
        if (originalRow) {
          rowInQuestion.data.incomeExecutive = originalRow.incomeExecutive;
          event.api.refreshCells({ force: true });
          isEditingRef.current = false;
          return;
        }
      }

      // Update the local mutable row data to persist the change in the grid
      setMutableRowData((prevData) =>
        prevData.map((row) =>
          row.badgeNumber === rowInQuestion.data.badgeNumber
            ? {
                ...row,
                hoursExecutive: rowInQuestion.data.hoursExecutive,
                incomeExecutive: rowInQuestion.data.incomeExecutive
              }
            : row
        )
      );

      // Update row if not in modal - this tracks changes for saving
      if (!isModal) {
        updateExecutiveRow(
          rowInQuestion.data.badgeNumber,
          rowInQuestion.data.hoursExecutive,
          rowInQuestion.data.incomeExecutive
        );
      }

      // Reset editing flag after a small delay
      setTimeout(() => {
        isEditingRef.current = false;
      }, 100);
    },
    [isModal, currentData, updateExecutiveRow, canEdit]
  );
  const hasData = Boolean(currentData?.response?.results && currentData.response.results.length > 0);
  const isPaginationNeeded = hasData;

  // Create a mutable copy of the row data for the grid to allow in-place editing
  const [mutableRowData, setMutableRowData] = useState<ExecutiveHoursAndDollars[]>([]);
  const isEditingRef = useRef(false);
  const dataInitializedRef = useRef(false);

  // Initialize mutable row data when we first get data or when we get new search results
  useEffect(() => {
    if (currentData?.response?.results && !dataInitializedRef.current && !isEditingRef.current) {
      setMutableRowData(currentData.response.results.map((row) => ({ ...row })));
      dataInitializedRef.current = true;
    } else if (!currentData?.response?.results && !isEditingRef.current) {
      setMutableRowData([]);
      dataInitializedRef.current = false;
    }
  }, [currentData]);

  // Update mutable row data when combined data changes (e.g., when executives are added from modal)
  const lastDataLengthRef = useRef<number>(0);
  useEffect(() => {
    if (currentData?.response?.results && dataInitializedRef.current && !isEditingRef.current) {
      const currentLength = currentData.response.results.length;
      if (currentLength !== lastDataLengthRef.current) {
        setMutableRowData(currentData.response.results.map((row) => ({ ...row })));
        lastDataLengthRef.current = currentLength;
      }
    } else if (currentData?.response?.results && dataInitializedRef.current) {
      lastDataLengthRef.current = currentData.response.results.length;
    }
  }, [currentData?.response?.results]);

  // Don't render anything if we don't have data (for modal)
  // For main grid, the parent component controls visibility
  if (isModal && !hasData) {
    return null;
  }

  return (
    <>
      <>
        {!isModal && (
          <>
            <div
              className="px-[24px]"
              style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
              {gridData && <ReportSummary report={gridData} />}
              <RenderAddExecutiveButton
                reportReponse={gridData}
                isModal={isModal}
                onOpenModal={openModal}
                isReadOnly={isReadOnly}
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
          preferenceKey={GRID_KEYS.MANAGE_EXECUTIVE_HOURS}
          isLoading={isSearching}
          handleSortChanged={sortEventHandler}
          maxHeight={gridMaxHeight}
          providedOptions={{
            rowData: mutableRowData,
            columnDefs: columnDefs,
            suppressMultiSort: true,
            rowSelection: isModal
              ? {
                  mode: "multiRow",
                  checkboxes: true,
                  headerCheckbox: true,
                  enableClickSelection: false
                }
              : undefined,
            onSelectionChanged: (event: SelectionChangedEvent) => {
              if (isModal) {
                const selectedRows = event.api.getSelectedRows();
                selectExecutivesInModal(selectedRows);
              }
            },
            onCellValueChanged: processEditedRow,
            getRowStyle: (params) => {
              // Rows with unsaved changes will have yellow color
              if (
                !isModal &&
                params.node.data &&
                isRowStagedToSave((params.node.data as ExecutiveHoursAndDollars).badgeNumber)
              ) {
                return { background: "lemonchiffon" };
              } else {
                return { background: "white" };
              }
            }
          }}
        />
      </>
      {isPaginationNeeded && currentPagination && (
        <Pagination
          pageNumber={currentPagination.pageNumber}
          setPageNumber={(value: number) => currentPagination.handlePageNumberChange(value - 1)}
          pageSize={currentPagination.pageSize}
          setPageSize={currentPagination.handlePageSizeChange}
          recordCount={currentData?.response.total ?? 0}
        />
      )}
      {!isModal && modalGridPagination && (
        <SmartModal
          open={isModalOpen}
          onClose={closeModal}>
          <SearchAndAddExecutive
            executeModalSearch={executeModalSearch}
            modalSelectedExecutives={modalSelectedExecutives}
            addExecutivesToMainGrid={addExecutivesToMainGrid}
            isModalSearching={isModalSearching}
            modalResults={modalResults}
            selectExecutivesInModal={selectExecutivesInModal}
            modalGridPagination={modalGridPagination}
            isReadOnly={isReadOnly}
          />
        </SmartModal>
      )}
    </>
  );
};

export default ManageExecutiveHoursAndDollarsGrid;
