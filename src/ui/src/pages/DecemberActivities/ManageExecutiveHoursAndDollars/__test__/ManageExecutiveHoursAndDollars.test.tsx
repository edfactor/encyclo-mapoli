import { configureStore } from "@reduxjs/toolkit";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { Provider } from "react-redux";
import { describe, expect, it, vi } from "vitest";
import { MissiveAlertProvider } from "../../../../components/MissiveAlerts/MissiveAlertContext";
import ManageExecutiveHoursAndDollars from "../ManageExecutiveHoursAndDollars";

// Mock the child components
vi.mock("./ManageExecutiveHoursAndDollarsGrid", () => ({
  default: vi.fn(() => <div data-testid="executive-grid">Grid Content</div>)
}));

vi.mock("./ManageExecutiveHoursAndDollarsSearchFilter", () => ({
  default: vi.fn(({ onSearch, onReset }) => (
    <div data-testid="search-filter">
      <button
        data-testid="search-button"
        onClick={() => onSearch({ badgeNumber: 12345 })}>
        Search
      </button>
      <button
        data-testid="reset-button"
        onClick={onReset}>
        Reset
      </button>
    </div>
  ))
}));

vi.mock("../../../components/StatusDropdownActionNode", () => ({
  default: vi.fn(() => <div data-testid="status-dropdown">Status Dropdown</div>)
}));

vi.mock("../../../hooks/useReadOnlyNavigation", () => ({
  useReadOnlyNavigation: vi.fn(() => false)
}));

vi.mock("../../../hooks/useIsReadOnlyByStatus", () => ({
  useIsReadOnlyByStatus: vi.fn(() => false)
}));

vi.mock("../../../hooks/useIsProfitYearFrozen", () => ({
  useIsProfitYearFrozen: vi.fn(() => false)
}));

// Mock the hook
const mockSaveChanges = vi.fn();
const mockExecuteSearch = vi.fn();
const mockResetSearch = vi.fn();

vi.mock("./hooks/useManageExecutiveHoursAndDollars", () => ({
  default: vi.fn(() => ({
    profitYear: 2024,
    executeSearch: mockExecuteSearch,
    resetSearch: mockResetSearch,
    isSearching: false,
    showGrid: true,
    gridData: [],
    modalResults: [],
    isModalOpen: false,
    openModal: vi.fn(),
    closeModal: vi.fn(),
    selectExecutivesInModal: vi.fn(),
    updateExecutiveRow: vi.fn(),
    isRowStagedToSave: vi.fn(),
    mainGridPagination: { pageSize: 25, pageNumber: 1 },
    modalGridPagination: { pageSize: 25, pageNumber: 1 },
    executeModalSearch: vi.fn(),
    modalSelectedExecutives: [],
    addExecutivesToMainGrid: vi.fn(),
    isModalSearching: false,
    hasPendingChanges: false,
    saveChanges: mockSaveChanges
  }))
}));

describe("ManageExecutiveHoursAndDollars", () => {
  const createMockStore = () => {
    return configureStore({
      reducer: {
        security: () => ({ token: "mock-token" }),
        navigation: () => ({ navigationData: null }),
        yearsEnd: () => ({
          selectedProfitYearForFiscalClose: 2024,
          executiveHoursAndDollarsGrid: []
        })
      }
    });
  };

  const wrapper =
    (store: ReturnType<typeof configureStore>) =>
    ({ children }: { children: React.ReactNode }) => (
      <Provider store={store}>
        <MissiveAlertProvider>{children}</MissiveAlertProvider>
      </Provider>
    );

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("should render the page with all components", () => {
    const mockStore = createMockStore();
    render(<ManageExecutiveHoursAndDollars />, { wrapper: wrapper(mockStore) });

    expect(screen.getByTestId("search-filter")).toBeInTheDocument();
    expect(screen.getByTestId("executive-grid")).toBeInTheDocument();
    expect(screen.getByTestId("status-dropdown")).toBeInTheDocument();
  });

  it("should render save button in disabled state when no pending changes", () => {
    const mockStore = createMockStore();
    render(<ManageExecutiveHoursAndDollars />, { wrapper: wrapper(mockStore) });

    const saveButton = screen.getByRole("button", { name: /save/i });
    expect(saveButton).toBeDisabled();
  });

  describe("Save button functionality", () => {
    it("should call saveChanges on successful save when button is enabled", async () => {
      mockSaveChanges.mockResolvedValueOnce(undefined);

      // Mock hook to return pending changes
      const useManageExecutiveHoursAndDollars = await import("./hooks/useManageExecutiveHoursAndDollars");
      vi.mocked(useManageExecutiveHoursAndDollars.default).mockReturnValueOnce({
        profitYear: 2024,
        executeSearch: mockExecuteSearch,
        resetSearch: mockResetSearch,
        isSearching: false,
        showGrid: true,
        gridData: [],
        modalResults: [],
        isModalOpen: false,
        openModal: vi.fn(),
        closeModal: vi.fn(),
        selectExecutivesInModal: vi.fn(),
        updateExecutiveRow: vi.fn(),
        isRowStagedToSave: vi.fn(),
        mainGridPagination: { pageSize: 25, pageNumber: 1 },
        modalGridPagination: { pageSize: 25, pageNumber: 1 },
        executeModalSearch: vi.fn(),
        modalSelectedExecutives: [],
        addExecutivesToMainGrid: vi.fn(),
        isModalSearching: false,
        hasPendingChanges: true, // Has pending changes
        saveChanges: mockSaveChanges
      });

      const mockStore = createMockStore();
      render(<ManageExecutiveHoursAndDollars />, { wrapper: wrapper(mockStore) });

      const saveButton = screen.getByRole("button", { name: /save/i });
      expect(saveButton).not.toBeDisabled();

      fireEvent.click(saveButton);

      await waitFor(() => {
        expect(mockSaveChanges).toHaveBeenCalledTimes(1);
      });
    });

    it("should call saveChanges even on save failure to handle error appropriately", async () => {
      mockSaveChanges.mockRejectedValueOnce(new Error("Save failed"));

      // Mock hook to return pending changes
      const useManageExecutiveHoursAndDollars = await import("./hooks/useManageExecutiveHoursAndDollars");
      vi.mocked(useManageExecutiveHoursAndDollars.default).mockReturnValueOnce({
        profitYear: 2024,
        executeSearch: mockExecuteSearch,
        resetSearch: mockResetSearch,
        isSearching: false,
        showGrid: true,
        gridData: [],
        modalResults: [],
        isModalOpen: false,
        openModal: vi.fn(),
        closeModal: vi.fn(),
        selectExecutivesInModal: vi.fn(),
        updateExecutiveRow: vi.fn(),
        isRowStagedToSave: vi.fn(),
        mainGridPagination: { pageSize: 25, pageNumber: 1 },
        modalGridPagination: { pageSize: 25, pageNumber: 1 },
        executeModalSearch: vi.fn(),
        modalSelectedExecutives: [],
        addExecutivesToMainGrid: vi.fn(),
        isModalSearching: false,
        hasPendingChanges: true,
        saveChanges: mockSaveChanges
      });

      const mockStore = createMockStore();
      render(<ManageExecutiveHoursAndDollars />, { wrapper: wrapper(mockStore) });

      const saveButton = screen.getByRole("button", { name: /save/i });
      fireEvent.click(saveButton);

      await waitFor(() => {
        expect(mockSaveChanges).toHaveBeenCalledTimes(1);
      });
    });

    it("should not call saveChanges when button is disabled (no pending changes)", () => {
      const mockStore = createMockStore();
      render(<ManageExecutiveHoursAndDollars />, { wrapper: wrapper(mockStore) });

      const saveButton = screen.getByRole("button", { name: /save/i });
      expect(saveButton).toBeDisabled();

      // Try to click disabled button (should not trigger save)
      fireEvent.click(saveButton);

      expect(mockSaveChanges).not.toHaveBeenCalled();
    });
  });

  describe("Read-only mode", () => {
    it("should disable save button in read-only mode even with pending changes", async () => {
      const useReadOnlyNavigation = await import("../../../hooks/useReadOnlyNavigation");
      vi.mocked(useReadOnlyNavigation.useReadOnlyNavigation).mockReturnValueOnce(true);

      const useManageExecutiveHoursAndDollars = await import("./hooks/useManageExecutiveHoursAndDollars");
      vi.mocked(useManageExecutiveHoursAndDollars.default).mockReturnValueOnce({
        profitYear: 2024,
        executeSearch: mockExecuteSearch,
        resetSearch: mockResetSearch,
        isSearching: false,
        showGrid: true,
        gridData: [],
        modalResults: [],
        isModalOpen: false,
        openModal: vi.fn(),
        closeModal: vi.fn(),
        selectExecutivesInModal: vi.fn(),
        updateExecutiveRow: vi.fn(),
        isRowStagedToSave: vi.fn(),
        mainGridPagination: { pageSize: 25, pageNumber: 1 },
        modalGridPagination: { pageSize: 25, pageNumber: 1 },
        executeModalSearch: vi.fn(),
        modalSelectedExecutives: [],
        addExecutivesToMainGrid: vi.fn(),
        isModalSearching: false,
        hasPendingChanges: true, // Has pending changes but read-only
        saveChanges: mockSaveChanges
      });

      const mockStore = createMockStore();
      render(<ManageExecutiveHoursAndDollars />, { wrapper: wrapper(mockStore) });

      const saveButton = screen.getByRole("button", { name: /save/i });
      expect(saveButton).toBeDisabled();
    });

    it("should show read-only tooltip when hovering disabled save button in read-only mode", async () => {
      const useReadOnlyNavigation = await import("../../../hooks/useReadOnlyNavigation");
      vi.mocked(useReadOnlyNavigation.useReadOnlyNavigation).mockReturnValueOnce(true);

      const mockStore = createMockStore();
      render(<ManageExecutiveHoursAndDollars />, { wrapper: wrapper(mockStore) });

      const saveButton = screen.getByRole("button", { name: /save/i });

      // Hover over the button
      fireEvent.mouseOver(saveButton);

      await waitFor(() => {
        const tooltip = screen.getByText(/You are in read-only mode and cannot save changes/i);
        expect(tooltip).toBeInTheDocument();
      });
    });
  });

  describe("Tooltip messages", () => {
    it("should show 'no pending changes' tooltip when save button is disabled due to no changes", async () => {
      const mockStore = createMockStore();
      render(<ManageExecutiveHoursAndDollars />, { wrapper: wrapper(mockStore) });

      const saveButton = screen.getByRole("button", { name: /save/i });

      // Hover over the button
      fireEvent.mouseOver(saveButton);

      await waitFor(() => {
        const tooltip = screen.getByText(/You must change hours or dollars to save/i);
        expect(tooltip).toBeInTheDocument();
      });
    });
  });
});
