import { useOktaAuth } from "@okta/okta-react";
import { useEffect, useState } from "react";
import { useDispatch } from "react-redux";

import { Outlet } from "react-router";
import { setToken } from "reduxstore/slices/securitySlice";

const Login = () => {
  const oktaEnabled = import.meta.env.VITE_REACT_APP_OKTA_ENABLED === "true";
  const { authState, oktaAuth } = useOktaAuth();
  const postLogoutRedirectUri = "https://marketbasket.okta.com/login/default";
  const dispatch = useDispatch();
  const [skipRole, setSkipRole] = useState<boolean>(true);
  const [skipPermission, setSkipPermission] = useState<boolean>(true);
  const [skipUsername, setSkipUsername] = useState<boolean>(true);

  useEffect(() => {
    const login = async () => oktaAuth.signInWithRedirect();
    if (oktaEnabled) {
      if (authState && !authState.isAuthenticated) {
        login();
      }

      if (authState && authState.isAuthenticated) {
        const accessToken = oktaAuth.getAccessToken();
        if (accessToken) {
          dispatch(setToken(accessToken));
          setSkipRole(false);
          setSkipPermission(false);
          setSkipUsername(false);
        }
      }
    }
  }, [authState, oktaAuth]);
  /*
  useEffect(() => {
    const logout = async () => oktaAuth.signOut({ postLogoutRedirectUri });
    if (performLogout) {
      logout();
      dispatch(setToken(""));
      dispatch(setPerformLogout(false));
    }
  }, [performLogout]);

  useEffect(() => {
    if (userRoles) {
      setSkipRole(true);
      dispatch(setUserRoles(userRoles.roles));
    }
  }, [userRoles]);

  useEffect(() => {
    if (userPermissions) {
      setSkipPermission(true);
      dispatch(setUserPermissions(userPermissions.permissions));
    }
  }, [userPermissions]);

  useEffect(() => {
    if (user) {
      setSkipUsername(true);
      dispatch(setUsername(user.userName));
    }
  }, [user]);

*/

  return <Outlet></Outlet>;
};

export default Login;
