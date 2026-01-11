import { OktaAuth, toRelativeUrl } from "@okta/okta-auth-js";
import { Security, useOktaAuth } from "@okta/okta-react";
import { ReactNode, useEffect, useState } from "react";
import { useDispatch } from "react-redux";

import { useNavigate } from "react-router";
import oktaConfig from "../../Okta/config";
import { setToken } from "../../reduxstore/slices/securitySlice";

const clientId = import.meta.env.VITE_REACT_APP_OKTA_CLIENT_ID;
const issuer = import.meta.env.VITE_REACT_APP_OKTA_ISSUER;

interface RouteSecurityProps {
  oktaEnabled: boolean;
  children?: ReactNode;
}

// Component that monitors Okta auth state and syncs token to Redux
const OktaTokenSync: React.FC = () => {
  const { authState } = useOktaAuth();
  const dispatch = useDispatch();

  useEffect(() => {
    console.log(
      "[OktaTokenSync] Auth state changed:",
      authState?.isAuthenticated,
      "token:",
      authState?.accessToken?.accessToken ? "present" : "null"
    );

    if (authState?.isAuthenticated && authState.accessToken?.accessToken) {
      dispatch(setToken(authState.accessToken.accessToken));
    }
  }, [authState, dispatch]);

  return null;
};

// Inner component that uses useNavigate - must be within Router context
const RestoreOriginalUriHandler: React.FC = () => {
  const navigate = useNavigate();

  useEffect(() => {
    const storageName = "SMARTAPP_SignInRedirectUrl";
    const signInRedirectUrl = localStorage.getItem(storageName) || "";
    if (signInRedirectUrl) {
      navigate(toRelativeUrl(signInRedirectUrl, window.location.origin));
      localStorage.removeItem(storageName);
    }
  }, [navigate]);

  return null;
};

const RouteSecurity: React.FC<RouteSecurityProps> = ({ oktaEnabled, children }) => {
  const [oktaAuth, setOktaAuth] = useState<OktaAuth | null>(null);
  const dispatch = useDispatch();

  console.log(
    "[RouteSecurity] Rendering - oktaEnabled:",
    oktaEnabled,
    "oktaAuth:",
    oktaAuth !== null ? "initialized" : "null"
  );

  // Initialize token for non-Okta environments
  useEffect(() => {
    if (!oktaEnabled) {
      console.log("[RouteSecurity] Okta disabled, setting default token");
      // In non-Okta environments (development), set a dummy token to allow API calls
      dispatch(setToken("dev-token"));
    }
  }, [oktaEnabled, dispatch]);

  useEffect(() => {
    if (oktaEnabled) {
      console.log("[RouteSecurity] Initializing OktaAuth...");
      const config = oktaConfig(clientId, issuer);
      setOktaAuth(new OktaAuth(config.oidc));
    }
  }, [oktaEnabled]);

  const restoreOriginalUri = async (_oktaAuth: OktaAuth, originalUri: string) => {
    const storageName = "SMARTAPP_SignInRedirectUrl";
    if (originalUri) {
      localStorage.setItem(storageName, originalUri);
    }
    // Navigation will be handled by RestoreOriginalUriHandler within Router context
  };

  // If Okta is not enabled, just render children without Security wrapper
  if (!oktaEnabled) {
    console.log("[RouteSecurity] Okta disabled, rendering children directly");
    return <>{children}</>;
  }

  // If Okta is enabled but not initialized yet, show loading
  if (oktaAuth === null) {
    console.log("[RouteSecurity] Okta enabled but not initialized yet");
    return <div>Loading authentication...</div>;
  }

  console.log("[RouteSecurity] Okta enabled and initialized, wrapping with Security");
  return (
    <Security
      oktaAuth={oktaAuth}
      restoreOriginalUri={restoreOriginalUri}>
      <OktaTokenSync />
      {children}
      <RestoreOriginalUriHandler />
    </Security>
  );
};

export default RouteSecurity;
