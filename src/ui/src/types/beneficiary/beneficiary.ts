import { Paged } from "smart-ui-library";
import type { PagedReportResponse, ProfitYearRequest, SortedPaginationRequestDto } from "../common/api";
import type { AddressDto, ContactInfoDto } from "../common/shared";

export interface BeneficiaryContactDto {
  id: number;
  ssn: string;
  dateOfBirth: Date;
  address?: AddressDto;
  contactInfo?: ContactInfoDto;
  createdDate: Date;
}

export interface BeneficiaryKindDto {
  id: string;
  name?: string;
}

export interface BeneficiaryDto {
  id: number;
  psnSuffix: number;
  badgeNumber: number;
  demographicId: number;
  psn: string;
  // Contact information
  ssn: string;
  dateOfBirth: Date;
  address?: AddressDto;
  contactInfo?: ContactInfoDto;
  createdDate: Date;
  // Contact Info fields
  fullName?: string;
  lastName: string;
  firstName: string;
  middleName?: string;
  phoneNumber?: string;
  mobileNumber?: string;
  emailAddress?: string;

  beneficiaryContactId: number;
  relationship?: string;
  kindId?: number;
  kind?: BeneficiaryKindDto;
  percent: number;
  currentBalance?: number;
}

export interface BeneficiaryResponse {
  beneficiaries: Paged<BeneficiaryDto>;
  beneficiaryOf: Paged<BeneficiaryDto>;
}

export interface BeneficiarySearchFilterRequest extends SortedPaginationRequestDto {
  badgeNumber?: number;
  psn?: number;
  name?: string;
  ssn?: string;
  memberType: string;
}

export interface BeneficiarySearchFilterResponse {
  badgeNumber: number;
  psn: number;
  name?: string;
  ssn?: string;
  street?: string;
  city?: string;
  state?: string;
  zip?: string;
  age?: string;
}

export interface BeneficiaryRequestDto extends SortedPaginationRequestDto {
  badgeNumber?: number;
  psnSuffix?: number;
}

export interface BeneficiaryResponseDto {
  beneficiaryList?: Paged<BeneficiaryDto>;
}

export interface CreateBeneficiaryRequest {
  beneficiaryContactId: number;
  employeeBadgeNumber: number;
  firstLevelBeneficiaryNumber: number | null;
  secondLevelBeneficiaryNumber: number | null;
  thirdLevelBeneficiaryNumber: number | null;
  relationship: string;
  kindId: string;
}

export interface CreateBeneficiaryResponse {
  beneficiaryId: number;
  psnSuffix: number;
  employeeBadgeNumber: number;
  demographicId: number;
  beneficiaryContactId: number;
  relationship: string | null;
  kindId: string | null;
  percent: number;
}

export interface UpdateBeneficiaryContactRequest {
  id?: number;
  contactSsn?: number;
  dateOfBirth?: string;
  street1?: string;
  street2?: string | null;
  street3?: string | null;
  street4?: string | null;
  city?: string;
  state?: string;
  postalCode?: string;
  countryIso?: string | null;
  firstName?: string;
  lastName?: string;
  middleName?: string | null;
  phoneNumber?: string | null;
  mobileNumber?: string | null;
  emailAddress?: string | null;
}

export interface UpdateBeneficiaryRequest extends UpdateBeneficiaryContactRequest {
  relationship?: string;
  kindId?: string;
  percentage?: number;
}

export interface UpdateBeneficiaryResponse {
  badgeNumber: number;
  demographicId: number;
  beneficiaryContactId: number;
  relationship: string | null;
  kindId: string | null;
  percent: number;
}

export interface DeleteBeneficiaryRequest {
  id: number;
}

export interface CreateBeneficiaryContactRequest {
  contactSsn: number;
  dateOfBirth: string;
  street: string;
  street2: string | null;
  street3: string | null;
  street4: string | null;
  city: string;
  state: string;
  postalCode: string;
  countryIso: string | null;
  firstName: string;
  lastName: string;
  middleName: string | null;
  phoneNumber: string | null;
  mobileNumber: string | null;
  emailAddress: string | null;
}

export interface CreateBeneficiaryContactResponse {
  id: number;
  ssn: string;
  dateOfBirth: string;
  street: string;
  street2: string | null;
  street3: string | null;
  street4: string | null;
  city: string;
  state: string;
  postalCode: string;
  countryIso: string | null;
  firstName: string;
  lastName: string;
  middleName: string | null;
  phoneNumber: string | null;
  mobileNumber: string | null;
  emailAddress: string | null;
}

export interface BeneficiaryTypeDto {
  id: number;
  name?: string;
}

export interface BeneficiaryTypesRequestDto {
  id?: number;
}

export interface BeneficiaryTypesResponseDto {
  beneficiaryTypeList?: BeneficiaryTypeDto[];
}

export interface BeneficiaryKindRequestDto {
  id?: number;
}

export interface BeneficiaryKindResponseDto {
  beneficiaryKindList?: BeneficiaryKindDto[];
}

export interface AdhocBeneficiariesReportRequest extends ProfitYearRequest, SortedPaginationRequestDto {
  isAlsoEmployee: boolean;
}

export interface ProfitDetailDto {
  year: number;
  code: string;
  contributions: number;
  earnings: number;
  forfeitures: number;
  date: Date;
  comments?: string;
}

export interface BeneficiaryReportDto {
  beneficiaryId: number;
  fullName: string;
  ssn: string;
  relationship?: string;
  balance?: string;
  badgeNumber: number;
  psnSuffix: number;
  profitDetails?: ProfitDetailDto[];
}

export interface adhocBeneficiariesReportResponse extends PagedReportResponse<BeneficiaryReportDto> {
  totalEndingBalance: number;
}

export interface PayBenReportRequest extends SortedPaginationRequestDto {
  id?: number;
}

export interface PayBenReport {
  ssn: string;
  beneficiaryFullName: string;
  psn: string;
  badge: number;
  demographicFullName: string;
  percentage: number;
}
// eslint-disable-next-line @typescript-eslint/no-empty-object-type
export interface PayBenReportResponse extends Paged<PayBenReport> {
  // PayBenReportResponse extends Paged interface
}
