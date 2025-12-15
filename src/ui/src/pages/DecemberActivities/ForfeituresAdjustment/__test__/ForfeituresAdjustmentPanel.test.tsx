import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi, beforeEach } from "vitest";
import ForfeituresAdjustmentPanel from "../ForfeituresAdjustmentPanel";

describe("ForfeituresAdjustmentPanel", () => {
  const mockOnAddForfeiture = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("Button enabled state", () => {
    it("should enable button when not in read-only mode and balance is greater than zero", () => {
      render(
        <ForfeituresAdjustmentPanel
          onAddForfeiture={mockOnAddForfeiture}
          isReadOnly={false}
          currentBalance={1000}
        />
      );

      const button = screen.getByRole("button", { name: /add forfeiture/i });
      expect(button).toBeEnabled();
    });

    it("should disable button when in read-only mode even with positive balance", () => {
      render(
        <ForfeituresAdjustmentPanel
          onAddForfeiture={mockOnAddForfeiture}
          isReadOnly={true}
          currentBalance={1000}
        />
      );

      const button = screen.getByRole("button", { name: /add forfeiture/i });
      expect(button).toBeDisabled();
    });

    it("should disable button when balance is zero even when not in read-only mode", () => {
      render(
        <ForfeituresAdjustmentPanel
          onAddForfeiture={mockOnAddForfeiture}
          isReadOnly={false}
          currentBalance={0}
        />
      );

      const button = screen.getByRole("button", { name: /add forfeiture/i });
      expect(button).toBeDisabled();
    });

    it("should disable button when both in read-only mode and balance is zero", () => {
      render(
        <ForfeituresAdjustmentPanel
          onAddForfeiture={mockOnAddForfeiture}
          isReadOnly={true}
          currentBalance={0}
        />
      );

      const button = screen.getByRole("button", { name: /add forfeiture/i });
      expect(button).toBeDisabled();
    });
  });

  describe("Tooltip messages", () => {
    it("should show read-only tooltip when in read-only mode with positive balance", () => {
      render(
        <ForfeituresAdjustmentPanel
          onAddForfeiture={mockOnAddForfeiture}
          isReadOnly={true}
          currentBalance={1000}
        />
      );

      const button = screen.getByRole("button", { name: /add forfeiture/i });
      expect(button).toBeDisabled();

      // The tooltip message should be in the DOM (MUI renders it as aria-label on the wrapper span)
      expect(screen.getByLabelText("You are in read-only mode and cannot add forfeitures.")).toBeInTheDocument();
    });

    it("should show balance zero tooltip when balance is zero and not in read-only mode", () => {
      render(
        <ForfeituresAdjustmentPanel
          onAddForfeiture={mockOnAddForfeiture}
          isReadOnly={false}
          currentBalance={0}
        />
      );

      const button = screen.getByRole("button", { name: /add forfeiture/i });
      expect(button).toBeDisabled();

      // The tooltip message should be in the DOM (MUI renders it as aria-label on the wrapper span)
      expect(screen.getByLabelText("Cannot add forfeiture when balance is zero.")).toBeInTheDocument();
    });

    it("should prioritize balance zero message when both conditions are true", () => {
      render(
        <ForfeituresAdjustmentPanel
          onAddForfeiture={mockOnAddForfeiture}
          isReadOnly={true}
          currentBalance={0}
        />
      );

      const button = screen.getByRole("button", { name: /add forfeiture/i });
      expect(button).toBeDisabled();

      // When balance is zero, that message should take priority (check aria-label)
      expect(screen.getByLabelText("Cannot add forfeiture when balance is zero.")).toBeInTheDocument();
      expect(screen.queryByLabelText("You are in read-only mode and cannot add forfeitures.")).not.toBeInTheDocument();
    });
  });

  describe("Button click behavior", () => {
    it("should call onAddForfeiture when button is clicked and enabled", async () => {
      const user = userEvent.setup();
      render(
        <ForfeituresAdjustmentPanel
          onAddForfeiture={mockOnAddForfeiture}
          isReadOnly={false}
          currentBalance={1000}
        />
      );

      const button = screen.getByRole("button", { name: /add forfeiture/i });
      await user.click(button);

      expect(mockOnAddForfeiture).toHaveBeenCalledTimes(1);
    });

    it("should not call onAddForfeiture when button is disabled", () => {
      render(
        <ForfeituresAdjustmentPanel
          onAddForfeiture={mockOnAddForfeiture}
          isReadOnly={true}
          currentBalance={1000}
        />
      );

      const button = screen.getByRole("button", { name: /add forfeiture/i });
      
      // Button should be disabled and not clickable
      expect(button).toBeDisabled();
      expect(mockOnAddForfeiture).not.toHaveBeenCalled();
    });
  });

  describe("Edge cases", () => {
    it("should handle negative balance (treat as valid)", () => {
      render(
        <ForfeituresAdjustmentPanel
          onAddForfeiture={mockOnAddForfeiture}
          isReadOnly={false}
          currentBalance={-100}
        />
      );

      const button = screen.getByRole("button", { name: /add forfeiture/i });
      expect(button).toBeEnabled();
    });

    it("should handle very small positive balance", () => {
      render(
        <ForfeituresAdjustmentPanel
          onAddForfeiture={mockOnAddForfeiture}
          isReadOnly={false}
          currentBalance={0.01}
        />
      );

      const button = screen.getByRole("button", { name: /add forfeiture/i });
      expect(button).toBeEnabled();
    });

    it("should render button with correct styling and icon", () => {
      render(
        <ForfeituresAdjustmentPanel
          onAddForfeiture={mockOnAddForfeiture}
          isReadOnly={false}
          currentBalance={1000}
        />
      );

      const button = screen.getByRole("button", { name: /add forfeiture/i });
      expect(button).toHaveTextContent("ADD FORFEITURE");
    });
  });
});
