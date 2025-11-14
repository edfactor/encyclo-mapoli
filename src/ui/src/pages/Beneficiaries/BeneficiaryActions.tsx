import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import LocalAtm from "@mui/icons-material/LocalAtm";
import { IconButton, Tooltip } from "@mui/material";
import { ICellRendererParams } from "ag-grid-community";
import type { BeneficiaryDto } from "../../types";

export interface BeneficiaryActionHandlers {
  onNewDistribution: (beneficiary: BeneficiaryDto) => void;
  onEdit: (beneficiary: BeneficiaryDto) => void;
  onDelete: (beneficiary: BeneficiaryDto) => void;
}

export interface BeneficiaryActionsCellRendererParams extends ICellRendererParams {
  handlers: BeneficiaryActionHandlers;
}

export const BeneficiaryActionsCellRenderer = (props: BeneficiaryActionsCellRendererParams) => {
  const beneficiary = props.data as BeneficiaryDto;
  const { handlers } = props;

  const handleNewDistribution = () => {
    handlers.onNewDistribution(beneficiary);
  };

  const handleEdit = () => {
    handlers.onEdit(beneficiary);
  };

  const handleDelete = () => {
    handlers.onDelete(beneficiary);
  };

  return (
    <div style={{ display: "flex", gap: "4px", alignItems: "center" }}>
      <Tooltip title="New Beneficiary Distribution">
        <IconButton
          size="small"
          onClick={handleNewDistribution}
          style={{ color: "#033059" }}>
          <LocalAtm fontSize="small" />
        </IconButton>
      </Tooltip>
      <Tooltip title="Edit">
        <IconButton
          size="small"
          onClick={handleEdit}
          style={{ color: "#033059" }}>
          <EditIcon fontSize="small" />
        </IconButton>
      </Tooltip>
      <Tooltip title="Delete">
        <IconButton
          size="small"
          onClick={handleDelete}
          style={{ color: "#033059" }}>
          <DeleteIcon fontSize="small" />
        </IconButton>
      </Tooltip>
    </div>
  );
};
