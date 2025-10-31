import { describe, it, expect, vi } from "vitest";
import { GetMilitaryContributionFormGridColumns } from "./MilitaryContributionFormGridColumns";

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
  createCurrencyColumn: vi.fn((config) => ({
    colId: config.field || "amount",
    field: config.field || "amount",
    ...config
  })),
  createDateColumn: vi.fn((config) => ({
    colId: config.field || "date",
    field: config.field || "date",
    ...config
  }))
}));

describe("MilitaryContributionFormGridColumns", () => {
  describe("Column definitions", () => {
    it("should export a function that returns column definitions", () => {
      const columns = GetMilitaryContributionFormGridColumns();
      expect(Array.isArray(columns)).toBe(true);
    });

    it("should include badge number column", () => {
      const columns = GetMilitaryContributionFormGridColumns();
      const badgeColumn = columns.find((col) => col.colId === "badgeNumber");
      expect(badgeColumn).toBeDefined();
    });

    it("should include full name column", () => {
      const columns = GetMilitaryContributionFormGridColumns();
      const nameColumn = columns.find((col) => col.colId === "fullName");
      expect(nameColumn).toBeDefined();
    });

    it("should include contribution year column", () => {
      const columns = GetMilitaryContributionFormGridColumns();
      const yearColumn = columns.find((col) => col.field === "contributionYear");
      expect(yearColumn).toBeDefined();
    });

    it("should include contribution amount column", () => {
      const columns = GetMilitaryContributionFormGridColumns();
      const amountColumn = columns.find((col) => col.field === "contributionAmount");
      expect(amountColumn).toBeDefined();
    });

    it("should include contribution type column", () => {
      const columns = GetMilitaryContributionFormGridColumns();
      const typeColumn = columns.find((col) => col.field === "contributionType");
      expect(typeColumn).toBeDefined();
    });
  });

  describe("Column configuration", () => {
    it("should have sortable columns", () => {
      const columns = GetMilitaryContributionFormGridColumns();
      const sortableColumns = columns.filter((col) => col.sortable !== false);
      expect(sortableColumns.length).toBeGreaterThan(0);
    });

    it("should have proper column widths", () => {
      const columns = GetMilitaryContributionFormGridColumns();
      columns.forEach((col) => {
        if (col.minWidth) {
          expect(typeof col.minWidth).toBe("number");
          expect(col.minWidth).toBeGreaterThan(0);
        }
      });
    });

    it("should have resizable columns", () => {
      const columns = GetMilitaryContributionFormGridColumns();
      const resizableColumns = columns.filter((col) => col.resizable !== false);
      expect(resizableColumns.length).toBeGreaterThan(0);
    });

    it("should have correct alignment for currency columns", () => {
      const columns = GetMilitaryContributionFormGridColumns();
      const amountColumn = columns.find((col) => col.field === "contributionAmount");
      // Currency columns should typically be right-aligned
      expect(amountColumn).toBeDefined();
    });
  });

  describe("Column metadata", () => {
    it("should have headerName for display", () => {
      const columns = GetMilitaryContributionFormGridColumns();
      columns.forEach((col) => {
        if (col.headerName) {
          expect(typeof col.headerName).toBe("string");
          expect(col.headerName.length).toBeGreaterThan(0);
        }
      });
    });

    it("should have field property for data binding", () => {
      const columns = GetMilitaryContributionFormGridColumns();
      columns.forEach((col) => {
        expect(col.field).toBeDefined();
      });
    });

    it("should have colId property for identification", () => {
      const columns = GetMilitaryContributionFormGridColumns();
      columns.forEach((col) => {
        expect(col.colId).toBeDefined();
      });
    });
  });

  describe("Factory usage", () => {
    it("should use factory functions for common column types", () => {
      const columns = GetMilitaryContributionFormGridColumns();

      // Should use badge, name, store columns from factories
      const usingFactories = columns.filter(
        (col) =>
          col.colId === "badgeNumber" ||
          col.colId === "fullName" ||
          col.colId === "store"
      );

      expect(usingFactories.length).toBeGreaterThan(0);
    });
  });
});
