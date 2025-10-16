import { Grid, Paper, Typography } from "@mui/material";
import { numberToCurrency } from "smart-ui-library";
import { DistributionSearchResponse } from "../../types";

interface DistributionDetailsSectionProps {
  distribution: DistributionSearchResponse;
}

const DistributionDetailsSection: React.FC<DistributionDetailsSectionProps> = ({ distribution }) => {
  // Helper function to format Yes/No
  const formatYesNo = (value: boolean): string => (value ? "Yes" : "No");

  // Format percentage
  const formatPercentage = (value: number): string => `${(value * 100).toFixed(1)}%`;

  return (
    <Grid
      width="100%"
      paddingX="24px">
      <Paper
        elevation={2}
        sx={{ padding: "16px" }}>
        {/* Section Header */}
        <Typography
          variant="h5"
          sx={{
            color: "#0258A5",
            marginBottom: "24px",
            fontWeight: "600"
          }}>
          Distribution Details
        </Typography>

        {/* Distribution Details Grid - 3 columns */}
        <Grid
          container
          spacing={3}>
          {/* Column 1 */}
          <Grid size={{ xs: 12, md: 4 }}>
            {/* Payment Flag */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography
                variant="body2"
                sx={{ color: "#666", fontWeight: "bold" }}>
                Payment Flag
              </Typography>
              <Typography variant="body1">
                {distribution.statusId} - {distribution.statusName}
              </Typography>
            </Grid>

            {/* Amount Requested */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography
                variant="body2"
                sx={{ color: "#666", fontWeight: "bold" }}>
                Amount Requested
              </Typography>
              <Typography variant="body1">{numberToCurrency(distribution.grossAmount)}</Typography>
            </Grid>

            {/* State Tax Override */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography
                variant="body2"
                sx={{ color: "#666", fontWeight: "bold" }}>
                State Tax Override
              </Typography>
              <Typography variant="body1">No</Typography>
            </Grid>

            {/* Sequence Number */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography
                variant="body2"
                sx={{ color: "#666", fontWeight: "bold" }}>
                Sequence Number
              </Typography>
              <Typography variant="body1">{distribution.paymentSequence}</Typography>
            </Grid>
          </Grid>

          {/* Column 2 */}
          <Grid size={{ xs: 12, md: 4 }}>
            {/* Tax Code */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography
                variant="body2"
                sx={{ color: "#666", fontWeight: "bold" }}>
                Tax Code
              </Typography>
              <Typography variant="body1">
                {distribution.taxCodeId} - {distribution.taxCodeName}
              </Typography>
            </Grid>

            {/* Fed Tax Override */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography
                variant="body2"
                sx={{ color: "#666", fontWeight: "bold" }}>
                Fed Tax Override
              </Typography>
              <Typography variant="body1">No</Typography>
            </Grid>

            {/* Fed Tax Percentage */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography
                variant="body2"
                sx={{ color: "#666", fontWeight: "bold" }}>
                Fed Tax Percentage
              </Typography>
              <Typography variant="body1">20%</Typography>
            </Grid>

            {/* State Tax Percentage */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography
                variant="body2"
                sx={{ color: "#666", fontWeight: "bold" }}>
                State Tax Percentage
              </Typography>
              <Typography variant="body1">5.1%</Typography>
            </Grid>

            {/* Memo */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography
                variant="body2"
                sx={{ color: "#666", fontWeight: "bold" }}>
                Memo
              </Typography>
              <Typography variant="body1">-</Typography>
            </Grid>
          </Grid>

          {/* Column 3 */}
          <Grid size={{ xs: 12, md: 4 }}>
            {/* Reason Code */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography
                variant="body2"
                sx={{ color: "#666", fontWeight: "bold" }}>
                Reason Code
              </Typography>
              <Typography variant="body1">R - Rollover</Typography>
            </Grid>

            {/* Fed Tax */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography
                variant="body2"
                sx={{ color: "#666", fontWeight: "bold" }}>
                Fed Tax
              </Typography>
              <Typography variant="body1">{numberToCurrency(distribution.federalTax)}</Typography>
            </Grid>

            {/* State Tax */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography
                variant="body2"
                sx={{ color: "#666", fontWeight: "bold" }}>
                State Tax
              </Typography>
              <Typography variant="body1">{numberToCurrency(distribution.stateTax)}</Typography>
            </Grid>

            {/* Employee Deceased */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography
                variant="body2"
                sx={{ color: "#666", fontWeight: "bold" }}>
                Employee Deceased
              </Typography>
              <Typography variant="body1">No</Typography>
            </Grid>
          </Grid>
        </Grid>
      </Paper>
    </Grid>
  );
};

export default DistributionDetailsSection;
