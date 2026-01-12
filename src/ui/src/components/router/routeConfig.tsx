import { lazy, Suspense } from "react";
import type { RouteObject } from "react-router-dom";
import { ROUTES } from "../../constants";
import DevDebug from "../../pages/Dev/DevDebug";
import Documentation from "../../pages/Documentation/Documentation";
import Unauthorized from "../../pages/Unauthorized/Unauthorized";
import type { AppStore } from "../../reduxstore/store";
import { ImpersonationRoles } from "../../reduxstore/types";
import EnvironmentUtils from "../../utils/environmentUtils";
import ProtectedRoute from "../ProtectedRoute/ProtectedRoute";
import LandingPage from "./LandingPage";
import { PageLoadingFallback } from "./LazyPageLoader";
import LoaderErrorBoundary from "./LoaderErrorBoundary";
import { navigationLoader } from "./loaders";
import RootLayout from "./RootLayout";

// Lazy load all route components
const Login = lazy(() => import("../Login/Login"));
const OktaLoginCallback = lazy(() => import("../MenuBar/OktaLoginCallback"));
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
const YTDWagesLive = lazy(() => import("../../pages/DecemberActivities/YTDWagesExtractLive/YTDWagesLive"));
const ForfeituresAdjustment = lazy(
  () => import("../../pages/DecemberActivities/ForfeituresAdjustment/ForfeituresAdjustment")
);
const MilitaryContribution = lazy(
  () => import("../../pages/DecemberActivities/MilitaryContribution/MilitaryContribution")
);

const BalanceByYears = lazy(() => import("../../pages/FiscalClose/AgeReports/BalanceByYears/BalanceByYears"));
const VestedAmountsByAge = lazy(
  () => import("../../pages/FiscalClose/AgeReports/VestedAmountsByAge/VestedAmountsByAge")
);
const BalanceByAge = lazy(() => import("../../pages/FiscalClose/AgeReports/BalanceByAge/BalanceByAge"));
const ContributionsByAge = lazy(
  () => import("../../pages/FiscalClose/AgeReports/ContributionsByAge/ContributionsByAge")
);
const DistributionByAge = lazy(
  () => import("../../pages/FiscalClose/AgeReports/DistributionsByAge/DistributionsByAge")
);
const ForfeituresByAge = lazy(() => import("../../pages/FiscalClose/AgeReports/ForfeituresByAge/ForfeituresByAge"));

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
const ProfitShareEditUpdate = lazy(() => import("../../pages/FiscalClose/ProfitShareEditUpdate/ProfitShareEditUpdate"));
const YTDWages = lazy(() => import("../../pages/FiscalClose/YTDWagesExtract/YTDWages"));
const PayMasterUpdateSummary = lazy(() => import("../../pages/FiscalClose/PaymasterUpdate/PayMasterUpdateSummary"));
const ProfitSharingControlSheet = lazy(
  () => import("../../pages/FiscalClose/PaymasterUpdate/ProfitSharingControlSheet")
);
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

const BeneficiaryInquiry = lazy(() => import("../../pages/Beneficiaries/BeneficiaryInquiry"));

const DistributionInquiry = lazy(() => import("../../pages/Distributions/DistributionInquiry/DistributionInquiry"));
const EditDistribution = lazy(() => import("../../pages/Distributions/EditDistribution/EditDistribution"));
const AddDistribution = lazy(() => import("../../pages/Distributions/AddDistribution/AddDistribution"));
const ViewDistribution = lazy(() => import("../../pages/Distributions/ViewDistribution/ViewDistribution"));

const DemographicFreeze = lazy(() => import("../../pages/ITOperations/DemographicFreeze/DemographicFreeze"));
const FakeTimeManagement = lazy(() => import("../../pages/ITOperations/FakeTimeManagement/FakeTimeManagement"));

const ManageStateTaxes = lazy(() => import("../../pages/Administration/ManageStateTaxes/ManageStateTaxes"));
const ManageAnnuityRates = lazy(() => import("../../pages/Administration/ManageAnnuityRates/ManageAnnuityRates"));
const ManageRmdFactors = lazy(() => import("../../pages/Administration/ManageRmdFactors/ManageRmdFactors"));
const ManageCommentTypes = lazy(() => import("../../pages/Administration/ManageCommentTypes/ManageCommentTypes"));
const ProfitSharingAdjustments = lazy(
  () => import("../../pages/Administration/ProfitSharingAdjustments/ProfitSharingAdjustments")
);
const OracleHcmDiagnostics = lazy(() => import("../../pages/Administration/OracleHcmDiagnostics/OracleHcmDiagnostics"));
const AuditSearch = lazy(() => import("../../pages/Administration/AuditSearch/AuditSearch"));

/**
 * Creates the React Router v7 data router configuration.
 *
 * Features:
 * - Route-level Suspense boundaries for granular loading states
 * - Individual protected route wrapping for role-based access
 * - Authentication-gated navigation loader
 * - Conditional Okta routes based on environment
 *
 * @param store - Redux store for loader access to authentication state
 * @returns Route configuration array for createBrowserRouter
 */
export function createRoutes(store: AppStore): RouteObject[] {
  const routes: RouteObject[] = [
    {
      path: "/",
      loader: ({ request }) => navigationLoader(store, request),
      element: <RootLayout />,
      errorElement: <LoaderErrorBoundary />,
      children: [
        // Landing page
        {
          index: true,
          element: <LandingPage />
        },

        // Unauthorized page (no auth required)
        {
          path: "unauthorized",
          element: <Unauthorized />
        },

        // Dev/Documentation pages (no auth required)
        {
          path: ROUTES.DEV_DEBUG,
          element: <DevDebug />
        },
        {
          path: ROUTES.DOCUMENTATION,
          element: <Documentation />
        },

        // Beneficiaries
        {
          path: ROUTES.BENEFICIARY_INQUIRY,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <BeneficiaryInquiry />
            </Suspense>
          )
        },

        // Distributions
        {
          path: ROUTES.DISTRIBUTIONS_INQUIRY,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <DistributionInquiry />
            </Suspense>
          )
        },
        {
          path: `${ROUTES.VIEW_DISTRIBUTION}/:memberId/:memberType`,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <ViewDistribution />
            </Suspense>
          )
        },
        {
          path: `${ROUTES.ADD_DISTRIBUTION}/:memberId/:memberType`,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <AddDistribution />
            </Suspense>
          )
        },
        {
          path: `${ROUTES.EDIT_DISTRIBUTION}/:memberId/:memberType`,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <EditDistribution />
            </Suspense>
          )
        },

        // Reports
        {
          path: ROUTES.PAY_BEN_REPORT,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <PayBenReport />
            </Suspense>
          )
        },
        {
          path: ROUTES.PAY_BE_NEXT,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <PayBeNext />
            </Suspense>
          )
        },
        {
          path: ROUTES.QPAY066_ADHOC,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <QPAY066xAdHocReports />
            </Suspense>
          )
        },
        {
          path: ROUTES.QPAY066B,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <QPAY066B />
            </Suspense>
          )
        },
        {
          path: ROUTES.ADHOC_PROF_LETTER73,
          element: <AdhocProfLetter73 />
        },
        {
          path: ROUTES.PRINT_PROFIT_CERTS,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <ReprintCertificates />
            </Suspense>
          )
        },
        {
          path: ROUTES.RECENTLY_TERMINATED,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <RecentlyTerminated />
            </Suspense>
          )
        },
        {
          path: ROUTES.TERMINATED_LETTERS,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <TerminatedLetters />
            </Suspense>
          )
        },
        {
          path: ROUTES.DIVORCE_REPORT,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <AccountHistoryReport />
            </Suspense>
          )
        },

        // December Activities
        {
          path: ROUTES.DEMOGRAPHIC_BADGES,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <DemographicBadgesNotInPayprofit />
            </Suspense>
          )
        },
        {
          path: ROUTES.DUPLICATE_SSNS,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <DuplicateSSNsOnDemographics />
            </Suspense>
          )
        },
        {
          path: ROUTES.NEGATIVE_ETVA,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <NegativeEtvaForSSNsOnPayprofit />
            </Suspense>
          )
        },
        {
          path: ROUTES.DUPLICATE_NAMES,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <DuplicateNamesAndBirthdays />
            </Suspense>
          )
        },
        {
          path: ROUTES.REHIRE_FORFEITURES,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <UnForfeit />
            </Suspense>
          )
        },
        {
          path: ROUTES.DISTRIBUTIONS_AND_FORFEITURES,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <DistributionsAndForfeitures />
            </Suspense>
          )
        },
        {
          path: ROUTES.MANAGE_EXECUTIVE_HOURS,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <ManageExecutiveHoursAndDollars />
            </Suspense>
          )
        },
        {
          path: ROUTES.YTD_WAGES_EXTRACT_LIVE,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <YTDWagesLive />
            </Suspense>
          )
        },
        {
          path: ROUTES.PROF_TERM,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <Termination />
            </Suspense>
          )
        },
        {
          path: ROUTES.MILITARY_CONTRIBUTION,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <MilitaryContribution />
            </Suspense>
          )
        },
        {
          path: ROUTES.PROFIT_SHARE_REPORT,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <ProfitShareReport />
            </Suspense>
          )
        },
        {
          path: ROUTES.FORFEITURES_ADJUSTMENT,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <ForfeituresAdjustment />
            </Suspense>
          )
        },

        // Fiscal Close / Year End
        {
          path: ROUTES.ELIGIBLE_EMPLOYEES,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <EligibleEmployees />
            </Suspense>
          )
        },
        {
          path: ROUTES.YTD_WAGES_EXTRACT,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <YTDWages />
            </Suspense>
          )
        },
        {
          path: "forfeit/:badgeNumber?",
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <Forfeit />
            </Suspense>
          )
        },
        {
          path: ROUTES.FISCAL_CLOSE,
          element: <></>
        },
        {
          path: ROUTES.PROFIT_SHARE_UPDATE,
          element: <ProfitShareEditUpdate />
        },
        {
          path: ROUTES.PROFIT_SHARE_BY_STORE,
          element: <ProfitShareByStore />
        },
        {
          path: ROUTES.PROFIT_SHARE_GROSS_REPORT,
          element: <ProfitShareGrossReport />
        },
        {
          path: ROUTES.PAY450_SUMMARY,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <PayMasterUpdateSummary />
            </Suspense>
          )
        },
        {
          path: ROUTES.PROF_CTRLSHEET,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <ProfitSharingControlSheet />
            </Suspense>
          )
        },
        {
          path: ROUTES.UNDER_21_REPORT,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <Under21Report />
            </Suspense>
          )
        },
        {
          path: ROUTES.PAY426_SUMMARY,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <FrozenProfitSummaryWrapper frozenData={true} />
            </Suspense>
          )
        },
        {
          path: ROUTES.QPAY066_UNDER21,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <Under21Report />
            </Suspense>
          )
        },
        {
          path: ROUTES.QPAY066TA_UNDER21,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <Under21TA />
            </Suspense>
          )
        },
        {
          path: ROUTES.QPAY066TA,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <QPAY066TA />
            </Suspense>
          )
        },
        {
          path: ROUTES.NEW_PS_LABELS,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <NewPSLabels />
            </Suspense>
          )
        },
        {
          path: ROUTES.PROFALL,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <Profall />
            </Suspense>
          )
        },
        {
          path: `${ROUTES.PAY426N_LIVE}/:presetNumber?`,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <PAY426N isFrozen={false} />
            </Suspense>
          )
        },
        {
          path: `${ROUTES.PAY426N_FROZEN}/:presetNumber?`,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <PAY426N isFrozen={true} />
            </Suspense>
          )
        },

        // Inquiries & Adjustments
        {
          path: `${ROUTES.MASTER_INQUIRY}/:badgeNumber?`,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <MasterInquiry />
            </Suspense>
          )
        },
        {
          path: ROUTES.ADJUSTMENTS,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <Adjustments />
            </Suspense>
          )
        },
        {
          path: ROUTES.REVERSALS,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <Reversals />
            </Suspense>
          )
        },

        // Age Reports
        {
          path: ROUTES.DISTRIBUTIONS_BY_AGE,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <DistributionByAge />
            </Suspense>
          )
        },
        {
          path: ROUTES.CONTRIBUTIONS_BY_AGE,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <ContributionsByAge />
            </Suspense>
          )
        },
        {
          path: ROUTES.FORFEITURES_BY_AGE,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <ForfeituresByAge />
            </Suspense>
          )
        },
        {
          path: ROUTES.BALANCE_BY_AGE,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <BalanceByAge />
            </Suspense>
          )
        },
        {
          path: ROUTES.FROZEN_SUMMARY,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <FrozenSummary />
            </Suspense>
          )
        },
        {
          path: ROUTES.BALANCE_BY_YEARS,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <BalanceByYears />
            </Suspense>
          )
        },
        {
          path: ROUTES.VESTED_AMOUNTS_BY_AGE,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <VestedAmountsByAge />
            </Suspense>
          )
        },

        // IT Operations (Protected)
        {
          path: ROUTES.DEMO_FREEZE,
          element: (
            <ProtectedRoute requiredRoles={ImpersonationRoles.ItDevOps}>
              <Suspense fallback={<PageLoadingFallback />}>
                <DemographicFreeze />
              </Suspense>
            </ProtectedRoute>
          )
        },
        {
          path: ROUTES.FAKE_TIME_MANAGEMENT,
          element: (
            <ProtectedRoute requiredRoles={ImpersonationRoles.ItDevOps}>
              <Suspense fallback={<PageLoadingFallback />}>
                <FakeTimeManagement />
              </Suspense>
            </ProtectedRoute>
          )
        },

        // Administration (Protected)
        {
          path: ROUTES.MANAGE_STATE_TAXES,
          element: (
            <ProtectedRoute
              requiredRoles={[ImpersonationRoles.ItDevOps, ImpersonationRoles.ProfitSharingAdministrator]}>
              <Suspense fallback={<PageLoadingFallback />}>
                <ManageStateTaxes />
              </Suspense>
            </ProtectedRoute>
          )
        },
        {
          path: ROUTES.MANAGE_ANNUITY_RATES,
          element: (
            <ProtectedRoute
              requiredRoles={[ImpersonationRoles.ItDevOps, ImpersonationRoles.ProfitSharingAdministrator]}>
              <Suspense fallback={<PageLoadingFallback />}>
                <ManageAnnuityRates />
              </Suspense>
            </ProtectedRoute>
          )
        },
        {
          path: ROUTES.MANAGE_RMD_FACTORS,
          element: (
            <ProtectedRoute
              requiredRoles={[ImpersonationRoles.ItDevOps, ImpersonationRoles.ProfitSharingAdministrator]}>
              <Suspense fallback={<PageLoadingFallback />}>
                <ManageRmdFactors />
              </Suspense>
            </ProtectedRoute>
          )
        },
        {
          path: ROUTES.MANAGE_COMMENT_TYPES,
          element: (
            <ProtectedRoute
              requiredRoles={[ImpersonationRoles.ItDevOps, ImpersonationRoles.ProfitSharingAdministrator]}>
              <Suspense fallback={<PageLoadingFallback />}>
                <ManageCommentTypes />
              </Suspense>
            </ProtectedRoute>
          )
        },
        {
          path: ROUTES.PROFIT_SHARING_ADJUSTMENTS,
          element: (
            <ProtectedRoute
              requiredRoles={[ImpersonationRoles.ItDevOps, ImpersonationRoles.ProfitSharingAdministrator]}>
              <Suspense fallback={<PageLoadingFallback />}>
                <ProfitSharingAdjustments />
              </Suspense>
            </ProtectedRoute>
          )
        },
        {
          path: ROUTES.AUDIT_SEARCH,
          element: (
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
          )
        },
        {
          path: ROUTES.ORACLE_HCM_DIAGNOSTICS,
          element: (
            <ProtectedRoute
              requiredRoles={[ImpersonationRoles.ItDevOps, ImpersonationRoles.ProfitSharingAdministrator]}>
              <Suspense fallback={<PageLoadingFallback />}>
                <OracleHcmDiagnostics />
              </Suspense>
            </ProtectedRoute>
          )
        },

        // Catch-all route for unmatched paths
        // Handles accordion containers (e.g., december-process-accordion) and other
        // navigation URLs that exist in the database but don't have dedicated page components.
        // Shows landing page content instead of an error.
        {
          path: "*",
          element: <LandingPage />
        }
      ]
    }
  ];

  // Add Okta login routes as TOP-LEVEL routes (NOT nested under RootLayout)
  // This prevents them from inheriting the navigationLoader which requires auth
  if (EnvironmentUtils.isOktaEnabled) {
    routes.push(
      {
        path: "/login/callback",
        element: (
          <Suspense fallback={<div>Loading...</div>}>
            <OktaLoginCallback />
          </Suspense>
        )
      },
      {
        path: "/login",
        element: (
          <Suspense fallback={<div>Loading...</div>}>
            <Login />
          </Suspense>
        )
      }
    );
  }

  return routes;
}
