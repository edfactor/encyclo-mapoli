import { act, renderHook, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { useAdhocProfLetter73Print } from "../useAdhocProfLetter73Print";

// Mock the API hook
vi.mock("../../../../../reduxstore/api/AdhocProfLetter73Api", () => ({
  useLazyDownloadAdhocProfLetter73FormLetterQuery: vi.fn(() => [
    vi.fn(),
    {
      isFetching: false,
      isLoading: false,
      isSuccess: false,
      isError: false,
      error: null,
      data: undefined,
      currentData: undefined,
      isUninitialized: true,
      reset: vi.fn()
    },
    { lastArg: undefined as never }
  ])
}));

describe("useAdhocProfLetter73Print", () => {
  it("should initialize with default state", () => {
    const filterParams = {
      profitYear: new Date(2023, 0, 1)
    };
    const selectedRows: Record<string, unknown>[] = [];

    const { result } = renderHook(() => useAdhocProfLetter73Print(filterParams, selectedRows));

    expect(result.current.isDownloading).toBe(false);
    expect(result.current.isPrintDialogOpen).toBe(false);
    expect(result.current.printContent).toBe("");
    expect(result.current.error).toBeNull();
    expect(result.current.handlePrint).toBeDefined();
    expect(result.current.printFormLetter).toBeDefined();
    expect(result.current.setIsPrintDialogOpen).toBeDefined();
    expect(result.current.clearError).toBeDefined();
  });

  it("should not call API when no profit year is selected", async () => {
    const filterParams = {
      profitYear: null
    };
    const selectedRows: Record<string, unknown>[] = [{ badgeNumber: "123456" }];

    const mockTriggerDownload = vi.fn();
    const { useLazyDownloadAdhocProfLetter73FormLetterQuery } =
      await import("../../../../../reduxstore/api/AdhocProfLetter73Api");
    vi.mocked(useLazyDownloadAdhocProfLetter73FormLetterQuery).mockReturnValue([
      mockTriggerDownload,
      {
        isFetching: false,
        isLoading: false,
        isSuccess: false,
        isError: false,
        error: null,
        data: undefined,
        currentData: undefined,
        isUninitialized: true,
        reset: vi.fn()
      },
      { lastArg: undefined as never }
    ]);

    const { result } = renderHook(() => useAdhocProfLetter73Print(filterParams, selectedRows));

    await result.current.handlePrint();

    expect(mockTriggerDownload).not.toHaveBeenCalled();
  });

  it("should not call API when no rows are selected", async () => {
    const filterParams = {
      profitYear: new Date(2023, 0, 1)
    };
    const selectedRows: Record<string, unknown>[] = [];

    const mockTriggerDownload = vi.fn();
    const { useLazyDownloadAdhocProfLetter73FormLetterQuery } =
      await import("../../../../../reduxstore/api/AdhocProfLetter73Api");
    vi.mocked(useLazyDownloadAdhocProfLetter73FormLetterQuery).mockReturnValue([
      mockTriggerDownload,
      {
        isFetching: false,
        isLoading: false,
        isSuccess: false,
        isError: false,
        error: null,
        data: undefined,
        currentData: undefined,
        isUninitialized: true,
        reset: vi.fn()
      },
      { lastArg: undefined as never }
    ]);

    const { result } = renderHook(() => useAdhocProfLetter73Print(filterParams, selectedRows));

    await result.current.handlePrint();

    expect(mockTriggerDownload).not.toHaveBeenCalled();
  });

  it("should call API with correct badge numbers when rows are selected", async () => {
    const filterParams = {
      profitYear: new Date(2023, 0, 1)
    };
    const selectedRows: Record<string, unknown>[] = [
      { badgeNumber: "123456", name: "John Doe" },
      { badgeNumber: "789012", name: "Jane Smith" }
    ];

    const mockBlob = {
      text: vi.fn().mockResolvedValue("Test content")
    };
    const mockTriggerDownload = vi.fn().mockResolvedValue({
      data: mockBlob
    });

    const { useLazyDownloadAdhocProfLetter73FormLetterQuery } =
      await import("../../../../../reduxstore/api/AdhocProfLetter73Api");
    vi.mocked(useLazyDownloadAdhocProfLetter73FormLetterQuery).mockReturnValue([
      mockTriggerDownload,
      {
        isFetching: false,
        isLoading: false,
        isSuccess: false,
        isError: false,
        error: null,
        data: undefined,
        currentData: undefined,
        isUninitialized: true,
        reset: vi.fn()
      },
      { lastArg: undefined as never }
    ]);

    const { result } = renderHook(() => useAdhocProfLetter73Print(filterParams, selectedRows));

    await result.current.handlePrint();

    await waitFor(() => {
      expect(mockTriggerDownload).toHaveBeenCalledWith({
        profitYear: 2023,
        badgeNumbers: ["123456", "789012"]
      });
    });
  });

  it("should filter out null/undefined badge numbers", async () => {
    const filterParams = {
      profitYear: new Date(2023, 0, 1)
    };
    const selectedRows: Record<string, unknown>[] = [
      { badgeNumber: "123456", name: "John Doe" },
      { badgeNumber: null, name: "Invalid" },
      { badgeNumber: "789012", name: "Jane Smith" },
      { name: "No Badge" }
    ];

    const mockBlob = {
      text: vi.fn().mockResolvedValue("Test content")
    };
    const mockTriggerDownload = vi.fn().mockResolvedValue({
      data: mockBlob
    });

    const { useLazyDownloadAdhocProfLetter73FormLetterQuery } =
      await import("../../../../../reduxstore/api/AdhocProfLetter73Api");
    vi.mocked(useLazyDownloadAdhocProfLetter73FormLetterQuery).mockReturnValue([
      mockTriggerDownload,
      {
        isFetching: false,
        isLoading: false,
        isSuccess: false,
        isError: false,
        error: null,
        data: undefined,
        currentData: undefined,
        isUninitialized: true,
        reset: vi.fn()
      },
      { lastArg: undefined as never }
    ]);

    const { result } = renderHook(() => useAdhocProfLetter73Print(filterParams, selectedRows));

    await act(async () => {
      await result.current.handlePrint();
    });

    await waitFor(() => {
      expect(mockTriggerDownload).toHaveBeenCalledWith({
        profitYear: 2023,
        badgeNumbers: ["123456", "789012"]
      });
    });
  });

  it("should open print dialog when API call succeeds", async () => {
    const filterParams = {
      profitYear: new Date(2023, 0, 1)
    };
    const selectedRows: Record<string, unknown>[] = [{ badgeNumber: "123456" }];

    const mockContent = "Form letter content";
    const mockBlob = {
      text: vi.fn().mockResolvedValue(mockContent)
    };
    const mockTriggerDownload = vi.fn().mockResolvedValue({
      data: mockBlob
    });

    const { useLazyDownloadAdhocProfLetter73FormLetterQuery } =
      await import("../../../../../reduxstore/api/AdhocProfLetter73Api");
    vi.mocked(useLazyDownloadAdhocProfLetter73FormLetterQuery).mockReturnValue([
      mockTriggerDownload,
      {
        isFetching: false,
        isLoading: false,
        isSuccess: false,
        isError: false,
        error: null,
        data: undefined,
        currentData: undefined,
        isUninitialized: true,
        reset: vi.fn()
      },
      { lastArg: undefined as never }
    ]);

    const { result } = renderHook(() => useAdhocProfLetter73Print(filterParams, selectedRows));

    await act(async () => {
      await result.current.handlePrint();
    });

    await waitFor(() => {
      expect(result.current.isPrintDialogOpen).toBe(true);
      expect(result.current.printContent).toBe(mockContent);
    });
  });

  it("should handle API errors gracefully", async () => {
    const filterParams = {
      profitYear: new Date(2023, 0, 1)
    };
    const selectedRows: Record<string, unknown>[] = [{ badgeNumber: "123456" }];

    const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});
    const mockTriggerDownload = vi.fn().mockRejectedValue(new Error("API Error"));

    const { useLazyDownloadAdhocProfLetter73FormLetterQuery } =
      await import("../../../../../reduxstore/api/AdhocProfLetter73Api");
    vi.mocked(useLazyDownloadAdhocProfLetter73FormLetterQuery).mockReturnValue([
      mockTriggerDownload,
      {
        isFetching: false,
        isLoading: false,
        isSuccess: false,
        isError: false,
        error: null,
        data: undefined,
        currentData: undefined,
        isUninitialized: true,
        reset: vi.fn()
      },
      { lastArg: undefined as never }
    ]);

    const { result } = renderHook(() => useAdhocProfLetter73Print(filterParams, selectedRows));

    await act(async () => {
      await result.current.handlePrint();
    });

    await waitFor(() => {
      expect(consoleErrorSpy).toHaveBeenCalledWith("Error downloading prof letter 73 form letter:", expect.any(Error));
      expect(result.current.isPrintDialogOpen).toBe(false);
      expect(result.current.error).toBe("An unexpected error occurred. Please try again.");
    });

    consoleErrorSpy.mockRestore();
  });

  it("should allow closing the print dialog", () => {
    const filterParams = {
      profitYear: new Date(2023, 0, 1)
    };
    const selectedRows: Record<string, unknown>[] = [];

    const { result } = renderHook(() => useAdhocProfLetter73Print(filterParams, selectedRows));

    act(() => {
      result.current.setIsPrintDialogOpen(true);
    });
    expect(result.current.isPrintDialogOpen).toBe(true);

    act(() => {
      result.current.setIsPrintDialogOpen(false);
    });
    expect(result.current.isPrintDialogOpen).toBe(false);
  });

  it("should open print window with correct content", () => {
    const filterParams = {
      profitYear: new Date(2023, 0, 1)
    };
    const selectedRows: Record<string, unknown>[] = [];

    const mockPrintWindow = {
      document: {
        write: vi.fn(),
        close: vi.fn()
      },
      focus: vi.fn(),
      print: vi.fn(),
      close: vi.fn()
    };

    const openSpy = vi.spyOn(window, "open").mockReturnValue(mockPrintWindow as unknown as Window);

    const { result } = renderHook(() => useAdhocProfLetter73Print(filterParams, selectedRows));

    const testContent = "Test form letter\nWith multiple lines";
    result.current.printFormLetter(testContent);

    expect(openSpy).toHaveBeenCalledWith("", "_blank");
    expect(mockPrintWindow.document.write).toHaveBeenCalledWith(
      expect.stringContaining("Test form letter\nWith multiple lines")
    );
    expect(mockPrintWindow.document.close).toHaveBeenCalled();
    expect(mockPrintWindow.focus).toHaveBeenCalled();
    expect(mockPrintWindow.print).toHaveBeenCalled();
    // Note: mockPrintWindow.close is called after a 1000ms timeout, so we don't test it here

    openSpy.mockRestore();
  });

  it("should handle null print window gracefully", () => {
    const filterParams = {
      profitYear: new Date(2023, 0, 1)
    };
    const selectedRows: Record<string, unknown>[] = [];

    const openSpy = vi.spyOn(window, "open").mockReturnValue(null);

    const { result } = renderHook(() => useAdhocProfLetter73Print(filterParams, selectedRows));

    // Should not throw error
    act(() => {
      result.current.printFormLetter("Test content");
    });

    expect(result.current.error).toBe("Failed to open print window. Please check your popup blocker settings.");

    openSpy.mockRestore();
  });

  it("should set error when no valid badge numbers found", async () => {
    const filterParams = {
      profitYear: new Date(2023, 0, 1)
    };
    const selectedRows: Record<string, unknown>[] = [
      { badgeNumber: null, name: "Invalid" },
      { badgeNumber: "", name: "Empty" },
      { name: "No Badge" }
    ];

    const mockTriggerDownload = vi.fn();
    const { useLazyDownloadAdhocProfLetter73FormLetterQuery } =
      await import("../../../../../reduxstore/api/AdhocProfLetter73Api");
    vi.mocked(useLazyDownloadAdhocProfLetter73FormLetterQuery).mockReturnValue([
      mockTriggerDownload,
      {
        isFetching: false,
        isLoading: false,
        isSuccess: false,
        isError: false,
        error: null,
        data: undefined,
        currentData: undefined,
        isUninitialized: true,
        reset: vi.fn()
      },
      { lastArg: undefined as never }
    ]);

    const { result } = renderHook(() => useAdhocProfLetter73Print(filterParams, selectedRows));

    await act(async () => {
      await result.current.handlePrint();
    });

    expect(mockTriggerDownload).not.toHaveBeenCalled();
    expect(result.current.error).toBe(
      "No valid badge numbers found in selected rows. Please ensure the data includes badge number information."
    );
  });

  it("should set error when API returns error response", async () => {
    const filterParams = {
      profitYear: new Date(2023, 0, 1)
    };
    const selectedRows: Record<string, unknown>[] = [{ badgeNumber: "123456" }];

    const mockTriggerDownload = vi.fn().mockResolvedValue({
      error: { status: 500, data: "Server error" }
    });

    const { useLazyDownloadAdhocProfLetter73FormLetterQuery } =
      await import("../../../../../reduxstore/api/AdhocProfLetter73Api");
    vi.mocked(useLazyDownloadAdhocProfLetter73FormLetterQuery).mockReturnValue([
      mockTriggerDownload,
      {
        isFetching: false,
        isLoading: false,
        isSuccess: false,
        isError: false,
        error: null,
        data: undefined,
        currentData: undefined,
        isUninitialized: true,
        reset: vi.fn()
      },
      { lastArg: undefined as never }
    ]);

    const { result } = renderHook(() => useAdhocProfLetter73Print(filterParams, selectedRows));

    await act(async () => {
      await result.current.handlePrint();
    });

    expect(result.current.error).toBe("Failed to generate form letter. Please try again.");
    expect(result.current.isPrintDialogOpen).toBe(false);
  });

  it("should clear error when clearError is called", () => {
    const filterParams = {
      profitYear: new Date(2023, 0, 1)
    };
    const selectedRows: Record<string, unknown>[] = [];

    const { result } = renderHook(() => useAdhocProfLetter73Print(filterParams, selectedRows));

    // Manually set error state (in real use, it would be set by handlePrint)
    act(() => {
      result.current.printFormLetter("Test content");
    });

    const openSpy = vi.spyOn(window, "open").mockReturnValue(null);
    act(() => {
      result.current.printFormLetter("Test content");
    });
    expect(result.current.error).toBeTruthy();

    act(() => {
      result.current.clearError();
    });

    expect(result.current.error).toBeNull();

    openSpy.mockRestore();
  });

  it("should clear previous error when handlePrint is called again", async () => {
    const filterParams = {
      profitYear: new Date(2023, 0, 1)
    };
    const selectedRows: Record<string, unknown>[] = [{ badgeNumber: "123456" }];

    const mockBlob = {
      text: vi.fn().mockResolvedValue("Test content")
    };
    const mockTriggerDownload = vi.fn().mockResolvedValue({
      data: mockBlob
    });

    const { useLazyDownloadAdhocProfLetter73FormLetterQuery } =
      await import("../../../../../reduxstore/api/AdhocProfLetter73Api");
    vi.mocked(useLazyDownloadAdhocProfLetter73FormLetterQuery).mockReturnValue([
      mockTriggerDownload,
      {
        isFetching: false,
        isLoading: false,
        isSuccess: false,
        isError: false,
        error: null,
        data: undefined,
        currentData: undefined,
        isUninitialized: true,
        reset: vi.fn()
      },
      { lastArg: undefined as never }
    ]);

    const { result } = renderHook(() => useAdhocProfLetter73Print(filterParams, selectedRows));

    // Simulate error by calling printFormLetter with null window
    const openSpy = vi.spyOn(window, "open").mockReturnValue(null);
    act(() => {
      result.current.printFormLetter("Test");
    });
    expect(result.current.error).toBeTruthy();

    // Restore window.open and call handlePrint
    openSpy.mockRestore();

    await act(async () => {
      await result.current.handlePrint();
    });

    await waitFor(() => {
      expect(result.current.error).toBeNull();
    });
  });
});
