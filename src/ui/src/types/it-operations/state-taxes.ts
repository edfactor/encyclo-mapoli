/// <summary>
/// State-level tax rate used during distribution calculations.
/// </summary>
export interface StateTaxRateDto {
  abbreviation: string;
  rate: number;
}

/// <summary>
/// Request to update a state tax rate.
/// </summary>
export interface UpdateStateTaxRateRequest {
  abbreviation: string;
  rate: number;
}
