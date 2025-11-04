import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook } from "@testing-library/react";
import React from "react";
import { Provider } from "react-redux";
import { configureStore } from "@reduxjs/toolkit";
import securityReducer, { type SecurityState } from "../../../../../reduxstore/slices/securitySlice";
import yearsEndReducer, { type YearsEndState } from "../../../../../reduxstore/slices/yearsEndSlice";

// Hoist all mock variables to be accessible in vi.mock() calls
const { mockTriggerSearch, mockUseGridPagination } = vi.hoisted(() => ({
  mockTriggerSearch: vi.fn(),
  mockUseGridPagination: vi.fn()
}));

// Mock RTK Query hook - this must return [triggerFunction, stateObject]
vi.mock("../../../../reduxstore/api/YearsEndApi", () => ({
  useLazyGetDuplicateNamesAndBirthdaysQuery: vi.fn(() => [mockTriggerSearch, { isFetching: false }])
}));

vi.mock("../../../../hooks/useGridPagination", () => ({
  useGridPagination: mockUseGridPagination
}));

import useDuplicateNamesAndBirthdays from "../useDuplicateNamesAndBirthdays";

type RootState = {
  security: SecurityState;
  yearsEnd: YearsEndState;
};

type MockStoreState = Partial<RootState>;

function createMockStore(preloadedState?: MockStoreState) {
  return configureStore<RootState>({
    reducer: {
      security: securityReducer,
      yearsEnd: yearsEndReducer
    } as const,
    preloadedState: preloadedState as RootState | undefined
  });
}

function renderHookWithProvider<T>(hook: () => T, preloadedState?: MockStoreState) {
  const defaultState: MockStoreState = {
    security: {
      token: "mock-token",
      userGroups: [],
      userRoles: [],
      userPermissions: [],
      username: "test-user",
      performLogout: false,
      appUser: null,
      impersonating: []
    },
    yearsEnd: {
      selectedProfitYearForDecemberActivities: 2024,
      selectedProfitYearForFiscalClose: 2024,
      invalidProfitShareEditYear: false,
      totalForfeituresGreaterThanZero: false,
      profitShareEditUpdateShowSearch: true,
      profitShareApplyOrRevertLoading: false,
      resetYearEndPage: false,
      profitMasterStatus: null,
      additionalExecutivesChosen: null,
      additionalExecutivesGrid: null,
      balanceByAgeFullTime: null,
      balanceByAgePartTime: null,
      balanceByAgeTotal: null,
      balanceByAgeQueryParams: null,
      balanceByYearsFullTime: null,
      balanceByYearsPartTime: null,
      balanceByYearsTotal: null,
      balanceByYearsQueryParams: null,
      contributionsByAgeFullTime: null,
      contributionsByAgePartTime: null,
      contributionsByAgeTotal: null,
      contributionsByAgeQueryParams: null,
      demographicBadges: null,
      distributionsAndForfeitures: null,
      distributionsAndForfeituresQueryParams: null,
      distributionsByAgeFullTime: null,
      distributionsByAgePartTime: null,
      distributionsByAgeTotal: null,
      distributionsByAgeQueryParams: null,
      duplicateSSNsData: null,
      duplicateNamesAndBirthdays: null,
      duplicateNamesAndBirthdaysQueryParams: null,
      eligibleEmployees: null,
      eligibleEmployeesQueryParams: null,
      employeeWagesForYear: null,
      employeeWagesForYearQueryParams: null,
      executiveHoursAndDollars: null,
      executiveHoursAndDollarsGrid: null,
      executiveRowsSelected: null,
      executiveHoursAndDollarsQueryParams: null,
      executiveHoursAndDollarsAddQueryParams: null,
      forfeituresByAgeFullTime: null,
      forfeituresByAgePartTime: null,
      forfeituresByAgeTotal: null,
      forfeituresByAgeQueryParams: null,
      forfeituresAndPoints: null,
      forfeituresAndPointsQueryParams: null,
      grossWagesReport: null,
      grossWagesReportQueryParams: null,
      rehireQueryParams: null,
      militaryEntryAndModification: null,
      recentlyTerminated: null,
      recentlyTerminatedQueryParams: null,
      terminatedLetters: null,
      terminatedLettersQueryParams: null,
      unForfeits: null,
      unForfeitsQueryParams: null,
      rehireProfitSummaryQueryParams: null,
      negativeEtvaForSSNsOnPayprofit: null,
      negativeEtvaForSSNsOnPayprofitParams: null,
      profitSharingUpdate: null,
      profitSharingUpdateQueryParams: null,
      profitSharingEditQueryParams: null,
      profitSharingEdit: null,
      profitSharingMaster: null,
      profitSharingRevert: null,
      profitSharingUpdateAdjustmentSummary: null,
      profitEditUpdateChangesAvailable: false,
      profitEditUpdateRevertChangesAvailable: false,
      termination: null,
      terminationQueryParams: null,
      vestedAmountsByAge: null,
      vestedAmountsByAgeQueryParams: null,
      yearEndProfitSharingReportLive: null,
      yearEndProfitSharingReportFrozen: null,
      yearEndProfitSharingReportQueryParams: null,
      yearEndProfitSharingReportTotals: null,
      breakdownByStore: null,
      breakdownByStoreMangement: null,
      breakdownByStoreTotals: null,
      storeManagementBreakdown: null,
      breakdownByStoreQueryParams: null,
      under21BreakdownByStore: null,
      under21BreakdownByStoreQueryParams: null,
      under21Inactive: null,
      under21InactiveQueryParams: null,
      under21Totals: null,
      under21TotalsQueryParams: null,
      profitShareSummaryReport: null,
      updateSummary: null,
      profitSharingLabels: null,
      controlSheet: null,
      breakdownGrandTotals: null,
      certificates: null
    }
  };

  const store = createMockStore(preloadedState || defaultState);
  return renderHook(() => hook(), {
    wrapper: ({ children }: { children: React.ReactNode }) => React.createElement(Provider, { store, children })
  });
}

const mockPaginationObject = {
  pageNumber: 0,
  pageSize: 25,
  sortParams: { sortBy: "name", isSortDescending: false },
  handlePaginationChange: vi.fn(),
  handleSortChange: vi.fn(),
  resetPagination: vi.fn()
};

const mockDuplicateData = {
  results: [
    {
      id: 1,
      firstName: "John",
      lastName: "Smith",
      dateOfBirth: "1990-01-15",
      badgeNumber: 123456
    }
  ],
  total: 1
};

describe("useDuplicateNamesAndBirthdays", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    // Reset RTK Query hook to return default state
    mockTriggerSearch.mockReturnValue({
      unwrap: vi.fn()
    });
  });

  it("should not fetch when token is missing", () => {
    // Setup mocks
    mockUseGridPagination.mockReturnValue(mockPaginationObject);

    const defaultYearsEndState: YearsEndState = {
      selectedProfitYearForDecemberActivities: 2024,
      selectedProfitYearForFiscalClose: 2024,
      invalidProfitShareEditYear: false,
      totalForfeituresGreaterThanZero: false,
      profitShareEditUpdateShowSearch: true,
      profitShareApplyOrRevertLoading: false,
      resetYearEndPage: false,
      profitMasterStatus: null,
      additionalExecutivesChosen: null,
      additionalExecutivesGrid: null,
      balanceByAgeFullTime: null,
      balanceByAgePartTime: null,
      balanceByAgeTotal: null,
      balanceByAgeQueryParams: null,
      balanceByYearsFullTime: null,
      balanceByYearsPartTime: null,
      balanceByYearsTotal: null,
      balanceByYearsQueryParams: null,
      contributionsByAgeFullTime: null,
      contributionsByAgePartTime: null,
      contributionsByAgeTotal: null,
      contributionsByAgeQueryParams: null,
      demographicBadges: null,
      distributionsAndForfeitures: null,
      distributionsAndForfeituresQueryParams: null,
      distributionsByAgeFullTime: null,
      distributionsByAgePartTime: null,
      distributionsByAgeTotal: null,
      distributionsByAgeQueryParams: null,
      duplicateSSNsData: null,
      duplicateNamesAndBirthdays: null,
      duplicateNamesAndBirthdaysQueryParams: null,
      eligibleEmployees: null,
      eligibleEmployeesQueryParams: null,
      employeeWagesForYear: null,
      employeeWagesForYearQueryParams: null,
      executiveHoursAndDollars: null,
      executiveHoursAndDollarsGrid: null,
      executiveRowsSelected: null,
      executiveHoursAndDollarsQueryParams: null,
      executiveHoursAndDollarsAddQueryParams: null,
      forfeituresByAgeFullTime: null,
      forfeituresByAgePartTime: null,
      forfeituresByAgeTotal: null,
      forfeituresByAgeQueryParams: null,
      forfeituresAndPoints: null,
      forfeituresAndPointsQueryParams: null,
      grossWagesReport: null,
      grossWagesReportQueryParams: null,
      rehireQueryParams: null,
      militaryEntryAndModification: null,
      recentlyTerminated: null,
      recentlyTerminatedQueryParams: null,
      terminatedLetters: null,
      terminatedLettersQueryParams: null,
      unForfeits: null,
      unForfeitsQueryParams: null,
      rehireProfitSummaryQueryParams: null,
      negativeEtvaForSSNsOnPayprofit: null,
      negativeEtvaForSSNsOnPayprofitParams: null,
      profitSharingUpdate: null,
      profitSharingUpdateQueryParams: null,
      profitSharingEditQueryParams: null,
      profitSharingEdit: null,
      profitSharingMaster: null,
      profitSharingRevert: null,
      profitSharingUpdateAdjustmentSummary: null,
      profitEditUpdateChangesAvailable: false,
      profitEditUpdateRevertChangesAvailable: false,
      termination: null,
      terminationQueryParams: null,
      vestedAmountsByAge: null,
      vestedAmountsByAgeQueryParams: null,
      yearEndProfitSharingReportLive: null,
      yearEndProfitSharingReportFrozen: null,
      yearEndProfitSharingReportQueryParams: null,
      yearEndProfitSharingReportTotals: null,
      breakdownByStore: null,
      breakdownByStoreMangement: null,
      breakdownByStoreTotals: null,
      storeManagementBreakdown: null,
      breakdownByStoreQueryParams: null,
      under21BreakdownByStore: null,
      under21BreakdownByStoreQueryParams: null,
      under21Inactive: null,
      under21InactiveQueryParams: null,
      under21Totals: null,
      under21TotalsQueryParams: null,
      profitShareSummaryReport: null,
      updateSummary: null,
      profitSharingLabels: null,
      controlSheet: null,
      breakdownGrandTotals: null,
      certificates: null
    };

    const { result } = renderHookWithProvider(() => useDuplicateNamesAndBirthdays(), {
      security: {
        token: null,
        userGroups: [],
        userRoles: [],
        userPermissions: [],
        username: "",
        performLogout: false,
        appUser: null,
        impersonating: []
      },
      yearsEnd: {
        ...defaultYearsEndState,
        selectedProfitYearForDecemberActivities: 2024
      }
    });

    expect(result.current.isSearching).toBe(false);
  });

  it("should not fetch when profitYear is missing", () => {
    // Setup mocks
    mockUseGridPagination.mockReturnValue(mockPaginationObject);

    const { result } = renderHookWithProvider(() => useDuplicateNamesAndBirthdays(), {
      security: {
        token: "mock-token",
        userGroups: [],
        userRoles: [],
        userPermissions: [],
        username: "test-user",
        performLogout: false,
        appUser: null,
        impersonating: []
      },
      yearsEnd: {
        selectedProfitYearForDecemberActivities: null
      } as unknown as Partial<YearsEndState>
    });

    expect(result.current.isSearching).toBe(false);
  });

  it("should expose executeSearch function", () => {
    // Setup mocks
    mockTriggerSearch.mockReturnValue({
      unwrap: vi.fn().mockResolvedValue(mockDuplicateData)
    });
    mockUseGridPagination.mockReturnValue(mockPaginationObject);

    const { result } = renderHookWithProvider(() => useDuplicateNamesAndBirthdays());

    expect(typeof result.current.executeSearch).toBe("function");
  });

  it("should expose pagination object", () => {
    // Setup mocks
    mockUseGridPagination.mockReturnValue(mockPaginationObject);

    const { result } = renderHookWithProvider(() => useDuplicateNamesAndBirthdays());

    expect(result.current.pagination).toBeDefined();
    expect(result.current.pagination.pageNumber).toBe(0);
    expect(result.current.pagination.pageSize).toBe(25);
  });

  it("should expose showData selector", () => {
    // Setup mocks
    mockUseGridPagination.mockReturnValue(mockPaginationObject);

    const { result } = renderHookWithProvider(() => useDuplicateNamesAndBirthdays());

    expect(typeof result.current.showData).toBe("boolean");
  });

  it("should expose hasResults selector", () => {
    // Setup mocks
    mockTriggerSearch.mockReturnValue({
      unwrap: vi.fn().mockResolvedValue(mockDuplicateData)
    });
    mockUseGridPagination.mockReturnValue(mockPaginationObject);

    const { result } = renderHookWithProvider(() => useDuplicateNamesAndBirthdays());

    expect(typeof result.current.hasResults).toBe("boolean");
  });

  it("should return null searchParams initially", () => {
    // Setup mocks
    mockUseGridPagination.mockReturnValue(mockPaginationObject);

    const defaultYearsEndState: YearsEndState = {
      selectedProfitYearForDecemberActivities: 2024,
      selectedProfitYearForFiscalClose: 2024,
      invalidProfitShareEditYear: false,
      totalForfeituresGreaterThanZero: false,
      profitShareEditUpdateShowSearch: true,
      profitShareApplyOrRevertLoading: false,
      resetYearEndPage: false,
      profitMasterStatus: null,
      additionalExecutivesChosen: null,
      additionalExecutivesGrid: null,
      balanceByAgeFullTime: null,
      balanceByAgePartTime: null,
      balanceByAgeTotal: null,
      balanceByAgeQueryParams: null,
      balanceByYearsFullTime: null,
      balanceByYearsPartTime: null,
      balanceByYearsTotal: null,
      balanceByYearsQueryParams: null,
      contributionsByAgeFullTime: null,
      contributionsByAgePartTime: null,
      contributionsByAgeTotal: null,
      contributionsByAgeQueryParams: null,
      demographicBadges: null,
      distributionsAndForfeitures: null,
      distributionsAndForfeituresQueryParams: null,
      distributionsByAgeFullTime: null,
      distributionsByAgePartTime: null,
      distributionsByAgeTotal: null,
      distributionsByAgeQueryParams: null,
      duplicateSSNsData: null,
      duplicateNamesAndBirthdays: null,
      duplicateNamesAndBirthdaysQueryParams: null,
      eligibleEmployees: null,
      eligibleEmployeesQueryParams: null,
      employeeWagesForYear: null,
      employeeWagesForYearQueryParams: null,
      executiveHoursAndDollars: null,
      executiveHoursAndDollarsGrid: null,
      executiveRowsSelected: null,
      executiveHoursAndDollarsQueryParams: null,
      executiveHoursAndDollarsAddQueryParams: null,
      forfeituresByAgeFullTime: null,
      forfeituresByAgePartTime: null,
      forfeituresByAgeTotal: null,
      forfeituresByAgeQueryParams: null,
      forfeituresAndPoints: null,
      forfeituresAndPointsQueryParams: null,
      grossWagesReport: null,
      grossWagesReportQueryParams: null,
      rehireQueryParams: null,
      militaryEntryAndModification: null,
      recentlyTerminated: null,
      recentlyTerminatedQueryParams: null,
      terminatedLetters: null,
      terminatedLettersQueryParams: null,
      unForfeits: null,
      unForfeitsQueryParams: null,
      rehireProfitSummaryQueryParams: null,
      negativeEtvaForSSNsOnPayprofit: null,
      negativeEtvaForSSNsOnPayprofitParams: null,
      profitSharingUpdate: null,
      profitSharingUpdateQueryParams: null,
      profitSharingEditQueryParams: null,
      profitSharingEdit: null,
      profitSharingMaster: null,
      profitSharingRevert: null,
      profitSharingUpdateAdjustmentSummary: null,
      profitEditUpdateChangesAvailable: false,
      profitEditUpdateRevertChangesAvailable: false,
      termination: null,
      terminationQueryParams: null,
      vestedAmountsByAge: null,
      vestedAmountsByAgeQueryParams: null,
      yearEndProfitSharingReportLive: null,
      yearEndProfitSharingReportFrozen: null,
      yearEndProfitSharingReportQueryParams: null,
      yearEndProfitSharingReportTotals: null,
      breakdownByStore: null,
      breakdownByStoreMangement: null,
      breakdownByStoreTotals: null,
      storeManagementBreakdown: null,
      breakdownByStoreQueryParams: null,
      under21BreakdownByStore: null,
      under21BreakdownByStoreQueryParams: null,
      under21Inactive: null,
      under21InactiveQueryParams: null,
      under21Totals: null,
      under21TotalsQueryParams: null,
      profitShareSummaryReport: null,
      updateSummary: null,
      profitSharingLabels: null,
      controlSheet: null,
      breakdownGrandTotals: null,
      certificates: null
    };

    const { result } = renderHookWithProvider(() => useDuplicateNamesAndBirthdays(), {
      security: {
        token: "mock-token",
        userGroups: [],
        userRoles: [],
        userPermissions: [],
        username: "test-user",
        performLogout: false,
        appUser: null,
        impersonating: []
      },
      yearsEnd: {
        ...defaultYearsEndState,
        selectedProfitYearForDecemberActivities: null
      }
    });

    expect(result.current.searchParams).toBeNull();
  });

  it("should expose isSearching state", () => {
    // Setup mocks - the hook returns isSearching || state.search.isLoading
    mockUseGridPagination.mockReturnValue(mockPaginationObject);

    const { result } = renderHookWithProvider(() => useDuplicateNamesAndBirthdays());

    expect(typeof result.current.isSearching).toBe("boolean");
  });
});
