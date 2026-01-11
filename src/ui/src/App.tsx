import { Description, Settings } from "@mui/icons-material";
import { createTheme, ThemeProvider } from "@mui/material";
import AppErrorBoundary from "components/ErrorBoundary";
import PSLayout from "components/Layout/PSLayout";
import { useCallback, useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetMissivesQuery } from "reduxstore/api/LookupsApi";
import { colors, themeOptions, ToastServiceProvider } from "smart-ui-library";
import "smart-ui-library/dist/smart-ui-library.css";
import { initializeAgGrid } from "../agGridConfig";
import Router from "./components/router/Router";
import { OktaProvider, useOktaInstance } from "./Okta/OktaProvider";
import { useLazyGetHealthQuery } from "./reduxstore/api/AppSupportApi";
import { useGetAppVersionQuery } from "./reduxstore/api/CommonApi";
import { clearUserData, setUserGroups, setUsername } from "./reduxstore/slices/securitySlice";
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

/**
 * Inner App component that uses the shared OktaAuth instance from OktaProvider.
 * This ensures a single OktaAuth instance is used throughout the application.
 */
const AppContent = () => {
  // State management
  const dispatch = useDispatch();

  useGetAppVersionQuery();
  const [uiBuildInfo, setUiBuildInfo] = useState<BuildInfo | null>(null);
  const [buildInfoText, setBuildInfoText] = useState("");
  const { buildNumber } = useSelector((state: RootState) => state.common);
  const [loadMissives] = useLazyGetMissivesQuery();
  const [triggerHealth] = useLazyGetHealthQuery();

  // Get shared OktaAuth instance from provider
  const { oktaAuth } = useOktaInstance();

  const health = useSelector((state: RootState) => state.support.health);

  // Initialize ag-grid modules (core immediately, advanced deferred to idle time)
  useEffect(() => {
    initializeAgGrid();
  }, []);

  // Redux selectors
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

        // Defer missives load to idle time (not critical for initial render)
        if ("requestIdleCallback" in window) {
          const handle = requestIdleCallback(() => {
            loadMissives();
          });
          return () => cancelIdleCallback(handle);
        } else {
          // Fallback for browsers without requestIdleCallback
          const timer = setTimeout(() => loadMissives(), 2000);
          return () => clearTimeout(timer);
        }
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
      } catch (_e) {
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

  // Defer health check to idle time (not critical for initial render)
  useEffect(() => {
    if ("requestIdleCallback" in window) {
      const handle = requestIdleCallback(
        () => {
          triggerHealth();
        },
        { timeout: 3000 }
      );
      return () => cancelIdleCallback(handle);
    } else {
      // Fallback for browsers without requestIdleCallback
      const timer = setTimeout(() => triggerHealth(), 3000);
      return () => clearTimeout(timer);
    }
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

/**
 * Root App component that wraps with OktaProvider for shared OktaAuth instance.
 *
 * CRITICAL: OktaProvider must be at the top level to ensure:
 * 1. Single OktaAuth instance shared across App and Router/Security
 * 2. OktaAuth is initialized before any components need it
 * 3. Logout functionality in PSLayout works with the same instance
 */
const App = () => {
  // Only wrap with OktaProvider when Okta is enabled
  if (EnvironmentUtils.isOktaEnabled) {
    return (
      <OktaProvider>
        <AppContent />
      </OktaProvider>
    );
  }

  // Non-Okta environments render directly
  return <AppContent />;
};

export default App;
