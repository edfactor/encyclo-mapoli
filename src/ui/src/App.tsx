import { createTheme, ThemeProvider } from "@mui/material";
import { useCallback, useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "./reduxstore/store";
import { clearUserData, setUsername } from "./reduxstore/slices/securitySlice"; // Adjust path as needed
import AppErrorBoundary from "components/ErrorBoundary";
import PSLayout from "components/Layout/PSLayout";
import Router from "./components/router/Router";
import { useOktaAuth } from "@okta/okta-react";
import { themeOptions, ToastServiceProvider } from "smart-ui-library";
import "smart-ui-library/dist/smart-ui-library.css";
import "../agGridConfig";
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
  const okta = useOktaAuth();
  const [uiBuildInfo, setUiBuildInfo] = useState<BuildInfo | null>(null);

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
      } catch (error) {
        console.warn("Could not parse token for username:", error);
      }
    }
  }, [token, stateUsername, dispatch]);

  // Derived values
  const postLogoutRedirectUri = EnvironmentUtils.postLogoutRedirectUri;
  const isAuthenticated = !!token;
  const username = isAuthenticated ? appUser?.userName || stateUsername || "Guest" : "Not authenticated";

  const buildVersionNumber = uiBuildInfo
    ? `${uiBuildInfo.buildNumber ?? ""}.${uiBuildInfo.buildId ?? ""}`
    : "Local.Dev";

  // Event handlers
  const handleClick = useCallback((_e: React.MouseEvent<HTMLDivElement>) => {}, []);

  const handleLogout = () => {
    if (okta?.oktaAuth) {
      okta?.oktaAuth.signOut({ postLogoutRedirectUri });
      dispatch(clearUserData());
    }
  };

  // Side effects
  useEffect(() => {
    const fetchBuildInfo = async () => {
      try {
        const response = await fetch("/.buildinfo.json");
        const data = await response.json();
        setUiBuildInfo(data);
      } catch (e) {
        console.warn("Error parsing buildinfo.json");
      }
    };

    if (!uiBuildInfo) {
      fetchBuildInfo();
    }
  }, [uiBuildInfo]);

  // Theme setup
  const theme = createTheme(themeOptions);

  return (
    <ThemeProvider theme={theme}>
      <PSLayout
        onClick={handleClick}
        appTitle="Profit Sharing"
        logout={handleLogout}
        buildVersionNumber={buildVersionNumber}
        userName={username}
        environmentMode={EnvironmentUtils.envMode}
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
