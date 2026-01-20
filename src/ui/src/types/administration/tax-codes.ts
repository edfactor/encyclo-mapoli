/// <summary>
/// Tax code configuration used for distributions and forfeitures.
/// </summary>
export interface TaxCodeAdminDto {
  id: string;
  name: string;
  isAvailableForDistribution: boolean;
  isAvailableForForfeiture: boolean;
  isProtected: boolean;
}

/// <summary>
/// Request to create a new tax code.
/// </summary>
export interface CreateTaxCodeRequest {
  id: string;
  name: string;
  isAvailableForDistribution: boolean;
  isAvailableForForfeiture: boolean;
  isProtected: boolean;
}

/// <summary>
/// Request to update a tax code.
/// </summary>
export interface UpdateTaxCodeRequest {
  id: string;
  name: string;
  isAvailableForDistribution: boolean;
  isAvailableForForfeiture: boolean;
  isProtected: boolean;
}
