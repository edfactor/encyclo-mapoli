import { ColDef, ICellRendererParams } from "ag-grid-community";
import { viewBadgeLinkRenderer } from "utils/masterInquiryLink";

export const GetUnder21ReportColumns = (navFunction: (badgeNumber: string) => void): ColDef[] => {
  return [
    {
      headerName: "Badge",
      field: "badge",
      colId: "badge",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      cellRenderer: (params: ICellRendererParams) => viewBadgeLinkRenderer(params.data.badge, navFunction)
    },
    {
      headerName: "Full Name",
      field: "fullName",
      colId: "fullName",
      minWidth: 180,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "SSN",
      field: "ssn",
      colId: "ssn",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "P/S Years",
      field: "psYears",
      colId: "psYears",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "NE",
      field: "ne",
      colId: "ne",
      minWidth: 70,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "This Year P/S Hours",
      field: "thisYearPSHours",
      colId: "thisYearPSHours",
      minWidth: 150,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Last Year P/S Hours",
      field: "lastYearPSHours",
      colId: "lastYearPSHours",
      minWidth: 150,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Type",
      field: "type",
      colId: "type",
      minWidth: 80,
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
      resizable: true
    },
    {
      headerName: "Full Date",
      field: "fullDate",
      colId: "fullDate",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Term Date",
      field: "termDate",
      colId: "termDate",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Birth Date",
      field: "birthDate",
      colId: "birthDate",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Age",
      field: "age",
      colId: "age",
      minWidth: 70,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    }
  ];
};
