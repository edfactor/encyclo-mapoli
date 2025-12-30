import { useCallback, useState } from "react";

/**
 * Hook for managing initial load/search state.
 *
 * This is commonly used in search-driven pages to track whether
 * the initial search has been performed. This helps:
 * - Show/hide grids based on whether a search has occurred
 * - Prevent duplicate initial fetches
 * - Control initial loading states
 *
 * @param initialValue - Starting value for the load state (default: false)
 * @returns Object containing:
 * - isLoaded: Current load state
 * - setLoaded: Function to mark as loaded
 * - reset: Function to reset to initial state
 */
export const useInitialLoad = (initialValue = false) => {
  const [isLoaded, setIsLoaded] = useState(initialValue);

  const setLoaded = useCallback((loaded = true) => {
    setIsLoaded(loaded);
  }, []);

  const reset = useCallback(() => {
    setIsLoaded(initialValue);
  }, [initialValue]);

  return {
    isLoaded,
    setLoaded,
    reset
  };
};

export default useInitialLoad;
