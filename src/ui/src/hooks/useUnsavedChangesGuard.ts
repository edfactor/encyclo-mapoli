import { useContext, useEffect } from "react";
import { UNSAFE_NavigationContext as NavigationContext } from "react-router-dom";

const UNSAVED_CHANGES_MESSAGE = "Please save your changes. Do you want to leave without saving?";

/**
 * Navigation guard that prevents accidental navigation when there are unsaved changes.
 * Works with legacy BrowserRouter (does not require data router).
 */
export const useUnsavedChangesGuard = (hasUnsavedChanges: boolean) => {
  const { navigator } = useContext(NavigationContext);

  // Block React Router SPA navigation (navigate(), <Link>, etc.)
  useEffect(() => {
    if (!hasUnsavedChanges) return;

    // Override the navigator's push and replace methods
    const originalPush = navigator.push;
    const originalReplace = navigator.replace;

    const blockNavigation = (originalFn: typeof navigator.push) => {
      return (...args: Parameters<typeof navigator.push>) => {
        const userConfirmed = window.confirm(UNSAVED_CHANGES_MESSAGE);
        if (userConfirmed) {
          originalFn.apply(navigator, args);
        }
      };
    };

    navigator.push = blockNavigation(originalPush);
    navigator.replace = blockNavigation(originalReplace);

    return () => {
      navigator.push = originalPush;
      navigator.replace = originalReplace;
    };
  }, [hasUnsavedChanges, navigator]);

  // Handle browser back/forward buttons
  useEffect(() => {
    if (!hasUnsavedChanges) return;

    const handlePopState = () => {
      const userConfirmed = window.confirm(UNSAVED_CHANGES_MESSAGE);
      if (!userConfirmed) {
        // Push current state back to prevent navigation
        window.history.pushState(null, "", window.location.href);
      }
    };

    // Push initial state so we can detect back button
    window.history.pushState(null, "", window.location.href);
    window.addEventListener("popstate", handlePopState);

    return () => {
      window.removeEventListener("popstate", handlePopState);
    };
  }, [hasUnsavedChanges]);

  // Handle browser navigation (refresh, close tab)
  useEffect(() => {
    if (!hasUnsavedChanges) return;

    const handleBeforeUnload = (e: BeforeUnloadEvent) => {
      e.preventDefault();
      e.returnValue = "Please save your changes.";
      return "Please save your changes.";
    };

    window.addEventListener("beforeunload", handleBeforeUnload);
    return () => window.removeEventListener("beforeunload", handleBeforeUnload);
  }, [hasUnsavedChanges]);
};
