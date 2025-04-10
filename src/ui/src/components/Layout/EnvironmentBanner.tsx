import { InfoOutlined } from "@mui/icons-material";
import Alert from "@mui/material/Alert";
import { purple } from "@mui/material/colors";
import { ICommon } from "../ICommon";

export interface IEnvironmentBannerProps extends ICommon {
  environmentMode: "development" | "qa" | "uat" | "production";
  buildVersionNumber?: string;
}
export const EnvironmentBanner: React.FC<IEnvironmentBannerProps> = ({ environmentMode, buildVersionNumber }) => {
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
        <div>WARNING! {environmentName} ENVIRONMENT. CHANGES WILL NOT BE REFLECTED IN PRODUCTION!</div>
        <div>v.{versionNumber}</div>
      </div>
    </Alert>
  );
};

export default EnvironmentBanner;
