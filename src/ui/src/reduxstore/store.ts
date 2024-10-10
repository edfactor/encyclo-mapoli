import { configureStore } from "@reduxjs/toolkit";
import generalSlice from "./slices/generalSlice";
import securitySlice from "./slices/securitySlice";
import yearsEndSlice from "./slices/yearsEndSlice";
import { SecurityApi } from "./api/SecurityApi";
import { YearsEndApi } from "./api/YearsEndApi";


export const store = configureStore({
  reducer: {
    general: generalSlice,
    security: securitySlice,
    yearsEnd: yearsEndSlice,

    [SecurityApi.reducerPath]: SecurityApi.reducer,
    [YearsEndApi.reducerPath]: YearsEndApi.reducer
  },

  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({ serializableCheck: false }).concat(SecurityApi.middleware).concat(YearsEndApi.middleware)
});

// Infer the `RootState` and `AppDispatch` types from the store itself
export type RootState = ReturnType<typeof store.getState>;
// Inferred type: {posts: PostsState, comments: CommentsState, users: UsersState}
export type AppDispatch = typeof store.dispatch;
