import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { MasterInquiryDetail, PagedReportResponse } from "reduxstore/types";
import { Paged } from "smart-ui-library";

export interface MilitaryState {
  militaryContributionsData: Paged<MasterInquiryDetail> | null;
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
    setMilitaryContributions: (state, action: PayloadAction<Paged<MasterInquiryDetail> | null>) => {
      if (action.payload) {
        state.militaryContributionsData = action.payload;
        state.error = null;
      } else {
        state.error = "Failed to fetch  contmilitaryributions";
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
