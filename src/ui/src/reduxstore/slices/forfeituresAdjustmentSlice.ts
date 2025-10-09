import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { SuggestForfeitAmountResponse, SuggestForfeitureAdjustmentRequest } from "../types";

export interface ForfeituresAdjustmentState {
  forfeitureAdjustmentData: SuggestForfeitAmountResponse | null;
  forfeitureAdjustmentQueryParams: SuggestForfeitureAdjustmentRequest | null;
}

const initialState: ForfeituresAdjustmentState = {
  forfeitureAdjustmentData: null,
  forfeitureAdjustmentQueryParams: null
};

export const forfeituresAdjustmentSlice = createSlice({
  name: "forfeituresAdjustment",
  initialState,
  reducers: {
    setForfeitureAdjustmentData: (state, action: PayloadAction<SuggestForfeitAmountResponse>) => {
      state.forfeitureAdjustmentData = action.payload;
    },
    clearForfeitureAdjustmentData: (state) => {
      state.forfeitureAdjustmentData = null;
    },
    setForfeitureAdjustmentQueryParams: (state, action: PayloadAction<SuggestForfeitureAdjustmentRequest>) => {
      state.forfeitureAdjustmentQueryParams = action.payload;
    },
    clearForfeitureAdjustmentQueryParams: (state) => {
      state.forfeitureAdjustmentQueryParams = null;
    }
  }
});

export const {
  setForfeitureAdjustmentData,
  clearForfeitureAdjustmentData,
  setForfeitureAdjustmentQueryParams,
  clearForfeitureAdjustmentQueryParams
} = forfeituresAdjustmentSlice.actions;

export default forfeituresAdjustmentSlice.reducer;
