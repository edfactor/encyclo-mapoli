import { useCallback, useContext, useEffect, useRef, useState } from "react";
import { UNSAFE_NavigationContext as NavigationContext } from "react-router-dom";
import { useNavigate } from "react-router-dom";

const UNSAVED_CHANGES_MESSAGE = "Please save your changes. Do you want to leave without saving?";

export interface UnsavedChangesGuardState {
  /** Whether the unsaved changes dialog should be shown */
  showDialog: boolean;
  /** Close the dialog and stay on the page */
  onStay: () => void;
  /** Close the dialog and proceed with navigation */
  onLeave: () => void;
}

/**
 * Navigation guard that prevents accidental navigation when there are unsaved changes.
 * Works with legacy BrowserRouter (does not require data router).
 *
 * @param hasUnsavedChanges - Whether there are unsaved changes to protect
 * @param useStyledDialog - If true, returns state for rendering a styled dialog instead of using window.confirm()
 * @returns State for the styled dialog (when useStyledDialog is true)
 */
export const useUnsavedChangesGuard = (
  hasUnsavedChanges: boolean,
  useStyledDialog: boolean = false
): UnsavedChangesGuardState => {
  const { navigator } = useContext(NavigationContext);
  const navigate = useNavigate();
  const [showDialog, setShowDialog] = useState(false);
  const pendingNavigationRef = useRef<{ to: string; replace: boolean } | null>(null);

  const onStay = useCallback(() => {
    setShowDialog(false);
    pendingNavigationRef.current = null;
  }, []);

  const onLeave = useCallback(() => {
    setShowDialog(false);
    const pending = pendingNavigationRef.current;
    pendingNavigationRef.current = null;

    if (pending) {
      // Temporarily disable guard by navigating directly
      navigate(pending.to, { replace: pending.replace });
    }
  }, [navigate]);

  // Block React Router SPA navigation (navigate(), <Link>, etc.)
  useEffect(() => {
    if (!hasUnsavedChanges) return;

    // Override the navigator's push and replace methods
    const originalPush = navigator.push;
    const originalReplace = navigator.replace;

    const blockNavigation = (originalFn: typeof navigator.push, isReplace: boolean) => {
      return (...args: Parameters<typeof navigator.push>) => {
        if (useStyledDialog) {
          // Store pending navigation and show dialog
          const to = typeof args[0] === "string" ? args[0] : (args[0] as { pathname?: string })?.pathname ?? "/";
          pendingNavigationRef.current = { to, replace: isReplace };
          setShowDialog(true);
        } else {
          // Use native confirm
          const userConfirmed = window.confirm(UNSAVED_CHANGES_MESSAGE);
          if (userConfirmed) {
            originalFn.apply(navigator, args);
          }
        }
      };
    };

    navigator.push = blockNavigation(originalPush, false);
    navigator.replace = blockNavigation(originalReplace, true);

    return () => {
      navigator.push = originalPush;
      navigator.replace = originalReplace;
    };
  }, [hasUnsavedChanges, navigator, useStyledDialog]);

  // Handle browser back/forward buttons
  useEffect(() => {
    if (!hasUnsavedChanges) return;

    const handlePopState = () => {
      if (useStyledDialog) {
        // For styled dialog, we can't easily intercept back button with async modal
        // Use native confirm for browser back/forward
        const userConfirmed = window.confirm(UNSAVED_CHANGES_MESSAGE);
        if (!userConfirmed) {
          window.history.pushState(null, "", window.location.href);
        }
      } else {
        const userConfirmed = window.confirm(UNSAVED_CHANGES_MESSAGE);
        if (!userConfirmed) {
          window.history.pushState(null, "", window.location.href);
        }
      }
    };

    // Push initial state so we can detect back button
    window.history.pushState(null, "", window.location.href);
    window.addEventListener("popstate", handlePopState);

    return () => {
      window.removeEventListener("popstate", handlePopState);
    };
  }, [hasUnsavedChanges, useStyledDialog]);

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

  return { showDialog, onStay, onLeave };
};
