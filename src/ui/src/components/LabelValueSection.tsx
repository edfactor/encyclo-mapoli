import { Box, Typography } from "@mui/material";
import React from "react";

import { Grid } from "@mui/material";

interface LabelValueItem {
  label: string;
  value: React.ReactNode;
  labelColor?: "primary" | "secondary" | "error" | "warning" | "info" | "success" | "text.primary" | "text.secondary" | "text.disabled" | "inherit";
  labelVariant?: "body1" | "body2" | "caption" | "h1" | "h2" | "h3" | "h4" | "h5" | "h6" | "inherit" | "subtitle1" | "subtitle2" | "overline";
  labelWeight?: "normal" | "bold" | "lighter" | "bolder" | number;
}

interface LabelValueSectionProps {
  title?: string;
  data: LabelValueItem[];
}

const LabelValueSection: React.FC<LabelValueSectionProps> = ({ title, data }) => (
  <Box>
    {title && <Typography variant="overline">{title}</Typography>}
    {data.map(({ label, value, labelColor, labelVariant, labelWeight }, index) => (
      <Grid
        container
        key={index}
        paddingY="4px"
        spacing={1}>
        {!!label && label != "" && (
          <Grid>
            <Typography
              variant={labelVariant || "body2"}
              align="left"
              fontWeight={labelWeight || "bold"}
              color={labelColor}>
              {label}
            </Typography>
          </Grid>
        )}
        <Grid size={{ xs: !!label && label != "" ? 6 : 12 }}>
          <Typography variant="body2">{value}</Typography>
        </Grid>
      </Grid>
    ))}
  </Box>
);

export default LabelValueSection;
