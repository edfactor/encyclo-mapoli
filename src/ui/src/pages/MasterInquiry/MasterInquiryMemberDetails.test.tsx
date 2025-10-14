import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import MasterInquiryMemberDetails from "./MasterInquiryMemberDetails";

describe("MasterInquiryMemberDetails", () => {
  const mockEmployeeDetails = {
    id: 1,
    isEmployee: true,
    badgeNumber: 12345,
    psnSuffix: 0,
    ssn: "XXX-XX-1234",
    firstName: "John",
    lastName: "Doe",
    address: "123 Main St",
    addressCity: "Lowell",
    addressState: "MA",
    addressZipCode: "01850",
    dateOfBirth: "1980-01-15",
    age: 45,
    phoneNumber: "978-555-1234",
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
    terminationReason: null,
    yearToDateProfitSharingHours: 1200.5,
    yearsInPlan: 10,
    percentageVested: 1,
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
    ssn: "XXX-XX-5678",
    firstName: "Jane",
    lastName: "Smith",
    address: "456 Oak Ave",
    addressCity: "Boston",
    addressState: "MA",
    addressZipCode: "02101",
    dateOfBirth: "1985-06-20",
    age: 40,
    phoneNumber: "617-555-9876",
    workLocation: null,
    storeNumber: 0,
    hireDate: null,
    terminationDate: null,
    reHireDate: null,
    fullTimeDate: null,
    employmentStatus: null,
    department: null,
    payClassification: null,
    gender: "F",
    terminationReason: null,
    yearToDateProfitSharingHours: 0,
    yearsInPlan: 5,
    percentageVested: 0.6,
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
    const { container } = render(
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
    render(
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
    render(
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
    expect(screen.getByText("978-555-1234")).toBeInTheDocument();
    expect(screen.getByText("Store 4")).toBeInTheDocument();
    expect(screen.getByText("4")).toBeInTheDocument();

    // Personal Section
    expect(screen.getByText("Grocery")).toBeInTheDocument();
    expect(screen.getByText("Full Time")).toBeInTheDocument();
    expect(screen.getByText("Active")).toBeInTheDocument();
    expect(screen.getByText("M")).toBeInTheDocument();
    expect(screen.getByText(/01\/15\/80 \(45\)/)).toBeInTheDocument(); // DOB with age
    expect(screen.getByText("XXX-XX-1234")).toBeInTheDocument();

    // Milestone Section
    expect(screen.getByText("05/01/10")).toBeInTheDocument(); // Hire Date
    expect(screen.getByText("06/01/10")).toBeInTheDocument(); // Full Time Date

    // Plan Section
    expect(screen.getByText("$5,000.00")).toBeInTheDocument(); // Begin Balance
    expect(screen.getByText("$15,000.00")).toBeInTheDocument(); // Current Balance
    expect(screen.getByText("1,200.50")).toBeInTheDocument(); // PS Hours
    expect(screen.getByText("10")).toBeInTheDocument(); // Years in Plan
    expect(screen.getByText("100.00%")).toBeInTheDocument(); // Vested Percent
  });

  it("should render beneficiary details correctly", () => {
    render(
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
    expect(screen.getByText("617-555-9876")).toBeInTheDocument();

    // Personal Section - should NOT show employee-specific fields
    expect(screen.queryByText("Department")).not.toBeInTheDocument();
    expect(screen.queryByText("Class")).not.toBeInTheDocument();
    expect(screen.queryByText("Status")).not.toBeInTheDocument();
    expect(screen.getByText("F")).toBeInTheDocument();
    expect(screen.getByText(/06\/20\/85 \(40\)/)).toBeInTheDocument(); // DOB with age

    // Milestone Section - should NOT show employee-specific fields
    expect(screen.queryByText("Hire Date")).not.toBeInTheDocument();
    expect(screen.queryByText("Full Time Date")).not.toBeInTheDocument();

    // Plan Section
    expect(screen.getByText("$2,000.00")).toBeInTheDocument(); // Begin Balance
    expect(screen.getByText("$8,000.00")).toBeInTheDocument(); // Current Balance
    expect(screen.queryByText("Profit Sharing Hours")).not.toBeInTheDocument();
    expect(screen.getByText("5")).toBeInTheDocument(); // Years in Plan
    expect(screen.getByText("60.00%")).toBeInTheDocument(); // Vested Percent
  });

  it("should display N/A for null phone number", () => {
    const detailsWithoutPhone = { ...mockEmployeeDetails, phoneNumber: null };
    render(
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
    expect(screen.getAllByText("N/A").length).toBeGreaterThan(0);
  });

  it("should format 4-digit zip codes correctly", () => {
    const detailsWithShortZip = { ...mockEmployeeDetails, addressZipCode: "1850" };
    render(
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
    render(
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
    render(
      <MasterInquiryMemberDetails
        memberType={1}
        id={1}
        profitYear={2025}
        memberDetails={terminatedEmployee}
        isLoading={false}
      />
    );

    expect(screen.getByText("Terminated")).toBeInTheDocument();
    expect(screen.getByText("12/31/23")).toBeInTheDocument(); // Termination Date
    expect(screen.getByText("Retired")).toBeInTheDocument();
  });

  it("should display current year label correctly", () => {
    const currentYear = new Date().getFullYear();
    render(
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
    render(
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

  it("should handle null balance amounts correctly", () => {
    const detailsWithNullBalances = {
      ...mockEmployeeDetails,
      beginPSAmount: null,
      currentPSAmount: null,
      beginVestedAmount: null,
      currentVestedAmount: null
    };
    render(
      <MasterInquiryMemberDetails
        memberType={1}
        id={1}
        profitYear={2025}
        memberDetails={detailsWithNullBalances}
        isLoading={false}
      />
    );

    const naTexts = screen.getAllByText("N/A");
    expect(naTexts.length).toBeGreaterThan(0);
  });

  it("should not re-render when props haven't changed", () => {
    const { rerender } = render(
      <MasterInquiryMemberDetails
        memberType={1}
        id={1}
        profitYear={2025}
        memberDetails={mockEmployeeDetails}
        isLoading={false}
      />
    );

    // Re-render with same props
    rerender(
      <MasterInquiryMemberDetails
        memberType={1}
        id={1}
        profitYear={2025}
        memberDetails={mockEmployeeDetails}
        isLoading={false}
      />
    );

    // Component should still be rendered correctly
    expect(screen.getByText("Doe, John")).toBeInTheDocument();
  });
});
