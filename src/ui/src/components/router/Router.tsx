import { BrowserRouter, Route } from "react-router-dom";
import EnvironmentUtils from "../../utils/environmentUtils";
import RouterSubAssembly from "./RouterSubAssembly";
import RouteSecurity from "./RouteSecurity";

// We need a SubAssembly to let navigable items be put into
// the drawer component. Otherwise, React thinks that the
// navigate calls inside are not in a Router.
const Router = () => {
  const oktaEnabled = EnvironmentUtils.isOktaEnabled;

  return (
    <BrowserRouter>
      <RouteSecurity oktaEnabled={oktaEnabled}>
        <Route
          path="/*"
          element={<RouterSubAssembly />}
        />
      </RouteSecurity>
    </BrowserRouter>
  );
};

export default Router;
