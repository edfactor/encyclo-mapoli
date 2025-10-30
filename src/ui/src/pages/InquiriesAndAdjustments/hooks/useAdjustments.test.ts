import { configureStore, type PreloadedState } from "@reduxjs/toolkit";
import { act, renderHook, waitFor } from "@testing-library/react";
import React from "react";
import { Provider } from "react-redux";
import { beforeEach, describe, expect, it, vi } from "vitest";
import inquiryReducer, { type InquiryState } from "../../../reduxstore/slices/inquirySlice";
import securityReducer, { type SecurityState } from "../../../reduxstore/slices/securitySlice";
import { useAdjustments } from "./useAdjustments";

// Create mock functions for triggers/actions
const mockMergeProfitsDetail = vi.fn();
const mockTriggerSearchMember = vi.fn();
const mockTriggerGetMemberDetails = vi.fn();
const mockMissiveAlertsHook = vi.fn();

// Mock the APIs
vi.mock("../../../reduxstore/api/AdjustmentsApi", () => ({
  useMergeProfitsDetailMutation: () => [mockMergeProfitsDetail, {}]
}));

vi.mock("../../../reduxstore/api/InquiryApi", () => ({
  useLazySearchProfitMasterInquiryQuery: () => [mockTriggerSearchMember, {}],
  useLazyGetProfitMasterInquiryMemberDetailsQuery: () => [mockTriggerGetMemberDetails, {}]
}));

// Mock the hooks
vi.mock("../../../hooks/useMissiveAlerts", () => ({
  useMissiveAlerts: mockMissiveAlertsHook
}));

const mockMember = {
  id: 1,
  badgeNumber: 123456,
  isEmployee: true,
  firstName: "John",
  lastName: "Doe"
};

const mockProfitDetails = {
  results: [
    {
      id: 1,
      year: 2023,
      amount: 1000
    },
    {
      id: 2,
      year: 2022,
      amount: 900
    }
  ],
  total: 2
};

type RootState = {
  security: SecurityState;
  inquiry: InquiryState;
};

type MockStoreState = PreloadedState<RootState>;

function createMockStore(preloadedState?: MockStoreState) {
  return configureStore<RootState>({
    reducer: {
      security: securityReducer,
      inquiry: inquiryReducer
    } as const,
    preloadedState: preloadedState as RootState | undefined
  });
}

function renderHookWithProvider<T>(hook: () => T, preloadedState?: MockStoreState) {
  const store = createMockStore(preloadedState || { security: { token: "mock-token", user: null }, inquiry: {} });
  return renderHook(() => hook(), {
    wrapper: ({ children }: { children: React.ReactNode }) =>
      React.createElement(Provider, { store, children })
  });
}

describe("useAdjustments", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("initial state", () => {
    it("should initialize with default values", () => {
      mockMissiveAlertsHook.mockReturnValue({
        missiveAlerts: [],
        addAlert: vi.fn(),
        clearAlerts: vi.fn()
      });

      const { result } = renderHookWithProvider(() => useAdjustments());

      expect(result.current.isMerging).toBe(false);
      expect(result.current.canMerge).toBe(false);
    });

    it("should expose executeSearch function", () => {
      mockMissiveAlertsHook.mockReturnValue({
        missiveAlerts: [],
        addAlert: vi.fn(),
        clearAlerts: vi.fn()
      });

      const { result } = renderHookWithProvider(() => useAdjustments());

      expect(typeof result.current.executeSearch).toBe("function");
    });

    it("should expose executeMerge function", () => {
      mockMissiveAlertsHook.mockReturnValue({
        missiveAlerts: [],
        addAlert: vi.fn(),
        clearAlerts: vi.fn()
      });

      const { result } = renderHookWithProvider(() => useAdjustments());

      expect(typeof result.current.executeMerge).toBe("function");
    });

    it("should expose resetSearch function", () => {
      mockMissiveAlertsHook.mockReturnValue({
        missiveAlerts: [],
        addAlert: vi.fn(),
        clearAlerts: vi.fn()
      });

      const { result } = renderHookWithProvider(() => useAdjustments());

      expect(typeof result.current.resetSearch).toBe("function");
    });
  });

  describe("executeSearch", () => {
    it("should call clearAlerts when search starts", async () => {
      const mockClearAlerts = vi.fn();
      const mockAddAlert = vi.fn();
      const mockTriggerSearch = vi.fn().mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({ results: [] })
      });

      mockTriggerSearchMember.mockReturnValue([mockTriggerSearch, {}]);
      mockTriggerGetMemberDetails.mockReturnValue([vi.fn(), {}]);

      mockMissiveAlertsHook.mockReturnValue({
        missiveAlerts: [],
        addAlert: mockAddAlert,
        clearAlerts: mockClearAlerts
      });

      const { result } = renderHookWithProvider(() => useAdjustments());

      await act(async () => {
        await result.current.executeSearch([123456789, 987654321], 2024);
      });

      expect(mockClearAlerts).toHaveBeenCalled();
    });

    it("should add alert when source SSN not found", async () => {
      const mockAddAlert = vi.fn();
      const mockTriggerSearch = vi.fn().mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({ results: [] })
      });

      mockTriggerSearchMember.mockReturnValue([mockTriggerSearch, {}]);
      mockTriggerGetMemberDetails.mockReturnValue([vi.fn(), {}]);

      mockMissiveAlertsHook.mockReturnValue({
        missiveAlerts: [],
        addAlert: mockAddAlert,
        clearAlerts: vi.fn()
      });

      const { result } = renderHookWithProvider(() => useAdjustments());

      await act(async () => {
        await result.current.executeSearch([123456789, 987654321], 2024);
      });

      expect(mockAddAlert).toHaveBeenCalledWith(
        expect.objectContaining({
          message: expect.stringContaining("No member found with SSN: 123456789")
        })
      );
    });

    it("should fetch profit details when member found", async () => {
      const mockUnwrapSearch = vi
        .fn()
        .mockResolvedValueOnce({ results: [mockMember] })
        .mockResolvedValueOnce({ results: [] });

      const mockUnwrapDetails = vi.fn().mockResolvedValue(mockProfitDetails);

      const mockTriggerSearch = vi.fn().mockReturnValue({
        unwrap: mockUnwrapSearch
      });

      const mockTriggerDetails = vi.fn().mockReturnValue({
        unwrap: mockUnwrapDetails
      });

      mockTriggerSearchMember.mockReturnValue([mockTriggerSearch, {}]);
      mockTriggerGetMemberDetails.mockReturnValue([mockTriggerDetails, {}]);

      mockMissiveAlertsHook.mockReturnValue({
        missiveAlerts: [],
        addAlert: vi.fn(),
        clearAlerts: vi.fn()
      });

      const { result } = renderHookWithProvider(() => useAdjustments());

      await act(async () => {
        await result.current.executeSearch([123456789, 987654321], 2024);
      });

      await waitFor(() => {
        expect(mockTriggerDetails).toHaveBeenCalled();
      });
    });

    it("should handle API errors gracefully", async () => {
      const mockAddAlert = vi.fn();
      const mockTriggerSearch = vi.fn().mockReturnValue({
        unwrap: vi.fn().mockRejectedValue(new Error("Network error"))
      });

      const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});

      mockTriggerSearchMember.mockReturnValue([mockTriggerSearch, {}]);
      mockTriggerGetMemberDetails.mockReturnValue([vi.fn(), {}]);

      mockMissiveAlertsHook.mockReturnValue({
        missiveAlerts: [],
        addAlert: mockAddAlert,
        clearAlerts: vi.fn()
      });

      const { result } = renderHookWithProvider(() => useAdjustments());

      await act(async () => {
        await result.current.executeSearch([123456789, 987654321], 2024);
      });

      expect(mockAddAlert).toHaveBeenCalledWith(
        expect.objectContaining({
          severity: "error"
        })
      );

      expect(consoleErrorSpy).toHaveBeenCalled();

      consoleErrorSpy.mockRestore();
    });
  });

  describe("executeMerge", () => {
    it("should require both source and destination members", async () => {
      const mockAddAlert = vi.fn();

      mockMissiveAlertsHook.mockReturnValue({
        missiveAlerts: [],
        addAlert: mockAddAlert,
        clearAlerts: vi.fn()
      });

      const { result } = renderHookWithProvider(() => useAdjustments());

      let mergeResult;
      await act(async () => {
        mergeResult = await result.current.executeMerge("123456789", "987654321");
      });

      expect(mergeResult).toBe(false);
      expect(mockAddAlert).toHaveBeenCalledWith(
        expect.objectContaining({
          message: "Source member required"
        })
      );
    });

    it("should validate SSN format", async () => {
      const mockAddAlert = vi.fn();

      mockMissiveAlertsHook.mockReturnValue({
        missiveAlerts: [],
        addAlert: mockAddAlert,
        clearAlerts: vi.fn()
      });

      const { result } = renderHookWithProvider(() => useAdjustments());

      let mergeResult;
      await act(async () => {
        mergeResult = await result.current.executeMerge("invalid", "987654321");
      });

      expect(mergeResult).toBe(false);
      expect(mockAddAlert).toHaveBeenCalled();
    });

    it("should reject identical source and destination SSNs", async () => {
      const mockAddAlert = vi.fn();

      mockMissiveAlertsHook.mockReturnValue({
        missiveAlerts: [],
        addAlert: mockAddAlert,
        clearAlerts: vi.fn()
      });

      const { result } = renderHookWithProvider(() => useAdjustments());

      let mergeResult;
      await act(async () => {
        mergeResult = await result.current.executeMerge("123456789", "123456789");
      });

      expect(mergeResult).toBe(false);
      expect(mockAddAlert).toHaveBeenCalledWith(
        expect.objectContaining({
          message: "Invalid merge operation"
        })
      );
    });

    it("should handle merge API errors", async () => {
      const mockMerge = vi.fn().mockReturnValue({
        unwrap: vi.fn().mockRejectedValue(new Error("Merge failed"))
      });

      const mockAddAlert = vi.fn();
      const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});

      mockMergeProfitsDetail.mockImplementation(mockMerge);

      mockMissiveAlertsHook.mockReturnValue({
        missiveAlerts: [],
        addAlert: mockAddAlert,
        clearAlerts: vi.fn()
      });

      const { result } = renderHookWithProvider(() => useAdjustments(), {
        security: { token: "mock-token", user: null },
        inquiry: {
          masterInquiryMemberDetails: mockMember,
          masterInquiryMemberDetailsSecondary: mockMember
        }
      });

      let mergeResult;
      await act(async () => {
        mergeResult = await result.current.executeMerge("123456789", "987654321");
      });

      expect(mergeResult).toBe(false);
      expect(consoleErrorSpy).toHaveBeenCalled();

      consoleErrorSpy.mockRestore();
    });
  });

  describe("resetSearch", () => {
    it("should clear alerts when resetting", () => {
      const mockClearAlerts = vi.fn();

      mockMissiveAlertsHook.mockReturnValue({
        missiveAlerts: [],
        addAlert: vi.fn(),
        clearAlerts: mockClearAlerts
      });

      const { result } = renderHookWithProvider(() => useAdjustments());

      act(() => {
        result.current.resetSearch();
      });

      expect(mockClearAlerts).toHaveBeenCalled();
    });
  });

  describe("canMerge", () => {
    it("should be false when members are not selected", () => {
      mockMissiveAlertsHook.mockReturnValue({
        missiveAlerts: [],
        addAlert: vi.fn(),
        clearAlerts: vi.fn()
      });

      const { result } = renderHookWithProvider(() => useAdjustments());

      expect(result.current.canMerge).toBe(false);
    });

    it("should be true when both members are selected", () => {
      mockMissiveAlertsHook.mockReturnValue({
        missiveAlerts: [],
        addAlert: vi.fn(),
        clearAlerts: vi.fn()
      });

      const { result } = renderHookWithProvider(() => useAdjustments(), {
        security: { token: "mock-token", user: null },
        inquiry: {
          masterInquiryMemberDetails: mockMember,
          masterInquiryMemberDetailsSecondary: mockMember
        }
      });

      expect(result.current.canMerge).toBe(true);
    });
  });
});
