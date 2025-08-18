import { BrowserRouter, Route } from "react-router-dom";
import RouterSubAssembly from "./RouterSubAssembly";
import RouteSecurity from "./RouteSecurity";
import EnvironmentUtils from "../../utils/environmentUtils";

// We need a SubAssembly to let navigable items be put into
// the drawer component. Otherwise, React thinks that the
// navigate calls inside are not in a Router.
const Router = () => {
  const oktaEnabled = EnvironmentUtils.isOktaEnabled;

  return (
    <BrowserRouter
      future={{
        v7_startTransition: true,
        v7_relativeSplatPath: true
      }}>
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
