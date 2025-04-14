import { Typography } from "@mui/material";
import { useMemo, useEffect, useCallback } from "react";
import { DSMGrid } from "smart-ui-library";
import { Path, useNavigate } from "react-router";
import { GetProfitSharingReportGridColumns } from "../PAY426-1/EighteenToTwentyGridColumns";
import { useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";
import { useSelector } from "react-redux";
import { RootState } from "../../../reduxstore/store";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";

const TermedNoPriorGrid = () => {
  const navigate = useNavigate();
  const [trigger, { data, isLoading }] = useLazyGetYearEndProfitSharingReportQuery();

  const hasToken = useSelector((state: RootState) => !!state.security.token);
  const profitYear = useDecemberFlowProfitYear();

  useEffect(() => {
    if (hasToken) {
      trigger({
      isYearEnd: true,
      minimumAgeInclusive: 18,
      maximumAgeInclusive: 200,
      minimumHoursInclusive: 0,
      maximumHoursInclusive: 999.99,
      includeActiveEmployees: false,
      includeInactiveEmployees: false,
      includeEmployeesTerminatedThisYear: true,
      includeTerminatedEmployees: true,
      includeBeneficiaries: false,
      includeEmployeesWithPriorProfitSharingAmounts: false,
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
          {`TERMED NO PRIOR PS REPORT (${data?.response?.results?.length || 0} records)`}
        </Typography>
      </div>
      <DSMGrid
        preferenceKey={"TERMED_NO_PRIOR_EMPLOYEES"}
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

export default TermedNoPriorGrid;
