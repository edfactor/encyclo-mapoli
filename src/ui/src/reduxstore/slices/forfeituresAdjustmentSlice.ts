import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { ForfeitureAdjustmentResponse, ForfeitureAdjustmentRequest } from "../types";

interface ForfeituresAdjustmentState {
  forfeitureAdjustmentData: ForfeitureAdjustmentResponse | null;
  forfeitureAdjustmentQueryParams: ForfeitureAdjustmentRequest | null;
}

const initialState: ForfeituresAdjustmentState = {
  forfeitureAdjustmentData: null,
  forfeitureAdjustmentQueryParams: null
};

export const forfeituresAdjustmentSlice = createSlice({
  name: "forfeituresAdjustment",
  initialState,
  reducers: {
    setForfeitureAdjustmentData: (state, action: PayloadAction<ForfeitureAdjustmentResponse>) => {
      state.forfeitureAdjustmentData = action.payload;
    },
    clearForfeitureAdjustmentData: (state) => {
      state.forfeitureAdjustmentData = null;
    },
    setForfeitureAdjustmentQueryParams: (state, action: PayloadAction<ForfeitureAdjustmentRequest>) => {
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
