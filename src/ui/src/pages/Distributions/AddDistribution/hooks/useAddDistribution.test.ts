import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

// Mock RTK Query hooks
const mockTriggerSearchDistributions = vi.fn();
const mockTriggerSearchMember = vi.fn();
const mockTriggerGetMember = vi.fn();
const mockTriggerGetStateTax = vi.fn();
const mockCreateDistribution = vi.fn();

vi.mock("../../../../reduxstore/api/DistributionApi", () => ({
  useCreateDistributionMutation: () => [mockCreateDistribution, {}],
  useLazySearchDistributionsQuery: () => [mockTriggerSearchDistributions, {}]
}));

vi.mock("../../../../reduxstore/api/InquiryApi", () => ({
  useLazySearchProfitMasterInquiryQuery: () => [mockTriggerSearchMember, {}],
  useLazyGetProfitMasterInquiryMemberQuery: () => [mockTriggerGetMember, {}]
}));

vi.mock("../../../../reduxstore/api/LookupsApi", () => ({
  useLazyGetStateTaxQuery: () => [mockTriggerGetStateTax, {}]
}));

import { useAddDistribution } from "./useAddDistribution";

describe("useAddDistribution - maximumReached logic", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("calculateSequenceNumber - maximumReached flag", () => {
    it("should set maximumReached to false when member has 0 distributions", async () => {
      // Mock API response with no distributions
      mockTriggerSearchDistributions.mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({
          results: [],
          total: 0
        })
      });

      const { result } = renderHook(() => useAddDistribution());

      await act(async () => {
        await result.current.calculateSequenceNumber(12345, 1);
      });

      expect(result.current.sequenceNumber).toBe(1);
      expect(result.current.maximumReached).toBe(false);
    });

    it("should set maximumReached to false when member has 8 distributions", async () => {
      // Mock API response with 8 distributions (sequences 1-8)
      const distributions = Array.from({ length: 8 }, (_, i) => ({
        paymentSequence: i + 1,
        id: i + 1,
        frequencyId: "M" // Not 'D', so should be counted
      }));

      mockTriggerSearchDistributions.mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({
          results: distributions,
          total: 8
        })
      });

      const { result } = renderHook(() => useAddDistribution());

      await act(async () => {
        await result.current.calculateSequenceNumber(12345, 1);
      });

      expect(result.current.sequenceNumber).toBe(9);
      expect(result.current.maximumReached).toBe(false);
    });

    it("should set maximumReached to true when member has exactly 9 distributions", async () => {
      // Mock API response with 9 distributions (sequences 1-9)
      const distributions = Array.from({ length: 9 }, (_, i) => ({
        paymentSequence: i + 1,
        id: i + 1,
        frequencyId: "M" // Not 'D', so should be counted
      }));

      mockTriggerSearchDistributions.mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({
          results: distributions,
          total: 9
        })
      });

      const { result } = renderHook(() => useAddDistribution());

      await act(async () => {
        await result.current.calculateSequenceNumber(12345, 1);
      });

      expect(result.current.sequenceNumber).toBe(10);
      expect(result.current.maximumReached).toBe(true);
    });

    it("should set maximumReached to true when member has more than 9 distributions", async () => {
      // Mock API response with 10 distributions
      const distributions = Array.from({ length: 10 }, (_, i) => ({
        paymentSequence: i + 1,
        id: i + 1,
        frequencyId: "M" // Not 'D', so should be counted
      }));

      mockTriggerSearchDistributions.mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({
          results: distributions,
          total: 10
        })
      });

      const { result } = renderHook(() => useAddDistribution());

      await act(async () => {
        await result.current.calculateSequenceNumber(12345, 1);
      });

      expect(result.current.sequenceNumber).toBe(11);
      expect(result.current.maximumReached).toBe(true);
    });

    it("should set maximumReached to false when member has 5 distributions with gaps (deleted distributions)", async () => {
      // Mock API response with 5 distributions but with gaps in sequence numbers
      // This tests the bug fix: sequences [1, 2, 3, 5, 9] should NOT trigger maximum
      const distributions = [
        { paymentSequence: 1, id: 1, frequencyId: "M" },
        { paymentSequence: 2, id: 2, frequencyId: "M" },
        { paymentSequence: 3, id: 3, frequencyId: "M" },
        { paymentSequence: 5, id: 5, frequencyId: "M" },
        { paymentSequence: 9, id: 9, frequencyId: "M" }
      ];

      mockTriggerSearchDistributions.mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({
          results: distributions,
          total: 5
        })
      });

      const { result } = renderHook(() => useAddDistribution());

      await act(async () => {
        await result.current.calculateSequenceNumber(12345, 1);
      });

      // Next sequence should be 10 (max sequence 9 + 1)
      expect(result.current.sequenceNumber).toBe(10);
      // But maximumReached should be false because only 5 distributions exist
      expect(result.current.maximumReached).toBe(false);
    });

    it("should set maximumReached to true when member has 9 distributions with gaps", async () => {
      // Mock API response with 9 distributions but with gaps in sequence numbers
      // Sequences: [1, 2, 3, 4, 5, 7, 8, 10, 12]
      const distributions = [
        { paymentSequence: 1, id: 1, frequencyId: "M" },
        { paymentSequence: 2, id: 2, frequencyId: "M" },
        { paymentSequence: 3, id: 3, frequencyId: "M" },
        { paymentSequence: 4, id: 4, frequencyId: "M" },
        { paymentSequence: 5, id: 5, frequencyId: "M" },
        { paymentSequence: 7, id: 7, frequencyId: "M" },
        { paymentSequence: 8, id: 8, frequencyId: "M" },
        { paymentSequence: 10, id: 10, frequencyId: "M" },
        { paymentSequence: 12, id: 12, frequencyId: "M" }
      ];

      mockTriggerSearchDistributions.mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({
          results: distributions,
          total: 9
        })
      });

      const { result } = renderHook(() => useAddDistribution());

      await act(async () => {
        await result.current.calculateSequenceNumber(12345, 1);
      });

      // Next sequence should be 13 (max sequence 12 + 1)
      expect(result.current.sequenceNumber).toBe(13);
      // maximumReached should be true because 9 distributions exist
      expect(result.current.maximumReached).toBe(true);
    });

    it("should handle API error gracefully and set maximumReached to false", async () => {
      // Mock API error
      mockTriggerSearchDistributions.mockReturnValue({
        unwrap: vi.fn().mockRejectedValue(new Error("API Error"))
      });

      const { result } = renderHook(() => useAddDistribution());

      await act(async () => {
        await result.current.calculateSequenceNumber(12345, 1);
      });

      // On error, defaults to sequence 1 and maximumReached false
      // Note: The error handling dispatches FAILURE then SUCCESS, so error is cleared
      expect(result.current.sequenceNumber).toBe(1);
      expect(result.current.maximumReached).toBe(false);
      // Error is cleared by the subsequent SUCCESS dispatch (this is by design for graceful degradation)
      expect(result.current.sequenceNumberError).toBeNull();
    });

    it("should correctly calculate when distributions are out of order", async () => {
      // Mock API response with out-of-order sequences
      const distributions = [
        { paymentSequence: 5, id: 5, frequencyId: "M" },
        { paymentSequence: 1, id: 1, frequencyId: "M" },
        { paymentSequence: 9, id: 9, frequencyId: "M" },
        { paymentSequence: 3, id: 3, frequencyId: "M" },
        { paymentSequence: 7, id: 7, frequencyId: "M" }
      ];

      mockTriggerSearchDistributions.mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({
          results: distributions,
          total: 5
        })
      });

      const { result } = renderHook(() => useAddDistribution());

      await act(async () => {
        await result.current.calculateSequenceNumber(12345, 1);
      });

      // Should find max sequence (9) regardless of order
      expect(result.current.sequenceNumber).toBe(10);
      // Only 5 distributions, so not at maximum
      expect(result.current.maximumReached).toBe(false);
    });

    it("should exclude distributions with frequencyId 'D' from count", async () => {
      // Mock API response with 10 total distributions but 3 have frequencyId 'D'
      // So only 7 should count toward the maximum
      const distributions = [
        { paymentSequence: 1, id: 1, frequencyId: "M" },
        { paymentSequence: 2, id: 2, frequencyId: "D" }, // Should NOT count
        { paymentSequence: 3, id: 3, frequencyId: "M" },
        { paymentSequence: 4, id: 4, frequencyId: "M" },
        { paymentSequence: 5, id: 5, frequencyId: "D" }, // Should NOT count
        { paymentSequence: 6, id: 6, frequencyId: "M" },
        { paymentSequence: 7, id: 7, frequencyId: "M" },
        { paymentSequence: 8, id: 8, frequencyId: "D" }, // Should NOT count
        { paymentSequence: 9, id: 9, frequencyId: "M" },
        { paymentSequence: 10, id: 10, frequencyId: "M" }
      ];

      mockTriggerSearchDistributions.mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({
          results: distributions,
          total: 10
        })
      });

      const { result } = renderHook(() => useAddDistribution());

      await act(async () => {
        await result.current.calculateSequenceNumber(12345, 1);
      });

      // Next sequence should be 11
      expect(result.current.sequenceNumber).toBe(11);
      // Only 7 non-'D' distributions, so maximum NOT reached
      expect(result.current.maximumReached).toBe(false);
    });

    it("should reach maximum when 9 non-D distributions exist even with D distributions present", async () => {
      // Mock API response with 12 total distributions but 3 have frequencyId 'D'
      // So 9 should count toward the maximum
      const distributions = [
        { paymentSequence: 1, id: 1, frequencyId: "M" },
        { paymentSequence: 2, id: 2, frequencyId: "M" },
        { paymentSequence: 3, id: 3, frequencyId: "D" }, // Should NOT count
        { paymentSequence: 4, id: 4, frequencyId: "M" },
        { paymentSequence: 5, id: 5, frequencyId: "M" },
        { paymentSequence: 6, id: 6, frequencyId: "M" },
        { paymentSequence: 7, id: 7, frequencyId: "D" }, // Should NOT count
        { paymentSequence: 8, id: 8, frequencyId: "M" },
        { paymentSequence: 9, id: 9, frequencyId: "M" },
        { paymentSequence: 10, id: 10, frequencyId: "M" },
        { paymentSequence: 11, id: 11, frequencyId: "D" }, // Should NOT count
        { paymentSequence: 12, id: 12, frequencyId: "M" }
      ];

      mockTriggerSearchDistributions.mockReturnValue({
        unwrap: vi.fn().mockResolvedValue({
          results: distributions,
          total: 12
        })
      });

      const { result } = renderHook(() => useAddDistribution());

      await act(async () => {
        await result.current.calculateSequenceNumber(12345, 1);
      });

      // Next sequence should be 13
      expect(result.current.sequenceNumber).toBe(13);
      // Exactly 9 non-'D' distributions, so maximum reached
      expect(result.current.maximumReached).toBe(true);
    });
  });
});
