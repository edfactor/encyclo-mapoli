import React from "react";
import Router from "./components/router/Router";
import { DSMLayout, themeOptions } from "smart-ui-library";
import Logout from "components/Logout/Logout";
import { createTheme, ThemeProvider } from "@mui/material";
import "smart-ui-library/dist/smart-ui-library.css";

const App = () => {
  const onClick = (e: any) => {};
  const theme = createTheme(themeOptions);
  return (
    <ThemeProvider theme={theme}>
      <DSMLayout
        onClick={onClick}
        appTitle="Profit Sharing"
        Logout={<Logout />}
        buildInfo={{ versionNumber: "1", buildNumber: "1" }}
        username={"TEST"}>
        <Router />
      </DSMLayout>
    </ThemeProvider>
  );
};

export default App;
