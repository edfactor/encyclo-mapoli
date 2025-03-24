import Grid2 from "@mui/material/Grid2";
import React from "react";
import EnvironmentBanner, { IEnvironmentBannerProps } from "../Layout/EnvironmentBanner";
import WelcomeDisplay, { WelcomeDisplayProps } from "../Layout/WelcomeDisplay";
import "./PSLayout.css";
import zIndex from "@mui/material/styles/zIndex";

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
  //const { drawerOpen } = useSelector((state: RootState) => state.general);

  return (
    <div
      onClick={onClick}
      /*
      style={{
        height: "100%",
        width: drawerOpen ? "calc(100% - 320px)" : "calc(100% - 64px)",
        marginLeft: drawerOpen ? 320 : 64
      }}>
        */
    >
      {/* THIS IS THE LIGHT BLUE BANNER AT VERY TOP */}
      <div
        style={{
          position: "fixed",
          width: "100%",
          top: 0
        }}>
        <EnvironmentBanner
          data-testid="environment-banner"
          id="environment-banner"
          environmentMode={environmentMode}
          buildVersionNumber={buildVersionNumber}
        />
      </div>
      <Grid2
        container
        style={{
          marginTop: "110px"
        }}>
        <Grid2
          size={12}
          style={{
            position: "fixed"
          }}>
          <div
            className="app-banner"
            style={{ position: "fixed", top: "51px", backgroundColor: "white" }}>
            <div className="text">{appTitle}</div>
            <WelcomeDisplay {...welcomeDisplayProps} />
          </div>
        </Grid2>

        <Grid2 size={12}>{children}</Grid2>
      </Grid2>
    </div>
  );
};

export default PSLayout;
