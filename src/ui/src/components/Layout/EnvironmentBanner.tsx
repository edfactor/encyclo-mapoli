
import { InfoOutlined } from "@mui/icons-material";
import Alert from "@mui/material/Alert";
import { purple } from "@mui/material/colors";
import { ICommon } from "../ICommon";
import Tooltip from "@mui/material/Tooltip";
import CircleIcon from "@mui/icons-material/Circle";

export interface IEnvironmentBannerProps extends ICommon {
  environmentMode: "development" | "qa" | "uat" | "production";
  buildVersionNumber?: string;
  apiStatus?: "Healthy" | "Degraded" | "Unhealthy";
  apiStatusMessage? : string;
}
export const EnvironmentBanner: React.FC<IEnvironmentBannerProps> = ({ environmentMode, buildVersionNumber, apiStatus, apiStatusMessage }) => {
  if (!environmentMode || environmentMode === "production") {
    return null; // No banner for production
  }

  let environmentName;
  let alertSeverity: "error" | "info" | "success" | "warning";
  let backgroundColor;
  let versionNumber = buildVersionNumber || "";
  switch (environmentMode) {
    case "development":
      environmentName = "DEVELOPMENT";
      alertSeverity = "info";
      backgroundColor = "#0288D1";
      break;
    case "qa":
      environmentName = "QA";
      alertSeverity = "warning";
      backgroundColor = "#EF6C00";
      break;
    case "uat":
      environmentName = "UAT";
      alertSeverity = "error";
      backgroundColor = "#DB1532";
      break;
    default:
      environmentName = `UNKNOWN: "${environmentMode}"`;
      alertSeverity = "error";
      backgroundColor = purple;
      versionNumber = "";
  }

  // API Status Icon renderer
  const renderApiStatusIcon = () => {
    if (!apiStatus || !["Healthy", "Degraded", "Unhealthy"].includes(apiStatus)) {
      return null; // Hide icon for unknown status
    }

    let textColor = "";
    switch (apiStatus) {
      case "Healthy":
        textColor = "#4caf50"; // Green
        break;
      case "Degraded":
        textColor = "#ffeb3b"; // Yellow
        break;
      case "Unhealthy":
        textColor = "#f44336"; // Red
        break;
    }
    return (
      <Tooltip title={apiStatusMessage || ""} arrow>
        <span
          style={{
            color: textColor,
            fontWeight: "bold",
            marginLeft: "4px"
          }}
        >
          {apiStatus}
        </span>
      </Tooltip>
    );
  };


  return (
    <Alert
      severity={alertSeverity}
      icon={<InfoOutlined sx={{ color: "white" }} />}
      sx={{
        position: "fixed",
        width: "100%",
        backgroundColor,
        "& .MuiAlert-message": {
          // had to resort to this to override defaults that didn't match design
          color: "white",
          fontFamily: "Lato",
          fontSize: "16px",
          fontStyle: "normal",
          fontWeight: 700,
          lineHeight: "150%",
          letterSpacing: "0.15px",
          fontFeatureSettings: "'liga' off, 'clig' off",
          width: "100%"
        }
      }}>
      <div
        className="flex justify-between"
        id="alert-message-container">
        <div className="flex items-center">
          WARNING! {environmentName} ENVIRONMENT. CHANGES WILL NOT BE REFLECTED IN PRODUCTION!        
        </div>
        <div></div>
        <div>API Status: {renderApiStatusIcon()}</div>     
        <div>v.{versionNumber}</div>
      </div>
    </Alert>
  );
};

export default EnvironmentBanner;