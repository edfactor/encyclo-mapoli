import Grid2 from "@mui/material/Unstable_Grid2";
import { Typography } from "@mui/material";
import { InfoCard } from '../FiscalFlow/ProfitShareReportEditRun/InfoCard';
import { useNavigate } from 'react-router-dom';
import { CAPTIONS, ROUTES } from "../../constants";

interface StoreReportCategory {
  code: string;
  title: string;
  data: { [key: string]: string };
  destinationUrl: string;
}

const storeReportCategories: StoreReportCategory[] = [
  {
      code: "QPAY066-UNDR21",
      title: CAPTIONS.QPAY066_UNDER21,
      data: { "Label": "Value" },
      destinationUrl: ROUTES.QPAY066_UNDER21
  },
  {
      code: "QPAY066TA-UNDR21",
      title: CAPTIONS.QPAY066TA_UNDER21,
      data: { "Label": "Value" },
      destinationUrl: ROUTES.QPAY066TA_UNDER21
  },
  {
      code: "QPAY066TA",
      title: CAPTIONS.QPAY066TA,
      data: { "Label": "Value" },
      destinationUrl: ROUTES.QPAY066TA
  },
  {
      code: "NEW-PS-LABELS",
      title: CAPTIONS.NEW_PS_LABELS,
      data: { "Label": "Value" },
      destinationUrl: ROUTES.NEW_PS_LABELS
  }
];

const ProfitShareByStoreResults = () => {
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
          {CAPTIONS.PAY_SHARE_BY_STORE_REPORTS}
        </Typography>
      </Grid2>

      <Grid2
        container
        spacing="24px"
        paddingLeft="24px"
        width="100%">
        {storeReportCategories.map((category) => (
          <Grid2
            key={category.code}
            xs={12}
            md={6}>
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
  );
};

export default ProfitShareByStoreResults;