import { Paged, PaginationParams } from "smart-ui-library";

export enum ImpersonationRoles {
  FinanceManager = "Finance-Manager",
  DistributionsClerk = "Distribution-Clerk",
  HardshipAdministrator = "Hardship-Administrator",
  ProfitSharingAdministrator = "Profit-Sharing-Administrator"
}

interface ImpersonationRequest {
  impersonation: ImpersonationRoles;
}

export interface DemographicBadgesNotInPayprofitResponse {
  reportName: string;
  reportDate: string;
  response: Paged<DemographicBadgesNotInPayprofit>;
}

export interface DemographicBadgesNotInPayprofit {
  employeeBadge: number;
  employeeSsn: number;
  empoyeeName: string;
  store: number;
  status: string;
}

export interface DemographicBadgesNotInPayprofitRequestDto extends ImpersonationRequest {
  pagination: PaginationParams;
}

export interface PagedReportResponse<T> {
  reportName: string;
  reportDate: string;
  response: Paged<T>;
}

export interface DistributionsAndForfeitures {}

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
  storeNumber: number;
  profitSharingRecords: number;
  hoursCurrentYear: number;
  hoursLastYear: number;
  incomeCurrentYear: number;
}

export interface DuplicateSSNsRequestDto extends ImpersonationRequest {
  profitYear: number;
  pagination: PaginationParams;
}

export interface MissingCommasInPYNameRequestDto extends ImpersonationRequest {
  pagination: PaginationParams;
}

export interface MissingCommasInPYName {
  exployeeBadge: number;
  employeeSsn: number;
  employeeName: string;
}