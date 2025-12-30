import { describe, it, expect } from "vitest";
import reducer, {
  setMasterInquiryRequestParams,
  clearMasterInquiryRequestParams,
  setMasterInquiryData,
  setMasterInquiryDataSecondary,
  clearMasterInquiryData,
  clearMasterInquiryDataSecondary,
  setMasterInquiryResults,
  clearMasterInquiryResults,
  setMasterInquiryGroupingData,
  clearMasterInquiryGroupingData,
  InquiryState
} from "./inquirySlice";
import { EmployeeDetails, GroupedProfitSummaryDto, MasterInquiryDetail, MasterInquirySearch } from "reduxstore/types";

describe("inquirySlice", () => {
  const initialState: InquiryState = {
    masterInquiryData: null,
    masterInquiryMemberDetails: null,
    masterInquiryMemberDetailsSecondary: null,
    masterInquiryResults: null,
    masterInquiryRequestParams: null,
    masterInquiryGroupingData: null
  };

  const mockEmployeeDetails: EmployeeDetails = {
    id: 1,
    badgeNumber: 12345,
    psnSuffix: 0,
    payFrequencyId: 1,
    isEmployee: true,
    firstName: "John",
    lastName: "Doe",
    fullName: "John Doe",
    address: "123 Main St",
    addressCity: "Boston",
    addressState: "MA",
    addressZipCode: "02101",
    dateOfBirth: "1990-01-15",
    ssn: "123-45-6789",
    yearToDateProfitSharingHours: 2000,
    yearsInPlan: 5,
    percentageVested: 100,
    contributionsLastYear: true,
    enrollmentId: 1,
    enrollment: "Active",
    hireDate: "2019-01-15",
    terminationDate: null,
    reHireDate: null,
    storeNumber: 100,
    beginPSAmount: 10000,
    currentPSAmount: 15000,
    beginVestedAmount: 8000,
    currentVestedAmount: 12000,
    currentEtva: 1000,
    previousEtva: 800,
    department: "Sales",
    payClassification: "FT",
    gender: "M",
    phoneNumber: "555-123-4567",
    workLocation: "Store 100",
    receivedContributionsLastYear: true,
    fullTimeDate: "2019-01-15",
    terminationReason: "",
    missives: null,
    allocationFromAmount: 0,
    allocationToAmount: 0,
    badgesOfDuplicateSsns: []
  };

  const mockSecondaryEmployeeDetails: EmployeeDetails = {
    ...mockEmployeeDetails,
    id: 2,
    badgeNumber: 67890,
    firstName: "Jane",
    lastName: "Smith",
    fullName: "Jane Smith"
  };

  const mockMasterInquirySearch: MasterInquirySearch = {
    endProfitYear: 2024,
    startProfitMonth: 1,
    endProfitMonth: 12,
    paymentType: "all",
    memberType: "employees",
    voids: false,
    pagination: {
      skip: 0,
      take: 50,
      sortBy: "profitYear",
      isSortDescending: true
    }
  };

  const mockMasterInquiryDetail: MasterInquiryDetail = {
    id: 1,
    profitYear: 2024,
    isEmployee: true,
    ssn: "123-45-6789",
    profitYearIteration: 1,
    distributionSequence: 1,
    profitCodeId: 10,
    contribution: 5000,
    earnings: 500,
    forfeiture: 0,
    monthToDate: 500,
    yearToDate: 5500,
    federalTaxes: 1100,
    stateTaxes: 250,
    badgeNumber: 12345
  };

  const mockGroupedProfitSummary: GroupedProfitSummaryDto = {
    profitYear: 2024,
    monthToDate: 500,
    totalContribution: 5000,
    totalEarnings: 500,
    totalForfeiture: 0,
    totalPayment: 5500,
    transactionCount: 12
  };

  describe("reducer", () => {
    it("should return initial state when called with undefined state", () => {
      expect(reducer(undefined, { type: "unknown" })).toEqual(initialState);
    });

    it("should return current state for unknown action", () => {
      const currentState: InquiryState = {
        ...initialState,
        masterInquiryData: mockEmployeeDetails
      };
      expect(reducer(currentState, { type: "unknown" })).toEqual(currentState);
    });
  });

  describe("setMasterInquiryRequestParams", () => {
    it("should set master inquiry request params", () => {
      const nextState = reducer(initialState, setMasterInquiryRequestParams(mockMasterInquirySearch));

      expect(nextState.masterInquiryRequestParams).toEqual(mockMasterInquirySearch);
    });

    it("should replace existing request params", () => {
      const currentState: InquiryState = {
        ...initialState,
        masterInquiryRequestParams: mockMasterInquirySearch
      };

      const newParams: MasterInquirySearch = {
        ...mockMasterInquirySearch,
        endProfitYear: 2025,
        memberType: "beneficiaries"
      };

      const nextState = reducer(currentState, setMasterInquiryRequestParams(newParams));

      expect(nextState.masterInquiryRequestParams?.endProfitYear).toBe(2025);
      expect(nextState.masterInquiryRequestParams?.memberType).toBe("beneficiaries");
    });

    it("should handle different payment types", () => {
      const paymentTypes: Array<"all" | "hardship" | "payoffs" | "rollovers"> = [
        "all",
        "hardship",
        "payoffs",
        "rollovers"
      ];

      paymentTypes.forEach((paymentType) => {
        const params: MasterInquirySearch = { ...mockMasterInquirySearch, paymentType };
        const state = reducer(initialState, setMasterInquiryRequestParams(params));
        expect(state.masterInquiryRequestParams?.paymentType).toBe(paymentType);
      });
    });
  });

  describe("clearMasterInquiryRequestParams", () => {
    it("should clear request params", () => {
      const currentState: InquiryState = {
        ...initialState,
        masterInquiryRequestParams: mockMasterInquirySearch
      };

      const nextState = reducer(currentState, clearMasterInquiryRequestParams());

      expect(nextState.masterInquiryRequestParams).toBeNull();
    });

    it("should handle clearing already null params", () => {
      const nextState = reducer(initialState, clearMasterInquiryRequestParams());

      expect(nextState.masterInquiryRequestParams).toBeNull();
    });
  });

  describe("setMasterInquiryData", () => {
    it("should set master inquiry data and member details", () => {
      const nextState = reducer(initialState, setMasterInquiryData(mockEmployeeDetails));

      expect(nextState.masterInquiryData).toEqual(mockEmployeeDetails);
      expect(nextState.masterInquiryMemberDetails).toEqual(mockEmployeeDetails);
    });

    it("should replace existing data", () => {
      const currentState: InquiryState = {
        ...initialState,
        masterInquiryData: mockEmployeeDetails,
        masterInquiryMemberDetails: mockEmployeeDetails
      };

      const newDetails: EmployeeDetails = {
        ...mockEmployeeDetails,
        id: 999,
        firstName: "Updated"
      };

      const nextState = reducer(currentState, setMasterInquiryData(newDetails));

      expect(nextState.masterInquiryData?.id).toBe(999);
      expect(nextState.masterInquiryMemberDetails?.firstName).toBe("Updated");
    });
  });

  describe("setMasterInquiryDataSecondary", () => {
    it("should set secondary member details", () => {
      const nextState = reducer(initialState, setMasterInquiryDataSecondary(mockSecondaryEmployeeDetails));

      expect(nextState.masterInquiryData).toEqual(mockSecondaryEmployeeDetails);
      expect(nextState.masterInquiryMemberDetailsSecondary).toEqual(mockSecondaryEmployeeDetails);
    });

    it("should not affect primary member details", () => {
      const currentState: InquiryState = {
        ...initialState,
        masterInquiryMemberDetails: mockEmployeeDetails
      };

      const nextState = reducer(currentState, setMasterInquiryDataSecondary(mockSecondaryEmployeeDetails));

      expect(nextState.masterInquiryMemberDetails).toEqual(mockEmployeeDetails);
      expect(nextState.masterInquiryMemberDetailsSecondary).toEqual(mockSecondaryEmployeeDetails);
    });
  });

  describe("clearMasterInquiryData", () => {
    it("should clear inquiry data and primary member details", () => {
      const currentState: InquiryState = {
        ...initialState,
        masterInquiryData: mockEmployeeDetails,
        masterInquiryMemberDetails: mockEmployeeDetails
      };

      const nextState = reducer(currentState, clearMasterInquiryData());

      expect(nextState.masterInquiryData).toBeNull();
      expect(nextState.masterInquiryMemberDetails).toBeNull();
    });

    it("should not affect secondary member details", () => {
      const currentState: InquiryState = {
        ...initialState,
        masterInquiryData: mockEmployeeDetails,
        masterInquiryMemberDetails: mockEmployeeDetails,
        masterInquiryMemberDetailsSecondary: mockSecondaryEmployeeDetails
      };

      const nextState = reducer(currentState, clearMasterInquiryData());

      expect(nextState.masterInquiryMemberDetailsSecondary).toEqual(mockSecondaryEmployeeDetails);
    });
  });

  describe("clearMasterInquiryDataSecondary", () => {
    it("should clear secondary member details only", () => {
      const currentState: InquiryState = {
        ...initialState,
        masterInquiryMemberDetails: mockEmployeeDetails,
        masterInquiryMemberDetailsSecondary: mockSecondaryEmployeeDetails
      };

      const nextState = reducer(currentState, clearMasterInquiryDataSecondary());

      expect(nextState.masterInquiryMemberDetails).toEqual(mockEmployeeDetails);
      expect(nextState.masterInquiryMemberDetailsSecondary).toBeNull();
    });
  });

  describe("setMasterInquiryResults", () => {
    it("should set inquiry results", () => {
      const results: MasterInquiryDetail[] = [mockMasterInquiryDetail];

      const nextState = reducer(initialState, setMasterInquiryResults(results));

      expect(nextState.masterInquiryResults).toEqual(results);
      expect(nextState.masterInquiryResults).toHaveLength(1);
    });

    it("should handle empty results", () => {
      const nextState = reducer(initialState, setMasterInquiryResults([]));

      expect(nextState.masterInquiryResults).toEqual([]);
      expect(nextState.masterInquiryResults).toHaveLength(0);
    });

    it("should handle multiple results", () => {
      const results: MasterInquiryDetail[] = Array.from({ length: 10 }, (_, i) => ({
        ...mockMasterInquiryDetail,
        id: i + 1,
        contribution: 1000 * (i + 1)
      }));

      const nextState = reducer(initialState, setMasterInquiryResults(results));

      expect(nextState.masterInquiryResults).toHaveLength(10);
      expect(nextState.masterInquiryResults?.[0].contribution).toBe(1000);
      expect(nextState.masterInquiryResults?.[9].contribution).toBe(10000);
    });
  });

  describe("clearMasterInquiryResults", () => {
    it("should clear inquiry results", () => {
      const currentState: InquiryState = {
        ...initialState,
        masterInquiryResults: [mockMasterInquiryDetail]
      };

      const nextState = reducer(currentState, clearMasterInquiryResults());

      expect(nextState.masterInquiryResults).toBeNull();
    });
  });

  describe("setMasterInquiryGroupingData", () => {
    it("should set grouping data", () => {
      const groupingData: GroupedProfitSummaryDto[] = [mockGroupedProfitSummary];

      const nextState = reducer(initialState, setMasterInquiryGroupingData(groupingData));

      expect(nextState.masterInquiryGroupingData).toEqual(groupingData);
    });

    it("should handle multiple years of grouping data", () => {
      const groupingData: GroupedProfitSummaryDto[] = [
        { ...mockGroupedProfitSummary, profitYear: 2024 },
        { ...mockGroupedProfitSummary, profitYear: 2023 },
        { ...mockGroupedProfitSummary, profitYear: 2022 }
      ];

      const nextState = reducer(initialState, setMasterInquiryGroupingData(groupingData));

      expect(nextState.masterInquiryGroupingData).toHaveLength(3);
      expect(nextState.masterInquiryGroupingData?.[0].profitYear).toBe(2024);
    });
  });

  describe("clearMasterInquiryGroupingData", () => {
    it("should clear grouping data", () => {
      const currentState: InquiryState = {
        ...initialState,
        masterInquiryGroupingData: [mockGroupedProfitSummary]
      };

      const nextState = reducer(currentState, clearMasterInquiryGroupingData());

      expect(nextState.masterInquiryGroupingData).toBeNull();
    });
  });

  describe("complex state transitions", () => {
    it("should handle complete inquiry workflow", () => {
      // Set search params
      let state = reducer(initialState, setMasterInquiryRequestParams(mockMasterInquirySearch));
      expect(state.masterInquiryRequestParams).toEqual(mockMasterInquirySearch);

      // Set primary member details
      state = reducer(state, setMasterInquiryData(mockEmployeeDetails));
      expect(state.masterInquiryMemberDetails).toEqual(mockEmployeeDetails);

      // Set secondary member details
      state = reducer(state, setMasterInquiryDataSecondary(mockSecondaryEmployeeDetails));
      expect(state.masterInquiryMemberDetailsSecondary).toEqual(mockSecondaryEmployeeDetails);

      // Set results
      state = reducer(state, setMasterInquiryResults([mockMasterInquiryDetail]));
      expect(state.masterInquiryResults).toHaveLength(1);

      // Set grouping data
      state = reducer(state, setMasterInquiryGroupingData([mockGroupedProfitSummary]));
      expect(state.masterInquiryGroupingData).toHaveLength(1);
    });

    it("should handle clearing all data", () => {
      const populatedState: InquiryState = {
        masterInquiryData: mockEmployeeDetails,
        masterInquiryMemberDetails: mockEmployeeDetails,
        masterInquiryMemberDetailsSecondary: mockSecondaryEmployeeDetails,
        masterInquiryResults: [mockMasterInquiryDetail],
        masterInquiryRequestParams: mockMasterInquirySearch,
        masterInquiryGroupingData: [mockGroupedProfitSummary]
      };

      let state = reducer(populatedState, clearMasterInquiryRequestParams());
      state = reducer(state, clearMasterInquiryData());
      state = reducer(state, clearMasterInquiryDataSecondary());
      state = reducer(state, clearMasterInquiryResults());
      state = reducer(state, clearMasterInquiryGroupingData());

      expect(state.masterInquiryRequestParams).toBeNull();
      expect(state.masterInquiryData).toBeNull();
      expect(state.masterInquiryMemberDetails).toBeNull();
      expect(state.masterInquiryMemberDetailsSecondary).toBeNull();
      expect(state.masterInquiryResults).toBeNull();
      expect(state.masterInquiryGroupingData).toBeNull();
    });
  });

  describe("edge cases", () => {
    it("should preserve state immutability", () => {
      const currentState: InquiryState = { ...initialState };
      const nextState = reducer(currentState, setMasterInquiryData(mockEmployeeDetails));

      expect(currentState.masterInquiryData).toBeNull();
      expect(nextState.masterInquiryData).toEqual(mockEmployeeDetails);
    });

    it("should handle employee with optional fields", () => {
      const employeeWithAge: EmployeeDetails = {
        ...mockEmployeeDetails,
        age: "34"
      };

      const nextState = reducer(initialState, setMasterInquiryData(employeeWithAge));

      expect(nextState.masterInquiryData?.age).toBe("34");
    });

    it("should handle inquiry detail with optional remark", () => {
      const detailWithRemark: MasterInquiryDetail = {
        ...mockMasterInquiryDetail,
        remark: "Special note"
      };

      const nextState = reducer(initialState, setMasterInquiryResults([detailWithRemark]));

      expect(nextState.masterInquiryResults?.[0].remark).toBe("Special note");
    });
  });
});
