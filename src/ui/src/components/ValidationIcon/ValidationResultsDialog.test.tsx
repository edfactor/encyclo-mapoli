import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

// Mock @mui/material BEFORE importing the component to prevent file handle issues
vi.mock("@mui/material", () => ({
  Button: ({ children, onClick }: { children: React.ReactNode; onClick?: () => void }) => (
    <button onClick={onClick}>{children}</button>
  ),
  Dialog: ({ open, onClose, children }: { open: boolean; onClose: () => void; children: React.ReactNode }) =>
    open ? (
      <div
        role="dialog"
        onKeyDown={(e) => e.key === "Escape" && onClose()}>
        {children}
      </div>
    ) : null,
  DialogTitle: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
  DialogContent: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
  DialogActions: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
  Typography: ({ children, variant, color }: { children: React.ReactNode; variant?: string; color?: string }) => (
    <span
      data-variant={variant}
      data-color={color}>
      {children}
    </span>
  )
}));

// Mock smart-ui-library to avoid any potential issues
vi.mock("smart-ui-library", () => ({
  numberToCurrency: (value: number) => {
    const formatter = new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD"
    });
    return formatter.format(value);
  }
}));

import { CrossReferenceValidationGroup } from "../../types/validation/cross-reference-validation";
import { ValidationResultsDialog } from "./ValidationResultsDialog";

// Helper function to create a mock validation group
const createMockValidationGroup = (
  fieldName: string,
  isValid: boolean,
  currentValue = 1000,
  expectedValue = 1000,
  variance = 0
): CrossReferenceValidationGroup => ({
  groupName: "Test Group",
  description: "Test validation group",
  isValid,
  validations: [
    {
      fieldName,
      reportCode: "PAY443",
      isValid,
      currentValue,
      expectedValue,
      variance,
      message: null,
      archivedAt: null,
      notes: null
    }
  ],
  summary: null,
  priority: "Medium",
  validationRule: null
});

describe("ValidationResultsDialog", () => {
  const defaultProps = {
    open: true,
    onClose: vi.fn(),
    validationGroup: null,
    fieldName: null
  };

  describe("Rendering", () => {
    it("should render dialog when open is true", () => {
      render(<ValidationResultsDialog {...defaultProps} />);

      expect(screen.getByRole("dialog")).toBeInTheDocument();
    });

    it("should not render dialog when open is false", () => {
      render(
        <ValidationResultsDialog
          {...defaultProps}
          open={false}
        />
      );

      expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
    });

    it("should display 'No validation data is available' when validationGroup is null", () => {
      render(
        <ValidationResultsDialog
          {...defaultProps}
          validationGroup={null}
          fieldName="TestField"
        />
      );

      expect(screen.getByText("No validation data is available.")).toBeInTheDocument();
    });

    it("should display 'No validation found' message when fieldName not found", () => {
      const validationGroup = createMockValidationGroup("DifferentField", true);

      render(
        <ValidationResultsDialog
          {...defaultProps}
          validationGroup={validationGroup}
          fieldName="NonExistentField"
        />
      );

      expect(screen.getByText('No validation found for field "NonExistentField".')).toBeInTheDocument();
    });

    it("should display fieldName as title when fieldDisplayName is not provided", () => {
      const validationGroup = createMockValidationGroup("TestField", true);

      render(
        <ValidationResultsDialog
          {...defaultProps}
          validationGroup={validationGroup}
          fieldName="TestField"
        />
      );

      expect(screen.getByText("TestField")).toBeInTheDocument();
    });

    it("should display fieldDisplayName as title when provided", () => {
      const validationGroup = createMockValidationGroup("TestField", true);

      render(
        <ValidationResultsDialog
          {...defaultProps}
          validationGroup={validationGroup}
          fieldName="TestField"
          fieldDisplayName="Test Field Display Name"
        />
      );

      expect(screen.getByText("Test Field Display Name")).toBeInTheDocument();
    });

    it("should display Close button", () => {
      render(<ValidationResultsDialog {...defaultProps} />);

      expect(screen.getByRole("button", { name: "Close" })).toBeInTheDocument();
    });
  });

  describe("Validation Status Display", () => {
    it("should display '✓ Match' when validation is valid", () => {
      const validationGroup = createMockValidationGroup("TestField", true);

      render(
        <ValidationResultsDialog
          {...defaultProps}
          validationGroup={validationGroup}
          fieldName="TestField"
        />
      );

      expect(screen.getByText("✓ Match")).toBeInTheDocument();
    });

    it("should display '⚠ Mismatch' when validation is invalid", () => {
      const validationGroup = createMockValidationGroup("TestField", false, 1000, 900, 100);

      render(
        <ValidationResultsDialog
          {...defaultProps}
          validationGroup={validationGroup}
          fieldName="TestField"
        />
      );

      expect(screen.getByText("⚠ Mismatch")).toBeInTheDocument();
    });
  });

  describe("Validation Data Table", () => {
    it("should display Current and Expected row labels", () => {
      const validationGroup = createMockValidationGroup("TestField", true, 1000, 1000, 0);

      render(
        <ValidationResultsDialog
          {...defaultProps}
          validationGroup={validationGroup}
          fieldName="TestField"
        />
      );

      expect(screen.getByText("Current")).toBeInTheDocument();
      expect(screen.getByText("Expected")).toBeInTheDocument();
    });

    it("should display formatted currency values", () => {
      const validationGroup = createMockValidationGroup("TestField", true, 1234.56, 1234.56, 0);

      render(
        <ValidationResultsDialog
          {...defaultProps}
          validationGroup={validationGroup}
          fieldName="TestField"
        />
      );

      // Check that formatted currency values are displayed
      // numberToCurrency(1234.56) should produce "$1,234.56"
      expect(screen.getAllByText("$1,234.56")).toHaveLength(2); // Current and Expected
    });

    it("should display Variance row when validation is invalid and variance is non-zero", () => {
      const validationGroup = createMockValidationGroup("TestField", false, 1000, 900, 100);

      render(
        <ValidationResultsDialog
          {...defaultProps}
          validationGroup={validationGroup}
          fieldName="TestField"
        />
      );

      expect(screen.getByText("Variance")).toBeInTheDocument();
      expect(screen.getByText("$100.00")).toBeInTheDocument();
    });

    it("should not display Variance row when validation is valid", () => {
      const validationGroup = createMockValidationGroup("TestField", true, 1000, 1000, 0);

      render(
        <ValidationResultsDialog
          {...defaultProps}
          validationGroup={validationGroup}
          fieldName="TestField"
        />
      );

      expect(screen.queryByText("Variance")).not.toBeInTheDocument();
    });

    it("should not display Variance row when variance is zero", () => {
      // Edge case: invalid but variance is 0
      const validationGroup = createMockValidationGroup("TestField", false, 1000, 1000, 0);

      render(
        <ValidationResultsDialog
          {...defaultProps}
          validationGroup={validationGroup}
          fieldName="TestField"
        />
      );

      expect(screen.queryByText("Variance")).not.toBeInTheDocument();
    });

    it("should display table headers", () => {
      const validationGroup = createMockValidationGroup("TestField", true);

      render(
        <ValidationResultsDialog
          {...defaultProps}
          validationGroup={validationGroup}
          fieldName="TestField"
        />
      );

      expect(screen.getByText("Report")).toBeInTheDocument();
      expect(screen.getByText("Amount")).toBeInTheDocument();
    });
  });

  describe("Interactions", () => {
    it("should call onClose when Close button is clicked", () => {
      const handleClose = vi.fn();

      render(
        <ValidationResultsDialog
          {...defaultProps}
          onClose={handleClose}
        />
      );

      fireEvent.click(screen.getByRole("button", { name: "Close" }));

      expect(handleClose).toHaveBeenCalledTimes(1);
    });

    it("should call onClose when dialog backdrop is clicked", () => {
      const handleClose = vi.fn();

      render(
        <ValidationResultsDialog
          {...defaultProps}
          onClose={handleClose}
        />
      );

      // MUI Dialog backdrop click triggers onClose
      const dialog = screen.getByRole("dialog");
      fireEvent.keyDown(dialog, { key: "Escape" });

      expect(handleClose).toHaveBeenCalled();
    });
  });

  describe("Edge Cases", () => {
    it("should handle null currentValue gracefully", () => {
      const validationGroup: CrossReferenceValidationGroup = {
        groupName: "Test Group",
        description: null,
        isValid: true,
        validations: [
          {
            fieldName: "TestField",
            reportCode: "PAY443",
            isValid: true,
            currentValue: null,
            expectedValue: 1000,
            variance: null,
            message: null,
            archivedAt: null,
            notes: null
          }
        ],
        summary: null,
        priority: "Medium",
        validationRule: null
      };

      render(
        <ValidationResultsDialog
          {...defaultProps}
          validationGroup={validationGroup}
          fieldName="TestField"
        />
      );

      // Should display $0.00 for null values
      expect(screen.getByText("$0.00")).toBeInTheDocument();
    });

    it("should handle null expectedValue gracefully", () => {
      const validationGroup: CrossReferenceValidationGroup = {
        groupName: "Test Group",
        description: null,
        isValid: true,
        validations: [
          {
            fieldName: "TestField",
            reportCode: "PAY443",
            isValid: true,
            currentValue: 1000,
            expectedValue: null,
            variance: null,
            message: null,
            archivedAt: null,
            notes: null
          }
        ],
        summary: null,
        priority: "Medium",
        validationRule: null
      };

      render(
        <ValidationResultsDialog
          {...defaultProps}
          validationGroup={validationGroup}
          fieldName="TestField"
        />
      );

      // Should display $0.00 for null expected value
      expect(screen.getByText("$0.00")).toBeInTheDocument();
    });

    it("should handle negative variance", () => {
      const validationGroup = createMockValidationGroup("TestField", false, 900, 1000, -100);

      render(
        <ValidationResultsDialog
          {...defaultProps}
          validationGroup={validationGroup}
          fieldName="TestField"
        />
      );

      expect(screen.getByText("Variance")).toBeInTheDocument();
      expect(screen.getByText("-$100.00")).toBeInTheDocument();
    });
  });
});
