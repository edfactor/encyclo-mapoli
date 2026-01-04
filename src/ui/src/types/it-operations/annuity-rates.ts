/// <summary>
/// Annuity rate by year and age.
/// </summary>
export interface AnnuityRateDto {
  year: number;
  age: number;
  singleRate: number;
  jointRate: number;
  dateModified?: string;
  userModified?: string;
}

/// <summary>
/// Request to update an annuity rate.
/// </summary>
export interface UpdateAnnuityRateRequest {
  year: number;
  age: number;
  singleRate: number;
  jointRate: number;
}

/// <summary>
/// Request to check which years have complete annuity rate data.
/// </summary>
export interface GetMissingAnnuityYearsRequest {
  startYear?: number;
  endYear?: number;
}

/// <summary>
/// Represents the completeness status of annuity rates for a single year.
/// </summary>
export interface AnnuityYearStatusDto {
  year: number;
  isComplete: boolean;
  missingAges: number[];
}

/// <summary>
/// Response indicating which years have complete/incomplete annuity rates.
/// </summary>
export interface MissingAnnuityYearsResponse {
  years: AnnuityYearStatusDto[];
}
