import { ISortParams, Paged, PaginationParams } from "smart-ui-library";

export interface SortedPaginationRequestDto extends PaginationParams, ISortParams {}

export interface RobustlyPaged<T = unknown> extends Paged<T> {
  timeoutOccurred?: boolean;
  isPartialResult?: boolean;
}

export interface ProfitYearRequest {
  profitYear: number;
}

export interface YearRangeRequest {
  beginProfitYear: number;
  endProfitYear: number;
}

export interface CalendarResponseDto {
  fiscalBeginDate: string;
  fiscalEndDate: string;
}

export interface FrozenProfitYearRequest extends ProfitYearRequest {
  useFrozenData: boolean;
}

export interface BadgeNumberRequest extends FrozenProfitYearRequest {
  badgeNumber: number | null;
}

export interface PagedReportResponse<T> {
  reportName: string;
  reportDate: string;
  startDate: string;
  endDate: string;
  dataSource: string;
  response: RobustlyPaged<T>;
}

export interface StartAndEndDateRequest extends ProfitYearRequest {
  beginningDate: string;
  endingDate: string;
  pagination: SortedPaginationRequestDto;
  excludeZeroBalance?: boolean;
  excludeZeroAndFullyVested?: boolean;
}

/**
 * Extends StartAndEndDateRequest with vested balance filtering capabilities.
 * Mirrors the backend FilterableStartAndEndDateRequest type.
 */
export interface FilterableStartAndEndDateRequest extends StartAndEndDateRequest {
  vestedBalanceValue?: number | null;
  vestedBalanceOperator?: number | null;
}

export interface FilterParams {
  reportId: number;
  badgeNumber?: number | null;
}

export interface ReportPreset {
  id: string;
  name: string;
  description: string;
  params: FilterParams;
  displayCriteria?: {
    ageRange?: string;
    hoursRange?: string;
    employeeStatus?: string;
    priorProfitShare?: string;
  };
}

export interface QPAY066xAdHocReportPreset {
  id: string;
  name: string;
  description: string;
  params: FilterParams;
  requiresDateRange: boolean;
}
