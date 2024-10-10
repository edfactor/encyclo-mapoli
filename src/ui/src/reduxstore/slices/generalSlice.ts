import { createSlice } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";

export const DEFAULT_BANNER = "Blackhawk";

export interface GeneralState {
  appBanner: string;
  error: string;
  onDropdownBlur: { onBlur: boolean };
  onFlatDateBlur: { onBlur: boolean };
  loading: boolean;
}

const initialState: GeneralState = {
  appBanner: DEFAULT_BANNER,
  error: "",
  onDropdownBlur: { onBlur: false },
  onFlatDateBlur: { onBlur: false },
  loading: false
};

export const generalSlice = createSlice({
  name: "general",
  initialState,
  reducers: {
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

export const { setBanner, setError, setOnDropdownBlur, setOnFlatdateBlur, setLoading } = generalSlice.actions;
export default generalSlice.reducer;
