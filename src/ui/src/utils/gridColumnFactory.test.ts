import { describe, expect, it } from "vitest";
import { createPointsColumn } from "./gridColumnFactory";

describe("gridColumnFactory - createPointsColumn", () => {
  it("should create a points column with default settings", () => {
    const column = createPointsColumn();

    expect(column.headerName).toBe("Points");
    expect(column.field).toBe("points");
    expect(column.minWidth).toBe(100);
    expect(column.type).toBe("rightAligned");
    expect(column.sortable).toBe(true);
    expect(column.resizable).toBe(true);
  });

  it("should format numeric values with comma formatting", () => {
    const column = createPointsColumn();
    const valueFormatter = column.valueFormatter as any;

    expect(valueFormatter({ value: 1000 })).toBe("1,000");
    expect(valueFormatter({ value: 1000000 })).toBe("1,000,000");
    expect(valueFormatter({ value: 100 })).toBe("100");
  });

  it("should handle masked points value 'X' (uppercase)", () => {
    const column = createPointsColumn();
    const valueFormatter = column.valueFormatter as any;

    expect(valueFormatter({ value: "X" })).toBe("X");
  });

  it("should handle masked points value 'x' (lowercase) and normalize to uppercase", () => {
    const column = createPointsColumn();
    const valueFormatter = column.valueFormatter as any;

    expect(valueFormatter({ value: "x" })).toBe("X");
  });

  it("should handle null values", () => {
    const column = createPointsColumn();
    const valueFormatter = column.valueFormatter as any;

    expect(valueFormatter({ value: null })).toBe("");
  });

  it("should handle undefined values", () => {
    const column = createPointsColumn();
    const valueFormatter = column.valueFormatter as any;

    expect(valueFormatter({ value: undefined })).toBe("");
  });

  it("should handle zero values", () => {
    const column = createPointsColumn();
    const valueFormatter = column.valueFormatter as any;

    expect(valueFormatter({ value: 0 })).toBe("0");
  });

  it("should handle decimal values", () => {
    const column = createPointsColumn();
    const valueFormatter = column.valueFormatter as any;

    expect(valueFormatter({ value: 1234.56 })).toBe("1,234.56");
  });

  it("should handle NaN values gracefully", () => {
    const column = createPointsColumn();
    const valueFormatter = column.valueFormatter as any;

    expect(valueFormatter({ value: NaN })).toBe("");
  });

  it("should allow custom valueFormatter to override default behavior", () => {
    const customFormatter = (params: any) => `Custom: ${params.value}`;
    const column = createPointsColumn({ valueFormatter: customFormatter });

    expect(column.valueFormatter).toBe(customFormatter);
  });

  it("should allow disabling comma formatting", () => {
    const column = createPointsColumn({ includeCommaFormatting: false });
    const valueFormatter = column.valueFormatter as any;

    // When includeCommaFormatting is false, valueFormatter should be undefined
    // and ag-grid will use default formatting
    expect(valueFormatter).toBeUndefined();
  });

  it("should respect custom column options", () => {
    const column = createPointsColumn({
      headerName: "Custom Points",
      field: "customField",
      minWidth: 150,
      maxWidth: 200,
      sortable: false,
      alignment: "center"
    });

    expect(column.headerName).toBe("Custom Points");
    expect(column.field).toBe("customField");
    expect(column.minWidth).toBe(150);
    expect(column.maxWidth).toBe(200);
    expect(column.sortable).toBe(false);
    expect(column.headerClass).toBe("center-align");
    expect(column.cellClass).toBe("center-align");
  });

  it("should handle left alignment correctly", () => {
    const column = createPointsColumn({ alignment: "left" });

    expect(column.type).toBeUndefined();
    expect(column.headerClass).toBe("left-align");
    expect(column.cellClass).toBe("left-align");
  });

  it("should handle right alignment (default) correctly", () => {
    const column = createPointsColumn({ alignment: "right" });

    expect(column.type).toBe("rightAligned");
    expect(column.headerClass).toBeUndefined();
    expect(column.cellClass).toBeUndefined();
  });

  it("should handle mixed masked and numeric values in sequence", () => {
    const column = createPointsColumn();
    const valueFormatter = column.valueFormatter as any;

    const testCases = [
      { input: "X", expected: "X" },
      { input: 1000, expected: "1,000" },
      { input: "x", expected: "X" },
      { input: 5000, expected: "5,000" },
      { input: null, expected: "" }
    ];

    testCases.forEach(({ input, expected }) => {
      expect(valueFormatter({ value: input })).toBe(expected);
    });
  });

  it("should handle string numeric values", () => {
    const column = createPointsColumn();
    const valueFormatter = column.valueFormatter as any;

    // String numeric values should be returned as-is (not formatted)
    expect(valueFormatter({ value: "1000" })).toBe("1000");
  });

  it("should create colId matching field by default", () => {
    const column = createPointsColumn();

    expect(column.colId).toBe(column.field);
  });

  it("should set resizable and sortable by default", () => {
    const column = createPointsColumn();

    expect(column.resizable).toBe(true);
    expect(column.sortable).toBe(true);
  });
});
