import { useContext } from "react";
import { MissiveAlertContext, MissiveAlertContextType } from "../utils/MissiveAlertContextDef";

export const useMissiveAlerts = (): MissiveAlertContextType => {
  const context = useContext(MissiveAlertContext);
  if (!context) {
    throw new Error("useMissiveAlerts must be used within a MissiveAlertProvider");
  }
  return context;
};
