import { ColDef, ICellRendererParams } from "ag-grid-community";
import { viewBadgeLinkRenderer } from "utils/masterInquiryLink";

export const PayBeNextColumnDef = (): ColDef[] => {
  return [
    {
      headerName: "Psn-Num",
      field: "badgeNumber",
      colId: "badgeNumber",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: true,
      unSortIcon: true,
      cellRenderer: (params: ICellRendererParams) =>
        viewBadgeLinkRenderer(params.data.badgeNumber, params.data.psnSuffix)
    },
    {
      headerName: "Name",
      field: "fullName",
      colId: "fullName",
      minWidth: 170,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: false
    },
    {
      headerName: "SSN",
      field: "ssn",
      colId: "ssn",
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => {
        return `${params.data.ssn}`;
      }
    },
    {
      headerName: "Relationship",
      field: "relationship",
      colId: "relationship",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    }
  ];
};

export const GetProfitDetailColumnDef = (
  addRowToSelectedRows: (id: number) => void,
  removeRowFromSelectedRows: (id: number) => void
) => {
  return [
    {
      headerName: "Year",
      field: "year",
      colId: "year",
      width: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false
    },
    {
      headerName: "Code",
      field: "code",
      colId: "code",
      width: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false
    },
    {
      headerName: "Contributions",
      field: "contributions",
      colId: "contributions",
      width: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false
    },
    {
      headerName: "Earnings",
      field: "earnings",
      colId: "earnings",
      width: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false
    },
    {
      headerName: "Forfeitures",
      field: "forfeitures",
      colId: "forfeitures",
      width: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false
    },
    {
      headerName: "Date",
      field: "date",
      colId: "date",
      width: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false
    },
    {
      headerName: "Comments",
      field: "comments",
      colId: "comments",
      width: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false
    }
  ];
};
