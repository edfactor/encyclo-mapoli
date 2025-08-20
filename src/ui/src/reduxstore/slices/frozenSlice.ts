import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { FrozenStateResponse } from "reduxstore/types";
import { Paged } from "smart-ui-library";

export interface FrozenState {
  frozenStateResponseData: FrozenStateResponse | null;
  frozenStateCollectionData: Paged<FrozenStateResponse> | null;
  profitYearSelectorData: Paged<FrozenStateResponse> | null;
  error: string | null;
}

const initialState: FrozenState = {
  frozenStateResponseData: null,
  frozenStateCollectionData: null,
  profitYearSelectorData: null,
  error: null
};

export const frozenSlice = createSlice({
  name: "frozen",
  initialState,
  reducers: {
    setFrozenStateResponse: (state, action: PayloadAction<FrozenStateResponse | null>) => {
      if (action.payload) {
        state.frozenStateResponseData = action.payload;
        state.error = null;
      } else {
        state.error = "Failed to fetch frozen state";
      }
    },
    setFrozenStateCollectionResponse: (state, action: PayloadAction<Paged<FrozenStateResponse> | null>) => {
      if (action.payload) {
        state.frozenStateCollectionData = action.payload;
        state.error = null;
      } else {
        state.error = "Failed to fetch frozen state collection";
      }
    },
    setProfitYearSelectorData: (state, action: PayloadAction<Paged<FrozenStateResponse> | null>) => {
      if (action.payload) {
        state.profitYearSelectorData = action.payload;
        state.error = null;
      } else {
        state.error = "Failed to fetch profit year selector data";
      }
    }
  }
});

export const { setFrozenStateResponse, setFrozenStateCollectionResponse, setProfitYearSelectorData } =
  frozenSlice.actions;
export default frozenSlice.reducer;
