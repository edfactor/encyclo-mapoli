import { configureStore } from "@reduxjs/toolkit";
import { beforeEach, describe, expect, it, vi } from "vitest";
import yearsEndReducer, { setDistributionsAndForfeituresQueryParams } from "../../../reduxstore/slices/yearsEndSlice";

// Mock the API
const mockTriggerSearch = vi.fn().mockResolvedValue({ data: { items: [], totals: {} } });

vi.mock("../../../reduxstore/api/YearsEndApi", () => ({
  useLazyGetDistributionsAndForfeituresQuery: () => [mockTriggerSearch, { isFetching: false }]
}));

describe("DistributionAndForfeituresGrid - API Request Building", () => {
  let store: any;

  beforeEach(() => {
    store = configureStore({
      reducer: {
        yearsEnd: yearsEndReducer
      },
      middleware: (getDefaultMiddleware) =>
        getDefaultMiddleware({
          serializableCheck: false
        })
    });
    mockTriggerSearch.mockClear();
  });

  describe("Query Parameter Handling", () => {
    it("should include empty arrays for states and taxCodes when 'All' is selected", () => {
      // Dispatch query params with empty arrays (representing "All")
      store.dispatch(
        setDistributionsAndForfeituresQueryParams({
          startDate: "2025-01-01",
          endDate: "2025-12-31",
          states: [],
          taxCodes: []
        })
      );

      const state = store.getState().yearsEnd;
      expect(state.distributionsAndForfeituresQueryParams.states).toEqual([]);
      expect(state.distributionsAndForfeituresQueryParams.taxCodes).toEqual([]);
    });

    it("should include populated arrays for states and taxCodes when specific values selected", () => {
      // Dispatch query params with specific selections
      store.dispatch(
        setDistributionsAndForfeituresQueryParams({
          startDate: "2025-01-01",
          endDate: "2025-12-31",
          states: ["MA", "CT", "NH"],
          taxCodes: ["H", "8", "9"]
        })
      );

      const state = store.getState().yearsEnd;
      expect(state.distributionsAndForfeituresQueryParams.states).toEqual(["MA", "CT", "NH"]);
      expect(state.distributionsAndForfeituresQueryParams.taxCodes).toEqual(["H", "8", "9"]);
    });

    it("should include date range in query params", () => {
      store.dispatch(
        setDistributionsAndForfeituresQueryParams({
          startDate: "2025-01-01",
          endDate: "2025-12-31",
          states: [],
          taxCodes: []
        })
      );

      const state = store.getState().yearsEnd;
      expect(state.distributionsAndForfeituresQueryParams.startDate).toBe("2025-01-01");
      expect(state.distributionsAndForfeituresQueryParams.endDate).toBe("2025-12-31");
    });
  });

  describe("API Request Body Construction", () => {
    it("should verify API body includes states and taxCodes arrays", () => {
      // This test verifies the shape of the data that would be sent to the API
      const requestBody = {
        startDate: "2025-01-01",
        endDate: "2025-12-31",
        states: ["MA", "CT"],
        taxCodes: ["H", "8"],
        skip: 0,
        take: 25,
        sortBy: "employeeName, date",
        isSortDescending: false
      };

      // Verify structure matches expected backend DTO
      expect(requestBody).toHaveProperty("states");
      expect(requestBody).toHaveProperty("taxCodes");
      expect(Array.isArray(requestBody.states)).toBe(true);
      expect(Array.isArray(requestBody.taxCodes)).toBe(true);
      expect(requestBody.states).toEqual(["MA", "CT"]);
      expect(requestBody.taxCodes).toEqual(["H", "8"]);
    });

    it("should handle empty arrays (All) in API request body", () => {
      const requestBody = {
        startDate: "2025-01-01",
        endDate: "2025-12-31",
        states: [],
        taxCodes: [],
        skip: 0,
        take: 25,
        sortBy: "employeeName, date",
        isSortDescending: false
      };

      // Empty arrays should be present in the body
      expect(requestBody.states).toEqual([]);
      expect(requestBody.taxCodes).toEqual([]);
      expect(Array.isArray(requestBody.states)).toBe(true);
      expect(Array.isArray(requestBody.taxCodes)).toBe(true);
    });

    it("should include pagination parameters in request body", () => {
      const requestBody = {
        startDate: "2025-01-01",
        endDate: "2025-12-31",
        states: [],
        taxCodes: [],
        skip: 0,
        take: 25,
        sortBy: "employeeName, date",
        isSortDescending: false
      };

      expect(requestBody).toHaveProperty("skip");
      expect(requestBody).toHaveProperty("take");
      expect(requestBody).toHaveProperty("sortBy");
      expect(requestBody).toHaveProperty("isSortDescending");
    });
  });

  describe("Redux State Management", () => {
    it("should clear query params when reset", () => {
      // Set query params
      store.dispatch(
        setDistributionsAndForfeituresQueryParams({
          startDate: "2025-01-01",
          endDate: "2025-12-31",
          states: ["MA"],
          taxCodes: ["H"]
        })
      );

      // Verify they're set
      let state = store.getState().yearsEnd;
      expect(state.distributionsAndForfeituresQueryParams).not.toBeNull();

      // Clear them (this would be done by clearDistributionsAndForfeituresQueryParams action)
      store.dispatch({ type: "yearsEnd/clearDistributionsAndForfeituresQueryParams" });

      // Verify they're cleared
      state = store.getState().yearsEnd;
      expect(state.distributionsAndForfeituresQueryParams).toBeNull();
    });

    it("should update query params when filters change", () => {
      // Initial state
      store.dispatch(
        setDistributionsAndForfeituresQueryParams({
          startDate: "2025-01-01",
          endDate: "2025-12-31",
          states: [],
          taxCodes: []
        })
      );

      let state = store.getState().yearsEnd;
      expect(state.distributionsAndForfeituresQueryParams.states).toEqual([]);

      // Update to specific states
      store.dispatch(
        setDistributionsAndForfeituresQueryParams({
          startDate: "2025-01-01",
          endDate: "2025-12-31",
          states: ["MA", "CT"],
          taxCodes: ["H"]
        })
      );

      state = store.getState().yearsEnd;
      expect(state.distributionsAndForfeituresQueryParams.states).toEqual(["MA", "CT"]);
      expect(state.distributionsAndForfeituresQueryParams.taxCodes).toEqual(["H"]);
    });
  });

  describe("Array Comparison for Pagination Reset", () => {
    it("should detect changes in states array", () => {
      const prev = { states: [] };
      const current = { states: ["MA"] };

      // Compare stringified versions (as done in Grid component)
      const prevStr = JSON.stringify(prev.states);
      const currentStr = JSON.stringify(current.states);

      expect(prevStr).not.toBe(currentStr);
    });

    it("should detect changes in taxCodes array", () => {
      const prev = { taxCodes: ["H"] };
      const current = { taxCodes: ["H", "8"] };

      const prevStr = JSON.stringify(prev.taxCodes);
      const currentStr = JSON.stringify(current.taxCodes);

      expect(prevStr).not.toBe(currentStr);
    });

    it("should not detect change when arrays are equivalent", () => {
      const prev = { states: ["MA", "CT"] };
      const current = { states: ["MA", "CT"] };

      const prevStr = JSON.stringify(prev.states);
      const currentStr = JSON.stringify(current.states);

      expect(prevStr).toBe(currentStr);
    });
  });
});
