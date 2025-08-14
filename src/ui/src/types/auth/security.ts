export interface CurrentUserResponseDto {
  userName?: string;
  email?: string;
  storeId?: number;
  isHQUser: boolean;
  claims: string[];
  permissions: string[];
}