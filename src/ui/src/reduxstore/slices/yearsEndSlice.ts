import { PayloadAction, createSlice } from "@reduxjs/toolkit";
import {
  DemographicBadgesNotInPayprofit,
  DuplicateNameAndBirthday,
  DuplicateSSNDetail,
  MilitaryAndRehire,
  MilitaryAndRehireForfeiture,
  MilitaryAndRehireProfitSummary,
  MissingCommasInPYName,
  NegativeEtvaForSSNsOnPayProfit,
  PagedReportResponse
} from "reduxstore/types";

export interface YearsEndState {
  duplicateSSNsData: PagedReportResponse<DuplicateSSNDetail> | null;
  demographicBadges: PagedReportResponse<DemographicBadgesNotInPayprofit> | null;
  duplicateNamesAndBirthday: PagedReportResponse<DuplicateNameAndBirthday> | null;
  negativeEtvaForSSNsOnPayprofit: PagedReportResponse<NegativeEtvaForSSNsOnPayProfit> | null;
  missingCommaInPYName: PagedReportResponse<MissingCommasInPYName> | null;
  militaryAndRehire: PagedReportResponse<MilitaryAndRehire> | null;
  militaryAndRehireForfeitures: PagedReportResponse<MilitaryAndRehireForfeiture> | null;
  militaryAndRehireProfitSummary: PagedReportResponse<MilitaryAndRehireProfitSummary> | null;
}

const initialState: YearsEndState = {
  duplicateSSNsData: null,
  demographicBadges: null,
  duplicateNamesAndBirthday: null,
  negativeEtvaForSSNsOnPayprofit: null,
  missingCommaInPYName: null,
  militaryAndRehire: null,
  militaryAndRehireForfeitures: null,
  militaryAndRehireProfitSummary: null
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
    }
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
  setMilitaryAndRehireProfitSummaryDetails
} = yearsEndSlice.actions;
export default yearsEndSlice.reducer;
