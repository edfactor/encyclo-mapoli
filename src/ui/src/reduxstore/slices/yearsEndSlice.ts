import { PayloadAction, createSlice } from "@reduxjs/toolkit";
import { DemographicBadgesNotInPayprofit, DuplicateNameAndBirthday, DuplicateSSNDetail, PagedReportResponse } from "reduxstore/types";

export interface YearsEndState {
  duplicateSSNsData: PagedReportResponse<DuplicateSSNDetail> | null;
  demographicBadges: PagedReportResponse<DemographicBadgesNotInPayprofit> | null;
  duplicateNamesAndBirthday: PagedReportResponse<DuplicateNameAndBirthday> | null;
}

const initialState: YearsEndState = {
    duplicateSSNsData: null,
    demographicBadges: null,
    duplicateNamesAndBirthday: null
};

export const yearsEndSlice = createSlice({
  name: "yearsEnd",
  initialState,
  reducers: {
    setDuplicateSSNsData: (state, action: PayloadAction<PagedReportResponse<DuplicateSSNDetail>>) => {
        state.duplicateSSNsData = action.payload;
      },
    setDemographicBadgesNotInPayprofitData: (state, action: PayloadAction<PagedReportResponse<DemographicBadgesNotInPayprofit>>) => {
        state.demographicBadges = action.payload;
    },
    setDuplicateNamesAndBirthdays: (state, action: PayloadAction<PagedReportResponse<DuplicateNameAndBirthday>>) => {
      state.duplicateNamesAndBirthday = action.payload;
  },
  }});

export const { setDuplicateSSNsData, setDemographicBadgesNotInPayprofitData, setDuplicateNamesAndBirthdays } = yearsEndSlice.actions;
export default yearsEndSlice.reducer;
