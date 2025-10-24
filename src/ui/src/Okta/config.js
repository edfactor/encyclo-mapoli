const OKTA_TESTING_DISABLEHTTPSCHECK = import.meta.env.OKTA_TESTING_DISABLEHTTPSCHECK || false;
const BASENAME = import.meta.env.PUBLIC_URL || "";
const REDIRECT_URI = `${window.location.origin}${BASENAME}/login/callback`;

const oktaConfig = (_clientId, _Issuer) => ({
  oidc: {
    clientId: _clientId,
    issuer: _Issuer,
    redirectUri: REDIRECT_URI,
    scopes: ["openid", "profile", "email"],
    pkce: true,
    disableHttpsCheck: OKTA_TESTING_DISABLEHTTPSCHECK
    // Note: restoreOriginalUri is intentionally NOT set here
    // It's provided via the Security component prop to avoid conflicts
  },
  app: {
    basename: BASENAME
  }
});

export default oktaConfig;
