export type ProfitSharingAdjustmentsKey = {
  profitYear: number;
  badgeNumber: number;
};

export type ProfitSharingAdjustmentRowDto = {
  profitDetailId: number | null;
  /**
   * Indicates whether this profit detail has already been reversed (another row references it).
   * When true, the user should not be allowed to reverse this row again.
   */
  hasBeenReversed: boolean;
  /**
   * The ID of the profit detail that this adjustment reverses (for tracking purposes).
   * Used for insert rows to track which original row was reversed.
   */
  reversedFromProfitDetailId?: number | null;
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
  /**
   * The ID of the profit detail that this adjustment reverses (for tracking purposes).
   * When provided, the system will validate that the source has not already been reversed.
   */
  reversedFromProfitDetailId: number | null;
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
