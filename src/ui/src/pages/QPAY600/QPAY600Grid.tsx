import React, { useCallback, useEffect, useMemo, useState } from "react";
import { Typography, Box, CircularProgress } from "@mui/material";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { useNavigate, Path } from "react-router-dom";
import { useSelector } from "react-redux";
import { RootState } from "../../reduxstore/store";
import { QPAY600FilterParams } from "./QPAY600FilterSection";
import { GetQPAY600GridColumns } from "./QPAY600GridColumns";

interface QPAY600GridProps {
  filterParams: QPAY600FilterParams;
  employeeStatus: "Full time" | "Part time";
  onLoadingChange?: (isLoading: boolean) => void;
}

const QPAY600Grid: React.FC<QPAY600GridProps> = ({ filterParams, employeeStatus, onLoadingChange }) => {
  const navigate = useNavigate();

  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);

  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "employeeName",
    isSortDescending: false
  });

  const [isFetching, setIsFetching] = useState(false);
  const hasToken = useSelector((state: RootState) => !!state.security.token);

  const mockData = useMemo(() => {
    return {
      fullTime: {
        results: [
          {
            yearsOfService: 5,
            employees: 1250,
            totalWeeklyPay: 87500.00,
            lastYearWages: 2450000.00
          },
          {
            yearsOfService: 3,
            employees: 980,
            totalWeeklyPay: 68600.00,
            lastYearWages: 1940000.00
          },
          {
            yearsOfService: 1,
            employees: 750,
            totalWeeklyPay: 52500.00,
            lastYearWages: 1500000.00
          }
        ],
        total: 3,
        totalEmployees: 2980,
        totalWeeklyPay: 208600.00,
        totalLastYearWages: 5890000.00
      },
      partTime: {
        results: [
          {
            yearsOfService: 2,
            employees: 650,
            totalWeeklyPay: 19500.00,
            lastYearWages: 975000.00
          },
          {
            yearsOfService: 4,
            employees: 420,
            totalWeeklyPay: 12600.00,
            lastYearWages: 630000.00
          },
          {
            yearsOfService: 1,
            employees: 380,
            totalWeeklyPay: 11400.00,
            lastYearWages: 570000.00
          }
        ],
        total: 3,
        totalEmployees: 1450,
        totalWeeklyPay: 43500.00,
        totalLastYearWages: 2175000.00
      }
    };
  }, [filterParams]);

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

  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  const sortEventHandler = (update: ISortParams) => {
    setSortParams(update);
  };

  const columnDefs = useMemo(
    () => GetQPAY600GridColumns(handleNavigationForButton),
    [handleNavigationForButton]
  );

  const data = employeeStatus === "Full time" ? mockData.fullTime : mockData.partTime;

  const getTitle = () => {
    const baseTitle = `Employees - ${employeeStatus}`;
    const employeeTypeFilter = filterParams.employeeType;
    
    if (employeeTypeFilter && employeeTypeFilter !== '') {
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
    <>
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
            preferenceKey={`QPAY600_${employeeStatus.toUpperCase().replace(' ', '_')}`}
            isLoading={isFetching}
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
              setPageNumber={(value: number) => setPageNumber(value - 1)}
              pageSize={pageSize}
              setPageSize={(value: number) => setPageSize(value)}
              recordCount={data.total}
            />
          )}
        </>
      )}
    </>
  );
};

export default QPAY600Grid;