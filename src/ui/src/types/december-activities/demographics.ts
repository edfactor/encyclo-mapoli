import type { PagedReportResponse, ProfitYearRequest, SortedPaginationRequestDto } from "../common/api";

export type DemographicBadgesNotInPayprofitResponse = PagedReportResponse<DemographicBadgesNotInPayprofit>;

export interface DemographicBadgesNotInPayprofit {
  badgeNumber: number;
  ssn: number;
  employeeName: string;
  store: number;
  status: string;
  statusName: string;
}

export interface DemographicBadgesNotInPayprofitRequestDto extends ProfitYearRequest {
  pagination: SortedPaginationRequestDto;
}

export interface DuplicateSSNAddress {
  street: string;
  street2: string | null;
  city: string;
  state: string;
  postalCode: string;
  countryIso: string;
}

export interface DuplicateSSNDetail {
  badgeNumber: number;
  ssn: string;
  name: string;
  address: DuplicateSSNAddress;
  hireDate: string;
  terminationDate: string | null;
  rehireDate: string | null;
  status: string;
  employmentStatusName: string;
  storeNumber: number;
  profitSharingRecords: number;
  hoursCurrentYear: number;
  hoursLastYear: number;
  incomeCurrentYear: number;
}

export interface DuplicateSSNsRequestDto extends ProfitYearRequest {
  pagination: SortedPaginationRequestDto;
}

export interface MissingCommasInPYNameRequestDto {
  pagination: SortedPaginationRequestDto;
}

export interface MissingCommasInPYName {
  badgeNumber: number;
  ssn: number;
  employeeName: string;
}

export interface DuplicateNameAndBirthdayRequestDto extends ProfitYearRequest {
  pagination: SortedPaginationRequestDto;
}

export interface DuplicateNameBirthdayAddress {
  street: string;
  street2: string | null;
  city: string;
  state: string;
  postalCode: string;
  countryIso: string;
}

export interface DuplicateNameAndBirthday {
  badgeNumber: number;
  ssn: string;
  name: string;
  dateOfBirth: string;
  address: DuplicateNameBirthdayAddress;
  years: number;
  hireDate: string;
  terminationDate: string | null;
  status: string;
  storeNumber: number;
  count: number;
  netBalance: number;
  hoursCurrentYear: number;
  incomeCurrentYear: number;
  employmentStatusName: string;
}

export interface NegativeEtvaForSSNsOnPayprofitRequestDto extends ProfitYearRequest {
  pagination: SortedPaginationRequestDto;
}

export interface NegativeEtvaForSSNsOnPayProfit {
  badgeNumber: number;
  ssn: number;
  etvaValue: number;
}

export interface FreezeDemographicsRequest {
  asOfDateTime: string;
  profitYear: number;
}

export interface FrozenStateResponse {
  id: number;
  profitYear: number;
  frozenBy: string;
  asOfDateTime: string;
  createdDateTime: string;
  isActive: boolean;
}
