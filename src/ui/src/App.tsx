import React from "react";
import Router from "./components/router/Router";
import { DSMLayout, themeOptions } from "smart-ui-library";
import Logout from "components/Logout/Logout";
import { createTheme, ThemeProvider } from "@mui/material";
import "smart-ui-library/dist/smart-ui-library.css";
import buildInfo from "./.buildinfo.json";

const App = () => {
  const onClick = (e: any) => {};
  const theme = createTheme(themeOptions);
  return (
    <ThemeProvider theme={theme}>
      <DSMLayout
        onClick={onClick}
        appTitle="Profit Sharing"
        Logout={<Logout />}
        versionNmber={`${buildInfo.BuildNumber}.${buildInfo.BuildId}`}
        username={"TEST"}>
        <Router />
      </DSMLayout>
    </ThemeProvider>
  );
};

export default App;
