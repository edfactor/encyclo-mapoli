import { createSlice, PayloadAction } from "@reduxjs/toolkit";

import {
  CalendarResponseDto,
  MissiveResponse,
  ProfitYearRequest
} from "reduxstore/types";

export interface LookupState {
  accountingYearData: CalendarResponseDto | null;
  accountingYearRequestParams: ProfitYearRequest | null;
  missives: MissiveResponse[] | null;
}

const initialState: LookupState = {
  accountingYearData: null,
  accountingYearRequestParams: null,
  missives: null
};

export const lookupsSlice = createSlice({
  name: "Lookup",
  initialState,
  reducers: {
    setAccountingYearParams: (state, action: PayloadAction<ProfitYearRequest>) => {
      state.accountingYearRequestParams = action.payload;
    },
    clearAccountingYearRequestParams: (state) => {
      state.accountingYearRequestParams = null;
    },

    setAccountingYearData: (state, action: PayloadAction<CalendarResponseDto>) => {
      state.accountingYearData = action.payload;     
    },
    clearAccountingYearData: (state) => {
      state.accountingYearData = null;
    },

    setMissivesData: (state, action: PayloadAction<MissiveResponse[]>) => {
      // Note that the setting of warning is temporary until the backend
      // is updated to return the severity
      state.missives = action.payload.map((missive: MissiveResponse) => ({
        ...missive,
        severity: missive.severity ?? "warning",
      }));
    },

  }
});

export const {
  setAccountingYearParams,
  clearAccountingYearRequestParams,
  setAccountingYearData,
  clearAccountingYearData,
  setMissivesData,
} = lookupsSlice.actions;
export default lookupsSlice.reducer;
