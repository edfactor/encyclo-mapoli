import { Box, CircularProgress, Typography } from "@mui/material";
import React, { useEffect, useMemo, useRef, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { DSMGrid, Pagination } from "smart-ui-library";
import { useDynamicGridHeight } from "../../../hooks/useDynamicGridHeight";
import { useGridPagination } from "../../../hooks/useGridPagination";
import {
  PayServicesApi,
  useLazyGetFullTimeAccruedPaidHolidaysPayServicesQuery,
  useLazyGetFullTimeEightPaidHolidaysPayServicesQuery,
  useLazyGetFullTimeStraightSalaryPayServicesQuery,
  useLazyGetPartTimePayServicesQuery
} from "../../../reduxstore/api/PayServicesApi";
import { RootState } from "../../../reduxstore/store";
import { QPAY600FilterParams } from "./QPAY600FilterSection";
import { GetQPAY600GridColumns } from "./QPAY600GridColumns";

interface QPAY600GridProps {
  filterParams: QPAY600FilterParams;
  onLoadingChange?: (isLoading: boolean) => void;
}

const QPAY600Grid: React.FC<QPAY600GridProps> = ({ filterParams, onLoadingChange }) => {
  const dispatch = useDispatch();
  const hasToken = useSelector((state: RootState) => !!state.security.token);
  const [hasSearchRun, setHasSearchRun] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const lastSearchRef = useRef<string>("");

  // Use lazy query hooks
  const [triggerPartTime, partTimeResult] = useLazyGetPartTimePayServicesQuery();
  const [triggerFullTimeAccruedHolidays, fullTimeAccruedResult] =
    useLazyGetFullTimeAccruedPaidHolidaysPayServicesQuery();
  const [triggerFullTimeEightPaidHolidays, fullTimeEightPaidHolidaysResult] =
    useLazyGetFullTimeEightPaidHolidaysPayServicesQuery();
  const [triggerFullTimeStraightSalary, fullTimeStraightSalaryResult] =
    useLazyGetFullTimeStraightSalaryPayServicesQuery();

  // Determine which result to use based on employee type
  const currentResult =
    filterParams.employeeType === "parttime"
      ? partTimeResult
      : filterParams.employeeType === "fulltimesalaried"
      ? fullTimeStraightSalaryResult
      : filterParams.employeeType === "fulltimehourlyaccrued"
      ? fullTimeAccruedResult
      : fullTimeEightPaidHolidaysResult;

  const { data: apiData, isFetching, error, isError } = currentResult;

  // Use dynamic grid height utility hook
  const gridMaxHeight = useDynamicGridHeight();

  const { pageNumber, pageSize, handlePaginationChange } = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "employeeName",
    initialSortDescending: false,
    onPaginationChange: () => {
      // Pagination handled by backend API
    }
  });

  useEffect(() => {
    onLoadingChange?.(isFetching);
  }, [isFetching, onLoadingChange]);

  // Trigger search when filter params change
  useEffect(() => {
    if (hasToken && filterParams.profitYear && filterParams.employeeType) {
      const profitYear = filterParams.profitYear.getFullYear();
      const employeeType = filterParams.employeeType;
      
      // Create a unique key to prevent double calls
      const searchKey = `${employeeType}-${profitYear}`;
      
      // Skip if this is the same search we just ran
      if (lastSearchRef.current === searchKey) {
        return;
      }
      
      lastSearchRef.current = searchKey;
      
      setErrorMessage(null);
      setHasSearchRun(true);

      let cancelled = false;

      const triggerSearch = async () => {
        try {
          let result;
          
          // Determine which tag to invalidate and which query to trigger
          switch (employeeType) {
            case "parttime":
              dispatch(PayServicesApi.util.invalidateTags(["PayServicesPartTime"]));
              result = await triggerPartTime({ profitYear }, false);
              break;
            case "fulltimesalaried":
              dispatch(PayServicesApi.util.invalidateTags(["PayServicesFullTimeSalary"]));
              result = await triggerFullTimeStraightSalary({ profitYear }, false);
              break;
            case "fulltimehourlyaccrued":
              dispatch(PayServicesApi.util.invalidateTags(["PayServicesFullTimeAccruedHolidays"]));
              result = await triggerFullTimeAccruedHolidays({ profitYear }, false);
              break;
            case "fulltimehourlyearned":
              dispatch(PayServicesApi.util.invalidateTags(["PayServicesFullTimeEightHolidays"]));
              result = await triggerFullTimeEightPaidHolidays({ profitYear }, false);
              break;
            default:
              console.error(`Unknown employee type: ${employeeType}`);
              if (!cancelled) {
                setErrorMessage("Invalid employee type selected.");
              }
              return;
          }

          if (!cancelled && result?.error) {
            console.error("QPAY600 search failed:", result.error);
            setErrorMessage("Failed to load pay services data. Please try again.");
          }
        } catch (error) {
          console.error("QPAY600 search error:", error);
          if (!cancelled) {
            setErrorMessage("An unexpected error occurred. Please try again.");
          }
        }
      };

      triggerSearch();

      return () => {
        cancelled = true;
      };
    }
    // Note: Trigger functions are stable references from RTK Query hooks and
    // intentionally excluded from dependencies to prevent unnecessary re-runs
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [
    hasToken,
    filterParams.profitYear?.getTime(), // Use timestamp to ensure change detection
    filterParams.employeeType,
    dispatch
  ]);

  const columnDefs = useMemo(() => GetQPAY600GridColumns(), []);

  const data = apiData || {
    payServicesForYear: { results: [], total: 0 },
    totalEmployeeNumber: 0,
    totalEmployeesWages: 0
  };

  const getTitle = () => {
    const titles = {
      parttime: "Part Time Hourly",
      fulltimesalaried: "Full Time Salaried",
      fulltimehourlyaccrued: "Full Time Hourly Accrued Holidays",
      fulltimehourlyearned: "Full Time Hourly Earned Holidays"
    };

    return `Employees - ${titles[filterParams.employeeType as keyof typeof titles] || filterParams.employeeType}`;
  };

  const pinnedBottomRowData = useMemo(() => {
    if (!apiData) return [];

    return [
      {
        yearsOfServiceLabel: "Total",
        employees: apiData.totalEmployeeNumber ?? 0,
        weeklyPay: apiData.totalWeeklyPay ?? null,
        yearsWages: apiData.totalEmployeesWages ?? 0,
        _isTotal: true
      }
    ];
  }, [apiData]);

  return (
    <div className="relative">
      <Box sx={{ padding: "0 24px 0 24px" }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          {getTitle()}
        </Typography>
      </Box>

      {errorMessage && (
        <Box sx={{ padding: "24px", color: "error.main" }}>
          <Typography color="error">{errorMessage}</Typography>
        </Box>
      )}

      {isFetching ? (
        <Box
          display="flex"
          justifyContent="center"
          alignItems="center"
          py={4}>
          <CircularProgress />
        </Box>
      ) : hasSearchRun && !errorMessage ? (
        <>
          <DSMGrid
            preferenceKey={`QPAY600_${filterParams.employeeType.toUpperCase()}`}
            isLoading={isFetching}
            maxHeight={gridMaxHeight}
            providedOptions={{
              rowData: data.payServicesForYear?.results || [],
              columnDefs: columnDefs,
              pinnedBottomRowData: pinnedBottomRowData
            }}
          />
          {!!data.payServicesForYear && data.payServicesForYear.results && data.payServicesForYear.results.length > 0 && (
            <Pagination
              pageNumber={pageNumber}
              setPageNumber={(value: number) => handlePaginationChange(value - 1, pageSize)}
              pageSize={pageSize}
              setPageSize={(value: number) => handlePaginationChange(0, value)}
              recordCount={data.payServicesForYear.total || 0}
            />
          )}
        </>
      ) : null}
    </div>
  );
};

export default QPAY600Grid;
