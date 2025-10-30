import { ExecutiveHoursAndDollarsRequestDto } from "../../../../types/fiscal/executive";

/**
 * Determines if the search is a simple search (badge, SSN, or name only)
 * vs a complex search (includes checkbox filters)
 * @param searchParams - The search parameters
 * @returns true if it's a simple search, false otherwise
 */
export const isSimpleSearch = (searchParams: ExecutiveHoursAndDollarsRequestDto | null): boolean => {
  if (!searchParams) return false;

  const hasBasicCriteria =
    !!searchParams.badgeNumber || !!searchParams.socialSecurity || !!searchParams.fullNameContains;

  const hasComplexCriteria =
    searchParams.hasExecutiveHoursAndDollars === true || searchParams.isMonthlyPayroll === true;

  // It's a simple search if we have basic criteria and no complex criteria
  return hasBasicCriteria && !hasComplexCriteria;
};
