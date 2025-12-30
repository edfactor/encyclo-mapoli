import { useCallback, useEffect, useRef, useState } from "react";

/**
 * Hook to manage tooltip open/close state with a delayed close behavior.
 * This prevents the tooltip from closing immediately when the mouse briefly leaves,
 * providing a smoother user experience for hover-based tooltips.
 *
 * @param closeDelay - Delay in milliseconds before the tooltip closes (default: 100ms)
 * @returns Object with isOpen state and open/close handlers
 */
export const useTooltipState = (closeDelay: number = 100) => {
  const [isOpen, setIsOpen] = useState(false);
  const timeoutRef = useRef<NodeJS.Timeout | null>(null);

  const handleOpen = useCallback(() => {
    if (timeoutRef.current) {
      clearTimeout(timeoutRef.current);
      timeoutRef.current = null;
    }
    setIsOpen(true);
  }, []);

  const handleClose = useCallback(() => {
    timeoutRef.current = setTimeout(() => {
      setIsOpen(false);
    }, closeDelay);
  }, [closeDelay]);

  // Cleanup timeout on unmount
  useEffect(() => {
    return () => {
      if (timeoutRef.current) {
        clearTimeout(timeoutRef.current);
      }
    };
  }, []);

  return {
    isOpen,
    handleOpen,
    handleClose
  };
};

export default useTooltipState;
