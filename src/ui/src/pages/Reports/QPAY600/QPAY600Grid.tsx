import { Box, CircularProgress, Typography } from "@mui/material";
import React, { useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { DSMGrid, Pagination } from "smart-ui-library";
import { useDynamicGridHeight } from "../../../hooks/useDynamicGridHeight";
import { useGridPagination } from "../../../hooks/useGridPagination";
import { RootState } from "../../../reduxstore/store";
import { QPAY600FilterParams } from "./QPAY600FilterSection";
import { GetQPAY600GridColumns } from "./QPAY600GridColumns";

interface QPAY600GridProps {
  filterParams: QPAY600FilterParams;
  employeeStatus: "Full time" | "Part time";
  onLoadingChange?: (isLoading: boolean) => void;
}

const QPAY600Grid: React.FC<QPAY600GridProps> = ({ filterParams, employeeStatus, onLoadingChange }) => {
  const hasToken = useSelector((state: RootState) => !!state.security.token);
  const [isFetching, setIsFetching] = useState(false);

  // Use dynamic grid height utility hook
  const gridMaxHeight = useDynamicGridHeight();

  const { pageNumber, pageSize, handlePaginationChange, handleSortChange } = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "employeeName",
    initialSortDescending: false,
    onPaginationChange: () => {
      // This component uses mock data, no API calls needed
    }
  });

  const mockData = useMemo(() => {
    return {
      fullTime: {
        results: [
          {
            yearsOfService: 5,
            employees: 1250,
            totalWeeklyPay: 87500.0,
            lastYearWages: 2450000.0
          },
          {
            yearsOfService: 3,
            employees: 980,
            totalWeeklyPay: 68600.0,
            lastYearWages: 1940000.0
          },
          {
            yearsOfService: 1,
            employees: 750,
            totalWeeklyPay: 52500.0,
            lastYearWages: 1500000.0
          }
        ],
        total: 3,
        totalEmployees: 2980,
        totalWeeklyPay: 208600.0,
        totalLastYearWages: 5890000.0
      },
      partTime: {
        results: [
          {
            yearsOfService: 2,
            employees: 650,
            totalWeeklyPay: 19500.0,
            lastYearWages: 975000.0
          },
          {
            yearsOfService: 4,
            employees: 420,
            totalWeeklyPay: 12600.0,
            lastYearWages: 630000.0
          },
          {
            yearsOfService: 1,
            employees: 380,
            totalWeeklyPay: 11400.0,
            lastYearWages: 570000.0
          }
        ],
        total: 3,
        totalEmployees: 1450,
        totalWeeklyPay: 43500.0,
        totalLastYearWages: 2175000.0
      }
    };
  }, []);

  useEffect(() => {
    onLoadingChange?.(isFetching);
  }, [isFetching, onLoadingChange]);

  useEffect(() => {
    if (hasToken && filterParams && (filterParams.startDate || filterParams.endDate)) {
      setIsFetching(true);
      const timer = setTimeout(() => {
        setIsFetching(false);
      }, 500);

      return () => clearTimeout(timer);
    }
  }, [hasToken, filterParams]);

  const sortEventHandler = (update: ISortParams) => {
    handleSortChange(update);
  };

  const columnDefs = useMemo(() => GetQPAY600GridColumns(), []);

  const data = employeeStatus === "Full time" ? mockData.fullTime : mockData.partTime;

  const getTitle = () => {
    const baseTitle = `Employees - ${employeeStatus}`;
    const employeeTypeFilter = filterParams.employeeType;

    if (employeeTypeFilter && employeeTypeFilter !== "") {
      return `${baseTitle} (${employeeTypeFilter})`;
    }

    return baseTitle;
  };

  const pinnedBottomRowData = useMemo(() => {
    if (!data) return [];

    return [
      {
        yearsOfService: null,
        employees: data.totalEmployees,
        totalWeeklyPay: data.totalWeeklyPay,
        lastYearWages: data.totalLastYearWages,
        _isTotal: true
      }
    ];
  }, [data]);

  return (
    <div className="relative">
      <div style={{ padding: "0 24px 0 24px" }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          {getTitle()}
        </Typography>
      </div>

      {isFetching ? (
        <Box
          display="flex"
          justifyContent="center"
          alignItems="center"
          py={4}>
          <CircularProgress />
        </Box>
      ) : (
        <>
          <DSMGrid
            preferenceKey={`QPAY600_${employeeStatus.toUpperCase().replace(" ", "_")}`}
            isLoading={isFetching}
            maxHeight={gridMaxHeight}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: data.results || [],
              columnDefs: columnDefs,
              pinnedBottomRowData: pinnedBottomRowData
            }}
          />
          {!!data && data.results.length > 0 && (
            <Pagination
              pageNumber={pageNumber}
              setPageNumber={(value: number) => handlePaginationChange(value - 1, pageSize)}
              pageSize={pageSize}
              setPageSize={(value: number) => handlePaginationChange(0, value)}
              recordCount={data.total}
            />
          )}
        </>
      )}
    </div>
  );
};

export default QPAY600Grid;
