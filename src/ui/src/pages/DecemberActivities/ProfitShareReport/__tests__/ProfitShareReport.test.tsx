import { configureStore } from "@reduxjs/toolkit";
import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { render, screen } from "@testing-library/react";
import { Provider } from "react-redux";
import { beforeEach, describe, expect, it, vi } from "vitest";
import securitySlice from "../../../../reduxstore/slices/securitySlice";
import yearsEndSlice from "../../../../reduxstore/slices/yearsEndSlice";
import ProfitShareReport from "../ProfitShareReport";

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

vi.mock("../../../FiscalClose/PAY426Reports/ProfitSummary/ProfitSummary", () => ({
  default: vi.fn(() => <div data-testid="profit-summary">Profit Summary</div>)
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

    it("should render profit summary section", () => {
      render(<ProfitShareReport />, { wrapper });

      expect(screen.getByTestId("profit-summary")).toBeInTheDocument();
    });
  });

  describe("State management", () => {
    it("should render with profit summary section", () => {
      render(<ProfitShareReport />, { wrapper });

      // Component should render profit summary
      expect(screen.getByTestId("profit-summary")).toBeInTheDocument();
    });
  });

  describe("Component integration", () => {
    it("should render main sections", () => {
      render(<ProfitShareReport />, { wrapper });

      expect(screen.getByTestId("profit-summary")).toBeInTheDocument();
    });

    it("should render profit summary", () => {
      render(<ProfitShareReport />, { wrapper });

      expect(screen.getByTestId("profit-summary")).toBeInTheDocument();
    });
  });

  describe("Props passing", () => {
    it("should render with proper page label", () => {
      render(<ProfitShareReport />, { wrapper });

      expect(screen.getByText(/PROFIT SHARE REPORT/i)).toBeInTheDocument();
    });
  });
});
