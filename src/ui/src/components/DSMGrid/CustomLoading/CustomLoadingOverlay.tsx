import { CircularProgress } from "@mui/material";

const CustomLoadingOverlay = () => {
  return (
    <div
      className="ag-custom-loading-cell"
      style={{ paddingLeft: "10px", lineHeight: "25px" }}>
      <CircularProgress />
    </div>
  );
};

export default CustomLoadingOverlay;
