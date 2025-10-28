import { ProfitShareEditUpdateQueryParams } from "../../../../reduxstore/types";

/**
 * Checks if any form fields have been filled out
 * @param profitSharingEditQueryParams - The query parameters from the form
 * @returns true if at least one field has a value > 0
 */
export const wasFormUsed = (profitSharingEditQueryParams: ProfitShareEditUpdateQueryParams | null): boolean => {
  if (!profitSharingEditQueryParams) return false;

  return (
    (profitSharingEditQueryParams.contributionPercent ?? 0) > 0 ||
    (profitSharingEditQueryParams.earningsPercent ?? 0) > 0 ||
    (profitSharingEditQueryParams.incomingForfeitPercent ?? 0) > 0 ||
    (profitSharingEditQueryParams.maxAllowedContributions ?? 0) > 0 ||
    (profitSharingEditQueryParams.badgeToAdjust ?? 0) > 0 ||
    (profitSharingEditQueryParams.adjustContributionAmount ?? 0) > 0 ||
    (profitSharingEditQueryParams.adjustEarningsAmount ?? 0) > 0 ||
    (profitSharingEditQueryParams.adjustIncomingForfeitAmount ?? 0) > 0 ||
    (profitSharingEditQueryParams.badgeToAdjust2 ?? 0) > 0 ||
    (profitSharingEditQueryParams.adjustEarningsSecondaryAmount ?? 0) > 0
  );
};
