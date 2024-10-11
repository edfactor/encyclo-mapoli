import { PayloadAction, createSlice } from "@reduxjs/toolkit";
import { DemographicBadgesNotInPayprofit, DuplicateSSNDetail, PagedReportResponse } from "reduxstore/types";

export interface YearsEndState {
  duplicateSSNsData: PagedReportResponse<DuplicateSSNDetail> | null;
  demographicBadges: PagedReportResponse<DemographicBadgesNotInPayprofit> | null;
}

const initialState: YearsEndState = {
    duplicateSSNsData: null,
    demographicBadges: null,
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
      }
  }});

export const { setDuplicateSSNsData, setDemographicBadgesNotInPayprofitData } = yearsEndSlice.actions;
export default yearsEndSlice.reducer;
