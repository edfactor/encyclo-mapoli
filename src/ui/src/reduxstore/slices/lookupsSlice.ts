import { createSlice, PayloadAction } from "@reduxjs/toolkit";

import { CalendarResponseDto, MissiveResponse, ProfitYearRequest, StateTaxLookupResponse } from "reduxstore/types";

export interface LookupState {
  accountingYearData: CalendarResponseDto | null;
  accountingYearRequestParams: ProfitYearRequest | null;
  missives: MissiveResponse[] | null;
  stateTaxData: StateTaxLookupResponse | null;
}

const initialState: LookupState = {
  accountingYearData: null,
  accountingYearRequestParams: null,
  missives: null,
  stateTaxData: null
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
      state.missives = action.payload;
    },

    setStateTaxData: (state, action: PayloadAction<StateTaxLookupResponse>) => {
      state.stateTaxData = action.payload;
    },
    clearStateTaxData: (state) => {
      state.stateTaxData = null;
    }
  }
});

export const {
  setAccountingYearParams,
  clearAccountingYearRequestParams,
  setAccountingYearData,
  clearAccountingYearData,
  setMissivesData,
  setStateTaxData,
  clearStateTaxData
} = lookupsSlice.actions;
export default lookupsSlice.reducer;
