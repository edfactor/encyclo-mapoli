import type { ProfitYearRequest, SortedPaginationRequestDto } from "../common/api";

export interface MasterInquirySearch {
  endProfitYear?: number | null;
  startProfitMonth?: number | null;
  endProfitMonth?: number | null;
  socialSecurity?: number | null;
  name?: string | null;
  badgeNumber?: number | null;
  comment?: string | null;
  paymentType: "all" | "hardship" | "payoffs" | "rollovers";
  memberType: "all" | "employees" | "beneficiaries" | "none";
  contribution?: number | null;
  earnings?: number | null;
  forfeiture?: number | null;
  payment?: number | null;
  voids: boolean;
  pagination: SortedPaginationRequestDto;
}

export interface MasterInquiryDetail extends ProfitYearRequest {
  id: number;
  isEmployee: boolean;
  ssn: string;
  profitYearIteration: number;
  distributionSequence: number;
  profitCodeId: number;
  contribution: number | string;
  earnings: number;
  forfeiture: number;
  monthToDate: number;
  yearToDate: number;
  remark?: string;
  zeroContributionReasonId?: number;
  badgeNumber?: number;
  federalTaxes: number;
  stateTaxes: number;
  taxCodeId?: string;
  commentTypeId?: number;
  commentRelatedCheckNumber?: string;
  commentRelatedState?: string;
  commentRelatedOracleHcmId?: number;
  commentRelatedPsnSuffix?: number;
  commentIsPartialTransaction?: boolean;
  profitCodeName?: string;
  zeroContributionReasonName?: string;
  taxCodeName?: string;
  commentTypeName?: string;
  payFrequencyId?: number;
  transactionDate?: Date;
  currentIncomeYear?: number;
  currentHoursYear?: number;
  psnSuffix?: number;
}

export interface MasterInquiryRequest {
  id?: number;
  memberType?: number;
  badgeNumber?: number;
  psnSuffix?: number;
  ssn?: number;
  profitYear?: number;
  endProfitYear?: number;
  startProfitMonth?: number;
  endProfitMonth?: number;
  profitCode?: number;
  contributionAmount?: number;
  earningsAmount?: number;
  forfeitureAmount?: number;
  paymentAmount?: number;
  name?: string;
  paymentType?: number;
  pagination: SortedPaginationRequestDto;
  _timestamp?: number;
}

export interface MasterInquiryMemberRequest {
  memberType?: number;
  id?: number;
  profitYear?: number;
  skip?: number;
  take?: number;
  sortBy?: string;
  isSortDescending?: boolean;
}

export interface MasterInquiryResponseDto {
  isEmployee: boolean;
  id: number;
  ssn: string;
  profitYear: number;
  profitYearIteration: number;
  distributionSequence: number;
  profitCodeId: number;
  contribution: number | string;
  earnings: number;
  forfeiture: number;
  monthToDate: number;
  yearToDate: number;
  remark?: string;
  zeroContributionReasonId?: number;
  federalTaxes: number;
  stateTaxes: number;
  taxCodeId?: string;
  commentTypeId?: number;
  commentRelatedCheckNumber?: string;
  commentRelatedState?: string;
  commentRelatedOracleHcmId?: number;
  commentRelatedPsnSuffix?: number;
  commentIsPartialTransaction?: boolean;
  badgeNumber?: number;
  profitCodeName?: string;
  zeroContributionReasonName?: string;
  taxCodeName?: string;
  commentTypeName?: string;
  psnSuffix: number;
  payment?: number;
  vestedBalance?: number;
  vestingPercent?: number;
  currentBalance?: number;
  payFrequencyId: number;
  transactionDate: string;
  currentIncomeYear: number;
  currentHoursYear: number;
  employmentStatusId: string;
  employmentStatus?: string;
  badgesOfDuplicateSsns: number[];
}

export interface GroupedProfitSummaryDto {
  profitYear: number;
  monthToDate: number;
  totalContribution: number;
  totalEarnings: number;
  totalForfeiture: number;
  totalPayment: number;
  transactionCount: number;
}
