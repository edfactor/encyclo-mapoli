import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { GetNavigationStatusResponseDto } from "../types";

export interface NavigationStatusState {
  navigationStatusData: GetNavigationStatusResponseDto | null;
  error: string | null;
}
const initialState: NavigationStatusState = {
  navigationStatusData: null,
  error: null
};

export const navigationStatusSlice = createSlice({
  name: "navigationStatus",
  initialState,
  reducers: {
    setNavigationStatus: (state, action: PayloadAction<GetNavigationStatusResponseDto | null>) => {
      if (action.payload) {
        state.navigationStatusData = action.payload;
        state.error = null;
      } else {
        state.error = "Failed to fetch  navigation status";
      }
    },
    setNavigationStatusError: (state, action: PayloadAction<string>) => {
      state.error = action.payload;
      state.navigationStatusData = null;
    }
  }
});

export const { setNavigationStatus, setNavigationStatusError } = navigationStatusSlice.actions;

export default navigationStatusSlice.reducer;
