import { describe, it, expect } from "vitest";
import reducer, {
  setAccountingYearParams,
  clearAccountingYearRequestParams,
  setAccountingYearData,
  clearAccountingYearData,
  setMissivesData,
  setStateTaxData,
  clearStateTaxData,
  LookupState
} from "./lookupsSlice";
import { CalendarResponseDto, MissiveResponse, ProfitYearRequest, StateTaxLookupResponse } from "reduxstore/types";

describe("lookupsSlice", () => {
  const initialState: LookupState = {
    accountingYearData: null,
    accountingYearRequestParams: null,
    missives: null,
    stateTaxData: null
  };

  const mockProfitYearRequest: ProfitYearRequest = {
    profitYear: 2024
  };

  const mockCalendarResponse: CalendarResponseDto = {
    fiscalBeginDate: "2024-01-01",
    fiscalEndDate: "2024-12-31"
  };

  const mockMissives: MissiveResponse[] = [
    {
      id: 1,
      message: "Please complete year-end processing",
      description: "Year End Reminder",
      severity: "high"
    },
    {
      id: 2,
      message: "Scheduled maintenance on Saturday",
      description: "System Maintenance",
      severity: "medium"
    }
  ];

  const mockStateTaxData: StateTaxLookupResponse = {
    state: "MA",
    stateTaxRate: 0.05
  };

  describe("reducer", () => {
    it("should return initial state when called with undefined state", () => {
      expect(reducer(undefined, { type: "unknown" })).toEqual(initialState);
    });

    it("should return current state for unknown action", () => {
      const currentState: LookupState = {
        ...initialState,
        accountingYearData: mockCalendarResponse
      };
      expect(reducer(currentState, { type: "unknown" })).toEqual(currentState);
    });
  });

  describe("setAccountingYearParams", () => {
    it("should set accounting year request params", () => {
      const nextState = reducer(initialState, setAccountingYearParams(mockProfitYearRequest));

      expect(nextState.accountingYearRequestParams).toEqual(mockProfitYearRequest);
      expect(nextState.accountingYearRequestParams?.profitYear).toBe(2024);
    });

    it("should replace existing accounting year params", () => {
      const currentState: LookupState = {
        ...initialState,
        accountingYearRequestParams: { profitYear: 2023 }
      };

      const nextState = reducer(currentState, setAccountingYearParams({ profitYear: 2024 }));

      expect(nextState.accountingYearRequestParams?.profitYear).toBe(2024);
    });

    it("should handle different profit years", () => {
      const years = [2020, 2021, 2022, 2023, 2024];

      years.forEach((year) => {
        const state = reducer(initialState, setAccountingYearParams({ profitYear: year }));
        expect(state.accountingYearRequestParams?.profitYear).toBe(year);
      });
    });
  });

  describe("clearAccountingYearRequestParams", () => {
    it("should clear accounting year request params", () => {
      const currentState: LookupState = {
        ...initialState,
        accountingYearRequestParams: mockProfitYearRequest
      };

      const nextState = reducer(currentState, clearAccountingYearRequestParams());

      expect(nextState.accountingYearRequestParams).toBeNull();
    });

    it("should handle clearing already null params", () => {
      const nextState = reducer(initialState, clearAccountingYearRequestParams());

      expect(nextState.accountingYearRequestParams).toBeNull();
    });

    it("should not affect other state properties", () => {
      const currentState: LookupState = {
        ...initialState,
        accountingYearRequestParams: mockProfitYearRequest,
        accountingYearData: mockCalendarResponse,
        missives: mockMissives
      };

      const nextState = reducer(currentState, clearAccountingYearRequestParams());

      expect(nextState.accountingYearRequestParams).toBeNull();
      expect(nextState.accountingYearData).toEqual(mockCalendarResponse);
      expect(nextState.missives).toEqual(mockMissives);
    });
  });

  describe("setAccountingYearData", () => {
    it("should set accounting year data", () => {
      const nextState = reducer(initialState, setAccountingYearData(mockCalendarResponse));

      expect(nextState.accountingYearData).toEqual(mockCalendarResponse);
      expect(nextState.accountingYearData?.fiscalBeginDate).toBe("2024-01-01");
      expect(nextState.accountingYearData?.fiscalEndDate).toBe("2024-12-31");
    });

    it("should replace existing accounting year data", () => {
      const oldCalendar: CalendarResponseDto = {
        fiscalBeginDate: "2023-01-01",
        fiscalEndDate: "2023-12-31"
      };

      const currentState: LookupState = {
        ...initialState,
        accountingYearData: oldCalendar
      };

      const nextState = reducer(currentState, setAccountingYearData(mockCalendarResponse));

      expect(nextState.accountingYearData?.fiscalBeginDate).toBe("2024-01-01");
    });
  });

  describe("clearAccountingYearData", () => {
    it("should clear accounting year data", () => {
      const currentState: LookupState = {
        ...initialState,
        accountingYearData: mockCalendarResponse
      };

      const nextState = reducer(currentState, clearAccountingYearData());

      expect(nextState.accountingYearData).toBeNull();
    });

    it("should handle clearing already null data", () => {
      const nextState = reducer(initialState, clearAccountingYearData());

      expect(nextState.accountingYearData).toBeNull();
    });

    it("should not affect request params", () => {
      const currentState: LookupState = {
        ...initialState,
        accountingYearData: mockCalendarResponse,
        accountingYearRequestParams: mockProfitYearRequest
      };

      const nextState = reducer(currentState, clearAccountingYearData());

      expect(nextState.accountingYearData).toBeNull();
      expect(nextState.accountingYearRequestParams).toEqual(mockProfitYearRequest);
    });
  });

  describe("setMissivesData", () => {
    it("should set missives data", () => {
      const nextState = reducer(initialState, setMissivesData(mockMissives));

      expect(nextState.missives).toEqual(mockMissives);
      expect(nextState.missives).toHaveLength(2);
    });

    it("should replace existing missives", () => {
      const oldMissives: MissiveResponse[] = [
        {
          id: 99,
          message: "Old message",
          description: "Old Missive",
          severity: "low"
        }
      ];

      const currentState: LookupState = {
        ...initialState,
        missives: oldMissives
      };

      const nextState = reducer(currentState, setMissivesData(mockMissives));

      expect(nextState.missives).toEqual(mockMissives);
      expect(nextState.missives).toHaveLength(2);
      expect(nextState.missives?.[0].id).not.toBe(99);
    });

    it("should handle empty missives array", () => {
      const nextState = reducer(initialState, setMissivesData([]));

      expect(nextState.missives).toEqual([]);
      expect(nextState.missives).toHaveLength(0);
    });

    it("should handle single missive", () => {
      const singleMissive: MissiveResponse[] = [mockMissives[0]];

      const nextState = reducer(initialState, setMissivesData(singleMissive));

      expect(nextState.missives).toHaveLength(1);
      expect(nextState.missives?.[0].description).toBe("Year End Reminder");
    });

    it("should handle many missives", () => {
      const manyMissives: MissiveResponse[] = Array.from({ length: 50 }, (_, i) => ({
        id: i + 1,
        message: `Message ${i + 1}`,
        description: `Missive ${i + 1}`,
        severity: i % 2 === 0 ? "high" : "low"
      }));

      const nextState = reducer(initialState, setMissivesData(manyMissives));

      expect(nextState.missives).toHaveLength(50);
      expect(nextState.missives?.[0].id).toBe(1);
      expect(nextState.missives?.[49].id).toBe(50);
    });

    it("should handle missives with different severities", () => {
      const severityMissives: MissiveResponse[] = [
        { ...mockMissives[0], severity: "high" },
        { ...mockMissives[1], severity: "medium" },
        { id: 3, message: "Low priority", description: "Low", severity: "low" }
      ];

      const nextState = reducer(initialState, setMissivesData(severityMissives));

      expect(nextState.missives).toHaveLength(3);
      expect(nextState.missives?.[0].severity).toBe("high");
      expect(nextState.missives?.[1].severity).toBe("medium");
      expect(nextState.missives?.[2].severity).toBe("low");
    });
  });

  describe("setStateTaxData", () => {
    it("should set state tax data", () => {
      const nextState = reducer(initialState, setStateTaxData(mockStateTaxData));

      expect(nextState.stateTaxData).toEqual(mockStateTaxData);
      expect(nextState.stateTaxData?.state).toBe("MA");
      expect(nextState.stateTaxData?.stateTaxRate).toBe(0.05);
    });

    it("should replace existing state tax data", () => {
      const oldTaxData: StateTaxLookupResponse = {
        state: "CA",
        stateTaxRate: 0.0725
      };

      const currentState: LookupState = {
        ...initialState,
        stateTaxData: oldTaxData
      };

      const nextState = reducer(currentState, setStateTaxData(mockStateTaxData));

      expect(nextState.stateTaxData?.state).toBe("MA");
      expect(nextState.stateTaxData?.stateTaxRate).toBe(0.05);
    });

    it("should handle different states", () => {
      const states = [
        { state: "NY", stateTaxRate: 0.04 },
        { state: "TX", stateTaxRate: 0.0 },
        { state: "FL", stateTaxRate: 0.0 }
      ];

      states.forEach((stateInfo) => {
        const taxData: StateTaxLookupResponse = stateInfo;

        const state = reducer(initialState, setStateTaxData(taxData));
        expect(state.stateTaxData?.state).toBe(stateInfo.state);
        expect(state.stateTaxData?.stateTaxRate).toBe(stateInfo.stateTaxRate);
      });
    });

    it("should handle zero tax rate states", () => {
      const zeroTaxData: StateTaxLookupResponse = {
        state: "TX",
        stateTaxRate: 0.0
      };

      const nextState = reducer(initialState, setStateTaxData(zeroTaxData));

      expect(nextState.stateTaxData?.stateTaxRate).toBe(0.0);
    });

    it("should handle high tax rate states", () => {
      const highTaxData: StateTaxLookupResponse = {
        state: "CA",
        stateTaxRate: 0.13
      };

      const nextState = reducer(initialState, setStateTaxData(highTaxData));

      expect(nextState.stateTaxData?.stateTaxRate).toBe(0.13);
    });
  });

  describe("clearStateTaxData", () => {
    it("should clear state tax data", () => {
      const currentState: LookupState = {
        ...initialState,
        stateTaxData: mockStateTaxData
      };

      const nextState = reducer(currentState, clearStateTaxData());

      expect(nextState.stateTaxData).toBeNull();
    });

    it("should handle clearing already null data", () => {
      const nextState = reducer(initialState, clearStateTaxData());

      expect(nextState.stateTaxData).toBeNull();
    });

    it("should not affect other state properties", () => {
      const currentState: LookupState = {
        ...initialState,
        stateTaxData: mockStateTaxData,
        missives: mockMissives,
        accountingYearData: mockCalendarResponse
      };

      const nextState = reducer(currentState, clearStateTaxData());

      expect(nextState.stateTaxData).toBeNull();
      expect(nextState.missives).toEqual(mockMissives);
      expect(nextState.accountingYearData).toEqual(mockCalendarResponse);
    });
  });

  describe("complex state transitions", () => {
    it("should handle setting all lookup data", () => {
      let state = reducer(initialState, setAccountingYearParams(mockProfitYearRequest));
      state = reducer(state, setAccountingYearData(mockCalendarResponse));
      state = reducer(state, setMissivesData(mockMissives));
      state = reducer(state, setStateTaxData(mockStateTaxData));

      expect(state.accountingYearRequestParams).toEqual(mockProfitYearRequest);
      expect(state.accountingYearData).toEqual(mockCalendarResponse);
      expect(state.missives).toEqual(mockMissives);
      expect(state.stateTaxData).toEqual(mockStateTaxData);
    });

    it("should handle clearing all lookup data", () => {
      const populatedState: LookupState = {
        accountingYearRequestParams: mockProfitYearRequest,
        accountingYearData: mockCalendarResponse,
        missives: mockMissives,
        stateTaxData: mockStateTaxData
      };

      let state = reducer(populatedState, clearAccountingYearRequestParams());
      state = reducer(state, clearAccountingYearData());
      state = reducer(state, setMissivesData([]));
      state = reducer(state, clearStateTaxData());

      expect(state.accountingYearRequestParams).toBeNull();
      expect(state.accountingYearData).toBeNull();
      expect(state.missives).toEqual([]);
      expect(state.stateTaxData).toBeNull();
    });

    it("should handle updating accounting year flow", () => {
      // Set params
      let state = reducer(initialState, setAccountingYearParams({ profitYear: 2023 }));

      // Fetch data
      state = reducer(state, setAccountingYearData(mockCalendarResponse));

      // Update to new year
      state = reducer(state, setAccountingYearParams({ profitYear: 2024 }));
      state = reducer(state, clearAccountingYearData());

      expect(state.accountingYearRequestParams?.profitYear).toBe(2024);
      expect(state.accountingYearData).toBeNull();
    });

    it("should handle state tax lookup for different states", () => {
      const maData: StateTaxLookupResponse = {
        state: "MA",
        stateTaxRate: 0.05
      };
      const nhData: StateTaxLookupResponse = {
        state: "NH",
        stateTaxRate: 0.0
      };

      let state = reducer(initialState, setStateTaxData(maData));
      expect(state.stateTaxData?.state).toBe("MA");

      state = reducer(state, setStateTaxData(nhData));
      expect(state.stateTaxData?.state).toBe("NH");
      expect(state.stateTaxData?.stateTaxRate).toBe(0.0);
    });
  });

  describe("edge cases", () => {
    it("should preserve state immutability", () => {
      const currentState: LookupState = { ...initialState };
      const nextState = reducer(currentState, setAccountingYearData(mockCalendarResponse));

      expect(currentState.accountingYearData).toBeNull();
      expect(nextState.accountingYearData).toEqual(mockCalendarResponse);
    });

    it("should handle multiple updates to same property", () => {
      let state = reducer(initialState, setMissivesData(mockMissives));
      expect(state.missives).toHaveLength(2);

      state = reducer(state, setMissivesData([mockMissives[0]]));
      expect(state.missives).toHaveLength(1);

      state = reducer(state, setMissivesData([]));
      expect(state.missives).toHaveLength(0);
    });

    it("should handle very old profit years", () => {
      const oldYear: ProfitYearRequest = { profitYear: 1990 };
      const nextState = reducer(initialState, setAccountingYearParams(oldYear));

      expect(nextState.accountingYearRequestParams?.profitYear).toBe(1990);
    });

    it("should handle future profit years", () => {
      const futureYear: ProfitYearRequest = { profitYear: 2030 };
      const nextState = reducer(initialState, setAccountingYearParams(futureYear));

      expect(nextState.accountingYearRequestParams?.profitYear).toBe(2030);
    });

    it("should handle clearing data without setting it first", () => {
      let state = reducer(initialState, clearAccountingYearData());
      state = reducer(state, clearStateTaxData());
      state = reducer(state, clearAccountingYearRequestParams());

      expect(state).toEqual(initialState);
    });

    it("should handle setting same data multiple times", () => {
      let state = reducer(initialState, setMissivesData(mockMissives));
      state = reducer(state, setMissivesData(mockMissives));
      state = reducer(state, setMissivesData(mockMissives));

      expect(state.missives).toEqual(mockMissives);
    });
  });
});
