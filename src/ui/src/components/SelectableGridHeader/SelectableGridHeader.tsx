import { IHeaderParams } from "ag-grid-community";
import { Checkbox, IconButton } from "@mui/material";
import { SaveOutlined } from "@mui/icons-material";

interface SelectableGridHeaderProps extends IHeaderParams {
  addRowToSelectedRows: (id: number) => void;
  removeRowFromSelectedRows: (id: number) => void;
  isNodeEligible: (node: any, context?: any) => boolean;
  createUpdatePayload: (node: any, context: any) => any;
  onBulkSave?: (requests: any[]) => Promise<void>;
}

export const SelectableGridHeader: React.FC<SelectableGridHeaderProps> = (props) => {
  const getSelectionState = () => {
    let totalEligible = 0;
    let totalSelected = 0;
    
    props.api.forEachNode(node => {
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
      props.api.forEachNode(node => {
        if (props.isNodeEligible(node.data, props.context)) {
          node.setSelected(true);
          const id = Number(node.id) || -1;
          props.addRowToSelectedRows(id);
        }
      });
    } else {
      props.api.deselectAll();
      props.api.forEachNode(node => {
        if (props.isNodeEligible(node.data, props.context)) {
          const id = Number(node.id) || -1;
          props.removeRowFromSelectedRows(id);
        }
      });
    }
    props.api.refreshCells({ force: true });
  };

  const handleSave = async () => {
    const selectedNodes: any[] = [];
    props.api.forEachNode(node => {
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

  return (
    <div>
      <Checkbox
        onClick={handleSelectAll}
        checked={allSelected}
        indeterminate={someSelected}
        onChange={handleSelectAll}
      />
      <IconButton onClick={handleSave}>
        <SaveOutlined />
      </IconButton>
    </div>
  );
};
