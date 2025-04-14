import { createSlice, PayloadAction } from "@reduxjs/toolkit";

export interface ApiMessage {
  type: "error" | "success";
  title: string;
  message?: string;
}

export interface MessagesState {
  [key: string]: ApiMessage;
}

export interface MessageUpdate {
  key: string;
  message: ApiMessage;
}
const initialState: MessagesState = {};

const messagesSlice = createSlice({
  name: "messages",
  initialState,
  reducers: {
    setMessage: (state, action: PayloadAction<MessageUpdate>) => {
      state[action.payload.key] = action.payload.message;
    },
    removeMessage: (state, action: PayloadAction<string>) => {
      delete state[action.payload];
    },
    clearMessages: () => initialState
  }
});

export const { setMessage, removeMessage, clearMessages } = messagesSlice.actions;

export const selectMessage = <RootState extends { messages: Record<string, string> }>(state: RootState, key: string) =>
  state.messages[key];

export const messageSlice = messagesSlice.reducer;
