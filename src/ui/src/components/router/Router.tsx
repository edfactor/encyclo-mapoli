import { BrowserRouter, Route } from "react-router-dom";
import RouteSecurity from "./RouteSecurity";
import LandingPage from "./LandingPage";
import { MenuBar } from "smart-ui-library";
import MenuData from "../../MenuData";

import ImpersonationMultiSelect from "components/MenuBar/ImpersonationMultiSelect";
import DemographicBadgesNotInPayprofit from "pages/DemographicBadgesNotInPayprofit/DemographicBadgesNotInPayprofit";
import DuplicateSSNsOnDemographics from "pages/DuplicateSSNsOnDemographics/DuplicateSSNsOnDemographics";
import NegativeEtvaForSSNsOnPayprofit from "pages/NegativeEtvaForSSNsOnPayprofit/NegativeEtvaForSSNsOnPayprofit";
import PayrollDuplicateSSNsOnPayprofit from "pages/PayrollDuplicateSSNsOnPayprofit/PayrollDuplicateSSNsOnPayprofit";
import DuplicateNamesAndBirthdays from "pages/DuplicateNamesAndBirthdays/DuplicateNamesAndBirthdays";
import MissingCommaInPyName from "pages/MissingCommaInPyName/MissingCommaInPyName";

const Router = () => {
  const oktaEnabled = import.meta.env.VITE_REACT_APP_OKTA_ENABLED == "true";
  const isProduction = false;
  const hasImpersonationRole = true;
  const showImpersonation = hasImpersonationRole && !isProduction;

  return (
    <BrowserRouter>
      <MenuBar data={MenuData}>{showImpersonation && <ImpersonationMultiSelect />}</MenuBar>
      <RouteSecurity oktaEnabled={oktaEnabled}>
        <Route
          path=""
          element={<LandingPage />}></Route>
        <Route
          path="demographic-badges-not-in-payprofit"
          element={<DemographicBadgesNotInPayprofit />}></Route>
        <Route
          path="duplicate-ssns-demographics"
          element={<DuplicateSSNsOnDemographics />}></Route>
          <Route
          path="negative-etva-for-ssns-on-payprofit"
          element={<NegativeEtvaForSSNsOnPayprofit />}></Route>
          <Route
          path="payroll-duplicate-ssns-on-payprofit"
          element={<PayrollDuplicateSSNsOnPayprofit />}></Route>
          <Route
          path="duplicate-names-and-birthdays"
          element={<DuplicateNamesAndBirthdays />}></Route>
          <Route
          path="missing-comma-in-py-name"
          element={<MissingCommaInPyName />}></Route>
      </RouteSecurity>
    </BrowserRouter>
  );
};

export default Router;
