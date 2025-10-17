import { Grid, Typography } from "@mui/material";
import { numberToCurrency } from "smart-ui-library";
import LabelValueSection from "../../components/LabelValueSection";
import { DistributionSearchResponse } from "../../types";

interface DistributionDetailsSectionProps {
  distribution: DistributionSearchResponse;
}

const DistributionDetailsSection: React.FC<DistributionDetailsSectionProps> = ({ distribution }) => {
  // Column 1 data
  const column1Data = [
    { label: "Payment Flag", value: `${distribution.statusId} - ${distribution.statusName}` },
    { label: "Amount Requested", value: numberToCurrency(distribution.grossAmount) },
    { label: "State Tax Override", value: "No" },
    { label: "Sequence Number", value: distribution.paymentSequence }
  ];

  // Column 2 data
  const column2Data = [
    { label: "Tax Code", value: `${distribution.taxCodeId} - ${distribution.taxCodeName}` },
    { label: "Fed Tax Override", value: "No" },
    { label: "Fed Tax Percentage", value: "20%" },
    { label: "State Tax Percentage", value: "5.1%" },
    { label: "Memo", value: "-" }
  ];

  // Column 3 data
  const column3Data = [
    { label: "Reason Code", value: "R - Rollover" },
    { label: "Fed Tax", value: numberToCurrency(distribution.federalTax) },
    { label: "State Tax", value: numberToCurrency(distribution.stateTax) },
    { label: "Employee Deceased", value: "No" }
  ];

  return (
    <Grid
      container
      paddingX="24px"
      width="100%">
      <Grid size={{ xs: 12 }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5", marginY: "8px" }}>
          Distribution Details
        </Typography>
      </Grid>

      <Grid size={{ xs: 12 }}>
        <Grid
          container
          spacing={3}>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <LabelValueSection data={column1Data} />
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <LabelValueSection data={column2Data} />
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <LabelValueSection data={column3Data} />
          </Grid>
        </Grid>
      </Grid>
    </Grid>
  );
};

export default DistributionDetailsSection;
