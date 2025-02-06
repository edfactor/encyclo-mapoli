import { BrowserRouter, Route } from "react-router-dom";
import RouteSecurity from "./RouteSecurity";
import LandingPage from "./LandingPage";
import { ImpersonationMultiSelect, MenuBar } from "smart-ui-library";
import MenuData from "../../MenuData";
import DemographicBadgesNotInPayprofit from "pages/DemographicBadgesNotInPayprofit/DemographicBadgesNotInPayprofit";
import DuplicateSSNsOnDemographics from "pages/DuplicateSSNsOnDemographics/DuplicateSSNsOnDemographics";
import NegativeEtvaForSSNsOnPayprofit from "pages/NegativeEtvaForSSNsOnPayprofit/NegativeEtvaForSSNsOnPayprofit";
import DuplicateNamesAndBirthdays from "pages/DuplicateNamesAndBirthdays/DuplicateNamesAndBirthdays";
import MissingCommaInPyName from "pages/MissingCommaInPyName/MissingCommaInPyName";
import EmployeesOnMilitaryLeave from "pages/EmployeesOnMilitaryLeave/EmployeesOnMilitaryLeave";
import MilitaryAndRehireForfeitures from "pages/MilitaryAndRehireForfeitures/MilitaryAndRehireForfeitures";
import MilitaryAndRehireProfitSummary from "pages/MilitaryAndRehireProfitSummary/MilitaryAndRehireProfitSummary";
import DistributionsAndForfeitures from "pages/DistributionsAndForfeitures/DistributionAndForfeitures";
import ManageExecutiveHoursAndDollars from "pages/ManageExecutiveHoursAndDollars/ManageExecutiveHoursAndDollars";
import EligibleEmployees from "pages/EligibleEmployees/EligibleEmployees";
import MasterInquiry from "pages/MasterInquiry/MasterInquiry";
import DistributionByAge from "../../pages/DistributionByAge/DistributionByAge";
import ContributionsByAge from "../../pages/ContributionsByAge/ContributionsByAge";
import ForfeituresByAge from "../../pages/ForfeituresByAge/ForfeituresByAge";
import BalanceByAge from "../../pages/BalanceByAge/BalanceByAge";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { setImpersonating } from "reduxstore/slices/securitySlice";
import { ImpersonationRoles } from "reduxstore/types";
import CleanUpSummary from "pages/CleanUpSummary/CleanUpSummary";
import { useEffect } from "react";
import FrozenSummary from "pages/FrozenSummary/FrozenSummary";
import BalanceByYears from "pages/BalanceByYears/BalanceByYears";
import VestedAmountsByAge from "pages/VestedAmountsByAge/VestedAmountsByAge";
import DecemberProcess from "pages/DecemberProcess/DecemberProcess";
import DecemberProcessAccordion from "pages/DecemberProcess/DecemberProcessAccordion";
import DecemberProcessLocalApi from "pages/DecemberProcess/DecemberProcessLOCALAPI";
import Termination from "pages/Termination/Termination";
import MilitaryAndRehireEntryAndModification from "pages/MilitaryAndRehireEntryAndModification/MilitaryAndRehireEntryAndModification";
import ProfitShareReport from "pages/ProfitShareReport/ProfitShareReport";
import ProfitShareUpdate from "../../pages/ProfitShareUpdate/ProfitShareUpdate";
import Forfeit from "pages/Forfeit/Forfeit";
import YearEndFlow from "pages/YearEndFlow/YearEndFlow";
import { Box } from "@mui/material";
import DSMDynamicBreadcrumbs from "components/DSMDynamicBreadcrumbs/DSMDynamicBreadcrumbs";

const Router = () => {
  const oktaEnabled = import.meta.env.VITE_REACT_APP_OKTA_ENABLED == "true";
  const isProduction = false;
  const hasImpersonationRole = true;
  const showImpersonation = hasImpersonationRole && !isProduction;

  const { impersonating } = useSelector((state: RootState) => state.security);
  const dispatch = useDispatch();

  const localStorageImpersonating: string | null = localStorage.getItem("impersonatingRole");

  useEffect(() => {
    if (!!localStorageImpersonating && !impersonating) {
      dispatch(setImpersonating(localStorageImpersonating as ImpersonationRoles));
    }
  }, [impersonating, localStorageImpersonating]);

  return (
    <BrowserRouter>
      <MenuBar
        menuInfo={MenuData}
        impersonationMultiSelect={
          showImpersonation ? (
            <ImpersonationMultiSelect
              impersonationRoles={[
                "Finance-Manager",
                "Distributions-Clerk",
                "Hardship-Administrator",
                "Profit-Sharing-Administrator"
              ]}
              currentRoles={impersonating ? [impersonating] : []}
              setCurrentRoles={(value: string[]) => {
                localStorage.setItem("impersonatingRole", value[0]);
                switch (value[0]) {
                  case ImpersonationRoles.FinanceManager:
                    dispatch(setImpersonating(ImpersonationRoles.FinanceManager));
                    break;
                  case ImpersonationRoles.DistributionsClerk:
                    dispatch(setImpersonating(ImpersonationRoles.DistributionsClerk));
                    break;
                  case ImpersonationRoles.HardshipAdministrator:
                    dispatch(setImpersonating(ImpersonationRoles.HardshipAdministrator));
                    break;
                  case ImpersonationRoles.ProfitSharingAdministrator:
                    dispatch(setImpersonating(ImpersonationRoles.ProfitSharingAdministrator));
                    break;
                  default:
                    localStorage.removeItem("impersonatingRole");
                    dispatch(setImpersonating(null));
                }
              }}
            />
          ) : (
            <></>
          )
        }
      />
      <Box sx={{ position: "relative", paddingTop: "8" }}>
        <Box
          sx={{
            position: "absolute",
            top: 0,
            left: 0,
            right: 0,
            paddingTop: "8px",
            marginLeft: "24px",
            marginRight: "24px"
          }}
        >
          <DSMDynamicBreadcrumbs />
        </Box>
      </Box>
      <RouteSecurity oktaEnabled={oktaEnabled}>
        <Route
          path=""
          element={<DecemberProcessAccordion />}></Route>
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
          path="duplicate-names-and-birthdays"
          element={<DuplicateNamesAndBirthdays />}></Route>
        <Route
          path="missing-comma-in-py-name"
          element={<MissingCommaInPyName />}></Route>
        <Route
          path="employees-on-military-leave"
          element={<EmployeesOnMilitaryLeave />}></Route>
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
          path="manage-executive-hours-and-dollars"
          element={<ManageExecutiveHoursAndDollars />}></Route>
        <Route
          path="eligible-employees"
          element={<EligibleEmployees />}></Route>
        <Route
          path="master-inquiry/:badgeNumber?"
          element={<MasterInquiry />}></Route>
        <Route
          path="distributions-by-age"
          element={<DistributionByAge />}></Route>
        <Route
          path="contributions-by-age"
          element={<ContributionsByAge />}></Route>
        <Route
          path="forfeitures-by-age"
          element={<ForfeituresByAge />}></Route>
        <Route
          path="balance-by-age"
          element={<BalanceByAge />}></Route>
        <Route
          path="clean-up-summary"
          element={<CleanUpSummary />}></Route>
        <Route
          path="frozen-summary"
          element={<FrozenSummary />}></Route>
        <Route
          path="balance-by-years"
          element={<BalanceByYears />}></Route>
        <Route
          path="vested-amounts-by-age"
          element={<VestedAmountsByAge />}></Route>
        <Route
          path="december-process"
          element={<DecemberProcess />}></Route>
        <Route
          path="december-process-local"
          element={<DecemberProcessLocalApi />}></Route>
        <Route
          path="december-process-accordion"
          element={<DecemberProcessAccordion />}></Route>
        <Route
          path="prof-term"
          element={<Termination />}></Route>
        <Route
          path="military-and-rehire-entry"
          element={<MilitaryAndRehireEntryAndModification />}></Route>
        <Route
          path="profit-share-report"
          element={<ProfitShareReport />}></Route>
        <Route
          path="forfeit/:badgeNumber?"
          element={<Forfeit />}></Route>
        <Route
          path="yearend-flow"
          element={<YearEndFlow />}></Route>
        <Route
          path="profit-share-report"
          element={<ProfitShareReport />}></Route>
        <Route
          path="profit-share-update"
          element={<ProfitShareUpdate />}></Route>
      </RouteSecurity>
    </BrowserRouter>
  );
};

export default Router;