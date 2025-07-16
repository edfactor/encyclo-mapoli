import { ColDef, ICellRendererParams } from "ag-grid-community";
import { viewBadgeLinkRenderer } from "../../utils/masterInquiryLink";
import { yyyyMMDDToMMDDYYYY, agGridNumberToCurrency } from "smart-ui-library";
import { GRID_COLUMN_WIDTHS } from "../../constants";

export const GetProfitShareReportColumns = (): ColDef[] => {
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
      cellRenderer: (params: ICellRendererParams) => viewBadgeLinkRenderer(params.data.badgeNumber)
    },
    {
      headerName: "Full Name",
      field: "employeeName",
      colId: "employeeName",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Store",
      field: "storeNumber",
      colId: "storeNumber",
      minWidth: 60,
      type: "rightAligned",
      resizable: true
    },
    {
      headerName: "Employee Type",
      field: "employeeTypeCode",
      colId: "employeeTypeCode",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: (params) => {
        const id = params.data.employeeTypeCode;
        const name = params.data.employeeTypeName;
        return `${id} - ${name}`;
      }
    },
    {
      headerName: "Date of Birth",
      field: "dateOfBirth",
      colId: "dateOfBirth",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: (params) => (params.value ? yyyyMMDDToMMDDYYYY(params.value) : "")
    },
    {
      headerName: "Age",
      field: "age",
      colId: "age",
      minWidth: 50,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
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
      headerName: "Wages",
      field: "wages",
      colId: "wages",
      minWidth: 150,
      type: "rightAligned",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Hours",
      field: "hours",
      colId: "hours",
      minWidth: 150,
      type: "rightAligned",
      resizable: true
    },
    {
      headerName: "Points",
      field: "points",
      colId: "points",
      minWidth: 150,
      type: "rightAligned",
      resizable: true
    },
    {
      headerName: "",
      colId: "isNew",
      minWidth: 150,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => {
        return params.data.isUnder21 ? "< 21" : params.data.isNew ? "New" : "";
      }
    }
  ];
};
