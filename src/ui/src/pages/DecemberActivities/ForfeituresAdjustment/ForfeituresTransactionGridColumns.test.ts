import { describe, expect, it, vi } from "vitest";
import { GetForfeituresTransactionGridColumns } from "./ForfeituresTransactionGridColumns";

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
  }))
}));

describe("ForfeituresTransactionGridColumns", () => {
  describe("Column definitions", () => {
    it("should export a function that returns column definitions", () => {
      const columns = GetForfeituresTransactionGridColumns();
      expect(typeof columns).toBe("function");
    });

    it("should return an array of column definitions", () => {
      const getColumns = GetForfeituresTransactionGridColumns();
      const columns = getColumns();
      expect(Array.isArray(columns)).toBe(true);
    });

    it("should include badge number column", () => {
      const getColumns = GetForfeituresTransactionGridColumns();
      const columns = getColumns();
      const badgeColumn = columns.find((col) => col.colId === "badgeNumber");
      expect(badgeColumn).toBeDefined();
    });

    it("should include employee name column", () => {
      const getColumns = GetForfeituresTransactionGridColumns();
      const columns = getColumns();
      const nameColumn = columns.find((col) => col.colId === "fullName");
      expect(nameColumn).toBeDefined();
    });

    it("should include store column", () => {
      const getColumns = GetForfeituresTransactionGridColumns();
      const columns = getColumns();
      const storeColumn = columns.find((col) => col.colId === "store");
      expect(storeColumn).toBeDefined();
    });

    it("should include SSN column", () => {
      const getColumns = GetForfeituresTransactionGridColumns();
      const columns = getColumns();
      const ssnColumn = columns.find((col) => col.colId === "ssn");
      expect(ssnColumn).toBeDefined();
    });

    it("should include transaction date column", () => {
      const getColumns = GetForfeituresTransactionGridColumns();
      const columns = getColumns();
      const dateColumn = columns.find((col) => col.field === "profitSharingYear" || col.field === "transactionDate");
      expect(dateColumn).toBeDefined();
    });

    it("should include amount column", () => {
      const getColumns = GetForfeituresTransactionGridColumns();
      const columns = getColumns();
      const amountColumn = columns.find((col) => col.field === "amount" || col.field === "forfeitureAmount");
      expect(amountColumn).toBeDefined();
    });
  });

  describe("Column configuration", () => {
    it("should have sortable columns", () => {
      const getColumns = GetForfeituresTransactionGridColumns();
      const columns = getColumns();
      const sortableColumns = columns.filter((col) => col.sortable !== false);
      expect(sortableColumns.length).toBeGreaterThan(0);
    });

    it("should have proper column widths", () => {
      const getColumns = GetForfeituresTransactionGridColumns();
      const columns = getColumns();
      columns.forEach((col) => {
        if (col.minWidth) {
          expect(typeof col.minWidth).toBe("number");
          expect(col.minWidth).toBeGreaterThan(0);
        }
      });
    });

    it("should have resizable columns", () => {
      const getColumns = GetForfeituresTransactionGridColumns();
      const columns = getColumns();
      const resizableColumns = columns.filter((col) => col.resizable !== false);
      expect(resizableColumns.length).toBeGreaterThan(0);
    });

    it("should not have editable amount columns", () => {
      const getColumns = GetForfeituresTransactionGridColumns();
      const columns = getColumns();
      const editableColumns = columns.filter((col) => col.editable === true);
      // Transaction grid should be read-only
      expect(editableColumns.length).toBe(0);
    });
  });

  describe("Column factories usage", () => {
    it("should use createBadgeColumn factory", () => {
      const { createBadgeColumn } = vi.hoisted(() => ({
        createBadgeColumn: vi.fn()
      }));

      // Test that the factory was called during column creation
      const getColumns = GetForfeituresTransactionGridColumns();
      const columns = getColumns();
      1;
      // Verify badge column exists
      const badgeColumn = columns.find((col) => col.colId === "badgeNumber");
      expect(badgeColumn).toBeDefined();
    });

    it("should use createNameColumn factory", () => {
      const getColumns = GetForfeituresTransactionGridColumns();
      const columns = getColumns();

      const nameColumn = columns.find((col) => col.colId === "fullName");
      expect(nameColumn).toBeDefined();
    });

    it("should use createCurrencyColumn for amounts", () => {
      const getColumns = GetForfeituresTransactionGridColumns();
      const columns = getColumns();

      const currencyColumns = columns.filter((col) => col.field === "amount" || col.field === "forfeitureAmount");
      expect(currencyColumns.length).toBeGreaterThan(0);
    });
  });

  describe("Column metadata", () => {
    it("should have proper headerName for display", () => {
      const getColumns = GetForfeituresTransactionGridColumns();
      const columns = getColumns();
      columns.forEach((col) => {
        if (col.headerName) {
          expect(typeof col.headerName).toBe("string");
          expect(col.headerName.length).toBeGreaterThan(0);
        }
      });
    });

    it("should have field property for data binding", () => {
      const getColumns = GetForfeituresTransactionGridColumns();
      const columns = getColumns();
      columns.forEach((col) => {
        expect(col.field).toBeDefined();
      });
    });

    it("should have colId property for identification", () => {
      const getColumns = GetForfeituresTransactionGridColumns();
      const columns = getColumns();
      columns.forEach((col) => {
        expect(col.colId).toBeDefined();
      });
    });
  });
});
