import { useEffect } from "react";

const UNSAVED_CHANGES_MESSAGE = "Please save your changes. Do you want to leave without saving?";

export const useUnsavedChangesGuard = (hasUnsavedChanges: boolean) => {
  // Handle browser navigation (refresh, close tab)
  useEffect(() => {
    const handleBeforeUnload = (e: BeforeUnloadEvent) => {
      if (hasUnsavedChanges) {
        e.preventDefault();
        e.returnValue = "Please save your changes.";
        return "Please save your changes.";
      }
    };

    window.addEventListener("beforeunload", handleBeforeUnload);
    return () => window.removeEventListener("beforeunload", handleBeforeUnload);
  }, [hasUnsavedChanges]);

  // Handle browser back/forward buttons
  useEffect(() => {
    if (!hasUnsavedChanges) return;

    const handlePopState = (event: PopStateEvent) => {
      if (hasUnsavedChanges) {
        const userConfirmed = window.confirm(UNSAVED_CHANGES_MESSAGE);
        if (!userConfirmed) {
          window.history.pushState(null, "", window.location.href);
          event.preventDefault();
        }
      }
    };

    // Push current state to enable detection of back button
    window.history.pushState(null, "", window.location.href);
    window.addEventListener("popstate", handlePopState);

    return () => {
      window.removeEventListener("popstate", handlePopState);
    };
  }, [hasUnsavedChanges]);

  // Handle navigation through links and buttons
  useEffect(() => {
    if (!hasUnsavedChanges) return;

    const handleClick = (event: Event) => {
      const target = event.target as HTMLElement;
      const link = target.closest('a, [role="button"], button');

      if (link && hasUnsavedChanges) {
        const href = link.getAttribute("href");

        if (href && href !== window.location.pathname && href !== "#") {
          const userConfirmed = window.confirm(UNSAVED_CHANGES_MESSAGE);
          if (!userConfirmed) {
            event.preventDefault();
            event.stopPropagation();
            return false;
          }
        }
      }
    };

    document.addEventListener("click", handleClick, true);

    return () => {
      document.removeEventListener("click", handleClick, true);
    };
  }, [hasUnsavedChanges]);
};