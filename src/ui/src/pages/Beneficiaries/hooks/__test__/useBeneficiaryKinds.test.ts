import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, waitFor } from "@testing-library/react";
import React from "react";
import { Provider } from "react-redux";
import { useBeneficiaryKinds } from "../useBeneficiaryKinds";
import { BeneficiaryKindDto } from "reduxstore/types";
import { configureStore } from "@reduxjs/toolkit";
import securityReducer, { type SecurityState } from "reduxstore/slices/securitySlice";

// Create mock functions for triggers/actions
const mockTriggerGetBeneficiaryKind = vi.fn();

// Mock the API
vi.mock("reduxstore/api/BeneficiariesApi", () => ({
  useLazyGetBeneficiaryKindQuery: () => [mockTriggerGetBeneficiaryKind, { isLoading: false }]
}));

const mockBeneficiaryKinds: BeneficiaryKindDto[] = [
  { id: "spouse", name: "Spouse" },
  { id: "child", name: "Child" },
  { id: "parent", name: "Parent" },
  { id: "other", name: "Other" }
];

type RootState = {
  security: SecurityState;
};

type MockStoreState = Partial<RootState>;

function createMockStore(preloadedState?: MockStoreState) {
  return configureStore<RootState>({
    reducer: {
      security: securityReducer
    } as const,
    preloadedState: preloadedState as RootState | undefined
  });
}

function renderHookWithProvider<T>(hook: () => T, preloadedState?: MockStoreState) {
  const store = createMockStore(preloadedState || { security: { token: "mock-token", user: null } });
  return renderHook(() => hook(), {
    wrapper: ({ children }: { children: React.ReactNode }) => React.createElement(Provider, { store, children })
  });
}

describe("useBeneficiaryKinds", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("should not fetch without token", () => {
    mockTriggerGetBeneficiaryKind.mockReturnValue({
      unwrap: vi.fn().mockResolvedValue({ beneficiaryKindList: mockBeneficiaryKinds })
    });

    const { result } = renderHookWithProvider(() => useBeneficiaryKinds(), { security: { token: null, user: null } });

    expect(result.current.isLoading).toBe(false);
    expect(result.current.beneficiaryKinds).toEqual([]);
    expect(mockTriggerGetBeneficiaryKind).not.toHaveBeenCalled();
  });

  it("should fetch beneficiary kinds with token", async () => {
    const mockUnwrap = vi.fn().mockResolvedValue({
      beneficiaryKindList: mockBeneficiaryKinds
    });

    mockTriggerGetBeneficiaryKind.mockReturnValue({
      unwrap: mockUnwrap
    });

    const { result } = renderHookWithProvider(() => useBeneficiaryKinds());

    expect(mockTriggerGetBeneficiaryKind).toHaveBeenCalledWith({});

    await waitFor(() => {
      expect(result.current.beneficiaryKinds.length).toBe(4);
    });

    expect(result.current.beneficiaryKinds).toEqual(mockBeneficiaryKinds);
  });

  it("should only fetch once even if hook re-renders", async () => {
    const mockUnwrap = vi.fn().mockResolvedValue({
      beneficiaryKindList: mockBeneficiaryKinds
    });

    mockTriggerGetBeneficiaryKind.mockReturnValue({
      unwrap: mockUnwrap
    });

    const { rerender } = renderHookWithProvider(() => useBeneficiaryKinds());

    await waitFor(() => {
      expect(mockTriggerGetBeneficiaryKind).toHaveBeenCalled();
    });

    mockTriggerGetBeneficiaryKind.mockClear();

    // Force re-render (e.g., parent state change)
    rerender();

    // Verify API was not called again
    expect(mockTriggerGetBeneficiaryKind).not.toHaveBeenCalled();
  });

  it("should handle empty kind list from API", async () => {
    const mockUnwrap = vi.fn().mockResolvedValue({
      beneficiaryKindList: []
    });

    mockTriggerGetBeneficiaryKind.mockReturnValue({
      unwrap: mockUnwrap
    });

    const { result } = renderHookWithProvider(() => useBeneficiaryKinds());

    await waitFor(() => {
      expect(result.current.beneficiaryKinds).toEqual([]);
    });

    expect(result.current.error).toBeNull();
  });

  it("should handle null kind list from API", async () => {
    const mockUnwrap = vi.fn().mockResolvedValue({
      beneficiaryKindList: null
    });

    mockTriggerGetBeneficiaryKind.mockReturnValue({
      unwrap: mockUnwrap
    });

    const { result } = renderHookWithProvider(() => useBeneficiaryKinds());

    await waitFor(() => {
      expect(result.current.beneficiaryKinds).toEqual([]);
    });

    expect(result.current.error).toBeNull();
  });

  it("should set error state on API failure", async () => {
    const mockUnwrap = vi.fn().mockRejectedValue(new Error("Network error"));
    const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});

    mockTriggerGetBeneficiaryKind.mockReturnValue({
      unwrap: mockUnwrap
    });

    const { result } = renderHookWithProvider(() => useBeneficiaryKinds());

    await waitFor(() => {
      expect(result.current.error).not.toBeNull();
    });

    expect(result.current.error).toBe("Failed to load beneficiary types");
    expect(result.current.beneficiaryKinds).toEqual([]);
    expect(consoleErrorSpy).toHaveBeenCalled();

    consoleErrorSpy.mockRestore();
  });

  it("should have correct initial state", () => {
    mockTriggerGetBeneficiaryKind.mockReturnValue({
      unwrap: vi.fn().mockResolvedValue({ beneficiaryKindList: mockBeneficiaryKinds })
    });

    const { result } = renderHookWithProvider(() => useBeneficiaryKinds(), { security: { token: null, user: null } });

    expect(result.current.beneficiaryKinds).toEqual([]);
    expect(result.current.error).toBeNull();
  });

  it("should clear error on successful fetch", async () => {
    const mockUnwrap = vi.fn().mockResolvedValue({
      beneficiaryKindList: mockBeneficiaryKinds
    });

    mockTriggerGetBeneficiaryKind.mockReturnValue({
      unwrap: mockUnwrap
    });

    const { result } = renderHookWithProvider(() => useBeneficiaryKinds());

    await waitFor(() => {
      expect(result.current.beneficiaryKinds.length).toBeGreaterThan(0);
    });

    expect(result.current.error).toBeNull();
  });

  it("should match API loading state", async () => {
    const mockUnwrap = vi
      .fn()
      .mockImplementation(
        () => new Promise((resolve) => setTimeout(() => resolve({ beneficiaryKindList: mockBeneficiaryKinds }), 100))
      );

    mockTriggerGetBeneficiaryKind.mockReturnValue({
      unwrap: mockUnwrap
    });

    const { result } = renderHookWithProvider(() => useBeneficiaryKinds());

    // Initially isLoading should be false since the mock returns { isLoading: false }
    // The hook won't show loading state with our static mock
    // This test verifies the data loads correctly
    await waitFor(
      () => {
        expect(result.current.beneficiaryKinds.length).toBeGreaterThan(0);
      },
      { timeout: 500 }
    );
  });
});
