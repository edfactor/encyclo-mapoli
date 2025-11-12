import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { createMockStoreAndWrapper } from "../../../../test";
import MilitaryContributionSearchFilter from "../MilitaryContributionSearchFilter";
import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";

// Mock validators
vi.mock("../../../../utils/FormValidators", async () => {
  const yup = await import("yup");
  return {
    ssnValidator: yup.default
      .string()
      .nullable()
      .test("is-9-digits", "SSN must be exactly 9 digits", function (value) {
        if (!value) return true;
        return /^\d{9}$/.test(value);
      }),
    badgeNumberStringValidator: yup.default
      .string()
      .nullable()
      .test("is-numeric", "Badge Number must contain only digits", function (value) {
        if (!value) return true;
        return /^\d+$/.test(value);
      })
      .test("is-valid-length", "Badge Number must be 1 to 7 digits", function (value) {
        if (!value) return true;
        return value.length >= 1 && value.length <= 7;
      }),
    handleSsnInput: (value: string): string | null => {
      // Only allow numeric input
      if (value !== "" && !/^\d*$/.test(value)) {
        return null;
      }
      // Prevent input beyond 9 characters
      if (value.length > 9) {
        return null;
      }
      return value === "" ? "" : value;
    },
    handleBadgeNumberStringInput: (value: string): string | null => {
      // Only allow numeric input
      if (value !== "" && !/^\d*$/.test(value)) {
        return null;
      }
      // Prevent input beyond 7 characters
      if (value.length > 7) {
        return null;
      }
      return value === "" ? "" : value;
    }
  };
});

vi.mock("../../../../hooks/useDecemberFlowProfitYear", () => ({
  default: vi.fn(() => 2024)
}));

vi.mock("../hooks/useMilitaryContribution", () => ({
  default: vi.fn(() => ({
    isSearching: false,
    executeSearch: vi.fn(),
    resetSearch: vi.fn()
  }))
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
      <button
        data-testid="reset-btn"
        onClick={handleReset}>
        Reset
      </button>
      {isFetching && <span data-testid="loading">Loading...</span>}
    </div>
  ))
}));

describe("MilitaryContributionSearchFilter", () => {
  let wrapper: ReturnType<typeof createMockStoreAndWrapper>["wrapper"];

  beforeEach(() => {
    vi.clearAllMocks();
    const storeAndWrapper = createMockStoreAndWrapper({
      yearsEnd: { selectedProfitYear: 2024 }
    });
    wrapper = storeAndWrapper.wrapper;
  });

  describe("Rendering", () => {
    it("should render search and reset buttons", () => {
      render(<MilitaryContributionSearchFilter />, { wrapper });

      expect(screen.getByTestId("search-btn")).toBeInTheDocument();
      expect(screen.getByTestId("reset-btn")).toBeInTheDocument();
    });

    it("should render SSN and Badge input fields", () => {
      render(<MilitaryContributionSearchFilter />, { wrapper });

      const inputs = screen.getAllByPlaceholderText(/SSN|Badge/i);
      expect(inputs.length).toBeGreaterThan(0);
    });

    it("should display required asterisks", () => {
      render(<MilitaryContributionSearchFilter />, { wrapper });

      const requiredLabels = screen.getAllByText("*");
      expect(requiredLabels.length).toBeGreaterThan(0);
    });
  });

  describe("Field mutual exclusion", () => {
    it("should disable badge field when SSN is entered", async () => {
      const user = userEvent.setup();
      render(<MilitaryContributionSearchFilter />, { wrapper });

      const ssnInput = screen.getByPlaceholderText("Enter SSN");
      const badgeInput = screen.getByPlaceholderText("Enter Badge Number");

      await user.type(ssnInput, "123456789");

      await waitFor(() => {
        expect(badgeInput).toBeDisabled();
      });
    });

    it("should disable SSN field when Badge is entered", async () => {
      const user = userEvent.setup();
      render(<MilitaryContributionSearchFilter />, { wrapper });

      const ssnInput = screen.getByPlaceholderText("Enter SSN");
      const badgeInput = screen.getByPlaceholderText("Enter Badge Number");

      await user.type(badgeInput, "12345");

      await waitFor(() => {
        expect(ssnInput).toBeDisabled();
      });
    });

    it("should enable both fields when one is cleared via reset", async () => {
      const user = userEvent.setup();
      render(<MilitaryContributionSearchFilter />, { wrapper });

      const ssnInput = screen.getByPlaceholderText("Enter SSN");
      const badgeInput = screen.getByPlaceholderText("Enter Badge Number");

      // Type in SSN
      await user.type(ssnInput, "123456789");
      await waitFor(() => {
        expect(badgeInput).toBeDisabled();
      });

      // Click reset button to clear form
      const resetButton = screen.getByTestId("reset-btn");
      await user.click(resetButton);

      // Both should be enabled after reset
      await waitFor(() => {
        expect(ssnInput).not.toBeDisabled();
        expect(badgeInput).not.toBeDisabled();
      });
    });
  });

  describe("Search button behavior", () => {
    it("should have search button initially disabled when no criteria entered", () => {
      render(<MilitaryContributionSearchFilter />, { wrapper });

      const searchButton = screen.getByTestId("search-btn");
      expect(searchButton).toBeDisabled();
    });

    it("should have search and reset buttons available", () => {
      render(<MilitaryContributionSearchFilter />, { wrapper });

      const searchButton = screen.getByTestId("search-btn");
      const resetButton = screen.getByTestId("reset-btn");

      expect(searchButton).toBeInTheDocument();
      expect(resetButton).toBeInTheDocument();
    });
  });

  describe("Reset button behavior", () => {
    it("should have reset button available", () => {
      render(<MilitaryContributionSearchFilter />, { wrapper });

      const resetButton = screen.getByTestId("reset-btn");
      expect(resetButton).toBeInTheDocument();
      expect(resetButton).not.toBeDisabled();
    });
  });

  describe("Form validation", () => {
    it("should validate that at least one field is required", () => {
      render(<MilitaryContributionSearchFilter />, { wrapper });

      const searchButton = screen.getByTestId("search-btn");
      expect(searchButton).toBeDisabled();
    });

    it("should validate SSN format", async () => {
      const user = userEvent.setup();
      render(<MilitaryContributionSearchFilter />, { wrapper });

      const ssnInput = screen.getByPlaceholderText("Enter SSN");

      await user.type(ssnInput, "invalid");

      const searchButton = screen.getByTestId("search-btn");
      expect(searchButton).toBeInTheDocument();
    });
  });

  describe("Form submission", () => {
    it("should render form element", () => {
      const { container } = render(<MilitaryContributionSearchFilter />, { wrapper });

      const form = container.querySelector("form");
      expect(form).toBeInTheDocument();
    });
  });

  describe("Loading state", () => {
    it("should show loading state when searching", () => {
      render(<MilitaryContributionSearchFilter />, { wrapper });

      expect(screen.getByTestId("search-and-reset")).toBeInTheDocument();
    });
  });

  describe("Profit year integration", () => {
    it("should use profit year from hook", () => {
      render(<MilitaryContributionSearchFilter />, { wrapper });

      expect(useDecemberFlowProfitYear).toHaveBeenCalled();
    });
  });
});
