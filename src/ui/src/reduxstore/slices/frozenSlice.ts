import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import {
  FrozenStateResponse
} from "reduxstore/types";

export interface FrozenState {
  frozenStateResponseData: FrozenStateResponse | null;
  error: string | null;
}

const initialState: FrozenState = {
  frozenStateResponseData: null,
  error: null,
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
    }
  }
});


export const {
  setFrozenStateResponse,  
} = frozenSlice.actions;
export default frozenSlice.reducer;
