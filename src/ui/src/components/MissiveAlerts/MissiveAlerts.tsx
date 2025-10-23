import { Grid, Typography } from "@mui/material";
import React from "react";
import { MissiveResponse } from "types";
import { useMissiveAlerts } from "../../hooks/useMissiveAlerts";

const MissiveAlerts: React.FC = () => {
  const { missiveAlerts } = useMissiveAlerts();

  if (!missiveAlerts || missiveAlerts.length === 0) {
    return null;
  }

  const getSeverityClass = (severity: string) => {
    if (severity === "Error") return "missive-error";
    if (severity === "success") return "missive-success";
    return "missive-warning";
  };

  const getSeverityColor = (severity: string) => {
    if (severity === "Error") return "error.main";
    if (severity === "success") return "success.main";
    return "warning.main";
  };

  return (
    <Grid size={{ xs: 12 }}>
      <div className="missive-alerts-box">
        {missiveAlerts.map((alert: MissiveResponse, idx: number) => (
          <div
            key={alert.id || idx}
            className={`missive-alert ${getSeverityClass(alert.severity)}`}>
            <Typography
              sx={{ color: getSeverityColor(alert.severity) }}
              variant="body1"
              fontWeight={600}>
              {alert.message}
            </Typography>
            <Typography variant="body2">{alert.description}</Typography>
          </div>
        ))}
      </div>
    </Grid>
  );
};

export default MissiveAlerts;
