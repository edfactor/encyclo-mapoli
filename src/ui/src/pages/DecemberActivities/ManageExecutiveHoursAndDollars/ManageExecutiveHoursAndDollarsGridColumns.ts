import { ColDef, ICellRendererParams } from "ag-grid-community";
import { viewBadgeLinkRenderer } from "../../../utils/masterInquiryLink";
import { agGridNumberToCurrency } from "smart-ui-library";

// The default is to show all columns, but if the mini flag is set to true, only show the
// badge, name, and ssn columns
export const GetManageExecutiveHoursAndDollarsColumns = (mini?: boolean): ColDef[] => {
  const columns: ColDef[] = [
    {
      headerName: "Badge",
      field: "badgeNumber",
      colId: "badgeNumber",
      minWidth: 80,
      headerClass: mini ? "left-align" : "right-align",
      cellClass: mini ? "left-align" : "right-align",
      resizable: true,
      sortable: true,
      checkboxSelection: mini,
      cellRenderer: (params: ICellRendererParams) => viewBadgeLinkRenderer(params.data.badgeNumber)
    },
    {
      headerName: "Employee Name",
      field: "fullName",
      colId: "fullName",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "STR",
      field: "storeNumber",
      colId: "storeNumber",
      minWidth: 120,
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
      headerName: "Executive Hours",
      field: "hoursExecutive",
      colId: "hoursExecutive",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      editable: !mini
    },
    {
      headerName: "Executive Dollars",
      field: "incomeExecutive",
      colId: "incomeExecutive",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      editable: !mini,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "ORA HRS LAST",
      field: "currentHoursYear",
      colId: "currentHoursYear",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "ORA DOLS LAST",
      field: "currentIncomeYear",
      colId: "currentIncomeYear",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "FREQ",
      field: "payFrequencyId",
      colId: "payFrequencyId",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "STATUS",
      field: "employmentStatusId",
      colId: "employmentStatusId",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: (params) => {
        switch (params.value) {
          case "i": {
            //statements;
            return "Inactive";
          }
          case "a": {
            //statements;
            return "Active";
          }
          case "t": {
            //statements;
            return "Terminated";
          }
          case "d": {
            //statements;
            return "Delete";
          }
          default: {
            //statements;
            return "N/A";
          }
        }
      }
    }
  ];

  // We could have a hide property in elements to be hidden and not filter this way,
  // but in the modal, the column selection panel would show them
  if (mini) {
    return columns.filter(
      (column) =>
        column.colId === "badgeNumber" ||
        column.colId === "fullName" ||
        column.colId === "ssn" ||
        column.colId === "hoursExecutive" ||
        column.colId === "incomeExecutive"
    );
  }
  return columns;
};
