import { useCallback, useEffect, useMemo, useState } from "react";
import { useDynamicGridHeight } from "./useDynamicGridHeight";

/** Reserved space for pagination, header chrome, and padding when in expanded mode */
const EXPANDED_MODE_RESERVED_SPACE = 150;

interface UseContentAwareGridHeightOptions {
  /** Number of rows currently in the grid */
  rowCount: number;
  /** Height of each row in pixels. Default: 41 (AG Grid default) */
  rowHeight?: number;
  /** Height of the header row in pixels. Default: 41 */
  headerHeight?: number;
  /** Additional padding/chrome height (horizontal scrollbar, borders, etc). Default: 10 */
  chromeHeight?: number;
  /** Percentage of window height to use as maximum (0-1). Default: 0.4 (40%) */
  heightPercentage?: number;
  /** Minimum height in pixels when there are rows. Default: 100 */
  minHeight?: number;
  /** Maximum height in pixels. Default: 900 */
  maxHeight?: number;
  /** Whether the grid is in expanded/fullscreen mode. When true, uses viewport-based calculation without caps. */
  isExpanded?: boolean;
}

/**
 * Custom hook for calculating grid height that adapts to content.
 *
 * When there are few rows, the grid shrinks to fit the content.
 * When there are many rows, the grid uses a viewport-based maximum height with scrolling.
 *
 * This prevents excessive whitespace when displaying small result sets while
 * still constraining large result sets to a reasonable viewport portion.
 *
 * @param options Configuration options for height calculation
 * @returns The calculated height in pixels, or undefined for auto-height
 *
 * @example
 * ```tsx
 * // Basic usage - grid shrinks for small data sets
 * const gridHeight = useContentAwareGridHeight({ rowCount: data.length });
 *
 * // With custom row height
 * const gridHeight = useContentAwareGridHeight({
 *   rowCount: data.length,
 *   rowHeight: 50,
 *   heightPercentage: 0.5
 * });
 *
 * <DSMGrid maxHeight={gridHeight} ... />
 * ```
 */
export const useContentAwareGridHeight = (options: UseContentAwareGridHeightOptions): number | undefined => {
  const {
    rowCount,
    rowHeight = 41,
    headerHeight = 41,
    chromeHeight = 10,
    heightPercentage = 0.4,
    minHeight = 100,
    maxHeight = 900,
    isExpanded = false
  } = options;

  // For expanded mode, calculate viewport height directly without caps
  const [expandedMaxHeight, setExpandedMaxHeight] = useState<number>(0);

  const calculateExpandedHeight = useCallback(() => {
    // In expanded mode, use viewport height minus reserved space for UI chrome
    const viewportHeight = window.innerHeight;
    return Math.max(viewportHeight - EXPANDED_MODE_RESERVED_SPACE, minHeight);
  }, [minHeight]);

  useEffect(() => {
    if (isExpanded) {
      const updateHeight = () => {
        setExpandedMaxHeight(calculateExpandedHeight());
      };
      updateHeight();
      window.addEventListener("resize", updateHeight);
      return () => window.removeEventListener("resize", updateHeight);
    }
  }, [isExpanded, calculateExpandedHeight]);

  // Get the viewport-based maximum height for non-expanded mode
  const viewportMaxHeight = useDynamicGridHeight({
    heightPercentage,
    minHeight,
    maxHeight
  });

  // Calculate content-aware height
  const contentAwareHeight = useMemo(() => {
    // Calculate the height needed to show all rows
    const contentHeight = headerHeight + rowCount * rowHeight + chromeHeight;

    // Determine the maximum available height based on expanded state
    const availableMaxHeight = isExpanded ? expandedMaxHeight : viewportMaxHeight;

    // If content fits within available max, use content height (but respect minHeight)
    // If content exceeds available max, use available max (enables scrolling)
    if (contentHeight < availableMaxHeight) {
      // Content is smaller than available allocation - shrink to fit
      // But don't go below minimum height
      return Math.max(contentHeight, minHeight);
    }

    // Content exceeds available allocation - use available max with scrolling
    return availableMaxHeight;
  }, [rowCount, rowHeight, headerHeight, chromeHeight, viewportMaxHeight, expandedMaxHeight, isExpanded, minHeight]);

  // Return undefined for auto-height when there are no rows
  // This lets the grid show "No records found" message properly
  if (rowCount === 0) {
    return undefined;
  }

  return contentAwareHeight;
};

export default useContentAwareGridHeight;
