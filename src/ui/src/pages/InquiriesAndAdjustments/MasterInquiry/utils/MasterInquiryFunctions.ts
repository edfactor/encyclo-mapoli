import { MasterInquirySearch } from "reduxstore/types";
import { MAX_EMPLOYEE_BADGE_LENGTH } from "../../../../constants";

export const paymentTypeGetNumberMap: Record<string, number> = {
  all: 0,
  hardship: 1,
  payoffs: 2,
  rollovers: 3
};

export const memberTypeGetNumberMap: Record<string, number> = {
  all: 0,
  employees: 1,
  beneficiaries: 2,
  none: 3
};

export const isSimpleSearch = (masterInquiryRequestParams: MasterInquirySearch | null): boolean => {
  const simpleFound: boolean =
    !!masterInquiryRequestParams &&
    (!!masterInquiryRequestParams.name ||
      !!masterInquiryRequestParams.socialSecurity ||
      !!masterInquiryRequestParams.badgeNumber) &&
    !(
      !!masterInquiryRequestParams.startProfitMonth ||
      !!masterInquiryRequestParams.endProfitMonth ||
      !!masterInquiryRequestParams.contribution ||
      !!masterInquiryRequestParams.earnings ||
      !!masterInquiryRequestParams.forfeiture ||
      !!masterInquiryRequestParams.payment
    );
  return simpleFound;
};

export const splitFullPSN = (
  badgeNumber: string | undefined
): { psnSuffix: number | undefined; verifiedBadgeNumber: number | undefined } => {
  if (!badgeNumber) {
    return { psnSuffix: undefined, verifiedBadgeNumber: undefined };
  }

  let psnSuffix: number | undefined;
  let verifiedBadgeNumber: number | undefined;

  if (badgeNumber.length <= MAX_EMPLOYEE_BADGE_LENGTH) {
    verifiedBadgeNumber = parseInt(badgeNumber);
  } else {
    verifiedBadgeNumber = parseInt(badgeNumber.slice(0, -4));
    psnSuffix = parseInt(badgeNumber.slice(-4));
  }

  return { psnSuffix, verifiedBadgeNumber };
};
