
import { createSlice, PayloadAction } from "@reduxjs/toolkit";

// Types for merge operations
export interface MergeProfitsDetailRequest {
  sourceEmployeeId: number;
  destinationEmployeeId: number;
  profitYear: number;
  notes?: string;
}

export interface MergeProfitsDetailResponse {
  success: boolean;
  message: string;
  mergedRecordsCount: number;
  sourceEmployeeId: number;
  destinationEmployeeId: number;
  transactionId?: string;
}

export interface AdjustmentsState {
  // Merge operation state
  isMerging: boolean;
  mergeError: string | null;
  mergeSuccess: boolean;
  mergeResult: MergeProfitsDetailResponse | null;
  
  // UI state
  selectedProfitYear: number;
  mergeNotes: string;
}

const currentYear = new Date().getFullYear();

const initialState: AdjustmentsState = {
  isMerging: false,
  mergeError: null,
  mergeSuccess: false,
  mergeResult: null,
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
      state.mergeResult = action.payload;
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
      state.mergeResult = null;
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