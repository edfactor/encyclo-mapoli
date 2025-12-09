import { renderHook, waitFor } from "@testing-library/react";
import { Provider } from "react-redux";
import { BrowserRouter } from "react-router-dom";
import { setupStore } from "reduxstore/store";
import type { MasterInquiryRequest } from "reduxstore/types";
import { beforeEach, describe, expect, it, vi } from "vitest";
import useMasterInquiry from "../useMasterInquiry";

/**
 * PS-2253 & PS-2254: Master Inquiry Filters Test Suite
 * Tests verify that all filters (voids, contribution, earnings, forfeiture, payment)
 * are properly passed from search params to member profit details API calls.
 */

const mockTriggerSearch = vi.fn();
const mockTriggerMemberDetails = vi.fn();
const mockTriggerProfitDetails = vi.fn();

vi.mock("reduxstore/api/InquiryApi", () => ({
  useLazySearchProfitMasterInquiryQuery: () => [
    mockTriggerSearch,
    { isLoading: false }
  ],
  useLazyGetProfitMasterInquiryMemberQuery: () => [mockTriggerMemberDetails],
  useLazyGetProfitMasterInquiryMemberDetailsQuery: () => [mockTriggerProfitDetails]
}));

const createWrapper = () => {
  const store = setupStore();
  return ({ children }: { children: React.ReactNode }) => (
    <Provider store={store}>
      <BrowserRouter>
        {children}
      </BrowserRouter>
    </Provider>
  );
};

describe("useMasterInquiry - Filter Propagation (PS-2253, PS-2254)", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("Voids Filter", () => {
    it("should include voids flag in profit details fetch when set to true", async () => {
      mockTriggerSearch.mockResolvedValue({
        results: [{ id: 1, memberType: 1, badgeNumber: 123 }],
        total: 1
      });
      mockTriggerMemberDetails.mockResolvedValue({ isEmployee: true });
      mockTriggerProfitDetails.mockResolvedValue({ results: [], total: 0 });

      const { result } = renderHook(() => useMasterInquiry(), {
        wrapper: createWrapper()
      });

      // Execute search with voids=true
      const searchParams: MasterInquiryRequest = {
        memberType: 1,
        voids: true,
        pagination: { skip: 0, take: 25 }
      };

      await result.current.executeSearch(searchParams);

      await waitFor(() => {
        expect(mockTriggerSearch).toHaveBeenCalledWith(
          expect.objectContaining({ voids: true })
        );
      });

      // Select member
      result.current.selectMember({
        id: 1,
        memberType: 1,
        badgeNumber: 123,
        name: "Test Member"
      });

      await waitFor(() => {
        expect(mockTriggerProfitDetails).toHaveBeenCalledWith(
          expect.objectContaining({
            voids: true,
            memberType: 1,
            id: 1
          })
        );
      });
    });

    it("should pass voids=false when not set in search params", async () => {
      mockTriggerSearch.mockResolvedValue({
        results: [{ id: 1, memberType: 1, badgeNumber: 123 }],
        total: 1
      });
      mockTriggerMemberDetails.mockResolvedValue({ isEmployee: true });
      mockTriggerProfitDetails.mockResolvedValue({ results: [], total: 0 });

      const { result } = renderHook(() => useMasterInquiry(), {
        wrapper: createWrapper()
      });

      const searchParams: MasterInquiryRequest = {
        memberType: 1,
        voids: false,
        pagination: { skip: 0, take: 25 }
      };

      await result.current.executeSearch(searchParams);

      result.current.selectMember({
        id: 1,
        memberType: 1,
        badgeNumber: 123,
        name: "Test Member"
      });

      await waitFor(() => {
        expect(mockTriggerProfitDetails).toHaveBeenCalledWith(
          expect.objectContaining({
            voids: false
          })
        );
      });
    });
  });

  describe("Contribution Amount Filter", () => {
    it("should pass contributionAmount to profit details API when set", async () => {
      mockTriggerSearch.mockResolvedValue({
        results: [{ id: 1, memberType: 1, badgeNumber: 123 }],
        total: 1
      });
      mockTriggerMemberDetails.mockResolvedValue({ isEmployee: true });
      mockTriggerProfitDetails.mockResolvedValue({ results: [], total: 0 });

      const { result } = renderHook(() => useMasterInquiry(), {
        wrapper: createWrapper()
      });

      const searchParams: MasterInquiryRequest = {
        memberType: 1,
        contributionAmount: 1000.50,
        pagination: { skip: 0, take: 25 }
      };

      await result.current.executeSearch(searchParams);

      result.current.selectMember({
        id: 1,
        memberType: 1,
        badgeNumber: 123,
        name: "Test Member"
      });

      await waitFor(() => {
        expect(mockTriggerProfitDetails).toHaveBeenCalledWith(
          expect.objectContaining({
            contributionAmount: 1000.50,
            memberType: 1,
            id: 1
          })
        );
      });
    });

    it("should omit contributionAmount when not set in search params", async () => {
      mockTriggerSearch.mockResolvedValue({
        results: [{ id: 1, memberType: 1, badgeNumber: 123 }],
        total: 1
      });
      mockTriggerMemberDetails.mockResolvedValue({ isEmployee: true });
      mockTriggerProfitDetails.mockResolvedValue({ results: [], total: 0 });

      const { result } = renderHook(() => useMasterInquiry(), {
        wrapper: createWrapper()
      });

      const searchParams: MasterInquiryRequest = {
        memberType: 1,
        pagination: { skip: 0, take: 25 }
      };

      await result.current.executeSearch(searchParams);

      result.current.selectMember({
        id: 1,
        memberType: 1,
        badgeNumber: 123,
        name: "Test Member"
      });

      await waitFor(() => {
        expect(mockTriggerProfitDetails).toHaveBeenCalledWith(
          expect.objectContaining({
            contributionAmount: undefined,
            memberType: 1,
            id: 1
          })
        );
      });
    });
  });

  describe("Earnings Amount Filter", () => {
    it("should pass earningsAmount to profit details API when set", async () => {
      mockTriggerSearch.mockResolvedValue({
        results: [{ id: 1, memberType: 1, badgeNumber: 123 }],
        total: 1
      });
      mockTriggerMemberDetails.mockResolvedValue({ isEmployee: true });
      mockTriggerProfitDetails.mockResolvedValue({ results: [], total: 0 });

      const { result } = renderHook(() => useMasterInquiry(), {
        wrapper: createWrapper()
      });

      const searchParams: MasterInquiryRequest = {
        memberType: 1,
        earningsAmount: 2500.75,
        pagination: { skip: 0, take: 25 }
      };

      await result.current.executeSearch(searchParams);

      result.current.selectMember({
        id: 1,
        memberType: 1,
        badgeNumber: 123,
        name: "Test Member"
      });

      await waitFor(() => {
        expect(mockTriggerProfitDetails).toHaveBeenCalledWith(
          expect.objectContaining({
            earningsAmount: 2500.75
          })
        );
      });
    });
  });

  describe("Forfeiture Amount Filter", () => {
    it("should pass forfeitureAmount to profit details API when set", async () => {
      mockTriggerSearch.mockResolvedValue({
        results: [{ id: 1, memberType: 1, badgeNumber: 123 }],
        total: 1
      });
      mockTriggerMemberDetails.mockResolvedValue({ isEmployee: true });
      mockTriggerProfitDetails.mockResolvedValue({ results: [], total: 0 });

      const { result } = renderHook(() => useMasterInquiry(), {
        wrapper: createWrapper()
      });

      const searchParams: MasterInquiryRequest = {
        memberType: 1,
        forfeitureAmount: 500.00,
        pagination: { skip: 0, take: 25 }
      };

      await result.current.executeSearch(searchParams);

      result.current.selectMember({
        id: 1,
        memberType: 1,
        badgeNumber: 123,
        name: "Test Member"
      });

      await waitFor(() => {
        expect(mockTriggerProfitDetails).toHaveBeenCalledWith(
          expect.objectContaining({
            forfeitureAmount: 500.00
          })
        );
      });
    });
  });

  describe("Payment Amount Filter", () => {
    it("should pass paymentAmount to profit details API when set", async () => {
      mockTriggerSearch.mockResolvedValue({
        results: [{ id: 1, memberType: 1, badgeNumber: 123 }],
        total: 1
      });
      mockTriggerMemberDetails.mockResolvedValue({ isEmployee: true });
      mockTriggerProfitDetails.mockResolvedValue({ results: [], total: 0 });

      const { result } = renderHook(() => useMasterInquiry(), {
        wrapper: createWrapper()
      });

      const searchParams: MasterInquiryRequest = {
        memberType: 1,
        paymentAmount: 1500.25,
        pagination: { skip: 0, take: 25 }
      };

      await result.current.executeSearch(searchParams);

      result.current.selectMember({
        id: 1,
        memberType: 1,
        badgeNumber: 123,
        name: "Test Member"
      });

      await waitFor(() => {
        expect(mockTriggerProfitDetails).toHaveBeenCalledWith(
          expect.objectContaining({
            paymentAmount: 1500.25
          })
        );
      });
    });
  });

  describe("Combined Filters", () => {
    it("should pass all filters together to profit details API", async () => {
      mockTriggerSearch.mockResolvedValue({
        results: [{ id: 1, memberType: 1, badgeNumber: 123 }],
        total: 1
      });
      mockTriggerMemberDetails.mockResolvedValue({ isEmployee: true });
      mockTriggerProfitDetails.mockResolvedValue({ results: [], total: 0 });

      const { result } = renderHook(() => useMasterInquiry(), {
        wrapper: createWrapper()
      });

      const searchParams: MasterInquiryRequest = {
        memberType: 1,
        voids: true,
        contributionAmount: 1000.00,
        earningsAmount: 2500.00,
        forfeitureAmount: 500.00,
        paymentAmount: 1500.00,
        pagination: { skip: 0, take: 25 }
      };

      await result.current.executeSearch(searchParams);

      result.current.selectMember({
        id: 1,
        memberType: 1,
        badgeNumber: 123,
        name: "Test Member"
      });

      await waitFor(() => {
        expect(mockTriggerProfitDetails).toHaveBeenCalledWith(
          expect.objectContaining({
            voids: true,
            contributionAmount: 1000.00,
            earningsAmount: 2500.00,
            forfeitureAmount: 500.00,
            paymentAmount: 1500.00,
            memberType: 1,
            id: 1
          })
        );
      });
    });
  });

  describe("Filter Persistence Across Pagination", () => {
    it("should maintain all filters when changing pagination", async () => {
      mockTriggerSearch.mockResolvedValue({
        results: [{ id: 1, memberType: 1, badgeNumber: 123 }],
        total: 100
      });
      mockTriggerMemberDetails.mockResolvedValue({ isEmployee: true });
      mockTriggerProfitDetails.mockResolvedValue({
        results: Array.from({ length: 25 }, (_, i) => ({ id: i })),
        total: 100
      });

      const { result } = renderHook(() => useMasterInquiry(), {
        wrapper: createWrapper()
      });

      const searchParams: MasterInquiryRequest = {
        memberType: 1,
        voids: true,
        contributionAmount: 1000.00,
        earningsAmount: 2500.00,
        forfeitureAmount: 500.00,
        paymentAmount: 1500.00,
        pagination: { skip: 0, take: 25 }
      };

      await result.current.executeSearch(searchParams);

      result.current.selectMember({
        id: 1,
        memberType: 1,
        badgeNumber: 123,
        name: "Test Member"
      });

      await waitFor(() => {
        expect(mockTriggerProfitDetails).toHaveBeenCalled();
      });

      // Simulate pagination change
      result.current.profitGridPagination.handlePageChange(1, 25);

      await waitFor(() => {
        // Should call with same filters but different pagination params
        expect(mockTriggerProfitDetails).toHaveBeenLastCalledWith(
          expect.objectContaining({
            voids: true,
            contributionAmount: 1000.00,
            earningsAmount: 2500.00,
            forfeitureAmount: 500.00,
            paymentAmount: 1500.00,
            skip: 25, // Changed page
            take: 25
          })
        );
      });
    });
  });
});
