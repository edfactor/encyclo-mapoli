import { NavigationApi } from "../../reduxstore/api/NavigationApi";
import type { AppStore } from "../../reduxstore/store";
import type { NavigationResponseDto } from "../../reduxstore/types";

/**
 * Navigation loader for React Router v7 data router.
 *
 * CRITICAL: This loader MUST have an authentication token before proceeding.
 * If no token exists, it throws a 401 response to prevent unauthorized API calls.
 * The LoaderErrorBoundary will handle this and automatically retry when token becomes available.
 *
 * @param store - Redux store instance for accessing auth state and dispatching RTK Query
 * @returns Navigation data with menu structure
 * @throws Response with 401 status if authentication token is missing
 */
export async function navigationLoader(store: AppStore, request?: Request): Promise<NavigationResponseDto> {
  const path = request ? new URL(request.url).pathname : undefined;

  // Skip token requirement for auth-centric routes so callback can complete
  if (path && (path.startsWith("/login") || path.includes("/login/callback"))) {
    return { navigation: [] };
  }

  // CRITICAL: Verify authentication token exists before making API call
  const token = store.getState().security.token;

  if (!token) {
    throw new Response("Unauthorized: Authentication token required", {
      status: 401,
      statusText: "Unauthorized"
    });
  }

  // Use RTK Query imperative API to fetch navigation data
  try {
    // RTK Query dispatch returns a thunk result with unwrap()
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const queryAction = (store.dispatch as any)(
      NavigationApi.endpoints.getNavigation.initiate({ navigationId: undefined }, { forceRefetch: false })
    );

    // unwrap() returns a promise with the actual data
    const data: NavigationResponseDto = await queryAction.unwrap();

    return data;
  } catch (error: unknown) {
    const err = error as { status?: number; statusText?: string };
    // Check if it's a 401 authentication error
    if (err?.status === 401) {
      throw new Response("Unauthorized", {
        status: 401,
        statusText: "Unauthorized"
      });
    }

    throw new Response("Failed to load navigation data", {
      status: err?.status || 500,
      statusText: err?.statusText || "Internal Server Error"
    });
  }
}
