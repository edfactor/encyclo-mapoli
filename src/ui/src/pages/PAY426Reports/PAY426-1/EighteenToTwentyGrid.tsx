import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo } from "react";
import { DSMGrid } from "smart-ui-library";
import { GetProfitSharingReportGridColumns } from "./EighteenToTwentyGridColumns";
import { useNavigate, Path } from "react-router";
import { useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";
interface EmployeeData {
  badge: number;
  employeeName: string;
  store: number;
  type: string;
  dateOfBirth: string;
  age: number;
  ssn: string;
  wages: number;
  hours: number;
  points: number;
  new: string;
  termDate: string | null;
  currentBalance: number;
  svc: number;
}

const EighteenToTwentyGrid = () => {
  const navigate = useNavigate();

  const [trigger, { data, isLoading, error }] = useLazyGetYearEndProfitSharingReportQuery();

  useEffect(() => {
    trigger({
      isYearEnd: true,
      minimumAgeInclusive: 17,
      maximumAgeInclusive: 20,
      minimumHoursInclusive: 999.9,
      maximumHoursInclusive: 4000,
      includeActiveEmployees: true,
      includeInactiveEmployees: true,
      includeEmployeesTerminatedThisYear: false,
      includeTerminatedEmployees: false,
      includeBeneficiaries: false,
      includeEmployeesWithPriorProfitSharingAmounts: true,
      includeEmployeesWithNoPriorProfitSharingAmounts: true,
      profitYear: 2024,
      pagination: {
        skip: 0,
        take: 25
      }
    });
  }, [trigger]);

  const getPinnedBottomRowData = (data: any[]) => {
    return [
      {
        employeeName: "Total EMPS",
        store: 1,
        wages: 100.0,
        currentBalance: 0
      },
      {
        employeeName: "No Wages",
        store: 0,
        wages: 0,
        currentBalance: 0
      }
    ];
  };
  // Wrapper to pass react function to non-react class
  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  const columnDefs = useMemo(
    () => GetProfitSharingReportGridColumns(handleNavigationForButton),
    [handleNavigationForButton]
  );

  return (
    <>
      <div style={{ padding: "0 24px 0 24px" }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          {`PROFIT-ELIGIBLE REPORT (${data?.response?.results?.length || 0})`}
        </Typography>
      </div>
      <DSMGrid
        preferenceKey={"ELIGIBLE_EMPLOYEES"}
        isLoading={isLoading}
        handleSortChanged={(_params) => {}}
        providedOptions={{
          rowData: data?.response?.results || [],
          columnDefs: columnDefs
        }}
      />
    </>
  );
};

export default EighteenToTwentyGrid;
