import { ICellRendererParams } from "ag-grid-community";
import { Tooltip } from "@mui/material";
import { ErrorOutline } from "@mui/icons-material";
import { validateSuggestedForfeit } from "./validateSuggestedForfeit";

interface SuggestedForfeitCellRendererProps extends ICellRendererParams {
  selectedProfitYear: number;
}

export function SuggestedForfeitCellRenderer(
  params: SuggestedForfeitCellRendererProps,
  isTerminations: boolean,
  isRehireForfeiture: boolean
) {
  if (!params.data?.isDetail || params.data.profitYear !== params.selectedProfitYear) {
    return null;
  }

  const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}${params.data.enrollmentId ? `-${params.data.enrollmentId}` : ""}-${params.node?.id || "unknown"}`;
  const currentValue = params.context?.editedValues?.[rowKey]?.value ?? params.data.suggestedForfeit;

  const forfeitValue = isTerminations ? params.data.suggestedForfeit : params.data.forfeiture || 0;
  const maxForfeitOrUnforfeiture = Math.abs(forfeitValue);

  const errorMessage = validateSuggestedForfeit(currentValue, maxForfeitOrUnforfeiture);
  const hasError = !!errorMessage;
  const formattedValue = new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD"
  }).format(currentValue || 0);

  const isClassAction = params.data.remark === "FORFEIT CA";

  return (
    <div style={{ display: "flex", alignItems: "center", height: "100%" }}>
      {isClassAction && (
        <Tooltip
          title={"This participant cannot be unforfeited due to class action"}
          placement="top">
          <ErrorOutline sx={{ color: "#1976d2", fontSize: 24, marginTop: "7px", marginLeft: "20px" }} />
        </Tooltip>
      )}
      {hasError && (
        <Tooltip
          title={errorMessage}
          placement="top">
          <ErrorOutline sx={{ color: "#d32f2f", fontSize: 16, marginRight: "4px" }} />
        </Tooltip>
      )}
      {!isClassAction && <span style={{ color: hasError ? "#d32f2f" : "inherit" }}>{formattedValue}</span>}
    </div>
  );
}
