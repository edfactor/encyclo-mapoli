/**
 * Bank and Bank Account types for administration management
 */

export interface BankDto {
  id: number;
  name: string;
  officeType: string | null;
  city: string | null;
  state: string | null;
  phone: string | null;
  status: string | null;
  isDisabled: boolean;
  createdAtUtc: string;
  createdBy: string;
  modifiedAtUtc: string | null;
  modifiedBy: string | null;
  accountCount: number;
}

export interface BankAccountDto {
  id: number;
  bankId: number;
  bankName: string;
  routingNumber: string;
  accountNumber: string; // Masked from backend
  isPrimary: boolean;
  isDisabled: boolean;
  servicingFedRoutingNumber: string | null;
  servicingFedAddress: string | null;
  fedwireTelegraphicName: string | null;
  fedwireLocation: string | null;
  fedAchChangeDate: string | null; // ISO date string
  fedwireRevisionDate: string | null; // ISO date string
  notes: string | null;
  effectiveDate: string | null; // ISO date string
  discontinuedDate: string | null; // ISO date string
  createdAtUtc: string;
  createdBy: string;
  modifiedAtUtc: string | null;
  modifiedBy: string | null;
}

export interface CreateBankRequest {
  name: string;
  officeType?: string | null;
  city?: string | null;
  state?: string | null;
  phone?: string | null;
  status?: string | null;
}

export interface UpdateBankRequest {
  id: number;
  name: string;
  officeType?: string | null;
  city?: string | null;
  state?: string | null;
  phone?: string | null;
  status?: string | null;
}

export interface CreateBankAccountRequest {
  bankId: number;
  routingNumber: string;
  accountNumber: string;
  isPrimary: boolean;
  servicingFedRoutingNumber?: string | null;
  servicingFedAddress?: string | null;
  fedwireTelegraphicName?: string | null;
  fedwireLocation?: string | null;
  fedAchChangeDate?: string | null;
  fedwireRevisionDate?: string | null;
  notes?: string | null;
  effectiveDate?: string | null;
  discontinuedDate?: string | null;
}

export interface UpdateBankAccountRequest {
  id: number;
  bankId: number;
  routingNumber: string;
  accountNumber: string;
  isPrimary: boolean;
  servicingFedRoutingNumber?: string | null;
  servicingFedAddress?: string | null;
  fedwireTelegraphicName?: string | null;
  fedwireLocation?: string | null;
  fedAchChangeDate?: string | null;
  fedwireRevisionDate?: string | null;
  notes?: string | null;
  effectiveDate?: string | null;
  discontinuedDate?: string | null;
}
