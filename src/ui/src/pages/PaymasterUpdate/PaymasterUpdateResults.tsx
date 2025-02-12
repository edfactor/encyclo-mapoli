import Grid2 from "@mui/material/Unstable_Grid2";
import { Typography } from "@mui/material";
import { InfoCard } from '../FiscalFlow/ProfitShareReportEditRun/InfoCard';
import { useNavigate } from 'react-router-dom';
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
  },
];

const PaymasterUpdateResults = () => {

  const navigate = useNavigate();

  return (
    <Grid2
      container
      width={"100%"}
      rowSpacing={"24px"}>
      <Grid2 paddingX="24px">
        <Typography
          variant="h2"
          sx={{
            color: "#0258A5"
          }}>
          {CAPTIONS.PROFIT_SHARE_REPORT_FINAL_RUN}
        </Typography>
      </Grid2>

      <Grid2
        container
        spacing="24px"
        paddingLeft="24px"
        width="100%">
        {profitShareCategories.map((category) => (
          <Grid2
            key={category.code}
            xs={12}
            md={6}
            lg={6}>
            <InfoCard
              buttonDisabled={false}
              title={category.title}
              handleClick={() => navigate(`/${category.destinationUrl}`)}
              data={category.data}
              valid={true}
            />
          </Grid2>
        ))}
      </Grid2>
    </Grid2>
  )
};

export default PaymasterUpdateResults;