import { SaveOutlined } from "@mui/icons-material";
import { Checkbox, CircularProgress, IconButton, Tooltip } from "@mui/material";
import { IHeaderParams, IRowNode } from "ag-grid-community";
import { ForfeitureAdjustmentUpdateRequest } from "../../types";

interface SelectableGridHeaderProps extends IHeaderParams {
  addRowToSelectedRows: (id: number) => void;
  removeRowFromSelectedRows: (id: number) => void;
  isNodeEligible: (
    nodeData: {
      isDetail: boolean;
      profitYear: number;
      badgeNumber: string;
      psn?: number;
      enrollmentId?: string;
      suggestedForfeit?: number;
      suggestedUnforfeiture?: number;
      profitDetailId?: number;
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
      psn?: number;
      enrollmentId?: string;
      suggestedForfeit?: number;
      suggestedUnforfeiture?: number;
      profitDetailId?: number;
      remark?: string;
    },
    context: {
      editedValues?: Record<string, { value?: number }>;
    }
  ) => ForfeitureAdjustmentUpdateRequest;
  onBulkSave?: (requests: ForfeitureAdjustmentUpdateRequest[], names: string[]) => Promise<void>;
  isBulkSaving?: () => boolean;
  loadingRowIds?: Set<number>;
  isReadOnly?: boolean;
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
    const employeeNames: string[] = [];
    props.api.forEachNode((node: IRowNode) => {
      if (node.isSelected() && props.isNodeEligible(node.data, props.context)) {
        const payload = props.createUpdatePayload(node.data, props.context);
        selectedNodes.push(payload);
        // Gather employee names from the grid data
        const employeeName = node.data.fullName || node.data.name || "Unknown Employee";
        employeeNames.push(employeeName);
      }
    });

    if (props.onBulkSave && selectedNodes.length > 0) {
      await props.onBulkSave(selectedNodes, employeeNames);
    }
  };

  const { totalEligible, totalSelected } = getSelectionState();
  const allSelected = totalSelected === totalEligible && totalEligible > 0;
  const someSelected = totalSelected > 0 && totalSelected < totalEligible;
  const isSaveDisabled = (props.isBulkSaving ? props.isBulkSaving() : false) || totalSelected === 0 || props.isReadOnly;
  const readOnlyTooltip = "You are in read-only mode and cannot perform bulk operations.";

  const checkboxElement = (
    <Checkbox
      onClick={props.isReadOnly ? undefined : handleSelectAll}
      checked={allSelected}
      indeterminate={someSelected}
      onChange={props.isReadOnly ? undefined : handleSelectAll}
      disabled={props.isReadOnly}
    />
  );

  const saveButtonElement = (
    <IconButton
      onClick={props.isReadOnly ? undefined : handleSave}
      disabled={isSaveDisabled}>
      {props.isBulkSaving && props.isBulkSaving() ? <CircularProgress size={20} /> : <SaveOutlined />}
    </IconButton>
  );

  return (
    <div>
      {props.isReadOnly ? (
        <Tooltip title={readOnlyTooltip}>
          <span>{checkboxElement}</span>
        </Tooltip>
      ) : (
        checkboxElement
      )}
      {props.isReadOnly ? (
        <Tooltip title={readOnlyTooltip}>
          <span>{saveButtonElement}</span>
        </Tooltip>
      ) : (
        saveButtonElement
      )}
    </div>
  );
};
