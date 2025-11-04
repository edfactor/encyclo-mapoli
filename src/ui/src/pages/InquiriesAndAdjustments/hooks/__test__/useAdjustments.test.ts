import { configureStore } from "@reduxjs/toolkit";
import { act, renderHook, waitFor } from "@testing-library/react";
import React from "react";
import { Provider } from "react-redux";
import { beforeEach, describe, expect, it, vi } from "vitest";
import inquiryReducer, { type InquiryState } from "../../../../reduxstore/slices/inquirySlice";
import securityReducer, { type SecurityState } from "../../../../reduxstore/slices/securitySlice";
import { useAdjustments } from "../useAdjustments";
import * as useMissiveAlertsModule from "../../../../hooks/useMissiveAlerts";

// Create mock functions for triggers/actions
const mockMergeProfitsDetail = vi.fn();
const mockTriggerSearchFunction = vi.fn();
const mockTriggerDetailsFunction = vi.fn();

// Mock the APIs
vi.mock("../../../../reduxstore/api/AdjustmentsApi", () => ({
  useMergeProfitsDetailMutation: () => [mockMergeProfitsDetail, { isLoading: false }]
}));

vi.mock("../../../../reduxstore/api/InquiryApi", () => ({
  useLazySearchProfitMasterInquiryQuery: () => [mockTriggerSearchFunction, {}],
  useLazyGetProfitMasterInquiryMemberDetailsQuery: () => [mockTriggerDetailsFunction, {}]
}));

// Mock the hooks - use vi.fn() directly in the factory
vi.mock("../../../../hooks/useMissiveAlerts", () => ({
  useMissiveAlerts: vi.fn()
}));

const mockMember = {
  id: 1,
  badgeNumber: 123456,
  psnSuffix: 0,
  payFrequencyId: 1,
  isEmployee: true,
  firstName: "John",
  lastName: "Doe",
  address: "123 Main St",
  addressCity: "Boston",
  addressState: "MA",
  addressZipCode: "02101",
  dateOfBirth: "1990-01-01",
  ssn: "123-45-6789",
  yearToDateProfitSharingHours: 2080,
  yearsInPlan: 5,
  percentageVested: 100,
  contributionsLastYear: true,
  enrollmentId: 1,
  enrollment: "Active",
  hireDate: "2018-01-01",
  terminationDate: null,
  reHireDate: null,
  storeNumber: 1,
  beginPSAmount: 10000,
  currentPSAmount: 12000,
  beginVestedAmount: 10000,
  currentVestedAmount: 12000,
  currentEtva: 0,
  previousEtva: 0,
  employmentStatus: "Active",
  department: "IT",
  payClassification: "Salaried",
  gender: "M",
  phoneNumber: "617-555-1234",
  workLocation: "Boston HQ",
  receivedContributionsLastYear: true,
  fullTimeDate: "2018-01-01",
  terminationReason: "",
  missives: null,
  allocationFromAmount: 0,
  allocationToAmount: 0,
  badgesOfDuplicateSsns: []
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

type MockStoreState = Partial<RootState>;

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
  const store = createMockStore(preloadedState || { security: { token: "mock-token" }, inquiry: {} });
  return renderHook(() => hook(), {
    wrapper: ({ children }: { children: React.ReactNode }) => React.createElement(Provider, { store, children })
  });
}

describe("useAdjustments", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  const getMissiveAlertsMock = () => {
    return vi.mocked(useMissiveAlertsModule.useMissiveAlerts);
  };

  describe("initial state", () => {
    it("should initialize with default values", () => {
      getMissiveAlertsMock().mockReturnValue({
        missiveAlerts: [],
        addAlert: vi.fn(),
        clearAlerts: vi.fn()
      });

      const { result } = renderHookWithProvider(() => useAdjustments());

      expect(result.current.isMerging).toBe(false);
      expect(result.current.canMerge).toBeFalsy();
    });

    it("should expose executeSearch function", () => {
      getMissiveAlertsMock().mockReturnValue({
        missiveAlerts: [],
        addAlert: vi.fn(),
        clearAlerts: vi.fn()
      });

      const { result } = renderHookWithProvider(() => useAdjustments());

      expect(typeof result.current.executeSearch).toBe("function");
    });

    it("should expose executeMerge function", () => {
      getMissiveAlertsMock().mockReturnValue({
        missiveAlerts: [],
        addAlert: vi.fn(),
        clearAlerts: vi.fn()
      });

      const { result } = renderHookWithProvider(() => useAdjustments());

      expect(typeof result.current.executeMerge).toBe("function");
    });

    it("should expose resetSearch function", () => {
      getMissiveAlertsMock().mockReturnValue({
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

      mockTriggerSearchFunction.mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({ results: [] })
      });
      mockTriggerDetailsFunction.mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({ results: [] })
      });

      getMissiveAlertsMock().mockReturnValue({
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

      mockTriggerSearchFunction.mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({ results: [] })
      });
      mockTriggerDetailsFunction.mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({ results: [] })
      });

      getMissiveAlertsMock().mockReturnValue({
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

      mockTriggerSearchFunction.mockReturnValue({
        unwrap: mockUnwrapSearch
      });

      mockTriggerDetailsFunction.mockReturnValue({
        unwrap: mockUnwrapDetails
      });

      getMissiveAlertsMock().mockReturnValue({
        missiveAlerts: [],
        addAlert: vi.fn(),
        clearAlerts: vi.fn()
      });

      const { result } = renderHookWithProvider(() => useAdjustments());

      await act(async () => {
        await result.current.executeSearch([123456789, 987654321], 2024);
      });

      await waitFor(() => {
        expect(mockTriggerDetailsFunction).toHaveBeenCalled();
      });
    });

    it("should handle API errors gracefully", async () => {
      const mockAddAlert = vi.fn();

      mockTriggerSearchFunction.mockReturnValue({
        unwrap: vi.fn().mockRejectedValue(new Error("Network error"))
      });
      mockTriggerDetailsFunction.mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({ results: [] })
      });

      const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});

      getMissiveAlertsMock().mockReturnValue({
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

      getMissiveAlertsMock().mockReturnValue({
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

      getMissiveAlertsMock().mockReturnValue({
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

      getMissiveAlertsMock().mockReturnValue({
        missiveAlerts: [],
        addAlert: mockAddAlert,
        clearAlerts: vi.fn()
      });

      const { result } = renderHookWithProvider(() => useAdjustments(), {
        security: {
          token: "mock-token",
          userGroups: [],
          userRoles: [],
          userPermissions: [],
          username: "test-user",
          performLogout: false,
          appUser: null,
          impersonating: []
        },
        inquiry: {
          masterInquiryData: null,
          masterInquiryMemberDetails: mockMember,
          masterInquiryMemberDetailsSecondary: mockMember,
          masterInquiryResults: null,
          masterInquiryRequestParams: null,
          masterInquiryGroupingData: null
        }
      });

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

      getMissiveAlertsMock().mockReturnValue({
        missiveAlerts: [],
        addAlert: mockAddAlert,
        clearAlerts: vi.fn()
      });

      const { result } = renderHookWithProvider(() => useAdjustments(), {
        security: {
          token: "mock-token",
          userGroups: [],
          userRoles: [],
          userPermissions: [],
          username: "test-user",
          performLogout: false,
          appUser: null,
          impersonating: []
        },
        inquiry: {
          masterInquiryData: null,
          masterInquiryMemberDetails: mockMember,
          masterInquiryMemberDetailsSecondary: mockMember,
          masterInquiryResults: null,
          masterInquiryRequestParams: null,
          masterInquiryGroupingData: null
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

      getMissiveAlertsMock().mockReturnValue({
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
    it("should be falsy when members are not selected", () => {
      getMissiveAlertsMock().mockReturnValue({
        missiveAlerts: [],
        addAlert: vi.fn(),
        clearAlerts: vi.fn()
      });

      const { result } = renderHookWithProvider(() => useAdjustments());

      expect(result.current.canMerge).toBeFalsy();
    });

    it("should be truthy when both members are selected", () => {
      getMissiveAlertsMock().mockReturnValue({
        missiveAlerts: [],
        addAlert: vi.fn(),
        clearAlerts: vi.fn()
      });

      const { result } = renderHookWithProvider(() => useAdjustments(), {
        security: {
          token: "mock-token",
          userGroups: [],
          userRoles: [],
          userPermissions: [],
          username: "test-user",
          performLogout: false,
          appUser: null,
          impersonating: []
        },
        inquiry: {
          masterInquiryData: null,
          masterInquiryMemberDetails: mockMember,
          masterInquiryMemberDetailsSecondary: mockMember,
          masterInquiryResults: null,
          masterInquiryRequestParams: null,
          masterInquiryGroupingData: null
        }
      });

      expect(result.current.canMerge).toBeTruthy();
    });
  });
});
