// src/reduxstore/slices/appSupportSlice.ts
import { PayloadAction, createSlice } from "@reduxjs/toolkit";
import { Health } from "../healthTypes";

const initialState: {
  health: Health | null;
} = {
  health: null
};

export const appSupportSlice = createSlice({
  name: "appSupport",
  initialState,
  reducers: {
    setHealthInfo: (state, action: PayloadAction<Health>) => {
      state.health = action.payload;
    }
  }
});

export const { setHealthInfo } = appSupportSlice.actions;
export default appSupportSlice.reducer;
