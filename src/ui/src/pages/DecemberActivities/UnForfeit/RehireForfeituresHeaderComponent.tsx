import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import { SelectableGridHeader } from "../../../components/SelectableGridHeader";
import { ForfeitureAdjustmentUpdateRequest, RehireForfeituresHeaderComponentProps } from "../../../reduxstore/types";

export const HeaderComponent: React.FC<RehireForfeituresHeaderComponentProps> = (
  params: RehireForfeituresHeaderComponentProps
) => {
  const selectedProfitYear = useDecemberFlowProfitYear();

  const isNodeEligible = (
    nodeData: {
      isDetail: boolean;
      profitYear: number;
      badgeNumber: string;
      enrollmentId?: string;
      suggestedForfeit?: number;
      remark?: string;
    },
    context: {
      editedValues?: Record<string, { value?: number }>;
    }
  ) => {
    if (!nodeData.isDetail || nodeData.profitYear !== selectedProfitYear) return false;
    // For bulk operations, we need to check all possible rowKeys for this data
    const baseRowKey = `${nodeData.badgeNumber}-${nodeData.profitYear}${nodeData.enrollmentId ? `-${nodeData.enrollmentId}` : ""}`;
    const editedValues = context?.editedValues || {};
    const matchingKey = Object.keys(editedValues).find((key) => key.startsWith(baseRowKey));
    const currentValue = matchingKey ? editedValues[matchingKey]?.value : nodeData.suggestedForfeit;
    return (currentValue || 0) !== 0;
  };

  const createUpdatePayload = (
    nodeData: {
      isDetail: boolean;
      profitYear: number;
      badgeNumber: string;
      enrollmentId?: string;
      suggestedForfeit?: number;
      remark?: string;
    },
    context: {
      editedValues?: Record<string, { value?: number }>;
    }
  ): ForfeitureAdjustmentUpdateRequest => {
    // For bulk operations, we need to find the actual edited value
    const baseRowKey = `${nodeData.badgeNumber}-${nodeData.profitYear}${nodeData.enrollmentId ? `-${nodeData.enrollmentId}` : ""}`;
    const editedValues = context?.editedValues || {};
    const matchingKey = Object.keys(editedValues).find((key) => key.startsWith(baseRowKey));
    const currentValue = matchingKey ? editedValues[matchingKey]?.value : nodeData.suggestedForfeit;
    const isClassAction = nodeData.remark === "FORFEIT CA";

    return {
      badgeNumber: Number(nodeData.badgeNumber),
      profitYear: nodeData.profitYear,
      forfeitureAmount: -(currentValue || 0),
      classAction: isClassAction
    };
  };

  // Check if any rows are in loading state
  const hasSavingInProgress = (): boolean => {
    return params.context?.loadingRowIds?.size > 0;
  };

  return (
    <SelectableGridHeader
      {...params}
      isNodeEligible={isNodeEligible}
      createUpdatePayload={createUpdatePayload}
      isBulkSaving={hasSavingInProgress}
    />
  );
};
