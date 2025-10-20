import React from "react";
import { useDispatch } from "react-redux";
import { useOktaAuth } from "@okta/okta-react";
import { Dispatch } from "@reduxjs/toolkit";
import { setToken, clearUserData } from "reduxstore/slices/securitySlice";
import { OktaAuth } from "@okta/okta-auth-js";

interface LogoutButtonProps {
  redirectUri?: string;
  children?: React.ReactNode;
}

const LogoutButton: React.FC<LogoutButtonProps> = ({ redirectUri, children }) => {
  const dispatch = useDispatch();
  const { oktaAuth } = useOktaAuth();

  const postLogoutRedirectUri = redirectUri || window.location.origin;

  const handleLogout = () => {
    if (oktaAuth) {
      dispatch(setToken(""));
      dispatch(clearUserData());
      oktaAuth.signOut({ postLogoutRedirectUri });
    }
  };

  return <button onClick={handleLogout}>{children || "Logout"}</button>;
};

export default LogoutButton;

interface PerformLogoutOptions {
  dispatch: Dispatch;
  oktaAuth: OktaAuth;
  redirectUri?: string;
}

// Also create a utility function for programmatic logout
export const performLogout = (options: PerformLogoutOptions) => {
  const { dispatch, oktaAuth, redirectUri } = options;
  const postLogoutRedirectUri = redirectUri || window.location.origin;

  if (oktaAuth) {
    dispatch(setToken(""));
    dispatch(clearUserData());
    oktaAuth.signOut({ postLogoutRedirectUri });
  }
};
