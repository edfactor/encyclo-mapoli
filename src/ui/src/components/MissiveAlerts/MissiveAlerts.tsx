import { Grid, Typography } from "@mui/material";
import React from "react";
import { useMissiveAlerts } from "../../hooks/useMissiveAlerts";

const MissiveAlerts: React.FC = () => {
  const { missiveAlerts } = useMissiveAlerts();
  if (!missiveAlerts || missiveAlerts.length === 0) {
    return null;
  }

  return (
    <Grid size={{ xs: 12 }}>
      <div className="missive-alerts-box">
        {missiveAlerts.map((alert: MissiveResponse, idx: number) => (
          <div
            key={alert.id || idx}
            className={`missive-alert ${alert.severity === "Error" ? "missive-error" : "missive-warning"}`}>
            <Typography
              sx={{ color: alert.severity === "Error" ? "error.main" : "warning.main" }}
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
