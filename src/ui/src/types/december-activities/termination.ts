import type { PagedReportResponse } from "../common/api";

export interface TerminationDetail {
  psn: number;
  name: string | null;
  yearDetails: TerminationYearDetail[];
  [key: string]: unknown;
}

export interface TerminationYearDetail {
  profitYear: number;
  beginningBalance: number;
  beneficiaryAllocation: number;
  distributionAmount: number;
  forfeit: number;
  endingBalance: number;
  vestedBalance: number;
  dateTerm: string | null;
  ytdPsHours: number;
  vestedPercent: number;
  suggestedForfeiture: number | null;
  age: string | null;
  enrollmentCode: number | null;
  [key: string]: unknown;
}

export interface TerminationResponse extends PagedReportResponse<TerminationDetail> {
  totalVested: number;
  totalForfeit: number;
  totalEndingBalance: number;
  totalBeneficiaryAllocation: number;
}
