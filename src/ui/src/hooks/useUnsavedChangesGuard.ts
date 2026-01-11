import { useCallback, useEffect, useState } from "react";
import { useBlocker } from "react-router-dom";

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
 * Uses React Router v7 useBlocker API for SPA navigation and beforeunload for browser actions.
 *
 * @param hasUnsavedChanges - Whether there are unsaved changes to protect
 * @param useStyledDialog - If true, returns state for rendering a styled dialog instead of using window.confirm()
 * @returns State for the styled dialog (when useStyledDialog is true)
 */
export const useUnsavedChangesGuard = (
  hasUnsavedChanges: boolean,
  useStyledDialog: boolean = false
): UnsavedChangesGuardState => {
  const [showDialog, setShowDialog] = useState(false);

  // Block React Router v7 SPA navigation (navigate(), <Link>, etc.)
  // useBlocker takes a predicate function that returns true when navigation should be blocked
  const blocker = useBlocker(
    ({ currentLocation, nextLocation }) => hasUnsavedChanges && currentLocation.pathname !== nextLocation.pathname
  );

  const onStay = useCallback(() => {
    setShowDialog(false);
    if (blocker.state === "blocked") {
      blocker.reset(); // Reset the blocker to unblock
    }
  }, [blocker]);

  const onLeave = useCallback(() => {
    setShowDialog(false);
    if (blocker.state === "blocked") {
      blocker.proceed(); // Proceed with the blocked navigation
    }
  }, [blocker]);

  // Show styled dialog or native confirm when blocker is triggered
  useEffect(() => {
    if (blocker.state === "blocked") {
      if (useStyledDialog) {
        setShowDialog(true);
      } else {
        const userConfirmed = window.confirm(UNSAVED_CHANGES_MESSAGE);
        if (userConfirmed) {
          blocker.proceed();
        } else {
          blocker.reset();
        }
      }
    }
  }, [blocker, useStyledDialog]);

  // Handle browser navigation (refresh, close tab, back/forward buttons)
  // useBlocker only handles in-app React Router navigation, not browser chrome actions
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
