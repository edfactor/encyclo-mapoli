import { ICellRendererParams } from "ag-grid-community";
import { Tooltip } from "@mui/material";
import { ErrorOutline } from "@mui/icons-material";
import { validateSuggestedForfeit } from "./validateSuggestedForfeit";

interface SuggestedForfeitCellRendererProps extends ICellRendererParams {
  selectedProfitYear: number;
}

export function SuggestedForfeitCellRenderer(params: SuggestedForfeitCellRendererProps) {
  if (!params.data?.isDetail || params.data.profitYear !== params.selectedProfitYear) {
    return null;
  }

  const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
  const currentValue = params.context?.editedValues?.[rowKey]?.value ?? params.data.suggestedForfeit;
  const maxForfeitOrUnforfeiture = Math.abs(params.data.forfeiture || 0);
  const errorMessage = validateSuggestedForfeit(currentValue, maxForfeitOrUnforfeiture);
  const hasError = !!errorMessage;
  const formattedValue = new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD"
  }).format(currentValue || 0);

  return (
    <div style={{ display: "flex", alignItems: "center", height: "100%" }}>
      {hasError && (
        <Tooltip
          title={errorMessage}
          placement="top">
          <ErrorOutline sx={{ color: "#d32f2f", fontSize: 16, marginRight: "4px" }} />
        </Tooltip>
      )}
      <span style={{ color: hasError ? "#d32f2f" : "inherit" }}>{formattedValue}</span>
    </div>
  );
}
