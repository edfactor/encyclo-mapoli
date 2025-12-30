import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import LabelValueSection from "./LabelValueSection";

describe("LabelValueSection", () => {
  describe("Rendering", () => {
    it("should render without title when title prop is not provided", () => {
      const data = [
        { label: "Name", value: "John Doe" },
        { label: "Email", value: "john@example.com" }
      ];

      render(<LabelValueSection data={data} />);

      expect(screen.queryByText(/^[A-Z\s]+$/)).not.toBeInTheDocument();
      expect(screen.getByText("Name")).toBeInTheDocument();
      expect(screen.getByText("John Doe")).toBeInTheDocument();
    });

    it("should render title when title prop is provided", () => {
      const data = [{ label: "Field", value: "Value" }];

      render(
        <LabelValueSection
          title="Contact Information"
          data={data}
        />
      );

      expect(screen.getByText("Contact Information")).toBeInTheDocument();
    });

    it("should render all label-value pairs", () => {
      const data = [
        { label: "First Name", value: "John" },
        { label: "Last Name", value: "Doe" },
        { label: "Age", value: "30" }
      ];

      render(<LabelValueSection data={data} />);

      expect(screen.getByText("First Name")).toBeInTheDocument();
      expect(screen.getByText("John")).toBeInTheDocument();
      expect(screen.getByText("Last Name")).toBeInTheDocument();
      expect(screen.getByText("Doe")).toBeInTheDocument();
      expect(screen.getByText("Age")).toBeInTheDocument();
      expect(screen.getByText("30")).toBeInTheDocument();
    });

    it("should render value without label when label is empty string", () => {
      const data = [
        { label: "", value: "Standalone value" },
        { label: "Normal Label", value: "Normal value" }
      ];

      render(<LabelValueSection data={data} />);

      expect(screen.getByText("Standalone value")).toBeInTheDocument();
      expect(screen.getByText("Normal Label")).toBeInTheDocument();
      expect(screen.getByText("Normal value")).toBeInTheDocument();
    });

    it("should handle React nodes as values", () => {
      const data = [
        {
          label: "Status",
          value: <span data-testid="custom-status">Active</span>
        },
        {
          label: "Actions",
          value: <button data-testid="action-button">Click me</button>
        }
      ];

      render(<LabelValueSection data={data} />);

      expect(screen.getByTestId("custom-status")).toBeInTheDocument();
      expect(screen.getByTestId("action-button")).toBeInTheDocument();
      expect(screen.getByText("Status")).toBeInTheDocument();
      expect(screen.getByText("Actions")).toBeInTheDocument();
    });

    it("should render empty data array without errors", () => {
      render(<LabelValueSection data={[]} />);

      // Should render without crashing, but no content
      const container = document.querySelector("div");
      expect(container).toBeInTheDocument();
    });
  });

  describe("Label Styling", () => {
    it("should apply default label styles when no custom styles provided", () => {
      const data = [{ label: "Default", value: "Value" }];

      render(<LabelValueSection data={data} />);

      const labelElement = screen.getByText("Default").closest("p");
      expect(labelElement).toHaveClass("MuiTypography-body2");
    });

    it("should apply custom labelColor", () => {
      const data = [
        {
          label: "Error Label",
          value: "Value",
          labelColor: "error" as const
        }
      ];

      render(<LabelValueSection data={data} />);

      const labelElement = screen.getByText("Error Label");
      expect(labelElement).toBeInTheDocument();
    });

    it("should apply custom labelVariant", () => {
      const data = [
        {
          label: "Heading",
          value: "Value",
          labelVariant: "h6" as const
        }
      ];

      render(<LabelValueSection data={data} />);

      const labelElement = screen.getByText("Heading").closest("h6");
      expect(labelElement).toBeInTheDocument();
      expect(labelElement).toHaveClass("MuiTypography-h6");
    });

    it("should apply custom labelWeight", () => {
      const data = [
        {
          label: "Light Label",
          value: "Value",
          labelWeight: "lighter" as const
        },
        {
          label: "Normal Label",
          value: "Value",
          labelWeight: "normal" as const
        }
      ];

      render(<LabelValueSection data={data} />);

      expect(screen.getByText("Light Label")).toBeInTheDocument();
      expect(screen.getByText("Normal Label")).toBeInTheDocument();
    });

    it("should apply multiple custom label styles together", () => {
      const data = [
        {
          label: "Custom Styled",
          value: "Value",
          labelColor: "primary" as const,
          labelVariant: "subtitle1" as const,
          labelWeight: "bold" as const
        }
      ];

      render(<LabelValueSection data={data} />);

      const labelElement = screen.getByText("Custom Styled");
      expect(labelElement).toBeInTheDocument();
      expect(labelElement).toHaveClass("MuiTypography-subtitle1");
    });
  });

  describe("Complex Data Scenarios", () => {
    it("should handle mixed styling across multiple items", () => {
      const data = [
        {
          label: "Bold Primary",
          value: "Value 1",
          labelColor: "primary" as const,
          labelWeight: "bold" as const
        },
        {
          label: "Error Heading",
          value: "Value 2",
          labelColor: "error" as const,
          labelVariant: "h6" as const
        },
        {
          label: "Default",
          value: "Value 3"
        }
      ];

      render(<LabelValueSection data={data} />);

      expect(screen.getByText("Bold Primary")).toBeInTheDocument();
      expect(screen.getByText("Error Heading")).toBeInTheDocument();
      expect(screen.getByText("Default")).toBeInTheDocument();
      expect(screen.getByText("Value 1")).toBeInTheDocument();
      expect(screen.getByText("Value 2")).toBeInTheDocument();
      expect(screen.getByText("Value 3")).toBeInTheDocument();
    });

    it("should handle numeric values", () => {
      const data = [
        { label: "Count", value: 42 },
        { label: "Price", value: 99.99 }
      ];

      render(<LabelValueSection data={data} />);

      expect(screen.getByText("Count")).toBeInTheDocument();
      expect(screen.getByText("42")).toBeInTheDocument();
      expect(screen.getByText("Price")).toBeInTheDocument();
      expect(screen.getByText("99.99")).toBeInTheDocument();
    });

    it("should handle boolean values", () => {
      const data = [
        { label: "Active", value: "true" },
        { label: "Disabled", value: "false" }
      ];

      render(<LabelValueSection data={data} />);

      expect(screen.getByText("Active")).toBeInTheDocument();
      expect(screen.getByText("true")).toBeInTheDocument();
      expect(screen.getByText("Disabled")).toBeInTheDocument();
      expect(screen.getByText("false")).toBeInTheDocument();
    });

    it("should handle null and undefined values gracefully", () => {
      const data = [
        { label: "Null Value", value: null },
        { label: "Undefined Value", value: undefined },
        { label: "Empty String", value: "" }
      ];

      render(<LabelValueSection data={data} />);

      expect(screen.getByText("Null Value")).toBeInTheDocument();
      expect(screen.getByText("Undefined Value")).toBeInTheDocument();
      expect(screen.getByText("Empty String")).toBeInTheDocument();
    });
  });

  describe("Accessibility", () => {
    it("should render with proper typography elements", () => {
      const data = [{ label: "Accessible Label", value: "Accessible Value" }];

      render(
        <LabelValueSection
          title="Section Title"
          data={data}
        />
      );

      const title = screen.getByText("Section Title");
      // Typography component renders as span by default for overline variant
      expect(title).toBeInTheDocument();
      expect(title).toHaveClass("MuiTypography-overline");
    });

    it("should maintain semantic structure with Grid layout", () => {
      const data = [
        { label: "First", value: "1" },
        { label: "Second", value: "2" }
      ];

      const { container } = render(<LabelValueSection data={data} />);

      const gridContainers = container.querySelectorAll(".MuiGrid-container");
      expect(gridContainers.length).toBe(2); // One for each data item
    });
  });

  describe("Edge Cases", () => {
    it("should handle very long labels and values", () => {
      const longLabel = "This is a very long label that might wrap to multiple lines in the UI";
      const longValue =
        "This is a very long value that contains a lot of text and might also wrap to multiple lines depending on the container width";

      const data = [{ label: longLabel, value: longValue }];

      render(<LabelValueSection data={data} />);

      expect(screen.getByText(longLabel)).toBeInTheDocument();
      expect(screen.getByText(longValue)).toBeInTheDocument();
    });

    it("should handle special characters in labels and values", () => {
      const data = [
        { label: "Email <test>", value: "user@example.com" },
        { label: "Math & Science", value: "A+" },
        { label: "Quote's Test", value: "It's working" }
      ];

      render(<LabelValueSection data={data} />);

      expect(screen.getByText("Email <test>")).toBeInTheDocument();
      expect(screen.getByText("user@example.com")).toBeInTheDocument();
      expect(screen.getByText("Math & Science")).toBeInTheDocument();
      expect(screen.getByText("A+")).toBeInTheDocument();
    });

    it("should render single item data array", () => {
      const data = [{ label: "Single", value: "Item" }];

      render(<LabelValueSection data={data} />);

      expect(screen.getByText("Single")).toBeInTheDocument();
      expect(screen.getByText("Item")).toBeInTheDocument();
    });

    it("should handle data with mixed empty and non-empty labels", () => {
      const data = [
        { label: "First", value: "Has Label" },
        { label: "", value: "No Label" },
        { label: "Third", value: "Has Label Again" }
      ];

      render(<LabelValueSection data={data} />);

      expect(screen.getByText("First")).toBeInTheDocument();
      expect(screen.getByText("Has Label")).toBeInTheDocument();
      expect(screen.getByText("No Label")).toBeInTheDocument();
      expect(screen.getByText("Third")).toBeInTheDocument();
      expect(screen.getByText("Has Label Again")).toBeInTheDocument();
    });
  });
});
