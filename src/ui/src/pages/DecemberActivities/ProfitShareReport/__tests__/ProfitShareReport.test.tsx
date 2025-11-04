import { configureStore } from "@reduxjs/toolkit";
import { render, screen, waitFor } from "@testing-library/react";
import { Provider } from "react-redux";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import ProfitShareReport from "../ProfitShareReport";
import securitySlice from "../../../../reduxstore/slices/securitySlice";
import yearsEndSlice from "../../../../reduxstore/slices/yearsEndSlice";

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
      <button
        onClick={() => onLoadingChange(true)}
        data-testid="start-loading">
        Start Loading
      </button>
      <button
        onClick={() => onLoadingChange(false)}
        data-testid="stop-loading">
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
  DSMGrid: vi.fn(() => <div data-testid="dsmgrid">DSMGrid</div>),
  Page: vi.fn(({ label, actionNode, children }) => (
    <div data-testid="page">
      <div>{label}</div>
      {actionNode}
      {children}
    </div>
  )),
  SmartModal: vi.fn(() => <div data-testid="smartmodal">SmartModal</div>),
  SearchAndReset: vi.fn(() => <div data-testid="searchandreset">SearchAndReset</div>),
  TotalsGrid: vi.fn(() => <div data-testid="totalsgrid">TotalsGrid</div>)
}));

vi.mock("components/ProfitShareTotalsDisplay", () => ({
  default: vi.fn(() => <div data-testid="totals-display">Totals Display</div>)
}));

vi.mock("../../../FiscalClose/PAY426Reports/ProfitSummary/ProfitSummary", () => ({
  default: vi.fn(({ onPresetParamsChange }: { onPresetParamsChange: (params: null) => void }) => (
    <div data-testid="profit-summary">
      <button
        data-testid="preset-btn"
        onClick={() => onPresetParamsChange({ reportId: 1 })}>
        Preset
      </button>
    </div>
  ))
}));

vi.mock("hooks/useFiscalCloseProfitYear", () => ({
  default: vi.fn(() => 2024)
}));

vi.mock("../../../reduxstore/api/YearsEndApi.ts", () => ({
  useLazyGetYearEndProfitSharingReportTotalsQuery: vi.fn(() => [
    vi.fn((_params) => Promise.resolve({ data: { totals: [] } })),
    { isFetching: false }
  ])
}));

describe("ProfitShareReport", () => {
  // Create a minimal mock API for yearsEndApi
  const mockYearsEndApi = createApi({
    reducerPath: "yearsEndApi",
    baseQuery: fetchBaseQuery({ baseUrl: "/" }),
    endpoints: () => ({})
  });

  let mockStore: ReturnType<typeof configureStore>;
  let wrapper: ({ children }: { children: React.ReactNode }) => React.ReactNode;

  beforeEach(() => {
    vi.clearAllMocks();

    // Create a fresh mock store for each test
    mockStore = configureStore({
      reducer: {
        security: securitySlice,
        yearsEnd: yearsEndSlice,
        [mockYearsEndApi.reducerPath]: mockYearsEndApi.reducer
      },
      middleware: (getDefaultMiddleware) =>
        getDefaultMiddleware({ serializableCheck: false }).concat(mockYearsEndApi.middleware)
    });

    // Create a wrapper with Provider
    wrapper = ({ children }: { children: React.ReactNode }) => <Provider store={mockStore}>{children}</Provider>;
  });

  describe("Rendering", () => {
    it("should render page with correct label", () => {
      render(<ProfitShareReport />, { wrapper });

      expect(screen.getByText(/PROFIT SHARE REPORT/i)).toBeInTheDocument();
    });

    it("should render totals display component", () => {
      render(<ProfitShareReport />, { wrapper });

      expect(screen.getByTestId("totals-display")).toBeInTheDocument();
    });

    it("should render profit summary section", () => {
      render(<ProfitShareReport />, { wrapper });

      expect(screen.getByTestId("profit-summary")).toBeInTheDocument();
    });
  });

  describe("State management", () => {
    it("should initialize with loading state", () => {
      render(<ProfitShareReport />, { wrapper });

      // Component should render without search filter initially
      expect(screen.queryByTestId("search-filter")).not.toBeInTheDocument();
    });

    it("should display totals display initially", () => {
      render(<ProfitShareReport />, { wrapper });

      expect(screen.getByTestId("totals-display")).toBeInTheDocument();
    });
  });

  describe("Preset parameter handling", () => {
    it("should show filter accordion when preset is selected", async () => {
      render(<ProfitShareReport />, { wrapper });

      const presetBtn = screen.getByTestId("preset-btn");
      presetBtn.click();

      await waitFor(() => {
        expect(screen.getByTestId("accordion")).toBeInTheDocument();
      });
    });

    it("should display search filter after preset selection", async () => {
      render(<ProfitShareReport />, { wrapper });

      const presetBtn = screen.getByTestId("preset-btn");
      presetBtn.click();

      await waitFor(() => {
        expect(screen.getByTestId("search-filter")).toBeInTheDocument();
      });
    });
  });

  describe("Component integration", () => {
    it("should render main sections", () => {
      render(<ProfitShareReport />, { wrapper });

      expect(screen.getByTestId("profit-summary")).toBeInTheDocument();
      expect(screen.getByTestId("totals-display")).toBeInTheDocument();
    });

    it("should render profit summary", () => {
      render(<ProfitShareReport />, { wrapper });

      expect(screen.getByTestId("profit-summary")).toBeInTheDocument();
    });
  });

  describe("Conditional rendering", () => {
    it("should not render filter initially", () => {
      render(<ProfitShareReport />, { wrapper });

      expect(screen.queryByTestId("accordion")).not.toBeInTheDocument();
    });

    it("should conditionally render accordion based on preset params", async () => {
      render(<ProfitShareReport />, { wrapper });

      // Initially no accordion
      expect(screen.queryByTestId("accordion")).not.toBeInTheDocument();

      // Click preset button to trigger preset params
      const presetBtn = screen.getByTestId("preset-btn");
      presetBtn.click();

      // Accordion should appear after preset selection
      await waitFor(() => {
        expect(screen.getByTestId("accordion")).toBeInTheDocument();
      });
    });
  });

  describe("Props passing", () => {
    it("should render profit summary with callback", () => {
      render(<ProfitShareReport />, { wrapper });

      const profitSummary = screen.getByTestId("profit-summary");
      expect(profitSummary).toBeInTheDocument();
    });

    it("should render with proper page label", () => {
      render(<ProfitShareReport />, { wrapper });

      expect(screen.getByText(/PROFIT SHARE REPORT/i)).toBeInTheDocument();
    });
  });
});
