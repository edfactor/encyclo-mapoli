import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { NavigationResponseDto } from "../types";

export interface NavigationState {
  navigationData: NavigationResponseDto | null;
  error: string | null;
  currentNavigationId: number | null;
}
const initialState: NavigationState = {
  navigationData: null,
  error: null,
  currentNavigationId: null
};

export const navigationSlice = createSlice({
  name: "navigation",
  initialState,
  reducers: {
    setNavigation: (state, action: PayloadAction<NavigationResponseDto | null>) => {
      if (action.payload) {
        state.navigationData = action.payload;
        state.error = null;
      } else {
        state.error = "Failed to fetch  navigations";
      }
    },
    setNavigationError: (state, action: PayloadAction<string>) => {
      state.error = action.payload;
    },
    setCurrentNavigationId: (state, action: PayloadAction<number>) => {
      state.currentNavigationId = action.payload;
    }
  }
});

export const { setNavigation, setNavigationError, setCurrentNavigationId } = navigationSlice.actions;

export default navigationSlice.reducer;
