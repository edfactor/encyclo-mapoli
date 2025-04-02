import { createTheme, ThemeProvider } from "@mui/material";
import AppErrorBoundary from "components/ErrorBoundary";
import PSLayout from "components/Layout/PSLayout";
import { themeOptions } from "smart-ui-library";
import "smart-ui-library/dist/smart-ui-library.css";
import "../agGridConfig";
import Router from "./components/router/Router";
import { useEffect, useState } from "react";

const defaultBuildInfo = {
  BuildNumber: "Local",
  BuildId: "Dev",
  Branch: "Local Development",
  CommitHash: ""
};

const App = () => {
  const [buildInfo, setBuildInfo] = useState(defaultBuildInfo);

  useEffect(() => {
    fetch('/.buildinfo.json')
      .then(response => {
        if (!response.ok) {
          throw new Error('Could not load buildinfo.json');
        }
        return response.json();
      })
      .then(data => {
        setBuildInfo(data);
      })
      .catch(error => {
        console.warn("Error loading buildInfo:", error);
        console.log("Using default buildInfo:", defaultBuildInfo);
      });
  }, []);

  const onClick = (_e: React.MouseEvent<HTMLDivElement>) => {};

  const theme = createTheme(themeOptions);
  return (
    <ThemeProvider theme={theme}>
      <PSLayout
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
      </PSLayout>
    </ThemeProvider>
  );
};

export default App;
