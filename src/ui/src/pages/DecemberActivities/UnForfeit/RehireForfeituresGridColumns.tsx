import { ColDef, ICellRendererParams, IHeaderParams } from "ag-grid-community";
import { agGridNumberToCurrency, formatNumberWithComma } from "smart-ui-library";
import { viewBadgeLinkRenderer } from "../../../utils/masterInquiryLink";
import { mmDDYYFormat } from "utils/dateUtils";
import { GRID_COLUMN_WIDTHS } from "../../../constants";
import { Checkbox, IconButton } from "@mui/material";
import { SaveOutlined } from "@mui/icons-material";
import { useState } from "react";
import { 
  RehireForfeituresHeaderComponentProps, 
  RehireForfeituresSaveButtonCellParams, 
  RehireForfeituresUpdatePayload 
} from "../../../reduxstore/types";
import { SuggestedForfeitEditor, SuggestedForfeitCellRenderer } from "../../../components/SuggestedForfeiture";

export const HeaderComponent: React.FC<RehireForfeituresHeaderComponentProps> = (props) => {
  const [allRowsSelected, setAllRowsSelected] = useState(false);

  const handleSelectAll = () => {
    if (allRowsSelected) {
      props.api.deselectAll();
      props.api.forEachNode(node => {
        if (node.data.isDetail) {
          const id = Number(node.id) || -1;
          props.removeRowFromSelectedRows(id);
        }
      });
    } else {
      props.api.forEachNode(node => {
        if (node.data.isDetail) {
          node.setSelected(true);
          const id = Number(node.id) || -1;
          props.addRowToSelectedRows(id);
        }
      });
    }
    props.api.refreshCells({ force: true });
    setAllRowsSelected(!allRowsSelected);
  };

  const handleSave = () => {
    const selectedNodes: RehireForfeituresUpdatePayload[] = [];
    props.api.forEachNode(node => {
      if (node.isSelected() && node.data.isDetail) {
        const rowKey = `${node.data.badgeNumber}-${node.data.profitYear}`;
        const currentValue = props.context?.editedValues?.[rowKey]?.value ?? node.data.suggestedForfeit;
        
        selectedNodes.push({
          badgeNumber: node.data.badgeNumber,
          profitYear: node.data.profitYear,
          suggestedForfeit: currentValue
        });
      }
    });
    console.log('Bulk update payload:', selectedNodes);
  };

  return (
    <div>
      <Checkbox 
        onClick={handleSelectAll}
        checked={allRowsSelected}
        onChange={handleSelectAll}
      />
      <IconButton onClick={handleSave}>
        <SaveOutlined />
      </IconButton>
    </div>
  );
};

export const GetMilitaryAndRehireForfeituresColumns = (): ColDef[] => {
  return [
    {
      headerName: "Badge",
      field: "badgeNumber",
      colId: "badgeNumber",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      unSortIcon: true,
      cellRenderer: (params: ICellRendererParams) => viewBadgeLinkRenderer(params.data.badgeNumber)
    },
    {
      headerName: "Full Name",
      field: "fullName",
      colId: "fullName",
      minWidth: GRID_COLUMN_WIDTHS.FULL_NAME,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      flex: 1
    },
    {
      headerName: "SSN",
      field: "ssn",
      colId: "ssn",
      minWidth: GRID_COLUMN_WIDTHS.SSN,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Hire Date",
      field: "hireDate",
      colId: "hireDate",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      valueFormatter: (params) => {
        const date = params.value;
        return mmDDYYFormat(date);
      }
    },
    {
      headerName: "Termination Date",
      field: "terminationDate",
      colId: "terminationDate",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      valueFormatter: (params) => {
        const date = params.value;
        return mmDDYYFormat(date);
      }
    },
    {
      headerName: "Rehired Date",
      field: "reHiredDate",
      colId: "reHiredDate",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      valueFormatter: (params) => {
        const date = params.value;
        return mmDDYYFormat(date);
      }
    },
    {
      headerName: "Beginning Balance",
      field: "netBalanceLastYear",
      colId: "netBalanceLastYear",
      minWidth: 150,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Vested Balance",
      field: "vestedBalanceLastYear",
      colId: "vestedBalanceLastYear",
      minWidth: 150,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      valueFormatter: agGridNumberToCurrency
    },    
    {
      headerName: "Store",
      field: "storeNumber",
      colId: "storeNumber",
      minWidth: 30,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
    }
  ];
};

export const GetDetailColumns = (addRowToSelectedRows: (id: number) => void, removeRowFromSelectedRows: (id: number) => void, selectedProfitYear: number): ColDef[] => {
  return [
    {
      headerName: "Profit Year",
      field: "profitYear",
      colId: "profitYear",
      width: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false
    },
    {
      headerName: "Hours",
      field: "hoursCurrentYear",
      colId: "hoursCurrentYear",
      width: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      valueFormatter: (params) => {
        const hours = params.value;
        return formatNumberWithComma(hours);
      }
    },    
    {
      headerName: "Wages",
      field: "wages",
      colId: "wages",
      width: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Contribution Years",
      field: "companyContributionYears",
      colId: "companyContributionYears",
      width: 100,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true
    },
    {
      headerName: "Enrollment",
      width: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      valueGetter: (params) => {
        const id = params.data?.enrollmentId;
        const name = params.data?.enrollmentName;        
        return `[${id}] ${name}`;
      }
    },
    {
      headerName: "Forfeiture",
      field: "forfeiture",
      colId: "forfeiture",
      width: 150,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Suggested Unforfeiture",
      field: "suggestedForfeit",
      colId: "suggestedForfeit",
      width: 150,
      headerClass: "right-align",
      cellClass: (params) => {
        if (!params.data.isDetail) return '';
        const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
        const hasError = params.context?.editedValues?.[rowKey]?.hasError;
        return `right-align ${hasError ? 'invalid-cell' : ''}`;
      },
      resizable: true,
      sortable: false,
      editable: ({ node }) => node.data.isDetail && node.data.profitYear === selectedProfitYear,
      cellEditor: SuggestedForfeitEditor,
      cellRenderer: (params: ICellRendererParams) => SuggestedForfeitCellRenderer({ ...params, selectedProfitYear }),
      valueFormatter: agGridNumberToCurrency,
      valueGetter: (params) => {
        if (!params.data.isDetail) return params.data.suggestedForfeit;
        const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
        const editedValue = params.context?.editedValues?.[rowKey]?.value;
        return editedValue !== undefined ? editedValue : params.data.suggestedForfeit;
      }
    },
    {
      headerName: "Remark",
      field: "remark",
      colId: "remark",
      width: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: false,
    },
    {
      headerName: "Save Button",
      field: "saveButton",
      colId: "saveButton",
      minWidth: 70,
      pinned: "right",
      lockPinned: true,
      resizable: false,
      sortable: false,
      cellStyle: { backgroundColor: '#E8E8E8' },
      headerComponent: HeaderComponent,
      headerComponentParams: {
        addRowToSelectedRows,
        removeRowFromSelectedRows
      },
      cellRendererParams: {
        addRowToSelectedRows,
        removeRowFromSelectedRows
      },
      cellRenderer: (params: RehireForfeituresSaveButtonCellParams) => {
        if (!params.data.isDetail || params.data.profitYear !== selectedProfitYear) {
          return '';
        }
        const id = Number(params.node?.id) || -1;
        const isSelected = params.node?.isSelected() || false;
        const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
        const hasError = params.context?.editedValues?.[rowKey]?.hasError;
        const currentValue = params.context?.editedValues?.[rowKey]?.value ?? params.data.suggestedForfeit;

        return <div>
          <Checkbox checked={isSelected} onChange={() => {
            if (isSelected) {
              params.removeRowFromSelectedRows(id);
              params.node?.setSelected(false);
            } else {
              params.addRowToSelectedRows(id);
              params.node?.setSelected(true);
            }
            params.api.refreshCells({ force: true });
          }} />
          <IconButton 
            onClick={() => {
              if (params.data.isDetail) {
                console.log('Update payload:', {
                  badgeNumber: params.data.badgeNumber,
                  profitYear: params.data.profitYear,
                  suggestedForfeit: currentValue
                });
              }
            }}
            disabled={hasError}
          >
            <SaveOutlined />
          </IconButton>
        </div>;
      }
    }
  ];
};