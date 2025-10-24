import { CircularProgress, Grid, Typography } from "@mui/material";
import React, { useEffect, useMemo } from "react";
import { useSelector } from "react-redux";
import { useLazyGetFrozenStateResponseQuery } from "reduxstore/api/ItOperationsApi";
import {
  useLazyGetBalanceByAgeQuery,
  useLazyGetContributionsByAgeQuery,
  useLazyGetDistributionsByAgeQuery,
  useLazyGetForfeituresByAgeQuery
} from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { FrozenReportsByAgeRequestType } from "reduxstore/types";
import { numberToCurrency } from "smart-ui-library";
import { FlexibleInfoCard } from "./FlexibleInfoCard";

interface FrozenSummaryCardsProps {
  setSelectedTab: (value: number) => void;
  disableButtons?: boolean;
}

const FrozenSummaryCards: React.FC<FrozenSummaryCardsProps> = ({ setSelectedTab, disableButtons }) => {
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);

  const [triggerBalanceSearch] = useLazyGetBalanceByAgeQuery();
  const [triggerDistributionsSearch] = useLazyGetDistributionsByAgeQuery();
  const [triggerForfeituresSearch] = useLazyGetForfeituresByAgeQuery();
  const [triggerContributionsSearch] = useLazyGetContributionsByAgeQuery();

  const {
    balanceByAgeTotal,
    balanceByAgeFullTime,
    balanceByAgePartTime,
    distributionsByAgeTotal,
    distributionsByAgeFullTime,
    distributionsByAgePartTime,
    forfeituresByAgeTotal,
    forfeituresByAgeFullTime,
    forfeituresByAgePartTime,
    contributionsByAgeTotal,
    contributionsByAgeFullTime,
    contributionsByAgePartTime
  } = useSelector((state: RootState) => state.yearsEnd);

  const [fetchFrozenState, { data: frozenState }] = useLazyGetFrozenStateResponseQuery();

  useEffect(() => {
    if (hasToken) {
      fetchFrozenState(undefined, false);
    }
  }, [fetchFrozenState, hasToken]);

  useEffect(() => {
    if (hasToken && frozenState?.profitYear) {
      const profitYear = frozenState.profitYear;

      triggerBalanceSearch(
        {
          profitYear: profitYear,
          reportType: FrozenReportsByAgeRequestType.Total,
          pagination: { skip: 0, take: 255 }
        },
        false
      );
      triggerBalanceSearch(
        {
          profitYear: profitYear,
          reportType: FrozenReportsByAgeRequestType.FullTime,
          pagination: { skip: 0, take: 255 }
        },
        false
      );
      triggerBalanceSearch(
        {
          profitYear: profitYear,
          reportType: FrozenReportsByAgeRequestType.PartTime,
          pagination: { skip: 0, take: 255 }
        },
        false
      );

      triggerContributionsSearch(
        {
          profitYear: profitYear,
          reportType: FrozenReportsByAgeRequestType.Total,
          pagination: { skip: 0, take: 255 }
        },
        false
      );
      triggerContributionsSearch(
        {
          profitYear: profitYear,
          reportType: FrozenReportsByAgeRequestType.FullTime,
          pagination: { skip: 0, take: 255 }
        },
        false
      );
      triggerContributionsSearch(
        {
          profitYear: profitYear,
          reportType: FrozenReportsByAgeRequestType.PartTime,
          pagination: { skip: 0, take: 255 }
        },
        false
      );

      triggerDistributionsSearch(
        {
          profitYear: profitYear,
          reportType: FrozenReportsByAgeRequestType.Total,
          pagination: { skip: 0, take: 255 }
        },
        false
      );
      triggerDistributionsSearch(
        {
          profitYear: profitYear,
          reportType: FrozenReportsByAgeRequestType.FullTime,
          pagination: { skip: 0, take: 255 }
        },
        false
      );
      triggerDistributionsSearch(
        {
          profitYear: profitYear,
          reportType: FrozenReportsByAgeRequestType.PartTime,
          pagination: { skip: 0, take: 255 }
        },
        false
      );

      triggerForfeituresSearch(
        {
          profitYear: profitYear,
          reportType: FrozenReportsByAgeRequestType.Total,
          pagination: { skip: 0, take: 255 }
        },
        false
      );
      triggerForfeituresSearch(
        {
          profitYear: profitYear,
          reportType: FrozenReportsByAgeRequestType.FullTime,
          pagination: { skip: 0, take: 255 }
        },
        false
      );
      triggerForfeituresSearch(
        {
          profitYear: profitYear,
          reportType: FrozenReportsByAgeRequestType.PartTime,
          pagination: { skip: 0, take: 255 }
        },
        false
      );
    }
  }, [
    hasToken,
    frozenState,
    triggerBalanceSearch,
    triggerContributionsSearch,
    triggerDistributionsSearch,
    triggerForfeituresSearch
  ]);

  const distributionsTotalChecksOut = useMemo(() => {
    if (!distributionsByAgeTotal || !distributionsByAgeFullTime || !distributionsByAgePartTime) return false;

    const employeesMatch =
      (distributionsByAgeFullTime.regularTotalEmployees || 0) +
        (distributionsByAgePartTime.regularTotalEmployees || 0) ===
        (distributionsByAgeTotal.regularTotalEmployees || 0) &&
      (distributionsByAgeFullTime.hardshipTotalEmployees || 0) +
        (distributionsByAgePartTime.hardshipTotalEmployees || 0) ===
        (distributionsByAgeTotal.hardshipTotalEmployees || 0);

    const amountsMatch =
      (distributionsByAgeFullTime.regularTotalAmount || 0) + (distributionsByAgePartTime.regularTotalAmount || 0) ===
        (distributionsByAgeTotal.regularTotalAmount || 0) &&
      (distributionsByAgeFullTime.hardshipTotalAmount || 0) + (distributionsByAgePartTime.hardshipTotalAmount || 0) ===
        (distributionsByAgeTotal.hardshipTotalAmount || 0);

    return employeesMatch && amountsMatch;
  }, [distributionsByAgeTotal, distributionsByAgeFullTime, distributionsByAgePartTime]);

  const contributionsTotalsChecksOut = useMemo(() => {
    if (!contributionsByAgeTotal || !contributionsByAgeFullTime || !contributionsByAgePartTime) return false;

    const employeesMatch =
      (contributionsByAgePartTime.totalEmployees || 0) + (contributionsByAgeFullTime.totalEmployees || 0) ===
      (contributionsByAgeTotal.totalEmployees || 0);

    const amountsMatch =
      (contributionsByAgePartTime.totalAmount || 0) + (contributionsByAgeFullTime.totalAmount || 0) ===
      (contributionsByAgeTotal.totalAmount || 0);

    return employeesMatch && amountsMatch;
  }, [contributionsByAgeTotal, contributionsByAgeFullTime, contributionsByAgePartTime]);

  const forfeituresTotalsChecksOut = useMemo(() => {
    if (!forfeituresByAgeTotal || !forfeituresByAgeFullTime || !forfeituresByAgePartTime) return false;

    const employeesMatch =
      (forfeituresByAgeFullTime.totalEmployees || 0) + (forfeituresByAgePartTime.totalEmployees || 0) ===
      (forfeituresByAgeTotal.totalEmployees || 0);

    const amountsMatch =
      (forfeituresByAgeFullTime.totalAmount || 0) + (forfeituresByAgePartTime.totalAmount || 0) ===
      (forfeituresByAgeTotal.totalAmount || 0);

    return employeesMatch && amountsMatch;
  }, [forfeituresByAgeTotal, forfeituresByAgeFullTime, forfeituresByAgePartTime]);

  const balanceTotalChecksOut = useMemo(() => {
    if (!balanceByAgeTotal || !balanceByAgeFullTime || !balanceByAgePartTime) return false;

    const totalVerticalChecksOut =
      (balanceByAgeTotal.totalBeneficiaries || 0) + (balanceByAgeTotal.totalEmployee || 0) ===
        balanceByAgeTotal.totalMembers &&
      (balanceByAgeTotal.totalBeneficiariesAmount || 0) + (balanceByAgeTotal.totalEmployeeAmount || 0) ===
        balanceByAgeTotal.balanceTotalAmount &&
      (balanceByAgeTotal.totalBeneficiariesVestedAmount || 0) + (balanceByAgeTotal.totalEmployeesVestedAmount || 0) ===
        balanceByAgeTotal.vestedTotalAmount;

    const horizontalChecksOut =
      (balanceByAgeFullTime.totalBeneficiaries || 0) + (balanceByAgePartTime.totalBeneficiaries || 0) ===
        balanceByAgeTotal.totalBeneficiaries &&
      (balanceByAgeFullTime.totalBeneficiariesAmount || 0) + (balanceByAgePartTime.totalBeneficiariesAmount || 0) ===
        balanceByAgeTotal.totalBeneficiariesAmount &&
      (balanceByAgeFullTime.totalBeneficiariesVestedAmount || 0) +
        (balanceByAgePartTime.totalBeneficiariesVestedAmount || 0) ===
        balanceByAgeTotal.totalBeneficiariesVestedAmount &&
      (balanceByAgeFullTime.totalEmployee || 0) + (balanceByAgePartTime.totalEmployee || 0) ===
        balanceByAgeTotal.totalEmployee &&
      (balanceByAgeFullTime.totalEmployeeAmount || 0) + (balanceByAgePartTime.totalEmployeeAmount || 0) ===
        balanceByAgeTotal.totalEmployeeAmount &&
      (balanceByAgeFullTime.totalEmployeesVestedAmount || 0) +
        (balanceByAgePartTime.totalEmployeesVestedAmount || 0) ===
        balanceByAgeTotal.totalEmployeesVestedAmount &&
      (balanceByAgeFullTime.totalMembers || 0) + (balanceByAgePartTime.totalMembers || 0) ===
        balanceByAgeTotal.totalMembers &&
      (balanceByAgeFullTime.balanceTotalAmount || 0) + (balanceByAgePartTime.balanceTotalAmount || 0) ===
        balanceByAgeTotal.balanceTotalAmount &&
      (balanceByAgeFullTime.vestedTotalAmount || 0) + (balanceByAgePartTime.vestedTotalAmount || 0) ===
        balanceByAgeTotal.vestedTotalAmount;

    return totalVerticalChecksOut && horizontalChecksOut;
  }, [balanceByAgeTotal, balanceByAgeFullTime, balanceByAgePartTime]);

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
          {`Frozen Summary As Of ${frozenState?.asOfDateTime ? new Date(frozenState.asOfDateTime).toLocaleDateString() : "Loading..."}`}
        </Typography>
      </Grid>

      <Grid
        container
        spacing={"24px"}
        paddingLeft={"24px"}
        width={"100%"}>
        <Grid size={{ xs: 12, md: 6, lg: 6 }}>
          {!!distributionsByAgeFullTime && !!distributionsByAgeFullTime && !!distributionsByAgePartTime && (
            <FlexibleInfoCard
              buttonDisabled={disableButtons}
              handleClick={() => setSelectedTab(1)}
              title="Distributions By Age"
              valid={distributionsTotalChecksOut}
              data={{
                "Total Regular": {
                  value1: distributionsByAgeTotal?.regularTotalEmployees?.toString() || "0",
                  value2: numberToCurrency(distributionsByAgeTotal?.regularTotalAmount || 0)
                },
                "Total Hardship": {
                  value1: distributionsByAgeTotal?.hardshipTotalEmployees?.toString() || "0",
                  value2: numberToCurrency(distributionsByAgeTotal?.hardshipTotalAmount || 0)
                },
                "Dist Total": {
                  value1: (
                    (distributionsByAgeTotal?.regularTotalEmployees || 0) +
                    (distributionsByAgeTotal?.hardshipTotalEmployees || 0)
                  ).toString(),
                  value2: numberToCurrency(
                    (distributionsByAgeTotal?.regularTotalAmount || 0) +
                      (distributionsByAgeTotal?.hardshipTotalAmount || 0)
                  )
                },
                "Regular FT": {
                  value1: distributionsByAgeFullTime?.regularTotalEmployees?.toString() || "0",
                  value2: numberToCurrency(distributionsByAgeFullTime?.regularTotalAmount || 0)
                },
                "Hardship FT": {
                  value1: distributionsByAgeFullTime?.hardshipTotalEmployees?.toString() || "0",
                  value2: numberToCurrency(distributionsByAgeFullTime?.hardshipTotalAmount || 0)
                },
                "Dist FT": {
                  value1: (
                    (distributionsByAgeFullTime?.regularTotalEmployees || 0) +
                    (distributionsByAgeFullTime?.hardshipTotalEmployees || 0)
                  ).toString(),
                  value2: numberToCurrency(
                    (distributionsByAgeFullTime?.regularTotalAmount || 0) +
                      (distributionsByAgeFullTime?.hardshipTotalAmount || 0)
                  )
                },
                "Regular PT": {
                  value1: distributionsByAgePartTime?.regularTotalEmployees?.toString() || "0",
                  value2: numberToCurrency(distributionsByAgePartTime?.regularTotalAmount || 0)
                },
                "Hardship PT": {
                  value1: distributionsByAgePartTime?.hardshipTotalEmployees?.toString() || "0",
                  value2: numberToCurrency(distributionsByAgePartTime?.hardshipTotalAmount || 0)
                },
                "Dist PT": {
                  value1: (
                    (distributionsByAgePartTime?.regularTotalEmployees || 0) +
                    (distributionsByAgePartTime?.hardshipTotalEmployees || 0)
                  ).toString(),
                  value2: numberToCurrency(
                    (distributionsByAgePartTime?.regularTotalAmount || 0) +
                      (distributionsByAgePartTime?.hardshipTotalAmount || 0)
                  )
                }
              }}
            />
          )}
        </Grid>
        <Grid size={{ xs: 12, md: 6, lg: 6 }}>
          {!!contributionsByAgeTotal && !!contributionsByAgeFullTime && !!contributionsByAgePartTime && (
            <FlexibleInfoCard
              buttonDisabled={disableButtons}
              title="Contributions By Age"
              handleClick={() => setSelectedTab(2)}
              valid={contributionsTotalsChecksOut}
              data={{
                "CONT TTL Total": {
                  value1: (contributionsByAgeTotal?.totalEmployees || 0).toString(),
                  value2: numberToCurrency(contributionsByAgeTotal?.totalAmount || 0)
                },
                "CONT TTL FT": {
                  value1: (contributionsByAgeFullTime?.totalEmployees || 0).toString(),
                  value2: numberToCurrency(contributionsByAgeFullTime?.totalAmount || 0)
                },
                "CONT TTL PT": {
                  value1: (contributionsByAgePartTime?.totalEmployees || 0).toString(),
                  value2: numberToCurrency(contributionsByAgePartTime?.totalAmount || 0)
                }
              }}
            />
          )}
        </Grid>
        <Grid size={{ xs: 12, md: 6, lg: 6 }}>
          {!!forfeituresByAgeTotal && !!forfeituresByAgePartTime && !!forfeituresByAgePartTime && (
            <FlexibleInfoCard
              buttonDisabled={disableButtons}
              title="Forfeitures By Age"
              handleClick={() => setSelectedTab(3)}
              valid={forfeituresTotalsChecksOut}
              data={{
                "FORF TTL Total": {
                  value1: (forfeituresByAgeTotal?.totalEmployees || 0).toString(),
                  value2: numberToCurrency(forfeituresByAgeTotal?.totalAmount || 0)
                },
                "FORF TTL FT": {
                  value1: (forfeituresByAgeFullTime?.totalEmployees || 0).toString(),
                  value2: numberToCurrency(forfeituresByAgeFullTime?.totalAmount || 0)
                },
                "FORF TTL PT": {
                  value1: (forfeituresByAgePartTime?.totalEmployees || 0).toString(),
                  value2: numberToCurrency(forfeituresByAgePartTime?.totalAmount || 0)
                }
              }}
            />
          )}
        </Grid>
        <Grid size={{ xs: 12, md: 6, lg: 6 }}>
          {!!balanceByAgeTotal && !!balanceByAgePartTime && !!balanceByAgeFullTime && (
            <FlexibleInfoCard
              buttonDisabled={disableButtons}
              title="Balance By Age"
              handleClick={() => setSelectedTab(4)}
              valid={balanceTotalChecksOut}
              data={{
                "BEN TOTAL": {
                  value1: (balanceByAgeTotal?.totalBeneficiaries || 0).toString(),
                  value2: numberToCurrency(balanceByAgeTotal?.totalBeneficiariesAmount || 0),
                  value3: numberToCurrency(balanceByAgeTotal?.totalBeneficiariesVestedAmount || 0)
                },
                "EMPLOYEE TOTAL": {
                  value1: (balanceByAgeTotal?.totalEmployee || 0).toString(),
                  value2: numberToCurrency(balanceByAgeTotal?.totalEmployeeAmount || 0),
                  value3: numberToCurrency(balanceByAgeTotal?.totalEmployeesVestedAmount || 0)
                },
                "TOTAL TOTAL": {
                  value1: (balanceByAgeTotal?.totalMembers || 0).toString(),
                  value2: numberToCurrency(balanceByAgeTotal?.balanceTotalAmount || 0),
                  value3: numberToCurrency(balanceByAgeTotal?.vestedTotalAmount || 0)
                },
                "BEN FT": {
                  value1: (balanceByAgeFullTime?.totalBeneficiaries || 0).toString(),
                  value2: numberToCurrency(balanceByAgeFullTime?.totalBeneficiariesAmount || 0),
                  value3: numberToCurrency(balanceByAgeFullTime?.totalBeneficiariesVestedAmount || 0)
                },
                "EMPLOYEE FT": {
                  value1: (balanceByAgeFullTime?.totalEmployee || 0).toString(),
                  value2: numberToCurrency(balanceByAgeFullTime?.totalEmployeeAmount || 0),
                  value3: numberToCurrency(balanceByAgeFullTime?.totalEmployeesVestedAmount || 0)
                },
                "TOTAL FT": {
                  value1: (balanceByAgeFullTime?.totalMembers || 0).toString(),
                  value2: numberToCurrency(balanceByAgeFullTime?.balanceTotalAmount || 0),
                  value3: numberToCurrency(balanceByAgeFullTime?.vestedTotalAmount || 0)
                },
                "BEN PT": {
                  value1: (balanceByAgePartTime?.totalBeneficiaries || 0).toString(),
                  value2: numberToCurrency(balanceByAgePartTime?.totalBeneficiariesAmount || 0),
                  value3: numberToCurrency(balanceByAgePartTime?.totalBeneficiariesVestedAmount || 0)
                },
                "EMPLOYEE PT": {
                  value1: (balanceByAgePartTime?.totalEmployee || 0).toString(),
                  value2: numberToCurrency(balanceByAgePartTime?.totalEmployeeAmount || 0),
                  value3: numberToCurrency(balanceByAgePartTime?.totalEmployeesVestedAmount || 0)
                },
                "TOTAL PT": {
                  value1: (balanceByAgePartTime?.totalMembers || 0).toString(),
                  value2: numberToCurrency(balanceByAgePartTime?.balanceTotalAmount || 0),
                  value3: numberToCurrency(balanceByAgePartTime?.vestedTotalAmount || 0)
                }
              }}
            />
          )}
        </Grid>
      </Grid>
      <div style={{ display: "grid", verticalAlign: "middle", height: "100%" }}>
        <Grid
          size={{ xs: 2, md: 1, lg: 0.5 }}
          paddingY={"48px"}
          justifySelf={"center"}>
          <CircularProgress size={"100%"} />
        </Grid>
      </div>
    </Grid>
  );
};

export default FrozenSummaryCards;
