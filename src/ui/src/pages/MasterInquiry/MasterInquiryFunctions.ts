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

export const isSimpleSearch = (masterInquiryRequestParams: MasterInquiryRequest | null): boolean => {
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
