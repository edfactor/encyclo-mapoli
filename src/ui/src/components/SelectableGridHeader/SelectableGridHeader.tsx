import { IHeaderParams, IRowNode } from "ag-grid-community";
import { Checkbox, IconButton, CircularProgress } from "@mui/material";
import { SaveOutlined } from "@mui/icons-material";
import { ForfeitureAdjustmentUpdateRequest } from "types";

interface SelectableGridHeaderProps extends IHeaderParams {
  addRowToSelectedRows: (id: number) => void;
  removeRowFromSelectedRows: (id: number) => void;
  isNodeEligible: (
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
  ) => boolean;
  createUpdatePayload: (
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
  ) => ForfeitureAdjustmentUpdateRequest;
  onBulkSave?: (requests: ForfeitureAdjustmentUpdateRequest[]) => Promise<void>;
  isBulkSaving?: () => boolean;
  loadingRowIds?: Set<number>;
}

export const SelectableGridHeader: React.FC<SelectableGridHeaderProps> = (props) => {
  const getSelectionState = () => {
    let totalEligible = 0;
    let totalSelected = 0;

    props.api.forEachNode((node) => {
      if (props.isNodeEligible(node.data, props.context)) {
        totalEligible++;
        if (node.isSelected()) {
          totalSelected++;
        }
      }
    });

    return { totalEligible, totalSelected };
  };

  const handleSelectAll = () => {
    const { totalEligible, totalSelected } = getSelectionState();
    const shouldSelectAll = totalSelected < totalEligible;

    if (shouldSelectAll) {
      props.api.forEachNode((node: IRowNode) => {
        if (props.isNodeEligible(node.data, props.context)) {
          node.setSelected(true);
          const id = Number(node.id) || -1;
          props.addRowToSelectedRows(id);
        }
      });
    } else {
      props.api.deselectAll();
      props.api.forEachNode((node: IRowNode) => {
        if (props.isNodeEligible(node.data, props.context)) {
          const id = Number(node.id) || -1;
          props.removeRowFromSelectedRows(id);
        }
      });
    }
    props.api.refreshCells({ force: true });
  };

  const handleSave = async () => {
    const selectedNodes: ForfeitureAdjustmentUpdateRequest[] = [];
    props.api.forEachNode((node: IRowNode) => {
      if (node.isSelected() && props.isNodeEligible(node.data, props.context)) {
        const payload = props.createUpdatePayload(node.data, props.context);
        selectedNodes.push(payload);
      }
    });

    if (props.onBulkSave && selectedNodes.length > 0) {
      await props.onBulkSave(selectedNodes);
    }
  };

  const { totalEligible, totalSelected } = getSelectionState();
  const allSelected = totalSelected === totalEligible && totalEligible > 0;
  const someSelected = totalSelected > 0 && totalSelected < totalEligible;
  const isSaveDisabled = (props.isBulkSaving ? props.isBulkSaving() : false) || totalSelected === 0;

  return (
    <div>
      <Checkbox
        onClick={handleSelectAll}
        checked={allSelected}
        indeterminate={someSelected}
        onChange={handleSelectAll}
      />
      <IconButton
        onClick={handleSave}
        disabled={isSaveDisabled}>
        {props.isBulkSaving && props.isBulkSaving() ? <CircularProgress size={20} /> : <SaveOutlined />}
      </IconButton>
    </div>
  );
};
