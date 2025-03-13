import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { MasterInquiryDetail, PagedReportResponse } from "reduxstore/types";

export interface MilitaryState {
  militaryContributionsData: PagedReportResponse<MasterInquiryDetail> | null;
  error: string | null;
}

const initialState: MilitaryState = {
  militaryContributionsData: null,
  error: null
};

export const militarySlice = createSlice({
  name: "military",
  initialState,
  reducers: {
    setMilitaryContributions: (state, action: PayloadAction<PagedReportResponse<MasterInquiryDetail> | null>) => {
      if (action.payload) {
        state.militaryContributionsData = action.payload;
        state.error = null;
      } else {
        state.error = "Failed to fetch military contributions";
      }
    },
    clearMilitaryContributions: (state) => {
      state.militaryContributionsData = null;
      state.error = null;
    },
    setMilitaryError: (state, action: PayloadAction<string>) => {
      state.error = action.payload;
      state.militaryContributionsData = null;
    }
  }
});

export const { setMilitaryContributions, clearMilitaryContributions, setMilitaryError } = militarySlice.actions;

export default militarySlice.reducer;
