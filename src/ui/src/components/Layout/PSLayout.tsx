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
    pollingInterval: 120000
  });

  const isFakeTimeBannerVisible = hasToken && fakeTimeStatus?.isActive;
  const fakeTimeBannerHeight = 24; // Approximate height of fake time banner in pixels

  // EnvironmentBanner only renders for non-production environments
  const showsEnvironmentBanner = environmentMode && environmentMode !== "production";
  const envBannerHeight = showsEnvironmentBanner ? 52 : 0;

  // Adjust positions based on which banners are visible
  const appBannerTop = envBannerHeight + (isFakeTimeBannerVisible ? fakeTimeBannerHeight : 0);
  const menuBarTop = appBannerTop + 52; // 52px is app banner height
  const contentMarginTop = menuBarTop + 44; // 44px is approximate menu bar height

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
            <div
              className="fixed flex h-[52px] w-full items-center justify-between bg-dsm-app-banner px-5 shadow-[0px_2px_4px_-1px_rgba(0,0,0,0.2),0px_4px_5px_0px_rgba(0,0,0,0.14),0px_1px_10px_0px_rgba(0,0,0,0.12)]"
              style={{ top: `${appBannerTop}px` }}>
              <div className="font-lato text-[2rem] font-bold uppercase tracking-[0.4rem] text-[#db1532]">
                {appTitle}
              </div>
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
