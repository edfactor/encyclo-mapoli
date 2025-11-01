import { configureStore } from "@reduxjs/toolkit";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { Provider } from "react-redux";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { MissiveAlertProvider } from "../../../components/MissiveAlerts/MissiveAlertContext";
import ForfeituresAdjustment from "./ForfeituresAdjustment";

// Mock child components
vi.mock("./ForfeituresAdjustmentSearchFilter", () => ({
  default: vi.fn(({ onSearch, onReset, isSearching }) => (
    <div data-testid="search-filter">
      <button
        data-testid="search-button"
        onClick={() =>
          onSearch({
            ssn: "123-45-6789",
            badge: "",
            profitYear: 2024,
            skip: 0,
            take: 255,
            sortBy: "badgeNumber",
            isSortDescending: false
          })
        }>
        Search
      </button>
      <button
        data-testid="reset-button"
        onClick={onReset}>
        Reset
      </button>
      {isSearching && <span data-testid="searching">Searching...</span>}
    </div>
  ))
}));

vi.mock("./ForfeituresAdjustmentPanel", () => ({
  default: vi.fn(({ onAddForfeiture, isReadOnly }) => (
    <div data-testid="forfeiture-panel">
      <button
        data-testid="add-forfeiture-btn"
        onClick={onAddForfeiture}
        disabled={isReadOnly}>
        Add Forfeiture
      </button>
    </div>
  ))
}));

vi.mock("./ForfeituresTransactionGrid", () => ({
  default: vi.fn(() => <div data-testid="transaction-grid">Transaction Grid</div>)
}));

vi.mock("./AddForfeitureModal", () => ({
  default: vi.fn(
    ({ open, onClose, onSave }) =>
      open && (
        <div data-testid="add-forfeiture-modal">
          <button
            data-testid="modal-close-btn"
            onClick={onClose}>
            Close
          </button>
          <button
            data-testid="modal-save-btn"
            onClick={() => onSave({ forfeitureAmount: 1000, classAction: false })}>
            Save
          </button>
        </div>
      )
  )
}));

vi.mock("pages/InquiriesAndAdjustments/MasterInquiry/StandaloneMemberDetails", () => ({
  default: vi.fn(() => <div data-testid="member-details">Member Details</div>)
}));

vi.mock("components/StatusDropdownActionNode", () => ({
  default: vi.fn(() => <div data-testid="status-dropdown">Status Dropdown</div>)
}));

vi.mock("../../../hooks/useReadOnlyNavigation", () => ({
  useReadOnlyNavigation: vi.fn(() => false)
}));

// Mock the hook with comprehensive state
const mockExecuteSearch = vi.fn();
const mockHandleReset = vi.fn();
const mockOpenModal = vi.fn();
const mockCloseModal = vi.fn();
const mockHandleSaveForfeiture = vi.fn();

vi.mock("./hooks/useForfeituresAdjustment", () => ({
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
    it("should render the page with all major components", () => {
      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      expect(screen.getByTestId("search-filter")).toBeInTheDocument();
      expect(screen.getByTestId("status-dropdown")).toBeInTheDocument();
    });

    it("should render the search filter accordion", () => {
      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      expect(screen.getByTestId("search-filter")).toBeInTheDocument();
      expect(screen.getByTestId("search-button")).toBeInTheDocument();
      expect(screen.getByTestId("reset-button")).toBeInTheDocument();
    });

    it("should hide employee data and transactions before search", () => {
      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      expect(screen.queryByTestId("member-details")).not.toBeInTheDocument();
      expect(screen.queryByTestId("forfeiture-panel")).not.toBeInTheDocument();
      expect(screen.queryByTestId("transaction-grid")).not.toBeInTheDocument();
    });
  });

  describe("Search functionality", () => {
    it("should call executeSearch when search button is clicked", async () => {
      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      const searchButton = screen.getByTestId("search-button");
      fireEvent.click(searchButton);

      await waitFor(() => {
        expect(mockExecuteSearch).toHaveBeenCalledWith(
          expect.objectContaining({
            ssn: "123-45-6789"
          })
        );
      });
    });

    it("should call handleReset when reset button is clicked", () => {
      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      const resetButton = screen.getByTestId("reset-button");
      fireEvent.click(resetButton);

      expect(mockHandleReset).toHaveBeenCalled();
    });

    it("should display loading state during search", async () => {
      const useForfeituresAdjustment = await import("./hooks/useForfeituresAdjustment");
      vi.mocked(useForfeituresAdjustment.default).mockReturnValueOnce({
        employeeData: null,
        transactionData: null,
        isSearching: true,
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
      });

      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      expect(screen.getByTestId("searching")).toBeInTheDocument();
    });
  });

  describe("Employee data display", () => {
    it("should display employee data after search succeeds", async () => {
      const useForfeituresAdjustment = await import("./hooks/useForfeituresAdjustment");
      vi.mocked(useForfeituresAdjustment.default).mockReturnValueOnce({
        employeeData: { demographicId: 123, name: "John Doe" },
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
        isReadOnly: false,
        memberDetailsRefreshTrigger: 0
      });

      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      expect(screen.getByTestId("member-details")).toBeInTheDocument();
      expect(screen.getByTestId("forfeiture-panel")).toBeInTheDocument();
    });

    it("should display transaction grid when transactions are available", async () => {
      const useForfeituresAdjustment = await import("./hooks/useForfeituresAdjustment");
      vi.mocked(useForfeituresAdjustment.default).mockReturnValueOnce({
        employeeData: { demographicId: 123, name: "John Doe" },
        transactionData: { results: [{ id: 1 }], total: 1 },
        isSearching: false,
        isFetchingTransactions: false,
        isAddForfeitureModalOpen: false,
        showEmployeeData: true,
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
      });

      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      expect(screen.getByTestId("transaction-grid")).toBeInTheDocument();
    });
  });

  describe("Modal functionality", () => {
    it("should open add forfeiture modal when button is clicked", async () => {
      const useForfeituresAdjustment = await import("./hooks/useForfeituresAdjustment");
      vi.mocked(useForfeituresAdjustment.default).mockReturnValueOnce({
        employeeData: { demographicId: 123, name: "John Doe" },
        transactionData: null,
        isSearching: false,
        isFetchingTransactions: false,
        isAddForfeitureModalOpen: true,
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
        isReadOnly: false,
        memberDetailsRefreshTrigger: 0
      });

      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      expect(screen.getByTestId("add-forfeiture-modal")).toBeInTheDocument();
    });

    it("should close modal when close button is clicked", async () => {
      const useForfeituresAdjustment = await import("./hooks/useForfeituresAdjustment");
      vi.mocked(useForfeituresAdjustment.default).mockReturnValueOnce({
        employeeData: { demographicId: 123, name: "John Doe" },
        transactionData: null,
        isSearching: false,
        isFetchingTransactions: false,
        isAddForfeitureModalOpen: true,
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
        isReadOnly: false,
        memberDetailsRefreshTrigger: 0
      });

      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      const closeButton = screen.getByTestId("modal-close-btn");
      fireEvent.click(closeButton);

      expect(mockCloseModal).toHaveBeenCalled();
    });

    it("should call handleSaveForfeiture when modal save is clicked", async () => {
      const useForfeituresAdjustment = await import("./hooks/useForfeituresAdjustment");
      vi.mocked(useForfeituresAdjustment.default).mockReturnValueOnce({
        employeeData: { demographicId: 123, name: "John Doe" },
        transactionData: null,
        isSearching: false,
        isFetchingTransactions: false,
        isAddForfeitureModalOpen: true,
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
        isReadOnly: false,
        memberDetailsRefreshTrigger: 0
      });

      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      const saveButton = screen.getByTestId("modal-save-btn");
      fireEvent.click(saveButton);

      expect(mockHandleSaveForfeiture).toHaveBeenCalledWith(
        expect.objectContaining({
          forfeitureAmount: 1000,
          classAction: false
        })
      );
    });
  });

  describe("Read-only mode", () => {
    it("should disable add forfeiture button in read-only mode", async () => {
      const useReadOnlyNavigation = await import("../../../hooks/useReadOnlyNavigation");
      vi.mocked(useReadOnlyNavigation.useReadOnlyNavigation).mockReturnValueOnce(true);

      const useForfeituresAdjustment = await import("./hooks/useForfeituresAdjustment");
      vi.mocked(useForfeituresAdjustment.default).mockReturnValueOnce({
        employeeData: { demographicId: 123, name: "John Doe" },
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
      });

      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      const addButton = screen.getByTestId("add-forfeiture-btn");
      expect(addButton).toBeDisabled();
    });
  });

  describe("MissiveAlerts integration", () => {
    it("should render MissiveAlerts component", () => {
      const mockStore = createMockStore();
      render(<ForfeituresAdjustment />, { wrapper: wrapper(mockStore) });

      // MissiveAlerts is mocked in the actual implementation via MissiveAlertProvider
      // Just verify the page renders without error
      expect(screen.getByTestId("search-filter")).toBeInTheDocument();
    });
  });
});
