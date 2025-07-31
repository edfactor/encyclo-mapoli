import { Grid } from "@mui/material";
import { Typography } from "@mui/material";
import { InfoCard } from "../FiscalFlow/ProfitShareReportEditRun/InfoCard";
import { useNavigate } from "react-router-dom";
import { CAPTIONS, ROUTES } from "../../constants";

interface ProfitShareCategory {
  code: string;
  title: string;
  data: { [key: string]: string };
  destinationUrl: string;
}

const profitShareCategories: ProfitShareCategory[] = [
  {
    code: "Pay450",
    title: CAPTIONS.PAY450,
    data: { "[Label]:": "[Value]" },
    destinationUrl: ROUTES.PAY450_SUMMARY
  },
  {
    code: "ProfCTRLSheet",
    title: CAPTIONS.PROF_CTRLSHEET,
    data: { "[Label]:": "[Value]" },
    destinationUrl: ROUTES.PROF_CTRLSHEET
  }
];

const PaymasterUpdateResults = () => {
  const navigate = useNavigate();

  return (
    <Grid
      container
      width={"100%"}
      rowSpacing={"24px"}>
      <Grid paddingX="24px">
        <Typography
          variant="h2"
          sx={{
            color: "#0258A5"
          }}>
          {CAPTIONS.PROFIT_SHARE_REPORT_FINAL_RUN}
        </Typography>
      </Grid>

      <Grid
        container
        spacing="24px"
        paddingLeft="24px"
        width="100%">
        {profitShareCategories.map((category) => (
          <Grid
            size={{ xs: 12, md: 6, lg: 6 }}
            key={category.code}>
            <InfoCard
              buttonDisabled={false}
              title={category.title}
              handleClick={() => navigate(`/${category.destinationUrl}`)}
              data={category.data}
              valid={true}
            />
          </Grid>
        ))}
      </Grid>
    </Grid>
  );
};

export default PaymasterUpdateResults;
