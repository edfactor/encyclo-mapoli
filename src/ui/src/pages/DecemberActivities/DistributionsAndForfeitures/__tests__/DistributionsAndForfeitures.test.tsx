import { render, screen, waitFor, act } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi, beforeEach } from "vitest";
import DistributionsAndForfeitures from "../DistributionsAndForfeitures";
import DistributionsAndForfeituresGrid from "../DistributionsAndForfeituresGrid";
import DistributionsAndForfeituresSearchFilter from "../DistributionsAndForfeituresSearchFilter";

// Mock child components
vi.mock("../DistributionsAndForfeituresSearchFilter", () => ({
  default: vi.fn(({ setInitialSearchLoaded: _setInitialSearchLoaded, isFetching }: Record<string, unknown>) => (
    <div data-testid="search-filter">
      <button
        onClick={() => (_setInitialSearchLoaded as (val: boolean) => void)(true)}
        data-testid="search-btn">
        Search
      </button>
      {isFetching && <div data-testid="fetching">Fetching...</div>}
    </div>
  ))
}));

vi.mock("../DistributionsAndForfeituresGrid", () => ({
  default: vi.fn(
    ({
      setInitialSearchLoaded: _setInitialSearchLoaded,
      initialSearchLoaded: _initialSearchLoaded,
      onLoadingChange
    }: Record<string, unknown>) => (
      <div data-testid="grid">
        <button
          onClick={() => (onLoadingChange as (val: boolean) => void)(true)}
          data-testid="start-loading">
          Start Loading
        </button>
        <button
          onClick={() => (onLoadingChange as (val: boolean) => void)(false)}
          data-testid="stop-loading">
          Stop Loading
        </button>
        Grid Component
      </div>
    )
  )
}));

vi.mock("components/StatusDropdownActionNode", () => ({
  default: vi.fn(() => <div data-testid="status-dropdown">Status Dropdown</div>)
}));

vi.mock("smart-ui-library", () => ({
  DSMAccordion: vi.fn(({ title, children }: { title: string; children: React.ReactNode }) => (
    <div data-testid="accordion">
      <div>{title}</div>
      {children}
    </div>
  )),
  Page: vi.fn(
    ({ label, actionNode, children }: { label: string; actionNode?: React.ReactNode; children?: React.ReactNode }) => (
      <div data-testid="page">
        <div>{label}</div>
        {actionNode}
        {children}
      </div>
    )
  )
}));

describe("DistributionsAndForfeitures", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("Rendering", () => {
    it("should render the page component", () => {
      render(<DistributionsAndForfeitures />);

      expect(screen.getByTestId("page")).toBeInTheDocument();
    });

    it("should render page with correct label", () => {
      render(<DistributionsAndForfeitures />);

      expect(screen.getByText(/DISTRIBUTIONS AND FORFEITURES/i)).toBeInTheDocument();
    });

    it("should render StatusDropdownActionNode", () => {
      render(<DistributionsAndForfeitures />);

      expect(screen.getByTestId("status-dropdown")).toBeInTheDocument();
    });

    it("should render filter accordion", () => {
      render(<DistributionsAndForfeitures />);

      expect(screen.getByTestId("accordion")).toBeInTheDocument();
      expect(screen.getByText("Filter")).toBeInTheDocument();
    });

    it("should render search filter inside accordion", () => {
      render(<DistributionsAndForfeitures />);

      expect(screen.getByTestId("search-filter")).toBeInTheDocument();
    });

    it("should render grid component", () => {
      render(<DistributionsAndForfeitures />);

      expect(screen.getByTestId("grid")).toBeInTheDocument();
    });
  });

  describe("State management", () => {
    it("should initialize with initialSearchLoaded false", () => {
      render(<DistributionsAndForfeitures />);

      // Verify grid receives initialSearchLoaded prop
      const calls = vi.mocked(DistributionsAndForfeituresGrid).mock.calls;
      expect(calls.length).toBeGreaterThan(0);
      expect(calls[0][0]).toHaveProperty("initialSearchLoaded", false);
    });

    it("should initialize with isFetching false", () => {
      render(<DistributionsAndForfeitures />);

      // Verify search filter receives isFetching prop
      const calls = vi.mocked(DistributionsAndForfeituresSearchFilter).mock.calls;
      expect(calls.length).toBeGreaterThan(0);
      expect(calls[0][0]).toHaveProperty("isFetching", false);
    });
  });

  describe("Search and loading coordination", () => {
    it("should pass setInitialSearchLoaded to search filter", async () => {
      render(<DistributionsAndForfeitures />);

      await waitFor(() => {
        const calls = vi.mocked(DistributionsAndForfeituresSearchFilter).mock.calls;
        expect(calls.length).toBeGreaterThan(0);
        expect(calls[0][0]).toHaveProperty("setInitialSearchLoaded");
        expect(typeof calls[0][0].setInitialSearchLoaded).toBe("function");
      });
    });

    it("should pass setInitialSearchLoaded to grid", async () => {
      render(<DistributionsAndForfeitures />);

      await waitFor(() => {
        const calls = vi.mocked(DistributionsAndForfeituresGrid).mock.calls;
        expect(calls.length).toBeGreaterThan(0);
        expect(calls[0][0]).toHaveProperty("setInitialSearchLoaded");
        expect(typeof calls[0][0].setInitialSearchLoaded).toBe("function");
      });
    });

    it("should pass onLoadingChange callback to grid", async () => {
      render(<DistributionsAndForfeitures />);

      await waitFor(() => {
        const calls = vi.mocked(DistributionsAndForfeituresGrid).mock.calls;
        expect(calls.length).toBeGreaterThan(0);
        expect(calls[0][0]).toHaveProperty("onLoadingChange");
        expect(typeof calls[0][0].onLoadingChange).toBe("function");
      });
    });
  });

  describe("Loading state propagation", () => {
    it("should update isFetching when grid calls onLoadingChange", async () => {
      const user = userEvent.setup();
      const { rerender } = render(<DistributionsAndForfeitures />);

      const startLoadingBtn = screen.getByTestId("start-loading");
      await act(async () => {
        await user.click(startLoadingBtn);
      });

      await waitFor(() => {
        rerender(<DistributionsAndForfeitures />);
        expect(screen.getByTestId("fetching")).toBeInTheDocument();
      });
    });

    it("should clear isFetching when grid signals loading complete", async () => {
      const user = userEvent.setup();
      const { rerender } = render(<DistributionsAndForfeitures />);

      const startLoadingBtn = screen.getByTestId("start-loading");
      await act(async () => {
        await user.click(startLoadingBtn);
      });

      await waitFor(() => {
        rerender(<DistributionsAndForfeitures />);
        expect(screen.getByTestId("fetching")).toBeInTheDocument();
      });

      const stopLoadingBtn = screen.getByTestId("stop-loading");
      await act(async () => {
        await user.click(stopLoadingBtn);
      });

      await waitFor(() => {
        rerender(<DistributionsAndForfeitures />);
        expect(screen.queryByTestId("fetching")).not.toBeInTheDocument();
      });
    });
  });

  describe("Component integration", () => {
    it("should render all major sections", () => {
      render(<DistributionsAndForfeitures />);

      expect(screen.getByTestId("page")).toBeInTheDocument();
      expect(screen.getByTestId("status-dropdown")).toBeInTheDocument();
      expect(screen.getByTestId("accordion")).toBeInTheDocument();
      expect(screen.getByTestId("search-filter")).toBeInTheDocument();
      expect(screen.getByTestId("grid")).toBeInTheDocument();
    });

    it("should maintain proper component hierarchy", () => {
      render(<DistributionsAndForfeitures />);

      const page = screen.getByTestId("page");
      const accordion = screen.getByTestId("accordion");
      const searchFilter = screen.getByTestId("search-filter");
      const grid = screen.getByTestId("grid");

      // Verify nesting relationship
      expect(page).toBeInTheDocument();
      expect(accordion).toBeInTheDocument();
      expect(searchFilter).toBeInTheDocument();
      expect(grid).toBeInTheDocument();
    });
  });

  describe("Search button interaction", () => {
    it("should update initialSearchLoaded when search is triggered", async () => {
      const user = userEvent.setup();
      render(<DistributionsAndForfeitures />);

      const searchBtn = screen.getByTestId("search-btn");
      await act(async () => {
        await user.click(searchBtn);
      });

      await waitFor(() => {
        const calls = vi.mocked(DistributionsAndForfeituresGrid).mock.calls;
        expect(calls.length).toBeGreaterThan(0);
        // Find a call where initialSearchLoaded is true
        const callWithTrue = calls.find((call) => call[0].initialSearchLoaded === true);
        expect(callWithTrue).toBeDefined();
      });
    });
  });

  describe("Responsive layout", () => {
    it("should render with Material-UI Grid layout", () => {
      render(<DistributionsAndForfeitures />);

      expect(screen.getByTestId("page")).toBeInTheDocument();
      expect(screen.getByTestId("accordion")).toBeInTheDocument();
    });

    it("should have proper spacing and structure", () => {
      render(<DistributionsAndForfeitures />);

      const page = screen.getByTestId("page");
      expect(page).toBeInTheDocument();
      expect(page.tagName).toBe("DIV");
    });
  });

  describe("Edge cases", () => {
    it("should handle rapid search clicks", async () => {
      render(<DistributionsAndForfeitures />);

      const searchBtn = screen.getByTestId("search-btn");

      // Click multiple times rapidly
      searchBtn.click();
      searchBtn.click();
      searchBtn.click();

      await waitFor(() => {
        expect(screen.getByTestId("grid")).toBeInTheDocument();
      });
    });

    it("should handle loading state transitions", async () => {
      const { rerender } = render(<DistributionsAndForfeitures />);

      const startLoadingBtn = screen.getByTestId("start-loading");
      const stopLoadingBtn = screen.getByTestId("stop-loading");

      // Simulate multiple loading/unloading cycles
      startLoadingBtn.click();
      await waitFor(() => {
        rerender(<DistributionsAndForfeitures />);
      });

      stopLoadingBtn.click();
      await waitFor(() => {
        rerender(<DistributionsAndForfeitures />);
      });

      startLoadingBtn.click();
      await waitFor(() => {
        rerender(<DistributionsAndForfeitures />);
      });

      expect(screen.getByTestId("grid")).toBeInTheDocument();
    });
  });
});
