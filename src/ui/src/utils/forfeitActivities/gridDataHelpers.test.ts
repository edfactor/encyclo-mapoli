import { describe, expect, it } from "vitest";
import {
  flattenMasterDetailData,
  generateRowKey,
  getEditableFieldName,
  getEditedValue,
  hasRowError,
  isDetailRowEditable,
  transformForfeitureValue,
  type FlattenConfig,
  type RowKeyConfig,
  type RowKeyParams
} from "./gridDataHelpers";

describe("gridDataHelpers", () => {
  describe("generateRowKey", () => {
    describe("unforfeit activity type", () => {
      const config: RowKeyConfig = { type: "unforfeit" };

      it("should generate key from profitDetailId for unforfeit", () => {
        const params: RowKeyParams = {
          badgeNumber: 123456,
          profitYear: 2025,
          profitDetailId: 789
        };
        const result = generateRowKey(config, params);
        expect(result).toBe("789");
      });

      it("should handle large profitDetailId values", () => {
        const params: RowKeyParams = {
          badgeNumber: 123456,
          profitYear: 2025,
          profitDetailId: 999999999
        };
        const result = generateRowKey(config, params);
        expect(result).toBe("999999999");
      });

      it("should throw error when profitDetailId is undefined for unforfeit", () => {
        const params: RowKeyParams = {
          badgeNumber: 123456,
          profitYear: 2025
        };
        expect(() => generateRowKey(config, params)).toThrow("profitDetailId is required for unforfeit row keys");
      });

      it("should handle profitDetailId of 0", () => {
        const params: RowKeyParams = {
          badgeNumber: 123456,
          profitYear: 2025,
          profitDetailId: 0
        };
        const result = generateRowKey(config, params);
        expect(result).toBe("0");
      });
    });

    describe("termination activity type", () => {
      const config: RowKeyConfig = { type: "termination" };

      it("should generate composite key for termination", () => {
        const params: RowKeyParams = {
          badgeNumber: 123456,
          profitYear: 2025
        };
        const result = generateRowKey(config, params);
        expect(result).toBe("123456-2025");
      });

      it("should ignore profitDetailId for termination", () => {
        const params: RowKeyParams = {
          badgeNumber: 123456,
          profitYear: 2025,
          profitDetailId: 789
        };
        const result = generateRowKey(config, params);
        expect(result).toBe("123456-2025");
      });

      it("should handle different profit years", () => {
        const params2024: RowKeyParams = {
          badgeNumber: 123456,
          profitYear: 2024
        };
        const params2025: RowKeyParams = {
          badgeNumber: 123456,
          profitYear: 2025
        };
        expect(generateRowKey(config, params2024)).toBe("123456-2024");
        expect(generateRowKey(config, params2025)).toBe("123456-2025");
      });

      it("should handle different badge numbers", () => {
        const params1: RowKeyParams = {
          badgeNumber: 111111,
          profitYear: 2025
        };
        const params2: RowKeyParams = {
          badgeNumber: 999999,
          profitYear: 2025
        };
        expect(generateRowKey(config, params1)).toBe("111111-2025");
        expect(generateRowKey(config, params2)).toBe("999999-2025");
      });
    });
  });

  describe("transformForfeitureValue", () => {
    describe("unforfeit activity type", () => {
      it("should negate positive value for unforfeit", () => {
        const result = transformForfeitureValue("unforfeit", 1500);
        expect(result).toBe(-1500);
      });

      it("should negate negative value for unforfeit (becomes positive)", () => {
        const result = transformForfeitureValue("unforfeit", -1500);
        expect(result).toBe(1500);
      });

      it("should handle zero value for unforfeit", () => {
        const result = transformForfeitureValue("unforfeit", 0);
        // JavaScript has -0 and +0, negating 0 gives -0, but they are equal in value
        expect(Math.abs(result)).toBe(0);
      });

      it("should handle decimal values for unforfeit", () => {
        const result = transformForfeitureValue("unforfeit", 1500.75);
        expect(result).toBe(-1500.75);
      });
    });

    describe("termination activity type", () => {
      it("should return value unchanged for termination", () => {
        const result = transformForfeitureValue("termination", 1500);
        expect(result).toBe(1500);
      });

      it("should return negative value unchanged for termination", () => {
        const result = transformForfeitureValue("termination", -1500);
        expect(result).toBe(-1500);
      });

      it("should handle zero value for termination", () => {
        const result = transformForfeitureValue("termination", 0);
        expect(result).toBe(0);
      });

      it("should handle decimal values for termination", () => {
        const result = transformForfeitureValue("termination", 1500.75);
        expect(result).toBe(1500.75);
      });
    });
  });

  describe("getEditableFieldName", () => {
    it("should return 'suggestedUnforfeiture' for unforfeit", () => {
      const result = getEditableFieldName("unforfeit");
      expect(result).toBe("suggestedUnforfeiture");
    });

    it("should return 'suggestedForfeit' for termination", () => {
      const result = getEditableFieldName("termination");
      expect(result).toBe("suggestedForfeit");
    });
  });

  describe("isDetailRowEditable", () => {
    describe("unforfeit activity type", () => {
      it("should return true when suggestedUnforfeiture is not null", () => {
        const detailRow = { suggestedUnforfeiture: 1500 };
        const result = isDetailRowEditable("unforfeit", detailRow, 2025);
        expect(result).toBe(true);
      });

      it("should return true when suggestedUnforfeiture is zero", () => {
        const detailRow = { suggestedUnforfeiture: 0 };
        const result = isDetailRowEditable("unforfeit", detailRow, 2025);
        expect(result).toBe(true);
      });

      it("should return false when suggestedUnforfeiture is null", () => {
        const detailRow = { suggestedUnforfeiture: null };
        const result = isDetailRowEditable("unforfeit", detailRow, 2025);
        expect(result).toBe(false);
      });

      it("should return false when suggestedUnforfeiture is undefined", () => {
        const detailRow = {};
        const result = isDetailRowEditable("unforfeit", detailRow, 2025);
        expect(result).toBe(false);
      });

      it("should ignore profit year for unforfeit", () => {
        const detailRow = { profitYear: 2024, suggestedUnforfeiture: 1500 };
        const result = isDetailRowEditable("unforfeit", detailRow, 2025);
        expect(result).toBe(true);
      });
    });

    describe("termination activity type", () => {
      it("should return true when profitYear matches selectedProfitYear", () => {
        const detailRow = { profitYear: 2025, suggestedForfeit: 1500 };
        const result = isDetailRowEditable("termination", detailRow, 2025);
        expect(result).toBe(true);
      });

      it("should return false when profitYear does not match selectedProfitYear", () => {
        const detailRow = { profitYear: 2024, suggestedForfeit: 1500 };
        const result = isDetailRowEditable("termination", detailRow, 2025);
        expect(result).toBe(false);
      });

      it("should return true for matching year even when suggestedForfeit is null", () => {
        const detailRow = { profitYear: 2025, suggestedForfeit: null };
        const result = isDetailRowEditable("termination", detailRow, 2025);
        expect(result).toBe(true);
      });

      it("should return true for matching year even when suggestedForfeit is zero", () => {
        const detailRow = { profitYear: 2025, suggestedForfeit: 0 };
        const result = isDetailRowEditable("termination", detailRow, 2025);
        expect(result).toBe(true);
      });

      it("should return false when profitYear is undefined", () => {
        const detailRow = { suggestedForfeit: 1500 };
        const result = isDetailRowEditable("termination", detailRow, 2025);
        expect(result).toBe(false);
      });
    });
  });

  describe("flattenMasterDetailData", () => {
    interface TestMaster {
      id: number;
      name: string;
      details?: TestDetail[];
    }

    interface TestDetail {
      detailId: number;
      value: number;
    }

    const config: FlattenConfig<TestMaster, TestDetail> = {
      getKey: (master) => master.id.toString(),
      getDetails: (master) => master.details || [],
      hasDetails: (master) => Boolean(master.details && master.details.length > 0)
    };

    it("should handle null/undefined master data", () => {
      const result = flattenMasterDetailData(null as never, new Set(), config);
      expect(result).toEqual([]);
    });

    it("should handle empty master data array", () => {
      const result = flattenMasterDetailData([], new Set(), config);
      expect(result).toEqual([]);
    });

    it("should flatten master rows without details", () => {
      const masterData: TestMaster[] = [
        { id: 1, name: "Master 1" },
        { id: 2, name: "Master 2" }
      ];
      const result = flattenMasterDetailData(masterData, new Set(), config);

      expect(result).toHaveLength(2);
      expect(result[0]).toMatchObject({
        id: 1,
        name: "Master 1",
        isExpandable: false,
        isExpanded: false,
        isDetail: false
      });
      expect(result[1]).toMatchObject({
        id: 2,
        name: "Master 2",
        isExpandable: false,
        isExpanded: false,
        isDetail: false
      });
    });

    it("should mark rows with details as expandable but not expanded", () => {
      const masterData: TestMaster[] = [
        {
          id: 1,
          name: "Master 1",
          details: [{ detailId: 101, value: 100 }]
        }
      ];
      const result = flattenMasterDetailData(masterData, new Set(), config);

      expect(result).toHaveLength(1);
      expect(result[0]).toMatchObject({
        id: 1,
        name: "Master 1",
        isExpandable: true,
        isExpanded: false,
        isDetail: false
      });
    });

    it("should include detail rows when master is expanded", () => {
      const masterData: TestMaster[] = [
        {
          id: 1,
          name: "Master 1",
          details: [
            { detailId: 101, value: 100 },
            { detailId: 102, value: 200 }
          ]
        }
      ];
      const expandedRows = new Set(["1"]);
      const result = flattenMasterDetailData(masterData, expandedRows, config);

      expect(result).toHaveLength(3); // 1 master + 2 details
      expect(result[0]).toMatchObject({
        id: 1,
        name: "Master 1",
        isExpandable: true,
        isExpanded: true,
        isDetail: false
      });
      expect(result[1]).toMatchObject({
        detailId: 101,
        value: 100,
        isDetail: true
      });
      expect(result[2]).toMatchObject({
        detailId: 102,
        value: 200,
        isDetail: true
      });
    });

    it("should handle mix of expanded and collapsed masters", () => {
      const masterData: TestMaster[] = [
        {
          id: 1,
          name: "Master 1",
          details: [{ detailId: 101, value: 100 }]
        },
        {
          id: 2,
          name: "Master 2",
          details: [{ detailId: 201, value: 200 }]
        }
      ];
      const expandedRows = new Set(["1"]); // Only first one expanded
      const result = flattenMasterDetailData(masterData, expandedRows, config);

      expect(result).toHaveLength(3); // Master 1 + detail + Master 2
      expect(result[0].isExpanded).toBe(true);
      expect(result[1].isDetail).toBe(true);
      expect(result[2].isExpanded).toBe(false);
    });

    it("should handle master with empty details array", () => {
      const masterData: TestMaster[] = [
        {
          id: 1,
          name: "Master 1",
          details: []
        }
      ];
      const result = flattenMasterDetailData(masterData, new Set(["1"]), config);

      expect(result).toHaveLength(1);
      // Empty details array means hasDetails is false, so isExpandable is false
      // But since "1" is in expandedRows set, isExpanded is true (tracks user's intent)
      expect(result[0]).toMatchObject({
        isExpandable: false,
        isExpanded: true
      });
    });

    it("should handle multiple expanded masters with multiple details", () => {
      const masterData: TestMaster[] = [
        {
          id: 1,
          name: "Master 1",
          details: [
            { detailId: 101, value: 100 },
            { detailId: 102, value: 200 }
          ]
        },
        {
          id: 2,
          name: "Master 2",
          details: [
            { detailId: 201, value: 300 },
            { detailId: 202, value: 400 }
          ]
        }
      ];
      const expandedRows = new Set(["1", "2"]);
      const result = flattenMasterDetailData(masterData, expandedRows, config);

      expect(result).toHaveLength(6); // 2 masters + 4 details
      expect(result[0].isDetail).toBe(false);
      expect(result[1].isDetail).toBe(true);
      expect(result[2].isDetail).toBe(true);
      expect(result[3].isDetail).toBe(false);
      expect(result[4].isDetail).toBe(true);
      expect(result[5].isDetail).toBe(true);
    });
  });

  describe("getEditedValue", () => {
    it("should return edited value when it exists", () => {
      const editedValues = {
        "123-2025": { value: 1500 }
      };
      const result = getEditedValue(editedValues, "123-2025", 1000);
      expect(result).toBe(1500);
    });

    it("should return original value when no edit exists", () => {
      const editedValues = {
        "123-2025": { value: 1500 }
      };
      const result = getEditedValue(editedValues, "456-2025", 1000);
      expect(result).toBe(1000);
    });

    it("should return 0 when original value is null", () => {
      const editedValues = {};
      const result = getEditedValue(editedValues, "123-2025", null);
      expect(result).toBe(0);
    });

    it("should return 0 when original value is undefined", () => {
      const editedValues = {};
      const result = getEditedValue(editedValues, "123-2025", undefined);
      expect(result).toBe(0);
    });

    it("should return 0 when both edited and original are undefined", () => {
      const editedValues = {};
      const result = getEditedValue(editedValues, "123-2025");
      expect(result).toBe(0);
    });

    it("should return edited value of 0 when explicitly set", () => {
      const editedValues = {
        "123-2025": { value: 0 }
      };
      const result = getEditedValue(editedValues, "123-2025", 1000);
      expect(result).toBe(0);
    });

    it("should handle negative edited values", () => {
      const editedValues = {
        "123-2025": { value: -500 }
      };
      const result = getEditedValue(editedValues, "123-2025", 1000);
      expect(result).toBe(-500);
    });

    it("should handle decimal edited values", () => {
      const editedValues = {
        "123-2025": { value: 1500.75 }
      };
      const result = getEditedValue(editedValues, "123-2025", 1000);
      expect(result).toBe(1500.75);
    });

    it("should ignore hasError property when getting value", () => {
      const editedValues = {
        "123-2025": { value: 1500, hasError: true }
      };
      const result = getEditedValue(editedValues, "123-2025", 1000);
      expect(result).toBe(1500);
    });

    it("should handle undefined editedValues object", () => {
      const result = getEditedValue(undefined as never, "123-2025", 1000);
      expect(result).toBe(1000);
    });
  });

  describe("hasRowError", () => {
    it("should return true when row has error", () => {
      const editedValues = {
        "123-2025": { value: 1500, hasError: true }
      };
      const result = hasRowError(editedValues, "123-2025");
      expect(result).toBe(true);
    });

    it("should return false when row has no error", () => {
      const editedValues = {
        "123-2025": { value: 1500, hasError: false }
      };
      const result = hasRowError(editedValues, "123-2025");
      expect(result).toBe(false);
    });

    it("should return false when hasError is undefined", () => {
      const editedValues = {
        "123-2025": { value: 1500 }
      };
      const result = hasRowError(editedValues, "123-2025");
      expect(result).toBe(false);
    });

    it("should return false when row key does not exist", () => {
      const editedValues = {
        "123-2025": { value: 1500, hasError: true }
      };
      const result = hasRowError(editedValues, "456-2025");
      expect(result).toBe(false);
    });

    it("should return false when editedValues is undefined", () => {
      const result = hasRowError(undefined as never, "123-2025");
      expect(result).toBe(false);
    });

    it("should return false when editedValues is empty", () => {
      const result = hasRowError({}, "123-2025");
      expect(result).toBe(false);
    });

    it("should handle multiple rows with different error states", () => {
      const editedValues = {
        "123-2025": { value: 1500, hasError: true },
        "456-2025": { value: 2000, hasError: false },
        "789-2025": { value: 2500 }
      };
      expect(hasRowError(editedValues, "123-2025")).toBe(true);
      expect(hasRowError(editedValues, "456-2025")).toBe(false);
      expect(hasRowError(editedValues, "789-2025")).toBe(false);
    });
  });
});
