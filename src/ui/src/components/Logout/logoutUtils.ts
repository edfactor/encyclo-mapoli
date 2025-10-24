import { Dispatch } from "@reduxjs/toolkit";
import { setToken, clearUserData } from "reduxstore/slices/securitySlice";
import { OktaAuth } from "@okta/okta-auth-js";

export interface PerformLogoutOptions {
  dispatch: Dispatch;
  oktaAuth: OktaAuth;
  redirectUri?: string;
}

// Utility function for programmatic logout
export const performLogout = (options: PerformLogoutOptions) => {
  const { dispatch, oktaAuth, redirectUri } = options;
  const postLogoutRedirectUri = redirectUri || window.location.origin;

  if (oktaAuth) {
    dispatch(setToken(""));
    dispatch(clearUserData());
    oktaAuth.signOut({ postLogoutRedirectUri });
  }
};
