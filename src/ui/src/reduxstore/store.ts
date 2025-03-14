import { configureStore } from "@reduxjs/toolkit";
import generalSlice from "./slices/generalSlice";
import securitySlice from "./slices/securitySlice";
import yearsEndSlice from "./slices/yearsEndSlice";
import { SecurityApi } from "./api/SecurityApi";
import { YearsEndApi } from "./api/YearsEndApi";
import frozenSlice from "./slices/frozenSlice";
import { FrozenApi } from "./api/FrozenApi";
import { MilitaryApi } from "./api/MilitaryApi";
import militarySlice from "./slices/militarySlice";
import { InquiryApi } from "./api/InquiryApi";
import inquirySlice from "./slices/inquirySlice";

export const store = configureStore({
  reducer: {
    general: generalSlice,
    security: securitySlice,
    yearsEnd: yearsEndSlice,
    frozen: frozenSlice,
    military: militarySlice,
    inquiry: inquirySlice,

    [SecurityApi.reducerPath]: SecurityApi.reducer,
    [YearsEndApi.reducerPath]: YearsEndApi.reducer,
    [FrozenApi.reducerPath]: FrozenApi.reducer,
    [MilitaryApi.reducerPath]: MilitaryApi.reducer,
    [InquiryApi.reducerPath]: InquiryApi.reducer
  },

  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({ serializableCheck: false })
      .concat(SecurityApi.middleware)
      .concat(YearsEndApi.middleware)
      .concat(FrozenApi.middleware)
      .concat(MilitaryApi.middleware)
      .concat(InquiryApi.middleware)
});

// Infer the `RootState` and `AppDispatch` types from the store itself
export type RootState = ReturnType<typeof store.getState>;
// Inferred type: {posts: PostsState, comments: CommentsState, users: UsersState}
export type AppDispatch = typeof store.dispatch;
