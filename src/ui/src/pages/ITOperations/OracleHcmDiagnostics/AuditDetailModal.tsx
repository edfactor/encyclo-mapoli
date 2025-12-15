import { Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, Typography } from "@mui/material";
import type { DemographicSyncAuditRecord } from "../../../types";
import { mmDDYYYY_HHMMSS_Format } from "../../../utils/dateUtils";

interface AuditDetailModalProps {
  record: DemographicSyncAuditRecord;
  onClose: () => void;
}

const AuditDetailModal: React.FC<AuditDetailModalProps> = ({ record, onClose }) => {
  return (
    <Dialog open={true} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>Demographic Sync Audit Details</DialogTitle>
      <DialogContent>
        <Box sx={{ paddingTop: "16px", display: "flex", flexDirection: "column", gap: "16px" }}>
          {/* Badge Number */}
          <Box>
            <Typography variant="body2" sx={{ color: "#666", marginBottom: "4px" }}>
              Badge Number
            </Typography>
            <Typography variant="body1" sx={{ fontWeight: "600" }}>
              {record.badgeNumber}
            </Typography>
          </Box>

          {/* Oracle HCM ID */}
          <Box>
            <Typography variant="body2" sx={{ color: "#666", marginBottom: "4px" }}>
              Oracle HCM ID
            </Typography>
            <Typography variant="body1" sx={{ fontWeight: "600" }}>
              {record.oracleHcmId}
            </Typography>
          </Box>

          {/* Created */}
          <Box>
            <Typography variant="body2" sx={{ color: "#666", marginBottom: "4px" }}>
              Created Date/Time
            </Typography>
            <Typography variant="body1" sx={{ fontWeight: "600" }}>
              {mmDDYYYY_HHMMSS_Format(record.created)}
            </Typography>
          </Box>

          {/* Property Name */}
          {record.propertyName && (
            <Box>
              <Typography variant="body2" sx={{ color: "#666", marginBottom: "4px" }}>
                Property Name
              </Typography>
              <Typography variant="body1" sx={{ fontWeight: "600" }}>
                {record.propertyName}
              </Typography>
            </Box>
          )}

          {/* Invalid Value */}
          {record.invalidValue && (
            <Box>
              <Typography variant="body2" sx={{ color: "#666", marginBottom: "4px" }}>
                Invalid Value
              </Typography>
              <Typography variant="body1" sx={{ fontWeight: "600", wordBreak: "break-word" }}>
                {record.invalidValue}
              </Typography>
            </Box>
          )}

          {/* User Name */}
          {record.userName && (
            <Box>
              <Typography variant="body2" sx={{ color: "#666", marginBottom: "4px" }}>
                User Name
              </Typography>
              <Typography variant="body1" sx={{ fontWeight: "600" }}>
                {record.userName}
              </Typography>
            </Box>
          )}

          {/* Message */}
          <Box>
            <Typography variant="body2" sx={{ color: "#666", marginBottom: "4px" }}>
              Message
            </Typography>
            <Typography variant="body1" sx={{ fontWeight: "600", wordBreak: "break-word" }}>
              {record.message}
            </Typography>
          </Box>

          {/* ID */}
          <Box>
            <Typography variant="body2" sx={{ color: "#666", marginBottom: "4px" }}>
              Record ID
            </Typography>
            <Typography variant="body1" sx={{ fontWeight: "600" }}>
              {record.id}
            </Typography>
          </Box>
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} variant="contained">
          Close
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default AuditDetailModal;
