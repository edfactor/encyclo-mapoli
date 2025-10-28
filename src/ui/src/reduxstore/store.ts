import { configureStore } from "@reduxjs/toolkit";
import { apiLoggerMiddleware } from "../middleware/apiLoggerMiddleware";
import { rtkQueryErrorToastMiddleware } from "../redux/rtkQueryErrorToastMiddleware";
import EnvironmentUtils from "../utils/environmentUtils";
import { AdjustmentsApi } from "./api/AdjustmentsApi";
import { AppSupportApi } from "./api/AppSupportApi";
import { BeneficiariesApi } from "./api/BeneficiariesApi";
import { CommonApi } from "./api/CommonApi";
import { DistributionApi } from "./api/DistributionApi";
import { InquiryApi } from "./api/InquiryApi";
import { ItOperationsApi } from "./api/ItOperationsApi";
import { LookupsApi } from "./api/LookupsApi";
import { MilitaryApi } from "./api/MilitaryApi";
import { NavigationApi } from "./api/NavigationApi";
import { NavigationStatusApi } from "./api/NavigationStatusApi";
import { PayServicesApi } from "./api/PayServicesApi";
import { SecurityApi } from "./api/SecurityApi";
import { validationApi } from "./api/ValidationApi";
import { YearsEndApi } from "./api/YearsEndApi";
import navigationStatusSlice from "./slices/NavigationStatusSlice";
import AppSupportSlice from "./slices/appSupportSlice";
import beneficiarySlice from "./slices/beneficiarySlice";
import commonSlice from "./slices/commonSlice";
import distributionSlice from "./slices/distributionSlice";
import forfeituresAdjustmentSlice from "./slices/forfeituresAdjustmentSlice";
import frozenSlice from "./slices/frozenSlice";
import generalSlice from "./slices/generalSlice";
import inquirySlice from "./slices/inquirySlice";
import lookupsSlice from "./slices/lookupsSlice";
import { messageSlice } from "./slices/messageSlice";
import militarySlice from "./slices/militarySlice";
import navigationSlice from "./slices/navigationSlice";
import securitySlice from "./slices/securitySlice";
import yearsEndSlice from "./slices/yearsEndSlice";

// Create the store with the slices and the APIs

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
    support: AppSupportSlice,
    messages: messageSlice,
    navigation: navigationSlice,
    navigationStatus: navigationStatusSlice,
    forfeituresAdjustment: forfeituresAdjustmentSlice,
    beneficiaries: beneficiarySlice,
    distribution: distributionSlice,

    [SecurityApi.reducerPath]: SecurityApi.reducer,
    [YearsEndApi.reducerPath]: YearsEndApi.reducer,
    [ItOperationsApi.reducerPath]: ItOperationsApi.reducer,
    [MilitaryApi.reducerPath]: MilitaryApi.reducer,
    [InquiryApi.reducerPath]: InquiryApi.reducer,
    [LookupsApi.reducerPath]: LookupsApi.reducer,
    [CommonApi.reducerPath]: CommonApi.reducer,
    [NavigationApi.reducerPath]: NavigationApi.reducer,
    [AppSupportApi.reducerPath]: AppSupportApi.reducer,
    [NavigationStatusApi.reducerPath]: NavigationStatusApi.reducer,
    [BeneficiariesApi.reducerPath]: BeneficiariesApi.reducer,
    [AdjustmentsApi.reducerPath]: AdjustmentsApi.reducer,
    [DistributionApi.reducerPath]: DistributionApi.reducer,
    [PayServicesApi.reducerPath]: PayServicesApi.reducer,
    [validationApi.reducerPath]: validationApi.reducer
  },

  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({ serializableCheck: false })
      .concat(rtkQueryErrorToastMiddleware(true))
      // Use array concat to avoid TS tuple union issues
      .concat(EnvironmentUtils.isDevelopmentOrQA ? [apiLoggerMiddleware] : [])
      .concat(SecurityApi.middleware)
      .concat(YearsEndApi.middleware)
      .concat(ItOperationsApi.middleware)
      .concat(MilitaryApi.middleware)
      .concat(InquiryApi.middleware)
      .concat(LookupsApi.middleware)
      .concat(CommonApi.middleware)
      .concat(NavigationApi.middleware)
      .concat(AppSupportApi.middleware)
      .concat(NavigationStatusApi.middleware)
      .concat(BeneficiariesApi.middleware)
      .concat(AdjustmentsApi.middleware)
      .concat(DistributionApi.middleware)
      .concat(PayServicesApi.middleware)
      .concat(validationApi.middleware)
});

// Infer the `RootState` and `AppDispatch` types from the store itself
export type RootState = ReturnType<typeof store.getState>;
// Inferred type: {posts: PostsState, comments: CommentsState, users: UsersState}
export type AppDispatch = typeof store.dispatch;
