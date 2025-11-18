import { Box, Button, CircularProgress, Dialog, DialogActions, DialogContent, DialogTitle } from "@mui/material";
import { ColDef, ICellRendererParams } from "ag-grid-community";
import { useState } from "react";
import { useLazyGetAuditChangesQuery } from "../../../reduxstore/api/ItOperationsApi";
import { NavigationStatusDto } from "../../../types";
import { createCountColumn, createDateColumn, createNameColumn } from "../../../utils/gridColumnFactory";

interface ChangesCellRendererProps extends ICellRendererParams {
  navigationStatusLookup: Map<number, string>;
  tableName: string;
}

const serializeValue = (value: string | null | undefined): string => {
  if (value === null || value === undefined) return "(null)";

  // Handle the case where backend sent "[object Object]" as a string
  if (value === "[object Object]") {
    return "(Unable to display - object was not properly serialized by backend)";
  }

  // Trim whitespace
  const trimmedValue = value.trim();

  // Try to parse as JSON to check if it's a serialized object
  if (trimmedValue.startsWith("{") || trimmedValue.startsWith("[")) {
    try {
      const parsed = JSON.parse(trimmedValue);
      if (typeof parsed === "object" && parsed !== null) {
        // Re-stringify with formatting for better readability
        return JSON.stringify(parsed, null, 2);
      }
    } catch {
      // Not valid JSON, return as-is
    }
  }

  return value;
};

const ChangesCellRenderer = (params: ChangesCellRendererProps) => {
  const changes = params.value;
  const { navigationStatusLookup, tableName } = params;
  const auditEventId = params.data?.auditEventId;

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [triggerGetChanges, { data: loadedChanges, isFetching }] = useLazyGetAuditChangesQuery();

  // Handle null value - means there was a lot of content
  if (changes === null) {
    const handleViewClick = async () => {
      setIsModalOpen(true);
      await triggerGetChanges(auditEventId);
    };

    return (
      <>
        <a
          href="#"
          onClick={(e) => {
            e.preventDefault();
            handleViewClick();
          }}
          style={{ color: "#1976d2", cursor: "pointer", textDecoration: "underline" }}>
          View...
        </a>
        <Dialog
          open={isModalOpen}
          onClose={() => setIsModalOpen(false)}
          maxWidth="lg"
          fullWidth>
          <DialogTitle>Audit Changes - Event ID: {auditEventId}</DialogTitle>
          <DialogContent>
            {isFetching ? (
              <Box
                display="flex"
                justifyContent="center"
                alignItems="center"
                minHeight="200px">
                <CircularProgress />
              </Box>
            ) : loadedChanges ? (
              <Box
                display="flex"
                gap={2}
                minHeight="400px">
                {loadedChanges.map((change, index) => (
                  <Box
                    key={index}
                    flex={1}
                    display="flex"
                    flexDirection="column"
                    gap={2}>
                    <Box>
                      <strong>Column: {change.columnName}</strong>
                    </Box>
                    <Box
                      flex={1}
                      display="flex"
                      gap={2}>
                      <Box
                        flex={1}
                        display="flex"
                        flexDirection="column">
                        <label style={{ fontWeight: "bold", marginBottom: "8px" }}>Original Value</label>
                        <textarea
                          readOnly
                          value={serializeValue(change.originalValue)}
                          style={{
                            flex: 1,
                            padding: "8px",
                            fontFamily: "monospace",
                            fontSize: "12px",
                            border: "1px solid #ccc",
                            borderRadius: "4px",
                            resize: "none",
                            backgroundColor: "#fff3e0"
                          }}
                        />
                      </Box>
                      <Box
                        flex={1}
                        display="flex"
                        flexDirection="column">
                        <label style={{ fontWeight: "bold", marginBottom: "8px" }}>New Value</label>
                        <textarea
                          readOnly
                          value={serializeValue(change.newValue)}
                          style={{
                            flex: 1,
                            padding: "8px",
                            fontFamily: "monospace",
                            fontSize: "12px",
                            border: "1px solid #ccc",
                            borderRadius: "4px",
                            resize: "none",
                            backgroundColor: "#e8f5e9"
                          }}
                        />
                      </Box>
                    </Box>
                  </Box>
                ))}
              </Box>
            ) : (
              <Box>No changes available</Box>
            )}
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setIsModalOpen(false)}>Close</Button>
          </DialogActions>
        </Dialog>
      </>
    );
  }

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
