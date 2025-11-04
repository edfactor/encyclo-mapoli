import { renderHook } from "@testing-library/react";
import { act } from "react";
import { describe, expect, it, vi } from "vitest";
import useExecutiveModal from "../useExecutiveModal";

describe("useExecutiveModal", () => {
  const mockExecutive = {
    badgeNumber: 12345,
    fullName: "John Doe",
    storeNumber: 1,
    socialSecurity: 123456789,
    hoursExecutive: 40,
    incomeExecutive: 1000,
    currentHoursYear: 35,
    currentIncomeYear: 900,
    payFrequencyId: 1,
    payFrequencyName: "Monthly",
    employmentStatusId: "1",
    employmentStatusName: "Active"
  };

  const defaultProps = {
    isOpen: false,
    selectedExecutives: [],
    onClose: vi.fn(),
    onSearch: vi.fn(),
    onSelectExecutives: vi.fn(),
    onAddExecutives: vi.fn(),
    isSearching: false
  };

  it("should initialize with correct values", () => {
    const { result } = renderHook(() => useExecutiveModal(defaultProps));

    expect(result.current.isModalOpen).toBe(false);
    expect(result.current.selectedExecutives).toEqual([]);
    expect(result.current.isModalSearching).toBe(false);
    expect(result.current.canAddExecutives).toBe(false);
  });

  it("should reflect modal open state", () => {
    const { result } = renderHook(() =>
      useExecutiveModal({
        ...defaultProps,
        isOpen: true
      })
    );

    expect(result.current.isModalOpen).toBe(true);
  });

  it("should reflect selected executives", () => {
    const { result } = renderHook(() =>
      useExecutiveModal({
        ...defaultProps,
        selectedExecutives: [mockExecutive]
      })
    );

    expect(result.current.selectedExecutives).toEqual([mockExecutive]);
    expect(result.current.canAddExecutives).toBe(true);
  });

  it("should reflect searching state", () => {
    const { result } = renderHook(() =>
      useExecutiveModal({
        ...defaultProps,
        isSearching: true
      })
    );

    expect(result.current.isModalSearching).toBe(true);
  });

  describe("handleModalSearch", () => {
    it("should call onSearch with search form data", () => {
      const onSearch = vi.fn();
      const { result } = renderHook(() =>
        useExecutiveModal({
          ...defaultProps,
          onSearch
        })
      );

      const searchForm = { name: "John", badgeNumber: "12345" };

      act(() => {
        result.current.handleModalSearch(searchForm);
      });

      expect(onSearch).toHaveBeenCalledWith(searchForm);
      expect(onSearch).toHaveBeenCalledTimes(1);
    });

    it("should be stable across re-renders", () => {
      const onSearch = vi.fn();
      const { result, rerender } = renderHook(() =>
        useExecutiveModal({
          ...defaultProps,
          onSearch
        })
      );

      const firstHandler = result.current.handleModalSearch;

      rerender();

      expect(result.current.handleModalSearch).toBe(firstHandler);
    });
  });

  describe("handleModalReset", () => {
    it("should call onSelectExecutives with empty array", () => {
      const onSelectExecutives = vi.fn();
      const { result } = renderHook(() =>
        useExecutiveModal({
          ...defaultProps,
          onSelectExecutives
        })
      );

      act(() => {
        result.current.handleModalReset();
      });

      expect(onSelectExecutives).toHaveBeenCalledWith([]);
      expect(onSelectExecutives).toHaveBeenCalledTimes(1);
    });
  });

  describe("handleExecutiveSelection", () => {
    it("should call onSelectExecutives with selected executives", () => {
      const onSelectExecutives = vi.fn();
      const { result } = renderHook(() =>
        useExecutiveModal({
          ...defaultProps,
          onSelectExecutives
        })
      );

      const executives = [mockExecutive];

      act(() => {
        result.current.handleExecutiveSelection(executives);
      });

      expect(onSelectExecutives).toHaveBeenCalledWith(executives);
      expect(onSelectExecutives).toHaveBeenCalledTimes(1);
    });

    it("should work with multiple executives", () => {
      const onSelectExecutives = vi.fn();
      const { result } = renderHook(() =>
        useExecutiveModal({
          ...defaultProps,
          onSelectExecutives
        })
      );

      const executives = [mockExecutive, { ...mockExecutive, badgeNumber: 22222, fullName: "Jane Doe" }];

      act(() => {
        result.current.handleExecutiveSelection(executives);
      });

      expect(onSelectExecutives).toHaveBeenCalledWith(executives);
    });
  });

  describe("handleAddToMainGrid", () => {
    it("should call onAddExecutives when executives are selected", () => {
      const onAddExecutives = vi.fn();
      const { result } = renderHook(() =>
        useExecutiveModal({
          ...defaultProps,
          selectedExecutives: [mockExecutive],
          onAddExecutives
        })
      );

      act(() => {
        result.current.handleAddToMainGrid();
      });

      expect(onAddExecutives).toHaveBeenCalledTimes(1);
    });

    it("should not call onAddExecutives when no executives selected", () => {
      const onAddExecutives = vi.fn();
      const { result } = renderHook(() =>
        useExecutiveModal({
          ...defaultProps,
          selectedExecutives: [],
          onAddExecutives
        })
      );

      act(() => {
        result.current.handleAddToMainGrid();
      });

      expect(onAddExecutives).not.toHaveBeenCalled();
    });
  });

  describe("handleModalClose", () => {
    it("should call onClose", () => {
      const onClose = vi.fn();
      const { result } = renderHook(() =>
        useExecutiveModal({
          ...defaultProps,
          onClose
        })
      );

      act(() => {
        result.current.handleModalClose();
      });

      expect(onClose).toHaveBeenCalledTimes(1);
    });
  });

  describe("canAddExecutives", () => {
    it("should be true when executives are selected", () => {
      const { result } = renderHook(() =>
        useExecutiveModal({
          ...defaultProps,
          selectedExecutives: [mockExecutive]
        })
      );

      expect(result.current.canAddExecutives).toBe(true);
    });

    it("should be false when no executives are selected", () => {
      const { result } = renderHook(() =>
        useExecutiveModal({
          ...defaultProps,
          selectedExecutives: []
        })
      );

      expect(result.current.canAddExecutives).toBe(false);
    });

    it("should be true when multiple executives are selected", () => {
      const { result } = renderHook(() =>
        useExecutiveModal({
          ...defaultProps,
          selectedExecutives: [mockExecutive, { ...mockExecutive, badgeNumber: 22222 }]
        })
      );

      expect(result.current.canAddExecutives).toBe(true);
    });
  });

  describe("prop updates", () => {
    it("should update when selectedExecutives changes", () => {
      const { result, rerender } = renderHook(
        ({ executives }) =>
          useExecutiveModal({
            ...defaultProps,
            selectedExecutives: executives
          }),
        { initialProps: { executives: [] } }
      );

      expect(result.current.selectedExecutives).toEqual([]);
      expect(result.current.canAddExecutives).toBe(false);

      rerender({ executives: [mockExecutive] });

      expect(result.current.selectedExecutives).toEqual([mockExecutive]);
      expect(result.current.canAddExecutives).toBe(true);
    });

    it("should update when isOpen changes", () => {
      const { result, rerender } = renderHook(
        ({ open }) =>
          useExecutiveModal({
            ...defaultProps,
            isOpen: open
          }),
        { initialProps: { open: false } }
      );

      expect(result.current.isModalOpen).toBe(false);

      rerender({ open: true });

      expect(result.current.isModalOpen).toBe(true);
    });

    it("should update when isSearching changes", () => {
      const { result, rerender } = renderHook(
        ({ searching }) =>
          useExecutiveModal({
            ...defaultProps,
            isSearching: searching
          }),
        { initialProps: { searching: false } }
      );

      expect(result.current.isModalSearching).toBe(false);

      rerender({ searching: true });

      expect(result.current.isModalSearching).toBe(true);
    });
  });

  describe("callback stability", () => {
    it("should maintain callback references when deps do not change", () => {
      const onSearch = vi.fn();
      const onSelectExecutives = vi.fn();
      const onAddExecutives = vi.fn();

      const { result, rerender } = renderHook(() =>
        useExecutiveModal({
          ...defaultProps,
          onSearch,
          onSelectExecutives,
          onAddExecutives,
          selectedExecutives: [mockExecutive]
        })
      );

      const firstHandlers = {
        handleModalSearch: result.current.handleModalSearch,
        handleModalReset: result.current.handleModalReset,
        handleExecutiveSelection: result.current.handleExecutiveSelection
      };

      rerender();

      expect(result.current.handleModalSearch).toBe(firstHandlers.handleModalSearch);
      expect(result.current.handleModalReset).toBe(firstHandlers.handleModalReset);
      expect(result.current.handleExecutiveSelection).toBe(firstHandlers.handleExecutiveSelection);
    });

    it("should update handleAddToMainGrid when selectedExecutives length changes", () => {
      const onAddExecutives = vi.fn();

      const { result, rerender } = renderHook(
        ({ executives }: { executives: (typeof mockExecutive)[] }) =>
          useExecutiveModal({
            ...defaultProps,
            onAddExecutives,
            selectedExecutives: executives
          }),
        { initialProps: { executives: [] } }
      );

      const firstHandler = result.current.handleAddToMainGrid;

      rerender({ executives: [mockExecutive] });

      expect(result.current.handleAddToMainGrid).not.toBe(firstHandler);
    });
  });
});
