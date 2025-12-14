import RefreshIcon from "@mui/icons-material/Refresh";
import { Box, Button, CircularProgress, Typography } from "@mui/material";
import type { OracleHcmSyncMetadata } from "../../../types";
import { mmDDYYYY_HHMMSS_Format } from "../../../utils/dateUtils";

interface OracleHcmMetadataProps {
  metadata?: OracleHcmSyncMetadata;
  isLoading: boolean;
  onRefresh: () => void;
}

const OracleHcmMetadata: React.FC<OracleHcmMetadataProps> = ({ metadata, isLoading, onRefresh }) => {
  return (
    <Box sx={{ padding: "0 24px" }}>
      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "16px" }}>
        <Typography variant="h2" sx={{ color: "#0258A5" }}>
          OracleHcm Sync Status
        </Typography>
        <Button
          variant="outlined"
          startIcon={<RefreshIcon />}
          onClick={onRefresh}
          disabled={isLoading}
          size="small">
          Refresh
        </Button>
      </Box>

      {isLoading ? (
        <CircularProgress />
      ) : (
        <Box sx={{ display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(300px, 1fr))", gap: "16px", marginTop: "8px" }}>
          {/* Demographic Created */}
          <Box sx={{ border: "1px solid #ddd", borderRadius: "4px", padding: "12px", backgroundColor: "#f9f9f9" }}>
            <Typography variant="body2" sx={{ color: "#666", marginBottom: "4px" }}>
              Demographic Table - Created
            </Typography>
            <Typography variant="body1" sx={{ fontWeight: "600", color: "#000" }}>
              {metadata?.demographicCreatedAtUtc ? mmDDYYYY_HHMMSS_Format(metadata.demographicCreatedAtUtc) : "No data"}
            </Typography>
          </Box>

          {/* Demographic Modified */}
          <Box sx={{ border: "1px solid #ddd", borderRadius: "4px", padding: "12px", backgroundColor: "#f9f9f9" }}>
            <Typography variant="body2" sx={{ color: "#666", marginBottom: "4px" }}>
              Demographic Table - Last Modified
            </Typography>
            <Typography variant="body1" sx={{ fontWeight: "600", color: "#000" }}>
              {metadata?.demographicModifiedAtUtc ? mmDDYYYY_HHMMSS_Format(metadata.demographicModifiedAtUtc) : "No data"}
            </Typography>
          </Box>

          {/* PayProfit Created */}
          <Box sx={{ border: "1px solid #ddd", borderRadius: "4px", padding: "12px", backgroundColor: "#f9f9f9" }}>
            <Typography variant="body2" sx={{ color: "#666", marginBottom: "4px" }}>
              PayProfit Table - Created
            </Typography>
            <Typography variant="body1" sx={{ fontWeight: "600", color: "#000" }}>
              {metadata?.payProfitCreatedAtUtc ? mmDDYYYY_HHMMSS_Format(metadata.payProfitCreatedAtUtc) : "No data"}
            </Typography>
          </Box>

          {/* PayProfit Modified */}
          <Box sx={{ border: "1px solid #ddd", borderRadius: "4px", padding: "12px", backgroundColor: "#f9f9f9" }}>
            <Typography variant="body2" sx={{ color: "#666", marginBottom: "4px" }}>
              PayProfit Table - Last Modified
            </Typography>
            <Typography variant="body1" sx={{ fontWeight: "600", color: "#000" }}>
              {metadata?.payProfitModifiedAtUtc ? mmDDYYYY_HHMMSS_Format(metadata.payProfitModifiedAtUtc) : "No data"}
            </Typography>
          </Box>
        </Box>
      )}
    </Box>
  );
};


export default OracleHcmMetadata;
