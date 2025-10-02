import { createSlice, PayloadAction } from "@reduxjs/toolkit";

export interface AdjustmentsState {
  // Merge operation state
  isMerging: boolean;
  mergeError: string | null;
  mergeSuccess: boolean;
}

const initialState: AdjustmentsState = {
  isMerging: false,
  mergeError: null,
  mergeSuccess: false,
};

const adjustmentsSlice = createSlice({
  name: "adjustments",
  initialState,
  reducers: {
    // Merge operation actions
    setMerging: (state, action: PayloadAction<boolean>) => {
      state.isMerging = action.payload;
      if (action.payload) {
        state.mergeError = null;
        state.mergeSuccess = false;
      }
    },
    
    setMergeSuccess: (state) => {
      state.isMerging = false;
      state.mergeSuccess = true;
      state.mergeError = null;
    },
    
    setMergeError: (state, action: PayloadAction<string>) => {
      state.isMerging = false;
      state.mergeError = action.payload;
      state.mergeSuccess = false;
    },
    
    // Reset actions
    resetMerge: (state) => {
      state.isMerging = false;
      state.mergeError = null;
      state.mergeSuccess = false;
    },
    
    resetAll: () => initialState
  }
});

export const {
  setMerging,
  setMergeSuccess,
  setMergeError,
  resetMerge,
  resetAll
} = adjustmentsSlice.actions;

export default adjustmentsSlice.reducer;