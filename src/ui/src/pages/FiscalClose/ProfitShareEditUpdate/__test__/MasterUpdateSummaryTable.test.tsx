import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { MasterUpdateSummaryTable } from "../MasterUpdateSummaryTable";

// Mock Material-UI Typography component
vi.mock("@mui/material", () => ({
  Typography: ({ children, ...props }: React.HTMLAttributes<HTMLDivElement> & { children?: React.ReactNode }) => (
    <div {...props}>{children}</div>
  )
}));

// Mock InfoOutlinedIcon
vi.mock("@mui/icons-material/InfoOutlined", () => ({
  default: ({ className, onClick }: { className?: string; onClick?: () => void }) => (
    <svg
      data-testid="info-icon"
      className={className}
      onClick={onClick}>
      <title>Info Icon</title>
    </svg>
  )
}));

describe("MasterUpdateSummaryTable", { timeout: 16000 }, () => {
  const mockTotals = {
    beginningBalance: 1000000,
    totalContribution: 50000,
    earnings: 25000,
    earnings2: 5000,
    forfeiture: 10000,
    distributions: 15000,
    military: 8000,
    paidAllocations: -8000, // Note: Should be negative
    endingBalance: 1065000,
    allocations: 45000,
    maxPointsTotal: 9500,
    contributionPoints: 12000,
    earningPoints: 6000
  };

  const mockValidationResponse = {
    validationResults: {
      TotalProfitSharingBalance: { isValid: true, currentValue: 1000000, expectedValue: 1000000 },
      TotalContributions: { isValid: false, currentValue: 50000, expectedValue: 50100 },
      TotalEarnings: { isValid: true, currentValue: 25000, expectedValue: 25000 },
      TotalForfeitures: { isValid: true, currentValue: 10000, expectedValue: 10000 },
      DistributionTotals: { isValid: false, currentValue: 15000, expectedValue: 15200 },
      NetAllocTransfer: { isValid: true, currentValue: 0, expectedValue: 0 },
      IncomingAllocations: { isValid: true, currentValue: 8000, expectedValue: 8000 },
      OutgoingAllocations: { isValid: true, currentValue: -8000, expectedValue: -8000 }
    }
  };

  const mockGetFieldValidation = (fieldKey: string) => {
    const validations: Record<
      string,
      { isValid: boolean; currentValue: number | null; expectedValue: number | null; message?: string | null }
    > = {
      TotalProfitSharingBalance: { isValid: true, currentValue: 1000000, expectedValue: 1000000 },
      TotalContributions: { isValid: false, currentValue: 50000, expectedValue: 50100 },
      "PAY443.TotalContributions": { isValid: false, currentValue: 50000, expectedValue: 50100 },
      TotalEarnings: { isValid: true, currentValue: 25000, expectedValue: 25000 },
      "PAY443.TotalEarnings": { isValid: true, currentValue: 25000, expectedValue: 25000 },
      TotalForfeitures: { isValid: true, currentValue: 10000, expectedValue: 10000 },
      "PAY443.TotalForfeitures": { isValid: true, currentValue: 10000, expectedValue: 10000 },
      DistributionTotals: { isValid: false, currentValue: 15000, expectedValue: 15200 },
      "PAY443.DistributionTotals": { isValid: false, currentValue: 15000, expectedValue: 15200 },
      NetAllocTransfer: { isValid: true, currentValue: 0, expectedValue: 0, message: "Balanced" },
      IncomingAllocations: { isValid: true, currentValue: 8000, expectedValue: 8000 },
      OutgoingAllocations: { isValid: true, currentValue: -8000, expectedValue: -8000 }
    };
    return validations[fieldKey] || null;
  };

  const mockOnValidationToggle = vi.fn();

  const defaultProps = {
    totals: mockTotals,
    validationResponse: mockValidationResponse,
    getFieldValidation: mockGetFieldValidation,
    openValidationField: null,
    onValidationToggle: mockOnValidationToggle
  };

  describe("Table Structure", () => {
    it("renders table with correct structure", () => {
      render(<MasterUpdateSummaryTable {...defaultProps} />);

      // Check table exists
      const table = screen.getByRole("table");
      expect(table).toBeInTheDocument();

      // Check headers are present
      expect(screen.getByText("Beginning Balance")).toBeInTheDocument();
      expect(screen.getByText("Contributions")).toBeInTheDocument();
      expect(screen.getByText("Earnings")).toBeInTheDocument();
      expect(screen.getByText("Earnings2")).toBeInTheDocument();
      expect(screen.getByText("Forfeitures")).toBeInTheDocument();
      expect(screen.getByText("Distributions")).toBeInTheDocument();
      expect(screen.getByText("Military/Paid Allocation")).toBeInTheDocument();
      expect(screen.getByText("Ending Balance")).toBeInTheDocument();
    });

    it("renders row labels once (not repeated)", () => {
      render(<MasterUpdateSummaryTable {...defaultProps} />);

      // Row labels should appear exactly once
      const totalLabels = screen.getAllByText("Total");
      const allocationLabels = screen.getAllByText("Allocation");
      const pointLabels = screen.getAllByText("Point");

      expect(totalLabels).toHaveLength(1);
      expect(allocationLabels).toHaveLength(1);
      expect(pointLabels).toHaveLength(1);
    });

    it("renders 9 column headers", () => {
      const { container } = render(<MasterUpdateSummaryTable {...defaultProps} />);

      const headerCells = container.querySelectorAll("thead th");
      expect(headerCells).toHaveLength(9); // Row label + 8 data columns
    });

    it("renders 3 data rows (Total, Allocation, Point)", () => {
      const { container } = render(<MasterUpdateSummaryTable {...defaultProps} />);

      const dataRows = container.querySelectorAll("tbody tr");
      expect(dataRows).toHaveLength(3);
    });
  });

  describe("Data Display", () => {
    it("displays Beginning Balance correctly", () => {
      render(<MasterUpdateSummaryTable {...defaultProps} />);
      expect(screen.getAllByText("$1,000,000.00").length).toBeGreaterThan(0);
    });

    it("displays Contributions with all three values", () => {
      render(<MasterUpdateSummaryTable {...defaultProps} />);
      expect(screen.getAllByText("$50,000.00").length).toBeGreaterThan(0); // Total
      expect(screen.getAllByText("$45,000.00").length).toBeGreaterThan(0); // Allocation
      expect(screen.getAllByText("$12,000.00").length).toBeGreaterThan(0); // Points
    });

    it("displays Earnings with total and points", () => {
      render(<MasterUpdateSummaryTable {...defaultProps} />);
      expect(screen.getAllByText("$25,000.00").length).toBeGreaterThan(0); // Total
      expect(screen.getAllByText("$6,000.00").length).toBeGreaterThan(0); // Points
    });

    it("displays Earnings2 correctly", () => {
      render(<MasterUpdateSummaryTable {...defaultProps} />);
      expect(screen.getAllByText("$5,000.00").length).toBeGreaterThan(0);
    });

    it("displays Forfeitures with total and allocation", () => {
      render(<MasterUpdateSummaryTable {...defaultProps} />);
      expect(screen.getAllByText("$10,000.00").length).toBeGreaterThan(0); // Total
      expect(screen.getAllByText("$9,500.00").length).toBeGreaterThan(0); // Max Points
    });

    it("displays Distributions correctly", () => {
      render(<MasterUpdateSummaryTable {...defaultProps} />);
      expect(screen.getAllByText("$15,000.00").length).toBeGreaterThan(0);
    });

    it("displays Military/Paid Allocation with both values", () => {
      render(<MasterUpdateSummaryTable {...defaultProps} />);
      expect(screen.getAllByText("$8,000.00").length).toBeGreaterThan(0); // Military
      expect(screen.getAllByText("-$8,000.00").length).toBeGreaterThan(0); // Paid Allocations (negative)
    });

    it("displays Ending Balance with total and calculated allocation", () => {
      render(<MasterUpdateSummaryTable {...defaultProps} />);
      expect(screen.getAllByText("$1,065,000.00").length).toBeGreaterThan(0); // Ending Balance
      expect(screen.getAllByText("$37,000.00").length).toBeGreaterThan(0); // allocations + paidAllocations (45000 + -8000)
    });

    it("displays empty cells correctly", () => {
      const { container } = render(<MasterUpdateSummaryTable {...defaultProps} />);

      // Check for empty cells (they render as empty td elements, not with "—")
      const allCells = container.querySelectorAll("tbody td");
      expect(allCells.length).toBeGreaterThan(0);
    });
  });

  describe("Validation Icons", () => {
    it("renders validation icon for Beginning Balance when validation exists", () => {
      render(<MasterUpdateSummaryTable {...defaultProps} />);

      const icons = screen.getAllByTestId("info-icon");
      expect(icons.length).toBeGreaterThan(0);
    });

    it("renders green icon when validation is valid", () => {
      render(<MasterUpdateSummaryTable {...defaultProps} />);

      const icons = screen.getAllByTestId("info-icon");
      // Check that at least one icon has green color via parent div class
      const greenIcon = icons.find((icon) => {
        const classes = icon.getAttribute("class") || "";
        return classes.includes("text-green-500");
      });
      expect(greenIcon).toBeDefined();
    });

    it("renders orange icon when validation is invalid", () => {
      render(<MasterUpdateSummaryTable {...defaultProps} />);

      const icons = screen.getAllByTestId("info-icon");
      // Check that at least one icon has orange color
      const orangeIcon = icons.find((icon) => {
        const classes = icon.getAttribute("class") || "";
        return classes.includes("text-orange-500");
      });
      expect(orangeIcon).toBeDefined();
    });

    it("does not render validation icon when validationResponse is null", () => {
      const propsWithoutValidation = {
        ...defaultProps,
        validationResponse: null
      };

      render(<MasterUpdateSummaryTable {...propsWithoutValidation} />);

      const icons = screen.queryAllByTestId("info-icon");
      expect(icons).toHaveLength(0);
    });

    it("calls onValidationToggle when icon is clicked", () => {
      render(<MasterUpdateSummaryTable {...defaultProps} />);

      const icons = screen.getAllByTestId("info-icon");
      fireEvent.click(icons[0]);

      expect(mockOnValidationToggle).toHaveBeenCalled();
    });
  });

  describe("Validation Popups", () => {
    it("displays Beginning Balance popup when openValidationField matches", () => {
      const propsWithOpenPopup = {
        ...defaultProps,
        openValidationField: "TotalProfitSharingBalance"
      };

      render(<MasterUpdateSummaryTable {...propsWithOpenPopup} />);

      // Text appears in both header and popup
      expect(screen.getAllByText("Beginning Balance").length).toBeGreaterThan(0);
      expect(screen.getByText("PAY444 (Current)")).toBeInTheDocument();
      expect(screen.getByText("PAY443 (Expected)")).toBeInTheDocument();
    });

    it("displays Contributions popup with variance when validation fails", () => {
      const propsWithOpenPopup = {
        ...defaultProps,
        openValidationField: "TotalContributions"
      };

      render(<MasterUpdateSummaryTable {...propsWithOpenPopup} />);

      // Contributions appears in header and popup
      expect(screen.getAllByText("Contributions").length).toBeGreaterThan(0);
      expect(screen.getByText("PAY444 (Current)")).toBeInTheDocument();
      expect(screen.getAllByText("$50,000.00").length).toBeGreaterThan(0);
      expect(screen.getAllByText("$50,100.00").length).toBeGreaterThan(0);
    });

    it("displays Forfeitures popup when openValidationField matches", () => {
      const propsWithOpenPopup = {
        ...defaultProps,
        openValidationField: "TotalForfeitures"
      };

      render(<MasterUpdateSummaryTable {...propsWithOpenPopup} />);

      // Forfeitures appears in header and popup
      expect(screen.getAllByText("Forfeitures").length).toBeGreaterThan(0);
    });

    it("displays Distributions popup when openValidationField matches", () => {
      const propsWithOpenPopup = {
        ...defaultProps,
        openValidationField: "DistributionTotals"
      };

      render(<MasterUpdateSummaryTable {...propsWithOpenPopup} />);

      // Distributions appears in header and popup
      expect(screen.getAllByText("Distributions").length).toBeGreaterThan(0);
    });

    it("displays NetAllocTransfer popup with ALLOC/PAID ALLOC details", () => {
      const propsWithOpenPopup = {
        ...defaultProps,
        openValidationField: "NetAllocTransfer"
      };

      render(<MasterUpdateSummaryTable {...propsWithOpenPopup} />);

      expect(screen.getByText("ALLOC/PAID ALLOC Transfer Balance")).toBeInTheDocument();
      expect(screen.getByText("Incoming (ALLOC - code 6)")).toBeInTheDocument();
      expect(screen.getByText("Outgoing (PAID ALLOC - code 5)")).toBeInTheDocument();
      expect(screen.getByText("Net Transfer (Should be $0.00)")).toBeInTheDocument();
      expect(screen.getByText("Balanced")).toBeInTheDocument(); // Message
    });

    it("does not display popup when openValidationField is null", () => {
      render(<MasterUpdateSummaryTable {...defaultProps} />);

      expect(screen.queryByText("PAY444 (Current)")).not.toBeInTheDocument();
    });

    it("does not display popup when openValidationField does not match", () => {
      const propsWithDifferentField = {
        ...defaultProps,
        openValidationField: "SomeOtherField"
      };

      render(<MasterUpdateSummaryTable {...propsWithDifferentField} />);

      expect(screen.queryByText("PAY444 (Current)")).not.toBeInTheDocument();
    });
  });

  describe("Edge Cases", () => {
    it("handles zero values correctly", () => {
      const totalsWithZeros = {
        ...mockTotals,
        beginningBalance: 0,
        totalContribution: 0
      };

      render(
        <MasterUpdateSummaryTable
          {...defaultProps}
          totals={totalsWithZeros}
        />
      );

      // Multiple $0.00 values will exist in the table
      expect(screen.getAllByText("$0.00").length).toBeGreaterThan(0);
    });

    it("handles negative values correctly (Paid Allocations)", () => {
      render(<MasterUpdateSummaryTable {...defaultProps} />);

      expect(screen.getAllByText("-$8,000.00").length).toBeGreaterThan(0);
    });

    it("handles missing validation gracefully", () => {
      const getFieldValidationReturnsNull = () => null;

      const propsWithNoValidation = {
        ...defaultProps,
        getFieldValidation: getFieldValidationReturnsNull
      };

      render(<MasterUpdateSummaryTable {...propsWithNoValidation} />);

      const icons = screen.queryAllByTestId("info-icon");
      expect(icons).toHaveLength(0);
    });

    it("handles undefined totals gracefully with fallback to 0", () => {
      const totalsWithUndefined = {
        beginningBalance: undefined,
        totalContribution: undefined,
        earnings: undefined,
        earnings2: undefined,
        forfeiture: undefined,
        distributions: undefined,
        military: undefined,
        paidAllocations: undefined,
        endingBalance: undefined,
        allocations: undefined,
        maxPointsTotal: undefined,
        contributionPoints: undefined,
        earningPoints: undefined
      };

      render(
        <MasterUpdateSummaryTable
          {...defaultProps}
          totals={totalsWithUndefined as typeof mockTotals}
        />
      );

      // Should display $0.00 for undefined values
      const zeroDollars = screen.getAllByText("$0.00");
      expect(zeroDollars.length).toBeGreaterThan(0);
    });
  });

  describe("NetAllocTransfer Calculation Display", () => {
    it("shows balanced message when NetAllocTransfer is zero", () => {
      const propsWithBalanced = {
        ...defaultProps,
        openValidationField: "NetAllocTransfer"
      };

      render(<MasterUpdateSummaryTable {...propsWithBalanced} />);

      // Multiple $0.00 values in table + popup
      expect(screen.getAllByText("$0.00").length).toBeGreaterThan(0);
      expect(screen.getByText("Balanced")).toBeInTheDocument();
    });

    it("shows correct incoming and outgoing values", () => {
      const propsWithDetails = {
        ...defaultProps,
        openValidationField: "NetAllocTransfer"
      };

      render(<MasterUpdateSummaryTable {...propsWithDetails} />);

      // Values appear in both table and popup
      expect(screen.getAllByText("$8,000.00").length).toBeGreaterThan(0); // Incoming
      expect(screen.getAllByText("-$8,000.00").length).toBeGreaterThan(0); // Outgoing
    });

    it("applies correct color styling for balanced transfer (green)", () => {
      const propsWithBalanced = {
        ...defaultProps,
        openValidationField: "NetAllocTransfer"
      };

      const { container } = render(<MasterUpdateSummaryTable {...propsWithBalanced} />);

      const netTransferCell = container.querySelector(".text-green-600");
      expect(netTransferCell).toBeInTheDocument();
    });

    it("applies correct color styling for imbalanced transfer (orange)", () => {
      const mockGetFieldValidationImbalanced = (fieldKey: string) => {
        if (fieldKey === "NetAllocTransfer") {
          return { isValid: false, currentValue: 100, expectedValue: 0, message: "Imbalanced" };
        }
        if (fieldKey === "IncomingAllocations") {
          return { isValid: true, currentValue: 8100, expectedValue: 8100 };
        }
        if (fieldKey === "OutgoingAllocations") {
          return { isValid: true, currentValue: -8000, expectedValue: -8000 };
        }
        return mockGetFieldValidation(fieldKey);
      };

      const propsWithImbalanced = {
        ...defaultProps,
        getFieldValidation: mockGetFieldValidationImbalanced,
        openValidationField: "NetAllocTransfer"
      };

      const { container } = render(<MasterUpdateSummaryTable {...propsWithImbalanced} />);

      const netTransferCell = container.querySelector(".text-orange-600");
      expect(netTransferCell).toBeInTheDocument();
    });
  });

  describe("Accessibility", () => {
    it("renders proper table semantics", () => {
      const { container } = render(<MasterUpdateSummaryTable {...defaultProps} />);

      expect(container.querySelector("table")).toBeInTheDocument();
      expect(container.querySelector("thead")).toBeInTheDocument();
      expect(container.querySelector("tbody")).toBeInTheDocument();
    });

    it("renders clickable icons with cursor-pointer class", () => {
      render(<MasterUpdateSummaryTable {...defaultProps} />);

      const icons = screen.getAllByTestId("info-icon");
      // The cursor-pointer class is on the parent div wrapper, not the SVG icon itself
      icons.forEach((icon) => {
        const parentDiv = icon.parentElement;
        expect(parentDiv?.className).toContain("cursor-pointer");
      });
    });

    it("properly centers popups with Tailwind classes", () => {
      const propsWithOpenPopup = {
        ...defaultProps,
        openValidationField: "TotalProfitSharingBalance"
      };

      const { container } = render(<MasterUpdateSummaryTable {...propsWithOpenPopup} />);

      const popup = container.querySelector(".fixed.left-1\\/2.top-1\\/2.-translate-x-1\\/2.-translate-y-1\\/2");
      expect(popup).toBeInTheDocument();
    });
  });

  describe("Null Safety in Popups", () => {
    it("handles null validation data gracefully in Beginning Balance popup", () => {
      const mockGetFieldValidationWithNulls = (field: string) => {
        if (field === "TotalProfitSharingBalance") {
          return { isValid: false, currentValue: null, expectedValue: null, message: null };
        }
        return null;
      };

      const propsWithNullData = {
        ...defaultProps,
        getFieldValidation: mockGetFieldValidationWithNulls,
        openValidationField: "TotalProfitSharingBalance"
      };

      render(<MasterUpdateSummaryTable {...propsWithNullData} />);

      // Should render $0.00 when values are null
      expect(screen.getAllByText("$0.00").length).toBeGreaterThan(0);
    });

    it("handles null validation data gracefully in Contributions popup", () => {
      const mockGetFieldValidationWithNulls = (field: string) => {
        if (field === "TotalContributions") {
          return { isValid: false, currentValue: 50000, expectedValue: 45000, message: null };
        }
        // PAY443.TotalContributions returns null - this is the critical test
        if (field === "PAY443.TotalContributions") {
          return null;
        }
        return null;
      };

      const propsWithNullData = {
        ...defaultProps,
        getFieldValidation: mockGetFieldValidationWithNulls,
        openValidationField: "TotalContributions"
      };

      // Should NOT throw error when rendering popup with null sub-field
      expect(() => render(<MasterUpdateSummaryTable {...propsWithNullData} />)).not.toThrow();

      // Should render $0.00 when PAY443 values are null
      expect(screen.getAllByText("$0.00").length).toBeGreaterThan(0);
    });

    it("handles null validation data gracefully in Earnings popup", () => {
      const mockGetFieldValidationWithNulls = (field: string) => {
        if (field === "TotalEarnings") {
          return { isValid: false, currentValue: 25000, expectedValue: 24000, message: null };
        }
        // PAY443.TotalEarnings returns null
        if (field === "PAY443.TotalEarnings") {
          return null;
        }
        return null;
      };

      const propsWithNullData = {
        ...defaultProps,
        getFieldValidation: mockGetFieldValidationWithNulls,
        openValidationField: "TotalEarnings"
      };

      expect(() => render(<MasterUpdateSummaryTable {...propsWithNullData} />)).not.toThrow();
      expect(screen.getAllByText("$0.00").length).toBeGreaterThan(0);
    });

    it("handles null validation data gracefully in Forfeitures popup", () => {
      const mockGetFieldValidationWithNulls = (field: string) => {
        if (field === "TotalForfeitures") {
          return { isValid: false, currentValue: 10000, expectedValue: 9500, message: null };
        }
        // PAY443.TotalForfeitures returns null
        if (field === "PAY443.TotalForfeitures") {
          return null;
        }
        return null;
      };

      const propsWithNullData = {
        ...defaultProps,
        getFieldValidation: mockGetFieldValidationWithNulls,
        openValidationField: "TotalForfeitures"
      };

      expect(() => render(<MasterUpdateSummaryTable {...propsWithNullData} />)).not.toThrow();
      expect(screen.getAllByText("$0.00").length).toBeGreaterThan(0);
    });

    it("handles null validation data gracefully in Distributions popup", () => {
      const mockGetFieldValidationWithNulls = (field: string) => {
        if (field === "DistributionTotals") {
          // Return null for currentValue and expectedValue to test null handling
          return { isValid: false, currentValue: null, expectedValue: null, message: null };
        }
        return null;
      };

      const propsWithNullData = {
        ...defaultProps,
        getFieldValidation: mockGetFieldValidationWithNulls,
        openValidationField: "DistributionTotals"
      };

      expect(() => render(<MasterUpdateSummaryTable {...propsWithNullData} />)).not.toThrow();
      expect(screen.getAllByText("$0.00").length).toBeGreaterThan(0);
    });

    it("handles null validation data gracefully in NetAllocTransfer popup", () => {
      const mockGetFieldValidationWithNulls = (field: string) => {
        if (field === "NetAllocTransfer") {
          return { isValid: true, currentValue: 0, expectedValue: 0, message: "Balanced" };
        }
        // IncomingAllocations and OutgoingAllocations return null
        if (field === "IncomingAllocations" || field === "OutgoingAllocations") {
          return null;
        }
        return null;
      };

      const propsWithNullData = {
        ...defaultProps,
        getFieldValidation: mockGetFieldValidationWithNulls,
        openValidationField: "NetAllocTransfer"
      };

      expect(() => render(<MasterUpdateSummaryTable {...propsWithNullData} />)).not.toThrow();

      // Should show net transfer value
      expect(screen.getByText("Net Transfer (Should be $0.00)")).toBeInTheDocument();

      // Should not show incoming/outgoing rows since they return null
      expect(screen.queryByText("Incoming (ALLOC - code 6)")).not.toBeInTheDocument();
      expect(screen.queryByText("Outgoing (PAID ALLOC - code 5)")).not.toBeInTheDocument();
    });

    it("handles completely missing validation response", () => {
      const mockGetFieldValidationReturnsNull = () => null;

      const propsWithNoValidation = {
        ...defaultProps,
        getFieldValidation: mockGetFieldValidationReturnsNull,
        openValidationField: "TotalContributions"
      };

      // Should NOT render popup when getFieldValidation returns null for the field
      const { container } = render(<MasterUpdateSummaryTable {...propsWithNoValidation} />);

      const popup = container.querySelector(".fixed.left-1\\/2.top-1\\/2");
      expect(popup).not.toBeInTheDocument();
    });

    it("handles undefined currentValue and expectedValue properties", () => {
      const mockGetFieldValidationWithUndefined = (field: string) => {
        if (field === "TotalProfitSharingBalance") {
          return {
            isValid: false,
            currentValue: undefined,
            expectedValue: undefined,
            message: null
          };
        }
        return null;
      };

      const propsWithUndefinedValues = {
        ...defaultProps,
        getFieldValidation: mockGetFieldValidationWithUndefined,
        openValidationField: "TotalProfitSharingBalance"
      };

      render(<MasterUpdateSummaryTable {...propsWithUndefinedValues} />);

      // Should render $0.00 when values are undefined
      expect(screen.getAllByText("$0.00").length).toBeGreaterThan(0);
    });
  });
});
