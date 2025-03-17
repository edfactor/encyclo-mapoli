import { createSlice, PayloadAction } from "@reduxjs/toolkit";

import {
  BalanceByAge,
  BalanceByYears,
  ContributionsByAge,
  DemographicBadgesNotInPayprofit,
  DistributionsAndForfeitures,
  DistributionsAndForfeituresQueryParams,
  DuplicateNameAndBirthday,
  DuplicateSSNDetail,
  EligibleEmployeeResponseDto,
  EmployeeDetails,
  EmployeesOnMilitaryLeaveResponse,
  EmployeeWagesForYear,
  ExecutiveHoursAndDollars,
  ExecutiveHoursAndDollarsGrid,
  ExecutiveHoursAndDollarsQueryParams,
  ForfeituresAndPoints,
  ForfeituresAndPointsQueryParams,
  ForfeituresByAge,
  FrozenReportsByAgeRequestType,
  MilitaryAndRehireForfeiture,
  MilitaryAndRehireProfitSummary,
  MissingCommasInPYName,
  NegativeEtvaForSSNsOnPayProfit,
  PagedReportResponse,
  ProfitAndReportingQueryParams,
  ProfitShareEditResponse,
  ProfitShareMasterResponse,
  ProfitShareUpdateResponse,
  ProfitSharingDistributionsByAge, ProfitYearRequest,
  TerminationResponse,
  VestedAmountsByAge,
  YearEndProfitSharingReportResponse
} from "reduxstore/types";

export interface YearsEndState {
  additionalExecutivesChosen: ExecutiveHoursAndDollars[] | null;
  additionalExecutivesGrid: PagedReportResponse<ExecutiveHoursAndDollars> | null;
  balanceByAgeFullTime: BalanceByAge | null;
  balanceByAgePartTime: BalanceByAge | null;
  balanceByAgeQueryParams: ProfitYearRequest | null;
  balanceByAgeTotal: BalanceByAge | null;
  balanceByYearsFullTime: BalanceByAge | null;
  balanceByYearsPartTime: BalanceByAge | null;
  balanceByYearsQueryParams: ProfitYearRequest | null;
  balanceByYearsTotal: BalanceByAge | null;
  contributionsByAgeFullTime: ContributionsByAge | null;
  contributionsByAgePartTime: ContributionsByAge | null;
  contributionsByAgeQueryParams: ProfitYearRequest | null;
  contributionsByAgeTotal: ContributionsByAge | null;
  demographicBadges: PagedReportResponse<DemographicBadgesNotInPayprofit> | null;
  distributionsAndForfeitures: PagedReportResponse<DistributionsAndForfeitures> | null;
  distributionsAndForfeituresQueryParams: DistributionsAndForfeituresQueryParams | null;
  distributionsByAgeFullTime: ProfitSharingDistributionsByAge | null;
  distributionsByAgePartTime: ProfitSharingDistributionsByAge | null;
  distributionsByAgeQueryParams: ProfitYearRequest | null;
  distributionsByAgeTotal: ProfitSharingDistributionsByAge | null;
  duplicateNamesAndBirthdays: PagedReportResponse<DuplicateNameAndBirthday> | null;
  duplicateNamesAndBirthdaysQueryParams: ProfitYearRequest | null;
  duplicateSSNsData: PagedReportResponse<DuplicateSSNDetail> | null;
  eligibleEmployees: EligibleEmployeeResponseDto | null;
  eligibleEmployeesQueryParams: ProfitYearRequest | null;
  employeeWagesForYear: PagedReportResponse<EmployeeWagesForYear> | null;
  employeeWagesForYearQueryParams: ProfitYearRequest | null;
  executiveHoursAndDollars: PagedReportResponse<ExecutiveHoursAndDollars> | null;
  executiveHoursAndDollarsGrid: ExecutiveHoursAndDollarsGrid | null;
  executiveHoursAndDollarsQueryParams: ExecutiveHoursAndDollarsQueryParams | null;
  executiveRowsSelected: ExecutiveHoursAndDollars[] | null;
  forfeituresAndPoints: ForfeituresAndPoints | null;
  forfeituresAndPointsQueryParams: ForfeituresAndPointsQueryParams | null;
  forfeituresByAgeFullTime: ForfeituresByAge | null;
  forfeituresByAgePartTime: ForfeituresByAge | null;
  forfeituresByAgeQueryParams: ProfitYearRequest | null;
  forfeituresByAgeTotal: ForfeituresByAge | null;
  militaryAndRehire: PagedReportResponse<EmployeesOnMilitaryLeaveResponse> | null;
  militaryEntryAndModification: EmployeeDetails | null;
  militaryAndRehireForfeitures: PagedReportResponse<MilitaryAndRehireForfeiture> | null;
  militaryAndRehireForfeituresQueryParams: ProfitAndReportingQueryParams | null;
  militaryAndRehireProfitSummary: PagedReportResponse<MilitaryAndRehireProfitSummary> | null;
  militaryAndRehireProfitSummaryQueryParams: ProfitAndReportingQueryParams | null;
  militaryAndRehireQueryParams: ProfitAndReportingQueryParams | null;
  missingCommaInPYName: PagedReportResponse<MissingCommasInPYName> | null;
  negativeEtvaForSSNsOnPayprofit: PagedReportResponse<NegativeEtvaForSSNsOnPayProfit> | null;
  negativeEtvaForSSNsOnPayprofitParams: ProfitYearRequest | null;
  profitSharingUpdate: ProfitShareUpdateResponse | ProfitShareEditResponse | ProfitShareMasterResponse | null;
  termination: TerminationResponse | null;
  terminationQueryParams: ProfitYearRequest | null;
  vestedAmountsByAge: VestedAmountsByAge | null;
  vestedAmountsByAgeQueryParams: ProfitYearRequest | null;
  yearEndProfitSharingReport: PagedReportResponse<YearEndProfitSharingReportResponse> | null;
  yearEndProfitSharingReportQueryParams: ProfitYearRequest | null;
}

const initialState: YearsEndState = {
  additionalExecutivesChosen: null,
  additionalExecutivesGrid: null,
  balanceByAgeFullTime: null,
  balanceByAgePartTime: null,
  balanceByAgeTotal: null,
  balanceByAgeQueryParams: null,
  balanceByYearsFullTime: null,
  balanceByYearsPartTime: null,
  balanceByYearsTotal: null,
  balanceByYearsQueryParams: null,
  contributionsByAgeFullTime: null,
  contributionsByAgePartTime: null,
  contributionsByAgeTotal: null,
  contributionsByAgeQueryParams: null,
  demographicBadges: null,
  distributionsAndForfeitures: null,
  distributionsAndForfeituresQueryParams: null,
  distributionsByAgeFullTime: null,
  distributionsByAgePartTime: null,
  distributionsByAgeTotal: null,
  distributionsByAgeQueryParams: null,
  duplicateSSNsData: null,
  duplicateNamesAndBirthdays: null,
  duplicateNamesAndBirthdaysQueryParams: null,
  eligibleEmployees: null,
  eligibleEmployeesQueryParams: null,
  employeeWagesForYear: null,
  employeeWagesForYearQueryParams: null,
  executiveHoursAndDollars: null,
  executiveHoursAndDollarsGrid: null,
  executiveRowsSelected: null,
  executiveHoursAndDollarsQueryParams: null,
  forfeituresByAgeFullTime: null,
  forfeituresByAgePartTime: null,
  forfeituresByAgeTotal: null,
  forfeituresByAgeQueryParams: null,
  forfeituresAndPoints: null,
  forfeituresAndPointsQueryParams: null,
  militaryAndRehire: null,
  militaryAndRehireQueryParams: null,
  militaryEntryAndModification: null,
  militaryAndRehireForfeitures: null,
  militaryAndRehireProfitSummary: null,
  militaryAndRehireForfeituresQueryParams: null,
  militaryAndRehireProfitSummaryQueryParams: null,
  missingCommaInPYName: null,
  negativeEtvaForSSNsOnPayprofit: null,
  negativeEtvaForSSNsOnPayprofitParams: null,
  profitSharingUpdate: null,
  termination: null,
  terminationQueryParams: null,
  vestedAmountsByAge: null,
  vestedAmountsByAgeQueryParams: null,
  yearEndProfitSharingReport: null,
  yearEndProfitSharingReportQueryParams: null
};

export const yearsEndSlice = createSlice({
  name: "yearsEnd",
  initialState,
  reducers: {
    setBalanceByYearsQueryParams: (state, action: PayloadAction<number>) => {
      state.balanceByYearsQueryParams = { profitYear: action.payload };
    },
    clearBalanceByYearsQueryParams: (state) => {
      state.balanceByYearsQueryParams = null;
    },
    setDuplicateSSNsData: (state, action: PayloadAction<PagedReportResponse<DuplicateSSNDetail>>) => {
      state.duplicateSSNsData = action.payload;
    },
    clearDuplicateSSNsData: (state) => {
      state.duplicateSSNsData = null;
    },
    setDemographicBadgesNotInPayprofitData: (
      state,
      action: PayloadAction<PagedReportResponse<DemographicBadgesNotInPayprofit>>
    ) => {
      state.demographicBadges = action.payload;
    },
    clearDemographicBadgesNotInPayprofitData: (state) => {
      state.demographicBadges = null;
    },
    setDuplicateNamesAndBirthdays: (state, action: PayloadAction<PagedReportResponse<DuplicateNameAndBirthday>>) => {
      state.duplicateNamesAndBirthdays = action.payload;
    },
    clearDuplicateNamesAndBirthdays: (state) => {
      state.duplicateNamesAndBirthdays = null;
    },
    setDuplicateNamesAndBirthdaysQueryParams: (state, action: PayloadAction<number>) => {
      state.duplicateNamesAndBirthdaysQueryParams = { profitYear: action.payload };
    },
    clearDuplicateNamesAndBirthdaysQueryParams: (state) => {
      state.duplicateNamesAndBirthdaysQueryParams = null;
    },
    setForfeituresAndPoints: (state, action: PayloadAction<ForfeituresAndPoints>) => {
      state.forfeituresAndPoints = action.payload;
    },
    clearForfeituresAndPoints: (state) => {
      state.forfeituresAndPoints = null;
    },
    setNegativeEtvaForSSNsOnPayprofitQueryParams: (state, action: PayloadAction<number>) => {
      state.negativeEtvaForSSNsOnPayprofitParams = { profitYear: action.payload };
    },
    clearNegativeEtvaForSSNsOnPayprofitQueryParams: (state) => {
      state.negativeEtvaForSSNsOnPayprofitParams = null;
    },
    setNegativeEtvaForSSNsOnPayprofit: (
      state,
      action: PayloadAction<PagedReportResponse<NegativeEtvaForSSNsOnPayProfit>>
    ) => {
      state.negativeEtvaForSSNsOnPayprofit = action.payload;
    },
    clearNegativeEtvaForSSNsOnPayprofit: (state) => {
      state.negativeEtvaForSSNsOnPayprofit = null;
    },
    setMissingCommaInPYName: (state, action: PayloadAction<PagedReportResponse<MissingCommasInPYName>>) => {
      state.missingCommaInPYName = action.payload;
    },
    clearMissingCommaInPYName: (state) => {
      state.missingCommaInPYName = null;
    },
    setForfeituresAndPointsQueryParams: (state, action: PayloadAction<ForfeituresAndPointsQueryParams>) => {
      state.forfeituresAndPointsQueryParams = action.payload;
    },
    clearForfeituresAndPointsQueryParams: (state) => {
      state.forfeituresAndPointsQueryParams = null;
    },
    setMilitaryAndRehireProfitSummaryQueryParams: (state, action: PayloadAction<ProfitAndReportingQueryParams>) => {
      state.militaryAndRehireProfitSummaryQueryParams = action.payload;
    },
    clearMilitaryAndRehireProfitSummaryQueryParams: (state) => {
      state.militaryAndRehireProfitSummaryQueryParams = null;
    },
    setEmployeesOnMilitaryLeaveDetails: (
      state,
      action: PayloadAction<PagedReportResponse<EmployeesOnMilitaryLeaveResponse>>
    ) => {
      state.militaryAndRehire = action.payload;
    },
    clearEmployeesOnMilitaryLeaveDetails: (state) => {
      state.militaryAndRehire = null;
    },
    setEmployeesOnMilitaryLeaveDetailsQueryParams: (state, action: PayloadAction<number>) => {
      if (state.militaryAndRehireQueryParams) {
        state.militaryAndRehireQueryParams.profitYear = action.payload;
      }
    },
    clearEmployeesOnMilitaryLeaveDetailsQueryParams: (state) => {
      state.militaryAndRehireQueryParams = null;
    },
    setMilitaryAndRehireForfeituresDetails: (
      state,
      action: PayloadAction<PagedReportResponse<MilitaryAndRehireForfeiture>>
    ) => {
      state.militaryAndRehireForfeitures = action.payload;
    },
    clearMilitaryAndRehireForfeituresDetails: (state) => {
      state.militaryAndRehireForfeitures = null;
    },
    setMilitaryAndRehireForfeituresQueryParams: (state, action: PayloadAction<ProfitAndReportingQueryParams>) => {
      state.militaryAndRehireForfeituresQueryParams = action.payload;
    },
    clearMilitaryAndRehireForfeituresQueryParams: (state) => {
      state.militaryAndRehireForfeituresQueryParams = null;
    },
    setMilitaryAndRehireProfitSummaryDetails: (
      state,
      action: PayloadAction<PagedReportResponse<MilitaryAndRehireProfitSummary>>
    ) => {
      state.militaryAndRehireProfitSummary = action.payload;
    },
    clearMilitaryAndRehireProfitSummaryDetails: (state) => {
      state.militaryAndRehireProfitSummary = null;
    },
    setDistributionsAndForfeitures: (
      state,
      action: PayloadAction<PagedReportResponse<DistributionsAndForfeitures>>
    ) => {
      state.distributionsAndForfeitures = action.payload;
    },
    clearDistributionsAndForfeitures: (state) => {
      state.distributionsAndForfeitures = null;
    },
    setDistributionsAndForfeituresQueryParams: (
      state,
      action: PayloadAction<DistributionsAndForfeituresQueryParams>
    ) => {
      state.distributionsAndForfeituresQueryParams = action.payload;
    },
    clearDistributionsAndForfeituresQueryParams: (state) => {
      state.distributionsAndForfeituresQueryParams = null;
    },
    setExecutiveHoursAndDollars: (state, action: PayloadAction<PagedReportResponse<ExecutiveHoursAndDollars>>) => {
      state.executiveHoursAndDollars = action.payload;
    },
    clearExecutiveHoursAndDollars: (state) => {
      state.executiveHoursAndDollars = null;
    },
    setAdditionalExecutivesGrid: (state, action: PayloadAction<PagedReportResponse<ExecutiveHoursAndDollars>>) => {
      state.additionalExecutivesGrid = action.payload;
    },
    setAdditionalExecutivesChosen: (state, action: PayloadAction<ExecutiveHoursAndDollars[]>) => {
      if (state.additionalExecutivesChosen === null) {
        state.additionalExecutivesChosen = action.payload;
      } else {
        state.additionalExecutivesChosen.push(...action.payload);
      }
    },
    setExecutiveRowsSelected: (state, action: PayloadAction<ExecutiveHoursAndDollars[]>) => {
      state.executiveRowsSelected = action.payload;
    },
    clearExecutiveRowsSelected: (state) => {
      state.executiveRowsSelected = null;
    },
    clearAdditionalExecutivesGrid: (state) => {
      state.additionalExecutivesGrid = null;
    },
    clearAdditionalExecutivesChosen: (state) => {
      state.additionalExecutivesChosen = null;
    },
    setExecutiveHoursAndDollarsGrid: (state, action: PayloadAction<ExecutiveHoursAndDollarsGrid>) => {
      state.executiveHoursAndDollarsGrid = action.payload;
    },
    clearExecutiveHoursAndDollarsGridRows: (state) => {
      state.executiveHoursAndDollarsGrid = null;
    },
    setExecutiveHoursAndDollarsGridYear: (state, action: PayloadAction<number>) => {
      if (state.executiveHoursAndDollarsGrid) {
        state.executiveHoursAndDollarsGrid.profitYear = action.payload;
      } else {
        state.executiveHoursAndDollarsGrid = { profitYear: action.payload, executiveHoursAndDollars: [] };
      }
    },
    /*
      This is putting a new changed row into our structure to be saved later after the button is pressed.
    */
    addExecutiveHoursAndDollarsGridRow: (state, action: PayloadAction<ExecutiveHoursAndDollarsGrid>) => {
      // So first, do we have a profit year already set for this?
      if (
        state.executiveHoursAndDollarsGrid &&
        state.executiveHoursAndDollarsGrid?.profitYear === action.payload.profitYear
      ) {
        // We have a structure and year alrady, so now we need to just add our row
        state.executiveHoursAndDollarsGrid.executiveHoursAndDollars.push(action.payload.executiveHoursAndDollars[0]);
      } else {
        // So we don't have a year for some reason, or any other data. Let us just add our whole structure
        state.executiveHoursAndDollarsGrid = action.payload;
      }
    },
    /* 
      We get here when the profit year is decided and another pending change for this row
      is already there. This will occur when the user edits a field more than once, or edits both 
      values in one row
    */
    updateExecutiveHoursAndDollarsGridRow: (state, action: PayloadAction<ExecutiveHoursAndDollarsGrid>) => {
      if (state.executiveHoursAndDollarsGrid) {
        // just need to find the correct row with the correct badge number and update both fields
        for (const hoursDollarsRow of state.executiveHoursAndDollarsGrid.executiveHoursAndDollars) {
          if (hoursDollarsRow.badgeNumber === action.payload.executiveHoursAndDollars[0].badgeNumber) {
            hoursDollarsRow.executiveDollars = action.payload.executiveHoursAndDollars[0].executiveDollars;
            hoursDollarsRow.executiveHours = action.payload.executiveHoursAndDollars[0].executiveHours;
            break;
          }
        }
      }
    },
    /*
      We remove a row from pending changes when the user has changed a pending row change back to 
      the original state of that unsaved row, which invalidates the need for an update
    */
    removeExecutiveHoursAndDollarsGridRow: (state, action: PayloadAction<ExecutiveHoursAndDollarsGrid>) => {
      // So first, do we have a profit year for this?
      if (
        state.executiveHoursAndDollarsGrid &&
        state.executiveHoursAndDollarsGrid?.profitYear === action.payload.profitYear
      ) {
        // So now we need to just remove one that has our badge number
        const newRows = state.executiveHoursAndDollarsGrid?.executiveHoursAndDollars.filter(function (pendingRow) {
          return pendingRow.badgeNumber !== action.payload.executiveHoursAndDollars[0].badgeNumber;
        });
        state.executiveHoursAndDollarsGrid.executiveHoursAndDollars = newRows;
      } else {
        // So if we do not have the year, or the grid is not there, we have nothing to do
        console.log(
          "WARN: Tried to remove a non-existent exec dollars and hours row with badge: " +
            action.payload.executiveHoursAndDollars[0].badgeNumber +
            " and profit year: " +
            action.payload.profitYear
        );
      }
    },
    setExecutiveHoursAndDollarsQueryParams: (state, action: PayloadAction<ExecutiveHoursAndDollarsQueryParams>) => {
      state.executiveHoursAndDollarsQueryParams = action.payload;
    },
    clearExecutiveHoursAndDollarsQueryParams: (state) => {
      state.executiveHoursAndDollarsQueryParams = null;
    },
    setEmployeeWagesForYear: (state, action: PayloadAction<PagedReportResponse<EmployeeWagesForYear>>) => {
      state.employeeWagesForYear = action.payload;
    },
    setEmployeeWagesForYearQueryParams: (state, action: PayloadAction<number>) => {
      state.employeeWagesForYearQueryParams = { profitYear: action.payload };
    },
    clearEmployeeWagesForYearQueryParams: (state) => {
      state.employeeWagesForYearQueryParams = null;
    },
    setEligibleEmployees: (state, action: PayloadAction<EligibleEmployeeResponseDto>) => {
      state.eligibleEmployees = action.payload;
    },
    setEligibleEmployeesQueryParams: (state, action: PayloadAction<number>) => {
      state.eligibleEmployeesQueryParams = { profitYear: action.payload };
    },
    clearEligibleEmployeesQueryParams: (state) => {
      state.eligibleEmployeesQueryParams = null;
    },
    clearEligibleEmployees: (state) => {
      state.eligibleEmployees = null;
    },    
    setDistributionsByAge: (state, action: PayloadAction<ProfitSharingDistributionsByAge>) => {
      if (action.payload.reportType == FrozenReportsByAgeRequestType.Total) {
        state.distributionsByAgeTotal = action.payload;
      }

      if (action.payload.reportType == FrozenReportsByAgeRequestType.FullTime) {
        state.distributionsByAgeFullTime = action.payload;
      }

      if (action.payload.reportType == FrozenReportsByAgeRequestType.PartTime) {
        state.distributionsByAgePartTime = action.payload;
      }
    },
    clearDistributionsByAge: (state) => {
      state.distributionsByAgeTotal = null;

      state.distributionsByAgeFullTime = null;

      state.distributionsByAgePartTime = null;
    },
    clearDistributionsByAgeQueryParams: (state) => {
      state.distributionsByAgeQueryParams = null;
    },
    setDistributionsByAgeQueryParams: (state, action: PayloadAction<number>) => {
      state.distributionsByAgeQueryParams = { profitYear: action.payload };
    },
    setContributionsByAge: (state, action: PayloadAction<ContributionsByAge>) => {
      if (action.payload.reportType == FrozenReportsByAgeRequestType.Total) {
        state.contributionsByAgeTotal = action.payload;
      }

      if (action.payload.reportType == FrozenReportsByAgeRequestType.FullTime) {
        state.contributionsByAgeFullTime = action.payload;
      }

      if (action.payload.reportType == FrozenReportsByAgeRequestType.PartTime) {
        state.contributionsByAgePartTime = action.payload;
      }
    },
    clearContributionsByAge: (state) => {
      state.contributionsByAgeTotal = null;

      state.contributionsByAgeFullTime = null;

      state.contributionsByAgePartTime = null;
    },
    setContributionsByAgeQueryParams: (state, action: PayloadAction<number>) => {
      state.contributionsByAgeQueryParams = { profitYear: action.payload };
    },
    clearContributionsByAgeQueryParams: (state) => {
      state.contributionsByAgeQueryParams = null;
    },
    setForfeituresByAgeQueryParams: (state, action: PayloadAction<number>) => {
      state.forfeituresByAgeQueryParams = { profitYear: action.payload };
    },
    clearForfeituresByAgeQueryParams: (state) => {
      state.forfeituresByAgeQueryParams = null;
    },
    setForfeituresByAge: (state, action: PayloadAction<ForfeituresByAge>) => {
      if (action.payload.reportType == FrozenReportsByAgeRequestType.Total) {
        state.forfeituresByAgeTotal = action.payload;
      }

      if (action.payload.reportType == FrozenReportsByAgeRequestType.FullTime) {
        state.forfeituresByAgeFullTime = action.payload;
      }

      if (action.payload.reportType == FrozenReportsByAgeRequestType.PartTime) {
        state.forfeituresByAgePartTime = action.payload;
      }
    },
    clearForfeituresByAge: (state) => {
      state.forfeituresByAgeTotal = null;

      state.forfeituresByAgeFullTime = null;

      state.forfeituresByAgePartTime = null;
    },
    setBalanceByAge: (state, action: PayloadAction<BalanceByAge>) => {
      if (action.payload.reportType == FrozenReportsByAgeRequestType.Total) {
        state.balanceByAgeTotal = action.payload;
      }

      if (action.payload.reportType == FrozenReportsByAgeRequestType.FullTime) {
        state.balanceByAgeFullTime = action.payload;
      }

      if (action.payload.reportType == FrozenReportsByAgeRequestType.PartTime) {
        state.balanceByAgePartTime = action.payload;
      }
    },
    clearBalanceByAge: (state) => {
      state.balanceByAgeTotal = null;

      state.balanceByAgeFullTime = null;

      state.balanceByAgePartTime = null;
    },
    setBalanceByAgeQueryParams: (state, action: PayloadAction<number>) => {
      state.balanceByAgeQueryParams = { profitYear: action.payload };
    },
    clearBalanceByAgeQueryParams: (state) => {
      state.balanceByAgeQueryParams = null;
    },
    clearBalanceByYears: (state) => {
      state.balanceByYearsTotal = null;

      state.balanceByYearsFullTime = null;

      state.balanceByYearsFullTime = null;
    },
    setBalanceByYears: (state, action: PayloadAction<BalanceByYears>) => {
      if (action.payload.reportType == FrozenReportsByAgeRequestType.Total) {
        state.balanceByYearsTotal = action.payload;
      }

      if (action.payload.reportType == FrozenReportsByAgeRequestType.FullTime) {
        state.balanceByYearsFullTime = action.payload;
      }

      if (action.payload.reportType == FrozenReportsByAgeRequestType.PartTime) {
        state.balanceByYearsFullTime = action.payload;
      }
    },
    setVestedAmountByAge: (state, action: PayloadAction<VestedAmountsByAge>) => {
      state.vestedAmountsByAge = action.payload;
    },
    setVestedAmountsByAgeQueryParams: (state, action: PayloadAction<number>) => {
      state.vestedAmountsByAgeQueryParams = { profitYear: action.payload };
    },
    clearVestedAmountsByAgeQueryParams: (state) => {
      state.vestedAmountsByAgeQueryParams = null;
    },
    setTermination: (state, action: PayloadAction<TerminationResponse>) => {
      state.termination = action.payload;
    },
    clearTermination: (state) => {
      state.termination = null;
    },
    setTerminationQueryParams: (state, action: PayloadAction<number>) => {
      state.terminationQueryParams = { profitYear: action.payload };
    },
    clearTerminationQueryParams: (state) => {
      state.terminationQueryParams = null;
    },
    setProfitUpdate: (state, action: PayloadAction<ProfitShareUpdateResponse>) => {
      state.profitSharingUpdate = action.payload;
    },
    setProfitUpdateLoading: (state) => {
      state.profitSharingUpdate = { isLoading: true, reportName: "Profit Sharing Update" };
    },
    clearProfitUpdate: (state) => {
      state.profitSharingUpdate = null;
    },
    setProfitEdit: (state, action: PayloadAction<ProfitShareEditResponse>) => {
      state.profitSharingUpdate = action.payload;
    },
    setProfitEditLoading: (state) => {
      state.profitSharingUpdate = { isLoading: true, reportName: "Profit Sharing Edit" };
    },
    clearProfitEdit: (state) => {
      state.profitSharingUpdate = null;
    },

    setProfitMasterApply: (state, action: PayloadAction<ProfitShareMasterResponse>) => {
      state.profitSharingUpdate = {
        ...action.payload,
        reportName: "Apply"
      };
    },
    setProfitMasterApplyLoading: (state) => {
      state.profitSharingUpdate = { isLoading: true, reportName: "Apply" };
    },

    setProfitMasterRevert: (state, action: PayloadAction<ProfitShareMasterResponse>) => {
      state.profitSharingUpdate = {
        ...action.payload,
        reportName: "Revert"
      };
    },
    setProfitMasterRevertLoading: (state) => {
      state.profitSharingUpdate = { isLoading: true, reportName: "Revert" };
    },
    setYearEndProfitSharingReport: (
      state,
      action: PayloadAction<PagedReportResponse<YearEndProfitSharingReportResponse>>
    ) => {
      state.yearEndProfitSharingReport = action.payload;
    },
    clearYearEndProfitSharingReport: (state) => {
      state.yearEndProfitSharingReport = null;
    },
    setYearEndProfitSharingReportQueryParams: (state, action: PayloadAction<number>) => {
      state.yearEndProfitSharingReportQueryParams = { profitYear: action.payload };
    },
    clearYearEndProfitSharingReportQueryParams: (state) => {
      state.yearEndProfitSharingReportQueryParams = null;
    }
  }
});

export const {
  addExecutiveHoursAndDollarsGridRow,
  clearAdditionalExecutivesChosen,
  clearAdditionalExecutivesGrid,
  clearBalanceByAge,
  clearBalanceByAgeQueryParams,
  clearBalanceByYears,
  clearBalanceByYearsQueryParams,
  clearContributionsByAge,
  clearContributionsByAgeQueryParams,
  clearDistributionsAndForfeitures,
  clearDistributionsAndForfeituresQueryParams,
  clearDistributionsByAge,
  clearDistributionsByAgeQueryParams,
  clearDuplicateNamesAndBirthdays,
  clearDuplicateNamesAndBirthdaysQueryParams,
  clearEligibleEmployees,
  clearEligibleEmployeesQueryParams,
  clearEmployeesOnMilitaryLeaveDetails,
  clearExecutiveHoursAndDollars,
  clearExecutiveHoursAndDollarsGridRows,
  clearExecutiveRowsSelected,
  clearForfeituresAndPoints,
  clearForfeituresAndPointsQueryParams,
  clearForfeituresByAge,
  clearForfeituresByAgeQueryParams,
  clearMilitaryAndRehireForfeituresDetails,
  clearMilitaryAndRehireForfeituresQueryParams,
  clearMilitaryAndRehireProfitSummaryDetails,
  clearMilitaryAndRehireProfitSummaryQueryParams,
  clearMissingCommaInPYName,
  clearNegativeEtvaForSSNsOnPayprofit,
  clearNegativeEtvaForSSNsOnPayprofitQueryParams,
  clearProfitEdit,
  clearProfitUpdate,
  clearTermination,
  clearTerminationQueryParams,
  clearVestedAmountsByAgeQueryParams,
  clearYearEndProfitSharingReport,
  clearYearEndProfitSharingReportQueryParams,
  removeExecutiveHoursAndDollarsGridRow,
  setAdditionalExecutivesChosen,
  setAdditionalExecutivesGrid,
  setBalanceByAge,
  setBalanceByAgeQueryParams,
  setBalanceByYears,
  setBalanceByYearsQueryParams,
  setContributionsByAge,
  setContributionsByAgeQueryParams,
  setDemographicBadgesNotInPayprofitData,
  setDistributionsAndForfeitures,
  setDistributionsAndForfeituresQueryParams,
  setDistributionsByAge,
  setDistributionsByAgeQueryParams,
  setDuplicateNamesAndBirthdays,
  setDuplicateNamesAndBirthdaysQueryParams,
  setDuplicateSSNsData,
  setEligibleEmployees,
  setEligibleEmployeesQueryParams,
  setEmployeesOnMilitaryLeaveDetails,
  setEmployeeWagesForYear,
  setEmployeeWagesForYearQueryParams,
  setExecutiveHoursAndDollars,
  setExecutiveHoursAndDollarsGridYear,
  setExecutiveHoursAndDollarsQueryParams,
  setExecutiveRowsSelected,
  setForfeituresAndPoints,
  setForfeituresAndPointsQueryParams,
  setForfeituresByAge,
  setForfeituresByAgeQueryParams,
  setMilitaryAndRehireForfeituresDetails,
  setMilitaryAndRehireForfeituresQueryParams,
  setMilitaryAndRehireProfitSummaryDetails,
  setMilitaryAndRehireProfitSummaryQueryParams,
  setMissingCommaInPYName,
  setNegativeEtvaForSSNsOnPayprofit,
  setNegativeEtvaForSSNsOnPayprofitQueryParams,
  setProfitEdit,
  setProfitEditLoading,
  setProfitMasterApply,
  setProfitMasterApplyLoading,
  setProfitMasterRevert,
  setProfitMasterRevertLoading,
  setProfitUpdate,
  setProfitUpdateLoading,
  setTermination,
  setTerminationQueryParams,
  setVestedAmountByAge,
  setVestedAmountsByAgeQueryParams,
  setYearEndProfitSharingReport,
  setYearEndProfitSharingReportQueryParams,
  updateExecutiveHoursAndDollarsGridRow
} = yearsEndSlice.actions;
export default yearsEndSlice.reducer;
