import React from "react";
import { Box, Typography } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";

interface LabelValueItem {
  label: string;
  value: string | number;
}

interface LabelValueSectionProps {
  title?: string;
  data: LabelValueItem[];
}

const LabelValueSection: React.FC<LabelValueSectionProps> = ({ title, data }) => (
  <Box>
    {title && <Typography variant="overline">{title}</Typography>}
    {data.map(({ label, value }, index) => (
      <Grid2
        container
        key={index}
        paddingY="4px"
        spacing={1}>
        {!!label && label != "" && (
          <Grid2>
            <Typography
              variant="body2"
              align="left"
              fontWeight="bold">
              {label}
            </Typography>
          </Grid2>
        )}
        <Grid2 xs={!!label && label != "" ? 6 : 12}>
          <Typography variant="body2">{value}</Typography>
        </Grid2>
      </Grid2>
    ))}
  </Box>
);

export default LabelValueSection;
