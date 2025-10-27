import { MasterInquiryRequest, MasterInquirySearch } from "reduxstore/types";
import { memberTypeGetNumberMap, paymentTypeGetNumberMap, splitFullPSN } from "./MasterInquiryFunctions";

/**
 * Transforms form data into API request parameters
 */
export const transformSearchParams = (data: MasterInquirySearch, profitYear: number): MasterInquiryRequest => {
  const { psnSuffix, verifiedBadgeNumber } = splitFullPSN(data.badgeNumber?.toString());

  return {
    pagination: {
      skip: data.pagination?.skip || 0,
      take: data.pagination?.take || 5,
      sortBy: data.pagination?.sortBy || "badgeNumber",
      isSortDescending: data.pagination?.isSortDescending ?? true
    },
    endProfitYear: data.endProfitYear ?? profitYear,
    ...(!!data.startProfitMonth && { startProfitMonth: data.startProfitMonth }),
    ...(!!data.endProfitMonth && { endProfitMonth: data.endProfitMonth }),
    ...(!!data.socialSecurity && { ssn: Number(data.socialSecurity) }),
    ...(!!data.name && { name: data.name }),
    ...(verifiedBadgeNumber !== undefined && { badgeNumber: verifiedBadgeNumber }),
    ...(psnSuffix !== undefined && { psnSuffix }),
    ...(!!data.paymentType && { paymentType: paymentTypeGetNumberMap[data.paymentType] }),
    ...(!!data.memberType && { memberType: memberTypeGetNumberMap[data.memberType] }),
    ...(!!data.contribution && { contributionAmount: data.contribution }),
    ...(!!data.earnings && { earningsAmount: data.earnings }),
    ...(!!data.forfeiture && { forfeitureAmount: data.forfeiture }),
    ...(!!data.payment && { paymentAmount: data.payment }),
    _timestamp: Date.now()
  };
};
