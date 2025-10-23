import { useOktaAuth } from "@okta/okta-react";
import { useEffect } from "react";
import { useDispatch } from "react-redux";

import { setToken } from "reduxstore/slices/securitySlice";
import EnvironmentUtils from "../../utils/environmentUtils";
import RouterSubAssembly from "../router/RouterSubAssembly";

const Login = () => {
  const oktaEnabled = EnvironmentUtils.isOktaEnabled;
  const { authState, oktaAuth } = useOktaAuth();
  const dispatch = useDispatch();

  // FIXME: Why are these here? They are unused
  //const [skipRole, setSkipRole] = useState<boolean>(true);
  //const [skipPermission, setSkipPermission] = useState<boolean>(true);
  //const [skipUsername, setSkipUsername] = useState<boolean>(true);

  useEffect(() => {
    const login = async () => {
      oktaAuth.signInWithRedirect();
    };

    if (oktaEnabled) {
      if (authState && !authState.isAuthenticated) {
        login();
      }

      if (authState && authState.isAuthenticated) {
        const accessToken = oktaAuth.getAccessToken();
        if (accessToken) {
          dispatch(setToken(accessToken));
          //(false);
          //setSkipPermission(false);
          //setSkipUsername(false);
        }
      }
    }
  }, [authState, dispatch, oktaAuth, oktaEnabled]);

  return <RouterSubAssembly />;
};

export default Login;
