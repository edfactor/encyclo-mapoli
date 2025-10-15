import { ViewModule } from "@mui/icons-material";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import UndoIcon from "@mui/icons-material/Undo";
import { IconButton, Tooltip } from "@mui/material";
import { ICellRendererParams } from "ag-grid-community";
import { useDispatch } from "react-redux";
import { setCurrentDistribution } from "../../reduxstore/slices/distributionSlice";
import type { DistributionSearchResponse } from "../../types/distributions";

export const ActionsCellRenderer = (props: ICellRendererParams) => {
  const dispatch = useDispatch();

  const handleReverse = () => {
    console.log("Reverse distribution", props.data);
  };

  const handleView = () => {
    dispatch(setCurrentDistribution(props.data as DistributionSearchResponse));
    console.log("View distribution", props.data);
  };

  const handleEdit = () => {
    dispatch(setCurrentDistribution(props.data as DistributionSearchResponse));
    console.log("Edit distribution", props.data);
  };

  const handleDelete = () => {
    console.log("Delete distribution", props.data);
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
