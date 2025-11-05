import type { PayloadAction } from "@reduxjs/toolkit";
import { createSlice } from "@reduxjs/toolkit";

export const DEFAULT_BANNER = "Profit Sharing";

export interface GeneralState {
  appBanner: string;
  error: string;
  onDropdownBlur: { onBlur: boolean };
  onFlatDateBlur: { onBlur: boolean };
  loading: boolean;
  isDrawerOpen?: boolean;
  activeSubmenu?: string;
}

const getStoredDrawerState = (): { isDrawerOpen: boolean; activeSubmenu: string } => {
  try {
    const storedDrawerState = localStorage.getItem("drawerState");
    if (storedDrawerState) {
      return JSON.parse(storedDrawerState);
    }
  } catch (error) {
    console.error("Error reading drawer state from localStorage:", error);
  }
  return { isDrawerOpen: false, activeSubmenu: "" };
};

const { isDrawerOpen, activeSubmenu } = getStoredDrawerState();

const initialState: GeneralState = {
  appBanner: DEFAULT_BANNER,
  error: "",
  onDropdownBlur: { onBlur: false },
  onFlatDateBlur: { onBlur: false },
  loading: false,
  isDrawerOpen,
  activeSubmenu
};

export const generalSlice = createSlice({
  name: "general",
  initialState,
  reducers: {
    setActiveSubMenu: (state, action: PayloadAction<string>) => {
      state.activeSubmenu = action.payload;
      try {
        localStorage.setItem(
          "drawerState",
          JSON.stringify({
            isDrawerOpen: state.isDrawerOpen,
            activeSubmenu: action.payload
          })
        );
      } catch (error) {
        console.error("Error saving drawer state to localStorage:", error);
      }
    },
    clearActiveSubMenu: (state) => {
      state.activeSubmenu = "";
      try {
        localStorage.setItem(
          "drawerState",
          JSON.stringify({
            isDrawerOpen: state.isDrawerOpen,
            activeSubmenu: ""
          })
        );
      } catch (error) {
        console.error("Error saving drawer state to localStorage:", error);
      }
    },
    openDrawer: (state) => {
      state.isDrawerOpen = true;
      try {
        localStorage.setItem(
          "drawerState",
          JSON.stringify({
            isDrawerOpen: true,
            activeSubmenu: state.activeSubmenu
          })
        );
      } catch (error) {
        console.error("Error saving drawer state to localStorage:", error);
      }
    },
    closeDrawer: (state) => {
      state.isDrawerOpen = false;
      try {
        localStorage.setItem(
          "drawerState",
          JSON.stringify({
            isDrawerOpen: false,
            activeSubmenu: state.activeSubmenu
          })
        );
      } catch (error) {
        console.error("Error saving drawer state to localStorage:", error);
      }
    },
    setBanner: (state, action: PayloadAction<string>) => {
      state.appBanner = action.payload;
    },

    setError: (state, action: PayloadAction<Record<string, unknown>>) => {
      const { payload } = action;
      if (payload.data && typeof payload.data === "object" && "Messag" in payload.data) {
        const data = payload.data as Record<string, unknown>;
        if (data.Message) {
          state.error = String(data.Message);
        }
        return;
      }
      if (payload.error) {
        state.error = String(payload.error || "");
        return;
      }
      if (payload.data && typeof payload.data === "object" && "title" in payload.data) {
        const data = payload.data as Record<string, unknown>;
        if (data.title) {
          state.error = String(data.title);
        }
        return;
      }

      if (typeof payload.status === "number" && payload.status !== 200) {
        if (payload.data) {
          state.error = String(payload.data);
        } else {
          state.error = "Network error - check log";
        }

        return;
      }
      state.error = String(payload);
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
