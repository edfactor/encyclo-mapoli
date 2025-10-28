import { Paged } from "smart-ui-library";

/**
 * Individual PayServices record
 */
export interface PayServicesDto {
  yearsOfService: number;
  yearsOfServiceLabel: string;
  employees: number;
  weeklyPay: number;
  yearsWages: number;
}

/**
 * Response from PayServices endpoints
 */
export interface PayServicesResponse {
  payServicesForYear?: Paged<PayServicesDto>;
  totalEmployeeNumber: number;
  totalEmployeesWages: number;
  totalWeeklyPay?: number;
}
