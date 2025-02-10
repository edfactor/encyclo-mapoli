import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import {
  ContributionsByAge,
  ForfeituresByAge,
  BalanceByAge,
  BalanceByYears,
  DemographicBadgesNotInPayprofit,
  DistributionsAndForfeitures,
  DuplicateNameAndBirthday,
  DuplicateSSNDetail,
  EligibleEmployeeResponseDto,
  ExecutiveHoursAndDollars,
  FrozenReportsByAgeRequestType,
  MasterInquiryDetail,
  EmployeesOnMilitaryLeaveResponse,
  MilitaryAndRehireForfeiture,
  MilitaryAndRehireProfitSummary,
  MissingCommasInPYName,
  NegativeEtvaForSSNsOnPayProfit,
  PagedReportResponse,
  ProfitSharingDistributionsByAge,
  EmployeeDetails,
  MasterInquiryResponseType,
  VestedAmountsByAge,
  TerminationResponse,
  ProfitShareUpdateResponse,
  ProfitShareEditResponse,
  ProfitShareMasterResponse,
  ExecutiveHoursAndDollarsGrid
} from "reduxstore/types";
import { Paged } from "smart-ui-library";

export interface YearsEndState {
  duplicateSSNsData: PagedReportResponse<DuplicateSSNDetail> | null;
  demographicBadges: PagedReportResponse<DemographicBadgesNotInPayprofit> | null;
  duplicateNamesAndBirthday: PagedReportResponse<DuplicateNameAndBirthday> | null;
  negativeEtvaForSSNsOnPayprofit: PagedReportResponse<NegativeEtvaForSSNsOnPayProfit> | null;
  missingCommaInPYName: PagedReportResponse<MissingCommasInPYName> | null;
  militaryAndRehire: PagedReportResponse<EmployeesOnMilitaryLeaveResponse> | null;
  militaryAndRehireForfeitures: PagedReportResponse<MilitaryAndRehireForfeiture> | null;
  militaryAndRehireProfitSummary: PagedReportResponse<MilitaryAndRehireProfitSummary> | null;
  distributionsAndForfeitures: PagedReportResponse<DistributionsAndForfeitures> | null;
  executiveHoursAndDollars: PagedReportResponse<ExecutiveHoursAndDollars> | null;
  executiveHoursAndDollarsGrid: ExecutiveHoursAndDollarsGrid | null;
  eligibleEmployees: EligibleEmployeeResponseDto | null;
  masterInquiryData: Paged<MasterInquiryDetail> | null;
  masterInquiryEmployeeDetails: EmployeeDetails | null;
  distributionsByAgeTotal: ProfitSharingDistributionsByAge | null;
  distributionsByAgeFullTime: ProfitSharingDistributionsByAge | null;
  distributionsByAgePartTime: ProfitSharingDistributionsByAge | null;
  contributionsByAgeTotal: ContributionsByAge | null;
  contributionsByAgeFullTime: ContributionsByAge | null;
  contributionsByAgePartTime: ContributionsByAge | null;
  forfeituresByAgeTotal: ForfeituresByAge | null;
  forfeituresByAgeFullTime: ForfeituresByAge | null;
  forfeituresByAgePartTime: ForfeituresByAge | null;
  balanceByAgeTotal: BalanceByAge | null;
  balanceByAgeFullTime: BalanceByAge | null;
  balanceByAgePartTime: BalanceByAge | null;
  balanceByYearsTotal: BalanceByAge | null;
  balanceByYearsFullTime: BalanceByAge | null;
  balanceByYearsPartTime: BalanceByAge | null;
  vestedAmountsByAge: VestedAmountsByAge | null;
  terminattion: TerminationResponse | null;
  militaryAndRehireEntryAndModification: EmployeeDetails | null;
  profitSharingUpdate: ProfitShareUpdateResponse | ProfitShareEditResponse | ProfitShareMasterResponse | null;
}

const initialState: YearsEndState = {
  duplicateSSNsData: null,
  demographicBadges: null,
  duplicateNamesAndBirthday: null,
  negativeEtvaForSSNsOnPayprofit: null,
  missingCommaInPYName: null,
  militaryAndRehire: null,
  militaryAndRehireForfeitures: null,
  militaryAndRehireProfitSummary: null,
  distributionsAndForfeitures: null,
  executiveHoursAndDollars: null,
  executiveHoursAndDollarsGrid: null,
  eligibleEmployees: null,
  masterInquiryData: null,
  masterInquiryEmployeeDetails: null,
  distributionsByAgeTotal: null,
  distributionsByAgeFullTime: null,
  distributionsByAgePartTime: null,
  contributionsByAgeTotal: null,
  contributionsByAgeFullTime: null,
  contributionsByAgePartTime: null,
  forfeituresByAgeTotal: null,
  forfeituresByAgeFullTime: null,
  forfeituresByAgePartTime: null,
  balanceByAgeTotal: null,
  balanceByAgeFullTime: null,
  balanceByAgePartTime: null,
  balanceByYearsTotal: null,
  balanceByYearsFullTime: null,
  balanceByYearsPartTime: null,
  vestedAmountsByAge: null,
  terminattion: null,
  profitSharingUpdate: null,
  militaryAndRehireEntryAndModification: null
};

export const yearsEndSlice = createSlice({
  name: "yearsEnd",
  initialState,
  reducers: {
    setDuplicateSSNsData: (state, action: PayloadAction<PagedReportResponse<DuplicateSSNDetail>>) => {
      state.duplicateSSNsData = action.payload;
    },
    setDemographicBadgesNotInPayprofitData: (
      state,
      action: PayloadAction<PagedReportResponse<DemographicBadgesNotInPayprofit>>
    ) => {
      state.demographicBadges = action.payload;
    },
    setDuplicateNamesAndBirthdays: (state, action: PayloadAction<PagedReportResponse<DuplicateNameAndBirthday>>) => {
      state.duplicateNamesAndBirthday = action.payload;
    },
    setNegativeEtvaForSssnsOnPayprofit: (
      state,
      action: PayloadAction<PagedReportResponse<NegativeEtvaForSSNsOnPayProfit>>
    ) => {
      state.negativeEtvaForSSNsOnPayprofit = action.payload;
    },
    setMissingCommaInPYName: (state, action: PayloadAction<PagedReportResponse<MissingCommasInPYName>>) => {
      state.missingCommaInPYName = action.payload;
    },
    setEmployeesOnMilitaryLeaveDetails: (
      state,
      action: PayloadAction<PagedReportResponse<EmployeesOnMilitaryLeaveResponse>>
    ) => {
      state.militaryAndRehire = action.payload;
    },
    setMilitaryAndRehireForfeituresDetails: (
      state,
      action: PayloadAction<PagedReportResponse<MilitaryAndRehireForfeiture>>
    ) => {
      state.militaryAndRehireForfeitures = action.payload;
    },
    setMilitaryAndRehireProfitSummaryDetails: (
      state,
      action: PayloadAction<PagedReportResponse<MilitaryAndRehireProfitSummary>>
    ) => {
      state.militaryAndRehireProfitSummary = action.payload;
    },
    setDistributionsAndForfeitures: (
      state,
      action: PayloadAction<PagedReportResponse<DistributionsAndForfeitures>>
    ) => {
      state.distributionsAndForfeitures = action.payload;
    },
    setExecutiveHoursAndDollars: (state, action: PayloadAction<PagedReportResponse<ExecutiveHoursAndDollars>>) => {
      state.executiveHoursAndDollars = action.payload;
    },
    setExecutiveHoursAndDollarsGrid: (state, action: PayloadAction<ExecutiveHoursAndDollarsGrid>) => {
      state.executiveHoursAndDollarsGrid = action.payload;
    },
    // We would call this when a save is successful and pending changes should be cleared
    clearExecutiveHoursAndDollarsGridRows: (state) => {
      state.executiveHoursAndDollarsGrid = null;
    },
    // We would call this when a search is done and
    // a profit year has been chosen.
    setExecutiveHoursAndDollarsGridYear: (state, action: PayloadAction<number>) => {
      if (state.executiveHoursAndDollarsGrid) {
        state.executiveHoursAndDollarsGrid.profitYear = action.payload;
      } else {
        state.executiveHoursAndDollarsGrid = { profitYear: action.payload, executiveHoursAndDollars: [] };
      }
    },
    // This is putting a new changed row into our structure to be saved later.
    addExecutiveHoursAndDollarsGridRow: (state, action: PayloadAction<ExecutiveHoursAndDollarsGrid>) => {
      // So first, do we have a profit year for this?
      if (
        state.executiveHoursAndDollarsGrid &&
        state.executiveHoursAndDollarsGrid?.profitYear === action.payload.profitYear
      ) {
        // So now we need to just add one
        state.executiveHoursAndDollarsGrid.executiveHoursAndDollars.push(action.payload.executiveHoursAndDollars[0]);
      } else {
        // So we need to assign our new year and entry
        state.executiveHoursAndDollarsGrid = action.payload;
      }
    },
    updateExecutiveHoursAndDollarsGridRow: (state, action: PayloadAction<ExecutiveHoursAndDollarsGrid>) => {
      // We get here when the profit year is decided and a row is already there. This will occur
      // when the user edits a field more than once, or edits both values in a row

      if (state.executiveHoursAndDollarsGrid) {
        // just need to find the correct row with the correct badge number
        for (const hoursDollarsRow of state.executiveHoursAndDollarsGrid.executiveHoursAndDollars) {
          if (hoursDollarsRow.badgeNumber === action.payload.executiveHoursAndDollars[0].badgeNumber) {
            hoursDollarsRow.executiveDollars = action.payload.executiveHoursAndDollars[0].executiveDollars;
            hoursDollarsRow.executiveHours = action.payload.executiveHoursAndDollars[0].executiveHours;
            break;
          }
        }
      }
    },
    // We remove from pending changes when the user has changed a pending row change back to original state,
    // invalidating the need for an update
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
    setEligibleEmployees: (state, action: PayloadAction<EligibleEmployeeResponseDto>) => {
      state.eligibleEmployees = action.payload;
    },
    setMasterInquiryData: (state, action: PayloadAction<MasterInquiryResponseType>) => {
      state.masterInquiryData = action.payload.inquiryResults;

      if (action.payload.employeeDetails) {
        state.masterInquiryEmployeeDetails = action.payload.employeeDetails;
      }
    },
    clearMasterInquiryData: (state) => {
      state.masterInquiryData = null;
      state.masterInquiryEmployeeDetails = null;
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
    setBalanceByYears: (state, action: PayloadAction<BalanceByYears>) => {
      if (action.payload.reportType == FrozenReportsByAgeRequestType.Total) {
        state.balanceByYearsTotal = action.payload;
      }

      if (action.payload.reportType == FrozenReportsByAgeRequestType.FullTime) {
        state.balanceByYearsFullTime = action.payload;
      }

      if (action.payload.reportType == FrozenReportsByAgeRequestType.PartTime) {
        state.balanceByYearsPartTime = action.payload;
      }
    },
    setVestingAmountByAge: (state, action: PayloadAction<VestedAmountsByAge>) => {
      state.vestedAmountsByAge = action.payload;
    },
    setTermination: (state, action: PayloadAction<TerminationResponse>) => {
      state.terminattion = action.payload;
    },
    setProfitUpdate: (state, action: PayloadAction<ProfitShareUpdateResponse>) => {
      state.profitSharingUpdate = action.payload;
    },
    setProfitUpdateLoading: (state) => {
      // @ts-ignore
      state.profitSharingUpdate = { isLoading: true, reportName: "Profit Sharing Update" };
    },
    clearProfitUpdate: (state) => {
      // @ts-ignore
      state.profitSharingUpdate = null;
    },
    setProfitEdit: (state, action: PayloadAction<ProfitShareEditResponse>) => {
      state.profitSharingUpdate = action.payload;
    },
    setProfitEditLoading: (state) => {
      // @ts-ignore
      state.profitSharingUpdate = { isLoading: true, reportName: "Profit Sharing Edit" };
    },
    clearProfitEdit: (state) => {
      // @ts-ignore
      state.profitSharingUpdate = null;
    },

    setProfitMasterApply: (state, action: PayloadAction<ProfitShareMasterResponse>) => {
      state.profitSharingUpdate = {
        ...action.payload,
        reportName: "Apply"
      };
    },
    setProfitMasterApplyLoading: (state) => {
      // @ts-ignore
      state.profitSharingUpdate = { isLoading: true, reportName: "Apply" };
    },
    clearProfitMasterApply: (state) => {
      // @ts-ignore
      state.profitSharingUpdate = null;
    },

    setProfitMasterRevert: (state, action: PayloadAction<ProfitShareMasterResponse>) => {
      state.profitSharingUpdate = {
        ...action.payload,
        reportName: "Revert"
      };
    },
    setProfitMasterRevertLoading: (state) => {
      // @ts-ignore
      state.profitSharingUpdate = { isLoading: true, reportName: "Revert" };
    },
    clearProfitMasterRevert: (state) => {
      // @ts-ignore
      state.profitSharingUpdate = null;
    },

    setMilitaryAndRehireEntryAndModificationEmployeeDetails: (state, action: PayloadAction<EmployeeDetails>) => {
      state.militaryAndRehireEntryAndModification = action.payload;
    }
  }
});

export const {
  setDuplicateSSNsData,
  setDemographicBadgesNotInPayprofitData,
  setNegativeEtvaForSssnsOnPayprofit,
  setDuplicateNamesAndBirthdays,
  setMissingCommaInPYName,
  setEmployeesOnMilitaryLeaveDetails,
  setMilitaryAndRehireForfeituresDetails,
  setMilitaryAndRehireProfitSummaryDetails,
  setDistributionsAndForfeitures,
  setExecutiveHoursAndDollars,
  setEligibleEmployees,
  setMasterInquiryData,
  clearMasterInquiryData,
  setDistributionsByAge,
  setContributionsByAge,
  setForfeituresByAge,
  setBalanceByAge,
  setBalanceByYears,
  setVestingAmountByAge,
  setTermination,

  setProfitUpdate,
  setProfitUpdateLoading,
  clearProfitUpdate,

  setProfitEdit,
  setProfitEditLoading,
  clearProfitEdit,

  setProfitMasterApply,
  setProfitMasterApplyLoading,
  clearProfitMasterApply,

  setProfitMasterRevert,
  setProfitMasterRevertLoading,
  clearProfitMasterRevert,

  setExecutiveHoursAndDollarsGridYear,
  updateExecutiveHoursAndDollarsGridRow,
  removeExecutiveHoursAndDollarsGridRow,
  clearExecutiveHoursAndDollarsGridRows,
  addExecutiveHoursAndDollarsGridRow
} = yearsEndSlice.actions;
export default yearsEndSlice.reducer;
