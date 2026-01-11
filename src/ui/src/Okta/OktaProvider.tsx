import { OktaAuth } from "@okta/okta-auth-js";
import { createContext, ReactNode, useContext, useEffect, useMemo, useState } from "react";
import oktaConfig from "./config";

/* eslint-disable react-refresh/only-export-components */

const clientId = import.meta.env.VITE_REACT_APP_OKTA_CLIENT_ID;
const issuer = import.meta.env.VITE_REACT_APP_OKTA_ISSUER;

/**
 * Context value for shared OktaAuth instance.
 * 
 * This provides a single, stable OktaAuth instance across the entire application,
 * preventing the dual-instance issue where App.tsx and RouteSecurity.tsx had
 * separate OktaAuth objects with inconsistent auth state.
 */
interface OktaContextValue {
  oktaAuth: OktaAuth | null;
  isInitialized: boolean;
}

const OktaContext = createContext<OktaContextValue>({ oktaAuth: null, isInitialized: false });

/**
 * Hook to access the shared OktaAuth instance.
 * 
 * Use this instead of creating new OktaAuth instances directly.
 * The oktaAuth will be null until initialization completes.
 */
export const useOktaInstance = (): OktaContextValue => useContext(OktaContext);

/**
 * Provider component for shared OktaAuth instance.
 * 
 * CRITICAL: This must be rendered at the app root level, ABOVE both
 * the Router and any components that need access to OktaAuth for logout.
 * 
 * Features:
 * - Creates single OktaAuth instance via useMemo (stable reference)
 * - Starts OktaAuth service for token auto-renewal
 * - Provides isInitialized flag to prevent premature renders
 * - Cleans up OktaAuth service on unmount
 */
export const OktaProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [isInitialized, setIsInitialized] = useState(false);

  // Create stable OktaAuth instance
  const oktaAuth = useMemo(() => {
    const config = oktaConfig(clientId, issuer);
    return new OktaAuth({
      ...config.oidc,
      services: {
        autoRenew: true,
        autoRemove: true,
        syncStorage: true
      }
    });
  }, []);

  // Start OktaAuth service and mark as initialized
  useEffect(() => {
    oktaAuth.start();
    setIsInitialized(true);

    return () => {
      oktaAuth.stop();
    };
  }, [oktaAuth]);

  const value = useMemo(() => ({ oktaAuth, isInitialized }), [oktaAuth, isInitialized]);

  return <OktaContext.Provider value={value}>{children}</OktaContext.Provider>;
};
