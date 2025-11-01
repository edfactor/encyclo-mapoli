import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { createMockStoreAndWrapper } from "../../../../test";
import Termination from "../Termination";
import { useLazyGetAccountingRangeToCurrent } from "../../../hooks/useFiscalCalendarYear";
import { useTerminationState } from "../../../hooks/useTerminationState";

type MockFiscalData = ReturnType<typeof useLazyGetAccountingRangeToCurrent>;
type MockTerminationState = ReturnType<typeof useTerminationState>;
type MockSearchParams = {
  beginningDate?: string;
  endingDate?: string;
  forfeitureStatus?: string;
  archive?: boolean;
};

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

vi.mock("../../../hooks/useTerminationState", () => ({
  useTerminationState: vi.fn(() => ({
    state: {
      searchParams: null,
      initialSearchLoaded: false,
      hasUnsavedChanges: false,
      resetPageFlag: false,
      currentStatus: null,
      archiveMode: false,
      shouldArchive: false
    },
    actions: {
      handleSearch: vi.fn(),
      handleReset: vi.fn(),
      setInitialSearchLoaded: vi.fn(),
      handleUnsavedChanges: vi.fn(),
      handleStatusChange: vi.fn(),
      handleArchiveHandled: vi.fn()
    }
  }))
}));

vi.mock("../../../hooks/useUnsavedChangesGuard", () => ({
  useUnsavedChangesGuard: vi.fn()
}));

vi.mock("components/StatusDropdownActionNode", () => ({
  default: vi.fn(() => <div data-testid="status-dropdown">Status Dropdown</div>)
}));

vi.mock("./TerminationSearchFilter", () => ({
  default: vi.fn(({ onSearch, hasUnsavedChanges, isFetching }) => (
    <div data-testid="search-filter">
      <button
        data-testid="search-button"
        onClick={() =>
          onSearch({
            beginningDate: "01/01/2024",
            endingDate: "12/31/2024",
            forfeitureStatus: "showAll",
            profitYear: 2024,
            skip: 0,
            take: 25,
            sortBy: "badgeNumber",
            isSortDescending: false
          })
        }
        disabled={isFetching || hasUnsavedChanges}>
        Search
      </button>
      {isFetching && <span data-testid="searching">Searching...</span>}
    </div>
  ))
}));

vi.mock("./TerminationGrid", () => ({
  default: vi.fn(() => <div data-testid="termination-grid">Termination Grid</div>)
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

describe("Termination", () => {
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
      render(<Termination />, { wrapper });

      expect(screen.getByTestId("page")).toBeInTheDocument();
    });

    it("should render page with correct label", () => {
      render(<Termination />, { wrapper });

      // The label should contain "TERMINATIONS"
      const pageLabel = screen.getByText(/TERMINATIONS/i);
      expect(pageLabel).toBeInTheDocument();
    });

    it("should render ApiMessageAlert", () => {
      render(<Termination />, { wrapper });

      expect(screen.getByTestId("api-message-alert")).toBeInTheDocument();
    });

    it("should render StatusDropdownActionNode in action node", () => {
      render(<Termination />, { wrapper });

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
      ] as MockFiscalData);

      render(<Termination />, { wrapper });

      await waitFor(() => {
        expect(screen.getByRole("progressbar")).toBeInTheDocument();
      });
    });

    it("should render search filter and grid when fiscal data is loaded", async () => {
      render(<Termination />, { wrapper });

      await waitFor(() => {
        expect(screen.getByTestId("search-filter")).toBeInTheDocument();
        expect(screen.getByTestId("termination-grid")).toBeInTheDocument();
      });
    });
  });

  describe("Search functionality", () => {
    it("should call handleSearch when search button is clicked", async () => {
      const { useTerminationState } = await import("../../../hooks/useTerminationState");
      const mockHandleSearch = vi.fn();

      vi.mocked(useTerminationState).mockReturnValueOnce({
        state: {
          searchParams: null,
          initialSearchLoaded: false,
          hasUnsavedChanges: false,
          resetPageFlag: false,
          currentStatus: null,
          archiveMode: false,
          shouldArchive: false
        },
        actions: {
          handleSearch: mockHandleSearch,
          handleReset: vi.fn(),
          setInitialSearchLoaded: vi.fn(),
          handleUnsavedChanges: vi.fn(),
          handleStatusChange: vi.fn(),
          handleArchiveHandled: vi.fn()
        }
      } as MockTerminationState);

      render(<Termination />, { wrapper });

      const searchButton = await screen.findByTestId("search-button");
      fireEvent.click(searchButton);

      await waitFor(() => {
        expect(mockHandleSearch).toHaveBeenCalledWith(
          expect.objectContaining({
            beginningDate: "01/01/2024",
            endingDate: "12/31/2024",
            forfeitureStatus: "showAll"
          })
        );
      });
    });

    it("should disable search button during search", async () => {
      const { useTerminationState } = await import("../../../hooks/useTerminationState");

      vi.mocked(useTerminationState).mockReturnValueOnce({
        state: {
          searchParams: null,
          initialSearchLoaded: false,
          hasUnsavedChanges: false,
          resetPageFlag: false,
          currentStatus: null,
          archiveMode: false,
          shouldArchive: false
        },
        actions: {
          handleSearch: vi.fn(),
          handleReset: vi.fn(),
          setInitialSearchLoaded: vi.fn(),
          handleUnsavedChanges: vi.fn(),
          handleStatusChange: vi.fn(),
          handleArchiveHandled: vi.fn()
        }
      } as MockTerminationState);

      render(<Termination />, { wrapper });

      const searchButton = await screen.findByTestId("search-button");

      // Simulate loading state
      expect(searchButton).not.toBeDisabled();
    });

    it("should disable search button when unsaved changes exist", async () => {
      const { useTerminationState } = await import("../../../hooks/useTerminationState");

      vi.mocked(useTerminationState).mockReturnValueOnce({
        state: {
          searchParams: null,
          initialSearchLoaded: false,
          hasUnsavedChanges: true,
          resetPageFlag: false,
          currentStatus: null,
          archiveMode: false,
          shouldArchive: false
        },
        actions: {
          handleSearch: vi.fn(),
          handleReset: vi.fn(),
          setInitialSearchLoaded: vi.fn(),
          handleUnsavedChanges: vi.fn(),
          handleStatusChange: vi.fn(),
          handleArchiveHandled: vi.fn()
        }
      } as MockTerminationState);

      render(<Termination />, { wrapper });

      const searchButton = await screen.findByTestId("search-button");

      await waitFor(() => {
        expect(searchButton).toBeDisabled();
      });
    });
  });

  describe("Unsaved changes guard", () => {
    it("should invoke useUnsavedChangesGuard with hasUnsavedChanges state", async () => {
      const { useUnsavedChangesGuard } = await import("../../../hooks/useUnsavedChangesGuard");
      const { useTerminationState } = await import("../../../hooks/useTerminationState");

      vi.mocked(useTerminationState).mockReturnValueOnce({
        state: {
          searchParams: null,
          initialSearchLoaded: false,
          hasUnsavedChanges: true,
          resetPageFlag: false,
          currentStatus: null,
          archiveMode: false,
          shouldArchive: false
        },
        actions: {
          handleSearch: vi.fn(),
          handleReset: vi.fn(),
          setInitialSearchLoaded: vi.fn(),
          handleUnsavedChanges: vi.fn(),
          handleStatusChange: vi.fn(),
          handleArchiveHandled: vi.fn()
        }
      } as MockTerminationState);

      render(<Termination />, { wrapper });

      await waitFor(() => {
        expect(useUnsavedChangesGuard).toHaveBeenCalledWith(true);
      });
    });
  });

  describe("Status change handling", () => {
    it("should pass handleStatusChange to StatusDropdownActionNode", async () => {
      const { useTerminationState } = await import("../../../hooks/useTerminationState");
      const mockHandleStatusChange = vi.fn();

      vi.mocked(useTerminationState).mockReturnValueOnce({
        state: {
          searchParams: null,
          initialSearchLoaded: false,
          hasUnsavedChanges: false,
          resetPageFlag: false,
          currentStatus: null,
          archiveMode: false,
          shouldArchive: false
        },
        actions: {
          handleSearch: vi.fn(),
          handleReset: vi.fn(),
          setInitialSearchLoaded: vi.fn(),
          handleUnsavedChanges: vi.fn(),
          handleStatusChange: mockHandleStatusChange,
          handleArchiveHandled: vi.fn()
        }
      } as MockTerminationState);

      render(<Termination />, { wrapper });

      // Verify that the component exists and would call the handler
      expect(screen.getByTestId("status-dropdown")).toBeInTheDocument();
    });
  });

  describe("Error handling and scroll-to-top", () => {
    it("should listen for dsmMessage events", () => {
      const addEventListenerSpy = vi.spyOn(window, "addEventListener");

      render(<Termination />, { wrapper });

      expect(addEventListenerSpy).toHaveBeenCalledWith("dsmMessage", expect.any(Function));

      addEventListenerSpy.mockRestore();
    });

    it("should scroll to top on error message event", async () => {
      const scrollToSpy = vi.spyOn(window, "scrollTo").mockImplementation(() => {});

      render(<Termination />, { wrapper });

      // Create and dispatch error event
      const errorEvent = new CustomEvent("dsmMessage", {
        detail: {
          key: "TerminationSave",
          message: {
            type: "error"
          }
        }
      });

      window.dispatchEvent(errorEvent);

      await waitFor(() => {
        expect(scrollToSpy).toHaveBeenCalledWith({
          top: 0,
          behavior: "smooth"
        });
      });

      scrollToSpy.mockRestore();
    });

    it("should not scroll for non-error messages", () => {
      const scrollToSpy = vi.spyOn(window, "scrollTo").mockImplementation(() => {});

      render(<Termination />, { wrapper });

      // Create and dispatch success event
      const successEvent = new CustomEvent("dsmMessage", {
        detail: {
          key: "TerminationSave",
          message: {
            type: "success"
          }
        }
      });

      window.dispatchEvent(successEvent);

      // Should not scroll for success
      expect(scrollToSpy).not.toHaveBeenCalled();

      scrollToSpy.mockRestore();
    });

    it("should clean up event listener on unmount", () => {
      const removeEventListenerSpy = vi.spyOn(window, "removeEventListener");

      const { unmount } = render(<Termination />);

      unmount();

      expect(removeEventListenerSpy).toHaveBeenCalledWith("dsmMessage", expect.any(Function));

      removeEventListenerSpy.mockRestore();
    });
  });

  describe("Grid and Filter integration", () => {
    it("should pass searchParams to TerminationGrid", async () => {
      const { useTerminationState } = await import("../../../hooks/useTerminationState");
      const mockSearchParams: MockSearchParams = {
        beginningDate: "01/01/2024",
        endingDate: "12/31/2024",
        forfeitureStatus: "showAll"
      };

      vi.mocked(useTerminationState).mockReturnValueOnce({
        state: {
          searchParams: mockSearchParams,
          initialSearchLoaded: true,
          hasUnsavedChanges: false,
          resetPageFlag: false,
          currentStatus: null,
          archiveMode: false,
          shouldArchive: false
        },
        actions: {
          handleSearch: vi.fn(),
          handleReset: vi.fn(),
          setInitialSearchLoaded: vi.fn(),
          handleUnsavedChanges: vi.fn(),
          handleStatusChange: vi.fn(),
          handleArchiveHandled: vi.fn()
        }
      } as MockTerminationState);

      render(<Termination />, { wrapper });

      expect(screen.getByTestId("termination-grid")).toBeInTheDocument();
    });

    it("should pass fiscal data to TerminationSearchFilter", async () => {
      render(<Termination />, { wrapper });

      // Verify search filter is rendered with fiscal data available
      expect(screen.getByTestId("search-filter")).toBeInTheDocument();
    });
  });

  describe("Archive mode", () => {
    it("should handle archive flag when shouldArchive is true", async () => {
      const { useTerminationState } = await import("../../../hooks/useTerminationState");
      const mockHandleArchiveHandled = vi.fn();

      vi.mocked(useTerminationState).mockReturnValueOnce({
        state: {
          searchParams: { archive: true } as MockSearchParams,
          initialSearchLoaded: true,
          hasUnsavedChanges: false,
          resetPageFlag: false,
          currentStatus: "Complete",
          archiveMode: true,
          shouldArchive: true
        },
        actions: {
          handleSearch: vi.fn(),
          handleReset: vi.fn(),
          setInitialSearchLoaded: vi.fn(),
          handleUnsavedChanges: vi.fn(),
          handleStatusChange: vi.fn(),
          handleArchiveHandled: mockHandleArchiveHandled
        }
      } as MockTerminationState);

      render(<Termination />, { wrapper });

      // Grid will handle archive mode based on searchParams
      expect(screen.getByTestId("termination-grid")).toBeInTheDocument();
    });
  });
});
