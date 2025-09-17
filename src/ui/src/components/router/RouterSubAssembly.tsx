import { Box } from "@mui/material";
import PSDrawer from "components/Drawer/PSDrawer";
import DSMDynamicBreadcrumbs from "components/DSMDynamicBreadcrumbs/DSMDynamicBreadcrumbs";
import ProtectedRoute from "components/ProtectedRoute/ProtectedRoute";
import DemographicBadgesNotInPayprofit from "../../pages/DecemberActivities/DemographicBadgesNotInPayprofit/DemographicBadgesNotInPayprofit";
import DistributionsAndForfeitures from "../../pages/DecemberActivities/DistributionsAndForfeitures/DistributionAndForfeitures";
import DuplicateNamesAndBirthdays from "../../pages/DecemberActivities/DuplicateNamesAndBirthdays/DuplicateNamesAndBirthdays";
import DuplicateSSNsOnDemographics from "../../pages/DecemberActivities/DuplicateSSNsOnDemographics/DuplicateSSNsOnDemographics";
import NegativeEtvaForSSNsOnPayprofit from "../../pages/DecemberActivities/NegativeEtvaForSSNsOnPayprofit/NegativeEtvaForSSNsOnPayprofit";
import Termination from "../../pages/DecemberActivities/Termination/Termination";
import RehireForfeitures from "../../pages/DecemberActivities/UnForfeit/RehireForfeitures";
import EligibleEmployees from "../../pages/FiscalClose/EligibleEmployees/EligibleEmployees";
import ManageExecutiveHoursAndDollars from "../../pages/FiscalClose/ManageExecutiveHoursAndDollars/ManageExecutiveHoursAndDollars";
import QPAY066TA from "../../pages/FiscalClose/ProfitShareByStore/BreakdownReport/QPAY066TA";
import NewPSLabels from "../../pages/FiscalClose/ProfitShareByStore/NewPSLabels";
import ProfitShareByStore from "../../pages/FiscalClose/ProfitShareByStore/ProfitShareByStore";
import Under21TA from "../../pages/FiscalClose/ProfitShareByStore/Under21/Under21TA";
import Under21Report from "../../pages/FiscalClose/ProfitShareByStore/Under21Report";
import ProfitShareReportEditRun from "../../pages/FiscalFlow/ProfitShareReportEditRun/ProfitShareReportEditRun";
import ProfitShareReportFinalRun from "../../pages/FiscalFlow/ProfitShareReportFinalRun/ProfitShareReportFinalRun";
import Forfeit from "../../pages/Forfeit/Forfeit";
import FrozenSummary from "../../pages/FrozenSummary/FrozenSummary";
import MasterInquiry from "../../pages/MasterInquiry/MasterInquiry";
import Pay450Summary from "../../pages/PaymasterUpdate/Pay450Summary";
import ProfCtrlSheet from "../../pages/PaymasterUpdate/ProfCtrlSheet";
import BalanceByYears from "../../pages/PROF130/BalanceByYears/BalanceByYears";
import VestedAmountsByAge from "../../pages/PROF130/VestedAmountsByAge/VestedAmountsByAge";
import Profall from "../../pages/Profall/Profall";
import ProfitShareGrossReport from "../../pages/ProfitShareGrossReport/ProfitShareGrossReport";
import ProfitShareReport from "../../pages/ProfitShareReport/ProfitShareReport";
import ProfitShareTotals426 from "../../pages/ProfitShareTotals426/ProfitShareTotals426";

import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { Route, Routes, useLocation, useNavigate } from "react-router-dom";
import { useGetNavigationQuery } from "reduxstore/api/NavigationApi";
import { setImpersonating } from "reduxstore/slices/securitySlice";
import { RootState } from "reduxstore/store";
import { ImpersonationRoles } from "reduxstore/types";
import { drawerClosedWidth, drawerOpenWidth, ROUTES } from "../../constants";
import MenuData from "../../MenuData";
import DemographicFreeze from "../../pages/ITOperations/DemographicFreeze/DemographicFreeze";
import BalanceByAge from "../../pages/PROF130/BalanceByAge/BalanceByAge";
import ContributionsByAge from "../../pages/PROF130/ContributionsByAge/ContributionsByAge";
import DistributionByAge from "../../pages/PROF130/DistributionByAge/DistributionByAge";
import ForfeituresByAge from "../../pages/PROF130/ForfeituresByAge/ForfeituresByAge";
import ProfitShareEditUpdate from "../../pages/ProfitShareEditUpdate/ProfitShareEditUpdate";
import Unauthorized from "../../pages/Unauthorized/Unauthorized";
import YTDWages from "../../pages/YTDWagesExtract/YTDWages";
import EnvironmentUtils from "../../utils/environmentUtils";
import { createUnauthorizedParams, isPathAllowedInNavigation } from "../../utils/navigationAccessUtils";

import ImpersonationMultiSelect from "components/MenuBar/ImpersonationMultiSelect";
import { MenuBar } from "components/MenuBar/MenuBar";
import BeneficiaryInquiry from "../../pages/BeneficiaryInquiry/BeneficiaryInquiry";
import MilitaryEntryAndModification from "../../pages/DecemberActivities/MilitaryEntryAndModification/MilitaryEntryAndModification";
import DevDebug from "../../pages/Dev/DevDebug";
import RecentlyTerminated from "../../pages/FiscalClose/RecentlyTerminated/RecentlyTerminated";
import TerminatedLetters from "../../pages/FiscalClose/TerminatedLetters/TerminatedLetters";
import ForfeituresAdjustment from "../../pages/ForfeituresAdjustment/ForfeituresAdjustment";
import PAY426N from "../../pages/PAY426Reports/PAY426N/PAY426N";
import ProfitSummary from "../../pages/PAY426Reports/ProfitSummary/ProfitSummary";
import QPAY066AdHocReports from "../../pages/QPAY066AdHocReports/QPAY066AdHocReports";
import QPAY066B from "../../pages/QPAY066B/QPAY066B";
import QPAY600 from "../../pages/QPAY600/QPAY600";
import PayBeNext from "../../pages/Reports/PayBeNext/PayBeNext";
import PayBenReport from "../../pages/Reports/PayBenReport/PayBenReport";
import ReprintCertificates from "../../pages/ReprintCertificates/ReprintCertificates";
import LandingPage from "./LandingPage";

const RouterSubAssembly: React.FC = () => {
  const isProductionOrUAT = EnvironmentUtils.isProduction || EnvironmentUtils.isUAT;
  const hasImpersonationRole = EnvironmentUtils.isDevelopmentOrQA;
  const showImpersonation = hasImpersonationRole && !isProductionOrUAT;

  const { impersonating, token } = useSelector((state: RootState) => state.security);

  const dispatch = useDispatch();
  const { isDrawerOpen } = useSelector((state: RootState) => state.general);
  const { data, isSuccess } = useGetNavigationQuery({ navigationId: undefined }, { skip: !token });
  const location = useLocation();
  const navigate = useNavigate();

  // Allow setting an impersonation role via query string in Dev/QA only.
  // Expected query param: ?impersonationRole={roleName}
  useEffect(() => {
    if (!hasImpersonationRole) return;

    const params = new URLSearchParams(location.search);
    const roleParam = params.get("impersonationRole");

    if (!roleParam) return;

    // If impersonating already set, don't override
    if (impersonating && impersonating.length > 0) return;

    const normalize = (s: string) => s.toLowerCase().replace(/[^a-z0-9]/g, "");

    // Try to match against enum keys or values (case/format tolerant)
    const matched = Object.values(ImpersonationRoles).find((r) => {
      const keyForValue = Object.keys(ImpersonationRoles).find((k) => (ImpersonationRoles as any)[k] === r) || "";
      return normalize(r) === normalize(roleParam) || normalize(keyForValue) === normalize(roleParam);
    });

    if (matched) {
      const roles = [matched as ImpersonationRoles];
      try {
        localStorage.setItem("impersonatingRoles", JSON.stringify(roles));
      } catch (e) {
        // ignore storage errors
      }
      dispatch(setImpersonating(roles));

      // Remove the impersonationRole param from the URL so it isn't reapplied on refresh
      params.delete("impersonationRole");
      const newSearch = params.toString();
      navigate(`${location.pathname}${newSearch ? `?${newSearch}` : ""}`, { replace: true });
    }
  }, [location.search, hasImpersonationRole, impersonating, dispatch, navigate, location.pathname]);

  const renderMenu = () => {
    return isSuccess && data ? (
      <>
        <MenuBar
          menuInfo={MenuData(data)}
          navigationData={data}
          impersonationMultiSelect={
            showImpersonation ? (
              <ImpersonationMultiSelect
                impersonationRoles={[
                  ImpersonationRoles.Auditor,
                  ImpersonationRoles.DistributionsClerk,
                  ImpersonationRoles.ExecutiveAdministrator,
                  ImpersonationRoles.FinanceManager,
                  ImpersonationRoles.HardshipAdministrator,
                  ImpersonationRoles.ItDevOps,
                  ImpersonationRoles.ItOperations,
                  ImpersonationRoles.ProfitSharingAdministrator
                ]}
                currentRoles={impersonating || []}
                setCurrentRoles={(value: string[]) => {
                  if (value.length > 0) {
                    localStorage.setItem("impersonatingRoles", JSON.stringify(value));
                    const selectedRoles = value.map((role) => role as ImpersonationRoles);
                    dispatch(setImpersonating(selectedRoles));
                  } else {
                    localStorage.removeItem("impersonatingRoles");
                    dispatch(setImpersonating([]));
                  }
                }}
              />
            ) : (
              <></>
            )
          }
        />
        <Box
          id="TopSubAssemblyRouterBox"
          sx={{ marginTop: "56px", position: "relative", zIndex: 1 }}>
          <Box
            id="SecondSubAssemblyRouterBox"
            sx={{
              height: "100%",
              width: isDrawerOpen ? `calc(100% - ${drawerOpenWidth}px)` : `calc(100% - ${drawerClosedWidth}px)`,
              marginLeft: isDrawerOpen ? `${drawerOpenWidth}px` : `${drawerClosedWidth}px`,

              transition: "all 225ms"
            }}>
            <Box
              id="ThirdSubAssemblyRouterBox"
              sx={{ position: "relative" }}>
              <Box
                id="Breadcrumbs-Box"
                sx={{
                  paddingTop: "24px",
                  paddingBottom: "8px",
                  marginLeft: "24px",
                  marginRight: "24px",
                  minHeight: "32px" // Reserve minimum space for consistent layout
                }}>
                <DSMDynamicBreadcrumbs />
              </Box>
              <PSDrawer navigationData={data} />
              <Routes>
                <Route
                  path="/unauthorized"
                  element={<Unauthorized />}
                />
                <Route
                  path={ROUTES.BENEFICIARY_INQUIRY}
                  element={<BeneficiaryInquiry />}></Route>
                <Route
                  path={ROUTES.PAY_BEN_REPORT}
                  element={<PayBenReport />}></Route>
                <Route
                  path={ROUTES.PAY_BE_NEXT}
                  element={<PayBeNext />}></Route>
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
                  path={ROUTES.FROZEN_SUMMARY}
                  element={<FrozenSummary />}></Route>
                <Route
                  path={ROUTES.BALANCE_BY_YEARS}
                  element={<BalanceByYears />}></Route>
                <Route
                  path={ROUTES.VESTED_AMOUNTS_BY_AGE}
                  element={<VestedAmountsByAge />}></Route>

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
                  path={ROUTES.PROFIT_SHARE_TOTALS}
                  element={<ProfitShareTotals426 />}></Route>
                <Route
                  path="forfeit/:badgeNumber?"
                  element={<Forfeit />}></Route>
                <Route
                  path={ROUTES.FORFEITURES_ADJUSTMENT}
                  element={<ForfeituresAdjustment />}></Route>
                <Route
                  path={ROUTES.FISCAL_CLOSE}
                  element={<></>}></Route>
                <Route
                  path={ROUTES.PROFIT_SHARE_REPORT_EDIT_RUN}
                  element={<ProfitShareReportEditRun />}></Route>
                <Route
                  path={ROUTES.PROFIT_SHARE_REPORT_FINAL_RUN}
                  element={<ProfitShareReportFinalRun />}></Route>
                <Route
                  path={ROUTES.PROFIT_SHARE_UPDATE}
                  element={<ProfitShareEditUpdate />}></Route>
                <Route
                  path=""
                  element={<LandingPage />}></Route>
                <Route
                  path={ROUTES.PROFIT_SHARE_BY_STORE}
                  element={<ProfitShareByStore />}></Route>
                <Route
                  path={ROUTES.PROFIT_SHARE_GROSS_REPORT}
                  element={<ProfitShareGrossReport />}></Route>
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
                  path={ROUTES.PAY426_SUMMARY}
                  element={<ProfitSummary />}
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
                  element={
                    <ProtectedRoute requiredRoles={ImpersonationRoles.ItDevOps}>
                      <DemographicFreeze />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path={ROUTES.DEV_DEBUG}
                  element={<DevDebug />}
                />
                <Route
                  path={ROUTES.PAY426N}
                  element={<PAY426N />}
                />
                <Route
                  path={ROUTES.QPAY066_ADHOC}
                  element={<QPAY066AdHocReports />}
                />
                <Route
                  path={ROUTES.QPAY066B}
                  element={<QPAY066B />}
                />
                <Route
                  path={ROUTES.QPAY600}
                  element={<QPAY600 />}
                />
                <Route
                  path={ROUTES.PRINT_PROFIT_CERTS}
                  element={<ReprintCertificates />}
                />
                <Route
                  path={ROUTES.RECENTLY_TERMINATED}
                  element={<RecentlyTerminated />}
                />
                <Route
                  path={ROUTES.TERMINATED_LETTERS}
                  element={<TerminatedLetters />}
                />
              </Routes>
            </Box>
          </Box>
        </Box>
      </>
    ) : (
      <></>
    );
  };

  useEffect(() => {
    const storedRoles = localStorage.getItem("impersonatingRoles");
    if (storedRoles && (!impersonating || impersonating.length === 0)) {
      try {
        const roles = JSON.parse(storedRoles) as ImpersonationRoles[];
        dispatch(setImpersonating(roles));
      } catch (_e) {
        // If there's an error parsing, clear the localStorage
        localStorage.removeItem("impersonatingRoles");
      }
    }
  }, [dispatch, impersonating]);

  useEffect(() => {
    if (
      isSuccess &&
      data?.navigation &&
      token &&
      location.pathname !== "/unauthorized" &&
      location.pathname !== "/dev-debug"
    ) {
      const currentPath = location.pathname;
      const isAllowed = isPathAllowedInNavigation(currentPath, data.navigation);

      if (!isAllowed) {
        const queryParams = createUnauthorizedParams(currentPath);
        navigate(`/unauthorized?${queryParams}`, { replace: true });
      }
    }
  }, [isSuccess, data, location.pathname, navigate, token]);

  return renderMenu();
};

export default RouterSubAssembly;
