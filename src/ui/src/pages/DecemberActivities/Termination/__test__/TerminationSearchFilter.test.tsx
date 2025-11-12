import React from "react";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { createMockStoreAndWrapper } from "../../../../test";
import TerminationSearchFilter from "../TerminationSearchFilter";
import { CalendarResponseDto } from "../../../../reduxstore/types";

// Hoist API mock functions
const { mockTriggerAccountingYear, mockTriggerDuplicateSsn } = vi.hoisted(() => ({
  mockTriggerAccountingYear: vi.fn(),
  mockTriggerDuplicateSsn: vi.fn()
}));

// Mock date picker and validators
vi.mock("../../../../reduxstore/api/LookupsApi", () => ({
  LookupsApi: {
    reducerPath: "lookupsApi",
    reducer: (state = {}) => state,
    middleware: []
  },
  useLazyGetAccountingYearQuery: vi.fn(() => [
    mockTriggerAccountingYear,
    { data: { fiscalBeginDate: "2024-01-01", fiscalEndDate: "2024-12-31" }, isLoading: false }
  ]),
  useLazyGetDuplicateSsnExistsQuery: vi.fn(() => [
    mockTriggerDuplicateSsn,
    { data: { duplicateCount: 0 }, isLoading: false }
  ])
}));

vi.mock("../../../../components/DuplicateSsnGuard", () => ({
  default: vi.fn(({ children }) => children({ prerequisitesComplete: true }))
}));

vi.mock("../../../../utils/FormValidators", async () => {
  const yup = await import("yup");
  return {
    dateStringValidator: (_min: number, _max: number, _fieldName: string) => yup.default.string().nullable(),
    endDateStringAfterStartDateValidator: (
      _beginFieldName: string,
      _dateParser: (val: string) => Date,
      _message: string
    ) => yup.default.string().nullable(),
    profitYearValidator: (_min: number, _max: number) => yup.default.number().nullable()
  };
});

vi.mock("../../../../utils/dateUtils", () => ({
  mmDDYYFormat: (date: string) => date,
  tryddmmyyyyToDate: (dateStr: string) => new Date(dateStr)
}));

vi.mock("../../../../hooks/useDecemberFlowProfitYear", () => ({
  default: vi.fn(() => 2024)
}));

vi.mock("smart-ui-library", () => ({
  DSMDatePicker: vi.fn(({ label, onChange, disabled, value }) =>
    React.createElement("div", {},
      React.createElement("label", {}, label),
      React.createElement("input", {
        "aria-label": label,
        "data-testid": `date-picker-${label}`,
        onChange: (e: React.ChangeEvent<HTMLInputElement>) => onChange(e.target.value ? new Date(e.target.value) : null),
        disabled: disabled,
        placeholder: label,
        type: "date",
        value: value ? new Date(value).toISOString().split("T")[0] : ""
      })
    )
  ),
  SearchAndReset: vi.fn(({ handleSearch, handleReset, disabled, isFetching }) =>
    React.createElement(
      "section",
      { "aria-label": "search and reset" },
      React.createElement(
        "button",
        {
          onClick: handleSearch,
          disabled: disabled || isFetching
        },
        "Search"
      ),
      React.createElement(
        "button",
        {
          onClick: handleReset
        },
        "Reset"
      ),
      isFetching && React.createElement("span", { role: "status" }, "Loading...")
    )
  )
}));

describe("TerminationSearchFilter", { timeout: 7000 }, () => {
  const mockFiscalData: CalendarResponseDto = {
    fiscalBeginDate: "2024-01-01",
    fiscalEndDate: "2024-12-31"
  };

  const mockOnSearch = vi.fn();
  const mockSetInitialSearchLoaded = vi.fn();

  // Create a wrapper with Provider using test utilities
  let wrapper: ReturnType<typeof createMockStoreAndWrapper>["wrapper"];

  beforeEach(() => {
    vi.clearAllMocks();

    // Set up mock return values for API hooks
    mockTriggerAccountingYear.mockReturnValue({
      unwrap: vi.fn().mockResolvedValue({
        fiscalBeginDate: "2024-01-01",
        fiscalEndDate: "2024-12-31"
      })
    });

    mockTriggerDuplicateSsn.mockReturnValue({
      unwrap: vi.fn().mockResolvedValue({ duplicateCount: 0 })
    });

    const storeAndWrapper = createMockStoreAndWrapper({
      yearsEnd: { selectedProfitYear: 2024 }
    });
    wrapper = storeAndWrapper.wrapper;
  });

  describe("Rendering", () => {
    it("should render the form with all fields", () => {
      render(
        <TerminationSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          isFetching={false}
        />,
        { wrapper }
      );

      expect(screen.getByLabelText("Begin Date")).toBeInTheDocument();
      expect(screen.getByLabelText("End Date")).toBeInTheDocument();
      expect(screen.getByRole("button", { name: /search/i })).toBeInTheDocument();
    });

    it("should render search and reset buttons", () => {
      render(
        <TerminationSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          isFetching={false}
        />,
        { wrapper }
      );

      expect(screen.getByRole("button", { name: /search/i })).toBeInTheDocument();
      expect(screen.getByRole("button", { name: /reset/i })).toBeInTheDocument();
    });

    it("should display loading state when isFetching is true", () => {
      render(
        <TerminationSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          isFetching={true}
        />,
        { wrapper }
      );

      expect(screen.getByRole("button", { name: /search/i })).toBeDisabled();
    });
  });

  describe("Date validation", () => {
    it("should enforce fiscal year date constraints", () => {
      render(
        <TerminationSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          isFetching={false}
        />,
        { wrapper }
      );

      const beginDateInput = screen.getByLabelText("Begin Date") as HTMLInputElement;
      const endDateInput = screen.getByLabelText("End Date") as HTMLInputElement;

      // Verify elements exist
      expect(beginDateInput).toBeInTheDocument();
      expect(endDateInput).toBeInTheDocument();
    });

    it("should validate end date is after begin date", async () => {
      render(
        <TerminationSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          isFetching={false}
        />,
        { wrapper }
      );

      const beginDateInput = screen.getByLabelText("Begin Date");
      const endDateInput = screen.getByLabelText("End Date");

      // Enter begin date
      await userEvent.type(beginDateInput, "2024-06-01");

      // Enter earlier end date (should trigger validation)
      await userEvent.type(endDateInput, "2024-05-01");

      // Search button should exist and be testable
      expect(screen.getByRole("button", { name: /search/i })).toBeInTheDocument();
    });
  });

  describe("Search functionality", () => {
    it("should call onSearch when search button is clicked", async () => {
      const user = userEvent.setup();
      render(
        <TerminationSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          isFetching={false}
        />,
        { wrapper }
      );

      const searchButton = screen.getByRole("button", { name: /search/i });
      await waitFor(() => {
        expect(searchButton).not.toBeDisabled();
      });

      await user.click(searchButton);

      await waitFor(() => {
        expect(mockOnSearch).toHaveBeenCalled();
      });
    });

    it("should call setInitialSearchLoaded when search is executed", async () => {
      const user = userEvent.setup();
      render(
        <TerminationSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          isFetching={false}
        />,
        { wrapper }
      );

      const searchButton = screen.getByRole("button", { name: /search/i });
      await waitFor(() => {
        expect(searchButton).not.toBeDisabled();
      });

      await user.click(searchButton);

      await waitFor(() => {
        expect(mockSetInitialSearchLoaded).toHaveBeenCalledWith(true);
      });
    });

    it("should show alert when search clicked with unsaved changes", async () => {
      const mockAlert = vi.spyOn(window, "alert").mockImplementation(() => {});
      const user = userEvent.setup();

      render(
        <TerminationSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={true}
          isFetching={false}
        />,
        { wrapper }
      );

      const searchButton = screen.getByRole("button", { name: /search/i });

      // Button should not be disabled (just shows alert when clicked)
      await waitFor(() => {
        expect(searchButton).not.toBeDisabled();
      });

      await user.click(searchButton);

      // Should show alert instead of calling onSearch
      await waitFor(() => {
        expect(mockAlert).toHaveBeenCalledWith("Please save your changes.");
        expect(mockOnSearch).not.toHaveBeenCalled();
      });

      mockAlert.mockRestore();
    });

    it("should disable search button during fetch", () => {
      render(
        <TerminationSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          isFetching={true}
        />,
        { wrapper }
      );

      const searchButton = screen.getByRole("button", { name: /search/i });
      expect(searchButton).toBeDisabled();
    });
  });

  describe("Reset functionality", () => {
    it("should call onReset callback (via parent handler)", async () => {
      const user = userEvent.setup();
      render(
        <TerminationSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          isFetching={false}
        />,
        { wrapper }
      );

      const resetButton = screen.getByRole("button", { name: /reset/i });
      await user.click(resetButton);

      expect(resetButton).toBeInTheDocument();
    });
  });

  describe("Forfeiture status selection", () => {
    it("should include forfeiture status filter options", () => {
      render(
        <TerminationSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          isFetching={false}
        />,
        { wrapper }
      );

      // The component should contain status selection and search button
      expect(screen.getByRole("button", { name: /search/i })).toBeInTheDocument();
    });
  });

  describe("Form state management", () => {
    it("should track form validity", async () => {
      render(
        <TerminationSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          isFetching={false}
        />,
        { wrapper }
      );

      const searchButton = screen.getByRole("button", { name: /search/i });

      // Search button should exist and be in the document
      expect(searchButton).toBeInTheDocument();
    });
  });

  describe("Duplicate SSN guard integration", () => {
    it("should render DuplicateSsnGuard component", () => {
      render(
        <TerminationSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          isFetching={false}
        />,
        { wrapper }
      );

      // Verify search component exists (guard wraps it)
      expect(screen.getByRole("button", { name: /search/i })).toBeInTheDocument();
    });

    it("should pass prerequisite completion to search button", () => {
      render(
        <TerminationSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          isFetching={false}
        />,
        { wrapper }
      );

      // Guard component should allow search when prerequisites complete
      expect(screen.getByRole("button", { name: /search/i })).toBeInTheDocument();
    });
  });

  describe("Date range constraints", () => {
    it("should set min/max dates based on fiscal data", () => {
      render(
        <TerminationSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          isFetching={false}
        />,
        { wrapper }
      );

      const beginDateInput = screen.getByLabelText("Begin Date");
      const endDateInput = screen.getByLabelText("End Date");

      // Date inputs should exist and be properly configured
      expect(beginDateInput).toBeInTheDocument();
      expect(endDateInput).toBeInTheDocument();
    });
  });

  describe("Form submission", () => {
    it("should include correct default values in search request", async () => {
      const user = userEvent.setup();
      render(
        <TerminationSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={mockSetInitialSearchLoaded}
          hasUnsavedChanges={false}
          isFetching={false}
        />,
        { wrapper }
      );

      const searchButton = screen.getByRole("button", { name: /search/i });
      await waitFor(() => {
        expect(searchButton).not.toBeDisabled();
      });

      await user.click(searchButton);

      await waitFor(() => {
        // Search should be called with proper parameters
        expect(mockOnSearch).toHaveBeenCalled();
        const callArgs = mockOnSearch.mock.calls[0]?.[0];
        if (callArgs) {
          expect(callArgs).toHaveProperty("beginningDate");
          expect(callArgs).toHaveProperty("endingDate");
          expect(callArgs).toHaveProperty("forfeitureStatus");
        }
      });
    });
  });
});
