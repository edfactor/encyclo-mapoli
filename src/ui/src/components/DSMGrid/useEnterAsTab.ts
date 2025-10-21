import { CellKeyDownEvent, FullWidthCellKeyDownEvent } from "ag-grid-community";
import { useEffect } from "react";

export interface UseNumpadEnterAsTabOptions {
  /** LocalStorage key for the enabled flag (`"true"` to enable). */
  enabledStorageKey?: string;
  /** Root under which to query focusable elements. Defaults to `document`. */
  scopeRoot?: Document | HTMLElement;
  /** CSS selector for focusable elements. */
  selector?: string;
}

export interface FocusNextElementOptions {
  /** Root under which to query focusable elements. Defaults to `document`. */
  scopeRoot?: Document | HTMLElement;
  /** CSS selector for focusable elements. */
  selector?: string;
  /** Current element to shift focus from, in case it is not picked up by CSS selectors */
  activeElement?: HTMLElement | null;
  /** Whether or not to exclude grid cells from the focus search */
  excludeGridCells?: boolean;
  /** Whether to move focus backwards (previous element) instead of forwards (next element) */
  backwardTab?: boolean;
}

let localStorageKey = "";

/**
 * Custom hook to enable numpad enter key as tab key behavior.
 *
 * When numpad enter is pressed, focus will move to the next focusable element
 * in the DOM order, similar to pressing the tab key. Shift+numpad enter moves
 * focus to the previous focusable element.
 */
export const useNumpadEnterAsTab = ({
  enabledStorageKey = "numpad-enter-as-tab-enabled",
  scopeRoot = document,
  selector = ""
}: UseNumpadEnterAsTabOptions = {}): void => {
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      // Only intercept NumpadEnter
      if (e.code !== "NumpadEnter") {
        return;
      }

      // Feature flag check
      if (window.localStorage.getItem(enabledStorageKey) !== "true") {
        return;
      } else {
        if (enabledStorageKey && localStorageKey !== enabledStorageKey) {
          localStorageKey = enabledStorageKey;
        }
      }

      const active = e.target as HTMLElement | null;

      const tag = active?.tagName.toLowerCase();
      if (tag === "textarea") {
        return;
      }

      if (!(e.shiftKey || e.altKey)) {
        // SPECIAL CASES: let default behavior occur
        if (tag === "button" || (tag === "input" && /^(submit)$/i.test((active as HTMLInputElement).type))) {
          return;
        }
        if (tag === "a" && active?.hasAttribute("href")) {
          return;
        }
      }

      const className = active?.className ?? "";
      if (className.includes("ag-cell") || className.includes("ag-header-cell")) {
        return;
      }

      // Prevent default Enter behavior
      e.preventDefault();
      e.stopPropagation();

      focusNextElement({ scopeRoot: scopeRoot, selector: selector ? selector : undefined, backwardTab: e.shiftKey });
    };

    // Attach global listener
    window.addEventListener("keydown", handleKeyDown, true);

    // Cleanup
    return () => {
      window.removeEventListener("keydown", handleKeyDown, true);
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [enabledStorageKey]);
};

/**
 * Focus the next focusable element in DOM order, forwards or backwards, as specified.
 * Wraps around at the ends. Excludes elements that are hidden, disabled, or have negative
 * tabindex. By default, excludes AG Grid cells to allow the onCellKeyDownEventHandler to
 * manage in grid navigation.
 */
export const focusNextElement = ({
  scopeRoot = document,
  selector = [
    '[tabindex]:not([tabindex="-1"]):not(.menubar * ):not(.MuiBreadcrumbs-root *)',
    "input:not([type=hidden]):not(.menubar * ):not(.MuiBreadcrumbs-root *)",
    "select:not(.menubar * ):not(.MuiBreadcrumbs-root *)",
    "textarea:not(.menubar * ):not(.MuiBreadcrumbs-root *)",
    "button:not(.menubar * ):not(.MuiBreadcrumbs-root *)",
    "a[href]:not(.menubar * ):not(.MuiBreadcrumbs-root *)",
    '[contenteditable="true"]:not(.menubar * ):not(.MuiBreadcrumbs-root *)',
    ".ag-root:not(.menubar * ):not(.MuiBreadcrumbs-root *)"
  ].join(","),
  activeElement = document.activeElement as HTMLElement | null,
  excludeGridCells = true,
  backwardTab = false
}: FocusNextElementOptions = {}): void => {
  // If no explicit scopeRoot, check for open MUI modal/dialog and use it as root
  // to capture focus within the modal context
  let root: Document | HTMLElement = scopeRoot ?? document;
  const modals = Array.from(
    document.querySelectorAll<HTMLElement>(".MuiDialog-container, .MuiDialog-scrollPaper")
  ).filter(
    (el) => window.getComputedStyle(el).visibility !== "hidden" && window.getComputedStyle(el).display !== "none"
  );
  if (modals.length > 0) {
    // Use the last (topmost) modal as the root
    root = modals[modals.length - 1];
  }

  // Query and filter focusable elements
  let all = Array.from(root.querySelectorAll<HTMLElement>(selector)).filter((el) => {
    // Skip non-focusable elements
    if (el.tabIndex < 0) {
      return false;
    }

    // Check if the element or any ancestor is hidden or has .ag-hidden
    let current: HTMLElement | null = el;
    while (current && current !== root) {
      const style = window.getComputedStyle(current);

      if (style.visibility === "hidden" || style.display === "none" || current.classList.contains("ag-hidden")) {
        return false;
      }

      current = current.parentElement;
    }

    // Ignore disabled or read-only elements
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    if (("disabled" in el && (el as any).disabled) || ("readOnly" in el && (el as any).readOnly)) {
      return false;
    }

    // Exclude AG Grid cells from tabbing, let the cell keydown event handler manage those
    if (excludeGridCells && el.classList.contains("ag-cell")) {
      return false;
    }

    return true;
  });

  // Ensure activeElement is included if not already present, used primarily for exiting AG Grid cell navigation
  if (activeElement && !all.includes(activeElement)) {
    all.push(activeElement);
  }

  if (all.length === 0) {
    return;
  }

  // Check for positive tabIndexes, and if set, limit active elements to those with a positive tabIndex
  // For custom restricted workflows
  const positiveTabIndexElements = all.filter((el) => el.tabIndex > 0);
  if (positiveTabIndexElements.length > 0) {
    all = positiveTabIndexElements;
  }

  // Sort by tabIndex, then by DOM order
  all.sort((a, b) => {
    // Handle activeElement AG Grid cells with negative tabIndex
    const ta = a === activeElement && a.tabIndex < 0 ? 0 : a.tabIndex;
    const tb = b === activeElement && b.tabIndex < 0 ? 0 : b.tabIndex;

    if (ta !== tb) {
      return ta - tb;
    }

    // Requires a bitwise and operation to determine the DOM order
    return a.compareDocumentPosition(b) & Node.DOCUMENT_POSITION_PRECEDING ? 1 : -1;
  });

  // Determine next element to focus
  const active = activeElement || (document.activeElement as HTMLElement | null);
  const currentIndex = all.findIndex((el) => el === active);

  let nextEl: HTMLElement;
  if (currentIndex < 0) {
    // Nothing focused or focused element not in list so go to first element
    nextEl = all[0];
  } else {
    // Wrap to next element, forward or backward
    const direction = backwardTab ? -1 : 1; // Shift+Enter goes to previous, Enter goes to next
    const length = all.length;

    // Add length and mod by it to handle wrapping at ends
    nextEl = all[(currentIndex + direction + length) % length];
  }

  nextEl.focus();
};

/**
 * Event handler to be registered with AG Grid's `onCellKeyDown` props, to allow AG Grid
 * to handle numpad enter key presses while focus is within the grid, before handing off
 * control back to the primary useEnterAsTab hook elsewhere on the page.
 *
 * @param event AG Grid cell key down event
 */
export const onCellKeyDownEventHandler = (event: CellKeyDownEvent | FullWidthCellKeyDownEvent): void => {
  // Feature flag check
  if (window.localStorage.getItem(localStorageKey) !== "true") {
    return;
  }

  const keyEvent = event.event as KeyboardEvent;
  if (keyEvent.code === "NumpadEnter") {
    keyEvent.preventDefault();
    keyEvent.stopPropagation();
    event.api.stopEditing();

    // Check for shift key press to determine direction
    if (keyEvent.shiftKey) {
      // If on the first cell, move focus backwards out of the grid, attempting to handle the case of pinned top rows
      if (
        ((event.api.getPinnedTopRowCount() === 0 &&
          event.api.getFocusedCell()?.rowIndex === 0 &&
          event.rowPinned !== "bottom") ||
          (event.api.getPinnedTopRowCount() === 1 && event.rowPinned === "top")) &&
        event.api.getFocusedCell()?.column.getColId() === event.api.getAllDisplayedColumns()[0].getColId()
      ) {
        focusNextElement({
          activeElement: keyEvent.target as HTMLElement | null,
          excludeGridCells: false,
          backwardTab: true
        });
      }
      // Otherwise, have AG Grid move to the previous cell
      else {
        event.api.tabToPreviousCell();
      }
    }
    // If on the last cell, move focus forwards out of the grid, attempting to handle the case of pinned bottom rows
    else if (
      ((event.api.getPinnedBottomRowCount() === 0 &&
        event.api.getFocusedCell()?.rowIndex === event.api.getDisplayedRowCount() - 1) ||
        (event.api.getPinnedBottomRowCount() === 1 && event.rowPinned === "bottom")) &&
      event.api.getFocusedCell()?.column.getColId() === event.api.getAllDisplayedColumns().slice(-1)[0].getColId()
    ) {
      keyEvent.preventDefault();
      keyEvent.stopPropagation();

      focusNextElement({
        activeElement: keyEvent.target as HTMLElement | null,
        excludeGridCells: false,
        backwardTab: false
      });
    }
    // Otherwise, have AG Grid move to the next cell
    else {
      event.api.tabToNextCell();
    }
  }
};

export default useNumpadEnterAsTab;
