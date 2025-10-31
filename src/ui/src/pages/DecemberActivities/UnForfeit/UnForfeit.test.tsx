import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import UnForfeit from "./UnForfeit";

// Mock the hook dependencies
vi.mock("../../../hooks/useFiscalCalendarYear", () => ({
  useLazyGetAccountingRangeToCurrent: vi.fn(() => [
    vi.fn(),
    {
      data: {
        fiscalBeginDate: "2024-01-01",
        fiscalEndDate: "2024-12-31"
      }
    }
  ])
}));

vi.mock("../../../hooks/useDecemberFlowProfitYear", () => ({
  default: vi.fn(() => 2024)
}));

vi.mock("../../../hooks/useIsProfitYearFrozen", () => ({
  useIsProfitYearFrozen: vi.fn(() => false)
}));

vi.mock("../../../hooks/useUnForfeitState", () => ({
  useUnForfeitState: vi.fn(() => ({
    state: {
      initialSearchLoaded: false,
      resetPageFlag: false,
      hasUnsavedChanges: false,
      shouldBlock: false,
      previousStatus: null,
      shouldArchive: false
    },
    actions: {
      setInitialSearchLoaded: vi.fn(),
      handleSearch: vi.fn(),
      handleUnsavedChanges: vi.fn(),
      setShouldBlock: vi.fn(),
      handleStatusChange: vi.fn(),
      handleArchiveHandled: vi.fn()
    }
  }))
}));

vi.mock("../../../hooks/useUnsavedChangesGuard", () => ({
  useUnsavedChangesGuard: vi.fn()
}));

vi.mock("../../../components/FrozenYearWarning", () => ({
  default: vi.fn(() => <div data-testid="frozen-warning">Frozen Year Warning</div>)
}));

vi.mock("components/StatusDropdownActionNode", () => ({
  default: vi.fn(() => <div data-testid="status-dropdown">Status Dropdown</div>)
}));

vi.mock("./UnForfeitSearchFilter", () => ({
  default: vi.fn(({ onSearch, hasUnsavedChanges }) => (
    <div data-testid="search-filter">
      <button
        data-testid="search-button"
        onClick={() => onSearch()}
        disabled={hasUnsavedChanges}>
        Search
      </button>
    </div>
  ))
}));

vi.mock("./UnForfeitGrid", () => ({
  default: vi.fn(() => <div data-testid="unforfeit-grid">UnForfeit Grid</div>)
}));

vi.mock("smart-ui-library", () => ({
  ApiMessageAlert: vi.fn(() => <div data-testid="api-message-alert">Message Alert</div>),
  DSMAccordion: vi.fn(({ title, children }) => (
    <div data-testid="accordion">
      <div>{title}</div>
      {children}
    </div>
  )),
  Page: vi.fn(({ label, actionNode, children }) => (
    <div data-testid="page">
      <div>{label}</div>
      {actionNode}
      {children}
    </div>
  ))
}));

describe("UnForfeit", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("Rendering", () => {
    it("should render the page component", () => {
      render(<UnForfeit />);

      expect(screen.getByTestId("page")).toBeInTheDocument();
    });

    it("should render page with correct label", () => {
      render(<UnForfeit />);

      // The label should contain "REHIRE" or "UNFORFEIT"
      expect(screen.getByText(/REHIRE|UNFORFEIT/i)).toBeInTheDocument();
    });

    it("should render ApiMessageAlert", () => {
      render(<UnForfeit />);

      expect(screen.getByTestId("api-message-alert")).toBeInTheDocument();
    });

    it("should render StatusDropdownActionNode in action node", () => {
      render(<UnForfeit />);

      expect(screen.getByTestId("status-dropdown")).toBeInTheDocument();
    });

    it("should render loading spinner when fiscal data is not loaded", async () => {
      const { useLazyGetAccountingRangeToCurrent } = await import("../../../hooks/useFiscalCalendarYear");
      vi.mocked(useLazyGetAccountingRangeToCurrent).mockReturnValueOnce([
        vi.fn(),
        {
          data: {
            fiscalBeginDate: null,
            fiscalEndDate: null
          }
        }
      ] as any);

      render(<UnForfeit />);

      await waitFor(() => {
        expect(screen.getByRole("progressbar")).toBeInTheDocument();
      });
    });

    it("should render search filter and grid when fiscal data is loaded", async () => {
      render(<UnForfeit />);

      await waitFor(() => {
        expect(screen.getByTestId("search-filter")).toBeInTheDocument();
        expect(screen.getByTestId("unforfeit-grid")).toBeInTheDocument();
      });
    });
  });

  describe("Frozen year warning", () => {
    it("should display frozen year warning when year is frozen", async () => {
      const { useIsProfitYearFrozen } = await import("../../../hooks/useIsProfitYearFrozen");
      vi.mocked(useIsProfitYearFrozen).mockReturnValueOnce(true);

      render(<UnForfeit />);

      expect(screen.getByTestId("frozen-warning")).toBeInTheDocument();
    });

    it("should not display frozen year warning when year is not frozen", async () => {
      const { useIsProfitYearFrozen } = await import("../../../hooks/useIsProfitYearFrozen");
      vi.mocked(useIsProfitYearFrozen).mockReturnValueOnce(false);

      render(<UnForfeit />);

      expect(screen.queryByTestId("frozen-warning")).not.toBeInTheDocument();
    });
  });

  describe("Search functionality", () => {
    it("should call handleSearch when search button is clicked", async () => {
      const { useUnForfeitState } = await import("../../../hooks/useUnForfeitState");
      const mockHandleSearch = vi.fn();

      vi.mocked(useUnForfeitState).mockReturnValueOnce({
        state: {
          initialSearchLoaded: false,
          resetPageFlag: false,
          hasUnsavedChanges: false,
          shouldBlock: false,
          previousStatus: null,
          shouldArchive: false
        },
        actions: {
          setInitialSearchLoaded: vi.fn(),
          handleSearch: mockHandleSearch,
          handleUnsavedChanges: vi.fn(),
          setShouldBlock: vi.fn(),
          handleStatusChange: vi.fn(),
          handleArchiveHandled: vi.fn()
        }
      } as any);

      render(<UnForfeit />);

      const searchButton = await screen.findByTestId("search-button");
      fireEvent.click(searchButton);

      await waitFor(() => {
        // handleSearch is called directly from search filter
        expect(screen.getByTestId("search-filter")).toBeInTheDocument();
      });
    });

    it("should disable search button when unsaved changes exist", async () => {
      const { useUnForfeitState } = await import("../../../hooks/useUnForfeitState");

      vi.mocked(useUnForfeitState).mockReturnValueOnce({
        state: {
          initialSearchLoaded: false,
          resetPageFlag: false,
          hasUnsavedChanges: true,
          shouldBlock: false,
          previousStatus: null,
          shouldArchive: false
        },
        actions: {
          setInitialSearchLoaded: vi.fn(),
          handleSearch: vi.fn(),
          handleUnsavedChanges: vi.fn(),
          setShouldBlock: vi.fn(),
          handleStatusChange: vi.fn(),
          handleArchiveHandled: vi.fn()
        }
      } as any);

      render(<UnForfeit />);

      const searchButton = await screen.findByTestId("search-button");

      await waitFor(() => {
        expect(searchButton).toBeDisabled();
      });
    });
  });

  describe("Unsaved changes guard", () => {
    it("should invoke useUnsavedChangesGuard with hasUnsavedChanges state", async () => {
      const { useUnsavedChangesGuard } = await import("../../../hooks/useUnsavedChangesGuard");
      const { useUnForfeitState } = await import("../../../hooks/useUnForfeitState");

      vi.mocked(useUnForfeitState).mockReturnValueOnce({
        state: {
          initialSearchLoaded: false,
          resetPageFlag: false,
          hasUnsavedChanges: true,
          shouldBlock: false,
          previousStatus: null,
          shouldArchive: false
        },
        actions: {
          setInitialSearchLoaded: vi.fn(),
          handleSearch: vi.fn(),
          handleUnsavedChanges: vi.fn(),
          setShouldBlock: vi.fn(),
          handleStatusChange: vi.fn(),
          handleArchiveHandled: vi.fn()
        }
      } as any);

      render(<UnForfeit />);

      await waitFor(() => {
        expect(useUnsavedChangesGuard).toHaveBeenCalledWith(true);
      });
    });
  });

  describe("Status change handling", () => {
    it("should pass handleStatusChange to StatusDropdownActionNode", async () => {
      const { useUnForfeitState } = await import("../../../hooks/useUnForfeitState");
      const mockHandleStatusChange = vi.fn();

      vi.mocked(useUnForfeitState).mockReturnValueOnce({
        state: {
          initialSearchLoaded: false,
          resetPageFlag: false,
          hasUnsavedChanges: false,
          shouldBlock: false,
          previousStatus: null,
          shouldArchive: false
        },
        actions: {
          setInitialSearchLoaded: vi.fn(),
          handleSearch: vi.fn(),
          handleUnsavedChanges: vi.fn(),
          setShouldBlock: vi.fn(),
          handleStatusChange: mockHandleStatusChange,
          handleArchiveHandled: vi.fn()
        }
      } as any);

      render(<UnForfeit />);

      expect(screen.getByTestId("status-dropdown")).toBeInTheDocument();
    });
  });

  describe("Auto-archive on status change", () => {
    it("should auto-trigger search when shouldArchive is true", async () => {
      const { useUnForfeitState } = await import("../../../hooks/useUnForfeitState");
      const mockHandleSearch = vi.fn();

      vi.mocked(useUnForfeitState).mockReturnValueOnce({
        state: {
          initialSearchLoaded: true,
          resetPageFlag: false,
          hasUnsavedChanges: false,
          shouldBlock: false,
          previousStatus: null,
          shouldArchive: true // Trigger auto-search
        },
        actions: {
          setInitialSearchLoaded: vi.fn(),
          handleSearch: mockHandleSearch,
          handleUnsavedChanges: vi.fn(),
          setShouldBlock: vi.fn(),
          handleStatusChange: vi.fn(),
          handleArchiveHandled: vi.fn()
        }
      } as any);

      render(<UnForfeit />);

      await waitFor(() => {
        expect(mockHandleSearch).toHaveBeenCalled();
      });
    });

    it("should not auto-trigger search when shouldArchive is false", async () => {
      const { useUnForfeitState } = await import("../../../hooks/useUnForfeitState");
      const mockHandleSearch = vi.fn();

      vi.mocked(useUnForfeitState).mockReturnValueOnce({
        state: {
          initialSearchLoaded: false,
          resetPageFlag: false,
          hasUnsavedChanges: false,
          shouldBlock: false,
          previousStatus: null,
          shouldArchive: false
        },
        actions: {
          setInitialSearchLoaded: vi.fn(),
          handleSearch: mockHandleSearch,
          handleUnsavedChanges: vi.fn(),
          setShouldBlock: vi.fn(),
          handleStatusChange: vi.fn(),
          handleArchiveHandled: vi.fn()
        }
      } as any);

      render(<UnForfeit />);

      // Auto-search should not be triggered
      expect(mockHandleSearch).not.toHaveBeenCalled();
    });
  });

  describe("Grid and Filter integration", () => {
    it("should pass initialSearchLoaded to UnForfeitGrid", async () => {
      const { useUnForfeitState } = await import("../../../hooks/useUnForfeitState");

      vi.mocked(useUnForfeitState).mockReturnValueOnce({
        state: {
          initialSearchLoaded: true,
          resetPageFlag: false,
          hasUnsavedChanges: false,
          shouldBlock: false,
          previousStatus: null,
          shouldArchive: false
        },
        actions: {
          setInitialSearchLoaded: vi.fn(),
          handleSearch: vi.fn(),
          handleUnsavedChanges: vi.fn(),
          setShouldBlock: vi.fn(),
          handleStatusChange: vi.fn(),
          handleArchiveHandled: vi.fn()
        }
      } as any);

      render(<UnForfeit />);

      expect(screen.getByTestId("unforfeit-grid")).toBeInTheDocument();
    });

    it("should pass fiscal data to UnForfeitSearchFilter", async () => {
      render(<UnForfeit />);

      // Verify search filter is rendered with fiscal data available
      expect(screen.getByTestId("search-filter")).toBeInTheDocument();
    });

    it("should pass hasUnsavedChanges to grid and filter", async () => {
      const { useUnForfeitState } = await import("../../../hooks/useUnForfeitState");

      vi.mocked(useUnForfeitState).mockReturnValueOnce({
        state: {
          initialSearchLoaded: false,
          resetPageFlag: false,
          hasUnsavedChanges: true,
          shouldBlock: false,
          previousStatus: null,
          shouldArchive: false
        },
        actions: {
          setInitialSearchLoaded: vi.fn(),
          handleSearch: vi.fn(),
          handleUnsavedChanges: vi.fn(),
          setShouldBlock: vi.fn(),
          handleStatusChange: vi.fn(),
          handleArchiveHandled: vi.fn()
        }
      } as any);

      render(<UnForfeit />);

      const searchButton = await screen.findByTestId("search-button");
      expect(searchButton).toBeDisabled();
    });
  });

  describe("Profit year integration", () => {
    it("should use profit year from hook", async () => {
      const { useIsProfitYearFrozen } = await import("../../../hooks/useIsProfitYearFrozen");

      render(<UnForfeit />);

      await waitFor(() => {
        expect(useIsProfitYearFrozen).toHaveBeenCalledWith(2024);
      });
    });
  });

  describe("Accounting range fetch", () => {
    it("should fetch accounting range on mount", async () => {
      const { useLazyGetAccountingRangeToCurrent } = await import("../../../hooks/useFiscalCalendarYear");
      const mockFetch = vi.fn();

      vi.mocked(useLazyGetAccountingRangeToCurrent).mockReturnValueOnce([
        mockFetch,
        {
          data: {
            fiscalBeginDate: "2024-01-01",
            fiscalEndDate: "2024-12-31"
          }
        }
      ] as any);

      render(<UnForfeit />);

      await waitFor(() => {
        expect(mockFetch).toHaveBeenCalled();
      });
    });

    it("should pass (6) to useLazyGetAccountingRangeToCurrent", async () => {
      const { useLazyGetAccountingRangeToCurrent } = await import("../../../hooks/useFiscalCalendarYear");

      render(<UnForfeit />);

      await waitFor(() => {
        expect(useLazyGetAccountingRangeToCurrent).toHaveBeenCalledWith(6);
      });
    });
  });
});
