import { Grid } from "@mui/material";
import { useNavigate } from "react-router-dom";
import { CAPTIONS, ROUTES } from "../../../constants";
import { InfoCard } from "./InfoCard";

interface ProfitShareCategory {
  code: string;
  title: string;
  data: { [key: string]: string };
  destinationUrl: string;
}

const profitShareCategories: ProfitShareCategory[] = [
  {
    code: "Pay426N-1",
    title: CAPTIONS.PAY426_ACTIVE_18_20,
    data: { "[Label]:": "[Value]" },
    destinationUrl: ROUTES.PAY426_ACTIVE_18_20
  },
  {
    code: "Pay426N-2",
    title: CAPTIONS.PAY426_ACTIVE_21_PLUS,
    data: { "[Label]:": "[Value]" },
    destinationUrl: ROUTES.PAY426_ACTIVE_21_PLUS
  },
  {
    code: "Pay426N-3",
    title: CAPTIONS.PAY426_ACTIVE_UNDER_18,
    data: { "[Label]:": "[Value]" },
    destinationUrl: ROUTES.PAY426_ACTIVE_UNDER_18
  },
  {
    code: "Pay426N-4",
    title: CAPTIONS.PAY426_ACTIVE_PRIOR_SHARING,
    data: { "[Label]:": "[Value]" },
    destinationUrl: ROUTES.PAY426_ACTIVE_PRIOR_SHARING
  },
  {
    code: "Pay426N-5",
    title: CAPTIONS.PAY426_ACTIVE_NO_PRIOR,
    data: { "[Label]:": "[Value]" },
    destinationUrl: ROUTES.PAY426_ACTIVE_NO_PRIOR
  },
  {
    code: "Pay426N-6",
    title: CAPTIONS.PAY426_TERMINATED_1000_PLUS,
    data: { "[Label]:": "[Value]" },
    destinationUrl: ROUTES.PAY426_TERMINATED_1000_PLUS
  },
  {
    code: "Pay426N-7",
    title: CAPTIONS.PAY426_TERMINATED_NO_PRIOR,
    data: { "[Label]:": "[Value]" },
    destinationUrl: ROUTES.PAY426_TERMINATED_NO_PRIOR
  },
  {
    code: "Pay426N-8",
    title: CAPTIONS.PAY426_TERMINATED_PRIOR,
    data: { "[Label]:": "[Value]" },
    destinationUrl: ROUTES.PAY426_TERMINATED_PRIOR
  },
  {
    code: "Pay426N-9",
    title: CAPTIONS.PAY426_SUMMARY,
    data: { "[Label]:": "[Value]" },
    destinationUrl: ROUTES.PAY426_SUMMARY
  },
  {
    code: "Pay426N-10",
    title: CAPTIONS.PAY426_NON_EMPLOYEE,
    data: { "[Label]:": "[Value]" },
    destinationUrl: ROUTES.PAY426_NON_EMPLOYEE
  }
];

const ProfitShareReportEditRunResults = () => {
  const navigate = useNavigate();

  return (
    <Grid
      container
      width={"100%"}
      rowSpacing={"24px"}>
      <Grid
        container
        spacing="24px"
        paddingLeft="24px"
        width="100%">
        {profitShareCategories.map((category, index) => (
          <Grid
            size={{ xs: 12, md: 6, lg: 6 }}
            key={category.code}>
            <InfoCard
              buttonDisabled={false}
              title={`${index + 1}. ${category.title}`}
              handleClick={() =>
                navigate(`/${ROUTES.PAY426N}/${category.code.substring(category.code.lastIndexOf("-") + 1)}`)
              }
              data={category.data}
              valid={true}
            />
          </Grid>
        ))}
      </Grid>
    </Grid>
  );
};

export default ProfitShareReportEditRunResults;
