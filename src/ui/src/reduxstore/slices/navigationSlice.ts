import { createSlice, PayloadAction } from '@reduxjs/toolkit'
import {NavigationResponseDto} from '../types';

export interface NavigationState {
  navigationData: NavigationResponseDto | null;
  error: string | null;
}
const initialState: NavigationState = {
  navigationData: null,
  error: null
};


export const navigationSlice = createSlice({
    name: 'navigation',
    initialState,
    reducers:{
        setNavigation: (state, action: PayloadAction<NavigationResponseDto | null>) => {
            if(action.payload){
                state.navigationData = action.payload;
                state.error = null;
            }
            else {
                state.error = "Failed to fetch  navigations"
            }
        },
        setNavigationError: (state, action: PayloadAction<string>) => {
            state.error = action.payload;
            state.navigationData = null;
        }
    }
});

export const { setNavigation, setNavigationError} = navigationSlice.actions;

export default navigationSlice.reducer;