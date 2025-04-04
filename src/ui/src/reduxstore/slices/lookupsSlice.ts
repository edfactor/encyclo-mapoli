import { createSlice, PayloadAction } from "@reduxjs/toolkit";

import {
  CalendarResponseDto,
  ProfitYearRequest
} from "reduxstore/types";

export interface LookupState {
  accountingYearData: CalendarResponseDto | null;
  accountingYearRequestParams: ProfitYearRequest | null;
}

const initialState: LookupState = {
  accountingYearData: null,
  accountingYearRequestParams: null
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
    }
  }
});

export const {
  setAccountingYearParams,
  clearAccountingYearRequestParams,
  setAccountingYearData,
  clearAccountingYearData
} = lookupsSlice.actions;
export default lookupsSlice.reducer;
