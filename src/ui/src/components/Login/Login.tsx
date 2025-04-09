import { useOktaAuth } from "@okta/okta-react";
import { useEffect, useState } from "react";
import { useDispatch } from "react-redux";

import { Outlet } from "react-router";
import { setToken } from "reduxstore/slices/securitySlice";
import EnvironmentUtils from "../../utils/environmentUtils";

const Login = () => {
  const oktaEnabled = EnvironmentUtils.isOktaEnabled;
  const { authState, oktaAuth } = useOktaAuth();
  const postLogoutRedirectUri = EnvironmentUtils.postLogoutRedirectUri;
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
  }, [authState, dispatch, oktaAuth, oktaEnabled]);
  
  return <Outlet></Outlet>;
};

export default Login;
