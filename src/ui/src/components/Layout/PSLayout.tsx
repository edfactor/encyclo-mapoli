import Grid from "@mui/material/Grid";
import React from "react";
import "./PSLayout.css";
import EnvironmentBanner, { IEnvironmentBannerProps } from "../Layout/EnvironmentBanner";
import WelcomeDisplay, { WelcomeDisplayProps } from "../Layout/WelcomeDisplay";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";

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
  ...welcomeDisplayProps
}) => {
  const { drawerOpen } = useSelector((state: RootState) => state.general);

  return (
    <div
      onClick={onClick}
      style={{
        height: "100%",
        width: drawerOpen ? "calc(100% - 320px)" : "calc(100% - 64px)",
        marginLeft: drawerOpen ? 320 : 64
      }}>
      <EnvironmentBanner
        data-testid="environment-banner"
        id="environment-banner"
        environmentMode={environmentMode}
        buildVersionNumber={buildVersionNumber}
      />
      <Grid
        container
        style={{
          height: "100%",
          margin: 0,
          width: "100%"
        }}>
        <Grid xs={12}>
          <div className="app-banner">
            <div className="text">{appTitle}</div>
            <WelcomeDisplay {...welcomeDisplayProps} />
          </div>
        </Grid>

        <Grid
          xs={12}
          style={{ height: "100%" }}>
          {children}
        </Grid>
      </Grid>
    </div>
  );
};

export default PSLayout;
