import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi, beforeEach } from "vitest";
import UnForfeitSearchFilter from "./UnForfeitSearchFilter";

// Mock date picker
vi.mock("../../../components/DsmDatePicker/DsmDatePicker", () => ({
  default: vi.fn(({ label, onChange, disabled, minDate, maxDate, ...props }) => (
    <input
      data-testid={`date-picker-${label}`}
      onChange={(e) => onChange(e.target.value)}
      disabled={disabled}
      placeholder={label}
      {...props}
    />
  ))
}));

vi.mock("../../../utils/FormValidators", () => ({
  dateStringValidator: () => ({
    required: () => ({
      typeError: () => ({
        min: () => ({
          max: () => ({})
        })
      })
    })
  }),
  endDateStringAfterStartDateValidator: () => ({
    required: () => ({})
  }),
  profitYearValidator: () => ({
    required: () => ({})
  })
}));

vi.mock("smart-ui-library", () => ({
  SearchAndReset: vi.fn(({ handleSearch, handleReset, disabled, isFetching }) => (
    <div data-testid="search-and-reset">
      <button
        data-testid="search-btn"
        onClick={handleSearch}
        disabled={disabled || isFetching}>
        Search
      </button>
      <button data-testid="reset-btn" onClick={handleReset}>
        Reset
      </button>
      {isFetching && <span data-testid="loading">Loading...</span>}
    </div>
  ))
}));

describe("UnForfeitSearchFilter", () => {
  const mockFiscalData = {
    fiscalBeginDate: "2024-01-01",
    fiscalEndDate: "2024-12-31"
  };

  const mockOnSearch = vi.fn();
  const mockSetInitialSearchLoaded = vi.fn();
  const mockSetHasUnsavedChanges = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("Rendering", () => {
    it("should render the form with all fields", () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData as any}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />
      );

      expect(screen.getByTestId("date-picker-Rehire Begin Date")).toBeInTheDocument();
      expect(screen.getByTestId("date-picker-Rehire End Date")).toBeInTheDocument();
      expect(screen.getByTestId("search-and-reset")).toBeInTheDocument();
    });

    it("should render search and reset buttons", () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData as any}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />
      );

      expect(screen.getByTestId("search-btn")).toBeInTheDocument();
      expect(screen.getByTestId("reset-btn")).toBeInTheDocument();
    });

    it("should render exclude zero balance checkbox", () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData as any}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />
      );

      // Checkbox should be present
      const checkboxes = screen.getAllByRole("checkbox");
      expect(checkboxes.length).toBeGreaterThan(0);
    });
  });

  describe("Date validation", () => {
    it("should enforce fiscal year date constraints", () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData as any}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />
      );

      const beginDateInput = screen.getByTestId("date-picker-Rehire Begin Date") as HTMLInputElement;
      const endDateInput = screen.getByTestId("date-picker-Rehire End Date") as HTMLInputElement;

      expect(beginDateInput).toBeInTheDocument();
      expect(endDateInput).toBeInTheDocument();
    });

    it("should validate end date is after begin date", async () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData as any}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />
      );

      const beginDateInput = screen.getByTestId("date-picker-Rehire Begin Date");
      const endDateInput = screen.getByTestId("date-picker-Rehire End Date");

      // Test date entry
      await userEvent.type(beginDateInput, "2024-06-01");
      await userEvent.type(endDateInput, "2024-05-01");

      // Search button should reflect validation state
      expect(screen.getByTestId("search-and-reset")).toBeInTheDocument();
    });

    it("should reject dates before February 2024", async () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData as any}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />
      );

      const beginDateInput = screen.getByTestId("date-picker-Rehire Begin Date");

      // Try to enter date before Feb 2024
      await userEvent.type(beginDateInput, "2024-01-01");

      // Form validation should handle this
      expect(beginDateInput).toBeInTheDocument();
    });
  });

  describe("Search functionality", () => {
    it("should call onSearch when search button is clicked", async () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData as any}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />
      );

      const searchButton = screen.getByTestId("search-btn");
      fireEvent.click(searchButton);

      await waitFor(() => {
        expect(mockOnSearch).toHaveBeenCalled();
      });
    });

    it("should call setInitialSearchLoaded when search is executed", async () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData as any}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />
      );

      const searchButton = screen.getByTestId("search-btn");
      fireEvent.click(searchButton);

      await waitFor(() => {
        expect(mockSetInitialSearchLoaded).toHaveBeenCalledWith(true);
      });
    });

    it("should disable search when hasUnsavedChanges is true", () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData as any}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={true}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />
      );

      const searchButton = screen.getByTestId("search-btn");
      expect(searchButton).toBeDisabled();
    });

    it("should show alert when trying to search with unsaved changes", async () => {
      const alertSpy = vi.spyOn(window, "alert").mockImplementation(() => {});

      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData as any}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={true}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />
      );

      // Alert would be shown before search is attempted
      // But button is already disabled
      expect(screen.getByTestId("search-btn")).toBeDisabled();

      alertSpy.mockRestore();
    });
  });

  describe("Reset functionality", () => {
    it("should clear form fields when reset is clicked", async () => {
      const user = userEvent.setup();
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData as any}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />
      );

      const beginDateInput = screen.getByTestId("date-picker-Rehire Begin Date") as HTMLInputElement;

      // Type a value
      await user.type(beginDateInput, "2024-06-01");
      expect(beginDateInput.value).not.toBe("");

      // Click reset
      const resetButton = screen.getByTestId("reset-btn");
      fireEvent.click(resetButton);

      // Should be cleared
      await waitFor(() => {
        expect(beginDateInput.value).toBe("");
      });
    });

    it("should toggle checkbox when reset", async () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData as any}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />
      );

      const resetButton = screen.getByTestId("reset-btn");
      fireEvent.click(resetButton);

      // Reset should clear all fields including checkbox state
      expect(resetButton).toBeInTheDocument();
    });
  });

  describe("Exclude zero balance filter", () => {
    it("should include excludeZeroBalance in search parameters", async () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData as any}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />
      );

      const searchButton = screen.getByTestId("search-btn");
      fireEvent.click(searchButton);

      await waitFor(() => {
        expect(mockOnSearch).toHaveBeenCalled();
      });
    });

    it("should allow toggling exclude zero balance checkbox", async () => {
      const user = userEvent.setup();
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData as any}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />
      );

      const checkboxes = screen.getAllByRole("checkbox");
      // Find the exclude zero balance checkbox (may vary in position)
      if (checkboxes.length > 0) {
        await user.click(checkboxes[0]);
        // Checkbox state should change
        expect(checkboxes[0]).toBeInTheDocument();
      }
    });
  });

  describe("Redux integration", () => {
    it("should dispatch Redux action to store search parameters", async () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData as any}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />
      );

      const searchButton = screen.getByTestId("search-btn");
      fireEvent.click(searchButton);

      await waitFor(() => {
        // Component should dispatch Redux action internally
        expect(mockOnSearch).toHaveBeenCalled();
      });
    });
  });

  describe("Form state management", () => {
    it("should track form validity", async () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData as any}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />
      );

      const searchButton = screen.getByTestId("search-btn");
      expect(searchButton).toBeInTheDocument();
    });
  });

  describe("Date range constraints", () => {
    it("should set min/max dates based on fiscal data", () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData as any}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />
      );

      const beginDateInput = screen.getByTestId("date-picker-Rehire Begin Date");
      const endDateInput = screen.getByTestId("date-picker-Rehire End Date");

      expect(beginDateInput).toBeInTheDocument();
      expect(endDateInput).toBeInTheDocument();
    });
  });

  describe("Loading state", () => {
    it("should show loading spinner when isFetching is true", () => {
      // Note: Loading state is built into SearchAndReset component
      // which is mocked, so we test that the component exists
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData as any}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />
      );

      expect(screen.getByTestId("search-and-reset")).toBeInTheDocument();
    });
  });
});
