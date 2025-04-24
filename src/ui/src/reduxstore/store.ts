import { configureStore } from "@reduxjs/toolkit";
import generalSlice from "./slices/generalSlice";
import securitySlice from "./slices/securitySlice";
import yearsEndSlice from "./slices/yearsEndSlice";
import { SecurityApi } from "./api/SecurityApi";
import { YearsEndApi } from "./api/YearsEndApi";
import frozenSlice from "./slices/frozenSlice";
import { ItOperations } from "./api/ItOperations";
import { MilitaryApi } from "./api/MilitaryApi";
import militarySlice from "./slices/militarySlice";
import { InquiryApi } from "./api/InquiryApi";
import inquirySlice from "./slices/inquirySlice";
import { LookupsApi } from "./api/LookupsApi";
import lookupsSlice from "./slices/lookupsSlice";
import { rtkQueryErrorToastMiddleware } from "smart-ui-library";
import { CommonApi } from "./api/CommonApi";
import commonSlice from "./slices/commonSlice";
import { messageSlice } from "./slices/messageSlice";
import forfeituresAdjustmentSlice from "./slices/forfeituresAdjustmentSlice";

export const store = configureStore({
  reducer: {
    general: generalSlice,
    security: securitySlice,
    yearsEnd: yearsEndSlice,
    frozen: frozenSlice,
    military: militarySlice,
    inquiry: inquirySlice,
    lookups: lookupsSlice,
    common: commonSlice,
    messages: messageSlice,
    forfeituresAdjustment: forfeituresAdjustmentSlice,

    [SecurityApi.reducerPath]: SecurityApi.reducer,
    [YearsEndApi.reducerPath]: YearsEndApi.reducer,
    [ItOperations.reducerPath]: ItOperations.reducer,
    [MilitaryApi.reducerPath]: MilitaryApi.reducer,
    [InquiryApi.reducerPath]: InquiryApi.reducer,
    [LookupsApi.reducerPath]: LookupsApi.reducer,
    [CommonApi.reducerPath]: CommonApi.reducer
  },

  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({ serializableCheck: false })
      .concat(rtkQueryErrorToastMiddleware(true))
      .concat(SecurityApi.middleware)
      .concat(YearsEndApi.middleware)
      .concat(ItOperations.middleware)
      .concat(MilitaryApi.middleware)
      .concat(InquiryApi.middleware)
      .concat(LookupsApi.middleware)
      .concat(CommonApi.middleware)
});

// Infer the `RootState` and `AppDispatch` types from the store itself
export type RootState = ReturnType<typeof store.getState>;
// Inferred type: {posts: PostsState, comments: CommentsState, users: UsersState}
export type AppDispatch = typeof store.dispatch;
