import { Box } from "@mui/material";
import { lazy, Suspense, useEffect } from "react";
import SmartPSDrawer from "../../components/Drawer/SmartPSDrawer";
import DSMDynamicBreadcrumbs from "../../components/DSMDynamicBreadcrumbs/DSMDynamicBreadcrumbs";
import ProtectedRoute from "../../components/ProtectedRoute/ProtectedRoute";
import { PageLoadingFallback } from "../../components/router/LazyPageLoader";
const FrozenSummary = lazy(() => import("../../pages/FrozenSummary/FrozenSummary"));
const MasterInquiry = lazy(() => import("../../pages/InquiriesAndAdjustments/MasterInquiry/MasterInquiry"));
const DemographicBadgesNotInPayprofit = lazy(
  () => import("../../pages/DecemberActivities/DemographicBadgesNotInPayprofit/DemographicBadgesNotInPayprofit")
);
const DistributionsAndForfeitures = lazy(
  () => import("../../pages/DecemberActivities/DistributionsAndForfeitures/DistributionsAndForfeitures")
);
const DuplicateNamesAndBirthdays = lazy(
  () => import("../../pages/DecemberActivities/DuplicateNamesAndBirthdays/DuplicateNamesAndBirthdays")
);
const DuplicateSSNsOnDemographics = lazy(
  () => import("../../pages/DecemberActivities/DuplicateSSNsOnDemographics/DuplicateSSNsOnDemographics")
);
const ManageExecutiveHoursAndDollars = lazy(
  () => import("../../pages/DecemberActivities/ManageExecutiveHoursAndDollars/ManageExecutiveHoursAndDollars")
);
const NegativeEtvaForSSNsOnPayprofit = lazy(
  () => import("../../pages/DecemberActivities/NegativeEtvaForSSNsOnPayprofit/NegativeEtvaForSSNsOnPayprofit")
);
const ProfitShareReport = lazy(() => import("../../pages/DecemberActivities/ProfitShareReport/ProfitShareReport"));
const Termination = lazy(() => import("../../pages/DecemberActivities/Termination/Termination"));
const UnForfeit = lazy(() => import("../../pages/DecemberActivities/UnForfeit/UnForfeit"));
const BalanceByYears = lazy(() => import("../../pages/FiscalClose/AgeReports/BalanceByYears/BalanceByYears"));
const VestedAmountsByAge = lazy(
  () => import("../../pages/FiscalClose/AgeReports/VestedAmountsByAge/VestedAmountsByAge")
);
const EligibleEmployees = lazy(() => import("../../pages/FiscalClose/EligibleEmployees/EligibleEmployees"));
const Forfeit = lazy(() => import("../../pages/FiscalClose/Forfeit/Forfeit"));
const Profall = lazy(() => import("../../pages/FiscalClose/Profall/Profall"));
const QPAY066TA = lazy(() => import("../../pages/FiscalClose/ProfitShareByStore/BreakdownReport/QPAY066TA"));
const NewPSLabels = lazy(() => import("../../pages/FiscalClose/ProfitShareByStore/NewPSLabels"));
const ProfitShareByStore = lazy(() => import("../../pages/FiscalClose/ProfitShareByStore/ProfitShareByStore"));
const Under21TA = lazy(() => import("../../pages/FiscalClose/ProfitShareByStore/Under21/Under21TA"));
const Under21Report = lazy(() => import("../../pages/FiscalClose/ProfitShareByStore/Under21Report"));
const ProfitShareGrossReport = lazy(
  () => import("../../pages/FiscalClose/ProfitShareGrossReport/ProfitShareGrossReport")
);

import { useDispatch, useSelector } from "react-redux";
import { Route, Routes, useLocation, useNavigate } from "react-router-dom";
import { ImpersonationMultiSelect } from "../../components/MenuBar/ImpersonationMultiSelect";
import { MenuBar } from "../../components/MenuBar/MenuBar";
import { drawerClosedWidth, drawerOpenWidth, ROUTES } from "../../constants";
import MenuData from "../../MenuData";
import DevDebug from "../../pages/Dev/DevDebug";
import Documentation from "../../pages/Documentation/Documentation";
import Unauthorized from "../../pages/Unauthorized/Unauthorized";
import { useGetNavigationQuery } from "../../reduxstore/api/NavigationApi";
import { setImpersonating } from "../../reduxstore/slices/securitySlice";
import { RootState } from "../../reduxstore/store";
import { ImpersonationRoles } from "../../reduxstore/types";
import EnvironmentUtils from "../../utils/environmentUtils";
import { createUnauthorizedParams, isPathAllowedInNavigation } from "../../utils/navigationAccessUtils";
import { validateImpersonationRoles, validateRoleRemoval } from "../../utils/roleUtils";
import LandingPage from "./LandingPage";
const YTDWagesLive = lazy(() => import("../../pages/DecemberActivities/YTDWagesExtractLive/YTDWagesLive"));
const BalanceByAge = lazy(() => import("../../pages/FiscalClose/AgeReports/BalanceByAge/BalanceByAge"));
const ContributionsByAge = lazy(
  () => import("../../pages/FiscalClose/AgeReports/ContributionsByAge/ContributionsByAge")
);
const DistributionByAge = lazy(
  () => import("../../pages/FiscalClose/AgeReports/DistributionsByAge/DistributionsByAge")
);
const ForfeituresByAge = lazy(() => import("../../pages/FiscalClose/AgeReports/ForfeituresByAge/ForfeituresByAge"));
const ProfitShareEditUpdate = lazy(() => import("../../pages/FiscalClose/ProfitShareEditUpdate/ProfitShareEditUpdate"));
const YTDWages = lazy(() => import("../../pages/FiscalClose/YTDWagesExtract/YTDWages"));
const DemographicFreeze = lazy(() => import("../../pages/ITOperations/DemographicFreeze/DemographicFreeze"));
const ManageStateTaxes = lazy(() => import("../../pages/Administration/ManageStateTaxes/ManageStateTaxes"));
const ManageAnnuityRates = lazy(() => import("../../pages/Administration/ManageAnnuityRates/ManageAnnuityRates"));
const ManageRmdFactors = lazy(() => import("../../pages/Administration/ManageRmdFactors/ManageRmdFactors"));
const ManageCommentTypes = lazy(() => import("../../pages/Administration/ManageCommentTypes/ManageCommentTypes"));
const ProfitSharingAdjustments = lazy(
  () => import("../../pages/Administration/ProfitSharingAdjustments/ProfitSharingAdjustments")
);
const OracleHcmDiagnostics = lazy(() => import("../../pages/Administration/OracleHcmDiagnostics/OracleHcmDiagnostics"));

const PayMasterUpdateSummary = lazy(() => import("@/pages/FiscalClose/PaymasterUpdate/PayMasterUpdateSummary"));
const ProfitSharingControlSheet = lazy(() => import("@/pages/FiscalClose/PaymasterUpdate/ProfitSharingControlSheet"));
const AuditSearch = lazy(() => import("@/pages/Administration/AuditSearch/AuditSearch"));
const DistributionInquiry = lazy(() => import("../../pages//Distributions/DistributionInquiry/DistributionInquiry"));
const EditDistribution = lazy(() => import("../../pages//Distributions/EditDistribution/EditDistribution"));
const BeneficiaryInquiry = lazy(() => import("../../pages/Beneficiaries/BeneficiaryInquiry"));
const ForfeituresAdjustment = lazy(
  () => import("../../pages/DecemberActivities/ForfeituresAdjustment/ForfeituresAdjustment")
);
const MilitaryContribution = lazy(
  () => import("../../pages/DecemberActivities/MilitaryContribution/MilitaryContribution")
);
const AddDistribution = lazy(() => import("../../pages/Distributions/AddDistribution/AddDistribution"));
const ViewDistribution = lazy(() => import("../../pages/Distributions/ViewDistribution/ViewDistribution"));
const PAY426N = lazy(() => import("../../pages/FiscalClose/PAY426Reports/PAY426N/PAY426N"));
const FrozenProfitSummaryWrapper = lazy(() =>
  import("../../pages/FiscalClose/PAY426Reports/ProfitSummary/ProfitSummary").then((module) => ({
    default: module.FrozenProfitSummaryWrapper
  }))
);
const QPAY066B = lazy(() => import("../../pages/FiscalClose/QPAY066B/QPAY066B"));
const ReprintCertificates = lazy(() => import("../../pages/FiscalClose/ReprintCertificates/ReprintCertificates"));
const Adjustments = lazy(() => import("../../pages/InquiriesAndAdjustments/Adjustments"));
const Reversals = lazy(() => import("../../pages/InquiriesAndAdjustments/Reversals/Reversals"));
const AccountHistoryReport = lazy(() => import("../../pages/Reports/AccountHistoryReport/AccountHistoryReport"));
const PayBeNext = lazy(() => import("../../pages/Reports/PayBeNext/PayBeNext"));
const PayBenReport = lazy(() => import("../../pages/Reports/PayBenReport/PayBenReport"));
const QPAY066xAdHocReports = lazy(() => import("../../pages/Reports/QPAY066xAdHocReports/QPAY066xAdHocReports"));
const AdhocProfLetter73 = lazy(() => import("../../pages/Reports/AdhocProfLetter73/AdhocProfLetter73"));
const TerminatedLetters = lazy(() => import("../../pages/Reports/TerminatedLetters/TerminatedLetters"));
const RecentlyTerminated = lazy(() => import("../../pages/Reports/RecentlyTerminated/RecentlyTerminated"));

const ImpersonatingRolesStorageKey = "impersonatingRoles";

const RouterSubAssembly: React.FC = () => {
  const isProductionOrUAT = EnvironmentUtils.isProduction || EnvironmentUtils.isUAT;
  const hasImpersonationRole = EnvironmentUtils.isDevelopmentOrQA;
  const showImpersonation = hasImpersonationRole && !isProductionOrUAT;

  const { impersonating, token } = useSelector((state: RootState) => state.security);

  const dispatch = useDispatch();

  // CRITICAL DEV/QA FUNCTIONALITY:
  // We intentionally persist impersonation roles to localStorage ONLY in Development/QA.
  // This supports rapid debugging/testing workflows across refreshes.
  // Do NOT remove this without providing an equivalent dev/qa-only mechanism.
  useEffect(() => {
    if (!EnvironmentUtils.isDevelopmentOrQA) {
      return;
    }

    // If impersonation is already present (e.g., hydrated at store init), do not override it.
    if (impersonating && impersonating.length > 0) {
      return;
    }

    try {
      const raw = localStorage.getItem(ImpersonatingRolesStorageKey);
      if (!raw) {
        return;
      }

      const parsed = JSON.parse(raw) as unknown;
      if (!Array.isArray(parsed)) {
        return;
      }

      const allowedRoleValues = new Set<string>(Object.values(ImpersonationRoles));
      const persistedRoles = parsed.filter(
        (x): x is ImpersonationRoles => typeof x === "string" && allowedRoleValues.has(x)
      );
      if (persistedRoles.length > 0) {
        dispatch(setImpersonating(persistedRoles));
      }
    } catch {
      // Ignore localStorage parse errors in dev/qa.
    }
  }, [dispatch, impersonating]);

  // CRITICAL DEV/QA FUNCTIONALITY:
  // Persist/clear impersonation roles across refreshes (Development/QA only).
  useEffect(() => {
    if (!EnvironmentUtils.isDevelopmentOrQA) {
      return;
    }

    try {
      if (impersonating && impersonating.length > 0) {
        localStorage.setItem(ImpersonatingRolesStorageKey, JSON.stringify(impersonating));
      } else {
        localStorage.removeItem(ImpersonatingRolesStorageKey);
      }
    } catch {
      // Ignore localStorage write errors in dev/qa.
    }
  }, [impersonating]);

  const { isDrawerOpen } = useSelector((state: RootState) => state.general);
  const { data, isSuccess } = useGetNavigationQuery({ navigationId: undefined }, { skip: !token });
  const location = useLocation();
  const navigate = useNavigate();

  const isFullscreen = useSelector((state: RootState) => state.general.isFullscreen);

  const renderMenu = () => {
    return isSuccess && data ? (
      <>
        {!isFullscreen && (
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
                    ImpersonationRoles.HrReadOnly,
                    ImpersonationRoles.ItDevOps,
                    ImpersonationRoles.ItOperations,
                    ImpersonationRoles.ProfitSharingAdministrator,
                    ImpersonationRoles.SsnUnmasking
                  ]}
                  currentRoles={impersonating || []}
                  setCurrentRoles={(value: string[]) => {
                    if (value.length === 0) {
                      // Clear all roles
                      dispatch(setImpersonating([]));
                      return;
                    }

                    const currentRoles = (impersonating || []) as ImpersonationRoles[];
                    const newRoles = value.map((role) => role as ImpersonationRoles);

                    // Determine if roles were added or removed
                    let validatedRoles: ImpersonationRoles[];

                    if (newRoles.length > currentRoles.length) {
                      // Role was added - find which one and validate
                      const addedRole = newRoles.find((role) => !currentRoles.includes(role));
                      if (addedRole) {
                        validatedRoles = validateImpersonationRoles(currentRoles, addedRole);
                      } else {
                        validatedRoles = newRoles;
                      }
                    } else if (newRoles.length < currentRoles.length) {
                      // Role was removed - find which one and validate
                      const removedRole = currentRoles.find((role) => !newRoles.includes(role));
                      if (removedRole) {
                        validatedRoles = validateRoleRemoval(currentRoles, removedRole);
                      } else {
                        validatedRoles = newRoles;
                      }
                    } else {
                      // Same length but different roles (shouldn't happen with multi-select, but handle it)
                      validatedRoles = newRoles;
                    }

                    // Update state with validated roles
                    dispatch(setImpersonating(validatedRoles));
                  }}
                />
              ) : (
                <></>
              )
            }
          />
        )}
        <Box
          id="TopSubAssemblyRouterBox"
          sx={{ marginTop: isFullscreen ? "0px" : "56px", position: "relative", zIndex: 1 }}>
          <Box
            id="SecondSubAssemblyRouterBox"
            sx={{
              height: "100%",
              width: isFullscreen
                ? "100%"
                : isDrawerOpen
                  ? `calc(100% - ${drawerOpenWidth}px)`
                  : `calc(100% - ${drawerClosedWidth}px)`,
              marginLeft: isFullscreen ? "0px" : isDrawerOpen ? `${drawerOpenWidth}px` : `${drawerClosedWidth}px`,

              transition: "all 225ms"
            }}>
            <Box
              id="ThirdSubAssemblyRouterBox"
              sx={{ position: "relative" }}>
              {!isFullscreen && (
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
              )}
              {!isFullscreen && <SmartPSDrawer navigationData={data} />}
              <Routes>
                <Route
                  path="/unauthorized"
                  element={<Unauthorized />}
                />
                <Route
                  path={ROUTES.BENEFICIARY_INQUIRY}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <BeneficiaryInquiry />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.DISTRIBUTIONS_INQUIRY}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <DistributionInquiry />
                    </Suspense>
                  }></Route>
                <Route
                  path={`${ROUTES.VIEW_DISTRIBUTION}/:memberId/:memberType`}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <ViewDistribution />
                    </Suspense>
                  }></Route>
                <Route
                  path={`${ROUTES.ADD_DISTRIBUTION}/:memberId/:memberType`}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <AddDistribution />
                    </Suspense>
                  }></Route>
                <Route
                  path={`${ROUTES.EDIT_DISTRIBUTION}/:memberId/:memberType`}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <EditDistribution />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.PAY_BEN_REPORT}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <PayBenReport />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.PAY_BE_NEXT}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <PayBeNext />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.DEMOGRAPHIC_BADGES}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <DemographicBadgesNotInPayprofit />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.DUPLICATE_SSNS}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <DuplicateSSNsOnDemographics />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.NEGATIVE_ETVA}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <NegativeEtvaForSSNsOnPayprofit />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.DUPLICATE_NAMES}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <DuplicateNamesAndBirthdays />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.REHIRE_FORFEITURES}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <UnForfeit />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.DISTRIBUTIONS_AND_FORFEITURES}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <DistributionsAndForfeitures />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.MANAGE_EXECUTIVE_HOURS}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <ManageExecutiveHoursAndDollars />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.ELIGIBLE_EMPLOYEES}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <EligibleEmployees />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.YTD_WAGES_EXTRACT}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <YTDWages />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.YTD_WAGES_EXTRACT_LIVE}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <YTDWagesLive />
                    </Suspense>
                  }></Route>
                <Route
                  path={`${ROUTES.MASTER_INQUIRY}/:badgeNumber?`}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <MasterInquiry />
                    </Suspense>
                  }></Route>
                <Route
                  path={`${ROUTES.ADJUSTMENTS}`}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <Adjustments />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.REVERSALS}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <Reversals />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.DISTRIBUTIONS_BY_AGE}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <DistributionByAge />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.CONTRIBUTIONS_BY_AGE}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <ContributionsByAge />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.FORFEITURES_BY_AGE}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <ForfeituresByAge />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.BALANCE_BY_AGE}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <BalanceByAge />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.FROZEN_SUMMARY}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <FrozenSummary />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.BALANCE_BY_YEARS}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <BalanceByYears />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.VESTED_AMOUNTS_BY_AGE}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <VestedAmountsByAge />
                    </Suspense>
                  }></Route>

                <Route
                  path={ROUTES.PROF_TERM}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <Termination />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.MILITARY_CONTRIBUTION}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <MilitaryContribution />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.PROFIT_SHARE_REPORT}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <ProfitShareReport />
                    </Suspense>
                  }></Route>
                <Route
                  path="forfeit/:badgeNumber?"
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <Forfeit />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.FORFEITURES_ADJUSTMENT}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <ForfeituresAdjustment />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.FISCAL_CLOSE}
                  element={<></>}></Route>
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
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <PayMasterUpdateSummary />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.PROF_CTRLSHEET}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <ProfitSharingControlSheet />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.PROFIT_SHARE_BY_STORE}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <ProfitShareByStore />
                    </Suspense>
                  }></Route>
                <Route
                  path={ROUTES.UNDER_21_REPORT}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <Under21Report />
                    </Suspense>
                  }>
                  {" "}
                </Route>
                <Route
                  path={ROUTES.PAY426_SUMMARY}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <FrozenProfitSummaryWrapper frozenData={true} />
                    </Suspense>
                  }
                />
                <Route
                  path={ROUTES.QPAY066_UNDER21}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <Under21Report />
                    </Suspense>
                  }
                />
                <Route
                  path={ROUTES.QPAY066TA_UNDER21}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <Under21TA />
                    </Suspense>
                  }
                />
                <Route
                  path={ROUTES.QPAY066TA}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <QPAY066TA />
                    </Suspense>
                  }
                />
                <Route
                  path={ROUTES.NEW_PS_LABELS}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <NewPSLabels />
                    </Suspense>
                  }
                />
                <Route
                  path={ROUTES.PROFALL}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <Profall />
                    </Suspense>
                  }
                />
                <Route
                  path={ROUTES.DEMO_FREEZE}
                  element={
                    <ProtectedRoute requiredRoles={ImpersonationRoles.ItDevOps}>
                      <Suspense fallback={<PageLoadingFallback />}>
                        <DemographicFreeze />
                      </Suspense>
                    </ProtectedRoute>
                  }
                />
                <Route
                  path={ROUTES.MANAGE_STATE_TAXES}
                  element={
                    <ProtectedRoute
                      requiredRoles={[ImpersonationRoles.ItDevOps, ImpersonationRoles.ProfitSharingAdministrator]}>
                      <Suspense fallback={<PageLoadingFallback />}>
                        <ManageStateTaxes />
                      </Suspense>
                    </ProtectedRoute>
                  }
                />
                <Route
                  path={ROUTES.MANAGE_ANNUITY_RATES}
                  element={
                    <ProtectedRoute
                      requiredRoles={[ImpersonationRoles.ItDevOps, ImpersonationRoles.ProfitSharingAdministrator]}>
                      <Suspense fallback={<PageLoadingFallback />}>
                        <ManageAnnuityRates />
                      </Suspense>
                    </ProtectedRoute>
                  }
                />
                <Route
                  path={ROUTES.MANAGE_RMD_FACTORS}
                  element={
                    <ProtectedRoute
                      requiredRoles={[ImpersonationRoles.ItDevOps, ImpersonationRoles.ProfitSharingAdministrator]}>
                      <Suspense fallback={<PageLoadingFallback />}>
                        <ManageRmdFactors />
                      </Suspense>
                    </ProtectedRoute>
                  }
                />
                <Route
                  path={ROUTES.MANAGE_COMMENT_TYPES}
                  element={
                    <ProtectedRoute
                      requiredRoles={[ImpersonationRoles.ItDevOps, ImpersonationRoles.ProfitSharingAdministrator]}>
                      <Suspense fallback={<PageLoadingFallback />}>
                        <ManageCommentTypes />
                      </Suspense>
                    </ProtectedRoute>
                  }
                />
                <Route
                  path={ROUTES.PROFIT_SHARING_ADJUSTMENTS}
                  element={
                    <ProtectedRoute
                      requiredRoles={[ImpersonationRoles.ItDevOps, ImpersonationRoles.ProfitSharingAdministrator]}>
                      <Suspense fallback={<PageLoadingFallback />}>
                        <ProfitSharingAdjustments />
                      </Suspense>
                    </ProtectedRoute>
                  }
                />
                <Route
                  path={ROUTES.AUDIT_SEARCH}
                  element={
                    <ProtectedRoute
                      requiredRoles={[
                        ImpersonationRoles.Auditor,
                        ImpersonationRoles.HrReadOnly,
                        ImpersonationRoles.ItDevOps,
                        ImpersonationRoles.ProfitSharingAdministrator,
                        ImpersonationRoles.SsnUnmasking
                      ]}>
                      <Suspense fallback={<PageLoadingFallback />}>
                        <AuditSearch />
                      </Suspense>
                    </ProtectedRoute>
                  }
                />
                <Route
                  path={ROUTES.ORACLE_HCM_DIAGNOSTICS}
                  element={
                    <ProtectedRoute
                      requiredRoles={[ImpersonationRoles.ItDevOps, ImpersonationRoles.ProfitSharingAdministrator]}>
                      <Suspense fallback={<PageLoadingFallback />}>
                        <OracleHcmDiagnostics />
                      </Suspense>
                    </ProtectedRoute>
                  }
                />
                <Route
                  path={ROUTES.DEV_DEBUG}
                  element={<DevDebug />}
                />
                <Route
                  path={ROUTES.DOCUMENTATION}
                  element={<Documentation />}
                />
                <Route
                  path={`${ROUTES.PAY426N_LIVE}/:presetNumber?`}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <PAY426N isFrozen={false} />
                    </Suspense>
                  }
                />
                <Route
                  path={`${ROUTES.PAY426N_FROZEN}/:presetNumber?`}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <PAY426N isFrozen={true} />
                    </Suspense>
                  }
                />

                <Route
                  path={ROUTES.QPAY066_ADHOC}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <QPAY066xAdHocReports />
                    </Suspense>
                  }
                />
                <Route
                  path={ROUTES.QPAY066B}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <QPAY066B />
                    </Suspense>
                  }
                />
                <Route
                  path={ROUTES.ADHOC_PROF_LETTER73}
                  element={<AdhocProfLetter73 />}
                />
                <Route
                  path={ROUTES.PRINT_PROFIT_CERTS}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <ReprintCertificates />
                    </Suspense>
                  }
                />
                <Route
                  path={ROUTES.RECENTLY_TERMINATED}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <RecentlyTerminated />
                    </Suspense>
                  }
                />
                <Route
                  path={ROUTES.TERMINATED_LETTERS}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <TerminatedLetters />
                    </Suspense>
                  }
                />
                <Route
                  path={ROUTES.DIVORCE_REPORT}
                  element={
                    <Suspense fallback={<PageLoadingFallback />}>
                      <AccountHistoryReport />
                    </Suspense>
                  }
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
    if (
      isSuccess &&
      data?.navigation &&
      token &&
      location.pathname !== "/unauthorized" &&
      location.pathname !== "/dev-debug" &&
      location.pathname !== "/documentation"
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
