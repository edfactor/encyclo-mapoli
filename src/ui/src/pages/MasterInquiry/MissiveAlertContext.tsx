import React, { useState, useCallback, useMemo, ReactNode } from "react";
import { MissiveResponse } from "reduxstore/types";
import { MissiveAlertContext, MissiveAlertContextType } from "./MissiveAlertContextDef";

interface MissiveAlertProviderProps {
  children: ReactNode;
}

export const MissiveAlertProvider: React.FC<MissiveAlertProviderProps> = ({ children }) => {
  const [missiveAlerts, setMissiveAlerts] = useState<MissiveResponse[]>([]);

  const addAlert = useCallback((alert: MissiveResponse) => {
    setMissiveAlerts((prev) => {
      // Avoid duplicates by checking if alert with same ID already exists
      const exists = prev.some((existing) => existing.id === alert.id);
      if (exists) {
        return prev;
      }
      return [...prev, alert];
    });
  }, []);

  const addAlerts = useCallback((alerts: MissiveResponse[]) => {
    setMissiveAlerts((prev) => {
      const newAlerts = alerts.filter((alert) => !prev.some((existing) => existing.id === alert.id));
      return [...prev, ...newAlerts];
    });
  }, []);

  const clearAlerts = useCallback(() => {
    setMissiveAlerts([]);
  }, []);

  const removeAlert = useCallback((alertId: number) => {
    setMissiveAlerts((prev) => prev.filter((alert) => alert.id !== alertId));
  }, []);

  const hasAlert = useCallback(
    (alertId: number) => {
      return missiveAlerts.some((alert) => alert.id === alertId);
    },
    [missiveAlerts]
  );

  const value = useMemo<MissiveAlertContextType>(
    () => ({
      missiveAlerts,
      addAlert,
      addAlerts,
      clearAlerts,
      removeAlert,
      hasAlert
    }),
    [missiveAlerts, addAlert, addAlerts, clearAlerts, removeAlert, hasAlert]
  );

  return <MissiveAlertContext.Provider value={value}>{children}</MissiveAlertContext.Provider>;
};
