import { useEffect } from "react";
import { useSelector } from "react-redux";
import { useLazyGetFrozenStateResponseQuery } from "reduxstore/api/ItOperationsApi";
import { RootState } from "reduxstore/store";
import { useFakeTimeAwareYear } from "./useFakeTimeAwareDate";

/**
 * Hook to provide the frozen profit year from Fiscal Close flow
 *
 * Returns the frozen profit year if available, otherwise falls back to current calendar year
 */
const useFiscalCloseProfitYear = (): number => {
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const frozenStateResponse = useSelector((state: RootState) => state.frozen.frozenStateResponseData);
  const [fetchActiveFrozenState] = useLazyGetFrozenStateResponseQuery();
  const currentYear = useFakeTimeAwareYear();

  useEffect(() => {
    if (hasToken && !frozenStateResponse) {
      fetchActiveFrozenState();
    }
  }, [fetchActiveFrozenState, frozenStateResponse, hasToken]);

  return frozenStateResponse?.profitYear ?? currentYear;
};

export default useFiscalCloseProfitYear;
