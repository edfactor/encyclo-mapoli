import { ColDef, ICellRendererParams } from "ag-grid-community";
import { NavigationStatusDto } from "../../../types";
import { createCountColumn, createDateColumn, createNameColumn } from "../../../utils/gridColumnFactory";

interface ChangesCellRendererProps extends ICellRendererParams {
  navigationStatusLookup: Map<number, string>;
  tableName: string;
}

const ChangesCellRenderer = (params: ChangesCellRendererProps) => {
  const changes = params.value;
  const { navigationStatusLookup, tableName } = params;

  if (!changes || !Array.isArray(changes) || changes.length === 0) {
    return <span style={{ color: "#999" }}>No changes</span>;
  }

  const translateValue = (value: string | null | undefined, columnName: string): string => {
    if (!value) return "(null)";

    // For NAVIGATION table, translate StatusId column values
    if (tableName?.toUpperCase() === "NAVIGATION" && columnName?.toUpperCase() === "STATUSID") {
      const numericValue = parseInt(value, 10);
      if (!isNaN(numericValue) && navigationStatusLookup.has(numericValue)) {
        return navigationStatusLookup.get(numericValue) || value;
      }
    }

    return value;
  };

  return (
    <div style={{ padding: "4px 0" }}>
      {changes.map((change, index) => {
        const original = translateValue(change.originalValue, change.columnName);
        const newVal = translateValue(change.newValue, change.columnName);
        return (
          <div
            key={index}
            style={{
              margin: "4px 0",
              padding: "4px",
              background: "#f5f5f5",
              borderRadius: "3px"
            }}>
            <strong>{change.columnName}:</strong>
            <br />
            <span style={{ color: "#d32f2f" }}>{original}</span> → <span style={{ color: "#388e3c" }}>{newVal}</span>
          </div>
        );
      })}
    </div>
  );
};

export const GetAuditSearchColumns = (navigationStatusList: NavigationStatusDto[]): ColDef[] => {
  // Create a lookup map for navigation status IDs to names
  const navigationStatusLookup = new Map<number, string>();
  navigationStatusList.forEach((status) => {
    navigationStatusLookup.set(status.id, status.name || `Status ${status.id}`);
  });

  return [
    createCountColumn({
      headerName: "Audit Event ID",
      field: "auditEventId",
      minWidth: 150,
      alignment: "center"
    }),
    createNameColumn({
      headerName: "Table Name",
      field: "tableName",
      minWidth: 200,
      alignment: "left"
    }),
    createNameColumn({
      headerName: "Operation",
      field: "operation",
      minWidth: 150,
      alignment: "left"
    }),
    createNameColumn({
      headerName: "Primary Key",
      field: "primaryKey",
      minWidth: 150,
      alignment: "left"
    }),
    createNameColumn({
      headerName: "User Name",
      field: "userName",
      minWidth: 200,
      alignment: "left"
    }),
    createDateColumn({
      headerName: "Created At",
      field: "createdAt",
      minWidth: 200,
      alignment: "center",
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
    }),
    {
      headerName: "Changes",
      field: "changesJson",
      sortable: false,
      filter: false,
      width: 300,
      autoHeight: true,
      wrapText: true,
      cellRenderer: ChangesCellRenderer,
      cellRendererParams: (params: ICellRendererParams) => ({
        navigationStatusLookup,
        tableName: params.data?.tableName
      })
    }
  ];
};
