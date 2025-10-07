import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useCallback, useEffect, useMemo, useReducer, useRef } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  useLazyGetAdditionalExecutivesQuery,
  useLazyGetExecutiveHoursAndDollarsQuery,
  useUpdateExecutiveHoursAndDollarsMutation
} from "reduxstore/api/YearsEndApi";
import {
  addExecutiveHoursAndDollarsGridRow,
  clearExecutiveHoursAndDollarsGridRows,
  removeExecutiveHoursAndDollarsGridRow,
  setExecutiveHoursAndDollarsGridYear,
  updateExecutiveHoursAndDollarsGridRow
} from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { ExecutiveHoursAndDollars, ExecutiveHoursAndDollarsGrid } from "reduxstore/types";
import { ISortParams } from "smart-ui-library";
import { ExecutiveHoursAndDollarsRequestDto } from "types/fiscal/executive";
import { useGridPagination } from "../../../../hooks/useGridPagination";
import {
  initialState,
  manageExecutiveHoursAndDollarsReducer,
  selectCombinedGridData,
  selectHasPendingChanges,
  selectIsRowStagedToSave,
  selectShowGrid,
  selectShowModal
} from "./useManageExecutiveHoursAndDollarsReducer";

const useManageExecutiveHoursAndDollars = () => {
  const [state, dispatch] = useReducer(manageExecutiveHoursAndDollarsReducer, initialState);
  const reduxDispatch = useDispatch();
  const profitYear = useFiscalCloseProfitYear();

  const [triggerSearch, { isLoading: isSearching }] = useLazyGetExecutiveHoursAndDollarsQuery();
  const [triggerModalSearch, { isLoading: isModalSearching }] = useLazyGetAdditionalExecutivesQuery();
  const [updateHoursAndDollars] = useUpdateExecutiveHoursAndDollarsMutation();

  const { executiveHoursAndDollarsGrid } = useSelector((state: RootState) => state.yearsEnd);

  const searchParamsRef = useRef(state.search.params);
  const modalSearchParamsRef = useRef(state.modal.searchParams);

  useEffect(() => {
    searchParamsRef.current = state.search.params;
  }, [state.search.params]);

  useEffect(() => {
    modalSearchParamsRef.current = state.modal.searchParams;
  }, [state.modal.searchParams]);

  const handleMainGridPaginationChange = useCallback(
    (pageNumber: number, pageSize: number, sortParams: ISortParams) => {
      const currentSearchParams = searchParamsRef.current;
      if (currentSearchParams) {
        const updatedParams: ExecutiveHoursAndDollarsRequestDto = {
          ...currentSearchParams,
          pagination: {
            skip: pageNumber * pageSize,
            take: pageSize,
            sortBy: sortParams.sortBy,
            isSortDescending: sortParams.isSortDescending
          }
        };

        dispatch({ type: "SEARCH_START", payload: { params: updatedParams } });

        triggerSearch(updatedParams)
          .unwrap()
          .then((response) => {
            dispatch({ type: "SEARCH_SUCCESS", payload: { results: response } });
          })
          .catch((error) => {
            dispatch({ type: "SEARCH_FAILURE", payload: { error: error?.toString() || "Search failed" } });
          });
      }
    },
    [triggerSearch]
  );

  const handleModalGridPaginationChange = useCallback(
    (pageNumber: number, pageSize: number, sortParams: ISortParams) => {
      const currentModalSearchParams = modalSearchParamsRef.current;
      if (currentModalSearchParams) {
        const updatedParams: ExecutiveHoursAndDollarsRequestDto = {
          ...currentModalSearchParams,
          pagination: {
            skip: pageNumber * pageSize,
            take: pageSize,
            sortBy: sortParams.sortBy,
            isSortDescending: sortParams.isSortDescending
          }
        };

        dispatch({ type: "MODAL_SEARCH_START", payload: { params: updatedParams } });

        triggerModalSearch(updatedParams)
          .unwrap()
          .then((response) => {
            dispatch({ type: "MODAL_SEARCH_SUCCESS", payload: { results: response } });
          })
          .catch((error) => {
            dispatch({ type: "MODAL_SEARCH_FAILURE", payload: { error: error?.toString() || "Modal search failed" } });
          });
      }
    },
    [triggerModalSearch]
  );

  const mainGridPagination = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "storeNumber",
    initialSortDescending: false,
    onPaginationChange: handleMainGridPaginationChange
  });

  const modalGridPagination = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "storeNumber",
    initialSortDescending: false,
    onPaginationChange: handleModalGridPaginationChange
  });

  const executeSearch = useCallback(
    async (searchForm: any) => {
      const searchParams: ExecutiveHoursAndDollarsRequestDto = {
        profitYear: profitYear || 0,
        ...(searchForm.badgeNumber && { badgeNumber: searchForm.badgeNumber }),
        ...(searchForm.socialSecurity && { socialSecurity: Number(searchForm.socialSecurity) }),
        ...(searchForm.fullNameContains && { fullNameContains: searchForm.fullNameContains }),
        hasExecutiveHoursAndDollars: searchForm.hasExecutiveHoursAndDollars ?? false,
        isMonthlyPayroll: searchForm.isMonthlyPayroll ?? false,
        pagination: {
          skip: 0,
          take: mainGridPagination.pageSize,
          sortBy: mainGridPagination.sortParams.sortBy,
          isSortDescending: mainGridPagination.sortParams.isSortDescending
        }
      };

      try {
        dispatch({ type: "SEARCH_START", payload: { params: searchParams } });

        const response = await triggerSearch(searchParams).unwrap();
        dispatch({ type: "SEARCH_SUCCESS", payload: { results: response } });

        reduxDispatch(setExecutiveHoursAndDollarsGridYear(profitYear));
        dispatch({ type: "CLEAR_ADDITIONAL_EXECUTIVES" });
      } catch (error) {
        dispatch({ type: "SEARCH_FAILURE", payload: { error: error?.toString() || "Search failed" } });
      }
    },
    [triggerSearch, profitYear, reduxDispatch, mainGridPagination]
  );

  const executeModalSearch = useCallback(
    async (searchForm: any) => {
      const searchParams: ExecutiveHoursAndDollarsRequestDto = {
        profitYear: profitYear || 0,
        ...(searchForm.badgeNumber && { badgeNumber: searchForm.badgeNumber }),
        ...(searchForm.socialSecurity && { socialSecurity: Number(searchForm.socialSecurity) }),
        ...(searchForm.fullNameContains && { fullNameContains: searchForm.fullNameContains }),
        hasExecutiveHoursAndDollars: false,
        isMonthlyPayroll: searchForm.isMonthlyPayroll ?? false,
        pagination: {
          skip: 0,
          take: modalGridPagination.pageSize,
          sortBy: modalGridPagination.sortParams.sortBy,
          isSortDescending: modalGridPagination.sortParams.isSortDescending
        }
      };

      try {
        dispatch({ type: "MODAL_SEARCH_START", payload: { params: searchParams } });

        const response = await triggerModalSearch(searchParams).unwrap();
        dispatch({ type: "MODAL_SEARCH_SUCCESS", payload: { results: response } });
      } catch (error) {
        dispatch({ type: "MODAL_SEARCH_FAILURE", payload: { error: error?.toString() || "Modal search failed" } });
      }
    },
    [triggerModalSearch, profitYear, modalGridPagination]
  );

  const resetSearch = useCallback(() => {
    dispatch({ type: "SEARCH_RESET" });
    mainGridPagination.resetPagination();
    modalGridPagination.resetPagination();
  }, [mainGridPagination, modalGridPagination]);

  const openModal = useCallback(() => {
    dispatch({ type: "MODAL_OPEN" });
    dispatch({ type: "MODAL_CLEAR_SELECTION" });
  }, []);

  const closeModal = useCallback(() => {
    dispatch({ type: "MODAL_CLOSE" });
    modalGridPagination.resetPagination();
  }, [modalGridPagination]);

  const selectExecutivesInModal = useCallback((executives: ExecutiveHoursAndDollars[]) => {
    dispatch({ type: "MODAL_SELECT_EXECUTIVES", payload: { executives } });
  }, []);

  const addExecutivesToMainGrid = useCallback(() => {
    if (state.modal.selectedExecutives.length > 0) {
      dispatch({ type: "ADD_ADDITIONAL_EXECUTIVES", payload: { executives: state.modal.selectedExecutives } });
      dispatch({ type: "MODAL_CLOSE" });
    }
  }, [state.modal.selectedExecutives]);

  const updateExecutiveRow = useCallback(
    (badgeNumber: number, hours: number | string, dollars: number | string) => {
      // Convert to numbers in case they come as strings from the grid
      const numericHours = typeof hours === "string" ? parseFloat(hours) : hours;
      const numericDollars = typeof dollars === "string" ? parseFloat(dollars) : dollars;

      const rowRecord: ExecutiveHoursAndDollarsGrid = {
        executiveHoursAndDollars: [
          {
            badgeNumber,
            executiveHours: numericHours,
            executiveDollars: numericDollars
          }
        ],
        profitYear: profitYear || null
      };

      const isRowStagedToSave = selectIsRowStagedToSave(state);

      if (isRowStagedToSave(badgeNumber)) {
        const combinedData = selectCombinedGridData(state);
        const originalRow = combinedData?.response.results.find((obj) => obj.badgeNumber === badgeNumber);

        if (
          originalRow &&
          numericHours === originalRow.hoursExecutive &&
          numericDollars === originalRow.incomeExecutive
        ) {
          dispatch({ type: "REMOVE_PENDING_CHANGE", payload: { change: rowRecord } });
          reduxDispatch(removeExecutiveHoursAndDollarsGridRow(rowRecord));
        } else {
          dispatch({ type: "UPDATE_PENDING_CHANGE", payload: { change: rowRecord } });
          reduxDispatch(updateExecutiveHoursAndDollarsGridRow(rowRecord));
        }
      } else {
        dispatch({ type: "ADD_PENDING_CHANGE", payload: { change: rowRecord } });
        reduxDispatch(addExecutiveHoursAndDollarsGridRow(rowRecord));
      }
    },
    [state, profitYear, reduxDispatch]
  );

  const saveChanges = useCallback(async () => {
    if (state.grid.pendingChanges && executiveHoursAndDollarsGrid) {
      try {
        await updateHoursAndDollars(executiveHoursAndDollarsGrid).unwrap();
        dispatch({ type: "CLEAR_PENDING_CHANGES" });
        reduxDispatch(clearExecutiveHoursAndDollarsGridRows());
      } catch (error) {
        console.error("ERROR: Did not update hours and dollars", error);
      }
    }
  }, [state.grid.pendingChanges, executiveHoursAndDollarsGrid, updateHoursAndDollars, reduxDispatch]);

  const saveExecutiveHoursAndDollars = useCallback(async () => {
    if (state.search.params) {
      try {
        await triggerSearch({
          ...state.search.params,
          archive: true
        }).unwrap();
      } catch (error) {
        console.error("Error saving executive hours and dollars:", error);
      }
    }
  }, [state.search.params, triggerSearch]);

  // Remove this useEffect as it causes circular pagination changes
  // useEffect(() => {
  //   if (state.view.pageNumberReset) {
  //     mainGridPagination.resetPagination();
  //     dispatch({ type: "SET_PAGE_RESET", payload: { reset: false } });
  //   }
  // }, [state.view.pageNumberReset, mainGridPagination]);

  const combinedGridData = useMemo(() => selectCombinedGridData(state), [state]);
  const hasPendingChanges = useMemo(() => selectHasPendingChanges(state), [state]);
  const showGrid = useMemo(() => selectShowGrid(state), [state]);
  const showModal = useMemo(() => selectShowModal(state), [state]);
  const isRowStagedToSave = useMemo(() => selectIsRowStagedToSave(state), [state]);

  return {
    profitYear, // Expose profit year for components to use (e.g., frozen year warnings)
    searchParams: state.search.params,
    searchResults: state.search.results,
    isSearching: isSearching || state.search.isSearching,
    gridData: combinedGridData,
    hasPendingChanges,
    showGrid,
    isRowStagedToSave,

    isModalOpen: showModal,
    modalResults: state.modal.results,
    modalSelectedExecutives: state.modal.selectedExecutives,
    isModalSearching: isModalSearching || state.modal.isSearching,

    mainGridPagination,
    modalGridPagination,

    executeSearch,
    executeModalSearch,
    resetSearch,
    openModal,
    closeModal,
    selectExecutivesInModal,
    addExecutivesToMainGrid,
    updateExecutiveRow,
    saveChanges,
    saveExecutiveHoursAndDollars,

    initialSearchLoaded: state.search.initialLoaded,
    setInitialSearchLoaded: (loaded: boolean) => dispatch({ type: "SET_INITIAL_LOADED", payload: { loaded } }),
    pageNumberReset: state.view.pageNumberReset,
    setPageNumberReset: (reset: boolean) => dispatch({ type: "SET_PAGE_RESET", payload: { reset } })
  };
};

export default useManageExecutiveHoursAndDollars;
