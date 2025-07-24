import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";

/**
 * Hook to provide the selected profit year from the December Process flow
 * This centralized hook allows us to control the profit year consistently across all
 * December Process screens
 */
const useDecemberFlowProfitYear = (): number => {
  const { selectedProfitYearForDecemberActivities } = useSelector((state: RootState) => state.yearsEnd);

  return selectedProfitYearForDecemberActivities;
};

export default useDecemberFlowProfitYear;
