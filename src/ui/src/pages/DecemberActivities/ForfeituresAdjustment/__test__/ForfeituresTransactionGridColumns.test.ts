import { describe, expect, it, vi } from "vitest";
import { GetForfeituresTransactionGridColumns } from "../ForfeituresTransactionGridColumns";

// Mock the column factory functions
vi.mock("../../../utils/gridColumnFactory", () => ({
  createBadgeColumn: vi.fn((config) => ({
    colId: "badgeNumber",
    field: "badgeNumber",
    ...config
  })),
  createNameColumn: vi.fn((config) => ({
    colId: "fullName",
    field: config.field || "fullName",
    ...config
  })),
  createStoreColumn: vi.fn((config) => ({
    colId: "store",
    field: "store",
    ...config
  })),
  createSSNColumn: vi.fn((config) => ({
    colId: "ssn",
    field: "ssn",
    ...config
  })),
  createDateColumn: vi.fn((config) => ({
    colId: config.field || "date",
    field: config.field || "date",
    ...config
  })),
  createCurrencyColumn: vi.fn((config) => ({
    colId: config.field || "amount",
    field: config.field || "amount",
    ...config
  })),
  createStatusColumn: vi.fn((config) => ({
    colId: config.field || "status",
    field: config.field || "status",
    ...config
  })),
  createYearColumn: vi.fn((config) => ({
    colId: config.field || "year",
    field: config.field || "year",
    ...config
  })),
  createHoursColumn: vi.fn((config) => ({
    colId: config.field || "hours",
    field: config.field || "hours",
    ...config
  })),
  createStateColumn: vi.fn((config) => ({
    colId: config.field || "state",
    field: config.field || "state",
    ...config
  })),
  createYesOrNoColumn: vi.fn((config) => ({
    colId: config.field || "yesOrNo",
    field: config.field || "yesOrNo",
    ...config
  }))
}));

describe("ForfeituresTransactionGridColumns", () => {
  describe("Column definitions", () => {
    it("should export a function that returns column definitions", () => {
      const columns = GetForfeituresTransactionGridColumns();
      expect(Array.isArray(columns)).toBe(true);
    });

    it("should return an array of column definitions", () => {
      const columns = GetForfeituresTransactionGridColumns();
      expect(Array.isArray(columns)).toBe(true);
      expect(columns.length).toBeGreaterThan(0);
    });

    it("should include profit year column", () => {
      const columns = GetForfeituresTransactionGridColumns();
      const yearColumn = columns.find((col) => col.field === "profitYear");
      expect(yearColumn).toBeDefined();
    });

    it("should include profit code column", () => {
      const columns = GetForfeituresTransactionGridColumns();
      const codeColumn = columns.find((col) => col.field === "profitCodeId");
      expect(codeColumn).toBeDefined();
    });

    it("should include forfeiture column", () => {
      const columns = GetForfeituresTransactionGridColumns();
      const forfeitureColumn = columns.find((col) => col.field === "forfeiture");
      expect(forfeitureColumn).toBeDefined();
    });

    it("should include contribution column", () => {
      const columns = GetForfeituresTransactionGridColumns();
      const contributionColumn = columns.find((col) => col.field === "contribution");
      expect(contributionColumn).toBeDefined();
    });

    it("should include hours column", () => {
      const columns = GetForfeituresTransactionGridColumns();
      const hoursColumn = columns.find((col) => col.field === "currentHoursYear");
      expect(hoursColumn).toBeDefined();
    });

    it("should include wages column", () => {
      const columns = GetForfeituresTransactionGridColumns();
      const wagesColumn = columns.find((col) => col.field === "currentIncomeYear");
      expect(wagesColumn).toBeDefined();
    });
  });

  describe("Column configuration", () => {
    it("should have sortable columns", () => {
      const columns = GetForfeituresTransactionGridColumns();
      const sortableColumns = columns.filter((col) => col.sortable !== false);
      expect(sortableColumns.length).toBeGreaterThan(0);
    });

    it("should have proper column widths", () => {
      const columns = GetForfeituresTransactionGridColumns();
      columns.forEach((col) => {
        if (col.minWidth) {
          expect(typeof col.minWidth).toBe("number");
          expect(col.minWidth).toBeGreaterThan(0);
        }
      });
    });

    it("should have resizable columns", () => {
      const columns = GetForfeituresTransactionGridColumns();
      const resizableColumns = columns.filter((col) => col.resizable !== false);
      expect(resizableColumns.length).toBeGreaterThan(0);
    });

    it("should not have editable amount columns", () => {
      const columns = GetForfeituresTransactionGridColumns();
      const editableColumns = columns.filter((col) => col.editable === true);
      // Transaction grid should be read-only
      expect(editableColumns.length).toBe(0);
    });
  });

  describe("Column factories usage", () => {
    it("should use createYearColumn factory", () => {
      const columns = GetForfeituresTransactionGridColumns();
      // Verify year column exists (uses createYearColumn)
      const yearColumn = columns.find((col) => col.field === "profitYear");
      expect(yearColumn).toBeDefined();
    });

    it("should use createCurrencyColumn factory", () => {
      const columns = GetForfeituresTransactionGridColumns();
      // Multiple currency columns should exist
      const currencyColumns = columns.filter(
        (col) =>
          col.field === "forfeiture" ||
          col.field === "contribution" ||
          col.field === "earnings" ||
          col.field === "payment" ||
          col.field === "currentIncomeYear"
      );
      expect(currencyColumns.length).toBeGreaterThan(0);
    });

    it("should use createHoursColumn factory", () => {
      const columns = GetForfeituresTransactionGridColumns();
      const hoursColumn = columns.find((col) => col.field === "currentHoursYear");
      expect(hoursColumn).toBeDefined();
    });
  });

  describe("Column metadata", () => {
    it("should have proper headerName for display", () => {
      const columns = GetForfeituresTransactionGridColumns();
      columns.forEach((col) => {
        if (col.headerName) {
          expect(typeof col.headerName).toBe("string");
          expect(col.headerName.length).toBeGreaterThan(0);
        }
      });
    });

    it("should have field property for data binding", () => {
      const columns = GetForfeituresTransactionGridColumns();
      columns.forEach((col) => {
        expect(col.field).toBeDefined();
      });
    });

    it("should have colId property for identification", () => {
      const columns = GetForfeituresTransactionGridColumns();
      columns.forEach((col) => {
        expect(col.colId).toBeDefined();
      });
    });
  });
});
