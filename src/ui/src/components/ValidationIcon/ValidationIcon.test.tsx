import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { CrossReferenceValidationGroup } from "../../types/validation/cross-reference-validation";
import { ValidationIcon } from "./ValidationIcon";

// Helper function to create a mock validation group
const createMockValidationGroup = (
  fieldName: string,
  isValid: boolean
): CrossReferenceValidationGroup => ({
  groupName: "Test Group",
  description: "Test validation group",
  isValid,
  validations: [
    {
      fieldName,
      reportCode: "PAY443",
      isValid,
      currentValue: 1000,
      expectedValue: isValid ? 1000 : 900,
      variance: isValid ? 0 : 100,
      message: null,
      archivedAt: null,
      notes: null
    }
  ],
  summary: null,
  priority: "Medium",
  validationRule: null
});

describe("ValidationIcon", () => {
  describe("Rendering", () => {
    it("should render nothing when validationGroup is null", () => {
      const { container } = render(
        <ValidationIcon
          validationGroup={null}
          fieldName="TestField"
        />
      );

      expect(container.firstChild).toBeNull();
    });

    it("should render nothing when fieldName is not found in validations", () => {
      const validationGroup = createMockValidationGroup("DifferentField", true);

      const { container } = render(
        <ValidationIcon
          validationGroup={validationGroup}
          fieldName="NonExistentField"
        />
      );

      expect(container.firstChild).toBeNull();
    });

    it("should render icon when validation matches fieldName", () => {
      const validationGroup = createMockValidationGroup("TestField", true);

      render(
        <ValidationIcon
          validationGroup={validationGroup}
          fieldName="TestField"
        />
      );

      expect(screen.getByLabelText(/Validation status for TestField/)).toBeInTheDocument();
    });

    it("should display green icon when validation is valid", () => {
      const validationGroup = createMockValidationGroup("TestField", true);

      render(
        <ValidationIcon
          validationGroup={validationGroup}
          fieldName="TestField"
        />
      );

      const icon = screen.getByLabelText("Validation status for TestField: valid");
      expect(icon.querySelector("svg")).toHaveClass("text-green-500");
    });

    it("should display orange icon when validation is invalid", () => {
      const validationGroup = createMockValidationGroup("TestField", false);

      render(
        <ValidationIcon
          validationGroup={validationGroup}
          fieldName="TestField"
        />
      );

      const icon = screen.getByLabelText("Validation status for TestField: invalid");
      expect(icon.querySelector("svg")).toHaveClass("text-orange-500");
    });

    it("should apply custom className", () => {
      const validationGroup = createMockValidationGroup("TestField", true);

      render(
        <ValidationIcon
          validationGroup={validationGroup}
          fieldName="TestField"
          className="custom-class"
        />
      );

      const wrapper = screen.getByLabelText(/Validation status for TestField/);
      expect(wrapper).toHaveClass("custom-class");
    });
  });

  describe("Interactions", () => {
    it("should call onClick when clicked", () => {
      const handleClick = vi.fn();
      const validationGroup = createMockValidationGroup("TestField", true);

      render(
        <ValidationIcon
          validationGroup={validationGroup}
          fieldName="TestField"
          onClick={handleClick}
        />
      );

      const icon = screen.getByLabelText(/Validation status for TestField/);
      fireEvent.click(icon);

      expect(handleClick).toHaveBeenCalledTimes(1);
    });

    it("should have cursor-pointer class when onClick is provided", () => {
      const validationGroup = createMockValidationGroup("TestField", true);

      render(
        <ValidationIcon
          validationGroup={validationGroup}
          fieldName="TestField"
          onClick={() => {}}
        />
      );

      const wrapper = screen.getByLabelText(/Validation status for TestField/);
      expect(wrapper).toHaveClass("cursor-pointer");
    });

    it("should not have cursor-pointer class when onClick is not provided", () => {
      const validationGroup = createMockValidationGroup("TestField", true);

      render(
        <ValidationIcon
          validationGroup={validationGroup}
          fieldName="TestField"
        />
      );

      const wrapper = screen.getByLabelText(/Validation status for TestField/);
      expect(wrapper).not.toHaveClass("cursor-pointer");
    });

    it("should have button role when onClick is provided", () => {
      const validationGroup = createMockValidationGroup("TestField", true);

      render(
        <ValidationIcon
          validationGroup={validationGroup}
          fieldName="TestField"
          onClick={() => {}}
        />
      );

      const wrapper = screen.getByRole("button");
      expect(wrapper).toBeInTheDocument();
    });

    it("should not have button role when onClick is not provided", () => {
      const validationGroup = createMockValidationGroup("TestField", true);

      render(
        <ValidationIcon
          validationGroup={validationGroup}
          fieldName="TestField"
        />
      );

      expect(screen.queryByRole("button")).not.toBeInTheDocument();
    });
  });

  describe("Keyboard Accessibility", () => {
    it("should call onClick on Enter key press", () => {
      const handleClick = vi.fn();
      const validationGroup = createMockValidationGroup("TestField", true);

      render(
        <ValidationIcon
          validationGroup={validationGroup}
          fieldName="TestField"
          onClick={handleClick}
        />
      );

      const icon = screen.getByLabelText(/Validation status for TestField/);
      fireEvent.keyDown(icon, { key: "Enter" });

      expect(handleClick).toHaveBeenCalledTimes(1);
    });

    it("should call onClick on Space key press", () => {
      const handleClick = vi.fn();
      const validationGroup = createMockValidationGroup("TestField", true);

      render(
        <ValidationIcon
          validationGroup={validationGroup}
          fieldName="TestField"
          onClick={handleClick}
        />
      );

      const icon = screen.getByLabelText(/Validation status for TestField/);
      fireEvent.keyDown(icon, { key: " " });

      expect(handleClick).toHaveBeenCalledTimes(1);
    });

    it("should not call onClick on other key presses", () => {
      const handleClick = vi.fn();
      const validationGroup = createMockValidationGroup("TestField", true);

      render(
        <ValidationIcon
          validationGroup={validationGroup}
          fieldName="TestField"
          onClick={handleClick}
        />
      );

      const icon = screen.getByLabelText(/Validation status for TestField/);
      fireEvent.keyDown(icon, { key: "Tab" });
      fireEvent.keyDown(icon, { key: "Escape" });

      expect(handleClick).not.toHaveBeenCalled();
    });

    it("should have tabIndex 0 when onClick is provided", () => {
      const validationGroup = createMockValidationGroup("TestField", true);

      render(
        <ValidationIcon
          validationGroup={validationGroup}
          fieldName="TestField"
          onClick={() => {}}
        />
      );

      const wrapper = screen.getByLabelText(/Validation status for TestField/);
      expect(wrapper).toHaveAttribute("tabIndex", "0");
    });
  });
});
