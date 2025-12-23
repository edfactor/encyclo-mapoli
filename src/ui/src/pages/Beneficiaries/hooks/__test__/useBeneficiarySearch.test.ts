import { describe, it, expect } from "vitest";
import { renderHook, act } from "@testing-library/react";
import { useBeneficiarySearch } from "../useBeneficiarySearch";

describe("useBeneficiarySearch", () => {
  it("should initialize with default values", () => {
    const { result } = renderHook(() => useBeneficiarySearch());

    expect(result.current.pageNumber).toBe(0);
    expect(result.current.pageSize).toBe(10);
    expect(result.current.sortParams.sortBy).toBe("fullName");
    expect(result.current.sortParams.isSortDescending).toBe(false);
  });

  it("should initialize with config values", () => {
    const { result } = renderHook(() =>
      useBeneficiarySearch({
        defaultPageSize: 25,
        defaultSortBy: "badgeNumber"
      })
    );

    expect(result.current.pageSize).toBe(25);
    expect(result.current.sortParams.sortBy).toBe("badgeNumber");
  });

  it("should update pagination when handlePaginationChange is called", () => {
    const { result } = renderHook(() => useBeneficiarySearch());

    act(() => {
      result.current.handlePaginationChange(2, 25);
    });

    expect(result.current.pageNumber).toBe(2);
    expect(result.current.pageSize).toBe(25);
  });

  it("should update sort params when handleSortChange is called", () => {
    const { result } = renderHook(() => useBeneficiarySearch());

    act(() => {
      result.current.handleSortChange({
        sortBy: "badgeNumber",
        isSortDescending: true
      });
    });

    expect(result.current.sortParams.sortBy).toBe("badgeNumber");
    expect(result.current.sortParams.isSortDescending).toBe(true);
  });

  it("should reset to default values when reset is called", () => {
    const { result } = renderHook(() =>
      useBeneficiarySearch({
        defaultPageSize: 25,
        defaultSortBy: "badgeNumber"
      })
    );

    // Change state
    act(() => {
      result.current.handlePaginationChange(3, 50);
      result.current.handleSortChange({
        sortBy: "ssn",
        isSortDescending: true
      });
    });

    expect(result.current.pageNumber).toBe(3);
    expect(result.current.pageSize).toBe(50);

    // Reset
    act(() => {
      result.current.reset();
    });

    expect(result.current.pageNumber).toBe(0);
    expect(result.current.pageSize).toBe(25);
    expect(result.current.sortParams.sortBy).toBe("badgeNumber");
    expect(result.current.sortParams.isSortDescending).toBe(false);
  });

  it("should reset to default config on each reset call", () => {
    const { result } = renderHook(() =>
      useBeneficiarySearch({
        defaultPageSize: 20
      })
    );

    act(() => {
      result.current.handlePaginationChange(1, 50);
    });

    expect(result.current.pageSize).toBe(50);

    act(() => {
      result.current.reset();
    });

    expect(result.current.pageSize).toBe(20);
  });

  it("should independently manage multiple instances", () => {
    const { result: result1 } = renderHook(() => useBeneficiarySearch({ defaultPageSize: 10 }));
    const { result: result2 } = renderHook(() => useBeneficiarySearch({ defaultPageSize: 25 }));

    act(() => {
      result1.current.handlePaginationChange(1, 15);
    });

    expect(result1.current.pageSize).toBe(15);
    expect(result2.current.pageSize).toBe(25); // Unaffected
  });
});
