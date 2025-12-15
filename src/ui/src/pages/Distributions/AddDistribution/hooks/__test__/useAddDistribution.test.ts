import { act, renderHook } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { createMockStoreAndWrapper } from "../../../../../test";

// Mock RTK Query hooks
vi.mock("../../../../reduxstore/api/DistributionApi", () => ({
  useCreateDistributionMutation: () => [vi.fn(), {}],
  useLazySearchDistributionsQuery: () => [vi.fn(), {}]
}));

vi.mock("../../../../reduxstore/api/InquiryApi", () => ({
  useLazySearchProfitMasterInquiryQuery: () => [vi.fn(), {}],
  useLazyGetProfitMasterInquiryMemberQuery: () => [vi.fn(), {}]
}));

vi.mock("../../../../reduxstore/api/LookupsApi", () => ({
  useLazyGetStateTaxQuery: () => [vi.fn(), {}]
}));

import { useAddDistribution } from "../useAddDistribution";

describe("useAddDistribution", () => {
  it("should initialize with default state", () => {
    const { wrapper } = createMockStoreAndWrapper();
    const { result } = renderHook(() => useAddDistribution(), { wrapper });

    expect(result.current.memberData).toBeNull();
    expect(result.current.isMemberLoading).toBe(false);
    expect(result.current.memberError).toBeNull();
    expect(result.current.stateTaxRate).toBeNull();
    expect(result.current.isStateTaxLoading).toBe(false);
    expect(result.current.isStateTaxLoading).toBe(false);
    expect(result.current.stateTaxError).toBeNull();
    expect(result.current.sequenceNumber).toBeNull();
    expect(result.current.isSequenceNumberLoading).toBe(false);
    expect(result.current.sequenceNumberError).toBeNull();
    expect(result.current.maximumReached).toBe(false);
    expect(result.current.isSubmitting).toBe(false);
    expect(result.current.submissionError).toBeNull();
    expect(result.current.submissionSuccess).toBe(false);
  });

  it("should have all required methods", () => {
    const { wrapper } = createMockStoreAndWrapper();
    const { result } = renderHook(() => useAddDistribution(), { wrapper });

    expect(typeof result.current.calculateSequenceNumber).toBe("function");
    expect(typeof result.current.fetchMemberData).toBe("function");
    expect(typeof result.current.fetchStateTaxRate).toBe("function");
    expect(typeof result.current.submitDistribution).toBe("function");
    expect(typeof result.current.clearSubmissionError).toBe("function");
    expect(typeof result.current.reset).toBe("function");
  });

  it("should reset state correctly", () => {
    const { wrapper } = createMockStoreAndWrapper();
    const { result } = renderHook(() => useAddDistribution(), { wrapper });

    // Reset
    act(() => {
      result.current.reset();
    });

    // Verify reset to initial state
    expect(result.current.memberData).toBeNull();
    expect(result.current.stateTaxRate).toBeNull();
    expect(result.current.sequenceNumber).toBeNull();
    expect(result.current.maximumReached).toBe(false);
  });
});
