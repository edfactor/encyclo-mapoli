import { createTheme, ThemeProvider } from "@mui/material";
import AppErrorBoundary from "components/ErrorBoundary";
import { DSMLayout, themeOptions } from "smart-ui-library";
import "smart-ui-library/dist/smart-ui-library.css";
import "../agGridConfig";
import buildInfo from "./.buildinfo.json";
import Router from "./components/router/Router";

const App = () => {
  const onClick = (e: any) => {};
  const theme = createTheme(themeOptions);
  return (
    <ThemeProvider theme={theme}>
      <DSMLayout
        onClick={onClick}
        appTitle="Profit Sharing"
        logout={() => alert("Logout")}
        buildVersionNumber={`${buildInfo.BuildNumber}.${buildInfo.BuildId}`}
        userName={"TEST"}
        environmentMode={"development"}
        oktaEnabled={true}>
        <AppErrorBoundary>
          <Router />
        </AppErrorBoundary>
      </DSMLayout>
    </ThemeProvider>
  );
};

export default App;
