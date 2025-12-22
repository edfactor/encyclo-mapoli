import { GridApi } from "ag-grid-community";
import { beforeEach, describe, expect, it, vi } from "vitest";
import {
  clearGridSelectionsForBadges,
  executeBatchSave,
  formatApiError,
  generateBulkSaveSuccessMessage,
  generateSaveSuccessMessage,
  getErrorMessage,
  getRowKeysForRequests,
  prepareBulkSaveRequests,
  prepareSaveRequest,
  type ActivityConfig,
  type BatchConfig,
  type ForfeitureAdjustmentUpdateRequest
} from "./saveOperationHelpers";

describe("saveOperationHelpers", () => {
  describe("prepareSaveRequest", () => {
    it("should preserve forfeitureAmount (FE already handles transformations)", () => {
      const request: ForfeitureAdjustmentUpdateRequest = {
        badgeNumber: 123456,
        profitYear: 2025,
        forfeitureAmount: -1500, // FE already negated this
        classAction: false
      };
      const result = prepareSaveRequest(request);

      expect(result).toEqual({
        badgeNumber: 123456,
        profitYear: 2025,
        forfeitureAmount: -1500, // Should remain as-is
        classAction: false
      });
    });

    it("should preserve positive forfeitureAmount", () => {
      const request: ForfeitureAdjustmentUpdateRequest = {
        badgeNumber: 123456,
        profitYear: 2025,
        forfeitureAmount: 1500,
        classAction: false
      };
      const result = prepareSaveRequest(request);

      expect(result.forfeitureAmount).toBe(1500);
    });

    it("should preserve all fields in request", () => {
      const request: ForfeitureAdjustmentUpdateRequest = {
        badgeNumber: 123456,
        profitYear: 2025,
        forfeitureAmount: -1500,
        classAction: true,
        offsettingProfitDetailId: 789
      };
      const result = prepareSaveRequest(request);

      expect(result).toEqual({
        badgeNumber: 123456,
        profitYear: 2025,
        forfeitureAmount: -1500,
        classAction: true,
        offsettingProfitDetailId: 789
      });
    });
  });

  describe("prepareBulkSaveRequests", () => {
    it("should preserve all requests in the array", () => {
      const requests: ForfeitureAdjustmentUpdateRequest[] = [
        { badgeNumber: 123456, profitYear: 2025, forfeitureAmount: -1500, classAction: false },
        { badgeNumber: 789012, profitYear: 2025, forfeitureAmount: -2000, classAction: false }
      ];

      const result = prepareBulkSaveRequests(requests);

      expect(result).toHaveLength(2);
      expect(result[0].forfeitureAmount).toBe(-1500);
      expect(result[1].forfeitureAmount).toBe(-2000);
    });

    it("should handle empty array", () => {
      const result = prepareBulkSaveRequests([]);
      expect(result).toEqual([]);
    });

    it("should handle single request", () => {
      const requests: ForfeitureAdjustmentUpdateRequest[] = [
        { badgeNumber: 123456, profitYear: 2025, forfeitureAmount: -1500, classAction: false }
      ];

      const result = prepareBulkSaveRequests(requests);

      expect(result).toHaveLength(1);
      expect(result[0].forfeitureAmount).toBe(-1500);
    });
  });

  describe("generateSaveSuccessMessage", () => {
    describe("unforfeit activity type", () => {
      it("should generate message with 'unforfeiture' for unforfeit", () => {
        const result = generateSaveSuccessMessage("unforfeit", "Doe, John", 1500);
        expect(result).toBe("Successfully saved unforfeiture of $1,500.00 for Doe, John");
      });

      it("should format amount with currency symbol", () => {
        const result = generateSaveSuccessMessage("unforfeit", "Smith, Jane", 2500.5);
        expect(result).toBe("Successfully saved unforfeiture of $2,500.50 for Smith, Jane");
      });

      it("should use absolute value for negative amounts", () => {
        const result = generateSaveSuccessMessage("unforfeit", "Doe, John", -1500);
        expect(result).toBe("Successfully saved unforfeiture of $1,500.00 for Doe, John");
      });

      it("should handle zero amount", () => {
        const result = generateSaveSuccessMessage("unforfeit", "Doe, John", 0);
        expect(result).toBe("Successfully saved unforfeiture of $0.00 for Doe, John");
      });
    });

    describe("termination activity type", () => {
      it("should generate message with 'forfeiture' for termination", () => {
        const result = generateSaveSuccessMessage("termination", "Doe, John", 1500);
        expect(result).toBe("Successfully saved forfeiture of $1,500.00 for Doe, John");
      });

      it("should format large amounts correctly", () => {
        const result = generateSaveSuccessMessage("termination", "Smith, Jane", 123456.78);
        expect(result).toBe("Successfully saved forfeiture of $123,456.78 for Smith, Jane");
      });
    });
  });

  describe("generateBulkSaveSuccessMessage", () => {
    describe("unforfeit activity type", () => {
      it("should generate message without names when not provided", () => {
        const result = generateBulkSaveSuccessMessage("unforfeit", 5);
        expect(result).toBe("Successfully saved 5 unforfeitures");
      });

      it("should generate message with names when provided", () => {
        const result = generateBulkSaveSuccessMessage("unforfeit", 3, ["Doe, John", "Smith, Jane", "Brown, Bob"]);
        expect(result).toBe("Successfully saved 3 unforfeitures for Doe, John, Smith, Jane, Brown, Bob");
      });

      it("should handle single record", () => {
        const result = generateBulkSaveSuccessMessage("unforfeit", 1, ["Doe, John"]);
        expect(result).toBe("Successfully saved 1 unforfeitures for Doe, John");
      });

      it("should handle empty names array", () => {
        const result = generateBulkSaveSuccessMessage("unforfeit", 5, []);
        expect(result).toBe("Successfully saved 5 unforfeitures");
      });
    });

    describe("termination activity type", () => {
      it("should generate message with 'forfeitures' for termination", () => {
        const result = generateBulkSaveSuccessMessage("termination", 5);
        expect(result).toBe("Successfully saved 5 forfeitures");
      });

      it("should generate message with names for termination", () => {
        const result = generateBulkSaveSuccessMessage("termination", 2, ["Doe, John", "Smith, Jane"]);
        expect(result).toBe("Successfully saved 2 forfeitures for Doe, John, Smith, Jane");
      });
    });
  });

  describe("getRowKeysForRequests", () => {
    describe("unforfeit activity type", () => {
      const config: ActivityConfig = {
        activityType: "unforfeit",
        rowKeyConfig: { type: "unforfeit" }
      };

      it("should extract profitDetailIds for unforfeit", () => {
        const requests: ForfeitureAdjustmentUpdateRequest[] = [
          {
            badgeNumber: 123456,
            profitYear: 2025,
            forfeitureAmount: 1500,
            classAction: false,
            offsettingProfitDetailId: 789
          },
          {
            badgeNumber: 789012,
            profitYear: 2025,
            forfeitureAmount: 2000,
            classAction: false,
            offsettingProfitDetailId: 456
          }
        ];

        const result = getRowKeysForRequests(config, requests);
        expect(result).toEqual(["789", "456"]);
      });

      it("should handle single request", () => {
        const requests: ForfeitureAdjustmentUpdateRequest[] = [
          {
            badgeNumber: 123456,
            profitYear: 2025,
            forfeitureAmount: 1500,
            classAction: false,
            offsettingProfitDetailId: 789
          }
        ];

        const result = getRowKeysForRequests(config, requests);
        expect(result).toEqual(["789"]);
      });
    });

    describe("termination activity type", () => {
      const config: ActivityConfig = {
        activityType: "termination",
        rowKeyConfig: { type: "termination" }
      };

      it("should generate composite keys for termination", () => {
        const requests: ForfeitureAdjustmentUpdateRequest[] = [
          { badgeNumber: 123456, profitYear: 2025, forfeitureAmount: 1500, classAction: false },
          { badgeNumber: 789012, profitYear: 2024, forfeitureAmount: 2000, classAction: false }
        ];

        const result = getRowKeysForRequests(config, requests);
        expect(result).toEqual(["123456-2025", "789012-2024"]);
      });

      it("should handle empty array", () => {
        const result = getRowKeysForRequests(config, []);
        expect(result).toEqual([]);
      });
    });
  });

  describe("clearGridSelectionsForBadges", () => {
    let mockGridApi: GridApi;
    let mockNodes: Array<{ data: { badgeNumber: number }; setSelected: ReturnType<typeof vi.fn> }>;

    beforeEach(() => {
      mockNodes = [
        { data: { badgeNumber: 123456 }, setSelected: vi.fn() },
        { data: { badgeNumber: 789012 }, setSelected: vi.fn() },
        { data: { badgeNumber: 111111 }, setSelected: vi.fn() }
      ];

      mockGridApi = {
        forEachNode: vi.fn((callback) => {
          mockNodes.forEach(callback);
        })
      } as unknown as GridApi;
    });

    it("should clear selections for specified badge numbers", () => {
      clearGridSelectionsForBadges(mockGridApi, [123456, 789012]);

      expect(mockNodes[0].setSelected).toHaveBeenCalledWith(false);
      expect(mockNodes[1].setSelected).toHaveBeenCalledWith(false);
      expect(mockNodes[2].setSelected).not.toHaveBeenCalled();
    });

    it("should handle single badge number", () => {
      clearGridSelectionsForBadges(mockGridApi, [123456]);

      expect(mockNodes[0].setSelected).toHaveBeenCalledWith(false);
      expect(mockNodes[1].setSelected).not.toHaveBeenCalled();
      expect(mockNodes[2].setSelected).not.toHaveBeenCalled();
    });

    it("should handle empty badge numbers array", () => {
      clearGridSelectionsForBadges(mockGridApi, []);

      expect(mockNodes[0].setSelected).not.toHaveBeenCalled();
      expect(mockNodes[1].setSelected).not.toHaveBeenCalled();
      expect(mockNodes[2].setSelected).not.toHaveBeenCalled();
    });

    it("should handle undefined gridApi", () => {
      expect(() => clearGridSelectionsForBadges(undefined, [123456])).not.toThrow();
    });

    it("should handle nodes without data", () => {
      const nodesWithoutData = [{ setSelected: vi.fn() }];
      const apiWithoutData = {
        forEachNode: vi.fn((callback) => {
          nodesWithoutData.forEach(callback);
        })
      } as unknown as GridApi;

      expect(() => clearGridSelectionsForBadges(apiWithoutData, [123456])).not.toThrow();
      expect(nodesWithoutData[0].setSelected).not.toHaveBeenCalled();
    });

    it("should handle badge numbers not in grid", () => {
      clearGridSelectionsForBadges(mockGridApi, [999999]);

      expect(mockNodes[0].setSelected).not.toHaveBeenCalled();
      expect(mockNodes[1].setSelected).not.toHaveBeenCalled();
      expect(mockNodes[2].setSelected).not.toHaveBeenCalled();
    });
  });

  describe("getErrorMessage", () => {
    describe("unforfeit activity type", () => {
      it("should return save error message", () => {
        const result = getErrorMessage("unforfeit", "save");
        expect(result).toBe("Failed to save unforfeiture adjustment");
      });

      it("should return bulkSave error message", () => {
        const result = getErrorMessage("unforfeit", "bulkSave");
        expect(result).toBe("Failed to save bulk unforfeiture adjustments");
      });

      it("should return fetch error message", () => {
        const result = getErrorMessage("unforfeit", "fetch");
        expect(result).toBe("Failed to fetch unforfeiture data");
      });
    });

    describe("termination activity type", () => {
      it("should return save error message", () => {
        const result = getErrorMessage("termination", "save");
        expect(result).toBe("Failed to save forfeiture adjustment");
      });

      it("should return bulkSave error message", () => {
        const result = getErrorMessage("termination", "bulkSave");
        expect(result).toBe("Failed to save bulk forfeiture adjustments");
      });

      it("should return fetch error message", () => {
        const result = getErrorMessage("termination", "fetch");
        expect(result).toBe("Failed to fetch termination data");
      });
    });
  });

  describe("formatApiError", () => {
    it("should return error message from error object", () => {
      const error = { message: "Network error occurred" };
      const result = formatApiError(error, "Default message");
      expect(result).toBe("Network error occurred");
    });

    it("should return error message from error.data.message", () => {
      const error = { data: { message: "Validation failed" } };
      const result = formatApiError(error, "Default message");
      expect(result).toBe("Validation failed");
    });

    it("should return default message when error has no message", () => {
      const error = { status: 500 };
      const result = formatApiError(error, "Default message");
      expect(result).toBe("Default message");
    });

    it("should return default message when error is null", () => {
      const result = formatApiError(null, "Default message");
      expect(result).toBe("Default message");
    });

    it("should return default message when error is undefined", () => {
      const result = formatApiError(undefined, "Default message");
      expect(result).toBe("Default message");
    });

    it("should return default message when error is not an object", () => {
      const result = formatApiError("string error", "Default message");
      expect(result).toBe("Default message");
    });

    it("should prioritize error.message over error.data.message", () => {
      const error = {
        message: "Direct message",
        data: { message: "Nested message" }
      };
      const result = formatApiError(error, "Default message");
      expect(result).toBe("Direct message");
    });

    it("should handle error.data.message when error.message is missing", () => {
      const error = {
        data: { message: "Nested message" }
      };
      const result = formatApiError(error, "Default message");
      expect(result).toBe("Nested message");
    });
  });

  describe("executeBatchSave", () => {
    it("should execute all requests in a single batch when count is less than batch size", async () => {
      const requests = [1, 2, 3];
      const saveFn = vi.fn().mockResolvedValue(undefined);
      const config: BatchConfig = { batchSize: 10, delayMs: 100 };

      await executeBatchSave(requests, saveFn, config);

      expect(saveFn).toHaveBeenCalledTimes(3);
      expect(saveFn).toHaveBeenCalledWith(1);
      expect(saveFn).toHaveBeenCalledWith(2);
      expect(saveFn).toHaveBeenCalledWith(3);
    });

    it("should split requests into multiple batches", async () => {
      const requests = [1, 2, 3, 4, 5];
      const saveFn = vi.fn().mockResolvedValue(undefined);
      const config: BatchConfig = { batchSize: 2, delayMs: 10 };

      await executeBatchSave(requests, saveFn, config);

      expect(saveFn).toHaveBeenCalledTimes(5);
    });

    it("should execute batches sequentially with delay", async () => {
      vi.useFakeTimers();

      const requests = [1, 2, 3, 4];
      const saveFn = vi.fn().mockResolvedValue(undefined);
      const config: BatchConfig = { batchSize: 2, delayMs: 50 };

      const executePromise = executeBatchSave(requests, saveFn, config);

      // Fast-forward through the delay
      await vi.advanceTimersByTimeAsync(50);

      await executePromise;

      // Verify batch delay was used (2 batches, 1 delay between them)
      expect(saveFn).toHaveBeenCalledTimes(4);

      vi.useRealTimers();
    });

    it("should handle empty requests array", async () => {
      const saveFn = vi.fn().mockResolvedValue(undefined);
      const config: BatchConfig = { batchSize: 10, delayMs: 100 };

      await executeBatchSave([], saveFn, config);

      expect(saveFn).not.toHaveBeenCalled();
    });

    it("should use default config when not provided", async () => {
      const requests = [1, 2, 3];
      const saveFn = vi.fn().mockResolvedValue(undefined);

      await executeBatchSave(requests, saveFn);

      expect(saveFn).toHaveBeenCalledTimes(3);
    });

    it("should execute requests in each batch in parallel", async () => {
      const requests = [1, 2, 3, 4];
      const callOrder: number[] = [];
      const saveFn = vi.fn().mockImplementation(async (req: number) => {
        // Simulate async work
        await new Promise((resolve) => setTimeout(resolve, 10));
        callOrder.push(req);
      });
      const config: BatchConfig = { batchSize: 2, delayMs: 50 };

      await executeBatchSave(requests, saveFn, config);

      // All items in first batch should complete before second batch
      expect(callOrder.slice(0, 2)).toContain(1);
      expect(callOrder.slice(0, 2)).toContain(2);
      expect(callOrder.slice(2, 4)).toContain(3);
      expect(callOrder.slice(2, 4)).toContain(4);
    });

    it("should propagate errors from save function", async () => {
      const requests = [1, 2, 3];
      const saveFn = vi.fn().mockRejectedValue(new Error("Save failed"));
      const config: BatchConfig = { batchSize: 10, delayMs: 100 };

      await expect(executeBatchSave(requests, saveFn, config)).rejects.toThrow("Save failed");
    });

    it("should not delay after the last batch", async () => {
      vi.useFakeTimers();

      const requests = [1, 2, 3];
      const saveFn = vi.fn().mockResolvedValue(undefined);
      const config: BatchConfig = { batchSize: 2, delayMs: 1000 };

      const executePromise = executeBatchSave(requests, saveFn, config);

      // Should have only one delay (between batch 1 and 2), not after batch 2
      await vi.advanceTimersByTimeAsync(1000);

      await executePromise;

      // Verify no additional delay occurred (if there were 2 delays, timer would need to advance more)
      expect(saveFn).toHaveBeenCalledTimes(3);

      vi.useRealTimers();
    });

    it("should handle exact multiple of batch size", async () => {
      const requests = [1, 2, 3, 4, 5, 6];
      const saveFn = vi.fn().mockResolvedValue(undefined);
      const config: BatchConfig = { batchSize: 3, delayMs: 10 };

      await executeBatchSave(requests, saveFn, config);

      expect(saveFn).toHaveBeenCalledTimes(6);
    });
  });
});
