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
  contactInfo?: ContactInfoDto;

  street?: string;
  city?: string;
  state?: string;
  postalCode?: string;
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

export interface BeneficiariesGetAPIResponse {
  beneficiaries: Paged<BeneficiaryDto>;
  beneficiaryOf: Paged<BeneficiaryDto>;
}

export interface BeneficiarySearchForm {
  badgeNumber?: number;
  psn?: number;
  name?: string;
  ssn?: string;
  memberType: 0 | 1 | 2; // 0=all, 1=employees, 2=beneficiaries
}

export interface BeneficiaryDetailAPIRequest extends SortedPaginationRequestDto {
  badgeNumber: number;
  psnSuffix: number;
}

export interface BeneficiaryDetail {
  badgeNumber: number;
  psnSuffix: number;
  fullName?: string | null;
  ssn?: string | null;
  street?: string | null;
  city?: string | null;
  state?: string | null;
  zip?: string | null;
  age?: number | null;
}

export interface BeneficiarySearchAPIRequest extends SortedPaginationRequestDto {
  badgeNumber?: number;
  psnSuffix?: number;
  name?: string;
  ssn?: string;
  memberType: number;
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
