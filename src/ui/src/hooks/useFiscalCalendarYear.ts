import { useEffect } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { CalendarResponseDto } from "../reduxstore/types";
import { useLazyGetAccountingYearQuery } from "reduxstore/api/LookupsApi";
import useDecemberFlowProfitYear from "./useDecemberFlowProfitYear";

/**
 * Custom hook to retrieve the fiscal calendar year data.
 * This hook fetches accounting year data based on the current profit year
 * and returns the data from the Redux store.
 *
 * @returns {CalendarResponseDto | null} The fiscal calendar year data.
 */
const useFiscalCalendarYear = (): CalendarResponseDto | null => {
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const { accountingYearData } = useSelector((state: RootState) => state.lookups);
  const [fetchAccountingYear, { isLoading }] = useLazyGetAccountingYearQuery();
  const profitYear = useDecemberFlowProfitYear();

  useEffect(() => {
    // Fetch accounting year data when profit year is available
    if (profitYear && hasToken) {
      fetchAccountingYear({
        profitYear: profitYear
      });
    }
  }, [profitYear, hasToken, fetchAccountingYear]);

  return accountingYearData;
};

export default useFiscalCalendarYear;