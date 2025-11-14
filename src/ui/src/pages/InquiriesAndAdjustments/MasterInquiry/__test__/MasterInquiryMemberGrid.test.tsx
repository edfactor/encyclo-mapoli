import { render } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { SearchResponse } from "../hooks/useMasterInquiryReducer";
import MasterInquiryMemberGrid from "../MasterInquiryMemberGrid";

/**
 * Master Inquiry Grid Tests - Performance Fixes
 * These tests verify the grid component badge link styling.
 * Focus: Ensure badges look like links (#0258A5 color) without causing navigation.
 */

describe("MasterInquiryMemberGrid - Badge Link Styling", { timeout: 40000 }, () => {
  // Minimal mock data - just enough to render the component
  const mockSearchResults: SearchResponse = {
    results: [],
    total: 0
  };

  const mockPagination = {
    pageNumber: 1,
    pageSize: 25,
    sortParams: {
      sortBy: "badgeNumber",
      isSortDescending: false
    }
  };

  const mockOnMemberSelect = vi.fn();
  const mockOnPaginationChange = vi.fn();
  const mockOnSortChange = vi.fn();

  describe("CSS Styling for Badge Links", () => {
    it("should render with badge-link-style CSS rules using app standard color #0258A5", () => {
      const { container } = render(
        <MasterInquiryMemberGrid
          searchResults={mockSearchResults}
          onMemberSelect={mockOnMemberSelect}
          memberGridPagination={mockPagination}
          onPaginationChange={mockOnPaginationChange}
          onSortChange={mockOnSortChange}
        />
      );

      // Check for style tag in the component
      const styleTag = container.querySelector("style");
      expect(styleTag).toBeTruthy();

      // Verify CSS rules are present with correct application color
      const styleContent = styleTag?.textContent || "";

      // Main styling rules
      expect(styleContent).toContain(".badge-link-style");
      expect(styleContent).toContain("color: #0258A5"); // Application standard link color
      expect(styleContent).toContain("text-decoration: underline");
      expect(styleContent).toContain("cursor: pointer");
      expect(styleContent).toContain("font-weight: 500");

      // Hover state
      expect(styleContent).toContain(".badge-link-style:hover");
      expect(styleContent).toContain("color: #014073"); // Darker blue for hover
    });

    it("should NOT use Material-UI default blue color", () => {
      const { container } = render(
        <MasterInquiryMemberGrid
          searchResults={mockSearchResults}
          onMemberSelect={mockOnMemberSelect}
          memberGridPagination={mockPagination}
          onPaginationChange={mockOnPaginationChange}
          onSortChange={mockOnSortChange}
        />
      );

      const styleTag = container.querySelector("style");
      const styleContent = styleTag?.textContent || "";

      // Verify we're NOT using old MUI blue (#1976d2)
      expect(styleContent).not.toContain("#1976d2");
      // Verify we ARE using app standard color
      expect(styleContent).toContain("#0258A5");
    });

    it("should have all required CSS properties for link appearance", () => {
      const { container } = render(
        <MasterInquiryMemberGrid
          searchResults={mockSearchResults}
          onMemberSelect={mockOnMemberSelect}
          memberGridPagination={mockPagination}
          onPaginationChange={mockOnPaginationChange}
          onSortChange={mockOnSortChange}
        />
      );

      const styleTag = container.querySelector("style");
      const styleContent = styleTag?.textContent || "";

      // All properties needed for link appearance
      const requiredStyles = [
        "color:",
        "text-decoration:",
        "cursor:",
        "font-weight:",
        "!important" // Ensures styles override ag-Grid defaults
      ];

      requiredStyles.forEach((style) => {
        expect(styleContent).toContain(style);
      });
    });
  });

  describe("Performance - No Navigation", () => {
    it("should not contain master-inquiry navigation anchor tags", () => {
      const { container } = render(
        <MasterInquiryMemberGrid
          searchResults={mockSearchResults}
          onMemberSelect={mockOnMemberSelect}
          memberGridPagination={mockPagination}
          onPaginationChange={mockOnPaginationChange}
          onSortChange={mockOnSortChange}
        />
      );

      // Grid should not render any anchor tags with href="/master-inquiry/..."
      // (using cellClass instead of renderAsLink prevents <a> tag generation)
      const anchorTags = container.querySelectorAll("a[href*='master-inquiry']");
      expect(anchorTags.length).toBe(0);
    });
  });
});
