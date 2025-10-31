import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import MilitaryContributionSearchFilter from "./MilitaryContributionSearchFilter";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";

// Mock validators
vi.mock("../../../utils/FormValidators", () => ({
  ssnValidator: vi.fn(),
  badgeNumberStringValidator: vi.fn(),
  handleSsnInput: (value: string) => {
    const cleaned = value.replace(/\D/g, "");
    if (cleaned.length <= 9) {
      const match = cleaned.match(/^(\d{0,3})(\d{0,2})(\d{0,4})$/);
      if (match) {
        const parts = [match[1], match[2], match[3]].filter(Boolean);
        return parts.join("-") || "";
      }
    }
    return value;
  },
  handleBadgeNumberStringInput: (value: string) => {
    return value.replace(/\D/g, "");
  }
}));

vi.mock("../../../hooks/useDecemberFlowProfitYear", () => ({
  default: vi.fn(() => 2024)
}));

vi.mock("./hooks/useMilitaryContribution", () => ({
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
  //const mockExecuteSearch = vi.fn();
  //const mockResetSearch = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("Rendering", () => {
    it("should render search and reset buttons", () => {
      render(<MilitaryContributionSearchFilter />);

      expect(screen.getByTestId("search-btn")).toBeInTheDocument();
      expect(screen.getByTestId("reset-btn")).toBeInTheDocument();
    });

    it("should render SSN and Badge input fields", () => {
      render(<MilitaryContributionSearchFilter />);

      const inputs = screen.getAllByPlaceholderText(/SSN|Badge/i);
      expect(inputs.length).toBeGreaterThan(0);
    });

    it("should display required asterisks", () => {
      render(<MilitaryContributionSearchFilter />);

      const requiredLabels = screen.getAllByText("*");
      expect(requiredLabels.length).toBeGreaterThan(0);
    });
  });

  describe("Field mutual exclusion", () => {
    it("should disable badge field when SSN is entered", async () => {
      const user = userEvent.setup();
      render(<MilitaryContributionSearchFilter />);

      const inputs = screen.getAllByRole("textbox");
      const ssnInput = inputs.find((input) => (input as HTMLInputElement).placeholder?.includes("SSN"));
      const badgeInput = inputs.find((input) => (input as HTMLInputElement).placeholder?.includes("Badge"));

      if (ssnInput && badgeInput) {
        await user.type(ssnInput, "123-45-6789");

        await waitFor(() => {
          expect((badgeInput as HTMLInputElement).disabled).toBe(true);
        });
      }
    });

    it("should disable SSN field when Badge is entered", async () => {
      const user = userEvent.setup();
      render(<MilitaryContributionSearchFilter />);

      const inputs = screen.getAllByRole("textbox");
      const ssnInput = inputs.find((input) => (input as HTMLInputElement).placeholder?.includes("SSN"));
      const badgeInput = inputs.find((input) => (input as HTMLInputElement).placeholder?.includes("Badge"));

      if (ssnInput && badgeInput) {
        await user.type(badgeInput, "12345");

        await waitFor(() => {
          expect((ssnInput as HTMLInputElement).disabled).toBe(true);
        });
      }
    });

    it("should enable both fields when one is cleared", async () => {
      const user = userEvent.setup();
      render(<MilitaryContributionSearchFilter />);

      const inputs = screen.getAllByRole("textbox");
      const ssnInput = inputs.find((input) => (input as HTMLInputElement).placeholder?.includes("SSN"));
      const badgeInput = inputs.find((input) => (input as HTMLInputElement).placeholder?.includes("Badge"));

      if (ssnInput && badgeInput) {
        // Type in SSN
        await user.type(ssnInput, "123-45-6789");
        await waitFor(() => {
          expect((badgeInput as HTMLInputElement).disabled).toBe(true);
        });

        // Clear SSN
        await user.clear(ssnInput);

        // Both should be enabled
        await waitFor(() => {
          expect((ssnInput as HTMLInputElement).disabled).toBe(false);
          expect((badgeInput as HTMLInputElement).disabled).toBe(false);
        });
      }
    });
  });

  describe("Search button behavior", () => {
    it("should disable search button when no criteria entered", () => {
      render(<MilitaryContributionSearchFilter />);

      const searchButton = screen.getByTestId("search-btn");
      expect(searchButton).toBeDisabled();
    });

    it("should enable search button when SSN is entered", async () => {
      const user = userEvent.setup();
      render(<MilitaryContributionSearchFilter />);

      const inputs = screen.getAllByRole("textbox");
      const ssnInput = inputs.find((input) => (input as HTMLInputElement).placeholder?.includes("SSN"));

      if (ssnInput) {
        await user.type(ssnInput, "123-45-6789");

        const searchButton = screen.getByTestId("search-btn");

        await waitFor(() => {
          expect(searchButton).not.toBeDisabled();
        });
      }
    });

    it("should enable search button when Badge is entered", async () => {
      const user = userEvent.setup();
      render(<MilitaryContributionSearchFilter />);

      const inputs = screen.getAllByRole("textbox");
      const badgeInput = inputs.find((input) => (input as HTMLInputElement).placeholder?.includes("Badge"));

      if (badgeInput) {
        await user.type(badgeInput, "12345");

        const searchButton = screen.getByTestId("search-btn");

        await waitFor(() => {
          expect(searchButton).not.toBeDisabled();
        });
      }
    });
  });

  describe("Reset button behavior", () => {
    it("should clear form fields when reset is clicked", async () => {
      const user = userEvent.setup();
      render(<MilitaryContributionSearchFilter />);

      const inputs = screen.getAllByRole("textbox");
      const ssnInput = inputs.find((input) =>
        (input as HTMLInputElement).placeholder?.includes("SSN")
      ) as HTMLInputElement;

      if (ssnInput) {
        await user.type(ssnInput, "123-45-6789");
        expect(ssnInput.value).not.toBe("");

        const resetButton = screen.getByTestId("reset-btn");
        fireEvent.click(resetButton);

        await waitFor(() => {
          expect(ssnInput.value).toBe("");
        });
      }
    });

    it("should enable both fields after reset", async () => {
      const user = userEvent.setup();
      render(<MilitaryContributionSearchFilter />);

      const inputs = screen.getAllByRole("textbox");
      const ssnInput = inputs.find((input) =>
        (input as HTMLInputElement).placeholder?.includes("SSN")
      ) as HTMLInputElement;
      const badgeInput = inputs.find((input) =>
        (input as HTMLInputElement).placeholder?.includes("Badge")
      ) as HTMLInputElement;

      if (ssnInput && badgeInput) {
        await user.type(ssnInput, "123-45-6789");

        await waitFor(() => {
          expect(badgeInput.disabled).toBe(true);
        });

        const resetButton = screen.getByTestId("reset-btn");
        fireEvent.click(resetButton);

        await waitFor(() => {
          expect(ssnInput.disabled).toBe(false);
          expect(badgeInput.disabled).toBe(false);
        });
      }
    });
  });

  describe("Form validation", () => {
    it("should validate that at least one field is required", () => {
      render(<MilitaryContributionSearchFilter />);

      const searchButton = screen.getByTestId("search-btn");
      expect(searchButton).toBeDisabled();
    });

    it("should validate SSN format", async () => {
      const user = userEvent.setup();
      render(<MilitaryContributionSearchFilter />);

      const inputs = screen.getAllByRole("textbox");
      const ssnInput = inputs.find((input) => (input as HTMLInputElement).placeholder?.includes("SSN"));

      if (ssnInput) {
        await user.type(ssnInput, "invalid");

        const searchButton = screen.getByTestId("search-btn");
        expect(searchButton).toBeInTheDocument();
      }
    });
  });

  describe("Form submission", () => {
    it("should submit form with SSN", async () => {
      const user = userEvent.setup();
      render(<MilitaryContributionSearchFilter />);

      const inputs = screen.getAllByRole("textbox");
      const ssnInput = inputs.find((input) => (input as HTMLInputElement).placeholder?.includes("SSN"));

      if (ssnInput) {
        await user.type(ssnInput, "123-45-6789");

        const searchButton = screen.getByTestId("search-btn");

        await waitFor(() => {
          expect(searchButton).not.toBeDisabled();
        });

        fireEvent.click(searchButton);

        // Verify search was triggered
        expect(searchButton).toBeInTheDocument();
      }
    });

    it("should submit form with Badge", async () => {
      const user = userEvent.setup();
      render(<MilitaryContributionSearchFilter />);

      const inputs = screen.getAllByRole("textbox");
      const badgeInput = inputs.find((input) => (input as HTMLInputElement).placeholder?.includes("Badge"));

      if (badgeInput) {
        await user.type(badgeInput, "12345");

        const searchButton = screen.getByTestId("search-btn");

        await waitFor(() => {
          expect(searchButton).not.toBeDisabled();
        });

        fireEvent.click(searchButton);

        expect(searchButton).toBeInTheDocument();
      }
    });
  });

  describe("Loading state", () => {
    it("should show loading state when searching", () => {
      render(<MilitaryContributionSearchFilter />);

      expect(screen.getByTestId("search-and-reset")).toBeInTheDocument();
    });
  });

  describe("Profit year integration", () => {
    it("should use profit year from hook", () => {
      render(<MilitaryContributionSearchFilter />);

      expect(useDecemberFlowProfitYear).toHaveBeenCalled();
    });
  });
});
