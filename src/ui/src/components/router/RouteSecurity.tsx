import { useEffect, useState, useCallback, ReactNode } from "react";
import { Security } from "@okta/okta-react";
import { OktaAuth, toRelativeUrl } from "@okta/okta-auth-js";

import { Route, Routes, useNavigate } from "react-router";
import oktaConfig from "../../Okta/config";
import Login from "components/Login/Login";
import OktaLoginCallback from "components/MenuBar/OktaLoginCallback";

const clientId = import.meta.env.VITE_REACT_APP_OKTA_CLIENT_ID;
const issuer = import.meta.env.VITE_REACT_APP_OKTA_ISSUER;

interface RouteSecurityProps {
  oktaEnabled: boolean;
  children?: ReactNode;
}

//@ts-expect-error we do not have types for this
const RouteSecurity: React.FC<RouteSecurityProps> = ({ oktaEnabled, children }) => {
  const [oktaAuth, setOktaAuth] = useState<OktaAuth | null>(null);
  const navigate = useNavigate();
  useEffect(() => {
    const config = oktaConfig(clientId, issuer);
    setOktaAuth(new OktaAuth(config.oidc));
  }, []);

  const restoreOriginalUri = useCallback(
    //@ts-expect-error we do not have types for this
    async (_oktaAuth, originalUri) => {
      const storageName = "SMARTAPP_SignInRedirectUrl"; // Project setup: update to new key
      let signInRedirectUrl = localStorage.getItem(storageName) || "";
      if (originalUri) {
        localStorage.setItem(storageName, originalUri || "");
        signInRedirectUrl = originalUri;
      }

      if (navigate) {
        navigate(toRelativeUrl(signInRedirectUrl, window.location.origin));
      }
    },
    [navigate]
  );

  if (oktaAuth === null) {
    return null;
  }

  if (oktaEnabled) {
    return (
      <Security
        oktaAuth={oktaAuth}
        restoreOriginalUri={restoreOriginalUri}>
        <Routes>
          <Route
            path="/login/callback"
            Component={OktaLoginCallback}
          />
          <Route
            path="/*"
            element={<Login></Login>}>
            {children}
          </Route>
        </Routes>
      </Security>
    );
  } else {
    return (
      <Security
        oktaAuth={oktaAuth}
        restoreOriginalUri={restoreOriginalUri}>
        <Routes>
          <Route path="/" />
          {children}
        </Routes>
      </Security>
    );
  }
};

export default RouteSecurity;
