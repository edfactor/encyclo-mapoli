import { ColDef } from "ag-grid-community";
import { createBadgeColumn, createNameColumn, createStoreColumn, createZipColumn } from "../../utils/gridColumnFactory";

export const GetProfallGridColumns = (navFunction: (badgeNumber: string) => void): ColDef[] => {
  return [
    createStoreColumn({
      minWidth: 80,
      sortable: true
    }),
    createBadgeColumn({
      headerName: "Badge",

      navigateFunction: navFunction
    }),
    createNameColumn({
      field: "employeeName",
      minWidth: 180,
      sortable: true
    }),
    {
      headerName: "Department",
      field: "departmentName",
      colId: "departmentName",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true
    },
    {
      headerName: "Classification",
      field: "payClassificationName",
      colId: "payClassificationName",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true
    },
    {
      headerName: "Address",
      field: "address1",
      colId: "address1",
      minWidth: 200,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true
    },
    {
      headerName: "City",
      field: "city",
      colId: "city",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true
    },
    {
      headerName: "State",
      field: "state",
      colId: "state",
      minWidth: 80,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: true
    },
    createZipColumn({
      field: "postalCode",
      minWidth: 100,
      alignment: "center"
    })
  ];
};
