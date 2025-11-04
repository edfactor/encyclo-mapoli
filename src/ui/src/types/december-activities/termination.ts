import type { PagedReportResponse } from "../common/api";

export interface TerminationDetail {
  psn: number;
  name: string | null;
  yearDetails: TerminationYearDetail[];
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
  age: number | null;
  enrollmentCode: number | null;
}

export interface TerminationResponse extends PagedReportResponse<TerminationDetail> {
  totalVested: number;
  totalForfeit: number;
  totalEndingBalance: number;
  totalBeneficiaryAllocation: number;
}
