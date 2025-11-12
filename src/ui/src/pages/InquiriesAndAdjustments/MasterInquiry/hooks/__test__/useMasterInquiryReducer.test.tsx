import { describe, expect, it } from "vitest";
import { type EmployeeDetails } from "reduxstore/types";
import {
  initialState,
  masterInquiryReducer,
  selectShowMemberDetails,
  selectShowMemberGrid,
  selectShowProfitDetails,
  type MasterInquiryState,
  type SelectedMember
} from "../useMasterInquiryReducer";

describe("useMasterInquiryReducer", () => {
  describe("initialState", () => {
    it("should have correct initial state", () => {
      expect(initialState.search.params).toBeNull();
      expect(initialState.search.results).toBeNull();
      expect(initialState.search.isSearching).toBe(false);
      expect(initialState.search.isManuallySearching).toBe(false);
      expect(initialState.selection.selectedMember).toBeNull();
      expect(initialState.view.mode).toBe("idle");
    });
  });

  describe("SEARCH_START", () => {
    it("should set search state to searching", () => {
      const params = {
        pagination: { skip: 0, take: 25, sortBy: "name", isSortDescending: false }
      };
      const action = { type: "SEARCH_START" as const, payload: { params, isManual: true } };
      const newState = masterInquiryReducer(initialState, action);

      expect(newState.search.params).toEqual(params);
      expect(newState.search.isSearching).toBe(true);
      expect(newState.search.isManuallySearching).toBe(true);
      expect(newState.view.mode).toBe("searching");
      expect(newState.selection.selectedMember).toBeNull();
    });

    it("should clear previous results and errors", () => {
      const stateWithData: MasterInquiryState = {
        ...initialState,
        search: {
          ...initialState.search,
          results: { results: [], total: 0 },
          error: "Previous error"
        }
      };

      const params = {
        badgeNumber: 12345, // Add actual search parameter to trigger isParametersChanged
        pagination: { skip: 0, take: 25, sortBy: "name", isSortDescending: false }
      };
      const action = { type: "SEARCH_START" as const, payload: { params, isManual: true } }; // Changed to true for manual search
      const newState = masterInquiryReducer(stateWithData, action);

      expect(newState.search.results).toBeNull();
      expect(newState.search.error).toBeNull();
      expect(newState.search.isManuallySearching).toBe(true); // Changed to match isManual: true
    });
  });

  describe("SEARCH_SUCCESS", () => {
    it("should handle single result and auto-select member", () => {
      const results = {
        results: [
          {
            id: "1",
            ssn: "123456789",
            badgeNumber: "12345",
            psnSuffix: "0",
            isEmployee: true
          }
        ] as unknown as EmployeeDetails[],
        total: 1
      };

      const action = { type: "SEARCH_SUCCESS" as const, payload: { results } };
      const newState = masterInquiryReducer(initialState, action);

      expect(newState.search.results).toEqual(results);
      expect(newState.search.isSearching).toBe(false);
      expect(newState.view.mode).toBe("memberDetails");
      expect(newState.selection.selectedMember).toEqual({
        memberType: 1,
        id: 1,
        ssn: 123456789,
        badgeNumber: 12345,
        psnSuffix: 0
      });
    });

    it("should handle multiple results and show grid", () => {
      const results = {
        results: [
          { id: "1", ssn: "111", badgeNumber: "11111", psnSuffix: "0", isEmployee: true },
          { id: "2", ssn: "222", badgeNumber: "22222", psnSuffix: "0", isEmployee: false }
        ] as unknown as EmployeeDetails[],
        total: 2
      };

      const action = { type: "SEARCH_SUCCESS" as const, payload: { results } };
      const newState = masterInquiryReducer(initialState, action);

      expect(newState.search.results).toEqual(results);
      expect(newState.view.mode).toBe("multipleMembers");
      expect(newState.selection.selectedMember).toBeNull();
    });

    it("should handle empty results", () => {
      const results = { results: [], total: 0 };
      const action = { type: "SEARCH_SUCCESS" as const, payload: { results } };
      const newState = masterInquiryReducer(initialState, action);

      expect(newState.search.results).toEqual(results);
      expect(newState.view.mode).toBe("idle");
      expect(newState.selection.selectedMember).toBeNull();
    });

    it("should differentiate between employee and beneficiary", () => {
      const results = {
        results: [
          { id: "1", ssn: "123", badgeNumber: "111", psnSuffix: "0", isEmployee: false }
        ] as unknown as EmployeeDetails[],
        total: 1
      };

      const action = { type: "SEARCH_SUCCESS" as const, payload: { results } };
      const newState = masterInquiryReducer(initialState, action);

      expect(newState.selection.selectedMember?.memberType).toBe(2);
    });
  });

  describe("SEARCH_FAILURE", () => {
    it("should set error and reset view", () => {
      const action = { type: "SEARCH_FAILURE" as const, payload: { error: "Network error" } };
      const newState = masterInquiryReducer(initialState, action);

      expect(newState.search.isSearching).toBe(false);
      expect(newState.search.error).toBe("Network error");
      expect(newState.view.mode).toBe("idle");
    });
  });

  describe("SEARCH_RESET", () => {
    it("should reset search and selection state", () => {
      const stateWithData: MasterInquiryState = {
        ...initialState,
        search: {
          ...initialState.search,
          results: { results: [], total: 0 },
          isSearching: true
        },
        selection: {
          ...initialState.selection,
          selectedMember: {
            memberType: 1,
            id: 1,
            ssn: 123,
            badgeNumber: 111,
            psnSuffix: 0
          }
        }
      };

      const action = { type: "SEARCH_RESET" as const };
      const newState = masterInquiryReducer(stateWithData, action);

      expect(newState.search).toEqual(initialState.search);
      expect(newState.selection).toEqual(initialState.selection);
      expect(newState.view.mode).toBe("idle");
    });
  });

  describe("SELECT_MEMBER", () => {
    it("should select a member and switch to member details view", () => {
      const member: SelectedMember = {
        memberType: 1,
        id: 123,
        ssn: 987654321,
        badgeNumber: 54321,
        psnSuffix: 0
      };

      const action = { type: "SELECT_MEMBER" as const, payload: { member } };
      const newState = masterInquiryReducer(initialState, action);

      expect(newState.selection.selectedMember).toEqual(member);
      expect(newState.view.mode).toBe("memberDetails");
      expect(newState.selection.memberDetails).toBeNull();
      expect(newState.selection.memberProfitData).toBeNull();
    });

    it("should deselect member and return to grid view", () => {
      const stateWithMember: MasterInquiryState = {
        ...initialState,
        selection: {
          ...initialState.selection,
          selectedMember: {
            memberType: 1,
            id: 1,
            ssn: 123,
            badgeNumber: 111,
            psnSuffix: 0
          }
        },
        view: { mode: "memberDetails" }
      };

      const action = { type: "SELECT_MEMBER" as const, payload: { member: null } };
      const newState = masterInquiryReducer(stateWithMember, action);

      expect(newState.selection.selectedMember).toBeNull();
      expect(newState.view.mode).toBe("multipleMembers");
    });
  });

  describe("MEMBER_DETAILS_FETCH", () => {
    it("should set fetching state on start", () => {
      const action = { type: "MEMBER_DETAILS_FETCH_START" as const };
      const newState = masterInquiryReducer(initialState, action);

      expect(newState.selection.isFetchingMemberDetails).toBe(true);
    });

    it("should set details on success", () => {
      const details = { name: "John Doe", ssn: "123456789" } as unknown as EmployeeDetails;
      const action = { type: "MEMBER_DETAILS_FETCH_SUCCESS" as const, payload: { details } };
      const newState = masterInquiryReducer(initialState, action);

      expect(newState.selection.memberDetails).toEqual(details);
      expect(newState.selection.isFetchingMemberDetails).toBe(false);
    });

    it("should clear details on failure", () => {
      const stateWithDetails: MasterInquiryState = {
        ...initialState,
        selection: {
          ...initialState.selection,
          memberDetails: {
            id: 1,
            badgeNumber: 123456,
            psnSuffix: 0,
            payFrequencyId: 1,
            isEmployee: true,
            firstName: "John",
            lastName: "Doe",
            address: "123 Main St",
            addressCity: "Boston",
            addressState: "MA",
            addressZipCode: "02101",
            dateOfBirth: "1990-01-01",
            ssn: "123-45-6789",
            yearToDateProfitSharingHours: 2080,
            yearsInPlan: 5,
            percentageVested: 100,
            contributionsLastYear: true,
            enrollmentId: 1,
            enrollment: "Active",
            hireDate: "2018-01-01",
            terminationDate: null,
            reHireDate: null,
            storeNumber: 1,
            beginPSAmount: 10000,
            currentPSAmount: 12000,
            beginVestedAmount: 10000,
            currentVestedAmount: 12000,
            currentEtva: 0,
            previousEtva: 0,
            employmentStatus: "Active",
            department: "IT",
            payClassification: "Salaried",
            gender: "M",
            phoneNumber: "617-555-1234",
            workLocation: "Boston HQ",
            receivedContributionsLastYear: true,
            fullTimeDate: "2018-01-01",
            terminationReason: "",
            missives: null,
            allocationFromAmount: 0,
            allocationToAmount: 0,
            badgesOfDuplicateSsns: []
          },
          isFetchingMemberDetails: true
        }
      };

      const action = { type: "MEMBER_DETAILS_FETCH_FAILURE" as const };
      const newState = masterInquiryReducer(stateWithDetails, action);

      expect(newState.selection.memberDetails).toBeNull();
      expect(newState.selection.isFetchingMemberDetails).toBe(false);
    });
  });

  describe("PROFIT_DATA_FETCH", () => {
    it("should set fetching state on start", () => {
      const action = { type: "PROFIT_DATA_FETCH_START" as const };
      const newState = masterInquiryReducer(initialState, action);

      expect(newState.selection.isFetchingProfitData).toBe(true);
    });

    it("should set profit data on success", () => {
      const profitData = { results: [{ year: 2024 }], total: 1 };
      const action = { type: "PROFIT_DATA_FETCH_SUCCESS" as const, payload: { profitData } };
      const newState = masterInquiryReducer(initialState, action);

      expect(newState.selection.memberProfitData).toEqual(profitData);
      expect(newState.selection.isFetchingProfitData).toBe(false);
    });

    it("should clear profit data on failure", () => {
      const action = { type: "PROFIT_DATA_FETCH_FAILURE" as const };
      const newState = masterInquiryReducer(initialState, action);

      expect(newState.selection.memberProfitData).toBeNull();
      expect(newState.selection.isFetchingProfitData).toBe(false);
    });
  });

  describe("SET_NO_RESULTS_MESSAGE", () => {
    it("should set no results message", () => {
      const action = {
        type: "SET_NO_RESULTS_MESSAGE" as const,
        payload: { message: "No results found" }
      };
      const newState = masterInquiryReducer(initialState, action);

      expect(newState.search.noResultsMessage).toBe("No results found");
    });

    it("should clear no results message", () => {
      const stateWithMessage: MasterInquiryState = {
        ...initialState,
        search: { ...initialState.search, noResultsMessage: "Some message" }
      };

      const action = { type: "SET_NO_RESULTS_MESSAGE" as const, payload: { message: null } };
      const newState = masterInquiryReducer(stateWithMessage, action);

      expect(newState.search.noResultsMessage).toBeNull();
    });
  });

  describe("SET_VIEW_MODE", () => {
    it("should set view mode", () => {
      const action = { type: "SET_VIEW_MODE" as const, payload: { mode: "memberDetails" as const } };
      const newState = masterInquiryReducer(initialState, action);

      expect(newState.view.mode).toBe("memberDetails");
    });
  });

  describe("RESET_ALL", () => {
    it("should reset to initial state", () => {
      const stateWithData: MasterInquiryState = {
        search: {
          params: { pagination: { skip: 0, take: 25, sortBy: "name", isSortDescending: false } },
          results: { results: [], total: 0 },
          isSearching: true,
          isManuallySearching: true,
          isFetchingMembers: true,
          noResultsMessage: "No results",
          error: "Error"
        },
        selection: {
          selectedMember: { memberType: 1, id: 1, ssn: 123, badgeNumber: 111, psnSuffix: 0 },
          memberDetails: {
            id: 1,
            badgeNumber: 123456,
            psnSuffix: 0,
            payFrequencyId: 1,
            isEmployee: true,
            firstName: "John",
            lastName: "Doe",
            address: "123 Main St",
            addressCity: "Boston",
            addressState: "MA",
            addressZipCode: "02101",
            dateOfBirth: "1990-01-01",
            ssn: "123-45-6789",
            yearToDateProfitSharingHours: 2080,
            yearsInPlan: 5,
            percentageVested: 100,
            contributionsLastYear: true,
            enrollmentId: 1,
            enrollment: "Active",
            hireDate: "2018-01-01",
            terminationDate: null,
            reHireDate: null,
            storeNumber: 1,
            beginPSAmount: 10000,
            currentPSAmount: 12000,
            beginVestedAmount: 10000,
            currentVestedAmount: 12000,
            currentEtva: 0,
            previousEtva: 0,
            employmentStatus: "Active",
            department: "IT",
            payClassification: "Salaried",
            gender: "M",
            phoneNumber: "617-555-1234",
            workLocation: "Boston HQ",
            receivedContributionsLastYear: true,
            fullTimeDate: "2018-01-01",
            terminationReason: "",
            missives: null,
            allocationFromAmount: 0,
            allocationToAmount: 0,
            badgesOfDuplicateSsns: []
          },
          memberProfitData: { results: [], total: 0 },
          isFetchingMemberDetails: true,
          isFetchingProfitData: true
        },
        view: { mode: "memberDetails" }
      };

      const action = { type: "RESET_ALL" as const };
      const newState = masterInquiryReducer(stateWithData, action);

      expect(newState).toEqual(initialState);
    });
  });

  describe("selectors", () => {
    describe("selectShowMemberGrid", () => {
      it("should return true when in multipleMembers mode with multiple results", () => {
        const state: MasterInquiryState = {
          ...initialState,
          search: {
            ...initialState.search,
            results: {
              results: [
                {
                  id: 1,
                  badgeNumber: 123456,
                  psnSuffix: 0,
                  payFrequencyId: 1,
                  isEmployee: true,
                  firstName: "John",
                  lastName: "Doe",
                  address: "123 Main St",
                  addressCity: "Boston",
                  addressState: "MA",
                  addressZipCode: "02101",
                  dateOfBirth: "1990-01-01",
                  ssn: "123-45-6789",
                  yearToDateProfitSharingHours: 2080,
                  yearsInPlan: 5,
                  percentageVested: 100,
                  contributionsLastYear: true,
                  enrollmentId: 1,
                  enrollment: "Active",
                  hireDate: "2018-01-01",
                  terminationDate: null,
                  reHireDate: null,
                  storeNumber: 1,
                  beginPSAmount: 10000,
                  currentPSAmount: 12000,
                  beginVestedAmount: 10000,
                  currentVestedAmount: 12000,
                  currentEtva: 0,
                  previousEtva: 0,
                  employmentStatus: "Active",
                  department: "IT",
                  payClassification: "Salaried",
                  gender: "M",
                  phoneNumber: "617-555-1234",
                  workLocation: "Boston HQ",
                  receivedContributionsLastYear: true,
                  fullTimeDate: "2018-01-01",
                  terminationReason: "",
                  missives: null,
                  allocationFromAmount: 0,
                  allocationToAmount: 0,
                  badgesOfDuplicateSsns: []
                },
                {
                  id: 2,
                  badgeNumber: 123457,
                  psnSuffix: 0,
                  payFrequencyId: 1,
                  isEmployee: true,
                  firstName: "Jane",
                  lastName: "Smith",
                  address: "456 Oak Ave",
                  addressCity: "Boston",
                  addressState: "MA",
                  addressZipCode: "02101",
                  dateOfBirth: "1992-05-15",
                  ssn: "987-65-4321",
                  yearToDateProfitSharingHours: 2000,
                  yearsInPlan: 3,
                  percentageVested: 80,
                  contributionsLastYear: true,
                  enrollmentId: 1,
                  enrollment: "Active",
                  hireDate: "2020-03-01",
                  terminationDate: null,
                  reHireDate: null,
                  storeNumber: 2,
                  beginPSAmount: 8000,
                  currentPSAmount: 10000,
                  beginVestedAmount: 6400,
                  currentVestedAmount: 8000,
                  currentEtva: 0,
                  previousEtva: 0,
                  employmentStatus: "Active",
                  department: "Sales",
                  payClassification: "Hourly",
                  gender: "F",
                  phoneNumber: "617-555-5678",
                  workLocation: "Boston HQ",
                  receivedContributionsLastYear: true,
                  fullTimeDate: "2020-03-01",
                  terminationReason: "",
                  missives: null,
                  allocationFromAmount: 0,
                  allocationToAmount: 0,
                  badgesOfDuplicateSsns: []
                }
              ],
              total: 2
            }
          },
          view: { mode: "multipleMembers" }
        };

        expect(selectShowMemberGrid(state)).toBe(true);
      });

      it("should return false when in multipleMembers mode with single result", () => {
        const state: MasterInquiryState = {
          ...initialState,
          search: {
            ...initialState.search,
            results: {
              results: [
                {
                  id: 1,
                  badgeNumber: 123456,
                  psnSuffix: 0,
                  payFrequencyId: 1,
                  isEmployee: true,
                  firstName: "John",
                  lastName: "Doe",
                  address: "123 Main St",
                  addressCity: "Boston",
                  addressState: "MA",
                  addressZipCode: "02101",
                  dateOfBirth: "1990-01-01",
                  ssn: "123-45-6789",
                  yearToDateProfitSharingHours: 2080,
                  yearsInPlan: 5,
                  percentageVested: 100,
                  contributionsLastYear: true,
                  enrollmentId: 1,
                  enrollment: "Active",
                  hireDate: "2018-01-01",
                  terminationDate: null,
                  reHireDate: null,
                  storeNumber: 1,
                  beginPSAmount: 10000,
                  currentPSAmount: 12000,
                  beginVestedAmount: 10000,
                  currentVestedAmount: 12000,
                  currentEtva: 0,
                  previousEtva: 0,
                  employmentStatus: "Active",
                  department: "IT",
                  payClassification: "Salaried",
                  gender: "M",
                  phoneNumber: "617-555-1234",
                  workLocation: "Boston HQ",
                  receivedContributionsLastYear: true,
                  fullTimeDate: "2018-01-01",
                  terminationReason: "",
                  missives: null,
                  allocationFromAmount: 0,
                  allocationToAmount: 0,
                  badgesOfDuplicateSsns: []
                }
              ],
              total: 1
            }
          },
          view: { mode: "multipleMembers" }
        };

        expect(selectShowMemberGrid(state)).toBe(false);
      });

      it("should return false when not in multipleMembers mode", () => {
        const state: MasterInquiryState = {
          ...initialState,
          search: {
            ...initialState.search,
            results: {
              results: [
                {
                  id: 1,
                  badgeNumber: 123456,
                  psnSuffix: 0,
                  payFrequencyId: 1,
                  isEmployee: true,
                  firstName: "John",
                  lastName: "Doe",
                  address: "123 Main St",
                  addressCity: "Boston",
                  addressState: "MA",
                  addressZipCode: "02101",
                  dateOfBirth: "1990-01-01",
                  ssn: "123-45-6789",
                  yearToDateProfitSharingHours: 2080,
                  yearsInPlan: 5,
                  percentageVested: 100,
                  contributionsLastYear: true,
                  enrollmentId: 1,
                  enrollment: "Active",
                  hireDate: "2018-01-01",
                  terminationDate: null,
                  reHireDate: null,
                  storeNumber: 1,
                  beginPSAmount: 10000,
                  currentPSAmount: 12000,
                  beginVestedAmount: 10000,
                  currentVestedAmount: 12000,
                  currentEtva: 0,
                  previousEtva: 0,
                  employmentStatus: "Active",
                  department: "IT",
                  payClassification: "Salaried",
                  gender: "M",
                  phoneNumber: "617-555-1234",
                  workLocation: "Boston HQ",
                  receivedContributionsLastYear: true,
                  fullTimeDate: "2018-01-01",
                  terminationReason: "",
                  missives: null,
                  allocationFromAmount: 0,
                  allocationToAmount: 0,
                  badgesOfDuplicateSsns: []
                },
                {
                  id: 2,
                  badgeNumber: 123457,
                  psnSuffix: 0,
                  payFrequencyId: 1,
                  isEmployee: true,
                  firstName: "Jane",
                  lastName: "Smith",
                  address: "456 Oak Ave",
                  addressCity: "Boston",
                  addressState: "MA",
                  addressZipCode: "02101",
                  dateOfBirth: "1992-05-15",
                  ssn: "987-65-4321",
                  yearToDateProfitSharingHours: 2000,
                  yearsInPlan: 3,
                  percentageVested: 80,
                  contributionsLastYear: true,
                  enrollmentId: 1,
                  enrollment: "Active",
                  hireDate: "2020-03-01",
                  terminationDate: null,
                  reHireDate: null,
                  storeNumber: 2,
                  beginPSAmount: 8000,
                  currentPSAmount: 10000,
                  beginVestedAmount: 6400,
                  currentVestedAmount: 8000,
                  currentEtva: 0,
                  previousEtva: 0,
                  employmentStatus: "Active",
                  department: "Sales",
                  payClassification: "Hourly",
                  gender: "F",
                  phoneNumber: "617-555-5678",
                  workLocation: "Boston HQ",
                  receivedContributionsLastYear: true,
                  fullTimeDate: "2020-03-01",
                  terminationReason: "",
                  missives: null,
                  allocationFromAmount: 0,
                  allocationToAmount: 0,
                  badgesOfDuplicateSsns: []
                }
              ],
              total: 2
            }
          },
          view: { mode: "memberDetails" }
        };

        expect(selectShowMemberGrid(state)).toBe(false);
      });
    });

    describe("selectShowMemberDetails", () => {
      it("should return true when in memberDetails mode with selected member", () => {
        const state: MasterInquiryState = {
          ...initialState,
          selection: {
            ...initialState.selection,
            selectedMember: { memberType: 1, id: 1, ssn: 123, badgeNumber: 111, psnSuffix: 0 }
          },
          view: { mode: "memberDetails" }
        };

        expect(selectShowMemberDetails(state)).toBe(true);
      });

      it("should return false when no member selected", () => {
        const state: MasterInquiryState = {
          ...initialState,
          view: { mode: "memberDetails" }
        };

        expect(selectShowMemberDetails(state)).toBe(false);
      });
    });

    describe("selectShowProfitDetails", () => {
      it("should return true when in memberDetails mode with member and profit data", () => {
        const state: MasterInquiryState = {
          ...initialState,
          selection: {
            ...initialState.selection,
            selectedMember: { memberType: 1, id: 1, ssn: 123, badgeNumber: 111, psnSuffix: 0 },
            memberProfitData: { results: [], total: 0 }
          },
          view: { mode: "memberDetails" }
        };

        expect(selectShowProfitDetails(state)).toBe(true);
      });

      it("should return false when no profit data", () => {
        const state: MasterInquiryState = {
          ...initialState,
          selection: {
            ...initialState.selection,
            selectedMember: { memberType: 1, id: 1, ssn: 123, badgeNumber: 111, psnSuffix: 0 }
          },
          view: { mode: "memberDetails" }
        };

        expect(selectShowProfitDetails(state)).toBe(false);
      });
    });
  });
});
