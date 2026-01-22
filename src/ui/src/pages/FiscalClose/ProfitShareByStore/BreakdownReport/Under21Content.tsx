import { GRID_KEYS } from "../../../../constants";
import BreakdownSummaryGrid from "./BreakdownSummaryGrid";

/**
 * Component displaying breakdown summary for employees under 21 by vesting category.
 * Shows counts grouped by store buckets (STE 1-140, 700, 701, 800, 801, 802, 900).
 * Filters to only include participants under 21 years of age.
 */
const Under21Content: React.FC = () => {
  return (
    <BreakdownSummaryGrid
      title="Under 21 Employees (By report section)"
      preferenceKey={GRID_KEYS.UNDER_21_BREAKDOWN_REPORT}
      under21Participants
      errorMessage="Failed to load Under 21 employee data. Please try again."
    />
  );
};

export default Under21Content;
