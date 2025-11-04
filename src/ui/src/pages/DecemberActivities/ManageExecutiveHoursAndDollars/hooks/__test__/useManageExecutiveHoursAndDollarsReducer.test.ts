import { describe, expect, it } from "vitest";
import type { ExecutiveHoursAndDollars, PagedReportResponse } from "reduxstore/types";
import {
  initialState,
  manageExecutiveHoursAndDollarsReducer,
  selectCombinedGridData,
  selectHasPendingChanges,
  selectIsRowStagedToSave,
  selectShowGrid,
  selectShowModal,
  type ManageExecutiveHoursAndDollarsState
} from "../useManageExecutiveHoursAndDollarsReducer";

describe("useManageExecutiveHoursAndDollarsReducer", () => {
  describe("initialState", () => {
    it("should have correct initial state", () => {
      expect(initialState.search.params).toBeNull();
      expect(initialState.search.results).toBeNull();
      expect(initialState.search.isSearching).toBe(false);
      expect(initialState.search.error).toBeNull();
      expect(initialState.search.initialLoaded).toBe(false);
      expect(initialState.grid.data).toBeNull();
      expect(initialState.grid.pendingChanges).toBeNull();
      expect(initialState.grid.additionalExecutives).toEqual([]);
      expect(initialState.modal.isOpen).toBe(false);
      expect(initialState.view.mode).toBe("idle");
    });
  });

  describe("SEARCH_START", () => {
    it("should set searching state", () => {
      const params = {
        profitYear: 2024,
        hasExecutiveHoursAndDollars: false,
        isMonthlyPayroll: false,
        pagination: { skip: 0, take: 25, sortBy: "name", isSortDescending: false }
      };
      const action = { type: "SEARCH_START" as const, payload: { params } };
      const newState = manageExecutiveHoursAndDollarsReducer(initialState, action);

      expect(newState.search.params).toEqual(params);
      expect(newState.search.isSearching).toBe(true);
      expect(newState.search.error).toBeNull();
      expect(newState.view.mode).toBe("searching");
    });
  });

  describe("SEARCH_SUCCESS", () => {
    it("should set results and grid data", () => {
      const mockPagedResponse = {
        results: [{ badgeNumber: 12345, fullName: "John Doe", hoursExecutive: 40, incomeExecutive: 1000 } as unknown as ExecutiveHoursAndDollars],
        total: 1
      };

      const action = { type: "SEARCH_SUCCESS" as const, payload: { results: { response: mockPagedResponse } as unknown as PagedReportResponse<ExecutiveHoursAndDollars> } };
      const newState = manageExecutiveHoursAndDollarsReducer(initialState, action);

      expect(newState.search.results).toEqual(mockPagedResponse);
      expect(newState.search.isSearching).toBe(false);
      expect(newState.search.initialLoaded).toBe(true);
      expect(newState.view.mode).toBe("results");
    });

    it("should handle empty results", () => {
      const mockPagedResponse = {
        results: [],
        total: 0
      };

      const action = { type: "SEARCH_SUCCESS" as const, payload: { results: { response: mockPagedResponse } as unknown as PagedReportResponse<ExecutiveHoursAndDollars> } };
      const newState = manageExecutiveHoursAndDollarsReducer(initialState, action);

      expect(newState.search.results).toEqual(mockPagedResponse);
    });
  });

  describe("SEARCH_FAILURE", () => {
    it("should set error and stop searching", () => {
      const action = { type: "SEARCH_FAILURE" as const, payload: { error: "Network error" } };
      const newState = manageExecutiveHoursAndDollarsReducer(initialState, action);

      expect(newState.search.isSearching).toBe(false);
      expect(newState.search.error).toBe("Network error");
      expect(newState.view.mode).toBe("idle");
    });
  });

  describe("SEARCH_RESET", () => {
    it("should reset search and grid state", () => {
      const stateWithData: ManageExecutiveHoursAndDollarsState = {
        ...initialState,
        search: {
          params: {
            profitYear: 2024,
            hasExecutiveHoursAndDollars: false,
            isMonthlyPayroll: false,
            pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: false }
          },
          results: { results: [], total: 0 },
          isSearching: false,
          error: null,
          initialLoaded: true
        },
        grid: {
          data: {
            reportName: "Test",
            reportDate: "2024-01-01",
            startDate: "2024-01-01",
            endDate: "2024-12-31",
            dataSource: "test",
            response: { results: [], total: 0, totalPages: 0, pageSize: 25, currentPage: 0 }
          },
          pendingChanges: null,
          additionalExecutives: [],
          selectedRows: []
        }
      };

      const action = { type: "SEARCH_RESET" as const };
      const newState = manageExecutiveHoursAndDollarsReducer(stateWithData, action);

      expect(newState.search).toEqual(initialState.search);
      expect(newState.grid).toEqual(initialState.grid);
      expect(newState.view.mode).toBe("idle");
    });
  });

  describe("pending changes", () => {
    const mockExecutive = {
      badgeNumber: 12345,
      fullName: "John Doe",
      executiveHours: 40,
      executiveDollars: 1000
    };

    describe("ADD_PENDING_CHANGE", () => {
      it("should add a new pending change", () => {
        const change = {
          profitYear: 2024,
          executiveHoursAndDollars: [mockExecutive]
        };

        const action = { type: "ADD_PENDING_CHANGE" as const, payload: { change } };
        const newState = manageExecutiveHoursAndDollarsReducer(initialState, action);

        expect(newState.grid.pendingChanges).toEqual(change);
      });

      it("should append to existing pending changes", () => {
        const existingExecutive = {
          badgeNumber: 11111,
          fullName: "Jane Smith",
          executiveHours: 35,
          executiveDollars: 900
        };

        const stateWithChange: ManageExecutiveHoursAndDollarsState = {
          ...initialState,
          grid: {
            ...initialState.grid,
            pendingChanges: {
              profitYear: 2024,
              executiveHoursAndDollars: [existingExecutive]
            }
          }
        };

        const change = {
          profitYear: 2024,
          executiveHoursAndDollars: [mockExecutive]
        };

        const action = { type: "ADD_PENDING_CHANGE" as const, payload: { change } };
        const newState = manageExecutiveHoursAndDollarsReducer(stateWithChange, action);

        expect(newState.grid.pendingChanges?.executiveHoursAndDollars).toHaveLength(2);
        expect(newState.grid.pendingChanges?.executiveHoursAndDollars).toContainEqual(existingExecutive);
        expect(newState.grid.pendingChanges?.executiveHoursAndDollars).toContainEqual(mockExecutive);
      });
    });

    describe("UPDATE_PENDING_CHANGE", () => {
      it("should update an existing pending change", () => {
        const stateWithChange: ManageExecutiveHoursAndDollarsState = {
          ...initialState,
          grid: {
            ...initialState.grid,
            pendingChanges: {
              profitYear: 2024,
              executiveHoursAndDollars: [mockExecutive]
            }
          }
        };

        const updatedExecutive = { ...mockExecutive, executiveHours: 50 };
        const change = {
          profitYear: 2024,
          executiveHoursAndDollars: [updatedExecutive]
        };

        const action = { type: "UPDATE_PENDING_CHANGE" as const, payload: { change } };
        const newState = manageExecutiveHoursAndDollarsReducer(stateWithChange, action);

        expect(newState.grid.pendingChanges?.executiveHoursAndDollars[0].executiveHours).toBe(50);
      });
    });

    describe("REMOVE_PENDING_CHANGE", () => {
      it("should remove a pending change", () => {
        const stateWithChange: ManageExecutiveHoursAndDollarsState = {
          ...initialState,
          grid: {
            ...initialState.grid,
            pendingChanges: {
              profitYear: 2024,
              executiveHoursAndDollars: [mockExecutive]
            }
          }
        };

        const change = {
          profitYear: 2024,
          executiveHoursAndDollars: [mockExecutive]
        };

        const action = { type: "REMOVE_PENDING_CHANGE" as const, payload: { change } };
        const newState = manageExecutiveHoursAndDollarsReducer(stateWithChange, action);

        expect(newState.grid.pendingChanges).toBeNull();
      });

      it("should keep other pending changes when removing one", () => {
        const executive2 = { ...mockExecutive, badgeNumber: 22222 };

        const stateWithChanges: ManageExecutiveHoursAndDollarsState = {
          ...initialState,
          grid: {
            ...initialState.grid,
            pendingChanges: {
              profitYear: 2024,
              executiveHoursAndDollars: [mockExecutive, executive2]
            }
          }
        };

        const change = {
          profitYear: 2024,
          executiveHoursAndDollars: [mockExecutive]
        };

        const action = { type: "REMOVE_PENDING_CHANGE" as const, payload: { change } };
        const newState = manageExecutiveHoursAndDollarsReducer(stateWithChanges, action);

        expect(newState.grid.pendingChanges?.executiveHoursAndDollars).toHaveLength(1);
        expect(newState.grid.pendingChanges?.executiveHoursAndDollars[0].badgeNumber).toBe(22222);
      });
    });

    describe("CLEAR_PENDING_CHANGES", () => {
      it("should clear all pending changes", () => {
        const stateWithChanges: ManageExecutiveHoursAndDollarsState = {
          ...initialState,
          grid: {
            ...initialState.grid,
            pendingChanges: {
              profitYear: 2024,
              executiveHoursAndDollars: [mockExecutive]
            }
          }
        };

        const action = { type: "CLEAR_PENDING_CHANGES" as const };
        const newState = manageExecutiveHoursAndDollarsReducer(stateWithChanges, action);

        expect(newState.grid.pendingChanges).toBeNull();
      });
    });
  });

  describe("additional executives", () => {
    it("should add additional executives", () => {
      const executives = [{ badgeNumber: 12345, fullName: "John Doe", hoursExecutive: 40, incomeExecutive: 1000 } as unknown as ExecutiveHoursAndDollars];

      const action = { type: "ADD_ADDITIONAL_EXECUTIVES" as const, payload: { executives } };
      const newState = manageExecutiveHoursAndDollarsReducer(initialState, action);

      expect(newState.grid.additionalExecutives).toEqual(executives);
    });

    it("should append to existing additional executives", () => {
      const existing = [{ badgeNumber: 11111, fullName: "Jane Smith", hoursExecutive: 35, incomeExecutive: 900 } as unknown as ExecutiveHoursAndDollars];

      const stateWithExecutives: ManageExecutiveHoursAndDollarsState = {
        ...initialState,
        grid: { ...initialState.grid, additionalExecutives: existing }
      };

      const newExecutives = [
        { badgeNumber: 22222, fullName: "Bob Johnson", hoursExecutive: 45, incomeExecutive: 1100 } as unknown as ExecutiveHoursAndDollars
      ];

      const action = { type: "ADD_ADDITIONAL_EXECUTIVES" as const, payload: { executives: newExecutives } };
      const newState = manageExecutiveHoursAndDollarsReducer(stateWithExecutives, action);

      expect(newState.grid.additionalExecutives).toHaveLength(2);
    });

    it("should clear additional executives", () => {
      const stateWithExecutives: ManageExecutiveHoursAndDollarsState = {
        ...initialState,
        grid: {
          ...initialState.grid,
          additionalExecutives: [
            { badgeNumber: 12345, fullName: "John Doe", hoursExecutive: 40, incomeExecutive: 1000 } as unknown as ExecutiveHoursAndDollars
          ]
        }
      };

      const action = { type: "CLEAR_ADDITIONAL_EXECUTIVES" as const };
      const newState = manageExecutiveHoursAndDollarsReducer(stateWithExecutives, action);

      expect(newState.grid.additionalExecutives).toEqual([]);
    });
  });

  describe("modal actions", () => {
    it("should open modal", () => {
      const action = { type: "MODAL_OPEN" as const };
      const newState = manageExecutiveHoursAndDollarsReducer(initialState, action);

      expect(newState.modal.isOpen).toBe(true);
    });

    it("should close modal and reset modal state", () => {
      const stateWithModal: ManageExecutiveHoursAndDollarsState = {
        ...initialState,
        modal: {
          isOpen: true,
          results: {
            reportName: "Test",
            reportDate: "2024-01-01",
            startDate: "2024-01-01",
            endDate: "2024-12-31",
            dataSource: "test",
            response: { results: [], total: 0, totalPages: 0, pageSize: 25, currentPage: 0 }
          },
          selectedExecutives: [{ badgeNumber: 12345, fullName: "Test", storeNumber: 1, socialSecurity: 123456789, hoursExecutive: 40, incomeExecutive: 1000, currentHoursYear: 35, currentIncomeYear: 900, payFrequencyId: 1, payFrequencyName: "Monthly", employmentStatusId: "1", employmentStatusName: "Active" }],
          isSearching: false,
          searchParams: {
            profitYear: 2024,
            hasExecutiveHoursAndDollars: false,
            isMonthlyPayroll: false,
            pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: false }
          }
        }
      };

      const action = { type: "MODAL_CLOSE" as const };
      const newState = manageExecutiveHoursAndDollarsReducer(stateWithModal, action);

      expect(newState.modal.isOpen).toBe(false);
      expect(newState.modal.selectedExecutives).toEqual([]);
      expect(newState.modal.results).toBeNull();
    });

    it("should start modal search", () => {
      const params = {
        profitYear: 2024,
        hasExecutiveHoursAndDollars: false,
        isMonthlyPayroll: false,
        pagination: { skip: 0, take: 25, sortBy: "name", isSortDescending: false }
      };
      const action = { type: "MODAL_SEARCH_START" as const, payload: { params } };
      const newState = manageExecutiveHoursAndDollarsReducer(initialState, action);

      expect(newState.modal.searchParams).toEqual(params);
      expect(newState.modal.isSearching).toBe(true);
    });

    it("should handle modal search success", () => {
      const mockPagedResponse = {
        results: [],
        total: 0
      };

      const action = { type: "MODAL_SEARCH_SUCCESS" as const, payload: { results: { response: mockPagedResponse } as unknown as PagedReportResponse<ExecutiveHoursAndDollars> } };
      const newState = manageExecutiveHoursAndDollarsReducer(initialState, action);

      expect(newState.modal.results).toEqual({ response: mockPagedResponse });
      expect(newState.modal.isSearching).toBe(false);
    });

    it("should handle modal search failure", () => {
      const searchingState: ManageExecutiveHoursAndDollarsState = {
        ...initialState,
        modal: { ...initialState.modal, isSearching: true }
      };

      const action = { type: "MODAL_SEARCH_FAILURE" as const, payload: { error: "error" } };
      const newState = manageExecutiveHoursAndDollarsReducer(searchingState, action);

      expect(newState.modal.isSearching).toBe(false);
    });

    it("should select executives in modal", () => {
      const executives = [{ badgeNumber: 12345, fullName: "John Doe", hoursExecutive: 40, incomeExecutive: 1000 } as unknown as ExecutiveHoursAndDollars];

      const action = { type: "MODAL_SELECT_EXECUTIVES" as const, payload: { executives } };
      const newState = manageExecutiveHoursAndDollarsReducer(initialState, action);

      expect(newState.modal.selectedExecutives).toEqual(executives);
    });

    it("should clear modal selection", () => {
      const stateWithSelection: ManageExecutiveHoursAndDollarsState = {
        ...initialState,
        modal: {
          ...initialState.modal,
          selectedExecutives: [{ badgeNumber: 12345, fullName: "Test", hoursExecutive: 40, incomeExecutive: 1000 } as unknown as ExecutiveHoursAndDollars]
        }
      };

      const action = { type: "MODAL_CLEAR_SELECTION" as const };
      const newState = manageExecutiveHoursAndDollarsReducer(stateWithSelection, action);

      expect(newState.modal.selectedExecutives).toEqual([]);
    });
  });

  describe("SET_VIEW_MODE", () => {
    it("should set view mode", () => {
      const action = { type: "SET_VIEW_MODE" as const, payload: { mode: "results" as const } };
      const newState = manageExecutiveHoursAndDollarsReducer(initialState, action);

      expect(newState.view.mode).toBe("results");
    });
  });

  describe("SET_INITIAL_LOADED", () => {
    it("should set initial loaded flag", () => {
      const action = { type: "SET_INITIAL_LOADED" as const, payload: { loaded: true } };
      const newState = manageExecutiveHoursAndDollarsReducer(initialState, action);

      expect(newState.search.initialLoaded).toBe(true);
    });
  });

  describe("SET_PAGE_RESET", () => {
    it("should set page reset flag", () => {
      const action = { type: "SET_PAGE_RESET" as const, payload: { reset: true } };
      const newState = manageExecutiveHoursAndDollarsReducer(initialState, action);

      expect(newState.view.pageNumberReset).toBe(true);
    });
  });

  describe("RESET_ALL", () => {
    it("should reset to initial state", () => {
      const stateWithData: ManageExecutiveHoursAndDollarsState = {
        search: {
          params: {
            profitYear: 2024,
            hasExecutiveHoursAndDollars: false,
            isMonthlyPayroll: false,
            pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: false }
          },
          results: { results: [], total: 0 },
          isSearching: false,
          error: null,
          initialLoaded: true
        },
        grid: {
          data: {
            reportName: "Test",
            reportDate: "2024-01-01",
            startDate: "2024-01-01",
            endDate: "2024-12-31",
            dataSource: "test",
            response: { results: [], total: 0, totalPages: 0, pageSize: 25, currentPage: 0 }
          },
          pendingChanges: {
            profitYear: 2024,
            executiveHoursAndDollars: [
              { badgeNumber: 12345, executiveHours: 40, executiveDollars: 1000 }
            ]
          },
          additionalExecutives: [],
          selectedRows: []
        },
        modal: {
          isOpen: true,
          results: {
            reportName: "Test",
            reportDate: "2024-01-01",
            startDate: "2024-01-01",
            endDate: "2024-12-31",
            dataSource: "test",
            response: { results: [], total: 0, totalPages: 0, pageSize: 25, currentPage: 0 }
          },
          selectedExecutives: [],
          isSearching: false,
          searchParams: {
            profitYear: 2024,
            hasExecutiveHoursAndDollars: false,
            isMonthlyPayroll: false,
            pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: false }
          }
        },
        view: {
          mode: "results",
          pageNumberReset: true
        }
      };

      const action = { type: "RESET_ALL" as const };
      const newState = manageExecutiveHoursAndDollarsReducer(stateWithData, action);

      expect(newState).toEqual(initialState);
    });
  });

  describe("selectors", () => {
    describe("selectHasPendingChanges", () => {
      it("should return true when there are pending changes", () => {
        const state: ManageExecutiveHoursAndDollarsState = {
          ...initialState,
          grid: {
            ...initialState.grid,
            pendingChanges: {
              profitYear: 2024,
              executiveHoursAndDollars: [
                { badgeNumber: 12345, executiveHours: 40, executiveDollars: 1000 }
              ]
            }
          }
        };

        expect(selectHasPendingChanges(state)).toBe(true);
      });

      it("should return false when there are no pending changes", () => {
        expect(selectHasPendingChanges(initialState)).toBe(false);
      });
    });

    describe("selectIsRowStagedToSave", () => {
      it("should return true when row is staged", () => {
        const state: ManageExecutiveHoursAndDollarsState = {
          ...initialState,
          grid: {
            ...initialState.grid,
            pendingChanges: {
              profitYear: 2024,
              executiveHoursAndDollars: [
                { badgeNumber: 12345, executiveHours: 40, executiveDollars: 1000 }
              ]
            }
          }
        };

        const isStaged = selectIsRowStagedToSave(state);
        expect(isStaged(12345)).toBe(true);
      });

      it("should return false when row is not staged", () => {
        const isStaged = selectIsRowStagedToSave(initialState);
        expect(isStaged(12345)).toBe(false);
      });
    });

    describe("selectShowGrid", () => {
      it("should return true when in results mode with data", () => {
        const state: ManageExecutiveHoursAndDollarsState = {
          ...initialState,
          search: {
            ...initialState.search,
            results: {
              results: [{ badgeNumber: 12345, fullName: "Test", storeNumber: 1, socialSecurity: 123456789, hoursExecutive: 40, incomeExecutive: 1000, currentHoursYear: 35, currentIncomeYear: 900, payFrequencyId: 1, payFrequencyName: "Monthly", employmentStatusId: "1", employmentStatusName: "Active" }],
              total: 1
            }
          },
          view: { mode: "results", pageNumberReset: false }
        };

        expect(selectShowGrid(state)).toBe(true);
      });

      it("should return false when not in results mode", () => {
        expect(selectShowGrid(initialState)).toBe(false);
      });
    });

    describe("selectShowModal", () => {
      it("should return true when modal is open", () => {
        const state: ManageExecutiveHoursAndDollarsState = {
          ...initialState,
          modal: { ...initialState.modal, isOpen: true }
        };

        expect(selectShowModal(state)).toBe(true);
      });

      it("should return false when modal is closed", () => {
        expect(selectShowModal(initialState)).toBe(false);
      });
    });

    describe("selectCombinedGridData", () => {
      it("should return main data when no additional executives", () => {
        const mainData = {
          reportName: "Test",
          reportDate: "2024-01-01",
          startDate: "2024-01-01",
          endDate: "2024-12-31",
          dataSource: "test",
          response: {
            results: [{ badgeNumber: 12345, fullName: "John", hoursExecutive: 40, incomeExecutive: 1000 } as unknown as ExecutiveHoursAndDollars],
            total: 1,
            totalPages: 1,
            pageSize: 25,
            currentPage: 0
          }
        } as unknown as PagedReportResponse<ExecutiveHoursAndDollars>;

        const state: ManageExecutiveHoursAndDollarsState = {
          ...initialState,
          grid: { ...initialState.grid, data: mainData }
        };

        const result = selectCombinedGridData(state);
        expect(result).toEqual(mainData);
      });

      it("should combine main data with additional executives", () => {
        const mainData = {
          reportName: "Test",
          reportDate: "2024-01-01",
          startDate: "2024-01-01",
          endDate: "2024-12-31",
          dataSource: "test",
          response: {
            results: [{ badgeNumber: 12345, fullName: "John", hoursExecutive: 40, incomeExecutive: 1000 } as unknown as ExecutiveHoursAndDollars],
            total: 1,
            totalPages: 1,
            pageSize: 25,
            currentPage: 0
          }
        } as unknown as PagedReportResponse<ExecutiveHoursAndDollars>;

        const additional = [{ badgeNumber: 22222, fullName: "Jane", hoursExecutive: 35, incomeExecutive: 900 } as unknown as ExecutiveHoursAndDollars];

        const state: ManageExecutiveHoursAndDollarsState = {
          ...initialState,
          grid: { ...initialState.grid, data: mainData, additionalExecutives: additional }
        };

        const result = selectCombinedGridData(state);
        expect(result?.response.results).toHaveLength(2);
      });

      it("should filter out duplicate badge numbers", () => {
        const mainData = {
          reportName: "Test",
          reportDate: "2024-01-01",
          startDate: "2024-01-01",
          endDate: "2024-12-31",
          dataSource: "test",
          response: {
            results: [{ badgeNumber: 12345, fullName: "John", hoursExecutive: 40, incomeExecutive: 1000 } as unknown as ExecutiveHoursAndDollars],
            total: 1,
            totalPages: 1,
            pageSize: 25,
            currentPage: 0
          }
        } as unknown as PagedReportResponse<ExecutiveHoursAndDollars>;

        const additional = [
          { badgeNumber: 12345, fullName: "John Updated", hoursExecutive: 45, incomeExecutive: 1100 } as unknown as ExecutiveHoursAndDollars
        ];

        const state: ManageExecutiveHoursAndDollarsState = {
          ...initialState,
          grid: { ...initialState.grid, data: mainData, additionalExecutives: additional }
        };

        const result = selectCombinedGridData(state);
        expect(result?.response.results).toHaveLength(1);
        expect(result?.response.results[0].fullName).toBe("John");
      });

      it("should return null when no main data", () => {
        const result = selectCombinedGridData(initialState);
        expect(result).toBeNull();
      });
    });
  });
});
