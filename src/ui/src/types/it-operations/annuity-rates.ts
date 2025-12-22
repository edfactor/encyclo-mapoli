/// <summary>
/// Annuity rate by year and age.
/// </summary>
export interface AnnuityRateDto {
  year: number;
  age: number;
  singleRate: number;
  jointRate: number;
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
