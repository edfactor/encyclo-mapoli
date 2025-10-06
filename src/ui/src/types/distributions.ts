export interface DistributionSearchRequest {
  ssn?: string;
  badgeNumber?: number;
  psnSuffix?: number;
  distributionFrequencyId?: string;
  distributionStatusId?: string;
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
  ssn: string;
  badgeNumber: number | null;
  fullName: string;
  isExecutive: boolean;
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
}

export interface DistributionSearchFormData {
  ssnOrMemberNumber?: string;
  frequency?: string | null;
  paymentFlag?: string | null;
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
