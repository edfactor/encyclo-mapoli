/// <summary>
/// RMD (Required Minimum Distribution) factor used for calculating minimum distributions.
/// </summary>
export interface RmdFactorDto {
  age: number;
  factor: number;
}

/// <summary>
/// Request to update an RMD factor.
/// </summary>
export interface UpdateRmdFactorRequest {
  age: number;
  factor: number;
}
