import { BrowserRouter, Route } from "react-router-dom";
import RouteSecurity from "./RouteSecurity";
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
import { Box } from "@mui/material";
import DSMDynamicBreadcrumbs from "components/DSMDynamicBreadcrumbs/DSMDynamicBreadcrumbs";
import { ROUTES } from "../../constants";
import FiscalFlow from "pages/YearEndFlow/YearEndFlow";
import ProfitShareReportEditRun from "pages/FiscalFlow/ProfitShareReportEditRun/ProfitShareReportEditRun";
import EighteenToTwenty from "pages/PAY426Reports/PAY426-1/EighteenToTwenty";
import ProfitShareReportFinalRun from "pages/FiscalFlow/ProfitShareReportFinalRun/ProfitShareReportFinalRun";

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
      <Box sx={{ position: "relative", paddingTop: "32px" }}>
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
          path={ROUTES.DEMOGRAPHIC_BADGES}

          element={<DemographicBadgesNotInPayprofit />}></Route>
        <Route
          path={ROUTES.DUPLICATE_SSNS}
          element={<DuplicateSSNsOnDemographics />}></Route>
        <Route
          path={ROUTES.NEGATIVE_ETVA}
          element={<NegativeEtvaForSSNsOnPayprofit />}></Route>
        <Route
          path={ROUTES.DUPLICATE_NAMES}
          element={<DuplicateNamesAndBirthdays />}></Route>
        <Route
          path={ROUTES.MISSING_COMMA}
          element={<MissingCommaInPyName />}></Route>
        <Route
          path={ROUTES.EMPLOYEES_MILITARY}
          element={<EmployeesOnMilitaryLeave />}></Route>
        <Route
          path={ROUTES.MILITARY_FORFEITURES}
          element={<MilitaryAndRehireForfeitures />}></Route>
        <Route
          path={ROUTES.MILITARY_PROFIT_SUMMARY}
          element={<MilitaryAndRehireProfitSummary />}></Route>
        <Route
          path={ROUTES.DISTRIBUTIONS_AND_FORFEITURES}
          element={<DistributionsAndForfeitures />}></Route>
        <Route
          path={ROUTES.MANAGE_EXECUTIVE_HOURS}
          element={<ManageExecutiveHoursAndDollars />}></Route>
        <Route
          path={ROUTES.ELIGIBLE_EMPLOYEES}
          element={<EligibleEmployees />}></Route>
        <Route
          path={`${ROUTES.MASTER_INQUIRY}/:badgeNumber?`}
          element={<MasterInquiry />}></Route>
        <Route
          path={ROUTES.DISTRIBUTIONS_BY_AGE}
          element={<DistributionByAge />}></Route>
        <Route
          path={ROUTES.CONTRIBUTIONS_BY_AGE}
          element={<ContributionsByAge />}></Route>
        <Route
          path={ROUTES.FORFEITURES_BY_AGE}
          element={<ForfeituresByAge />}></Route>
        <Route
          path={ROUTES.BALANCE_BY_AGE}
          element={<BalanceByAge />}></Route>
        <Route
          path="clean-up-summary"
          element={<CleanUpSummary />}></Route>
        <Route
          path={ROUTES.FROZEN_SUMMARY}
          element={<FrozenSummary />}></Route>
        <Route
          path={ROUTES.BALANCE_BY_YEARS}
          element={<BalanceByYears />}></Route>
        <Route
          path={ROUTES.VESTED_AMOUNTS_BY_AGE}
          element={<VestedAmountsByAge />}></Route>
        <Route
          path={ROUTES.DECEMBER_PROCESS_ACCORDION}
          element={<DecemberProcessAccordion />}></Route>
        <Route
          path={ROUTES.PROF_TERM}
          element={<Termination />}></Route>
        <Route
          path={ROUTES.MILITARY_AND_REHIRE_ENTRY}
          element={<MilitaryAndRehireEntryAndModification />}></Route>
        <Route
          path={ROUTES.PROFIT_SHARE_REPORT}
          element={<ProfitShareReport />}></Route>
        <Route
          path="forfeit/:badgeNumber?"
          element={<Forfeit />}></Route>
        <Route
          path={ROUTES.FISCAL_CLOSE}
          element={<FiscalFlow />}></Route>
        <Route
          path={ROUTES.PROFIT_SHARE_REPORT}
          element={<ProfitShareReport />}></Route>
        <Route
          path={ROUTES.PROFIT_SHARE_REPORT_EDIT_RUN}
          element={<ProfitShareReportEditRun />}></Route>
        <Route path={ROUTES.PROFIT_SHARE_REPORT_FINAL_RUN} element={<ProfitShareReportFinalRun />}></Route>
        <Route
          path={ROUTES.PROFIT_SHARE_UPDATE}
          element={<ProfitShareUpdate />}></Route>
        <Route path={ROUTES.PAY426_ACTIVE_18_20} element={<EighteenToTwenty />}></Route>
        <Route
          path=""
          element={<DecemberProcessAccordion />}></Route>
      </RouteSecurity>
    </BrowserRouter>
  );
};

export default Router;