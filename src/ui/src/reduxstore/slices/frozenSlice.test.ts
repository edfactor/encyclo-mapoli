import { describe, it, expect } from "vitest";
import reducer, { setFrozenStateResponse, setFrozenStateCollectionResponse, FrozenState } from "./frozenSlice";
import { FrozenStateResponse } from "reduxstore/types";
import { Paged } from "smart-ui-library";

describe("frozenSlice", () => {
  const initialState: FrozenState = {
    frozenStateResponseData: null,
    frozenStateCollectionData: null,
    error: null
  };

  const mockFrozenStateResponse: FrozenStateResponse = {
    id: 1,
    profitYear: 2024,
    frozenBy: "admin@company.com",
    asOfDateTime: "2024-12-25T10:00:00Z",
    createdDateTime: "2024-12-20T08:00:00Z",
    isActive: true
  };

  const mockFrozenStateCollection: Paged<FrozenStateResponse> = {
    total: 3,
    totalPages: 1,
    pageSize: 50,
    currentPage: 1,
    results: [
      mockFrozenStateResponse,
      {
        id: 2,
        profitYear: 2023,
        frozenBy: "admin@company.com",
        asOfDateTime: "2023-12-25T10:00:00Z",
        createdDateTime: "2023-12-20T08:00:00Z",
        isActive: false
      },
      {
        id: 3,
        profitYear: 2022,
        frozenBy: "manager@company.com",
        asOfDateTime: "2022-12-25T10:00:00Z",
        createdDateTime: "2022-12-20T08:00:00Z",
        isActive: false
      }
    ]
  };

  describe("reducer", () => {
    it("should return initial state when called with undefined state", () => {
      expect(reducer(undefined, { type: "unknown" })).toEqual(initialState);
    });

    it("should return current state for unknown action", () => {
      const currentState: FrozenState = {
        ...initialState,
        frozenStateResponseData: mockFrozenStateResponse
      };
      expect(reducer(currentState, { type: "unknown" })).toEqual(currentState);
    });
  });

  describe("setFrozenStateResponse", () => {
    it("should set frozen state response data", () => {
      const nextState = reducer(initialState, setFrozenStateResponse(mockFrozenStateResponse));

      expect(nextState.frozenStateResponseData).toEqual(mockFrozenStateResponse);
      expect(nextState.error).toBeNull();
    });

    it("should set error when payload is null", () => {
      const nextState = reducer(initialState, setFrozenStateResponse(null));

      expect(nextState.frozenStateResponseData).toBeNull();
      expect(nextState.error).toBe("Failed to fetch frozen state");
    });

    it("should clear previous error when valid payload is set", () => {
      const stateWithError: FrozenState = {
        ...initialState,
        error: "Previous error"
      };

      const nextState = reducer(stateWithError, setFrozenStateResponse(mockFrozenStateResponse));

      expect(nextState.error).toBeNull();
      expect(nextState.frozenStateResponseData).toEqual(mockFrozenStateResponse);
    });

    it("should replace existing frozen state data", () => {
      const oldResponse: FrozenStateResponse = {
        id: 99,
        profitYear: 2020,
        frozenBy: "old@user.com",
        asOfDateTime: "2020-01-01T00:00:00Z",
        createdDateTime: "2020-01-01T00:00:00Z",
        isActive: false
      };

      const currentState: FrozenState = {
        ...initialState,
        frozenStateResponseData: oldResponse
      };

      const nextState = reducer(currentState, setFrozenStateResponse(mockFrozenStateResponse));

      expect(nextState.frozenStateResponseData?.id).toBe(1);
      expect(nextState.frozenStateResponseData?.profitYear).toBe(2024);
    });

    it("should handle inactive frozen state", () => {
      const inactiveResponse: FrozenStateResponse = {
        ...mockFrozenStateResponse,
        isActive: false
      };

      const nextState = reducer(initialState, setFrozenStateResponse(inactiveResponse));

      expect(nextState.frozenStateResponseData?.isActive).toBe(false);
    });
  });

  describe("setFrozenStateCollectionResponse", () => {
    it("should set frozen state collection data", () => {
      const nextState = reducer(initialState, setFrozenStateCollectionResponse(mockFrozenStateCollection));

      expect(nextState.frozenStateCollectionData).toEqual(mockFrozenStateCollection);
      expect(nextState.frozenStateCollectionData?.results).toHaveLength(3);
      expect(nextState.error).toBeNull();
    });

    it("should set error when payload is null", () => {
      const nextState = reducer(initialState, setFrozenStateCollectionResponse(null));

      expect(nextState.frozenStateCollectionData).toBeNull();
      expect(nextState.error).toBe("Failed to fetch frozen state collection");
    });

    it("should clear previous error when valid payload is set", () => {
      const stateWithError: FrozenState = {
        ...initialState,
        error: "Previous collection error"
      };

      const nextState = reducer(stateWithError, setFrozenStateCollectionResponse(mockFrozenStateCollection));

      expect(nextState.error).toBeNull();
      expect(nextState.frozenStateCollectionData).toEqual(mockFrozenStateCollection);
    });

    it("should handle empty collection", () => {
      const emptyCollection: Paged<FrozenStateResponse> = {
        total: 0,
        totalPages: 0,
        pageSize: 50,
        currentPage: 1,
        results: []
      };

      const nextState = reducer(initialState, setFrozenStateCollectionResponse(emptyCollection));

      expect(nextState.frozenStateCollectionData?.results).toHaveLength(0);
      expect(nextState.frozenStateCollectionData?.total).toBe(0);
    });

    it("should handle paginated collection", () => {
      const paginatedCollection: Paged<FrozenStateResponse> = {
        total: 100,
        totalPages: 4,
        pageSize: 25,
        currentPage: 2,
        results: Array.from({ length: 25 }, (_, i) => ({
          id: i + 26,
          profitYear: 2024 - (i % 5),
          frozenBy: `user${i}@company.com`,
          asOfDateTime: "2024-01-01T00:00:00Z",
          createdDateTime: "2024-01-01T00:00:00Z",
          isActive: i % 2 === 0
        }))
      };

      const nextState = reducer(initialState, setFrozenStateCollectionResponse(paginatedCollection));

      expect(nextState.frozenStateCollectionData?.total).toBe(100);
      expect(nextState.frozenStateCollectionData?.totalPages).toBe(4);
      expect(nextState.frozenStateCollectionData?.currentPage).toBe(2);
      expect(nextState.frozenStateCollectionData?.results).toHaveLength(25);
    });
  });

  describe("state isolation", () => {
    it("should not affect collection data when setting response data", () => {
      const currentState: FrozenState = {
        ...initialState,
        frozenStateCollectionData: mockFrozenStateCollection
      };

      const nextState = reducer(currentState, setFrozenStateResponse(mockFrozenStateResponse));

      expect(nextState.frozenStateResponseData).toEqual(mockFrozenStateResponse);
      expect(nextState.frozenStateCollectionData).toEqual(mockFrozenStateCollection);
    });

    it("should not affect response data when setting collection data", () => {
      const currentState: FrozenState = {
        ...initialState,
        frozenStateResponseData: mockFrozenStateResponse
      };

      const nextState = reducer(currentState, setFrozenStateCollectionResponse(mockFrozenStateCollection));

      expect(nextState.frozenStateCollectionData).toEqual(mockFrozenStateCollection);
      expect(nextState.frozenStateResponseData).toEqual(mockFrozenStateResponse);
    });
  });

  describe("edge cases", () => {
    it("should preserve state immutability", () => {
      const currentState: FrozenState = { ...initialState };
      const nextState = reducer(currentState, setFrozenStateResponse(mockFrozenStateResponse));

      expect(currentState.frozenStateResponseData).toBeNull();
      expect(nextState.frozenStateResponseData).toEqual(mockFrozenStateResponse);
    });

    it("should handle sequential updates", () => {
      let state = reducer(initialState, setFrozenStateResponse(mockFrozenStateResponse));
      expect(state.frozenStateResponseData?.profitYear).toBe(2024);

      const updatedResponse: FrozenStateResponse = {
        ...mockFrozenStateResponse,
        profitYear: 2025
      };
      state = reducer(state, setFrozenStateResponse(updatedResponse));
      expect(state.frozenStateResponseData?.profitYear).toBe(2025);
    });

    it("should handle error then success flow", () => {
      let state = reducer(initialState, setFrozenStateResponse(null));
      expect(state.error).toBe("Failed to fetch frozen state");

      state = reducer(state, setFrozenStateResponse(mockFrozenStateResponse));
      expect(state.error).toBeNull();
      expect(state.frozenStateResponseData).toEqual(mockFrozenStateResponse);
    });
  });
});
