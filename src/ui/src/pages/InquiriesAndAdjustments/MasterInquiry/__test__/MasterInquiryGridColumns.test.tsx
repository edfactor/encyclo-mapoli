import { ColDef, ITooltipParams } from "ag-grid-community";
import { describe, expect, it } from "vitest";
import { GetMasterInquiryGridColumns } from "../MasterInquiryGridColumns";

describe("MasterInquiryGridColumns - Comment Type QDRO Links", () => {
  describe("Comment Type Column Configuration", () => {
    it("should have comment type column with cellRenderer", () => {
      const columns = GetMasterInquiryGridColumns();
      const commentColumn = columns.find((col: ColDef) => col.field === "commentTypeName");

      expect(commentColumn).toBeDefined();
      expect(commentColumn?.headerName).toBe("Comment Type");
      expect(commentColumn?.cellRenderer).toBeDefined();
    });

    it("should have tooltip configured for QDRO entries", () => {
      const columns = GetMasterInquiryGridColumns();
      const commentColumn = columns.find((col: ColDef) => col.field === "commentTypeName");

      expect(commentColumn).toBeDefined();
      expect(commentColumn?.tooltipValueGetter).toBeDefined();
    });
  });

  describe("Comment Type Cell Renderer - QDRO Links", () => {
    let commentColumn: ColDef | undefined;

    beforeEach(() => {
      const columns = GetMasterInquiryGridColumns();
      commentColumn = columns.find((col: ColDef) => col.field === "commentTypeName");
    });

    it("should render link for QDRO In (type 3) entries", () => {
      const mockParams = {
        data: {
          commentTypeId: 3,
          commentTypeName: "QDRO In",
          xFerQdroId: 123456
        },
        value: "QDRO In"
      };

      const rendered = commentColumn?.cellRenderer?.(mockParams);

      expect(rendered).toBeDefined();
      // Should return a Link component object
      expect(rendered).toHaveProperty("props");
      expect(rendered.props.href).toBe("/master-inquiry/123456");
      expect(rendered.props.children).toBe("QDRO In");
    });

    it("should render link for QDRO Out (type 4) entries", () => {
      const mockParams = {
        data: {
          commentTypeId: 4,
          commentTypeName: "QDRO Out",
          xFerQdroId: 789012
        },
        value: "QDRO Out"
      };

      const rendered = commentColumn?.cellRenderer?.(mockParams);

      expect(rendered).toBeDefined();
      expect(rendered).toHaveProperty("props");
      expect(rendered.props.href).toBe("/master-inquiry/789012");
      expect(rendered.props.children).toBe("QDRO Out");
    });

    it("should render plain text for non-QDRO entries", () => {
      const mockParams = {
        data: {
          commentTypeId: 1,
          commentTypeName: "Regular Comment",
          xFerQdroId: null
        },
        value: "Regular Comment"
      };

      const rendered = commentColumn?.cellRenderer?.(mockParams);

      expect(rendered).toBe("Regular Comment");
    });

    it("should render plain text when xFerQdroId is missing", () => {
      const mockParams = {
        data: {
          commentTypeId: 3,
          commentTypeName: "QDRO In",
          xFerQdroId: null
        },
        value: "QDRO In"
      };

      const rendered = commentColumn?.cellRenderer?.(mockParams);

      expect(rendered).toBe("QDRO In");
    });

    it("should handle multi-digit badge numbers", () => {
      const mockParams = {
        data: {
          commentTypeId: 3,
          commentTypeName: "QDRO In",
          xFerQdroId: 1234567890
        },
        value: "QDRO In"
      };

      const rendered = commentColumn?.cellRenderer?.(mockParams);

      expect(rendered).toBeDefined();
      expect(rendered.props.href).toBe("/master-inquiry/1234567890");
    });

    it("should return value when data is undefined", () => {
      const mockParams = {
        data: undefined,
        value: "Test Value"
      };

      const rendered = commentColumn?.cellRenderer?.(mockParams);

      expect(rendered).toBe("Test Value");
    });
  });

  describe("Tooltip Value Getter - QDRO Information", () => {
    let commentColumn: ColDef | undefined;

    beforeEach(() => {
      const columns = GetMasterInquiryGridColumns();
      commentColumn = columns.find((col: ColDef) => col.field === "commentTypeName");
    });

    it("should show QDRO info for QDRO In (type 3)", () => {
      const mockParams: Partial<ITooltipParams> = {
        data: {
          commentTypeId: 3,
          xFerQdroId: 123456,
          xFerQdroName: "John Doe"
        }
      };

      const tooltip = commentColumn?.tooltipValueGetter?.(mockParams as ITooltipParams);

      expect(tooltip).toBe("Name: John Doe\nBadge:123456");
    });

    it("should show QDRO info for QDRO Out (type 4)", () => {
      const mockParams: Partial<ITooltipParams> = {
        data: {
          commentTypeId: 4,
          xFerQdroId: 789012,
          xFerQdroName: "Jane Smith"
        }
      };

      const tooltip = commentColumn?.tooltipValueGetter?.(mockParams as ITooltipParams);

      expect(tooltip).toBe("Name: Jane Smith\nBadge:789012");
    });

    it("should show N/A when QDRO ID is missing", () => {
      const mockParams: Partial<ITooltipParams> = {
        data: {
          commentTypeId: 3,
          xFerQdroId: null,
          xFerQdroName: "John Doe"
        }
      };

      const tooltip = commentColumn?.tooltipValueGetter?.(mockParams as ITooltipParams);

      expect(tooltip).toBe("Name: John Doe\nBadge:N/A");
    });

    it("should show N/A when QDRO Name is missing", () => {
      const mockParams: Partial<ITooltipParams> = {
        data: {
          commentTypeId: 3,
          xFerQdroId: 123456,
          xFerQdroName: null
        }
      };

      const tooltip = commentColumn?.tooltipValueGetter?.(mockParams as ITooltipParams);

      expect(tooltip).toBe("Name: N/A\nBadge:123456");
    });

    it("should return empty string for non-QDRO entries", () => {
      const mockParams: Partial<ITooltipParams> = {
        data: {
          commentTypeId: 1,
          xFerQdroId: 123456,
          xFerQdroName: "John Doe"
        }
      };

      const tooltip = commentColumn?.tooltipValueGetter?.(mockParams as ITooltipParams);

      expect(tooltip).toBe("");
    });

    it("should return empty string when both QDRO fields are missing", () => {
      const mockParams: Partial<ITooltipParams> = {
        data: {
          commentTypeId: 3,
          xFerQdroId: null,
          xFerQdroName: null
        }
      };

      const tooltip = commentColumn?.tooltipValueGetter?.(mockParams as ITooltipParams);

      expect(tooltip).toBe("");
    });
  });

  describe("Column Structure", () => {
    it("should have all expected columns", () => {
      const columns = GetMasterInquiryGridColumns();

      expect(columns.length).toBeGreaterThan(10);
      
      const columnFields = columns.map((col: ColDef) => col.field).filter(Boolean);
      
      expect(columnFields).toContain("profitYear");
      expect(columnFields).toContain("commentTypeName");
      expect(columnFields).toContain("commentRelatedCheckNumber");
    });

    it("should have comment type column with correct alignment", () => {
      const columns = GetMasterInquiryGridColumns();
      const commentColumn = columns.find((col: ColDef) => col.field === "commentTypeName");

      expect(commentColumn?.headerClass).toBe("left-align");
      expect(commentColumn?.cellClass).toBe("left-align");
      expect(commentColumn?.resizable).toBe(true);
    });
  });
});
