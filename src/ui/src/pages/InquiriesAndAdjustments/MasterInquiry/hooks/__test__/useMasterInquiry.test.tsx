import { describe, expect, it } from "vitest";
import { initialState, masterInquiryReducer, MasterInquiryState } from "../useMasterInquiryReducer";

/**
 * Performance Fix Tests - PS-XXXX
 * These tests verify the performance optimizations to prevent duplicate API calls
 * and unnecessary component re-renders in Master Inquiry.
 */

describe("useMasterInquiry - Performance Optimizations", () => {
  describe("State Preservation on Pagination", () => {
    it("should preserve state when only pagination changes (Fix #2)", () => {
      const searchParams1 = {
        badgeNumber: 12345,
        pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: false }
      };

      const searchParams2 = {
        badgeNumber: 12345,
        pagination: { skip: 25, take: 25, sortBy: "badgeNumber", isSortDescending: false }
      };

      const stateWithResults: MasterInquiryState = {
        ...initialState,
        search: {
          ...initialState.search,
          params: searchParams1,
          results: { results: [{ id: 1 }] as any, total: 100 }
        },
        view: { mode: "multipleMembers" as const }
      };

      const action = {
        type: "SEARCH_START" as const,
        payload: { params: searchParams2, isManual: false }
      };

      const newState = masterInquiryReducer(stateWithResults, action);

      // Should NOT clear results when only pagination changes
      expect(newState.search.results).not.toBeNull();
      expect(newState.search.results?.results).toEqual([{ id: 1 }]);
      expect(newState.view.mode).toBe("multipleMembers");
    });

    it("should clear state when search parameters change (not just pagination)", () => {
      const searchParams1 = {
        badgeNumber: 12345,
        pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: false }
      };

      const searchParams2 = {
        badgeNumber: 54321, // Different badge number
        pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: false }
      };

      const stateWithResults: MasterInquiryState = {
        ...initialState,
        search: {
          ...initialState.search,
          params: searchParams1,
          results: { results: [{ id: 1 }] as any, total: 100 }
        },
        view: { mode: "multipleMembers" as const }
      };

      const action = {
        type: "SEARCH_START" as const,
        payload: { params: searchParams2, isManual: true }
      };

      const newState = masterInquiryReducer(stateWithResults, action);

      // SHOULD clear results when actual search parameters change
      expect(newState.search.results).toBeNull();
      expect(newState.view.mode).toBe("searching");
    });

    it("should preserve state when only sort order changes", () => {
      const searchParams1 = {
        badgeNumber: 12345,
        pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: false }
      };

      const searchParams2 = {
        badgeNumber: 12345,
        pagination: { skip: 0, take: 25, sortBy: "name", isSortDescending: true }
      };

      const stateWithResults: MasterInquiryState = {
        ...initialState,
        search: {
          ...initialState.search,
          params: searchParams1,
          results: { results: [{ id: 1 }] as any, total: 100 }
        }
      };

      const action = {
        type: "SEARCH_START" as const,
        payload: { params: searchParams2, isManual: false }
      };

      const newState = masterInquiryReducer(stateWithResults, action);

      // Should preserve results when only sorting changes
      expect(newState.search.results).not.toBeNull();
    });
  });

  describe("Manual vs Automatic Search Tracking", () => {
    it("should track isManual flag correctly", () => {
      const params = {
        badgeNumber: 12345,
        pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: false }
      };

      const manualAction = {
        type: "SEARCH_START" as const,
        payload: { params, isManual: true }
      };

      const manualState = masterInquiryReducer(initialState, manualAction);
      expect(manualState.search.isManuallySearching).toBe(true);

      const autoAction = {
        type: "SEARCH_START" as const,
        payload: { params, isManual: false }
      };

      const autoState = masterInquiryReducer(initialState, autoAction);
      expect(autoState.search.isManuallySearching).toBe(false);
    });
  });

  describe("View Mode Transitions", () => {
    it("should transition to searching mode on SEARCH_START", () => {
      const params = {
        badgeNumber: 12345,
        pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: false }
      };

      const action = {
        type: "SEARCH_START" as const,
        payload: { params, isManual: true }
      };

      const newState = masterInquiryReducer(initialState, action);
      expect(newState.view.mode).toBe("searching");
    });

    it("should not transition mode during pagination-only changes", () => {
      const stateInGridMode: MasterInquiryState = {
        ...initialState,
        view: { mode: "multipleMembers" as const }
      };

      const params = {
        badgeNumber: 12345,
        pagination: { skip: 25, take: 25, sortBy: "badgeNumber", isSortDescending: false }
      };

      const action = {
        type: "SEARCH_START" as const,
        payload: { params, isManual: false }
      };

      const newState = masterInquiryReducer(stateInGridMode, action);
      // Should stay in multipleMembers mode for pagination
      expect(newState.view.mode).toBe("multipleMembers");
    });
  });
});
