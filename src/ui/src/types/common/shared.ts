export interface MissiveResponse {
  id: number;
  message: string;
  description: string;
  severity: string;
}

export interface StateListResponse {
  abbreviation: string;
  name: string;
}

export interface TaxCodeResponse {
  id: string;
  name: string;
}

export interface RowCountResult {
  tableName: string;
  rowCount: number;
}

export interface ContactInfoDto {
  fullName?: string;
  lastName: string;
  firstName: string;
  middleName?: string;
  phoneNumber?: string;
  mobileNumber?: string;
  emailAddress?: string;
}

export interface AddressDto {
  street?: string;
  street2?: string;
  city?: string;
  state?: string;
  postalCode?: string;
  countryIso?: string;
}
