import { Grid, Typography } from "@mui/material";
import { useNavigate } from "react-router-dom";
import { CAPTIONS, ROUTES } from "../../../constants";
import { InfoCard } from "../InfoCard";

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
    data: { Label: "Value" },
    destinationUrl: ROUTES.QPAY066_UNDER21
  },
  {
    code: "QPAY066TA-UNDR21",
    title: CAPTIONS.QPAY066TA_UNDER21,
    data: { Label: "Value" },
    destinationUrl: ROUTES.QPAY066TA_UNDER21
  },
  {
    code: "QPAY066TA",
    title: CAPTIONS.QPAY066TA,
    data: { Label: "Value" },
    destinationUrl: ROUTES.QPAY066TA
  },
  {
    code: "NEW-PS-LABELS",
    title: CAPTIONS.NEW_PS_LABELS,
    data: { Label: "Value" },
    destinationUrl: ROUTES.NEW_PS_LABELS
  }
];

const ProfitShareByStoreResults = () => {
  const navigate = useNavigate();

  const handleCategoryClick = (category: StoreReportCategory) => {
    navigate(`/${category.destinationUrl}`);
  };

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
          {CAPTIONS.PAY_SHARE_BY_STORE_REPORTS}
        </Typography>
      </Grid>

      <Grid
        container
        spacing="24px"
        paddingLeft="24px"
        width="100%">
        {storeReportCategories.map((category) => (
          <Grid
            size={{ xs: 12, md: 6 }}
            key={category.code}>
            <InfoCard
              buttonDisabled={false}
              title={category.title}
              handleClick={() => handleCategoryClick(category)}
              data={category.data}
              valid={true}
            />
          </Grid>
        ))}
      </Grid>
    </Grid>
  );
};

export default ProfitShareByStoreResults;
