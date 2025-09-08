import { createSlice, PayloadAction } from "@reduxjs/toolkit";

import { EmployeeDetails, GroupedProfitSummaryDto, MasterInquiryDetail, MasterInquirySearch } from "reduxstore/types";

export interface InquiryState {
  masterInquiryData: EmployeeDetails | null;
  masterInquiryMemberDetails: EmployeeDetails | null;
  masterInquiryResults: MasterInquiryDetail[] | null;
  masterInquiryRequestParams: MasterInquirySearch | null;
  masterInquiryGroupingData: GroupedProfitSummaryDto[] | null;
}

const initialState: InquiryState = {
  masterInquiryData: null,
  masterInquiryMemberDetails: null,
  masterInquiryResults: null,
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
        state.masterInquiryMemberDetails = action.payload;
      } else {
        state.masterInquiryMemberDetails = null;
      }
    },
    clearMasterInquiryData: (state) => {
      state.masterInquiryData = null;
      state.masterInquiryMemberDetails = null;
    },
    setMasterInquiryResults: (state, action: PayloadAction<MasterInquiryDetail[]>) => {
      state.masterInquiryResults = action.payload;
    },
    clearMasterInquiryResults: (state) => {
      state.masterInquiryResults = null;
    },
    setMasterInquiryGroupingData: (state, action: PayloadAction<GroupedProfitSummaryDto[]>) => {
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
  setMasterInquiryResults,
  clearMasterInquiryResults,
  setMasterInquiryGroupingData,
  clearMasterInquiryGroupingData
} = inquirySlice.actions;
export default inquirySlice.reducer;
