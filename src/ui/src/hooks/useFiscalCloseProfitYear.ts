import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";

/**
 * Hook to provide the selected profit year from the Fiscal Close flow
 * This centralized hook allows us to control the profit year consistently across all
 * Fiscal Close
 */
const useFiscalCloseProfitYear = (): number => {
  const { selectedProfitYearForFiscalClose } = useSelector((state: RootState) => state.yearsEnd);

  return selectedProfitYearForFiscalClose;
};

export default useFiscalCloseProfitYear;
