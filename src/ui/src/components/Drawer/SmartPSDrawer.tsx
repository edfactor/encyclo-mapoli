/**
 * SmartPSDrawer - Auto-Detecting Drawer Wrapper
 *
 * Automatically detects which L0 navigation section to display based on:
 * 1. Current route path
 * 2. Active menu bar item
 *
 * This wrapper makes the drawer 100% dynamic - it shows the appropriate
 * navigation section automatically without requiring explicit configuration.
 *
 * Example:
 * - User at /master-inquiry → Shows INQUIRIES drawer
 * - User at /december-activities → Shows YEAR END drawer
 * - User at /distributions-inquiry → Shows DISTRIBUTIONS drawer
 */

import { FC, useMemo } from "react";
import { useLocation } from "react-router-dom";
import { ICommon } from "smart-ui-library";
import { NavigationResponseDto } from "../../reduxstore/types";
import { createDrawerConfig, getDefaultDrawerConfig } from "./models";
import PSDrawer from "./PSDrawer";
import { getL0NavigationForRoute } from "./utils";

export interface SmartPSDrawerProps extends ICommon {
  navigationData?: NavigationResponseDto;
}

/**
 * SmartPSDrawer - Automatically shows the correct navigation section
 *
 * No configuration needed - it detects which L0 section contains the current route
 * and displays that section's drawer items.
 */
const SmartPSDrawer: FC<SmartPSDrawerProps> = ({ navigationData }) => {
  const location = useLocation();

  /**
   * Automatically determine which drawer config to use based on current route
   */
  const drawerConfig = useMemo(() => {
    if (!navigationData?.navigation) {
      return getDefaultDrawerConfig();
    }

    // Find which L0 navigation item contains the current route
    const currentL0 = getL0NavigationForRoute(navigationData, location.pathname);

    if (currentL0?.title) {
      // Create config for the detected L0 section
      return createDrawerConfig(currentL0.title);
    }

    // Fallback to default (Year End) if route not found
    return getDefaultDrawerConfig();
  }, [navigationData, location.pathname]);

  return (
    <PSDrawer
      navigationData={navigationData}
      drawerConfig={drawerConfig}
    />
  );
};

export default SmartPSDrawer;
