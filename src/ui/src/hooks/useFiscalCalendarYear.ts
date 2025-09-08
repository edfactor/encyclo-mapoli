import { useCallback, useEffect } from "react";
import { useSelector } from "react-redux";
import { useLazyGetAccountingRangeQuery, useLazyGetAccountingYearQuery } from "reduxstore/api/LookupsApi";
import { RootState } from "reduxstore/store";
import { CalendarResponseDto } from "../reduxstore/types";
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

/**
 * Overload: Fetches accounting range from beginYear to current December flow profit year.
 * @param yearsBack The number of years to go back from the current year.
 * @returns [trigger, result] from useLazyGetAccountingRangeQuery
 */
export const useLazyGetAccountingRangeToCurrent = (yearsBack: number) => {
  const endYear = useDecemberFlowProfitYear();
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const [trigger, result] = useLazyGetAccountingRangeQuery();
  // Memoized wrapped trigger: only call if hasToken is true
  const wrappedTrigger = useCallback(() => {
    if (hasToken) {
      return trigger({ beginProfitYear: endYear - yearsBack, endProfitYear: endYear });
    }
    return Promise.resolve(undefined);
  }, [hasToken, endYear, yearsBack, trigger]);
  return [wrappedTrigger, result] as const;
};

export default useFiscalCalendarYear;
