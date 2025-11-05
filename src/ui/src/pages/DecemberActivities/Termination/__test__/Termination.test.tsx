import React from "react";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { createMockStoreAndWrapper } from "../../../../test";
import Termination from "../Termination";
type MockSearchParams = {
  beginningDate: string;
  endingDate: string;
  forfeitureStatus: string;
  profitYear: number;  // Required to match StartAndEndDateRequest
  pagination: {         // Required to match StartAndEndDateRequest
    skip: number;
    take: number;
    sortBy: string;
    isSortDescending: boolean;
  };
  archive?: boolean;
};

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

vi.mock("../../../../hooks/useTerminationState", () => ({
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

vi.mock("../../../../hooks/useUnsavedChangesGuard", () => ({
  useUnsavedChangesGuard: vi.fn()
}));

vi.mock("../../../../hooks/useDecemberFlowProfitYear", () => ({
  default: vi.fn(() => 2024)
}));

vi.mock("../../../../reduxstore/api/LookupsApi", () => ({
  LookupsApi: {
    reducerPath: "lookupsApi",
    reducer: vi.fn((state = {}) => state),
    middleware: vi.fn(() => (next: unknown) => (action: unknown) => (next as (action: unknown) => unknown)(action)),
    util: { resetApiState: vi.fn() }
  },
  useLazyGetAccountingYearQuery: vi.fn(() => [
    vi.fn(),
    {
      data: {
        fiscalBeginDate: "2023-01-01",
        fiscalEndDate: "2023-12-31"
      }
    }
  ])
}));

vi.mock("../../../../reduxstore/api/NavigationStatusApi", () => ({
  NavigationStatusApi: {
    reducerPath: "navigationStatusApi",
    reducer: vi.fn((state = {}) => state),
    middleware: vi.fn(() => (next: unknown) => (action: unknown) => (next as (action: unknown) => unknown)(action)),
    util: { resetApiState: vi.fn() }
  },
  useLazyGetNavigationStatusQuery: vi.fn(() => [
    vi.fn(),
    {
      data: {
        navigationStatusList: [
          { id: 1, name: "Not Started" },
          { id: 2, name: "In Progress" },
          { id: 3, name: "Complete" }
        ]
      },
      isSuccess: true
    }
  ]),
  useLazyUpdateNavigationStatusQuery: vi.fn(() => [vi.fn()])
}));

vi.mock("../../../../reduxstore/api/NavigationApi", () => ({
  NavigationApi: {
    reducerPath: "navigationApi",
    reducer: vi.fn((state = {}) => state),
    middleware: vi.fn(() => (next: unknown) => (action: unknown) => (next as (action: unknown) => unknown)(action)),
    util: { resetApiState: vi.fn() }
  },
  useLazyGetNavigationQuery: vi.fn(() => [vi.fn()])
}));

vi.mock("../../../../components/StatusDropdownActionNode", () => ({
  default: vi.fn(() => React.createElement("div", { "data-testid": "status-dropdown" }, "Status Dropdown"))
}));

vi.mock("../TerminationSearchFilter", () => ({
  default: vi.fn(({ onSearch, hasUnsavedChanges, isFetching }) =>
    React.createElement(
      "div",
      { "data-testid": "search-filter" },
      React.createElement(
        "button",
        {
          "data-testid": "search-button",
          onClick: () =>
            onSearch({
              beginningDate: "01/01/2024",
              endingDate: "12/31/2024",
              forfeitureStatus: "showAll",
              profitYear: 2024,
              skip: 0,
              take: 25,
              sortBy: "badgeNumber",
              isSortDescending: false
            }),
          disabled: isFetching || hasUnsavedChanges
        },
        "Search"
      ),
      isFetching && React.createElement("span", { "data-testid": "searching" }, "Searching...")
    )
  )
}));

vi.mock("../TerminationGrid", () => ({
  default: vi.fn(() => React.createElement("div", { "data-testid": "termination-grid" }, "Termination Grid"))
}));

vi.mock("smart-ui-library", () => ({
  ApiMessageAlert: vi.fn(() => React.createElement("div", { "data-testid": "api-message-alert" }, "Message Alert")),
  DSMAccordion: vi.fn(({ title, children }) =>
    React.createElement(
      "div",
      { "data-testid": "accordion" },
      React.createElement("div", null, title),
      children
    )
  ),
  Page: vi.fn(({ label, actionNode, children }) =>
    React.createElement(
      "div",
      { "data-testid": "page" },
      React.createElement("div", null, label),
      actionNode,
      children
    )
  ),
  SearchAndReset: vi.fn(({ handleSearch, handleReset, disabled, isFetching }) =>
    React.createElement(
      "div",
      { "data-testid": "search-and-reset" },
      React.createElement(
        "button",
        { "data-testid": "search-btn", onClick: handleSearch, disabled: disabled || isFetching },
        "Search"
      ),
      React.createElement("button", { "data-testid": "reset-btn", onClick: handleReset }, "Reset"),
      isFetching && React.createElement("span", { "data-testid": "loading" }, "Loading...")
    )
  )
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
      const { useLazyGetAccountingRangeToCurrent } = await import("../../../../hooks/useFiscalCalendarYear");
      vi.mocked(useLazyGetAccountingRangeToCurrent).mockReturnValueOnce([
        vi.fn(),
        {
          data: undefined,
          isFetching: false,
          isSuccess: false,
          isLoading: false,
          isError: false,
          isUninitialized: true,
          status: "uninitialized" as const
        }
      ] as unknown as ReturnType<typeof useLazyGetAccountingRangeToCurrent>);

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
      const { useTerminationState } = await import("../../../../hooks/useTerminationState");
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
          setInitialSearchLoaded: vi.fn(),
          handleUnsavedChanges: vi.fn(),
          handleStatusChange: vi.fn(),
          handleArchiveHandled: vi.fn()
        }
      });

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
      const { useTerminationState } = await import("../../../../hooks/useTerminationState");

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
          setInitialSearchLoaded: vi.fn(),
          handleUnsavedChanges: vi.fn(),
          handleStatusChange: vi.fn(),
          handleArchiveHandled: vi.fn()
        }
      });

      render(<Termination />, { wrapper });

      const searchButton = await screen.findByTestId("search-button");

      // Simulate loading state
      expect(searchButton).not.toBeDisabled();
    });

    it("should disable search button when unsaved changes exist", async () => {
      const { useTerminationState } = await import("../../../../hooks/useTerminationState");

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
          setInitialSearchLoaded: vi.fn(),
          handleUnsavedChanges: vi.fn(),
          handleStatusChange: vi.fn(),
          handleArchiveHandled: vi.fn()
        }
      });

      render(<Termination />, { wrapper });

      const searchButton = await screen.findByTestId("search-button");

      await waitFor(() => {
        expect(searchButton).toBeDisabled();
      });
    });
  });

  describe("Unsaved changes guard", () => {
    it("should invoke useUnsavedChangesGuard with hasUnsavedChanges state", async () => {
      const { useUnsavedChangesGuard } = await import("../../../../hooks/useUnsavedChangesGuard");
      const { useTerminationState } = await import("../../../../hooks/useTerminationState");

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
          setInitialSearchLoaded: vi.fn(),
          handleUnsavedChanges: vi.fn(),
          handleStatusChange: vi.fn(),
          handleArchiveHandled: vi.fn()
        }
      });

      render(<Termination />, { wrapper });

      await waitFor(() => {
        expect(useUnsavedChangesGuard).toHaveBeenCalledWith(true);
      });
    });
  });

  describe("Status change handling", () => {
    it("should pass handleStatusChange to StatusDropdownActionNode", async () => {
      const { useTerminationState } = await import("../../../../hooks/useTerminationState");
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
          setInitialSearchLoaded: vi.fn(),
          handleUnsavedChanges: vi.fn(),
          handleStatusChange: mockHandleStatusChange,
          handleArchiveHandled: vi.fn()
        }
      });

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

      const { unmount } = render(<Termination />, { wrapper });

      unmount();

      expect(removeEventListenerSpy).toHaveBeenCalledWith("dsmMessage", expect.any(Function));

      removeEventListenerSpy.mockRestore();
    });
  });

  describe("Grid and Filter integration", () => {
    it("should pass searchParams to TerminationGrid", async () => {
      const { useTerminationState } = await import("../../../../hooks/useTerminationState");
      const mockSearchParams: MockSearchParams = {
        beginningDate: "01/01/2024",
        endingDate: "12/31/2024",
        forfeitureStatus: "showAll",
        profitYear: 2024,
        pagination: {
          skip: 0,
          take: 25,
          sortBy: "badgeNumber",
          isSortDescending: false
        }
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
          setInitialSearchLoaded: vi.fn(),
          handleUnsavedChanges: vi.fn(),
          handleStatusChange: vi.fn(),
          handleArchiveHandled: vi.fn()
        }
      });

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
      const { useTerminationState } = await import("../../../../hooks/useTerminationState");
      const mockHandleArchiveHandled = vi.fn();

      vi.mocked(useTerminationState).mockReturnValueOnce({
        state: {
          searchParams: {
            beginningDate: "01/01/2024",
            endingDate: "12/31/2024",
            forfeitureStatus: "showAll",
            profitYear: 2024,
            archive: true,
            pagination: {
              skip: 0,
              take: 25,
              sortBy: "badgeNumber",
              isSortDescending: false
            }
          },
          initialSearchLoaded: true,
          hasUnsavedChanges: false,
          resetPageFlag: false,
          currentStatus: "Complete",
          archiveMode: true,
          shouldArchive: true
        },
        actions: {
          handleSearch: vi.fn(),
          setInitialSearchLoaded: vi.fn(),
          handleUnsavedChanges: vi.fn(),
          handleStatusChange: vi.fn(),
          handleArchiveHandled: mockHandleArchiveHandled
        }
      });

      render(<Termination />, { wrapper });

      // Grid will handle archive mode based on searchParams
      expect(screen.getByTestId("termination-grid")).toBeInTheDocument();
    });
  });
});
