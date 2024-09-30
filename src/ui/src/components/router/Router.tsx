import { BrowserRouter, Route } from "react-router-dom";
import RouteSecurity from "./RouteSecurity";
import LandingPage from "./LandingPage";
import { MenuBar } from "smart-ui-library";
import MenuData from "../../MenuData";

import ImpersonationMultiSelect from "components/MenuBar/ImpersonationMultiSelect";

const Router = () => {
  const oktaEnabled = import.meta.env.VITE_REACT_APP_OKTA_ENABLED == 'true';
  const isProduction = false;
  const hasImpersonationRole = true;
  const showImpersonation = hasImpersonationRole && !isProduction;

  return (
    <BrowserRouter>
      <MenuBar data={MenuData}>{showImpersonation && <ImpersonationMultiSelect />}</MenuBar>
      <RouteSecurity oktaEnabled={oktaEnabled}>
        <Route
          path=""
          index={true}
          element={<LandingPage />}></Route>
      </RouteSecurity>
    </BrowserRouter>
  );
};

export default Router;
