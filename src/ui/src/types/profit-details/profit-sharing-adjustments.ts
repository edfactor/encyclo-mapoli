export type ProfitSharingAdjustmentsKey = {
  profitYear: number;
  badgeNumber: number;
};

export type ProfitSharingAdjustmentRowDto = {
  profitDetailId: number | null;
  rowNumber: number;
  profitYear: number;
  profitYearIteration: number;
  profitCodeId: number;
  profitCodeName: string;
  contribution: number;
  earnings: number;
  forfeiture: number;
  payment: number;
  federalTaxes: number;
  stateTaxes: number;
  taxCodeId: string;
  activityDate: string | null;
  comment: string;
  isEditable: boolean;
};

export type GetProfitSharingAdjustmentsRequest = ProfitSharingAdjustmentsKey & {
  getAllRows?: boolean;
};

export type GetProfitSharingAdjustmentsResponse = ProfitSharingAdjustmentsKey & {
  demographicId: number;
  rows: ProfitSharingAdjustmentRowDto[];
};

export type SaveProfitSharingAdjustmentRowRequest = {
  profitDetailId: number | null;
  rowNumber: number;
  profitCodeId: number;
  contribution: number;
  earnings: number;
  forfeiture: number;
  activityDate: string | null;
  comment: string;
};

export type SaveProfitSharingAdjustmentsRequest = ProfitSharingAdjustmentsKey & {
  rows: SaveProfitSharingAdjustmentRowRequest[];
};
