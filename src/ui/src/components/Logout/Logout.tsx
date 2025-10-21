import React from "react";
import { useDispatch } from "react-redux";
import { useOktaAuth } from "@okta/okta-react";
import { setToken, clearUserData } from "reduxstore/slices/securitySlice";

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
