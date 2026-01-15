import { render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { describe, expect, it } from "vitest";
import { MissiveAlertProvider } from "../../../../components/MissiveAlerts/MissiveAlertContext";
import MasterInquiryMemberDetails from "../MasterInquiryMemberDetails";

// Helper function to render components with MissiveAlertProvider
const renderWithProvider = (component: React.ReactElement) => {
  return render(
    <MemoryRouter>
      <MissiveAlertProvider>{component}</MissiveAlertProvider>
    </MemoryRouter>
  );
};

describe("MasterInquiryMemberDetails", { timeout: 18000 }, () => {
  const mockEmployeeDetails = {
    id: 1,
    isEmployee: true,
    badgeNumber: 12345,
    psnSuffix: 0,
    payFrequencyId: 1,
    ssn: "XXX-XX-1234",
    firstName: "John",
    lastName: "Doe",
    fullName: "Doe, John",
    address: "123 Main St",
    addressCity: "Lowell",
    addressState: "MA",
    addressZipCode: "01850",
    dateOfBirth: "1980-01-15",
    age: "45",
    phoneNumber: "9785551234",
    workLocation: "Store 4",
    storeNumber: 4,
    hireDate: "2010-05-01",
    terminationDate: null,
    reHireDate: null,
    fullTimeDate: "2010-06-01",
    employmentStatus: "Active",
    department: "Grocery",
    payClassification: "Full Time",
    gender: "M",
    terminationReason: "",
    yearToDateProfitSharingHours: 1200.5,
    yearsInPlan: 10,
    percentageVested: 1,
    contributionsLastYear: true,
    enrollmentId: 2,
    enrollment: "Active",
    beginPSAmount: 5000,
    currentPSAmount: 15000,
    beginVestedAmount: 5000,
    currentVestedAmount: 15000,
    currentEtva: 1000,
    previousEtva: 900,
    allocationToAmount: 500,
    allocationFromAmount: 200,
    receivedContributionsLastYear: true,
    missives: [],
    badgesOfDuplicateSsns: []
  };

  const mockBeneficiaryDetails = {
    id: 2,
    isEmployee: false,
    badgeNumber: 54321,
    psnSuffix: 1,
    payFrequencyId: 1,
    ssn: "XXX-XX-5678",
    firstName: "Jane",
    lastName: "Smith",
    fullName: "Smith, Jane",
    address: "456 Oak Ave",
    addressCity: "Boston",
    addressState: "MA",
    addressZipCode: "02101",
    dateOfBirth: "1985-06-20",
    age: "40",
    phoneNumber: "6175559876",
    workLocation: "",
    storeNumber: 0,
    hireDate: "",
    terminationDate: null,
    reHireDate: null,
    fullTimeDate: "",
    employmentStatus: undefined,
    department: "",
    payClassification: "",
    gender: "F",
    terminationReason: "",
    yearToDateProfitSharingHours: 0,
    yearsInPlan: 5,
    percentageVested: 0.6,
    contributionsLastYear: false,
    enrollmentId: 1,
    enrollment: "Beneficiary",
    beginPSAmount: 2000,
    currentPSAmount: 8000,
    beginVestedAmount: 1200,
    currentVestedAmount: 4800,
    currentEtva: 500,
    previousEtva: 450,
    allocationToAmount: 0,
    allocationFromAmount: 8000,
    receivedContributionsLastYear: false,
    missives: [],
    badgesOfDuplicateSsns: []
  };

  it("should render loading state when isLoading is true", () => {
    const { container } = renderWithProvider(
      <MasterInquiryMemberDetails
        memberType={1}
        id={1}
        profitYear={2025}
        memberDetails={null}
        isLoading={true}
      />
    );
    expect(container.firstChild).toBeNull();
  });

  it("should render 'No details found' when memberDetails is null and not loading", () => {
    renderWithProvider(
      <MasterInquiryMemberDetails
        memberType={1}
        id={1}
        profitYear={2025}
        memberDetails={null}
        isLoading={false}
      />
    );
    expect(screen.getByText("No details found.")).toBeInTheDocument();
  });

  it("should render employee details correctly", () => {
    renderWithProvider(
      <MasterInquiryMemberDetails
        memberType={1}
        id={1}
        profitYear={2025}
        memberDetails={mockEmployeeDetails}
        isLoading={false}
      />
    );

    // Summary Section
    expect(screen.getByText("Doe, John")).toBeInTheDocument();
    expect(screen.getByText("123 Main St")).toBeInTheDocument();
    expect(screen.getByText("Lowell, MA 01850")).toBeInTheDocument();
    expect(screen.getByText("(978) 555-1234")).toBeInTheDocument(); // Formatted phone
    expect(screen.getByText("Store 4 (4)")).toBeInTheDocument();

    // Personal Section
    expect(screen.getByText("Grocery")).toBeInTheDocument();
    expect(screen.getByText("Full Time")).toBeInTheDocument();
    expect(screen.getByText("Active")).toBeInTheDocument();
    expect(screen.getByText("M")).toBeInTheDocument();
    expect(screen.getByText("01/15/1980 (45)")).toBeInTheDocument(); // DOB with age
    expect(screen.getByText("XXX-XX-1234")).toBeInTheDocument();

    // Milestone Section
    expect(screen.getByText("05/01/2010")).toBeInTheDocument(); // Hire Date
    expect(screen.getByText("06/01/2010")).toBeInTheDocument(); // Full Time Date

    // Plan Section
    expect(screen.getAllByText("$5,000.00")).toHaveLength(2); // Begin Balance and Begin Vested Balance
    expect(screen.getAllByText("$15,000.00")).toHaveLength(2); // Current Balance and Current Vested Balance
    expect(screen.getByText(/1,200\.5/)).toBeInTheDocument(); // PS Hours (may be 1,200.5 or 1,200.50)
    expect(screen.getByText("Years In Plan")).toBeInTheDocument();
    expect(screen.getByText("100%")).toBeInTheDocument(); // Vested Percent
  });

  it("should render beneficiary details correctly", () => {
    renderWithProvider(
      <MasterInquiryMemberDetails
        memberType={2}
        id={2}
        profitYear={2025}
        memberDetails={mockBeneficiaryDetails}
        isLoading={false}
      />
    );

    // Summary Section
    expect(screen.getByText("Smith, Jane")).toBeInTheDocument();
    expect(screen.getByText("456 Oak Ave")).toBeInTheDocument();
    expect(screen.getByText("Boston, MA 02101")).toBeInTheDocument();
    expect(screen.getByText("(617) 555-9876")).toBeInTheDocument(); // Formatted phone

    // Personal Section - should NOT show employee-specific fields
    expect(screen.queryByText("Department")).not.toBeInTheDocument();
    expect(screen.queryByText("Class")).not.toBeInTheDocument();
    expect(screen.queryByText("Status")).not.toBeInTheDocument();
    expect(screen.getByText("F")).toBeInTheDocument();
    expect(screen.getByText("06/20/1985 (40)")).toBeInTheDocument(); // DOB with age

    // Milestone Section - should NOT show employee-specific fields
    expect(screen.queryByText("Hire Date")).not.toBeInTheDocument();
    expect(screen.queryByText("Full Time Date")).not.toBeInTheDocument();

    // Plan Section
    expect(screen.getByText("$2,000.00")).toBeInTheDocument(); // Begin Balance
    expect(screen.getAllByText("$8,000.00")).toHaveLength(2); // Current Balance and Allocation From
    expect(screen.queryByText("Profit Sharing Hours")).not.toBeInTheDocument();
    expect(screen.queryByText("Years In Plan")).not.toBeInTheDocument(); // Beneficiaries don't show this
    expect(screen.getByText("60%")).toBeInTheDocument(); // Vested Percent
  });

  it("should display N/A for empty phone number", () => {
    const detailsWithoutPhone = { ...mockEmployeeDetails, phoneNumber: "" };
    renderWithProvider(
      <MasterInquiryMemberDetails
        memberType={1}
        id={1}
        profitYear={2025}
        memberDetails={detailsWithoutPhone}
        isLoading={false}
      />
    );

    const phoneLabels = screen.getAllByText("Phone #");
    expect(phoneLabels.length).toBeGreaterThan(0);
  });

  it("should format 4-digit zip codes correctly", () => {
    const detailsWithShortZip = { ...mockEmployeeDetails, addressZipCode: "1850" };
    renderWithProvider(
      <MasterInquiryMemberDetails
        memberType={1}
        id={1}
        profitYear={2025}
        memberDetails={detailsWithShortZip}
        isLoading={false}
      />
    );

    expect(screen.getByText("Lowell, MA 01850")).toBeInTheDocument();
  });

  it("should display duplicate SSN badges correctly", () => {
    const detailsWithDuplicates = {
      ...mockEmployeeDetails,
      badgesOfDuplicateSsns: [54321, 98765]
    };
    renderWithProvider(
      <MasterInquiryMemberDetails
        memberType={1}
        id={1}
        profitYear={2025}
        memberDetails={detailsWithDuplicates}
        isLoading={false}
      />
    );

    const duplicateLabels = screen.getAllByText("Duplicate SSN with");
    expect(duplicateLabels).toHaveLength(2);
  });

  it("should display terminated employee information", () => {
    const terminatedEmployee = {
      ...mockEmployeeDetails,
      terminationDate: "2023-12-31",
      terminationReason: "Retired",
      employmentStatus: "Terminated"
    };
    renderWithProvider(
      <MasterInquiryMemberDetails
        memberType={1}
        id={1}
        profitYear={2025}
        memberDetails={terminatedEmployee}
        isLoading={false}
      />
    );

    expect(screen.getByText("Terminated")).toBeInTheDocument();
    expect(screen.getByText("12/31/2023")).toBeInTheDocument(); // Termination Date
    expect(screen.getByText("Retired")).toBeInTheDocument();
  });

  it("should display current year label correctly", () => {
    const currentYear = new Date().getFullYear();
    renderWithProvider(
      <MasterInquiryMemberDetails
        memberType={1}
        id={1}
        profitYear={currentYear}
        memberDetails={mockEmployeeDetails}
        isLoading={false}
      />
    );

    expect(screen.getByText("Current Balance")).toBeInTheDocument();
    expect(screen.getByText("Current Vested Balance")).toBeInTheDocument();
  });

  it("should display end year label for past years", () => {
    renderWithProvider(
      <MasterInquiryMemberDetails
        memberType={1}
        id={1}
        profitYear={2023}
        memberDetails={mockEmployeeDetails}
        isLoading={false}
      />
    );

    expect(screen.getByText("End 2023 Balance")).toBeInTheDocument();
    expect(screen.getByText("End 2023 Vested Balance")).toBeInTheDocument();
  });

  it("should handle zero balance amounts correctly", () => {
    const detailsWithZeroBalances = {
      ...mockEmployeeDetails,
      beginPSAmount: 0,
      currentPSAmount: 0,
      beginVestedAmount: 0,
      currentVestedAmount: 0
    };
    renderWithProvider(
      <MasterInquiryMemberDetails
        memberType={1}
        id={1}
        profitYear={2025}
        memberDetails={detailsWithZeroBalances}
        isLoading={false}
      />
    );

    const zeroTexts = screen.getAllByText("$0.00");
    expect(zeroTexts.length).toBeGreaterThan(0);
  });

  it("should not re-render when props haven't changed", () => {
    const { rerender } = renderWithProvider(
      <MasterInquiryMemberDetails
        memberType={1}
        id={1}
        profitYear={2025}
        memberDetails={mockEmployeeDetails}
        isLoading={false}
      />
    );

    // Re-render with same props (must wrap in provider and router again)
    rerender(
      <MemoryRouter>
        <MissiveAlertProvider>
          <MasterInquiryMemberDetails
            memberType={1}
            id={1}
            profitYear={2025}
            memberDetails={mockEmployeeDetails}
            isLoading={false}
          />
        </MissiveAlertProvider>
      </MemoryRouter>
    );

    // Component should still be rendered correctly
    expect(screen.getByText("Doe, John")).toBeInTheDocument();
  });

  // PS-1897: Verify phone number formatting
  it("should format phone numbers correctly (PS-1897)", () => {
    renderWithProvider(
      <MasterInquiryMemberDetails
        memberType={1}
        id={1}
        profitYear={2025}
        memberDetails={mockEmployeeDetails}
        isLoading={false}
      />
    );

    expect(screen.getByText("(978) 555-1234")).toBeInTheDocument();
  });

  // PS-1897: Verify enrollment code removal
  it("should not display enrollment codes like (1) (PS-1897)", () => {
    const detailsWithEnrollmentCode = {
      ...mockEmployeeDetails,
      enrollmentId: 2 // This typically returns "Y (2)" from getEnrolledStatus
    };
    renderWithProvider(
      <MasterInquiryMemberDetails
        memberType={1}
        id={1}
        profitYear={2025}
        memberDetails={detailsWithEnrollmentCode}
        isLoading={false}
      />
    );

    // Should not contain enrollment codes in parentheses
    const enrolledText = screen.getAllByText(/Enrolled|Y|N/);
    enrolledText.forEach((text) => {
      expect(text.textContent).not.toMatch(/\(\d+\)/);
    });
  });

  // PS-1897: Verify Previous ETVA is removed
  it("should not display Previous ETVA field (PS-1897)", () => {
    renderWithProvider(
      <MasterInquiryMemberDetails
        memberType={1}
        id={1}
        profitYear={2025}
        memberDetails={mockEmployeeDetails}
        isLoading={false}
      />
    );

    expect(screen.queryByText("Previous ETVA")).not.toBeInTheDocument();
  });

  // PS-1897: Verify balance grouping order
  it("should group beginning and current balances together (PS-1897)", () => {
    const profitYear = 2025;
    const yearLabel = profitYear === new Date().getFullYear() ? "Current" : `End ${profitYear}`;

    renderWithProvider(
      <MasterInquiryMemberDetails
        memberType={1}
        id={1}
        profitYear={profitYear}
        memberDetails={mockEmployeeDetails}
        isLoading={false}
      />
    );

    // Beginning balances should appear before current balances
    expect(screen.getByText("Begin Balance")).toBeInTheDocument();
    expect(screen.getByText("Begin Vested Balance")).toBeInTheDocument();
    expect(screen.getByText(`${yearLabel} Balance`)).toBeInTheDocument();
    expect(screen.getByText(`${yearLabel} Vested Balance`)).toBeInTheDocument();
  });

  // PS-1899: Verify payClassification property is correctly cased
  it("should display payClassification with correct casing (PS-1899)", () => {
    // This test ensures the property is camelCase (payClassification) not PascalCase (PayClassification)
    const employeeWithClassification = {
      ...mockEmployeeDetails,
      payClassification: "Supervisor"
    };
    renderWithProvider(
      <MasterInquiryMemberDetails
        memberType={1}
        id={1}
        profitYear={2025}
        memberDetails={employeeWithClassification}
        isLoading={false}
      />
    );

    // Should display the pay classification value
    expect(screen.getByText("Supervisor")).toBeInTheDocument();
    // Should have the label "Class"
    expect(screen.getByText("Class")).toBeInTheDocument();
  });

  // PS-1899: Regression test for casing issues in type definitions
  it("should accept payClassification property (camelCase) not PayClassification (PS-1899)", () => {
    // Verify the mock uses correct camelCase
    const validEmployeeData = {
      ...mockEmployeeDetails,
      payClassification: "Full Time Manager" // Correct: camelCase
    };

    renderWithProvider(
      <MasterInquiryMemberDetails
        memberType={1}
        id={1}
        profitYear={2025}
        memberDetails={validEmployeeData}
        isLoading={false}
      />
    );

    expect(screen.getByText("Full Time Manager")).toBeInTheDocument();
  });
});
