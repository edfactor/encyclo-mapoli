import { Box } from "@mui/material";
import PSDrawer from "components/Drawer/PSDrawer";
import DSMDynamicBreadcrumbs from "components/DSMDynamicBreadcrumbs/DSMDynamicBreadcrumbs";
import DemographicBadgesNotInPayprofit from "pages/DecemberActivities/DemographicBadgesNotInPayprofit/DemographicBadgesNotInPayprofit";
import DistributionsAndForfeitures from "pages/DecemberActivities/DistributionsAndForfeitures/DistributionAndForfeitures";
import DuplicateNamesAndBirthdays from "pages/DecemberActivities/DuplicateNamesAndBirthdays/DuplicateNamesAndBirthdays";
import DuplicateSSNsOnDemographics from "pages/DecemberActivities/DuplicateSSNsOnDemographics/DuplicateSSNsOnDemographics";
import EmployeesOnMilitaryLeave from "pages/DecemberActivities/EmployeesOnMilitaryLeave/EmployeesOnMilitaryLeave";
import NegativeEtvaForSSNsOnPayprofit from "pages/DecemberActivities/NegativeEtvaForSSNsOnPayprofit/NegativeEtvaForSSNsOnPayprofit";
import Termination from "pages/DecemberActivities/Termination/Termination";
import RehireForfeitures from "pages/DecemberActivities/UnForfeit/RehireForfeitures";
import EligibleEmployees from "pages/FiscalClose/EligibleEmployees/EligibleEmployees";
import ManageExecutiveHoursAndDollars from "pages/FiscalClose/ManageExecutiveHoursAndDollars/ManageExecutiveHoursAndDollars";
import QPAY066TA from "pages/FiscalClose/ProfitShareByStore/BreakdownReport/QPAY066TA";
import NewPSLabels from "pages/FiscalClose/ProfitShareByStore/NewPSLabels";
import ProfitShareByStore from "pages/FiscalClose/ProfitShareByStore/ProfitShareByStore";
import Under21TA from "pages/FiscalClose/ProfitShareByStore/Under21/Under21TA";
import Under21Report from "pages/FiscalClose/ProfitShareByStore/Under21Report";
import ProfitShareReportEditRun from "pages/FiscalFlow/ProfitShareReportEditRun/ProfitShareReportEditRun";
import ProfitShareReportFinalRun from "pages/FiscalFlow/ProfitShareReportFinalRun/ProfitShareReportFinalRun";
import Forfeit from "pages/Forfeit/Forfeit";
import FrozenSummary from "pages/FrozenSummary/FrozenSummary";
import MasterInquiry from "pages/MasterInquiry/MasterInquiry";
import Pay450Summary from "pages/PaymasterUpdate/Pay450Summary";
import PaymasterUpdate from "pages/PaymasterUpdate/PaymasterUpdate";
import ProfCtrlSheet from "pages/PaymasterUpdate/ProfCtrlSheet";
import BalanceByYears from "pages/PROF130/BalanceByYears/BalanceByYears";
import VestedAmountsByAge from "pages/PROF130/VestedAmountsByAge/VestedAmountsByAge";
import Profall from "pages/Profall/Profall";
import ProfitShareGrossReport from "pages/ProfitShareGrossReport/ProfitShareGrossReport";
import ProfitShareReport from "pages/ProfitShareReport/ProfitShareReport";
import ProfitShareTotals426 from "pages/ProfitShareTotals426/ProfitShareTotals426";

import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { Route, Routes } from "react-router-dom";
import { useGetNavigationQuery } from "reduxstore/api/NavigationApi";
import { setImpersonating } from "reduxstore/slices/securitySlice";
import { RootState } from "reduxstore/store";
import { ImpersonationRoles } from "reduxstore/types";
import { ImpersonationMultiSelect } from "smart-ui-library";
import { drawerClosedWidth, drawerOpenWidth, ROUTES, SMART_PS_QA_IMPERSONATION } from "../../constants";
import MenuData from "../../MenuData";
import DemographicFreeze from "../../pages/ITOperations/DemographicFreeze/DemographicFreeze";
import BalanceByAge from "../../pages/PROF130/BalanceByAge/BalanceByAge";
import ContributionsByAge from "../../pages/PROF130/ContributionsByAge/ContributionsByAge";
import DistributionByAge from "../../pages/PROF130/DistributionByAge/DistributionByAge";
import ForfeituresByAge from "../../pages/PROF130/ForfeituresByAge/ForfeituresByAge";
import ProfitShareEditUpdate from "../../pages/ProfitShareEditUpdate/ProfitShareEditUpdate";
import YTDWages from "../../pages/YTDWagesExtract/YTDWages";

import { MenuBar } from "components/MenuBar/MenuBar";
import BeneficiaryInquiry from "pages/BeneficiaryInquiry/BeneficiaryInquiry";
import PAY426N from "pages/PAY426Reports/PAY426N/PAY426N";
import ProfitSummary from "pages/PAY426Reports/ProfitSummary/ProfitSummary";
import QPAY066AdHocReports from "pages/QPAY066AdHocReports/QPAY066AdHocReports";
import QPAY600 from "pages/QPAY600/QPAY600";
import PayBeNext from "pages/Reports/PayBeNext/PayBeNext";
import PayBenReport from "pages/Reports/PayBenReport/PayBenReport";
import MilitaryEntryAndModification from "../../pages/DecemberActivities/MilitaryEntryAndModification/MilitaryEntryAndModification";
import DevDebug from "../../pages/Dev/DevDebug";
import ForfeituresAdjustment from "../../pages/ForfeituresAdjustment/ForfeituresAdjustment";

const RouterSubAssembly: React.FC = () => {
  const isProduction = false;
  const userGroups = useSelector((state: RootState) => state.security.userGroups);
  const hasImpersonationRole = userGroups.includes(SMART_PS_QA_IMPERSONATION);
  const showImpersonation = hasImpersonationRole && !isProduction;

  const { impersonating, token } = useSelector((state: RootState) => state.security);

  const dispatch = useDispatch();
  const { isDrawerOpen } = useSelector((state: RootState) => state.general);
  const { data, isSuccess } = useGetNavigationQuery({ navigationId: undefined }, { skip: !token });

  const localStorageImpersonating: string | null = localStorage.getItem("impersonatingRole");

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
        {/* THIS IS THE BOX THAT HOLDS ALL CONTENT BELOW THE TOP MENU 
          AND MAKES IT POSSIBLE TO KEEP THE BREADCRUMBS WHERE THEY SHOULD BE */}
        <Box
          id="TopSubAssemblyRouterBox"
          sx={{ marginTop: "56px", position: "relative", zIndex: 1 }}>
          {/* THIS BOX ALLOWS THE CONTENT TO BE PUSHED RIGHT BY DRAWER CONTROL */}
          <Box
            id="SecondSubAssemblyRouterBox"
            sx={{
              height: "100%",
              width: isDrawerOpen ? `calc(100% - ${drawerOpenWidth}px)` : `calc(100% - ${drawerClosedWidth}px)`,
              marginLeft: isDrawerOpen ? `${drawerOpenWidth}px` : `${drawerClosedWidth}px`,

              transition: "all 225ms"
            }}>
            <Box
              id="ThirdSubAssemblyRouterBox-all-under-menu"
              sx={{ position: "relative", paddingTop: "32px" }}>
              <Box
                id="Breadcrumbs-Box"
                sx={{
                  position: "absolute",
                  top: 0,
                  left: 0,
                  right: 0,
                  paddingTop: "24px",
                  marginLeft: "24px",
                  marginRight: "24px"
                }}>
                <DSMDynamicBreadcrumbs />
              </Box>
              <PSDrawer navigationData={data} />
              <Routes>
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
                  path={ROUTES.MILITARY_LEAVE}
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
                  element={<></>}></Route>
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
                  element={<DemographicFreeze />}
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
                  path={ROUTES.QPAY600}
                  element={<QPAY600 />}
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
    if (!!localStorageImpersonating && !impersonating) {
      dispatch(setImpersonating(localStorageImpersonating as ImpersonationRoles));
    }
  }, [dispatch, impersonating, localStorageImpersonating]);

  // This is here if we want this
  // Open drawer when navigation data is loaded
  /*
  useEffect(() => {
    if (isSuccess && data && !isDrawerOpen) {
      dispatch(openDrawer());
    }
  }, [isSuccess, data, isDrawerOpen, dispatch]);
  */

  return renderMenu();
};

export default RouterSubAssembly;
