import { Typography } from "@mui/material";
import { useMemo, useEffect, useCallback } from "react";
import { DSMGrid } from "smart-ui-library";
import { Path, useNavigate } from "react-router";
import { GetProfitSharingReportGridColumns } from "../PAY426-1/EighteenToTwentyGridColumns";
import { useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";
import { useSelector } from "react-redux";
import { RootState } from "../../../reduxstore/store";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";

const TwentyOnePlusGrid = () => {
  const navigate = useNavigate();

  const [trigger, { data, isLoading }] = useLazyGetYearEndProfitSharingReportQuery();

  const hasToken = useSelector((state: RootState) => !!state.security.token);
  const profitYear = useDecemberFlowProfitYear();

  useEffect(() => {
    if (hasToken) {
      trigger({
        isYearEnd: true,
        minimumAgeInclusive: 21,
        maximumAgeInclusive: 200,
        minimumHoursInclusive: 999.9,
        maximumHoursInclusive: 4000,
        includeActiveEmployees: true,
        includeInactiveEmployees: true,
        includeEmployeesTerminatedThisYear: false,
        includeTerminatedEmployees: false,
        includeBeneficiaries: false,
        includeEmployeesWithPriorProfitSharingAmounts: true,
        includeEmployeesWithNoPriorProfitSharingAmounts: true,
        profitYear: profitYear,
        pagination: {
          skip: 0,
          take: 25,
          sortBy: "badgeNumber",
          isSortDescending: true
        }
      });
    }
  }, [trigger, hasToken, profitYear]);

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
          {`ACTIVE/INACTIVE 21+ REPORT (${data?.response?.results?.length || 0} records)`}
        </Typography>
      </div>
      <DSMGrid
        preferenceKey={"TWENTYONE_PLUS_EMPLOYEES"}
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

export default TwentyOnePlusGrid;
