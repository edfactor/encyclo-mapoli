import { useCallback, useEffect, useState } from "react";

interface UseDynamicGridHeightOptions {
  /** Percentage of window height to use (0-1). Default: 0.5 (50%) */
  heightPercentage?: number;
  /** Minimum height in pixels. Default: 300 */
  minHeight?: number;
  /** Maximum height in pixels. Default: 800 */
  maxHeight?: number;
}

/**
 * Custom hook for calculating and managing dynamic grid height based on window size.
 * Automatically recalculates on window resize and provides reasonable bounds.
 *
 * @param options Configuration options for height calculation
 * @returns The calculated maximum height in pixels
 *
 * @example
 * ```tsx
 * // Use default settings (40% of window height, 300-800px bounds)
 * const gridMaxHeight = useDynamicGridHeight();
 *
 * // Custom settings
 * const gridMaxHeight = useDynamicGridHeight({
 *   heightPercentage: 0.40, // 40% of window height
 *   minHeight: 400,
 *   maxHeight: 1000
 * });
 *
 * <DSMGrid maxHeight={gridMaxHeight} ... />
 * ```
 */
export const useDynamicGridHeight = (options: UseDynamicGridHeightOptions = {}) => {
  const {
    heightPercentage = 0.4, // Default to 40%
    minHeight = 300,
    maxHeight = 800
  } = options;

  const [gridMaxHeight, setGridMaxHeight] = useState(400);

  // Calculate dynamic grid height based on window size
  const calculateGridHeight = useCallback(() => {
    const windowHeight = window.innerHeight;
    const targetHeight = Math.floor(windowHeight * heightPercentage);

    // Apply bounds: ensure height stays within min/max limits
    return Math.max(minHeight, Math.min(maxHeight, targetHeight));
  }, [heightPercentage, minHeight, maxHeight]);

  // Initialize and update grid height on window resize
  useEffect(() => {
    const updateGridHeight = () => {
      setGridMaxHeight(calculateGridHeight());
    };

    // Set initial height
    updateGridHeight();

    // Add resize listener
    window.addEventListener("resize", updateGridHeight);

    // Cleanup
    return () => {
      window.removeEventListener("resize", updateGridHeight);
    };
  }, [calculateGridHeight]);

  return gridMaxHeight;
};
