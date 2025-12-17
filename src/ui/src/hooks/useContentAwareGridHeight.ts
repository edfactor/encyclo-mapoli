import { useMemo } from "react";
import { useDynamicGridHeight } from "./useDynamicGridHeight";

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
    maxHeight = 900
  } = options;

  // Get the viewport-based maximum height
  const viewportMaxHeight = useDynamicGridHeight({
    heightPercentage,
    minHeight,
    maxHeight
  });

  // Calculate content-aware height
  const contentAwareHeight = useMemo(() => {
    // Calculate the height needed to show all rows
    const contentHeight = headerHeight + rowCount * rowHeight + chromeHeight;

    // If content fits within viewport max, use content height (but respect minHeight)
    // If content exceeds viewport max, use viewport max (enables scrolling)
    if (contentHeight < viewportMaxHeight) {
      // Content is smaller than viewport allocation - shrink to fit
      // But don't go below minimum height
      return Math.max(contentHeight, minHeight);
    }

    // Content exceeds viewport allocation - use viewport max with scrolling
    return viewportMaxHeight;
  }, [rowCount, rowHeight, headerHeight, chromeHeight, viewportMaxHeight, minHeight]);

  // Return undefined for auto-height when there are no rows
  // This lets the grid show "No records found" message properly
  if (rowCount === 0) {
    return undefined;
  }

  return contentAwareHeight;
};

export default useContentAwareGridHeight;
