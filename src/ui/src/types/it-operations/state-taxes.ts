/// <summary>
/// State-level tax rate used during distribution calculations.
/// </summary>
export interface StateTaxRateDto {
  abbreviation: string;
  rate: number;
  dateModified?: string;
  userModified?: string;
}

/// <summary>
/// Request to update a state tax rate.
/// </summary>
export interface UpdateStateTaxRateRequest {
  abbreviation: string;
  rate: number;
}
