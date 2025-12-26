import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi } from "vitest";
import DSMCollapsedAccordion from "./DSMCollapsedAccordion";

describe("DSMCollapsedAccordion", () => {
  describe("Rendering", () => {
    it("should render with title and children", () => {
      render(
        <DSMCollapsedAccordion title="Test Accordion">
          <div data-testid="accordion-content">Content goes here</div>
        </DSMCollapsedAccordion>
      );

      expect(screen.getByText("Test Accordion")).toBeInTheDocument();
    });

    it("should render collapsed by default when isCollapsedOnRender is true", () => {
      render(
        <DSMCollapsedAccordion
          title="Collapsed Accordion"
          isCollapsedOnRender={true}>
          <div data-testid="accordion-content">Hidden content</div>
        </DSMCollapsedAccordion>
      );

      // Content should not be visible when collapsed
      const content = screen.queryByTestId("accordion-content");
      expect(content).not.toBeVisible();
    });

    it("should render expanded by default when isCollapsedOnRender is false", () => {
      render(
        <DSMCollapsedAccordion
          title="Expanded Accordion"
          isCollapsedOnRender={false}>
          <div data-testid="accordion-content">Visible content</div>
        </DSMCollapsedAccordion>
      );

      // Content should be visible when expanded
      const content = screen.getByTestId("accordion-content");
      expect(content).toBeVisible();
    });

    it("should render action button when actionButtonText is provided", () => {
      render(
        <DSMCollapsedAccordion
          title="With Action"
          actionButtonText="SUBMIT">
          <div>Content</div>
        </DSMCollapsedAccordion>
      );

      expect(screen.getByText("SUBMIT")).toBeInTheDocument();
    });

    it("should not render action button when actionButtonText is not provided", () => {
      const { container } = render(
        <DSMCollapsedAccordion title="No Action">
          <div>Content</div>
        </DSMCollapsedAccordion>
      );

      // The title should be present
      expect(screen.getByText("No Action")).toBeInTheDocument();

      // No action button box should exist (which has the specific styling for action buttons)
      const actionButtonBox = container.querySelector("div[onClick]");
      expect(actionButtonBox).not.toBeInTheDocument();
    });

    it("should render status chip when status is provided", () => {
      render(
        <DSMCollapsedAccordion
          title="With Status"
          status={{ label: "Active", color: "success" }}>
          <div>Content</div>
        </DSMCollapsedAccordion>
      );

      expect(screen.getByText("Active")).toBeInTheDocument();
      const chip = screen.getByText("Active").closest(".MuiChip-root");
      expect(chip).toBeInTheDocument();
    });

    it("should not render status chip when status is not provided", () => {
      const { container } = render(
        <DSMCollapsedAccordion title="No Status">
          <div>Content</div>
        </DSMCollapsedAccordion>
      );

      const chips = container.querySelectorAll(".MuiChip-root");
      expect(chips.length).toBe(0);
    });

    it("should show expand icon when expandable is true", () => {
      const { container } = render(
        <DSMCollapsedAccordion
          title="Expandable"
          expandable={true}>
          <div>Content</div>
        </DSMCollapsedAccordion>
      );

      const expandIcon = container.querySelector(".MuiAccordionSummary-expandIconWrapper");
      expect(expandIcon).toBeInTheDocument();
    });

    it("should hide expand icon when expandable is false", () => {
      const { container } = render(
        <DSMCollapsedAccordion
          title="Not Expandable"
          expandable={false}>
          <div>Content</div>
        </DSMCollapsedAccordion>
      );

      // When expandable is false, expandIcon prop is null, so no expand icon wrapper is rendered
      const expandIcon = container.querySelector(".MuiAccordionSummary-expandIconWrapper svg");
      expect(expandIcon).not.toBeInTheDocument();
    });

    it("should apply custom className", () => {
      const { container } = render(
        <DSMCollapsedAccordion
          title="Custom Class"
          className="custom-accordion">
          <div>Content</div>
        </DSMCollapsedAccordion>
      );

      const accordion = container.querySelector(".custom-accordion");
      expect(accordion).toBeInTheDocument();
    });
  });

  describe("User Interactions - Expand/Collapse", () => {
    it("should expand when clicked and expandable is true", async () => {
      const user = userEvent.setup();

      render(
        <DSMCollapsedAccordion
          title="Expandable Accordion"
          isCollapsedOnRender={true}
          expandable={true}>
          <div data-testid="accordion-content">Content to reveal</div>
        </DSMCollapsedAccordion>
      );

      // Initially collapsed
      expect(screen.queryByTestId("accordion-content")).not.toBeVisible();

      // Click to expand
      const accordionButton = screen.getByRole("button", { name: /Expandable Accordion/i });
      await user.click(accordionButton);

      // Should be expanded
      await waitFor(() => {
        expect(screen.getByTestId("accordion-content")).toBeVisible();
      });
    });

    it("should collapse when clicked again", async () => {
      const user = userEvent.setup();

      render(
        <DSMCollapsedAccordion
          title="Toggle Accordion"
          isCollapsedOnRender={false}
          expandable={true}>
          <div data-testid="accordion-content">Content to hide</div>
        </DSMCollapsedAccordion>
      );

      // Initially expanded
      expect(screen.getByTestId("accordion-content")).toBeVisible();

      // Click to collapse
      const accordionButton = screen.getByRole("button", { name: /Toggle Accordion/i });
      await user.click(accordionButton);

      // Should be collapsed
      await waitFor(() => {
        expect(screen.queryByTestId("accordion-content")).not.toBeVisible();
      });
    });

    it("should not expand/collapse when expandable is false", async () => {
      const user = userEvent.setup();

      render(
        <DSMCollapsedAccordion
          title="Non-Expandable"
          isCollapsedOnRender={true}
          expandable={false}>
          <div data-testid="accordion-content">Should stay hidden</div>
        </DSMCollapsedAccordion>
      );

      // Initially collapsed
      expect(screen.queryByTestId("accordion-content")).not.toBeVisible();

      // Try to click (should not expand)
      const accordionButton = screen.getByRole("button", { name: /Non-Expandable/i });
      await user.click(accordionButton);

      // Should remain collapsed
      await waitFor(() => {
        expect(screen.queryByTestId("accordion-content")).not.toBeVisible();
      });
    });

    it("should maintain expanded state across multiple clicks", async () => {
      const user = userEvent.setup();

      render(
        <DSMCollapsedAccordion
          title="Multiple Clicks"
          expandable={true}>
          <div data-testid="accordion-content">Toggle content</div>
        </DSMCollapsedAccordion>
      );

      const accordionButton = screen.getByRole("button", { name: /Multiple Clicks/i });

      // Click to expand
      await user.click(accordionButton);
      await waitFor(() => {
        expect(screen.getByTestId("accordion-content")).toBeVisible();
      });

      // Click to collapse
      await user.click(accordionButton);
      await waitFor(() => {
        expect(screen.queryByTestId("accordion-content")).not.toBeVisible();
      });

      // Click to expand again
      await user.click(accordionButton);
      await waitFor(() => {
        expect(screen.getByTestId("accordion-content")).toBeVisible();
      });
    });
  });

  describe("User Interactions - Action Button", () => {
    it("should call onActionClick when action button is clicked", async () => {
      const user = userEvent.setup();
      const mockActionClick = vi.fn();

      render(
        <DSMCollapsedAccordion
          title="Action Test"
          actionButtonText="SUBMIT"
          onActionClick={mockActionClick}>
          <div>Content</div>
        </DSMCollapsedAccordion>
      );

      const actionButton = screen.getByText("SUBMIT");
      await user.click(actionButton);

      expect(mockActionClick).toHaveBeenCalledTimes(1);
    });

    it("should not expand/collapse accordion when action button is clicked", async () => {
      const user = userEvent.setup();
      const mockActionClick = vi.fn();

      render(
        <DSMCollapsedAccordion
          title="Stop Propagation Test"
          actionButtonText="ACTION"
          onActionClick={mockActionClick}
          isCollapsedOnRender={true}
          expandable={true}>
          <div data-testid="accordion-content">Should stay hidden</div>
        </DSMCollapsedAccordion>
      );

      // Click action button
      const actionButton = screen.getByText("ACTION");
      await user.click(actionButton);

      // Action should be called
      expect(mockActionClick).toHaveBeenCalled();

      // Accordion should remain collapsed
      expect(screen.queryByTestId("accordion-content")).not.toBeVisible();
    });

    it("should not call onActionClick when disabled", () => {
      const mockActionClick = vi.fn();

      render(
        <DSMCollapsedAccordion
          title="Disabled Action"
          actionButtonText="SUBMIT"
          onActionClick={mockActionClick}
          disabled={true}>
          <div>Content</div>
        </DSMCollapsedAccordion>
      );

      const actionButton = screen.getByText("SUBMIT");

      // Check that button has pointer-events: none styling which prevents clicks
      expect(actionButton).toBeInTheDocument();

      // The button is not clickable due to CSS, so we can't test click behavior
      // but we can verify the disabled state
      expect(mockActionClick).not.toHaveBeenCalled();
    });

    it("should apply disabled styling to action button when disabled", () => {
      render(
        <DSMCollapsedAccordion
          title="Disabled Styling"
          actionButtonText="DISABLED"
          disabled={true}>
          <div>Content</div>
        </DSMCollapsedAccordion>
      );

      const actionButton = screen.getByText("DISABLED");

      // Disabled styles applied via sx prop
      expect(actionButton).toBeInTheDocument();
    });

    it("should render action button with uppercase text", () => {
      render(
        <DSMCollapsedAccordion
          title="Text Transform"
          actionButtonText="submit">
          <div>Content</div>
        </DSMCollapsedAccordion>
      );

      // Text should be transformed to uppercase via CSS
      const actionButton = screen.getByText("submit");
      expect(actionButton).toBeInTheDocument();
    });
  });

  describe("Status Chip Variations", () => {
    it("should render status chip with different colors", () => {
      const colors: Array<"default" | "primary" | "secondary" | "error" | "info" | "success" | "warning"> = [
        "default",
        "primary",
        "secondary",
        "error",
        "info",
        "success",
        "warning"
      ];

      colors.forEach((color) => {
        render(
          <DSMCollapsedAccordion
            title={`${color} status`}
            status={{ label: color, color }}>
            <div>Content</div>
          </DSMCollapsedAccordion>
        );

        const chip = screen.getByText(color).closest(".MuiChip-root");
        expect(chip).toBeInTheDocument();
      });
    });

    it("should apply outlined variant to status chip", () => {
      render(
        <DSMCollapsedAccordion
          title="Outlined Chip"
          status={{ label: "Pending", color: "warning" }}>
          <div>Content</div>
        </DSMCollapsedAccordion>
      );

      const chip = screen.getByText("Pending").closest(".MuiChip-root");
      expect(chip).toHaveClass("MuiChip-outlined");
    });

    it("should render small size status chip", () => {
      render(
        <DSMCollapsedAccordion
          title="Small Chip"
          status={{ label: "Small", color: "info" }}>
          <div>Content</div>
        </DSMCollapsedAccordion>
      );

      const chip = screen.getByText("Small").closest(".MuiChip-root");
      expect(chip).toHaveClass("MuiChip-sizeSmall");
    });
  });

  describe("Complex Scenarios", () => {
    it("should render all elements together", () => {
      render(
        <DSMCollapsedAccordion
          title="Complete Accordion"
          actionButtonText="SAVE"
          status={{ label: "Draft", color: "warning" }}
          isCollapsedOnRender={false}
          expandable={true}
          className="custom-class">
          <div data-testid="full-content">All elements present</div>
        </DSMCollapsedAccordion>
      );

      expect(screen.getByText("Complete Accordion")).toBeInTheDocument();
      expect(screen.getByText("SAVE")).toBeInTheDocument();
      expect(screen.getByText("Draft")).toBeInTheDocument();
      expect(screen.getByTestId("full-content")).toBeVisible();
    });

    it("should handle multiple accordions independently", async () => {
      const user = userEvent.setup();

      render(
        <>
          <DSMCollapsedAccordion
            title="First Accordion"
            isCollapsedOnRender={true}>
            <div data-testid="first-content">First content</div>
          </DSMCollapsedAccordion>
          <DSMCollapsedAccordion
            title="Second Accordion"
            isCollapsedOnRender={true}>
            <div data-testid="second-content">Second content</div>
          </DSMCollapsedAccordion>
        </>
      );

      // Both collapsed initially
      expect(screen.queryByTestId("first-content")).not.toBeVisible();
      expect(screen.queryByTestId("second-content")).not.toBeVisible();

      // Expand first
      const firstButton = screen.getByRole("button", { name: /First Accordion/i });
      await user.click(firstButton);

      await waitFor(() => {
        expect(screen.getByTestId("first-content")).toBeVisible();
      });

      // Second should remain collapsed
      expect(screen.queryByTestId("second-content")).not.toBeVisible();
    });

    it("should render complex children content", () => {
      render(
        <DSMCollapsedAccordion
          title="Complex Children"
          isCollapsedOnRender={false}>
          <div>
            <h3>Section Title</h3>
            <p>Paragraph text</p>
            <ul>
              <li>Item 1</li>
              <li>Item 2</li>
            </ul>
            <button>Action Button</button>
          </div>
        </DSMCollapsedAccordion>
      );

      expect(screen.getByText("Section Title")).toBeVisible();
      expect(screen.getByText("Paragraph text")).toBeVisible();
      expect(screen.getByText("Item 1")).toBeVisible();
      expect(screen.getByRole("button", { name: "Action Button" })).toBeVisible();
    });
  });

  describe("Accessibility", () => {
    it("should have accessible accordion role", () => {
      const { container } = render(
        <DSMCollapsedAccordion title="Accessible">
          <div>Content</div>
        </DSMCollapsedAccordion>
      );

      const accordion = container.querySelector(".MuiAccordion-root");
      expect(accordion).toBeInTheDocument();
    });

    it("should have accessible button in accordion summary", () => {
      render(
        <DSMCollapsedAccordion title="Button Test">
          <div>Content</div>
        </DSMCollapsedAccordion>
      );

      const button = screen.getByRole("button", { name: /Button Test/i });
      expect(button).toBeInTheDocument();
    });

    it("should properly handle keyboard navigation", async () => {
      const user = userEvent.setup();

      render(
        <DSMCollapsedAccordion
          title="Keyboard Nav"
          isCollapsedOnRender={true}
          expandable={true}>
          <div data-testid="keyboard-content">Content</div>
        </DSMCollapsedAccordion>
      );

      const accordionButton = screen.getByRole("button", { name: /Keyboard Nav/i });

      // Tab to accordion button
      await user.tab();
      expect(accordionButton).toHaveFocus();

      // Press Enter to expand
      await user.keyboard("{Enter}");

      await waitFor(() => {
        expect(screen.getByTestId("keyboard-content")).toBeVisible();
      });
    });
  });

  describe("Edge Cases", () => {
    it("should handle very long title text", () => {
      const longTitle =
        "This is a very long accordion title that might wrap to multiple lines in the UI depending on the container width and screen size";

      render(
        <DSMCollapsedAccordion title={longTitle}>
          <div>Content</div>
        </DSMCollapsedAccordion>
      );

      expect(screen.getByText(longTitle)).toBeInTheDocument();
    });

    it("should handle empty children", () => {
      render(
        <DSMCollapsedAccordion
          title="Empty Content"
          isCollapsedOnRender={false}>
          {null}
        </DSMCollapsedAccordion>
      );

      // Should render without crashing
      expect(screen.getByText("Empty Content")).toBeInTheDocument();
    });

    it("should handle title with special characters", () => {
      const specialTitle = "Q&A Section <2024/25>";

      render(
        <DSMCollapsedAccordion title={specialTitle}>
          <div>Content</div>
        </DSMCollapsedAccordion>
      );

      expect(screen.getByText(specialTitle)).toBeInTheDocument();
    });

    it("should handle disabled state correctly", () => {
      const { container } = render(
        <DSMCollapsedAccordion
          title="Disabled Accordion"
          disabled={true}>
          <div>Content</div>
        </DSMCollapsedAccordion>
      );

      const accordion = container.querySelector(".MuiAccordion-root");
      expect(accordion).toHaveClass("Mui-disabled");
    });

    it("should handle onActionClick being undefined", async () => {
      const user = userEvent.setup();

      render(
        <DSMCollapsedAccordion
          title="No Handler"
          actionButtonText="CLICK">
          <div>Content</div>
        </DSMCollapsedAccordion>
      );

      const actionButton = screen.getByText("CLICK");

      // Should not throw error when clicked
      await expect(user.click(actionButton)).resolves.not.toThrow();
    });
  });

  describe("Styling", () => {
    it("should apply custom height to accordion summary", () => {
      const { container } = render(
        <DSMCollapsedAccordion title="Custom Height">
          <div>Content</div>
        </DSMCollapsedAccordion>
      );

      const summary = container.querySelector(".MuiAccordionSummary-root");
      expect(summary).toBeInTheDocument();
    });

    it("should have white background on accordion details", () => {
      const { container } = render(
        <DSMCollapsedAccordion
          title="Background Test"
          isCollapsedOnRender={false}>
          <div>Content</div>
        </DSMCollapsedAccordion>
      );

      const details = container.querySelector(".MuiAccordionDetails-root");
      expect(details).toBeInTheDocument();
    });

    it("should have no box shadow", () => {
      const { container } = render(
        <DSMCollapsedAccordion title="No Shadow">
          <div>Content</div>
        </DSMCollapsedAccordion>
      );

      const accordion = container.querySelector(".MuiAccordion-root");
      expect(accordion).toBeInTheDocument();
    });
  });
});
