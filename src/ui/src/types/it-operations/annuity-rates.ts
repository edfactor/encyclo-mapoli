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
