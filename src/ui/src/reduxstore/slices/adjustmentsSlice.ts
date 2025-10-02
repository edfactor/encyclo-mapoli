
import { createSlice, PayloadAction } from "@reduxjs/toolkit";

export interface AdjustmentsState {
  // Merge operation state
  isMerging: boolean;
  mergeError: string | null;
  mergeSuccess: boolean;
  
  selectedProfitYear: number;
  mergeNotes: string;
}

const currentYear = new Date().getFullYear();

const initialState: AdjustmentsState = {
  isMerging: false,
  mergeError: null,
  mergeSuccess: false,
  selectedProfitYear: currentYear,
  mergeNotes: ""
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
    
    setMergeSuccess: (state, action: PayloadAction<MergeProfitsDetailResponse>) => {
      state.isMerging = false;
      state.mergeSuccess = true;
      state.mergeError = null;
    },
    
    setMergeError: (state, action: PayloadAction<string>) => {
      state.isMerging = false;
      state.mergeError = action.payload;
      state.mergeSuccess = false;
    },
    
    // UI state actions
    setProfitYear: (state, action: PayloadAction<number>) => {
      state.selectedProfitYear = action.payload;
    },
    
    setMergeNotes: (state, action: PayloadAction<string>) => {
      state.mergeNotes = action.payload;
    },
    
    // Reset actions
    resetMerge: (state) => {
      state.isMerging = false;
      state.mergeError = null;
      state.mergeSuccess = false;
      state.mergeNotes = "";
    },
    
    resetAll: () => initialState
  }
});

export const {
  setMerging,
  setMergeSuccess,
  setMergeError,
  setProfitYear,
  setMergeNotes,
  resetMerge,
  resetAll
} = adjustmentsSlice.actions;

export default adjustmentsSlice.reducer;