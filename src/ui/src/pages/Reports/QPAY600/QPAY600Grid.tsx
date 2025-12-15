import { Box, CircularProgress, Typography } from "@mui/material";
import React, { useEffect, useMemo, useState } from "react";
import { DSMGrid, Pagination } from "smart-ui-library";
import { useContentAwareGridHeight } from "../../../hooks/useContentAwareGridHeight";
import { GRID_KEYS } from "../../../constants";
import { useGridPagination } from "../../../hooks/useGridPagination";
import {
  useGetFullTimeAccruedPaidHolidaysPayServicesQuery,
  useGetFullTimeEightPaidHolidaysPayServicesQuery,
  useGetFullTimeStraightSalaryPayServicesQuery,
  useGetPartTimePayServicesQuery
} from "../../../reduxstore/api/PayServicesApi";
import { GetQPAY600GridColumns } from "./QPAY600GridColumns";
import { QPAY600FilterParams } from "./QPAY600SearchFilter";

interface QPAY600GridProps {
  filterParams: QPAY600FilterParams;
  onLoadingChange?: (isLoading: boolean) => void;
}

const QPAY600Grid: React.FC<QPAY600GridProps> = ({ filterParams, onLoadingChange }) => {
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const profitYear = filterParams.profitYear?.getFullYear() || 0;

  // Use regular query hooks with skip option - they maintain cache across remounts
  const partTimeResult = useGetPartTimePayServicesQuery(
    { profitYear },
    { skip: filterParams.employeeType !== "parttime" || !profitYear }
  );
  const fullTimeAccruedResult = useGetFullTimeAccruedPaidHolidaysPayServicesQuery(
    { profitYear },
    { skip: filterParams.employeeType !== "fulltimehourlyaccrued" || !profitYear }
  );
  const fullTimeEightPaidHolidaysResult = useGetFullTimeEightPaidHolidaysPayServicesQuery(
    { profitYear },
    { skip: filterParams.employeeType !== "fulltimehourlyearned" || !profitYear }
  );
  const fullTimeStraightSalaryResult = useGetFullTimeStraightSalaryPayServicesQuery(
    { profitYear },
    { skip: filterParams.employeeType !== "fulltimesalaried" || !profitYear }
  );

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

  // Use content-aware grid height utility hook
  const gridMaxHeight = useContentAwareGridHeight({
    rowCount: apiData?.payServicesForYear?.results?.length ?? 0
  });

  const { pageNumber, pageSize, handlePaginationChange } = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "employeeName",
    initialSortDescending: false,
    persistenceKey: GRID_KEYS.QPAY600,
    onPaginationChange: () => {
      // Pagination handled by backend API
    }
  });

  useEffect(() => {
    onLoadingChange?.(isFetching);
  }, [isFetching, onLoadingChange]);

  // Handle API errors
  useEffect(() => {
    if (isError && error) {
      console.error("QPAY600 API error:", error);
      setErrorMessage("Failed to load pay services data. Please try again.");
    } else if (!isFetching && apiData) {
      setErrorMessage(null);
    }
  }, [isError, error, isFetching, apiData]);

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
      ) : apiData && !errorMessage ? (
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
          {!!data.payServicesForYear &&
            data.payServicesForYear.results &&
            data.payServicesForYear.results.length > 0 && (
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
