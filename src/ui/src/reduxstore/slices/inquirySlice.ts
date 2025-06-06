import { createSlice, PayloadAction } from "@reduxjs/toolkit";

import { EmployeeDetails, MasterInquiryDetail, MasterInquiryResponseDto, MasterInquirySearch } from "reduxstore/types";

export interface InquiryState {
  masterInquiryData: EmployeeDetails | null;
  masterInquiryEmployeeDetails: EmployeeDetails | null;
  masterInquiryRequestParams: MasterInquirySearch | null;
  masterInquiryGroupingData: MasterInquiryResponseDto[] | null;
}

const initialState: InquiryState = {
  masterInquiryData: null,
  masterInquiryEmployeeDetails: null,
  masterInquiryRequestParams: null,
  masterInquiryGroupingData: null
};

export const inquirySlice = createSlice({
  name: "inquiry",
  initialState,
  reducers: {
    setMasterInquiryRequestParams: (state, action: PayloadAction<MasterInquirySearch>) => {
      state.masterInquiryRequestParams = action.payload;
    },
    clearMasterInquiryRequestParams: (state) => {
      state.masterInquiryRequestParams = null;
    },

    setMasterInquiryData: (state, action: PayloadAction<EmployeeDetails>) => {
      state.masterInquiryData = action.payload;

      if (action.payload) {
        state.masterInquiryEmployeeDetails = action.payload;
      } else {
        state.masterInquiryEmployeeDetails = null;
      }
    },
    clearMasterInquiryData: (state) => {
      state.masterInquiryData = null;
      state.masterInquiryEmployeeDetails = null;
    },
    updateMasterInquiryResults: (state, action: PayloadAction<MasterInquiryDetail[]>) => {
      // Only update if masterInquiryData has inquiryResults property
      if (state.masterInquiryData && (state.masterInquiryData as any).inquiryResults) {
        (state.masterInquiryData as any).inquiryResults.results = [
          ...action.payload
        ];
      }
    },
    setMasterInquiryGroupingData: (state, action: PayloadAction<MasterInquiryResponseDto[]>) => {
      state.masterInquiryGroupingData = action.payload;
    },
    clearMasterInquiryGroupingData: (state) => {
      state.masterInquiryGroupingData = null;
    }
  }
});

export const {
  clearMasterInquiryData,
  clearMasterInquiryRequestParams,
  setMasterInquiryData,
  setMasterInquiryRequestParams,
  updateMasterInquiryResults,
  setMasterInquiryGroupingData,
  clearMasterInquiryGroupingData
} = inquirySlice.actions;
export default inquirySlice.reducer;
