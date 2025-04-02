import { createTheme, ThemeProvider } from "@mui/material";
import AppErrorBoundary from "components/ErrorBoundary";
import PSLayout from "components/Layout/PSLayout";
import { themeOptions } from "smart-ui-library";
import "smart-ui-library/dist/smart-ui-library.css";
import "../agGridConfig";
import Router from "./components/router/Router";
import { useEffect, useState } from "react";

interface BuildInfo {
  buildNumber?: string;
  buildId?: string;
  branch?: string;
  commitHash?: string;
}

const App = () => {
  const [uiBuildInfo, setUiBuildInfo] = useState<BuildInfo | null>(null);

  useEffect(() => {
    if (!uiBuildInfo) {
      fetch("/.buildinfo.json").then(async (response) => {
        try {
          const data = await response.json();
          setUiBuildInfo(data);
        } catch (e) {
          console.warn("Error parsing buildinfo.json");
        }
      });
    }
  }, [uiBuildInfo]);

  const buildVersionNumber = uiBuildInfo 
    ? `${uiBuildInfo.buildNumber ?? ""}.${uiBuildInfo.buildId ?? ""}`
    : "Local.Dev";

  const onClick = (_e: React.MouseEvent<HTMLDivElement>) => {};

  const theme = createTheme(themeOptions);
  return (
    <ThemeProvider theme={theme}>
      <PSLayout
        onClick={onClick}
        appTitle="Profit Sharing"
        logout={() => alert("Logout")}
        buildVersionNumber={buildVersionNumber}
        userName={"TEST"}
        environmentMode={"development"}
        oktaEnabled={true}>
        <AppErrorBoundary>
          <Router />
        </AppErrorBoundary>
      </PSLayout>
    </ThemeProvider>
  );
};

export default App;
