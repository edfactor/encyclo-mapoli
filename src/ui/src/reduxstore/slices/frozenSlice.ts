import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import {
  ContributionsByAge,
  ForfeituresByAge,
  BalanceByAge,
  BalanceByYears,
  DemographicBadgesNotInPayprofit,
  DistributionsAndForfeitures,
  DuplicateNameAndBirthday,
  DuplicateSSNDetail,
  EligibleEmployeeResponseDto,
  ExecutiveHoursAndDollars,
  FrozenReportsByAgeRequestType,
  MasterInquiryDetail,
  EmployeesOnMilitaryLeaveResponse,
  MilitaryAndRehireForfeiture,
  MilitaryAndRehireProfitSummary,
  MissingCommasInPYName,
  NegativeEtvaForSSNsOnPayProfit,
  PagedReportResponse,
  ProfitSharingDistributionsByAge,
  EmployeeDetails,
  MasterInquiryResponseType,
  VestedAmountsByAge,
  TerminationResponse, ProfitShareUpdateResponse, ProfitShareEditResponse, ProfitShareMasterResponse
} from "reduxstore/types";
import { Paged } from "smart-ui-library";

export interface FrozenState {
  duplicateSSNsData: PagedReportResponse<DuplicateSSNDetail> | null;
  
}

const initialState: FrozenState = {
  duplicateSSNsData: null,  
};

export const frozenSlice = createSlice({
  name: "yearsEnd",
  initialState,
  reducers: {
    setDuplicateSSNsData: (state, action: PayloadAction<PagedReportResponse<DuplicateSSNDetail>>) => {
      state.duplicateSSNsData = action.payload;
    }
  }
});

export const {
  setDuplicateSSNsData,
  
} = frozenSlice.actions;
export default frozenSlice.reducer;
