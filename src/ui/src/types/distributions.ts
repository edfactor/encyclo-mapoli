import type { PagedReportResponse, SortedPaginationRequestDto } from "./common/api";

export interface DistributionSearchRequest {
  ssn?: string;
  badgeNumber?: number;
  psnSuffix?: number;
  memberType?: number | null; // 1 = employees, 2 = beneficiaries, null = all
  distributionFrequencyId?: string;
  distributionStatusId?: string;
  distributionStatusIds?: string[];
  taxCodeId?: string;
  minGrossAmount?: number;
  maxGrossAmount?: number;
  minCheckAmount?: number;
  maxCheckAmount?: number;
  skip?: number;
  take?: number;
  sortBy?: string;
  isSortDescending?: boolean;
}

export interface DistributionSearchResponse {
  id: number;
  paymentSequence: number;
  ssn: string;
  badgeNumber: number | null;
  fullName: string;
  isExecutive: boolean;
  isEmployee: boolean;
  frequencyId: string;
  frequencyName: string;
  statusId: string;
  statusName: string;
  taxCodeId: string;
  taxCodeName: string;
  grossAmount: number;
  federalTax: number;
  stateTax: number;
  checkAmount: number;
  demographicId: number | null;
  beneficiaryId: number | null;
}

export interface DistributionSearchFormData {
  socialSecurity?: string;
  badgeNumber?: string;
  memberType?: string; // "all", "employees", "beneficiaries"
  frequency?: string | null;
  paymentFlag?: string | null;
  paymentFlags?: string[];
  taxCode?: string | null;
  minGrossAmount?: string;
  maxGrossAmount?: string;
  minCheckAmount?: string;
  maxCheckAmount?: string;
}

export interface DistributionSearchResultDto {
  results: DistributionSearchResponse[];
  total: number;
}

// December Activities Distribution types
export interface DistributionsAndForfeituresRequestDto {
  startDate?: string;
  endDate?: string;
  pagination: SortedPaginationRequestDto;
}

export interface Distribution {
  badgeNumber: number;
  statusId: "A" | "I" | "D" | "T";
  frequencyId: "M" | "W";
  payeeId: number | null;
  thirdPartyPayee: string | null;
  forTheBenefitOfPayee: string | null;
  forTheBenefitOfAccountType: string | null;
  tax1099ForEmployee: boolean;
  tax1099ForBeneficiary: boolean;
  federalTaxPercentage: number;
  stateTaxPercentage: number;
  grossAmount: number;
  federalTaxAmount: number;
  stateTaxAmount: number;
  checkAmount: number;
  taxCodeId:
    | "0"
    | "1"
    | "2"
    | "3"
    | "4"
    | "5"
    | "6"
    | "7"
    | "8"
    | "9"
    | "A"
    | "B"
    | "C"
    | "D"
    | "E"
    | "F"
    | "G"
    | "H"
    | "P";

  isDeceased: boolean;
  genderId: string | null;
  isQdro: boolean;
  memo: string | null;
  isRothIra: boolean;
}

export interface DistributionsAndForfeitureTotalsResponse extends PagedReportResponse<DistributionsAndForfeitures> {
  distributionTotal: number;
  stateTaxTotal: number;
  federalTaxTotal: number;
  forfeitureTotal: number;
  stateTaxTotals: Record<string, number>;
}

export interface DistributionsAndForfeitures {
  badgeNumber: number;
  psnSuffix: number;
  employeeName: string;
  ssn: string;
  date: string;
  distributionAmount: number;
  stateTax: number;
  state: string | null;
  federalTax: number;
  forfeitAmount: number;
  age: number;
  taxCode: string | null;
  otherName: string | null;
  otherSsn: string | null;
  enrolled: boolean | null;
}

export interface DistributionsAndForfeituresQueryParams {
  startDate?: string;
  endDate?: string;
}
