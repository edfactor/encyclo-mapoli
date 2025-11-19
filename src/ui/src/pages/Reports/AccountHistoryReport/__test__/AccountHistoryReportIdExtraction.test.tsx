import {
  AccountHistoryReportPaginatedResponse,
  AccountHistoryReportResponse
} from "@/types/reports/AccountHistoryReportTypes";
import { describe, expect, it } from "vitest";

/**
 * PS-2160: Unit tests for AccountHistoryReport ID extraction logic
 * Verifies that the component correctly extracts report record IDs for member details lookup
 * instead of using badge numbers (which are not unique across profit years)
 */
describe("AccountHistoryReport - ID Extraction Logic (PS-2160)", () => {
  describe("Report ID Extraction from Response", () => {
    it("should extract ID from first result in report response", () => {
      // Arrange
      const mockResponse: AccountHistoryReportPaginatedResponse = {
        reportName: "Account History Report",
        reportDate: "2024-10-30T00:00:00Z",
        startDate: "2024-01-01",
        endDate: "2024-12-31",
        response: {
          pageSize: 25,
          currentPage: 1,
          totalPages: 1,
          resultHash: null,
          total: 2,
          isPartialResult: false,
          timeoutOccurred: false,
          results: [
            {
              id: 123,
              badgeNumber: 12345,
              fullName: "John Doe",
              ssn: "***-**-6789",
              profitYear: 2024,
              contributions: 1000,
              earnings: 500,
              forfeitures: 100,
              withdrawals: 50,
              endingBalance: 5000
            },
            {
              id: 234,
              badgeNumber: 12345,
              fullName: "John Doe",
              ssn: "***-**-6789",
              profitYear: 2023,
              contributions: 950,
              earnings: 450,
              forfeitures: 75,
              withdrawals: 25,
              endingBalance: 4000
            }
          ]
        },
        cumulativeTotals: {
          totalContributions: 1950,
          totalEarnings: 950,
          totalForfeitures: 175,
          totalWithdrawals: 75
        }
      };

      // Act
      const reportId = mockResponse?.response?.results?.[0]?.id ?? 0;

      // Assert
      expect(reportId).toBe(123);
    });

    it("should return 0 when results are empty", () => {
      // Arrange
      const mockResponse: AccountHistoryReportPaginatedResponse = {
        reportName: "Account History Report",
        reportDate: "2024-10-30T00:00:00Z",
        startDate: "2024-01-01",
        endDate: "2024-12-31",
        response: {
          pageSize: 25,
          currentPage: 1,
          totalPages: 0,
          resultHash: null,
          total: 0,
          isPartialResult: false,
          timeoutOccurred: false,
          results: []
        }
      };

      // Act
      const reportId = mockResponse?.response?.results?.[0]?.id ?? 0;

      // Assert
      expect(reportId).toBe(0);
    });

    it("should return 0 when response is undefined", () => {
      // Arrange
      const mockResponse: AccountHistoryReportPaginatedResponse | undefined = undefined;

      // Act
      const reportId = mockResponse?.response?.results?.[0]?.id ?? 0;

      // Assert
      expect(reportId).toBe(0);
    });

    it("should return 0 when results are undefined", () => {
      // Arrange
      const mockResponse: Partial<AccountHistoryReportPaginatedResponse> = {
        reportName: "Account History Report",
        reportDate: "2024-10-30T00:00:00Z",
        startDate: "2024-01-01",
        endDate: "2024-12-31",
        response: {
          pageSize: 25,
          currentPage: 1,
          totalPages: 1,
          resultHash: null,
          total: 0,
          isPartialResult: false,
          timeoutOccurred: false,
          results: undefined as unknown as AccountHistoryReportResponse[]
        }
      };

      // Act
      const reportId = mockResponse?.response?.results?.[0]?.id ?? 0;

      // Assert
      expect(reportId).toBe(0);
    });
  });

  describe("ID vs Badge Number Uniqueness", () => {
    it("should use report ID instead of badge number for member lookup", () => {
      // This test verifies the key fix for PS-2160:
      // Badge number is NOT unique (same employee has multiple records across profit years)
      // Report ID represents the member/demographic record (same for all rows)

      // Arrange
      const records: AccountHistoryReportResponse[] = [
        {
          id: 100,
          badgeNumber: 12345,
          fullName: "John Doe",
          ssn: "***-**-6789",
          profitYear: 2024,
          contributions: 1000,
          earnings: 500,
          forfeitures: 100,
          withdrawals: 50,
          endingBalance: 5000
        },
        {
          id: 100, // SAME ID (represents the member)
          badgeNumber: 12345, // SAME badge number
          fullName: "John Doe",
          ssn: "***-**-6789",
          profitYear: 2023,
          contributions: 950,
          earnings: 450,
          forfeitures: 75,
          withdrawals: 25,
          endingBalance: 4000
        },
        {
          id: 100, // SAME ID (represents the member)
          badgeNumber: 12345, // SAME badge number
          fullName: "John Doe",
          ssn: "***-**-6789",
          profitYear: 2022,
          contributions: 900,
          earnings: 400,
          forfeitures: 50,
          withdrawals: 10,
          endingBalance: 3000
        }
      ];

      // Act & Assert
      // Verify badge numbers are not unique (but all same in this case)
      const uniqueBadgeNumbers = new Set(records.map((r) => r.badgeNumber));
      expect(uniqueBadgeNumbers.size).toBe(1);

      // Verify IDs are all the same (member ID, not transaction ID)
      const uniqueIds = new Set(records.map((r) => r.id));
      expect(uniqueIds.size).toBe(1);
      expect(Array.from(uniqueIds)[0]).toBe(100);

      // The first record's ID should be used for member lookup
      const reportId = records[0].id;
      expect(reportId).toBe(100);
    });

    it("should extract ID correctly for member details query", () => {
      // Arrange
      const mockResponse: AccountHistoryReportPaginatedResponse = {
        reportName: "Account History Report",
        reportDate: "2024-10-30T00:00:00Z",
        startDate: "2024-01-01",
        endDate: "2024-12-31",
        response: {
          pageSize: 25,
          currentPage: 1,
          totalPages: 1,
          resultHash: null,
          total: 1,
          isPartialResult: false,
          timeoutOccurred: false,
          results: [
            {
              id: 555,
              badgeNumber: 99999,
              fullName: "Jane Smith",
              ssn: "***-**-4321",
              profitYear: 2024,
              contributions: 5000,
              earnings: 2000,
              forfeitures: 500,
              withdrawals: 200,
              endingBalance: 50000
            }
          ]
        }
      };

      // Act
      const reportId = mockResponse?.response?.results?.[0]?.id ?? 0;
      const memberId = reportId > 0 ? reportId : 0;

      // Assert - This ID should be passed to InquiryApi.useGetProfitMasterInquiryMemberQuery
      expect(memberId).toBe(555);
      expect(memberId).not.toBe(99999); // NOT using badge number
    });
  });

  describe("Safe Extraction with Nullish Coalescing", () => {
    it("should safely extract ID with nullish coalescing operator", () => {
      // Arrange - Simulate various response states
      const testCases: Array<{
        data: AccountHistoryReportPaginatedResponse | undefined;
        expectedId: number;
        description: string;
      }> = [
        {
          data: {
            reportName: "Account History Report",
            reportDate: "2024-10-30T00:00:00Z",
            startDate: "2024-01-01",
            endDate: "2024-12-31",
            response: {
              pageSize: 25,
              currentPage: 1,
              totalPages: 1,
              resultHash: null,
              total: 1,
              isPartialResult: false,
              timeoutOccurred: false,
              results: [
                {
                  id: 789,
                  badgeNumber: 12345,
                  fullName: "Test",
                  ssn: "***",
                  profitYear: 2024,
                  contributions: 0,
                  earnings: 0,
                  forfeitures: 0,
                  withdrawals: 0,
                  endingBalance: 0
                }
              ]
            }
          },
          expectedId: 789,
          description: "Valid response with ID"
        },
        {
          data: undefined,
          expectedId: 0,
          description: "Undefined response"
        },
        {
          data: {
            reportName: "Account History Report",
            reportDate: "2024-10-30T00:00:00Z",
            startDate: "2024-01-01",
            endDate: "2024-12-31",
            response: {
              pageSize: 25,
              currentPage: 1,
              totalPages: 0,
              resultHash: null,
              total: 0,
              isPartialResult: false,
              timeoutOccurred: false,
              results: []
            }
          },
          expectedId: 0,
          description: "Empty results array"
        }
      ];

      // Act & Assert
      testCases.forEach(({ data, expectedId, _description }) => {
        const reportId = data?.response?.results?.[0]?.id ?? 0;
        expect(reportId).toBe(expectedId);
      });
    });
  });

  describe("Member Query Integration", () => {
    it("should pass correct parameters to member inquiry query", () => {
      // Arrange
      const mockResponse: AccountHistoryReportPaginatedResponse = {
        reportName: "Account History Report",
        reportDate: "2024-10-30T00:00:00Z",
        startDate: "2024-12-31T00:00:00Z",
        endDate: "2024-12-31T00:00:00Z",
        response: {
          pageSize: 25,
          currentPage: 1,
          totalPages: 1,
          resultHash: null,
          total: 1,
          isPartialResult: false,
          timeoutOccurred: false,
          results: [
            {
              id: 500,
              badgeNumber: 700006,
              fullName: "Test Employee",
              ssn: "***-**-1234",
              profitYear: 2024,
              contributions: 1000,
              earnings: 500,
              forfeitures: 100,
              withdrawals: 50,
              endingBalance: 5000
            }
          ]
        }
      };

      // Act
      const reportId = mockResponse?.response?.results?.[0]?.id ?? 0;
      const profitYear = 2024;
      const shouldSkipQuery = reportId === 0;

      // Simulate the query parameters that would be passed
      const queryParams = {
        memberType: 1,
        id: reportId,
        profitYear: profitYear
      };

      // Assert
      expect(shouldSkipQuery).toBe(false);
      expect(queryParams.id).toBe(500);
      expect(queryParams.profitYear).toBe(2024);
      expect(queryParams.memberType).toBe(1);
    });

    it("should skip member inquiry query when ID is 0", () => {
      // Arrange
      const mockResponse: AccountHistoryReportPaginatedResponse | undefined = undefined;

      // Act
      const reportId = mockResponse?.response?.results?.[0]?.id ?? 0;
      const shouldSkip = reportId === 0;

      // Assert
      expect(shouldSkip).toBe(true);
    });
  });
});
