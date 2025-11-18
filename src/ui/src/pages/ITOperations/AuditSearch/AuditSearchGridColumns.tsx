import { ColDef, ICellRendererParams } from "ag-grid-community";

export const GetAuditSearchColumns = (): ColDef[] => {
  return [
    {
      headerName: "Audit Event ID",
      field: "auditEventId",
      sortable: true,
      filter: "agNumberColumnFilter",
      width: 150
    },
    {
      headerName: "Table Name",
      field: "tableName",
      sortable: true,
      filter: "agTextColumnFilter",
      width: 200
    },
    {
      headerName: "Operation",
      field: "operation",
      sortable: true,
      filter: "agTextColumnFilter",
      width: 150
    },
    {
      headerName: "Primary Key",
      field: "primaryKey",
      sortable: true,
      filter: "agTextColumnFilter",
      width: 150
    },
    {
      headerName: "User Name",
      field: "userName",
      sortable: true,
      filter: "agTextColumnFilter",
      width: 200
    },
    {
      headerName: "Created At",
      field: "createdAt",
      sortable: true,
      filter: "agDateColumnFilter",
      width: 200,
      valueFormatter: (params) => {
        if (!params.value) return "";
        const date = new Date(params.value);
        return date.toLocaleString("en-US", {
          year: "numeric",
          month: "2-digit",
          day: "2-digit",
          hour: "2-digit",
          minute: "2-digit",
          second: "2-digit",
          hour12: true
        });
      }
    },
    {
      headerName: "Changes",
      field: "changesJson",
      sortable: false,
      filter: false,
      width: 300,
      autoHeight: true,
      wrapText: true,
      cellRenderer: (params: ICellRendererParams) => {
        const changes = params.value;
        if (!changes || !Array.isArray(changes) || changes.length === 0) {
          return '<span style="color: #999;">No changes</span>';
        }

        const changesHtml = changes
          .map((change) => {
            const original = change.originalValue || "(null)";
            const newVal = change.newValue || "(null)";
            return `
              <div style="margin: 4px 0; padding: 4px; background: #f5f5f5; border-radius: 3px;">
                <strong>${change.columnName}:</strong><br/>
                <span style="color: #d32f2f;">${original}</span> → 
                <span style="color: #388e3c;">${newVal}</span>
              </div>
            `;
          })
          .join("");

        return `<div style="padding: 4px 0;">${changesHtml}</div>`;
      }
    }
  ];
};
