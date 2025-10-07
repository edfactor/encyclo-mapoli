import { renderHook } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { MissiveAlertContext, MissiveAlertContextType } from "../components/MissiveAlerts/MissiveAlertContextDef";
import { useMissiveAlerts } from "./useMissiveAlerts";

describe("useMissiveAlerts", () => {
  const mockContextValue: MissiveAlertContextType = {
    missiveAlerts: [],
    addAlert: () => {},
    addAlerts: () => {},
    removeAlert: () => {},
    clearAlerts: () => {},
    hasAlert: () => false
  };

  const wrapper =
    (contextValue: MissiveAlertContextType | undefined) =>
    ({ children }: { children: React.ReactNode }) => (
      <MissiveAlertContext.Provider value={contextValue as any}>{children}</MissiveAlertContext.Provider>
    );

  it("should return context value when used within provider", () => {
    const { result } = renderHook(() => useMissiveAlerts(), {
      wrapper: wrapper(mockContextValue)
    });

    expect(result.current).toBe(mockContextValue);
  });

  it("should throw error when used outside provider", () => {
    // Suppress console.error for this test
    const originalError = console.error;
    console.error = () => {};

    expect(() => {
      renderHook(() => useMissiveAlerts(), {
        wrapper: wrapper(undefined)
      });
    }).toThrow("useMissiveAlerts must be used within a MissiveAlertProvider");

    console.error = originalError;
  });

  it("should return the full context API", () => {
    const contextValue: MissiveAlertContextType = {
      missiveAlerts: [{ id: 1, message: "Test alert", severity: "info" as const }],
      addAlert: () => {},
      addAlerts: () => {},
      removeAlert: () => {},
      clearAlerts: () => {},
      hasAlert: () => false
    };

    const { result } = renderHook(() => useMissiveAlerts(), {
      wrapper: wrapper(contextValue)
    });

    expect(result.current.missiveAlerts).toHaveLength(1);
    expect(result.current.addAlert).toBeDefined();
    expect(result.current.addAlerts).toBeDefined();
    expect(result.current.removeAlert).toBeDefined();
    expect(result.current.clearAlerts).toBeDefined();
    expect(result.current.hasAlert).toBeDefined();
  });
});
