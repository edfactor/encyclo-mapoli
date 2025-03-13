import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo } from "react";
import { Path, useNavigate } from "react-router";
import { useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";
import { DSMGrid } from "smart-ui-library";
import { GetProfitSharingReportGridColumns } from "../PAY426-1/EighteenToTwentyGridColumns";

const UnderEighteenGrid = () => {
  const navigate = useNavigate();
  const [trigger, { data, isLoading }] = useLazyGetYearEndProfitSharingReportQuery();

  useEffect(() => {
    trigger({
      isYearEnd: true,
      minimumAgeInclusive: 0,
      maximumAgeInclusive: 17,
      minimumHoursInclusive: 0,
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
          {`UNDER 18 REPORT (${data?.response?.results?.length || 0})`}
        </Typography>
      </div>
      <DSMGrid
        preferenceKey={"UNDER_EIGHTEEN_EMPLOYEES"}
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

export default UnderEighteenGrid;
