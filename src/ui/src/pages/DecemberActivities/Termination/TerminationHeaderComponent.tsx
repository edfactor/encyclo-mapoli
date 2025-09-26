import { IHeaderParams } from "ag-grid-community";
import { SelectableGridHeader } from "components/SelectableGridHeader";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import { ForfeitureAdjustmentUpdateRequest } from "types";

interface HeaderComponentProps extends IHeaderParams {
  addRowToSelectedRows: (id: number) => void;
  removeRowFromSelectedRows: (id: number) => void;
  onBulkSave?: (requests: ForfeitureAdjustmentUpdateRequest[], names: string[]) => Promise<void>;
  isBulkSaving?: boolean;
  isReadOnly?: boolean;
}

export const HeaderComponent: React.FC<HeaderComponentProps> = (params: HeaderComponentProps) => {
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
    const rowKey = `${nodeData.badgeNumber}-${nodeData.profitYear}`;
    const currentValue = context?.editedValues?.[rowKey]?.value ?? nodeData.suggestedForfeit;
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
    const rowKey = `${nodeData.badgeNumber}-${nodeData.profitYear}`;
    const currentValue = context?.editedValues?.[rowKey]?.value ?? nodeData.suggestedForfeit;

    return {
      badgeNumber: Number(nodeData.badgeNumber),
      profitYear: nodeData.profitYear,
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
