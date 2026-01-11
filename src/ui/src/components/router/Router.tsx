import { useMemo } from "react";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import { store } from "../../reduxstore/store";
import { createRoutes } from "./routeConfig";
import { withOktaSecurity } from "./RouteSecurity";

/* eslint-disable react-refresh/only-export-components */

/**
 * Root router component for React Router v7 data router.
 *
 * Replaces legacy BrowserRouter pattern with createBrowserRouter for:
 * - Loader-based data fetching before rendering
 * - Error boundaries at route level
 * - Better TypeScript integration with route definitions
 *
 * IMPORTANT: Router is created with useMemo to maintain stable reference
 * across re-renders. Store is imported directly to avoid React hook type issues.
 */
const Router = () => {
  // Create router with stable reference using direct store import
  // This avoids TypeScript issues with useStore hook while maintaining loader access
  const router = useMemo(() => createBrowserRouter(createRoutes(store)), []);

  return <RouterProvider router={router} />;
};

// Export Router wrapped with Okta Security HOC
export default withOktaSecurity(Router);
