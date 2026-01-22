import { Typography, CircularProgress } from "@mui/material";
import LabelValueSection from "../../../../components/LabelValueSection";
import { Grid } from "@mui/material";
import { useEffect } from "react";
import { useLazyGetBreakdownByStoreTotalsQuery } from "reduxstore/api/AdhocApi";
import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import { numberToCurrency } from "smart-ui-library";
import { useSelector } from "react-redux";
import { RootState } from "../../../../reduxstore/store";

interface TotalsContentProps {
  store: number | null;
  onLoadingChange?: (isLoading: boolean) => void;
}

const TotalsContent: React.FC<TotalsContentProps> = ({ store, onLoadingChange }) => {
  const { breakdownByStoreTotals, breakdownGrandTotals } = useSelector((state: RootState) => state.yearsEnd);
  const profitYear = useDecemberFlowProfitYear();
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  // Use the API hook to fetch data
  const [getBreakdownByStoreTotals, { isFetching }] = useLazyGetBreakdownByStoreTotalsQuery();

  useEffect(() => {
    if (hasToken && store !== null) {
      if (store === -1) {
        // For all stores, wait for breakdownGrandTotals to be loaded first
        // (since getBreakdownGrandTotals clears breakdownByStoreTotals)
        if (breakdownGrandTotals) {
          getBreakdownByStoreTotals({
            profitYear: profitYear,
            storeNumber: -1,
            pagination: {
              take: 10,
              skip: 0,
              sortBy: "",
              isSortDescending: false
            }
          });
        }
      } else if (store > 0) {
        // For individual stores, fetch immediately
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
    }
  }, [store, profitYear, breakdownGrandTotals, getBreakdownByStoreTotals, hasToken]);

  // Notify parent component about loading state changes
  useEffect(() => {
    onLoadingChange?.(isFetching);
  }, [isFetching, onLoadingChange]);

  // Use breakdownByStoreTotals for all cases (works for both store > 0 and store === -1)
  const totalsToDisplay = breakdownByStoreTotals;

  // Prepare data for display, using actual values when available
  const data = [
    {
      label: "Total Number of Employees:",
      value: totalsToDisplay ? totalsToDisplay.totalNumberEmployees.toString() : "0"
    },
    {
      label: "Total Beginning Balances:",
      value: totalsToDisplay ? numberToCurrency(totalsToDisplay.totalBeginningBalances, 2) : "0.00"
    },
    {
      label: "Total Earnings:",
      value: totalsToDisplay ? numberToCurrency(totalsToDisplay.totalEarnings, 2) : "0.00"
    },
    {
      label: "Total Contributions:",
      value: totalsToDisplay ? numberToCurrency(totalsToDisplay.totalContributions, 2) : "0.00"
    },
    {
      label: "Total Forfeitures:",
      value: totalsToDisplay ? numberToCurrency(totalsToDisplay.totalForfeitures, 2) : "0.00"
    },
    {
      label: "Total Disbursements:",
      value: totalsToDisplay ? numberToCurrency(totalsToDisplay.totalDisbursements, 2) : "0.00"
    },
    {
      label: "Total End Balances:",
      value: totalsToDisplay ? numberToCurrency(totalsToDisplay.totalEndBalances, 2) : "0.00"
    },
    {
      label: "Total Vested Balance:",
      value: totalsToDisplay ? numberToCurrency(totalsToDisplay.totalVestedBalance, 2) : "0.00"
    }
  ];

  return (
    <Grid
      container
      direction="column"
      width="100%">
      <Grid paddingX="24px">
        <Typography
          variant="h2"
          sx={{ color: "#0258A5", marginBottom: "16px" }}>
          {store && store > 0 ? `Totals for Store ${store}` : "Totals For All Stores"}
        </Typography>
      </Grid>
      <Grid width="100%">
        {isFetching ? (
          <Grid sx={{ display: "flex", justifyContent: "center", padding: 4 }}>
            <CircularProgress />
          </Grid>
        ) : totalsToDisplay ? (
          <Grid
            width="100%"
            paddingX="24px">
            <LabelValueSection data={data} />
          </Grid>
        ) : null}
      </Grid>
    </Grid>
  );
};

export default TotalsContent;
