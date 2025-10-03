import { renderHook } from "@testing-library/react";
import { act } from "react";
import { describe, expect, it } from "vitest";
import { useUnForfeitState } from "./useUnForfeitState";

describe("useUnForfeitState", () => {
  it("should initialize with default state", () => {
    const { result } = renderHook(() => useUnForfeitState());

    expect(result.current.state.initialSearchLoaded).toBe(false);
    expect(result.current.state.resetPageFlag).toBe(false);
    expect(result.current.state.hasUnsavedChanges).toBe(false);
    expect(result.current.state.shouldBlock).toBe(false);
    expect(result.current.state.previousStatus).toBeNull();
    expect(result.current.state.shouldArchive).toBe(false);
  });

  describe("setInitialSearchLoaded", () => {
    it("should update initial search loaded state", () => {
      const { result } = renderHook(() => useUnForfeitState());

      act(() => {
        result.current.actions.setInitialSearchLoaded(true);
      });

      expect(result.current.state.initialSearchLoaded).toBe(true);
    });

    it("should toggle initial search loaded state", () => {
      const { result } = renderHook(() => useUnForfeitState());

      act(() => {
        result.current.actions.setInitialSearchLoaded(true);
      });

      expect(result.current.state.initialSearchLoaded).toBe(true);

      act(() => {
        result.current.actions.setInitialSearchLoaded(false);
      });

      expect(result.current.state.initialSearchLoaded).toBe(false);
    });
  });

  describe("handleSearch", () => {
    it("should toggle reset page flag", () => {
      const { result } = renderHook(() => useUnForfeitState());

      const initialFlag = result.current.state.resetPageFlag;

      act(() => {
        result.current.actions.handleSearch();
      });

      expect(result.current.state.resetPageFlag).toBe(!initialFlag);
    });

    it("should toggle reset page flag multiple times", () => {
      const { result } = renderHook(() => useUnForfeitState());

      const initialFlag = result.current.state.resetPageFlag;

      act(() => {
        result.current.actions.handleSearch();
      });

      expect(result.current.state.resetPageFlag).toBe(!initialFlag);

      act(() => {
        result.current.actions.handleSearch();
      });

      expect(result.current.state.resetPageFlag).toBe(initialFlag);
    });
  });

  describe("handleUnsavedChanges", () => {
    it("should update unsaved changes and should block", () => {
      const { result } = renderHook(() => useUnForfeitState());

      act(() => {
        result.current.actions.handleUnsavedChanges(true);
      });

      expect(result.current.state.hasUnsavedChanges).toBe(true);
      expect(result.current.state.shouldBlock).toBe(true);
    });

    it("should clear unsaved changes and should block", () => {
      const { result } = renderHook(() => useUnForfeitState());

      act(() => {
        result.current.actions.handleUnsavedChanges(true);
      });

      act(() => {
        result.current.actions.handleUnsavedChanges(false);
      });

      expect(result.current.state.hasUnsavedChanges).toBe(false);
      expect(result.current.state.shouldBlock).toBe(false);
    });
  });

  describe("setShouldBlock", () => {
    it("should update should block state", () => {
      const { result } = renderHook(() => useUnForfeitState());

      act(() => {
        result.current.actions.setShouldBlock(true);
      });

      expect(result.current.state.shouldBlock).toBe(true);
    });

    it("should clear should block state", () => {
      const { result } = renderHook(() => useUnForfeitState());

      act(() => {
        result.current.actions.setShouldBlock(true);
      });

      act(() => {
        result.current.actions.setShouldBlock(false);
      });

      expect(result.current.state.shouldBlock).toBe(false);
    });
  });

  describe("handleStatusChange", () => {
    it("should set archive flag when changing to complete status", () => {
      const { result } = renderHook(() => useUnForfeitState());

      act(() => {
        result.current.actions.handleStatusChange("status1", "Complete");
      });

      expect(result.current.state.previousStatus).toBe("status1");
      expect(result.current.state.shouldArchive).toBe(true);
    });

    it("should handle status name with 'complete' in lowercase", () => {
      const { result } = renderHook(() => useUnForfeitState());

      act(() => {
        result.current.actions.handleStatusChange("status1", "complete");
      });

      expect(result.current.state.shouldArchive).toBe(true);
    });

    it("should handle status name with 'Complete' in mixed case", () => {
      const { result } = renderHook(() => useUnForfeitState());

      act(() => {
        result.current.actions.handleStatusChange("status1", "Task Complete");
      });

      expect(result.current.state.shouldArchive).toBe(true);
    });

    it("should not set archive flag for non-complete status", () => {
      const { result } = renderHook(() => useUnForfeitState());

      act(() => {
        result.current.actions.handleStatusChange("status1", "In Progress");
      });

      expect(result.current.state.shouldArchive).toBe(false);
    });

    it("should not trigger archive when changing to the same complete status", () => {
      const { result } = renderHook(() => useUnForfeitState());

      act(() => {
        result.current.actions.handleStatusChange("status1", "Complete");
      });

      expect(result.current.state.shouldArchive).toBe(true);

      act(() => {
        result.current.actions.handleArchiveHandled();
      });

      expect(result.current.state.shouldArchive).toBe(false);

      // Change to same status again - should not trigger archive
      act(() => {
        result.current.actions.handleStatusChange("status1", "Complete");
      });

      expect(result.current.state.shouldArchive).toBe(false);
    });

    it("should trigger archive when changing from one status to another complete status", () => {
      const { result } = renderHook(() => useUnForfeitState());

      act(() => {
        result.current.actions.handleStatusChange("status1", "In Progress");
      });

      expect(result.current.state.shouldArchive).toBe(false);

      act(() => {
        result.current.actions.handleStatusChange("status2", "Complete");
      });

      expect(result.current.state.shouldArchive).toBe(true);
    });
  });

  describe("handleArchiveHandled", () => {
    it("should clear shouldArchive flag", () => {
      const { result } = renderHook(() => useUnForfeitState());

      act(() => {
        result.current.actions.handleStatusChange("status1", "Complete");
      });

      expect(result.current.state.shouldArchive).toBe(true);

      act(() => {
        result.current.actions.handleArchiveHandled();
      });

      expect(result.current.state.shouldArchive).toBe(false);
    });
  });

  describe("complex scenarios", () => {
    it("should handle complete workflow", () => {
      const { result } = renderHook(() => useUnForfeitState());

      // Initial search
      act(() => {
        result.current.actions.setInitialSearchLoaded(true);
      });

      expect(result.current.state.initialSearchLoaded).toBe(true);

      // User makes edits
      act(() => {
        result.current.actions.handleUnsavedChanges(true);
      });

      expect(result.current.state.hasUnsavedChanges).toBe(true);
      expect(result.current.state.shouldBlock).toBe(true);

      // User saves and changes status to complete
      act(() => {
        result.current.actions.handleUnsavedChanges(false);
        result.current.actions.handleStatusChange("complete-status", "Complete");
      });

      expect(result.current.state.hasUnsavedChanges).toBe(false);
      expect(result.current.state.shouldBlock).toBe(false);
      expect(result.current.state.shouldArchive).toBe(true);

      // Archive is handled
      act(() => {
        result.current.actions.handleArchiveHandled();
      });

      expect(result.current.state.shouldArchive).toBe(false);
    });

    it("should handle multiple status changes", () => {
      const { result } = renderHook(() => useUnForfeitState());

      act(() => {
        result.current.actions.handleStatusChange("status1", "In Progress");
      });

      expect(result.current.state.previousStatus).toBe("status1");
      expect(result.current.state.shouldArchive).toBe(false);

      act(() => {
        result.current.actions.handleStatusChange("status2", "Review");
      });

      expect(result.current.state.previousStatus).toBe("status2");
      expect(result.current.state.shouldArchive).toBe(false);

      act(() => {
        result.current.actions.handleStatusChange("status3", "Complete");
      });

      expect(result.current.state.previousStatus).toBe("status3");
      expect(result.current.state.shouldArchive).toBe(true);
    });
  });
});
