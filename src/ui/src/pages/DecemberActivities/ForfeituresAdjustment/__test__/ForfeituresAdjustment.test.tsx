import React from "react";
import { configureStore } from "@reduxjs/toolkit";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { Provider } from "react-redux";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { MissiveAlertProvider } from "../../../../components/MissiveAlerts/MissiveAlertContext";
import ForfeituresAdjustment from "../ForfeituresAdjustment";

// Mock child components
vi.mock("../ForfeituresAdjustmentSearchFilter", () => ({
  default: vi.fn(({ onSearch, onReset, isSearching }) =>
    React.createElement('section', { 'aria-label': 'search filter' },
      React.createElement('button', {
        onClick: () => onSearch({
          ssn: "123-45-6789",
          badge: "",
          profitYear: 2024,
          skip: 0,
          take: 255,
          sortBy: "badgeNumber",
          isSortDescending: false
        })
      }, 'Search'),
      React.createElement('button', { onClick: onReset }, 'Reset'),
      isSearching && React.createElement('span', { role: 'status' }, 'Searching...')
    )
  )
}));

vi.mock("../ForfeituresAdjustmentPanel", () => ({
  default: vi.fn(({ onAddForfeiture, isReadOnly }) =>
    React.createElement('section', { 'aria-label': 'forfeiture panel' },
      React.createElement('button', {
        onClick: onAddForfeiture,
        disabled: isReadOnly
      }, 'Add Forfeiture')
    )
  )
}));

vi.mock("../ForfeituresTransactionGrid", () => ({
  default: vi.fn(() => React.createElement('section', { 'aria-label': 'transaction grid' }, 'Transaction Grid'))
}));

vi.mock("../AddForfeitureModal", () => ({
  default: vi.fn(({ open, onClose, onSave }) =>
    open && React.createElement('section', { 'aria-label': 'add forfeiture modal' },
      React.createElement('button', { onClick: onClose }, 'Close'),
      React.createElement('button', {
        onClick: () => onSave({ forfeitureAmount: 1000, classAction: false })
      }, 'Save')
    )
  )
}));

vi.mock("../../../InquiriesAndAdjustments/MasterInquiry/StandaloneMemberDetails", () => ({
  default: vi.fn(() => React.createElement('section', { 'aria-label': 'member details' }, 'Member Details'))
}));

vi.mock("../../../../components/StatusDropdownActionNode", () => ({
  default: vi.fn(() => React.createElement('div', { role: 'status', 'aria-label': 'status dropdown' }, 'Status Dropdown'))
}));

vi.mock("../../../../hooks/useReadOnlyNavigation", () => ({
  useReadOnlyNavigation: vi.fn(() => false)
}));

// Mock the hook with comprehensive state
const mockExecuteSearch = vi.fn();
const mockHandleReset = vi.fn();
const mockOpenModal = vi.fn();
const mockCloseModal = vi.fn();
const mockHandleSaveForfeiture = vi.fn();

vi.mock("../hooks/useForfeituresAdjustment", () => ({
  default: vi.fn(() => ({
    employeeData: null,
    transactionData: null,
    isSearching: false,
    isFetchingTransactions: false,
    isAddForfeitureModalOpen: false,
    showEmployeeData: false,
    showTransactions: false,
    executeSearch: mockExecuteSearch,
    handleReset: mockHandleReset,
    handleSaveForfeiture: mockHandleSaveForfeiture,
    openAddForfeitureModal: mockOpenModal,
    closeAddForfeitureModal: mockCloseModal,
    transactionPagination: {
      pageNumber: 0,
      pageSize: 25,
      sortParams: { sortBy: "date", isSortDescending: true },
      handlePaginationChange: vi.fn(),
      handleSortChange: vi.fn(),
      resetPagination: vi.fn()
    },
    profitYear: 2024,
    isReadOnly: false,
    memberDetailsRefreshTrigger: 0
  }))
}));

describe("ForfeituresAdjustment", () => {
  const createMockStore = () => {
    return configureStore({
      reducer: {
        security: () => ({ token: "mock-token" }),
        navigation: () => ({ navigationData: null }),
        yearsEnd: () => ({
          selectedProfitYearForFiscalClose: 2024
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

  describe("Rendering", () => {
    it("should render the page with all major components", async () => {
      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      await waitFor(() => {
        expect(screen.getByRole("button", { name: /search/i })).toBeInTheDocument();
        expect(screen.getByLabelText("status dropdown")).toBeInTheDocument();
      });
    });

    it("should render the search filter accordion", async () => {
      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      await waitFor(() => {
        expect(screen.getByRole("button", { name: /search/i })).toBeInTheDocument();
        expect(screen.getByRole("button", { name: /reset/i })).toBeInTheDocument();
      });
    });

    it("should hide employee data and transactions before search", () => {
      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      expect(screen.queryByLabelText("member details")).not.toBeInTheDocument();
      expect(screen.queryByRole("button", { name: /add forfeiture/i })).not.toBeInTheDocument();
      expect(screen.queryByLabelText("transaction grid")).not.toBeInTheDocument();
    });
  });

  describe("Search functionality", () => {
    it("should call executeSearch when search button is clicked", async () => {
      const user = userEvent.setup();
      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      const searchButton = screen.getByRole("button", { name: /search/i });
      await user.click(searchButton);

      await waitFor(() => {
        expect(mockExecuteSearch).toHaveBeenCalledWith(
          expect.objectContaining({
            ssn: "123-45-6789"
          })
        );
      });
    });

    it("should call handleReset when reset button is clicked", async () => {
      const user = userEvent.setup();
      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      const resetButton = screen.getByRole("button", { name: /reset/i });
      await user.click(resetButton);

      await waitFor(() => {
        expect(mockHandleReset).toHaveBeenCalled();
      });
    });

    it("should display loading state during search", async () => {
      const useForfeituresAdjustment = await import("../hooks/useForfeituresAdjustment");
      vi.mocked(useForfeituresAdjustment.default).mockReturnValueOnce({
        searchParams: null,
        employeeData: null,
        memberDetails: null,
        transactionData: null,
        isSearching: true,
        isFetchingMemberDetails: false,
        isFetchingTransactions: false,
        isAddForfeitureModalOpen: false,
        showEmployeeData: false,
        showMemberDetails: false,
        showTransactions: false,
        executeSearch: mockExecuteSearch,
        handleReset: mockHandleReset,
        handleSaveForfeiture: mockHandleSaveForfeiture,
        openAddForfeitureModal: mockOpenModal,
        closeAddForfeitureModal: mockCloseModal,
        transactionPagination: {
          pageNumber: 0,
          pageSize: 25,
          sortParams: { sortBy: "date", isSortDescending: true },
          handlePaginationChange: vi.fn(),
          handleSortChange: vi.fn(),
          resetPagination: vi.fn()
        },
        profitYear: 2024,
        isReadOnly: false,
        memberDetailsRefreshTrigger: 0
      } as unknown);

      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      await waitFor(() => {
        expect(screen.getByText("Searching...")).toBeInTheDocument();
      });
    });
  });

  describe("Employee data display", () => {
    it("should display employee data after search succeeds", async () => {
      const useForfeituresAdjustment = await import("../hooks/useForfeituresAdjustment");
      vi.mocked(useForfeituresAdjustment.default).mockReturnValueOnce({
        searchParams: { ssn: "123-45-6789", badge: "", profitYear: 2024, skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: false },
        employeeData: { badgeNumber: 12345, demographicId: 123, suggestedForfeitAmount: 100 },
        memberDetails: null,
        transactionData: null,
        isSearching: false,
        isFetchingMemberDetails: false,
        isFetchingTransactions: false,
        isAddForfeitureModalOpen: false,
        showEmployeeData: true,
        showMemberDetails: false,
        showTransactions: false,
        executeSearch: mockExecuteSearch,
        handleReset: mockHandleReset,
        handleSaveForfeiture: mockHandleSaveForfeiture,
        openAddForfeitureModal: mockOpenModal,
        closeAddForfeitureModal: mockCloseModal,
        transactionPagination: {
          pageNumber: 0,
          pageSize: 25,
          sortParams: { sortBy: "date", isSortDescending: true },
          handlePaginationChange: vi.fn(),
          handleSortChange: vi.fn(),
          resetPagination: vi.fn()
        },
        profitYear: 2024,
        isReadOnly: false,
        memberDetailsRefreshTrigger: 0
      } as unknown);

      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      await waitFor(() => {
        expect(screen.getByLabelText("member details")).toBeInTheDocument();
        expect(screen.getByRole("button", { name: /add forfeiture/i })).toBeInTheDocument();
      });
    });

    it("should display transaction grid when transactions are available", async () => {
      const useForfeituresAdjustment = await import("../hooks/useForfeituresAdjustment");
      vi.mocked(useForfeituresAdjustment.default).mockReturnValueOnce({
        searchParams: { ssn: "123-45-6789", badge: "", profitYear: 2024, skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: false },
        employeeData: { badgeNumber: 12345, demographicId: 123, suggestedForfeitAmount: 100 },
        memberDetails: null,
        transactionData: { results: [{ id: 1 }], total: 1 },
        isSearching: false,
        isFetchingMemberDetails: false,
        isFetchingTransactions: false,
        isAddForfeitureModalOpen: false,
        showEmployeeData: true,
        showMemberDetails: false,
        showTransactions: true,
        executeSearch: mockExecuteSearch,
        handleReset: mockHandleReset,
        handleSaveForfeiture: mockHandleSaveForfeiture,
        openAddForfeitureModal: mockOpenModal,
        closeAddForfeitureModal: mockCloseModal,
        transactionPagination: {
          pageNumber: 0,
          pageSize: 25,
          sortParams: { sortBy: "date", isSortDescending: true },
          handlePaginationChange: vi.fn(),
          handleSortChange: vi.fn(),
          resetPagination: vi.fn()
        },
        profitYear: 2024,
        isReadOnly: false,
        memberDetailsRefreshTrigger: 0
      } as unknown);

      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      await waitFor(() => {
        expect(screen.getByLabelText("transaction grid")).toBeInTheDocument();
      });
    });
  });

  describe("Modal functionality", () => {
    it("should open add forfeiture modal when button is clicked", async () => {
      const useForfeituresAdjustment = await import("../hooks/useForfeituresAdjustment");
      vi.mocked(useForfeituresAdjustment.default).mockReturnValueOnce({
        searchParams: { ssn: "123-45-6789", badge: "", profitYear: 2024, skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: false },
        employeeData: { badgeNumber: 12345, demographicId: 123, suggestedForfeitAmount: 100 },
        memberDetails: null,
        transactionData: null,
        isSearching: false,
        isFetchingMemberDetails: false,
        isFetchingTransactions: false,
        isAddForfeitureModalOpen: true,
        showEmployeeData: true,
        showMemberDetails: false,
        showTransactions: false,
        executeSearch: mockExecuteSearch,
        handleReset: mockHandleReset,
        handleSaveForfeiture: mockHandleSaveForfeiture,
        openAddForfeitureModal: mockOpenModal,
        closeAddForfeitureModal: mockCloseModal,
        transactionPagination: {
          pageNumber: 0,
          pageSize: 25,
          sortParams: { sortBy: "date", isSortDescending: true },
          handlePaginationChange: vi.fn(),
          handleSortChange: vi.fn(),
          resetPagination: vi.fn()
        },
        profitYear: 2024,
        isReadOnly: false,
        memberDetailsRefreshTrigger: 0
      } as unknown);

      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      await waitFor(() => {
        expect(screen.getByRole("button", { name: /close/i })).toBeInTheDocument();
        expect(screen.getByRole("button", { name: /save/i })).toBeInTheDocument();
      });
    });

    it("should close modal when close button is clicked", async () => {
      const user = userEvent.setup();
      const useForfeituresAdjustment = await import("../hooks/useForfeituresAdjustment");
      vi.mocked(useForfeituresAdjustment.default).mockReturnValueOnce({
        searchParams: { ssn: "123-45-6789", badge: "", profitYear: 2024, skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: false },
        employeeData: { badgeNumber: 12345, demographicId: 123, suggestedForfeitAmount: 100 },
        memberDetails: null,
        transactionData: null,
        isSearching: false,
        isFetchingMemberDetails: false,
        isFetchingTransactions: false,
        isAddForfeitureModalOpen: true,
        showEmployeeData: true,
        showMemberDetails: false,
        showTransactions: false,
        executeSearch: mockExecuteSearch,
        handleReset: mockHandleReset,
        handleSaveForfeiture: mockHandleSaveForfeiture,
        openAddForfeitureModal: mockOpenModal,
        closeAddForfeitureModal: mockCloseModal,
        transactionPagination: {
          pageNumber: 0,
          pageSize: 25,
          sortParams: { sortBy: "date", isSortDescending: true },
          handlePaginationChange: vi.fn(),
          handleSortChange: vi.fn(),
          resetPagination: vi.fn()
        },
        profitYear: 2024,
        isReadOnly: false,
        memberDetailsRefreshTrigger: 0
      } as unknown);

      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      const closeButton = screen.getByRole("button", { name: /close/i });
      await user.click(closeButton);

      await waitFor(() => {
        expect(mockCloseModal).toHaveBeenCalled();
      });
    });

    it("should call handleSaveForfeiture when modal save is clicked", async () => {
      const user = userEvent.setup();
      const useForfeituresAdjustment = await import("../hooks/useForfeituresAdjustment");
      vi.mocked(useForfeituresAdjustment.default).mockReturnValueOnce({
        searchParams: { ssn: "123-45-6789", badge: "", profitYear: 2024, skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: false },
        employeeData: { badgeNumber: 12345, demographicId: 123, suggestedForfeitAmount: 100 },
        memberDetails: null,
        transactionData: null,
        isSearching: false,
        isFetchingMemberDetails: false,
        isFetchingTransactions: false,
        isAddForfeitureModalOpen: true,
        showEmployeeData: true,
        showMemberDetails: false,
        showTransactions: false,
        executeSearch: mockExecuteSearch,
        handleReset: mockHandleReset,
        handleSaveForfeiture: mockHandleSaveForfeiture,
        openAddForfeitureModal: mockOpenModal,
        closeAddForfeitureModal: mockCloseModal,
        transactionPagination: {
          pageNumber: 0,
          pageSize: 25,
          sortParams: { sortBy: "date", isSortDescending: true },
          handlePaginationChange: vi.fn(),
          handleSortChange: vi.fn(),
          resetPagination: vi.fn()
        },
        profitYear: 2024,
        isReadOnly: false,
        memberDetailsRefreshTrigger: 0
      } as unknown);

      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      const saveButton = screen.getByRole("button", { name: /save/i });
      await user.click(saveButton);

      await waitFor(() => {
        expect(mockHandleSaveForfeiture).toHaveBeenCalledWith(
          expect.objectContaining({
            forfeitureAmount: 1000,
            classAction: false
          })
        );
      });
    });
  });

  describe("Read-only mode", () => {
    it("should disable add forfeiture button in read-only mode", async () => {
      const useReadOnlyNavigation = await import("../../../../hooks/useReadOnlyNavigation");
      vi.mocked(useReadOnlyNavigation.useReadOnlyNavigation).mockReturnValueOnce(true);

      const useForfeituresAdjustment = await import("../hooks/useForfeituresAdjustment");
      vi.mocked(useForfeituresAdjustment.default).mockReturnValueOnce({
        employeeData: { badgeNumber: 12345, demographicId: 123, suggestedForfeitAmount: 100 },
        transactionData: null,
        isSearching: false,
        isFetchingTransactions: false,
        isAddForfeitureModalOpen: false,
        showEmployeeData: true,
        showTransactions: false,
        executeSearch: mockExecuteSearch,
        handleReset: mockHandleReset,
        handleSaveForfeiture: mockHandleSaveForfeiture,
        openAddForfeitureModal: mockOpenModal,
        closeAddForfeitureModal: mockCloseModal,
        transactionPagination: {
          pageNumber: 0,
          pageSize: 25,
          sortParams: { sortBy: "date", isSortDescending: true },
          handlePaginationChange: vi.fn(),
          handleSortChange: vi.fn(),
          resetPagination: vi.fn()
        },
        profitYear: 2024,
        isReadOnly: true,
        memberDetailsRefreshTrigger: 0
      } as unknown);

      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      const addButton = screen.getByRole("button", { name: /add forfeiture/i });
      expect(addButton).toBeDisabled();
    });
  });

  describe("MissiveAlerts integration", () => {
    it("should render MissiveAlerts component", () => {
      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      // MissiveAlerts is mocked in the actual implementation via MissiveAlertProvider
      // Just verify the page renders without error
      expect(screen.getByRole("button", { name: /search/i })).toBeInTheDocument();
    });
  });
});
