import { ISortParams, Paged, PaginationParams } from "smart-ui-library";

export interface SortedPaginationRequestDto extends PaginationParams, ISortParams {}

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
  response: Paged<T>;
}

export interface StartAndEndDateRequest extends ProfitYearRequest {
  beginningDate: string;
  endingDate: string;
  pagination: SortedPaginationRequestDto;
  excludeZeroBalance?: boolean;
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
}

export interface ApiResponse<T> {}