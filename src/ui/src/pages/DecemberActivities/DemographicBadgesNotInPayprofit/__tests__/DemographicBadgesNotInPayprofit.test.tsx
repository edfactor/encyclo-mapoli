import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { Provider } from "react-redux";
import { configureStore } from "@reduxjs/toolkit";

// Mock dependencies
vi.mock("../hooks/useDemographicBadgesNotInPayprofit");
vi.mock("../DemographicBadgesNotInPayprofitGrid");
vi.mock("components/StatusDropdownActionNode");
vi.mock("smart-ui-library", () => ({
  Page: ({
    label,
    actionNode,
    children
  }: {
    label: string;
    actionNode?: React.ReactNode;
    children?: React.ReactNode;
  }) => (
    <div
      data-testid="page"
      data-label={label}>
      <div>{label}</div>
      {actionNode}
      {children}
    </div>
  )
}));

import DemographicBadgesNotInPayprofit from "../DemographicBadgesNotInPayprofit";
import useDemographicBadgesNotInPayprofit from "../hooks/useDemographicBadgesNotInPayprofit";
import DemographicBadgesNotInPayprofitGrid from "../DemographicBadgesNotInPayprofitGrid";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";

interface MockResult {
  badgeNumber: number;
  ssn: number;
  employeeName: string;
  store: number;
  status: string;
  statusName: string;
}

interface MockSearchResults {
  reportName: string;
  reportDate: string;
  startDate: string;
  endDate: string;
  dataSource: string;
  response: {
    results: MockResult[];
    total: number;
    totalPages: number;
    pageSize: number;
    currentPage: number;
  };
}

interface MockPagination {
  pageNumber: number;
  pageSize: number;
  sortParams: { sortBy: string; isSortDescending: boolean };
  handlePaginationChange: ReturnType<typeof vi.fn>;
  handleSortChange: ReturnType<typeof vi.fn>;
}

interface MockHookReturn {
  searchResults: MockSearchResults | null;
  isSearching: boolean;
  pagination: MockPagination;
  showData: boolean;
  hasResults: boolean;
  searchParams: { profitYear: number } | null;
  executeSearch: ReturnType<typeof vi.fn>;
}

const createMockStore = () => {
  return configureStore({
    reducer: {
      security: (state = { token: "mock-token" }) => state,
      decemberFlow: (state = { profitYear: 2024 }) => state
    }
  });
};

const createMockSearchResults = (results: MockResult[], total: number): MockSearchResults => ({
  reportName: "Demographic Badges Not In Payprofit",
  reportDate: "2024-01-15",
  startDate: "2024-01-01",
  endDate: "2024-12-31",
  dataSource: "Test Data",
  response: {
    results,
    total,
    totalPages: Math.ceil(total / 25),
    pageSize: 25,
    currentPage: 0
  }
});

describe("DemographicBadgesNotInPayprofit Component", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("Rendering", () => {
    it("should render the page component", () => {
      vi.mocked(useDemographicBadgesNotInPayprofit).mockReturnValue({
        searchResults: createMockSearchResults([{ badgeNumber: 12345, ssn: 123456789, employeeName: "John Doe", store: 1, status: "Active", statusName: "Active" }], 1),
        isSearching: false,
        pagination: {
          pageNumber: 0,
          pageSize: 25,
          sortParams: { sortBy: "badgeNumber", isSortDescending: true },
          handlePaginationChange: vi.fn(),
          handleSortChange: vi.fn()
        },
        showData: true,
        hasResults: true,
        searchParams: { profitYear: 2024 },
        executeSearch: vi.fn()
      } as MockHookReturn);

      vi.mocked(DemographicBadgesNotInPayprofitGrid).mockReturnValue(<div data-testid="grid">Grid Component</div>);

      vi.mocked(StatusDropdownActionNode).mockReturnValue(<div data-testid="status-dropdown">Status Dropdown</div>);

      const store = createMockStore();
      render(
        <Provider store={store}>
          <DemographicBadgesNotInPayprofit />
        </Provider>
      );

      expect(screen.getByTestId("page")).toBeInTheDocument();
    });

    it("should render page with correct label including record count", () => {
      vi.mocked(useDemographicBadgesNotInPayprofit).mockReturnValue({
        searchResults: createMockSearchResults([{ badgeNumber: 12345, ssn: 123456789, employeeName: "John Doe", store: 1, status: "Active", statusName: "Active" }], 1),
        isSearching: false,
        pagination: {
          pageNumber: 0,
          pageSize: 25,
          sortParams: { sortBy: "badgeNumber", isSortDescending: true },
          handlePaginationChange: vi.fn(),
          handleSortChange: vi.fn()
        },
        showData: true,
        hasResults: true,
        searchParams: { profitYear: 2024 },
        executeSearch: vi.fn()
      } as MockHookReturn);

      vi.mocked(DemographicBadgesNotInPayprofitGrid).mockReturnValue(<div data-testid="grid">Grid</div>);

      vi.mocked(StatusDropdownActionNode).mockReturnValue(<div data-testid="status-dropdown">Status</div>);

      const store = createMockStore();
      render(
        <Provider store={store}>
          <DemographicBadgesNotInPayprofit />
        </Provider>
      );

      const pageLabel = screen.getByText(/1 records/i);
      expect(pageLabel).toBeInTheDocument();
    });

    it("should render StatusDropdownActionNode", () => {
      vi.mocked(useDemographicBadgesNotInPayprofit).mockReturnValue({
        searchResults: null,
        isSearching: false,
        pagination: {
          pageNumber: 0,
          pageSize: 25,
          sortParams: { sortBy: "badgeNumber", isSortDescending: true },
          handlePaginationChange: vi.fn(),
          handleSortChange: vi.fn()
        },
        showData: false,
        hasResults: false,
        searchParams: null,
        executeSearch: vi.fn()
      } as MockHookReturn);

      vi.mocked(DemographicBadgesNotInPayprofitGrid).mockReturnValue(<div data-testid="grid">Grid</div>);

      vi.mocked(StatusDropdownActionNode).mockReturnValue(<div data-testid="status-dropdown">Status Dropdown</div>);

      const store = createMockStore();
      render(
        <Provider store={store}>
          <DemographicBadgesNotInPayprofit />
        </Provider>
      );

      expect(screen.getByTestId("status-dropdown")).toBeInTheDocument();
    });

    it("should render DemographicBadgesNotInPayprofitGrid", () => {
      vi.mocked(useDemographicBadgesNotInPayprofit).mockReturnValue({
        searchResults: createMockSearchResults([], 0),
        isSearching: false,
        pagination: {
          pageNumber: 0,
          pageSize: 25,
          sortParams: { sortBy: "badgeNumber", isSortDescending: true },
          handlePaginationChange: vi.fn(),
          handleSortChange: vi.fn()
        },
        showData: false,
        hasResults: false,
        searchParams: { profitYear: 2024 },
        executeSearch: vi.fn()
      } as MockHookReturn);

      vi.mocked(DemographicBadgesNotInPayprofitGrid).mockReturnValue(<div data-testid="grid">Grid Component</div>);

      vi.mocked(StatusDropdownActionNode).mockReturnValue(<div data-testid="status-dropdown">Status</div>);

      const store = createMockStore();
      render(
        <Provider store={store}>
          <DemographicBadgesNotInPayprofit />
        </Provider>
      );

      expect(screen.getByTestId("grid")).toBeInTheDocument();
    });
  });

  describe("Data display", () => {
    it("should display record count from hook results", () => {
      vi.mocked(useDemographicBadgesNotInPayprofit).mockReturnValue({
        searchResults: createMockSearchResults([{ badgeNumber: 1, ssn: 111111111, employeeName: "Name", store: 1, status: "Active", statusName: "Active" }], 1),
        isSearching: false,
        pagination: {
          pageNumber: 0,
          pageSize: 25,
          sortParams: { sortBy: "badgeNumber", isSortDescending: true },
          handlePaginationChange: vi.fn(),
          handleSortChange: vi.fn()
        },
        showData: true,
        hasResults: true,
        searchParams: { profitYear: 2024 },
        executeSearch: vi.fn()
      } as MockHookReturn);

      vi.mocked(DemographicBadgesNotInPayprofitGrid).mockReturnValue(<div data-testid="grid">Grid</div>);

      vi.mocked(StatusDropdownActionNode).mockReturnValue(<div data-testid="status-dropdown">Status</div>);

      const store = createMockStore();
      render(
        <Provider store={store}>
          <DemographicBadgesNotInPayprofit />
        </Provider>
      );

      expect(screen.getByText(/1 records/i)).toBeInTheDocument();
    });

    it("should display zero records when no results", () => {
      vi.mocked(useDemographicBadgesNotInPayprofit).mockReturnValue({
        searchResults: null,
        isSearching: false,
        pagination: {
          pageNumber: 0,
          pageSize: 25,
          sortParams: { sortBy: "badgeNumber", isSortDescending: true },
          handlePaginationChange: vi.fn(),
          handleSortChange: vi.fn()
        },
        showData: false,
        hasResults: false,
        searchParams: null,
        executeSearch: vi.fn()
      } as MockHookReturn);

      vi.mocked(DemographicBadgesNotInPayprofitGrid).mockReturnValue(<div data-testid="grid">Grid</div>);

      vi.mocked(StatusDropdownActionNode).mockReturnValue(<div data-testid="status-dropdown">Status</div>);

      const store = createMockStore();
      render(
        <Provider store={store}>
          <DemographicBadgesNotInPayprofit />
        </Provider>
      );

      expect(screen.getByText(/0 records/i)).toBeInTheDocument();
    });

    it("should display multiple records count", () => {
      vi.mocked(useDemographicBadgesNotInPayprofit).mockReturnValue({
        searchResults: createMockSearchResults([
          { badgeNumber: 1, ssn: 111111111, employeeName: "Jane Doe", store: 1, status: "Active", statusName: "Active" },
          { badgeNumber: 2, ssn: 222222222, employeeName: "John Smith", store: 2, status: "Active", statusName: "Active" },
          { badgeNumber: 3, ssn: 333333333, employeeName: "Bob Johnson", store: 3, status: "Active", statusName: "Active" }
        ], 3),
        isSearching: false,
        pagination: {
          pageNumber: 0,
          pageSize: 25,
          sortParams: { sortBy: "badgeNumber", isSortDescending: true },
          handlePaginationChange: vi.fn(),
          handleSortChange: vi.fn()
        },
        showData: true,
        hasResults: true,
        searchParams: { profitYear: 2024 },
        executeSearch: vi.fn()
      } as MockHookReturn);

      vi.mocked(DemographicBadgesNotInPayprofitGrid).mockReturnValue(<div data-testid="grid">Grid</div>);

      vi.mocked(StatusDropdownActionNode).mockReturnValue(<div data-testid="status-dropdown">Status</div>);

      const store = createMockStore();
      render(
        <Provider store={store}>
          <DemographicBadgesNotInPayprofit />
        </Provider>
      );

      expect(screen.getByText(/3 records/i)).toBeInTheDocument();
    });
  });

  describe("Grid props passing", () => {
    it("should pass data to grid", () => {
      const mockResults = createMockSearchResults([{ badgeNumber: 123, ssn: 123000000, employeeName: "Test", store: 1, status: "Active", statusName: "Active" }], 1);

      const mockHandlePaginationChange = vi.fn();
      const mockHandleSortChange = vi.fn();

      vi.mocked(useDemographicBadgesNotInPayprofit).mockReturnValue({
        searchResults: mockResults,
        isSearching: false,
        pagination: {
          pageNumber: 0,
          pageSize: 25,
          sortParams: { sortBy: "badgeNumber", isSortDescending: true },
          handlePaginationChange: mockHandlePaginationChange,
          handleSortChange: mockHandleSortChange
        },
        showData: true,
        hasResults: true,
        searchParams: { profitYear: 2024 },
        executeSearch: vi.fn()
      } as MockHookReturn);

      vi.mocked(DemographicBadgesNotInPayprofitGrid).mockReturnValue(<div data-testid="grid">Grid</div>);

      vi.mocked(StatusDropdownActionNode).mockReturnValue(<div data-testid="status-dropdown">Status</div>);

      const store = createMockStore();
      render(
        <Provider store={store}>
          <DemographicBadgesNotInPayprofit />
        </Provider>
      );

      // Verify grid component was called with correct props
      const gridCalls = vi.mocked(DemographicBadgesNotInPayprofitGrid).mock.calls;
      expect(gridCalls.length).toBeGreaterThan(0);
      const lastCall = gridCalls[gridCalls.length - 1];
      expect(lastCall[0].data).toEqual(mockResults);
      expect(lastCall[0].isLoading).toBe(false);
      expect(lastCall[0].showData).toBe(true);
      expect(lastCall[0].hasResults).toBe(true);
    });
  });

  describe("Loading states", () => {
    it("should pass isLoading state to grid", () => {
      vi.mocked(useDemographicBadgesNotInPayprofit).mockReturnValue({
        searchResults: createMockSearchResults([], 0),
        isSearching: true,
        pagination: {
          pageNumber: 0,
          pageSize: 25,
          sortParams: { sortBy: "badgeNumber", isSortDescending: true },
          handlePaginationChange: vi.fn(),
          handleSortChange: vi.fn()
        },
        showData: true,
        hasResults: false,
        searchParams: { profitYear: 2024 },
        executeSearch: vi.fn()
      } as MockHookReturn);

      vi.mocked(DemographicBadgesNotInPayprofitGrid).mockReturnValue(<div data-testid="grid">Grid</div>);

      vi.mocked(StatusDropdownActionNode).mockReturnValue(<div data-testid="status-dropdown">Status</div>);

      const store = createMockStore();
      render(
        <Provider store={store}>
          <DemographicBadgesNotInPayprofit />
        </Provider>
      );

      const gridCalls = vi.mocked(DemographicBadgesNotInPayprofitGrid).mock.calls;
      expect(gridCalls.length).toBeGreaterThan(0);
      const lastCall = gridCalls[gridCalls.length - 1];
      expect(lastCall[0]).toHaveProperty("isLoading", true);
    });
  });
});
