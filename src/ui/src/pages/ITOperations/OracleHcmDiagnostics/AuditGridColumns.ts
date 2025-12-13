import { ColDef } from "ag-grid-community";
import { mmDDYYYY_HHMMSS_Format } from "../../../utils/dateUtils";
import { createBadgeColumn } from "../../../utils/gridColumnFactory";

export const GetAuditGridColumns = (): ColDef[] => {
  return [
    createBadgeColumn({
      headerName: "Badge Number",
      field: "badgeNumber",
      minWidth: 120
    }),
    {
      headerName: "Property Name",
      field: "propertyName",
      sortable: true,
      filter: false,
      flex: 1,
      minWidth: 150
    },
    {
      headerName: "Message",
      field: "message",
      sortable: true,
      filter: false,
      flex: 2,
      minWidth: 250
    },
    {
      headerName: "Oracle HCM ID",
      field: "oracleHcmId",
      sortable: true,
      filter: false,
      width: 120
    },
    {
      headerName: "Created",
      field: "created",
      sortable: true,
      filter: false,
      width: 200,
      valueFormatter: (params: any) => (params.value ? mmDDYYYY_HHMMSS_Format(params.value) : "")
    }
  ];
};
