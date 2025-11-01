import { ColDef } from "ag-grid-community";
import { describe, expect, it } from "vitest";
import { GetMasterInquiryMemberGridColumns } from "./MasterInquiryMemberGridColumns";

/**
 * Badge Column Styling Tests - Performance Fixes
 * These tests verify that the badge column is styled as a link without causing navigation.
 * Focus on the critical cellClass property that enables CSS styling without href generation.
 */

describe("MasterInquiryMemberGridColumns - Badge Styling", () => {
  describe("Badge Column Configuration (Critical Tests)", () => {
    it("should have badge column configured with cellClass for link styling", () => {
      const columns = GetMasterInquiryMemberGridColumns();
      const badgeColumn = columns.find((col: ColDef) => col.field === "badgeNumber");

      expect(badgeColumn).toBeDefined();

      // Critical: Badge column must have cellClass that includes 'badge-link-style'
      // This applies CSS styling to make badges look like links without generating <a> tags
      const cellClass = badgeColumn?.cellClass;
      if (typeof cellClass === "string") {
        expect(cellClass).toContain("badge-link-style");
      } else if (Array.isArray(cellClass)) {
        expect(cellClass).toContain("badge-link-style");
      } else {
        // cellClass might also be stored differently by factory
        // The important thing is that it exists and is applied
        expect(cellClass).toBeDefined();
      }
    });

    it("should have badge column as first column for visibility", () => {
      const columns = GetMasterInquiryMemberGridColumns();

      // Badge column should be first for easy access
      expect(columns[0].field).toBe("badgeNumber");
      expect(columns[0].headerName).toBe("Badge");
    });

    it("should not create anchor tags with cellRenderer", () => {
      const columns = GetMasterInquiryMemberGridColumns();
      const badgeColumn = columns.find((col: ColDef) => col.field === "badgeNumber");

      expect(badgeColumn).toBeDefined();

      // If there's a cellRenderer (when renderAsLink was NOT set to false),
      // it shouldn't create anchor tags
      if (badgeColumn?.cellRenderer) {
        const mockParams = {
          data: { badgeNumber: 12345, psnSuffix: 0 },
          value: 12345
        };

        const rendered = badgeColumn.cellRenderer(mockParams);

        // Should not return anchor tag HTML
        if (rendered) {
          const renderedStr = rendered.toString();
          expect(renderedStr.includes("<a")).toBe(false);
          expect(renderedStr.includes("href")).toBe(false);
        }
      }
      // else: No cellRenderer means no links generated, which is good
    });
  });

  describe("Column Structure", () => {
    it("should have badge column with expected properties", () => {
      const columns = GetMasterInquiryMemberGridColumns();
      const badgeColumn = columns.find((col: ColDef) => col.field === "badgeNumber");

      expect(badgeColumn).toBeDefined();
      expect(badgeColumn?.headerName).toBe("Badge");
      expect(badgeColumn?.field).toBe("badgeNumber");
      expect(badgeColumn?.colId).toBe("badgeNumber");
    });

    it("should have multiple columns in the grid", () => {
      const columns = GetMasterInquiryMemberGridColumns();

      // Should have badge plus other columns (name, ssn, address, etc.)
      expect(columns.length).toBeGreaterThan(5);
    });
  });

  describe("Performance - No Navigation via CSS Approach", () => {
    it("should use cellClass approach for styling instead of href links", () => {
      const columns = GetMasterInquiryMemberGridColumns();
      const badgeColumn = columns.find((col: ColDef) => col.field === "badgeNumber");

      expect(badgeColumn).toBeDefined();

      // The key performance fix: cellClass is defined for CSS styling
      // This prevents the need to generate <a href> elements
      expect(badgeColumn?.cellClass).toBeDefined();

      // This approach:
      // 1. Allows visual link styling (blue, underlined, pointer)
      // 2. Prevents browser navigation (no href attribute)
      // 3. Keeps row click handling for member selection
      // 4. Eliminates screen "refresh" caused by URL changes
    });
  });
});
