import { PayloadAction, createSlice } from "@reduxjs/toolkit";
import { DemographicBadgesNotInPayprofit, DuplicateSSNDetail, NegativeEtvaForSSNsOnPayProfit, PagedReportResponse } from "reduxstore/types";

export interface YearsEndState {
  duplicateSSNsData: PagedReportResponse<DuplicateSSNDetail> | null;
  demographicBadges: PagedReportResponse<DemographicBadgesNotInPayprofit> | null;
  negativeEtvaForSSNsOnPayprofit: PagedReportResponse<NegativeEtvaForSSNsOnPayProfit> | null;
}

const initialState: YearsEndState = {
    duplicateSSNsData: null,
    demographicBadges: null,
    negativeEtvaForSSNsOnPayprofit: null,
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
    setNegativeEtvaForSssnsOnPayprofit: (state, action: PayloadAction<PagedReportResponse<NegativeEtvaForSSNsOnPayProfit>>) => {
        state.negativeEtvaForSSNsOnPayprofit = action.payload;
    }
  }});

export const { setDuplicateSSNsData, setDemographicBadgesNotInPayprofitData, setNegativeEtvaForSssnsOnPayprofit } = yearsEndSlice.actions;
export default yearsEndSlice.reducer;
