import { createSlice, PayloadAction } from '@reduxjs/toolkit'
import {CurrentNavigation, NavigationResponseDto} from '../types';

export interface NavigationState {
  navigationData: NavigationResponseDto | null;
  currentNavigation?: CurrentNavigation;
  error: string | null;
}
const initialState: NavigationState = {
  navigationData: null,
  currentNavigation: undefined,
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
        setCurrentNavigationId: (state, action: PayloadAction<CurrentNavigation | undefined>)=>{
            if(action?.payload){
                state.currentNavigation = action.payload;
            }
        },
        setNavigationError: (state, action: PayloadAction<string>) => {
            state.error = action.payload;
            state.navigationData = null;
        }
    }
});

export const { setNavigation,setCurrentNavigationId, setNavigationError} = navigationSlice.actions;

export default navigationSlice.reducer;