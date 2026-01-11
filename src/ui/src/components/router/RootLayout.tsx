import { Box } from "@mui/material";
import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { Outlet, useLoaderData, useLocation, useNavigate } from "react-router-dom";
import { drawerClosedWidth, drawerOpenWidth } from "../../constants";
import MenuData from "../../MenuData";
import { setImpersonating } from "../../reduxstore/slices/securitySlice";
import type { RootState } from "../../reduxstore/store";
import type { NavigationResponseDto } from "../../reduxstore/types";
import { ImpersonationRoles } from "../../reduxstore/types";
import EnvironmentUtils from "../../utils/environmentUtils";
import { createUnauthorizedParams, isPathAllowedInNavigation } from "../../utils/navigationAccessUtils";
import { validateImpersonationRoles, validateRoleRemoval } from "../../utils/roleUtils";
import SmartPSDrawer from "../Drawer/SmartPSDrawer";
import DSMDynamicBreadcrumbs from "../DSMDynamicBreadcrumbs/DSMDynamicBreadcrumbs";
import { ImpersonationMultiSelect } from "../MenuBar/ImpersonationMultiSelect";
import { MenuBar } from "../MenuBar/MenuBar";

const ImpersonatingRolesStorageKey = "impersonatingRoles";

/**
 * Root layout component for React Router v7 data router.
 *
 * Replaces RouterSubAssembly.tsx with loader-based navigation data access.
 * Key differences from legacy pattern:
 * - Uses useLoaderData() instead of useGetNavigationQuery hook
 * - Navigation data is pre-loaded before rendering (no loading state needed)
 * - Renders Outlet for child route rendering instead of Routes/Route
 *
 * Preserves all existing functionality:
 * - Impersonation role management with localStorage persistence (dev/qa only)
 * - MenuBar, Drawer, and breadcrumb rendering
 * - Fullscreen mode support
 * - Path authorization validation
 */
const RootLayout: React.FC = () => {
  // Get pre-loaded navigation data from router loader
  const navigationData = useLoaderData() as NavigationResponseDto;

  const isProductionOrUAT = EnvironmentUtils.isProduction || EnvironmentUtils.isUAT;
  const hasImpersonationRole = EnvironmentUtils.isDevelopmentOrQA;
  const showImpersonation = hasImpersonationRole && !isProductionOrUAT;

  const { impersonating, token } = useSelector((state: RootState) => state.security);
  const { isDrawerOpen, isFullscreen } = useSelector((state: RootState) => state.general);

  const dispatch = useDispatch();
  const location = useLocation();
  const navigate = useNavigate();

  // CRITICAL DEV/QA FUNCTIONALITY:
  // Hydrate impersonation roles from localStorage on mount (Development/QA only)
  useEffect(() => {
    if (!EnvironmentUtils.isDevelopmentOrQA) {
      return;
    }

    // If impersonation is already present (e.g., hydrated at store init), do not override it
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
      // Ignore localStorage parse errors in dev/qa
    }
  }, [dispatch, impersonating]);

  // CRITICAL DEV/QA FUNCTIONALITY:
  // Persist/clear impersonation roles to localStorage on change (Development/QA only)
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
      // Ignore localStorage write errors in dev/qa
    }
  }, [impersonating]);

  // Validate path authorization and redirect to unauthorized if needed
  useEffect(() => {
    const currentPath = location.pathname;

    // Skip auth-centric routes from navigation restriction to avoid redirect loops
    if (currentPath.startsWith("/login")) {
      return;
    }

    if (
      navigationData?.navigation &&
      token &&
      currentPath !== "/unauthorized" &&
      currentPath !== "/dev-debug" &&
      currentPath !== "/documentation"
    ) {
      const isAllowed = isPathAllowedInNavigation(currentPath, navigationData.navigation);

      if (!isAllowed) {
        const queryParams = createUnauthorizedParams(currentPath);
        navigate(`/unauthorized?${queryParams}`, { replace: true });
      }
    }
  }, [navigationData, location.pathname, navigate, token]);

  // Render layout with MenuBar, Drawer, and child routes
  return (
    <>
      {!isFullscreen && (
        <MenuBar
          menuInfo={MenuData(navigationData)}
          navigationData={navigationData}
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
            {!isFullscreen && <SmartPSDrawer navigationData={navigationData} />}

            {/* Render child routes */}
            <Outlet />
          </Box>
        </Box>
      </Box>
    </>
  );
};

export default RootLayout;
