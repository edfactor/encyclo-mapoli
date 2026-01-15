import { ColDef } from "ag-grid-community";
import { createBadgeColumn, createPSNColumn, createSSNColumn } from "../../../utils/gridColumnFactory";

export const PayBenReportGridColumn = (): ColDef[] => {
  return [
    createPSNColumn({
      enableLinking: true,
      linkingStyle: "badge-psn"
    }),
    createSSNColumn({ minWidth: 170, sortable: false }),
    {
      headerName: "Beneficiary",
      field: "beneficiaryFullName",
      colId: "beneficiaryFullName",
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    },

    createBadgeColumn({ minWidth: 120 }),
    {
      headerName: "Beneficiary of",
      field: "demographicFullName",
      colId: "demographicFullName",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    },
    {
      headerName: "Percent",
      field: "percentage",
      colId: "percentage",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    }
  ];
};
