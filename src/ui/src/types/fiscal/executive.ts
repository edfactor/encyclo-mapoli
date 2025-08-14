import type { 
  SortedPaginationRequestDto,
  ProfitYearRequest 
} from "../common/api";

export interface ExecutiveHoursAndDollarsRequestDto extends ProfitYearRequest {
  badgeNumber?: number;
  socialSecurity?: number;
  fullNameContains?: string;
  hasExecutiveHoursAndDollars: boolean;
  isMonthlyPayroll: boolean;
  pagination: SortedPaginationRequestDto;
}

// This is the state of an editable executive's hours and dollars
export interface ExecutiveHoursAndDollarsRow {
  badgeNumber: number;
  executiveHours: number;
  executiveDollars: number;
}

// This structure is used to store the state of the Manage
// Executive Hours and Dollars Grid; which allows editing
// and writing of data
export interface ExecutiveHoursAndDollarsGrid {
  executiveHoursAndDollars: ExecutiveHoursAndDollarsRow[];
  profitYear: number | null;
}

export interface ExecutiveHoursAndDollars {
  badgeNumber: number;
  fullName: string;
  storeNumber: number;
  socialSecurity: number;
  hoursExecutive: number;
  incomeExecutive: number;
  currentHoursYear: number;
  currentIncomeYear: number;
  payFrequencyId: number;
  payFrequencyName: string;
  employmentStatusId: string;
  employmentStatusName: string;
}

export interface EmployeeWagesForYearRequestDto extends ProfitYearRequest {
  pagination: SortedPaginationRequestDto;
}

export interface EmployeeWagesForYear {
  badgeNumber: number;
  hoursExecutive: number;
  incomeExecutive: number;
}

export interface ExecutiveHoursAndDollarsQueryParams extends ProfitYearRequest {
  badgeNumber: number;
  socialSecurity: number;
  fullNameContains: string;
  hasExecutiveHoursAndDollars: boolean;
  isMonthlyPayroll: boolean;
}