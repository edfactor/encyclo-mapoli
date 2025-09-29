import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import { SelectableGridHeader } from "../../../components/SelectableGridHeader";
import { ForfeitureAdjustmentUpdateRequest, UnForfeitHeaderComponentProps } from "../../../reduxstore/types";

export const HeaderComponent: React.FC<UnForfeitHeaderComponentProps> = (params: UnForfeitHeaderComponentProps) => {
  const profitYear = useDecemberFlowProfitYear();

  const isNodeEligible = (
    nodeData: {
      isDetail: boolean;
      profitYear: number;
      badgeNumber: string;
      suggestedUnforfeiture: number;
      profitDetailId: number;
      remark?: string;
    },
    context: {
      editedValues?: Record<number, { value?: number }>;
    }
  ) => {
    if (!nodeData.isDetail) return false;
    // For bulk operations, we need to check all possible rowKeys for this data
    const baseRowKey = nodeData.profitDetailId;
    const editedValues = context?.editedValues || {};
    const matchingKey = baseRowKey in editedValues ? baseRowKey : undefined;
    const currentValue = matchingKey ? editedValues[matchingKey]?.value : nodeData.suggestedUnforfeiture;
    return (currentValue || 0) !== 0;
  };

  const createUpdatePayload = (
    nodeData: {
      isDetail: boolean;
      profitYear: number;
      badgeNumber: string;
      profitDetailId: number;
      suggestedUnforfeiture: number;
      remark?: string;
    },
    context: {
      editedValues?: Record<string, { value?: number }>;
    }
  ): ForfeitureAdjustmentUpdateRequest => {
    // For bulk operations, we need to find the actual edited value
    const baseRowKey = nodeData.profitDetailId;
    const editedValues = context?.editedValues || {};
    const matchingKey = baseRowKey in editedValues ? baseRowKey : undefined;
    const currentValue = matchingKey ? editedValues[matchingKey]?.value : nodeData.suggestedUnforfeiture;

    return {
      badgeNumber: Number(nodeData.badgeNumber),
      profitYear: profitYear, // use active profit year.
      forfeitureAmount: -(currentValue || 0),
      classAction: false
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
      isReadOnly={params.isReadOnly}
    />
  );
};
