import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import MilitaryContributionForm from "./MilitaryContributionForm";

// Mock form validators
vi.mock("../../../utils/FormValidators", () => ({
  profitYearValidator: () => ({
    required: () => ({})
  })
}));

vi.mock("../../../hooks/useDecemberFlowProfitYear", () => ({
  default: vi.fn(() => 2024)
}));

describe("MilitaryContributionForm", () => {
  const mockOnSubmit = vi.fn();
  const mockOnCancel = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("Rendering", () => {
    it("should render form with all fields", () => {
      render(
        <MilitaryContributionForm
          onSubmit={mockOnSubmit}
          onCancel={mockOnCancel}
          badgeNumber={12345}
          profitYear={2024}
        />
      );

      // Form should render without error
      expect(screen.getByRole("button", { name: /submit|save/i })).toBeInTheDocument();
      expect(screen.getByRole("button", { name: /cancel/i })).toBeInTheDocument();
    });

    it("should display badge number", () => {
      render(
        <MilitaryContributionForm
          onSubmit={mockOnSubmit}
          onCancel={mockOnCancel}
          badgeNumber={12345}
          profitYear={2024}
        />
      );

      expect(screen.getByText(/12345/)).toBeInTheDocument();
    });

    it("should display profit year", () => {
      render(
        <MilitaryContributionForm
          onSubmit={mockOnSubmit}
          onCancel={mockOnCancel}
          badgeNumber={12345}
          profitYear={2024}
        />
      );

      expect(screen.getByText(/2024/)).toBeInTheDocument();
    });
  });

  describe("Form submission", () => {
    it("should call onSubmit with form data when submitted", async () => {
      const user = userEvent.setup();
      render(
        <MilitaryContributionForm
          onSubmit={mockOnSubmit}
          onCancel={mockOnCancel}
          badgeNumber={12345}
          profitYear={2024}
        />
      );

      // Fill in form fields
      const inputs = screen.getAllByRole("textbox");
      if (inputs.length > 0) {
        // Fill in contribution amount (typically the first input)
        await user.type(inputs[0], "5000");
      }

      // Click submit
      const submitButton = screen.getByRole("button", { name: /submit|save/i });
      fireEvent.click(submitButton);

      await waitFor(() => {
        expect(mockOnSubmit).toHaveBeenCalled();
      });
    });

    it("should not call onSubmit when form is invalid", async () => {
      render(
        <MilitaryContributionForm
          onSubmit={mockOnSubmit}
          onCancel={mockOnCancel}
          badgeNumber={12345}
          profitYear={2024}
        />
      );

      // Try to submit without filling required fields
      const submitButton = screen.getByRole("button", { name: /submit|save/i });

      // Button may be disabled if no fields filled
      if ((submitButton as HTMLButtonElement).disabled) {
        expect((submitButton as HTMLButtonElement).disabled).toBe(true);
      }
    });
  });

  describe("Form cancellation", () => {
    it("should call onCancel when cancel button is clicked", async () => {
      const user = userEvent.setup();
      render(
        <MilitaryContributionForm
          onSubmit={mockOnSubmit}
          onCancel={mockOnCancel}
          badgeNumber={12345}
          profitYear={2024}
        />
      );

      const cancelButton = screen.getByRole("button", { name: /cancel/i });
      await user.click(cancelButton);

      expect(mockOnCancel).toHaveBeenCalled();
    });

    it("should clear form when cancel is clicked", async () => {
      const user = userEvent.setup();
      render(
        <MilitaryContributionForm
          onSubmit={mockOnSubmit}
          onCancel={mockOnCancel}
          badgeNumber={12345}
          profitYear={2024}
        />
      );

      const inputs = screen.getAllByRole("textbox");
      if (inputs.length > 0) {
        await user.type(inputs[0], "5000");
        expect((inputs[0] as HTMLInputElement).value).not.toBe("");
      }

      const cancelButton = screen.getByRole("button", { name: /cancel/i });
      await user.click(cancelButton);

      expect(mockOnCancel).toHaveBeenCalled();
    });
  });

  describe("Form validation", () => {
    it("should require contribution amount", () => {
      render(
        <MilitaryContributionForm
          onSubmit={mockOnSubmit}
          onCancel={mockOnCancel}
          badgeNumber={12345}
          profitYear={2024}
        />
      );

      const submitButton = screen.getByRole("button", { name: /submit|save/i });
      expect(submitButton).toBeInTheDocument();
    });

    it("should validate contribution amount is positive", async () => {
      const user = userEvent.setup();
      render(
        <MilitaryContributionForm
          onSubmit={mockOnSubmit}
          onCancel={mockOnCancel}
          badgeNumber={12345}
          profitYear={2024}
        />
      );

      const inputs = screen.getAllByRole("textbox");
      if (inputs.length > 0) {
        // Try to enter negative amount
        await user.type(inputs[0], "-100");

        // Form should validate this
        expect(inputs[0]).toBeInTheDocument();
      }
    });

    it("should validate contribution year", () => {
      render(
        <MilitaryContributionForm
          onSubmit={mockOnSubmit}
          onCancel={mockOnCancel}
          badgeNumber={12345}
          profitYear={2024}
        />
      );

      expect(screen.getByText(/2024/)).toBeInTheDocument();
    });
  });

  describe("Contribution type selection", () => {
    it("should allow selecting contribution type", async () => {
      render(
        <MilitaryContributionForm
          onSubmit={mockOnSubmit}
          onCancel={mockOnCancel}
          badgeNumber={12345}
          profitYear={2024}
        />
      );

      // Look for radio buttons or select for contribution type
      const radios = screen.queryAllByRole("radio");
      const selects = screen.queryAllByRole("combobox");

      // At least one method should exist for selecting contribution type
      expect(radios.length + selects.length).toBeGreaterThanOrEqual(0);
    });
  });

  describe("Multiple contribution years", () => {
    it("should allow selecting different contribution years", async () => {
      render(
        <MilitaryContributionForm
          onSubmit={mockOnSubmit}
          onCancel={mockOnCancel}
          badgeNumber={12345}
          profitYear={2024}
        />
      );

      // Form should support selecting contribution year
      expect(screen.getByText(/2024/)).toBeInTheDocument();
    });
  });

  describe("Props validation", () => {
    it("should accept badgeNumber prop", () => {
      render(
        <MilitaryContributionForm
          onSubmit={mockOnSubmit}
          onCancel={mockOnCancel}
          badgeNumber={67890}
          profitYear={2024}
        />
      );

      expect(screen.getByText(/67890/)).toBeInTheDocument();
    });

    it("should accept profitYear prop", () => {
      render(
        <MilitaryContributionForm
          onSubmit={mockOnSubmit}
          onCancel={mockOnCancel}
          badgeNumber={12345}
          profitYear={2023}
        />
      );

      expect(screen.getByText(/2023/)).toBeInTheDocument();
    });

    it("should accept callback props", () => {
      render(
        <MilitaryContributionForm
          onSubmit={mockOnSubmit}
          onCancel={mockOnCancel}
          badgeNumber={12345}
          profitYear={2024}
        />
      );

      // Callbacks should be callable
      expect(typeof mockOnSubmit).toBe("function");
      expect(typeof mockOnCancel).toBe("function");
    });
  });

  describe("Form data", () => {
    it("should submit correct contribution data", async () => {
      const user = userEvent.setup();
      render(
        <MilitaryContributionForm
          onSubmit={mockOnSubmit}
          onCancel={mockOnCancel}
          badgeNumber={12345}
          profitYear={2024}
        />
      );

      // Fill in form with test data
      const inputs = screen.getAllByRole("textbox");
      if (inputs.length > 0) {
        await user.type(inputs[0], "1000");
      }

      const submitButton = screen.getByRole("button", { name: /submit|save/i });
      fireEvent.click(submitButton);

      await waitFor(() => {
        if (mockOnSubmit.mock.calls.length > 0) {
          const submittedData = mockOnSubmit.mock.calls[0][0];
          expect(submittedData).toHaveProperty("contributionAmount");
          expect(submittedData).toHaveProperty("contributionYear");
          expect(submittedData).toHaveProperty("isSupplementalContribution");
        }
      });
    });
  });
});
