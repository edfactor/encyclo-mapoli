import { useOktaAuth } from "@okta/okta-react";
import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import EnvironmentUtils from "../../utils/environmentUtils";

/**
 * Login page that triggers Okta authentication.
 * 
 * In the React Router v7 data router pattern, this component:
 * - Triggers signInWithRedirect() for unauthenticated users
 * - Redirects authenticated users to home page
 * 
 * Note: Token sync is handled by OktaTokenSync in RouteSecurity.tsx,
 * not here. This keeps login logic simple and focused.
 */
const Login = () => {
  const oktaEnabled = EnvironmentUtils.isOktaEnabled;
  const { authState, oktaAuth } = useOktaAuth();
  const navigate = useNavigate();

  useEffect(() => {
    if (!oktaEnabled) {
      // Non-Okta environments redirect to home
      navigate("/", { replace: true });
      return;
    }

    if (authState && !authState.isAuthenticated) {
      // Not authenticated - trigger Okta login
      oktaAuth.signInWithRedirect();
    } else if (authState?.isAuthenticated) {
      // Already authenticated - redirect to home
      navigate("/", { replace: true });
    }
  }, [authState, oktaAuth, oktaEnabled, navigate]);

  // Show loading message while auth state resolves or redirect happens
  return <div>Redirecting to login...</div>;
};

export default Login;
