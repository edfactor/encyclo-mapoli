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
import MilitaryAndRehire from "pages/MilitaryAndRehire/MilitaryAndRehire";
import MilitaryAndRehireForfeitures from "pages/MilitaryAndRehireForfeitures/MilitaryAndRehireForfeitures";
import MilitaryAndRehireProfitSummary from "pages/MilitaryAndRehireProfitSummary/MilitaryAndRehireProfitSummary";
import DistributionsAndForfeitures from "pages/DistributionsAndForfeitures/DistributionAndForfeitures";
import ManageExecutiveHoursAndDollars from "pages/ManageExecutiveHoursAndDollars/ManageExecutiveHoursAndDollars";
import EligibleEmployees from "pages/EligibleEmployees/EligibleEmployees";

const Router = () => {
  const oktaEnabled = import.meta.env.VITE_REACT_APP_OKTA_ENABLED == "true";
  const isProduction = false;
  const hasImpersonationRole = true;
  const showImpersonation = hasImpersonationRole && !isProduction;

  return (
    <BrowserRouter>
      <MenuBar
        menuInfo={MenuData}
        impersonationMultiSelect={showImpersonation && <ImpersonationMultiSelect />}
      />
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
        <Route
          path="military-and-rehire"
          element={<MilitaryAndRehire />}></Route>
        <Route
          path="military-and-rehire-forfeitures"
          element={<MilitaryAndRehireForfeitures />}></Route>
        <Route
          path="military-and-rehire-profit-summary"
          element={<MilitaryAndRehireProfitSummary />}></Route>
        <Route
          path="distributions-and-forfeitures"
          element={<DistributionsAndForfeitures />}></Route>
          <Route
          path="manage-executive-hours-and-dolars"
          element={<ManageExecutiveHoursAndDollars />}></Route>
          <Route
          path="eligible-employees"
          element={<EligibleEmployees />}></Route>
      </RouteSecurity>
    </BrowserRouter>
  );
};

export default Router;
