import { Description, Settings } from "@mui/icons-material";
import { createTheme, ThemeProvider } from "@mui/material";
import { OktaAuth } from "@okta/okta-auth-js";
import AppErrorBoundary from "components/ErrorBoundary";
import PSLayout from "components/Layout/PSLayout";
import { useCallback, useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetMissivesQuery } from "reduxstore/api/LookupsApi";
import { colors, themeOptions, ToastServiceProvider } from "smart-ui-library";
import "smart-ui-library/dist/smart-ui-library.css";
import "../agGridConfig";
import Router from "./components/router/Router";
import oktaConfig from "./Okta/config";
import { useLazyGetHealthQuery } from "./reduxstore/api/AppSupportApi";
import { useGetAppVersionQuery } from "./reduxstore/api/CommonApi";
import { clearUserData, setUserGroups, setUsername } from "./reduxstore/slices/securitySlice"; // Adjust path as needed
import { RootState } from "./reduxstore/store";
import { getHealthStatusDescription } from "./utils/appSupportUtil";
import EnvironmentUtils from "./utils/environmentUtils";

// Types
interface BuildInfo {
  buildNumber?: string;
  buildId?: string;
  branch?: string;
  commitHash?: string;
}

const App = () => {
  // State management
  const dispatch = useDispatch();
  const clientId = import.meta.env.VITE_REACT_APP_OKTA_CLIENT_ID;
  const issuer = import.meta.env.VITE_REACT_APP_OKTA_ISSUER;

  useGetAppVersionQuery();
  const [uiBuildInfo, setUiBuildInfo] = useState<BuildInfo | null>(null);
  const [buildInfoText, setBuildInfoText] = useState("");
  const { buildNumber } = useSelector((state: RootState) => state.common);
  const [oktaAuth, setOktaAuth] = useState<any>(null);
  const [loadMissives] = useLazyGetMissivesQuery();
  const [triggerHealth] = useLazyGetHealthQuery();

  const health = useSelector((state: RootState) => state.support.health);

  useEffect(() => {
    const config = oktaConfig(clientId, issuer);
    const auth = new OktaAuth({
      ...config.oidc,
      services: {
        autoRenew: true,
        autoRemove: true,
        syncStorage: true
      }
    });
    setOktaAuth(auth);

    // Start the OktaAuth service to enable token auto-renewal (temporary, longterm solution is to use refresh tokens)
    // https://github.com/okta/okta-auth-js/blob/master/docs/autoRenew-notice.md
    auth.start();

    return () => {
      auth.stop();
    };
  }, [clientId, issuer]);

  // Redux selectors
  //const state = useSelector((state: RootState) => state);
  const { token, appUser, username: stateUsername } = useSelector((state: RootState) => state.security);

  // Add effect to update username when token changes
  useEffect(() => {
    if (token) {
      try {
        // Option 1: Extract username from JWT token
        const tokenPayload = JSON.parse(atob(token.split(".")[1]));
        let usernameFromToken =
          tokenPayload.username || tokenPayload.preferred_username || tokenPayload.sub || tokenPayload.email;

        // Format email-style usernames by extracting part before @
        if (usernameFromToken && usernameFromToken.includes("@")) {
          usernameFromToken = usernameFromToken.split("@")[0];
        }

        if (usernameFromToken && !stateUsername) {
          dispatch(setUsername(usernameFromToken));
        }

        const userGroups = tokenPayload.groups || [];
        dispatch(setUserGroups(userGroups));

        // Load lookup data
        loadMissives();
      } catch (error) {
        console.warn("Could not parse token for username:", error);
      }
    }
  }, [token, stateUsername, dispatch, loadMissives]);

  // Derived values
  const postLogoutRedirectUri = EnvironmentUtils.postLogoutRedirectUri;
  const isAuthenticated = !!token;
  const username = isAuthenticated ? appUser?.userName || stateUsername || "Guest" : "Not authenticated";

  // Event handlers
  const handleClick = useCallback((_e: React.MouseEvent<HTMLDivElement>) => {}, []);

  const handleLogout = () => {
    if (oktaAuth) {
      oktaAuth.signOut({ postLogoutRedirectUri });
      dispatch(clearUserData());
    }
  };

  // Side effects
  useEffect(() => {
    const fetchBuildInfo = async () => {
      try {
        const response = await fetch("/.buildinfo.json");
        if (!response.ok) {
          console.debug("buildinfo.json not available (expected in dev mode)");
          return;
        }
        const data = await response.json();
        setUiBuildInfo(data);
      } catch (e) {
        // Silently ignore buildinfo.json errors in development
        console.debug("buildinfo.json not available (expected in dev mode)");
      }
    };

    if (!uiBuildInfo) {
      fetchBuildInfo();
    }
  }, [uiBuildInfo]);

  useEffect(() => {
    const buildVersionNumber = uiBuildInfo
      ? `${uiBuildInfo.buildNumber ?? ""}.${uiBuildInfo.buildId ?? ""}`
      : "Local.Dev";

    if (buildNumber && buildVersionNumber) {
      const buildInfo = `${buildVersionNumber} | API Version: ${buildNumber}`;
      setBuildInfoText(buildInfo);
    }
  }, [buildNumber, uiBuildInfo]);

  useEffect(() => {
    triggerHealth();
  }, [triggerHealth]);

  // Theme setup
  const theme = createTheme({
    ...themeOptions,
    components: {
      ...themeOptions.components,
      MuiOutlinedInput: {
        styleOverrides: {
          root: {
            backgroundColor: colors["dsm-white"]
          }
        }
      }
    }
  });

  return (
    <ThemeProvider theme={theme}>
      <PSLayout
        onClick={handleClick}
        appTitle="Profit Sharing"
        items={[
          {
            title: "Debug Page",
            icon: <Settings />,
            onClick: () => {
              window.location.href = "/dev-debug";
            }
          },
          {
            title: "Documentation",
            icon: <Description />,
            onClick: () => {
              window.location.href = "/documentation";
            }
          }
        ]}
        logout={handleLogout}
        buildVersionNumber={buildInfoText}
        userName={username}
        environmentMode={EnvironmentUtils.envMode}
        apiStatus={health?.status as "Healthy" | "Degraded" | "Unhealthy" | undefined}
        apiStatusMessage={health ? getHealthStatusDescription(health) : undefined}
        oktaEnabled={EnvironmentUtils.isOktaEnabled}>
        <AppErrorBoundary>
          <ToastServiceProvider
            maxSnack={3}
            anchorOrigin={{ horizontal: "left", vertical: "bottom" }}>
            <Router />
          </ToastServiceProvider>
        </AppErrorBoundary>
      </PSLayout>
    </ThemeProvider>
  );
};

export default App;
