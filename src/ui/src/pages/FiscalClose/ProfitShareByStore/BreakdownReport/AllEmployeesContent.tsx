import { GRID_KEYS } from "../../../../constants";
import BreakdownSummaryGrid from "./BreakdownSummaryGrid";

/**
 * Component displaying breakdown summary for all employees by vesting category.
 * Shows counts grouped by store buckets (STE 1-140, 700, 701, 800, 801, 802, 900).
 */
const AllEmployeesContent: React.FC = () => {
  return (
    <BreakdownSummaryGrid
      title="All Employees (By report section)"
      preferenceKey={GRID_KEYS.BREAKDOWN_REPORT_SUMMARY}
      errorMessage="Failed to load All Employees data. Please try again."
    />
  );
};

export default AllEmployeesContent;

