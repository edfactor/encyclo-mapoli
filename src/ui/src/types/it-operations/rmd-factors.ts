/// <summary>
/// RMD factor used for Required Minimum Distribution calculations.
/// </summary>
export interface RmdFactorDto {
  age: number;
  factor: number;
}

/// <summary>
/// Request to insert or update an RMD factor.
/// </summary>
export interface UpdateRmdFactorRequest {
  age: number;
  factor: number;
}
