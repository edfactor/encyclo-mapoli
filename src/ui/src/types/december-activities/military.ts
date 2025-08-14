import type { 
  SortedPaginationRequestDto,
  ProfitYearRequest 
} from "../common/api";

export interface MilitaryContributionRequest extends ProfitYearRequest {
  badgeNumber: number;
  contributionAmount: number;
  contributionDate: string;
  pagination: SortedPaginationRequestDto;
}

export interface CreateMilitaryContributionRequest extends ProfitYearRequest {
  badgeNumber: number;
  contributionAmount: number;
  contributionDate: Date;
  isSupplementalContribution: boolean;
}

export interface MilitaryContribution {
  contributionDate: Date | null;
  contributionAmount: number | null;
  isSupplementalContribution: boolean | false;
}