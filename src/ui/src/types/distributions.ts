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
  states?: string[];
  taxCodes?: string[];
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

export interface UnattributedTotals {
  count: number;
  federalTax: number;
  stateTax: number;
  netProceeds: number;
}

export interface DistributionsAndForfeitureTotalsResponse extends PagedReportResponse<DistributionsAndForfeitures> {
  distributionTotal: number;
  stateTaxTotal: number;
  federalTaxTotal: number;
  forfeitureTotal: number;
  forfeitureRegularTotal: number;
  forfeitureAdministrativeTotal: number;
  forfeitureClassActionTotal: number;
  stateTaxTotals: Record<string, number>;
  unattributedTotals?: UnattributedTotals | null;
  hasUnattributedRecords?: boolean;
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
  forfeitType: string | null;
  age: string;
  taxCode: string | null;
  otherName: string | null;
  otherSsn: string | null;
  enrolled: boolean | null;
}

export interface DistributionsAndForfeituresQueryParams {
  startDate?: string;
  endDate?: string;
  states?: string[];
  taxCodes?: string[];
}

// State Tax Lookup
export interface StateTaxLookupResponse {
  state: string;
  stateTaxRate: number;
}

// Create Distribution Request (matches backend CreateDistributionRequest)
export interface CreateDistributionRequest {
  badgeNumber: number;
  statusId: string;
  frequencyId: string;
  payeeId?: number | null;
  thirdPartyPayee?: {
    payee?: string | null;
    name?: string | null;
    account?: string | null;
    address: {
      street: string;
      street2?: string | null;
      street3?: string | null;
      street4?: string | null;
      city?: string | null;
      state?: string | null;
      postalCode?: string | null;
      countryIso?: string;
    };
    memo?: string | null;
  } | null;
  forTheBenefitOfPayee?: string | null;
  forTheBenefitOfAccountType?: string | null;
  tax1099ForEmployee: boolean;
  tax1099ForBeneficiary: boolean;
  federalTaxPercentage: number;
  stateTaxPercentage: number;
  grossAmount: number;
  federalTaxAmount: number;
  stateTaxAmount: number;
  checkAmount: number;
  taxCodeId: string;
  isDeceased: boolean;
  genderId?: string | null;
  isQdro: boolean;
  memo?: string | null;
  isRothIra: boolean;
}

// Create/Update Distribution Response
export interface CreateOrUpdateDistributionResponse {
  id: number;
  badgeNumber: number;
  statusId: string;
  frequencyId: string;
  federalTaxPercentage: number;
  federalTaxAmount: number;
  stateTaxPercentage: number;
  stateTaxAmount: number;
  grossAmount: number;
  checkAmount: number;
  taxCodeId: string;
  maskSsn: string;
  paymentSequence: number;
  createdAt: string;
  memo?: string | null;
}

// Edit Distribution Request (extends CreateDistributionRequest with id)
export interface EditDistributionRequest extends CreateDistributionRequest {
  id: number;
}
