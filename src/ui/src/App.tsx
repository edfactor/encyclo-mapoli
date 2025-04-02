import { createTheme, ThemeProvider } from "@mui/material";
import AppErrorBoundary from "components/ErrorBoundary";
import PSLayout from "components/Layout/PSLayout";
import { themeOptions } from "smart-ui-library";
import "smart-ui-library/dist/smart-ui-library.css";
import "../agGridConfig";
import Router from "./components/router/Router";

let buildInfo: { BuildNumber: string; BuildId: string; Branch: string; CommitHash: string };

try {
  buildInfo = require("./.buildinfo.json");
} catch (e) {
  buildInfo = {
    BuildNumber: "Local",
    BuildId: "Dev",
    Branch: "Local Development",
    CommitHash: ""
  };
}

const App = () => {
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
