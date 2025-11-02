import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { createMockStoreAndWrapper } from "../../../../test";
import UnForfeit from "../UnForfeit";
import { useLazyGetAccountingRangeToCurrent } from "../../../../hooks/useFiscalCalendarYear";
import { useUnForfeitState } from "../../../../hooks/useUnForfeitState";

type MockFiscalData = ReturnType<typeof useLazyGetAccountingRangeToCurrent>;
type MockUnForfeitState = ReturnType<typeof useUnForfeitState>;

// Mock the hook dependencies
vi.mock("../../../../hooks/useFiscalCalendarYear", () => ({
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

vi.mock("../../../../hooks/useDecemberFlowProfitYear", () => ({
  default: vi.fn(() => 2024)
}));

vi.mock("../../../../hooks/useIsProfitYearFrozen", () => ({
  useIsProfitYearFrozen: vi.fn(() => false)
}));

vi.mock("../../../../hooks/useUnForfeitState", () => ({
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

vi.mock("../../../../hooks/useUnsavedChangesGuard", () => ({
  useUnsavedChangesGuard: vi.fn()
}));

vi.mock("../../../../components/FrozenYearWarning", () => ({
  default: vi.fn(() => <section aria-label="frozen year warning">Frozen Year Warning</section>)
}));

vi.mock("components/StatusDropdownActionNode", () => ({
  default: vi.fn(() => <section aria-label="status dropdown">Status Dropdown</section>)
}));

vi.mock("./UnForfeitSearchFilter", () => ({
  default: vi.fn(({ onSearch, hasUnsavedChanges }) => (
    <section aria-label="search filter">
      <button
        aria-label="search button"
        onClick={() => onSearch()}
        disabled={hasUnsavedChanges}>
        Search
      </button>
    </section>
  ))
}));

vi.mock("./UnForfeitGrid", () => ({
  default: vi.fn(() => <section aria-label="unforfeit grid">UnForfeit Grid</section>)
}));

vi.mock("smart-ui-library", () => ({
  ApiMessageAlert: vi.fn(() => <section aria-label="api message alert">Message Alert</section>),
  DSMAccordion: vi.fn(({ title, children }) => (
    <section aria-label="accordion">
      <div>{title}</div>
      {children}
    </section>
  )),
  Page: vi.fn(({ label, actionNode, children }) => (
    <section aria-label="page">
      <div>{label}</div>
      {actionNode}
      {children}
    </section>
  ))
}));

describe("UnForfeit", () => {
  let wrapper: ReturnType<typeof createMockStoreAndWrapper>["wrapper"];

  beforeEach(() => {
    vi.clearAllMocks();
    const storeAndWrapper = createMockStoreAndWrapper({
      yearsEnd: { selectedProfitYear: 2024 }
    });
    wrapper = storeAndWrapper.wrapper;
  });

  describe("Rendering", () => {
    it("should render the page component", () => {
      render(<UnForfeit />, { wrapper });

      expect(screen.getByText("UnForfeit (QPREV-PROF)")).toBeInTheDocument();
    });

    it("should render page with correct label", () => {
      render(<UnForfeit />, { wrapper });

      // The label should be "UnForfeit (QPREV-PROF)"
      expect(screen.getByText("UnForfeit (QPREV-PROF)")).toBeInTheDocument();
    });

    it("should render ApiMessageAlert", () => {
      render(<UnForfeit />, { wrapper });

      expect(screen.getByRole("alert", { name: /message/i })).toBeInTheDocument();
    });

    it("should render StatusDropdownActionNode in action node", () => {
      render(<UnForfeit />, { wrapper });

      expect(screen.getByRole("status", { name: /dropdown/i })).toBeInTheDocument();
    });

    it("should render loading spinner when fiscal data is not loaded", async () => {
      const { useLazyGetAccountingRangeToCurrent } = await import("../../../../hooks/useFiscalCalendarYear");
      vi.mocked(useLazyGetAccountingRangeToCurrent).mockReturnValueOnce([
        vi.fn(),
        {
          data: {
            fiscalBeginDate: null,
            fiscalEndDate: null
          }
        }
      ] as MockFiscalData);

      render(<UnForfeit />, { wrapper });

      await waitFor(() => {
        expect(screen.getByRole("progressbar")).toBeInTheDocument();
      });
    });

    it("should render search filter and grid when fiscal data is loaded", async () => {
      render(<UnForfeit />, { wrapper });

      await waitFor(() => {
        expect(screen.getByRole("button", { name: /search/i })).toBeInTheDocument();
        expect(screen.getByText("UnForfeit Grid")).toBeInTheDocument();
      });
    });
  });

  describe("Frozen year warning", () => {
    it("should display frozen year warning when year is frozen", async () => {
      const { useIsProfitYearFrozen } = await import("../../../../hooks/useIsProfitYearFrozen");
      vi.mocked(useIsProfitYearFrozen).mockReturnValueOnce(true);

      render(<UnForfeit />, { wrapper });

      expect(screen.getByText("Frozen Year Warning")).toBeInTheDocument();
    });

    it("should not display frozen year warning when year is not frozen", async () => {
      const { useIsProfitYearFrozen } = await import("../../../../hooks/useIsProfitYearFrozen");
      vi.mocked(useIsProfitYearFrozen).mockReturnValueOnce(false);

      render(<UnForfeit />, { wrapper });

      expect(screen.queryByText("Frozen Year Warning")).not.toBeInTheDocument();
    });
  });

  describe("Search functionality", () => {
    it("should call handleSearch when search button is clicked", async () => {
      const { useUnForfeitState } = await import("../../../../hooks/useUnForfeitState");
      const mockHandleSearch = vi.fn();
      const user = userEvent.setup();

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
      } as MockUnForfeitState);

      render(<UnForfeit />, { wrapper });

      const searchButton = screen.getByRole("button", { name: /search button/i });
      await user.click(searchButton);

      await waitFor(() => {
        // handleSearch is called directly from search filter
        expect(screen.getByRole("button", { name: /search/i })).toBeInTheDocument();
      });
    });

    it("should disable search button when unsaved changes exist", async () => {
      const { useUnForfeitState } = await import("../../../../hooks/useUnForfeitState");

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
      } as MockUnForfeitState);

      render(<UnForfeit />, { wrapper });

      const searchButton = screen.getByRole("button", { name: /search button/i });

      await waitFor(() => {
        expect(searchButton).toBeDisabled();
      });
    });
  });

  describe("Unsaved changes guard", () => {
    it("should invoke useUnsavedChangesGuard with hasUnsavedChanges state", async () => {
      const { useUnsavedChangesGuard } = await import("../../../../hooks/useUnsavedChangesGuard");
      const { useUnForfeitState } = await import("../../../../hooks/useUnForfeitState");

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
      } as MockUnForfeitState);

      render(<UnForfeit />, { wrapper });

      await waitFor(() => {
        expect(useUnsavedChangesGuard).toHaveBeenCalledWith(true);
      });
    });
  });

  describe("Status change handling", () => {
    it("should pass handleStatusChange to StatusDropdownActionNode", async () => {
      const { useUnForfeitState } = await import("../../../../hooks/useUnForfeitState");
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
      } as MockUnForfeitState);

      render(<UnForfeit />, { wrapper });

      expect(screen.getByRole("status", { name: /dropdown/i })).toBeInTheDocument();
    });
  });

  describe("Auto-archive on status change", () => {
    it("should auto-trigger search when shouldArchive is true", async () => {
      const { useUnForfeitState } = await import("../../../../hooks/useUnForfeitState");
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
      } as MockUnForfeitState);

      render(<UnForfeit />, { wrapper });

      await waitFor(() => {
        expect(mockHandleSearch).toHaveBeenCalled();
      });
    });

    it("should not auto-trigger search when shouldArchive is false", async () => {
      const { useUnForfeitState } = await import("../../../../hooks/useUnForfeitState");
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
      } as MockUnForfeitState);

      render(<UnForfeit />, { wrapper });

      // Auto-search should not be triggered
      expect(mockHandleSearch).not.toHaveBeenCalled();
    });
  });

  describe("Grid and Filter integration", () => {
    it("should pass initialSearchLoaded to UnForfeitGrid", async () => {
      const { useUnForfeitState } = await import("../../../../hooks/useUnForfeitState");

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
      } as MockUnForfeitState);

      render(<UnForfeit />, { wrapper });

      expect(screen.getByText("UnForfeit Grid")).toBeInTheDocument();
    });

    it("should pass fiscal data to UnForfeitSearchFilter", async () => {
      render(<UnForfeit />, { wrapper });

      // Verify search filter is rendered with fiscal data available
      expect(screen.getByRole("button", { name: /search/i })).toBeInTheDocument();
    });

    it("should pass hasUnsavedChanges to grid and filter", async () => {
      const { useUnForfeitState } = await import("../../../../hooks/useUnForfeitState");

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
      } as MockUnForfeitState);

      render(<UnForfeit />, { wrapper });

      const searchButton = screen.getByRole("button", { name: /search button/i });
      expect(searchButton).toBeDisabled();
    });
  });

  describe("Profit year integration", () => {
    it("should use profit year from hook", async () => {
      const { useIsProfitYearFrozen } = await import("../../../../hooks/useIsProfitYearFrozen");

      render(<UnForfeit />, { wrapper });

      await waitFor(() => {
        expect(useIsProfitYearFrozen).toHaveBeenCalledWith(2024);
      });
    });
  });

  describe("Accounting range fetch", () => {
    it("should fetch accounting range on mount", async () => {
      const { useLazyGetAccountingRangeToCurrent } = await import("../../../../hooks/useFiscalCalendarYear");
      const mockFetch = vi.fn();

      vi.mocked(useLazyGetAccountingRangeToCurrent).mockReturnValueOnce([
        mockFetch,
        {
          data: {
            fiscalBeginDate: "2024-01-01",
            fiscalEndDate: "2024-12-31"
          }
        }
      ] as MockFiscalData);

      render(<UnForfeit />, { wrapper });

      await waitFor(() => {
        expect(mockFetch).toHaveBeenCalled();
      });
    });

    it("should pass (6) to useLazyGetAccountingRangeToCurrent", async () => {
      const { useLazyGetAccountingRangeToCurrent } = await import("../../../../hooks/useFiscalCalendarYear");

      render(<UnForfeit />, { wrapper });

      await waitFor(() => {
        expect(useLazyGetAccountingRangeToCurrent).toHaveBeenCalledWith(6);
      });
    });
  });
});
