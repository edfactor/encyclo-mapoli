export type ProfitSharingAdjustmentsKey = {
  profitYear: number;
  badgeNumber: number;
  sequenceNumber: number;
};

export type ProfitSharingAdjustmentRowDto = {
  profitDetailId: number | null;
  rowNumber: number;
  profitYear: number;
  profitCodeId: number;
  contribution: number;
  earnings: number;
  forfeiture: number;
  activityDate: string | null;
  comment: string;
  isEditable: boolean;
};

export type GetProfitSharingAdjustmentsRequest = ProfitSharingAdjustmentsKey & {
  getAllRows?: boolean;
};

export type GetProfitSharingAdjustmentsResponse = ProfitSharingAdjustmentsKey & {
  demographicId: number | null;
  isOver21AtInitialHire: boolean;
  currentBalance: number;
  vestedBalance: number;
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
