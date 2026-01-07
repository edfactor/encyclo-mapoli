import { Grid } from "@mui/material";
import React from "react";
import { useSelector } from "react-redux";
import { EnvironmentBanner, IEnvironmentBannerProps } from "smart-ui-library";
import { useGetFakeTimeStatusQuery } from "../../reduxstore/api/ItOperationsApi";
import { RootState } from "../../reduxstore/store";
import WelcomeDisplay, { WelcomeDisplayProps } from "../Layout/WelcomeDisplay";
import FakeTimeBanner from "./FakeTimeBanner";

export interface DSMLayoutProps extends IEnvironmentBannerProps, WelcomeDisplayProps {
  children: React.ReactNode;
  appTitle: string;
  onClick?: React.MouseEventHandler<HTMLDivElement>;
}

export const PSLayout: React.FC<DSMLayoutProps> = ({
  appTitle,
  children,
  onClick,
  environmentMode,
  buildVersionNumber,
  apiStatus,
  apiStatusMessage,
  ...welcomeDisplayProps
}) => {
  const isFullscreen = useSelector((state: RootState) => state.general.isFullscreen);
  const hasToken = !!useSelector((state: RootState) => state.security.token);

  // Check if fake time banner will be displayed (skip API call if not authenticated)
  const { data: fakeTimeStatus } = useGetFakeTimeStatusQuery(undefined, {
    skip: !hasToken,
    pollingInterval: 60000
  });

  const isFakeTimeBannerVisible = hasToken && fakeTimeStatus?.isActive;
  const fakeTimeBannerHeight = 24; // Approximate height of fake time banner in pixels

  // Adjust positions when fake time banner is visible
  const appBannerTop = isFakeTimeBannerVisible ? 52 + fakeTimeBannerHeight : 52;
  const contentMarginTop = isFakeTimeBannerVisible ? 110 + fakeTimeBannerHeight : 110;

  return (
    <div
      onClick={onClick}
      className="flex flex-col">
      {/* THIS IS THE LIGHT BLUE BANNER AT VERY TOP */}
      {!isFullscreen && (
        <div
          id="environment-banner"
          className="fixed top-0 z-[1000] w-full">
          <EnvironmentBanner
            data-testid="environment-banner"
            id="environment-banner"
            environmentMode={environmentMode}
            buildVersionNumber={buildVersionNumber}
            apiStatus={apiStatus}
            apiStatusMessage={apiStatusMessage}
          />
          {/* Fake Time Warning Banner - appears below environment banner when active */}
          <FakeTimeBanner />
        </div>
      )}
      <Grid
        id="top-level-all-contentbit-alert-container"
        container
        sx={{
          marginTop: isFullscreen ? "0px" : `${contentMarginTop}px`
        }}>
        {!isFullscreen && (
          <Grid
            id="app-banner-and-right-side-avatar-grid-container"
            size={12}
            sx={{
              position: "fixed",
              zIndex: 1000
            }}>
            <div className="app-banner fixed bg-white w-full" style={{ top: `${appBannerTop}px` }}>
              <div className="text">{appTitle}</div>
              <WelcomeDisplay {...welcomeDisplayProps} />
            </div>
          </Grid>
        )}

        <Grid
          size={12}
          id="all-router-sub-assembly-ps-layout">
          {children}
        </Grid>
      </Grid>
    </div>
  );
};

export default PSLayout;
