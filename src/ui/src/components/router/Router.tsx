import { Box } from "@mui/material";
import DSMDynamicBreadcrumbs from "components/DSMDynamicBreadcrumbs/DSMDynamicBreadcrumbs";
import BalanceByYears from "pages/PROF130/BalanceByYears/BalanceByYears";
import CleanUpSummary from "pages/CleanUpSummary/CleanUpSummary";
import DecemberProcessAccordion from "pages/DecemberActivities/DecemberProcess/DecemberProcessAccordion";
import DemographicBadgesNotInPayprofit from "pages/DecemberActivities/DemographicBadgesNotInPayprofit/DemographicBadgesNotInPayprofit";
import DistributionsAndForfeitures from "pages/DecemberActivities/DistributionsAndForfeitures/DistributionAndForfeitures";
import DuplicateNamesAndBirthdays from "pages/DecemberActivities/DuplicateNamesAndBirthdays/DuplicateNamesAndBirthdays";
import DuplicateSSNsOnDemographics from "pages/DecemberActivities/DuplicateSSNsOnDemographics/DuplicateSSNsOnDemographics";
import EligibleEmployees from "pages/EligibleEmployees/EligibleEmployees";
import EmployeesOnMilitaryLeave from "pages/DecemberActivities/EmployeesOnMilitaryLeave/EmployeesOnMilitaryLeave";
import ProfitShareReportEditRun from "pages/FiscalFlow/ProfitShareReportEditRun/ProfitShareReportEditRun";
import ProfitShareReportFinalRun from "pages/FiscalFlow/ProfitShareReportFinalRun/ProfitShareReportFinalRun";
import Forfeit from "pages/Forfeit/Forfeit";
import FrozenSummary from "pages/FrozenSummary/FrozenSummary";
import ManageExecutiveHoursAndDollars from "pages/DecemberActivities/ManageExecutiveHoursAndDollars/ManageExecutiveHoursAndDollars";
import MasterInquiry from "pages/MasterInquiry/MasterInquiry";
import RehireForfeitures from "pages/RehireForfeitures/RehireForfeitures";
import MissingCommaInPyName from "pages/DecemberActivities/MissingCommaInPyName/MissingCommaInPyName";
import NegativeEtvaForSSNsOnPayprofit from "pages/DecemberActivities/NegativeEtvaForSSNsOnPayprofit/NegativeEtvaForSSNsOnPayprofit";
import EighteenToTwenty from "pages/PAY426Reports/PAY426-1/EighteenToTwenty";
import Beneficiaries from "pages/PAY426Reports/PAY426-10/Beneficiaries";
import TwentyOnePlus from "pages/PAY426Reports/PAY426-2/TwentyOnePlus";
import UnderEighteen from "pages/PAY426Reports/PAY426-3/UnderEighteen";
import PriorHours from "pages/PAY426Reports/PAY426-4/PriorHours";
import NoPriorHours from "pages/PAY426Reports/PAY426-5/NoPriorHours";
import TermedWithHours from "pages/PAY426Reports/PAY426-6/TermedWithHours";
import TermedNoPrior from "pages/PAY426Reports/PAY426-7/TermedNoPrior";
import TermedWithPrior from "pages/PAY426Reports/PAY426-8/TermedWithPrior";
import ProfitSummary from "pages/PAY426Reports/PAY426-9/ProfitSummary";
import Pay450Summary from "pages/PaymasterUpdate/Pay450Summary";
import PaymasterUpdate from "pages/PaymasterUpdate/PaymasterUpdate";
import ProfCtrlSheet from "pages/PaymasterUpdate/ProfCtrlSheet";
import Profall from "pages/Profall/Profall";
import NewPSLabels from "pages/ProfitShareByStore/NewPSLabels";
import ProfitShareByStore from "pages/ProfitShareByStore/ProfitShareByStore";
import QPAY066TA from "pages/ProfitShareByStore/BreakdownReport/QPAY066TA";
import Under21Report from "pages/ProfitShareByStore/Under21Report";
import Under21TA from "pages/ProfitShareByStore/Under21TA";
import ProfitShareGrossReport from "pages/ProfitShareGrossReport/ProfitShareGrossReport";
import ProfitShareReport from "pages/ProfitShareReport/ProfitShareReport";
import Termination from "pages/DecemberActivities/Termination/Termination";
import VestedAmountsByAge from "pages/PROF130/VestedAmountsByAge/VestedAmountsByAge";
import FiscalFlow from "pages/YearEndFlow/YearEndFlow";
import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { BrowserRouter, Route } from "react-router-dom";
import { setImpersonating } from "reduxstore/slices/securitySlice";
import { RootState } from "reduxstore/store";
import { ImpersonationRoles } from "reduxstore/types";
import { ImpersonationMultiSelect, MenuBar } from "smart-ui-library";
import { ROUTES } from "../../constants";
import MenuData from "../../MenuData";
import BalanceByAge from "../../pages/PROF130/BalanceByAge/BalanceByAge";
import ContributionsByAge from "../../pages/PROF130/ContributionsByAge/ContributionsByAge";
import DistributionByAge from "../../pages/PROF130/DistributionByAge/DistributionByAge";
import ForfeituresByAge from "../../pages/PROF130/ForfeituresByAge/ForfeituresByAge";
import ProfitShareUpdate from "../../pages/ProfitShareUpdate/ProfitShareUpdate";
import YTDWages from "../../pages/YTDWagesExtract/YTDWages";
import RouteSecurity from "./RouteSecurity";
import MilitaryEntryAndModification from "pages/MilitaryEntryAndModification/MilitaryEntryAndModification";
import DemographicFreeze from "../../pages/ITOperations/DemographicFreeze/DemographicFreeze";

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
  }, [dispatch, impersonating, localStorageImpersonating]);

  return (
    <BrowserRouter>
      <MenuBar
        menuInfo={MenuData}
        impersonationMultiSelect={
          showImpersonation ? (
            <ImpersonationMultiSelect
              impersonationRoles={[
                ImpersonationRoles.FinanceManager,
                ImpersonationRoles.DistributionsClerk,
                ImpersonationRoles.HardshipAdministrator,
                ImpersonationRoles.ProfitSharingAdministrator,
                ImpersonationRoles.ItOperations
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
                  case ImpersonationRoles.ItOperations:
                    dispatch(setImpersonating(ImpersonationRoles.ItOperations));
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
          }}>
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
          path={ROUTES.REHIRE_FORFEITURES}
          element={<RehireForfeitures />}></Route>
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
          path={ROUTES.YTD_WAGES_EXTRACT}
          element={<YTDWages />}></Route>
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
          path={ROUTES.MILITARY_ENTRY_AND_MODIFICATION}
          element={<MilitaryEntryAndModification />}></Route>
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
        <Route
          path={ROUTES.PROFIT_SHARE_REPORT_FINAL_RUN}
          element={<ProfitShareReportFinalRun />}></Route>
        <Route
          path={ROUTES.PROFIT_SHARE_UPDATE}
          element={<ProfitShareUpdate />}></Route>
        <Route
          path={ROUTES.PAY426_ACTIVE_18_20}
          element={<EighteenToTwenty />}></Route>
        <Route
          path=""
          element={<DecemberProcessAccordion />}></Route>
        <Route
          path={ROUTES.PROFIT_SHARE_BY_STORE}
          element={<ProfitShareByStore />}></Route>
        <Route
          path={ROUTES.PROFIT_SHARE_GROSS_REPORT}
          element={<ProfitShareGrossReport />}></Route>
        <Route
          path={ROUTES.PAYMASTER_UPDATE}
          element={<PaymasterUpdate />}></Route>
        <Route
          path={ROUTES.PAY450_SUMMARY}
          element={<Pay450Summary />}></Route>
        <Route
          path={ROUTES.PROF_CTRLSHEET}
          element={<ProfCtrlSheet />}></Route>
        <Route
          path={ROUTES.PROFIT_SHARE_BY_STORE}
          element={<ProfitShareByStore />}></Route>
        <Route
          path={ROUTES.UNDER_21_REPORT}
          element={<Under21Report />}>
          {" "}
        </Route>
        <Route
          path={ROUTES.PAY426_ACTIVE_21_PLUS}
          element={<TwentyOnePlus />}></Route>
        <Route
          path={ROUTES.PAY426_ACTIVE_18_20}
          element={<EighteenToTwenty />}
        />
        <Route
          path={ROUTES.PAY426_ACTIVE_21_PLUS}
          element={<TwentyOnePlus />}
        />
        <Route
          path={ROUTES.PAY426_ACTIVE_UNDER_18}
          element={<UnderEighteen />}
        />
        <Route
          path={ROUTES.PAY426_ACTIVE_PRIOR_SHARING}
          element={<PriorHours />}
        />
        <Route
          path={ROUTES.PAY426_ACTIVE_NO_PRIOR}
          element={<NoPriorHours />}
        />
        <Route
          path={ROUTES.PAY426_TERMINATED_1000_PLUS}
          element={<TermedWithHours />}
        />
        <Route
          path={ROUTES.PAY426_TERMINATED_NO_PRIOR}
          element={<TermedNoPrior />}
        />
        <Route
          path={ROUTES.PAY426_TERMINATED_PRIOR}
          element={<TermedWithPrior />}
        />
        <Route
          path={ROUTES.PAY426_SUMMARY}
          element={<ProfitSummary />}
        />
        <Route
          path={ROUTES.PAY426_NON_EMPLOYEE}
          element={<Beneficiaries />}
        />
        <Route
          path={ROUTES.PROFIT_SHARE_BY_STORE}
          element={<ProfitShareByStore />}
        />
        <Route
          path={ROUTES.QPAY066_UNDER21}
          element={<Under21Report />}
        />
        <Route
          path={ROUTES.QPAY066TA_UNDER21}
          element={<Under21TA />}
        />
        <Route
          path={ROUTES.QPAY066TA}
          element={<QPAY066TA />}
        />
        <Route
          path={ROUTES.NEW_PS_LABELS}
          element={<NewPSLabels />}
        />
        <Route
          path={ROUTES.PROFALL}
          element={<Profall />}
        />
        <Route
          path={ROUTES.DEMO_FREEZE}
          element={<DemographicFreeze />}
        />
      </RouteSecurity>
    </BrowserRouter>
  );
};

export default Router;
