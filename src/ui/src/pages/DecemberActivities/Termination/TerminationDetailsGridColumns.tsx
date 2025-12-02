import { MAX_EMPLOYEE_BADGE_LENGTH } from "@/constants";
import { ColDef, ICellRendererParams } from "ag-grid-community";
import { numberToCurrency } from "smart-ui-library";
import { createSaveButtonCellRenderer } from "../../../components/ForfeitActivities";
import { SuggestedForfeitCellRenderer, SuggestedForfeitEditor } from "../../../components/SuggestedForfeiture";
import { ForfeitureAdjustmentUpdateRequest } from "../../../types";
import {
  createAgeColumn,
  createCurrencyColumn,
  createDateColumn,
  createHoursColumn,
  createYesOrNoColumn
} from "../../../utils/gridColumnFactory";
import { HeaderComponent } from "./TerminationHeaderComponent";

// Separate function for detail columns that will be used for master-detail view
export const GetDetailColumns = (
  addRowToSelectedRows: (id: number) => void,
  removeRowFromSelectedRows: (id: number) => void,
  selectedProfitYear: number,
  onSave?: (request: ForfeitureAdjustmentUpdateRequest, name: string) => Promise<void>,
  onBulkSave?: (requests: ForfeitureAdjustmentUpdateRequest[], names: string[]) => Promise<void>,
  isReadOnly = true
): ColDef[] => {
  return [
    createCurrencyColumn({
      headerName: "Beginning Balance",
      field: "beginningBalance",
      sortable: false
    }),
    createCurrencyColumn({
      headerName: "Beneficiary Allocation",
      field: "beneficiaryAllocation",
      sortable: false
    }),
    createCurrencyColumn({
      headerName: "Distribution Amount",
      field: "distributionAmount",
      sortable: false
    }),
    createCurrencyColumn({
      headerName: "Forfeit Amount",
      field: "forfeit",
      sortable: false
    }),
    createCurrencyColumn({
      headerName: "Ending Balance",
      field: "endingBalance",
      sortable: false
    }),
    createCurrencyColumn({
      headerName: "Vested Balance",
      field: "vestedBalance",
      sortable: false
    }),
    {
      headerName: "Vested %",
      field: "vestedPercent",
      colId: "vestedPercent",
      width: 100,
      type: "rightAligned",
      resizable: true,
      sortable: false,
      valueFormatter: (params) => `${params.value}%`
    },
    createDateColumn({
      headerName: "Term Date",
      field: "dateTerm",
      sortable: false
    }),
    createHoursColumn({
      headerName: "YTD PS Hours",
      field: "ytdPsHours",
      sortable: false
    }),
    createAgeColumn({
      maxWidth: 70,
      sortable: false
    }),
    createYesOrNoColumn({
      headerName: "Forfeited",
      field: "hasForfeited",
      colId: "hasForfeited",
      sortable: false
    }),
    {
      headerName: "Suggested Forfeit",
      field: "suggestedForfeit",
      colId: "suggestedForfeit",
      minWidth: 150,
      pinned: "right",
      type: "rightAligned",
      resizable: true,
      sortable: false,
      cellClass: (params) => {
        if (params.data.suggestedForfeit === null) return "";
        const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
        const hasError = params.context?.editedValues?.[rowKey]?.hasError;
        return hasError ? "bg-blue-50" : "";
      },
      editable: ({ node }) => node.data.suggestedForfeit !== null,
      flex: 1,
      cellEditor: SuggestedForfeitEditor,
      cellRenderer: (params: ICellRendererParams) => {
        // If the psn is longer than 7 chars, it is a beneficiary and should not have a suggested forfeit
        if (params.data.suggestedForfeit === null || params.data.psn.length > MAX_EMPLOYEE_BADGE_LENGTH) {
          return null;
        }
        // If value is 0, show blank
        const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
        const editedValue = params.context?.editedValues?.[rowKey]?.value;
        const currentValue = editedValue ?? params.data.suggestedForfeit;
        if (currentValue === 0) {
          return null;
        }
        return SuggestedForfeitCellRenderer(
          {
            ...params,
            selectedProfitYear
          },
          true,
          false
        );
      },
      valueFormatter: (params) => (params.value !== null && params.value !== 0 ? numberToCurrency(params.value) : ""),
      valueGetter: (params) => {
        const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
        const editedValue = params.context?.editedValues?.[rowKey]?.value;
        return editedValue ?? params.data.suggestedForfeit;
      }
    },
    {
      headerName: "Save Button",
      field: "saveButton",
      colId: "saveButton",
      minWidth: 130,
      width: 130,
      pinned: "right",
      lockPinned: true,
      resizable: false,
      sortable: false,
      cellStyle: { backgroundColor: "#E8E8E8" },
      headerComponent: HeaderComponent,
      valueGetter: (params) => {
        const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
        const editedValue = params.context?.editedValues?.[rowKey]?.value;
        const currentValue = editedValue ?? params.data.suggestedForfeit ?? 0;
        return `${currentValue}-${params.context?.loadingRowIds?.has(params.data.psn)}-${params.node?.isSelected()}`;
      },
      headerComponentParams: {
        addRowToSelectedRows,
        removeRowFromSelectedRows,
        onBulkSave,
        isReadOnly
      },
      cellRendererParams: {
        addRowToSelectedRows,
        removeRowFromSelectedRows,
        onSave
      },
      cellRenderer: createSaveButtonCellRenderer({
        activityType: "termination",
        selectedProfitYear,
        isReadOnly
      })
    }
  ];
};
