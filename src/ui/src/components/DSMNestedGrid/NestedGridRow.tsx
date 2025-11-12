import React, { useState } from "react";
import { TableRow, TableCell, IconButton, Collapse, Box } from "@mui/material";
import { KeyboardArrowDown, KeyboardArrowRight } from "@mui/icons-material";

export type INestedGridRowData<T extends Record<string, unknown> = Record<string, unknown>> = {
  id: string | number;
  [key: string]: unknown;
} & T;

export interface INestedGridColumn<T extends Record<string, unknown> = Record<string, unknown>> {
  key: string;
  label: string;
  align?: "left" | "center" | "right";
  width?: number | string;
  render?: (value: unknown, row: INestedGridRowData<T>) => React.ReactNode;
}

interface INestedGridRowProps<T extends Record<string, unknown> = Record<string, unknown>> {
  row: INestedGridRowData<T>;
  columns: INestedGridColumn<T>[];
  renderNestedContent: (row: INestedGridRowData<T>, isExpanded: boolean) => React.ReactNode;
  onRowExpand?: (row: INestedGridRowData<T>, isExpanded: boolean) => void;
  expandedBackgroundColor: string;
}

export const NestedGridRow = <T extends Record<string, unknown>>({
  row,
  columns,
  renderNestedContent,
  onRowExpand,
  expandedBackgroundColor
}: INestedGridRowProps<T>) => {
  const [open, setOpen] = useState(false);

  const handleToggle = () => {
    const newState = !open;
    setOpen(newState);
    onRowExpand?.(row, newState);
  };

  return (
    <>
      <TableRow
        sx={{
          "& > *": { borderBottom: "unset" },
          "&:hover": { backgroundColor: "rgba(2, 88, 165, 0.1)" },
          transition: "background-color 0.2s"
        }}>
        {columns.map((column, index) => (
          <TableCell
            key={column.key}
            component={index === 0 ? "th" : "td"}
            scope={index === 0 ? "row" : undefined}
            align={column.align || "left"}
            sx={{
              py: 1.5,
              width: column.width
            }}>
            {column.render ? column.render(row[column.key], row) : (row[column.key] as React.ReactNode)}
          </TableCell>
        ))}

        <TableCell sx={{ py: 1.5, width: 48 }}>
          <IconButton
            aria-label="expand row"
            size="small"
            onClick={handleToggle}
            sx={{
              color: "#0258A5",
              "&:hover": { backgroundColor: "rgba(2, 88, 165, 0.04)" },
              transition: "background-color 0.2s"
            }}>
            {open ? <KeyboardArrowDown /> : <KeyboardArrowRight />}
          </IconButton>
        </TableCell>
      </TableRow>

      <TableRow>
        <TableCell
          style={{ paddingBottom: 0, paddingTop: 0, paddingLeft: 0, paddingRight: 0 }}
          colSpan={columns.length + 1}>
          <Collapse
            in={open}
            timeout={{ enter: 300, exit: 300 }}
            unmountOnExit>
            <Box sx={{ backgroundColor: expandedBackgroundColor, p: 0 }}>{renderNestedContent(row, open)}</Box>
          </Collapse>
        </TableCell>
      </TableRow>
    </>
  );
};

export default NestedGridRow;
