import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { CalendarResponseDto } from "../reduxstore/types";

/**
 * Custom hook to retrieve the fiscal calendar year data from the Redux store.
 *
 * This hook accesses the global Redux state and specifically extracts the `accountingYearData`
 * from the `lookups` slice of the store.
 *
 * @returns {CalendarResponseDto | null} The fiscal calendar year data (accountingYearData).
 */
const useFiscalCalendarYear = (): CalendarResponseDto | null => {
  const { accountingYearData } = useSelector((state: RootState) => state.lookups);

  return accountingYearData;
};

export default useFiscalCalendarYear; 