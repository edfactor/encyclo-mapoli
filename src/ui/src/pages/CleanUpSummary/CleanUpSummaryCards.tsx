import { Typography, CircularProgress } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import React, { useEffect, useMemo } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { TotalsRow } from "smart-ui-library";
import { InfoCard } from "./InfoCard";
import {
  useLazyGetDemographicBadgesNotInPayprofitQuery,
  useLazyGetDuplicateNamesAndBirthdaysQuery,
  useLazyGetDuplicateSSNsQuery,
  useLazyGetNegativeEVTASSNQuery
} from "reduxstore/api/YearsEndApi";

interface CleanUpSummaryCardsProps {
  setSelectedTab: (value: number) => void;
  disableButtons?: boolean;
}

const CleanUpSummaryCards: React.FC<CleanUpSummaryCardsProps> = ({ setSelectedTab, disableButtons }) => {
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);

  // "Summary" - needs to get details for:

  // "Negative ETVA"

  // "Payroll Duplicate SSNs on Demographics"

  // "Demographic Badges Not On Payprofit"

  // "Duplicate Names and Birthdays"

  const [triggerETVASearch, { isFetching: isFetchingETVA }] = useLazyGetNegativeEVTASSNQuery();
  const [triggerPayrollDupeSsnsOnDemographics, { isFetching: isFetchingPayRollDupeSsns }] =
    useLazyGetDuplicateSSNsQuery();
  const [triggerDemographicBadgesNotInPayprofit, { isFetching: isfetchingDemographicBadges }] =
    useLazyGetDemographicBadgesNotInPayprofitQuery();
  const [triggerDuplicateNamesAndBirthdays, { isFetching: isFetchingDuplicateNames }] =
    useLazyGetDuplicateNamesAndBirthdaysQuery();

  const { negativeEtvaForSSNsOnPayprofit, duplicateSSNsData, demographicBadges, duplicateNamesAndBirthday } =
    useSelector((state: RootState) => state.yearsEnd);

  useEffect(() => {
    if (hasToken) {
      triggerETVASearch({ profitYear: 2023, pagination: { take: 25, skip: 0 } });
      triggerPayrollDupeSsnsOnDemographics({ profitYear: 2023, pagination: { take: 25, skip: 0 } });
      triggerDemographicBadgesNotInPayprofit({ pagination: { take: 25, skip: 0 } });
      triggerDuplicateNamesAndBirthdays({ profitYear: 2023, pagination: { take: 25, skip: 0 } });
    }
  }, [hasToken]);

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
          {`Clean Up Summary As Of 12/6/2024`}
        </Typography>
      </Grid2>

      <Grid2
        container
        spacing={"24px"}
        paddingLeft={"24px"}
        width={"100%"}>
        <Grid2
          xs={12}
          md={6}
          lg={6}>
          {!!negativeEtvaForSSNsOnPayprofit && (
            <InfoCard
              buttonDisabled={disableButtons}
              handleClick={() => setSelectedTab(1)}
              title="Negatibe ETVA for SSNs on Payprofit"
              valid={negativeEtvaForSSNsOnPayprofit.response.total == 0}
              data={{
                Count: negativeEtvaForSSNsOnPayprofit.response.total.toString()
              }}
            />
          )}
        </Grid2>
        <Grid2
          xs={12}
          md={6}
          lg={6}>
          {!!duplicateSSNsData && (
            <InfoCard
              buttonDisabled={disableButtons}
              title="Duplicate SSNs in Demographics"
              handleClick={() => setSelectedTab(2)}
              valid={duplicateSSNsData.response.total == 0}
              data={{
                Count: duplicateSSNsData.response.total.toString()
              }}
            />
          )}
        </Grid2>
        <Grid2
          xs={12}
          md={6}
          lg={6}>
          {!!demographicBadges && (
            <InfoCard
              buttonDisabled={disableButtons}
              title="Demographic Badges Not In Payprofit"
              handleClick={() => setSelectedTab(3)}
              valid={demographicBadges.response.total == 0}
              data={{
                Count: demographicBadges.response.total.toString()
              }}
            />
          )}
        </Grid2>
        <Grid2
          xs={12}
          md={6}
          lg={6}>
          {!!duplicateNamesAndBirthday && (
            <InfoCard
              buttonDisabled={disableButtons}
              title="Duplicate Names and Birthdays"
              handleClick={() => setSelectedTab(4)}
              valid={duplicateNamesAndBirthday.response.total == 0}
              data={{
                Count: duplicateNamesAndBirthday.response.total.toString()
              }}
            />
          )}
        </Grid2>
      </Grid2>
      <div style={{ display: "grid", verticalAlign: "middle", height: "100%" }}>
        <Grid2
          xs={2}
          md={1}
          lg={0.5}
          paddingY={"48px"}
          justifySelf={"center"}>
          <CircularProgress size={"100%"} />
        </Grid2>
      </div>
    </Grid2>
  );
};

export default CleanUpSummaryCards;
