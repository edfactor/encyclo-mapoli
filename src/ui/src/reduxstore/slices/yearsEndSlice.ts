import { PayloadAction, createSlice } from "@reduxjs/toolkit";
import { DemographicBadgesNotInPayprofit, DuplicateSSNDetail, MissingCommasInPYName, PagedReportResponse } from "reduxstore/types";

export interface YearsEndState {
  duplicateSSNsData: PagedReportResponse<DuplicateSSNDetail> | null;
  demographicBadges: PagedReportResponse<DemographicBadgesNotInPayprofit> | null;
  missingCommaInPYName: PagedReportResponse<MissingCommasInPYName> | null;
}

const initialState: YearsEndState = {
    duplicateSSNsData: null,
    demographicBadges: null,
    missingCommaInPYName: null
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
    setMissingCommaInPYName: (state, action: PayloadAction<PagedReportResponse<MissingCommasInPYName>>) => {
      state.missingCommaInPYName = action.payload;
    },
  }});

export const { setDuplicateSSNsData, setDemographicBadgesNotInPayprofitData, setMissingCommaInPYName } = yearsEndSlice.actions;
export default yearsEndSlice.reducer;
