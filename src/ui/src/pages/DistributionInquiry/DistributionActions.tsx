import { ViewModule } from "@mui/icons-material";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import UndoIcon from "@mui/icons-material/Undo";
import { IconButton, Tooltip } from "@mui/material";
import { ICellRendererParams } from "ag-grid-community";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router-dom";
import { ROUTES } from "../../constants";
import { setCurrentDistribution } from "../../reduxstore/slices/distributionSlice";
import type { DistributionSearchResponse } from "../../types/distributions";

export const ActionsCellRenderer = (props: ICellRendererParams) => {
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const handleReverse = () => {
    console.log("Reverse distribution", props.data);
  };

  const handleView = () => {
    const distribution = props.data as DistributionSearchResponse;
    dispatch(setCurrentDistribution(distribution));

    // Navigate to view distribution page with memberId and memberType as URL parameters
    const memberId = distribution.demographicId || distribution.beneficiaryId;
    const memberType = distribution.demographicId ? 1 : 2; // 1 = employee, 2 = beneficiary

    if (memberId) {
      navigate(`/${ROUTES.VIEW_DISTRIBUTION}/${memberId}/${memberType}`);
    }
  };

  const handleEdit = () => {
    const distribution = props.data as DistributionSearchResponse;
    dispatch(setCurrentDistribution(distribution));

    // Navigate to edit distribution page with memberId and memberType as URL parameters
    const memberId = distribution.demographicId || distribution.beneficiaryId;
    const memberType = distribution.demographicId ? 1 : 2; // 1 = employee, 2 = beneficiary

    if (memberId) {
      navigate(`/${ROUTES.EDIT_DISTRIBUTION}/${memberId}/${memberType}`);
    }
  };

  const handleDelete = () => {
    const distribution = props.data as DistributionSearchResponse;
    dispatch(setCurrentDistribution(distribution));
    // Parent component will handle opening the delete modal
    window.dispatchEvent(new CustomEvent("openDeleteModal", { detail: distribution }));
  };

  return (
    <div style={{ display: "flex", gap: "4px", alignItems: "center" }}>
      <Tooltip title="Reverse Distribution">
        <IconButton
          size="small"
          onClick={handleReverse}
          style={{ color: "#033059" }}>
          <UndoIcon fontSize="small" />
        </IconButton>
      </Tooltip>
      <Tooltip title="View Detail">
        <IconButton
          size="small"
          onClick={handleView}
          style={{ color: "#033059" }}>
          <ViewModule fontSize="small" />
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
