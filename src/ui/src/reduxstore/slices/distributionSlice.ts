import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import type { EmployeeDetails } from "../../types/employee/employee";
import type { DistributionSearchResponse } from "../../types/distributions";

export interface DistributionState {
  currentMember: EmployeeDetails | null;
  currentDistribution: DistributionSearchResponse | null;
  pendingDisbursements: DistributionSearchResponse[];
  historicalDisbursements: DistributionSearchResponse[];
}

const initialState: DistributionState = {
  currentMember: null,
  currentDistribution: null,
  pendingDisbursements: [],
  historicalDisbursements: []
};

const distributionSlice = createSlice({
  name: "distribution",
  initialState,
  reducers: {
    setCurrentMember: (state, action: PayloadAction<EmployeeDetails>) => {
      state.currentMember = action.payload;
    },
    clearCurrentMember: (state) => {
      state.currentMember = null;
    },
    setCurrentDistribution: (state, action: PayloadAction<DistributionSearchResponse>) => {
      state.currentDistribution = action.payload;
    },
    clearCurrentDistribution: (state) => {
      state.currentDistribution = null;
    },
    setPendingDisbursements: (state, action: PayloadAction<DistributionSearchResponse[]>) => {
      state.pendingDisbursements = action.payload;
    },
    clearPendingDisbursements: (state) => {
      state.pendingDisbursements = [];
    },
    setHistoricalDisbursements: (state, action: PayloadAction<DistributionSearchResponse[]>) => {
      state.historicalDisbursements = action.payload;
    },
    clearHistoricalDisbursements: (state) => {
      state.historicalDisbursements = [];
    }
  }
});

export const {
  setCurrentMember,
  clearCurrentMember,
  setCurrentDistribution,
  clearCurrentDistribution,
  setPendingDisbursements,
  clearPendingDisbursements,
  setHistoricalDisbursements,
  clearHistoricalDisbursements
} = distributionSlice.actions;

export default distributionSlice.reducer;
