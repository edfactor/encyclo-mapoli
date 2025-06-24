import { ColDef, ICellRendererParams, IHeaderParams } from "ag-grid-community";
import { agGridNumberToCurrency, formatNumberWithComma } from "smart-ui-library";
import { viewBadgeLinkRenderer } from "../../../utils/masterInquiryLink";
import { getEnrolledStatus } from "../../../utils/enrollmentUtil";
import { mmDDYYFormat } from "utils/dateUtils";
import { Checkbox, IconButton } from "@mui/material";
import { SaveOutlined } from "@mui/icons-material";
import { useState } from "react";

export const GetTerminationColumns = (): ColDef[] => {
  return [
    {
      headerName: "Badge",
      field: "badgePSn",
      colId: "badgePSn",
      width: 100,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      cellRenderer: (params: ICellRendererParams) => viewBadgeLinkRenderer(params.data.badgeNumber, params.data.psnSuffix)
    },
    {
      headerName: "Name",
      field: "name",
      colId: "name",
      width: 200,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      flex: 1,
    }
  ];
};

// Separate function for detail columns that will be used for master-detail view
export const GetDetailColumns = (addRowToSelectedRows: (id: number) => void, removeRowFromSelectedRows: (id: number) => void, selectedRowIds: number[]): ColDef[] => {
  return [
    {
      headerName: "Profit Year",
      field: "profitYear",
      colId: "profitYear",
      width: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false,
    },
    {
      headerName: "Beginning Balance",
      field: "beginningBalance",
      colId: "beginningBalance",
      width: 150,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Beneficiary Allocation",
      field: "beneficiaryAllocation",
      colId: "beneficiaryAllocation",
      width: 150,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Distribution Amount",
      field: "distributionAmount",
      colId: "distributionAmount",
      width: 150,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Forfeit",
      field: "forfeit",
      colId: "forfeit",
      width: 125,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Ending Balance",
      field: "endingBalance",
      colId: "endingBalance",
      width: 150,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Vested Balance",
      field: "vestedBalance",
      colId: "vestedBalance",
      width: 150,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Vested %",
      field: "vestedPercent",
      colId: "vestedPercent",
      width: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false,
      valueFormatter: (params) => `${params.value}%`
    },
    {
      headerName: "Term Date",
      field: "dateTerm",
      colId: "dateTerm",
      width: 150,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: false,
      valueFormatter: (params) => {
        const date = params.value;
        return mmDDYYFormat(date);
      }
    },
    {
      headerName: "YTD PS Hours",
      field: "ytdPsHours",
      colId: "ytdPsHours",
      width: 125,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false,
      valueFormatter: (params) => {
        const hours = params.value;
        return formatNumberWithComma(hours);
      }
    },
    {
      headerName: "Age",
      field: "age",
      colId: "age",
      width: 70,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false
    },
    {
      headerName: "Enrollment",
      field: "enrollmentCode",
      colId: "enrollmentCode",
      width: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: false,
      valueFormatter: (params) => getEnrolledStatus(params.value)
    },
    {
      headerName: "Suggested Forfeit",
      field: "suggestedForfeit",
      colId: "suggestedForfeit",
      minWidth: 150,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false,
      editable: ({ node }) => node.data.isDetail,
      flex: 1,
      valueFormatter: agGridNumberToCurrency,
      cellRenderer: (params: ICellRendererParams) => {
        if (!params.data.isDetail) {
          return '';
        }
        return params.valueFormatted || params.value;
      }
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
      headerComponent: HeaderComponent,
      headerComponentParams: {
        addRowToSelectedRows,
        removeRowFromSelectedRows
      },
      cellRendererParams: {
        addRowToSelectedRows,
        removeRowFromSelectedRows
      },
      cellRenderer: (params: SaveButtonCellParams) => {
        const id = Number(params.node?.id) || -1;
        const isSelected = params.node?.isSelected() || false;
        return <div>
          <Checkbox checked={isSelected} onChange={() => {
            if (isSelected) {
              params.removeRowFromSelectedRows(id);
            } else {
              params.addRowToSelectedRows(id);
            }
            params.node?.setSelected(!isSelected);
          }} />
          <IconButton onClick={() => {
            if (params.data.isDetail) {
              console.log('Update payload:', {
                badgeNumber: params.data.badgeNumber,
                profitYear: params.data.profitYear,
                suggestedForfeit: params.data.suggestedForfeit
              });
            }
          }}>
            <SaveOutlined />
          </IconButton>
        </div>;
      }
    }
  ];
};

interface HeaderComponentProps extends IHeaderParams {
  addRowToSelectedRows: (id: number) => void;
  removeRowFromSelectedRows: (id: number) => void;
}

interface SaveButtonCellParams extends ICellRendererParams {
  removeRowFromSelectedRows: (id: number) => void;
  addRowToSelectedRows: (id: number) => void;
}

interface UpdatePayload {
  badgeNumber: string;
  profitYear: number;
  suggestedForfeit: number;
}

export const HeaderComponent: React.FC<HeaderComponentProps> = (params: HeaderComponentProps) => {
  const [allRowsSelected, setAllRowsSelected] = useState(false);

  const handleSelectAll = () => {
    if (allRowsSelected) {
      params.api.deselectAll();
      params.api.forEachNode(node => {
        if (node.data.isDetail) {
          const id = Number(node.id) || -1;
          params.removeRowFromSelectedRows(id);
        }
      });
    } else {
      params.api.forEachNode(node => {
        if (node.data.isDetail) {
          node.setSelected(true);
          const id = Number(node.id) || -1;
          params.addRowToSelectedRows(id);
        }
      });
    }
    params.api.refreshCells({ columns: ['saveButton'] });
    setAllRowsSelected(!allRowsSelected);
  };

  const handleSave = () => {
    const selectedNodes: UpdatePayload[] = [];
    params.api.forEachNode(node => {
      if (node.isSelected() && node.data.isDetail) {
        selectedNodes.push({
          badgeNumber: node.data.badgeNumber,
          profitYear: node.data.profitYear,
          suggestedForfeit: node.data.suggestedForfeit
        });
      }
    });
    console.log('Bulk update payload:', selectedNodes);
  };

  return <div>
    <Checkbox 
      onClick={handleSelectAll}
      checked={allRowsSelected}
      onChange={handleSelectAll}
    />
    <IconButton onClick={handleSave}>
      <SaveOutlined />
    </IconButton>
  </div>;
};