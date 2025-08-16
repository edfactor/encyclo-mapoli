import { ICellEditorParams } from "ag-grid-community";
import { TextField, Tooltip } from "@mui/material";
import { ErrorOutline } from "@mui/icons-material";
import { useState, useRef, useEffect } from "react";
import { validateSuggestedForfeit } from "./validateSuggestedForfeit";

export function SuggestedForfeitEditor(props: ICellEditorParams) {
  const [value, setValue] = useState(props.data.suggestedForfeit ?? 0);
  const [error, setError] = useState<string | null>(null);
  const refInput = useRef<HTMLInputElement>(null);

  useEffect(() => {
    refInput.current?.focus();
  }, []);

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const newValue = parseFloat(event.target.value) || 0;
    setValue(newValue);
    const forfeitValue = props.data.forfeit || props.data.forfeiture || 0;
    const newError = validateSuggestedForfeit(newValue, Math.abs(forfeitValue));
    setError(newError);

    const rowKey = `${props.data.badgeNumber}-${props.data.profitYear}${props.data.enrollmentId ? `-${props.data.enrollmentId}` : ""}-${props.node?.id || "unknown"}`;
    props.context?.updateEditedValue?.(rowKey, newValue, !!newError);
  };

  const handleKeyDown = (event: React.KeyboardEvent) => {
    if (event.key === "Enter" && !error) {
      props.api.stopEditing();
    }
    if (event.key === "Escape") {
      setValue(props.data.suggestedForfeit ?? 0);
      props.api.stopEditing();
    }
  };

  return (
    <div style={{ display: "flex", alignItems: "center" }}>
      {error && (
        <Tooltip
          title={error}
          placement="top">
          <ErrorOutline sx={{ color: "#d32f2f", fontSize: 20, marginRight: "8px" }} />
        </Tooltip>
      )}
      <TextField
        style={{ flex: 1 }}
        inputRef={refInput}
        type="number"
        value={value}
        onChange={handleChange}
        onKeyDown={handleKeyDown}
        error={!!error}
        variant="outlined"
        fullWidth
      />
    </div>
  );
}
