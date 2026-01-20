import LabelValueSection from "@/components/LabelValueSection";
import { ValidationIcon } from "@/components/ValidationIcon/ValidationIcon";
import { ValidationResultsDialog } from "@/components/ValidationIcon/ValidationResultsDialog";
import useDecemberFlowProfitYear from "@/hooks/useDecemberFlowProfitYear";
import { useLazyGetBreakdownByStoreTotalsQuery } from "@/reduxstore/api/AdhocApi";
import { RootState } from "@/reduxstore/store";
import { CrossReferenceValidationGroup } from "@/types/validation/cross-reference-validation";
import { Grid, Typography } from "@mui/material";
import { useCallback, useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { numberToCurrency } from "smart-ui-library";

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

  // State for managing validation dialog
  const [dialogState, setDialogState] = useState<{
    isOpen: boolean;
    fieldName: string | null;
    groupName: string | null;
  }>({ isOpen: false, fieldName: null, groupName: null });

  // Helper function to find a validation group by name
  const getValidationGroup = useCallback(
    (groupName: string): CrossReferenceValidationGroup | null => {
      if (!breakdownByStoreTotals?.crossReferenceValidation?.validationGroups) {
        return null;
      }
      return (
        breakdownByStoreTotals.crossReferenceValidation.validationGroups.find((g) => g.groupName === groupName) || null
      );
    },
    [breakdownByStoreTotals?.crossReferenceValidation?.validationGroups]
  );

  // Handler to open validation dialog
  const handleValidationClick = useCallback((groupName: string, fieldName: string) => {
    setDialogState({
      isOpen: true,
      fieldName,
      groupName
    });
  }, []);

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

  const shouldShowValidation = store !== null && store < 0;

  // Prepare data for display, using actual values when available
  const data = [
    {
      label: "Total Number of Employees:",
      value: breakdownByStoreTotals ? breakdownByStoreTotals.totalNumberEmployees : "0"
    },
    {
      label: "Total Beginning Balances:",
      value: (
        <>
          {breakdownByStoreTotals ? numberToCurrency(breakdownByStoreTotals.totalBeginningBalances, 2) : "0.00"}
          {shouldShowValidation && (
            <ValidationIcon
              validationGroup={getValidationGroup("Beginning Balance")}
              fieldName="BeginningBalanceTotal"
              onClick={() => handleValidationClick("Beginning Balance", "BeginningBalanceTotal")}
              className="ml-2"
            />
          )}
        </>
      )
    },
    {
      label: "Total Earnings:",
      value: (
        <>
          {breakdownByStoreTotals ? numberToCurrency(breakdownByStoreTotals.totalEarnings, 2) : "0.00"}
          {shouldShowValidation && (
            <ValidationIcon
              validationGroup={getValidationGroup("Earnings Total")}
              fieldName="EarningsGrandTotal"
              onClick={() => handleValidationClick("Earnings Total", "EarningsGrandTotal")}
              className="ml-2"
            />
          )}
        </>
      )
    },
    {
      label: "Total Contributions:",
      value: (
        <>
          {breakdownByStoreTotals ? numberToCurrency(breakdownByStoreTotals.totalContributions, 2) : "0.00"}
          {shouldShowValidation && (
            <ValidationIcon
              validationGroup={getValidationGroup("Contributions Total")}
              fieldName="ContributionsGrandTotal"
              onClick={() => handleValidationClick("Contributions Total", "ContributionsGrandTotal")}
              className="ml-2"
            />
          )}
        </>
      )
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
          <ValidationResultsDialog
            open={dialogState.isOpen}
            onClose={() => setDialogState({ isOpen: false, fieldName: null, groupName: null })}
            validationGroup={dialogState.groupName ? getValidationGroup(dialogState.groupName) : null}
            fieldName={dialogState.fieldName}
          />
          <Grid paddingX="24px">
            <Typography
              variant="h2"
              sx={{ color: "#0258A5", marginBottom: "16px" }}>
              {store && store > 0 ? `Totals for Store ${store}` : "Totals For All Stores"}
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
