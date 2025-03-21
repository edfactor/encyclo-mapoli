import { createSlice } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";
import { clear } from "console";

export const DEFAULT_BANNER = "Profit Sharing";

export interface GeneralState {
  appBanner: string;
  error: string;
  onDropdownBlur: { onBlur: boolean };
  onFlatDateBlur: { onBlur: boolean };
  loading: boolean;
  drawerOpen?: boolean;
  activeSubmenu?: string;
}

const initialState: GeneralState = {
  appBanner: DEFAULT_BANNER,
  error: "",
  onDropdownBlur: { onBlur: false },
  onFlatDateBlur: { onBlur: false },
  loading: false,
  drawerOpen: false,
  activeSubmenu: ""
};

export const generalSlice = createSlice({
  name: "general",
  initialState,
  reducers: {
    setActiveSubMenu: (state, action: PayloadAction<string>) => {
      state.activeSubmenu = action.payload;
    },
    clearActiveSubMenu: (state) => {
      state.activeSubmenu = "";
    },
    openDrawer: (state) => {
      console.log("In reducer opening drawer");
      state.drawerOpen = true;
    },
    closeDrawer: (state) => {
      console.log("In reducer closing drawer");
      state.drawerOpen = false;
    },
    setBanner: (state, action: PayloadAction<string>) => {
      state.appBanner = action.payload;
    },

    setError: (state, { payload }) => {
      if (payload.data.Messag) {
        state.error = payload.data.Message;
        return;
      }
      if (payload.error || payload === "") {
        state.error = payload.error;
        return;
      }
      if (payload.data.title) {
        state.error = payload.data.title;
        return;
      }

      if (payload.status !== 200) {
        if (payload.data) {
          state.error = payload.data;
        } else {
          state.error = "Network error - check log";
        }

        return;
      }
      state.error = payload;
    },
    setOnDropdownBlur: (state, action: PayloadAction<{ onBlur: boolean }>) => {
      state.onDropdownBlur = action.payload;
    },
    setOnFlatdateBlur: (state, action: PayloadAction<{ onBlur: boolean }>) => {
      state.onFlatDateBlur = action.payload;
    },
    setLoading: (state, action: PayloadAction<boolean>) => {
      state.loading = action.payload;
    }
  }
});

export const {
  setBanner,
  setError,
  setOnDropdownBlur,
  setOnFlatdateBlur,
  setLoading,
  openDrawer,
  closeDrawer,
  setActiveSubMenu,
  clearActiveSubMenu
} = generalSlice.actions;
export default generalSlice.reducer;
