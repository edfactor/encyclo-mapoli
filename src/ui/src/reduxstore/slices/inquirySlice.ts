import { createSlice, PayloadAction } from "@reduxjs/toolkit";

import {
  BalanceByAge,
  BalanceByYears,
  ContributionsByAge,
  DemographicBadgesNotInPayprofit,
  DistributionsAndForfeitures,
  DistributionsAndForfeituresQueryParams,
  DuplicateNameAndBirthday,
  DuplicateSSNDetail,
  EligibleEmployeeResponseDto,
  EmployeeDetails,
  EmployeesOnMilitaryLeaveResponse,
  EmployeeWagesForYear,
  ExecutiveHoursAndDollars,
  ExecutiveHoursAndDollarsGrid,
  ExecutiveHoursAndDollarsQueryParams,
  ForfeituresAndPoints,
  ForfeituresAndPointsQueryParams,
  ForfeituresByAge,
  FrozenReportsByAgeRequestType,
  MasterInquiryResponseType,
  MasterInquirySearch,
  MilitaryAndRehireForfeiture,
  MilitaryAndRehireProfitSummary,
  MissingCommasInPYName,
  NegativeEtvaForSSNsOnPayProfit,
  PagedReportResponse,
  ProfitAndReportingQueryParams,
  ProfitShareEditResponse,
  ProfitShareMasterResponse,
  ProfitShareUpdateResponse,
  ProfitSharingDistributionsByAge, ProfitYearRequest,
  TerminationResponse,
  VestedAmountsByAge,
  YearEndProfitSharingReportResponse
} from "reduxstore/types";

export interface InquiryState {
  masterInquiryData: MasterInquiryResponseType | null;
  masterInquiryEmployeeDetails: EmployeeDetails | null;
  masterInquiryRequestParams: MasterInquirySearch | null;  
}

const initialState: InquiryState = {
  masterInquiryData: null,
  masterInquiryEmployeeDetails: null,
  masterInquiryRequestParams: null 
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

    setMasterInquiryData: (state, action: PayloadAction<MasterInquiryResponseType>) => {
      state.masterInquiryData = action.payload;

      if (action.payload.employeeDetails) {
        state.masterInquiryEmployeeDetails = action.payload.employeeDetails;
      }
    },
    clearMasterInquiryData: (state) => {
      state.masterInquiryData = null;
      state.masterInquiryEmployeeDetails = null;
    },   
  }
});

export const {
  clearMasterInquiryData,
  clearMasterInquiryRequestParams,
  setMasterInquiryData,
  setMasterInquiryRequestParams,  
} = inquirySlice.actions;
export default inquirySlice.reducer;
