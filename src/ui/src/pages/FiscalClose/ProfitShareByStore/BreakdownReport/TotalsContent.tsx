import { Typography } from "@mui/material";
import LabelValueSection from "../../../../components/LabelValueSection";
import { Grid } from "@mui/material";
import { useEffect } from "react";
import { useLazyGetBreakdownByStoreTotalsQuery } from "reduxstore/api/YearsEndApi";
import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import { numberToCurrency } from "smart-ui-library";
import { useSelector } from "react-redux";
import { RootState } from "../../../../reduxstore/store";

interface TotalsContentProps {
  store: number | null;
  onLoadingChange?: (isLoading: boolean) => void;
}

const TotalsContent: React.FC<TotalsContentProps> = ({ store, onLoadingChange }) => {
  const { breakdownByStoreTotals } = useSelector((state: RootState) => state.yearsEnd);
  const profitYear = useDecemberFlowProfitYear();
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  // Use the API hook to fetch data
  const [getBreakdownByStoreTotals, { isFetching }] = useLazyGetBreakdownByStoreTotalsQuery();

  useEffect(() => {
    if (hasToken && store) {
      // Refetch when store changes
      getBreakdownByStoreTotals({
        profitYear: profitYear,
        storeNumber: store,
        pagination: {
          take: 10,
          skip: 0,
          sortBy: "",
          isSortDescending: false
        }
      });
    }
  }, [store, profitYear, getBreakdownByStoreTotals, hasToken]);

  // Notify parent component about loading state changes
  useEffect(() => {
    onLoadingChange?.(isFetching);
  }, [isFetching, onLoadingChange]);

  // Prepare data for display, using actual values when available
  const data = [
    {
      label: "Total Number of Employees:",
      value: breakdownByStoreTotals ? breakdownByStoreTotals.totalNumberEmployees : "0"
    },
    {
      label: "Total Beginning Balances:",
      value: breakdownByStoreTotals ? numberToCurrency(breakdownByStoreTotals.totalBeginningBalances, 2) : "0.00"
    },
    {
      label: "Total Earnings:",
      value: breakdownByStoreTotals ? numberToCurrency(breakdownByStoreTotals.totalEarnings, 2) : "0.00"
    },
    {
      label: "Total Contributions:",
      value: breakdownByStoreTotals ? numberToCurrency(breakdownByStoreTotals.totalContributions, 2) : "0.00"
    },
    {
      label: "Total Forfeitures:",
      value: breakdownByStoreTotals ? numberToCurrency(breakdownByStoreTotals.totalForfeitures, 2) : "0.00"
    },
    {
      label: "Total Disbursements:",
      value: breakdownByStoreTotals ? numberToCurrency(breakdownByStoreTotals.totalDisbursements, 2) : "0.00"
    },
    {
      label: "Total End Balances:",
      value: breakdownByStoreTotals ? numberToCurrency(breakdownByStoreTotals.totalEndBalances, 2) : "0.00"
    },
    {
      label: "Total Vested Balance:",
      value: breakdownByStoreTotals ? numberToCurrency(breakdownByStoreTotals.totalVestedBalance, 2) : "0.00"
    }
  ];

  return (
    <Grid
      container
      direction="column"
      width="100%">
      {breakdownByStoreTotals && (
        <>
          <Grid paddingX="24px">
            <Typography
              variant="h2"
              sx={{ color: "#0258A5", marginBottom: "16px" }}>
              {store && store > 0 ? `Totals for Store ${store}` : 'Totals For All Stores'}
            </Typography>
          </Grid>
          <Grid
            width="100%"
            paddingX="24px">
            <LabelValueSection data={data} />
          </Grid>
        </>
      )}
    </Grid>
  );
};

export default TotalsContent;
