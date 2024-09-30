import { TextField } from "@mui/material";
import { CustomCellEditorProps, CustomCellRendererProps } from "ag-grid-react";
import { memo, useState } from "react";

interface DSMTextCellRendererProps extends CustomCellRendererProps {
  errorMsg: string;
  textAlign?: "center" | "right" | "left";
  endAdornment?: React.ReactNode;
  startAdornment?: React.ReactNode;
  type?: string;
}

const DSMTextCellRenderer: React.FC<DSMTextCellRendererProps> = memo(function ({
  errorMsg,
  startAdornment,
  endAdornment,
  value,
  textAlign = "right",
  type,
  data,
  node,
  eGridCell,
  colDef,
  column
}) {
  const intermediate = !value ? "" : value;
  const finalValue = type === "number" && intermediate ? Math.abs(parseInt(intermediate)).toString() : intermediate;
  return (
    <TextField
      size="small"
      value={finalValue ?? ""}
      error={true}
      sx={{ paddingBottom: "5px" }}
      inputProps={{ style: { textAlign } }}
      helperText={errorMsg}
      InputProps={{
        startAdornment: startAdornment,
        endAdornment: endAdornment
      }}
      fullWidth
    />
  );
});

export default DSMTextCellRenderer;
