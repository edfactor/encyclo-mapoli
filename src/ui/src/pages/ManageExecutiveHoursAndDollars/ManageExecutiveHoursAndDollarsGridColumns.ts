import { ColDef } from "ag-grid-community";

export const GetManageExecutiveHoursAndDollarsColumns = (): ColDef[] => {
  return [
    {
      headerName: "ID",
      field: "badgeNumber",
      colId: "badgeNumber",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true
    },
    {
      headerName: "Employee Name",
      field: "fullName",
      colId: "fullName",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
    },
    {
      headerName: "STR",
      field: "storeNumber",
      colId: "storeNumber",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
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
      editable: true
    },
    {
      headerName: "Executive Dollars",
      field: "incomeExecutive",
      colId: "incomeExecutive",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      editable: true
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
      resizable: true
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
        switch(params.value) { 
          case 'i': { 
             //statements;
             return "Inactive" ;
          }
          case 'a': { 
            //statements;
            return "Active" ;
          }
          case 't': { 
            //statements;
            return "Terminated" ;
          } 
          case 'd': { 
            //statements;
            return "Delete" ;
          } 
          default: { 
             //statements; 
             return "N/A";
          } 
        }
      }
    }
  ];
};