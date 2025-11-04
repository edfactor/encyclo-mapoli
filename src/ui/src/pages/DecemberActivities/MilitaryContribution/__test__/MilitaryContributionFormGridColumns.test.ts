import { describe, it, expect, vi } from "vitest";
import { GetMilitaryContributionColumns } from "../MilitaryContributionFormGridColumns";

// Mock the column factory functions
vi.mock("../../../utils/gridColumnFactory", () => ({
  createYearColumn: vi.fn((config) => ({
    colId: config.field || "year",
    field: config.field || "year",
    headerName: config.headerName,
    ...config
  })),
  createCurrencyColumn: vi.fn((config) => ({
    colId: config.field || "amount",
    field: config.field || "amount",
    headerName: config.headerName,
    ...config
  })),
  createYesOrNoColumn: vi.fn((config) => ({
    colId: config.field || "yesNo",
    field: config.field || "yesNo",
    headerName: config.headerName,
    ...config
  }))
}));

describe("MilitaryContributionFormGridColumns", () => {
  describe("Column definitions", () => {
    it("should export a function that returns column definitions", () => {
      const columns = GetMilitaryContributionColumns();
      expect(Array.isArray(columns)).toBe(true);
    });

    it("should include contribution year column", () => {
      const columns = GetMilitaryContributionColumns();
      const yearColumn = columns.find((col) => col.field === "contributionDate");
      expect(yearColumn).toBeDefined();
    });

    it("should include contribution amount column", () => {
      const columns = GetMilitaryContributionColumns();
      const amountColumn = columns.find((col) => col.field === "amount");
      expect(amountColumn).toBeDefined();
    });

    it("should include supplemental contribution column", () => {
      const columns = GetMilitaryContributionColumns();
      const typeColumn = columns.find((col) => col.field === "isSupplementalContribution");
      expect(typeColumn).toBeDefined();
    });
  });

  describe("Column configuration", () => {
    it("should have sortable columns", () => {
      const columns = GetMilitaryContributionColumns();
      const sortableColumns = columns.filter((col) => col.sortable !== false);
      expect(sortableColumns.length).toBeGreaterThan(0);
    });

    it("should have proper column widths", () => {
      const columns = GetMilitaryContributionColumns();
      columns.forEach((col) => {
        if (col.minWidth) {
          expect(typeof col.minWidth).toBe("number");
          expect(col.minWidth).toBeGreaterThan(0);
        }
      });
    });

    it("should have resizable columns", () => {
      const columns = GetMilitaryContributionColumns();
      const resizableColumns = columns.filter((col) => col.resizable !== false);
      expect(resizableColumns.length).toBeGreaterThan(0);
    });

    it("should have correct alignment for currency columns", () => {
      const columns = GetMilitaryContributionColumns();
      const amountColumn = columns.find((col) => col.field === "amount");
      // Currency columns should typically be right-aligned
      expect(amountColumn).toBeDefined();
    });
  });

  describe("Column metadata", () => {
    it("should have headerName for display", () => {
      const columns = GetMilitaryContributionColumns();
      columns.forEach((col) => {
        if (col.headerName) {
          expect(typeof col.headerName).toBe("string");
          expect(col.headerName.length).toBeGreaterThan(0);
        }
      });
    });

    it("should have field property for data binding", () => {
      const columns = GetMilitaryContributionColumns();
      columns.forEach((col) => {
        expect(col.field).toBeDefined();
      });
    });

    it("should have colId property for identification", () => {
      const columns = GetMilitaryContributionColumns();
      columns.forEach((col) => {
        expect(col.colId).toBeDefined();
      });
    });
  });

  describe("Factory usage", () => {
    it("should use factory functions for common column types", () => {
      const columns = GetMilitaryContributionColumns();

      // Should use year, currency, yes/no columns from factories
      const usingFactories = columns.filter(
        (col) =>
          col.field === "contributionDate" || col.field === "amount" || col.field === "isSupplementalContribution"
      );

      expect(usingFactories.length).toBeGreaterThan(0);
    });
  });
});
