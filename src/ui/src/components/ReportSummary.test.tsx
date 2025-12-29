import { render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import ReportSummary from "./ReportSummary";
import { PagedReportResponse } from "../reduxstore/types";

// Mock the utility functions
vi.mock("smart-ui-library", () => ({
  formatNumberWithComma: (num: number | undefined) => {
    if (num === undefined || num === null) return "0";
    return num.toLocaleString("en-US");
  }
}));

vi.mock("./ReportSummaryUtils", () => ({
  shouldShowDataSource: vi.fn(() => false),
  renderDateRangeLabel: vi.fn((report) => {
    if (report.reportName === "Distributions and Forfeitures") {
      return `Report range: ${report.startDate} to ${report.endDate}`;
    }
    return `Report range: ${report.startDate} to ${report.endDate}`;
  })
}));

// Import the mocked functions for manipulation in tests
import { shouldShowDataSource, renderDateRangeLabel } from "./ReportSummaryUtils";

// Helper to create a complete mock report
function createMockReport(overrides: Partial<PagedReportResponse<unknown>> = {}): PagedReportResponse<unknown> {
  return {
    reportName: "Test Report",
    reportDate: "2024-12-25",
    startDate: "2024-01-01",
    endDate: "2024-12-31",
    dataSource: "Production",
    response: {
      results: [],
      total: 0,
      totalPages: 1,
      pageSize: 50,
      currentPage: 1,
      ...(overrides.response || {})
    },
    ...overrides
  } as PagedReportResponse<unknown>;
}

describe("ReportSummary", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("Rendering", () => {
    it("should render report name and total count", () => {
      const mockReport = createMockReport({
        reportName: "Employee Report",
        response: { results: [], total: 150, totalPages: 1, pageSize: 50, currentPage: 1 }
      });

      render(<ReportSummary report={mockReport} />);

      expect(screen.getByText(/Employee Report/)).toBeInTheDocument();
      expect(screen.getByText(/150/)).toBeInTheDocument();
    });

    it("should format total count with commas", () => {
      const mockReport = createMockReport({
        reportName: "Large Report",
        response: { results: [], total: 1234567, totalPages: 100, pageSize: 50, currentPage: 1 }
      });

      render(<ReportSummary report={mockReport} />);

      expect(screen.getByText(/Large Report \(1,234,567\)/)).toBeInTheDocument();
    });

    it("should render date range chip", () => {
      const mockReport = createMockReport({
        reportName: "Test Report",
        startDate: "01/01/2024",
        endDate: "12/31/2024",
        response: { results: [], total: 50, totalPages: 1, pageSize: 50, currentPage: 1 }
      });

      render(<ReportSummary report={mockReport} />);

      expect(screen.getByText(/Report range: 01\/01\/2024 to 12\/31\/2024/)).toBeInTheDocument();
    });

    it("should show data source chip when shouldShowDataSource returns true", () => {
      vi.mocked(shouldShowDataSource).mockReturnValue(true);

      const mockReport = createMockReport({
        reportName: "Development Report",
        dataSource: "QA Database",
        response: { results: [], total: 25, totalPages: 1, pageSize: 50, currentPage: 1 }
      });

      render(<ReportSummary report={mockReport} />);

      expect(screen.getByText("Data Source: QA Database")).toBeInTheDocument();
    });

    it("should hide data source chip when shouldShowDataSource returns false", () => {
      vi.mocked(shouldShowDataSource).mockReturnValue(false);

      const mockReport = createMockReport({
        reportName: "Production Report",
        response: { results: [], total: 100, totalPages: 1, pageSize: 50, currentPage: 1 }
      });

      render(<ReportSummary report={mockReport} />);

      expect(screen.queryByText(/Data Source:/)).not.toBeInTheDocument();
    });

    it("should display zero when total is zero", () => {
      const mockReport = createMockReport({
        reportName: "Empty Report",
        response: { results: [], total: 0, totalPages: 0, pageSize: 50, currentPage: 1 }
      });

      render(<ReportSummary report={mockReport} />);

      expect(screen.getByText(/Empty Report \(0\)/)).toBeInTheDocument();
    });

    it("should handle missing report name gracefully", () => {
      const mockReport = createMockReport({
        reportName: undefined,
        response: { results: [], total: 42, totalPages: 1, pageSize: 50, currentPage: 1 }
      });

      render(<ReportSummary report={mockReport} />);

      // Should render total count even without report name
      expect(screen.getByText(/\(42\)/)).toBeInTheDocument();
    });
  });

  describe("Date Range Formatting", () => {
    it("should call renderDateRangeLabel with correct report data", () => {
      const mockReport = createMockReport({
        reportName: "Test Report",
        response: { results: [], total: 10, totalPages: 1, pageSize: 50, currentPage: 1 }
      });

      render(<ReportSummary report={mockReport} />);

      expect(renderDateRangeLabel).toHaveBeenCalledWith(mockReport);
    });

    it("should display custom date format for Distributions and Forfeitures report", () => {
      vi.mocked(renderDateRangeLabel).mockReturnValue("Report range: 01/2024 to 12/2024");

      const mockReport = createMockReport({
        reportName: "Distributions and Forfeitures",
        response: { results: [], total: 75, totalPages: 1, pageSize: 50, currentPage: 1 }
      });

      render(<ReportSummary report={mockReport} />);

      expect(screen.getByText("Report range: 01/2024 to 12/2024")).toBeInTheDocument();
    });
  });

  describe("Data Source Display", () => {
    it("should show data source in development/QA environments", () => {
      vi.mocked(shouldShowDataSource).mockReturnValue(true);

      const mockReport = createMockReport({
        reportName: "QA Report",
        dataSource: "Development DB",
        response: { results: [], total: 30, totalPages: 1, pageSize: 50, currentPage: 1 }
      });

      render(<ReportSummary report={mockReport} />);

      const dataSourceChip = screen.getByText("Data Source: Development DB");
      expect(dataSourceChip).toBeInTheDocument();
      expect(dataSourceChip.closest(".MuiChip-root")).toBeInTheDocument();
    });

    it("should apply correct styling to data source chip", () => {
      vi.mocked(shouldShowDataSource).mockReturnValue(true);

      const mockReport = createMockReport({
        reportName: "Test Report",
        dataSource: "Test Database",
        response: { results: [], total: 20, totalPages: 1, pageSize: 50, currentPage: 1 }
      });

      render(<ReportSummary report={mockReport} />);

      const dataSourceChip = screen.getByText("Data Source: Test Database").closest(".MuiChip-root");
      expect(dataSourceChip).toHaveClass("bg-dsm-blue");
    });
  });

  describe("Styling and Layout", () => {
    it("should apply correct CSS classes to container", () => {
      const mockReport = createMockReport({
        reportName: "Styled Report",
        response: { results: [], total: 5, totalPages: 1, pageSize: 50, currentPage: 1 }
      });

      const { container } = render(<ReportSummary report={mockReport} />);

      const mainContainer = container.querySelector(".flex.flex-wrap.items-center.gap-2");
      expect(mainContainer).toBeInTheDocument();
    });

    it("should apply secondary color to report title", () => {
      const mockReport = createMockReport({
        reportName: "Colored Report",
        response: { results: [], total: 15, totalPages: 1, pageSize: 50, currentPage: 1 }
      });

      render(<ReportSummary report={mockReport} />);

      const title = screen.getByText(/Colored Report/);
      expect(title).toHaveClass("text-dsm-secondary");
      expect(title.closest("h2")).toHaveClass("MuiTypography-h2");
    });

    it("should apply correct styling to date range chip", () => {
      const mockReport = createMockReport({
        reportName: "Test Report",
        response: { results: [], total: 8, totalPages: 1, pageSize: 50, currentPage: 1 }
      });

      render(<ReportSummary report={mockReport} />);

      const dateChip = screen.getByText(/Report range:/).closest(".MuiChip-root");
      expect(dateChip).toHaveClass("bg-dsm-grey-hover");
    });
  });

  describe("Edge Cases", () => {
    it("should handle very large total numbers", () => {
      const mockReport = createMockReport({
        reportName: "Massive Report",
        response: { results: [], total: 999999999, totalPages: 1000, pageSize: 50, currentPage: 1 }
      });

      render(<ReportSummary report={mockReport} />);

      expect(screen.getByText(/Massive Report \(999,999,999\)/)).toBeInTheDocument();
    });

    it("should handle report names with special characters", () => {
      const mockReport = createMockReport({
        reportName: "Q&A Report - 2024/25",
        response: { results: [], total: 33, totalPages: 1, pageSize: 50, currentPage: 1 }
      });

      render(<ReportSummary report={mockReport} />);

      expect(screen.getByText(/Q&A Report - 2024\/25 \(33\)/)).toBeInTheDocument();
    });

    it("should handle undefined response total", () => {
      const mockReport = {
        reportName: "Undefined Total Report",
        reportDate: "2024-12-25",
        response: {
          results: [],
          total: undefined,
          totalPages: 1,
          pageSize: 50,
          currentPage: 1
        },
        startDate: "2024-01-01",
        endDate: "2024-12-31",
        dataSource: "Production"
      } as unknown as PagedReportResponse<unknown>;

      render(<ReportSummary report={mockReport} />);

      // Should fallback to 0 when total is undefined
      expect(screen.getByText(/Undefined Total Report \(0\)/)).toBeInTheDocument();
    });

    it("should handle empty strings in dates", () => {
      vi.mocked(renderDateRangeLabel).mockReturnValue("Report range:  to ");

      const mockReport = createMockReport({
        reportName: "No Date Report",
        startDate: "",
        endDate: "",
        response: { results: [], total: 12, totalPages: 1, pageSize: 50, currentPage: 1 }
      });

      render(<ReportSummary report={mockReport} />);

      expect(screen.getByText(/Report range:.*to/)).toBeInTheDocument();
    });

    it("should render with all fields populated", () => {
      vi.mocked(shouldShowDataSource).mockReturnValue(true);

      const mockReport = createMockReport({
        reportName: "Complete Report",
        startDate: "01/01/2024",
        endDate: "12/31/2024",
        dataSource: "Complete DB",
        response: { results: [], total: 500, totalPages: 10, pageSize: 50, currentPage: 1 }
      });

      render(<ReportSummary report={mockReport} />);

      expect(screen.getByText(/Complete Report \(500\)/)).toBeInTheDocument();
      expect(screen.getByText(/Report range:/)).toBeInTheDocument();
      expect(screen.getByText("Data Source: Complete DB")).toBeInTheDocument();
    });
  });

  describe("Chip Layout", () => {
    it("should render chips in correct container", () => {
      const mockReport = createMockReport({
        reportName: "Layout Test",
        response: { results: [], total: 7, totalPages: 1, pageSize: 50, currentPage: 1 }
      });

      const { container } = render(<ReportSummary report={mockReport} />);

      const chipContainer = container.querySelector(".flex.flex-wrap.gap-1");
      expect(chipContainer).toBeInTheDocument();

      const chips = chipContainer?.querySelectorAll(".MuiChip-root");
      expect(chips?.length).toBeGreaterThan(0);
    });

    it("should render date chip always and data source chip conditionally", () => {
      vi.mocked(shouldShowDataSource).mockReturnValue(false);

      const mockReport = createMockReport({
        reportName: "Conditional Chips",
        response: { results: [], total: 18, totalPages: 1, pageSize: 50, currentPage: 1 }
      });

      const { container } = render(<ReportSummary report={mockReport} />);

      const chips = container.querySelectorAll(".MuiChip-root");
      expect(chips.length).toBe(1); // Only date chip

      vi.mocked(shouldShowDataSource).mockReturnValue(true);
      const { container: container2 } = render(<ReportSummary report={mockReport} />);

      const chips2 = container2.querySelectorAll(".MuiChip-root");
      expect(chips2.length).toBe(2); // Date chip + data source chip
    });
  });
});
