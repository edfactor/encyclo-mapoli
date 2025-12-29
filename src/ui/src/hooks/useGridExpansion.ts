import { useCallback, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { closeDrawer, openDrawer, setFullscreen } from "../reduxstore/slices/generalSlice";
import { RootState } from "../reduxstore/store";

/**
 * Hook for managing grid expansion state with drawer and fullscreen coordination.
 *
 * When a grid is expanded:
 * - The drawer is closed (if open)
 * - Fullscreen mode is enabled
 * - The previous drawer state is remembered
 *
 * When collapsed:
 * - Fullscreen mode is disabled
 * - The drawer is restored to its previous state
 *
 * @returns Object containing:
 * - isGridExpanded: Current expansion state
 * - handleToggleGridExpand: Function to toggle expansion
 */
export const useGridExpansion = () => {
  const dispatch = useDispatch();
  const isDrawerOpen = useSelector((state: RootState) => state.general.isDrawerOpen);
  const [isGridExpanded, setIsGridExpanded] = useState(false);
  const [wasDrawerOpenBeforeExpand, setWasDrawerOpenBeforeExpand] = useState(false);

  const handleToggleGridExpand = useCallback(() => {
    setIsGridExpanded((prev) => {
      if (!prev) {
        // Expanding: remember current drawer state and close it
        setWasDrawerOpenBeforeExpand(isDrawerOpen ?? false);
        dispatch(closeDrawer());
        dispatch(setFullscreen(true));
      } else {
        // Collapsing: restore previous drawer state
        dispatch(setFullscreen(false));
        if (wasDrawerOpenBeforeExpand) {
          dispatch(openDrawer());
        }
      }
      return !prev;
    });
  }, [dispatch, isDrawerOpen, wasDrawerOpenBeforeExpand]);

  return {
    isGridExpanded,
    handleToggleGridExpand
  };
};

export default useGridExpansion;
