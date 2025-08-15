import { SaveOutlined } from "@mui/icons-material";
import { Checkbox, CircularProgress, IconButton } from "@mui/material";
import { ColDef, ICellRendererParams } from "ag-grid-community";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import { agGridNumberToCurrency } from "smart-ui-library";
import { SelectableGridHeader } from "../../../components/SelectableGridHeader";
import { SuggestedForfeitCellRenderer, SuggestedForfeitEditor } from "../../../components/SuggestedForfeiture";
import {
  ForfeitureAdjustmentUpdateRequest,
  RehireForfeituresHeaderComponentProps,
  RehireForfeituresSaveButtonCellParams
} from "../../../reduxstore/types";
import {
  createBadgeColumn,
  createCountColumn,
  createCurrencyColumn,
  createDateColumn,
  createHoursColumn,
  createNameColumn,
  createSSNColumn,
  createStoreColumn,
  createYearColumn
} from "../../../utils/gridColumnFactory";

export const HeaderComponent: React.FC<RehireForfeituresHeaderComponentProps> = (
  params: RehireForfeituresHeaderComponentProps
) => {
  const selectedProfitYear = useDecemberFlowProfitYear();

  const isNodeEligible = (nodeData: any, context: any) => {
    if (!nodeData.isDetail || nodeData.profitYear !== selectedProfitYear) return false;
    // For bulk operations, we need to check all possible rowKeys for this data
    const baseRowKey = `${nodeData.badgeNumber}-${nodeData.profitYear}${nodeData.enrollmentId ? `-${nodeData.enrollmentId}` : ""}`;
    const editedValues = context?.editedValues || {};
    const matchingKey = Object.keys(editedValues).find((key) => key.startsWith(baseRowKey));
    const currentValue = matchingKey ? editedValues[matchingKey]?.value : nodeData.suggestedForfeit;
    return (currentValue || 0) !== 0;
  };

  const createUpdatePayload = (nodeData: any, context: any): ForfeitureAdjustmentUpdateRequest => {
    // For bulk operations, we need to find the actual edited value
    const baseRowKey = `${nodeData.badgeNumber}-${nodeData.profitYear}${nodeData.enrollmentId ? `-${nodeData.enrollmentId}` : ""}`;
    const editedValues = context?.editedValues || {};
    const matchingKey = Object.keys(editedValues).find((key) => key.startsWith(baseRowKey));
    const currentValue = matchingKey ? editedValues[matchingKey]?.value : nodeData.suggestedForfeit;
    const isClassAction = nodeData.remark === "FORFEIT CA";

    return {
      badgeNumber: nodeData.badgeNumber,
      profitYear: nodeData.profitYear,
      forfeitureAmount: -(currentValue || 0),
      classAction: isClassAction
    };
  };

  // Check if any rows are in loading state
  const hasSavingInProgress = params.context?.loadingRowIds?.size > 0;

  return (
    <SelectableGridHeader
      {...params}
      isNodeEligible={isNodeEligible}
      createUpdatePayload={createUpdatePayload}
      isBulkSaving={hasSavingInProgress}
    />
  );
};

export const GetRehireForfeituresColumns = (): ColDef[] => {
  return [
    createBadgeColumn({}),
    createNameColumn({
      field: "fullName"
    }),
    createSSNColumn({}),
    createDateColumn({
      headerName: "Hire Date",
      field: "hireDate"
    }),
    createDateColumn({
      headerName: "Termination Date",
      field: "terminationDate"
    }),
    createDateColumn({
      headerName: "Rehired Date",
      field: "reHiredDate"
    }),
    createCurrencyColumn({
      headerName: "Current Balance",
      field: "netBalanceLastYear"
    }),
    createCurrencyColumn({
      headerName: "Vested Balance",
      field: "vestedBalanceLastYear"
    }),
    createStoreColumn({})
  ];
};
