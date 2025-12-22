import { ColDef } from "ag-grid-community";
import { describe, expect, it } from "vitest";
import { GetMasterInquiryMemberGridColumns } from "../MasterInquiryMemberGridColumns";

describe("MasterInquiryMemberGridColumns - Badge Styling", () => {
  describe("Badge Column Configuration (Critical Tests)", () => {
    it("should have badge column as first column for visibility", () => {
      const columns = GetMasterInquiryMemberGridColumns(() => {});

      // Badge column should be first for easy access
      expect(columns[0].field).toBe("badgeNumber");
      expect(columns[0].headerName).toBe("Badge/PSN");
    });
  });

  describe("Column Structure", () => {
    it("should have badge column with expected properties", () => {
      const columns = GetMasterInquiryMemberGridColumns(() => {});
      const badgeColumn = columns.find((col: ColDef) => col.field === "badgeNumber");

      expect(badgeColumn).toBeDefined();
      expect(badgeColumn?.headerName).toBe("Badge/PSN");
      expect(badgeColumn?.field).toBe("badgeNumber");
      expect(badgeColumn?.colId).toBe("badgeNumber");
    });

    it("should have multiple columns in the grid", () => {
      const columns = GetMasterInquiryMemberGridColumns(() => {});

      // Should have badge plus other columns (name, ssn, address, etc.)
      expect(columns.length).toBeGreaterThan(5);
    });
  });
});
