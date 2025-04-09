import { PayloadAction, createSlice } from "@reduxjs/toolkit";
import { AppVersionInfo } from "reduxstore/api/CommonApi";

const initialState: AppVersionInfo = {
    buildNumber: '',
    gitHash: '',
    shortGitHash: '',
};

export const commonSlice = createSlice({
  name: "common",
  initialState,
  reducers: {
    setVersionInfo: (state, action: PayloadAction<AppVersionInfo>) => {
      state.buildNumber = action.payload.buildNumber;
      state.gitHash = action.payload.gitHash;
      state.shortGitHash = action.payload.shortGitHash;
    },
  }
});

export const { setVersionInfo } = commonSlice.actions;
export default commonSlice.reducer;
