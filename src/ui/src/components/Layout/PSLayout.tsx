import { Grid } from "@mui/material";
import React from "react";
import { useSelector } from "react-redux";
import { EnvironmentBanner, IEnvironmentBannerProps } from "smart-ui-library";
import { RootState } from "../../reduxstore/store";
import WelcomeDisplay, { WelcomeDisplayProps } from "../Layout/WelcomeDisplay";

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
        </div>
      )}
      <Grid
        id="top-level-all-contentbit-alert-container"
        container
        sx={{
          marginTop: isFullscreen ? "0px" : "110px"
        }}>
        {!isFullscreen && (
          <Grid
            id="app-banner-and-right-side-avatar-grid-container"
            size={12}
            sx={{
              position: "fixed",
              zIndex: 1000
            }}>
            <div className="app-banner fixed top-[52px] w-full bg-white">
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
