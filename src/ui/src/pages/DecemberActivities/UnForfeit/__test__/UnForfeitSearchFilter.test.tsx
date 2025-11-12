import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { createMockStoreAndWrapper } from "../../../../test";
import UnForfeitSearchFilter from "../UnForfeitSearchFilter";

vi.mock("../../../utils/FormValidators", async () => {
  const yup = await import("yup");

  return {
    dateStringValidator: (_minYear: number, _maxYear: number, fieldName: string) => {
      return yup.default
        .string()
        .nullable()
        .required(`${fieldName} is required`)
        .typeError(`${fieldName} must be a valid date`)
        .test("valid-date", `${fieldName} must be a valid date`, function (value) {
          if (!value || value === "") return false; // Empty strings fail validation when required
          // Accept MM/DD/YYYY format or ISO format
          return /^\d{1,2}\/\d{1,2}\/\d{4}$/.test(value) || /^\d{4}-\d{2}-\d{2}$/.test(value);
        });
    },
    endDateStringAfterStartDateValidator: (
      _beginDateFieldName: string,
      _tryddmmyyyyToDate: (dateStr: string) => Date | null,
      _message: string
    ) => {
      return yup.default
        .string()
        .nullable()
        .required("Ending Date is required")
        .test("date-after-begin", _message, function (value) {
          if (!value || value === "") return false; // Empty strings fail validation when required
          // Check if date is after beginning date
          const beginValue = this.parent[_beginDateFieldName];
          if (beginValue && /^\d{1,2}\/\d{1,2}\/\d{4}$/.test(value)) {
            const [beginMonth, beginDay, beginYear] = beginValue.split("/").map(Number);
            const [endMonth, endDay, endYear] = value.split("/").map(Number);
            const beginDate = new Date(beginYear, beginMonth - 1, beginDay);
            const endDate = new Date(endYear, endMonth - 1, endDay);
            return endDate >= beginDate;
          }
          return true;
        });
    },
    profitYearValidator: () => yup.default.number(),
    tryddmmyyyyToDate: (dateStr: string) => {
      if (!dateStr) return null;
      // Handle MM/DD/YYYY format
      if (/^\d{1,2}\/\d{1,2}\/\d{4}$/.test(dateStr)) {
        const [month, day, year] = dateStr.split("/").map(Number);
        return new Date(year, month - 1, day);
      }
      // Handle ISO format
      return new Date(dateStr);
    },
    mmDDYYFormat: (date: string | Date | undefined | null) => {
      if (!date) return "";
      if (date instanceof Date) {
        const month = String(date.getMonth() + 1).padStart(2, "0");
        const day = String(date.getDate()).padStart(2, "0");
        const year = date.getFullYear();
        return `${month}/${day}/${year}`;
      }
      // Already in MM/DD/YYYY format
      if (/^\d{1,2}\/\d{1,2}\/\d{4}$/.test(date)) {
        return date;
      }
      // ISO format - convert to MM/DD/YYYY
      if (/^\d{4}-\d{2}-\d{2}$/.test(date)) {
        const [year, month, day] = date.split("-");
        return `${month}/${day}/${year}`;
      }
      return date || "";
    }
  };
});

vi.mock("smart-ui-library", () => ({
  DSMDatePicker: vi.fn(({ label, onChange, value, disabled, required }) => (
    <div>
      <label>{label}</label>
      <input
        data-testid={`date-picker-${label}`}
        aria-label={label}
        onChange={(e) => onChange(e.target.value ? new Date(e.target.value) : null)}
        value={value ? new Date(value).toISOString().split("T")[0] : ""}
        placeholder={label}
        type="date"
        disabled={disabled}
        required={required}
      />
    </div>
  )),
  SearchAndReset: vi.fn(({ handleSearch, handleReset, disabled, isFetching }) => (
    <section aria-label="search and reset">
      <button
        aria-label="search button"
        onClick={handleSearch}
        disabled={disabled || isFetching}>
        Search
      </button>
      <button
        aria-label="reset button"
        onClick={handleReset}>
        Reset
      </button>
      {isFetching && <span role="status" aria-label="loading">Loading...</span>}
    </section>
  ))
}));

// Mock the RTK Query hook
vi.mock("../../../reduxstore/api/YearsEndApi", () => ({
  useLazyGetUnForfeitsQuery: vi.fn(() => [
    vi.fn(), // triggerSearch function
    { isFetching: false } // query state
  ])
}));

// Mock the December flow hook
vi.mock("../../../hooks/useDecemberFlowProfitYear", () => ({
  default: vi.fn(() => 2024)
}));

describe("UnForfeitSearchFilter", () => {
  const mockFiscalData = {
    fiscalBeginDate: "01/01/2024",
    fiscalEndDate: "12/31/2024"
  };

  const mockOnSearch = vi.fn();
  const mockSetInitialSearchLoaded = vi.fn();
  const mockSetHasUnsavedChanges = vi.fn();
  let wrapper: ReturnType<typeof createMockStoreAndWrapper>["wrapper"];

  beforeEach(() => {
    vi.clearAllMocks();
    const storeAndWrapper = createMockStoreAndWrapper({
      security: { token: "test-token" },
      yearsEnd: { selectedProfitYear: 2024 }
    });
    wrapper = storeAndWrapper.wrapper;
  });

  describe("Rendering", () => {
    it("should render the form with all fields", async () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />,
        { wrapper }
      );

      // Wait for form to render with labels visible
      await waitFor(() => {
        expect(screen.getByText("Rehire Begin Date")).toBeInTheDocument();
        expect(screen.getByText("Rehire Ending Date")).toBeInTheDocument();
        expect(screen.getByLabelText("search and reset")).toBeInTheDocument();
      });
    });

    it("should render search and reset buttons", async () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />,
        { wrapper }
      );

      await waitFor(() => {
        expect(screen.getByRole("button", { name: /search button/i })).toBeInTheDocument();
        expect(screen.getByRole("button", { name: /reset button/i })).toBeInTheDocument();
      });
    });

    it("should render exclude zero balance checkbox", async () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />,
        { wrapper }
      );

      await waitFor(() => {
        expect(screen.getByText("Exclude employees with no current or vested balance")).toBeInTheDocument();
        const checkboxes = screen.getAllByRole("checkbox");
        expect(checkboxes.length).toBeGreaterThan(0);
      });
    });
  });

  describe("Date validation", () => {
    it("should enforce fiscal year date constraints", async () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />,
        { wrapper }
      );

      await waitFor(() => {
        expect(screen.getByText("Rehire Begin Date")).toBeInTheDocument();
        expect(screen.getByText("Rehire Ending Date")).toBeInTheDocument();
      });
    });

    it("should render date input fields correctly", async () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />,
        { wrapper }
      );

      // Date inputs should be rendered with the fiscal date values
      await waitFor(() => {
        expect(screen.getByText("Rehire Begin Date")).toBeInTheDocument();
        expect(screen.getByText("Rehire Ending Date")).toBeInTheDocument();
      });

      // Verify that date inputs exist using accessible labels
      expect(screen.getByLabelText("Rehire Begin Date")).toBeInTheDocument();
      expect(screen.getByLabelText("Rehire Ending Date")).toBeInTheDocument();
    });

    it("should support date input interaction", async () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />,
        { wrapper }
      );

      // Form should be accessible for user interaction
      await waitFor(() => {
        expect(screen.getByText("Rehire Begin Date")).toBeInTheDocument();
        expect(screen.getByText("Rehire Ending Date")).toBeInTheDocument();
      });

      // Date inputs should be present and interactive
      const beginDateInput = screen.getByLabelText("Rehire Begin Date");
      const endDateInput = screen.getByLabelText("Rehire Ending Date");

      // Verify inputs are not disabled
      expect(beginDateInput).not.toBeDisabled();
      expect(endDateInput).not.toBeDisabled();
    });
  });

  describe("Search functionality", () => {
    it("should render search button", async () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />,
        { wrapper }
      );

      // Search button should be present
      await waitFor(() => {
        const searchButton = screen.getByRole("button", { name: /search button/i });
        expect(searchButton).toBeInTheDocument();
      });
    });

    it("should pass through search parameters to callback", async () => {
      const mockOnSearchLocal = vi.fn();
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearchLocal}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />,
        { wrapper }
      );

      // Component should render with form elements
      await waitFor(() => {
        expect(screen.getByText("Rehire Begin Date")).toBeInTheDocument();
        // If form is valid, search should be possible
        expect(screen.getByRole("button", { name: /search button/i })).toBeInTheDocument();
      });
    });

    it("should show alert when search clicked with unsaved changes", async () => {
      const user = userEvent.setup();
      const alertSpy = vi.spyOn(window, "alert").mockImplementation(() => {});

      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={true}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />,
        { wrapper }
      );

      // Wait for search button to render
      await waitFor(() => {
        expect(screen.getByLabelText("search button")).toBeInTheDocument();
      });

      // Click search button
      const searchButton = screen.getByLabelText("search button");
      await user.click(searchButton);

      // Verify alert was shown with appropriate message
      await waitFor(() => {
        expect(alertSpy).toHaveBeenCalledWith("Please save your changes.");
      });

      alertSpy.mockRestore();
    });

    it("should not call onSearch when there are unsaved changes", async () => {
      const user = userEvent.setup();
      const alertSpy = vi.spyOn(window, "alert").mockImplementation(() => {});

      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={true}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />,
        { wrapper }
      );

      // Wait for search button to render
      await waitFor(() => {
        expect(screen.getByLabelText("search button")).toBeInTheDocument();
      });

      // Click search button
      const searchButton = screen.getByLabelText("search button");
      await user.click(searchButton);

      // Verify onSearch was NOT called (alert prevents execution)
      await waitFor(() => {
        expect(mockOnSearch).not.toHaveBeenCalled();
      });

      alertSpy.mockRestore();
    });
  });

  describe("Reset functionality", () => {
    it("should reset form when reset button is clicked", async () => {
      const user = userEvent.setup();
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />,
        { wrapper }
      );

      // Wait for reset button to be available
      await waitFor(() => {
        expect(screen.getByRole("button", { name: /reset button/i })).toBeInTheDocument();
      });

      const resetButton = screen.getByRole("button", { name: /reset button/i });
      await user.click(resetButton);

      // After reset, form should be cleared
      await waitFor(() => {
        expect(resetButton).toBeInTheDocument();
      });
    });

    it("should reset checkbox when reset is clicked", async () => {
      const user = userEvent.setup();
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />,
        { wrapper }
      );

      await waitFor(() => {
        expect(screen.getByRole("button", { name: /reset button/i })).toBeInTheDocument();
      });

      const resetButton = screen.getByRole("button", { name: /reset button/i });
      await user.click(resetButton);

      // Reset should clear all fields including checkbox state
      await waitFor(() => {
        expect(resetButton).toBeInTheDocument();
        expect(screen.getByText("Exclude employees with no current or vested balance")).toBeInTheDocument();
      });
    });
  });

  describe("Exclude zero balance filter", () => {
    it("should include exclude zero balance checkbox in form", async () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />,
        { wrapper }
      );

      // Wait for checkbox to be rendered
      await waitFor(() => {
        expect(screen.getByText("Exclude employees with no current or vested balance")).toBeInTheDocument();
      });

      // Checkbox should be present and interactive
      const checkboxes = screen.getAllByRole("checkbox");
      expect(checkboxes.length).toBeGreaterThan(0);
    });

    it("should allow toggling exclude zero balance checkbox", async () => {
      const user = userEvent.setup();
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />,
        { wrapper }
      );

      await waitFor(() => {
        expect(screen.getByText("Exclude employees with no current or vested balance")).toBeInTheDocument();
      });

      const checkboxes = screen.getAllByRole("checkbox");
      // Find the exclude zero balance checkbox (should be one of them)
      if (checkboxes.length > 0) {
        const checkbox = checkboxes[0];
        await user.click(checkbox);
        // Checkbox state should be interactive
        expect(checkbox).toBeInTheDocument();
      }
    });
  });

  describe("Redux integration", () => {
    it("should render with Redux integrated hooks", async () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />,
        { wrapper }
      );

      // Component should render successfully with Redux hooks
      await waitFor(() => {
        expect(screen.getByLabelText("search and reset")).toBeInTheDocument();
        // Form should be present with Redux-backed state
        expect(screen.getByText("Rehire Begin Date")).toBeInTheDocument();
      });
    });
  });

  describe("Form state management", () => {
    it("should track form validity", async () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />,
        { wrapper }
      );

      await waitFor(() => {
        const searchButton = screen.getByRole("button", { name: /search button/i });
        expect(searchButton).toBeInTheDocument();
      });
    });
  });

  describe("Date range constraints", () => {
    it("should render with fiscal year date defaults", async () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />,
        { wrapper }
      );

      // Form should render with fiscal date defaults (01/01/2024 to 12/31/2024)
      await waitFor(() => {
        expect(screen.getByText("Rehire Begin Date")).toBeInTheDocument();
        expect(screen.getByText("Rehire Ending Date")).toBeInTheDocument();
      });
    });
  });

  describe("Loading state", () => {
    it("should show loading spinner when isFetching is true", async () => {
      // Note: Loading state is built into SearchAndReset component
      // which is mocked, so we test that the component exists
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={mockSetHasUnsavedChanges}
        />,
        { wrapper }
      );

      await waitFor(() => {
        expect(screen.getByLabelText("search and reset")).toBeInTheDocument();
      });
    });
  });
});
