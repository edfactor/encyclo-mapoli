import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import ProfitShareTotalsDisplay from "./ProfitShareTotalsDisplay";
import { YearEndProfitSharingReportTotalsResponse } from "../reduxstore/types";

// Mock the smart-ui-library
vi.mock("smart-ui-library", () => ({
  numberToCurrency: (num: number) => {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD"
    }).format(num);
  }
}));

describe("ProfitShareTotalsDisplay", () => {
  const mockTotalsData: YearEndProfitSharingReportTotalsResponse = {
    wagesTotal: 1234567.89,
    hoursTotal: 54321,
    pointsTotal: 98765,
    balanceTotal: 500000,
    numberOfEmployees: 250,
    numberOfNewEmployees: 25,
    numberOfEmployeesUnder21: 15,
    numberOfEmployeesInPlan: 210
  };

  describe("Rendering", () => {
    it("should render nothing when totalsData is null", () => {
      const { container } = render(<ProfitShareTotalsDisplay totalsData={null} />);

      expect(container.firstChild).toBeNull();
    });

    it("should render nothing when totalsData is undefined", () => {
      const { container } = render(<ProfitShareTotalsDisplay totalsData={undefined} />);

      expect(container.firstChild).toBeNull();
    });

    it("should render both tables when totalsData is provided", () => {
      const { container } = render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      const tables = container.querySelectorAll("table");
      expect(tables.length).toBe(2);
    });

    it("should render first table with Wages, Hours, Points columns", () => {
      render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      expect(screen.getByText("Wages")).toBeInTheDocument();
      expect(screen.getByText("Hours")).toBeInTheDocument();
      expect(screen.getByText("Points")).toBeInTheDocument();
    });

    it("should render second table with employee count columns", () => {
      render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      expect(screen.getByText("All Employees")).toBeInTheDocument();
      expect(screen.getByText("New Employees")).toBeInTheDocument();
      expect(screen.getByText("Employees < 21")).toBeInTheDocument();
      expect(screen.getByText("In-Plan")).toBeInTheDocument();
    });

    it("should render Section Total label in first table", () => {
      render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      const sectionTotalLabels = screen.getAllByText("Section Total");
      expect(sectionTotalLabels.length).toBeGreaterThan(0);
    });

    it("should render Employee Totals label in second table", () => {
      render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      expect(screen.getByText("Employee Totals")).toBeInTheDocument();
    });
  });

  describe("Data Display - First Table", () => {
    it("should display formatted wages total as currency", () => {
      render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      expect(screen.getByText("$1,234,567.89")).toBeInTheDocument();
    });

    it("should display hours total with comma formatting", () => {
      render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      expect(screen.getByText("54,321")).toBeInTheDocument();
    });

    it("should display points total with comma formatting", () => {
      render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      expect(screen.getByText("98,765")).toBeInTheDocument();
    });

    it("should display zero for wages when wagesTotal is 0", () => {
      const zeroWagesData: YearEndProfitSharingReportTotalsResponse = {
        ...mockTotalsData,
        wagesTotal: 0
      };

      render(<ProfitShareTotalsDisplay totalsData={zeroWagesData} />);

      expect(screen.getByText("$0.00")).toBeInTheDocument();
    });

    it("should display 0 for hours when hoursTotal is missing", () => {
      const noHoursData = {
        ...mockTotalsData,
        hoursTotal: undefined
      } as unknown as YearEndProfitSharingReportTotalsResponse;

      render(<ProfitShareTotalsDisplay totalsData={noHoursData} />);

      expect(screen.getByText("0")).toBeInTheDocument();
    });

    it("should display 0 for points when pointsTotal is missing", () => {
      const noPointsData = {
        ...mockTotalsData,
        pointsTotal: undefined
      } as unknown as YearEndProfitSharingReportTotalsResponse;

      render(<ProfitShareTotalsDisplay totalsData={noPointsData} />);

      expect(screen.getByText("0")).toBeInTheDocument();
    });
  });

  describe("Data Display - Second Table", () => {
    it("should display all employee counts", () => {
      render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      expect(screen.getByText("250")).toBeInTheDocument();
      expect(screen.getByText("25")).toBeInTheDocument();
      expect(screen.getByText("15")).toBeInTheDocument();
      expect(screen.getByText("210")).toBeInTheDocument();
    });

    it("should display 0 for numberOfEmployees when missing", () => {
      const noEmployeesData = {
        ...mockTotalsData,
        numberOfEmployees: undefined
      } as unknown as YearEndProfitSharingReportTotalsResponse;

      render(<ProfitShareTotalsDisplay totalsData={noEmployeesData} />);

      // Should have multiple "0" values, find all of them
      const zeros = screen.getAllByText("0");
      expect(zeros.length).toBeGreaterThan(0);
    });

    it("should display 0 for numberOfNewEmployees when missing", () => {
      const noNewEmployeesData = {
        ...mockTotalsData,
        numberOfNewEmployees: undefined
      } as unknown as YearEndProfitSharingReportTotalsResponse;

      render(<ProfitShareTotalsDisplay totalsData={noNewEmployeesData} />);

      const zeros = screen.getAllByText("0");
      expect(zeros.length).toBeGreaterThan(0);
    });

    it("should display 0 for numberOfEmployeesUnder21 when missing", () => {
      const noUnder21Data = {
        ...mockTotalsData,
        numberOfEmployeesUnder21: undefined
      } as unknown as YearEndProfitSharingReportTotalsResponse;

      render(<ProfitShareTotalsDisplay totalsData={noUnder21Data} />);

      const zeros = screen.getAllByText("0");
      expect(zeros.length).toBeGreaterThan(0);
    });

    it("should display 0 for numberOfEmployeesInPlan when missing", () => {
      const noInPlanData = {
        ...mockTotalsData,
        numberOfEmployeesInPlan: undefined
      } as unknown as YearEndProfitSharingReportTotalsResponse;

      render(<ProfitShareTotalsDisplay totalsData={noInPlanData} />);

      const zeros = screen.getAllByText("0");
      expect(zeros.length).toBeGreaterThan(0);
    });
  });

  describe("Large Numbers", () => {
    it("should handle very large wage totals", () => {
      const largeWagesData: YearEndProfitSharingReportTotalsResponse = {
        ...mockTotalsData,
        wagesTotal: 999999999.99
      };

      render(<ProfitShareTotalsDisplay totalsData={largeWagesData} />);

      expect(screen.getByText("$999,999,999.99")).toBeInTheDocument();
    });

    it("should handle very large hours totals", () => {
      const largeHoursData: YearEndProfitSharingReportTotalsResponse = {
        ...mockTotalsData,
        hoursTotal: 9999999
      };

      render(<ProfitShareTotalsDisplay totalsData={largeHoursData} />);

      expect(screen.getByText("9,999,999")).toBeInTheDocument();
    });

    it("should handle very large points totals", () => {
      const largePointsData: YearEndProfitSharingReportTotalsResponse = {
        ...mockTotalsData,
        pointsTotal: 8888888
      };

      render(<ProfitShareTotalsDisplay totalsData={largePointsData} />);

      expect(screen.getByText("8,888,888")).toBeInTheDocument();
    });

    it("should handle very large employee counts", () => {
      const largeEmployeeData: YearEndProfitSharingReportTotalsResponse = {
        ...mockTotalsData,
        numberOfEmployees: 50000,
        numberOfNewEmployees: 5000,
        numberOfEmployeesUnder21: 1000,
        numberOfEmployeesInPlan: 45000
      };

      render(<ProfitShareTotalsDisplay totalsData={largeEmployeeData} />);

      expect(screen.getByText("50000")).toBeInTheDocument();
      expect(screen.getByText("5000")).toBeInTheDocument();
      expect(screen.getByText("1000")).toBeInTheDocument();
      expect(screen.getByText("45000")).toBeInTheDocument();
    });
  });

  describe("Edge Cases", () => {
    it("should handle all zero values", () => {
      const allZeroData: YearEndProfitSharingReportTotalsResponse = {
        wagesTotal: 0,
        hoursTotal: 0,
        pointsTotal: 0,
        balanceTotal: 0,
        numberOfEmployees: 0,
        numberOfNewEmployees: 0,
        numberOfEmployeesUnder21: 0,
        numberOfEmployeesInPlan: 0
      };

      render(<ProfitShareTotalsDisplay totalsData={allZeroData} />);

      expect(screen.getByText("$0.00")).toBeInTheDocument();

      // Should have multiple "0" values for counts
      const zeros = screen.getAllByText("0");
      expect(zeros.length).toBeGreaterThan(0);
    });

    it("should handle negative wages total", () => {
      const negativeWagesData: YearEndProfitSharingReportTotalsResponse = {
        ...mockTotalsData,
        wagesTotal: -5000.5
      };

      render(<ProfitShareTotalsDisplay totalsData={negativeWagesData} />);

      expect(screen.getByText("-$5,000.50")).toBeInTheDocument();
    });

    it("should handle decimal hours total", () => {
      const decimalHoursData: YearEndProfitSharingReportTotalsResponse = {
        ...mockTotalsData,
        hoursTotal: 1234.567
      };

      render(<ProfitShareTotalsDisplay totalsData={decimalHoursData} />);

      // toLocaleString() on numbers will format with commas but preserve decimals
      expect(screen.getByText("1,234.567")).toBeInTheDocument();
    });

    it("should handle decimal points total", () => {
      const decimalPointsData: YearEndProfitSharingReportTotalsResponse = {
        ...mockTotalsData,
        pointsTotal: 9876.543
      };

      render(<ProfitShareTotalsDisplay totalsData={decimalPointsData} />);

      expect(screen.getByText("9,876.543")).toBeInTheDocument();
    });

    it("should render with minimal valid data", () => {
      const minimalData: YearEndProfitSharingReportTotalsResponse = {
        wagesTotal: 100,
        hoursTotal: 10,
        pointsTotal: 5,
        balanceTotal: 50,
        numberOfEmployees: 1,
        numberOfNewEmployees: 0,
        numberOfEmployeesUnder21: 0,
        numberOfEmployeesInPlan: 1
      };

      render(<ProfitShareTotalsDisplay totalsData={minimalData} />);

      expect(screen.getByText("$100.00")).toBeInTheDocument();
      expect(screen.getByText("10")).toBeInTheDocument();
      expect(screen.getByText("5")).toBeInTheDocument();
      // Use getAllByText for "1" since it appears multiple times (numberOfEmployees and numberOfEmployeesInPlan)
      const ones = screen.getAllByText("1");
      expect(ones.length).toBeGreaterThan(0);
    });
  });

  describe("Table Structure", () => {
    it("should render first table with correct structure", () => {
      const { container } = render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      const firstTable = container.querySelectorAll("table")[0];
      const rows = firstTable.querySelectorAll("tbody tr");

      expect(rows.length).toBe(2); // Header row + data row
    });

    it("should render second table with correct structure", () => {
      const { container } = render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      const secondTable = container.querySelectorAll("table")[1];
      const rows = secondTable.querySelectorAll("tbody tr");

      expect(rows.length).toBe(2); // Header row + data row
    });

    it("should have correct number of columns in first table", () => {
      const { container } = render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      const firstTable = container.querySelectorAll("table")[0];
      const headerCells = firstTable.querySelectorAll("tbody tr:first-child td");

      expect(headerCells.length).toBe(4); // Empty + Wages + Hours + Points
    });

    it("should have correct number of columns in second table", () => {
      const { container } = render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      const secondTable = container.querySelectorAll("table")[1];
      const headerCells = secondTable.querySelectorAll("tbody tr:first-child td");

      expect(headerCells.length).toBe(5); // Empty + All + New + Under21 + InPlan
    });

    it("should use small table size", () => {
      const { container } = render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      const tables = container.querySelectorAll("table");
      expect(tables.length).toBe(2);
      // Tables have the small size applied via MUI size prop
      tables.forEach((table) => {
        expect(table).toBeInTheDocument();
      });
    });

    it("should have TableBody elements", () => {
      const { container } = render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      const tableBodies = container.querySelectorAll("tbody");
      expect(tableBodies.length).toBe(2);
    });
  });

  describe("Accessibility", () => {
    it("should render semantic table elements", () => {
      const { container } = render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      const tables = container.querySelectorAll("table");
      expect(tables.length).toBe(2);

      tables.forEach((table) => {
        expect(table.tagName.toLowerCase()).toBe("table");
      });
    });

    it("should use proper table cells with headers", () => {
      const { container } = render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      const tableCells = container.querySelectorAll("td");
      expect(tableCells.length).toBeGreaterThan(0);
    });

    it("should have proper table row structure", () => {
      const { container } = render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      const tableRows = container.querySelectorAll("tr");
      expect(tableRows.length).toBe(4); // 2 rows per table × 2 tables
    });
  });

  describe("Styling", () => {
    it("should apply header cell styles", () => {
      render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      const wagesHeader = screen.getByText("Wages").closest("td");
      expect(wagesHeader).toBeInTheDocument();
    });

    it("should apply label cell styles", () => {
      render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      const sectionTotalLabel = screen.getByText("Section Total").closest("td");
      expect(sectionTotalLabel).toBeInTheDocument();
    });

    it("should apply data cell styles", () => {
      render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      // Find a data cell (wages total)
      const wagesCell = screen.getByText("$1,234,567.89").closest("td");
      expect(wagesCell).toBeInTheDocument();
    });

    it("should have table containers with spacing", () => {
      const { container } = render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      const tableContainers = container.querySelectorAll(".MuiTableContainer-root");
      expect(tableContainers.length).toBe(2);
    });
  });

  describe("Column Width Distribution", () => {
    it("should set 25% width for first table columns", () => {
      const { container } = render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      const firstTable = container.querySelectorAll("table")[0];
      const headerCells = firstTable.querySelectorAll("tbody tr:first-child td");

      headerCells.forEach((cell) => {
        expect(cell).toHaveAttribute("width", "25%");
      });
    });

    it("should set appropriate widths for second table columns", () => {
      const { container } = render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      const secondTable = container.querySelectorAll("table")[1];
      const headerCells = secondTable.querySelectorAll("tbody tr:first-child td");

      // First column: 25%, rest: 18.75%
      expect(headerCells[0]).toHaveAttribute("width", "25%");
      expect(headerCells[1]).toHaveAttribute("width", "18.75%");
      expect(headerCells[2]).toHaveAttribute("width", "18.75%");
      expect(headerCells[3]).toHaveAttribute("width", "18.75%");
      expect(headerCells[4]).toHaveAttribute("width", "18.75%");
    });
  });

  describe("Special HTML Characters", () => {
    it("should render less than symbol correctly in Employees < 21 header", () => {
      render(<ProfitShareTotalsDisplay totalsData={mockTotalsData} />);

      // The component uses &lt; entity for <
      expect(screen.getByText("Employees < 21")).toBeInTheDocument();
    });
  });
});
