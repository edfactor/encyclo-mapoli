export type ProfitSharingAdjustmentsKey = {
  profitYear: number;
  oracleHcmId: number;
  sequenceNumber: number;
};

export type ProfitSharingAdjustmentRowDto = {
  profitDetailId: number | null;
  rowNumber: number;
  profitYear: number;
  profitYearIteration: number;
  profitCodeId: number;
  contribution: number;
  earnings: number;
  forfeiture: number;
  activityDate: string | null;
  comment: string;
  isEditable: boolean;
};

export type GetProfitSharingAdjustmentsRequest = ProfitSharingAdjustmentsKey;

export type GetProfitSharingAdjustmentsResponse = ProfitSharingAdjustmentsKey & {
  rows: ProfitSharingAdjustmentRowDto[];
};

export type SaveProfitSharingAdjustmentRowRequest = {
  profitDetailId: number | null;
  rowNumber: number;
  profitYearIteration: number;
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
