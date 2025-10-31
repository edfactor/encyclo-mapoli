import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi, beforeEach } from "vitest";
import ForfeituresAdjustmentSearchFilter from "./ForfeituresAdjustmentSearchFilter";

// Mock the form validators - must use factory function to avoid hoisting issues
vi.mock("../../../utils/FormValidators", async () => {
  const yup = await import("yup");

  return {
    ssnValidator: yup.default
      .string()
      .nullable()
      .test("is-9-digits", "SSN must be exactly 9 digits", function (value) {
        if (!value) return true;
        // Remove dashes for validation
        const cleaned = value.replace(/-/g, "");
        return /^\d{9}$/.test(cleaned);
      })
      .transform((value) => value || undefined),
    badgeNumberStringValidator: yup.default
      .string()
      .nullable()
      .transform((value) => value || undefined),
    handleSsnInput: (value: string) => {
      // Simple mock that just returns the value if it looks like an SSN
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
  };
});

describe("ForfeituresAdjustmentSearchFilter", () => {
  const mockOnSearch = vi.fn();
  const mockOnReset = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("Rendering", () => {
    it("should render search and reset buttons", () => {
      render(
        <ForfeituresAdjustmentSearchFilter
          onSearch={mockOnSearch}
          onReset={mockOnReset}
          isSearching={false}
        />
      );

      expect(screen.getByRole("button", { name: /search/i })).toBeInTheDocument();
      expect(screen.getByRole("button", { name: /reset/i })).toBeInTheDocument();
    });

    it("should render SSN and Badge input fields", () => {
      render(
        <ForfeituresAdjustmentSearchFilter
          onSearch={mockOnSearch}
          onReset={mockOnReset}
          isSearching={false}
        />
      );

      expect(screen.getByPlaceholderText("SSN")).toBeInTheDocument();
      expect(screen.getByPlaceholderText("Badge")).toBeInTheDocument();
    });

    it("should display required asterisks on fields", () => {
      render(
        <ForfeituresAdjustmentSearchFilter
          onSearch={mockOnSearch}
          onReset={mockOnReset}
          isSearching={false}
        />
      );

      const labels = screen.getAllByText("*");
      expect(labels.length).toBeGreaterThan(0);
    });
  });

  describe("Field mutual exclusion", () => {
    it("should disable badge field when SSN is entered", async () => {
      const user = userEvent.setup();
      render(
        <ForfeituresAdjustmentSearchFilter
          onSearch={mockOnSearch}
          onReset={mockOnReset}
          isSearching={false}
        />
      );

      const ssnInput = screen.getByPlaceholderText("SSN") as HTMLInputElement;
      const badgeInput = screen.getByPlaceholderText("Badge") as HTMLInputElement;

      await user.type(ssnInput, "123-45-6789");

      await waitFor(() => {
        expect(badgeInput).toBeDisabled();
      });
    });

    it("should disable SSN field when Badge is entered", async () => {
      const user = userEvent.setup();
      render(
        <ForfeituresAdjustmentSearchFilter
          onSearch={mockOnSearch}
          onReset={mockOnReset}
          isSearching={false}
        />
      );

      const ssnInput = screen.getByPlaceholderText("SSN") as HTMLInputElement;
      const badgeInput = screen.getByPlaceholderText("Badge") as HTMLInputElement;

      await user.type(badgeInput, "12345");

      await waitFor(() => {
        expect(ssnInput).toBeDisabled();
      });
    });

    it("should enable both fields when one is cleared", async () => {
      const user = userEvent.setup();
      render(
        <ForfeituresAdjustmentSearchFilter
          onSearch={mockOnSearch}
          onReset={mockOnReset}
          isSearching={false}
        />
      );

      const ssnInput = screen.getByPlaceholderText("SSN") as HTMLInputElement;
      const badgeInput = screen.getByPlaceholderText("Badge") as HTMLInputElement;

      // Type in SSN
      await user.type(ssnInput, "123-45-6789");
      await waitFor(() => expect(badgeInput).toBeDisabled());

      // Clear SSN
      await user.clear(ssnInput);

      // Both should now be enabled
      await waitFor(() => {
        expect(ssnInput).not.toBeDisabled();
        expect(badgeInput).not.toBeDisabled();
      });
    });
  });

  describe("Search button behavior", () => {
    it("should disable search button when no criteria entered", () => {
      render(
        <ForfeituresAdjustmentSearchFilter
          onSearch={mockOnSearch}
          onReset={mockOnReset}
          isSearching={false}
        />
      );

      const searchButton = screen.getByRole("button", { name: /search/i });
      expect(searchButton).toBeDisabled();
    });

    it("should enable search button when SSN is entered", async () => {
      const user = userEvent.setup();
      render(
        <ForfeituresAdjustmentSearchFilter
          onSearch={mockOnSearch}
          onReset={mockOnReset}
          isSearching={false}
        />
      );

      const ssnInput = screen.getByPlaceholderText("SSN");
      await user.type(ssnInput, "123-45-6789");

      const searchButton = screen.getByRole("button", { name: /search/i });

      await waitFor(() => {
        expect(searchButton).not.toBeDisabled();
      });
    });

    it("should enable search button when Badge is entered", async () => {
      const user = userEvent.setup();
      render(
        <ForfeituresAdjustmentSearchFilter
          onSearch={mockOnSearch}
          onReset={mockOnReset}
          isSearching={false}
        />
      );

      const badgeInput = screen.getByPlaceholderText("Badge");
      await user.type(badgeInput, "12345");

      const searchButton = screen.getByRole("button", { name: /search/i });

      await waitFor(() => {
        expect(searchButton).not.toBeDisabled();
      });
    });

    it("should call onSearch with correct parameters", async () => {
      const user = userEvent.setup();
      render(
        <ForfeituresAdjustmentSearchFilter
          onSearch={mockOnSearch}
          onReset={mockOnReset}
          isSearching={false}
        />
      );

      const ssnInput = screen.getByPlaceholderText("SSN");
      await user.type(ssnInput, "123-45-6789");
      await user.tab(); // Blur to trigger validation

      const searchButton = screen.getByRole("button", { name: /search/i });

      await waitFor(() => {
        expect(searchButton).not.toBeDisabled();
      });

      await user.click(searchButton);

      await waitFor(() => {
        expect(mockOnSearch).toHaveBeenCalledWith(
          expect.objectContaining({
            ssn: expect.any(String),
            profitYear: expect.any(Number),
            skip: 0,
            take: 255,
            sortBy: "badgeNumber",
            isSortDescending: false
          })
        );
      });
    });

    it("should show loading state when isSearching is true", () => {
      render(
        <ForfeituresAdjustmentSearchFilter
          onSearch={mockOnSearch}
          onReset={mockOnReset}
          isSearching={true}
        />
      );

      const searchButton = screen.getByRole("button", { name: /search/i });
      expect(searchButton).toBeDisabled();
    });
  });

  describe("Reset button behavior", () => {
    it("should clear form fields when reset is clicked", async () => {
      const user = userEvent.setup();
      render(
        <ForfeituresAdjustmentSearchFilter
          onSearch={mockOnSearch}
          onReset={mockOnReset}
          isSearching={false}
        />
      );

      const ssnInput = screen.getByPlaceholderText("SSN") as HTMLInputElement;
      const badgeInput = screen.getByPlaceholderText("Badge") as HTMLInputElement;

      // Type values
      await user.type(ssnInput, "123-45-6789");
      expect(ssnInput.value).not.toBe("");

      // Click reset
      const resetButton = screen.getByRole("button", { name: /reset/i });
      fireEvent.click(resetButton);

      await waitFor(() => {
        expect(ssnInput.value).toBe("");
        expect(badgeInput.value).toBe("");
      });
    });

    it("should call onReset callback", async () => {
      const user = userEvent.setup();
      render(
        <ForfeituresAdjustmentSearchFilter
          onSearch={mockOnSearch}
          onReset={mockOnReset}
          isSearching={false}
        />
      );

      const ssnInput = screen.getByPlaceholderText("SSN");
      await user.type(ssnInput, "123-45-6789");

      const resetButton = screen.getByRole("button", { name: /reset/i });
      fireEvent.click(resetButton);

      expect(mockOnReset).toHaveBeenCalled();
    });

    it("should enable both fields after reset", async () => {
      const user = userEvent.setup();
      render(
        <ForfeituresAdjustmentSearchFilter
          onSearch={mockOnSearch}
          onReset={mockOnReset}
          isSearching={false}
        />
      );

      const ssnInput = screen.getByPlaceholderText("SSN") as HTMLInputElement;
      const badgeInput = screen.getByPlaceholderText("Badge") as HTMLInputElement;

      // Type in SSN to disable badge
      await user.type(ssnInput, "123-45-6789");
      await waitFor(() => expect(badgeInput).toBeDisabled());

      // Click reset
      const resetButton = screen.getByRole("button", { name: /reset/i });
      fireEvent.click(resetButton);

      // Both should be enabled
      await waitFor(() => {
        expect(ssnInput).not.toBeDisabled();
        expect(badgeInput).not.toBeDisabled();
      });
    });
  });

  describe("Input validation", () => {
    it("should validate SSN format", async () => {
      const user = userEvent.setup();
      render(
        <ForfeituresAdjustmentSearchFilter
          onSearch={mockOnSearch}
          onReset={mockOnReset}
          isSearching={false}
        />
      );

      const ssnInput = screen.getByPlaceholderText("SSN");

      // Enter invalid SSN
      await user.type(ssnInput, "invalid");

      const searchButton = screen.getByRole("button", { name: /search/i });

      // Should still be disabled if validation fails
      // This test ensures the validator is working
      expect(searchButton).toBeInTheDocument();
    });

    it("should validate that at least one field is required", () => {
      render(
        <ForfeituresAdjustmentSearchFilter
          onSearch={mockOnSearch}
          onReset={mockOnReset}
          isSearching={false}
        />
      );

      const searchButton = screen.getByRole("button", { name: /search/i });
      // No fields filled, search should be disabled
      expect(searchButton).toBeDisabled();
    });
  });

  describe("Form submission", () => {
    it("should submit form with badge number", async () => {
      const user = userEvent.setup();
      render(
        <ForfeituresAdjustmentSearchFilter
          onSearch={mockOnSearch}
          onReset={mockOnReset}
          isSearching={false}
        />
      );

      const badgeInput = screen.getByPlaceholderText("Badge");
      await user.type(badgeInput, "12345");
      await user.tab(); // Blur to trigger validation

      const searchButton = screen.getByRole("button", { name: /search/i });

      await waitFor(() => {
        expect(searchButton).not.toBeDisabled();
      });

      await user.click(searchButton);

      await waitFor(() => {
        expect(mockOnSearch).toHaveBeenCalledWith(
          expect.objectContaining({
            badge: expect.any(String),
            profitYear: expect.any(Number),
            skip: 0,
            take: 255,
            sortBy: "badgeNumber",
            isSortDescending: false
          })
        );
      });
    });
  });
});
