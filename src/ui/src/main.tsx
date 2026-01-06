import React from "react";
import ReactDOM from "react-dom/client";
import { Provider } from "react-redux";
import { store } from "reduxstore/store";
import App from "./App";
import "./styles/index.css";

const cleanupLegacyLocalStorageKeys = (): void => {
  // These keys are no longer used by the app, but may still exist in users' browsers.
  // Also clean up any accidental writes under an undefined storage key.
  const keysToRemove = [
    "selectedProfitYearForDecemberActivities",
    "selectedProfitYearForFiscalClose",
    "undefined",
    "undefined_pagination"
  ];

  for (const key of keysToRemove) {
    try {
      localStorage.removeItem(key);
    } catch {
      // ignore
    }
  }
};

cleanupLegacyLocalStorageKeys();

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <Provider store={store}>
      <App />
    </Provider>
  </React.StrictMode>
);
