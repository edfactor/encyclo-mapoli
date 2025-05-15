import { createSlice, PayloadAction } from '@reduxjs/toolkit'
import {BeneficiaryRequestDto, BeneficiaryResponseDto} from '../types';

export interface NavigationState {
  beneficiaryList: BeneficiaryResponseDto | null;
  beneficiaryRequest: BeneficiaryRequestDto | null;
  error: string | null;
}
const initialState: NavigationState = {
  beneficiaryList: null,
  beneficiaryRequest: null,
  error: null
};


export const beneficiarySlice = createSlice({
    name: 'navigation',
    initialState,
    reducers:{
        setBeneficiary: (state, action: PayloadAction<BeneficiaryResponseDto | null>) => {
            if(action.payload){
                state.beneficiaryList = action.payload;
                state.error = null;
            }
            else {
                state.error = "Failed to fetch  navigations"
            }
        },
        setBeneficiaryRequest: (state, action: PayloadAction<BeneficiaryRequestDto | null>) => {
            if(action.payload){
                state.beneficiaryRequest = action.payload;
                state.error = null;
            }
        },
        setBeneficiaryError: (state, action: PayloadAction<string>) => {
            state.error = action.payload;
            state.beneficiaryList = null;
        }
    }
});

export const { setBeneficiary, setBeneficiaryError, setBeneficiaryRequest} = beneficiarySlice.actions;

export default beneficiarySlice.reducer;