import { OktaAuth, toRelativeUrl } from "@okta/okta-auth-js";
import { Security, useOktaAuth } from "@okta/okta-react";
import { ComponentType, ReactNode, useEffect, useLayoutEffect, useRef } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useOktaInstance } from "../../Okta/OktaProvider";
import { setImpersonating, setToken } from "../../reduxstore/slices/securitySlice";
import type { RootState } from "../../reduxstore/store";
import { ImpersonationRoles } from "../../reduxstore/types";
import EnvironmentUtils from "../../utils/environmentUtils";

/* eslint-disable react-refresh/only-export-components */

const ImpersonatingRolesStorageKey = "impersonatingRoles";

/**
 * Component that monitors Okta auth state and syncs token to Redux.
 * 
 * MUST be inside Security component to use useOktaAuth hook.
 * When auth state changes to authenticated, dispatches token to Redux store.
 * 
 * IMPORTANT: This component ONLY syncs tokens. It does NOT trigger login redirects.
 * Login redirects are handled by the LoaderErrorBoundary which shows the auth pending
 * UI and waits for the Security component to complete authentication.
 */
/**
 * Component that monitors Okta auth state and syncs token AND impersonation roles to Redux.
 * 
 * MUST be inside Security component to use useOktaAuth hook.
 * When auth state changes to authenticated, dispatches token to Redux store.
 * Also loads impersonation roles from localStorage (dev/qa only).
 * 
 * CRITICAL FIX: Uses useLayoutEffect to dispatch token SYNCHRONOUSLY before
 * the browser paints. This ensures the token AND impersonation roles are in Redux 
 * before AuthGate renders children (which creates the Router and runs loaders).
 * 
 * CRITICAL FIX: Uses a ref to track the last dispatched token to prevent
 * infinite re-render loops.
 */
const OktaTokenSync: React.FC = () => {
  const { authState } = useOktaAuth();
  const dispatch = useDispatch();
  const lastDispatchedTokenRef = useRef<string | null>(null);
  const hasLoadedImpersonationRef = useRef(false);

  const currentToken = authState?.accessToken?.accessToken;

  // Use useLayoutEffect for SYNCHRONOUS dispatch before paint
  useLayoutEffect(() => {
    if (currentToken && currentToken !== lastDispatchedTokenRef.current) {
      lastDispatchedTokenRef.current = currentToken;
      dispatch(setToken(currentToken));

      // Also load impersonation roles from localStorage (dev/qa only)
      // This must happen BEFORE the Router renders and runs loaders
      if (!hasLoadedImpersonationRef.current && EnvironmentUtils.isDevelopmentOrQA) {
        hasLoadedImpersonationRef.current = true;
        try {
          const raw = localStorage.getItem(ImpersonatingRolesStorageKey);
          if (raw) {
            const parsed = JSON.parse(raw) as unknown;
            if (Array.isArray(parsed)) {
              const allowedRoleValues = new Set<string>(Object.values(ImpersonationRoles));
              const persistedRoles = parsed.filter(
                (x): x is ImpersonationRoles => typeof x === "string" && allowedRoleValues.has(x)
              );
              if (persistedRoles.length > 0) {
                dispatch(setImpersonating(persistedRoles));
              }
            }
          }
        } catch {
          // Ignore localStorage parse errors
        }
      }
    }
  }, [currentToken, dispatch]);

  return null;
};

/**
 * Component that gates access to the app by requiring authentication.
 * 
 * MUST be inside Security component to use useOktaAuth hook.
 * 
 * Flow:
 * 1. Wait for auth state to resolve (not null, not pending)
 * 2. If authenticated: render children (the Router)
 * 3. If NOT authenticated: trigger ONE redirect to Okta login
 * 
 * CRITICAL: This component BLOCKS rendering of children until auth is resolved.
 * This prevents the Router from being created (and running loaders) before
 * we know the auth state.
 * 
 * CRITICAL: Uses a ref to ensure we only redirect ONCE per mount.
 * This prevents infinite redirect loops during callback processing.
 */

const AuthGate: React.FC<{ children: ReactNode }> = ({ children }) => {
  const { authState, oktaAuth } = useOktaAuth();
  const hasRedirectedRef = useRef(false);
  const reduxToken = useSelector((state: RootState) => state.security.token);

  // Check if we're on an auth route (login callback, etc.)
  const currentPath = window.location.pathname;
  const isAuthRoute = currentPath.startsWith("/login");

  // For auth routes (like /login/callback), ALWAYS render children immediately
  // The callback component needs to render to process the OAuth response
  if (isAuthRoute) {
    return <>{children}</>;
  }

  // Wait for auth state to resolve before doing anything
  if (!authState || authState.isPending) {
    return <div>Checking authentication...</div>;
  }

  // If authenticated AND token is in Redux, render children (the Router)
  // CRITICAL: We must wait for BOTH conditions to prevent the loader race condition
  if (authState.isAuthenticated && reduxToken) {
    return <>{children}</>;
  }

  // Authenticated but token not yet in Redux - wait for OktaTokenSync
  if (authState.isAuthenticated && !reduxToken) {
    return <div>Syncing authentication...</div>;
  }

  // NOT authenticated - trigger redirect (once only)
  if (!hasRedirectedRef.current) {
    hasRedirectedRef.current = true;
    // Use setTimeout to avoid calling during render
    setTimeout(() => {
      oktaAuth.signInWithRedirect({ originalUri: window.location.href });
    }, 0);
  }

  // Show message while redirect happens (children are NOT rendered)
  return <div>Redirecting to login...</div>;
};

/**
 * Inner component that wraps router with Okta Security component.
 * 
 * Uses the SHARED OktaAuth instance from OktaProvider instead of
 * creating a new instance. This prevents the dual-instance issue
 * where App.tsx and RouteSecurity.tsx had separate auth state.
 */
const OktaSecurityWrapper: React.FC<{ children: ReactNode }> = ({ children }) => {
  const { oktaAuth, isInitialized } = useOktaInstance();

  /**
   * Callback invoked by Okta LoginCallback after successful authentication.
   * 
   * CRITICAL: This callback MUST navigate to the original URI.
   * We use window.location.replace() because:
   * 1. We're outside of Router context here (can't use useNavigate)
   * 2. The LoginCallback route is top-level, so RestoreOriginalUriHandler isn't rendered
   * 3. replace() ensures the callback URL isn't in browser history
   */
  const restoreOriginalUri = async (_oktaAuth: OktaAuth, originalUri: string) => {
    // Navigate to the original URI (or home if none provided)
    const targetUri = originalUri || "/";
    const relativeUri = toRelativeUrl(targetUri, window.location.origin);
    
    // Use replace to avoid the callback URL staying in browser history
    window.location.replace(relativeUri);
  };

  // Wait for OktaAuth to be initialized before rendering
  if (!isInitialized || oktaAuth === null) {
    return <div>Loading authentication...</div>;
  }

  return (
    <Security oktaAuth={oktaAuth} restoreOriginalUri={restoreOriginalUri}>
      <OktaTokenSync />
      <AuthGate>{children}</AuthGate>
    </Security>
  );
};

/**
 * Higher-order component that wraps Router with Okta Security.
 * 
 * Refactored for React Router v7 data router pattern.
 * 
 * Behavior:
 * - If Okta enabled: wraps Router with Security component and token sync
 * - If Okta disabled: initializes dev token and renders Router directly
 * 
 * IMPORTANT: Uses shared OktaAuth instance from OktaProvider context.
 * The OktaProvider MUST wrap this component in the component tree.
 */
export function withOktaSecurity<P extends object>(RouterComponent: ComponentType<P>): ComponentType<P> {
  return (props: P) => {
    const oktaEnabled = EnvironmentUtils.isOktaEnabled;
    const dispatch = useDispatch();

    // Initialize token for non-Okta environments
    useEffect(() => {
      if (!oktaEnabled) {
        // In non-Okta environments (development), set a dummy token to allow API calls
        dispatch(setToken("dev-token"));
      }
    }, [oktaEnabled, dispatch]);

    // If Okta is not enabled, just render Router directly
    if (!oktaEnabled) {
      return <RouterComponent {...props} />;
    }

    // Wrap Router with Okta Security
    return (
      <OktaSecurityWrapper>
        <RouterComponent {...props} />
      </OktaSecurityWrapper>
    );
  };
}

