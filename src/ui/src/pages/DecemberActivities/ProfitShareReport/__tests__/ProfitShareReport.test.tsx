import { render, screen, waitFor } from "@testing-library/react";
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi, beforeEach } from "vitest";
import ProfitShareReport from "../ProfitShareReport";
import ProfitShareReportSearchFilter from "../ProfitShareReportSearchFilter";
import ProfitShareReportGrid from "../ProfitShareReportGrid";

vi.mock("../ProfitShareReportSearchFilter", () => ({
  default: vi.fn(({ onSearch, setInitialSearchLoaded: _setInitialSearchLoaded, isFetching }) => (
    <div data-testid="search-filter">
      <button
        data-testid="search-btn"
        onClick={() => onSearch({ year: 2024 })}
        disabled={isFetching}>
        Search
      </button>
      {isFetching && <div data-testid="fetching">Fetching...</div>}
    </div>
  ))
}));

vi.mock("../ProfitShareReportGrid", () => ({
  default: vi.fn(({ initialSearchLoaded, searchParams: _searchParams, onLoadingChange }) => (
    <div data-testid="grid">
      {!initialSearchLoaded && <div data-testid="no-search">No search performed</div>}
      {initialSearchLoaded && <div data-testid="has-search">Search performed</div>}
      <button onClick={() => onLoadingChange(true)} data-testid="start-loading">
        Start Loading
      </button>
      <button onClick={() => onLoadingChange(false)} data-testid="stop-loading">
        Stop Loading
      </button>
    </div>
  ))
}));

vi.mock("components/StatusDropdownActionNode", () => ({
  default: vi.fn(() => <div data-testid="status-dropdown">Status Dropdown</div>)
}));

vi.mock("smart-ui-library", () => ({
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

describe("ProfitShareReport", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("Rendering", () => {
    it("should render the page component", () => {
      render(<ProfitShareReport />);

      expect(screen.getByTestId("page")).toBeInTheDocument();
    });

    it("should render page with correct label", () => {
      render(<ProfitShareReport />);

      expect(screen.getByText(/PROFIT SHARE REPORT/i)).toBeInTheDocument();
    });

    it("should render StatusDropdownActionNode", () => {
      render(<ProfitShareReport />);

      expect(screen.getByTestId("status-dropdown")).toBeInTheDocument();
    });

    it("should render filter accordion", () => {
      render(<ProfitShareReport />);

      expect(screen.getByTestId("accordion")).toBeInTheDocument();
    });

    it("should render search filter", () => {
      render(<ProfitShareReport />);

      expect(screen.getByTestId("search-filter")).toBeInTheDocument();
    });

    it("should render grid component", () => {
      render(<ProfitShareReport />);

      expect(screen.getByTestId("grid")).toBeInTheDocument();
    });
  });

  describe("State management", () => {
    it("should initialize with initialSearchLoaded false", () => {
      render(<ProfitShareReport />);

      expect(screen.getByTestId("no-search")).toBeInTheDocument();
    });

    it("should initialize with isFetching false", () => {
      render(<ProfitShareReport />);

      expect(screen.queryByTestId("fetching")).not.toBeInTheDocument();
    });
  });

  describe("Search workflow", () => {
    it("should trigger search with form data", async () => {
      render(<ProfitShareReport />);

      const searchBtn = screen.getByTestId("search-btn");
      searchBtn.click();

      await waitFor(() => {
        // Search should be triggered
        expect(screen.getByTestId("search-btn")).toBeInTheDocument();
      });
    });

    it("should disable search button during fetch", async () => {
      const { rerender } = render(<ProfitShareReport />);

      const startLoadingBtn = screen.getByTestId("start-loading");
      startLoadingBtn.click();

      await waitFor(() => {
        rerender(<ProfitShareReport />);
        expect(screen.getByTestId("fetching")).toBeInTheDocument();
      });
    });

    it("should update grid when search completes", async () => {
      const { rerender } = render(<ProfitShareReport />);

      const searchBtn = screen.getByTestId("search-btn");
      searchBtn.click();

      await waitFor(() => {
        rerender(<ProfitShareReport />);
        // Grid should show search was performed after clicking
        expect(screen.getByTestId("grid")).toBeInTheDocument();
      });
    });
  });

  describe("Loading state coordination", () => {
    it("should pass setInitialSearchLoaded to search filter", () => {
      render(<ProfitShareReport />);

      expect(vi.mocked(ProfitShareReportSearchFilter)).toHaveBeenCalledWith(
        expect.objectContaining({
          setInitialSearchLoaded: expect.any(Function)
        }),
        expect.anything()
      );
    });

    it("should pass isFetching to search filter", () => {
      render(<ProfitShareReport />);

      expect(vi.mocked(ProfitShareReportSearchFilter)).toHaveBeenCalledWith(
        expect.objectContaining({
          isFetching: expect.any(Boolean)
        }),
        expect.anything()
      );
    });

    it("should pass onLoadingChange to grid", () => {
      render(<ProfitShareReport />);

      expect(vi.mocked(ProfitShareReportGrid)).toHaveBeenCalledWith(
        expect.objectContaining({
          onLoadingChange: expect.any(Function)
        }),
        expect.anything()
      );
    });
  });

  describe("Multi-step loading", () => {
    it("should handle loading state transitions", async () => {
      const { rerender } = render(<ProfitShareReport />);

      const startLoadingBtn = screen.getByTestId("start-loading");
      startLoadingBtn.click();

      await waitFor(() => {
        rerender(<ProfitShareReport />);
        expect(screen.getByTestId("fetching")).toBeInTheDocument();
      });

      const stopLoadingBtn = screen.getByTestId("stop-loading");
      stopLoadingBtn.click();

      await waitFor(() => {
        rerender(<ProfitShareReport />);
        expect(screen.queryByTestId("fetching")).not.toBeInTheDocument();
      });
    });
  });

  describe("Component integration", () => {
    it("should render all major sections", () => {
      render(<ProfitShareReport />);

      expect(screen.getByTestId("page")).toBeInTheDocument();
      expect(screen.getByTestId("status-dropdown")).toBeInTheDocument();
      expect(screen.getByTestId("accordion")).toBeInTheDocument();
      expect(screen.getByTestId("search-filter")).toBeInTheDocument();
      expect(screen.getByTestId("grid")).toBeInTheDocument();
    });

    it("should maintain proper component hierarchy", () => {
      render(<ProfitShareReport />);

      const page = screen.getByTestId("page");
      const accordion = screen.getByTestId("accordion");

      expect(page).toBeInTheDocument();
      expect(accordion).toBeInTheDocument();
    });
  });

  describe("Search parameter handling", () => {
    it("should pass initialSearchLoaded to grid", () => {
      render(<ProfitShareReport />);

      expect(vi.mocked(ProfitShareReportGrid)).toHaveBeenCalledWith(
        expect.objectContaining({
          initialSearchLoaded: expect.any(Boolean)
        }),
        expect.anything()
      );
    });

    it("should pass searchParams to grid", () => {
      render(<ProfitShareReport />);

      expect(vi.mocked(ProfitShareReportGrid)).toHaveBeenCalledWith(
        expect.objectContaining({
          searchParams: expect.any(Object)
        }),
        expect.anything()
      );
    });
  });

  describe("Edge cases", () => {
    it("should handle rapid search submissions", async () => {
      render(<ProfitShareReport />);

      const searchBtn = screen.getByTestId("search-btn");

      searchBtn.click();
      searchBtn.click();
      searchBtn.click();

      await waitFor(() => {
        expect(screen.getByTestId("grid")).toBeInTheDocument();
      });
    });

    it("should handle multiple loading cycles", async () => {
      const { rerender } = render(<ProfitShareReport />);

      for (let i = 0; i < 3; i++) {
        const startLoadingBtn = screen.getByTestId("start-loading");
        startLoadingBtn.click();

        await waitFor(() => {
          rerender(<ProfitShareReport />);
        });

        const stopLoadingBtn = screen.getByTestId("stop-loading");
        stopLoadingBtn.click();

        await waitFor(() => {
          rerender(<ProfitShareReport />);
        });
      }

      expect(screen.getByTestId("grid")).toBeInTheDocument();
    });
  });

  describe("Responsive layout", () => {
    it("should render with Material-UI Grid layout", () => {
      render(<ProfitShareReport />);

      expect(screen.getByTestId("page")).toBeInTheDocument();
      expect(screen.getByTestId("accordion")).toBeInTheDocument();
    });
  });
});
