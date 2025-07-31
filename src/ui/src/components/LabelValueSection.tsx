import React from "react";
import { Box, Typography } from "@mui/material";

import { Grid } from "@mui/material";
interface LabelValueItem {
  label: string;
  value: React.ReactNode;
}

interface LabelValueSectionProps {
  title?: string;
  data: LabelValueItem[];
}

const LabelValueSection: React.FC<LabelValueSectionProps> = ({ title, data }) => (
  <Box>
    {title && <Typography variant="overline">{title}</Typography>}
    {data.map(({ label, value }, index) => (
      <Grid
        container
        key={index}
        paddingY="4px"
        spacing={1}>
        {!!label && label != "" && (
          <Grid>
            <Typography
              variant="body2"
              align="left"
              fontWeight="bold">
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
