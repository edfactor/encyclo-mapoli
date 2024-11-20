import { PayloadAction, createSlice } from "@reduxjs/toolkit";
import {
  DemographicBadgesNotInPayprofit,
  DistributionsAndForfeitures,
  DuplicateNameAndBirthday,
  DuplicateSSNDetail,
  EligibleEmployeeResponseDto,
  ExecutiveHoursAndDollars,
  MasterInquiryDetail,
  MilitaryAndRehire,
  MilitaryAndRehireForfeiture,
  MilitaryAndRehireProfitSummary,
  MissingCommasInPYName,
  NegativeEtvaForSSNsOnPayProfit,
  PagedReportResponse
} from "reduxstore/types";
import { Paged } from "smart-ui-library";

export interface YearsEndState {
  duplicateSSNsData: PagedReportResponse<DuplicateSSNDetail> | null;
  demographicBadges: PagedReportResponse<DemographicBadgesNotInPayprofit> | null;
  duplicateNamesAndBirthday: PagedReportResponse<DuplicateNameAndBirthday> | null;
  negativeEtvaForSSNsOnPayprofit: PagedReportResponse<NegativeEtvaForSSNsOnPayProfit> | null;
  missingCommaInPYName: PagedReportResponse<MissingCommasInPYName> | null;
  militaryAndRehire: PagedReportResponse<MilitaryAndRehire> | null;
  militaryAndRehireForfeitures: PagedReportResponse<MilitaryAndRehireForfeiture> | null;
  militaryAndRehireProfitSummary: PagedReportResponse<MilitaryAndRehireProfitSummary> | null;
  distributionsAndForfeitures: PagedReportResponse<DistributionsAndForfeitures> | null;
  executiveHoursAndDollars: PagedReportResponse<ExecutiveHoursAndDollars> | null;
  eligibleEmployees: EligibleEmployeeResponseDto | null;
  masterInquiryData: Paged<MasterInquiryDetail> | null;
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
  eligibleEmployees: null,
  masterInquiryData: null
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
    setMilitaryAndRehireDetails: (state, action: PayloadAction<PagedReportResponse<MilitaryAndRehire>>) => {
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
    setExecutiveHoursAndDollars: (
      state,
      action: PayloadAction<PagedReportResponse<ExecutiveHoursAndDollars>>
    ) => {
      state.executiveHoursAndDollars = action.payload;
    },
    setEligibleEmployees: (
      state,
      action: PayloadAction<EligibleEmployeeResponseDto>
    ) => {
      state.eligibleEmployees = action.payload;
    },
    setMasterInquiryData: (
      state,
      action: PayloadAction<Paged<MasterInquiryDetail>>
    ) => {
      state.masterInquiryData = action.payload;
    },
    clearMasterInquiryData: (
      state
    ) => {
      state.masterInquiryData = null;
    },
  }
});

export const {
  setDuplicateSSNsData,
  setDemographicBadgesNotInPayprofitData,
  setNegativeEtvaForSssnsOnPayprofit,
  setDuplicateNamesAndBirthdays,
  setMissingCommaInPYName,
  setMilitaryAndRehireDetails,
  setMilitaryAndRehireForfeituresDetails,
  setMilitaryAndRehireProfitSummaryDetails,
  setDistributionsAndForfeitures,
  setExecutiveHoursAndDollars,
  setEligibleEmployees,
  setMasterInquiryData,
  clearMasterInquiryData
} = yearsEndSlice.actions;
export default yearsEndSlice.reducer;
