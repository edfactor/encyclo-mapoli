import { ICellRendererParams, IHeaderParams } from "ag-grid-community";
import type { ProfitYearRequest, SortedPaginationRequestDto } from "../common/api";

export interface ForfeitureDetail extends ProfitYearRequest {
  forfeiture: number;
  remark: string;
  hoursCurrentYear: number;
  wages: number;
  enrollmentId: number;
  enrollmentName: string;
  profitDetailId: number;
}

export interface UnForfeit {
  badgeNumber: number;
  fullName: string;
  ssn: string;
  reHiredDate: string;
  companyContributionYears: number;
  employmentStatus: string;
  netBalanceLastYear: number;
  vestedBalanceLastYear: number;
  hireDate: string;
  terminationDate: string;
  storeNumber: number;
  details: ForfeitureDetail[];
}

export interface SuggestForfeitureAdjustmentRequest extends SortedPaginationRequestDto {
  ssn?: string;
  badge?: string;
  profitYear: number;
}

export interface ForfeitureAdjustmentUpdateRequest {
  badgeNumber: number;
  forfeitureAmount: number;
  classAction: boolean;
  profitYear: number;
  offsettingProfitDetailId?: number;
}

export interface ForfeitureAdjustmentDetail {
  demographicId: number;
  badgeNumber: number;
  startingBalance: number;
  forfeitureAmount: number;
  netBalance: number;
  netVested: number;
}

export interface SuggestedForfeitResponse {
  badgeNumber: number;
  suggestedForfeitAmount: number;
  demographicId: number;
}

// Alias for backwards compatibility
export type SuggestForfeitAmountResponse = SuggestedForfeitResponse;

// Rehire forfeitures editing functionality
export interface UnForfeitEditedValues {
  [rowKey: string]: {
    value: number;
    hasError: boolean;
  };
}

export interface UnForfeitHeaderComponentProps extends IHeaderParams {
  addRowToSelectedRows: (id: number) => void;
  removeRowFromSelectedRows: (id: number) => void;
  onBulkSave?: (requests: ForfeitureAdjustmentUpdateRequest[]) => Promise<void>;
}

export interface UnForfeitSaveButtonCellParams extends ICellRendererParams {
  removeRowFromSelectedRows: (id: number) => void;
  addRowToSelectedRows: (id: number) => void;
  onSave?: (request: ForfeitureAdjustmentUpdateRequest) => Promise<void>;
}

export interface UnForfeitUpdatePayload {
  badgeNumber: number;
  profitYear: number;
  suggestedForfeit: number;
}

export interface UnForfeitSelectedRow {
  id: number;
  badgeNumber: number;
  profitYear: number;
  suggestedForfeit: number;
}
