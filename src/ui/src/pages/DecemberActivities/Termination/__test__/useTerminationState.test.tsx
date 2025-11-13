import { renderHook } from "@testing-library/react";
import { act } from "react";
import { describe, expect, it } from "vitest";
import { useTerminationState } from "../hooks/useTerminationState";

describe("useTerminationState", () => {
  it("should initialize with default state", () => {
    const { result } = renderHook(() => useTerminationState());

    expect(result.current.state.searchParams).toBeNull();
    expect(result.current.state.initialSearchLoaded).toBe(false);
    expect(result.current.state.hasUnsavedChanges).toBe(false);
    expect(result.current.state.resetPageFlag).toBe(false);
    expect(result.current.state.currentStatus).toBeNull();
    expect(result.current.state.archiveMode).toBe(false);
    expect(result.current.state.shouldArchive).toBe(false);
  });

  describe("handleSearch", () => {
    it("should set search params and mark initial search as loaded", () => {
      const { result } = renderHook(() => useTerminationState());

      const searchParams = {
        profitYear: 2024,
        beginningDate: "2024-01-01",
        endingDate: "2024-12-31",
        forfeitureStatus: "all",
        pagination: { skip: 0, take: 25, sortBy: "name", isSortDescending: false }
      };

      act(() => {
        result.current.actions.handleSearch(searchParams);
      });

      expect(result.current.state.searchParams).toEqual(searchParams);
      expect(result.current.state.initialSearchLoaded).toBe(true);
    });

    it("should toggle reset page flag when search is performed", () => {
      const { result } = renderHook(() => useTerminationState());

      const initialResetFlag = result.current.state.resetPageFlag;

      const searchParams = {
        profitYear: 2024,
        beginningDate: "2024-01-01",
        endingDate: "2024-12-31",
        forfeitureStatus: "all",
        pagination: { skip: 0, take: 25, sortBy: "name", isSortDescending: false }
      };

      act(() => {
        result.current.actions.handleSearch(searchParams);
      });

      expect(result.current.state.resetPageFlag).toBe(!initialResetFlag);
    });
  });

  describe("handleUnsavedChanges", () => {
    it("should update unsaved changes state", () => {
      const { result } = renderHook(() => useTerminationState());

      act(() => {
        result.current.actions.handleUnsavedChanges(true);
      });

      expect(result.current.state.hasUnsavedChanges).toBe(true);
    });

    it("should clear unsaved changes", () => {
      const { result } = renderHook(() => useTerminationState());

      act(() => {
        result.current.actions.handleUnsavedChanges(true);
      });

      act(() => {
        result.current.actions.handleUnsavedChanges(false);
      });

      expect(result.current.state.hasUnsavedChanges).toBe(false);
    });
  });

  describe("handleStatusChange", () => {
    it("should set archive mode when changing to complete status", () => {
      const { result } = renderHook(() => useTerminationState());

      act(() => {
        result.current.actions.handleStatusChange("complete", "Complete");
      });

      expect(result.current.state.currentStatus).toBe("Complete");
      expect(result.current.state.archiveMode).toBe(true);
      expect(result.current.state.shouldArchive).toBe(true);
    });

    it("should handle status name with 'complete' in lowercase", () => {
      const { result } = renderHook(() => useTerminationState());

      act(() => {
        result.current.actions.handleStatusChange("status1", "complete");
      });

      expect(result.current.state.archiveMode).toBe(true);
      expect(result.current.state.shouldArchive).toBe(true);
    });

    it("should handle status name with 'Complete' in mixed case", () => {
      const { result } = renderHook(() => useTerminationState());

      act(() => {
        result.current.actions.handleStatusChange("status1", "Task Complete");
      });

      expect(result.current.state.archiveMode).toBe(true);
      expect(result.current.state.shouldArchive).toBe(true);
    });

    it("should not set archive mode for non-complete status", () => {
      const { result } = renderHook(() => useTerminationState());

      act(() => {
        result.current.actions.handleStatusChange("in-progress", "In Progress");
      });

      expect(result.current.state.archiveMode).toBe(false);
      expect(result.current.state.shouldArchive).toBe(false);
    });

    it("should not trigger archive if already in complete status", () => {
      const { result } = renderHook(() => useTerminationState());

      act(() => {
        result.current.actions.handleStatusChange("complete", "Complete");
      });

      act(() => {
        result.current.actions.handleArchiveHandled();
      });

      expect(result.current.state.shouldArchive).toBe(false);

      act(() => {
        result.current.actions.handleStatusChange("complete", "Complete");
      });

      expect(result.current.state.shouldArchive).toBe(false);
    });
  });

  describe("handleArchiveHandled", () => {
    it("should clear shouldArchive flag", () => {
      const { result } = renderHook(() => useTerminationState());

      act(() => {
        result.current.actions.handleStatusChange("complete", "Complete");
      });

      expect(result.current.state.shouldArchive).toBe(true);

      act(() => {
        result.current.actions.handleArchiveHandled();
      });

      expect(result.current.state.shouldArchive).toBe(false);
    });
  });

  describe("setInitialSearchLoaded", () => {
    it("should update initial search loaded state", () => {
      const { result } = renderHook(() => useTerminationState());

      act(() => {
        result.current.actions.setInitialSearchLoaded(true);
      });

      expect(result.current.state.initialSearchLoaded).toBe(true);
    });
  });

  describe("complex scenarios", () => {
    it("should handle complete workflow", () => {
      const { result } = renderHook(() => useTerminationState());

      const searchParams = {
        profitYear: 2024,
        beginningDate: "2024-01-01",
        endingDate: "2024-12-31",
        forfeitureStatus: "all",
        pagination: { skip: 0, take: 25, sortBy: "name", isSortDescending: false }
      };

      act(() => {
        result.current.actions.handleSearch(searchParams);
      });

      expect(result.current.state.initialSearchLoaded).toBe(true);

      act(() => {
        result.current.actions.handleUnsavedChanges(true);
      });

      expect(result.current.state.hasUnsavedChanges).toBe(true);

      act(() => {
        result.current.actions.handleStatusChange("complete", "Complete");
      });

      expect(result.current.state.archiveMode).toBe(true);
      expect(result.current.state.shouldArchive).toBe(true);

      act(() => {
        result.current.actions.handleArchiveHandled();
      });

      expect(result.current.state.shouldArchive).toBe(false);
      expect(result.current.state.archiveMode).toBe(true);
    });
  });
});
