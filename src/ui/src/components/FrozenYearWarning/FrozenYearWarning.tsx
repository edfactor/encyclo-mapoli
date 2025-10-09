import { WarningAmber } from "@mui/icons-material";
import { Grid, Typography } from "@mui/material";
import React from "react";

interface FrozenYearWarningProps {
  profitYear?: number;
  message?: string;
}

/**
 * FrozenYearWarning Component
 *
 * Displays a prominent warning banner when attempting to modify data for a frozen profit year.
 * Uses the Missive framework styling for consistent alert presentation across the application.
 *
 * @param profitYear - The frozen profit year to display in the warning
 * @param message - Optional custom message. Defaults to standard frozen year warning text.
 *
 * @example
 * ```tsx
 * <FrozenYearWarning profitYear={2024} />
 * ```
 *
 * @example Custom message
 * ```tsx
 * <FrozenYearWarning
 *   profitYear={2024}
 *   message="This year is frozen. Contact Finance to request changes."
 * />
 * ```
 */
const FrozenYearWarning: React.FC<FrozenYearWarningProps> = ({
  profitYear,
  message = "This profit year is frozen. Changes to frozen years may require approval from Finance or IT Operations."
}) => {
  // Don't render if no profit year is provided
  if (!profitYear) {
    return null;
  }

  return (
    <Grid size={{ xs: 12 }}>
      <div className="missive-alerts-box">
        <div className="missive-alert missive-warning frozen-year-warning">
          <WarningAmber sx={{ color: "warning.main", fontSize: "28px" }} />
          <div className="frozen-year-warning-content">
            <Typography
              sx={{ color: "warning.main" }}
              variant="body1"
              fontWeight={600}>
              Frozen Year Warning: {profitYear}
            </Typography>
            <Typography variant="body2">{message}</Typography>
          </div>
        </div>
      </div>
    </Grid>
  );
};

export default FrozenYearWarning;
