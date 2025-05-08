import Grid2 from "@mui/material/Grid2";
import React from "react";
import EnvironmentBanner, { IEnvironmentBannerProps } from "../Layout/EnvironmentBanner";
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
  return (
    <div onClick={onClick}>
      {/* THIS IS THE LIGHT BLUE BANNER AT VERY TOP */}
      <div
        id="environment-banner"
        style={{
          position: "fixed",
          width: "100%",
          top: 0,
          zIndex: 1000
        }}>
        <EnvironmentBanner
          data-testid="environment-banner"
          id="environment-banner"
          environmentMode={environmentMode}
          buildVersionNumber={buildVersionNumber}
          apiStatus={apiStatus}
          apiStatusMessage={apiStatusMessage}
        />
      </div>
      <Grid2
        id="top-level-all-contentbit-alert-container"
        container
        style={{
          marginTop: "110px"
        }}>
        <Grid2
          id="app-banner-and-right-side-avatar-grid-container"
          size={12}
          style={{
            position: "fixed",
            zIndex: 1000
          }}>
          <div
            className="app-banner"
            style={{ width: "100%", position: "fixed", top: "52px", backgroundColor: "white" }}>
            <div className="text">{appTitle}</div>
            <WelcomeDisplay {...welcomeDisplayProps} />
          </div>
        </Grid2>

        <Grid2
          size={12}
          id="all-router-sub-assembly-ps-layout">
          {children}
        </Grid2>
      </Grid2>
    </div>
  );
};

export default PSLayout;
