import type { PagedReportResponse, SortedPaginationRequestDto } from "../common/api";

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
