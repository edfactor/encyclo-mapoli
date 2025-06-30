import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { Paged } from 'smart-ui-library';
import { BeneficiaryDto, BeneficiaryRequestDto } from '../types';

export interface BeneficiaryState {
  beneficiaryList: Paged<BeneficiaryDto> | null;
  beneficiaryRequest: BeneficiaryRequestDto | null;
  error: string | null;
}
const initialState: BeneficiaryState = {
  beneficiaryList: null,
  beneficiaryRequest: null,
  error: null
};


export const beneficiarySlice = createSlice({
    name: 'navigation',
    initialState,
    reducers:{
        setBeneficiary: (state, action: PayloadAction<Paged<BeneficiaryDto> | null>) => {
            if(action.payload){
                state.beneficiaryList = action.payload;
                state.error = null;
            }
            else {
                state.error = "Failed to fetch  beneficiaries"
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