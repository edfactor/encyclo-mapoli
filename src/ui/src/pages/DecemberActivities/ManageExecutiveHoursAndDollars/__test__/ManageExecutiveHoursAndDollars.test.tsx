import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import React from "react";
import { createMockStoreAndWrapper } from "../../../../test";
import { MissiveAlertProvider } from "../../../../components/MissiveAlerts/MissiveAlertContext";
import ManageExecutiveHoursAndDollars from "../ManageExecutiveHoursAndDollars";

// Mock the child components
vi.mock("../ManageExecutiveHoursAndDollarsGrid", () => ({
  default: vi.fn(() => React.createElement('section', { 'aria-label': 'executive grid' }, 'Grid Content'))
}));

vi.mock("../ManageExecutiveHoursAndDollarsSearchFilter", () => ({
  default: vi.fn(({ onSearch, onReset }) =>
    React.createElement('section', { 'aria-label': 'search filter' },
      React.createElement('button', { onClick: () => onSearch({ badgeNumber: 12345 }) }, 'Search'),
      React.createElement('button', { onClick: onReset }, 'Reset')
    )
  )
}));

vi.mock("../../../../components/StatusDropdownActionNode", () => ({
  default: vi.fn(() => React.createElement('div', { role: 'status', 'aria-label': 'status dropdown' }, 'Status Dropdown'))
}));

vi.mock("../../../../hooks/useReadOnlyNavigation", () => ({
  useReadOnlyNavigation: vi.fn(() => false)
}));

vi.mock("../../../../hooks/useIsReadOnlyByStatus", () => ({
  useIsReadOnlyByStatus: vi.fn(() => false)
}));

vi.mock("../../../../hooks/useIsProfitYearFrozen", () => ({
  useIsProfitYearFrozen: vi.fn(() => false)
}));

// Mock the hook
const mockSaveChanges = vi.fn();
const mockExecuteSearch = vi.fn();
const mockResetSearch = vi.fn();

// Create proper report data structure that ReportSummary expects
const mockGridData = {
  reportName: "Executive Hours and Dollars",
  dataSource: "Test",
  response: {
    results: [],
    total: 0
  }
};

vi.mock("../hooks/useManageExecutiveHoursAndDollars", () => ({
  default: vi.fn(() => ({
    profitYear: 2024,
    executeSearch: mockExecuteSearch,
    resetSearch: mockResetSearch,
    isSearching: false,
    showGrid: true,
    gridData: mockGridData,
    modalResults: null,
    isModalOpen: false,
    openModal: vi.fn(),
    closeModal: vi.fn(),
    selectExecutivesInModal: vi.fn(),
    updateExecutiveRow: vi.fn(),
    isRowStagedToSave: vi.fn(() => false),
    mainGridPagination: {
      pageSize: 25,
      pageNumber: 1,
      handlePaginationChange: vi.fn(),
      handleSortChange: vi.fn(),
      resetPagination: vi.fn()
    },
    modalGridPagination: {
      pageSize: 25,
      pageNumber: 1,
      handlePaginationChange: vi.fn(),
      handleSortChange: vi.fn(),
      resetPagination: vi.fn()
    },
    executeModalSearch: vi.fn(),
    modalSelectedExecutives: [],
    addExecutivesToMainGrid: vi.fn(),
    isModalSearching: false,
    hasPendingChanges: false,
    saveChanges: mockSaveChanges
  }))
}));

describe("ManageExecutiveHoursAndDollars", () => {
  let customWrapper: ({ children }: { children: React.ReactNode }) => React.ReactElement;

  beforeEach(() => {
    vi.clearAllMocks();
    const mockState = {
      security: { token: "mock-token" },
      navigation: { navigationData: null },
      yearsEnd: {
        selectedProfitYearForFiscalClose: 2024,
        executiveHoursAndDollarsGrid: []
      }
    };
    const { wrapper } = createMockStoreAndWrapper(mockState);
    customWrapper = ({ children }: { children: React.ReactNode }) => (
      <MissiveAlertProvider>{wrapper({ children })}</MissiveAlertProvider>
    );
  });

  it("should render the page with all components", () => {
    render(<ManageExecutiveHoursAndDollars />, { wrapper: customWrapper });

    expect(screen.getByLabelText("search filter")).toBeInTheDocument();
    expect(screen.getByLabelText("executive grid")).toBeInTheDocument();
    expect(screen.getByLabelText("status dropdown")).toBeInTheDocument();
  });

  it("should render save button in disabled state when no pending changes", () => {
    render(<ManageExecutiveHoursAndDollars />, { wrapper: customWrapper });

    const saveButton = screen.getByRole("button", { name: /save/i });
    expect(saveButton).toBeDisabled();
  });

  describe("Save button functionality", () => {
    it("should call saveChanges on successful save when button is enabled", async () => {
      const user = userEvent.setup();
      mockSaveChanges.mockResolvedValueOnce(undefined);

      // Mock hook to return pending changes
      const useManageExecutiveHoursAndDollars = await import("../hooks/useManageExecutiveHoursAndDollars");
      vi.mocked(useManageExecutiveHoursAndDollars.default).mockReturnValueOnce({
        profitYear: 2024,
        executeSearch: mockExecuteSearch,
        resetSearch: mockResetSearch,
        isSearching: false,
        showGrid: true,
        gridData: mockGridData,
        modalResults: null,
        isModalOpen: false,
        openModal: vi.fn(),
        closeModal: vi.fn(),
        selectExecutivesInModal: vi.fn(),
        updateExecutiveRow: vi.fn(),
        isRowStagedToSave: vi.fn(() => false),
        mainGridPagination: {
          pageSize: 25,
          pageNumber: 1,
          handlePaginationChange: vi.fn(),
          handleSortChange: vi.fn(),
          resetPagination: vi.fn()
        },
        modalGridPagination: {
          pageSize: 25,
          pageNumber: 1,
          handlePaginationChange: vi.fn(),
          handleSortChange: vi.fn(),
          resetPagination: vi.fn()
        },
        executeModalSearch: vi.fn(),
        modalSelectedExecutives: [],
        addExecutivesToMainGrid: vi.fn(),
        isModalSearching: false,
        hasPendingChanges: true, // Has pending changes
        saveChanges: mockSaveChanges
      });

      render(<ManageExecutiveHoursAndDollars />, { wrapper: customWrapper });

      const saveButton = screen.getByRole("button", { name: /save/i });
      expect(saveButton).not.toBeDisabled();

      await user.click(saveButton);

      await waitFor(() => {
        expect(mockSaveChanges).toHaveBeenCalledTimes(1);
      });
    });

    it("should call saveChanges even on save failure to handle error appropriately", async () => {
      const user = userEvent.setup();
      mockSaveChanges.mockRejectedValueOnce(new Error("Save failed"));

      // Mock hook to return pending changes
      const useManageExecutiveHoursAndDollars = await import("../hooks/useManageExecutiveHoursAndDollars");
      vi.mocked(useManageExecutiveHoursAndDollars.default).mockReturnValueOnce({
        profitYear: 2024,
        executeSearch: mockExecuteSearch,
        resetSearch: mockResetSearch,
        isSearching: false,
        showGrid: true,
        gridData: mockGridData,
        modalResults: null,
        isModalOpen: false,
        openModal: vi.fn(),
        closeModal: vi.fn(),
        selectExecutivesInModal: vi.fn(),
        updateExecutiveRow: vi.fn(),
        isRowStagedToSave: vi.fn(() => false),
        mainGridPagination: {
          pageSize: 25,
          pageNumber: 1,
          handlePaginationChange: vi.fn(),
          handleSortChange: vi.fn(),
          resetPagination: vi.fn()
        },
        modalGridPagination: {
          pageSize: 25,
          pageNumber: 1,
          handlePaginationChange: vi.fn(),
          handleSortChange: vi.fn(),
          resetPagination: vi.fn()
        },
        executeModalSearch: vi.fn(),
        modalSelectedExecutives: [],
        addExecutivesToMainGrid: vi.fn(),
        isModalSearching: false,
        hasPendingChanges: true,
        saveChanges: mockSaveChanges
      });

      render(<ManageExecutiveHoursAndDollars />, { wrapper: customWrapper });

      const saveButton = screen.getByRole("button", { name: /save/i });
      await user.click(saveButton);

      await waitFor(() => {
        expect(mockSaveChanges).toHaveBeenCalledTimes(1);
      });
    });

    it("should not call saveChanges when button is disabled (no pending changes)", () => {
      render(<ManageExecutiveHoursAndDollars />, { wrapper: customWrapper });

      const saveButton = screen.getByRole("button", { name: /save/i });
      expect(saveButton).toBeDisabled();

      expect(mockSaveChanges).not.toHaveBeenCalled();
    });
  });

  describe("Read-only mode", () => {
    it("should disable save button in read-only mode even with pending changes", async () => {
      const useReadOnlyNavigation = await import("../../../../hooks/useReadOnlyNavigation");
      vi.mocked(useReadOnlyNavigation.useReadOnlyNavigation).mockReturnValueOnce(true);

      const useManageExecutiveHoursAndDollars = await import("../hooks/useManageExecutiveHoursAndDollars");
      vi.mocked(useManageExecutiveHoursAndDollars.default).mockReturnValueOnce({
        profitYear: 2024,
        executeSearch: mockExecuteSearch,
        resetSearch: mockResetSearch,
        isSearching: false,
        showGrid: true,
        gridData: mockGridData,
        modalResults: null,
        isModalOpen: false,
        openModal: vi.fn(),
        closeModal: vi.fn(),
        selectExecutivesInModal: vi.fn(),
        updateExecutiveRow: vi.fn(),
        isRowStagedToSave: vi.fn(() => false),
        mainGridPagination: {
          pageSize: 25,
          pageNumber: 1,
          handlePaginationChange: vi.fn(),
          handleSortChange: vi.fn(),
          resetPagination: vi.fn()
        },
        modalGridPagination: {
          pageSize: 25,
          pageNumber: 1,
          handlePaginationChange: vi.fn(),
          handleSortChange: vi.fn(),
          resetPagination: vi.fn()
        },
        executeModalSearch: vi.fn(),
        modalSelectedExecutives: [],
        addExecutivesToMainGrid: vi.fn(),
        isModalSearching: false,
        hasPendingChanges: true, // Has pending changes but read-only
        saveChanges: mockSaveChanges
      });

      render(<ManageExecutiveHoursAndDollars />, { wrapper: customWrapper });

      const saveButton = screen.getByRole("button", { name: /save/i });
      expect(saveButton).toBeDisabled();
    });

    it("should disable save button in read-only mode with tooltip wrapper", async () => {
      const useReadOnlyNavigation = await import("../../../../hooks/useReadOnlyNavigation");
      vi.mocked(useReadOnlyNavigation.useReadOnlyNavigation).mockReturnValueOnce(true);

      render(<ManageExecutiveHoursAndDollars />, { wrapper: customWrapper });

      const saveButton = screen.getByRole("button", { name: /save/i });

      // Button should be disabled in read-only mode
      expect(saveButton).toBeDisabled();

      // Button should be wrapped in a span (Tooltip requires wrapping disabled buttons)
      const span = saveButton.parentElement;
      expect(span?.tagName).toBe('SPAN');
    });
  });

  describe("Tooltip messages", () => {
    it("should wrap disabled save button in span for tooltip when no pending changes", () => {
      render(<ManageExecutiveHoursAndDollars />, { wrapper: customWrapper });

      const saveButton = screen.getByRole("button", { name: /save/i });

      // Button should be disabled when no pending changes
      expect(saveButton).toBeDisabled();

      // Button should be wrapped in a span (Tooltip requires wrapping disabled buttons)
      const span = saveButton.parentElement;
      expect(span?.tagName).toBe('SPAN');
    });
  });
});
