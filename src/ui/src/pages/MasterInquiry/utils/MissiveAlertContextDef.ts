import { createContext } from "react";
import { MissiveResponse } from "reduxstore/types";

export interface MissiveAlertContextType {
  missiveAlerts: MissiveResponse[];
  addAlert: (alert: MissiveResponse) => void;
  addAlerts: (alerts: MissiveResponse[]) => void;
  clearAlerts: () => void;
  removeAlert: (alertId: number) => void;
  hasAlert: (alertId: number) => boolean;
}

export const MissiveAlertContext = createContext<MissiveAlertContextType | null>(null);
